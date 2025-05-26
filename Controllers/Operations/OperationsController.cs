using EInvoiceKSADemo.Helpers.Zatca;
using EInvoiceKSADemo.Helpers.Zatca.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Data;
using System.Data.SqlClient;
using VenusERP_API.Utils;
using Microsoft.AspNetCore.SignalR;
using VenusERP_API.RealTimeHub;
using PharmacyIntegrationTrack.Services;
using PharmacyIntegrationTrack.Services.Models;
using VenusERP_Application.Settings;
using VenusERP_Application.Dtos.GeneralDto;
using EInvoiceKSADemo.Helpers.Zatca.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace VenusERP_API.Controllers.Operations
{
    public class OperationsController : BaseApiController
    {
        private IConfiguration _configuration;
        private readonly ICertificateConfiguration _certificateConfiguration;
        private RSD _rsd;
        private readonly IZatcaReporter _reporter;
        private readonly IInvoiceInfoGenerator _invoiceInfoGenerator;
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly LocationService _locationService;
        private readonly GeneralService _generalService;

        public OperationsController(IHubContext<NotificationHub> hubContext,
            IConfiguration configuration,
            RSD rsd,
            IZatcaReporter reporter,
            ICertificateConfiguration certificateConfiguration,
            LocationService locationService,
            GeneralService generalService,
            IInvoiceInfoGenerator invoiceInfoGenerator
            )
        {
            _configuration = configuration;
            _rsd = rsd;
            _reporter = reporter;
            _certificateConfiguration = certificateConfiguration;
            _hubContext = hubContext;
            _locationService = locationService;
            _generalService = generalService;
            _invoiceInfoGenerator = invoiceInfoGenerator;
        }



        [HttpGet("GetOperationTypes")]
        public ActionResult GetOperationTypes()
        {
            try
            {

                List<SqlParameter> l = new List<SqlParameter>();
                l.Add(new SqlParameter("@User", GetCurrentUser()));
                l.Add(new SqlParameter("@Lang", ExtractLang()));

                var x = DataHelper.ExecScalar("mms_GetOperationTypes", System.Data.CommandType.StoredProcedure, l.ToArray());
                JObject o = JObject.Parse(x.ToString());
                return StatusCode(200, o);
            }
            catch (Exception ex)
            {

                return StatusCode(500, ex.Message);
            }


        }

        [HttpGet("GetOperationStatuses")]
        public ActionResult GetOperationStatuses(string? OperationHeadId = "")
        {
            try
            {

                List<SqlParameter> l = new List<SqlParameter>();
                l.Add(new SqlParameter("@User", GetCurrentUser()));
                l.Add(new SqlParameter("@FormId", ExtractFormId()));
                l.Add(new SqlParameter("@Lang", ExtractLang()));
                if (!string.IsNullOrEmpty(OperationHeadId))
                    l.Add(new SqlParameter("@OperationHeadId", OperationHeadId));

                var x = DataHelper.ExecScalar("mms_GetOperationStatuses", System.Data.CommandType.StoredProcedure, l.ToArray());
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

                var x = DataHelper.ExecScalar("mms_OperationsList", System.Data.CommandType.StoredProcedure, l.ToArray());
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



        [HttpGet("GetListRepostOperations/GetList")]
        public ActionResult GetListRepostOperations(int pageSize, int pageNumber, string? orderBy = "", string? orderDirection = "", string? criteria = "")
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

                var x = DataHelper.ExecScalar("mms_RepostOperationsToGLList", System.Data.CommandType.StoredProcedure, l.ToArray());
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


        [HttpGet("GetApprovalList/GetList")]
        public ActionResult GetApprovalList(int pageSize, int pageNumber, string? orderBy = "", string? orderDirection = "", string? criteria = "")
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

                var x = DataHelper.ExecScalar("mms_BulkOperationsApprovalList", System.Data.CommandType.StoredProcedure, l.ToArray());
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

        [HttpGet("GetZatcaRepostingList/GetList")]
        public ActionResult GetZatcaRepostingList(int pageSize, int pageNumber, string? orderBy = "", string? orderDirection = "", string? criteria = "")
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

                var x = DataHelper.ExecScalar("mms_BulkZatcaOperationsList", System.Data.CommandType.StoredProcedure, l.ToArray());
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

        [HttpGet("GetRSDRepostingList/GetList")]
        public ActionResult GetRSDRepostingList(int pageSize, int pageNumber, string? orderBy = "", string? orderDirection = "", string? criteria = "")
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

                var x = DataHelper.ExecScalar("mms_BulkRSDOperationsList", System.Data.CommandType.StoredProcedure, l.ToArray());
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

        [HttpGet("GetOrderItems")]
        public ActionResult GetOrderItems(string? criteria = "")
        {
            try
            {
                List<SqlParameter> l = new List<SqlParameter>();
                l.Add(new SqlParameter("@User", GetCurrentUser()));
                l.Add(new SqlParameter("@FormId", ExtractFormId()));
                l.Add(new SqlParameter("@Lang", ExtractLang()));
                if (!string.IsNullOrEmpty(criteria))
                    l.Add(new SqlParameter("@Criteria", criteria));

                //var x = DataHelper.ExecScalar("mms_GetOrderItems", System.Data.CommandType.StoredProcedure, l.ToArray());
                JObject newJson = new JObject();
                var d = DataHelper.ExcuteDataTable("mms_GetOrderItems", System.Data.CommandType.StoredProcedure, l.ToArray());
                string JSONString = string.Empty;
                //JSONString = JsonConvert.SerializeObject(d);
                JObject o = JObject.Parse(d.ToString());
                //return StatusCode(200, o);

                if (d != null)
                {
                    newJson.Add("list", o);
                }
                return StatusCode(200, newJson);

            }
            catch (Exception ex)
            {

                return StatusCode(500, ex.Message);
            }


        }

        [HttpGet("GetOperationAllSequences")]
        public ActionResult GetOperationAllSequences(string OperationTypeId, string HeadId)
        {
            try
            {
                // Input parameters
                List<SqlParameter> l = new List<SqlParameter>
                    {
                        new SqlParameter("@User", GetCurrentUser()),
                        new SqlParameter("@Lang", ExtractLang()),
                        new SqlParameter("@ID", OperationTypeId),
                        new SqlParameter("@HeadID", HeadId)
                    };

                            // Output parameter
                            List<SqlParameter> listOutPutParams = new List<SqlParameter>
                    {
                        new SqlParameter
                        {
                            ParameterName = "@Result",
                            SqlDbType = SqlDbType.NVarChar,
                            Size = -1,
                            Direction = ParameterDirection.Output
                        }
                    };

                // Execute the stored procedure
                DataHelper.ExecScalar(
                    "mms_GetOperationAllSequence",
                    CommandType.StoredProcedure,
                    ref listOutPutParams,
                    l.ToArray());

                string result = listOutPutParams[0].Value.ToString();

                if (string.IsNullOrEmpty(result))
                {
                    return StatusCode(204); 
                }

                JArray parsedResult = JArray.Parse(result);

                return Ok(parsedResult); 
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message); // Internal Server Error
            }
        }

        [HttpGet("GetByID")]
        public ActionResult GetByID(string ID, bool IsRetrieve = false)
        {
            try
            {

                List<SqlParameter> l = new List<SqlParameter>();
                l.Add(new SqlParameter("@User", GetCurrentUser()));
                l.Add(new SqlParameter("@FormId", ExtractFormId()));
                l.Add(new SqlParameter("@Lang", ExtractLang()));
                l.Add(new SqlParameter("@ID", ID));
                l.Add(new SqlParameter("@IsRetrieve", IsRetrieve));
                List<SqlParameter> listOutPutParams = new List<SqlParameter>();
                listOutPutParams.Add(new SqlParameter() { ParameterName = "@Result", SqlDbType = SqlDbType.NVarChar, Size = -1, Direction = ParameterDirection.Output });

                DataHelper.ExecScalar("mms_OperationDetails", System.Data.CommandType.StoredProcedure, ref listOutPutParams, l.ToArray());
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

        [HttpGet("GetOperationDetailsForZatca")]
        public ActionResult GetOperationDetailsForZatca(string ID)
        {
            try
            {

                List<SqlParameter> l = new List<SqlParameter>();
                l.Add(new SqlParameter("@Lang", ExtractLang()));
                l.Add(new SqlParameter("@ID", ID));
                List<SqlParameter> listOutPutParams = new List<SqlParameter>();
                listOutPutParams.Add(new SqlParameter() { ParameterName = "@Result", SqlDbType = SqlDbType.NVarChar, Size = -1, Direction = ParameterDirection.Output });

                DataHelper.ExecScalar("mms_OperationDetailsForZatca", System.Data.CommandType.StoredProcedure, ref listOutPutParams, l.ToArray());
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
        [HttpGet("GetOperationDetailsForRSD")]
        public ActionResult GetOperationDetailsForZatcaRSD(string ID)
        {
            try
            {

                List<SqlParameter> l = new List<SqlParameter>();
                l.Add(new SqlParameter("@Lang", ExtractLang()));
                l.Add(new SqlParameter("@ID", ID));
                List<SqlParameter> listOutPutParams = new List<SqlParameter>();
                listOutPutParams.Add(new SqlParameter() { ParameterName = "@Result", SqlDbType = SqlDbType.NVarChar, Size = -1, Direction = ParameterDirection.Output });

                DataHelper.ExecScalar("mms_OperationDetailsForRSD", System.Data.CommandType.StoredProcedure, ref listOutPutParams, l.ToArray());
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

        [HttpGet("PosPrintOperation")]
        public ActionResult PosPrintOperation(string ID)
        {
            try

            {

                List<SqlParameter> l = new List<SqlParameter>();
                l.Add(new SqlParameter("@User", GetCurrentUser()));
                l.Add(new SqlParameter("@FormId", ExtractFormId()));
                l.Add(new SqlParameter("@Lang", ExtractLang()));
                l.Add(new SqlParameter("@ID", ID));
                var x = DataHelper.ExecScalar("mms_PosPrintOperation", System.Data.CommandType.StoredProcedure, l.ToArray());
                if (string.IsNullOrEmpty(x.ToString()))
                {
                    return StatusCode(200, "");
                }
                JObject o = JObject.Parse(x.ToString());
                return StatusCode(200, o);
            }
            catch (Exception ex)
            {
                //DataHelper.Log(DataHelper.GetHeaders(Request));
                DataHelper.Log(ex.Message);
                return StatusCode(500, ex.Message);
            }


        }

        [HttpGet("srchItemsList")]
        public ActionResult srchItemsList(int LocationId, string? ExciptIds = "", string? Filter = "", bool? IsItemIdentifierID = false, int? ItemId = null, int? ItemIdentifierID = null, int? CustomerID = null, int? VendorID = null, int? OperationTypeID = null)
        {
            try
            {
                List<SqlParameter> l = new List<SqlParameter>();
                l.Add(new SqlParameter("@LocationId", LocationId));
                l.Add(new SqlParameter("@User", GetCurrentUser()));
                l.Add(new SqlParameter("@FormId", ExtractFormId()));
                l.Add(new SqlParameter("@Lang", ExtractLang()));
                if (!string.IsNullOrEmpty(ExciptIds))
                    l.Add(new SqlParameter("@ExciptIds", ExciptIds));
                if (!string.IsNullOrEmpty(Filter))
                    l.Add(new SqlParameter("@Filter", Filter));
                if (IsItemIdentifierID != null)
                    l.Add(new SqlParameter("@IsItemIdentifierID", IsItemIdentifierID));
                if (ItemId != 0)
                    l.Add(new SqlParameter("@ItemId", ItemId));
                if (ItemIdentifierID != 0)
                    l.Add(new SqlParameter("@ItemIdentifierID", ItemIdentifierID));
                if (CustomerID != null)
                    l.Add(new SqlParameter("@CustomerID ", CustomerID));
                if (VendorID != 0)
                    l.Add(new SqlParameter("@VendorID ", VendorID));
                if (OperationTypeID != null)
                    l.Add(new SqlParameter("@OperationTypeID ", OperationTypeID));

                var x = DataHelper.ExecScalar("mms_srchItemsListNew", System.Data.CommandType.StoredProcedure, l.ToArray());
                if (string.IsNullOrEmpty(x.ToString()))
                {
                    return StatusCode(200, "");
                }
                JObject o = JObject.Parse(x.ToString());
                return StatusCode(200, o);
            }
            catch (Exception ex)
            {
                //DataHelper.Log(DataHelper.GetHeaders(Request));
                DataHelper.Log("srchItemsList()");
                DataHelper.Log(ex.Message);
                return StatusCode(500, ex.Message);
            }


        }

        [HttpGet("GetItemForScannedBarcode")]
        public ActionResult GetItemForScannedBarcode(int LocationId, string? Filter)
        {
            try
            {
                List<SqlParameter> l = new List<SqlParameter>();
                l.Add(new SqlParameter("@LocationId", LocationId));
                l.Add(new SqlParameter("@User", GetCurrentUser()));
                l.Add(new SqlParameter("@FormId", ExtractFormId()));
                l.Add(new SqlParameter("@Lang", ExtractLang()));
                if (!string.IsNullOrEmpty(Filter))
                    l.Add(new SqlParameter("@Filter", Filter));

                var x = DataHelper.ExecScalar("mms_srchItemsListNewForBarecodeReader", System.Data.CommandType.StoredProcedure, l.ToArray());
                if (string.IsNullOrEmpty(x.ToString()))
                {
                    return StatusCode(200, "");
                }
                JObject o = JObject.Parse(x.ToString());
                return StatusCode(200, o);
            }
            catch (Exception ex)
            {
                //DataHelper.Log(DataHelper.GetHeaders(Request));
                DataHelper.Log("srchItemsList()");
                DataHelper.Log(ex.Message);
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("GetItemItemIdentifiers")]
        public ActionResult GetItemItemIdentifiers(int LocationId, int ItemId)
        {
            try
            {
                List<SqlParameter> l = new List<SqlParameter>();
                l.Add(new SqlParameter("@LocationId", LocationId));
                l.Add(new SqlParameter("@User", GetCurrentUser()));
                l.Add(new SqlParameter("@FormId", ExtractFormId()));
                l.Add(new SqlParameter("@Lang", ExtractLang()));
                l.Add(new SqlParameter("@ItemId", ItemId));

                var x = DataHelper.ExecScalar("mms_GetItemItemIdentifiers", System.Data.CommandType.StoredProcedure, l.ToArray());
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

        [HttpGet("getItemUnits")]
        public ActionResult getItemUnits(int ItemIdentifierID)
        {
            try
            {

                List<SqlParameter> l = new List<SqlParameter>();
                l.Add(new SqlParameter("@ItemIdentifierID", ItemIdentifierID));
                l.Add(new SqlParameter("@User", GetCurrentUser()));
                l.Add(new SqlParameter("@FormId", ExtractFormId()));
                l.Add(new SqlParameter("@Lang", ExtractLang()));


                var x = DataHelper.ExecScalar("mms_GetItemUnits", System.Data.CommandType.StoredProcedure, l.ToArray());
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

        [HttpGet("GetCustomersList")]
        public ActionResult GetCustomersList(string MaxCode = "", string? Filter = "")
        {
            try
            {
                List<SqlParameter> l = new List<SqlParameter>();
                l.Add(new SqlParameter("@User", GetCurrentUser()));
                l.Add(new SqlParameter("@FormId", ExtractFormId()));
                l.Add(new SqlParameter("@Lang", ExtractLang()));
                l.Add(new SqlParameter("@MaxCode", MaxCode));
                if (!string.IsNullOrEmpty(Filter))
                    l.Add(new SqlParameter("@Filter", Filter));

                var x = DataHelper.ExecScalar("mms_GetCustomersList", System.Data.CommandType.StoredProcedure, l.ToArray());
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

        [HttpGet("GetFormControls")]
        public ActionResult GetFormControls()
        {
            try
            {
                List<SqlParameter> l = new List<SqlParameter>();
                l.Add(new SqlParameter("@User", GetCurrentUser()));
                l.Add(new SqlParameter("@FormId", ExtractFormId()));
                l.Add(new SqlParameter("@Lang", ExtractLang()));

                var x = DataHelper.ExecScalar("sys_GetFormControls", System.Data.CommandType.StoredProcedure, l.ToArray());
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

        [HttpGet("GetOperationsRetrieveList")]
        public ActionResult GetOperationsRetrieveList(int MinId = int.MaxValue, string? Filter = "")
        {
            try
            {
                List<SqlParameter> l = new List<SqlParameter>();
                l.Add(new SqlParameter("@User", GetCurrentUser()));
                l.Add(new SqlParameter("@FormId", ExtractFormId()));
                l.Add(new SqlParameter("@Lang", ExtractLang()));
                l.Add(new SqlParameter("@MinId", MinId));
                if (!string.IsNullOrEmpty(Filter))
                    l.Add(new SqlParameter("@Filter", Filter));

                var x = DataHelper.ExecScalar("mms_SrchRetrieveOperationApp", System.Data.CommandType.StoredProcedure, l.ToArray());
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

        [HttpGet("SrchRetrieveOperation")]
        public ActionResult SrchRetrieveOperation(int LocationId, int? DstLocationID)
        {
            try
            {
                List<SqlParameter> l = new List<SqlParameter>();
                l.Add(new SqlParameter("@User", GetCurrentUser()));
                l.Add(new SqlParameter("@FormId", ExtractFormId()));
                l.Add(new SqlParameter("@Lang", ExtractLang()));
                l.Add(new SqlParameter("@LocationID", LocationId));
                if (DstLocationID != null)
                { l.Add(new SqlParameter("@DstLocationID", DstLocationID)); }

                var x = DataHelper.ExecScalar("mms_SrchRetrieveOperationNew", System.Data.CommandType.StoredProcedure, l.ToArray());
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

        [HttpGet("GetRetrieveOperations")]
        public ActionResult GetRetrieveOperations(string OperationHeadIDs, int ChildOperationTypeId)
        {
            try
            {
                List<SqlParameter> l = new List<SqlParameter>();
                l.Add(new SqlParameter("@User", GetCurrentUser()));
                l.Add(new SqlParameter("@FormId", ExtractFormId()));
                l.Add(new SqlParameter("@Lang", ExtractLang()));
                l.Add(new SqlParameter("@OperationIDs", OperationHeadIDs));
                l.Add(new SqlParameter("@ChildOperationTypeId", ChildOperationTypeId));

                var x = DataHelper.ExecScalar("mms_GetNetItems2Patch", System.Data.CommandType.StoredProcedure, l.ToArray());
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

        [HttpGet("GetLocationItems")]
        public ActionResult GetLocationItems(int LocationId, int? VendorId = null)
        {
            try
            {
                List<SqlParameter> l = new List<SqlParameter>();
                l.Add(new SqlParameter("@User", GetCurrentUser()));
                l.Add(new SqlParameter("@FormId", ExtractFormId()));
                l.Add(new SqlParameter("@Lang", ExtractLang()));
                l.Add(new SqlParameter("@LocationId", LocationId));
                if (VendorId != null)
                    l.Add(new SqlParameter("@VendorId", VendorId));

                var x = DataHelper.ExecScalar("GetLocationItems", System.Data.CommandType.StoredProcedure, l.ToArray());
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

        #region Save
        [HttpPost("SaveOperation")]
        public async Task<ActionResult> SaveOperation(DataObj dataObj)
        {
            try
            {
                try
                {
                    DataHelper.Log(GetAllHeaders());
                    DataHelper.Log(dataObj.dataObj);

                }
                catch (Exception ex)
                {
                    DataHelper.Log(ex.Message);

                }

                Newtonsoft.Json.Linq.JObject oo = JObject.Parse(dataObj.dataObj);
                var headJson = new JObject();
                string pName = "";
                foreach (var item in oo.Properties().Where(x => x.Name.ToLower() != "images"
                && x.Name.ToLower() != "attachments" && x.Name.ToLower() != "operationDetails".ToLower()
                && x.Name.ToLower() != "operationExpenses".ToLower() && x.Name.ToLower() != "operationsDetailsSpecificAttributes".ToLower()
                && x.Name.ToLower() != "operationsHeadPaymentTypes".ToLower()
                && x.Name.ToLower() != "operationAssets".ToLower()
                                && x.Name.ToLower() != "historyNotes".ToLower()
                ).ToArray())
                {
                    pName = ((Newtonsoft.Json.Linq.JProperty)item).Name.ToString().ToLower();
                    //if (pName != "images" && pName != "files")
                    //{
                    headJson.Add(item);
                    //}
                }
                var detailJson = oo["operationDetails"];
                var operationExpenses = oo["operationExpenses"];
                var attachments = oo["attachments"].ToArray();
                var operationsDetailsSpecificAttributes = oo["operationsDetailsSpecificAttributes"];
                var operationsHeadPaymentTypes = oo["operationsHeadPaymentTypes"];
                var operationAssets = oo["operationAssets"];
                var historyNotes = oo["historyNotes"];
                var formNotes = oo["formNotes"].ToArray();
                JObject newJson = new JObject();
                newJson.Add("Head", headJson);
                newJson.Add("Details", detailJson);
                newJson.Add("operationExpenses", operationExpenses);
                newJson.Add("operationsDetailsSpecificAttributes", operationsDetailsSpecificAttributes);
                newJson.Add("operationsHeadPaymentTypes", operationsHeadPaymentTypes);
                newJson.Add("operationAssets", operationAssets);
                newJson.Add("historyNotes", historyNotes);
                List<SqlParameter> l = new List<SqlParameter>();
                string userCode = GetCurrentUser();
                l.Add(new SqlParameter("@User", userCode));
                l.Add(new SqlParameter("@FormId", ExtractFormId()));
                l.Add(new SqlParameter("@Lang", ExtractLang()));
                l.Add(new SqlParameter("@json", newJson.ToString()) { Size = -1 });

                List<SqlParameter> listOutParameters = new List<SqlParameter>();
                SqlParameter resPar = new SqlParameter() { ParameterName = "@Result", SqlDbType = SqlDbType.NVarChar, Size = -1, Direction = ParameterDirection.Output };
                listOutParameters.Add(resPar);
                SqlParameter errPar = new SqlParameter() { ParameterName = "@Error", SqlDbType = SqlDbType.NVarChar, Size = -1, Direction = ParameterDirection.Output };
                listOutParameters.Add(errPar);
                SqlParameter warrPar = new SqlParameter() { ParameterName = "@Warning", SqlDbType = SqlDbType.NVarChar, Size = -1, Direction = ParameterDirection.Output };
                listOutParameters.Add(warrPar);

                DataHelper.ExecScalar("mms_OperationsSave", System.Data.CommandType.StoredProcedure, ref listOutParameters, l.ToArray());
                string error = listOutParameters.Find(X => X.ParameterName == "@Error").Value.ToString();
                string warning = listOutParameters.Find(X => X.ParameterName == "@Warning").Value.ToString();
                if (!string.IsNullOrEmpty(error))
                    return StatusCode(500, error);

                string result = listOutParameters.Find(X => X.ParameterName == "@Result").Value.ToString();
                JObject operationResult = JObject.Parse(result);
                if (string.IsNullOrEmpty(result))
                {
                    return StatusCode(500, result);
                }
                string strIsZatca = _configuration.GetSection("Zatca")["IsZatca"];
                int isZatca = int.Parse(strIsZatca);
                if (isZatca != 0)
                {

                    string zerror;

                    #region Zatca E-Invoice Code
                    JToken isEInvoiceClearedToken = operationResult["isEInvoiceCleared"];
                    JToken isInvoiceToken = oo["isInvoice"];
                    JToken AccountingAutoPostToken = oo["accountingAutoPost"];
                    JToken isCreditToken = oo["isCredit"];
                    JToken isDebitToken = oo["isDebit"];
                    JToken isSimplifiedToken = operationResult["isSimplified"];
                    var operationCategory = oo["operationCategory"].ToString();
                    var EInvoiceResult = "";

                    bool isEInvoiceCleared = isEInvoiceClearedToken != null ? isEInvoiceClearedToken.ToObject<bool>() : false;
                    bool isInvoice = isInvoiceToken != null ? isInvoiceToken.ToObject<bool>() : false;
                    bool isAccountingAutoPost = AccountingAutoPostToken != null ? AccountingAutoPostToken.ToObject<bool>() : false;
                    bool isCredit = isCreditToken != null ? isCreditToken.ToObject<bool>() : false;
                    bool isDebit = isDebitToken != null ? isDebitToken.ToObject<bool>() : false;
                    bool isSimplified = isSimplifiedToken != null ? isSimplifiedToken.ToObject<bool>() : false;
                    string timeString = (string)operationResult["operationTime"];
                    DateTime dateTime = DateTime.Parse(timeString);
                    string issueTime = dateTime.ToString("HH:mm:ssZ");

                    if (!isEInvoiceCleared && isInvoice && isAccountingAutoPost && operationCategory.ToLower().Contains("sales"))
                    {
                        List<SqlParameter> OutParameters = new List<SqlParameter>();
                        SqlParameter resultPar = new SqlParameter() { ParameterName = "@Result", SqlDbType = SqlDbType.NVarChar, Size = -1, Direction = ParameterDirection.Output };
                        OutParameters.Add(resultPar);
                        DataHelper.ExecScalar("sys_GetCSID", System.Data.CommandType.StoredProcedure, ref OutParameters);
                        string certificate = OutParameters.Find(X => X.ParameterName == "@Result").Value.ToString();
                        if (string.IsNullOrEmpty(certificate))
                        {
                            return StatusCode(500, "No Certificate Found");
                        }
                        JObject certificateData = JObject.Parse(certificate);

                        var certificateDetails = _certificateConfiguration.GetCertificateDetaildata(certificateData);

                        if (certificateDetails != null)
                        {
                            SharedData.UserName = certificateDetails.UserName;
                            SharedData.Secret = certificateDetails.Secret;

                        }
                        var EInvoiceNo = operationResult["EInvNo"];
                        List<LineItem> line = new List<LineItem>();
                        var operationDetails = operationResult["operationDetails"].ToArray();
                        for (int i = 0; i < operationDetails.Length; i++)
                        {
                            JObject objResult = (JObject)operationDetails[i];

                            var lineItem = new LineItem
                            {
                                Index = i,
                                ProductName = objResult["itemName"].ToString(),
                                Quantity = double.Parse(objResult["quantity"].ToString()),
                                NetPrice = double.Parse(objResult["zatcaPrice"].ToString()),
                                Tax = double.Parse(objResult["taxPercent"].ToString()) * 100
                            };
                            line.Add(lineItem);

                        }

                        if (isCredit || isDebit)
                        {
                            //var generateQRCode =  _invoiceInfoGenerator.GenerateQrCode(new InvoiceDataModel
                            //{
                            //    Id = Guid.NewGuid().ToString()

                            //});
                            //if (generateQRCode.Success)
                            //{
                            //    var qrcode = new
                            //    {
                            //        qrcode = generateQRCode.ResultValue,
                            //    };
                            //}
                            var creditInvoice = await _reporter.ReportInvoiceAsync(new InvoiceDataModel
                            {
                                InvoiceNumber = EInvoiceNo.ToString(),
                                InvoiceTypeCode = (int)InvoiceTypeCode.Credit,
                                Id = Guid.NewGuid().ToString(),
                                Order = 2,
                                TransactionTypeCode = isSimplified ? TransactionTypeCode.Simplified : TransactionTypeCode.Standard,
                                Lines = line,
                                Discount = (double)operationResult["oh_totalDiscountAmount"],
                                PaymentMeansCode = 10,
                                ReferenceId = operationResult["OperationParentNo"].ToString(),
                                Notes = operationResult["shortDescription"].ToString(),
                                Supplier = new Supplier
                                {
                                    SellerName = operationResult["companyDetails"][0]["organisationName"].ToString(),
                                    SellerTRN = operationResult["companyDetails"][0]["vATRegNumber"].ToString(),
                                    AdditionalStreetAddress = operationResult["companyDetails"][0]["streetName"].ToString(),
                                    BuildingNumber = operationResult["companyDetails"][0]["buildingNo"].ToString(),
                                    CityName = operationResult["companyDetails"][0]["city"].ToString(),
                                    IdentityNumber = operationResult["companyDetails"][0]["CRN"].ToString(),
                                    IdentityType = "CRN",
                                    CountryCode = "SA",
                                    DistrictName = operationResult["companyDetails"][0]["city"].ToString(),
                                    PostalCode = operationResult["companyDetails"][0]["postalCode"].ToString(),
                                    StreetName = operationResult["companyDetails"][0]["streetName"].ToString(),
                                },
                                Customer = new Customer
                                {
                                    CustomerName = operationResult["customer"]["textField"].ToString(),
                                    IdentityNumber = operationResult["customer"]["identityNo"].ToString(),
                                    IdentityType = "NAT",
                                    StreetName = operationResult["customer"]["streetName"].ToString(),
                                    BuildingNumber = operationResult["customer"]["buildingNo"].ToString(),
                                    ZipCode = operationResult["customer"]["postalCode"].ToString(),
                                    CityName = operationResult["customer"]["city"].ToString(),
                                    DistrictName = operationResult["customer"]["city"].ToString(),
                                    RegionName = operationResult["customer"]["city"].ToString(),
                                    CountryCode = operationResult["customer"]["countryCode"].ToString()
                                },
                                IssueDate = operationResult["operationDate"].ToString(),// "2022-09-26",
                                IssueTime = operationResult["operationTime"].ToString(), // "17:00:00Z",
                                DeliveryDate = operationResult["operationDate"].ToString(),
                                PreviousInvoiceHash = operationResult["invoiceBase64"].ToString()
                            });
                            if (creditInvoice.Data != null)
                            {
                                var fileName = $"{operationResult["companyDetails"][0]["vATRegNumber"]}_{operationResult["operationDate"].ToString().Replace("-", "")}_{issueTime.ToString().Replace(":", "")}_{EInvoiceNo}.xml";
                                if (!string.IsNullOrWhiteSpace(creditInvoice.Data?.SignedXml))
                                {

                                    var path = Path.Combine(Directory.GetCurrentDirectory(), "ZatcaInvoices", fileName);
                                    Directory.CreateDirectory(Path.GetDirectoryName(path)); // Ensure directory exists

                                    // Writing the XML data to the file
                                    await System.IO.File.WriteAllTextAsync(path, creditInvoice.Data?.SignedXml);
                                }

                                var zatcaResponse = new
                                {
                                    operationHeadId = operationResult["ID"],
                                    zatcaStatus = creditInvoice.Data.ReportingStatus,
                                    isEInvoiceCleared = creditInvoice.Success ? 1 : 0,
                                    zatcaErrors = JsonConvert.SerializeObject(creditInvoice.Data?.ErrorMessages?.ToList()),
                                    zatcaMessage = creditInvoice.Data?.ReportingResult,
                                    zatcaQRCode = creditInvoice.Data?.QrCode,
                                    submissionDate = creditInvoice.Data?.SubmissionDate.ToString(),
                                    zatcaWarningMessage = JsonConvert.SerializeObject(creditInvoice.Data?.WarningMessages.ToList()),
                                    invoiceBase64 = creditInvoice.Data?.InvoiceHash,
                                    signedXml = fileName
                                };
                                JObject obj = JObject.FromObject(zatcaResponse);
                                List<SqlParameter> zl = new List<SqlParameter>();
                                zl.Add(new SqlParameter("@User", GetCurrentUser()));
                                zl.Add(new SqlParameter("@json", obj.ToString()) { Size = -1 });
                                List<SqlParameter> listOutParameters1 = new List<SqlParameter>();
                                SqlParameter resPar1 = new SqlParameter() { ParameterName = "@Result", SqlDbType = SqlDbType.NVarChar, Size = -1, Direction = ParameterDirection.Output };
                                listOutParameters1.Add(resPar1);
                                SqlParameter errPar1 = new SqlParameter() { ParameterName = "@Error", SqlDbType = SqlDbType.NVarChar, Size = -1, Direction = ParameterDirection.Output };
                                listOutParameters1.Add(errPar1);

                                DataHelper.ExecScalar("sys_ZatcaResponseSave", System.Data.CommandType.StoredProcedure, ref listOutParameters1, zl.ToArray());
                                zerror = listOutParameters1.Find(X => X.ParameterName == "@Error").Value.ToString();

                            }
                        }

                        else
                        {

                            var normalInvoice = await _reporter.ReportInvoiceAsync(new InvoiceDataModel

                            {
                                InvoiceNumber = EInvoiceNo.ToString(),
                                InvoiceTypeCode = (int)InvoiceTypeCode.Invoice,
                                Id = Guid.NewGuid().ToString(),
                                Order = 2,
                                TransactionTypeCode = isSimplified ? TransactionTypeCode.Simplified : TransactionTypeCode.Standard,
                                Lines = line,
                                Discount = (double)operationResult["oh_totalDiscountAmount"],
                                PaymentMeansCode = 10,
                                Supplier = new Supplier
                                {
                                    SellerName = operationResult["companyDetails"][0]["organisationName"].ToString(),
                                    SellerTRN = operationResult["companyDetails"][0]["vATRegNumber"].ToString(),
                                    AdditionalStreetAddress = operationResult["companyDetails"][0]["streetName"].ToString(),
                                    BuildingNumber = operationResult["companyDetails"][0]["buildingNo"].ToString(),
                                    CityName = operationResult["companyDetails"][0]["city"].ToString(),
                                    IdentityNumber = operationResult["companyDetails"][0]["CRN"].ToString(),
                                    IdentityType = "CRN",
                                    CountryCode = "SA",
                                    DistrictName = operationResult["companyDetails"][0]["city"].ToString(),
                                    PostalCode = operationResult["companyDetails"][0]["postalCode"].ToString(),
                                    StreetName = operationResult["companyDetails"][0]["streetName"].ToString(),
                                },
                                Customer = new Customer
                                {
                                    CustomerName = operationResult["customer"]["textField"].ToString(),
                                    IdentityNumber = operationResult["customer"]["identityNo"].ToString(),
                                    IdentityType = "NAT",
                                    StreetName = operationResult["customer"]["streetName"].ToString(),
                                    BuildingNumber = operationResult["customer"]["buildingNo"].ToString(),
                                    ZipCode = operationResult["customer"]["postalCode"].ToString(),
                                    CityName = operationResult["customer"]["city"].ToString(),
                                    DistrictName = operationResult["customer"]["city"].ToString(),
                                    RegionName = operationResult["customer"]["city"].ToString(),
                                    CountryCode = operationResult["customer"]["countryCode"].ToString()
                                },
                                IssueDate = operationResult["operationDate"].ToString(),// "2022-09-26",
                                IssueTime = issueTime, // "17:00:00Z",
                                DeliveryDate = operationResult["operationDate"].ToString(),
                                PreviousInvoiceHash = operationResult["invoiceBase64"].ToString()
                            });
                            if (normalInvoice.Data != null)
                            {
                                var fileName = $"{operationResult["companyDetails"][0]["vATRegNumber"]}_{operationResult["operationDate"].ToString().Replace("-", "")}_{issueTime.ToString().Replace(":", "")}_{EInvoiceNo}.xml";

                                if (!string.IsNullOrWhiteSpace(normalInvoice.Data?.SignedXml))
                                {

                                    var path = Path.Combine(Directory.GetCurrentDirectory(), "ZatcaInvoices", fileName);
                                    Directory.CreateDirectory(Path.GetDirectoryName(path)); // Ensure directory exists

                                    // Writing the XML data to the file
                                    await System.IO.File.WriteAllTextAsync(path, normalInvoice.Data?.SignedXml);
                                }
                                var zatcaResponse = new
                                {
                                    operationHeadId = operationResult["ID"],
                                    zatcaStatus = normalInvoice.Data.ReportingStatus,
                                    isEInvoiceCleared = normalInvoice.Success ? 1 : 0,
                                    zatcaErrors = JsonConvert.SerializeObject(normalInvoice.Data?.ErrorMessages?.ToList()),
                                    zatcaMessage = normalInvoice.Data?.ReportingResult,
                                    zatcaQRCode = normalInvoice.Data?.QrCode,
                                    submissionDate = normalInvoice.Data?.SubmissionDate.ToString(),
                                    zatcaWarningMessage = JsonConvert.SerializeObject(normalInvoice.Data?.WarningMessages.ToList()),
                                    invoiceBase64 = normalInvoice.Data?.InvoiceHash,
                                    signedXml = fileName

                                };
                                JObject obj = JObject.FromObject(zatcaResponse);
                                List<SqlParameter> zl = new List<SqlParameter>();
                                zl.Add(new SqlParameter("@User", GetCurrentUser()));
                                zl.Add(new SqlParameter("@json", obj.ToString()) { Size = -1 });
                                List<SqlParameter> listOutParameters1 = new List<SqlParameter>();
                                SqlParameter resPar1 = new SqlParameter() { ParameterName = "@Result", SqlDbType = SqlDbType.NVarChar, Size = -1, Direction = ParameterDirection.Output };
                                listOutParameters1.Add(resPar1);
                                SqlParameter errPar1 = new SqlParameter() { ParameterName = "@Error", SqlDbType = SqlDbType.NVarChar, Size = -1, Direction = ParameterDirection.Output };
                                listOutParameters1.Add(errPar1);

                                DataHelper.ExecScalar("sys_ZatcaResponseSave", System.Data.CommandType.StoredProcedure, ref listOutParameters1, zl.ToArray());
                                zerror = listOutParameters1.Find(X => X.ParameterName == "@Error").Value.ToString();
                                //if (!string.IsNullOrEmpty(zerror))
                                //    return StatusCode(500, zerror);

                                //string zresult = listOutParameters1.Find(X => X.ParameterName == "@Result").Value.ToString();

                                //if (string.IsNullOrEmpty(zresult))
                                //{
                                //    return StatusCode(500, zresult);
                                //}
                                //JObject o = JObject.Parse(zresult);
                                //return StatusCode(200, o);
                                //var errors = normalInvoice.Data?.ErrorMessages?.ToList();
                                //var reported = normalInvoice.Data?.ReportingStatus;
                                //return StatusCode(200, reported.ToString());
                            }




                        }

                    }
                    #endregion
                }

                #region //Save Attachments if invoice has been saved
                try
                {
                    Newtonsoft.Json.Linq.JObject file;
                    if (attachments.Length > 0)
                    {
                        List<SqlParameter> listAttachments;
                        int operationHeadId = Convert.ToInt32(operationResult["ID"]);
                        string nonDeletedAttachments = "";
                        //TODO>> كملها علشان الحذف 
                        //for (int i = 0; i < attachments.Length; i++)
                        //{
                        //    file = (Newtonsoft.Json.Linq.JObject)attachments[i];
                        //    if (file["id"] != null)
                        //    {
                        //        nonDeletedAttachments += file["id"].ToString() + ",";
                        //    }


                        //}
                        //listAttachments = new List<SqlParameter>();
                        //listAttachments.Add(new SqlParameter("@User", GetCurrentUser()));
                        //listAttachments.Add(new SqlParameter("@FormId", ExtractFormId()));
                        //listAttachments.Add(new SqlParameter("@TransactionId", operationHeadId));
                        //listAttachments.Add(new SqlParameter("@IDs", nonDeletedAttachments.));

                        //byte[] imageData = Convert.FromBase64String(file["data"].ToString().Substring(("data:" + file["type"] + ";base64,").Length));
                        //listAttachments.Add(new SqlParameter("@AttachedFile", SqlDbType.Binary, imageData.Length) { Value = imageData });
                        //bool x = DataHelper.ExecNonQuery("sys_SaveFormAttachments", CommandType.StoredProcedure, listAttachments.ToArray());

                        for (int i = 0; i < attachments.Length; i++)
                        {
                            file = (Newtonsoft.Json.Linq.JObject)attachments[i];
                            if (file["data"] != null)//New Attachments Only
                            {
                                listAttachments = new List<SqlParameter>();
                                listAttachments.Add(new SqlParameter("@User", userCode));
                                listAttachments.Add(new SqlParameter("@FormId", ExtractFormId()));
                                listAttachments.Add(new SqlParameter("@TransactionId", operationHeadId));
                                listAttachments.Add(new SqlParameter("@FileName", file["path"].ToString()));
                                listAttachments.Add(new SqlParameter("@Size", file["size"].ToString()));
                                listAttachments.Add(new SqlParameter("@type", file["type"].ToString()));

                                byte[] imageData = Convert.FromBase64String(file["data"].ToString().Substring(("data:" + file["type"] + ";base64,").Length));
                                listAttachments.Add(new SqlParameter("@AttachedFile", SqlDbType.Binary, imageData.Length) { Value = imageData });
                                bool x = DataHelper.ExecNonQuery("sys_SaveFormAttachments", CommandType.StoredProcedure, listAttachments.ToArray());

                            }


                        }
                    }

                }
                catch (Exception ex)
                {
                    return StatusCode(500, ex.Message);
                }
                #endregion

                #region //Save Notes if record has been saved
                try
                {
                    HelperUtilitis.saveNotes(ExtractFormId(), Convert.ToInt64(oo["ID"]), GetCurrentUser(), formNotes);
                }
                catch (Exception ex)
                {
                    return StatusCode(500, ex.Message);
                }
                #endregion
                var rsdMethodName = (JValue)operationResult["rsdMethodName"];
                if (rsdMethodName == null || string.IsNullOrEmpty(rsdMethodName.ToString()))
                {
                    return StatusCode(200, operationResult);
                }
                string strIsRSD = _configuration.GetSection("Leen")["IsRSD"];
                int isRSD = int.Parse(strIsRSD);
                if (isRSD != 0)
                {
                    try
                    {
                        int id = 0;
                        JObject resultObj = JObject.Parse(result);
                        var x = (JValue)resultObj["ID"];
                        int.TryParse(x.ToString(), out id);

                        //string strConnectThrowLeen = System.Web.Configuration.WebConfigurationManager.AppSettings["ConnectThrowLeen"];
                        string strConnectThrowLeen = _configuration.GetSection("Leen")["ConnectThrowLeen"];


                        int connectThrowLeen = 0;
                        int.TryParse(strConnectThrowLeen, out connectThrowLeen);
                        if (connectThrowLeen == 0)//Connect throw Julib
                        {
                            new Thread(() =>
                            {
                                Thread.CurrentThread.IsBackground = true;
                                _rsd.post(id, userCode);
                            })
                      .Start();
                        }
                        else//Connect throw Leen
                        {


                            var operationSpecificAttributes = operationResult["operationsDetailsSpecificAttributes"].ToArray();
                            var ProductSpecificAttributesList = new List<ProductModel>();

                            for (int i = 0; i < operationSpecificAttributes.Length; i++)
                            {
                                JObject objResult = (JObject)operationSpecificAttributes[i];

                                var itemList = new ProductModel
                                {
                                    BN = objResult["ODSA_attribute1"].ToString(),
                                    SN = objResult["ODSA_attribute2"].ToString(),
                                    GTIN = objResult["ODSA_attribute3"].ToString(),
                                    XD = DateTime.Parse(objResult["ODSA_date1"].ToString()),
                                    Quantity = int.Parse(objResult["ODSA_Qty"].ToString())
                                };
                                ProductSpecificAttributesList.Add(itemList);

                            }

                            await RsdIntegrationHelper.SetCurrentCredentialsAsync(new CredentialsModel
                            {
                                UserName = resultObj["rsdUserDetails"][0]["rsdUser"].ToString(),
                                Password = resultObj["rsdUserDetails"][0]["rsdPassword"].ToString()
                            });

                            var rsdResponse = new PostRSDResponseDto();
                            switch (rsdMethodName.ToString())
                            {
                                case "InvAccept":
                                    // _rsd.InvAccept(id, userCode);
                                    var RsdAcceptResult = await RsdIntegrationHelper.AcceptByBatch(new AcceptBatchModel
                                    {
                                        Products = ProductSpecificAttributesList,
                                        //FromGLN = resultObj["rsdUserDetails"][0]["gln"].ToString()
                                    });
                                    if (RsdAcceptResult != null)
                                    {

                                        rsdResponse.OperationHeadId = resultObj["ID"].ToString();
                                        //rsdResponse.RequestedNotificationId = resultObj["RequestedNotificationId"].ToString();
                                        rsdResponse.NotificationId = RsdAcceptResult.NotificationId.ToString();
                                        rsdResponse.RSDStatus = RsdAcceptResult.ProductList.Where(x => x.Code != "0000").Any() ? false : true;
                                        rsdResponse.RSDResponseProductList = JsonConvert.SerializeObject(RsdAcceptResult.ProductList.ToList());
                                        rsdResponse.CreadtedBy = GetCurrentUser();
                                        rsdResponse.PostedDate = DateTime.Now;

                                    }

                                    break;
                                case "PharmacySale":
                                    //_rsd.PharmacySale(id, userCode);
                                    var RsdPharmacySalesResult = await RsdIntegrationHelper.PharmacySale(new PharmacySaleModel
                                    {
                                        Products = ProductSpecificAttributesList,
                                        TOGLN = resultObj["customer"]["Gln"].ToString(),

                                    });
                                    if (RsdPharmacySalesResult != null)
                                    {

                                        rsdResponse.OperationHeadId = resultObj["ID"].ToString();
                                        rsdResponse.NotificationId = RsdPharmacySalesResult.NotificationId.ToString();
                                        rsdResponse.RSDStatus = RsdPharmacySalesResult.ProductList.Where(x => x.Code != "0000").Any() ? false : true;
                                        rsdResponse.RSDResponseProductList = JsonConvert.SerializeObject(RsdPharmacySalesResult.ProductList.ToList());
                                        rsdResponse.CreadtedBy = GetCurrentUser();
                                        rsdResponse.PostedDate = DateTime.Now;
                                    }
                                    break;
                                case "PharmacySaleCancel":
                                    //_rsd.PharmacySaleCancel(id, userCode);
                                    var rsdPharmacySaleCancelResponse = await RsdIntegrationHelper.PharmacyCancelSale(new PharmacyCancelSaleModel
                                    {
                                        Products = ProductSpecificAttributesList,
                                        //TOGLN = "0000000000000"
                                        TOGLN = resultObj["rsdUserDetails"][0]["gln"].ToString()
                                    });
                                    if (rsdPharmacySaleCancelResponse != null)
                                    {

                                        rsdResponse.OperationHeadId = resultObj["ID"].ToString();
                                        rsdResponse.NotificationId = rsdPharmacySaleCancelResponse.NotificationId.ToString();
                                        rsdResponse.RSDStatus = rsdPharmacySaleCancelResponse.ProductList.Where(x => x.Code != "0000").Any() ? false : true;
                                        rsdResponse.RSDResponseProductList = JsonConvert.SerializeObject(rsdPharmacySaleCancelResponse.ProductList.ToList());
                                        rsdResponse.CreadtedBy = GetCurrentUser();
                                        rsdResponse.PostedDate = DateTime.Now;
                                    }
                                    break;
                                case "Deactivat":
                                    //_rsd.Deactivat(id, userCode, "30");
                                    break;
                                case "DeactivatCancel":
                                    //_rsd.DeactivatCancel(id, userCode);
                                    break;
                                case "Transfer":

                                    var rsdTransferResponse = await RsdIntegrationHelper.TransferByBatch(new TransferBatchModel
                                    {
                                        Products = ProductSpecificAttributesList,
                                        TOGLN = resultObj["rsdUserDetails"][0]["toGln"].ToString()
                                    });
                                    if (rsdTransferResponse != null)
                                    {

                                        rsdResponse.OperationHeadId = resultObj["ID"].ToString();
                                        rsdResponse.NotificationId = rsdTransferResponse.NotificationId.ToString();
                                        rsdResponse.RSDStatus = rsdTransferResponse.ProductList.Where(x => x.Code != "0000").Any() ? false : true;
                                        rsdResponse.RSDResponseProductList = JsonConvert.SerializeObject(rsdTransferResponse.ProductList.ToList());
                                        rsdResponse.CreadtedBy = GetCurrentUser();
                                        rsdResponse.PostedDate = DateTime.Now;
                                    }

                                    //_rsd.Transfer(id, userCode);

                                    break;
                                case "TransferCancel":
                                    //_rsd.TransferCancel(id, userCode);

                                    var rsdTransferCancelResponse = await RsdIntegrationHelper.TransferCancelByBatch(new TransferCancelBatchModel
                                    {
                                        Products = ProductSpecificAttributesList,
                                        TOGLN = resultObj["rsdUserDetails"][0]["gln"].ToString()

                                    });
                                    if (rsdTransferCancelResponse != null)
                                    {

                                        rsdResponse.OperationHeadId = resultObj["ID"].ToString();
                                        rsdResponse.NotificationId = rsdTransferCancelResponse.NotificationId.ToString();
                                        rsdResponse.RSDStatus = rsdTransferCancelResponse.ProductList.Where(x => x.Code != "0000").Any() ? false : true;
                                        rsdResponse.RSDResponseProductList = JsonConvert.SerializeObject(rsdTransferCancelResponse.ProductList.ToList());
                                        rsdResponse.CreadtedBy = GetCurrentUser();
                                        rsdResponse.PostedDate = DateTime.Now;
                                    }

                                    break;
                                case "Return":
                                    //_rsd.Return(id, userCode);

                                    var rsdReturnResponse = await RsdIntegrationHelper.ReturnByBatch(new ReturnBatchModel
                                    {
                                        //TOGLN = "0000000000000",

                                        TOGLN = resultObj["vendor"]["Gln"].ToString(),
                                        Products = ProductSpecificAttributesList
                                    });
                                    if (rsdReturnResponse != null)
                                    {
                                        rsdResponse.OperationHeadId = resultObj["ID"].ToString();
                                        rsdResponse.NotificationId = rsdReturnResponse.NotificationId.ToString();
                                        rsdResponse.RSDStatus = rsdReturnResponse.ProductList.Where(x => x.Code != "0000").Any() ? false : true;
                                        rsdResponse.RSDResponseProductList = JsonConvert.SerializeObject(rsdReturnResponse.ProductList.ToList());
                                        rsdResponse.CreadtedBy = GetCurrentUser();
                                        rsdResponse.PostedDate = DateTime.Now;
                                    }
                                    var data = await _generalService.PostRsdResponse(rsdResponse);
                                    if (data.Code == System.Net.HttpStatusCode.OK)
                                    {
                                        return StatusCode(200, data.Object);
                                    }

                                    break;
                                case "Consume":
                                    //_rsd.Consume(id, userCode);
                                    var rsdConsumeResponse = await RsdIntegrationHelper.Consume(new ConsumeModel { Products = ProductSpecificAttributesList, });
                                    if (rsdConsumeResponse != null)
                                    {
                                        rsdResponse.OperationHeadId = resultObj["ID"].ToString();
                                        rsdResponse.NotificationId = rsdConsumeResponse.NotificationId.ToString();
                                        rsdResponse.RSDStatus = rsdConsumeResponse.ProductList.Where(x => x.Code != "0000").Any() ? false : true;
                                        rsdResponse.RSDResponseProductList = JsonConvert.SerializeObject(rsdConsumeResponse.ProductList.ToList());
                                        rsdResponse.CreadtedBy = GetCurrentUser();
                                        rsdResponse.PostedDate = DateTime.Now;
                                    }

                                    break;
                                case "ConsumeCancel":
                                    //_rsd.ConsumeCancel(id, userCode);
                                    var rsdConsumeCancelResponse = await RsdIntegrationHelper.ConsumeCancel(new ConsumeCancelModel
                                    {
                                        Products = ProductSpecificAttributesList
                                    });
                                    if (rsdConsumeCancelResponse != null)
                                    {
                                        rsdResponse.OperationHeadId = resultObj["ID"].ToString();
                                        rsdResponse.NotificationId = rsdConsumeCancelResponse.NotificationId.ToString();
                                        rsdResponse.RSDStatus = rsdConsumeCancelResponse.ProductList.Where(x => x.Code != "0000").Any() ? false : true;
                                        rsdResponse.RSDResponseProductList = JsonConvert.SerializeObject(rsdConsumeCancelResponse.ProductList.ToList());
                                        rsdResponse.CreadtedBy = GetCurrentUser();
                                        rsdResponse.PostedDate = DateTime.Now;
                                    }
                                    break;
                                default:
                                    break;
                            }
                            if (rsdResponse != null)
                            {


                                var data = await _generalService.PostRsdResponse(rsdResponse);
                                if (data.Code == System.Net.HttpStatusCode.OK)
                                {
                                    return StatusCode(200, data.Object);
                                }



                                return StatusCode(200, rsdResponse);
                            }

                        }

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);

                    }
                }

                if (!string.IsNullOrEmpty(warning))
                {
                    return StatusCode(200, warning);
                }
                return StatusCode(200, operationResult);
            }
            catch (Exception ex)
            {

                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("SaveFormNotes")]
        public ActionResult SaveFormNotes(DataObj dataObj)
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
                    pName = item.Name.ToString().ToLower();
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

                DataHelper.ExecScalar("sys_FormNotesSave", CommandType.StoredProcedure, ref listOutParameters, l.ToArray());
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


        [HttpPost("RSDBulkRePosting")]
        public async Task<ActionResult> RSDBulkRePosting(DataObj dataObj)
        {
            try
            {
                try
                {
                    //DataHelper.Log(DataHelper.GetHeaders(Request));
                    DataHelper.Log(dataObj.dataObj);
                }
                catch (Exception ex)
                {

                }
                List<SqlParameter> l = new List<SqlParameter>();
                l.Add(new SqlParameter("@FormId", ExtractFormId()));
                l.Add(new SqlParameter("@User", GetCurrentUser()));
                l.Add(new SqlParameter("@Lang", ExtractLang()));
                l.Add(new SqlParameter("@ID", dataObj.dataObj.ToString()) { Size = -1 });

                List<SqlParameter> listOutParameters = new List<SqlParameter>();
                SqlParameter resPar = new SqlParameter() { ParameterName = "@Result", SqlDbType = SqlDbType.NVarChar, Size = -1, Direction = ParameterDirection.Output };
                listOutParameters.Add(resPar);

                DataHelper.ExecScalar("mms_OperationDataByForRSD", System.Data.CommandType.StoredProcedure, ref listOutParameters, l.ToArray());

                string result = listOutParameters.Find(X => X.ParameterName == "@Result").Value.ToString();
                //JObject operationData = JObject.Parse(result);
                JArray operations = JArray.Parse(result);
                List<PostRSDResponseDto> allResponses = new List<PostRSDResponseDto>();
                foreach (JObject operation in operations)
                {
                    var rsdMethodName = operation["rsdMethodName"].ToString();
                    var gln = operation["gln"].ToString();
                    var rsdUser = operation["rsdUser"].ToString();
                    var rsdpassword = operation["rsdpassword"].ToString();
                    var opearationDate = operation["operationDate"].ToString();
                    DateTime operationDateTime = DateTime.Parse(opearationDate);
                    await RsdIntegrationHelper.SetCurrentCredentialsAsync(new CredentialsModel
                    {
                        UserName = rsdUser,
                        Password = rsdpassword
                    });

                    var operationDetails = operation["operationsDetailsSpecificAttributes"].ToArray();
                    var ProductList = new List<ProductModel>();
                    for (int i = 0; i < operationDetails.Length; i++)
                    {
                        JObject objResult = (JObject)operationDetails[i];

                        ProductList.Add(new ProductModel
                        {
                            BN = objResult["ODSA_attribute1"].ToString(),
                            SN = objResult["ODSA_attribute2"].ToString(),
                            GTIN = objResult["ODSA_attribute3"].ToString(),
                            XD = DateTime.Parse(objResult["ODSA_date1"].ToString()),
                            Quantity = int.Parse(objResult["ODSA_Qty"].ToString())
                        }); 
                    }
                    var rsdResponse = new PostRSDResponseDto();
                    switch (rsdMethodName.ToString())
                    {
                        case "PharmacySale":
                            var rsdPharmacySaleResponse = await RsdIntegrationHelper.PharmacySale(new PharmacySaleModel
                            {
                                Products = ProductList,
                                PRESCRIPTIONID = operation["operationheadId"].ToString(),
                                PRESCRIPTIONDATE = operationDateTime,
                                //TOGLN = gln
                                TOGLN = operation["custGln"].ToString(),
                            });
                            if (rsdPharmacySaleResponse != null)
                            {
                                rsdResponse =  BuildResponseDto(operation["operationheadId"].ToString(), rsdPharmacySaleResponse.NotificationId, rsdPharmacySaleResponse);
                            }
                            break;

                        case "PharmacySaleCancel":
                            var rsdPharmacyCancelResponse = await RsdIntegrationHelper.PharmacyCancelSale(new PharmacyCancelSaleModel
                            {
                                Products = ProductList,
                                //PRESCRIPTIONID = operation["operationheadId"].ToString(),
                                //TOGLN = gln
                                TOGLN = gln
                            });
                            if (rsdPharmacyCancelResponse != null)
                            {
                              rsdResponse = BuildResponseDto(operation["operationheadId"].ToString(), rsdPharmacyCancelResponse.NotificationId, rsdPharmacyCancelResponse);

                            }
                            break;

                        case "InvAccept":
                            // _rsd.InvAccept(id, userCode);
                            var RsdAcceptResult = await RsdIntegrationHelper.AcceptByBatch(new AcceptBatchModel
                            {
                                Products = ProductList,
                                //FromGLN = resultObj["rsdUserDetails"][0]["gln"].ToString()
                            });
                            if (RsdAcceptResult != null)
                            {

                                rsdResponse.OperationHeadId = operation["operationheadId"].ToString();
                                rsdResponse.NotificationId = RsdAcceptResult.NotificationId.ToString();
                                rsdResponse.RSDStatus = RsdAcceptResult.ProductList.Where(x => x.Code != "0000").Any() ? false : true;
                                rsdResponse.RSDResponseProductList = JsonConvert.SerializeObject(RsdAcceptResult.ProductList.ToList());
                                rsdResponse.CreadtedBy = GetCurrentUser();
                                rsdResponse.PostedDate = DateTime.Now;
                            }
                            break;
                        case "Return":
                            //_rsd.Return(id, userCode);

                            var rsdReturnResponse = await RsdIntegrationHelper.ReturnByBatch(new ReturnBatchModel
                            {
                                //TOGLN = "0000000000000",

                                TOGLN = operation["vendGln"].ToString(),
                                Products = ProductList
                            });
                            if (rsdReturnResponse != null)
                            {
                                rsdResponse = BuildResponseDto(operation["operationheadId"].ToString(), rsdReturnResponse.NotificationId, rsdReturnResponse);
                            }


                            break;

                        default: break;

                    }
                    await _generalService.PostRsdResponse(rsdResponse);
                    allResponses.Add(rsdResponse);

                }
                var responseDto = new 
                {
                    TotalProcessed = allResponses.Count,
                    SuccessCount = allResponses.Count(x => x.RSDStatus),
                    FailureCount = allResponses.Count(x => !x.RSDStatus),
                    Responses = allResponses
                };

                return StatusCode(200, responseDto);
            }
            catch (Exception)
            {

                return StatusCode(500, "");
            }
        }

        private PostRSDResponseDto BuildResponseDto(string operationheadId, string notificationId, RsdResultModel response)
        {
            return new PostRSDResponseDto
            {
                OperationHeadId = operationheadId,
                NotificationId = notificationId,
                RSDStatus = !response.ProductList.Any(x => x.Code != "0000"),
                RSDResponseProductList = JsonConvert.SerializeObject(response.ProductList),
                CreadtedBy = GetCurrentUser(),
                PostedDate = DateTime.Now
            };
        }


        [HttpPost("ZatcaBulkOperationRePost")]
        public async Task<ActionResult> ZatcaBulkOperationRePost(DataObj dataObj)
        {
            try
            {
                try
                {
                    //DataHelper.Log(DataHelper.GetHeaders(Request));
                    DataHelper.Log(dataObj.dataObj);
                }
                catch (Exception ex)
                {

                }
                List<SqlParameter> l = new List<SqlParameter>();
                l.Add(new SqlParameter("@FormId", ExtractFormId()));
                l.Add(new SqlParameter("@User", GetCurrentUser()));
                l.Add(new SqlParameter("@Lang", ExtractLang()));
                l.Add(new SqlParameter("@ID", dataObj.dataObj.ToString()) { Size = -1 });

                List<SqlParameter> listOutParameters = new List<SqlParameter>();
                SqlParameter resPar = new SqlParameter() { ParameterName = "@Result", SqlDbType = SqlDbType.NVarChar, Size = -1, Direction = ParameterDirection.Output };
                listOutParameters.Add(resPar);

                DataHelper.ExecScalar("mms_OperationDataByForZatca", System.Data.CommandType.StoredProcedure, ref listOutParameters, l.ToArray());

                string result = listOutParameters.Find(X => X.ParameterName == "@Result").Value.ToString();
                //JObject operationData = JObject.Parse(result);
                JArray operations = JArray.Parse(result);
                foreach (JObject operation in operations)
                {

                    JToken isEInvoiceClearedToken = operation["isEInvoiceCleared"];
                    JToken isInvoiceToken = operation["isInvoice"];
                    JToken AccountingAutoPostToken = operation["accountingAutoPost"];
                    JToken isCreditToken = operation["isCredit"];
                    JToken isDebitToken = operation["isDebit"];
                    JToken isSimplifiedToken = operation["isSimplified"];
                    var operationCategory = operation["operationCategory"].ToString();
                    var EInvoiceResult = "";
                    string zerror;
                    bool isEInvoiceCleared = isEInvoiceClearedToken.ToObject<bool>();
                    bool isInvoice = isInvoiceToken.ToObject<bool>();
                    bool isAccountingAutoPost = AccountingAutoPostToken.ToObject<bool>();
                    bool isCredit = isCreditToken.ToObject<bool>();
                    bool isDebit = isDebitToken.ToObject<bool>();
                    bool isSimplified = isSimplifiedToken != null ? isSimplifiedToken.ToObject<bool>() : false;
                    string timeString = (string)operation["operationTime"];
                    DateTime dateTime = DateTime.Parse(timeString);
                    string issueTime = dateTime.ToString("HH:mm:ssZ");
                    List<SqlParameter> OutParameters = new List<SqlParameter>();
                    SqlParameter resultPar = new SqlParameter() { ParameterName = "@Result", SqlDbType = SqlDbType.NVarChar, Size = -1, Direction = ParameterDirection.Output };
                    OutParameters.Add(resultPar);
                    DataHelper.ExecScalar("sys_GetCSID", System.Data.CommandType.StoredProcedure, ref OutParameters);
                    string certificate = OutParameters.Find(X => X.ParameterName == "@Result").Value.ToString();
                    JObject certificateData = JObject.Parse(certificate);

                    var certificateDetails = _certificateConfiguration.GetCertificateDetaildata(certificateData);

                    if (certificateDetails != null)
                    {
                        SharedData.UserName = certificateDetails.UserName;
                        SharedData.Secret = certificateDetails.Secret;

                    }

                    var EInvoiceNo = operation["EInvNo"].ToString();
                    List<LineItem> line = new List<LineItem>();
                    var operationDetails = operation["operationDetails"].ToArray();
                    for (int i = 0; i < operationDetails.Length; i++)
                    {
                        // await _hubContext.Clients.All.SendAsync("ZatcaResponse", "Posting To Zatca Started");
                        JObject objResult = (JObject)operationDetails[i];

                        var lineItem = new LineItem
                        {
                            Index = i,
                            ProductName = objResult["itemName"].ToString(),
                            Quantity = double.Parse(objResult["quantity"].ToString()),
                            NetPrice = double.Parse(objResult["zatcaPrice"].ToString()),
                            Tax = double.Parse(objResult["taxPercent"].ToString()) * 100,
                            //TaxCategory = "S"

                        };
                        if (lineItem.Tax == 0)
                        {
                            lineItem.TaxCategory = "Z";
                            lineItem.TaxCategoryReason = "";
                            lineItem.TaxCategoryReasonCode = "";
                        }
                        line.Add(lineItem);

                    }

                    if (isCredit || isDebit)
                    {
                        var creditInvoice = await _reporter.ReportInvoiceAsync(new InvoiceDataModel
                        {
                            InvoiceNumber = EInvoiceNo.ToString(),
                            InvoiceTypeCode = (int)InvoiceTypeCode.Credit,
                            Id = Guid.NewGuid().ToString(),
                            Order = 2,
                            TransactionTypeCode = isSimplified ? TransactionTypeCode.Simplified : TransactionTypeCode.Standard,
                            Lines = line,
                            Discount = (double)operation["oh_totalDiscountAmount"],
                            PaymentMeansCode = 10,
                            ReferenceId = operation["OperationParentNo"].ToString(),
                            Notes = operation["shortDescription"].ToString(),
                            Supplier = new Supplier
                            {
                                SellerName = operation["companyDetails"][0]["organisationName"].ToString(),
                                SellerTRN = operation["companyDetails"][0]["vATRegNumber"].ToString(),
                                AdditionalStreetAddress = operation["companyDetails"][0]["streetName"].ToString(),
                                BuildingNumber = operation["companyDetails"][0]["buildingNo"].ToString(),
                                CityName = operation["companyDetails"][0]["city"].ToString(),
                                IdentityNumber = operation["companyDetails"][0]["CRN"].ToString(),
                                IdentityType = "CRN",
                                CountryCode = "SA",
                                DistrictName = operation["companyDetails"][0]["city"].ToString(),
                                PostalCode = operation["companyDetails"][0]["postalCode"].ToString(),
                                StreetName = operation["companyDetails"][0]["streetName"].ToString(),
                            },
                            Customer = new Customer
                            {
                                CustomerName = operation["customer"]["textField"].ToString(),
                                IdentityNumber = operation["customer"]["identityNo"].ToString(),
                                IdentityType = "NAT",
                                StreetName = operation["customer"]["streetName"].ToString(),
                                BuildingNumber = operation["customer"]["buildingNo"].ToString(),
                                ZipCode = operation["customer"]["postalCode"].ToString(),
                                CityName = operation["customer"]["city"].ToString(),
                                DistrictName = operation["customer"]["city"].ToString(),
                                RegionName = operation["customer"]["city"].ToString(),
                                CountryCode = operation["customer"]["countryCode"].ToString()
                            },
                            IssueDate = operation["operationDate"].ToString(),// "2022-09-26",
                            IssueTime = issueTime, // "17:00:00Z",
                            DeliveryDate = operation["operationDate"].ToString(),
                            PreviousInvoiceHash = operation["invoiceBase64"].ToString()
                        });
                        if (creditInvoice.Data != null)
                        {
                            var existingFile = operation["signedXml"].ToString();
                            if (!string.IsNullOrEmpty(existingFile))
                            {
                                var path = Path.Combine(Directory.GetCurrentDirectory(), "ZatcaInvoices", existingFile);
                                if (System.IO.File.Exists(path))
                                {
                                    System.IO.File.Delete(path);
                                }
                            }
                            var fileName = $"{operation["companyDetails"][0]["vATRegNumber"]}_{operation["operationDate"].ToString().Replace("-", "")}_{issueTime.ToString().Replace(":", "")}_{EInvoiceNo}.xml";
                            if (!string.IsNullOrWhiteSpace(creditInvoice.Data?.SignedXml))
                            {

                                var path = Path.Combine(Directory.GetCurrentDirectory(), "ZatcaInvoices", fileName);
                                Directory.CreateDirectory(Path.GetDirectoryName(path)); // Ensure directory exists
                                await System.IO.File.WriteAllTextAsync(path, creditInvoice.Data?.SignedXml);
                            }
                            var zatcaResponse = new
                            {
                                operationHeadId = operation["operationheadId"],
                                zatcaStatus = creditInvoice.Data.ReportingStatus,
                                isEInvoiceCleared = creditInvoice.Success ? 1 : 0,
                                zatcaErrors = JsonConvert.SerializeObject(creditInvoice.Data?.ErrorMessages?.ToList()),
                                zatcaMessage = creditInvoice.Data?.ReportingResult,
                                zatcaQRCode = creditInvoice.Data?.QrCode,
                                submissionDate = creditInvoice.Data?.SubmissionDate.ToString(),
                                zatcaWarningMessage = JsonConvert.SerializeObject(creditInvoice.Data?.WarningMessages.ToList()),
                                invoiceBase64 = creditInvoice.Data?.InvoiceHash,
                                signedXml = fileName
                            };
                            JObject obj = JObject.FromObject(zatcaResponse);
                            List<SqlParameter> zl = new List<SqlParameter>();
                            zl.Add(new SqlParameter("@User", GetCurrentUser()));
                            zl.Add(new SqlParameter("@json", obj.ToString()) { Size = -1 });
                            List<SqlParameter> listOutParameters1 = new List<SqlParameter>();
                            SqlParameter resPar1 = new SqlParameter() { ParameterName = "@Result", SqlDbType = SqlDbType.NVarChar, Size = -1, Direction = ParameterDirection.Output };
                            listOutParameters1.Add(resPar1);
                            SqlParameter errPar1 = new SqlParameter() { ParameterName = "@Error", SqlDbType = SqlDbType.NVarChar, Size = -1, Direction = ParameterDirection.Output };
                            listOutParameters1.Add(errPar1);

                            DataHelper.ExecScalar("sys_ZatcaResponseSave", System.Data.CommandType.StoredProcedure, ref listOutParameters1, zl.ToArray());
                            zerror = listOutParameters1.Find(X => X.ParameterName == "@Error").Value.ToString();
                            //if (creditInvoice.Success)
                            //{
                            //    await _hubContext.Clients.All.SendAsync("ZatcaResponse", $"Invoice No {EInvoiceNo} Posted to Zatca");
                            //}
                            //else
                            //{
                            //    await _hubContext.Clients.All.SendAsync("ZatcaResponse", $"Invoice No {EInvoiceNo} Not Posted  to Zatca");
                            //}
                            // if (!creditInvoice.Success) return StatusCode(500, creditInvoice.Data.ErrorMessages.ToString());
                        }
                        else
                        {
                            return StatusCode(500, creditInvoice.Message);
                        }
                    }

                    else
                    {

                        var normalInvoice = await _reporter.ReportInvoiceAsync(new InvoiceDataModel
                        {
                            InvoiceNumber = EInvoiceNo.ToString(),
                            InvoiceTypeCode = (int)InvoiceTypeCode.Invoice,
                            Id = Guid.NewGuid().ToString(),
                            Order = 2,
                            TransactionTypeCode = isSimplified ? TransactionTypeCode.Simplified : TransactionTypeCode.Standard,
                            Lines = line,
                            Discount = (double)operation["oh_totalDiscountAmount"],
                            PaymentMeansCode = 10,
                            Supplier = new Supplier
                            {
                                SellerName = operation["companyDetails"][0]["organisationName"].ToString(),
                                SellerTRN = operation["companyDetails"][0]["vATRegNumber"].ToString(),
                                AdditionalStreetAddress = operation["companyDetails"][0]["streetName"].ToString(),
                                BuildingNumber = operation["companyDetails"][0]["buildingNo"].ToString(),
                                CityName = operation["companyDetails"][0]["city"].ToString(),
                                IdentityNumber = operation["companyDetails"][0]["CRN"].ToString(),
                                IdentityType = "CRN",
                                CountryCode = "SA",
                                DistrictName = operation["companyDetails"][0]["city"].ToString(),
                                PostalCode = operation["companyDetails"][0]["postalCode"].ToString(),
                                StreetName = operation["companyDetails"][0]["streetName"].ToString(),
                            },
                            Customer = new Customer
                            {
                                CustomerName = operation["customer"]["textField"].ToString(),
                                IdentityNumber = operation["customer"]["identityNo"].ToString(),
                                IdentityType = "NAT",
                                StreetName = operation["customer"]["streetName"].ToString(),
                                BuildingNumber = operation["customer"]["buildingNo"].ToString(),
                                ZipCode = operation["customer"]["postalCode"].ToString(),
                                CityName = operation["customer"]["city"].ToString(),
                                DistrictName = operation["customer"]["city"].ToString(),
                                RegionName = operation["customer"]["city"].ToString(),
                                CountryCode = operation["customer"]["countryCode"].ToString()
                            },
                            IssueDate = operation["operationDate"].ToString(),// "2022-09-26",
                            IssueTime = issueTime, // "17:00:00Z",
                            DeliveryDate = operation["operationDate"].ToString(),
                            PreviousInvoiceHash = operation["invoiceBase64"].ToString()
                        });
                        if (normalInvoice.Data != null)
                        {
                            var existingFile = operation["signedXml"].ToString();
                            if (!string.IsNullOrEmpty(existingFile))
                            {
                                var path = Path.Combine(Directory.GetCurrentDirectory(), "ZatcaInvoices", existingFile);
                                if (System.IO.File.Exists(path))
                                {
                                    System.IO.File.Delete(path);
                                }
                            }
                            var fileName = $"{operation["companyDetails"][0]["vATRegNumber"]}_{operation["operationDate"].ToString().Replace("-", "")}_{issueTime.ToString().Replace(":", "")}_{EInvoiceNo}.xml";
                            if (!string.IsNullOrWhiteSpace(normalInvoice.Data?.SignedXml))
                            {

                                var path = Path.Combine(Directory.GetCurrentDirectory(), "ZatcaInvoices", fileName);
                                Directory.CreateDirectory(Path.GetDirectoryName(path));
                                await System.IO.File.WriteAllTextAsync(path, normalInvoice.Data?.SignedXml);
                            }
                            var zatcaResponse = new
                            {
                                operationHeadId = operation["operationheadId"],
                                zatcaStatus = normalInvoice.Data.ReportingStatus,
                                isEInvoiceCleared = normalInvoice.Success ? 1 : 0,
                                zatcaErrors = JsonConvert.SerializeObject(normalInvoice.Data?.ErrorMessages?.ToList()),
                                zatcaMessage = normalInvoice.Data?.ReportingResult,
                                zatcaQRCode = normalInvoice.Data?.QrCode,
                                submissionDate = normalInvoice.Data?.SubmissionDate.ToString(),
                                zatcaWarningMessage = JsonConvert.SerializeObject(normalInvoice.Data?.WarningMessages.ToList()),
                                invoiceBase64 = normalInvoice.Data?.InvoiceHash,
                                signedXml = fileName
                            };
                            JObject obj = JObject.FromObject(zatcaResponse);
                            List<SqlParameter> zl = new List<SqlParameter>();
                            zl.Add(new SqlParameter("@User", GetCurrentUser()));
                            zl.Add(new SqlParameter("@json", obj.ToString()) { Size = -1 });
                            List<SqlParameter> listOutParameters1 = new List<SqlParameter>();
                            SqlParameter resPar1 = new SqlParameter() { ParameterName = "@Result", SqlDbType = SqlDbType.NVarChar, Size = -1, Direction = ParameterDirection.Output };
                            listOutParameters1.Add(resPar1);
                            SqlParameter errPar1 = new SqlParameter() { ParameterName = "@Error", SqlDbType = SqlDbType.NVarChar, Size = -1, Direction = ParameterDirection.Output };
                            listOutParameters1.Add(errPar1);

                            DataHelper.ExecScalar("sys_ZatcaResponseSave", System.Data.CommandType.StoredProcedure, ref listOutParameters1, zl.ToArray());
                            zerror = listOutParameters1.Find(X => X.ParameterName == "@Error").Value.ToString();

                            // if(!normalInvoice.Success) return StatusCode(500, normalInvoice.Data.ErrorMessages.ToString());

                        }
                        else
                        {
                            return StatusCode(500, normalInvoice.Data.ErrorMessages.ToString());
                        }




                    }


                }
                if (string.IsNullOrEmpty(result))
                {
                    return StatusCode(500, result);
                }


                return StatusCode(200, result);


            }
            catch
            {
                return StatusCode(500, "");
            }
        }

        [HttpPost("RepostOperationsToGL")]
        public ActionResult RepostOperationsToGL(DataObj dataObj)
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
                && x.Name.ToLower() != "selectedIDs"
                ).ToArray())
                {
                    pName = ((Newtonsoft.Json.Linq.JProperty)item).Name.ToString().ToLower();
                    headJson.Add(item);
                }

                //var attachments = oo["attachments"].ToArray();


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

                DataHelper.ExecScalar("mms_RepostOperationsToGL", System.Data.CommandType.StoredProcedure, ref listOutParameters, l.ToArray());
                string error = listOutParameters.Find(X => X.ParameterName == "@Error").Value.ToString();
                if (!string.IsNullOrEmpty(error))
                    return StatusCode(500, error);

                string result = listOutParameters.Find(X => X.ParameterName == "@Result").Value.ToString();

                if (string.IsNullOrEmpty(result))
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



        [HttpPost("SaveOperationHMS")]
        public async Task<ActionResult> SaveOperationHMS()

        {
            try
            {
                JObject parsedData = null;
                if (Request.ContentType.Contains("multipart/form-data"))
                {
                    var formHMS = await HttpContext.Request.ReadFormAsync();
                    var dataObjJsonHMS = formHMS["dataObj"].ToString();
                    parsedData = JObject.Parse(dataObjJsonHMS);
                }
                else if (Request.ContentType.Contains("application/x-www-form-urlencoded"))
                {
                    var formHMS = await HttpContext.Request.ReadFormAsync();
                    var dataObjJsonHMS = formHMS["dataObj"].ToString();
                    parsedData = JObject.Parse(dataObjJsonHMS);
                }
                else if (Request.ContentType.Contains("application/json"))
                {
                    var body = await new StreamReader(Request.Body).ReadToEndAsync();
                    parsedData = JObject.Parse(body);
                }
                else
                {
                    return StatusCode(StatusCodes.Status415UnsupportedMediaType, "Unsupported content type");
                }

                var dataObjJson = parsedData.ToString();
                var data = new DataObj { dataObj = dataObjJson };
                JObject oo = JObject.Parse(data.dataObj);
                List<SqlParameter> l = new List<SqlParameter>();
                string userCode = GetCurrentUser();
                l.Add(new SqlParameter("@Lang", ExtractLang()));
                l.Add(new SqlParameter("@json", data.dataObj) { Size = -1 });

                List<SqlParameter> listOutParameters = new List<SqlParameter>();
                SqlParameter resPar = new SqlParameter() { ParameterName = "@Result", SqlDbType = SqlDbType.NVarChar, Size = -1, Direction = ParameterDirection.Output };
                listOutParameters.Add(resPar);
                SqlParameter errPar = new SqlParameter() { ParameterName = "@Error", SqlDbType = SqlDbType.NVarChar, Size = -1, Direction = ParameterDirection.Output };
                listOutParameters.Add(errPar);


                DataHelper.ExecScalar("mms_OperationsSave_HMS", System.Data.CommandType.StoredProcedure, ref listOutParameters, l.ToArray());
                var error = listOutParameters.Find(X => X.ParameterName == "@Error").Value;
                if (error != null && !string.IsNullOrEmpty(error.ToString()))
                {
                    DataHelper.Log(error.ToString());
                    return StatusCode(400, error);
                }
                string result = listOutParameters.Find(X => X.ParameterName == "@Result").Value.ToString();
                JObject operationResult = JObject.Parse(result);
                if (string.IsNullOrEmpty(result))
                {
                    DataHelper.Log(result.ToString());
                    return StatusCode(500, result);
                }
                JObject o = JObject.Parse(result);
                JObject resultObj = JObject.Parse(result.ToString());
                var x = (Newtonsoft.Json.Linq.JValue)resultObj["ID"];
                DataHelper.Log(result.ToString());
                string strIsRSD = _configuration.GetSection("Leen")["IsRSD"];
                string strPostToRSD = _configuration.GetSection("Leen")["HmsPostToRSD"];
                int isRSD = int.Parse(strIsRSD);
                int postToRSD = int.Parse(strPostToRSD);
                dynamic hmsresponse = new System.Dynamic.ExpandoObject();
                if (isRSD != 0 && postToRSD != 0)
                {
                    try
                    {
                        var operationDetails = operationResult["operationDetails"].ToArray();
                        var rsdMethodName = operationResult["rsdMethodName"].ToString();
                        var ProductList = new List<ProductModel>();
                        for (int i = 0; i < operationDetails.Length; i++)
                        {
                            JObject objResult = (JObject)operationDetails[i];

                            var itemList = new ProductModel
                            {
                                BN = objResult["attribute1"].ToString(),
                                SN = objResult["attribute2"].ToString(),
                                GTIN = objResult["attribute3"].ToString(),
                                XD = DateTime.Parse(objResult["expireDate1"].ToString()),
                                Quantity = int.Parse(objResult["quantity"].ToString())
                            };
                            ProductList.Add(itemList);

                        }
                        string strConnectThrowLeen = _configuration.GetSection("Leen")["ConnectThrowLeen"];
                        int id = 0;

                        int.TryParse(x.ToString(), out id);
                        int connectThrowLeen = 0;
                        int.TryParse(strConnectThrowLeen, out connectThrowLeen);
                        if (connectThrowLeen == 0 && isRSD != 0)//Connect throw Julib
                        {
                            new Thread(() =>
                            {
                                Thread.CurrentThread.IsBackground = true;
                                _rsd.post(id, userCode);
                            }).Start();
                        }
                        else
                        {
                            // _rsd.PharmacySale(id, userCode);
                            await RsdIntegrationHelper.SetCurrentCredentialsAsync(new CredentialsModel
                            {
                                UserName = resultObj["rsdUserDetails"][0]["rsdUser"].ToString(),
                                Password = resultObj["rsdUserDetails"][0]["rsdPassword"].ToString()
                            });

                            //var rsdResponse = new PostRSDResponseDto();
                            //var rsdPharmacySaleCancelResponse = await RsdIntegrationHelper.PharmacySale(new PharmacySaleModel
                            //{
                            //    Products = ProductList,
                            //    TOGLN = "0000000000000"
                            //});
                            //if (rsdPharmacySaleCancelResponse != null)
                            //{
                            //    rsdResponse.OperationHeadId = resultObj["ID"].ToString();
                            //    rsdResponse.NotificationId = rsdPharmacySaleCancelResponse.NotificationId.ToString();
                            //    rsdResponse.RSDStatus = rsdPharmacySaleCancelResponse.ProductList.Where(x => x.Code != "0000").Any() ? false : true;
                            //    rsdResponse.RSDResponseProductList = JsonConvert.SerializeObject(rsdPharmacySaleCancelResponse.ProductList.ToList());
                            //    rsdResponse.CreadtedBy = GetCurrentUser();
                            //    rsdResponse.PostedDate = DateTime.Now;

                            //    if (rsdResponse != null)
                            //    {


                            //        var rsddata = await _generalService.PostRsdResponse(rsdResponse);
                            //        if (rsddata.Code == System.Net.HttpStatusCode.OK)
                            //        {

                            //            hmsresponse.ID = x;
                            //            hmsresponse.status = "Saved Succefully";
                            //            return StatusCode(200, hmsresponse);
                            //        }
                            //    }

                            //}

                            var rsdResponse = new PostRSDResponseDto();
                            switch (rsdMethodName.ToString())
                            {
                                case "PharmacySale":
                                    var RsdPharmacySalesResult = await RsdIntegrationHelper.PharmacySale(new PharmacySaleModel
                                    {
                                        Products = ProductList,
                                        TOGLN = "0000000000000"
                                    });
                                    if (RsdPharmacySalesResult != null)
                                    {
                                        rsdResponse.OperationHeadId = resultObj["ID"].ToString();
                                        rsdResponse.NotificationId = RsdPharmacySalesResult.NotificationId.ToString();
                                        rsdResponse.RSDStatus = RsdPharmacySalesResult.ProductList.Where(x => x.Code != "0000").Any() ? false : true;
                                        rsdResponse.RSDResponseProductList = JsonConvert.SerializeObject(RsdPharmacySalesResult.ProductList.ToList());
                                        rsdResponse.CreadtedBy = GetCurrentUser();
                                        rsdResponse.PostedDate = DateTime.Now;

                                        if (rsdResponse != null)
                                        {


                                            var rsddata = await _generalService.PostRsdResponse(rsdResponse);
                                            if (rsddata.Code == System.Net.HttpStatusCode.OK)
                                            {

                                                hmsresponse.ID = x;
                                                hmsresponse.status = "Saved Succefully";
                                                return StatusCode(200, hmsresponse);
                                            }
                                        }

                                    }
                                    break;

                                case "PharmacySaleCancel":
                                    //_rsd.PharmacySaleCancel(id, userCode);
                                    var rsdPharmacySaleCancelResponse = await RsdIntegrationHelper.PharmacyCancelSale(new PharmacyCancelSaleModel
                                    {
                                        Products = ProductList,
                                        //TOGLN = "0000000000000"
                                        TOGLN = resultObj["rsdUserDetails"][0]["gln"].ToString()
                                    });
                                    if (rsdPharmacySaleCancelResponse != null)
                                    {
                                        rsdResponse.OperationHeadId = resultObj["ID"].ToString();
                                        rsdResponse.NotificationId = rsdPharmacySaleCancelResponse.NotificationId.ToString();
                                        rsdResponse.RSDStatus = rsdPharmacySaleCancelResponse.ProductList.Where(x => x.Code != "0000").Any() ? false : true;
                                        rsdResponse.RSDResponseProductList = JsonConvert.SerializeObject(rsdPharmacySaleCancelResponse.ProductList.ToList());
                                        rsdResponse.CreadtedBy = GetCurrentUser();
                                        rsdResponse.PostedDate = DateTime.Now;

                                        if (rsdResponse != null)
                                        {
                                            var rsddata = await _generalService.PostRsdResponse(rsdResponse);
                                            if (rsddata.Code == System.Net.HttpStatusCode.OK)
                                            {

                                                hmsresponse.ID = x;
                                                hmsresponse.status = "Saved Succefully";
                                                return StatusCode(200, hmsresponse);
                                            }
                                        }

                                    }
                                    break;

                                case "Consume":
                                    var rsdConsumeResponse = await RsdIntegrationHelper.Consume(new ConsumeModel { Products = ProductList, });
                                    if (rsdConsumeResponse != null)
                                    {
                                        rsdResponse.OperationHeadId = resultObj["ID"].ToString();
                                        rsdResponse.NotificationId = rsdConsumeResponse.NotificationId.ToString();
                                        rsdResponse.RSDStatus = rsdConsumeResponse.ProductList.Where(x => x.Code != "0000").Any() ? false : true;
                                        rsdResponse.RSDResponseProductList = JsonConvert.SerializeObject(rsdConsumeResponse.ProductList.ToList());
                                        rsdResponse.CreadtedBy = GetCurrentUser();
                                        rsdResponse.PostedDate = DateTime.Now;
                                    }
                                    if (rsdResponse != null)
                                    {
                                        var rsddata = await _generalService.PostRsdResponse(rsdResponse);
                                        if (rsddata.Code == System.Net.HttpStatusCode.OK)
                                        {

                                            hmsresponse.ID = x;
                                            hmsresponse.status = "Saved Succefully";
                                            return StatusCode(200, hmsresponse);
                                        }
                                    }

                                    break;
                                case "ConsumeCancel":
                                    var rsdConsumeCancelResponse = await RsdIntegrationHelper.ConsumeCancel(new ConsumeCancelModel
                                    {
                                        Products = ProductList
                                    });
                                    if (rsdConsumeCancelResponse != null)
                                    {
                                        rsdResponse.OperationHeadId = resultObj["ID"].ToString();
                                        rsdResponse.NotificationId = rsdConsumeCancelResponse.NotificationId.ToString();
                                        rsdResponse.RSDStatus = rsdConsumeCancelResponse.ProductList.Where(x => x.Code != "0000").Any() ? false : true;
                                        rsdResponse.RSDResponseProductList = JsonConvert.SerializeObject(rsdConsumeCancelResponse.ProductList.ToList());
                                        rsdResponse.CreadtedBy = GetCurrentUser();
                                        rsdResponse.PostedDate = DateTime.Now;
                                    }
                                    if (rsdResponse != null)
                                    {
                                        var rsddata = await _generalService.PostRsdResponse(rsdResponse);
                                        if (rsddata.Code == System.Net.HttpStatusCode.OK)
                                        {

                                            hmsresponse.ID = x;
                                            hmsresponse.status = "Saved Succefully";
                                            return StatusCode(200, hmsresponse);
                                        }
                                    }
                                    break;

                                default:
                                    break;
                            }

                        }
                        hmsresponse.ID = x;
                        hmsresponse.status = "Saved Succefully";
                        return StatusCode(200, hmsresponse);

                    }
                    catch (Exception ex)
                    {
                        hmsresponse.ID = x;
                        hmsresponse.status = "Saved Succefully";
                        return StatusCode(200, hmsresponse);
                    }
                }
                hmsresponse.ID = x;
                hmsresponse.status = "Saved Succefully";
                return StatusCode(200, hmsresponse);
            }
            catch (Exception ex)
            {

                DataHelper.Log(ex.Message);
                return StatusCode(500, ex.Message);
            }
        }


       
        [HttpPost("CancelOperationHMS")]
        public async Task<ActionResult> CancelOperationHMS()
        {
            try
            {
                JObject parsedData = null;
                if (Request.ContentType.Contains("multipart/form-data"))
                {
                    var formHMS = await HttpContext.Request.ReadFormAsync();
                    var dataObjJsonHMS = formHMS["dataObj"].ToString();
                    parsedData = JObject.Parse(dataObjJsonHMS);
                }
                else if (Request.ContentType.Contains("application/x-www-form-urlencoded"))
                {
                    var formHMS = await HttpContext.Request.ReadFormAsync();
                    var dataObjJsonHMS = formHMS["dataObj"].ToString();
                    parsedData = JObject.Parse(dataObjJsonHMS);
                }
                else if (Request.ContentType.Contains("application/json"))
                {
                    var body = await new StreamReader(Request.Body).ReadToEndAsync();
                    parsedData = JObject.Parse(body);
                }
                else
                {
                    return StatusCode(StatusCodes.Status415UnsupportedMediaType, "Unsupported content type");
                }

                var dataObjJson = parsedData.ToString();
                var data = new DataObj { dataObj = dataObjJson };
                JObject oo = JObject.Parse(data.dataObj);

                List<SqlParameter> l = new List<SqlParameter>();
                string userCode = GetCurrentUser();
                //l.Add(new SqlParameter("@UserId", userCode));
                l.Add(new SqlParameter("@Lang", ExtractLang()));
                l.Add(new SqlParameter("@json", data.dataObj) { Size = -1 });

                List<SqlParameter> listOutParameters = new List<SqlParameter>();
                SqlParameter resPar = new SqlParameter() { ParameterName = "@Result", SqlDbType = SqlDbType.NVarChar, Size = -1, Direction = ParameterDirection.Output };
                listOutParameters.Add(resPar);
                SqlParameter errPar = new SqlParameter() { ParameterName = "@Error", SqlDbType = SqlDbType.NVarChar, Size = -1, Direction = ParameterDirection.Output };
                listOutParameters.Add(errPar);

                DataHelper.ExecScalar("mms_OperationsSave_HMS_Cancel", System.Data.CommandType.StoredProcedure, ref listOutParameters, l.ToArray());
                var error = listOutParameters.Find(X => X.ParameterName == "@Error").Value;
                if (error != null && !string.IsNullOrEmpty(error.ToString()))
                {
                    DataHelper.Log(error.ToString());
                    return StatusCode(400, error);
                }
                string result = listOutParameters.Find(X => X.ParameterName == "@Result").Value.ToString();
                JObject operationResult = JObject.Parse(result);
                if (string.IsNullOrEmpty(result))
                {
                    DataHelper.Log(result.ToString());
                    return StatusCode(500, result);
                }
                JObject o = JObject.Parse(result);
                JObject resultObj = JObject.Parse(result.ToString());
                var x = (Newtonsoft.Json.Linq.JValue)resultObj["ID"];
                string strIsRSD = _configuration.GetSection("Leen")["IsRSD"];
                string strPostToRSD = _configuration.GetSection("Leen")["HmsPostToRSD"];
                int isRSD = int.Parse(strIsRSD);
                dynamic hmsresponse = new System.Dynamic.ExpandoObject();
                int postToRSD = int.Parse(strPostToRSD);
                if (isRSD != 0 && postToRSD != 0)
                {
                    try
                    {
                        var operationDetails = operationResult["operationDetails"].ToArray();
                        var rsdMethodName = operationResult["rsdMethodName"].ToString();
                        var ProductList = new List<ProductModel>();
                        for (int i = 0; i < operationDetails.Length; i++)
                        {
                            JObject objResult = (JObject)operationDetails[i];

                            var itemList = new ProductModel
                            {
                                BN = objResult["attribute1"].ToString(),
                                SN = objResult["attribute2"].ToString(),
                                GTIN = objResult["attribute3"].ToString(),
                                XD = DateTime.Parse(objResult["expireDate1"].ToString()),
                                Quantity = int.Parse(objResult["quantity"].ToString())
                            };
                            ProductList.Add(itemList);
                        }
                        int id = 0;
                        string strConnectThrowLeen = _configuration.GetSection("Leen")["ConnectThrowLeen"];
                        int connectThrowLeen = 0;
                        int.TryParse(strConnectThrowLeen, out connectThrowLeen);

                        int.TryParse(x.ToString(), out id);
                        if (connectThrowLeen == 0 && isRSD != 0)
                        {
                            new Thread(() =>
                            {
                                Thread.CurrentThread.IsBackground = true;
                                _rsd.post(id, userCode);
                            }).Start();
                        }
                        else
                        {
                            await RsdIntegrationHelper.SetCurrentCredentialsAsync(new CredentialsModel
                            {
                                UserName = resultObj["rsdUserDetails"][0]["rsdUser"].ToString(),
                                Password = resultObj["rsdUserDetails"][0]["rsdPassword"].ToString()
                            });

                            var rsdResponse = new PostRSDResponseDto();
                            switch (rsdMethodName.ToString())
                            {
                                case "PharmacySaleCancel":
                                    //_rsd.PharmacySaleCancel(id, userCode);
                                    var rsdPharmacySaleCancelResponse = await RsdIntegrationHelper.PharmacyCancelSale(new PharmacyCancelSaleModel
                                    {
                                        Products = ProductList,
                                        //TOGLN = "0000000000000"
                                        TOGLN = resultObj["rsdUserDetails"][0]["gln"].ToString()
                                    });
                                    if (rsdPharmacySaleCancelResponse != null)
                                    {
                                        rsdResponse.OperationHeadId = resultObj["ID"].ToString();
                                        rsdResponse.NotificationId = rsdPharmacySaleCancelResponse.NotificationId.ToString();
                                        rsdResponse.RSDStatus = rsdPharmacySaleCancelResponse.ProductList.Where(x => x.Code != "0000").Any() ? false : true;
                                        rsdResponse.RSDResponseProductList = JsonConvert.SerializeObject(rsdPharmacySaleCancelResponse.ProductList.ToList());
                                        rsdResponse.CreadtedBy = GetCurrentUser();
                                        rsdResponse.PostedDate = DateTime.Now;

                                        if (rsdResponse != null)
                                        {
                                            var rsddata = await _generalService.PostRsdResponse(rsdResponse);
                                            if (rsddata.Code == System.Net.HttpStatusCode.OK)
                                            {

                                                hmsresponse.ID = x;
                                                hmsresponse.status = "Saved Succefully";
                                                return StatusCode(200, hmsresponse);
                                            }
                                        }

                                    }
                                    break;
                                case "ConsumeCancel":
                                    var rsdConsumeCancelResponse = await RsdIntegrationHelper.ConsumeCancel(new ConsumeCancelModel
                                    {
                                        Products = ProductList
                                    });
                                    if (rsdConsumeCancelResponse != null)
                                    {
                                        rsdResponse.OperationHeadId = resultObj["ID"].ToString();
                                        rsdResponse.NotificationId = rsdConsumeCancelResponse.NotificationId.ToString();
                                        rsdResponse.RSDStatus = rsdConsumeCancelResponse.ProductList.Where(x => x.Code != "0000").Any() ? false : true;
                                        rsdResponse.RSDResponseProductList = JsonConvert.SerializeObject(rsdConsumeCancelResponse.ProductList.ToList());
                                        rsdResponse.CreadtedBy = GetCurrentUser();
                                        rsdResponse.PostedDate = DateTime.Now;
                                    }
                                    if (rsdResponse != null)
                                    {
                                        var rsddata = await _generalService.PostRsdResponse(rsdResponse);
                                        if (rsddata.Code == System.Net.HttpStatusCode.OK)
                                        {

                                            hmsresponse.ID = x;
                                            hmsresponse.status = "Saved Succefully";
                                            return StatusCode(200, hmsresponse);
                                        }
                                    }
                                    break;
                                default:
                                    break;
                            }

                        }
                        hmsresponse.ID = x;
                        hmsresponse.status = "Saved Succefully";
                        return StatusCode(200, hmsresponse);
                    }
                    catch (Exception ex)
                    {
                        hmsresponse.ID = x;
                        hmsresponse.status = "Saved Succefully";
                        return StatusCode(200, hmsresponse);
                    }
                }
                hmsresponse.ID = x;
                hmsresponse.status = "Saved Succefully";
                return StatusCode(200, hmsresponse);
            }
            catch (Exception ex)
            {
                DataHelper.Log(ex.Message);
                return StatusCode(500, ex.Message);
            }
        }




        [HttpPost("SaveOperationExpensesAndRepost")]
        public ActionResult SaveOperationExpensesAndRepost(DataObj dataObj)
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
                //var parent = JsonConvert.DeserializeObject<JObject>(dataObj.dataObj);
                var headJson = new JObject();
                string pName = "";
                foreach (var item in oo.Properties().Where(x => x.Name.ToLower() != "images"
                && x.Name.ToLower() != "attachments" && x.Name.ToLower() != "operationDetails".ToLower()
                && x.Name.ToLower() != "operationExpenses".ToLower() && x.Name.ToLower() != "operationsDetailsSpecificAttributes".ToLower()
                ).ToArray())
                {
                    pName = ((Newtonsoft.Json.Linq.JProperty)item).Name.ToString().ToLower();
                    //if (pName != "images" && pName != "files")
                    //{
                    headJson.Add(item);
                    //}
                }
                var operationExpenses = oo["operationExpenses"];

                JObject newJson = new JObject();
                newJson.Add("Head", headJson);
                newJson.Add("operationExpenses", operationExpenses);





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

                DataHelper.ExecScalar("mms_UpdateOperationExpensesAndRepost", System.Data.CommandType.StoredProcedure, ref listOutParameters, l.ToArray());
                string error = listOutParameters.Find(X => X.ParameterName == "@Error").Value.ToString();
                if (!string.IsNullOrEmpty(error))
                    return StatusCode(500, error);

                string result = listOutParameters.Find(X => X.ParameterName == "@Result").Value.ToString();

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
        #endregion

        [HttpGet("GetLastSequence")]
        public ActionResult GetLastSequence(int LocationId)
        {
            try
            {

                List<SqlParameter> l = new List<SqlParameter>();
                l.Add(new SqlParameter("@FormId", ExtractFormId()));
                l.Add(new SqlParameter("@LocationID", LocationId));
                List<SqlParameter> listOutPutParams = new List<SqlParameter>();
                listOutPutParams.Add(new SqlParameter() { ParameterName = "@OLastSeq", SqlDbType = SqlDbType.Int, Direction = ParameterDirection.Output });

                DataHelper.ExecScalar("sys_GetFormLastSequence", System.Data.CommandType.StoredProcedure, ref listOutPutParams, l.ToArray());
                string lastSeq = listOutPutParams[0].Value.ToString();
                return StatusCode(200, lastSeq);
            }
            catch (Exception ex)
            {

                return StatusCode(500, ex.Message);
            }


        }

        [HttpGet("GetUnits")]
        public ActionResult GetUnits()
        {
            try
            {
                List<SqlParameter> l = new List<SqlParameter>();
                l.Add(new SqlParameter("@User", GetCurrentUser()));
                l.Add(new SqlParameter("@FormId", ExtractFormId()));
                l.Add(new SqlParameter("@Lang", ExtractLang()));

                var x = DataHelper.ExecScalar("mms_GetUnits", System.Data.CommandType.StoredProcedure, l.ToArray());
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
        [HttpGet("GetExpenses")]
        public ActionResult GetExpenses()
        {
            try
            {
                List<SqlParameter> l = new List<SqlParameter>();
                l.Add(new SqlParameter("@User", GetCurrentUser()));
                l.Add(new SqlParameter("@FormId", ExtractFormId()));
                l.Add(new SqlParameter("@Lang", ExtractLang()));

                var x = DataHelper.ExecScalar("mms_GetExpenses", System.Data.CommandType.StoredProcedure, l.ToArray());
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

        [HttpGet("GetItemKitIdentifiers")]
        public ActionResult GetItemKitIdentifiers(int ItemId, int BaseUomQty)
        {
            try
            {
                List<SqlParameter> l = new List<SqlParameter>();
                l.Add(new SqlParameter("@User", GetCurrentUser()));
                l.Add(new SqlParameter("@FormId", ExtractFormId()));
                l.Add(new SqlParameter("@Lang", ExtractLang()));
                l.Add(new SqlParameter("@ItemId", ItemId));
                l.Add(new SqlParameter("@BaseUnitofMeasurementEquivalentQty", BaseUomQty));

                var x = DataHelper.ExecScalar("mms_GetItemKitIdentifiers", System.Data.CommandType.StoredProcedure, l.ToArray());
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

        [HttpGet("GetItemExpenses")]
        public ActionResult GetItemExpenses(int ItemId)
        {
            try
            {
                List<SqlParameter> l = new List<SqlParameter>();
                l.Add(new SqlParameter("@User", GetCurrentUser()));
                l.Add(new SqlParameter("@FormId", ExtractFormId()));
                l.Add(new SqlParameter("@Lang", ExtractLang()));
                l.Add(new SqlParameter("@ItemId", ItemId));

                var x = DataHelper.ExecScalar("mms_GetItemExpenses", System.Data.CommandType.StoredProcedure, l.ToArray());
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
        [HttpGet("srchExpenses")]
        public ActionResult srchExpenses(String ExciptIds = "", string Filter = "")
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

                var x = DataHelper.ExecScalar("mms_srchExpenses", System.Data.CommandType.StoredProcedure, l.ToArray());
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

        [HttpGet("DeleteOperation")]
        public ActionResult DeleteOperation(int OperationHeadId)
        {
            try
            {

                List<SqlParameter> l = new List<SqlParameter>();
                l.Add(new SqlParameter("@User", GetCurrentUser()));
                l.Add(new SqlParameter("@FormId", ExtractFormId()));
                l.Add(new SqlParameter("@Lang", ExtractLang()));
                l.Add(new SqlParameter("@ID", OperationHeadId));

                List<SqlParameter> listOutParameters = new List<SqlParameter>();
                SqlParameter errPar = new SqlParameter() { ParameterName = "@Error", SqlDbType = SqlDbType.NVarChar, Size = -1, Direction = ParameterDirection.Output };
                listOutParameters.Add(errPar);
                var x = DataHelper.ExecScalar("mms_OperationsDelete", System.Data.CommandType.StoredProcedure, ref listOutParameters, l.ToArray());
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
        [HttpGet("GetUserOperationStatuses")]
        public ActionResult GetUserOperationStatuses(int? OperationTypeID)
        {
            try
            {

                List<SqlParameter> l = new List<SqlParameter>();
                l.Add(new SqlParameter("@User", GetCurrentUser()));
                l.Add(new SqlParameter("@FormId", ExtractFormId()));
                l.Add(new SqlParameter("@Lang", ExtractLang()));
                if (OperationTypeID != null || OperationTypeID != 0)
                    l.Add(new SqlParameter("@OperationTypeID", OperationTypeID));
                var x = DataHelper.ExecScalar("mms_GetUserOperationStatuses", System.Data.CommandType.StoredProcedure, l.ToArray());
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

        [HttpPost("BulkStatusApproval")]
        public ActionResult BulkStatusApproval(DataObj dataObj)
        {
            try
            {
                try
                {
                    //DataHelper.Log(DataHelper.GetHeaders(Request));
                    DataHelper.Log(dataObj.dataObj);
                }
                catch (Exception ex)
                {

                }
                JObject oo = JObject.Parse(dataObj.dataObj);
                JObject newJson = new JObject();
                newJson.Add("Head", oo);
                List<SqlParameter> l = new List<SqlParameter>();
                l.Add(new SqlParameter("@User", GetCurrentUser()));
                l.Add(new SqlParameter("@Lang", ExtractLang()));
                l.Add(new SqlParameter("@json", newJson.ToString()) { Size = -1 });

                List<SqlParameter> listOutParameters = new List<SqlParameter>();
                SqlParameter resPar = new SqlParameter() { ParameterName = "@Result", SqlDbType = SqlDbType.NVarChar, Size = -1, Direction = ParameterDirection.Output };
                listOutParameters.Add(resPar);
                SqlParameter errPar = new SqlParameter() { ParameterName = "@Error", SqlDbType = SqlDbType.NVarChar, Size = -1, Direction = ParameterDirection.Output };
                listOutParameters.Add(errPar);

                DataHelper.ExecScalar("mms_BulkOperationsApproval", System.Data.CommandType.StoredProcedure, ref listOutParameters, l.ToArray());
                string error = listOutParameters.Find(X => X.ParameterName == "@Error").Value.ToString();
                if (!string.IsNullOrEmpty(error))
                    return StatusCode(500, error);

                string result = listOutParameters.Find(X => X.ParameterName == "@Result").Value.ToString();

                if (string.IsNullOrEmpty(result))
                {
                    return StatusCode(500, result);
                }


                return StatusCode(200, result);


            }
            catch
            {
                return StatusCode(500, "");
            }
        }



        [HttpPost("GeneratePurchaseOrders")]
        public ActionResult GeneratePurchaseOrders(DataObj dataObj)
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

                DataHelper.ExecScalar("mms_GeneratePurchaseOrders", System.Data.CommandType.StoredProcedure, ref listOutParameters, l.ToArray());
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

        [HttpPost("GetAutoDiscounts")]
        public ActionResult GetAutoDiscounts(DataObj dataObj)
        {
            try
            {

                Newtonsoft.Json.Linq.JObject oo = JObject.Parse(dataObj.dataObj);

                List<SqlParameter> l = new List<SqlParameter>();
                l.Add(new SqlParameter("@User", GetCurrentUser()));
                l.Add(new SqlParameter("@FormId", ExtractFormId()));
                l.Add(new SqlParameter("@Lang", ExtractLang()));
                l.Add(new SqlParameter("@json", dataObj.dataObj) { Size = -1 });

                List<SqlParameter> listOutParameters = new List<SqlParameter>();
                listOutParameters.Add(new SqlParameter() { ParameterName = "@Result", SqlDbType = SqlDbType.NVarChar, Size = -1, Direction = ParameterDirection.Output });
                //listOutParameters.Add(new SqlParameter() { ParameterName = "@ResultDiscount2", SqlDbType = SqlDbType.NVarChar, Size = -1, Direction = ParameterDirection.Output });
                //listOutParameters.Add(new SqlParameter() { ParameterName = "@ResultDiscount3", SqlDbType = SqlDbType.NVarChar, Size = -1, Direction = ParameterDirection.Output });

                DataHelper.ExecScalar("mms_OperationsGetAutoDiscounts", System.Data.CommandType.StoredProcedure, ref listOutParameters, l.ToArray());
                string result = listOutParameters[0].Value.ToString();
                if (string.IsNullOrEmpty(result.ToString()))
                {
                    return StatusCode(200, "");
                }
                JObject o = JObject.Parse(result);
                return StatusCode(200, o);
            }
            catch (Exception ex)
            {

                DataHelper.Log(ex.Message);
                return StatusCode(500, ex.Message);
            }


        }

        [HttpGet("GetRsdNotificationDetailsFromSP")]
        public ActionResult GetRsdNotificationDetailsFromSP(string NotificationID)
        {
            try
            {
                List<SqlParameter> l = new List<SqlParameter>();
                l.Add(new SqlParameter("@User", GetCurrentUser()));
                l.Add(new SqlParameter("@FormId", ExtractFormId()));
                l.Add(new SqlParameter("@Lang", ExtractLang()));
                l.Add(new SqlParameter("@NotificationID", NotificationID));

                var x = DataHelper.ExecScalar("rsd_GetRsdNotificationDetails", System.Data.CommandType.StoredProcedure, l.ToArray());
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



        [HttpGet("GetItemsFromRsdUsingNotificationId")]
        public async Task<ActionResult> GetItemsFromRsdUsingNotificationId(string NotificationID, int LocationId)
        {
            try
            {
                if (LocationId > 0)
                {
                    var result = await _locationService.GetUserForLocation(LocationId);
                    if (result.Code == System.Net.HttpStatusCode.OK && result.Object != null)
                    {
                        var userDetails = result.Object;
                        await RsdIntegrationHelper.SetCurrentCredentialsAsync(new CredentialsModel
                        {
                            UserName = userDetails.UserName,
                            Password = userDetails.Password
                        });
                    }
                    else
                    {
                        return StatusCode(500, "Unable to Find Rsd User details for this location");
                    }

                }
                var response = await RsdIntegrationHelper.AcceptDispatch(new AcceptDispatchModel
                {
                    DispatchNotificationId = NotificationID
                });
                if (response.Status)
                {
                    var resultObj = new
                    {
                        responseProductList = JsonConvert.SerializeObject(response.ProductList),
                        notificationId = response.NotificationId
                    };
                    JObject obj = JObject.FromObject(resultObj);
                    List<SqlParameter> l = new List<SqlParameter>();
                    string userCode = GetCurrentUser();
                    l.Add(new SqlParameter("@RequestNotificationid", NotificationID));
                    l.Add(new SqlParameter("@ResponseNotificationid", resultObj.notificationId));
                    l.Add(new SqlParameter("@User", userCode));
                    l.Add(new SqlParameter("@Lang", ExtractLang()));
                    l.Add(new SqlParameter("@json", obj.ToString()) { Size = -1 });

                    List<SqlParameter> listOutParameters = new List<SqlParameter>();
                    SqlParameter resPar = new SqlParameter() { ParameterName = "@Result", SqlDbType = SqlDbType.NVarChar, Size = -1, Direction = ParameterDirection.Output };
                    listOutParameters.Add(resPar);
                    SqlParameter errPar = new SqlParameter() { ParameterName = "@Error", SqlDbType = SqlDbType.NVarChar, Size = -1, Direction = ParameterDirection.Output };
                    listOutParameters.Add(errPar);


                    DataHelper.ExecScalar("sys_RsdDISPATCHSave", System.Data.CommandType.StoredProcedure, ref listOutParameters, l.ToArray());
                    string error = listOutParameters.Find(X => X.ParameterName == "@Error").Value.ToString();
                    if (!string.IsNullOrEmpty(error))
                        return StatusCode(500, error);

                    string result = listOutParameters.Find(X => X.ParameterName == "@Result").Value.ToString();
                    //JObject o = JObject.Parse(result);
                    if (string.IsNullOrEmpty(result))
                    {
                        return StatusCode(500, result);
                    }

                    return StatusCode(200, result);

                }
                else
                {
                    List<SqlParameter> l = new List<SqlParameter>();
                    string userCode = GetCurrentUser();
                    l.Add(new SqlParameter("@Notificationid", NotificationID));
                    l.Add(new SqlParameter("@User", userCode));
                    l.Add(new SqlParameter("@Lang", ExtractLang()));

                    List<SqlParameter> listOutParameters = new List<SqlParameter>();
                    SqlParameter resPar = new SqlParameter() { ParameterName = "@Result", SqlDbType = SqlDbType.NVarChar, Size = -1, Direction = ParameterDirection.Output };
                    listOutParameters.Add(resPar);
                    SqlParameter errPar = new SqlParameter() { ParameterName = "@Error", SqlDbType = SqlDbType.NVarChar, Size = -1, Direction = ParameterDirection.Output };
                    listOutParameters.Add(errPar);

                    DataHelper.ExecScalar("sys_ReadRsdDISPATCH", System.Data.CommandType.StoredProcedure, ref listOutParameters, l.ToArray());
                    string error = listOutParameters.Find(X => X.ParameterName == "@Error").Value.ToString();
                    if (!string.IsNullOrEmpty(error))
                        return StatusCode(500, error);

                    string rsdresult = listOutParameters.Find(X => X.ParameterName == "@Result").Value.ToString();
                    if (string.IsNullOrEmpty(rsdresult))
                    {
                        return StatusCode(500, rsdresult);
                    }

                    return StatusCode(200, rsdresult);
                }
                //return StatusCode(500, $"Data for NotificationId {NotificationID} not Found");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Unable to Process Request Error" + ex.Message);
            }
        }

        [HttpGet("GetRsdNotificationDetails")]
        public ActionResult GetRsdNotificationDetails(string NotificationNo, int LocationId)
        {
            try
            {

                var x = _rsd.getDispatchInfo_juleb(NotificationNo, LocationId, GetCurrentUser());
                if (string.IsNullOrEmpty(x))
                {
                    return StatusCode(400, "No Data");
                }
                return StatusCode(200, JArray.Parse(x));
            }
            catch (Exception ex)
            {

                return StatusCode(500, ex.Message);
            }


        }

        [HttpPost("GetItemsByDispatchInfo")]
        public ActionResult GetItemsByDispatchInfo(DataObj dataObj)
        {
            try
            {

                Newtonsoft.Json.Linq.JObject oo = JObject.Parse(dataObj.dataObj);

                List<SqlParameter> l = new List<SqlParameter>();
                l.Add(new SqlParameter("@User", GetCurrentUser()));
                l.Add(new SqlParameter("@FormId", ExtractFormId()));
                l.Add(new SqlParameter("@Lang", ExtractLang()));
                l.Add(new SqlParameter("@json", dataObj.dataObj) { Size = -1 });

                List<SqlParameter> listOutParameters = new List<SqlParameter>();
                listOutParameters.Add(new SqlParameter() { ParameterName = "@Result", SqlDbType = SqlDbType.NVarChar, Size = -1, Direction = ParameterDirection.Output });
                //listOutParameters.Add(new SqlParameter() { ParameterName = "@ResultDiscount2", SqlDbType = SqlDbType.NVarChar, Size = -1, Direction = ParameterDirection.Output });
                //listOutParameters.Add(new SqlParameter() { ParameterName = "@ResultDiscount3", SqlDbType = SqlDbType.NVarChar, Size = -1, Direction = ParameterDirection.Output });

                DataHelper.ExecScalar("mms_GetItemsByDispatchInfo", System.Data.CommandType.StoredProcedure, ref listOutParameters, l.ToArray());
                string result = listOutParameters[0].Value.ToString();
                if (string.IsNullOrEmpty(result.ToString()))
                {
                    return StatusCode(200, "");
                }
                JObject o = JObject.Parse(result);
                return StatusCode(200, o);
            }
            catch (Exception ex)
            {

                DataHelper.Log(ex.Message);
                return StatusCode(500, ex.Message);
            }


        }

        [HttpGet("PTSQuery")]

        public ActionResult PTSQuery(string toGln, string fromDate, string toDate, string fromGLN = "", int locationId = 0)
        {
            try
            {
                string strResult = _rsd.PTSQuery(locationId, "sa", Convert.ToDateTime(fromDate), Convert.ToDateTime(toDate), toGln/*"6823075100001"*/, fromGLN);

                if (string.IsNullOrEmpty(strResult))
                { return StatusCode(500, "No Data found."); }

                JObject o = JObject.Parse(strResult);
                return StatusCode(200, o);
            }
            catch (Exception ex)
            {

                DataHelper.Log(ex.Message);
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("RSDDownloadPTS")]
        public HttpResponseMessage RSDDownloadPTS(long transferId, int locationId)
        {
            try
            {

                //r.PTSQuery(0, "sa", new DateTime(2023, 7, 1), new DateTime(2023, 7, 31), "6823075100001");
                return _rsd.PTSDownLoad(transferId, locationId);

            }
            catch (Exception ex)
            {
                DataHelper.Log(ex.Message);
                return null;

            }
        }

        [HttpGet("SendToRsd")]
        public ActionResult SendToRsd(int OperationHeadId)
        {
            try
            {
                try
                {

                    List<SqlParameter> l = new List<SqlParameter>();
                    string userCode = GetCurrentUser();
                    l.Add(new SqlParameter("@User", userCode));
                    l.Add(new SqlParameter("@FormId", ExtractFormId()));
                    l.Add(new SqlParameter("@Lang", ExtractLang()));
                    l.Add(new SqlParameter("@ID", OperationHeadId));


                    List<SqlParameter> listOutParameters = new List<SqlParameter>();
                    SqlParameter resPar = new SqlParameter() { ParameterName = "@Result", SqlDbType = SqlDbType.NVarChar, Size = -1, Direction = ParameterDirection.Output };
                    listOutParameters.Add(resPar);
                    DataHelper.ExecScalar("mms_GetRsdMethodName", System.Data.CommandType.StoredProcedure, ref listOutParameters, l.ToArray());

                    string rsdMethodName = listOutParameters.Find(X => X.ParameterName == "@Result").Value.ToString();
                    if (string.IsNullOrEmpty(rsdMethodName))
                    {
                        return StatusCode(500, "Missing Operation type configuration");
                    }


                    int id = OperationHeadId;

                    if (rsdMethodName != null && !string.IsNullOrEmpty(rsdMethodName.ToString()))
                    {
                        switch (rsdMethodName.ToString())
                        {
                            case "InvAccept":
                                _rsd.InvAccept(id, userCode);
                                break;
                            case "PharmacySale":
                                _rsd.PharmacySale(id, userCode);
                                break;
                            case "PharmacySaleCancel":
                                _rsd.PharmacySaleCancel(id, userCode);
                                break;
                            case "Deactivat":
                                _rsd.Deactivat(id, userCode, "30");
                                break;
                            case "DeactivatCancel":
                                _rsd.DeactivatCancel(id, userCode);
                                break;
                            case "Transfer":
                                _rsd.Transfer(id, userCode);
                                break;
                            case "TransferCancel":
                                _rsd.TransferCancel(id, userCode);
                                break;
                            case "Return":
                                _rsd.Return(id, userCode);
                                break;
                            case "Consume":
                                _rsd.Consume(id, userCode);
                                break;
                            case "ConsumeCancel":
                                _rsd.ConsumeCancel(id, userCode);
                                break;
                            default:
                                break;
                        }
                    }


                }
                catch (Exception ex)
                {
                    throw ex;

                }

                return StatusCode(200, "Sent To Rsd.");
            }
            catch (Exception ex)
            {

                return StatusCode(500, ex.Message);
            }


        }


       


    }
}
