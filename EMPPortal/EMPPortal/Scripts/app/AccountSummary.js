$(window).load(function () {
    getEfileData();
    getBalanceData();
})
function getEfileData() {
    var UserId = $('#UserId').val();
    var EntityId = $('#entityid').val();
    var bankid = getUrlVars()["bankid"];

    if ($('#entityid').val() != $('#myentityid').val()) {
        UserId = $('#myid').val();
        EntityId = $('#myentityid').val();
    }

    var custmorUri = '/api/CustomerPaymentOptions/GetCustomerPaymentInfo';
    if (UserId != '' && UserId != null && UserId != '00000000-0000-0000-0000-000000000000')
        ajaxHelper(custmorUri + '?UserId=' + UserId + '&EntityId=' + EntityId + '&SiteType=1&BankId=00000000-0000-0000-0000-000000000000', 'GET').done(function (data) {

            if (data.status) {
                Id = data.Id;
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
                            $("select[name=ddl_state_efilecc] option[value=" + data.CreditCard.StateId + "]").attr('selected', 'selected');
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
                    $('#dbBankSubQuestion').show();
                    if (data.IsSameBankAccount == 1)
                        $('#rbBankProdYes').prop('checked', true);
                    else
                        $('#rbBankProdNo').prop('checked', true);

                    if (data.ACH) {
                        if (data.ACH.status) {
                            $('#txtNameofAccount_efile').val(data.ACH.AccountName);
                            $('#txtBankName_efile').val(data.ACH.BankName);
                            $('#txtRTN, #txtConfirmRTN').val(data.ACH.RTN);
                            $('#txtBankAccount, #txtConfirmBankAccount').val(data.ACH.AccountNumber);
                            if (data.ACH.AccountType == 1)
                                $('#rbAccountTypeYes_efile').prop('checked', true);
                            else if (data.ACH.AccountType == 2)
                                $('#rbAccountTypeNo_efile').prop('checked', true);
                            $('#chkAughorizeACH').prop('checked', true);
                        }
                        else if (data.IsSameBankAccount == 1) {
                            var bankURI = '/api/CustomerPaymentOptions/GetCustomerBankDetails?UserId=' + UserId + '&EntityId=' + EntityId + '&CustId=' + UserId + '&BankId=00000000-0000-0000-0000-000000000000';
                            ajaxHelper(bankURI, 'GET').done(function (data) {
                                if (data)
                                    if (data.Result.status) {
                                        $('#txtNameofAccount').val(data.Result.AccountName);
                                        $('#txtBankName').val(data.Result.BankName);
                                        $('#txtRTN, #txtConfirmRTN').val(data.Result.RTN);
                                        $('#txtBankAccount, #txtConfirmBankAccount').val(data.Result.AccountNumber);
                                        if (data.Result.AccountType == 1)
                                            $('#rbAccountTypeYes').prop('checked', true);
                                        else if (data.Result.AccountType == 2)
                                            $('#rbAccountTypeYes').prop('checked', true);
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
            }
            else {
                $('#liefileCC').hide();
                $('#liefileBA').hide();
            }
        });
}

function getBalanceData() {
    var UserId = $('#UserId').val();
    var EntityId = $('#entityid').val();
    var bankid = getUrlVars()["bankid"];

    if ($('#entityid').val() != $('#myentityid').val()) {
        UserId = $('#myid').val();
        EntityId = $('#myentityid').val();
    }

    var custmorUri = '/api/CustomerPaymentOptions/GetCustomerPaymentInfo';
    if (UserId != '' && UserId != null && UserId != '00000000-0000-0000-0000-000000000000')
        ajaxHelper(custmorUri + '?UserId=' + UserId + '&EntityId=' + EntityId + '&SiteType=2&BankId=00000000-0000-0000-0000-000000000000', 'GET').done(function (data) {

            if (data.status) {
                Id = data.Id;
                if (data.PaymentType == 1) {
                    $('#rbefileCC_bal').prop('checked', true);
                    $('#dbBankSubQuestion_bal').hide();
                    $('#liefileCC_bal').show();
                    $('#liefileBA_bal').hide();
                    if (data.CreditCard) {
                        if (data.CreditCard.status) {
                            $('#txtCardholderName_bal').val(data.CreditCard.CardHolderName);
                            $('#txtBillingAddress_bal').val(data.CreditCard.Address);
                            $('#txtCardNumber_bal').val(data.CreditCard.CardNumber);
                            $('#txtExpiration_bal').val(data.CreditCard.Expiration);
                            $('#txtZip_bal').val(data.CreditCard.ZipCode);
                            $("select[name=ddlstate_bal] option[value=" + data.CreditCard.StateId + "]").attr('selected', 'selected');
                            $('#txtCity_bal').val(data.CreditCard.City);
                            if (data.CreditCard.CardType == 1)
                                $('#rbmastercard_bal').prop('checked', true);
                            else if (data.CreditCard.CardType == 2)
                                $('#rbvisa_bal').prop('checked', true);
                            else if (data.CreditCard.CardType == 3)
                                $('#rbamericanexpress_bal').prop('checked', true);
                            $('#chkAughorizeCC_bal').prop('checked', true);
                        }
                    }
                }
                else if (data.PaymentType == 2) {
                    $('#rbefileBA_bal').prop('checked', true);
                    $('#liefileCC_bal').hide();
                    $('#liefileBA_bal').show();
                    $('#dbBankSubQuestion_bal').show();
                    if (data.IsSameBankAccount == 1)
                        $('#rbBankProdYes_bal').prop('checked', true);
                    else
                        $('#rbBankProdNo_bal').prop('checked', true);

                    if (data.ACH) {
                        if (data.ACH.status) {
                            $('#txtNameofAccount_bal').val(data.ACH.AccountName);
                            $('#txtBankName_bal').val(data.ACH.BankName);
                            $('#txtRTN_bal, #txtConfirmRTN_bal').val(data.ACH.RTN);
                            $('#txtBankAccount_bal, #txtConfirmBankAccount_bal').val(data.ACH.AccountNumber);
                            if (data.ACH.AccountType == 1)
                                $('#rbAccountTypeYes_bal').prop('checked', true);
                            else if (data.ACH.AccountType == 2)
                                $('#rbAccountTypeNo_bal').prop('checked', true);
                            $('#chkAughorizeACH_bal').prop('checked', true);
                        }
                        else if (data.IsSameBankAccount == 1) {
                            var bankURI = '/api/CustomerPaymentOptions/GetCustomerBankDetails?UserId=' + UserId + '&EntityId=' + EntityId + '&CustId=' + UserId + '&BankId=00000000-0000-0000-0000-000000000000';
                            ajaxHelper(bankURI, 'GET').done(function (data) {
                                if (data)
                                    if (data.Result.status) {
                                        $('#txtNameofAccount_bal').val(data.Result.AccountName);
                                        $('#txtBankName_bal').val(data.Result.BankName);
                                        $('#txtRTN_bal, #txtConfirmRTN_bal').val(data.Result.RTN);
                                        $('#txtBankAccount_bal, #txtConfirmBankAccount_bal').val(data.Result.AccountNumber);
                                        if (data.Result.AccountType == 1)
                                            $('#rbAccountTypeYes_bal').prop('checked', true);
                                        else if (data.Result.AccountType == 2)
                                            $('#rbAccountTypeYes_bal').prop('checked', true);
                                        else
                                            $('input[name="AccountType_bal"]').prop('checked', false);
                                    }
                            });
                        }
                    }
                }
                $('#tb_feeSummary_bal').html('');
                $.each(data.Fees, function (item, value) {
                    $('#tb_feeSummary_bal').append('<tr><td>' + value.Fee + '</td><td><span>$</span> <span class="hdnTranFee">' + value.Amount + '</span></td></tr>');
                })
            }
            else {
                $('#liefileCC_bal').hide();
                $('#liefileBA_bal').hide();
            }
        });
}