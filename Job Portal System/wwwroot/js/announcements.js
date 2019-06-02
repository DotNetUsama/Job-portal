$.validator.setDefaults({
    ignore: null
});

$(document).ready(() => {
    tinymce.init({
        selector: "textarea#content",
        height: 300,
        menubar: "view",
        plugins: [
            "advlist autolink lists link image charmap print preview anchor textcolor",
            "searchreplace visualblocks code fullscreen",
            "insertdatetime media table paste code help wordcount"
        ],
        toolbar: "undo redo | bold italic strikethrough forecolor backcolor permanentpen formatpainter | link image | alignleft aligncenter alignright alignjustify  | numlist bullist outdent indent | removeformat | fullscrreen | help",
        content_css: [
            "//fonts.googleapis.com/css?family=Lato:300,300i,400,400i",
            "//www.tiny.cloud/css/codepen.min.css"
        ]
    });
});