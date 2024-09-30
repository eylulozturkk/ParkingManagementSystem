using Autofac.Extensions.DependencyInjection;
using Autofac;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using ParkingManagementSystem.BL.Interface;
using ParkingManagementSystem.BL.Services;
using ParkingManagementSystem.DAL.Context;
using ParkingManagementSystem.DAL.GenericRepository;
using ParkingManagementSystem.DAL.UOW;
using Swashbuckle.AspNetCore.Swagger;
using System.Reflection;
using Autofac.Core;
using Microsoft.AspNetCore.Mvc.Controllers;
using ParkingManagementSystem.API.Swagger.CustomAttributes;
using ParkingManagementSystem.API.Swagger.Filters;
using ParkingManagementSystem.API.Swagger.OperationFilters;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddResponseCompression();
builder.Services.AddDbContext<DataContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
    sqlOptions => sqlOptions.EnableRetryOnFailure(
        maxRetryCount: 5,        
        maxRetryDelay: TimeSpan.FromSeconds(10), 
        errorNumbersToAdd: null) // Belirli hata numaralarý üzerinde deneme yapar (boþ býrakýrsak tüm hatalarda yeniden dener)
    ));

builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var configuration = builder.Configuration.GetSection("RedisSettings:ConnectionString").Value;
    return ConnectionMultiplexer.Connect(configuration);
});

builder.Services.AddAutoMapper(typeof(ParkingManagementSystem.BL.Mapper.AutoMapperProfile));
builder.Services.AddTransient<IUnitOfWork, UnitOfWork>();
builder.Services.AddSingleton<IAuditService, AuditService>();
builder.Services.AddSingleton<IRedisCacheService, RedisCacheService>();

// Liveness - Health Checks
builder.Services.AddHealthChecks();

// CORS
builder.Services.AddCors(o => o.AddPolicy("ParkingManagementSystem-policy", b =>
{
    b.AllowAnyOrigin()
     .AllowAnyMethod()
     .AllowAnyHeader();
}));

// API Versioning
builder.Services.AddApiVersioning(options =>
{
    options.ReportApiVersions = true;
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.DefaultApiVersion = new ApiVersion(1, 0);
});

builder.Services.AddVersionedApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});


// Swagger Configuration
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("Back-End", new OpenApiInfo
    {
        Version = "1.0",
        Title = "Back End Endpoints",
        Description = "ParkingManagementSystem-MS",
        Contact = new OpenApiContact
        {
            Name = "ParkingManagement"
        },
    });
    c.SwaggerDoc("Front-End", new OpenApiInfo
    {
        //Version = "v1-fe",
        Title = "Front End Endpoints",
        Description = "ParkingManagementSystem-MS",
        Contact = new OpenApiContact
        {
            Name = "ParkingManagement"
        },
    });
    c.SwaggerDoc("All", new OpenApiInfo
    {
        //Version = "v1-all",
        Title = "Park Management System Endpoints",
        Description = "ParkingManagementSystem-MS",
        Contact = new OpenApiContact
        {
            Name = "ParkingManagement"
        },
    });

    c.OperationFilter<LanguageCultureHeaderParameterOperationFilter>();

    c.DocInclusionPredicate((docName, apiDesc) =>
    {
        if (docName.Equals("All")) return true;

        if (!(apiDesc.ActionDescriptor is ControllerActionDescriptor controllerActionDescriptor))
            return true;

        var hasFrontEndEndpointAttribute = controllerActionDescriptor.MethodInfo
        .CustomAttributes
        .Any(x => x.AttributeType.Name == nameof(FrontEndEndpointAttribute));

        var hasVersioned = controllerActionDescriptor.MethodInfo
         .CustomAttributes
         .Any(x => x.AttributeType.Name == nameof(VersioningEndpointAttribute));

        if (docName.Equals("Front-End") &&  hasFrontEndEndpointAttribute)
        {
            return true;
        }

        if (docName.Equals("Back-End") && !hasVersioned)
        {
            return true;
        }

        if (docName.Equals("Versioning") && hasVersioned)
        {
            return true;
        }

        return false;
    });
    // Set the comments path for the Swagger JSON and UI.
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
    c.OperationFilter<SummaryOperationFilter>();
    c.AddSecurityDefinition("Bearer",
        new OpenApiSecurityScheme
        {
            In = ParameterLocation.Header,
            Description = "Please enter JWT with Bearer into field",
            Name = "Authorization",
            Type = SecuritySchemeType.ApiKey
        });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Scheme = "oauth2",
                            Name = "Bearer",
                            In = ParameterLocation.Header,

                        },
                        new List<string>()
                    }
    });
    c.AddFluentValidationRules();
});

// Autofac Dependency Injection
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());

builder.Host.ConfigureContainer<ContainerBuilder>(builder =>
{
    builder.RegisterGeneric(typeof(GenericRepository<>)).As(typeof(IGenericRepository<>));

    var ibu = typeof(IBusinessUnit);
    ibu.Assembly.GetExportedTypes()
        .Select(t => new { Type = t, Interface = t.GetInterfaces().FirstOrDefault(i => i != ibu && ibu.IsAssignableFrom(i) && !i.IsGenericType) })
        .Where(t => t.Interface != null && t.Type.IsClass)
        .ToList()
        .ForEach(t => builder.RegisterType(t.Type).As(t.Interface));

});

var app = builder.Build();

app.UseResponseCompression();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseHsts();
}

// Liveness - Health Checks
app.UseHealthChecks("/health-check");

app.UseHttpsRedirection();
app.UseRouting();

// CORS
app.UseCors("ParkingManagementSystem-policy");

app.UseCookiePolicy(new CookiePolicyOptions
{
    HttpOnly = Microsoft.AspNetCore.CookiePolicy.HttpOnlyPolicy.Always,
    Secure = Microsoft.AspNetCore.Http.CookieSecurePolicy.Always
});


// Swagger UI
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.DocumentTitle = "Park Management System";
    c.SwaggerEndpoint("/swagger/All/swagger.json", "Park Management System All API Endpoints");
    c.SwaggerEndpoint("/swagger/Back-End/swagger.json", "Park Management System Backend API Endpoints v1");
    c.SwaggerEndpoint("/swagger/Front-End/swagger.json", "Park Management System FrontEnd API Endpoints");
    c.RoutePrefix = string.Empty;
});

app.MapControllers();

app.Run();