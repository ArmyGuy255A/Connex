using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Identity;

public class ApplicationUser : IdentityUser
{
    public ApplicationUser(string userName) : base(userName)
    {
        base.Email = userName;
        LoginCount = 0;
        LastLoginDate = DateTime.Now;
    }

    public string? GivenName { get; set; }
    public string? Surname { get; set; }
    public string? DisplayName { get; set; }
    [DataType(DataType.DateTime)] public DateTime LastLoginDate { get; set; }
    public int LoginCount { get; set; }
}