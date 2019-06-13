$.validator.setDefaults({
    ignore: null
});

$(document).ready(() => {
    $("#field-of-study-input").autocomplete({ source: "/AutoComplete/FieldsOfStudy" });
    $("#work-experience-job-title-input").autocomplete({ source: "/AutoComplete/JobTitles" });
    $("#job-title-input").autocomplete({ source: "/AutoComplete/JobTitles" });
    $("#company-department-input").autocomplete({
        source: `/AutoComplete/CompanyDepartments?companyId=${$("#company-id").val()}`
    });
    $("#skill-input").autocomplete({ source: "/AutoComplete/Skills" });
    
    $("#desired-skills-sub-form").subform(() => {
        const overallSpan = $("<span>");
        const nameSpan = $("<span class=\"font-weight-bold\"></span>");
        nameSpan.html($("#skill-input").val());
        const yearsSpan = $("<span class=\"font-weight-bold\"></span>");
        yearsSpan.html($("#skill-years").val());
        overallSpan.append("Minimum ");
        overallSpan.append(yearsSpan);
        overallSpan.append(" years in ");
        overallSpan.append(nameSpan);
        return overallSpan;
    });
    $("#work-experiences-partial").subform(() => {
        const jobTitleSpan = $("<span class=\"font-weight-bold\"></span>");
        jobTitleSpan.html($("#work-experience-job-title-input").val());
        return jobTitleSpan;
    });
    $("#educations-partial").subform(() => {
        const fieldOfStudySpan = $("<span class=\"font-weight-bold\"></span>");
        fieldOfStudySpan.html($("#field-of-study-input").val());
        return fieldOfStudySpan;
    });

    new SimpleMDE({
        element: $("#description")[0],
        forceSync: true
    });
});