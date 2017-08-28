var error = $('#error');
var success = $('#success');
ResetErrorSuccess();
var Id = '';
var bankDetails;
var SiteType = 0;
var IsFeeReimbursement = false;
var IsEnrollment = false;
var IsonHold = false;
var getpaymentInterval = null;

$(window).load(function () {

    if (window.location.href.indexOf('efile') > 0)
        SiteType = 1;
    else
        SiteType = 2;

    //$("#txtExpiration").datepicker({
    //    format: "mm/yy",
    //    viewMode: "months",
    //    minViewMode: "months",
    //    startDate: new Date()
    //});
    getStateMaster($('#ddl_state'));
    $('#ddl_state').val(0);
    getpaymentInterval = setInterval(getPaymentInfo, 1000);

})

function ResetErrorSuccess() {
    error.html('');
    error.hide();
    success.html('');
    success.hide();
}

function getPaymentInfo() {

    if ($('#divmenu ul').length > 0)
        clearInterval(getpaymentInterval);
    else
        return;

    var UserId = $('#UserId').val();
    var EntityId = $('#entityid').val();
    var bankid = getUrlVars()["bankid"];

    if ((bankid == '' || bankid == null || bankid == undefined || bankid == '00000000-0000-0000-0000-000000000000') && $('#item6df12cef-8bdb-4d84-9057-095590ea0a79 #site0eda5d25-591c-4e01-a845-fb580572cff5').length == 0) {
        return;
    }
    if (!bankid)
        bankid = '00000000-0000-0000-0000-000000000000';

    if ($('#entityid').val() != $('#myentityid').val()) {
        UserId = $('#myid').val();
        EntityId = $('#myentityid').val();
    }
    debugger;
    //$.blockUI({ message: '<img src="../content/images/loading-img.gif"/>' });
    var custmorUri = '/api/CustomerPaymentOptions/GetCustomerPaymentInfo';
    if (UserId != '' && UserId != null && UserId != '00000000-0000-0000-0000-000000000000')
        ajaxHelper(custmorUri + '?UserId=' + UserId + '&EntityId=' + EntityId + '&SiteType=' + SiteType + '&BankId=' + bankid, 'GET').done(function (data) {

            if (data.status) {
                Id = data.Id;
                IsFeeReimbursement = data.IsFeeReimbursement;
                IsEnrollment = data.IsEnrollment;
                IsonHold = data.IsonHold;
                if (data.PaymentType == 1) {
                    $('#rbefileCC').prop('checked', true);
                    $('#dbBankSubQuestion').hide();
                    $('#liefileCC').show();
                    $('#liefileBA').hide();
                    if (data.CreditCard) {
                        if (data.CreditCard.status) {
                            $('#txtCardholderName').val(data.CreditCard.CardHolderName);
                            $('#txtBillingAddress').val(data.CreditCard.Address);
                            $('#txtCardNumber').val(data.CreditCard.CardNumber);
                            $('#txtExpiration').val(data.CreditCard.Expiration);
                            $('#txtZip').val(data.CreditCard.ZipCode);
                            $("select[name=ddlstate] option[value=" + data.CreditCard.StateId + "]").attr('selected', 'selected');
                            $('#txtCity').val(data.CreditCard.City);
                            if (data.CreditCard.CardType == 1)
                                $('#rbmastercard').prop('checked', true);
                            else if (data.CreditCard.CardType == 2)
                                $('#rbvisa').prop('checked', true);
                            else if (data.CreditCard.CardType == 3)
                                $('#rbamericanexpress').prop('checked', true);
                            $('#chkAughorizeCC').prop('checked', true);
                        }
                    }

                }
                else if (data.PaymentType == 2) {
                    $('#rbefileBA').prop('checked', true);
                    $('#liefileCC').hide();
                    $('#liefileBA').show();
                    if ($('#sitea55334d1-3960-44c4-8cf1-e3ba9901f2be').length > 0 || $('#site60025459-7568-4a77-b152-f81904aaaa63').length > 0 || IsFeeReimbursement)
                        $('#dbBankSubQuestion').show();
                    if (data.IsSameBankAccount == 1)
                        $('#rbBankProdYes').prop('checked', true);
                    else
                        $('#rbBankProdNo').prop('checked', true);

                    if (data.ACH) {
                        if (data.ACH.status) {
                            $('#txtNameofAccount').val(data.ACH.AccountName);
                            $('#txtBankName').val(data.ACH.BankName);
                            $('#txtRTN, #txtConfirmRTN').val(data.ACH.RTN);
                            $('#txtBankAccount, #txtConfirmBankAccount').val(data.ACH.AccountNumber);
                            if (data.ACH.AccountType == 1)
                                $('#rbAccountTypeYes').prop('checked', true);
                            else if (data.ACH.AccountType == 2)
                                $('#rbAccountTypeNo').prop('checked', true);
                            $('#chkAughorizeACH').prop('checked', true);

                        }
                        else if (data.IsSameBankAccount == 1) {
                            var bankURI = '/api/CustomerPaymentOptions/GetCustomerBankDetails?UserId=' + UserId + '&EntityId=' + EntityId + '&CustId=' + UserId + '&BankId=' + bankid;
                            ajaxHelper(bankURI, 'GET').done(function (data) {
                                if (data)
                                    if (data.status) {
                                        $('#txtNameofAccount').val(data.AccountName);
                                        $('#txtBankName').val(data.BankName);
                                        $('#txtRTN, #txtConfirmRTN').val(data.RTN);
                                        $('#txtBankAccount, #txtConfirmBankAccount').val(data.AccountNumber);
                                        if (data.AccountType == 1)
                                            $('#rbAccountTypeYes').prop('checked', true);
                                        else if (data.AccountType == 2)
                                            $('#rbAccountTypeNo').prop('checked', true);
                                        else
                                            $('input[name="AccountType"]').prop('checked', false);
                                    }

                            });
                        }
                    }
                }

                $('#tb_feeSummary').html('');
                $.each(data.Fees, function (item, value) {

                    $('#tb_feeSummary').append('<tr><td>' + value.Fee + '</td><td><span>$</span> <span class="hdnTranFee">' + value.Amount + '</span></td></tr>');
                })

                bankDetails = data.BankDetails;
            }
            else {
                $('#liefileCC').hide();
                $('#liefileBA').hide();

            }
        });
}

function efilePaymentChanges(type) {
    var bankid = getUrlVars()["bankid"];
    if (type == 'cc')
        $('#dbBankSubQuestion').hide();
    else {
        if ($('#sitea55334d1-3960-44c4-8cf1-e3ba9901f2be').length > 0 || $('#site60025459-7568-4a77-b152-f81904aaaa63').length > 0 || IsFeeReimbursement)
            $('#dbBankSubQuestion').show();
    }
}

function SaveefilePaymentOptions() {

    var errMsg = '';
    ResetErrorSuccess();
    $('*').removeClass('error_msg');
    var bankid = getUrlVars()["bankid"];
    if (!bankid && $('#item6df12cef-8bdb-4d84-9057-095590ea0a79 #site0eda5d25-591c-4e01-a845-fb580572cff5').length > 0) {
        bankid = '00000000-0000-0000-0000-000000000000';
    }

    if ($('input[name="rbefile"]:checked').length == 0) {
        errMsg += '<p>  Please select payment type. </p>';
        $('#dvFirst').addClass('error_msg');
    }

    if ($('#rbefileBA').prop('checked') && $('#dbBankSubQuestion').css('display') != 'none') {
        if ($('input[name="rbBank"]:checked').length == 0) {
            errMsg += '<p>  Please select Is Same Bank Account. </p>';
            $('#dbBankSubQuestion').addClass('error_msg');
        }
    }

    if (errMsg != '') {
        error.show();
        error.append(errMsg);
        return;
    }

    var UserId = $('#UserId').val();
    var EntityId = $('#entityid').val();

    if ($('#entityid').val() != $('#myentityid').val()) {
        UserId = $('#myid').val();
        EntityId = $('#myentityid').val();
    }

    var req = {};
    req.PaymentType = $('#rbefileCC').prop('checked') ? 1 : 2;
    req.IsSameBankAccount = $('#rbefileBA').prop('checked') ? ($('#rbBankProdYes').prop('checked') ? 1 : 2) : 0;
    req.UserId = UserId;
    req.Id = Id;
    req.SiteType = SiteType;
    req.BankId = bankid;

    //$.blockUI({ message: '<img src="../content/images/loading-img.gif"/>' });
    var Uri = '/api/CustomerPaymentOptions/SaveefilePaymentOptions';
    ajaxHelper(Uri, 'POST', req).done(function (data, status) {
        if (data) {

            if (data.status) {
                Id = data.Id;
                if (req.PaymentType == 1) {
                    $('#liefileCC').show();
                    $('#liefileBA').hide();
                    $('#a_cc').click();

                }
                else {
                    $('#liefileCC').hide();
                    $('#liefileBA').show();

                    if (req.IsSameBankAccount == 1) {
                        var bankURI = '/api/CustomerPaymentOptions/GetCustomerBankDetails?UserId=' + UserId + '&EntityId=' + EntityId + '&CustId=' + UserId + '&BankId=' + bankid;
                        ajaxHelper(bankURI, 'GET').done(function (data) {

                            if (data)
                                if (data.status) {
                                    $('#txtNameofAccount').val(data.AccountName);
                                    $('#txtBankName').val(data.BankName);
                                    $('#txtRTN, #txtConfirmRTN').val(data.RTN);
                                    $('#txtBankAccount, #txtConfirmBankAccount').val(data.AccountNumber);
                                    if (data.AccountType == 1)
                                        $('#rbAccountTypeYes').prop('checked', true);
                                    else if (data.AccountType == 2)
                                        $('#rbAccountTypeNo').prop('checked', true);
                                    else
                                        $('input[name="AccountType"]').prop('checked', false);
                                }
                            $('#a_ba').click();

                        });
                    }
                    else {
                        $('#txtNameofAccount').val('');
                        $('#txtBankName').val('');
                        $('#txtRTN, #txtConfirmRTN').val('');
                        $('#txtBankAccount, #txtConfirmBankAccount').val('');
                        $('input[name="AccountType"]').prop('checked', false);

                        $('#a_ba').click();
                    }
                }
            }
            success.show();
            success.append('<p> Payment Option details saved successfully</p>');
        }
    });
}

function saveCC(type) {

    ResetErrorSuccess();

    if (Id == '' || Id == '00000000-0000-0000-0000-000000000000') {
        error.show();
        error.append('<p> Please save Payment Options. </p>');
        return;
    }

    var _continue = true;

    if ($('#txtCardholderName').val().trim() == '') {
        $('#txtCardholderName').addClass("error_msg");
        $('#txtCardholderName').attr('title', 'Please enter Cardholder Name');
        _continue = false;
    }
    else {
        $('#txtCardholderName').removeClass("error_msg");
        $('#txtCardholderName').attr('title', '');
    }

    if ($('#txtBillingAddress').val().trim() == '') {
        $('#txtBillingAddress').addClass("error_msg");
        $('#txtBillingAddress').attr('title', 'Please enter Billing Address');
        _continue = false;
    }
    else {
        $('#txtBillingAddress').removeClass("error_msg");
        $('#txtBillingAddress').attr('title', '');
    }

    if ($('#txtCardNumber').val().trim() == '') {
        $('#txtCardNumber').addClass("error_msg");
        $('#txtCardNumber').attr('title', 'Please enter Credit Card Number');
        _continue = false;
    }
    else {
        $('#txtCardNumber').removeClass("error_msg");
        $('#txtCardNumber').attr('title', '');
    }

    if ($('#txtExpiration').val().trim() == '') {
        $('#txtExpiration').addClass("error_msg");
        $('#txtExpiration').attr('title', 'Please enter Expiration');
        _continue = false;
    }
    else {
        $('#txtExpiration').removeClass("error_msg");
        $('#txtExpiration').attr('title', '');
    }

    if ($('#txtExpiration').val().trim() != '') {
        var exp = $('#txtExpiration').val();
        if (exp.indexOf('/') > 0) {
            var dt = new Date();
            var cm = dt.getMonth() + 1;
            cm = cm < 10 ? '0' + cm : cm;
            var cy = dt.getFullYear() - 2000;
            var expi = exp.split('/');
            if (parseInt(expi[0]) > 12 || (parseInt(expi[0]) < cm && parseInt(expi[1]) <= cy) || parseInt(expi[1]) < cy) {
                $('#txtExpiration').addClass("error_msg");
                $('#txtExpiration').attr('title', 'Please enter valid Expiration');
                _continue = false;
            }
        }
        else {
            $('#txtExpiration').addClass("error_msg");
            $('#txtExpiration').attr('title', 'Please enter valid Expiration');
            _continue = false;
        }
    }

    if ($('#txtZip').val().trim() == '') {
        $('#txtZip').addClass("error_msg");
        $('#txtZip').attr('title', 'Please enter Enter Zip Code');
        _continue = false;
    }
    else {
        $('#txtZip').removeClass("error_msg");
        $('#txtZip').attr('title', '');
    }

    if ($('#ddl_state').val() == 0 || $('#ddl_state').val() == '' || $('#ddl_state').val() == undefined) {
        $('#ddl_state').addClass("error_msg");
        $('#ddl_state').attr('title', 'Please select State');
        _continue = false;
    }
    else {
        $('#ddl_state').removeClass("error_msg");
        $('#ddl_state').attr('title', '');
    }

    if ($('#txtCity').val().trim() == '') {
        $('#txtCity').addClass("error_msg");
        $('#txtCity').attr('title', 'Please enter City');
        _continue = false;
    }
    else {
        $('#txtCity').removeClass("error_msg");
        $('#txtCity').attr('title', '');
    }

    if ($('input[name="CardType"]:checked').length == 0) {
        $('#dvCardtype').addClass("error_msg");
        $('#dvCardtype').attr('title', 'Please select');
        _continue = false;
    }
    else {
        $('#dvCardtype').removeClass("error_msg");
        $('#dvCardtype').attr('title', '');
    }

    if (!$('#chkAughorizeCC').prop('checked')) {
        $('#dvccauth').addClass("error_msg");
        $('#dvccauth').attr('title', 'Please select');
        _continue = false;
    }
    else {
        $('#dvccauth').removeClass("error_msg");
        $('#dvccauth').attr('title', '');
    }

    var ccNum = $('#txtCardNumber').val();
    if (ccNum != '') {
        var isValidCC = luhnChk(ccNum);
        if (!isValidCC) {
            $('#txtCardNumber').addClass("error_msg");
            $('#txtCardNumber').attr('title', 'Please enter valid Credit Card Number');
            _continue = false;
        }
        else {
            $('#txtCardNumber').removeClass("error_msg");
            $('#txtCardNumber').attr('title', '');
        }
    }

    if (!_continue) {
        error.show();
        error.append('<p> Please correct error(s). </p>');
        return;
    }

    var UserId = $('#UserId').val();
    var EntityId = $('#entityid').val();

    if ($('#entityid').val() != $('#myentityid').val()) {
        UserId = $('#myid').val();
        EntityId = $('#myentityid').val();
    }

    var req = {};
    req.CardHolderName = $('#txtCardholderName').val().trim();
    req.CardType = $('#rbmastercard').prop('checked') ? 1 : ($('#rbvisa').prop('checked') ? 2 : 3);
    req.Address = $('#txtBillingAddress').val().trim();
    req.CardNumber = $('#txtCardNumber').val().trim();
    req.Expiration = $('#txtExpiration').val().trim();
    req.City = $('#txtCity').val().trim();
    req.StateId = $('#ddl_state').val();
    req.ZipCode = $('#txtZip').val().trim();
    req.UserId = UserId;
    req.PaymentOptionId = Id;


    var bankid = getUrlVars()["bankid"];

    var Uri = '/api/CustomerPaymentOptions/SaveCreditCardDetails';
    ajaxHelper(Uri, 'POST', req, false).done(function (data, status) {
        if (data) {
            success.show();
            success.append('<p> Credit Card  details saved successfully</p>');
            SaveConfigStatusActive('done', bankid);
            $("html, body").animate({ scrollTop: 0 }, "slow");

            if (SiteType == 1) {
                $('#site0eda5d25-591c-4e01-a845-fb580572cff5').addClass('done');
                if (type == 1) {

                    if ($('#site0eda5d25-591c-4e01-a845-fb580572cff5').parent().next('li').length) {
                        var hf = $('#site0eda5d25-591c-4e01-a845-fb580572cff5').parent().next('li')[0].children[0].href;
                        window.location.href = hf;
                        return;
                    }
                    else if ($('#entityid').val() != $('#myentityid').val()) {
                        if (ptype == 'config') {
                            window.location.href = $('#site98a706d7-031f-4c5d-8cc4-d32cc7658b63').attr('href');
                            return;
                        }
                    }
                    else {
                        if ($('#item6df12cef-8bdb-4d84-9057-095590ea0a79 #site0eda5d25-591c-4e01-a845-fb580572cff5').length == 1) {
                            window.location.href = $('#site98a706d7-031f-4c5d-8cc4-d32cc7658b63').attr('href');
                            return;
                        }
                    }

                    if (IsonHold) {
                        if ($('#site0eda5d25-591c-4e01-a845-fb580572cff5').parent().next('li').length) {
                            var hf = $('#site0eda5d25-591c-4e01-a845-fb580572cff5').parent().next('li')[0].children[0].href;
                            window.location.href = hf;
                            return;
                        }
                        else
                            return;
                    }

                    if ($('#site0eda5d25-591c-4e01-a845-fb580572cfe8').length > 0) {
                        window.location.href = $('#site0eda5d25-591c-4e01-a845-fb580572cfe8').attr('href');
                    }
                    else {
                        if ($('#site0feeb0fe-d0e7-4370-8733-dd5f7d2041fc').length > 0) {
                            if ($('#site0feeb0fe-d0e7-4370-8733-dd5f7d2041fc').hasClass('done') || IsEnrollment) {
                                if ($('#sitea55334d1-3960-44c4-8cf1-e3ba9901f2be').length > 0) {
                                    if ($('#site0eda5d25-591c-4e01-a845-fb580572cff5').hasClass('done')) {
                                        if ($('#entityid').val() != $('#myentityid').val()) {
                                            window.location.href = '/Enrollment/enrollmentsummary?Id=' + UserId + '&entitydisplayid=' + $('#myentitydisplayid').val() + '&ParentId=' + $('#myparentid').val() + '&entityid=' + $('#myentityid').val() + '&ptype=enrollment&bankid=' + bankid;
                                        }
                                        else
                                            window.location.href = '/Enrollment/enrollmentsummary?bankid=' + bankid;
                                    }
                                }
                                else {
                                    if ($('#entityid').val() != $('#myentityid').val()) {
                                        window.location.href = '/Enrollment/enrollmentsummary?Id=' + UserId + '&entitydisplayid=' + $('#myentitydisplayid').val() + '&ParentId=' + $('#myparentid').val() + '&entityid=' + $('#myentityid').val() + '&ptype=enrollment&bankid=' + bankid;
                                    }
                                    else
                                        window.location.href = '/Enrollment/enrollmentsummary?bankid=' + bankid;
                                }
                            }
                        }
                        else {
                            if ($('#sitea55334d1-3960-44c4-8cf1-e3ba9901f2be').length > 0) {
                                if ($('#site0eda5d25-591c-4e01-a845-fb580572cff5').hasClass('done')) {
                                    if ($('#entityid').val() != $('#myentityid').val()) {
                                        window.location.href = '/Enrollment/enrollmentsummary?Id=' + UserId + '&entitydisplayid=' + $('#myentitydisplayid').val() + '&ParentId=' + $('#myparentid').val() + '&entityid=' + $('#myentityid').val() + '&ptype=enrollment&bankid=' + bankid;
                                    }
                                    else
                                        window.location.href = '/Enrollment/enrollmentsummary?bankid=' + bankid;
                                }
                            }
                            else {
                                if ($('#entityid').val() != $('#myentityid').val()) {
                                    window.location.href = '/Enrollment/enrollmentsummary?Id=' + UserId + '&entitydisplayid=' + $('#myentitydisplayid').val() + '&ParentId=' + $('#myparentid').val() + '&entityid=' + $('#myentityid').val() + '&ptype=enrollment&bankid=' + bankid;
                                }
                                else
                                    window.location.href = '/Enrollment/enrollmentsummary?bankid=' + bankid;
                            }
                        }
                    }
                }
            }
            else if (SiteType == 2) {
                $('#site0eda5d25-591c-4e01-a845-fb580572cfe8').addClass('done');
                if (type == 1) {

                    if ($('#site0eda5d25-591c-4e01-a845-fb580572cfe8').parent().next('li').length) {
                        var hf = $('#site0eda5d25-591c-4e01-a845-fb580572cfe8').parent().next('li')[0].children[0].href;
                        window.location.href = hf;
                        return;
                    }
                    else if ($('#entityid').val() != $('#myentityid').val()) {
                        if (ptype == 'config') {
                            window.location.href = $('#site98a706d7-031f-4c5d-8cc4-d32cc7658b63').attr('href');
                            return;
                        }
                    }
                    else {
                        if ($('#item6df12cef-8bdb-4d84-9057-095590ea0a79 #site0eda5d25-591c-4e01-a845-fb580572cfe8').length == 1) {
                            window.location.href = $('#site98a706d7-031f-4c5d-8cc4-d32cc7658b63').attr('href');
                            return;
                        }
                    }

                    if (IsonHold) {
                        return;
                    }

                    if ($('#site0feeb0fe-d0e7-4370-8733-dd5f7d2041fc').length > 0)
                        if (!$('#site0feeb0fe-d0e7-4370-8733-dd5f7d2041fc').hasClass('done') && !IsEnrollment)
                            return;

                    if ($('#site0eda5d25-591c-4e01-a845-fb580572cff5').length > 0) {
                        if ($('#site0eda5d25-591c-4e01-a845-fb580572cff5').hasClass('done')) {
                            if ($('#sitea55334d1-3960-44c4-8cf1-e3ba9901f2be').length > 0) {
                                if ($('#sitea55334d1-3960-44c4-8cf1-e3ba9901f2be').hasClass('done')) {
                                    if ($('#entityid').val() != $('#myentityid').val()) {
                                        window.location.href = '/Enrollment/enrollmentsummary?Id=' + UserId + '&entitydisplayid=' + $('#myentitydisplayid').val() + '&ParentId=' + $('#myparentid').val() + '&entityid=' + $('#myentityid').val() + '&ptype=enrollment&bankid=' + bankid;
                                    }
                                    else
                                        window.location.href = '/Enrollment/enrollmentsummary?bankid=' + bankid;
                                }
                            }
                            else {
                                if ($('#entityid').val() != $('#myentityid').val()) {
                                    window.location.href = '/Enrollment/enrollmentsummary?Id=' + UserId + '&entitydisplayid=' + $('#myentitydisplayid').val() + '&ParentId=' + $('#myparentid').val() + '&entityid=' + $('#myentityid').val() + '&ptype=enrollment&bankid=' + bankid;
                                }
                                else
                                    window.location.href = '/Enrollment/enrollmentsummary?bankid=' + bankid;
                            }
                        }
                    }
                    else {
                        if ($('#sitea55334d1-3960-44c4-8cf1-e3ba9901f2be').length > 0) {
                            if ($('#sitea55334d1-3960-44c4-8cf1-e3ba9901f2be').hasClass('done')) {
                                if ($('#entityid').val() != $('#myentityid').val()) {
                                    window.location.href = '/Enrollment/enrollmentsummary?Id=' + UserId + '&entitydisplayid=' + $('#myentitydisplayid').val() + '&ParentId=' + $('#myparentid').val() + '&entityid=' + $('#myentityid').val() + '&ptype=enrollment&bankid=' + bankid;
                                }
                                else
                                    window.location.href = '/Enrollment/enrollmentsummary?bankid=' + bankid;
                            }
                        }
                        else {
                            if ($('#entityid').val() != $('#myentityid').val()) {
                                window.location.href = '/Enrollment/enrollmentsummary?Id=' + UserId + '&entitydisplayid=' + $('#myentitydisplayid').val() + '&ParentId=' + $('#myparentid').val() + '&entityid=' + $('#myentityid').val() + '&ptype=enrollment&bankid=' + bankid;
                            }
                            else
                                window.location.href = '/Enrollment/enrollmentsummary?bankid=' + bankid;
                        }
                    }
                }
            }
        }
    });
}

function saveACH(type) {
    ResetErrorSuccess();
    if (Id == '' || Id == '00000000-0000-0000-0000-000000000000') {
        error.show();
        error.append('<p> Please save Payment Options. </p>');
        return;
    }

    var _continue = true;

    if ($('#txtNameofAccount').val().trim() == '') {
        $('#txtNameofAccount').addClass("error_msg");
        $('#txtNameofAccount').attr('title', 'Please enter Name on Account');
        _continue = false;
    }
    else {
        $('#txtNameofAccount').removeClass("error_msg");
        $('#txtNameofAccount').attr('title', '');
    }

    if ($('#txtBankName').val().trim() == '') {
        $('#txtBankName').addClass("error_msg");
        $('#txtBankName').attr('title', 'Please enter Bank Name');
        _continue = false;
    }
    else {
        $('#txtBankName').removeClass("error_msg");
        $('#txtBankName').attr('title', '');
    }

    if ($('#txtRTN').val().trim() == '') {
        $('#txtRTN').addClass("error_msg");
        $('#txtRTN').attr('title', 'Please enter RTN');
        _continue = false;
    }
    else {
        $('#txtRTN').removeClass("error_msg");
        $('#txtRTN').attr('title', '');
    }

    if ($('#txtConfirmRTN').val().trim() == '') {
        $('#txtConfirmRTN').addClass("error_msg");
        $('#txtConfirmRTN').attr('title', 'Please enter Confirm RTN');
        _continue = false;
    }
    else {
        $('#txtConfirmRTN').removeClass("error_msg");
        $('#txtConfirmRTN').attr('title', '');
    }

    if ($('#txtBankAccount').val().trim() == '') {
        $('#txtBankAccount').addClass("error_msg");
        $('#txtBankAccount').attr('title', 'Please enter Enter Bank Account Number');
        _continue = false;
    }
    else {
        $('#txtBankAccount').removeClass("error_msg");
        $('#txtBankAccount').attr('title', '');
    }

    if ($('#txtConfirmBankAccount').val().trim() == '') {
        $('#txtConfirmBankAccount').addClass("error_msg");
        $('#txtConfirmBankAccount').attr('title', 'Please enter Enter Confirm Bank Account Number');
        _continue = false;
    }
    else {
        $('#txtConfirmBankAccount').removeClass("error_msg");
        $('#txtConfirmBankAccount').attr('title', '');
    }

    if ($('input[name="AccountType"]:checked').length == 0) {
        $('#dvAccounttype').addClass("error_msg");
        $('#dvAccounttype').attr('title', 'Please select');
        _continue = false;
    }
    else {
        $('#dvAccounttype').removeClass("error_msg");
        $('#dvAccounttype').attr('title', '');
    }

    if (!$('#chkAughorizeACH').prop('checked')) {
        $('#dvauthorize').addClass("error_msg");
        $('#dvauthorize').attr('title', 'Please select');
        _continue = false;
    }
    else {
        $('#dvauthorize').removeClass("error_msg");
        $('#dvauthorize').attr('title', '');
    }


    var rtn = $('#txtRTN').val().trim();
    var cnfrtn = $('#txtConfirmRTN').val().trim();
    var bnkact = $('#txtBankAccount').val().trim();
    var cnfbnkact = $('#txtConfirmBankAccount').val().trim();

    if (rtn != cnfrtn) {
        $('#txtConfirmRTN').addClass("error_msg");
        $('#txtConfirmRTN').attr('title', 'Please enter correct RTN');
        _continue = false;
    }
    else {
        $('#txtConfirmRTN').removeClass("error_msg");
        $('#txtConfirmRTN').attr('title', '');
    }

    if (bnkact != cnfbnkact) {
        $('#txtConfirmBankAccount').addClass("error_msg");
        $('#txtConfirmBankAccount').attr('title', 'Please enter correct Bank Account Number');
        _continue = false;
    }
    else {
        $('#txtConfirmBankAccount').removeClass("error_msg");
        $('#txtConfirmBankAccount').attr('title', '');
    }

    var isValidrtn = checkABA(rtn);
    if (!isValidrtn) {
        $('#txtRTN').addClass("error_msg");
        $('#txtRTN').attr('title', 'Please enter valid RTN');
        _continue = false;
    }
    else {
        $('#txtConfirmRTN').removeClass("error_msg");
        $('#txtConfirmRTN').attr('title', '');
    }

    if (!_continue) {
        error.show();
        error.append('<p> Please correct error(s). </p>');
        return;
    }

    if (bankDetails.Availble && bankDetails.BankName != $('#txtBankName').val().trim() && $('#sitea55334d1-3960-44c4-8cf1-e3ba9901f2be').length > 0) {
        alert('Please note: The bank account changes just made will only change the bank account associated to this section.  If you need to update your Fee Deposit Bank Enrollment account on your bank application or your Add on Reimbursement bank account (if applicable), you must enter the new information in those sections.');
    }

    var UserId = $('#UserId').val();
    var EntityId = $('#entityid').val();

    if ($('#entityid').val() != $('#myentityid').val()) {
        UserId = $('#myid').val();
        EntityId = $('#myentityid').val();
    }

    var req = {};
    req.AccountName = $('#txtNameofAccount').val().trim();
    req.BankName = $('#txtBankName').val().trim();
    req.AccountNumber = bnkact;
    req.RTN = rtn;
    req.AccountType = $('#rbAccountTypeYes').prop('checked') ? 1 : 2;
    req.UserId = UserId;
    req.PaymentOptionId = Id;

    var bankid = getUrlVars()["bankid"];
    var ptype = getUrlVars()["ptype"];

    var Uri = '/api/CustomerPaymentOptions/SaveACHDetails';
    ajaxHelper(Uri, 'POST', req, false).done(function (data, status) {
        if (data) {
            success.show();
            success.append('<p> Bank Account details saved successfully</p>');
            SaveConfigStatusActive('done', bankid);
            $("html, body").animate({ scrollTop: 0 }, "slow");
            if (SiteType == 1) {
                $('#site0eda5d25-591c-4e01-a845-fb580572cff5').addClass('done');
                if (type == 1) {

                    if ($('#site0eda5d25-591c-4e01-a845-fb580572cff5').parent().next('li').length) {
                        var hf = $('#site0eda5d25-591c-4e01-a845-fb580572cff5').parent().next('li')[0].children[0].href;
                        window.location.href = hf;
                        return;
                    }
                    else if ($('#entityid').val() != $('#myentityid').val()) {
                        if (ptype == 'config') {
                            window.location.href = $('#site98a706d7-031f-4c5d-8cc4-d32cc7658b63').attr('href');
                            return;
                        }
                    }
                    else {
                        if ($('#item6df12cef-8bdb-4d84-9057-095590ea0a79 #site0eda5d25-591c-4e01-a845-fb580572cff5').length == 1) {
                            window.location.href = $('#site98a706d7-031f-4c5d-8cc4-d32cc7658b63').attr('href');
                            return;
                        }
                    }

                    if (IsonHold) {
                        if ($('#site0eda5d25-591c-4e01-a845-fb580572cff5').parent().next('li').length) {
                            var hf = $('#site0eda5d25-591c-4e01-a845-fb580572cff5').parent().next('li')[0].children[0].href;
                            window.location.href = hf;
                            return;
                        }
                        else
                            return;
                    }

                    if ($('#site0eda5d25-591c-4e01-a845-fb580572cfe8').length > 0) {
                        window.location.href = $('#site0eda5d25-591c-4e01-a845-fb580572cfe8').attr('href');
                    }
                    else {
                        if ($('#site0feeb0fe-d0e7-4370-8733-dd5f7d2041fc').length > 0) {
                            if ($('#site0feeb0fe-d0e7-4370-8733-dd5f7d2041fc').hasClass('done') || IsEnrollment) {
                                if ($('#sitea55334d1-3960-44c4-8cf1-e3ba9901f2be').length > 0) {
                                    if ($('#site0eda5d25-591c-4e01-a845-fb580572cff5').hasClass('done')) {
                                        if ($('#entityid').val() != $('#myentityid').val()) {
                                            window.location.href = '/Enrollment/enrollmentsummary?Id=' + UserId + '&entitydisplayid=' + $('#myentitydisplayid').val() + '&ParentId=' + $('#myparentid').val() + '&entityid=' + $('#myentityid').val() + '&ptype=enrollment&bankid=' + bankid;
                                        }
                                        else
                                            window.location.href = '/Enrollment/enrollmentsummary?bankid=' + bankid;
                                    }
                                }
                                else {
                                    if ($('#entityid').val() != $('#myentityid').val()) {
                                        window.location.href = '/Enrollment/enrollmentsummary?Id=' + UserId + '&entitydisplayid=' + $('#myentitydisplayid').val() + '&ParentId=' + $('#myparentid').val() + '&entityid=' + $('#myentityid').val() + '&ptype=enrollment&bankid=' + bankid;
                                    }
                                    else
                                        window.location.href = '/Enrollment/enrollmentsummary?bankid=' + bankid;
                                }
                            }
                        }
                        else {
                            if ($('#sitea55334d1-3960-44c4-8cf1-e3ba9901f2be').length > 0) {
                                if ($('#site0eda5d25-591c-4e01-a845-fb580572cff5').hasClass('done')) {
                                    if ($('#entityid').val() != $('#myentityid').val()) {
                                        window.location.href = '/Enrollment/enrollmentsummary?Id=' + UserId + '&entitydisplayid=' + $('#myentitydisplayid').val() + '&ParentId=' + $('#myparentid').val() + '&entityid=' + $('#myentityid').val() + '&ptype=enrollment&bankid=' + bankid;
                                    }
                                    else
                                        window.location.href = '/Enrollment/enrollmentsummary?bankid=' + bankid;
                                }
                            }
                            else {
                                if ($('#entityid').val() != $('#myentityid').val()) {
                                    window.location.href = '/Enrollment/enrollmentsummary?Id=' + UserId + '&entitydisplayid=' + $('#myentitydisplayid').val() + '&ParentId=' + $('#myparentid').val() + '&entityid=' + $('#myentityid').val() + '&ptype=enrollment&bankid=' + bankid;
                                }
                                else
                                    window.location.href = '/Enrollment/enrollmentsummary?bankid=' + bankid;
                            }
                        }
                    }
                }
            }
            else if (SiteType == 2) {
                $('#site0eda5d25-591c-4e01-a845-fb580572cfe8').addClass('done');
                if (type == 1) {

                    if ($('#site0eda5d25-591c-4e01-a845-fb580572cfe8').parent().next('li').length) {
                        var hf = $('#site0eda5d25-591c-4e01-a845-fb580572cfe8').parent().next('li')[0].children[0].href;
                        window.location.href = hf;
                        return;
                    }
                    else if ($('#entityid').val() != $('#myentityid').val()) {
                        if (ptype == 'config') {
                            window.location.href = $('#site98a706d7-031f-4c5d-8cc4-d32cc7658b63').attr('href');
                            return;
                        }
                    }
                    else {
                        if ($('#item6df12cef-8bdb-4d84-9057-095590ea0a79 #site0eda5d25-591c-4e01-a845-fb580572cfe8').length == 1) {
                            window.location.href = $('#site98a706d7-031f-4c5d-8cc4-d32cc7658b63').attr('href');
                            return;
                        }
                    }

                    if (IsonHold) {
                        return;
                    }

                    if ($('#site0feeb0fe-d0e7-4370-8733-dd5f7d2041fc').length > 0)
                        if (!$('#site0feeb0fe-d0e7-4370-8733-dd5f7d2041fc').hasClass('done') && !IsEnrollment)
                            return;

                    if ($('#site0eda5d25-591c-4e01-a845-fb580572cff5').length > 0) {
                        if ($('#site0eda5d25-591c-4e01-a845-fb580572cff5').hasClass('done')) {
                            if ($('#sitea55334d1-3960-44c4-8cf1-e3ba9901f2be').length > 0) {
                                if ($('#sitea55334d1-3960-44c4-8cf1-e3ba9901f2be').hasClass('done')) {
                                    if ($('#entityid').val() != $('#myentityid').val()) {
                                        window.location.href = '/Enrollment/enrollmentsummary?Id=' + UserId + '&entitydisplayid=' + $('#myentitydisplayid').val() + '&ParentId=' + $('#myparentid').val() + '&entityid=' + $('#myentityid').val() + '&ptype=enrollment&bankid=' + bankid;
                                    }
                                    else
                                        window.location.href = '/Enrollment/enrollmentsummary?bankid=' + bankid;
                                }
                            }
                            else {
                                if ($('#entityid').val() != $('#myentityid').val()) {
                                    window.location.href = '/Enrollment/enrollmentsummary?Id=' + UserId + '&entitydisplayid=' + $('#myentitydisplayid').val() + '&ParentId=' + $('#myparentid').val() + '&entityid=' + $('#myentityid').val() + '&ptype=enrollment&bankid=' + bankid;
                                }
                                else
                                    window.location.href = '/Enrollment/enrollmentsummary?bankid=' + bankid;
                            }
                        }
                    }
                    else {
                        if ($('#sitea55334d1-3960-44c4-8cf1-e3ba9901f2be').length > 0) {
                            if ($('#sitea55334d1-3960-44c4-8cf1-e3ba9901f2be').hasClass('done')) {
                                if ($('#entityid').val() != $('#myentityid').val()) {
                                    window.location.href = '/Enrollment/enrollmentsummary?Id=' + UserId + '&entitydisplayid=' + $('#myentitydisplayid').val() + '&ParentId=' + $('#myparentid').val() + '&entityid=' + $('#myentityid').val() + '&ptype=enrollment&bankid=' + bankid;
                                }
                                else
                                    window.location.href = '/Enrollment/enrollmentsummary?bankid=' + bankid;
                            }
                        }
                        else {
                            if ($('#entityid').val() != $('#myentityid').val()) {
                                window.location.href = '/Enrollment/enrollmentsummary?Id=' + UserId + '&entitydisplayid=' + $('#myentitydisplayid').val() + '&ParentId=' + $('#myparentid').val() + '&entityid=' + $('#myentityid').val() + '&ptype=enrollment&bankid=' + bankid;
                            }
                            else
                                window.location.href = '/Enrollment/enrollmentsummary?bankid=' + bankid;
                        }
                    }
                }
            }
        }
    });
}

function epiration(event) {

    if (event.which != 99 && event.which != 8 && event.which != 0 && isNaN(String.fromCharCode(event.which)) && event.which != 47) {
        event.preventDefault(); //stop characters from entering input
    }
}

function goPrev() {
    $('#a_po').click();
}