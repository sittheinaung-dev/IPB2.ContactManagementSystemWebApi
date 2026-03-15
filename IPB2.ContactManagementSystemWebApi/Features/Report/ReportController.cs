using Microsoft.AspNetCore.Mvc;
using IPB2.ContactManagementSystemWebApi.Features.Report;

namespace IPB2.ContactManagementSystemWebApi.Features.Report;

[Route("api/[controller]")]
[ApiController]
public class ReportController : ControllerBase
{
    private readonly ReportServices _reportServices;

    public ReportController(ReportServices reportServices)
    {
        _reportServices = reportServices;
    }

    [HttpGet("View Contact Pagination")]
    public async Task<IActionResult> GetContacts([FromQuery] ContactPaginationRequest request)
    {
        var response = await _reportServices.GetContactsPaginatedAsync(request);
        return Ok(response);
    }

    [HttpGet("ContactsByCategory")]
    public async Task<IActionResult> GetContactsByCategory([FromQuery] ContactsByCategoryReportRequest request)
    {
        if (request.CategoryId.HasValue)
        {
            var response = await _reportServices.GetContactsByCategoryAsync(request);
            if (!response.IsSuccess) return NotFound(response);
            return Ok(response);
        }
        else
        {
            var response = await _reportServices.GetContactsByAllCategoriesAsync();
            return Ok(response);
        }
    }

    [HttpGet("Search")]
    public async Task<IActionResult> SearchContacts([FromQuery] ContactSearchRequest request)
    {
        var response = await _reportServices.SearchContactsAsync(request);
        if (!response.IsSuccess) return BadRequest(response);
        return Ok(response);
    }
}
