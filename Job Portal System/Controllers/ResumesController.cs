using System.Collections.Generic;
using System.IO;
using System.Linq;
using DinkToPdf;
using DinkToPdf.Contracts;
using Job_Portal_System.Data;
using Job_Portal_System.Models;
using Job_Portal_System.Utilities.Semantic;
using Job_Portal_System.ViewModels;
using JW;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Job_Portal_System.Controllers
{
    [Route("Resumes")]
    public class ResumesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IHostingEnvironment _env;
        private readonly IConverter _converter;

        public ResumesController(ApplicationDbContext context,
            IHostingEnvironment env,
            IConverter converter)
        {
            _context = context;
            _env = env;
            _converter = converter;
        }

        [HttpGet]
        [Route("Search")]
        public IActionResult Search(string query, int p = 1)
        {
            var similaritiesQueryPath = Path.Combine(_env.ContentRootPath, "Queries", "GetSimilarities.txt");
            query = query.ToLower().Trim();
            var similarities = SimilaritiesOperator.GetSimilarities(query, similaritiesQueryPath);
            var jobTitles = similarities
                .Select(similarity => _context.JobTitles.SingleOrDefault(j => j.Title == similarity))
                .Where(jobTitle => jobTitle != null)
                .ToList();

            var resumes = new List<Resume>();
            foreach (var job in jobTitles)
            {
                resumes.AddRange(_context.SeekedJobTitles
                    .Where(s => s.JobTitleId == job.Id)
                    .Select(s => s.Resume)
                    .Include(r => r.User)
                    .Include(r => r.SeekedJobTitles).ThenInclude(s => s.JobTitle));
            }
            var pager = new Pager(resumes.Count, p, 3);

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
    }
}