$(document).ready(() => {
    autoComplete({
        inputFieldSelector: "#company-name",
        idSelector: "#company-id",
        dataUrl: "/AutoComplete/Companies"
    });
});