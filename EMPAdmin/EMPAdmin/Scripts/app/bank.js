function getAllBank() {
    var booksUri = '/api/BankMaster/';
    $("#table_bank > tbody").remove();
    var table = $('#table_bank').append('<tbody/>'); //$(document.body)
    ajaxHelper(booksUri, 'GET').done(function (data) {
        $.each(data, function (rowIndex, r) {
            var row = $("<tr/>").appendTo(table);;
            row.append($("<td/>").text(r["BankName"]));
            row.append($("<td/>").text(r["BankCode"]));
            row.append($("<td/>").text(r["BankServiceFees"]));
            row.append($("<td/>").text(r["MaxFeeLimitDeskTop"]));
            row.append($("<td/>").text(r["MaxTranFeeDeskTop"]));
            row.append($("<td/>").text(r["MaxFeeLimitMSO"]));
            row.append($("<td/>").text(r["MaxTranFeeMSO"]));
            row.append($("<td/>").text(r["StatusCode"] == "ACT" ? "Activate" : "Deactivate"));
            row.append(BankActionList('/bank/create/', r["Id"], '/banksubquestions/index/', r["Id"], r["StatusCode"]));
        });
    });
    $(".office-table .table").trigger('footable_initialize');
    // GridFunctionality();
}

function getBankDetail(Id) {

    var Uri = '/api/BankMaster/';
    var allchkbox = $('input[type=checkbox].chkEntity');
    if (Id != '' && Id != null && Id != '00000000-0000-0000-0000-000000000000') {
        ajaxHelper(Uri + Id, 'GET').done(function (data) {
            $('#Id').val(data["Id"])
            $('#BankName').val(data["BankName"]);
            $('#BankCode').val(data["BankCode"]);
            $('#BankServiceFees').val(data["BankServiceFees"]);
            $('#MaxFeeLimitDeskTop').val(data["MaxFeeLimitDeskTop"])
            $('#MaxTranFeeDeskTop').val(data["MaxTranFeeDeskTop"])
            $('#MaxFeeLimitMSO').val(data["MaxFeeLimitMSO"])
            $('#MaxTranFeeMSO').val(data["MaxTranFeeMSO"])
            $('#Description').val(data["Description"])

            $('#BankProductDocument').val(data["BankProductDocument"]);
            $.each(data.Entities, function (coIndex, c) {
                $('#chk' + c["Id"]).attr('checked', 'checked');
            });

            if (data["BankProductDocument"] != 'NoFile') {
                getDocument(data["BankProductDocument"], $('#tcpathabcd'));
            }
        });
    }
}

function IsBankNumber(input) {
    //var regexp = /^\d+\.\d{0,5}$/;
    var regexp = /^\s*-?[1-9]\d*(\.\d{1,2})?\s*$/;
    return  regexp.test(input)
}

function fnSaveBank() {
    $('#error').hide();
    $('#error p').html('');
    $('#success').hide();
    $('#success p').html('');
    var req = {};
    var cansubmit = true;

    $('#BankName').removeClass("error_msg");
    if ($.trim($('#BankName').val()) == "") {
        $('#BankName').addClass("error_msg");
        $('#BankName').attr('title', 'Please enter Bank Name');
        cansubmit = false;
    }

    $('#BankCode').removeClass("error_msg");
    if ($.trim($('#BankCode').val()) == "") {
        $('#BankCode').addClass("error_msg");
        $('#BankCode').attr('title', 'Please enter Bank Code');
        cansubmit = false;
    }

    $('#BankServiceFees').removeClass("error_msg");
    var BankServiceFees = $('#BankServiceFees').val();
    if (!IsBankNumber(BankServiceFees)) {
        $('#BankServiceFees').addClass("error_msg");
        $('#BankServiceFees').attr('title', 'Please enter valid Bank Service Fees');
        cansubmit = false;
    }


    $('#MaxFeeLimitDeskTop').removeClass("error_msg");
    var intRegex = /^\d+$/;
    var MaxFeeLimitDeskTop = $.trim($('#MaxFeeLimitDeskTop').val());
    if (!IsBankNumber(MaxFeeLimitDeskTop)) {
        // alert();
        $('#MaxFeeLimitDeskTop').addClass("error_msg");
        $('#MaxFeeLimitDeskTop').attr('title', 'Please enter valid Maximum Fees Limit (DeskTop)');
        cansubmit = false;
    }

    $('#MaxTranFeeDeskTop').removeClass("error_msg");
    var MaxTranFeeDeskTop = $('#MaxTranFeeDeskTop').val();
    if (!IsBankNumber(MaxTranFeeDeskTop)) {
        $('#MaxTranFeeDeskTop').addClass("error_msg");
        $('#MaxTranFeeDeskTop').attr('title', 'Please enter valid Maximum Transmission Fees (DeskTop)');
        cansubmit = false;
    }

    $('#MaxFeeLimitMSO').removeClass("error_msg");
    var MaxFeeLimitMSO = $('#MaxFeeLimitMSO').val();
    if (!IsBankNumber(MaxFeeLimitMSO)) {
        $('#MaxFeeLimitMSO').addClass("error_msg");
        $('#MaxFeeLimitMSO').attr('title', 'Please enter valid Maximum Fees Limit (MSO)');
        cansubmit = false;
    }

    $('#MaxTranFeeMSO').removeClass("error_msg");
    var MaxTranFeeMSO = $('#MaxTranFeeMSO').val();
    if (!IsBankNumber(MaxTranFeeMSO)) {
        $('#MaxTranFeeMSO').addClass("error_msg");
        $('#MaxTranFeeMSO').attr('title', 'Please enter valid Maximum Transmission Fees (MSO)');
        cansubmit = false;
    }

    var Entities = [];

    var IsChecked = false;
    var allchkbox = $('input[type=checkbox].chkEntity');
    $.each(allchkbox, function (indx, valu) {
        var chkId = $(valu).attr('id');
        var valE = $('#' + chkId).val()
        if ($(valu).is(':checked')) {
            IsChecked = true;
            Entities.push({
                Id: valE
            });
        }
    });


    if(!fnUploadBankDocument())
    {
        cansubmit = false;
    }

    if (cansubmit) {
        req.Id = $('#Id').val();
        req.BankName = $('#BankName').val();
        req.BankServiceFees = BankServiceFees;
        req.MaxFeeLimitDeskTop = $('#MaxFeeLimitDeskTop').val();
        req.MaxTranFeeDeskTop = $('#MaxTranFeeDeskTop').val();
        req.MaxFeeLimitMSO = $('#MaxFeeLimitMSO').val();
        req.MaxTranFeeMSO = $('#MaxTranFeeMSO').val();
        req.Description = $('#Description').val();
        req.UserId = $('#UserId').val();
        req.BankProductDocument = $('#BankProductDocument').val();
        req.Entities = Entities;
        req.BankCode = $('#BankCode').val();

        var Uri = '/api/BankMaster/';
        ajaxHelper(Uri, 'POST', req).done(function (data, textStatus) {
            if (textStatus == 'notmodified') {
                $('#error p').html('Bank is already exist.');
                $('#error').show();
            } else if (textStatus == 'notccceptable') {
                $('#error p').html('Bank has not saved.');
                $('#error').show();
            } else {
                $('#Id').val(data)
                $('#success').show();
                if (req.Id == data) {
                    $('#success p').html('Bank updated successfully.');
                } else {
                    $('#success p').html('Bank created successfully.');
                }

                if (req.BankProductDocument != 'NoFile') {
                    getDocument(req.BankProductDocument, $('#tcpathabcd'));
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
    var conformResult = confirm("Are you sure you wish to  " + statuscode + "  the specific Bank Master \n and no longer have it available for the users to select ?");
    if (conformResult == true) {

        // alert(Id +","+ Status)
        var req = {};
        var cansubmit = true;

        if (cansubmit) {

            req.Id = Id;
            req.BankName = $('#BankName').val();
            req.StatusCode = Status;

            var reqUri = '/api/BankMaster/';
            ajaxHelper(reqUri + Id, 'PUT', req).done(function (data, textStatus) {
                if (textStatus == 'notmodified') {
                }
                else {
                    $('#success').show();
                    $('#success p').html('This Bank ' + statuscode + 'd successfully.');
                    getAllBank();
                }
            });
        }
    }
    else {
        return false;
    }
}

function fnUploadBankDocument() {
    var documentpath = "nofile";
    var cansubmit = true;

    $('#fileToUpload').parent('div').find('.bootstrap-filestyle .form-control').removeClass("error_msg");

    if ($('#fileToUpload').val() != '') {
        if (!$('#fileToUpload').val().match(/(?:pdf|PDF)$/)) {
            $('#fileToUpload').parent('div').find('.bootstrap-filestyle .form-control').addClass("error_msg");
            $('#fileToUpload').parent('div').find('.bootstrap-filestyle .form-control').attr('title', 'Please upload file having extension (.pdf)');
            return false;
        }
    }

    if (cansubmit) {
        var data = new FormData()
        var files = $("#fileToUpload").get(0).files;
        if (files.length > 0) {
            data.append("UploadedImage", files[0]);
        }

        data.append("UserID", $('#UserId').val());
        data.append("Id", $('#BankProductDocument').val());
        var url = '/api/Document/SaveAjaxDocuments';
        ajaxUploadHelper(url, 'POST', data).done(function (result) {
            documentpath = result;
            $('#BankProductDocument').val(result);
            //$('#h_docpath').val(result);
            //SaveApiData();
        });
    }

    return true;
    
}