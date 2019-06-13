using System.Threading.Tasks;
using Imgur.API;
using Imgur.API.Authentication.Impl;
using Imgur.API.Endpoints.Impl;
using Microsoft.AspNetCore.Mvc;

namespace Job_Portal_System.Controllers
{
    [Route("Util")]
    public class UtilController : Controller
    {
        [HttpPost]
        [Route("UploadImage")]
        public async Task<IActionResult> UploadImage()
        {
            var formImage = Request.Form.Files["image"];
            try
            {
                var client = new ImgurClient("7ce7d2a9cb1e759", "38427ac0bed7bb7ff49816783ae0a0a0a4cd7d0e");
                var endpoint = new ImageEndpoint(client);
                var image = await endpoint.UploadImageStreamAsync(formImage.OpenReadStream());
                return Json(new { Uploaded = true, Url = image.Link });
            }
            catch (ImgurException)
            {
                return Json(new { Uploaded = false });
            }
        }
    }
}