using Job_Portal_System.Models;

namespace Job_Portal_System.Dependencies
{
    public interface ITermsManager
    {
        JobTitle GetJobTitle(string title);
        FieldOfStudy GetFieldOfStudy(string title);
        Skill GetSkill(string title);
    }
}
