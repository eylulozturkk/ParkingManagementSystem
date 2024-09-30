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
    public class ParkingSpotService : IParkingSpotService
    {
        #region fields

        public const string PARKINNG_SPOT_ALL_KEY = "ParkingSpotTableAll";
        public const string PARKINNG_SPOT_PRICE_ALL_KEY = "ParkingSpotPriceTableAll";

        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRedisCacheService _redisCacheService;
        private readonly ILogger _logger;

        #endregion

        #region ctor

        public ParkingSpotService(
            IMapper mapper,
            IUnitOfWork unitOfWork,
            IRedisCacheService redisCacheService,
            ILogger logger)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _redisCacheService = redisCacheService;
            _logger = logger;
        }

        #endregion

        #region Methods

        public async Task<List<ParkingSpotResponse>> GetParkingSpotAllAsync()
        {
            ICollection<ParkingSpot>? repo = null;
            var cachedData = await _redisCacheService.GetValueAsync(PARKINNG_SPOT_ALL_KEY);

            if (!string.IsNullOrEmpty(cachedData))
            {
                // Cache bulundu
                repo = JsonConvert.DeserializeObject<ICollection<ParkingSpot>>(cachedData);
            }
            else
            {
                // Cache bulunamadı
                var repository = _unitOfWork.GetRepository<ParkingSpot>();
                repo = await repository.GetAllAsync();

                if (repo != null && repo.Any())
                {
                    await _redisCacheService.SetValueAsync(PARKINNG_SPOT_ALL_KEY, JsonConvert.SerializeObject(repo));
                }
            }

            if (repo == null || !repo.Any())
            {
                throw new InvalidOperationException("Parking spots not found.");
            }

            _logger.InsertLogAsync(LogLevelType.Information, "ParkingSpotService | GetParkingSpotAllAsync", $"ParkingSpotRepo: {JsonConvert.SerializeObject(repo)}");
            return _mapper.Map<List<ParkingSpotResponse>>(repo);
        }

        public async Task<List<ParkingSpotPriceResponse>> GetParkingSpotPriceAllAsync()
        {
            ICollection<PriceParkingSpotMapping>? repo = null;
            var cachedData = await _redisCacheService.GetValueAsync(PARKINNG_SPOT_PRICE_ALL_KEY);
            if (!string.IsNullOrEmpty(cachedData))
            {
                // Cache bulundu
                repo = JsonConvert.DeserializeObject<ICollection<PriceParkingSpotMapping>>(cachedData);
            }
            else
            {
                // Cache bulunamadı
                var repository = _unitOfWork.GetRepository<PriceParkingSpotMapping>();
                repo = await repository.GetAllAsync();

                if (repo != null && repo.Any())
                {
                    await _redisCacheService.SetValueAsync(PARKINNG_SPOT_PRICE_ALL_KEY, JsonConvert.SerializeObject(repo));
                }
            }

            if (repo == null || !repo.Any())
            {
                throw new InvalidOperationException("Parking spot prices not found.");
            }

            _logger.InsertLogAsync(LogLevelType.Information, "ParkingSpotService | GetParkingSpotPriceAllAsync", $"ParkingSpotPriceRepo: {JsonConvert.SerializeObject(repo)}");
            return _mapper.Map<List<ParkingSpotPriceResponse>>(repo);
        }

        public async Task<ParkingSpotResponse> GetParkingSpotByIdAsync(long id)
        {
            ICollection<ParkingSpot>? repo = null;
            ParkingSpot? response = null;
            var cachedData = await _redisCacheService.GetValueAsync(PARKINNG_SPOT_ALL_KEY);
            if (!string.IsNullOrEmpty(cachedData))
            {
                // Cache bulundu
                repo = JsonConvert.DeserializeObject<ICollection<ParkingSpot>>(cachedData);
                response = repo?.FirstOrDefault(p => p.Id == id && p.IsActive && !p.IsDeleted);
            }
            else
            {
                // Cache bulunamadı
                var repository = _unitOfWork.GetRepository<ParkingSpot>();
                var repoAll = await repository.GetAllAsync();
                response = repoAll?.FirstOrDefault(p => p.Id == id && p.IsActive && !p.IsDeleted);

                if (repoAll != null && repoAll.Any())
                {
                    await _redisCacheService.SetValueAsync(PARKINNG_SPOT_ALL_KEY, JsonConvert.SerializeObject(repoAll));
                }
            }

            if (response == null)
            {
                throw new InvalidOperationException($"Parking spot with id {id} not found.");
            }

            _logger.InsertLogAsync(LogLevelType.Information, "ParkingSpotService | GetParkingSpotByIdAsync", $"ParkingSpot: {JsonConvert.SerializeObject(response)} RequestId: {id}");
            return _mapper.Map<ParkingSpotResponse>(response);
        }

        public async Task<ParkingSpotResponse> GetParkingSpotBySizeAsync(VehicleSizeType vehicleSizeType)
        {
            ICollection<ParkingSpot>? repo = null;
            ParkingSpot? response = null;
            var cachedData = await _redisCacheService.GetValueAsync(PARKINNG_SPOT_ALL_KEY);
            if (!string.IsNullOrEmpty(cachedData))
            {
                // Cache bulundu
                repo = JsonConvert.DeserializeObject<ICollection<ParkingSpot>>(cachedData);
                response = repo?.FirstOrDefault(p => p.Size == vehicleSizeType && p.IsActive && !p.IsDeleted);
            }
            else
            {
                // Cache bulunamadı
                var repository = _unitOfWork.GetRepository<ParkingSpot>();
                var repoAll = await repository.GetAllAsync();
                response = repoAll?.FirstOrDefault(p => p.Size == vehicleSizeType && p.IsActive && !p.IsDeleted);

                if (repoAll != null && repoAll.Any())
                {
                    await _redisCacheService.SetValueAsync(PARKINNG_SPOT_ALL_KEY, JsonConvert.SerializeObject(repoAll));
                }
            }

            if (response == null)
            {
                throw new InvalidOperationException($"Parking spot with size {vehicleSizeType} not found.");
            }

            _logger.InsertLogAsync(LogLevelType.Information, "ParkingSpotService | GetParkingSpotBySizeAsync", $"ParkingSpot: {JsonConvert.SerializeObject(response)} RequestVehicleSize: {vehicleSizeType}");
            return _mapper.Map<ParkingSpotResponse>(response);
        }

        public async Task<ParkingSpotPriceResponse> GetParkingSpotPriceByParkingSpotIdAsync(long parkingSpotId, double time)
        {
            ICollection<PriceParkingSpotMapping>? repo = null;
            PriceParkingSpotMapping? response = null;
            var cachedData = await _redisCacheService.GetValueAsync(PARKINNG_SPOT_PRICE_ALL_KEY);
            if (!string.IsNullOrEmpty(cachedData))
            {
                // Cache bulundu
                repo = JsonConvert.DeserializeObject<ICollection<PriceParkingSpotMapping>>(cachedData);
                response = repo?.FirstOrDefault(p => p.ParkingSpotId == parkingSpotId && p.IsActive && time >= p.ParkingMinTime && time <= p.ParkingMaxTime && !p.IsDeleted);
            }
            else
            {
                // Cache bulunamadı
                var repository = _unitOfWork.GetRepository<PriceParkingSpotMapping>();
                var repoAll = await repository.GetAllAsync();
                response = repoAll?.FirstOrDefault(p => p.ParkingSpotId == parkingSpotId && p.IsActive && time >= p.ParkingMinTime && time <= p.ParkingMaxTime && !p.IsDeleted);

                if (repoAll != null && repoAll.Any())
                {
                    await _redisCacheService.SetValueAsync(PARKINNG_SPOT_PRICE_ALL_KEY, JsonConvert.SerializeObject(repoAll));
                }
            }

            if (response == null)
            {
                throw new InvalidOperationException($"Price for parking spot with id {parkingSpotId} and time {time} not found.");
            }

            _logger.InsertLogAsync(LogLevelType.Information, "ParkingSpotService | GetParkingSpotPriceByParkingSpotIdAsync", $"ParkingSpotPrice: {JsonConvert.SerializeObject(response)} RequestParkingSpotId: {parkingSpotId} Time: {time}");
            return _mapper.Map<ParkingSpotPriceResponse>(response);
        }

        public async Task<ParkingSpotResponse> CreateParkingSpotAsync(ParkingSpotRequest parkingSpotRequest)
        {
            if (parkingSpotRequest == null)
                throw new ArgumentNullException(nameof(parkingSpotRequest));

            var repository = _unitOfWork.GetRepository<ParkingSpot>();

            var entity = _mapper.Map<ParkingSpot>(parkingSpotRequest);

            entity.CreatedAt = DateTime.Now;
            entity.CreatedById = parkingSpotRequest.UserId;
            entity.IsActive = true;
            entity.Name = parkingSpotRequest.Name;
            entity.Size = parkingSpotRequest.Size;
            entity.MaxCapacity = parkingSpotRequest.MaxCapacity;

            var response = await repository.AddAsync(entity);

            if (response == null)
            {
                throw new InvalidOperationException("Failed to create parking spot.");
            }

            await _unitOfWork.SaveChangesAsync();

            await _redisCacheService.RemoveValueAsync(PARKINNG_SPOT_ALL_KEY);

            return _mapper.Map<ParkingSpotResponse>(response);
        }

        public async Task<ParkingSpotPriceResponse> CreateParkingSpotPriceAsync(ParkingSpotPriceRequest parkingSpotPriceRequest)
        {
            if (parkingSpotPriceRequest == null)
                throw new ArgumentNullException(nameof(parkingSpotPriceRequest));

            var repository = _unitOfWork.GetRepository<PriceParkingSpotMapping>();

            var entity = _mapper.Map<PriceParkingSpotMapping>(parkingSpotPriceRequest);

            entity.CreatedAt = DateTime.Now;
            entity.CreatedById = parkingSpotPriceRequest.UserId;
            entity.IsActive = true;
            entity.IsDeleted = false;
            entity.ParkingSpotId = parkingSpotPriceRequest.ParkingSpotId;
            entity.ParkingMinTime = parkingSpotPriceRequest.ParkingMinTime;
            entity.ParkingMaxTime = parkingSpotPriceRequest.ParkingMaxTime;
            entity.ParkingPrice = parkingSpotPriceRequest.ParkingPrice;

            var response = await repository.AddAsync(entity);

            if (response == null)
            {
                throw new InvalidOperationException("Failed to create parking spot price.");
            }

            await _unitOfWork.SaveChangesAsync();

            await _redisCacheService.RemoveValueAsync(PARKINNG_SPOT_PRICE_ALL_KEY);

            return _mapper.Map<ParkingSpotPriceResponse>(response);
        }

        public async Task<ParkingSpotResponse> UpdateParkingSpotAsync(ParkingSpotRequest parkingSpotRequest)
        {
            var repository = _unitOfWork.GetRepository<ParkingSpot>();

            var entity = _mapper.Map<ParkingSpot>(parkingSpotRequest);

            entity.UpdatedById = 1;
            entity.UpdatedAt = DateTime.Now;

            var response = await repository.UpdateAsync(entity);

            await _unitOfWork.SaveChangesAsync();

            await _redisCacheService.RemoveValueAsync(PARKINNG_SPOT_ALL_KEY);

            return _mapper.Map<ParkingSpotResponse>(response);

        }

        public async Task<ParkingSpotPriceResponse> UpdateParkingSpotPriceMappingAsync(ParkingSpotPriceRequest parkingSpotPriceRequest)
        {
            var repository = _unitOfWork.GetRepository<PriceParkingSpotMapping>();

            var entity = _mapper.Map<PriceParkingSpotMapping>(parkingSpotPriceRequest);

            entity.UpdatedById = 1;
            entity.UpdatedAt = DateTime.Now;

            var response = await repository.UpdateAsync(entity);

            await _unitOfWork.SaveChangesAsync();

            await _redisCacheService.RemoveValueAsync(PARKINNG_SPOT_PRICE_ALL_KEY);

            return _mapper.Map<ParkingSpotPriceResponse>(response);

        }

        public async Task<bool> DeleteParkingSpotByIdAsync(long id)
        {
            var repository = _unitOfWork.GetRepository<ParkingSpot>();

            var parkingSpot = await repository.GetAsync(id);

            if (parkingSpot == null)
                throw new ArgumentException(nameof(parkingSpot));

            parkingSpot.Id = id;
            parkingSpot.UpdatedAt = DateTime.Now;
            parkingSpot.DeletedAt = DateTime.Now;
            parkingSpot.IsDeleted = true;
            parkingSpot.IsActive = false;

            await repository.UpdateAsync(parkingSpot);

            var result = await _unitOfWork.SaveChangesAsync();

            await _redisCacheService.RemoveValueAsync(PARKINNG_SPOT_ALL_KEY);

            return result > 0;
        }


        public async Task<bool> DeleteParkingSpotPriceMappingByIdAsync(long id)
        {
            var repository = _unitOfWork.GetRepository<PriceParkingSpotMapping>();

            var priceParkingSpotMapping = await repository.GetAsync(id);

            if (priceParkingSpotMapping == null)
                throw new ArgumentException(nameof(priceParkingSpotMapping));

            priceParkingSpotMapping.Id = id;
            priceParkingSpotMapping.UpdatedAt = DateTime.Now;
            priceParkingSpotMapping.DeletedAt = DateTime.Now;
            priceParkingSpotMapping.IsDeleted = true;
            priceParkingSpotMapping.IsActive = false;

            await repository.UpdateAsync(priceParkingSpotMapping);

            var result = await _unitOfWork.SaveChangesAsync();

            await _redisCacheService.RemoveValueAsync(PARKINNG_SPOT_PRICE_ALL_KEY);

            return result > 0;
        }

        #endregion
    }
}