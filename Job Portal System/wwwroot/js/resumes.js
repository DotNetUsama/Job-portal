$.validator.setDefaults({
    ignore: null
});

$(document).ready(() => {

    $("#field-of-study-input").autocomplete({ source: "/AutoComplete/FieldsOfStudy" });
    $("#work-experience-job-title-input").autocomplete({ source: "/AutoComplete/JobTitles" });
    $("#seeked-job-title-input").autocomplete({ source: "/AutoComplete/JobTitles" });
    $("#company-input").autocomplete({ source: "/AutoComplete/Companies" });
    $("#skill-input").autocomplete({ source: "/AutoComplete/Skills" });
    $("#school-input").autocomplete({ source: "/AutoComplete/Schools" });

    $("#seeked-job-titles-sub-form").subform(() => {
        const nameSpan = $("<span class=\"font-weight-bold\"></span>");
        nameSpan.html($("#seeked-job-title-input").val());
        return nameSpan;
    });
    $("#owned-skills-sub-form").subform(() => {
        const overallSpan = $("<span>");
        const nameSpan = $("<span class=\"font-weight-bold\"></span>");
        nameSpan.html($("#skill-input").val());
        const yearsSpan = $("<span class=\"font-weight-bold\"></span>");
        yearsSpan.html($("#skill-years").val());
        overallSpan.append(yearsSpan);
        overallSpan.append(" years in ");
        overallSpan.append(nameSpan);
        return overallSpan;
    });
    $("#work-experiences-partial").subform(() => {
        const overallSpan = $("<span>");
        const jobTitleSpan = $("<span class=\"font-weight-bold\"></span>");
        jobTitleSpan.html($("#work-experience-job-title-input").val());
        const companySpan = $("<span class=\"font-weight-bold\"></span>");
        companySpan.html($("#company-input").val());
        overallSpan.append(jobTitleSpan);
        overallSpan.append(" in ");
        overallSpan.append(companySpan);
        return overallSpan;
    });
    $("#educations-partial").subform(() => {
        const overallSpan = $("<span>");
        const fieldOfStudySpan = $("<span class=\"font-weight-bold\"></span>");
        fieldOfStudySpan.html($("#field-of-study-input").val());
        const schoolSpan = $("<span class=\"font-weight-bold\"></span>");
        schoolSpan.html($("#school-input").val());
        overallSpan.append(fieldOfStudySpan);
        overallSpan.append(" in ");
        overallSpan.append(schoolSpan);
        return overallSpan;
    });
});