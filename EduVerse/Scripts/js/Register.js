
//< !--Google Login Design Start-- >

////Code For Gmail Button
//function onSuccess(googleUser) {
//    console.log('Logged in as: ' + googleUser.getBasicProfile().getName());
//}
//function onFailure(error) {
//    console.error("Google Sign-In Failed:", error);
//}
//function renderButton() {
//    console.log("renderButton called"); // Debug log
//    gapi.signin2.render('my-signin2', {
//        'scope': 'profile email',
//        'width': 240,
//        'height': 50,
//        'longtitle': true,
//        'theme': 'dark',
//        'onsuccess': onSuccess,
//        'onfailure': onFailure
//    });
//}


// code to send data to register

$(document).ready(function () {


    //document.addEventListener("keydown", function (e) {
    //    if (e.key === "Enter") {
    //        e.preventDefault();
    //        document.getElementById("btn").click();
    //    }
    //});


    const sendOtpUrl = "/Account/SendOtp";     // Update with correct controller path
    const verifyOtpUrl = "/Account/VerifyOtp"; // Update with correct controller path

    // Send OTP
    $("#sendOtpBtn").click(function () {
        var email = $("#email").val();
        if (!email) {
            Swal.fire("Error", "Please enter your email!", "warning");
            return;
        }
        let emailPattern = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
        if (!emailPattern.test(email)) {
            alert("Invalid email format!");
            return;
        }
        $.ajax({
            type: "POST",
            url: sendOtpUrl,
            data: { email: email },
            success: function (response) {
                if (response.success) {
                    Swal.fire("Success", "OTP sent to your email!", "success");
                    $("#otpSection").show();
                    $("#sendOtpBtn").hide();
                } else {
                    Swal.fire("Error", "Failed to send OTP!", "error");
                }
            },
            error: function () {
                Swal.fire("Error", "Something went wrong while sending OTP!", "error");
            }
        });
    });

    // Verify OTP
    $("#verifyOtpBtn").click(function () {
        var otp = $("#otp").val();
        if (!otp) {
            Swal.fire("Error", "Please enter OTP!", "warning");
            return;
        }

        $.ajax({
            type: "POST",
            url: verifyOtpUrl,
            data: { otp: otp },
            success: function (response) {
                if (response.success) {
                    Swal.fire("Verified", "OTP verified successfully!", "success");
                    $("#otpSection").hide();
                    $("#passwordSection").show();
                } else {
                    Swal.fire("Error", response.message, "error");
                }
            },
            error: function () {
                Swal.fire("Error", "Something went wrong while verifying OTP!", "error");
            }
        });
    });




    const apiConnection = getAPIConnection();

    $("#btn").click(function () {
        var roleId = $("#roleId").val();
        var name = $("#name").val();
        var email = $("#email").val();
        var pass = $("#password").val();
        var cpass = $("#cPassword").val();

        if (!roleId) {
            alert("Please Select Any one !!");
            return;
        }
        if (name == "") {
            alert("Please Enter Your Full Name !!");
            return;
        }
        if (!/^[a-zA-Z\s]+$/.test(name)) {
            alert("Name should contain only letters and spaces!");
            return;
        }

        if (email == "") {
            alert("Please Enter Email !!");
            return;
        }
        let emailPattern = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
        if (!emailPattern.test(email)) {
            alert("Invalid email format!");
            return;
        }

        if (pass == "") {
            alert("Please Enter Password !!");
            return;
        }

        if (pass.length < 6) {
            alert("Password must be at least 6 characters!");
            return;
        }

        if (cpass == "") {
            alert("Please Enter Confirm Password !!");
            return;
        }
        if (pass != cpass) {
            alert("Password And Confirm Password must Be Same !! ");
            return;
        }
        

        $.ajax({
            url: apiConnection + 'api/AccountApi/Register',
            type: 'POST',
            dataType: 'json',
            contentType: 'application/json',
            
            data: JSON.stringify({
                roleId: roleId,
                name: name,
                email: email,
                password: pass,
                googleId: null,
                profilePic: null
            }),
            success: function (res) {
                if (res.status === 1) {
                    Swal.fire({
                        title: 'Goto Login',
                        text: res.message,
                        icon: 'success',
                        showCancelButton: true,
                        confirmButtonText: 'OK',
                        cancelButtonText: 'Cancel'
                    }).then((result) => {
                        if (result.isConfirmed) {
                            location.href = 'Login';
                        }
                    });
                } else {
                    alert("Something Went Wrong!! ", res.message);
                }
                
            },
            error: function (xhr) {


                // Try to extract custom message from server response
                if (xhr.responseJSON && xhr.responseJSON.message) {
                    msg = xhr.responseJSON.message;
                }

                if (xhr.status === 409) {
                    Swal.fire("Already Registered", msg, "warning");
                }
                else if (xhr.status === 400) {
                    Swal.fire("Invalid Data", msg, "warning");
                }
                else if (xhr.status === 500) {
                    Swal.fire("Registration Failed", msg, "error");
                }
                else {
                    Swal.fire("Error", msg, "error");
                }
                
            }
        });
    });

});

