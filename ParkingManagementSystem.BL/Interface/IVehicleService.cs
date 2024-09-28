using ParkingManagementSystem.BL.Dto.Request;
using ParkingManagementSystem.BL.Dto.Response;

namespace ParkingManagementSystem.BL.Interface
{
    public interface IVehicleService : IBusinessUnit
    {
        Task<VehicleResponse> GetVehicleByIdAsync(long Id);
        Task<ParkingSpotVehicleMappingResponse> GetParkingSpotVehicleMappingByVehicleIdAsync(long vehicleId);
        Task<List<ParkingSpotVehicleMappingResponse>> GetParkingSpotVehicleMappingAllByParkingSpotIdAsync(long parkingSpotId);
        Task<PriceResponse> GetTotalParkingSpotPriceVehicleIdAsync(long vehicleId);
        Task<VehicleSuccessResponse> CreateVehicleAsync(VehicleRequest vehicleRequest);
        Task<ParkingSpotVehicleMappingResponse> CreateParkingSpotVehicleMappingAsync(ParkingSpotVehicleMappingRequest parkingSpotVehicleMapping);
        Task<VehicleResponse> UpdateVehicleAsync(VehicleRequest vehicleRequest);
        Task<ParkingSpotVehicleMappingResponse> UpdateParkingSpotVehicleMappingAsync(ParkingSpotVehicleMappingRequest parkingSpotVehicleMappingRequest);
        Task<bool> DeleteVehicleByIdAsync(long id);
        Task<bool> DeleteParkingSpotVehicleMappingByIdAsync(long id);
    }
}
