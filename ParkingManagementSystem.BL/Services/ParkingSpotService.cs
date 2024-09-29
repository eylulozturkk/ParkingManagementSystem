using AutoMapper;
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
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public ParkingSpotService(
            IMapper mapper,
            IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<List<ParkingSpotResponse>> GetParkingSpotAllAsync()
        {
            var repository = _unitOfWork.GetRepository<ParkingSpot>();
            var repoAll = await repository.GetAllAsync();

            return _mapper.Map<List<ParkingSpotResponse>>(repoAll);
        }

        public async Task<List<ParkingSpotPriceResponse>> GetParkingSpotPriceAllAsync()
        {
            var repository = _unitOfWork.GetRepository<PriceParkingSpotMapping>();
            var repoAll = await repository.GetAllAsync();

            return _mapper.Map<List<ParkingSpotPriceResponse>>(repoAll);
        }

        public async Task<ParkingSpotResponse> GetParkingSpotByIdAsync(long id)
        {
            var repository = _unitOfWork.GetRepository<ParkingSpot>();
            var repoAll = await repository.GetAllAsync();
            var response = repoAll.FirstOrDefault(p => p.Id == id && p.IsActive);

            return _mapper.Map<ParkingSpotResponse>(response);
        }

        public async Task<ParkingSpotResponse> GetParkingSpotBySizeAsync(VehicleSizeType vehicleSizeType)
        {
            var repository = _unitOfWork.GetRepository<ParkingSpot>();
            var repoAll = await repository.GetAllAsync();
            var response = repoAll.FirstOrDefault(p =>
                p.Size == vehicleSizeType);

            return _mapper.Map<ParkingSpotResponse>(response);
        }

        public async Task<ParkingSpotPriceResponse> GetParkingSpotPriceByParkingSpotIdAsync(long parkingSpotId, double time)
        {
            var repository = _unitOfWork.GetRepository<PriceParkingSpotMapping>();
            var repoAll = await repository.GetAllAsync();
            var response = repoAll.FirstOrDefault(p =>
                p.ParkingSpotId == parkingSpotId &&
                p.IsActive &&
                time >= p.ParkingMinTime &&
                time <= p.ParkingMaxTime);

            return _mapper.Map<ParkingSpotPriceResponse>(response);
        }

        public async Task<ParkingSpotResponse> CreateParkingSpotAsync(ParkingSpotRequest parkingSpotRequest)
        {

            var repository = _unitOfWork.GetRepository<ParkingSpot>();

            var entity = _mapper.Map<ParkingSpot>(parkingSpotRequest);

            entity.CreatedAt = DateTime.Now;
            entity.CreatedById = parkingSpotRequest.UserId;
            entity.IsActive = true;
            entity.Name = parkingSpotRequest.Name;
            entity.Size = parkingSpotRequest.Size;
            entity.MaxCapacity = parkingSpotRequest.MaxCapacity;

            var response = await repository.AddAsync(entity);

            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<ParkingSpotResponse>(response);

        }

        public async Task<ParkingSpotPriceResponse> CreateParkingSpotPriceAsync(ParkingSpotPriceRequest parkingSpotPriceRequest)
        {
            var repository = _unitOfWork.GetRepository<PriceParkingSpotMapping>();

            var entity = _mapper.Map<PriceParkingSpotMapping>(parkingSpotPriceRequest);

            entity.CreatedAt = DateTime.Now;
            entity.IsActive = true;
            entity.ParkingSpotId = parkingSpotPriceRequest.ParkingSpotId;
            entity.ParkingPrice = parkingSpotPriceRequest.ParkingPrice;
            entity.ParkingMinTime = parkingSpotPriceRequest.ParkingMinTime;
            entity.ParkingMaxTime = parkingSpotPriceRequest.ParkingMaxTime;

            var response = await repository.AddAsync(entity);

            await _unitOfWork.SaveChangesAsync();

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

            return result > 0;
        }
    }
}
