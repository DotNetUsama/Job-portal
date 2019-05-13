using System.Collections.Generic;
using System.Linq;
using Job_Portal_System.Data;
using Job_Portal_System.Models;
using Job_Portal_System.ViewModels;
using JW;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Job_Portal_System.Controllers
{
    [Route("Resumes")]
    public class ResumesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ResumesController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("Search")]
        public IActionResult Search(string query, int p = 1)
        {
            var queries = query.Split(",");
            var jobTitles = new List<JobTitle>();
            foreach (var j in queries)
            {
                var queryJob = j.Trim();
                var jobInDb = _context.JobTitles.SingleOrDefault(jobTitle => jobTitle.Title == queryJob);
                if (jobInDb != null)
                {
                    jobTitles.Add(jobInDb);
                    continue;
                }

                var similar = _context.JobTitleSimilarities
                    .Include(s => s.JobTitle)
                    .SingleOrDefault(s => s.SimilarTitle.Title == queryJob);
                if (similar == null) continue;
                jobTitles.Add(similar.JobTitle);
            }

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
    }
}