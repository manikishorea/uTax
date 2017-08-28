function getAllBankSubQuestion(BankId) {
     var booksUri = '/api/BankMaster/';
    $("#table_BankSubQuestion > tbody").remove();
    var table = $('#table_BankSubQuestion').append('<tbody/>'); //$(document.body)
    if (BankId != '' && BankId != null && BankId != '00000000-0000-0000-0000-000000000000') {
        ajaxHelper(booksUri + BankId, 'GET').done(function (data) {
            $('#BankName').html(data["BankName"]);
            $('#banksubquestionlink').html('<a href="/banksubquestions/create?BankId=' + data["Id"] + '&BankName=' + data["BankName"] + '"><i class="fa fa-plus" aria-hidden="true" title="Add Contact Person Title"></i> Add ' + data["BankName"] + ' Sub Question </a>');
            $.each(data.BankSubQuestions, function (rowIndex, r) {
                var row = $("<tr/>").appendTo(table);
                row.append($("<td/>").text(r["Questions"]));
                 row.append(ActionList('/BankSubQuestions/create/', r["Id"], r["StatusCode"]));
            });
        });
    }
}


function getBankSubQuestionDetail(Id) {

    var Uri = '/api/BankSubQuestion/';

    if (Id != '' && Id != null && Id != '00000000-0000-0000-0000-000000000000') {
        ajaxHelper(Uri + Id, 'GET').done(function (data) {
            $('#Id').val(data["Id"])
            $('#Questions').val(data["Questions"])
            $('#BankId').val(data["BankId"])
            $('#BankName').html(data["BankName"])
            $('#Description').val(data["Description"])
        });
    }
}

function fnSaveBankSubQuestion() {
    $('#success').hide();
    $('#success p').html('');
    var req = {};
    var cansubmit = true;

    $('#Questions').removeClass("error_msg");
    if ($.trim($('#Questions').val()) == "") {
        $('#Questions').addClass("error_msg");
        $('#Questions').attr('title', 'Please enter Sub-Question');
        cansubmit = false;
    }


    if (cansubmit) {

        req.Id = $('#Id').val();
        req.BankId = $('#BankId').val();
        req.Questions = $('#Questions').val();
        req.Description = $('#Description').val();
        req.UserId = $('#UserId').val();

        var Uri = '/api/BankSubQuestion/';
        ajaxHelper(Uri, 'POST', req).done(function (data) {
          //  window.location.href = "/BankSubQuestions/Index/" + req.BankId;

            $('#Id').val(data["Id"])
            $('#success').show();

            if (req.Id == data["Id"]) {
                $('#success p').html('Bank Sub Question updated successfully.');
            } else {
                $('#success p').html('Bank Sub Question created successfully.');
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
    var conformResult = confirm("Are you sure you wish to " + statuscode + " the specific Bank Sub Question \n and no longer have it available for the users to select ?");
    if (conformResult == true) {

        // alert(Id +","+ Status)
        var req = {};
        var cansubmit = true;

        if (cansubmit) {

            req.Id = Id;
            req.BankId = $('#BankId').val();
            req.Questions = $('#Questions').val();
            req.Description = $('#Description').val();
            req.StatusCode = Status;

            var reqUri = '/api/BankSubQuestion/';
            ajaxHelper(reqUri + Id, 'PUT', req).done(function (data) {
                // window.location.href = "/BankSubQuestions/Index/" + req.BankId;
                getAllBankSubQuestion(req.BankId);
            });
        }
    }
    else {
        return false;
    }
}