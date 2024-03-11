
  $("input.image-upload").on("change", (e) => {
    const fileInput = e.target;
    const formData = new FormData();
    formData.append("file", fileInput.files[0]);
    const container = fileInput.closest('.news-model-form');
    const index = fileInput.getAttribute("data-id");
    fetch(`/News/Uploads/${index}`, {
        method: "post",
        body: formData
    }).then(r => r.json())
        .then(data => {
            console.log(data);
        });
});
    function addDeleteEventListeners() {
            // Add event listeners for deleting news models
            const deleteButtons = document.querySelectorAll('.delete-news-model');

            deleteButtons.forEach(button => {
        button.addEventListener('click', function () {
            const container = this.closest('.news-model-form');
            const index = Array.from(container.parentElement.children).indexOf(container);

            if (index !== -1) {

                newsModels.splice(index, 1);

                container.parentElement.removeChild(container);
            }
        });
            });
        }
    addDeleteEventListeners();
document.getElementById('add-news-model').addEventListener('click', function () {
    const newNewsModel = {
        Id: 0,
        Title: '',
        ImageUrl: '',
        ShortText: '',
        FullText: '',
        Date: ''
    };
    console.log("hi");
    const template = document.querySelector('.news-model-form');
    const clone = template.cloneNode(true);
    document.getElementById('news-form-container').appendChild(clone);
    const clonedForm = clone.querySelector('.edit-news-model-form');
    clonedForm.querySelector('input[name="Id"]').value = newNewsModel.Id;
    clonedForm.querySelector('input[name="Title"]').value = newNewsModel.Title;
    clonedForm.querySelector('input[name="ImageUrl"]').value = newNewsModel.ImageUrl;
    clonedForm.querySelector('textarea[name="ShortText"]').value = newNewsModel.ShortText;
    clonedForm.querySelector('textarea[name="FullText"]').value = newNewsModel.FullText;
    clonedForm.querySelector('input[name="Date"]').value = newNewsModel.Date;
    addDeleteEventListeners();
});
document.getElementById('save-news-models').addEventListener('click', function () {
    const forms = document.querySelectorAll('.edit-news-model-form');
    const newsModelsData = [];

    forms.forEach(form => {
        const formData = new FormData(form);
        const newsModel = {
            Id: formData.get('Id'),
            Title: formData.get('Title'),
            ImageUrl: formData.get('ImageUrl'),
            ShortText: formData.get('ShortText'),
            FullText: formData.get('FullText'),
            Date: formData.get('Date')
        };
        newsModelsData.push(newsModel);
    });

    document.getElementById('save-news-models').addEventListener('click', function () {
            const forms = document.querySelectorAll('.edit-news-model-form');
    const newsModelsData = [];

            forms.forEach(form => {
                const formData = new FormData(form);
    const newsModel = {
        Id: formData.get('Id'),
    Title: formData.get('Title'),
    ShortText: formData.get('ShortText'),
    FullText: formData.get('FullText'),
    Date: formData.get('Date')
                };
    newsModelsData.push(newsModel);
            });

    // Make an AJAX request to save the news models
    fetch('/News/SaveNewsModels', {
        method: 'POST',
    body: JSON.stringify(newsModelsData),
    headers: {
        'Content-Type': 'application/json'
                }
            })
                .then(response => {
                    if (!response.ok) {
                        throw new Error('Network response was not ok');
                    }
    return response.json();
                })
                .then(data => {
                    if (data.success) {
        console.log(newsModels)
                        Swal.fire({
        icon: 'success',
    title: 'Success',
    text: 'News models saved!',
    confirmButtonText: 'OK'
                        }).then(() => {
        // Redirect to the main news page
        window.location.href = '/News/Index';
                        });
                    } else {
        // Show an error message using SweetAlert
        Swal.fire({
            icon: 'error',
            title: 'Error',
            text: 'An error occurred. Please try again.',
            confirmButtonText: 'OK'
        });
                    }
                })
                .catch(error => {
        console.error('Error:', error);
                });
        });