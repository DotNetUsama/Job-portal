using Job_Portal_System.Models;
using System.Collections.Generic;

namespace Job_Portal_System.Dependencies
{
    public interface ITermsManager
    {
        JobTitle GetJobTitle(string title);
        FieldOfStudy GetFieldOfStudy(string title);
        Skill GetSkill(string title);
        long? GetJobTitleSynset(string title);
        long? GetFieldOfStudySynset(string title);
        long? GetSkillSynset(string title);
        IEnumerable<long> GetSimilarJobTitles(string title);
    }
}
