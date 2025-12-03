$(document).ready(function () {

    document.addEventListener("keydown", function (e) {
        if (e.key === "Enter") {
            e.preventDefault();
            document.getElementById("btn").click();
        }
    });

    //const apiConnection = getAPIConnection();

    $("#btn").click(function () {
        var email = $("#email").val().trim();
        var pass = $("#password").val().trim();
        var roleid = $("#roleid").val().trim();

        if (!roleid) {
            alert("Please Select Any one !!");
            return;
        }
        if (!email) {
            alert("Please Enter Email !!");
            return;
        }
        let emailPattern = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
        if (!emailPattern.test(email)) {
            alert("Invalid email format!");
            return;
        }
        if (!pass) {
            alert("Please Enter Password !!");
            return;
        }
        if (pass.length < 6) {
            alert("Password must be at least 6 characters!");
            return;
        }

        $.ajax({
            url:'/Account/UserLogin',
            type: 'POST',
            dataType: 'json',
            data: {
                email: email,
                password: pass,
                roleId: roleid
            },


            success: function (res) {
                console.log(res);
                if (res.status === 1) {
                    Swal.fire("Login Successful", res.message, "success").then(() => {
                        
                        //localStorage.setItem("fullName", res.fullName);

                        // Redirect based on role
                        if (roleid === "2" || roleid === "3") {
                            window.location.href = "/Student/StudentDashboard";
                        } else {
                            Swal.fire("Error", "Unknown role!", "error");
                        }
                    });
                } else {
                    Swal.fire("Login Failed", res.message, "error");
                }
            },
            error: function () {
                Swal.fire("Login Failed", "An error occurred. Please try again.", "error");
            }
        });


    });

});
