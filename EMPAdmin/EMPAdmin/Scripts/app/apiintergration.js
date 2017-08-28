function getAllapiintergration() {
    var booksUri = '/api/APIIntegrationMaster/';
    $("#table_APIIntegration > tbody").remove();
    var table = $('#table_APIIntegration').append('<tbody/>'); //$(document.body)
    ajaxHelper(booksUri, 'GET').done(function (data) {
        $.each(data, function (rowIndex, r) {
            var row = $("<tr/>").appendTo(table);;
            //  $.each(r, function (colIndex, c) {
            row.append($("<td/>").text(r["Name"]));
            row.append($("<td/>").text(r["URL"]));
            row.append($("<td/>").text(r["UserName"]));
            row.append($("<td/>").text(r["StatusCode"] == "ACT" ? "Activate" : "Deactivate"));
            row.append(EditActionList('/APIIntegration/create/', r["Id"]));
        });
    });
    GridFunctionality();
}


function getAPIIntegrationDetail(Id) {

    var Uri = '/api/APIIntegrationMaster/';

    if (Id != '' && Id != null && Id != '00000000-0000-0000-0000-000000000000') {
        ajaxHelper(Uri + Id, 'GET').done(function (data) {
            $('#Name').val(data["Name"])
            $('#URL').val(data["URL"])
            $('#UserName').val(data["UserName"])
            $('#Password').val(data["Password"])
        });
    }
}

function fnSaveAPIIntegration() {
    $('#success').hide();
    $('#success p').html('');

    var req = {};
    var cansubmit = true;


    $('#URL').removeClass("error_msg");
    if ($.trim($('#URL').val()) == "") {  
            $('#URL').addClass("error_msg");
            $('#URL').attr('title', 'Please enter API Access URL');
        cansubmit = false;
    }

    $('#URL').removeClass("error_msg"); 
    var urlvalue=$.trim($('#URL').val());
    var urlvalidation = /^(http[s]?:\/\/){0,1}(www\.){0,1}[a-zA-Z0-9\.\-]+\.[a-zA-Z]{2,5}[\.]{0,1}/;
    if (!urlvalidation.test(urlvalue)) {
        $('#URL').addClass("error_msg");
        $('#URL').attr('title', 'Please enter API Access URL'); 
        cansubmit = false;
    }

    $('#UserName').removeClass("error_msg");
    if ($.trim($('#UserName').val()) == "") {
        $('#UserName').addClass("error_msg");
        $('#UserName').attr('title', 'Please enter Access User Name');
        cansubmit = false;
    }

    $('#Password').removeClass("error_msg");
    if ($.trim($('#Password').val()) == "") {
        $('#Password').addClass("error_msg");
        $('#Password').attr('title', 'Please enter Access Password');
        cansubmit = false;
    }

    if (cansubmit) {
         
        req.Id = $('#Id').val();
        req.Name = $('#Name').val();
        req.URL = $('#URL').val();
        req.UserName = $('#UserName').val();
        req.Password = $('#Password').val();
        req.UserId = $('#UserId').val();

        var Uri = '/api/APIIntegrationMaster/';
        ajaxHelper(Uri, 'POST', req).done(function (data) {
            $('#Id').val(data["Id"])

            $('#success').show();

            if (req.Id == data["Id"]) {
                $('#success p').html('API Integration updated successfully.');
            } else {
                $('#success p').html('API Integration created successfully.');
            }
        });
    }
}
