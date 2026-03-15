using Microsoft.AspNetCore.Mvc;
using IPB2.ContactManagementSystemWebApi.Features.Contact.Models;

namespace IPB2.ContactManagementSystemWebApi.Features.Contact;

[Route("api/[controller]")]
[ApiController]
public class ContactController : ControllerBase
{
    private readonly ContactFeature _contactFeature;

    public ContactController(ContactFeature contactFeature)
    {
        _contactFeature = contactFeature;
    }

    [HttpGet]
    public async Task<IActionResult> GetContacts()
    {
        var response = await _contactFeature.GetAllAsync();
        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetContact(int id)
    {
        var response = await _contactFeature.GetByIdAsync(id);
        if (!response.IsSuccess) return NotFound(response);
        return Ok(response);
    }

    [HttpPost]
    public async Task<IActionResult> CreateContact(CreateContactRequest request)
    {
        var response = await _contactFeature.CreateAsync(request);
        return Ok(response);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateContact(UpdateContactRequest request)
    {
        var response = await _contactFeature.UpdateAsync(request);
        if (!response.IsSuccess) return NotFound(response);
        return Ok(response);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteContact(int id)
    {
        var response = await _contactFeature.DeleteAsync(id);
        if (!response.IsSuccess) return NotFound(response);
        return Ok(response);
    }
}
