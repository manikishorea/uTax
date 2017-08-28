function getAllPhoneType() {
    var booksUri = '/api/PhoneTypeMaster/';
    $("#table_phonetype > tbody").remove();
    var table = $('#table_phonetype').append('<tbody/>'); //$(document.body)
    ajaxHelper(booksUri, 'GET').done(function (data) {
        $.each(data, function (rowIndex, r) {
            var row = $("<tr/>").appendTo(table);;
            row.append($("<td/>").text(r["PhoneType"]));
            row.append($("<td/>").text(r["Description"]));
            row.append($("<td/>").text(r["StatusCode"] == "ACT" ? "Activate" : "Deactivate"));
            var jsonDate = r["ActivatedDate"];
            var currentTime = new Date(jsonDate);
            var month = currentTime.getMonth() + 1;
            var day = currentTime.getDate();
            var year = currentTime.getFullYear();
            var date = day + "/" + month + "/" + year;
            // row.append($("<td/>").text(date));
            row.append(EditPopUpActionList('getPhoneTypeDetail', r["Id"], r["StatusCode"]));
        });
    });
    CallGridFunctionality();
    $(".office-table .table").trigger('footable_initialize');
}


function getPhoneTypeDetail(Id) {

    var Uri = '/api/PhoneTypeMaster/';
    $('*').removeClass("error_msg");
    if (Id != '' && Id != null && Id!='00000000-0000-0000-0000-000000000000') {
        ajaxHelper(Uri + Id, 'GET').done(function (data) {
            $('#Id').val(data["Id"])
            $('#PhoneType').val(data["PhoneType"])
            $('#Description').val(data["Description"])
            //debugger;
            $('#Phonetype_popup').html('<h2 class="panel-title">Edit Phone Type</h2>');
        });
    }
}

function fnSavePhoneType() {
    $('#popup_error').hide();
    $('#popup_error p').html('');

    $('#popup_success').hide();
    $('#popup_success p').html('');
    var req = {};
    var cansubmit = true;

    $('#PhoneType').removeClass("error_msg");
    if ($.trim($('#PhoneType').val()) == "") {
        $('#PhoneType').addClass("error_msg");
        $('#PhoneType').attr('title', 'Please enter Phone Type');
        cansubmit = false;
    }
  
    if (cansubmit) {

        req.Id = $('#Id').val();
        req.PhoneType = $('#PhoneType').val();
        req.Description = $('#Description').val();
        req.UserId = $('#UserId').val();

        var Uri = '/api/PhoneTypeMaster/';
        ajaxHelper(Uri, 'POST', req).done(function (data, textStatus) {
            if (textStatus == 'notmodified') {
                $('#popup_error p').html('Phone Type is already exist.');
                $('#popup_error').show();
            } else if (textStatus == 'notccceptable') {
                $('#popup_error p').html('Phone Type has not saved.');
                $('#popup_error').show();
            } else {
                $('#Id').val(data)
                $('#popup_success').show();
                if (req.Id == data) {
                    $('#popup_success p').html('Phone Type updated successfully.');
                } else {
                    $('#popup_success p').html('Phone Type created successfully.');
                }
                getAllPhoneType();
            }
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

    var conformResult = confirm("Are you sure you wish to " + statuscode + "  the specific Phone Type \n and no longer have it available for the users to select ?");
    if (conformResult == true) {


        // alert(Id +","+ Status)
        var req = {};
        var cansubmit = true;

        if (cansubmit) {

            req.Id = Id;
            req.PhoneType = $('#PhoneType').val();
            req.StatusCode = Status;

            var reqUri = '/api/PhoneTypeMaster/';
            ajaxHelper(reqUri + Id, 'PUT', req).done(function (data, textStatus) {
                if (textStatus == 'notmodified') {

                } else
                {
                    $('#popup_success').show();
                    $('#popup_success p').html('This Contact Person ' + statuscode + 'd successfully.');
                    getAllPhoneType();
                }
            });
        }
    }
    else {
        return false;
    }
}

function fnCancle() {
    $('#Id').val('00000000-0000-0000-0000-000000000000');
    $('input[type=text]')
        .removeClass('error_msg')
        .val('');
    $('#Description').val('');
    $('#popup_success').hide();
    $('#popup_success p').html('');
    $('#popup_error').hide();
    $('#popup_error p').html('');

}