using FarmManager.Domain.Dtos;
using FarmManager.Domain.Exceptions;
using FarmManager.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FarmManager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeviceController : ControllerBase
    {
        private readonly ILogger<DeviceController> _logger;
        private readonly IService<DeviceDto> _service;

        private IActionResult HandleException(Exception ex, string apiMethodName)
        {
            _logger.LogError(ex, $"{apiMethodName} devices api fatal error.");
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }

        public DeviceController(IService<DeviceDto> service, ILogger<DeviceController> logger)
        {
            _service = service;
            _logger = logger;
        }

        /// <summary>
        /// Delete the specified device
        /// </summary>
        /// <param name="id">Device identifier</param>
        /// <response code="200">When the information was successfully deleted</response>
        /// <response code="404">When not found the device to delete</response>
        /// <response code="500">When an internal error occurs</response>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            try
            {
                await _service.DeleteAsync(id);
                return Ok();
            }
            catch (EntityNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return HandleException(ex, "Delete");
            }
        }

        /// <summary>
        /// Gets all registered devices
        /// </summary>
        /// <returns>List os devices</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Produces("application/json")]
        public async Task<IActionResult> GetAsync()
        {
            try
            {
                return Ok(await _service.GetAsync());
            }
            catch (Exception ex)
            {
                return HandleException(ex, "Get all");
            }
        }

        /// <summary>
        /// Create a device
        /// </summary>
        /// <param name="device">Device data</param>
        /// <response code="200">When the information was successfully integrated</response>
        /// <response code="404">When not found the farm to create a device</response>
        /// <response code="500">When an internal error occurs</response>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PostAsync([FromBody] DeviceDto device)
        {
            try
            {
                await _service.CreateAsync(device);
                return Ok();
            }
            catch (EntityNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return HandleException(ex, "Post");
            }
        }

        /// <summary>
        /// Update a device
        /// </summary>
        /// <param name="device">Device data</param>
        /// <response code="200">When the information was successfully updated</response>
        /// <response code="404">When not found the farm to update a device</response>
        /// <response code="500">When an internal error occurs</response>
        /// <returns></returns>
        [HttpPut()]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PutAsync([FromBody] DeviceDto device)
        {
            try
            {
                await _service.UpdateAsync(device);
                return Ok();
            }
            catch (EntityNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return HandleException(ex, "Put");
            }
        }
    }
}
