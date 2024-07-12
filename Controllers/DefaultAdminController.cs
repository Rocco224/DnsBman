using DnsBman.Data;
using DnsBman.Models;
using DnsBman.Models.IdentityModels;
using DnsBman.Services.ApiKey;
using DnsBman.Utilities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace DnsBman.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DefaultAdminController : ControllerBase
    {
        private readonly DnsBmanContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly ApiKeyHandler _apiKeyHandler;

        public DefaultAdminController(DnsBmanContext context, RoleManager<ApplicationRole> roleManager, UserManager<ApplicationUser> userManager, ApiKeyHandler apiKeyHandler)
        {
            _context = context;
            _roleManager = roleManager;
            _userManager = userManager;
            _apiKeyHandler = apiKeyHandler;
        }

        [HttpGet("CreateAdmin")]
        public async Task<IActionResult> CreateDefaultAdmin()
        {
            try
            {
                if (!await AdminExist() && Request.Headers.TryGetValue("X-API-KEY", out var values))
                {
                    var keyValue = values.ToString();

                    UsersApiKey? apiKey = _context.UsersApiKeys.FirstOrDefault(k => k.Value == keyValue);

                    if (apiKey != null || keyValue == ConfigurationHandler.GetApiKey())
                    {
                        var user = new ApplicationUser { UserName = "Admin" };
                        var result = await _userManager.CreateAsync(user, "Pass123$");

                        if (result.Succeeded) 
                        {
                            await _userManager.AddToRoleAsync(user, "Admin");
                            await _apiKeyHandler.AssignApiKeyToUser(user);
                        }

                        return Ok(new { username = "Admin", password = "Pass123$" });
                    }
                    else
                    {
                        return BadRequest("La chiave non è valida.");
                    }
                }
                else
                {
                    return BadRequest("La chiave non è presente nell'header. Oppure è già presente un admin nel database");
                }
            }
            catch (Exception ex)
            {
                Log.Error("Errore durante la creazione dell'admin di default", ex);
                return Problem(ex.Message);
            }
        }

        [HttpGet("GetApiKey")]
        public async Task<IActionResult> GetApiKey()
        {
            if(!await AdminExist())
                return Ok(ConfigurationHandler.GetApiKey());

            return BadRequest("È già presente un admin nel database");
        }

        private async Task<bool> AdminExist()
        {
            if (!await _roleManager.RoleExistsAsync("Admin"))
            {
                await _roleManager.CreateAsync(new ApplicationRole { Name = "Admin" });
                return false;
            }

            var adminRole = _context.Roles.FirstOrDefault(r => r.Name == "Admin");
            var adminUser = _context.UserRoles.FirstOrDefault(r => r.RoleId == adminRole.Id);

            if (adminUser == null) 
                return false;

            return true;
        }
    }
}
