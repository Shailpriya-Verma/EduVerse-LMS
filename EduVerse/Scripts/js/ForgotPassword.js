
    $(document).ready(function () {

        $('#btnSendOtp').click(function () {
            var email = $('#forgotEmail').val();
            var roleId = $("#roleId").val();
            if (!roleId) {
                alert("Please Select Any one !!");
                return;
            }
            if (email === '') {
                Swal.fire('Error', 'Please enter your email.', 'error');
                return;
            }
            var emailPattern = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
            if (!emailPattern.test(email)) {
                Swal.fire('Error', 'Invalid Email Format', 'error');
                return;
            }
            $.ajax({
                url: '/Account/SendOtp',
                type: 'POST',
                data: { email: email },
                success: function (response) {
                    if (response.success) {
                        Swal.fire('Success', 'OTP sent to your email.', 'success');
                        $('#forgotPasswordSection').hide();
                        $('#otpSection').show();
                    } else {
                        Swal.fire('Error', response.message || 'Something went wrong!', 'error');
                    }
                },
                error: function () {
                    Swal.fire('Error', 'Server error. Please try again later.', 'error');
                }
            });
        });


    $('#btnVerifyOtp').click(function () {
                var otp = $('#otp').val();
    if (otp === '') {
        Swal.fire('Error', 'Please enter the OTP.', 'error');
    return;
                }
    $.ajax({
        url: '/Account/VerifyOtp',
    type: 'POST',
    data: {otp: otp },
    success: function (response) {
                        if (response.success) {
        Swal.fire('Success', 'OTP verified.', 'success');
    $('#otpSection').hide();
    $('#resetSection').show();
                        } else {
        Swal.fire('Error', response.message || 'OTP verification failed.', 'error');
                        }
                    },
    error: function () {
        Swal.fire('Error', 'Server error. Please try again later.', 'error');
                    }
                });
            });

    $('#resendOtp').click(function () {
                var email = $('#forgotEmail').val();

    if (email === '') {
        Swal.fire('Error', 'Please enter your email.', 'error');
    return;
                }
    let emailPattern = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    if (!emailPattern.test(email)) {
        alert("Invalid email format!");
    return;
                }
    $.ajax({
        url: '/Account/SendOtp', // Same endpoint as the original
    type: 'POST',
    data: {email: email },
    success: function (response) {
                        if (response.success) {
        Swal.fire('Success', 'OTP resent to your email.', 'success');
                        } else {
        Swal.fire('Error', response.message || 'Failed to resend OTP.', 'error');
                        }
                    },
    error: function () {
        Swal.fire('Error', 'An error occurred while resending the OTP.', 'error');
                    }
                });
            });



    $('#btnResetPassword').click(function () {
                var newPassword = $('#newPassword').val();
    var confirmPassword = $('#confirmPassword').val();
    var email = $('#forgotEmail').val();
    var roleId = $("#roleId").val();

    if (newPassword.length < 6) {
    alert("Password must be at least 6 characters!");
    return;
    }

    if (newPassword === '' || confirmPassword === '') {
        Swal.fire('Error', 'Please fill in both password fields.', 'error');
    return;
                }

    if (newPassword !== confirmPassword) {
        Swal.fire('Error', 'Password and Confirm Password Must Be Match', 'error');
    return;
                }

    $.ajax({
        url: '/Account/ResetPassword',
    type: 'POST',
    data: {
        email: email,
    newPassword: newPassword,
    confirmPassword: confirmPassword,
    roleId: roleId
                    },
    success: function (response) {
                        if (response.success) {
        Swal.fire('Success', 'Password has been reset.', 'success')
            .then(() => {
                window.location.href = '/Account/Login';
            });
                        } else {
        Swal.fire('Error', response.message || 'Failed to reset password.', 'error');
                        }
                    },
    error: function () {
        Swal.fire('Error', 'An error occurred while resetting the password.', 'error');
                    }
                });
            });


        });