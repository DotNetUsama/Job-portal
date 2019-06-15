using System.Linq;
using Job_Portal_System.Data;
using Job_Portal_System.Models;
using Job_Portal_System.Utilities.Semantic;
using Microsoft.AspNetCore.Hosting;

namespace Job_Portal_System.Dependencies
{
    public class TermsManager : ITermsManager
    {
        private readonly ApplicationDbContext _context;
        private readonly IHostingEnvironment _env;

        public TermsManager(ApplicationDbContext context, IHostingEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public JobTitle GetJobTitle(string title)
        {
            var normalizedTitle = title.ToLower();
            var jobTitle = _context.JobTitles
                .FirstOrDefault(j => j.NormalizedTitle == normalizedTitle);
            if (jobTitle != null) return jobTitle;
            {
                jobTitle = new JobTitle
                {
                    Title = title,
                    NormalizedTitle = normalizedTitle,
                };
                var similarities = SimilaritiesOperator.GetSimilarities(normalizedTitle, _env);
                var similarityFound = false;
                foreach (var similarity in similarities)
                {
                    var similarityInDb = _context.JobTitles
                        .FirstOrDefault(j => j.NormalizedTitle == similarity);
                    if (similarityInDb == null) continue;
                    jobTitle.JobTitleSynsetId = similarityInDb.JobTitleSynsetId;
                    similarityFound = true;
                    break;
                }

                if (!similarityFound)
                {
                    jobTitle.JobTitleSynset = new JobTitleSynset();
                }
            }

            return jobTitle;
        }

        public FieldOfStudy GetFieldOfStudy(string title)
        {
            var normalizedTitle = title.ToLower();
            var fieldOfStudy = _context.FieldOfStudies
                .FirstOrDefault(f => f.NormalizedTitle == normalizedTitle);
            if (fieldOfStudy != null) return fieldOfStudy;
            {
                fieldOfStudy = new FieldOfStudy
                {
                    Title = title,
                    NormalizedTitle = normalizedTitle,
                };
                var similarities = SimilaritiesOperator.GetSimilarities(normalizedTitle, _env);
                var similarityFound = false;
                foreach (var similarity in similarities)
                {
                    var similarityInDb = _context.FieldOfStudies
                        .FirstOrDefault(f => f.NormalizedTitle == similarity);
                    if (similarityInDb == null) continue;
                    fieldOfStudy.FieldOfStudySynsetId = similarityInDb.FieldOfStudySynsetId;
                    similarityFound = true;
                    break;
                }

                if (!similarityFound)
                {
                    fieldOfStudy.FieldOfStudySynset = new FieldOfStudySynset();
                }
            }

            return fieldOfStudy;
        }

        public Skill GetSkill(string title)
        {
            var normalizedTitle = title.ToLower();
            var skill = _context.Skills
                .FirstOrDefault(s => s.NormalizedTitle == normalizedTitle);
            if (skill != null) return skill;
            {
                skill = new Skill
                {
                    Title = title,
                    NormalizedTitle = normalizedTitle,
                };
                var similarities = SimilaritiesOperator.GetSimilarities(normalizedTitle, _env);
                var similarityFound = false;
                foreach (var similarity in similarities)
                {
                    var similarityInDb = _context.Skills
                        .FirstOrDefault(s => s.NormalizedTitle == similarity);
                    if (similarityInDb == null) continue;
                    skill.SkillSynsetId = similarityInDb.SkillSynsetId;
                    similarityFound = true;
                    break;
                }

                if (!similarityFound)
                {
                    skill.SkillSynset = new SkillSynset();
                }
            }

            return skill;
        }
    }
}
