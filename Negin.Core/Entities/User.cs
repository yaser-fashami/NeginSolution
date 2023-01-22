using DataAnnotationsExtensions;
using Microsoft.AspNetCore.Identity;
using Negin.Core.Domain.Entities.Basic;

namespace Negin.Core.Domain.Entities;

public class User : IdentityUser
{
    public override string UserName { get; set; }
    public override string PasswordHash { get; set; }
    public string? FirstName { get; set; }
	public string? LastName { get; set; }
	public override string? PhoneNumber { get; set; }
	[Email]
	public override string? Email { get; set; }
	public bool IsActived { get; set; } = false;
	public DateTime CreateDate { get; set; }
	public DateTime? LastLogInDate { get; set; }

	public ICollection<IdentityRole>? Roles { get; set; }

}
