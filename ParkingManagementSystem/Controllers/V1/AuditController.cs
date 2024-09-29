using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using ParkingManagementSystem.BL.Dto.Request;
using ParkingManagementSystem.BL.Interface;

namespace ParkingManagementSystem.API.Controllers.V1
{
    [ApiVersion("1.0")]
    [Route("ParkingManagementSystem/api/[controller]")]
    [ApiController]
    [EnableCors("Phantom-policy")]
    public class AuditController : ControllerBase
    {

        #region Fields
        private readonly IAuditService _auditService;
        #endregion

        #region Ctor
        public AuditController(IAuditService auditService)
        {
            _auditService = auditService;
        }
        #endregion

        #region Apis

        /// <summary>
        /// Get audits by AuditRequest [Returns PageListResponse]
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet("")]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> Get([FromQuery] AuditRequest request)
        {
            try
            {
                var result = await _auditService.GetAsync(request);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, ex.Message);
            }
        }
        #endregion

    }
}
