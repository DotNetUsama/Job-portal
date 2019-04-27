using System.Threading.Tasks;
using Job_Portal_System.Data;
using Job_Portal_System.Migrations;
using Job_Portal_System.Models;
using Job_Portal_System.SignalR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace Job_Portal_System.Controllers
{
    [Route("Applicants")]
    public class ApplicantsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IHubContext<SignalRHub> _hubContext;

        public ApplicantsController(ApplicationDbContext context,
            IHubContext<SignalRHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        [HttpGet]
        [Route("Details")]
        public async Task<IActionResult> Details(string id)
        {
            if (!User.IsInRole("Recruiter") && !User.IsInRole("JobSeeker")) return BadRequest();

            Applicant applicant;


            if (User.IsInRole("JobSeeker"))
            {
                applicant = await _context.Applicants
                    .Include(a => a.JobSeeker)
                    .SingleOrDefaultAsync(a => a.Id == id);

                if (applicant == null) return NotFound();
                if (applicant.JobSeeker.UserName != User.Identity.Name) return BadRequest();

                applicant.JobVacancy = await _context.JobVacancies
                    .Include(j => j.CompanyDepartment).ThenInclude(d => d.Company)
                    .Include(j => j.CompanyDepartment).ThenInclude(d => d.City)
                    .Include(r => r.EducationQualifications).ThenInclude(e => e.FieldOfStudy)
                    .Include(r => r.WorkExperienceQualifications).ThenInclude(w => w.JobTitle)
                    .Include(r => r.DesiredSkills).ThenInclude(s => s.Skill)
                    .Include(r => r.JobTypes)
                    .Include(r => r.JobTitle)
                    .SingleOrDefaultAsync(j => j.Id == applicant.JobVacancyId);
                return View("DetailsForJobSeeker", applicant);
            }

            applicant = await _context.Applicants
                .Include(a => a.Recruiter)
                .SingleOrDefaultAsync(a => a.Id == id);

            if (applicant == null) return NotFound();
            if (applicant.Recruiter.UserName != User.Identity.Name) return BadRequest();

            applicant.Resume = await _context.Resumes
                .Include(r => r.Educations).ThenInclude(e => e.FieldOfStudy)
                .Include(r => r.Educations).ThenInclude(e => e.School).ThenInclude(s => s.City)
                .Include(r => r.WorkExperiences).ThenInclude(w => w.JobTitle)
                .Include(r => r.WorkExperiences).ThenInclude(w => w.Company)
                .Include(r => r.OwnedSkills).ThenInclude(s => s.Skill)
                .Include(r => r.User)
                .SingleOrDefaultAsync(r => r.Id == applicant.ResumeId);
            return View("DetailsForRecruiter", applicant);
        }
    }
}
