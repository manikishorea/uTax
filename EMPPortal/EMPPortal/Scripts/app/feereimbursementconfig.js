var _prevData = {};

function getServiceBureauTransInformation() {
    var Id;

    if ($('#entityid').val() != $('#myentityid').val()) {
        Id = $('#myid').val();
    }
    else {
        Id = $('#UserId').val();
    }
    var url = '/api/FeeReimbursement';
    if (Id != '' && Id != null && Id != '00000000-0000-0000-0000-000000000000') {
        ajaxHelper(url + '?Id=' + Id, 'GET').done(function (data) {
            _prevData = data;
            if (data != null && data != undefined && data != '') {
                $('#ID').val(data["ID"]);
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


function fnSaveFeeReimbursementConfig(SaveType) {
    var req = {};
    var cansubmit = true;
    var error = $('#error');
    error.html('');
    error.hide();

    var success = $('#success');
    success.html('');
    success.hide();

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
    else if ($('#txtConfirmRTN').val() != req.RTN) {
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
    else if ($('#txtConfirmBankAccount').val() != req.BankAccountNo) {
        $('#txtConfirmBankAccount').addClass("error_msg");
        $('#txtConfirmBankAccount').attr('title', 'Bank Account Number and Confirm Bank Account Number not matching');
        cansubmit = false;
    }
    req.IsAuthorize = $('#chkAughorize').is(':checked');

    if (!req.IsAuthorize) {
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
        if ($('#entityid').val() != $('#myentityid').val()) {
            req.UserId = $('#UserId').val();
            req.refId = $.trim($('#myid').val());
        } else {
            req.UserId = $('#UserId').val();
            req.refId = $.trim($('#UserId').val());
        }
        var Uri = '/api/FeeReimbursement/';
        ajaxHelper(Uri, 'POST', req, false).done(function (data, status) {
            $("html, body").animate({ scrollTop: 0 }, "slow");
            if (data == 'true' || data == 'True' || data == true) {
                success.show();
                success.append('<p> Record saved successfully. </p>');

                SaveConfigStatusActive('done'); //,'mainsite',''
                //ResetMainSiteActivation();
                getConfigStatus();

                var ActiveMyAccountStatus = $('#ActiveMyAccountStatus').val();
                if (SaveType == 1) {

                    if ($('#item6df12cef-8bdb-4d84-9057-095590ea0a79 #site0eda5d25-591c-4e01-a845-fb580572cff5').length > 0) {
                        window.location.href = $('#item6df12cef-8bdb-4d84-9057-095590ea0a79 #site0eda5d25-591c-4e01-a845-fb580572cff5').attr('href');
                        return;
                    }
                    if ($('#item6df12cef-8bdb-4d84-9057-095590ea0a79 #site0eda5d25-591c-4e01-a845-fb580572cfe8').length > 0) {
                        window.location.href = $('#item6df12cef-8bdb-4d84-9057-095590ea0a79 #site0eda5d25-591c-4e01-a845-fb580572cfe8').attr('href');
                        return;
                    }

                    if (ActiveMyAccountStatus == '0' || ActiveMyAccountStatus == 0) {

                        if ($('#entityid').val() != $('#myentityid').val()) {
                            window.location.href = "/Configuration/ActivateInformation?Id=" + $('#myid').val() + '&entitydisplayid=' + $('#myentitydisplayid').val() + '&ptype=config&entityid=' + $('#myentityid').val();
                        } else {
                            window.location.href = "/Configuration/ActivateInformation";
                        }
                    } else {

                        if ($('#entityid').val() != $('#myentityid').val()) {
                            window.location.href = "/Configuration/Dashboard?Id=" + $('#myid').val() + '&entitydisplayid=' + $('#myentitydisplayid').val() + '&ptype=config&entityid=' + $('#myentityid').val();
                        } else {
                            window.location.href = "/Configuration/Dashboard";
                        }
                    }

                }

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

function SaveFeeReimForNext(SaveType) {


    var ActiveMyAccountStatus = $('#ActiveMyAccountStatus').val();
    if (SaveType == 3) {
        SaveConfigStatusActive('done');
        if (ActiveMyAccountStatus == '0' || ActiveMyAccountStatus == 0) {

            if ($('#entityid').val() != $('#myentityid').val()) {
                window.location.href = "/Configuration/ActivateInformation?Id=" + $('#myid').val() + '&entitydisplayid=' + $('#myentitydisplayid').val() + '&ptype=config&entityid=' + $('#myentityid').val();
            } else {
                window.location.href = "/Configuration/ActivateInformation";
            }
        } else {

            if ($('#entityid').val() != $('#myentityid').val()) {
                window.location.href = "/Configuration/Dashboard?Id=" + $('#myid').val() + '&entitydisplayid=' + $('#myentitydisplayid').val() + '&ptype=config&entityid=' + $('#myentityid').val();
            } else {
                window.location.href = "/CustomerInformation/Dashboard";
            }
        }

    }
}

$('.panel-default input').change(function (e) {
    var _changed = false;
    if (_prevData) {
        if (_prevData.AccountName != $('#txtNameofAccount').val())
            _changed = true;
        if (_prevData.BankName != $('#txtBankName').val())
            _changed = true;
        if (_prevData.RTN != $('#txtRTN').val() || _prevData.RTN != $('#txtConfirmRTN').val())
            _changed = true;
        if (_prevData.BankAccountNo != $('#txtBankAccount').val() || _prevData.BankAccountNo != $('#txtConfirmBankAccount').val())
            _changed = true;
        if ((_prevData.AccountType && $('#rbAccountTypeNo').prop('checked')) || (!_prevData.AccountType && $('#rbAccountTypeYes').prop('checked')))
            _changed = true;
    }
    if (_changed)
        $('#divSVBFeeReimNoteActive').show();
    else
        $('#divSVBFeeReimNoteActive').hide();
})
