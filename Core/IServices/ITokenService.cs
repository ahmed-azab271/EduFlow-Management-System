using Core.Entites.Identity;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.IServices
{
    public interface ITokenService
    {
        Task<string> CreateTokenAsync(ApplicationUser User, UserManager<ApplicationUser> userManager);
    }
}
