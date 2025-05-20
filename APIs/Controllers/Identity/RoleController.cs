using APIs.Errors;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace APIs.Controllers.Identity
{

    public class RoleController : BaseController
    {
        private readonly RoleManager<IdentityRole> roleManager;

        public RoleController(RoleManager<IdentityRole> _roleManager)
        {
            roleManager = _roleManager;
        }

        [HttpPost("AddRole")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<string>> AddRole (string name)
        {
            var Role = await roleManager.FindByNameAsync(name);
            if (Role is null)
            {
                var NewRole = new IdentityRole { Name = name };
                await roleManager.CreateAsync(NewRole); 
                return Ok($"The Role : '{name}' Successfully Added");
            }
            return BadRequest(new ApiResponce(404 , $" Role : '{name}' Already Excist"));
        }
    }
}
