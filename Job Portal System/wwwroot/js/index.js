$(document).ready(() => {
    $("#search-input").autocomplete({ source: "/AutoComplete/JobTitles" });
});