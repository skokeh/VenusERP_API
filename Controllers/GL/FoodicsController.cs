using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration.UserSecrets;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Data.SqlClient;
using System.Net;
using System.Net.Http.Headers;
using VenusERP_API.Utils;


namespace VenusERP_API.Controllers.GL
{
    public class FoodicsController : BaseApiController
    {
        private IConfiguration _configuration;
        public FoodicsController(IConfiguration configuration)
        {
            _configuration= configuration;
        }
        [HttpGet("GetList")]
        public ActionResult GetList(int pageSize, int pageNumber, string? orderBy = "", string? orderDirection = "", string? criteria = "")
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

                var x = DataHelper.ExecScalar("sys_FoodicsSettingsGetList", System.Data.CommandType.StoredProcedure, l.ToArray());
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

                DataHelper.ExecScalar("sys_FoodicsSettingDetails", System.Data.CommandType.StoredProcedure, ref listOutPutParams, l.ToArray());
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

        [HttpGet("GetLocationList")]
        public ActionResult GetLocationList()
        {
            try
            {
                List<SqlParameter> l = new List<SqlParameter>();
            
                l.Add(new SqlParameter("@Lang", ExtractLang()));
                List<SqlParameter> listOutParameters = new List<SqlParameter>();
                SqlParameter errPar = new SqlParameter() { ParameterName = "@Error", SqlDbType = SqlDbType.NVarChar, Size = -1, Direction = ParameterDirection.Output };
                listOutParameters.Add(errPar);
                var x = DataHelper.ExecScalar("sys_GetFoodicsBranches", System.Data.CommandType.StoredProcedure,ref listOutParameters, l.ToArray());
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

       

        [HttpGet("GetUnMappedBranches")]
        public ActionResult GetUnMappedBranches()
        {
            try
            {
                List<SqlParameter> l = new List<SqlParameter>();

                l.Add(new SqlParameter("@Lang", ExtractLang()));
                List<SqlParameter> listOutParameters = new List<SqlParameter>();
                SqlParameter errPar = new SqlParameter() { ParameterName = "@Error", SqlDbType = SqlDbType.NVarChar, Size = -1, Direction = ParameterDirection.Output };
                SqlParameter resPar = new SqlParameter() { ParameterName = "@Result", SqlDbType = SqlDbType.NVarChar, Size = -1, Direction = ParameterDirection.Output };
                listOutParameters.Add(resPar);
                listOutParameters.Add(errPar);
               // var x = DataHelper.ExecScalar("sys_Foodics_Branches_Check", System.Data.CommandType.StoredProcedure, ref listOutParameters, l.ToArray());
                DataHelper.ExecScalar("sys_Get_Foodics_UnMappedBranches", System.Data.CommandType.StoredProcedure, ref listOutParameters, l.ToArray());
                string error = listOutParameters.Find(X => X.ParameterName == "@Error").Value.ToString();
                if (!string.IsNullOrEmpty(error))
                    return StatusCode(500, error);


                string result1 = listOutParameters.Find(X => X.ParameterName == "@Result").Value.ToString();
                var result = result1.Split(',');


                if (string.IsNullOrEmpty(result1))
                {
                    return StatusCode(200, result1);
                }


                //JObject o = JObject.Parse(result.ToString());
                return StatusCode(200, result);
            }
            catch (Exception ex)
            {

                return StatusCode(500, ex.Message);
            }


        }

        [HttpGet("GetUnMappedSkus")]
        public ActionResult GetUnMappedSkus()
        {
            try
            {
                List<SqlParameter> l = new List<SqlParameter>();

                l.Add(new SqlParameter("@Lang", ExtractLang()));
                List<SqlParameter> listOutParameters = new List<SqlParameter>();
                SqlParameter errPar = new SqlParameter() { ParameterName = "@Error", SqlDbType = SqlDbType.NVarChar, Size = -1, Direction = ParameterDirection.Output };
                SqlParameter resPar = new SqlParameter() { ParameterName = "@Result", SqlDbType = SqlDbType.NVarChar, Size = -1, Direction = ParameterDirection.Output };
                listOutParameters.Add(resPar);
                listOutParameters.Add(errPar);


                // var x = DataHelper.ExecScalar("sys_Foodics_Branches_Check", System.Data.CommandType.StoredProcedure, ref listOutParameters, l.ToArray());
                DataHelper.ExecScalar("sys_Get_Foodics_UnMappedSKUs", System.Data.CommandType.StoredProcedure, ref listOutParameters, l.ToArray());
                string error = listOutParameters.Find(X => X.ParameterName == "@Error").Value.ToString();
                if (!string.IsNullOrEmpty(error))
                    return StatusCode(500, error);

                string result1 = listOutParameters.Find(X => X.ParameterName == "@Result").Value.ToString();
                var result = result1.Split(',');


                if (string.IsNullOrEmpty(result1))
                {
                    return StatusCode(200, result1);
                }


                //JObject o = JObject.Parse(result.ToString());
                return StatusCode(200, result);
            }
            catch (Exception ex)
            {

                return StatusCode(500, ex.Message);
            }


        }

        [HttpGet("GetItemsList")]
        public ActionResult GetItemsList()
        {
            try
            {
                List<SqlParameter> l = new List<SqlParameter>();

                l.Add(new SqlParameter("@Lang", ExtractLang()));
                List<SqlParameter> listOutParameters = new List<SqlParameter>();
                SqlParameter errPar = new SqlParameter() { ParameterName = "@Error", SqlDbType = SqlDbType.NVarChar, Size = -1, Direction = ParameterDirection.Output };
                listOutParameters.Add(errPar);

                var x = DataHelper.ExecScalar("sys_GetFoodicsSKUs", System.Data.CommandType.StoredProcedure,ref listOutParameters, l.ToArray());
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

        [HttpGet("GetBranchsFromFoodics")]
        public async Task<ActionResult> GetBranchsFromFoodics()
        {
            try
            {

                string baseurl = _configuration.GetSection("Foodics")["FoodicsUrl"];
                string mediaType = _configuration.GetSection("Foodics")["FoodicsMediatype"];
                string Bearer = _configuration.GetSection("Foodics")["FoodicsTokenBearer"];
                string token = _configuration.GetSection("Foodics")["FoodicsToken"];
                HttpClient httpClient = new HttpClient();
                httpClient.BaseAddress = new Uri(baseurl);
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                httpClient.DefaultRequestHeaders.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType));
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(Bearer, token);
                var response = await httpClient.GetStringAsync("products?page=1");
                JObject o = JObject.Parse(response.ToString());
                return StatusCode(200, o);
            }
            catch (Exception ex)
            {

                return StatusCode(500, ex.Message);
            }
        }


        [HttpPost("SaveBranchAndLocations")]
        public ActionResult SaveBranchAndLocations(DataObj dataObj)
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
               // long? formId = Convert.ToInt64(DataHelper.GetDataFromHeaders(Request, "FormId"));

                JObject oo = JObject.Parse(dataObj.dataObj);
                //var parent = JsonConvert.DeserializeObject<JObject>(dataObj.dataObj);
                var headJson = new JObject();
                string pName = "";
                foreach (var item in oo.Properties().Where(x => x.Name.ToLower() != "attachments").ToArray())
                {
                    pName = ((Newtonsoft.Json.Linq.JProperty)item).Name.ToString().ToLower();
                    headJson.Add(item);
                }
                JObject newJson = new JObject();
                newJson.Add("Head", headJson);
                List<SqlParameter> l = new List<SqlParameter>();
                l.Add(new SqlParameter("@User", GetCurrentUser()));
                l.Add(new SqlParameter("@Lang", ExtractLang()));
                l.Add(new SqlParameter("@json", newJson.ToString()) { Size = -1 });

                List<SqlParameter> listOutParameters = new List<SqlParameter>();
                SqlParameter resPar = new SqlParameter() { ParameterName = "@Result", SqlDbType = SqlDbType.NVarChar, Size = -1, Direction = ParameterDirection.Output };
                listOutParameters.Add(resPar);
                SqlParameter errPar = new SqlParameter() { ParameterName = "@Error", SqlDbType = SqlDbType.NVarChar, Size = -1, Direction = ParameterDirection.Output };
                listOutParameters.Add(errPar);

                DataHelper.ExecScalar("sys_FoodicsItemsAndLocationSave", System.Data.CommandType.StoredProcedure, ref listOutParameters, l.ToArray());
                string error = listOutParameters.Find(X => X.ParameterName == "@Error").Value.ToString();
                if (!string.IsNullOrEmpty(error))
                    return StatusCode(500, error);

                string result = listOutParameters.Find(X => X.ParameterName == "@Result").Value.ToString();

                if (string.IsNullOrEmpty(result))
                {
                    return StatusCode(500, result);
                }


                // JObject o = JObject.Parse(result);



                return StatusCode(200, result);
            }
            catch (Exception ex)
            {

                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("GetUnPostedOrdersFromFoodics")]
        public async Task<ActionResult> GetUnPostedOrdersFromFoodics(int pageSize, int pageNumber, string orderBy = "", string orderDirection = "", string criteria = "")
        {
            try
                {
                string fresponse = "";
                #region For Foodics API Calling
                DataSet MrtDataSet = new DataSet();
                MrtDataSet = DataHelper.ExcuteDataSet(@"SELECT MAX(RetrivedDate) AS RetrivedDate
                                                        FROM sys_FoodicsOrders");
                var retriveddate = MrtDataSet.Tables[0].Rows[0]["RetrivedDate"].ToString();
                if(retriveddate == "") retriveddate=DateTime.Now.ToString();
                var onedaybefore = DateTime.Now.AddDays(-1);

                var getdatedifference =  onedaybefore - Convert.ToDateTime(retriveddate) ;
                if (getdatedifference.Days == 0)
                {
                    List<SqlParameter> l = new List<SqlParameter>();
                    l.Add(new SqlParameter("@User", GetCurrentUser()));
                    l.Add(new SqlParameter("@FormId", ExtractFormId()));
                    l.Add(new SqlParameter("@RoutePath", ExtractRoutePath()));
                    l.Add(new SqlParameter("@Lang", ExtractLang()));
                    if (!string.IsNullOrEmpty(orderBy))
                        l.Add(new SqlParameter("@OrderBy", orderBy));
                    if (!string.IsNullOrEmpty(orderDirection))
                        l.Add(new SqlParameter("@OrderDirection", orderDirection));
                    if (!string.IsNullOrEmpty(criteria))
                        l.Add(new SqlParameter("@Criteria", criteria));
                    List<SqlParameter> listOutParameters = new List<SqlParameter>();
                    SqlParameter resPar = new SqlParameter() { ParameterName = "@Result", SqlDbType = SqlDbType.NVarChar, Size = -1, Direction = ParameterDirection.Output };
                    listOutParameters.Add(resPar);
                    SqlParameter errPar = new SqlParameter() { ParameterName = "@Error", SqlDbType = SqlDbType.NVarChar, Size = -1, Direction = ParameterDirection.Output };
                    listOutParameters.Add(errPar);

                    DataHelper.ExecScalar("sys_Foodics_UnpostedOrdersList", System.Data.CommandType.StoredProcedure, ref listOutParameters, l.ToArray());
                    string error = listOutParameters.Find(X => X.ParameterName == "@Error").Value.ToString();
                    if (!string.IsNullOrEmpty(error))
                        return StatusCode(500, error);

                    string result = listOutParameters.Find(X => X.ParameterName == "@Result").Value.ToString();

                    if (string.IsNullOrEmpty(result))
                    {
                        return StatusCode(500, result);
                    }


                    //JObject o = JObject.Parse(result);


                    return StatusCode(200, result);
                
    
            
           
        }
                if (getdatedifference.Days > 1)
                {
                    var date = "";
                    for (var i = 0; i < getdatedifference.Days; i++)
                    {
                       
                        var loopdate = Convert.ToDateTime(date == "" ? retriveddate : date ).AddDays(1);
                         fresponse = await GetOrdersFromFoodics(loopdate);
                        if(fresponse != null)
                        {
                            date = loopdate.ToString();
                        }



                    }
                }
                else
                {

                    fresponse = await GetOrdersFromFoodics(onedaybefore);
                }
               
               
                
             if(fresponse != null)
                {
                List<SqlParameter> l = new List<SqlParameter>();
                    l.Add(new SqlParameter("@User", GetCurrentUser()));
                    l.Add(new SqlParameter("@FormId", ExtractFormId()));
                    l.Add(new SqlParameter("@RoutePath", ExtractRoutePath()));
                    l.Add(new SqlParameter("@Lang", ExtractLang()));
                    l.Add(new SqlParameter("@PageNumber", pageNumber));
                    if (!string.IsNullOrEmpty(orderBy))
                        l.Add(new SqlParameter("@OrderBy", orderBy));
                    if (!string.IsNullOrEmpty(orderDirection))
                        l.Add(new SqlParameter("@OrderDirection", orderDirection));
                    if (!string.IsNullOrEmpty(criteria))
                        l.Add(new SqlParameter("@Criteria", criteria));
                        List<SqlParameter> listOutParameters = new List<SqlParameter>();
                        SqlParameter resPar = new SqlParameter() { ParameterName = "@Result", SqlDbType = SqlDbType.NVarChar, Size = -1, Direction = ParameterDirection.Output };
                        listOutParameters.Add(resPar);
                        SqlParameter errPar = new SqlParameter() { ParameterName = "@Error", SqlDbType = SqlDbType.NVarChar, Size = -1, Direction = ParameterDirection.Output };
                        listOutParameters.Add(errPar);

                    DataHelper.ExecScalar("sys_Foodics_UnpostedOrdersList", System.Data.CommandType.StoredProcedure, ref listOutParameters, l.ToArray());
                string error = listOutParameters.Find(X => X.ParameterName == "@Error").Value.ToString();
                if (!string.IsNullOrEmpty(error))
                    return StatusCode(500, error);

                string result = listOutParameters.Find(X => X.ParameterName == "@Result").Value.ToString();

                if (string.IsNullOrEmpty(result))
                {
                    return StatusCode(500, result);
                }


                //JObject o = JObject.Parse(result);


                return StatusCode(200, result);
                }
 

                return StatusCode(200, "No orders Found");
            }
            catch (Exception ex)
            {

                return StatusCode(500, ex.Message);
            }

            #endregion

        }

        [HttpPost("Save")]
        public ActionResult Save(DataObj dataObj)
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
                    pName = ((JProperty)item).Name.ToString().ToLower();
                    headJson.Add(item);
                }

                JObject newJson = new JObject();
                newJson.Add("Head", headJson);

                List<SqlParameter> l = new List<SqlParameter>();
                l.Add(new SqlParameter("@User", GetCurrentUser()));
                l.Add(new SqlParameter("@FormId", formId));
                l.Add(new SqlParameter("@Lang", ExtractLang()));
                l.Add(new SqlParameter("@json", newJson.ToString()) { Size = -1 });
                List<SqlParameter> listOutParameters = new List<SqlParameter>();
                SqlParameter resPar = new SqlParameter() { ParameterName = "@Result", SqlDbType = SqlDbType.NVarChar, Size = -1, Direction = ParameterDirection.Output };
                listOutParameters.Add(resPar);
                SqlParameter errPar = new SqlParameter() { ParameterName = "@Error", SqlDbType = SqlDbType.NVarChar, Size = -1, Direction = ParameterDirection.Output };
                listOutParameters.Add(errPar);

                DataHelper.ExecScalar("fcs_FoodicsSettingSave", System.Data.CommandType.StoredProcedure, ref listOutParameters, l.ToArray());
                string error = listOutParameters.Find(X => X.ParameterName == "@Error").Value.ToString();
                if (!string.IsNullOrEmpty(error))
                    return StatusCode(500, error);

                string result = listOutParameters.Find(X => X.ParameterName == "@Result").Value.ToString();

                if (string.IsNullOrEmpty(result))
                {
                    return StatusCode(200, result);
                }


                JObject o = JObject.Parse(result);
               


                return StatusCode(200, o);
            }
            catch (Exception ex)
            {

                return StatusCode(500, ex.Message);
            }


        }
        [NonAction]
        public async Task<string> GetOrdersFromFoodics(DateTime date)
        {
            
            var getdate = date.ToString("yyyy-MM-dd HH-mm-ss").Replace(" ", "%");
            string baseurl = _configuration.GetValue<string>("FoodicsUrl");
            string mediaType = _configuration.GetValue<string>("FoodicsMediatype");
            string Bearer = _configuration.GetValue<string>("FoodicsTokenBearer");
            string token = _configuration.GetValue<string>("FoodicsToken");


            HttpClient httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(baseurl);
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            httpClient.DefaultRequestHeaders.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType));
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(Bearer, token);
            var response = await httpClient.GetStringAsync($"orders?include=products.product&include=branch&include=products.taxes&filter[business_date]={getdate}");
            if (response != null)
            {
                List<SqlParameter> la = new List<SqlParameter>();
                la.Add(new SqlParameter("@User", GetCurrentUser()));
                la.Add(new SqlParameter("@RetrivedDate", date));
                la.Add(new SqlParameter("@OrderJson", response));
                DataHelper.ExecScalar("sys_FoodicsSaveOrders", System.Data.CommandType.StoredProcedure, la.ToArray());
            }
            return response;

           
        }

        [HttpGet("GetPostedFoodicsOrders")]
        public async Task<ActionResult> GetPostedFoodicsOrders(int pageSize, int pageNumber, string orderBy = "", string orderDirection = "", string criteria = "")
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
                List<SqlParameter> listOutParameters = new List<SqlParameter>();
                SqlParameter resPar = new SqlParameter() { ParameterName = "@Result", SqlDbType = SqlDbType.NVarChar, Size = -1, Direction = ParameterDirection.Output };
                listOutParameters.Add(resPar);
                SqlParameter errPar = new SqlParameter() { ParameterName = "@Error", SqlDbType = SqlDbType.NVarChar, Size = -1, Direction = ParameterDirection.Output };
                listOutParameters.Add(errPar);

                DataHelper.ExecScalar("sys_Foodics_ReadyPostedOrdersList", System.Data.CommandType.StoredProcedure, ref listOutParameters, l.ToArray());
                string error = listOutParameters.Find(X => X.ParameterName == "@Error").Value.ToString();
                if (!string.IsNullOrEmpty(error))
                    return StatusCode(500, error);

                string result = listOutParameters.Find(X => X.ParameterName == "@Result").Value.ToString();

                if (string.IsNullOrEmpty(result))
                {
                    return StatusCode(200, result);
                }


                //JObject o = JObject.Parse(result);


                return StatusCode(200, result);

            }
            catch (Exception ex)
            {

                return StatusCode(500, ex.Message);
            }

        }


        [HttpPost("PreparingUnpostedFoodicsOrders")]
        public ActionResult PreparingUnpostedFoodicsOrders()
        {
            try
            {
                List<SqlParameter> l = new List<SqlParameter>();
                l.Add(new SqlParameter("@Lang", ExtractLang()));
                List<SqlParameter> listOutParameters = new List<SqlParameter>();
                SqlParameter errPar = new SqlParameter() { ParameterName = "@Error", SqlDbType = SqlDbType.NVarChar, Size = -1, Direction = ParameterDirection.Output };             
                listOutParameters.Add(errPar);

                var x = DataHelper.ExecScalar("sys_Foodics_PrepareOrdersForPosting", System.Data.CommandType.StoredProcedure, ref listOutParameters, l.ToArray());
                if (x == null)
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

        [HttpGet("PreprocessingFoodicsOrders")]
        public ActionResult PreprocessingFoodicsOrders()
        {
            try
            {

                List<SqlParameter> l = new List<SqlParameter>();
                l.Add(new SqlParameter("@Lang", ExtractLang()));

                List<SqlParameter> listOutPutParams = new List<SqlParameter>();
                listOutPutParams.Add(new SqlParameter() { ParameterName = "@Result", SqlDbType = SqlDbType.NVarChar, Size = -1, Direction = ParameterDirection.Output });
                listOutPutParams.Add(new SqlParameter() { ParameterName = "@Error", SqlDbType = SqlDbType.NVarChar, Size = -1, Direction = ParameterDirection.Output });
                DataHelper.ExecScalar("sys_Foodics_ReadyUnpostedOrdersList", System.Data.CommandType.StoredProcedure, ref listOutPutParams, l.ToArray());
                string error = listOutPutParams.Find(X => X.ParameterName == "@Error").Value.ToString();
                if (!string.IsNullOrEmpty(error))
                    return StatusCode(500, error);

                string result = listOutPutParams.Find(X => X.ParameterName == "@Result").Value.ToString();

                if (string.IsNullOrEmpty(result))
                {
                    return StatusCode(200, result);
                }


                 JObject o = JObject.Parse(result);



                return StatusCode(200, o);
            }
            catch (Exception ex)
            {

                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("GetFoodicsPostOrders")]
        public ActionResult GetFoodicsPostOrders()
        {
            try
            {

                List<SqlParameter> l = new List<SqlParameter>();
                l.Add(new SqlParameter("@Lang", ExtractLang()));

                List<SqlParameter> listOutPutParams = new List<SqlParameter>();
                listOutPutParams.Add(new SqlParameter() { ParameterName = "@Result", SqlDbType = SqlDbType.NVarChar, Size = -1, Direction = ParameterDirection.Output });
                listOutPutParams.Add(new SqlParameter() { ParameterName = "@Error", SqlDbType = SqlDbType.NVarChar, Size = -1, Direction = ParameterDirection.Output });
                DataHelper.ExecScalar("sys_Foodics_Post_Orders", System.Data.CommandType.StoredProcedure, ref listOutPutParams, l.ToArray());
                string error = listOutPutParams.Find(X => X.ParameterName == "@Error").Value.ToString();
                if (!string.IsNullOrEmpty(error))
                    return StatusCode(500, error);

                string result = listOutPutParams.Find(X => X.ParameterName == "@Result").Value.ToString();

                if (string.IsNullOrEmpty(result))
                {
                    return StatusCode(500, result);
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
                var result = DataHelper.ExecScalar(@"select CAST( isnull((max(cast(code as bigint))+1),1) AS VARCHAR(20))
                                                       from fcs_BudgetHead WHERE ISNUMERIC(code) = 1"
                                    , System.Data.CommandType.Text);
                return StatusCode(200, result.ToString());
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }


        }

    
    }
}
