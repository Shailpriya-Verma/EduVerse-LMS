
    $(document).ready(function () {
        loadSubmissions();

    

    // Save grade and feedback
    $(document).on('click', '.saveBtn', function () {
            var id = $(this).data('id');
    var grade = $(`.grade-input[data-id='${id}']`).val();
    var feedback = $(`.feedback-input[data-id='${id}']`).val();

        $.ajax({
            url: '/Instructor/UpdateGradeFeedback',
            type: 'POST',
            data: {submissionId: id, grade: grade, feedback: feedback },
            success: function (res) {
                if (res.success) {
                    alert("Grade & Feedback Updated Successfully!");
                    loadSubmissions(); 
                }
                else {
                    alert("Error updating record!");
                }
            }
        });
    });
});

function loadSubmissions() {
    $.ajax({
        url: '/Instructor/GetSubmissionsByInstructor',
        type: 'GET',
        success: function (data) {
            var rows = '';
            if (data.length === 0) {
                rows = '<tr><td colspan="10" class="text-center">No submissions found.</td></tr>';
            } else {
                $.each(data, function (i, sub) {
                    rows += `
                                <tr>
                                    <td>${i + 1}</td>
                                    <td>${sub.StudentName}</td>
                                    <td>${sub.AssessmentTitle}</td>
                                    <td>${sub.CourseName}</td>
                                    <td>${sub.DueDate}</td>
                                    <td>${sub.SubmittedOn}</td>
                                    <td><a href="${sub.FilePath}" class="create-btn" target="_blank" style="text-decoration:none;">Download</a></td>
                                    <td><input type="text" class="grade-input" value="${sub.Grade || ''}" data-id="${sub.SubmissionId}" style="width:70px;" /></td>
                                    <td><textarea class="feedback-input" data-id="${sub.SubmissionId}" rows="1" style="width:120px;">${sub.Feedback || ''}</textarea></td>
                                    
                                    <td><button class="create-btn saveBtn" style="background:green;" data-id="${sub.SubmissionId}">Save</button></td>
                                </tr>`;
                });
            }
            $('#submissionBody').html(rows);
        }
    });
}