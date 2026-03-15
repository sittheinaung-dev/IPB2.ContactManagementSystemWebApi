using Microsoft.AspNetCore.Mvc;
using IPB2.ContactManagementSystemWebApi.Features.Category;

namespace IPB2.ContactManagementSystemWebApi.Features.Category;

[Route("api/[controller]")]
[ApiController]
public class CategoryController : ControllerBase
{
    private readonly CategoryServices _categoryServices;

    public CategoryController(CategoryServices categoryServices)
    {
        _categoryServices = categoryServices;
    }

    [HttpGet]
    public async Task<IActionResult> GetCategories()
    {
        var response = await _categoryServices.GetAllAsync();
        return Ok(response);
    }

    [HttpGet("WithContacts/{id}")]
    public async Task<IActionResult> GetCategoryWithContacts(int id)
    {
        var response = await _categoryServices.GetWithContactsAsync(id);
        if (!response.IsSuccess) return NotFound(response);
        return Ok(response);
    }

    [HttpPost]
    public async Task<IActionResult> CreateCategory(CreateCategoryRequest request)
    {
        var response = await _categoryServices.CreateAsync(request);
        if (!response.IsSuccess) return BadRequest(response);
        return Ok(response);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateCategory(UpdateCategoryRequest request)
    {
        var response = await _categoryServices.UpdateAsync(request);
        if (!response.IsSuccess) return (response.Message == "Category not found") ? NotFound(response) : BadRequest(response);
        return Ok(response);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCategory(int id)
    {
        var response = await _categoryServices.DeleteAsync(id);
        if (!response.IsSuccess) return (response.Message == "Category not found") ? NotFound(response) : BadRequest(response);
        return Ok(response);
    }
}
