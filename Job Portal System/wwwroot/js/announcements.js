$.validator.setDefaults({
    ignore: null
});

$(document).ready(() => {
    uploadImage({
        fileInput: $("#file-input"),
        urlInput: $("#url-input"),
        errorMessage: $("#error-message"),
        imageViewer: $("#image-viewer"),
        loader: $("#loader"),
        shader: $("#shader")
    });

    new SimpleMDE({
        element: $("#editor")[0],
        forceSync: true
    });
});