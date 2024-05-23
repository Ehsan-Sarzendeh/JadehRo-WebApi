using AutoMapper;
using JadehRo.Common.Exceptions;
using JadehRo.Database.Entities.Common;
using JadehRo.Database.Repositories.RepositoryWrapper;
using JadehRo.Service.CommonService.Dto;
using JadehRo.Service.OptionServices;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;

namespace JadehRo.Service.CommonService;

public class CommonService : ICommonService
{
    private readonly IConfiguration _configuration;
    private readonly IRepositoryWrapper _repository;
    private readonly IOptionService _optionService;
    private readonly IMemoryCache _cache;
    private readonly IMapper _mapper;


    public CommonService(IConfiguration configuration, IRepositoryWrapper repository, IOptionService optionService, IMapper mapper, IMemoryCache cache)
    {
        _configuration = configuration;
        _repository = repository;
        _optionService = optionService;
        _mapper = mapper;
        _cache = cache;
    }


    public string GenerateFileName(string extension = "")
    {
        return string.Concat(Path.GetRandomFileName().Replace(".", ""),
            (!string.IsNullOrEmpty(extension)) ? (extension.StartsWith(".") ? extension : string.Concat(".", extension)) : "");
    }
    public async Task<string> UploadBase64Image(string base64Image, string path, string fileName, CancellationToken cancellationToken)
    {
        var name = $"{fileName}.jpg";
        var savedPath = Path.Combine(path, name);
        var filePath = Path.Combine(_configuration.GetSection("Paths:Files").Value!, savedPath);
        var imageBytes = Convert.FromBase64String(base64Image);
        await File.WriteAllBytesAsync(filePath, imageBytes, cancellationToken);
        return savedPath;
    }
    public async Task<string> UploadFile(IFormFile file, string path, string fileName, CancellationToken cancellationToken)
    {
        if (file.Length <= 0) throw new AppException("فایل ارسال نشده است");

        var name = $"{fileName}" + Path.GetExtension(file.FileName);
        var savedPath = Path.Combine(path, name);
        var filePath = Path.Combine(_configuration.GetSection("Paths:Files").Value!, savedPath);
        await using var stream = File.Create(filePath);
        await file.CopyToAsync(stream, cancellationToken);

        return savedPath;
    }

    public IList<CountryDivisionDto> GetAllCountryDivisions()
    {
        var countryDivisions = GetAllCountryDivision();
        return CountryDivisionDto.FromEntities(_mapper, countryDivisions);
    }

    public IList<CountryDivisionDto> GetAllProvince()
    {
        var province = GetAllCountryDivision()
            .Where(x => x.ParentId == null)
            .ToList();
        return CountryDivisionDto.FromEntities(_mapper, province);
    }

    public IList<CountryDivisionDto> GetAllCities()
    {
        var countryDivisions = GetAllCountryDivision()
            .Where(x => x.ParentId != null && x.Parent.ParentId == null)
            .ToList();
        return CountryDivisionDto.FromEntities(_mapper, countryDivisions);
    }

    private IList<CountryDivision> GetAllCountryDivision()
    {
        const string cacheKey = "GetAllCountryDivision";

        if (!_cache.TryGetValue(cacheKey, out IList<CountryDivision> result))
        {
            result = _repository.CountryDivision.TableNoTracking
                .Include(x => x.Parent)
                .ThenInclude(x => x.Parent)
                .ToList();

            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromDays(5))
                .SetAbsoluteExpiration(TimeSpan.FromDays(20))
                .SetPriority(CacheItemPriority.Normal);

            _cache.Set(cacheKey, result, cacheEntryOptions);
        }

        return result;
    }

    public IList<CountryDivisionDto> GetCountryDivisionByProvince(long? provinceId)
    {
        var models = GetAllCountryDivision();

        if (provinceId is null)
        {
            models = models
                .Where(x => x.ParentId != null)
                .ToList();
        }
        else
        {
            models = models
                .Where(x => 
                    x.ParentId != null && 
                    x.ParentId.ToString()?[..2] == provinceId.ToString())
                .ToList();
        }

        return CountryDivisionDto.FromEntities(_mapper, models.ToList());
    }
}