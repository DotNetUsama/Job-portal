$.validator.setDefaults({
    ignore: null
});

$(document).ready(() => {
    uploadImage({
        fileInputSelector: "#file-input",
        urlInputSelector: "#url-input",
        errorMessageSelector: "#error-message",
        imageViewerSelector: "#image-viewer",
        loaderSelector: "#loader",
        shaderSelector: "#shader"
    });

    var simplemde = new SimpleMDE({
        element: $("#editor")[0],
        forceSync: true
    });
});