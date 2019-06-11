using System;
using System.Net;
using System.Threading.Tasks;
using Imgur.API;
using Imgur.API.Authentication.Impl;
using Imgur.API.Endpoints.Impl;
using Job_Portal_System.Enums;
using Job_Portal_System.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

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

        [HttpGet]
        [Route("Test")]
        public IActionResult Test()
        {
            using (var client = new WebClient())
            {
                var json = client.DownloadString("https://randomuser.me/api");
                var obj = JsonConvert.DeserializeAnonymousType(json, new
                {
                    results = new[]
                    {
                        new
                        {
                            gender = string.Empty,
                            name = new
                            {
                                first = string.Empty,
                                last = string.Empty,
                            },
                            email = string.Empty,
                            dob = new
                            {
                                age = 0,
                            },
                            phone = string.Empty,
                            picture = new
                            {
                                large = string.Empty,
                            }
                        }
                    },
                });
                return Json(new User
                {
                    FirstName = obj.results[0].name.first,
                    LastName = obj.results[0].name.last,
                    Email = obj.results[0].email,
                    PhoneNumber = obj.results[0].phone,
                    Gender = obj.results[0].gender == "male" ? (byte)GenderType.Male : (byte)GenderType.Female,
                    BirthDate = DateTime.Now.AddYears(-obj.results[0].dob.age),
                    Image = obj.results[0].picture.large,
                });
            }
        }
    }
}