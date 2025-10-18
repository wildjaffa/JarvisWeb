namespace JarvisWeb.Controllers;

using System.IO;
using Microsoft.AspNetCore.Mvc;

public class MediaController : ControllerBase
{
    [HttpGet("media/audio")]
    public IActionResult GetAudioFile([FromQuery] string filePath)
    {
        if (string.IsNullOrEmpty(filePath) || !System.IO.File.Exists(filePath))
        {
            return NotFound("File not found.");
        }

        var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
        var contentType = "audio/mpeg"; // Adjust the MIME type as needed
        var fileName = Path.GetFileName(filePath);

        return File(fileStream, contentType, fileName);
    }
}
