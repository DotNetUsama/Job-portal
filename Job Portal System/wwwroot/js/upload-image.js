const uploadImage = ({ fileInputSelector, urlInputSelector, errorMessageSelector,
    imageViewerSelector, loaderSelector, shaderSelector }) => {
    const url = window.URL || window.webkitURL;
    const fileInput = $(fileInputSelector);
    const urlInput = $(urlInputSelector);
    const imageViewer = $(imageViewerSelector);
    const errorMessage = $(errorMessageSelector);
    const loader = $(loaderSelector);
    const shader = $(shaderSelector);

    fileInput.on("change", function () {
        const file = fileInput[0].files[0];
        if (file.size > 1048576) {
            errorMessage.html("Image is too big, only images less than 1MB allowed.");
        } else {
            const img = new Image();
            let imgWidth = 0, imgHeight = 0;
            const maxWidth = 640, maxHeight = 640;
            img.src = url.createObjectURL(file);
            img.onload = function () {
                imgWidth = this.width;
                imgHeight = this.height;
                if (imgWidth <= maxWidth && imgHeight <= maxHeight) {
                    const formData = new FormData();
                    formData.append("image", file);
                    shader.css("display", "block");
                    loader.css("display", "block");
                    $.ajax({
                        url: "/Util/UploadImage",
                        type: "POST",
                        data: formData,
                        processData: false,
                        contentType: false,
                        dataType: "json",
                        success: (response) => {
                            if (response["uploaded"]) {
                                const imageUrl = response["url"];
                                urlInput.val(imageUrl);
                                imageViewer.attr("src", imageUrl);
                                imageViewer.show();
                            } else {
                                errorMessage.html("Couldn't upload the image.");
                            }
                            shader.css("display", "");
                            loader.css("display", "");
                        },
                        error: (err) => {
                            console.log(err);
                            shader.css("display", "");
                            loader.css("display", "");
                        }
                    });
                } else {
                    errorMessage.html(`Image dimensions should be less than ${maxWidth}x${maxHeight}`);
                }
            }
        }
    });
};