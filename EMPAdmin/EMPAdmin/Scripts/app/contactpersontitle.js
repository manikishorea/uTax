function getAllContactPersonTitle() {

    var booksUri = '/api/ContactPersonTitleMaster/';
    $("#table_contactpersontitle > tbody").remove();
    var table = $('#table_contactpersontitle').append('<tbody/>'); //$(document.body)
    ajaxHelper(booksUri, 'GET').done(function (data) {
        $.each(data, function (rowIndex, r) {
            var row = $("<tr/>").appendTo(table);;
            //  $.each(r, function (colIndex, c) {
            row.append($("<td/>").text(r["ContactPersonTitle"]));
            row.append($("<td/>").text(r["Description"]));
            row.append($("<td/>").text(r["StatusCode"] == "ACT" ? "Activate" : "Deactivate"));
            var jsonDate = r["ActivatedDate"];
            var currentTime = new Date(jsonDate);
            var month = currentTime.getMonth() + 1;
            var day = currentTime.getDate();
            var year = currentTime.getFullYear();
            var date = day + "/" + month + "/" + year;
            row.append(EditPopUpActionList('getContactPersonTitleDetail', r["Id"], r["StatusCode"]));
        });
    });
    CallGridFunctionality();
    $(".office-table .table").trigger('footable_initialize');
}


function getContactPersonTitleDetail(Id) {
    $('#popup_error').hide();
    $('#popup_error p').html('');

    $('#popup_success').hide();
    $('#popup_success p').html('');
    var Uri = '/api/ContactPersonTitleMaster/';

    if (Id != '' && Id != null && Id != '00000000-0000-0000-0000-000000000000') {
        ajaxHelper(Uri + Id, 'GET').done(function (data) {
            $('#Id').val(data["Id"])
            $('#ContactPersonTitle').val(data["ContactPersonTitle"])
            $('#Description').val(data["Description"])
            $('.modal-header .panel-heading .panel-title').html('Edit Contact Person Title');
        });
    }
}

function fnSaveContactPersonTitle() {
    $('#popup_error').hide();
    $('#popup_error p').html('');

    $('#popup_success').hide();
    $('#popup_success p').html('');
    var req = {};
    var cansubmit = true;

    $('#ContactPersonTitle').removeClass("error_msg");
    if ($.trim($('#ContactPersonTitle').val()) == "") {
        $('#ContactPersonTitle').addClass("error_msg");
        $('#ContactPersonTitle').attr('title', 'Please enter Contact Person Title');
        cansubmit = false;
    }
    if (cansubmit) {

        req.Id = $('#Id').val();
        req.ContactPersonTitle = $('#ContactPersonTitle').val();
        req.Description = $('#Description').val();
        req.UserId = $('#UserId').val();

        var Uri = '/api/ContactPersonTitleMaster/';
        ajaxHelper(Uri, 'POST', req).done(function (data, textStatus) {
            if (textStatus == 'notmodified') {
                $('#popup_error p').html('Contact Person is already exist.');
                $('#popup_error').show();
            } else if (textStatus == 'notccceptable') {
                $('#popup_error p').html('Contact Person has not saved.');
                $('#popup_error').show();
            } else {
                $('#Id').val(data)
                $('#popup_success').show();

                if (req.Id == data) {
                    $('#popup_success p').html('Contact Person updated successfully.');
                } else {
                    $('#popup_success p').html('Contact Person created successfully.');
                }
                getAllContactPersonTitle();
            }
           // window.location.href = "/contactpersontitle/";
        });
    }
}
function fnSaveStatus(Id, Status) {
    $('#popup_error').hide();
    $('#popup_error p').html('');

    $('#popup_success').hide();
    $('#popup_success p').html('');
    var statuscode = '';
    if (Status == "INA") {
        statuscode = "Deactivate";
    }
    else {

        statuscode = "Activate"
    }
    var conformResult = confirm("Are you sure you wish to " + statuscode + " the specific Contact Person Title \n and no longer have it available for the users to select ?");
    if (conformResult == true) {

        var req = {};
        var cansubmit = true;

        if (cansubmit) {

            req.Id = Id;
            req.ContactPersonTitle = $('#ContactPersonTitle').val();
            req.StatusCode = Status;

            var reqUri = '/api/ContactPersonTitleMaster/';
            ajaxHelper(reqUri + Id, 'PUT', req).done(function (data, textStatus) {
                if (textStatus == 'notmodified') {
                } else
                {
                    $('#popup_success').show();
                    $('#popup_success p').html('This Contact Person ' + statuscode + 'd successfully.');
                    getAllContactPersonTitle();
                }                
            });
        }
    }
    else {
        return false;
    }
}
function fnCancle() {
    $('input[type=text]')
        .removeClass('error_msg')
        .val('');
    $('#Id').val('00000000-0000-0000-0000-000000000000');
    $('#Description').val('');
    $('#popup_success').hide();
    $('#popup_success p').html('');
    $('#popup_error').hide();
    $('#popup_error p').html('');

}