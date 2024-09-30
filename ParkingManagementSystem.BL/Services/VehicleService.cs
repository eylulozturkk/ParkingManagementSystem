using AutoMapper;
using Newtonsoft.Json;
using ParkingManagementSystem.BL.Dto.Request;
using ParkingManagementSystem.BL.Dto.Response;
using ParkingManagementSystem.BL.Interface;
using ParkingManagementSystem.DAL.Entity;
using ParkingManagementSystem.DAL.Enums;
using ParkingManagementSystem.DAL.UOW;

namespace ParkingManagementSystem.BL.Services
{
    public class VehicleService : IVehicleService
    {
        #region fields

        public const string VEHICLE_ALL_KEY = "VehicleTableAll";
        public const string PARKINNG_SPOT_VEHICLE_ALL_KEY = "ParkingSpotVehicleTableAll";
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IParkingSpotService _parkingSpotService;
        private readonly IRedisCacheService _redisCacheService;
        private readonly ILogger _logger;

        #endregion

        #region ctor

        public VehicleService(
            IMapper mapper,
            IUnitOfWork unitOfWork,
            IParkingSpotService parkingSpotService,
            IRedisCacheService redisCacheService,
            ILogger logger)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _parkingSpotService = parkingSpotService;
            _redisCacheService = redisCacheService;
            _logger = logger;
        }

        #endregion

        #region methods

        public async Task<VehicleResponse> GetVehicleByIdAsync(long id)
        {
            ICollection<Vehicle>? repo = null;
            Vehicle response = null;
            var cachedData = await _redisCacheService.GetValueAsync(VEHICLE_ALL_KEY);
            if (!string.IsNullOrEmpty(cachedData))
            {
                //Cache bulundu
                repo = JsonConvert.DeserializeObject<ICollection<Vehicle>>(cachedData);
                response = repo?.FirstOrDefault(p => p.Id == id && p.IsActive && !p.IsDeleted);
            }
            else
            {
                //Cache bulunamadı
                var repository = _unitOfWork.GetRepository<Vehicle>();
                if (repository == null)
                    throw new InvalidOperationException("Vehicle repository could not be found.");

                var repoAll = await repository.GetAllAsync();
                response = repoAll?.FirstOrDefault(p => p.Id == id && p.IsActive && !p.IsDeleted);

                if (repoAll != null)
                {
                    await _redisCacheService.SetValueAsync(VEHICLE_ALL_KEY, JsonConvert.SerializeObject(repoAll));
                }
            }

            if (response == null)
                throw new InvalidOperationException("Vehicle not found.");

            _logger.InsertLogAsync(LogLevelType.Information, "VehicleService | GetVehicleByIdAsync", $"Vehicle: {JsonConvert.SerializeObject(response)} RequestVehicleId: {id}");
            return _mapper.Map<VehicleResponse>(response);
        }

        public async Task<VehicleResponse> GetVehicleByLicensePlateAsync(string licensePlate)
        {
            if (string.IsNullOrEmpty(licensePlate))
                throw new ArgumentNullException(nameof(licensePlate));

            ICollection<Vehicle>? repo = null;
            Vehicle response = null;
            var cachedData = await _redisCacheService.GetValueAsync(VEHICLE_ALL_KEY);
            if (!string.IsNullOrEmpty(cachedData))
            {
                //Cache bulundu
                repo = JsonConvert.DeserializeObject<ICollection<Vehicle>>(cachedData);
                response = repo?.FirstOrDefault(p => p.LicensePlate == licensePlate && p.IsActive && !p.IsDeleted);
            }
            else
            {
                //Cache bulunamadı
                var repository = _unitOfWork.GetRepository<Vehicle>();
                if (repository == null)
                    throw new InvalidOperationException("Vehicle repository could not be found.");

                var repoAll = await repository.GetAllAsync();
                response = repoAll?.FirstOrDefault(p => p.LicensePlate == licensePlate && p.IsActive && !p.IsDeleted);

                if (repoAll != null)
                {
                    await _redisCacheService.SetValueAsync(VEHICLE_ALL_KEY, JsonConvert.SerializeObject(repoAll));
                }
            }

            if (response == null)
                throw new InvalidOperationException("Vehicle not found.");

            _logger.InsertLogAsync(LogLevelType.Information, "VehicleService | GetVehicleByLicensePlateAsync", $"Vehicle: {JsonConvert.SerializeObject(response)} RequestLicensePlate: {licensePlate}");
            return _mapper.Map<VehicleResponse>(response);
        }

        public async Task<ParkingSpotVehicleMappingResponse> GetParkingSpotVehicleMappingByVehicleIdAsync(long vehicleId)
        {
            if (vehicleId <= 0)
                throw new ArgumentOutOfRangeException(nameof(vehicleId));

            ICollection<VehicleParkingSpotMapping>? repo = null;
            VehicleParkingSpotMapping response = null;
            var cachedData = await _redisCacheService.GetValueAsync(PARKINNG_SPOT_VEHICLE_ALL_KEY);
            if (!string.IsNullOrEmpty(cachedData))
            {
                //Cache bulundu
                repo = JsonConvert.DeserializeObject<ICollection<VehicleParkingSpotMapping>>(cachedData);
                response = repo?.FirstOrDefault(p => p.VehicleId == vehicleId && p.IsActive && !p.IsDeleted);
            }
            else
            {
                //Cache bulunamadı
                var repository = _unitOfWork.GetRepository<VehicleParkingSpotMapping>();
                if (repository == null)
                    throw new InvalidOperationException("VehicleParkingSpotMapping repository could not be found.");

                var repoAll = await repository.GetAllAsync();
                response = repoAll?.FirstOrDefault(p => p.VehicleId == vehicleId && p.IsActive && !p.IsDeleted);

                if (repoAll != null)
                {
                    await _redisCacheService.SetValueAsync(PARKINNG_SPOT_VEHICLE_ALL_KEY, JsonConvert.SerializeObject(repoAll));
                }
            }

            if (response == null)
                throw new InvalidOperationException("Vehicle parking spot mapping not found.");

            _logger.InsertLogAsync(LogLevelType.Information, "VehicleService | GetParkingSpotVehicleMappingByVehicleIdAsync", $"VehicleMapping: {JsonConvert.SerializeObject(response)} RequestVehicleId: {vehicleId}");
            return _mapper.Map<ParkingSpotVehicleMappingResponse>(response);
        }
        public async Task<List<ParkingSpotVehicleMappingResponse>> GetParkingSpotVehicleMappingAllByParkingSpotIdAsync(long parkingSpotId)
        {
            var repository = _unitOfWork.GetRepository<VehicleParkingSpotMapping>();
            var repoAll = await repository.FindAllAsync((p => p.ParkingSpotId == parkingSpotId && p.IsActive && !p.IsDeleted));

            return _mapper.Map<List<ParkingSpotVehicleMappingResponse>>(repoAll);
        }
        public async Task<PriceResponse> GetTotalParkingSpotPriceVehicleIdAsync(long vehicleId)
        {
            var priceResponse = new PriceResponse();

            var vehicle = await GetVehicleByIdAsync(vehicleId);
            if (vehicle == null)
            {
                return new PriceResponse
                {
                    IsSuccess = false,
                    Message = "Araç bulunamadı."
                };
            }

            vehicle.ExitTime = DateTime.Now;

            var vehicleParkingTime = Math.Round((vehicle.ExitTime - vehicle.EntryTime)?.TotalHours ?? 0);

            var vehicleMapping = await GetParkingSpotVehicleMappingByVehicleIdAsync(vehicleId);

            if (vehicleMapping != null)
            {
                var parkingPrice = await _parkingSpotService.GetParkingSpotPriceByParkingSpotIdAsync(vehicleMapping.ParkingSpotId, vehicleParkingTime);
                priceResponse.ParkingPrice = parkingPrice.ParkingPrice;
                priceResponse.ParkingTime = vehicleParkingTime;
                priceResponse.EntryTime = vehicle.EntryTime;
                priceResponse.ExitTime = vehicle.ExitTime;
                priceResponse.IsSuccess = true;
                priceResponse.Message = "Park ücreti hesaplandı.";
                await UpdateVehicleAsync(new VehicleRequest
                {
                    Id = vehicle.Id,
                    LicensePlate = vehicle.LicensePlate,
                    Size = vehicle.Size,
                    EntryTime = vehicle.EntryTime,
                    ExitTime = vehicle.ExitTime,
                    TotalParkingFee = priceResponse.ParkingPrice,
                    IsActive = false
                });

                await UpdateParkingSpotVehicleMappingAsync(new ParkingSpotVehicleMappingRequest
                {
                    Id = vehicleMapping.Id,
                    VehicleId = vehicleMapping.VehicleId,
                    ParkingSpotId = vehicleMapping.ParkingSpotId,
                    IsActive = false
                });

                await _redisCacheService.RemoveValueAsync(PARKINNG_SPOT_VEHICLE_ALL_KEY);
                await _redisCacheService.RemoveValueAsync(VEHICLE_ALL_KEY);
            }

            return (priceResponse);
        }

        public async Task<PriceResponse> GetTotalParkingSpotPriceByLicensePlateAsync(string licensePlate)
        {
            var priceResponse = new PriceResponse();

            var vehicle = await GetVehicleByLicensePlateAsync(licensePlate);
            if (vehicle == null)
            {
                return new PriceResponse
                {
                    IsSuccess = false,
                    Message = "Araç bulunamadı."
                };
            }

            vehicle.ExitTime = DateTime.Now;

            var vehicleParkingTime = Math.Round((vehicle.ExitTime - vehicle.EntryTime)?.TotalHours ?? 0);

            var vehicleMapping = await GetParkingSpotVehicleMappingByVehicleIdAsync(vehicle.Id);

            if (vehicleMapping != null)
            {
                var parkingPrice = await _parkingSpotService.GetParkingSpotPriceByParkingSpotIdAsync(vehicleMapping.ParkingSpotId, vehicleParkingTime);
                priceResponse.ParkingPrice = parkingPrice.ParkingPrice;
                priceResponse.ParkingTime = vehicleParkingTime;
                priceResponse.EntryTime = vehicle.EntryTime;
                priceResponse.ExitTime = vehicle.ExitTime;
                priceResponse.IsSuccess = true;
                priceResponse.Message = "Park ücreti hesaplandı.";

                await UpdateVehicleAsync(new VehicleRequest
                {
                    Id = vehicle.Id,
                    LicensePlate = vehicle.LicensePlate,
                    Size = vehicle.Size,
                    EntryTime = vehicle.EntryTime,
                    ExitTime = vehicle.ExitTime,
                    TotalParkingFee = priceResponse.ParkingPrice,
                    IsActive = false
                });

                await UpdateParkingSpotVehicleMappingAsync(new ParkingSpotVehicleMappingRequest
                {
                    Id = vehicleMapping.Id,
                    VehicleId = vehicleMapping.VehicleId,
                    ParkingSpotId = vehicleMapping.ParkingSpotId,
                    IsActive = false
                });

                await _redisCacheService.RemoveValueAsync(PARKINNG_SPOT_VEHICLE_ALL_KEY);
                await _redisCacheService.RemoveValueAsync(VEHICLE_ALL_KEY);
            }

            return (priceResponse);
        }


        private async Task<bool> IsFullCapacity(long parkingSpotId, int maxCapacity)
        {
            var parkingSpotVehicleMapping = await GetParkingSpotVehicleMappingAllByParkingSpotIdAsync(parkingSpotId);
            var parkingSpotVehicleMappingTotal = parkingSpotVehicleMapping.Count();

            if (parkingSpotVehicleMappingTotal >= maxCapacity)
                return false;

            return true;
        }

        public async Task<VehicleSuccessResponse> CreateVehicleAsync(VehicleRequest vehicleRequest)
        {
            var parkingSpot = await _parkingSpotService.GetParkingSpotBySizeAsync(vehicleRequest.Size);

            if (! await IsFullCapacity(parkingSpot.Id, parkingSpot.MaxCapacity))
                return new VehicleSuccessResponse
                {
                    IsSuccess = false,
                    Message = "Araç için uygun park yeri bulunamadı. Otopark kapasitesi dolu",
                    VehicleResponse = new VehicleResponse()
                };

            var repository = _unitOfWork.GetRepository<Vehicle>();

            var entity = _mapper.Map<Vehicle>(vehicleRequest);

            entity.EntryTime = DateTime.Now;
            entity.CreatedAt = DateTime.Now;
            entity.CreatedById = vehicleRequest.UserId;
            entity.IsActive = true;
            entity.TotalParkingFee = decimal.Zero;
            entity.LicensePlate = vehicleRequest.LicensePlate;
            entity.Size = vehicleRequest.Size;

            var response = await repository.AddAsync(entity);

            await _unitOfWork.SaveChangesAsync();

            var vehicleResponse = _mapper.Map<VehicleResponse>(response);

            await CreateParkingSpotVehicleMappingAsync(new ParkingSpotVehicleMappingRequest
            {
                VehicleId = vehicleResponse.Id,
                ParkingSpotId = parkingSpot.Id,
                IsActive = true
            });

            await _redisCacheService.RemoveValueAsync(VEHICLE_ALL_KEY);

            return new VehicleSuccessResponse
            {
                VehicleResponse = vehicleResponse,
                IsSuccess = true,
                Message = "Araç eklendi"
            };

        }

        public async Task<ParkingSpotVehicleMappingResponse> CreateParkingSpotVehicleMappingAsync(ParkingSpotVehicleMappingRequest parkingSpotVehicleMapping)
        {
            var repository = _unitOfWork.GetRepository<VehicleParkingSpotMapping>();

            var entity = _mapper.Map<VehicleParkingSpotMapping>(parkingSpotVehicleMapping);

            entity.VehicleId = parkingSpotVehicleMapping.VehicleId;
            entity.ParkingSpotId = parkingSpotVehicleMapping.ParkingSpotId;
            entity.CreatedAt = DateTime.Now;
            entity.IsActive = true;

            var response = await repository.AddAsync(entity);

            await _unitOfWork.SaveChangesAsync();

            await _redisCacheService.RemoveValueAsync(PARKINNG_SPOT_VEHICLE_ALL_KEY);

            return _mapper.Map<ParkingSpotVehicleMappingResponse>(response);

        }

        public async Task<VehicleResponse> UpdateVehicleAsync(VehicleRequest vehicleRequest)
        {
            if (vehicleRequest == null)
                throw new ArgumentNullException(nameof(vehicleRequest));

            var repository = _unitOfWork.GetRepository<Vehicle>();
            if (repository == null)
                throw new InvalidOperationException("Vehicle repository could not be found.");

            var entity = _mapper.Map<Vehicle>(vehicleRequest);

            entity.IsActive = vehicleRequest.IsActive;
            entity.UpdatedById = 1;
            entity.UpdatedAt = DateTime.Now;

            var response = await repository.UpdateAsync(entity);

            if (response == null)
                throw new InvalidOperationException("Vehicle update failed.");

            await _unitOfWork.SaveChangesAsync();

            await _redisCacheService.RemoveValueAsync(VEHICLE_ALL_KEY);

            return _mapper.Map<VehicleResponse>(response);
        }

        public async Task<ParkingSpotVehicleMappingResponse> UpdateParkingSpotVehicleMappingAsync(ParkingSpotVehicleMappingRequest parkingSpotVehicleMappingRequest)
        {
            if (parkingSpotVehicleMappingRequest == null)
                throw new ArgumentNullException(nameof(parkingSpotVehicleMappingRequest));

            var repository = _unitOfWork.GetRepository<VehicleParkingSpotMapping>();
            if (repository == null)
                throw new InvalidOperationException("VehicleParkingSpotMapping repository could not be found.");

            var entity = _mapper.Map<VehicleParkingSpotMapping>(parkingSpotVehicleMappingRequest);

            entity.IsActive = parkingSpotVehicleMappingRequest.IsActive;
            entity.UpdatedById = 1;
            entity.UpdatedAt = DateTime.Now;

            var response = await repository.UpdateAsync(entity);

            if (response == null)
                throw new InvalidOperationException("Vehicle parking spot mapping update failed.");

            await _unitOfWork.SaveChangesAsync();

            await _redisCacheService.RemoveValueAsync(PARKINNG_SPOT_VEHICLE_ALL_KEY);

            return _mapper.Map<ParkingSpotVehicleMappingResponse>(response);
        }

        public async Task<bool> DeleteVehicleByIdAsync(long id)
        {
            var repository = _unitOfWork.GetRepository<Vehicle>();

            var vehicle = await repository.GetAsync(id);

            if (vehicle == null)
                throw new ArgumentException(nameof(vehicle));

            vehicle.Id = id;
            vehicle.UpdatedAt = DateTime.Now;
            vehicle.DeletedAt = DateTime.Now;
            vehicle.IsDeleted = true;
            vehicle.IsActive = false;

            await repository.UpdateAsync(vehicle);

            var result = await _unitOfWork.SaveChangesAsync(); 
            
            await _redisCacheService.RemoveValueAsync(VEHICLE_ALL_KEY);

            return result > 0;
        }


        public async Task<bool> DeleteParkingSpotVehicleMappingByIdAsync(long id)
        {
            var repository = _unitOfWork.GetRepository<VehicleParkingSpotMapping>();

            var vehicleParkingSpotMapping = await repository.GetAsync(id);

            if (vehicleParkingSpotMapping == null)
                throw new ArgumentException(nameof(vehicleParkingSpotMapping));

            vehicleParkingSpotMapping.Id = id;
            vehicleParkingSpotMapping.UpdatedAt = DateTime.Now;
            vehicleParkingSpotMapping.DeletedAt = DateTime.Now;
            vehicleParkingSpotMapping.IsDeleted = true;
            vehicleParkingSpotMapping.IsActive = false;

            await repository.UpdateAsync(vehicleParkingSpotMapping);

            var result = await _unitOfWork.SaveChangesAsync();

            await _redisCacheService.RemoveValueAsync(PARKINNG_SPOT_VEHICLE_ALL_KEY);

            return result > 0;
        }

        #endregion
    }
}