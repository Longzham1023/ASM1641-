using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ASM1641_.Dtos;
using ASM1641_.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ASM1641_.Controllers
{
    [Route("api/v1/admin")]
    [Authorize]
    public class AdminController : Controller
    {
        private readonly IAdminService _adminService;
        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }
        // GET: api/values
        [HttpGet("view-list-account-users"), Authorize(Roles = "Admin")]
        public async Task<IActionResult> Get([FromQuery]int page)
        {
            try
            {
                var list = await _adminService.ViewListAccountUser(page);
                return Ok(list);
            }catch(Exception ex)
            {
                return BadRequest(ex);
            }
        }

        // GET api/values
        [HttpGet("view-list-account-store-owners"), Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAccountById([FromQuery]int page)
        {
            try
            {
                var accounts = await _adminService.ViewListAccountStoreOwners(page);
                return Ok(accounts);
            }catch(Exception e)
            {
                return BadRequest(e);
            }
        }

        // POST api/values
        [HttpPost("create-store-owner-account"), Authorize(Roles = "Admin")]
        public async Task<IActionResult> Post([FromForm]StoreOwnerDto storeOwner)
        {
            try
            {
                await _adminService.CreateStoreOwnerAccount(storeOwner);
                return Ok("Create store owner account successfully!");
            }catch(Exception e)
            {
                return BadRequest(e);
            }
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}"), Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                await _adminService.DeleteUserAccount(id);
                return Ok("Delete user successfully!");
            }catch(Exception e)
            {
                return BadRequest(e);
            }
        }
    }
}

