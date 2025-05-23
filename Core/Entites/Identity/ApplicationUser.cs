﻿using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entites.Identity
{
    public class ApplicationUser: IdentityUser
    {
        [Required]
        public string FullName { get; set; }
        public string? Address { get; set; }
    }
}
