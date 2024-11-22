using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace MyApiProject.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ImageUploadController : ControllerBase
    {
        [HttpPost("upload")]
        public async Task<IActionResult> UploadImage([FromBody] Base64ImageUploadRequest request)
        {
                            try
                {
                    // Ensure the base64 string is not empty or null
                    if (string.IsNullOrEmpty(request.Base64Image))
                    {
                        return BadRequest("Base64 string is empty.");
                    }

                    // Remove the "data:image/png;base64," part, if present
                    string base64String = request.Base64Image;
                    if (base64String.StartsWith("data:image", StringComparison.OrdinalIgnoreCase))
                    {
                        int index = base64String.IndexOf(",", StringComparison.Ordinal);
                        if (index != -1)
                        {
                            base64String = base64String.Substring(index + 1); // Get the actual base64 part
                        }
                        else
                        {
                            return BadRequest("Invalid base64 string format.");
                        }
                    }

                    // Ensure the base64 string is valid by checking its length
                    int mod4 = base64String.Length % 4;
                    if (mod4 > 0)
                    {
                        base64String += new string('=', 4 - mod4);  // Add padding if necessary
                    }

                    // Decode the base64 string to a byte array
                    byte[] imageBytes = Convert.FromBase64String(base64String);

                    // Validate the image type (optional, based on file signature or MIME type)
                    var imageSignature = Encoding.ASCII.GetString(imageBytes, 0, 4); // Check for PNG/JPEG/GIF signatures
                    // Replace the "?" character with an empty string
                    imageSignature = imageSignature.Replace("?", "");
                    var supportedSignatures = new[] { "\x89PNG", "GIF8", "\xFF\xD8\xFF" }; // PNG, GIF, JPEG

                    if (!Array.Exists(supportedSignatures, sig => imageSignature.StartsWith(sig)))
                    {
                        // Optionally log the base64 string and image signature
                        Console.WriteLine("Base64 Image String: " + request.Base64Image);
                        Console.WriteLine("Image Signature: " + imageSignature);  // Optionally print image signature
                        return BadRequest("Invalid image format.");
                    }

                    // Save the file to a directory
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "UploadedImages");
                    Directory.CreateDirectory(uploadsFolder); // Ensure the directory exists

                    var fileName = $"{Guid.NewGuid()}.png"; // You can derive the file name from the request or use a GUID
                    var filePath = Path.Combine(uploadsFolder, fileName);

                    await System.IO.File.WriteAllBytesAsync(filePath, imageBytes);  // Save image

                    return Ok(new { FilePath = filePath });
                }
                catch (FormatException ex)
                {
                    return BadRequest("Invalid base64 string format: " + ex.Message);
                }
                catch (Exception ex)
                {
                    return BadRequest("Error: " + ex.Message);
                }

        }

    }

    public class Base64ImageUploadRequest
    {
        public string Base64Image { get; set; }
    }
}
