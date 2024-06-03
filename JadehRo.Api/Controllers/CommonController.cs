using JadehRo.Api.Infrastructure.Api;
using JadehRo.Service.CommonService;
using JadehRo.Service.CommonService.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JadehRo.Api.Controllers;

public class CommonController : BaseController
{
    private readonly ICommonService _commonService;
    private readonly IConfiguration _configuration;


    public CommonController(ICommonService commonService, IConfiguration configuration)
    {
        _commonService = commonService;
        _configuration = configuration;
    }

    [HttpGet("PdfFile/{fileName}")]
    public IActionResult GetPdfReport(string fileName)
    {
        var filePath = Path.Combine(_configuration.GetSection("Paths:Files").Value!, FilesPath.Report, fileName);
        return File(
            fileContents: System.IO.File.ReadAllBytes(filePath),
            contentType: "application/pdf",
            fileDownloadName: $"{fileName}");
    }

    [HttpGet("ExcelFile/{fileName}")]
    public IActionResult GetExcelReport(string fileName)
    {
        var filePath = Path.Combine(_configuration.GetSection("Paths:Files").Value!, FilesPath.Report, fileName);
        return File(
            fileContents: System.IO.File.ReadAllBytes(filePath),
            contentType: "application/ms-excel",
            fileDownloadName: $"{fileName}");
    }

    [HttpGet("Province"), AllowAnonymous]
    public ApiResult<IList<CountryDivisionDto>> GetProvinces()
    {
        var response = _commonService.GetAllProvince();
        return Ok(response);
    }

    [HttpGet("Cities"), AllowAnonymous]
    public ApiResult<IList<CountryDivisionDto>> GetCities()
    {
        var response = _commonService.GetAllCities();
        return Ok(response);
    }

    [HttpGet("CountryDivisionByProvince/{provinceId:long}"), AllowAnonymous]
    public IList<CountryDivisionDto> GetCountryDivisionByProvince(long? provinceId)
    {
        var result = _commonService.GetCountryDivisionByProvince(provinceId);
        return result;
    }
}