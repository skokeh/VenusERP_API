using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Data;
using System.Net;
using VenusERP_API.Utils;
using VenusERP_Application.Dtos.GeneralDtos;
using VenusERP_Application.Services;

namespace VenusERP_API.Controllers.Settings
{

    public class TaxGroupHeadController : BaseApiController
    {
        private readonly TaxHeadGroupService _taxHeadGroupService;
        public TaxGroupHeadController(TaxHeadGroupService taxHeadGroupService)
        {
            _taxHeadGroupService = taxHeadGroupService;
        }

        [HttpGet("GetList")]
        public async Task<ActionResult> GetList([FromQuery] GetListParameters getListParameters)
        {
            try
            {
                var result = await _taxHeadGroupService.GetAllTaxHeadGroup(getListParameters, ExtractFormId(), ExtractLang(), ExtractRoutePath(), ExtractUserId());
                if (result.Code == HttpStatusCode.OK)
                {
                    return Ok(result.Object);
                }
                return StatusCode((int)result.Code, result.Message);

            }
            catch (Exception ex)
            {

                return StatusCode(500, ex.Message);
            }


        }

        [HttpGet("GetNextCode")]
        public async Task<ActionResult> GetNextCode()
        {
            var result = await _taxHeadGroupService.GetNextCode();
            return Ok(result.Object);
        }

        [HttpGet("GetByID")]
        public async Task<ActionResult> GetByID(int ID)
        {
            try
            {
                var result = await _taxHeadGroupService.GetTaxGroupById(ID);
                if (result.Code == HttpStatusCode.OK && result.Object != null)
                {
                    return StatusCode((int)result.Code, result.Object);
                }

                return StatusCode((int)result.Code, result.Message);
            }
            catch (Exception ex)
            {

                return StatusCode(500, ex.Message);
            }
        }


        [HttpPost("Save")]
        public async Task<ActionResult> Save(DataObj dataObj)
        {
            try
            {
                var result = await _taxHeadGroupService.AddtaxGroup(dataObj.dataObj);
                if (result.Code == HttpStatusCode.OK)
                {
                    return Ok(result.Object);
                }
                return StatusCode((int)result.Code, result.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("Delete")]
        public async Task<ActionResult> Delete(DataObj dataObj)
        {
            try
            {
                var result = await _taxHeadGroupService.DeleteTaxGroup(dataObj.dataObj);
                if (result.Code == HttpStatusCode.OK)
                {

                    return Ok(result.Object);
                }
                return StatusCode((int)result.Code, result.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);

            }
        }
    }
}
