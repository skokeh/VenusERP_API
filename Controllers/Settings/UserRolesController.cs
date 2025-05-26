using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Data;
using System.Data.SqlClient;
using VenusERP_API.Utils;
using VenusERP_Application.Settings;

namespace VenusERP_API.Controllers.Settings
{

    public class UserRolesController : BaseApiController
    {
        private readonly UsersService _usersService;
        public UserRolesController(UsersService usersService)
        {
            _usersService = usersService;
        }

        [HttpGet("GetList")]
        public async Task<ActionResult> GetList(int pageSize, int pageNumber, string? orderBy = "", string? orderDirection = "", string? criteria = "")
        {
            try
            {
                var result = await _usersService.GetRoleList(ExtractFormId(), GetCurrentUser(), ExtractRoutePath(), ExtractLang(), pageSize, pageNumber, orderBy, orderDirection, criteria);

                if (result.StartsWith("Error:"))
                {
                    return StatusCode(500, result);
                }

                return StatusCode(200, result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("GetNextCode")]
        public async Task<ActionResult> GetNextCode()
        {
            try
            {
                var result = await _usersService.GetNextCode();
                if (result != 0)
                {
                    return StatusCode(200, result);
                }
                return StatusCode(500, result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        
    }
}
