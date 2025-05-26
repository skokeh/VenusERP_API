using EInvoiceKSADemo.Helpers.Zatca;
using EInvoiceKSADemo.Helpers.Zatca.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Data;
using System.Data.SqlClient;
using VenusERP_API.Controllers;
using VenusERP_API.Utils;
using EInvoiceKSADemo.Helpers.Zatca.Models;
using System.Text;

namespace API.Controllers.Settings
{
    public class CompaniesController : BaseApiController
    {
        private readonly IZatcaCSIDIssuer _zatcaCSIDIssuer;
        private readonly ICertificateConfiguration _certificateConfiguration;
        private readonly IZatcaCsrGenerator _zatcaCsrGenerator;
        public CompaniesController(IZatcaCSIDIssuer zatcaCSIDIssuer, ICertificateConfiguration certificateConfiguration, IZatcaCsrGenerator zatcaCsrGenerator)
        {
            _zatcaCSIDIssuer = zatcaCSIDIssuer;
            _certificateConfiguration = certificateConfiguration;
            _zatcaCsrGenerator = zatcaCsrGenerator;

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

                var x = DataHelper.ExecScalar("sys_CompaniesGetList", System.Data.CommandType.StoredProcedure, l.ToArray());
                if (string.IsNullOrEmpty(x.ToString()))
                {
                    return StatusCode(200, x);
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

                DataHelper.ExecScalar("sys_CompanyDetails", System.Data.CommandType.StoredProcedure, ref listOutPutParams, l.ToArray());
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

                //return Content(HttpStatusCode.InternalServerError, ex.Message);
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
                    DataHelper.Log(GetAllHeaders());
                    DataHelper.Log(dataObj.dataObj);

                }
                catch (Exception)
                {

                }
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
                var attachments = oo["attachments"].ToArray();

                JObject newJson = new JObject();
                newJson.Add("Head", headJson);

                List<SqlParameter> l = new List<SqlParameter>();
                l.Add(new SqlParameter("@User", GetCurrentUser()));
                l.Add(new SqlParameter("@FormId", ExtractFormId()));
                l.Add(new SqlParameter("@Lang", ExtractLang()));
                l.Add(new SqlParameter("@json", newJson.ToString()) { Size = -1 });

                List<SqlParameter> listOutParameters = new List<SqlParameter>();
                SqlParameter resPar = new SqlParameter() { ParameterName = "@Result", SqlDbType = SqlDbType.NVarChar, Size = -1, Direction = ParameterDirection.Output };
                listOutParameters.Add(resPar);
                SqlParameter errPar = new SqlParameter() { ParameterName = "@Error", SqlDbType = SqlDbType.NVarChar, Size = -1, Direction = ParameterDirection.Output };
                listOutParameters.Add(errPar);

                DataHelper.ExecScalar("sys_CompaniesSave", System.Data.CommandType.StoredProcedure, ref listOutParameters, l.ToArray());
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

        [HttpGet("GetZatcaCSID")]

        public ActionResult GetZatcaCSID()
        {
            try
            {

                List<SqlParameter> listOutPutParams = new List<SqlParameter>();
                SqlParameter resPar = new SqlParameter() { ParameterName = "@Result", SqlDbType = SqlDbType.NVarChar, Size = -1, Direction = ParameterDirection.Output };
                listOutPutParams.Add(resPar);
                DataHelper.ExecScalar("sys_GetCompanyCSID", System.Data.CommandType.StoredProcedure, ref listOutPutParams);
                string result = listOutPutParams[0].Value.ToString();
                if (string.IsNullOrEmpty(result))
                {
                    return StatusCode(500, "");
                }
                JObject o = JObject.Parse(result);
                return StatusCode(200, o);
            }
            catch (Exception ex)
            {

                return StatusCode(500, ex.Message);
            }


        }

        [HttpPost("ZatcaOnBoarding")]
        public async Task<IActionResult> ZatcaOnBoarding(DataObj dataObj)
        {
            try
            {
                JObject oo = JObject.Parse(dataObj.dataObj);
                var result = _zatcaCsrGenerator.GenerateCsr(new InputZatcaCsr
                {
                    VATNumber = oo["vat"].ToString(),
                    IsProduction = oo["isProduction"].ToString() == "true" ? true : false,
                    BranchName = oo["branchName"].ToString(),
                    BusinessCategory = oo["businessCategory"].ToString(),
                    CountryName = "SA",
                    Email = "info@dataocean.com",
                    InvoiceType = oo["invoiceType"].ToString(),
                    SerialNumber = String.Format("1-{0}|2-V1|3-{1}", oo["organisationName"], Guid.NewGuid().ToString()),
                    OrganizationName = oo["organisationName"].ToString(),
                    LocationAddress = oo["address"].ToString(),
                });
                if (result.Success)
                {
                    var cSIDResult = await _zatcaCSIDIssuer.OnboardingCSIDAsync(new InputCSIDOnboardingModel
                    {
                        CSR = Convert.ToBase64String(Encoding.UTF8.GetBytes(result.Csr)),
                        OTP = oo["oTP"].ToString(),
                        Supplier = new Supplier
                        {
                            SellerName = oo["organisationName"].ToString(),
                            SellerTRN = oo["vat"].ToString(),
                            AdditionalStreetAddress = oo["address"].ToString(),
                            BuildingNumber = oo["buildingNo"].ToString(),
                            CityName = oo["city"].ToString(),
                            IdentityNumber = oo["crNumber"].ToString(),
                            IdentityType = "CRN",
                            CountryCode = "SA",
                            DistrictName = oo["district"].ToString(),
                            PostalCode = oo["postalCode"].ToString(),
                            StreetName = oo["streetName"].ToString(),
                        }
                    });
                    if(cSIDResult != null)
                    {
                        var csrdetails = new
                        {
                            certificate = cSIDResult.Certificate,
                            startDate = cSIDResult.StartedDate,
                            endDate = cSIDResult.ExpiredDate,
                            vatNumber = oo["vat"].ToString(),
                            isProduction = oo["isProduction"].ToString() == "true" ? true : false,
                            branchName = oo["branchName"].ToString(),
                            location = oo["address"].ToString(),
                            locationGroupId = oo["locationGroupId"].ToString(),
                            invoiceType = oo["invoiceType"].ToString(),
                            privateKey = result.PrivateKey,
                            secretKey = cSIDResult.Secret


                        };
                        JObject obj = JObject.FromObject(csrdetails);
                        List<SqlParameter> l = new List<SqlParameter>();
                        l.Add(new SqlParameter("@User", GetCurrentUser()));
                        l.Add(new SqlParameter("@json", obj.ToString()) { Size = -1 });
                        List<SqlParameter> listOutParameters = new List<SqlParameter>();
                        SqlParameter resPar = new SqlParameter() { ParameterName = "@Result", SqlDbType = SqlDbType.NVarChar, Size = -1, Direction = ParameterDirection.Output };
                        listOutParameters.Add(resPar);
                        SqlParameter errPar = new SqlParameter() { ParameterName = "@Error", SqlDbType = SqlDbType.NVarChar, Size = -1, Direction = ParameterDirection.Output };
                        listOutParameters.Add(errPar);

                        DataHelper.ExecScalar("sys_ZatcaCertificateSave", System.Data.CommandType.StoredProcedure, ref listOutParameters, l.ToArray());
                        string error = listOutParameters.Find(X => X.ParameterName == "@Error").Value.ToString();
                        if (!string.IsNullOrEmpty(error))
                            return StatusCode(500, error);

                        string spresult = listOutParameters.Find(X => X.ParameterName == "@Result").Value.ToString();

                        if (string.IsNullOrEmpty(spresult))
                        {
                            return StatusCode(500, spresult);
                        }
                        JObject o = JObject.Parse(spresult);
                        return StatusCode(200, o);
                    }
                    else
                    {
                       var error =  ZatcaHttpClient.LastErrorMessage;
                        return StatusCode(500, error);
                    }
                }
                return StatusCode(500, result.ErrorMessage);

            }
            catch(Exception ex)
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
                                                       from sys_Companies WHERE ISNUMERIC(code) = 1"
                                    , System.Data.CommandType.Text);
                //return Content(HttpStatusCode.OK, result.ToString());
                return StatusCode(200, result);
            }
            catch (Exception ex)
            {

                return StatusCode(500, ex);
            }


        }

        [HttpGet("CheckCodeExist")]
        public ActionResult CheckCodeExist(string code)
        {
            try
            {
                List<SqlParameter> l = new List<SqlParameter>();
                l.Add(new SqlParameter("@Code", code));
                var result = DataHelper.ExecScalar(@"select count(*) from sys_Companies WHERE code = @Code", System.Data.CommandType.Text, l.ToArray());
                return StatusCode(200, result.ToString());
            }
            catch (Exception ex)
            {

                return StatusCode(500, ex);
            }


        }

        [HttpPost("Delete")]
        public ActionResult Delete(DataObj dataObj)
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
                l.Add(new SqlParameter("@FormId", ExtractFormId()));
                l.Add(new SqlParameter("@Lang", ExtractLang()));
                l.Add(new SqlParameter("@Id", ((JToken)headJson["ID"]).ToString()));

                List<SqlParameter> listOutParameters = new List<SqlParameter>();
                SqlParameter errPar = new SqlParameter() { ParameterName = "@Error", SqlDbType = SqlDbType.NVarChar, Size = -1, Direction = ParameterDirection.Output };
                listOutParameters.Add(errPar);

                DataHelper.ExecScalar("sys_CompaniesDelete", System.Data.CommandType.StoredProcedure, ref listOutParameters, l.ToArray());
                string error = listOutParameters.Find(X => X.ParameterName == "@Error").Value.ToString();
                if (!string.IsNullOrEmpty(error))
                    return StatusCode(500, error);

                return StatusCode(200, true);
            }
            catch (Exception ex)
            {

                return StatusCode(500, ex);
            }


        }


        
    }
}
