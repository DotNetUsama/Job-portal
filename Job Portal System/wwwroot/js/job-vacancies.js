$.validator.setDefaults({
    ignore: null
});

$(document).ready(() => {
    autoComplete({
        inputFieldSelector: "#field-of-study-input",
        idSelector: "#field-of-study-id",
        dataUrl: "/AutoComplete/FieldsOfStudy",
        similaritiesUrl: "/AutoComplete/SimilarFieldsOfStudy",
        nameSelector: "#field-of-study-name",
        allowNewEntry: false,
        delay: 800
    });
    autoComplete({
        inputFieldSelector: "#work-experience-job-title-input",
        idSelector: "#work-experience-job-title-id",
        dataUrl: "/AutoComplete/JobTitles",
        similaritiesUrl: "/AutoComplete/SimilarJobTitles",
        nameSelector: "#work-experience-job-title-name",
        allowNewEntry: false,
        delay: 800
    });

    autoComplete({
        inputFieldSelector: "#job-title-input",
        idSelector: "#job-title-id",
        dataUrl: "/AutoComplete/JobTitles",
        allowNewEntry: true,
        delay: 300
    });

    autoComplete({
        inputFieldSelector: "#company-department-input",
        idSelector: "#company-department-id",
        dataUrl: `/AutoComplete/CompanyDepartments?companyId=${$("#company-id").val()}`,
        allowNewEntry: true,
        delay: 300
    });
    autoComplete({
        inputFieldSelector: "#skill-input",
        idSelector: "#skill-id",
        dataUrl: "/AutoComplete/Skills",
        allowNewEntry: true,
        delay: 300
    });
    
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

    tinymce.init({
        selector: "textarea#description",
        height: 300,
        menubar: "view",
        plugins: [
            "advlist autolink lists link image charmap print preview anchor textcolor",
            "searchreplace visualblocks code fullscreen",
            "insertdatetime media table paste code help wordcount"
        ],
        toolbar: "undo redo | bold italic strikethrough forecolor backcolor permanentpen formatpainter | link image | alignleft aligncenter alignright alignjustify  | numlist bullist outdent indent | removeformat | fullscrreen | help",
        content_css: [
            "//fonts.googleapis.com/css?family=Lato:300,300i,400,400i",
            "//www.tiny.cloud/css/codepen.min.css"
        ]
    });
});