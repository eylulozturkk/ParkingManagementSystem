using ParkingManagementSystem.BL.Dto.Request;
using ParkingManagementSystem.BL.Dto.Response;
using ParkingManagementSystem.DAL.Entity;
using ParkingManagementSystem.DAL.Enums;

namespace ParkingManagementSystem.BL.Interface
{
    public interface IParkingSpotService : IBusinessUnit
    {
        Task<List<ParkingSpotResponse>> GetParkingSpotAllAsync();
        Task<List<ParkingSpotPriceResponse>> GetParkingSpotPriceAllAsync();
        Task<ParkingSpotResponse> GetParkingSpotByIdAsync(long id);
        Task<ParkingSpotResponse> GetParkingSpotBySizeAsync(VehicleSizeType vehicleSizeType);
        Task<ParkingSpotPriceResponse> GetParkingSpotPriceByParkingSpotIdAsync(long parkingSpotId, double time);
        Task<ParkingSpotResponse> CreateParkingSpotAsync(ParkingSpotRequest parkingSpotRequest);
        Task<ParkingSpotPriceResponse> CreateParkingSpotPriceAsync(ParkingSpotPriceRequest parkingSpotPriceRequest);
        Task<ParkingSpotResponse> UpdateParkingSpotAsync(ParkingSpotRequest parkingSpotRequest);
        Task<ParkingSpotPriceResponse> UpdateParkingSpotPriceMappingAsync(ParkingSpotPriceRequest parkingSpotPriceRequest);
        Task<bool> DeleteParkingSpotByIdAsync(long id);
        Task<bool> DeleteParkingSpotPriceMappingByIdAsync(long id);
    }
}
