function getAllRole() {
    var booksUri = '/api/RoleMasters';
    $("#table_role > tbody").remove();
    var table = $('#table_role').append('<tbody/>'); //$(document.body)
    ajaxHelper(booksUri, 'GET').done(function (data) {
        $.each(data, function (rowIndex, r) {
            var row = $("<tr/>").appendTo(table);;
            row.append($("<td/>").text(r["Name"]));
            row.append($("<td/>").text(r["StatusCode"] == "ACT" ? "Activate" : "Deactivate"));
            row.append(EditPopUpActionListForRole('getRoleDetail', r["Id"], r["StatusCode"], '/RoleMasters/ScreenPermission/'));
        });
    });
    $(".office-table .table").trigger('footable_initialize');
}


function getRoleDetail(Id) {
    var Uri = '/api/RoleMasters/';
    if (Id != '' && Id != null && Id!='00000000-0000-0000-0000-000000000000') {
        ajaxHelper(Uri + Id, 'GET').done(function (data) {
            $('#Id').val(data["Id"])
            $('#Name').val(data["Name"])
            $('#Role_popup').html('Edit Role');
        });
    }
}

function fnSaveRole() {
    $('#popup_error').hide();
    $('#popup_error p').html('');

    $('#popup_success').hide();
    $('#popup_success p').html('');
    var req = {};
    var cansubmit = true;

    $('#Name').removeClass("error_msg");
    if ($.trim($('#Name').val()) == "") {
        $('#Name').addClass("error_msg");
        $('#Name').attr('title', 'Please enter Role');
        cansubmit = false;
    }
  
    if (cansubmit) {

        req.Id = $('#Id').val();
        req.Name = $('#Name').val();
        req.UserId = $('#UserId').val();

        var Uri = '/api/RoleMasters';
        ajaxHelper(Uri, 'POST', req).done(function (data, textStatus) {
            if (textStatus == 'notmodified') {
                $('#popup_error p').html('Role is already exist.');
                $('#popup_error').show();
            } else if (textStatus == 'notccceptable') {
                $('#popup_error p').html('Role has not saved.');
                $('#popup_error').show();
            } else {
                $('#Id').val(data)
                $('#popup_success').show();

                if (req.Id == data) {
                    $('#popup_success p').html('Role updated successfully.');
                } else {
                    $('#popup_success p').html('Role created successfully.');
                }
            }
            getAllRole();
        });
    }
}
function fnSaveStatus(Id, Status) {
    var statuscode = '';
    if (Status == "INA") {
        statuscode = "Deactivate";
    }
    else {

        statuscode = "Activate"
    }

    var conformResult = confirm("Are you sure you wish to " + statuscode + "  the specific Role \n and no longer have it available for the users to select ?");
    if (conformResult == true) {


        // alert(Id +","+ Status)
        var req = {};
        var cansubmit = true;

        if (cansubmit) {

            req.Id = Id;
            req.Name = $('#Name').val();
            req.StatusCode = Status;

            var reqUri = '/api/RoleMasters/';
            ajaxHelper(reqUri + Id, 'PUT', req).done(function (data) {
                getAllRole();
            });
        }
    }
    else {
        return false;
    }
}

function fnCancel() {
    $('#Id').val('00000000-0000-0000-0000-000000000000');
    $('input[type=text]')
        .removeClass('error_msg')
        .val('');
    $('#popup_success').hide();
    $('#popup_success p').html('');
    $('#popup_error').hide();
    $('#popup_error p').html('');
}