using Microsoft.Extensions.Options;
using ParkingManagementSystem.DAL.GenericRepository;
using System.ComponentModel;

namespace ParkingManagementSystem.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public IContainer ApplicationContainer { get; private set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddResponseCompression();

            services.AddDbContext<DataContext>(options =>
            {
                options.UseMySQL(Configuration.GetConnectionString("DefaultConnection"));

            }, ServiceLifetime.Transient);

            services.RegisterCacheServices(Configuration);
            services.RegisterLocalizationServices(Configuration);

            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            services.AddSingleton<ICacheHelper, CacheHelper>();
            services.AddTransient<IUnitOfWork, UnitOfWork>();

            services.AddSingleton<IApiResponseFactory, ApiResponseFactory>();
            services.AddCoreLogging(Configuration);
            services.AddSingleton<IAuditService, AuditService>();

            services.Configure<ServiceHostsConfiguration>(Configuration.GetSection("ServiceHostsConfiguration"));

            // Explicitly register the settings object by delegating to the IOptions object
            services.AddSingleton(resolver =>
                resolver.GetRequiredService<IOptions<ServiceHostsConfiguration>>().Value);


            #region Liveness

            services.AddHealthChecks();

            #endregion

            services.AddCors(o => o.AddPolicy("Phantom-policy", b =>
            {
                b.AllowAnyOrigin()
                 .AllowAnyMethod()
                 .AllowAnyHeader();
            }));
            services.AddControllers(options => { options.RespectBrowserAcceptHeader = true; });

            services.AddApiVersioning(options =>
            {
                options.ReportApiVersions = true;
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.DefaultApiVersion = new ApiVersion(1, 0);
            });

            services.AddVersionedApiExplorer(options =>
            {

                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            });

            #region Settings

            services.AddSingleton<ISettingService, SettingService>();
            services.AddSingleton(resolver => resolver.GetService<ISettingService>().LoadSettingAsync<NotificationSettings>().GetAwaiter().GetResult());
            services.AddSingleton(resolver => resolver.GetService<ISettingService>().LoadSettingAsync<ApplicationSettings>().GetAwaiter().GetResult());

            #endregion

            #region Swagger
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("Back-End", new OpenApiInfo
                {
                    Version = "1.0",
                    Title = "Back End Endpoints",
                    Description = "ParkManagementSystem-MS",
                    TermsOfService = new Uri("https://www.inveon.com"),
                    Contact = new OpenApiContact
                    {
                        Name = "EylulParkSystem"
                    },
                });
                c.SwaggerDoc("Front-End", new OpenApiInfo
                {
                    //Version = "v1-fe",
                    Title = "Front End Endpoints",
                    Description = "ParkManagementSystem-MS",
                    TermsOfService = new Uri("https://www.inveon.com"),
                    Contact = new OpenApiContact
                    {
                        Name = "EylulParkSystem"
                    },
                });
                c.SwaggerDoc("All", new OpenApiInfo
                {
                    //Version = "v1-all",
                    Title = "Park Management System Endpoints",
                    Description = "ParkManagementSysteme-MS",
                    TermsOfService = new Uri("https://www.inveon.com"),
                    Contact = new OpenApiContact
                    {
                        Name = "EylulParkSystem"
                    },
                });
                c.OperationFilter<LanguageCultureHeaderParameterOperationFilter>();

                c.DocInclusionPredicate((docName, apiDesc) =>
                {
                    if (docName.Equals("All")) return true;

                    if (!(apiDesc.ActionDescriptor is ControllerActionDescriptor controllerActionDescriptor))
                        return true;

                    var hasAuthorizeAttribute = controllerActionDescriptor.MethodInfo
                   .CustomAttributes
                   .Any(x => x.AttributeType.Name == nameof(AuthorizeAttribute));

                    var hasFrontEndEndpointAttribute = controllerActionDescriptor.MethodInfo
                    .CustomAttributes
                    .Any(x => x.AttributeType.Name == nameof(FrontEndEndpointAttribute));

                    var hasVersioned = controllerActionDescriptor.MethodInfo
                     .CustomAttributes
                     .Any(x => x.AttributeType.Name == nameof(VersioningEndpointAttribute));

                    if (docName.Equals("Front-End") && (hasAuthorizeAttribute || hasFrontEndEndpointAttribute))
                    {
                        return true;
                    }

                    if (docName.Equals("Back-End") && !hasAuthorizeAttribute && !hasVersioned)
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
            #endregion


            //AutoFac
            var builder = new ContainerBuilder();
            builder.Populate(services);
            builder.RegisterGeneric(typeof(GenericRepository<>)).As(typeof(IGenericRepository<>));


            //IBusinessUnit'den türeyen her şeyi inject etmek için kulllandık
            var ibu = typeof(IBusinessUnit);
            ibu.Assembly.GetExportedTypes()
                .Select(t => new { Type = t, Interface = t.GetInterfaces().FirstOrDefault(i => i != ibu && ibu.IsAssignableFrom(i) && !i.IsGenericType) })
                .Where(t => t.Interface != null && t.Type.IsClass).ToList()
                .ForEach(t =>
                {
                    builder.RegisterType(t.Type).As(t.Interface);
                });


            this.ApplicationContainer = builder.Build();

            return new AutofacServiceProvider(this.ApplicationContainer);

        }

      
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseResponseCompression();

            #region APM
            app.UseAllElasticApm(Configuration);
            #endregion

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            #region Prometheus
            app.UseMetricServer();
            #endregion

            #region Liveness

            app.UseHealthChecks("/health-check");

            #endregion liveness

            app.UseRouting();
            app.UseHttpMetrics();
            app.UseCors("Phantom-policy");
            app.UseCookiePolicy(new CookiePolicyOptions
            {
                HttpOnly = Microsoft.AspNetCore.CookiePolicy.HttpOnlyPolicy.Always,
                Secure = Microsoft.AspNetCore.Http.CookieSecurePolicy.Always
            });

            app.UseLocalization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapMetrics();
            });

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.DocumentTitle = "Park Management System";
                c.SwaggerEndpoint("/swagger/All/swagger.json", "Park Management System All API Endpoints");
                c.SwaggerEndpoint("/swagger/Back-End/swagger.json", "Park Management System Backend API Endpoints v1");
                c.SwaggerEndpoint("/swagger/Front-End/swagger.json", "Park Management System FrontEnd API Endpoints");
                c.RoutePrefix = "";
            });
        }
    }

}
