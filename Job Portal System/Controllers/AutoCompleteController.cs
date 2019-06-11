using System.Linq;
using Job_Portal_System.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace Job_Portal_System.Controllers
{
    [Route("AutoComplete")]
    public class AutoCompleteController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IHostingEnvironment _env;

        public AutoCompleteController(IHostingEnvironment env,
            ApplicationDbContext context)
        {
            _env = env;
            _context = context;
        }

        [HttpPost]
        [Route("Companies")]
        public IActionResult Companies()
        {
            return Json(_context.Companies
                .Select(company => new
                {
                    company.Id,
                    Label = company.Name,
                }));
        }

        [HttpPost]
        [Route("CompanyDepartments")]
        public IActionResult CompanyDepartments()
        {
            return Json(_context.CompanyDepartments
                .Select(department => new
                {
                    department.Id,
                    Label = $"{department.City.Name} ({department.DetailedAddress})",
                }));
        }

        [HttpPost]
        [Route("Skills")]
        public IActionResult Skills()
        {
            return Json(_context.Skills
                .Select(skill => new
                {
                    skill.Id,
                    Label = skill.Title,
                }));
        }

        [HttpPost]
        [Route("States")]
        public IActionResult States()
        {
            return Json(_context.States
                .Select(state => new
                {
                    state.Id,
                    Label = state.Name,
                }));
        }

        [HttpPost]
        [Route("Cities")]
        public IActionResult Cities(string stateId)
        {
            return Json(_context.Cities
                .Where(city => city.StateId == stateId)
                .Select(city => new
                {
                    city.Id,
                    Label = city.Name,
                }));
        }

        [HttpPost]
        [Route("Schools")]
        public IActionResult Schools()
        {
            return Json(_context.Schools
                .Select(school => new
                {
                    school.Id,
                    Label = school.Name,
                }));
        }

        [HttpPost]
        [Route("FieldsOfStudy")]
        public IActionResult FieldsOfStudy()
        {
            return Json(_context.FieldOfStudies
                .Select(fieldOfStudy => new
                {
                    fieldOfStudy.Id,
                    Label = fieldOfStudy.Title,
                }));
        }

        [HttpPost]
        [Route("JobTitles")]
        public IActionResult JobTitles()
        {
            return Json(_context.JobTitles
                .Select(jobTitle => new
                {
                    jobTitle.Id,
                    Label = jobTitle.Title,
                }));
        }
    }
}