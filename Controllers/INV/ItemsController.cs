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


namespace VenusERP_API.Controllers.INV
{
    public class ItemsController : BaseApiController
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

                var x = DataHelper.ExecScalar("mms_ItemsGetList", System.Data.CommandType.StoredProcedure, l.ToArray());
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

     
        [HttpGet("GetByID")]
        public ActionResult GetByID(int ID)
        {
            try
            {

                List<SqlParameter> l = new List<SqlParameter>();
                l.Add(new SqlParameter("@User", GetCurrentUser()));
                l.Add(new SqlParameter("@FormId", ExtractFormId()));
                l.Add(new SqlParameter("@Lang", ExtractLang()));
                l.Add(new SqlParameter("@ID", ID));
                List<SqlParameter> listOutPutParams = new List<SqlParameter>();
                listOutPutParams.Add(new SqlParameter() { ParameterName = "@Result", SqlDbType = SqlDbType.NVarChar, Size = -1, Direction = ParameterDirection.Output });

                DataHelper.ExecScalar("mms_ItemDetails", System.Data.CommandType.StoredProcedure, ref listOutPutParams, l.ToArray());
                string result = listOutPutParams[0].Value.ToString();
                if (string.IsNullOrEmpty(result.ToString()))
                {
                    return StatusCode(200, "");
                }
                JObject o = JObject.Parse(result);
                return StatusCode(200, o);
            }
            catch (Exception ex)
            {

                return StatusCode(500, ex.Message);
            }


        }

        [HttpGet("GetNextCode")]
        public ActionResult GetNextCode()
        {
            try
            {
                List<SqlParameter> l = new List<SqlParameter>();
                var result = DataHelper.ExecScalar(@"select CAST( isnull((max(cast(code as decimal))+1),1) AS VARCHAR(20))
                                                       from mms_Items WHERE ISNUMERIC(code) = 1"
                                    , System.Data.CommandType.Text);
                return StatusCode(200, result.ToString());
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }


        }

        [HttpGet("CheckCodeExist")]
        public ActionResult CheckCodeExist(string code)
        {
            try
            {
                List<SqlParameter> l = new List<SqlParameter>();
                l.Add(new SqlParameter("@Code", code));
                var result = DataHelper.ExecScalar(@"select count(*) from mms_Items WHERE code = @Code"
                                    , System.Data.CommandType.Text, l.ToArray());
                return StatusCode(200, result.ToString());
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }


        }

        [HttpPost("Save")]
        public ActionResult Save(DataObj dataObj)
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
                //long? formId = Convert.ToInt64(DataHelper.GetDataFromHeaders(Request, "FormId"));
                long? formId = ExtractFormId();

                JObject oo = JObject.Parse(dataObj.dataObj);
                //var parent = JsonConvert.DeserializeObject<JObject>(dataObj.dataObj);
                var headJson = new JObject();
                string pName = "";
                foreach (var item in oo.Properties().Where(x => x.Name.ToLower() != "attachments" 
                && x.Name.ToLower() != "itemUnits" && x.Name.ToLower() != "itemPricesLevels"
                && x.Name.ToLower() != "itemKits" && x.Name.ToLower() != "itemLocations"
                && x.Name.ToLower() != "itemExpenses" && x.Name.ToLower() != "vendorsList"
                && x.Name.ToLower() != "imageData"
                ).ToArray())
                {
                    pName = ((Newtonsoft.Json.Linq.JProperty)item).Name.ToString().ToLower();
                    headJson.Add(item);
                }
                var itemUnits = oo["itemUnits"]; 
                var itemPricesLevels = oo["itemPricesLevels"]; 
                var itemKits = oo["itemKits"];
                var itemLocations = oo["itemLocations"];
                var itemExpenses = oo["itemExpenses"];
                var vendorsList = oo["vendorsList"];
                var attachments = oo["attachments"].ToArray();
                var imageData = oo["imageData"].ToString();

                JObject newJson = new JObject();
                newJson.Add("Head", headJson);
                newJson.Add("itemUnits", itemUnits);
                newJson.Add("itemPricesLevels", itemPricesLevels);
                newJson.Add("itemKits", itemKits);
                newJson.Add("itemLocations", itemLocations);
                newJson.Add("itemExpenses", itemExpenses);
                newJson.Add("itemVendors", vendorsList);
                List<SqlParameter> l = new List<SqlParameter>();
                l.Add(new SqlParameter("@User", GetCurrentUser()));
                l.Add(new SqlParameter("@FormId", formId));
                l.Add(new SqlParameter("@Lang", ExtractLang()));
                l.Add(new SqlParameter("@json", newJson.ToString()) { Size = -1 });
                l.Add(new SqlParameter("@DefualtImage", imageData));

                List<SqlParameter> listOutParameters = new List<SqlParameter>();
                SqlParameter resPar = new SqlParameter() { ParameterName = "@Result", SqlDbType = SqlDbType.NVarChar, Size = -1, Direction = ParameterDirection.Output };
                listOutParameters.Add(resPar);
                SqlParameter errPar = new SqlParameter() { ParameterName = "@Error", SqlDbType = SqlDbType.NVarChar, Size = -1, Direction = ParameterDirection.Output };
                listOutParameters.Add(errPar);

                DataHelper.ExecScalar("mms_ItemsSave", System.Data.CommandType.StoredProcedure, ref listOutParameters, l.ToArray());
                string error = listOutParameters.Find(X => X.ParameterName == "@Error").Value.ToString();
                if (!string.IsNullOrEmpty(error))
                    return StatusCode(500, error);

                string result = listOutParameters.Find(X => X.ParameterName == "@Result").Value.ToString();

                if (string.IsNullOrEmpty(result))
                {
                    return StatusCode(500, result);
                }


                JObject o = JObject.Parse(result);
                #region //Save Attachments if record has been saved
                try
                {
                    HelperUtilitis.saveAttachments(formId, Convert.ToInt64(o["ID"]), attachments);
                }
                catch (Exception ex)
                {
                    return StatusCode(500, ex.Message);
                }
                #endregion


                return StatusCode(200, o);
            }
            catch (Exception ex)
            {

                return StatusCode(500, ex.Message);
            }


        }

        [HttpPost("Delete")]
        public ActionResult Delete(DataObj dataObj)
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
                //long? formId = Convert.ToInt64(DataHelper.GetDataFromHeaders(Request, "FormId"));
                long? formId = ExtractFormId();

                JObject oo = JObject.Parse(dataObj.dataObj);
                var headJson = new JObject();
                string pName = "";
                foreach (var item in oo.Properties().Where(x => x.Name.ToLower() != "attachments").ToArray())
                {
                    pName = ((Newtonsoft.Json.Linq.JProperty)item).Name.ToString().ToLower();
                    headJson.Add(item);
                }
                var attachments = oo["attachments"].ToArray();

                JObject newJson = new JObject();
                newJson.Add("Head", headJson);
                List<SqlParameter> l = new List<SqlParameter>();
                l.Add(new SqlParameter("@User", GetCurrentUser()));
                l.Add(new SqlParameter("@FormId", formId));
                l.Add(new SqlParameter("@Lang", ExtractLang()));
                l.Add(new SqlParameter("@Id", ((JToken)headJson["ID"]).ToString()));

                List<SqlParameter> listOutParameters = new List<SqlParameter>();
                SqlParameter errPar = new SqlParameter() { ParameterName = "@Error", SqlDbType = SqlDbType.NVarChar, Size = -1, Direction = ParameterDirection.Output };
                listOutParameters.Add(errPar);

                DataHelper.ExecScalar("mms_ItemsDelete", System.Data.CommandType.StoredProcedure, ref listOutParameters, l.ToArray());
                string error = listOutParameters.Find(X => X.ParameterName == "@Error").Value.ToString();
                if (!string.IsNullOrEmpty(error))
                    return StatusCode(500, error);

                return StatusCode(200, true);
            }
            catch (Exception ex)
            {

                return StatusCode(500, ex.Message);
            }


        }
        
        [HttpGet("GetLookups")]
        public ActionResult GetLookups()
        {
            try
            {
                List<SqlParameter> l = new List<SqlParameter>();
                l.Add(new SqlParameter("@User", GetCurrentUser()));
                l.Add(new SqlParameter("@FormId", ExtractFormId()));
                l.Add(new SqlParameter("@RoutePath", ExtractRoutePath()));
                l.Add(new SqlParameter("@Lang", ExtractLang()));
                var x = DataHelper.ExecScalar("mms_ItemsGetLookups", System.Data.CommandType.StoredProcedure, l.ToArray());
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

        [HttpPost("UpdateSFDAItemsPriceLevelFromExcel")]
        public ActionResult UpdateSFDAItemsPriceLevelFromExcel(DataObj dataObj)
        {
            try
            {
                try
                {
                    DataHelper.Log(GetAllHeaders());
                    DataHelper.Log(dataObj.dataObj);

                }
                catch (Exception)
                {

                }
                //long? formId = Convert.ToInt64(DataHelper.GetDataFromHeaders(Request, "FormId"));
                long? formId = ExtractFormId();

                JObject oo = JObject.Parse(dataObj.dataObj);
                //var parent = JsonConvert.DeserializeObject<JObject>(dataObj.dataObj);
                var headJson = new JObject();
                string pName = "";
                foreach (var item in oo.Properties().Where(x => x.Name.ToLower() != "attachments").ToArray())
                {
                    pName = ((Newtonsoft.Json.Linq.JProperty)item).Name.ToString().ToLower();
                    headJson.Add(item);
                }
              //  var attachments = oo["attachments"].ToArray();

                JObject newJson = new JObject();
                newJson.Add("Head", headJson);
                List<SqlParameter> l = new List<SqlParameter>();
                l.Add(new SqlParameter("@User", GetCurrentUser()));
                //l.Add(new SqlParameter("@FormId", formId));
                l.Add(new SqlParameter("@Lang", ExtractLang()));
                l.Add(new SqlParameter("@json", newJson.ToString()) { Size = -1 });

                List<SqlParameter> listOutParameters = new List<SqlParameter>();
                SqlParameter resPar = new SqlParameter() { ParameterName = "@Result", SqlDbType = SqlDbType.NVarChar, Size = -1, Direction = ParameterDirection.Output };
                listOutParameters.Add(resPar);
                SqlParameter errPar = new SqlParameter() { ParameterName = "@Error", SqlDbType = SqlDbType.NVarChar, Size = -1, Direction = ParameterDirection.Output };
                listOutParameters.Add(errPar);

                DataHelper.ExecScalar("mms_UpdateSFDAItemsPriceLevelFromExcel", System.Data.CommandType.StoredProcedure, ref listOutParameters, l.ToArray());
                string error = listOutParameters.Find(X => X.ParameterName == "@Error").Value.ToString();
                if (!string.IsNullOrEmpty(error))
                    return StatusCode(500, error);

                //string result = listOutParameters.Find(X => X.ParameterName == "@Result").Value.ToString();
                string result = "Save successfully";
                if (string.IsNullOrEmpty(result))
                {
                    return StatusCode(500, result);
                }


               // JObject o = JObject.Parse(result);
                #region //Save Attachments if record has been saved
                //try
                //{
                //    HelperUtilitis.saveAttachments(formId, Convert.ToInt64(o["ID"]), attachments);
                //}
                //catch (Exception ex)
                //{
                //    return StatusCode(500, ex.Message);
                //}
                #endregion


                return StatusCode(200);
            }
            catch (Exception ex)
            {

                return StatusCode(500, ex.Message);
            }


        }



        [HttpGet("srchItems")]
        public ActionResult srchItems(String ExciptIds = "", string Filter = "")
        {
            try
            {
                List<SqlParameter> l = new List<SqlParameter>();
                l.Add(new SqlParameter("@User", GetCurrentUser()));
                l.Add(new SqlParameter("@FormId", ExtractFormId()));
                l.Add(new SqlParameter("@Lang", ExtractLang()));
                if (!string.IsNullOrEmpty(ExciptIds))
                    l.Add(new SqlParameter("@ExciptIds", ExciptIds));
                if (!string.IsNullOrEmpty(Filter))
                    l.Add(new SqlParameter("@Filter", Filter));

                var x = DataHelper.ExecScalar("mms_srchItems", System.Data.CommandType.StoredProcedure, l.ToArray());
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

        [HttpGet("GetItemCosts")]
        public ActionResult GetItemCosts(int ItemId)
        {
            try
            {
                List<SqlParameter> l = new List<SqlParameter>();
                l.Add(new SqlParameter("@User", GetCurrentUser()));
                l.Add(new SqlParameter("@FormId", ExtractFormId()));
                l.Add(new SqlParameter("@Lang", ExtractLang()));
                l.Add(new SqlParameter("@ItemId", ItemId));

                var x = DataHelper.ExecScalar("mms_GetItemCosts", System.Data.CommandType.StoredProcedure, l.ToArray());
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

        [HttpGet("GetItemLastPurchases")]
        public ActionResult GetItemLastPurchases(int ItemId,int? VendorId=null)
        {
            try
            {

                List<SqlParameter> l = new List<SqlParameter>();
                l.Add(new SqlParameter("@User", GetCurrentUser()));
                l.Add(new SqlParameter("@FormId", ExtractFormId()));
                l.Add(new SqlParameter("@Lang", ExtractLang()));
                l.Add(new SqlParameter("@ItemId", ItemId));
                if(VendorId !=null)
                l.Add(new SqlParameter("@VendorId", VendorId));

                var x = DataHelper.ExecScalar("mms_GetItemLastPurchases", System.Data.CommandType.StoredProcedure, l.ToArray());
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

        [HttpGet("GetItemLastSales")]
        public ActionResult GetItemLastSales(int ItemId, int? CustomerId = null)
        {
            try
            {

                List<SqlParameter> l = new List<SqlParameter>();
                l.Add(new SqlParameter("@User", GetCurrentUser()));
                l.Add(new SqlParameter("@FormId", ExtractFormId()));
                l.Add(new SqlParameter("@Lang", ExtractLang()));
                l.Add(new SqlParameter("@ItemId", ItemId));
                if (CustomerId != null)
                    l.Add(new SqlParameter("@CustomerId", CustomerId));

                var x = DataHelper.ExecScalar("mms_GetItemLastSales", System.Data.CommandType.StoredProcedure, l.ToArray());
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

        [HttpGet("GetItemInOut")]
        public ActionResult GetItemInOut(int ItemId)
        {
            try
            {

                List<SqlParameter> l = new List<SqlParameter>();
                l.Add(new SqlParameter("@User", GetCurrentUser()));
                l.Add(new SqlParameter("@FormId", ExtractFormId()));
                l.Add(new SqlParameter("@Lang", ExtractLang()));
                l.Add(new SqlParameter("@ItemId", ItemId));

                var x = DataHelper.ExecScalar("mms_GetItemLastInOut", System.Data.CommandType.StoredProcedure, l.ToArray());
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

        [HttpGet("GetItemBalance")]
        public ActionResult GetItemBalance(int ItemId)
        {
            try
            {

                List<SqlParameter> l = new List<SqlParameter>();
                l.Add(new SqlParameter("@User", GetCurrentUser()));
                l.Add(new SqlParameter("@FormId", ExtractFormId()));
                l.Add(new SqlParameter("@Lang", ExtractLang()));
                l.Add(new SqlParameter("@ItemId", ItemId));

                var x = DataHelper.ExecScalar("mms_GetItemBalance", System.Data.CommandType.StoredProcedure, l.ToArray());
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
