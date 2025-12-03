$(document).ready(function () {

    getAssignmentByStudentId();


    // Open modal on "Submit" button click
    $(document).on("click", ".create-btn", function () {
        if ($(this).text().trim() === "Submit") {
           
            const assessmentId = $(this).closest("tr").data("assesmentid");
            $("#AssessmentId").val(assessmentId);
            $("#assignmentModal").fadeIn(200);
        }
    });

    // Cancel button → Close + Clear form
    $("#cancelModal").click(function () {
        $("#assignmentForm")[0].reset();
        $("#assignmentModal").fadeOut(200);
    });

    // Prevent closing modal when clicking outside
    $("#assignmentModal").on("click", function (e) {
        if (!$(e.target).closest(".modal-content").length) {
            e.stopPropagation(); // Just prevent accidental close
        }
    });



    // ===== Save Assignment (AJAX) =====
    $("#saveAssignment").on("click", function () {
        const form = $("#assignmentForm")[0];
        const fileInput = $("#AssignmentFile")[0];
        const formData = new FormData(form);

        // ----- Client-side Validation -----
        if (fileInput.files.length === 0) {
            alert("Please upload a file before submitting.");
            return;
        }

        const file = fileInput.files[0];
        const validTypes = ["image/jpeg", "image/jpg", "image/png", "application/pdf"];

        if (!validTypes.includes(file.type)) {
            alert("Only JPG, PNG, or PDF files are allowed.");
            return;
        }

        // ===== AJAX Request =====
        $.ajax({
            url: '/Student/SubmitAssignment',  
            type: 'POST',
            dataType: 'JSON',
            data: formData,
            processData: false,
            contentType: false,
            success: function (response) {
                if (response.success) {
                    alert("Assignment submitted successfully!");
                    $("#assignmentForm")[0].reset();
                    $("#assignmentModal").fadeOut(200);
                    location.reload();

                } else {
                    alert("Failed to submit assignment. " + (response.message || ""));
                }
            },
            error: function () {
                alert("Error submitting assignment. Please try again.");
            }
        });
    });

});


// ===== Load assignments dynamically =====
function getAssignmentByStudentId() {
    $.ajax({
        url: '/Student/GetAssignmentByStudentId',
        type: 'GET',
        dataType: 'JSON',
        success: function (response) {
            if (response.success && response.data.length > 0) {
                let rows = "";
                $.each(response.data, function (index, item) {
                    let actionBtn = "";

                    if (item.IsSubmitted) {
                        // Already submitted → disable button
                        actionBtn = `<button class="disabled-btn" style="background-color:gray;" disabled>Submitted</button>`;
                    } else {
                        // Not submitted yet
                        actionBtn = `<button class="create-btn" style="background-color:green;">Submit</button>`;
                    }

                    rows += `
                        <tr data-assesmentid="${item.AssessmentId}">
                            <td class="border">${index + 1}</td>
                            <td class="border">${item.Title}</td>
                            <td class="border">${item.Description}</td>
                            <td class="border">${item.DueDate}</td>
                            <td class="border">${item.CreatedAt}</td>
                            <td class="border">
                                <a href="${item.FilePath}" download>
                                    <button class="create-btn">Download</button>
                                </a>
                            </td>
                            <td class="border">${actionBtn}</td>
                            <td class="border">${item.Grade || ""}</td>
                            <td class="border">${item.Feedback || ""}</td>
                        </tr>`;
                });
                $("#assignmentBody").html(rows);
            } else {
                $("#assignmentBody").html("<tr><td colspan='9' class='text-center'>No assignments found.</td></tr>");
            }
        },
        error: function () {
            alert("Error!! Please try again.");
        }
    });
}
