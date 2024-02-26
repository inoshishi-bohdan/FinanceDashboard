// imageInterop.js
window.changeImageSource = function (imageId, newSource) {
    var image = document.getElementById(imageId);
    if (image) {
        if (newSource !== "") {
            image.src = `/images/profiles/${newSource}`;
        }
    }
}
window.selectImage =  function (imageId) {
    // Deselect all images with class 'image-option'
    var images = document.querySelectorAll('.image-option');
    images.forEach(function (image) {
        if (image.classList.contains('selected')) {
            if (image.id == imageId) {
                return;
            }
            image.classList.remove('selected');
        }
    });

    // Select the specified image by ID
    var selectedImage = document.getElementById(imageId);
    if (selectedImage) {
        selectedImage.classList.add('selected');
    }
}
