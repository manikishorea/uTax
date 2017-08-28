function getAllSecurityQuestion() {
    var booksUri = '/api/SecurityQuestionMaster/';
    var table = $('#table_securityquestion');
    ajaxHelper(booksUri, 'GET').done(function (data) {
        $.each(data, function (rowIndex, r) {
            var row = $("<tr/>").appendTo(table);;
            row.append($("<td/>").text(r["Question"]));
            row.append($("<td/>").text(r["Description"]));
            row.append($("<td/>").text(r["StatusCode"]));
            // row.append($("<td/>").html("<a href='/securityquestion/create/" + r['Id'] + "'> Edit </a>"));
            if (r["StatusCode"] == 'INA') {
                Status = 'ACT';
                StatusCode = 'Active';
            } else if (r["StatusCode"] == 'ACT') {
                Status = 'INA';
                StatusCode = 'InActive';
            }
            row.append('<td><a href="/securityquestion/create/' + r["Id"] + '"> Edit</a> | <a href="javascript:void(0);"  onclick="fnSaveStatus(' + "'" + r["Id"] + "'" + ',' + "'" + Status + "'" + ') "> ' + StatusCode + ' </a></td>');
        });
    });
}


function getSecurityQuestionDetail(Id) {

    var Uri = '/api/SecurityQuestionMaster/';

    if (Id != '' && Id != null && Id != '00000000-0000-0000-0000-000000000000') {
        ajaxHelper(Uri + Id, 'GET').done(function (data) {
            $('#Question').val(data["Question"])
            $('#Description').val(data["Description"])
        });
    }
}

function fnSaveSecurityQuestion() {
    $('#success').hide();
    $('#success p').html('');
    var req = {};
    var cansubmit = true;

    $('#Question').removeClass("error_msg");
    if ($.trim($('#Question').val()) == "") {
        $('#Question').addClass("error_msg");
        $('#Question').attr('title', 'Please enter Security Question');
        cansubmit = false;
    }

    if (cansubmit) {

        req.Id = $('#Id').val();
        req.Question = $('#Question').val();
        req.Description = $('#Description').val();
        req.UserId = $('#UserId').val();

        var Uri = '/api/SecurityQuestionMaster/';
        ajaxHelper(Uri, 'POST', req).done(function (data) {
            $('#Id').val(data["Id"])
            $('#success').show();

            if (req.Id == data["Id"]) {
                $('#success p').html('Security Question updated successfully.');
            } else {
                $('#success p').html('Security Question created successfully.');
            }
        });
    }
}

function fnSaveStatus(Id, Status) {
    // alert(Id +","+ Status)
    var statuscode = '';
    if (Status == "INA") {
        statuscode = "Deactivate";
    }
    else {

        statuscode = "Activate"
    }
    var req = {};
    var cansubmit = true;
    var conformResult = confirm("Are you sure you wish to " + statuscode + " the specific Sequrity Question \n and no longer have it available for the users to select ?");
    if (conformResult == true) {
        if (cansubmit) {

            req.Id = Id;
            req.Question = $('#Question').val();
            req.StatusCode = Status;

            var reqUri = '/api/SecurityQuestionMaster/';
            ajaxHelper(reqUri + Id, 'PUT', req).done(function (data) {
                window.location.href = "/SecurityQuestion/";
            });
        }
    }
    else {
        return false;
    }
}