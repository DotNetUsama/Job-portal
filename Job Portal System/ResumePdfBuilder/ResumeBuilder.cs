using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using DinkToPdf;
using DinkToPdf.Contracts;
using Job_Portal_System.Models;
using Microsoft.AspNetCore.Hosting;

namespace Job_Portal_System.ResumePdfBuilder
{
    public static class ResumeBuilder
    {
        private static StringBuilder _builder;


        public static byte[] GetResumeAsPdf(IHostingEnvironment env,
            IConverter converter, Resume resume)
        {

            var globalSetting = new GlobalSettings
            {
                ColorMode = ColorMode.Color,
                Orientation = Orientation.Portrait,
                PaperSize = PaperKind.A4,
                Margins = new MarginSettings { Top = 0, Left = 0, Right = 0, Bottom = 0 },
                DocumentTitle = $"{resume.User.LastName} Resume"
            };
            var objectSettings = new ObjectSettings
            {
                PagesCount = true,
                HtmlContent = GetResumeHtmlContent(env, resume),
                WebSettings =
                {
                    DefaultEncoding = "utf-8",
                    UserStyleSheet = GetResumeCss(env),
                },
                HeaderSettings =
                {
                    FontName = "Arial",
                    FontSize = 9
                },
                FooterSettings =
                {
                    FontName = "Arial",
                    FontSize = 7,
                    Line = true,
                    Left = "This resume created on ALNSER site.",
                    Spacing = 3.5
                },
            };

            var file = new HtmlToPdfDocument
            {
                GlobalSettings = globalSetting,
                Objects = { objectSettings }
            };
            return converter.Convert(file);
        }


        private static string GetResumeHtmlContent(IHostingEnvironment env ,Resume resume)
        {
            var htmlFilePath = Path.Combine(env.ContentRootPath, "ResumePdfBuilder", 
                "PdfTemplate", "Template.html");
            string htmlText;
            try
            {
                using (var template = new StreamReader(htmlFilePath))
                {
                    htmlText = ConvertHtmlToString(template, false , resume);
                    template.Close();
                }
            }
            catch (Exception exception)
            {
                return exception.StackTrace;
            }

            return htmlText;
        }

        private static string GetResumeCss(IHostingEnvironment env)
        {
            return Path.Combine(env.ContentRootPath, "ResumePdfBuilder",
                "PdfTemplate", "TemplateStyle.css");
        }

        private static string ConvertHtmlToString(TextReader streamToRead, bool isHtml , Resume resume)
        {
            var body = new StringBuilder();
            var nextTag = new StringBuilder();
            var inTag = false;
            const char tagStart = '$';

            while (streamToRead.Peek() >= 0)
            {
                var nextCharacter = Convert.ToChar(streamToRead.Peek());
                if (nextCharacter.Equals(tagStart)) inTag = !inTag;

                if (inTag)
                {
                    nextTag.Append(Convert.ToChar(streamToRead.Read()));
                    if (nextTag.Length >= 50)
                    {
                        body.Append(nextTag.ToString());
                        nextTag.Length = 0;
                        inTag = false;
                    }
                }
                else if (nextTag.Length > 0)
                {
                    if (nextCharacter.Equals(tagStart)) nextTag.Append(Convert.ToChar(streamToRead.Read()));
                    body.Append(ReplaceHtmlValues(nextTag.ToString(), isHtml , resume));
                    nextTag.Length = 0;
                }
                else
                {
                    body.Append(Convert.ToChar(streamToRead.Read()));
                }
            }

            return body.ToString();
        }

        private static string AppendMainDetails(User user , IReadOnlyList<SeekedJobTitle> seekedJobTitles)
        {

            _builder.AppendFormat(@"<div id='name'><h2>{0}</h2>", $"{user.FirstName} {user.LastName}");
            _builder.Append("<ul class='jobTitles'>");

            if (seekedJobTitles.Count != 0)
            {
                for (var i = 0; i < seekedJobTitles.Count; i = i + 2)
                {
                    if (i < seekedJobTitles.Count - 1)
                        _builder.AppendFormat(@"<li>{0}, {1}</li>", seekedJobTitles[i].JobTitle.Title, seekedJobTitles[i + 1].JobTitle.Title);
                    else
                        _builder.AppendFormat(@"<li>{0}</li>", seekedJobTitles[i].JobTitle.Title);
                }
            }
            _builder.Append("</ul></div><div id='contactDetails' style='float: right;'><ul>");
            _builder.AppendFormat(@"<li><strong>Email: </strong> <a href='{0}'>{1}</a></li>",
                user.Email, user.Email);
            _builder.AppendFormat(@"<li><strong>Birthdate: </strong>{0}<li>",
                user.BirthDate);
            _builder.Append("</ul></div>");

            return _builder.ToString();
        }

        private static string AppendEducations(List<Education> educations)
        {
            if (educations.Count == 0) return _builder.ToString();

            _builder.Append(@"<section>
                            <div class='sectionTitle'><h1><i class='fa fa-graduation-cap'></i> Educations</h1></div>
                            <div class='sectionContent'>");

            foreach (var education in educations)
            {
                _builder.AppendFormat(@"<article><h2> {0} in {1} </h2>" +
                                      "<p class = 'subDetails'>{2}</p>" +
                                      "<p>{3}</p></article>"

                    , education.FieldOfStudy.Title, education.School.Name, education.Degree
                    , "Graduated since " + education.EndDate + " with degree " + education.Degree + " in " +
                      education.FieldOfStudy.Title + " from " + education.School.Name + " in " + education.School.City.Name + ".");
            }

            _builder.Append(@"</div>
                            <div class='clear'></div>
                            </section>");

            return _builder.ToString();
        }

        private static string AppendWorkExperiences(List<WorkExperience> workExperiences)
        {
            if (workExperiences.Count == 0) return _builder.ToString();

            _builder.Append(@"<section>
                            <div class='sectionTitle'><h1><i class='fa fa-briefcase'></i> Work Experiences</h1></div>
                            <div class='sectionContent'>");

            foreach (var workExperience in workExperiences)
            {
                var endDate = workExperience.EndDate.HasValue ? workExperience.EndDate.Value.Year.ToString() : "persent";

                _builder.AppendFormat(@"<article><h2> {0} at {1} </h2>" +
                                     "<p class = 'subDetails'>{2}</p>" +
                                     "<p>{3}</p></article>"

                    , workExperience.JobTitle.Title, workExperience.Company.Name, $"{workExperience.StartDate.Year} to {endDate}"
                    , "Worked as " + workExperience.JobTitle.Title + " in " + workExperience.Company.Name);
            }

            _builder.Append(@"</div>
                            <div class='clear'></div>
                            </section>");

            return _builder.ToString();
        }

        private static string AppendSkills(List<OwnedSkill> skills)
        {
            if (skills.Count == 0) return _builder.ToString();

            _builder.Append(@"
                <section>
                    <div class='sectionTitle'>
                        <h1><i class='fa fa-star'></i> Skills </h1>
                    </div>
                    <div class='sectionContent'>
                        <ul class='keySkills'>");

            foreach (var skill in skills)
            {
                _builder.AppendFormat(@"<li>{0} years in {1}</li>"

                    , skill.Years, skill.Skill.Title);
            }

            _builder.Append(@"
                        </ul>
                    </div>
                <div class='clear'></div>
            </section>");

            return _builder.ToString();
        }

        private static string ReplaceHtmlValues(string tag, bool isHtml , Resume resume)
        {
            _builder = new StringBuilder();
            string returnValue;

            tag = tag.Trim();

            switch (tag)
            {
                case "$mainDetails$":
                    returnValue = AppendMainDetails(resume.User, resume.SeekedJobTitles);
                    break;
                case "$Educations$":
                    returnValue = AppendEducations(resume.Educations);
                    break;
                case "$WorkExperiences$":
                    returnValue = AppendWorkExperiences(resume.WorkExperiences);
                    break;
                case "$Skills$":
                    returnValue = AppendSkills(resume.OwnedSkills);
                    break;
                default:
                    returnValue = "";
                    break;
            }
            return returnValue;
        }

        
    }
}
