using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Negin.Core.Domain.Entities;

namespace Negin.WebUI.Models.ViewModels;

public class UserViewModel
{
    public User User { get; set; }
	public IFormFile? Avatar { get; set; }
    [ValidateNever]
    public byte? avatar_remove { get; set; }
    public string[] Role { get; set; }
}
