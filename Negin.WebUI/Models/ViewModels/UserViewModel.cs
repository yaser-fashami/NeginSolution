﻿using Negin.Core.Domain.Entities;

namespace Negin.WebUI.Models.ViewModels;

public class UserViewModel
{
    public User User { get; set; }
    public string[] Role { get; set; }
}
