namespace IPB2.ContactManagementSystemWebApi.Features.Category;

public class CreateCategoryRequest
{
    public string CategoryName { get; set; } = string.Empty;
}

public class CreateCategoryResponse
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = string.Empty;
    public int CategoryId { get; set; }
}

public class UpdateCategoryRequest
{
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
}

public class UpdateCategoryResponse
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = string.Empty;
}

public class DeleteCategoryResponse
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = string.Empty;
}

public class CategoryResponse
{
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public int ContactCount { get; set; }
}

public class CategoryListResponse
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = string.Empty;
    public List<CategoryResponse> Categories { get; set; } = new();
}

public class CategoryWithContactsResponse
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = string.Empty;
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public int ContactCount { get; set; }
    public List<Contact.ContactResponse> Contacts { get; set; } = new();
}
