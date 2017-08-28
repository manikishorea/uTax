function getUsers() {

    var booksUri = '/api/UserMasters/';
    $("#table_user > tbody").remove();
    var table = $('#table_user').append('<tbody/>'); //$(document.body)

    ajaxHelper(booksUri, 'GET').done(function (data) {
        $.each(data, function (rowIndex, r) {
            var row = $("<tr/>").addClass('odd').attr('role', 'row');
            row.appendTo(table);
            row.append($("<td/>").text(r["FirstName"] + " " + r["LastName"]));
            row.append($("<td/>").text(r["EmailAddress"]));
            row.append($("<td/>").text(r["UserName"]));
            row.append($("<td/>").text(r["GroupName"]));
            row.append($("<td/>").text(r["AccessType"]));
            row.append($("<td/>").text(r["StatusCode"] == "ACT" ? "Activate" : "Deactivate"));
            row.append(ActionList('/user/create/', r["Id"], r["StatusCode"]));
        });
        $(".office-table .table").trigger('footable_initialize');
    });
  //  GridFunctionality();
}


function getUser(Id) {

    var userUri = '/api/UserMasters/';
    var divDetail = $('#div_bookdetail'); //$(document.body)

    if (Id != '' && Id != null && Id != '00000000-0000-0000-0000-000000000000') {
        ajaxHelper(userUri + Id, 'GET').done(function (data) {
            $('#FirstName').val(data["FirstName"])
            $('#LastName').val(data["LastName"])
            $('#txtUserName').val(data["UserName"])
            $('#EmailAddress').val(data["EmailAddress"])
            $('#IsEmailConfirmed').val(data["IsEmailConfirmed"])
            $('#EmailConfirmationCode').val(data["EmailConfirmationCode"])
            $('#PasswordResetCode').val(data["PasswordResetCode"])
            $('#LastLoginDate').val(data["LastLoginDate"])
            $('#EntityId').val(data["EntityId"])
            $('#CustomerId').val(data["CustomerId"])
            //  $('#GroupId').val(data["GroupId"])
            $('#CreatedBy').val($('#UserId').val())
            $('#LastUpdatedBy').val($('#UserId').val())
            // getUserRoleMap(Id);

            $('#GroupId').val(data.Groups["Id"])

            $.each(data.Roles, function (rowIndex, r) {
                $('#chk' + r["Id"]).attr('checked', 'checked');
            });

        });
    }
}


function fnSaveUser() {
    $('#error').hide();
    $('#error p').html('');
    $('#success').hide();
    $('#success p').html('');

    var User = {};
    var cansubmit = true;
    var email = $.trim($('#EmailAddress').val());
    $('*').removeClass("error_msg");
    if ($.trim($('#FirstName').val()) == "") {
        $('#FirstName').addClass("error_msg");
        $('#FirstName').attr('title', 'Please enter First Name');
        cansubmit = false;
    }

    if ($.trim($('#LastName').val()) == "") {
        $('#LastName').addClass("error_msg");
        $('#LastName').attr('title', 'Please enter Last Name');
        cansubmit = false;
    }

    if ($.trim($('#txtUserName').val()) == "") {
        $('#txtUserName').addClass("error_msg");
        $('#txtUserName').attr('title', 'Please enter User Name');
        cansubmit = false;
    }

    if (email == "") {
        $('#EmailAddress').addClass("error_msg");
        $('#EmailAddress').attr('title', 'Please enter password');
        cansubmit = false;
    }
    var IsEamil = ValidateEmail(email);
    if (IsEamil == false) {
        $('#EmailAddress').addClass("error_msg");
        $('#EmailAddress').attr('title', 'Please enter valid email');
        cansubmit = false;
    }

    var Groups = {};
    if ($.trim($('#GroupId :selected').val()) == "") {
        $('#GroupId').addClass("error_msg");
        $('#GroupId').attr('title', 'Please select Group');
        cansubmit = false;
    }
    else {
        Groups.Id = $('#GroupId').val();
    }

    var Roles = [];
    var IsRoleChecked = false;
    var allchkbox = $('input[type=checkbox].chkRole');
    $.each(allchkbox, function (indx, valu) {
        var chkId = $(valu).attr('id');
        var valR = $('#' + chkId).val()
        if ($(valu).is(':checked')) {
            IsRoleChecked = true;
            Roles.push({
                Id: valR
            });
        }
    });


    if (!IsRoleChecked) {
        $('#RoleIds').addClass("error_msg");
        $('#RoleIds').attr('title', 'Please select at least one role');
        cansubmit = false;
    }


    if (cansubmit) {

        User.Id = $('#Id').val();
        User.UserId = $('#UserId').val();
        User.EntityId = $('#EntityId').val();
        User.CustomerId = $('#CustomerId').val();
        User.FirstName = $('#FirstName').val();
        User.LastName = $('#LastName').val();
        User.UserName = $('#txtUserName').val();
        User.EmailAddress = $('#EmailAddress').val();
        User.IsEmailConfirmed = $('#IsEmailConfirmed').val();
        User.EmailConfirmationCode = $('#EmailConfirmationCode').val();
        User.PasswordResetCode = $('#PasswordResetCode').val();
        User.LastLoginDate = $('#LastLoginDate').val()
        // User.GroupId = $('#GroupId').val();
        User.Roles = Roles;
        User.Groups = Groups;

        var userUri = '/api/UserMasters/';
        ajaxHelper(userUri, 'POST', User).done(function (data, textStatus) {
            if (textStatus == 'notmodified') {
                $('#error p').html('User is already exist.');
                $('#error').show();
            } else if (textStatus == 'notccceptable') {
                $('#error p').html('User has not saved.');
                $('#error').show();
            } else {
                $('#Id').val(data)
                $('#success').show();
                if (User.Id == data) {
                    $('#success p').html('User updated successfully.');
                } else {
                    $('#success p').html('User created successfully.');
                }
            }
        });
    }
}



function fnSaveStatus(Id, Status) {
    $('#error').hide();
    $('#error p').html('');
    $('#success').hide();
    $('#success p').html('');
    var statuscode = '';
    if (Status == "INA") {
        statuscode = "Deactivate";
    }
    else {

        statuscode = "Activate"
    }
    var User = {};
    var cansubmit = true;
    var conformResult = confirm("Are you sure you wish to " + statuscode + " the specific User Master \n and no longer have it available for the users to select ?");
    if (conformResult == true) {
        if (cansubmit) {

            User.Id = Id;
            User.UserId = $('#UserId').val();
            User.StatusCode = Status;

            var userUri = '/api/UserMasters/';
            ajaxHelper(userUri + Id, 'PUT', User).done(function (data, textStatus) {
                if (textStatus == 'notmodified') {
                }
                else {
                    $('#success').show();
                    $('#success p').html('This User ' + statuscode + 'd successfully.');
                    getUsers();
                }               
            });
        }
    }
    else {
        return false;
    }
}