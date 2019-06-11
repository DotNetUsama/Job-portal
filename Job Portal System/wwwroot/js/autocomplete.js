const autoComplete = ({ inputFieldSelector,
    idSelector,
    dataUrl,
    similaritiesUrl,
    nameSelector,
    change,
    allowNewEntry,
    delay
}) => {
    $.ajax({
        url: dataUrl,
        method: "post",
        dataType: "json",
        success: data => {
            const response = similaritiesUrl ?
                (event, ui) => {
                    if (!ui.content.length) {
                        const query = event.target.value;
                        $.ajax({
                            url: similaritiesUrl + "?query=" + query,
                            method: "post",
                            dataType: "json",
                            async: false,
                            success: similarities => {
                                similarities = similarities.map(item => { return { value: item.label, query: query, ...item }; });
                                Array.prototype.push.apply(ui.content, similarities);
                            },
                            error: err => console.log(err)
                        });
                    }
                } :
                () => { };

            const changeMethod = (event, ui) => {
                if (ui.item) {
                    $(idSelector).val(ui.item.id);
                    if (nameSelector) $(nameSelector).val(ui.item.query || ui.item.value);
                    if (change) change(ui.item.id);
                } else {
                    const $this = $(event.currentTarget);
                    const enteredValue = $this.val();
                    const itemInData = data.find(item => item.label.trim().toLowerCase() === enteredValue.trim().toLowerCase());
                    if (itemInData) {
                        $(idSelector).val(itemInData.id);
                        if (nameSelector) $(nameSelector).val(itemInData.label);
                        if (change) change(itemInData.id);
                    } else {
                        if (!allowNewEntry) {
                            $this.val(null);
                            $this.focus();
                        }
                        $(idSelector).val(null);
                        if (nameSelector) $(nameSelector).val(null);
                    }
                }
            };

            $(inputFieldSelector).autocomplete({
                source: data,
                delay: delay,
                change: changeMethod,
                response: response
            });
        },
        error: err => console.log(err)
    });
};