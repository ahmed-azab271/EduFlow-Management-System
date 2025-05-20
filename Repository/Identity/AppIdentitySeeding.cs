using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Repository.Identity
{
    public static class AppIdentitySeeding
    {
        public async static Task Seed (RoleManager<IdentityRole> roleManager)
        {
            if(!roleManager.Roles.Any())
            {
                var RoleString = File.ReadAllText("../Repository/Identity/DataSeed/roles.json");
                var Roles = JsonSerializer.Deserialize<List<string>>(RoleString);
                if (Roles?.Count() > 0)
                {
                    foreach (var Role in Roles)
                        await roleManager.CreateAsync(new IdentityRole(Role));
                }
            }
        }
    }
}
