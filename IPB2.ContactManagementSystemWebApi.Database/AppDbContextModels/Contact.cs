using System;
using System.Collections.Generic;

namespace IPB2.ContactManagementSystemWebApi.Database.AppDbContextModels;

public partial class Contact
{
    public int ContactId { get; set; }

    public string ContactName { get; set; } = null!;

    public string? Phone { get; set; }

    public string? Email { get; set; }

    public int? CategoryId { get; set; }

    public bool? IsDelete { get; set; }
    public virtual Category? Category { get; set; }

}
