using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using VenusERP_API.Models;

namespace VenusERP_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BaseApiController : ControllerBase
    {
        protected string GetAllHeaders()
        {
            var headers = HttpContext.Request.Headers;

            // Convert headers to a string
            string headersAsString = string.Join(Environment.NewLine, headers.Select(h => $"{h.Key}: {h.Value}"));

            // Return the headers as a string
            return headersAsString;
        }
        protected int? ExtractFormId()
        {
            const string FormId = "FormId";
            if(Request.Headers.TryGetValue(FormId, out var formid) && formid.Count >0 && int.TryParse(formid[0], out var id))
            {
                return id;
            }
            return null;
        }

        protected int? ExtractUserId()
        {
            var userIdClaim = HttpContext?.User?.FindFirst("Id");
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
            {
                return userId;
            }
            return null;
        }

        protected int? ExtractReportId()
        {
            const string ReportId = "ReportId";
            if (Request.Headers.TryGetValue(ReportId, out var reportid) && reportid.Count > 0 && int.TryParse(reportid[0], out var id))
            {
                return id;
            }
            return null;
            //Request.Headers.TryGetValue(ReportId, out var reportId);
            //return int.Parse(reportId);
        }

        protected string ExtractRoutePath()
        {
            const string RoutePath = "RoutePath";
            Request.Headers.TryGetValue(RoutePath, out var path);
            return path;
        }
        protected string ExtractLang()
        {
            const string lang = "lang";
            Request.Headers.TryGetValue(lang, out var l);
            return l;
        }

        protected string GetCurrentUser()
        {
            ClaimsIdentity? identity = HttpContext.User.Identity as ClaimsIdentity;
            var UserCode = "";
            if (identity != null)
            {
                var userClaims = identity.Claims;
                //return new UserModel
                //{
                //    Code = userClaims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value,
                //    EngName = userClaims.FirstOrDefault(x => x.Type == "UserName")?.Value
                //};
                return UserCode = userClaims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
            }
            return null;
        }
    }
}
