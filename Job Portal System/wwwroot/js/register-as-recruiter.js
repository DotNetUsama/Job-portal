$(document).ready(() => {
    $("#company-name").autocomplete({ source: "/AutoComplete/Companies" });
});