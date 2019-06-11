$(document).ready(() => {
    uploadImage({
        fileInputSelector: "#file-input",
        urlInputSelector: "#url-input",
        errorMessageSelector: "#error-message",
        imageViewerSelector: "#image-viewer",
        loaderSelector: "#loader",
        shaderSelector: "#shader"
    });
});