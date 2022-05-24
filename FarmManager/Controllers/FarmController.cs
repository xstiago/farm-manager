using FarmManager.Domain.Dtos;
using FarmManager.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FarmManager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FarmController : ControllerBase
    {
        private readonly ILogger<FarmController> _logger;
        private readonly IFarmService _service;

        private IActionResult HandleException(Exception ex, string apiMethodName)
        {
            _logger.LogError(ex, $"{apiMethodName} farms api fatal error.");
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }

        public FarmController(IFarmService service, ILogger<FarmController> logger)
        {
            _service = service;
            _logger = logger;
        }

        /// <summary>
        /// Delete the specified farm
        /// </summary>
        /// <param name="id">Farm identifier</param>
        /// <response code="200">When the information was successfully deleted</response>
        /// <response code="500">When an internal error occurs</response>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            try
            {
                await _service.DeleteAsync(id);
                return Ok();
            }
            catch (Exception ex)
            {
                return HandleException(ex, "Delete");
            }
        }

        /// <summary>
        /// Gets all registered farms
        /// </summary>
        /// <returns>List os farms</returns>
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
        /// Create a farm
        /// </summary>
        /// <param name="farm">Farm data</param>
        /// <response code="200">When the information was successfully integrated</response>
        /// <response code="500">When an internal error occurs</response>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PostAsync([FromBody] FarmDto farm)
        {
            try
            {
                await _service.CreateAsync(farm);
                return Ok();
            }
            catch (Exception ex)
            {
                return HandleException(ex, "Post");
            }
        }

        /// <summary>
        /// Update a farm
        /// </summary>
        /// <param name="farm">Farm data</param>
        /// <response code="200">When the information was successfully updated</response>
        /// <response code="500">When an internal error occurs</response>
        /// <returns></returns>
        [HttpPut()]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PutAsync([FromBody] FarmDto farm)
        {
            try
            {
                await _service.UpdateAsync(farm);
                return Ok();
            }
            catch (Exception ex)
            {
                return HandleException(ex, "Put");
            }
        }
    }
}
