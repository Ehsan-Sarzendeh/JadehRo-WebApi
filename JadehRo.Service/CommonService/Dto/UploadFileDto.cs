namespace JadehRo.Service.CommonService.Dto;

public class UploadFileDto
{
    public IFormFile File { get; set; }
    public string Path { get; set; }
    public string FileName { get; set; }
}