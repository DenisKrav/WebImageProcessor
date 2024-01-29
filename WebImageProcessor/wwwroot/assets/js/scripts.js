document.addEventListener("DOMContentLoaded", function () {
    const uploadBtn = document.getElementById("uploadBtn");
    const captureBtn = document.getElementById("captureBtn");
    const cameraContainer = document.getElementById("cameraContainer");
    const cameraStream = document.getElementById("cameraStream");
    const takePhotoButton = document.getElementById("takePhotoButton");
    const imageContainer = document.getElementById("imageContainer");
    const uploadedImage = document.getElementById("uploadedImage");
    const processedImageContainer = document.getElementById("processedImageBlock");
    const saveLink = document.getElementById("saveLink");
    const processedImg = document.getElementById("processedImg");
    const photoInfBlock = document.getElementById("photoInfBlock");

    let isCameraActive = false;
    let mediaStream = null;


    // Функції для занесення поточного адресу сторінки у кукі файли до натискання на кнопку реєстрації чи входу
    // після натискання кукі буде дійсна 7 днів
    var buttons = document.getElementsByClassName('regBtn');

    function setCookie(name, value, days) {
        var expires = "";
        if (days) {
            var date = new Date();
            date.setTime(date.getTime() + (days * 24 * 60 * 60 * 1000));
            expires = "; expires=" + date.toUTCString();
        }
        document.cookie = name + "=" + (value || "") + expires + "; path=/";
    }

    for (var i = 0; i < buttons.length; i++) {
        buttons[i].addEventListener('click', function () {
            setCookie('lastVisitedURL', window.location.href, 7);
        });
    }

    // Функція для анмації числа на сторінці статистики
    $(document).ready(function () {
        // Знаходимо всі елементи з класом 'statNumber'
        $('.statNumber').each(function () {
            // Отримуємо цільове значення з атрибута 'data-target-value'
            var targetValue = parseInt($(this).data('target-value'));

            // Стартове значення
            var currentValue = 0;

            // Встановлюємо інтервал для збільшення числа
            var interval = setInterval(function () {
                // Змінюємо текст елементу на поточне значення
                $(this).text(currentValue);

                // Збільшуємо поточне значення
                currentValue++;

                // Перевіряємо, чи досягли максимального значення
                if (currentValue > targetValue) {
                    // Зупиняємо інтервал, якщо досягли максимального значення
                    clearInterval(interval);
                }
                // Задаємо інтервал на 50 мілісекунд
            }.bind(this), 50);
        });
    });

    // Функція для передачі нікнейму з таблиці у модальне вікно, які розташовані на сторінці адміна
    $('.delete-user-btn').on('click', function () {
        var nickname = $(this).data('nickname');
        $('#confirm-delete-user').find('.modal-body strong').text(nickname);
        $('#delete-user-nickname').val(nickname);
    });

    // Функція для відображення відеопотоку з камери
    function startCamera() {
        navigator.mediaDevices.getUserMedia({ video: true })
            .then(function (stream) {
                mediaStream = stream;
                cameraStream.srcObject = stream;
                cameraContainer.style.display = "block";
                imageContainer.style.display = "none"; // Приховуємо контейнер зображення
            })
            .catch(function (error) {
                console.error("Error starting the camera: ", error);
            });
    }

    function stopCamera() {
        if (mediaStream) {
            mediaStream.getTracks().forEach(track => track.stop());
            isCameraActive = false;
        }
    }

    function displayImage(file) {
        const reader = new FileReader();
        reader.onload = function (e) {
            uploadedImage.src = e.target.result;
            imageContainer.style.display = "block";
            cameraContainer.style.display = "none"; // Приховуємо контейнер камери
            stopCamera(); // Зупиняємо відеопотік, якщо він активний
        };
        reader.readAsDataURL(file);
    }

    uploadBtn.addEventListener("click", function () {       
        const fileInput = document.createElement("input");
        fileInput.type = "file";
        fileInput.accept = "image/*";
        fileInput.style.display = "none";
        fileInput.addEventListener("change", function () {
            const file = fileInput.files[0];
            if (file) {
                displayImage(file);

                // Встановлюємо значення для #imageFile
                document.getElementById("imageFile").files = fileInput.files;
            }
        });
        fileInput.click();
        processedImageContainer.style.display = "none";
        photoInfBlock.style.display = "none";
    });

    captureBtn.addEventListener("click", function () {
        if (!isCameraActive) {
            startCamera();
            isCameraActive = true;
        }
        processedImageContainer.style.display = "none";
        photoInfBlock.style.display = "none";
    });

    takePhotoButton.addEventListener("click", function () {
        if (mediaStream) {
            const canvas = document.createElement("canvas");
            canvas.width = cameraStream.videoWidth;
            canvas.height = cameraStream.videoHeight;
            const ctx = canvas.getContext("2d");
            ctx.drawImage(cameraStream, 0, 0, canvas.width, canvas.height);

            // Отримуємо зображення у форматі Blob
            canvas.toBlob(function (blob) {
                // Створюємо File об'єкт з Blob
                const photoFile = new File([blob], "photo.jpg", { type: "image/jpeg" });

                // Відображаємо зображення
                uploadedImage.src = URL.createObjectURL(photoFile);

                // Встановлюємо значення для #imageFile
                const imageFileInput = document.getElementById("imageFile");
                const files = new DataTransfer();
                files.items.add(photoFile);
                imageFileInput.files = files.files;

                // Відображаємо відповідні контейнери
                imageContainer.style.display = "block";
                cameraContainer.style.display = "none"; // Приховуємо контейнер камери

                // Зупиняємо відеопотік
                stopCamera();
            }, "image/jpeg");
        }
    });

    // Функція для збереження обробленого фото
    saveLink.addEventListener("click", function () {
        if (processedImg.src) {
            const a = document.createElement("a");
            a.href = processedImg.src;
            a.download = "processed_image.png";
            a.click();
        }
    });
});
