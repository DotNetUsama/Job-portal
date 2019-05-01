$(document).ready(() => {
    autoComplete({
        inputFieldSelector: "#company-name",
        idSelector: "#company-id",
        dataUrl: "/AutoComplete/Companies",
        allowNewEntry: true,
        delay: 300
    });
});