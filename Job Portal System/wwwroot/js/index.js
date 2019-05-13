$(document).ready(() => {
    $.ajax({
        url: "/AutoComplete/JobTitlesWithSimilarities",
        method: "post",
        dataType: "json",
        success: data => {
            $("#search-input").autocomplete({
                source: data,
                delay: 200
            });
        }
    });
});