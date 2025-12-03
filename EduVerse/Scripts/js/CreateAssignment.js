
    document.addEventListener("DOMContentLoaded", function () {

        // Open Modal on "Create Assignment" button click
        document.querySelectorAll(".create-btn").forEach(btn => {
            btn.addEventListener("click", function () {
                const row = this.closest("tr");

                // Get CourseId from data attribute (recommended)
                const courseId = row.getAttribute("data-courseid") || row.cells[0].innerText;
                document.getElementById("CourseId").value = courseId;

                // Show modal
                document.getElementById("assignmentModal").style.display = "block";
            });
        });

    // Cancel button → Close + Clear Form
    const cancelBtn = document.getElementById("cancelModal");
    if (cancelBtn) {
        cancelBtn.addEventListener("click", function () {
            const modal = document.getElementById("assignmentModal");
            const form = document.getElementById("assignmentForm");

            form.reset(); // clear all fields
            modal.style.display = "none"; // hide modal
        });
    }

    // Disable closing modal on outside click
    window.onclick = function (event) {
        const modal = document.getElementById("assignmentModal");
    const modalContent = document.querySelector(".modal-content");
    if (modal.style.display === "block" && !modalContent.contains(event.target)) {
        // do nothing (prevents accidental closing)
    }
    };

    // Save button click (AJAX call)
    const saveBtn = document.getElementById("saveAssignment");
    if (saveBtn) {
        saveBtn.addEventListener("click", function () {
            let form = document.getElementById("assignmentForm");

            // ====== Client-side validation ======
            let title = document.getElementById("Title").value.trim();
            let desc = document.getElementById("Description").value.trim();
            let dueDate = document.getElementById("DueDate").value;
            let fileInput = document.getElementById("AssignmentFile");

            if (!title) {
                alert("Please enter assignment title.");
                return;
            }
            if (!desc) {
                alert("Please enter assignment description.");
                return;
            }
            if (!dueDate) {
                alert("Please select a due date.");
                return;
            }
            if (!(fileInput.files.length > 0)) {
                alert("Please select a file");
                return;
            }
            // Validate file (optional)
            if (fileInput.files.length > 0) {
                let file = fileInput.files[0];
                let validTypes = ["image/jpeg", "image/jpg", "image/png", "application/pdf"];
                if (!validTypes.includes(file.type)) {
                    alert("Only JPG, PNG, or PDF files are allowed.");
                    return;
                }
            }

            // ====== AJAX Submit ======

            let formData = new FormData(form);

            $.ajax({
                url: '/Instructor/SaveAssignment',
                type: 'POST',
                data: formData,
                processData: false,
                contentType: false,
                success: function (response) {
                    if (response.success) {
                        alert('Assignment created successfully!');
                        form.reset();
                        document.getElementById("assignmentModal").style.display = "none";
                    } else {
                        alert('Unable to save assignment. ' + (response.message || 'Please check your inputs.'));
                    }
                },
                error: function () {
                    alert('Error saving assignment! Please try again.');
                }
            });
        });
    }

});
