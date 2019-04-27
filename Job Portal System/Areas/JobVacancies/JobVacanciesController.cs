using System.Threading.Tasks;
using Job_Portal_System.Data;
using Job_Portal_System.Enums;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Job_Portal_System.Areas.JobVacancies
{
    [Route("JobVacancies")]
    public class JobVacanciesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public JobVacanciesController(ApplicationDbContext context)
        {
            _context = context;
        }


        [HttpPost]
        [Route("Close")]
        public async Task<IActionResult> Close(string id)
        {

            if (id == null) return NotFound();

            var jobVacancy = await _context.JobVacancies
                .Include(j => j.User)
                .SingleOrDefaultAsync(m => m.Id == id);

            if (jobVacancy == null) return NotFound();

            if (jobVacancy.User.UserName != User.Identity.Name ||
                jobVacancy.Status != (int)JobVacancyStatus.Open ||
                jobVacancy.Method != (int)JobVacancyMethod.Submission)
            {
                return BadRequest();
            }

            jobVacancy.Status = (int)JobVacancyStatus.Closed;

            await _context.SaveChangesAsync();

            return Redirect(Request.GetEncodedPathAndQuery());

        }
    }
}
