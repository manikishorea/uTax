﻿function getEnrollServiceBureauTransInformation() {

    var Id = $('#UserId').val();
    var Cid = getUrlVars()["Id"];
    if (Cid) {
        Id = Cid;
    }


    var BankId = getUrlVars()["bankid"];
    if (BankId) {
        var url = '/api/EnrollFeeReimbursement';
        if (Id != '' && Id != null && Id != '00000000-0000-0000-0000-000000000000') {
            ajaxHelper(url + '?Id=' + Id + '&BankId=' + BankId, 'GET').done(function (data) {
                if (data != null) {
                    if (window.location.href.indexOf('enrollmentsummary') < 0) {
                        $('#ID').val(data["ID"]);
                    }
                    $('#txtNameofAccount').val(data["AccountName"]);
                    $('#txtBankName').val(data["BankName"]);
                    if (data["AccountType"] == true) {
                        $('#rbAccountTypeYes').prop('checked', true);
                    }
                    else {
                        $('#rbAccountTypeNo').prop('checked', true);
                    }
                    $('#txtRTN').val(data["RTN"]);
                    $('#txtConfirmRTN').val(data["RTN"]);

                    $('#txtBankAccount').val(data["BankAccountNo"]);
                    $('#txtConfirmBankAccount').val(data["BankAccountNo"]);

                    if (data["IsAuthorize"] == true) {
                        $('#chkAughorize').prop('checked', true);
                    } else {
                        $('#chkAughorize').prop('checked', false);
                    }
                }
            });
        }
    }
}

function fnSaveEnrollFeeReimbursementConfig(SaveType) {
    var req = {};
    var cansubmit = true;
    var error = $('#error');
    error.html('');
    error.hide();

    var success = $('#success');
    success.html('');
    success.hide();

    $('*').removeClass("error_msg");
    $('*').attr('title', '');

    var BankId = getUrlVars()["bankid"];

    req.BankId = BankId;
    req.AccountName = $('#txtNameofAccount').val();
    req.BankName = $('#txtBankName').val();
    req.AccountType = $('#rbAccountTypeYes').is(':checked') ? 1 : 2;
    req.RTN = $('#txtRTN').val();
    if (req.AccountName == "") {
        $('#txtNameofAccount').addClass("error_msg");
        $('#txtNameofAccount').attr('title', 'Please enter Name on Account');
        cansubmit = false;
    }
    if (req.AccountName == "") {
        $('#txtBankName').addClass("error_msg");
        $('#txtBankName').attr('title', 'Please enter Bank Name');
        cansubmit = false;
    }
    if (req.RTN == "") {
        $('#txtRTN').addClass("error_msg");
        $('#txtRTN').attr('title', 'Please enter RTN');
        cansubmit = false;
    }
    else if (!checkABA(req.RTN)) {
        $('#txtRTN').addClass("error_msg");
        $('#txtRTN').attr('title', 'Please enter Valid RTN');
        cansubmit = false;
    }
    if ($('#txtConfirmRTN').val() != req.RTN) {
        $('#txtConfirmRTN').addClass("error_msg");
        $('#txtConfirmRTN').attr('title', 'RTN and Confirm RTN not matching');
        cansubmit = false;
    }

    req.BankAccountNo = $('#txtBankAccount').val();
    if (req.BankAccountNo == "") {
        $('#txtBankAccount').addClass("error_msg");
        $('#txtBankAccount').attr('title', 'Please enter Bank Account Number');
        cansubmit = false;
    }
    if ($('#txtConfirmBankAccount').val() != req.BankAccountNo) {
        $('#txtConfirmBankAccount').addClass("error_msg");
        $('#txtConfirmBankAccount').attr('title', 'Bank Account Number and Confirm Bank Account Number not matching');
        cansubmit = false;
    }

    req.IsAuthorize = $('#chkAughorize').is(':checked');
    if (!req.IsAuthorize) {
        $('#dv_authorize').addClass("error_msg");
        $('#dv_authorize').attr('title', 'Please select');
        cansubmit = false;
    }

    if (!cansubmit) {
        error.show();
        error.append('<p> Please correct the error(s). </p>');
        $("html, body").animate({ scrollTop: 0 }, "slow");
        return false;
    }
    if (cansubmit) {
        $('*').removeClass("error_msg");
        error.hide();
        error.html('');
        req.Id = $('#ID').val();
        req.UserId = $('#UserId').val();
        req.refId = $.trim($('#UserId').val());

        var Cid = getUrlVars()["Id"];
        if (Cid) {
            req.refId = Cid;
        }
        var entitydisplayid = getUrlVars()["entitydisplayid"];
        var Uri = '/api/EnrollFeeReimbursement/';
        ajaxHelper(Uri, 'POST', req).done(function (data, status) {
            $("html, body").animate({ scrollTop: 0 }, "slow");
            if (data == 'true' || data == 'True' || data == true) {
                success.show();
                success.append('<p> Record saved successfully. </p>');

                SaveConfigStatusActive("done", req.BankId);

                if (SaveType == 1) {
                    window.location.href = "/Configuration/ActivateInformation/";
                } else if (SaveType == 3) {
                    if ($('#entityid').val() != $('#myentityid').val()) {
                        if ($('#site0eda5d25-591c-4e01-a845-fb580572cff5').length > 0)
                            window.location.href = $('#site0eda5d25-591c-4e01-a845-fb580572cff5').attr('href');
                        else if ($('#site0eda5d25-591c-4e01-a845-fb580572cfe8').length > 0)
                            window.location.href = $('#site0eda5d25-591c-4e01-a845-fb580572cfe8').attr('href');
                        else
                            window.location.href = "/Enrollment/enrollmentsummary?Id=" + req.refId + '&entitydisplayid=' + entitydisplayid + '&entityid=' + $('#myentityid').val() + '&ParentId=' + $('#myparentid').val() + '&ptype=enrollment&bankid=' + BankId;
                    }
                    else {
                        if ($('#site0eda5d25-591c-4e01-a845-fb580572cff5').length > 0)
                            window.location.href = $('#site0eda5d25-591c-4e01-a845-fb580572cff5').attr('href');
                        else if ($('#site0eda5d25-591c-4e01-a845-fb580572cfe8').length > 0)
                            window.location.href = $('#site0eda5d25-591c-4e01-a845-fb580572cfe8').attr('href');
                        else
                            window.location.href = "/Enrollment/enrollmentsummary?bankid=" + BankId;
                    }
                }
                else
                    //  getConfigStatus();
                    return true;
            }
            else {
                error.show();
                error.append('<p>  Record not saved. </p>');
                return false;
            }
        });
    }
}
