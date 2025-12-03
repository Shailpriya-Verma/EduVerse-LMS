function togglePriceField() {
    const isPaid = document.getElementById("IsPaid").value === "true";
    document.getElementById("priceGroup").classList.toggle("d-none", !isPaid);
}

$(document).ready(function () {

    // prevent form submit on Enter:
    document.addEventListener("keydown", function (e) {
        if (e.key === "Enter") {
            e.preventDefault();
            $('#btn').click();
        }
    });

    $('#btn').click(function () {

        var Title = $("#Title").val().trim();
        var Description = $("#Description").val().trim();
        var IsPaid = $("#IsPaid").val();
        var Price = $("#Price").val().trim();
        var Thumbnail = $("#Thumbnail")[0].files[0];

        // ==== BASIC VALIDATION ON FIELDS ====
        if (Title === "") { alert("Please enter course title."); return; }
        if (Description === "") { alert("Please enter course description."); return; }
        if (IsPaid === "true" && (Price === "" || parseFloat(Price) <= 0)) {
            alert("Please enter a valid price for the paid course."); return;
        }
        if (!Thumbnail) { alert("Please upload a thumbnail."); return; }

        // thumbnail validation
        var allowedImages = /(\.jpg|\.jpeg|\.png)$/i;
        if (!allowedImages.exec($("#Thumbnail").val())) {
            alert("Invalid thumbnail file type. Allowed: jpg, jpeg, png."); return;
        }
        if (Thumbnail.size > 10 * 1024 * 1024) {
            alert("Thumbnail must be less than 10MB."); return;
        }

        // ==== VALIDATE MATERIAL BLOCKS ====
        let materialsValid = true;
        $('.material-block').each(function () {
            let file = $(this).find('input[type="file"]')[0].files[0];
            let title = $(this).find('input[type="text"]').val().trim();

            if (!file) {
                alert("Please upload course material in each block.");
                materialsValid = false; return false;
            }
            if (title === "") {
                alert("Please enter title for all course materials.");
                materialsValid = false; return false;
            }

            var allowExt = /(\.pdf|\.docx|\.pptx|\.mp4|\.jpg|\.png)$/i;
            if (!allowExt.exec(file.name)) {
                alert("Invalid material type. Allowed: pdf, docx, pptx, mp4, jpg, png.");
                materialsValid = false; return false;
            }
            if (file.size > (100 * 1024 * 1024)) {
                alert("Each material must be less than 100MB.");
                materialsValid = false; return false;
            }
        });
        if (!materialsValid) return;

        // ==== BUILD FORMDATA ====
        var formData = new FormData();
        formData.append("Title", Title);
        formData.append("Description", Description);
        formData.append("IsPaid", IsPaid);
        formData.append("Price", Price);
        formData.append("Thumbnail", Thumbnail);

        // append dynamic materials
        $('.material-block').each(function () {
            let file = $(this).find('input[type="file"]')[0].files[0];
            let title = $(this).find('input[type="text"]').val().trim();
            let type = $(this).find('select option:selected').val();

            formData.append("CourseMaterials", file);
            formData.append("MaterialTitles", title);
            formData.append("MaterialTypes", type);
        });

        $.ajax({
            url: '/Instructor/InsertCourse',
            type: 'POST',
            data: formData,
            processData: false,
            contentType: false,
            success: function (res) {
                if (res.status === 1) {
                    alert(res.message);
                    $('#courseForm')[0].reset();
                    $('#priceGroup').addClass('d-none');
                    $('#materialContainer').html(`
                        <div class="material-block">
                            <input type="file" name="CourseMaterials" required />
                            <input type="text" name="MaterialTitles" placeholder="Material Title" required />
                            <select name="MaterialTypes">
                                <option value="Video">Video</option>
                                <option value="PDF">PDF</option>
                            </select>
                        </div>`);
                }
                else if (res.status === -1) {
                    alert("Session expired. Please log in again.");
                    window.location.href = '/Account/Login';
                }
                else {
                    alert(res.message);
                }
            },
            error: function () {
                alert("Error while creating course.");
            }
        });

    });

    // Add more material-blocks
    $('#addMaterial').click(function () {
        var block = `
        <div class="material-block" style="margin-top:10px;">
            <input type="file" name="CourseMaterials" required /><br><br>
            <input type="text" name="MaterialTitles" placeholder="Material Title" required/><br><br>
            <select name="MaterialTypes">
               <option value="Video">Video</option>
               <option value="PDF">PDF</option>
            </select>
        </div>`;
        $("#materialContainer").append(block);
    });

});
