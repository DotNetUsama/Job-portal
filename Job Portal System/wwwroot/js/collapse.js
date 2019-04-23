$(document).ready(() => {
    $(".extend").click(function () {
        const $this = $(this);
        const container = $this.closest(".collapse-container");
        const content = container.find(".collapse-content");
        $this.html("");
        if (content.is(":visible")) {
            content.slideUp(300);
            $this.append($("<i>").addClass("fa fa-angle-down"));
        } else {
            content.slideDown(300);
            $this.append($("<i>").addClass("fa fa-angle-right"));
        }
    });
});