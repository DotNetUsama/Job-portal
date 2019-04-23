var currentTab = 0;

$(document).ready(() => {
    showTab(currentTab);
});

function showTab(tapIndex) {
    const tabs = $(".tab");
    $(tabs[tapIndex]).show();

    $("#prevBtn").css("display", tapIndex === 0 ? "none" : "inline");
    $("#nextBtn").css("display", tapIndex === tabs.length - 1 ? "none" : "inline");

    const currentStep = tapIndex + 1;
    const stepsCount = tabs.length;

    $("#step-num").html(currentStep);
    $("#steps-count").html(stepsCount);

    const progressBar = $(".progress-bar");
    progressBar.css("width", currentStep / stepsCount * 100 + "%");
    if (currentStep === stepsCount) {
        progressBar.removeClass("bg-info").addClass("bg-success");
    } else {
        progressBar.addClass("bg-info").removeClass("bg-success");
    }
}

function nextPrev(n) {
    const tabs = $(".tab");
    $(tabs[currentTab]).css("display", "none");
    currentTab = currentTab + n;
    if (currentTab >= tabs.length) {
        return false;
    }
    showTab(currentTab);
    return true;
}