using System;
using System.Collections.Generic;

namespace IPB2.ContactManagementSystemWebApi.Database.AppDbContextModels;

public partial class Category
{
    public int CategoryId { get; set; }

    public string CategoryName { get; set; } = null!;

    public bool? IsDelete { get; set; }
}
