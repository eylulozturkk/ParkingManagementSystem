using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NotificationService.BL.Attributes;
using ParkingManagementSystem.BL.Dto.Request;
using ParkingManagementSystem.BL.Dto.Response;
using ParkingManagementSystem.BL.Interface;
using ParkingManagementSystem.DAL.Entity;

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

        /// <summary>
        /// Get parkingspot All [Returns ParkingSpotResponse]
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(typeof(List<ParkingSpotResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetParkingSpotAll()
        {
            try
            {
                var response = await _parkingSpotService.GetParkingSpotAllAsync();
                return response != null
                       ? Ok(response)
                       : StatusCode(StatusCodes.Status400BadRequest, "get işlemi sırasında bir hata oluştu");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest,ex.Message);
            }
        }

        /// <summary>
        /// Get parkingspot by id [Returns ParkingSpotResponse]
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("id/{id}")]
        [ProducesResponseType(typeof(ParkingSpotResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetParkingSpotById(long id)
        {
            try
            {
                var response = await _parkingSpotService.GetParkingSpotByIdAsync(id);
                return response != null
                       ? Ok(response)
                       : StatusCode(StatusCodes.Status400BadRequest, "get işlemi sırasında bir hata oluştu");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, ex.Message);
            }
        }

        /// <summary>
        /// Get vehicle by id [Returns VehicleResponse]
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("id/{id}")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(typeof(VehicleResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetVehicleById(long id)
        {
            try
            {
                var response = await _vehicleService.GetVehicleByIdAsync(id);
                return response != null
                       ? Ok(response)
                       : StatusCode(StatusCodes.Status400BadRequest, "get işlemi sırasında bir hata oluştu");

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, ex.Message);

            }
        }

        /// <summary>
        /// Get Parking Spot Price All [Returns ParkingSpotPriceResponse]
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(typeof(List<ParkingSpotPriceResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetParkingSpotPriceAll()
        {
            try
            {
                var response = await _parkingSpotService.GetParkingSpotPriceAllAsync();
                return response != null
                          ? Ok(response)
                          : StatusCode(StatusCodes.Status400BadRequest, "get işlemi sırasında bir hata oluştu");

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, ex.Message);

            }
        }

        /// <summary>
        /// Get Total Parking Spot Price By Vehicle Id [Returns PriceResponse]
        /// </summary>
        /// <returns></returns>
        [HttpGet("id/{id}")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(typeof(PriceResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetTotalParkingSpotPriceByVehicleId(long id)
        {
            try
            {
                var response = await _vehicleService.GetTotalParkingSpotPriceVehicleIdAsync(id);
                return response.IsSuccess
                                ? Ok(response)
                                : StatusCode(StatusCodes.Status400BadRequest, "get işlemi sırasında bir hata oluştu");


            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, ex.Message);

            }
        }

        /// <summary>
        /// Create Parking Spot by ParkingSpotRequest
        /// </summary>
        /// <param name="parkingSpotRequest"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateModel]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(typeof(ParkingSpotResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> CreateParkingSpot([FromBody] ParkingSpotRequest parkingSpotRequest)
        {
            try
            {
                var parkingSpot = await _parkingSpotService.CreateParkingSpotAsync(parkingSpotRequest);
                return parkingSpot.Name != null
                 ? Ok(parkingSpot)
                 : StatusCode(StatusCodes.Status400BadRequest, "create işlemi sırasında bir hata oluştu");

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, ex.Message);

            }
        }

        /// <summary>
        /// Create Vehicle by vehicleRequest
        /// </summary>
        /// <param name="vehicleRequest"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateModel]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(typeof(VehicleSuccessResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> CreateVehicle([FromBody] VehicleRequest vehicleRequest)
        {
            try
            {
                var vehicle = await _vehicleService.CreateVehicleAsync(vehicleRequest);

                return vehicle.VehicleResponse.LicensePlate != null
                                  ? Ok(vehicle)
                                  : StatusCode(StatusCodes.Status400BadRequest, "create işlemi sırasında bir hata oluştu");

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, ex.Message);

            }
        }

        /// <summary>
        /// Create Parking Spot Price by ParkingSpotPriceRequest
        /// </summary>
        /// <param name="parkingSpotPriceRequest"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateModel]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(typeof(ParkingSpotPriceResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> CreateParkingSpotPrice([FromBody] ParkingSpotPriceRequest parkingSpotPriceRequest)
        {
            try
            {
                var parkingSpotPrice = await _parkingSpotService.CreateParkingSpotPriceAsync(parkingSpotPriceRequest);

                return parkingSpotPrice.ParkingSpotId > 0
                                   ? Ok(parkingSpotPrice)
                                   : StatusCode(StatusCodes.Status400BadRequest, "create işlemi sırasında bir hata oluştu");

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, ex.Message);

            }
        }


        /// <summary>
        /// Create Parking Spot vehicle mapping by ParkingSpotVehicleMappingRequest
        /// </summary>
        /// <param name="parkingSpotVehicleMappingRequest"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateModel]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(typeof(ParkingSpotVehicleMappingResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> CreateParkingSpotVehicleMapping([FromBody] ParkingSpotVehicleMappingRequest parkingSpotVehicleMappingRequest)
        {
            try
            {
                var parkingSpotVehicleMapping = await _vehicleService.CreateParkingSpotVehicleMappingAsync(parkingSpotVehicleMappingRequest);
                return parkingSpotVehicleMapping.ParkingSpotId > 0
                                   ? Ok(parkingSpotVehicleMapping)
                                   : StatusCode(StatusCodes.Status400BadRequest, "create işlemi sırasında bir hata oluştu");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, ex.Message);

            }
        }


        /// <summary>
        /// Update Parking Spot by ParkingSpotRequest
        /// </summary>
        /// <param name="parkingSpotRequest"></param>
        /// <returns></returns>
        [HttpPut]
        [ValidateModel]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(typeof(ParkingSpotResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateParkingSpot([FromBody] ParkingSpotRequest parkingSpotRequest)
        {
            try
            {
                var parkingSpot = await _parkingSpotService.UpdateParkingSpotAsync(parkingSpotRequest);
                return parkingSpot.Name != null
                    ? Ok(parkingSpot)
                    : StatusCode(StatusCodes.Status400BadRequest, "update işlemi sırasında bir hata oluştu");

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, ex.Message);

            }
        }

        /// <summary>
        /// Update Vehicle by vehicleRequest
        /// </summary>
        /// <param name="vehicleRequest"></param>
        /// <returns></returns>
        [HttpPut]
        [ValidateModel]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(typeof(VehicleResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateVehicle([FromBody] VehicleRequest vehicleRequest)
        {
            try
            {
                var vehicle = await _vehicleService.UpdateVehicleAsync(vehicleRequest);

                return vehicle.LicensePlate != null
                    ? Ok(vehicle)
                    : StatusCode(StatusCodes.Status400BadRequest, "update işlemi sırasında bir hata oluştu");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, ex.Message);

            }
        }

        /// <summary>
        /// Update Parking Spot Price by ParkingSpotPriceRequest
        /// </summary>
        /// <param name="parkingSpotPriceRequest"></param>
        /// <returns></returns>
        [HttpPut]
        [ValidateModel]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(typeof(ParkingSpotPriceResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateParkingSpotPrice([FromBody] ParkingSpotPriceRequest parkingSpotPriceRequest)
        {
            try
            {
                var parkingSpotPrice = await _parkingSpotService.UpdateParkingSpotPriceMappingAsync(parkingSpotPriceRequest);

                return parkingSpotPrice.ParkingSpotId > 0
                    ? Ok(parkingSpotPrice)
                    : StatusCode(StatusCodes.Status400BadRequest, "update işlemi sırasında bir hata oluştu");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, ex.Message);

            }
        }


        /// <summary>
        /// Update Parking Spot vehicle mapping by ParkingSpotVehicleMappingRequest
        /// </summary>
        /// <param name="parkingSpotVehicleMappingRequest"></param>
        /// <returns></returns>
        [HttpPut]
        [ValidateModel]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(typeof(ParkingSpotVehicleMappingResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateParkingSpotVehicleMapping([FromBody] ParkingSpotVehicleMappingRequest parkingSpotVehicleMappingRequest)
        {
            try
            {
                var parkingSpotVehicleMapping = await _vehicleService.UpdateParkingSpotVehicleMappingAsync(parkingSpotVehicleMappingRequest);

                return parkingSpotVehicleMapping.ParkingSpotId > 0
                    ? Ok(parkingSpotVehicleMapping)
                    : StatusCode(StatusCodes.Status400BadRequest, "update işlemi sırasında bir hata oluştu");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, ex.Message);

            }
        }


        /// <summary>
        /// Delete Parking Spot by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("id/{id}")]
        [ValidateModel]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteParkingSpotById(long id)
        {
            try
            {
                var parkingSpot = await _parkingSpotService.DeleteParkingSpotByIdAsync(id);

                return parkingSpot
                       ? Ok(parkingSpot)
                       : StatusCode(StatusCodes.Status400BadRequest, "delete işlemi sırasında bir hata oluştu");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, ex.Message);

            }
        }

        /// <summary>
        /// Delete Vehicle by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("id/{id}")]
        [ValidateModel]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteVehicleById(long id)
        {
            try
            {
                var vehicle = await _vehicleService.DeleteVehicleByIdAsync(id);
                return vehicle
                       ? Ok(vehicle)
                       : StatusCode(StatusCodes.Status400BadRequest, "delete işlemi sırasında bir hata oluştu");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, ex.Message);

            }
        }

        /// <summary>
        /// Delete Parking Spot Price by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("id/{id}")]
        [ValidateModel]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteParkingSpotPriceById(long id)
        {
            try
            {
                var parkingSpotPrice = await _parkingSpotService.DeleteParkingSpotPriceMappingByIdAsync(id);
                return parkingSpotPrice
                         ? Ok(parkingSpotPrice)
                         : StatusCode(StatusCodes.Status400BadRequest, "delete işlemi sırasında bir hata oluştu");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, ex.Message);

            }
        }


        /// <summary>
        /// Delete Parking Spot vehicle mapping by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("id/{id}")]
        [ValidateModel]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteParkingSpotVehicleMappingById(long id)
        {
            try
            {
                var parkingSpotVehicleMapping = await _vehicleService.DeleteParkingSpotVehicleMappingByIdAsync(id);
                return parkingSpotVehicleMapping
                       ? Ok(parkingSpotVehicleMapping)
                       : StatusCode(StatusCodes.Status400BadRequest, "delete işlemi sırasında bir hata oluştu");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, ex.Message);

            }
        }

        #endregion
    }
}
