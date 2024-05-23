using JadehRo.Common.Utilities;
using JadehRo.Service.CommonService.Dto;
using Microsoft.AspNetCore.Http;

namespace JadehRo.Service.CommonService;

public interface ICommonService : IScopedDependency
{
    string GenerateFileName(string extension = "");
    Task<string> UploadFile(IFormFile file, string path, string fileName, CancellationToken cancellationToken);
    Task<string> UploadBase64Image(string base64Image, string path, string fileName, CancellationToken cancellationToken);

    IList<CountryDivisionDto> GetAllCountryDivisions();
    IList<CountryDivisionDto> GetAllProvince();
    IList<CountryDivisionDto> GetAllCities();
    IList<CountryDivisionDto> GetCountryDivisionByProvince(long? provinceId);
}