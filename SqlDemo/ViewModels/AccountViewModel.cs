using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace SqlDemo.ViewModels
{
    public class AccountViewModel
    {
        public IdentityUser<Guid> User { get; set; }
        public IList<Claim> Claims { get; set; }
    }
}