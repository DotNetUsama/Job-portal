using System.Collections.Generic;
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
            var normalizedTitle = title.ToLower().Split(" - ").Last();
            var jobTitle = _context.JobTitles
                .FirstOrDefault(j => j.NormalizedTitle == normalizedTitle);
            if (jobTitle != null) return jobTitle;

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

            return jobTitle;
        }

        public FieldOfStudy GetFieldOfStudy(string title)
        {
            var normalizedTitle = title.ToLower().Split(" - ").Last();
            var fieldOfStudy = _context.FieldOfStudies
                .FirstOrDefault(f => f.NormalizedTitle == normalizedTitle);
            if (fieldOfStudy != null) return fieldOfStudy;

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

            return fieldOfStudy;
        }

        public Skill GetSkill(string title)
        {
            var normalizedTitle = title.ToLower().Split(" - ").Last();
            var skill = _context.Skills
                .FirstOrDefault(s => s.NormalizedTitle == normalizedTitle);
            if (skill != null) return skill;

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

            return skill;
        }

        public long? GetJobTitleSynset(string title)
        {
            var normalizedTitle = title.ToLower().Split(" - ").Last();
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
            return jobTitleSynsetId;
        }

        public long? GetFieldOfStudySynset(string title)
        {
            var normalizedTitle = title.ToLower().Split(" - ").Last();
            var fieldOfStudySynsetId = _context.FieldOfStudies
                .FirstOrDefault(j => j.NormalizedTitle == normalizedTitle)?.FieldOfStudySynsetId;

            if (fieldOfStudySynsetId == null)
            {
                var similarities = SimilaritiesOperator.GetSimilarities(normalizedTitle, _env);
                foreach (var similarity in similarities)
                {
                    fieldOfStudySynsetId = _context.FieldOfStudies
                        .FirstOrDefault(j => j.NormalizedTitle == similarity)?.FieldOfStudySynsetId;
                    if (fieldOfStudySynsetId != null) break;
                }
            }
            return fieldOfStudySynsetId;
        }

        public long? GetSkillSynset(string title)
        {
            var normalizedTitle = title.ToLower().Split(" - ").Last();
            var skillSynsetId = _context.Skills
                .FirstOrDefault(j => j.NormalizedTitle == normalizedTitle)?.SkillSynsetId;

            if (skillSynsetId == null)
            {
                var similarities = SimilaritiesOperator.GetSimilarities(normalizedTitle, _env);
                foreach (var similarity in similarities)
                {
                    skillSynsetId = _context.Skills
                        .FirstOrDefault(j => j.NormalizedTitle == similarity)?.SkillSynsetId;
                    if (skillSynsetId != null) break;
                }
            }
            return skillSynsetId;
        }


        public IEnumerable<long> GetSimilarJobTitles(string title)
        {
            var synsetId = GetJobTitleSynset(title);
            return synsetId == null ? null : _context.JobTitles
                .Where(j => _context.JobTitles
                    .Any(jt => jt.JobTitleSynsetId == synsetId && j.Title.Contains(jt.Title)))
                .Select(j => j.Id)
                .ToList();
        }
    }
}
