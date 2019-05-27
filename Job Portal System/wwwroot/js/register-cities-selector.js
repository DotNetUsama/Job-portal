$(document).ready(() => {
    $(".state-select").each((i, obj) => {
        const stateSelect = $(obj);
        stateSelect.on("change", () => {
            const selectContainer = stateSelect.closest(".city-select-container");
            const citySelect = $(selectContainer.find(".city-select")[0]);
            citySelect.prop("disabled", false);
            citySelect.find("option").remove().end()
                    .append('<option value="">Select city</option>').val(null);
            $.ajax({
                url: `/AutoComplete/Cities?stateId=${stateSelect.val()}`,
                method: "post",
                success: function (cities) {
                    cities.forEach(city => {
                        citySelect.append(new Option(city.label, city.id));
                    });
                },
                error: function (err) {
                    console.log(err);
                }
            });
        });
    });
});