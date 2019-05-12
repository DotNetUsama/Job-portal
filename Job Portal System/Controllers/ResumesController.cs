using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        public async Task<IActionResult> Search(string query, int p = 1)
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
                    break;
                }

                var similar = _context.JobTitleSimilarities
                    .SingleOrDefault(s => s.SimilarTitle.Title == queryJob);
                if (similar == null) break;
                jobTitles.Add(similar.JobTitle);
            }
            
            var resumes = await _context.Resumes
                .Include(r => r.User)
                .Include(r => r.SeekedJobTitles).ThenInclude(s => s.JobTitle)
                .Where(r => r.IsPublic && 
                            r.SeekedJobTitles.Count != 0 &&
                            r.SeekedJobTitles.Select(s => s.JobTitle).Intersect(jobTitles).Any())
                .ToListAsync();
            var pager = new Pager(resumes.Count, p);

            var viewedResumes = resumes.Skip((pager.CurrentPage - 1) * pager.PageSize).Take(pager.PageSize);
            return View("ResumesSearchResult", new ResumesSearchResult
            {
                Resumes = viewedResumes,
                Query = query,
            });
        }
    }
}