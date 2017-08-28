var success = $('#success');
var error = $('#error');
ResetSuccessError();

function GetCustomerInfomationForMainOffice() {

    var Id = '';
    if (getUrlVars()["Id"])
        Id = getUrlVars()["Id"];
    else
        Id = $('#UserId').val();

    var url = '/api/CustomerInformation';
    if (Id != '' && Id != null && Id != '00000000-0000-0000-0000-000000000000') {
        ajaxHelper(url + '?id=' + Id, 'GET').done(function (data) {
            $('#spCompanyName').text(data["CompanyName"]);
            $('#spBusinessOwnerName').text(data["BusinessOwnerFirstName"]);
            $('#spBusinessOwnerLastName').text(data["BusinessOwnerLastName"]);
            $('#spPhysicalAddress').text(data["PhysicalAddress1"]);
            $('#spCitystatezip').text(data["PhysicalCity"] + ', ' + data["PhysicalState"] + ' ' + data["PhysicalZipcode"]);
            $('#spOfficePhone').text(data["OfficePhone"]);
            $('#spAlternatePhone').text(data["AlternatePhone"]);
            $('#spPrimaryemail').text(data["Primaryemail"]);
            $('#spSupportemail').text(data["SupportNotificationemail"]);
            $('#spAlternativeContact').text(data["AlternativeContact"]);
            if (data["PhoneType"] == null)
                $('#spphoneType').text('');
            else
                $('#spphoneType').text(data["PhoneType"]);
            $('#spContactType').text(data["ContactTitle"]);
            if (data["EROType"] == "SVB Sub-Office" || data["EROType"] == "Multi-Office Sub-Office" || data["EROType"] == "Single Office") {
                $('#trNoSO1').remove();
                $('#trNoSO2').remove();
            }
        });
    }


    var url1 = '/api/CustomerLoginInformation';
    ajaxHelper(url1 + '?Id=' + Id, 'GET').done(function (data) {
        $('#spEFIN').text(PadLeft(data["EFIN"],6));
        $('#spMasterident').text(data["MasterIdentifier"]); // Master ID
        $('#spMasterUserID').text(data["CrossLinkUserId"]); // User ID
        $('#spTransmissionpwd').text(data["CrossLinkPassword"]); // Transmission Password
        $('#spofficeportalusername').text(data["TaxOfficeUsername"]);
        $('#spoofficeportalpwd').text(data["TaxOfficePassword"]);
        // $('#spemppwd').spemppwd(data["EMPPassword"]);
    });
}

var _configId = '';

function fnGetEnrollOfficeConfig() {
    var UserId = $('#UserId').val();
    if ($('#entityid').val() != $('#myentityid').val()) {
        UserId = $('#myid').val();
    }



    //var ParentId = $('#parentid').val();

    //var Cid = getUrlVars()["Id"];
    //if (Cid) {
    //    UserId = Cid;
    //    //ParentId = localStorage.getItem("EnrollparentId");
    //    ParentId = $('#myparentid').val();
    //}

    var url = '/api/EnrollmentOfficeConfig/Self';
    $('#IsMainSiteTransmitTaxReturn').val(0);

    if (UserId != '' && UserId != null && UserId != '00000000-0000-0000-0000-000000000000') {
        ajaxHelper(url + '?id=' + UserId, 'GET').done(function (data) {

            if (data != null && data != '' && data != undefined) {
                _configId = data["Id"];

                //if (data["IsMainSiteTransmitTaxReturn"] == 'true' || data["IsMainSiteTransmitTaxReturn"] == 'True' || data["IsMainSiteTransmitTaxReturn"] == true) {
                //    $('#IsMainSiteTransmitTaxReturn1').html('Yes');
                //    $('#IsMainSiteTransmitTaxReturn').val(1);

                //} else if (data["IsMainSiteTransmitTaxReturn"] == 'false' || data["IsMainSiteTransmitTaxReturn"] == 'False' || data["IsMainSiteTransmitTaxReturn"] == false) {
                //    $('#IsMainSiteTransmitTaxReturn1').html('No');
                //    $('#IsMainSiteTransmitTaxReturn').val(0);
                //}


                $('#NoofTaxProfessionals').val(data["NoofTaxProfessionals"]);
                if (data["IsSoftwareOnNetwork"] == 'true' || data["IsSoftwareOnNetwork"] == 'True' || data["IsSoftwareOnNetwork"] == true) {
                    $('#IsSoftwareOnNetwork1').prop('checked', true);

                } else if (data["IsSoftwareOnNetwork"] == 'false' || data["IsSoftwareOnNetwork"] == 'False' || data["IsSoftwareOnNetwork"] == false) {
                    $('#IsSoftwareOnNetwork2').prop('checked', true);
                }
                $('#NoofComputers').val(data["NoofComputers"]);
                if (data["PreferredLanguage"] == 'true' || data["PreferredLanguage"] == 'True' || data["PreferredLanguage"] == true || data["PreferredLanguage"] == 1) {
                    $('#PreferredLanguage1').prop('checked', true);

                } else if (data["PreferredLanguage"] == 'false' || data["PreferredLanguage"] == 'False' || data["PreferredLanguage"] == false || data["PreferredLanguage"] == 2) {
                    $('#PreferredLanguage2').prop('checked', true);
                }
            }

        });

        //var Id = _configId;
        //if (Id == '' || Id == null || Id == '00000000-0000-0000-0000-000000000000') {
        //    fnGetEnrollOfficeMainConfig(ParentId);
        //}
    }
}

function fnGetEnrollOfficeMainConfig(ParentId) {

    var url = '/api/EnrollmentOfficeConfig/MainConfig';
    if (ParentId != '' && ParentId != null && ParentId != '00000000-0000-0000-0000-000000000000') {
        ajaxHelper(url + '?id=' + ParentId, 'GET').done(function (data) {

            if (data["IsMainSiteTransmitTaxReturn"] == 'true' || data["IsMainSiteTransmitTaxReturn"] == 'True' || data["IsMainSiteTransmitTaxReturn"] == true) {
                $('#IsMainSiteTransmitTaxReturn1').html('Yes');
                $('#IsMainSiteTransmitTaxReturn').val(1);

            }
            else if (data["IsMainSiteTransmitTaxReturn"] == 'false' || data["IsMainSiteTransmitTaxReturn"] == 'False' || data["IsMainSiteTransmitTaxReturn"] == false) {
                $('#IsMainSiteTransmitTaxReturn1').html('No');
                $('#IsMainSiteTransmitTaxReturn').val(0);
            }

        });
    }
}

function fnGetEnrollAffiliateConfig() {

    //  getAffiliateEnrollSummary($('#divAffiliates'));
    //  getAffiliateEnrollSummary_Sub($('#divAffiliates'));
    var Id = $('#UserId').val();
    var ParentId = $('#parentid').val();

    var Cid = getUrlVars()["Id"];
    if (Cid) {
        Id = Cid;
        ParentId = $('#myparentid').val(); //localStorage.getItem("EnrollparentId");
    }

    var IsMainConfig = false;
    var url = '/api/EnrollmentAffiliateConfig/Self';
    if (Id != '' && Id != null && Id != '00000000-0000-0000-0000-000000000000') {
        ajaxHelper(url + '?id=' + Id, 'GET').done(function (data) {
            if (data != null && data != '' && data != undefined) {
                $.each(data, function (coIndex, c) {
                    $('#chkAE' + c["AffiliateProgramId"]).attr('checked', 'checked');
                    var chktext = $('#chkAE' + c["AffiliateProgramId"]).attr('chkname');
                    if (chktext == 'iProtect') {
                        $('#diviProtect').show();
                    }
                    $('#chkAE' + c["AffiliateProgramId"] + '_charge').val(c["AffiliateProgramCharge"]);
                });
            } else {
                IsMainConfig = true;
            }
        });

        //if (IsMainConfig) {
        //    fnGetEnrollAffiliateMainConfig(ParentId);
        //}
    }
}

function fnGetEnrollAffiliateMainConfig(ParentId) {
    var url = '/api/EnrollmentAffiliateConfig/MainConfig';
    if (ParentId != '' && ParentId != null && ParentId != '00000000-0000-0000-0000-000000000000') {
        ajaxHelper(url + '?id=' + ParentId, 'GET').done(function (data) {
            $.each(data, function (coIndex, c) {
                $('#chkAE' + c["AffiliateProgramId"]).attr('checked', 'checked');
                var chktext = $('#chkAE' + c["AffiliateProgramId"]).attr('chkname');
                if (chktext == 'iProtect') {
                    $('#diviProtect').show();
                }
            });
        });
    }
}

function chkBelowinfo(e) {
    $('#diviProtect').hide();
    var bval = false;
    $('.chkAEAffiliate').each(function (e) {
        var id = $(this).attr('Id');
        var isChecked = $(this).is(':checked');
        var chkname = $(this).attr('chkname');
        if (chkname == 'iProtect' && isChecked) {
            bval = true;
        }
    });

    if (bval) {
        $('#diviProtect').show();
    }
}

function EnrollmentClick() {
    setTimeout(function () {
        $('#dv_saveBank').hide();
        $('#dvefinowner').addClass('active in');
    })
}

function SubmitEnrollment() {
    var CustomerId = $('#UserId').val();
    var UserId = $('#UserId').val();
    var Cid = getUrlVars()["Id"];
    if (Cid) {
        CustomerId = Cid;
    }

    var bankid = getUrlVars()["bankid"];
    ResetSuccessError();
    ResetEnrollSummary();

    var _showapproved = false;

    $.blockUI({ message: '<img src="../../content/images/loading-img.gif"/>' });
    //setTimeout(function () {
        var appurl = '/api/EnrollmentBankSelection/getApprovedBanks?CustomerId=' + CustomerId + '&BankId=' + bankid;
        ajaxHelper(appurl, 'GET', null, false).done(function (res) {
            if (res) {
                if (res.length > 1) {
                    _showapproved = true;

                    $('#tbd_prefer').empty();
                    var tdata = '';
                    $.each(res, function (item, value) {
                        tdata += '<tr><td>' + value.BankName + '</td><td><input type="radio" name="isprefer" id="' + value.BankId + '"/></td></tr>'
                    })
                    $('#tbd_prefer').html(tdata);
                    $('#popupPrefferedBank').modal('show');
                }
            }
            $.unblockUI();
        })
    //}, 500);
    
    if (!_showapproved) {
        SubmitBankEnrollmenet();
    }
}

function SubmitBankEnrollmenet() {

    var bankid = getUrlVars()["bankid"];
    var UserId = $('#UserId').val();
    var CustomerId = $('#UserId').val();
    var Cid = getUrlVars()["Id"];
    if (Cid) {
        CustomerId = Cid;
    }
    $('#popupEnrollStatus').modal('show');

    var IsStaging = false;
    if (getUrlVars()["Staging"])
        IsStaging = true;

    var updated = false;
    if (IsStaging) {
        var UpdateURI = '/api/EnrollmentBankSelection/UpdateAddon?CustomerId=' + CustomerId + '&UserId=' + UserId + '&BankId=' + bankid;
        ajaxHelper(UpdateURI, 'POST', null, false).done(function (data) {
            if (data)
                updated = true;
        })
    }

    //var urlisvallid = '/api/EnrollmentBankSelection/IsValidRecord?CustomerId=' + CustomerId;
    //ajaxHelper(urlisvallid, 'POST').done(function (data) {
    //    if (data) {

    if (IsStaging && !updated) {
        $("html, body").animate({ scrollTop: 0 }, "slow");
        error.show();
        error.append('<p> Submission failed.  Please contact support. </p>');
        $('#popupEnrollStatus').modal('hide');
        return;
    }

    if ($('input[name="isprefer"]:checked').length > 0)
        var preferbankid = $('input[name="isprefer"]:checked').attr('id');
    else
        preferbankid = '00000000-0000-0000-0000-000000000000';

    var url = '/api/EnrollmentBankSelection/saveEnrollmenttoService?CustomerId=' + CustomerId + '&UserId=' + UserId + '&BankId=' + bankid + '&Prefer=' + preferbankid;
    ajaxHelper(url, 'POST').done(function (data) {
        if (data) {
            
            if (data != 1) {
                if (data == 0) {
                    $("html, body").animate({ scrollTop: 0 }, "slow");
                    error.show();
                    error.append('<p> Bank is Inactive </p>');
                    $('#popupEnrollStatus').modal('hide');
                    return;
                }
                else {
                    $("html, body").animate({ scrollTop: 0 }, "slow");
                    error.show();
                    error.append('<p> Submission failed.  Please contact support. </p>');
                    $('#popupEnrollStatus').modal('hide');
                    return;
                }
            }


            $('#p_enrollsave').addClass('enroll-completed');
            $('#sp_enrollsave').text('Saving into DB');
            $('#sp_enrollconnect').text($('#sp_enrollconnect').text() + '...');

            setTimeout(function () {
                $('#p_enrollconnect').addClass('enroll-completed');
                $('#sp_enrollconnect').text('Connecting to Crosslink');
                $('#sp_enrollsavingtoxlink').text($('#sp_enrollsavingtoxlink').text() + '...');
            }, 1000);

            UpdateOfficeManagement(CustomerId);
            var urlsub = '/api/EnrollmentBankSelection/SubmitBankApptoXlink?CustomerId=' + CustomerId + '&UserId=' + UserId + '&bankid=' + bankid;
            ajaxHelper(urlsub, 'POST').done(function (data) {
                if (data) {
                    if (data.Status) {

                        $('#site98a706d7-031f-4c5d-8cc4-d32cc7658b69').addClass('done');
                        $('#p_enrollsavingtoxlink').addClass('enroll-completed');
                        $("html, body").animate({ scrollTop: 0 }, "slow");
                        success.show();
                        success.append('<p> Enrollment details submitted successfully</p>');
                        $('#popupEnrollStatus').modal('hide');
                        $('#btnBankEnrollSubmit').hide();
                        SaveConfigStatusActive('done', bankid);
                        UpdateOfficeManagement(CustomerId);
                    }
                    else {
                        $("html, body").animate({ scrollTop: 0 }, "slow");
                        error.show();
                        error.append('<p> Bank application was not created. </p>');
                        $.each(data.Messages, function (item, value) {
                            error.append('<p> ' + value + ' </p>');
                        })
                        error.append('<p> Please contact support if assistance is required. </p>');
                        $('#popupEnrollStatus').modal('hide');
                    }
                }
            });
            //$("html, body").animate({ scrollTop: 0 }, "slow");
            //success.show();
            //success.append('<p> Enrollment details submitted successfully</p>');
            //getConfigStatus();


            //  if ($('#entitydisplayid').val() != $('#myentitydisplayid').val()) {
            //        window.location.href = "/CustomerInformation/AllCustomerInfo";
            //    } else {
            //      window.location.href = "/enrollment/officeinformation";
            //    }

            // window.location.href = window.location.href.replace('/enrollmentsummary', '/OfficeInformation');
        }
        else {
            $("html, body").animate({ scrollTop: 0 }, "slow");
            error.show();
            error.append('<p> Submission failed.  Please contact support. </p>');
            $('#popupEnrollStatus').modal('hide');
        }
    })


    //    }
    //    else {
    //        $("html, body").animate({ scrollTop: 0 }, "slow");
    //        error.show();
    //        error.append('<p> The record is Invalid. </p>');
    //        $('#popupEnrollStatus').modal('hide');
    //    }
    //})
}

function SavePrefer() {
    if ($('input[name="isprefer"]:checked').length <= 0) {
        alert('Please select a bank as prefer.');
        return;
    }

    $('#popupPrefferedBank').modal('hide');
    SubmitBankEnrollmenet();
}

function ResetSuccessError() {
    success.html('');
    success.hide();
    error.html('');
    error.hide();
}

function ResetEnrollSummary() {
    $('#p_enrollsave').removeClass('enroll-completed');
    $('#sp_enrollsave').text('Saving into DB...');
    $('#p_enrollconnect').removeClass('enroll-completed');
    $('#sp_enrollconnect').text('Connecting to Crosslink');
    $('#p_enrollsavingtoxlink').removeClass('enroll-completed');
    $('#sp_enrollsavingtoxlink').text('Submitting to Crosslink');
}

function checkAddon() {
    var entityid = $('#entityid').val();
    if (!entityid)
        entityid = $('#myentityid').val();
    if (entityid == $('#Entity_SO').val() || entityid == $('#Entity_SOME').val() || entityid == $('#Entity_SOME_SS').val()) {
        var Id = '';
        if (getUrlVars()["Id"])
            Id = getUrlVars()["Id"];
        else
            Id = $('#UserId').val();

        var bankid = getUrlVars()["bankid"];

        var feeuri = '/api/EnrollmentBankSelection/getAddonSelection?CustomerId=' + Id + '&bankid=' + bankid;
        ajaxHelper(feeuri, 'GET').done(function (res) {

            if (res) {

            }
            else
                $('#lifee').css('display', 'none');
        })
    }
}

function Exittab() {
    window.close();
}

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
        ajaxHelper(custmorUri + '?UserId=' + UserId + '&EntityId=' + EntityId + '&SiteType=1&BankId=' + bankid, 'GET').done(function (data) {

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
                            var bankURI = '/api/CustomerPaymentOptions/GetCustomerBankDetails?UserId=' + UserId + '&EntityId=' + EntityId + '&CustId=' + UserId + '&BankId=' + bankid;
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
        ajaxHelper(custmorUri + '?UserId=' + UserId + '&EntityId=' + EntityId + '&SiteType=2&BankId=' + bankid, 'GET').done(function (data) {

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
                            var bankURI = '/api/CustomerPaymentOptions/GetCustomerBankDetails?UserId=' + UserId + '&EntityId=' + EntityId + '&CustId=' + UserId + '&BankId=' + bankid;
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

function CancelSubmission() {

}