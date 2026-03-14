using System.Collections.Generic;

namespace IPB2.ContactManagementSystemWebApi.Dtos
{
    public class CategoryCreateRequestDto
    {
        public string CategoryName { get; set; }
    }

    public class CategoryCreateResponseDto
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public int CategoryId { get; set; }
    }

    public class CategoryUpdateRequestDto
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
    }

    public class CategoryUpdateResponseDto
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
    }

    public class CategoryDeleteResponseDto
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
    }

    public class CategoryResponseDto
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public int ContactCount { get; set; }
    }

    public class CategoryListResponseDto
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public List<CategoryResponseDto> Categories { get; set; }
    }

    public class CategoryWithContactsResponseDto
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public int ContactCount { get; set; }
        public List<ContactResponseDto> Contacts { get; set; }
    }
}