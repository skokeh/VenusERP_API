using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Data;
using System.Data.SqlClient;
using VenusERP_API.Utils;
using VenusERP_Application.Assets;
using VenusERP_Application.Dtos.AssetsDto;
using VenusERP_Application.Dtos.AssetsDto.AssetPhysicalCount;

namespace VenusERP_API.Controllers.Assets
{

    public class AssetPhysicalCountController : BaseApiController
    {
        public readonly AssetCountService _assetCountService;
        public AssetPhysicalCountController(AssetCountService assetCountService)
        {
            _assetCountService = assetCountService;
        }

        [HttpGet("GetAssetsForLocation")]
        public ActionResult GetAssetsForLocation(long locationId)
        {
            try
            {
                List<SqlParameter> l = new List<SqlParameter>
                {
                    new SqlParameter("@User", GetCurrentUser()),
                    new SqlParameter("@Lang", ExtractLang()),
                    new SqlParameter("@LocationID", locationId)
                };
                var x = DataHelper.ExecScalar("Ass_Items_GetByLocationList", System.Data.CommandType.StoredProcedure, l.ToArray());
                if (x == null || string.IsNullOrEmpty(x.ToString()))
                {
                    return StatusCode(200, "");
                }
                string jsonString = x.ToString().Trim();
                //System.Diagnostics.Debug.WriteLine("Raw JSON string: " + jsonString);  
                try
                {
                   
                    JArray array = JArray.Parse(jsonString);
                    return StatusCode(200, array);
                }
                catch (JsonReaderException ex)
                {
                    System.Diagnostics.Debug.WriteLine("JSON Reader Exception: " + ex.Message);
                    return StatusCode(500, "Invalid JSON format.");
                }
            }

            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("PostAssetCount")]
        public async Task<ActionResult> PostAssetCount(PostAssetCountDto postAssetCountDto)
        {
            try
            {
               var result = await _assetCountService.AddAssetCount(postAssetCountDto, ExtractUserId());
                if(result != 0)
                {
                    return StatusCode(200, "Asset Count Successful");
                }
                return StatusCode(500, "unable to stored the count");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
         
        }

        [HttpGet("GetAssetPhysicalCountList")]
        public async Task<ActionResult<string>> GetAssetPhysicalCountList(int LocationId)
        {
            try
            {
                var result = await _assetCountService.GetAssetCountForLocation(LocationId, ExtractLang());
                return StatusCode(200, result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("ApproveAssetCount")]
        public async Task<ActionResult> ApproveAssetCount(PostApprovedAssetCountDto approveAssetCountDto)
        {
            try
            {
                var result = await _assetCountService.ApproveAssetCount(approveAssetCountDto, ExtractUserId());
                return StatusCode(200, result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
