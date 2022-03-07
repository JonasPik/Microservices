using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Play.Identity.Service.Dtos;
using Play.Identity.Service.Entities;
using System.Threading.Tasks;
using System;
using Microsoft.AspNetCore.Authorization;
using static IdentityServer4.IdentityServerConstants;

namespace Play.Identity.Service.Controllers
{
    [ApiController]
    [Route("users")]
    [Authorize(Policy = LocalApi.PolicyName, Roles = Roles.Admin)]
    public class UsersController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> userManager;
        public UsersController(UserManager<ApplicationUser> userManager)
        {
            this.userManager = userManager;
        }

        // GET /users
        [HttpGet]
        public ActionResult<IEnumerable<UserDto>> Get()
        {
            var users = userManager.Users
            .ToList()
            .Select(user => user.AsDto());

            return Ok(users);
        }

        // GET /users/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> GetByIdAsync(Guid id)
        {
            var user = await userManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                return NotFound();
            }

            return user.AsDto();
        }

        // PUT /users/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAsync(Guid id, UpdateUserDto userDto)
        {
            var existingUser = await userManager.FindByIdAsync(id.ToString());
            if (existingUser == null)
            {
                return NotFound();
            }

            existingUser.Email = userDto.Email;
            existingUser.UserName = userDto.Email;
            existingUser.Gil = userDto.Gil;

            await userManager.UpdateAsync(existingUser);
            return NoContent();
        }

        // DELETE /users/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            var existingUser = await userManager.FindByIdAsync(id.ToString());
            if (existingUser == null)
            {
                return NotFound();
            }

            await userManager.DeleteAsync(existingUser);
            return NoContent();
        }
    }
}