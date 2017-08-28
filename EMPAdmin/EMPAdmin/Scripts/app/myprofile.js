function getMyProfile(Id) {

    var userUri = '/api/UserMasters/';
    var divDetail = $('#div_bookdetail'); //$(document.body)

    if (Id != '' && Id != null && Id != '00000000-0000-0000-0000-000000000000') {
        ajaxHelper(userUri + Id, 'GET').done(function (data) {
            $('#FirstName').val(data["FirstName"])
            $('#LastName').val(data["LastName"])
            $('#UserName').html(data["UserName"])
            $('#EmailAddress').val(data["EmailAddress"])
            $('#IsEmailConfirmed').val(data["IsEmailConfirmed"])
            $('#EmailConfirmationCode').val(data["EmailConfirmationCode"])
            $('#PasswordResetCode').val(data["PasswordResetCode"])
            $('#LastLoginDate').val(data["LastLoginDate"])
            $('#EntityId').val(data["EntityId"])
            $('#CustomerId').val(data["CustomerId"])

            $('#GroupId').val(data.Groups["Id"])
            $('#GroupName').html(data.Groups["Name"])

            var rolename = '';
            $.each(data.Roles, function (colIndex, c) {
                rolename = rolename + c["Name"] + ', ';
            });

            if (rolename.length > 2) {
                rolename = rolename.substring(0, rolename.length - 2);
            }

            $("#Roles").html(rolename);

            // getUserRoleMap(Id);
        });
    }
}


function fnSaveMyProfile() {
    $('#success').hide();
    $('#success p').html('');
    var User = {};
    var cansubmit = true;
    var email = $.trim($('#EmailAddress').val());
    $('#FirstName').removeClass("error_msg");
    if ($.trim($('#FirstName').val()) == "") {
        $('#FirstName').addClass("error_msg");
        $('#FirstName').attr('title', 'Please enter Category Name');
        cansubmit = false;
    }

    $('#LastName').removeClass("error_msg");
    if ($.trim($('#LastName').val()) == "") {
        $('#LastName').addClass("error_msg");
        $('#LastName').attr('title', 'Please enter Last Name');
        cansubmit = false;
    }

    $('#EmailAddress').removeClass("error_msg");
    if (email == "") {
        $('#EmailAddress').addClass("error_msg");
        $('#EmailAddress').attr('title', 'Please enter Password');
        cansubmit = false;
    }

    if (!ValidateEmail(email)) {
        $('#EmailAddress').addClass("error_msg");
        $('#EmailAddress').attr('title', 'Please enter valid email');
        cansubmit = false;
    }

    if (cansubmit) {

        User.Id = $('#Id').val();
        User.UserId = $('#UserId').val();
        User.FirstName = $('#FirstName').val();
        User.LastName = $('#LastName').val();
        User.EmailAddress = $('#EmailAddress').val();
        User.StatusCode = 'MY_PROFILE';

        var userUri = '/api/UserMasters/';
        ajaxHelper(userUri + User.Id, 'PUT', User).done(function (data) {
            $('#success').show();
            $('#success p').html('My Profile updated successfully.');

        });
    }
}

function getScreenPermissions() {

    var url = '/api/PermissionMasters?';
    var userId = $("#UserId").val();
    var roleId = $("#UserRoleId").val();

    $("#AddLink").hide();
    $("table#table_user .EditLink").hide();
    $("table#table_user .ActiveInactiveLink").hide();

    var IsAllFalse = false;
    ajaxHelper(url + "userId=" + userId + "&roleId=" + roleId, 'GET').done(function (data) {
        $.each(data, function (rowIndex, r) {

            if (r["Name"] == "Add") {
                $("#AddLink").show();
            }
            if (r["Name"] == "Edit") {
                $("li.EditLink").show();
                IsAllFalse = true;
            }
            if (r["Name"] == "Deactivate/Activate") {
                $("li.ActiveInactiveLink").show();
                IsAllFalse = true;
            }
        });
    });

    if (!IsAllFalse) {
        $('.btn-group').hide();
    }
}