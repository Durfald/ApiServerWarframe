using ApiServerWarframe.Models;
using ApiServerWarframe.Services.Sorting;
using ApiServerWarframe.Services.State;
using ApiServerWarframe.Services.Storage;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ApiServerWarframe.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InfoController : ControllerBase
    {
        private readonly DataProcessingState _state;
        private readonly IDataStorage _fileHelper;
        private readonly SortingService _sortingService;

        public InfoController(DataProcessingState state, IDataStorage fileHelper, SortingService sortingService)
        {
            _state = state;
            _fileHelper = fileHelper;
            _sortingService = sortingService;
        }

        [HttpGet("status")]
        public IActionResult GetStatus()
        {
            return Ok(new
            {
                IsDownloading = _state.IsDownloading,
                IsSorting = _state.IsSorting
            });
        }

        [HttpGet("items")]
        public async Task<IActionResult> GetItems(
            [FromHeader] string language = "ru",
            [FromQuery] bool is48Hours = true,
            [FromQuery] int minSpread = 15,
            [FromQuery] int minLiquidity = 60)
        {
            //if (_state.IsSorting)
            //{
            //    return StatusCode(503, "Data is currently being sorted. Please try again later.");
            //}

            //var sortedItems = _fileHelper.GetWhiteList(is48Hours, language);
            var sortedItems = await _sortingService.SortData(language,minSpread,minLiquidity);
            if (sortedItems == null)
                return StatusCode(500, "Failed to get items");

            if (!sortedItems.Any())
                return StatusCode(500, "Items list is empty");

            return Ok(sortedItems);
        }

        [HttpGet("items/details")]
        public IActionResult GetDetails()
        {
            var details = _fileHelper.GetItemDetails();
            if (details == null)
                return StatusCode(500, "Failed to get items");

            if (!details.Any())
                return StatusCode(500, "Items list is empty");
            return Ok(details);
        }
    }

}