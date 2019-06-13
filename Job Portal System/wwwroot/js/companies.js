$.validator.setDefaults({
    ignore: null
});

$(document).ready(() => {
    $("#departments-partial").subform(function () {
        const nameSpan = $("<span class=\"font-weight-bold\"></span>");
        const stateInput = $("#state-input option:selected").text();
        const cityInput = $("#city-input option:selected").text();
        nameSpan.html(`${stateInput} - ${cityInput} - ${$("#detailed-address-input").val()}`);
        return nameSpan;
    });

    new SimpleMDE({
        element: $("#description")[0],
        forceSync: true
    }); 
});