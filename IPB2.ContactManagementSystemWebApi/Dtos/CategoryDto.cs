namespace IPB2.ContactManagementSystemWebApi.Dtos
{
    public class CategoryCreateRequestDto
    {
        public string CategoryName { get; set; }
    }

    public class CategoryUpdateRequestDto
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
    }

    public class CategoryResponseDto
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public int ContactCount { get; set; }
    }
}