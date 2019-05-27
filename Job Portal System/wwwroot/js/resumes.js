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
        inputFieldSelector: "#seeked-job-title-input",
        idSelector: "#seeked-job-title-id",
        dataUrl: "/AutoComplete/JobTitles",
        similaritiesUrl: "/AutoComplete/SimilarJobTitles",
        nameSelector: "#seeked-job-title-name",
        allowNewEntry: false,
        delay: 800
    });
    autoComplete({
        inputFieldSelector: "#company-input",
        idSelector: "#company-id",
        dataUrl: "/AutoComplete/Companies",
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

    $("#seeked-job-titles-sub-form").subform(() => {
        const nameSpan = $("<span class=\"font-weight-bold\"></span>");
        nameSpan.html($("#seeked-job-title-name").val());
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

    tinymce.init({
        selector: "textarea#biography",
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