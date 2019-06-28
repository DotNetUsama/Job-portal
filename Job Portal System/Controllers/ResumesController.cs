using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DinkToPdf;
using DinkToPdf.Contracts;
using Job_Portal_System.Data;
using Job_Portal_System.Dependencies;
using Job_Portal_System.Models;
using Job_Portal_System.Utilities.Semantic;
using Job_Portal_System.ViewModels.Resumes;
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
        private readonly ITermsManager _termsManager;

        public ResumesController(ApplicationDbContext context,
            UserManager<User> userManager,
            IHostingEnvironment env,
            IConverter converter,
            ITermsManager termsManager)
        {
            _context = context;
            _userManager = userManager;
            _env = env;
            _converter = converter;
            _termsManager = termsManager;
        }

        [HttpGet]
        [Route("Index")]
        public IActionResult Index(string tab, string q)
        {
            IQueryable<Resume> resumesQueryable;
            switch (tab)
            {
                case "recent":
                    resumesQueryable = RecentResumes();
                    break;
                case "interesting":
                case null:
                    resumesQueryable = InterestingResumes();
                    break;
                default:
                    return NotFound();
            }

            var jobTitleSynsetId = string.IsNullOrEmpty(q) ? null : _termsManager.GetJobTitleSynset(q);

            var similarities = string.IsNullOrEmpty(q) ? null : _termsManager.GetSimilarJobTitles(q);

            var resumes = similarities != null && similarities.Any() ?
                resumesQueryable.Where(r =>
                    r.IsPublic &&
                    r.SeekedJobTitles.Any(j => _context.JobTitles
                        .Any(jt => 
                            jt.JobTitleSynsetId == jobTitleSynsetId &&
                            j.JobTitle.Title.Contains(jt.Title)))) :
                resumesQueryable.Where(r => r.IsPublic && (string.IsNullOrEmpty(q) || r.SeekedJobTitles.Any(sj => sj.JobTitle.Title.Contains(q))));

            var isJobSeeker = User.IsInRole("JobSeeker");
            var ownResumeCreated = false;
            if (isJobSeeker)
            {
                ownResumeCreated = _context.Resumes.Any(r => r.UserId == _userManager.GetUserId(User));
            }

            return View("ResumesIndex", new ResumesIndexViewModel
            {
                IsJobSeeker = isJobSeeker,
                OwnResumeCreated = ownResumeCreated,
                ActiveTab = tab ?? "interesting",
                Query = q,
                Count = resumes.Count(),
                Resumes = resumes
                    .Include(r => r.User).ThenInclude(u => u.City).ThenInclude(c => c.State)
                    .Include(r => r.WorkExperiences)
                    .Include(r => r.Educations)
                    .Include(r => r.OwnedSkills)
                    .Include(r => r.SeekedJobTitles).ThenInclude(j => j.JobTitle)
                    .OrderByDescending(r => r.UpdatedAt)
                    .Take(20)
                    .Select(r => new ResumeGeneralViewModel
                    {
                        Id = r.Id,
                        OwnerName = $"{r.User.FirstName} {r.User.LastName}",
                        IsModified = r.UpdatedAt != r.CreatedAt,
                        LastUpdate = r.UpdatedAt,
                        Location = $"{r.User.City.State.Name}-{r.User.City.Name}",
                        WorksCount = r.WorkExperiences.Count,
                        EducationsCount = r.Educations.Count,
                        SkillsCount = r.OwnedSkills.Count,
                        SeekedJobTitles = r.SeekedJobTitles
                            .Select(j => j.JobTitle.Title),
                    }),
            });
        }

        [HttpGet]
        [Route("Details")]
        public async Task<IActionResult> Details(string id)
        {
            if (id == null) return NotFound();

            var resume = await _context.Resumes
                .Include(r => r.Educations).ThenInclude(e => e.FieldOfStudy)
                .Include(r => r.Educations).ThenInclude(e => e.School)
                .Include(r => r.WorkExperiences).ThenInclude(w => w.JobTitle)
                .Include(r => r.WorkExperiences).ThenInclude(w => w.Company)
                .Include(r => r.OwnedSkills).ThenInclude(s => s.Skill)
                .Include(r => r.SeekedJobTitles).ThenInclude(j => j.JobTitle)
                .Include(r => r.User).ThenInclude(u => u.City).ThenInclude(c => c.State)
                .SingleOrDefaultAsync(m => m.Id == id);

            if (resume == null) return NotFound();

            return View("ResumeDetails", new ResumeFullViewModel
            {
                Resume = resume,
                IsOwner = _userManager.GetUserId(User) == resume.UserId,
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

        [HttpGet]
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

        private IQueryable<Resume> RecentResumes()
        {
            return _context.Resumes
                .OrderByDescending(r => r.UpdatedAt);
        }

        private IQueryable<Resume> InterestingResumes()
        {
            return _context.Resumes
                .Take(200)
                .OrderBy(r => Guid.NewGuid());
        }
    }
}