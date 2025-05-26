using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration.UserSecrets;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.SqlClient;
using System.Net;
using VenusERP_API.Utils;


namespace VenusERP_API.Controllers.Assets
{
    public class Ass_RuningItemsController : BaseApiController
    {
        [HttpGet("GetList")]
        public ActionResult GetList(int pageSize, int pageNumber, string? orderBy = "", string? orderDirection = "", string? criteria = "")
        {
            try
            {
                List<SqlParameter> l = new List<SqlParameter>();
                l.Add(new SqlParameter("@User", GetCurrentUser()));
                l.Add(new SqlParameter("@FormId", ExtractFormId()));
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

                var x = DataHelper.ExecScalar("Ass_RuningItems_ToRun", System.Data.CommandType.StoredProcedure, l.ToArray());
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

        //[HttpGet]
        //[AutherizationAttribute(ActionMapName = "SaveRuningItems")]
        //public ActionResult SaveRuningItems(int AssItemId, DateTime StartDate)
        //{
        //    try
        //    {
        //        List<SqlParameter> l = new List<SqlParameter>();
        //        l.Add(new SqlParameter("@FormId", ExtractFormId()));
        //        l.Add(new SqlParameter("@Lang", ExtractLang()));
        //        l.Add(new SqlParameter("@User", GetCurrentUser()));

        //        l.Add(new SqlParameter("@CompanyId", DataHelper.GetCompanyId()));
        //        l.Add(new SqlParameter("@BranchId", DataHelper.GetBranchId()));
        //        l.Add(new SqlParameter("@AccountId", DataHelper.GetAccountId()));

        //        l.Add(new SqlParameter("@AssItemId", AssItemId));
        //        l.Add(new SqlParameter("@StartDate", StartDate));

        //        List<SqlParameter> listOutParameters = new List<SqlParameter>();
        //        SqlParameter resPar = new SqlParameter() { ParameterName = "@Result", SqlDbType = SqlDbType.NVarChar, Size = -1, Direction = ParameterDirection.Output };
        //        listOutParameters.Add(resPar);
        //        SqlParameter errPar = new SqlParameter() { ParameterName = "@Error", SqlDbType = SqlDbType.NVarChar, Size = -1, Direction = ParameterDirection.Output };
        //        listOutParameters.Add(errPar);

        //        DataHelper.ExecScalar("Ass_RuningItems_Save", System.Data.CommandType.StoredProcedure, ref listOutParameters, l.ToArray());
        //        string error = listOutParameters.Find(X => X.ParameterName == "@Error").Value.ToString();
        //        if (!string.IsNullOrEmpty(error))
        //            return StatusCode(500, error);

        //        string result = listOutParameters.Find(X => X.ParameterName == "@Result").Value.ToString();

        //        if (string.IsNullOrEmpty(result))
        //        {
        //            return StatusCode(500, result);
        //        }


        //        JObject o = JObject.Parse(result);
        //        return StatusCode(200, o);
        //    }
        //    catch (Exception ex)
        //    {

        //        return StatusCode(500, ex.Message);
        //    }


        //    //var X = DataHelper.ExecScalar("Ass_RuningItems_Save", CommandType.StoredProcedure, l.ToArray());
        //    //    if (string.IsNullOrEmpty(X.ToString()))
        //    //    {
        //    //        return StatusCode(200, "");
        //    //    }
        //    //    JObject o = JObject.Parse(X.ToString());
        //    //    return StatusCode(200, o);
        //    //}
        //    //catch(Exception EX)
        //    //{
        //    //    return StatusCode(500, EX.Message);
        //    //}
        //}

        [HttpPost("StartAssetRuning")]
        public ActionResult StartAssetRuning(DataObj dataObj)
        {
            try
            {
                try
                {
                    //DataHelper.Log(DataHelper.GetHeaders(Request));
                    DataHelper.Log(dataObj.dataObj);

                }
                catch (Exception)
                {

                }

                Newtonsoft.Json.Linq.JObject oo = JObject.Parse(dataObj.dataObj);

                JObject newJson = new JObject();
                newJson.Add("Head", oo["head"]);
                newJson.Add("Details", oo["details"]);



                List<SqlParameter> l = new List<SqlParameter>();
                l.Add(new SqlParameter("@User", GetCurrentUser()));
                l.Add(new SqlParameter("@FormId", ExtractFormId()));
                l.Add(new SqlParameter("@Lang", ExtractLang()));
                l.Add(new SqlParameter("@json", newJson.ToString()) { Size = -1 });

                List<SqlParameter> listOutParameters = new List<SqlParameter>();
                SqlParameter errPar = new SqlParameter() { ParameterName = "@Error", SqlDbType = SqlDbType.NVarChar, Size = -1, Direction = ParameterDirection.Output };
                listOutParameters.Add(errPar);

                DataHelper.ExecScalar("Ass_RuningItems_Save_batch", System.Data.CommandType.StoredProcedure, ref listOutParameters, l.ToArray());
                string error = listOutParameters.Find(X => X.ParameterName == "@Error").Value.ToString();
                if (!string.IsNullOrEmpty(error))
                    return StatusCode(500, error);

                return StatusCode(200, "");
            }
            catch (Exception ex)
            {

                return StatusCode(500, ex.Message);
            }


        }

    }
}