using FarmMonitor.Domain.Dtos;
using FarmMonitor.Domain.Exceptions;
using FarmMonitor.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FarmMonitor.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TelemetryController : ControllerBase
    {
        private readonly ITelemetryService _service;
        private readonly ILogger<TelemetryController> _logger;

        public TelemetryController(
            ITelemetryService service,
            ILogger<TelemetryController> logger)
        {
            _service = service;
            _logger = logger;
        }

        /// <summary>
        /// Created a telemetry to farm's device
        /// </summary>
        /// <param name="telemetry">Telemetry data</param>
        /// <response code="200">When the information was successfully created</response>
        /// <response code="404">When not found a related device to create a telemetry</response>
        /// <response code="500">When an internal error occurs</response>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PostAsync([FromBody] TelemetryDto telemetry)
        {
            try
            {
                await _service.CreateAsync(telemetry);
                return Ok();
            }
            catch (EntityNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Post devices api fatal error.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}