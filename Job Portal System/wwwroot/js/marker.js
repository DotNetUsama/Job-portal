
$(document).ready(() => {
    $(".marker").each(function(index, element) {
        $(element).find(".view").html(marked($(element).find(".content").html()));
    });
});