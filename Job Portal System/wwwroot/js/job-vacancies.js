$.validator.setDefaults({
    ignore: null
});

$(document).ready(() => {
    $("#field-of-study-input").autocomplete({ source: "/AutoComplete/FieldsOfStudy" });
    $("#work-experience-job-title-input").autocomplete({ source: "/AutoComplete/JobTitles" });
    $("#job-title-input").autocomplete({ source: "/AutoComplete/JobTitles" });
    $.ajax({
        url: `/AutoComplete/CompanyDepartments?companyId=${$("#company-id").val()}`,
        method: "get",
        success: departments => {
            $("#company-department-input").autocomplete({
                source: departments,
                change: (event, ui) => {
                    const $idInput = $("#company-department-id");
                    if (ui.item) {
                        $idInput.val(ui.item.id);
                    } else {
                        const $this = $(event.currentTarget);
                        const enteredValue = $this.val();
                        const itemInData = departments.find(department =>
                            department.label.trim().toLowerCase() === enteredValue.trim().toLowerCase());
                        if (itemInData) {
                            $idInput.val(itemInData.id);
                        } else {
                            $idInput.val(null);
                            $this.val(null);
                            $this.focus();
                        }
                    }
                }
            });
        },
        error: err => console.log(err)
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