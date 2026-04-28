using Microsoft.AspNetCore.Mvc;
using WebAPI_WebApp.BlobStorage;

namespace WebAPI_WebApp.Controllers;

[ApiController]
[Route("documents")]
public sealed class DocumentsController : ControllerBase
{
    private readonly BlobStorageService _blobService;

    public DocumentsController(BlobStorageService blobService)
    {
        _blobService = blobService;
    }

    [HttpPost("upload")]
    public async Task<IActionResult> Upload(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("File is empty");

        var url = await _blobService.UploadFileAsync(file);

        return Ok(new { FileUrl = url });
    }
}