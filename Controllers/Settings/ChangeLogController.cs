using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Data.SqlClient;
using VenusERP_API.Utils;

namespace VenusERP_API.Controllers.Settings
{

    public class ChangeLogController : BaseApiController
    {
        [HttpGet("GetList")]
        public ActionResult GetList(int? pageSize, int? pageNumber, string? orderBy = "", string? orderDirection = "", string? criteria = "")
        {
            try
            {
                List<SqlParameter> l = new List<SqlParameter>();
                l.Add(new SqlParameter("@FormId", ExtractFormId()));
                l.Add(new SqlParameter("@User", GetCurrentUser()));
                l.Add(new SqlParameter("@RoutePath", ExtractRoutePath()));
                l.Add(new SqlParameter("@Lang", ExtractLang()));
                l.Add(new SqlParameter("@PageSize", pageSize));
                l.Add(new SqlParameter("@PageNumber", pageNumber));
                if (!string.IsNullOrEmpty(orderBy))
                    l.Add(new SqlParameter("@OrderBy", orderBy));
                if (!string.IsNullOrEmpty(orderDirection))
                    l.Add(new SqlParameter("@OrderDirection", orderDirection));
                if (!string.IsNullOrEmpty(criteria))
                    l.Add(new SqlParameter("@Criteria", criteria));

                var x = DataHelper.ExecScalar("sys_ChangeLogsGetList", System.Data.CommandType.StoredProcedure, l.ToArray());
                if (string.IsNullOrEmpty(x.ToString()))
                {
                    return StatusCode(200, "");
                }
                JObject o = JObject.Parse(x.ToString());
                return StatusCode(200, o);
            }
            catch (Exception ex)
            {

                return StatusCode(500, ex.Message);
            }


        }

    }
}
