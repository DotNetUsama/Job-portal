using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DinkToPdf;
using DinkToPdf.Contracts;
using Job_Portal_System.Data;
using Job_Portal_System.Models;
using Job_Portal_System.Utilities.Semantic;
using Job_Portal_System.ViewModels;
using JW;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Job_Portal_System.Controllers
{
    [Route("Resumes")]
    public class ResumesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly IHostingEnvironment _env;
        private readonly IConverter _converter;

        public ResumesController(ApplicationDbContext context,
            UserManager<User> userManager,
            IHostingEnvironment env,
            IConverter converter)
        {
            _context = context;
            _userManager = userManager;
            _env = env;
            _converter = converter;
        }
        
        [HttpGet]
        [Route("Index")]
        public async Task<IActionResult> Index(string tab)
        {
            IEnumerable<ResumeGeneralViewModel> resumes;
            switch (tab)
            {
                case "recent":
                    resumes = RecentResumes();
                    break;
                case "interesting":
                case null:
                    resumes = InterestingResumes();
                    break;
                default:
                    return NotFound();
            }

            var isJobSeeker = User.IsInRole("JobSeeker");
            var ownResumeCreated = false;
            if (isJobSeeker)
            {
                var user = await _userManager.GetUserAsync(User);
                ownResumeCreated = _context.Resumes.Any(r => r.UserId == user.Id);
            }

            return View("ResumesIndex", new ResumesIndexViewModel
            {
                IsJobSeeker = isJobSeeker,
                OwnResumeCreated = ownResumeCreated,
                ActiveTab = tab ?? "interesting",
                Resumes = resumes,
            });
        }

        [HttpGet]
        [Route("Search")]
        public IActionResult Search(string query, int p = 1)
        {
            var normalizedTitle = query.ToLower().Split(" - ").Last();
            var jobTitleSynsetId = _context.JobTitles
                .FirstOrDefault(j => j.NormalizedTitle == normalizedTitle)?.JobTitleSynsetId;

            if (jobTitleSynsetId == null)
            {
                var similarities = SimilaritiesOperator.GetSimilarities(normalizedTitle, _env);
                foreach (var similarity in similarities)
                {
                    jobTitleSynsetId = _context.JobTitles
                        .FirstOrDefault(j => j.NormalizedTitle == similarity)?.JobTitleSynsetId;
                    if (jobTitleSynsetId != null) break;
                }
            }

            IEnumerable<Resume> resumes = new List<Resume>();

            if (jobTitleSynsetId != null)
            {
                var watch = System.Diagnostics.Stopwatch.StartNew();
                resumes = _context.Resumes
                    .Include(r => r.User)
                    .Include(r => r.SeekedJobTitles).ThenInclude(s => s.JobTitle)
                    .Where(r => r.SeekedJobTitles.Any(j => j.JobTitle.JobTitleSynsetId == jobTitleSynsetId));
                watch.Stop();
            }

            var pager = new Pager(resumes.Count(), p, 15);

            var viewedResumes = resumes
                .Distinct()
                .Skip((pager.CurrentPage - 1) * pager.PageSize)
                .Take(pager.PageSize);
            return View("ResumesSearchResult", new ResumesSearchResult
            {
                Resumes = viewedResumes,
                Query = query,
                TotalPages = pager.TotalPages,
                PageNumber = pager.CurrentPage,
                IsFirst = pager.CurrentPage == 1 || pager.CurrentPage == 0,
                IsLast = pager.CurrentPage == pager.TotalPages,
            });
        }

        [HttpPost]
        [Route("ExportAsPdf")]
        public IActionResult ExportAsPdf(string id)
        {
            if (id == null) return NotFound();

            var resume = _context.Resumes
                .Include(r => r.User)
                .SingleOrDefault(r => r.Id == id);

            if (resume == null) return NotFound();

            var doc = new HtmlToPdfDocument()
            {
                GlobalSettings = {
                    ColorMode = ColorMode.Color,
                    Orientation = Orientation.Portrait,
                    PaperSize = PaperKind.A4,
                    Margins = new MarginSettings { Top = 0, Left = 0, Right = 0, Bottom = 0 },
                },

                Objects = {
                    new ObjectSettings()
                    {
                        Page = $"{Request.Scheme}://{Request.Host}/Resumes/Download?id={id}",
                    },
                }
            };

            var pdf = _converter.Convert(doc);

            return File(pdf, "application/pdf", $"{resume.User.LastName}_Resume.pdf");
        }

        private IEnumerable<ResumeGeneralViewModel> RecentResumes()
        {
            return _context.Resumes
                .Where(r => r.IsPublic)
                .Include(r => r.User).ThenInclude(u => u.City).ThenInclude(c => c.State)
                .OrderByDescending(r => r.UpdatedAt)
                .Take(20)
                .Select(r => new ResumeGeneralViewModel
                {
                    Id = r.Id,
                    OwnerName = $"{r.User.FirstName} {r.User.LastName}",
                    IsModified = r.UpdatedAt != r.CreatedAt,
                    LastUpdate = r.UpdatedAt,
                    Location = $"{r.User.City.State.Name}-{r.User.City.Name}",
                    WorksCount = _context.WorkExperiences.Count(w => w.ResumeId == r.Id),
                    EducationsCount = _context.Educations.Count(e => e.ResumeId == r.Id),
                    SkillsCount = _context.OwnedSkills.Count(s => s.ResumeId == r.Id),
                    SeekedJobTitles = _context.SeekedJobTitles
                        .Where(s => s.ResumeId == r.Id)
                        .Select(j => j.JobTitle.Title)
                        .ToList(),
                });
        }

        private IEnumerable<ResumeGeneralViewModel> InterestingResumes()
        {
            return _context.Resumes
                .Where(r => r.IsPublic)
                .Include(r => r.User).ThenInclude(u => u.City).ThenInclude(c => c.State)
                .OrderByDescending(r => r.UpdatedAt)
                .Take(200)
                .OrderBy(r => Guid.NewGuid())
                .Take(20)
                .OrderByDescending(r => r.UpdatedAt)
                .Select(r => new ResumeGeneralViewModel
                {
                    Id = r.Id,
                    OwnerName = $"{r.User.FirstName} {r.User.LastName}",
                    IsModified = r.UpdatedAt != r.CreatedAt,
                    LastUpdate = r.UpdatedAt,
                    Location = $"{r.User.City.State.Name}-{r.User.City.Name}",
                    WorksCount = _context.WorkExperiences.Count(w => w.ResumeId == r.Id),
                    EducationsCount = _context.Educations.Count(e => e.ResumeId == r.Id),
                    SkillsCount = _context.OwnedSkills.Count(s => s.ResumeId == r.Id),
                    SeekedJobTitles = _context.SeekedJobTitles
                        .Where(s => s.ResumeId == r.Id)
                        .Select(j => j.JobTitle.Title)
                        .ToList(),
                });
        }
    }
}