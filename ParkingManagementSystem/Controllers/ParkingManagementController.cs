using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using ParkingManagementSystem.BL.Interface;

namespace ParkingManagementSystem.API.Controllers
{
    [ApiVersion("1.0")]
    [Route("ParkingManagementSystem/api/[controller]")]
    [ApiController]
    [EnableCors("Phantom-policy")]
    public class ParkingManagementController: ControllerBase
    {
        #region Fields
        private readonly IVehicleService _vehicleService;
        private readonly IParkingSpotService _parkingSpotService;

        #endregion

        #region Ctor
        public ParkingManagementController(IVehicleService vehicleService, IParkingSpotService parkingSpotService)
        {
            _parkingSpotService = parkingSpotService;
            _vehicleService = vehicleService;
        }
        #endregion

        #region Apis
        #endregion
    }
}
