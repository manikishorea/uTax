function ArchiveMainSite() {
    var Id = $('#UserId').val();
    var url = '/api/Archive/MainSite';
    if (Id != '' && Id != null && Id != '00000000-0000-0000-0000-000000000000') {
        ajaxHelper(url + '?Id=' + Id, 'GET').done(function (data) {

           

            getFees($('#tdFeeFixed'), $('#tdFeeUser'));
            getArchiveUserBanks($('#divBankAddOn'), $('#divBankAddOn_MSO'), $('#divBankAddOn_trns'), $('#divBankAddOn_trns_MSO'));
            getBankAndQuestions($('#divBanks'));
            getAffiliateArchive($('#divAffiliates'));

            getArchiveMainOfficeInformation(data.MainOfficeConfigDTO);
            getArchiveSubSiteConfig(data.SubSiteConfigDTO);
            getArchiveFeeReimbersment(data.FeeReimburseConfigDTO);

            getArchiveFeeSetup(data.SubSiteFeeDTOs);

        });

    }
}


function getArchiveMainOfficeInformation(data) {

    if (data != null && data != '' && data != undefined) {
        $('#dvmainofficeinfo_nocontent')
           .hide()
           .html('');

        $('#ID').val(data["Id"]);
        if (data["IsSiteTransmitTaxReturns"] == true || data["IsSiteTransmitTaxReturns"] == 'true') {
            $('#rbTaxReturnYes').prop('checked', true);
            $("#dbsubquestions").show();
        }
        else if (data["IsSiteTransmitTaxReturns"] == false || data["IsSiteTransmitTaxReturns"] == 'false') {
            $('#rbTaxReturnNo').prop('checked', true);
            $("#dbsubquestions").hide();
        }

        if (data["IsSiteOfferBankProducts"] == true || data["IsSiteOfferBankProducts"] == 'true') {
            $('#rbBankProdYes').prop('checked', true);
        }
        else if (data["IsSiteOfferBankProducts"] == false || data["IsSiteOfferBankProducts"] == 'false') {
            $('#rbBankProdNo').prop('checked', true);
        }

        $('#TaxProfessionals').val(data["TaxProfessionals"]);

        if (data["IsSoftwarebeInstalledNetwork"] == true || data["IsSoftwarebeInstalledNetwork"] == 'true') {
            $('#rbsoftwareYes').prop('checked', true);
        }
        else if (data["IsSoftwarebeInstalledNetwork"] == false || data["IsSoftwarebeInstalledNetwork"] == 'false') {
            $('#rbsoftwareNo').prop('checked', true);
        }

        $('#computersoftware').val(data["ComputerswillruninSoftware"]);

        if (data["PreferredSupportLanguage"] == 1 || data["PreferredSupportLanguage"] == "1") {
            $('#rbsupportYes').prop('checked', true);
        }
        else {
            $('#rbsupportNo').prop('checked', true);
        }
    } else {

        $('#dvmainofficeinfo_content')
            .hide()
            .html('');

        $('#dvmainofficeinfo_nocontent').html('No Record Found');
    }
}

function getArchiveSubSiteConfig(data) {

    if (data != null && data != undefined && data != '') {

        $('#bankenrollment_nocontent')
           .hide()
           .html('');


        $('#Id').val(data["Id"]);
        $('#refId').val(data["refId"]);
        if (data["CanSubSiteLoginToEmp"] == true)
            $('#CanSubSiteLoginToEmp').prop('checked', true);
        else
            $('#CanSubSiteLoginToEmp').prop('checked', false);
        if (data["IsuTaxManageingEnrolling"] == "true" || data["IsuTaxManageingEnrolling"] == true || data["IsuTaxManageingEnrolling"] == "True") {
            $('#IsuTaxManageingEnrolling1').attr('checked', 'checked');
        } else if (data["IsuTaxManageingEnrolling"] == "false" || data["IsuTaxManageingEnrolling"] == false || data["IsuTaxManageingEnrolling"] == "False") {
            $('#IsuTaxManageingEnrolling2').attr('checked', 'checked');
        }

        if (data["IsuTaxPortalEnrollment"] == "true" || data["IsuTaxPortalEnrollment"] == true || data["IsuTaxPortalEnrollment"] == "True") {
            $('#IsuTaxPortalEnrollment1').attr('checked', 'checked');
        } else if (data["IsuTaxPortalEnrollment"] == "false" || data["IsuTaxPortalEnrollment"] == false || data["IsuTaxPortalEnrollment"] == "False") {
            $('#IsuTaxPortalEnrollment2').attr('checked', 'checked');
        }

        if (data["IsuTaxManageOnboarding"] == "true" || data["IsuTaxManageOnboarding"] == true || data["IsuTaxManageOnboarding"] == "True") {
            $('#IsuTaxManageOnboarding1').attr('checked', 'checked');
        } else if (data["IsuTaxManageOnboarding"] == "false" || data["IsuTaxManageOnboarding"] == false || data["IsuTaxManageOnboarding"] == "False") {
            $('#IsuTaxManageOnboarding2').attr('checked', 'checked');
        }

        if (data["IsuTaxCustomerSupport"] == "true" || data["IsuTaxCustomerSupport"] == true || data["IsuTaxCustomerSupport"] == "True") {
            $('#IsuTaxCustomerSupport1').attr('checked', 'checked');
        } else if (data["IsuTaxCustomerSupport"] == "false" || data["IsuTaxCustomerSupport"] == false || data["IsuTaxCustomerSupport"] == "False") {
            $('#IsuTaxCustomerSupport2').attr('checked', 'checked');
        }

        if (data["IsSubSiteEFINAllow"] == "true" || data["IsSubSiteEFINAllow"] == true || data["IsSubSiteEFINAllow"] == "True") {
            $('#IsSubSiteEFINAllow1').attr('checked', 'checked');
            $('#IsSubSiteEFINAllow2').attr('disabled', 'disabled');
        } else if (data["IsSubSiteEFINAllow"] == "false" || data["IsSubSiteEFINAllow"] == false || data["IsSubSiteEFINAllow"] == "False") {
            $('#IsSubSiteEFINAllow2').attr('checked', 'checked');
        }

        $('#NoofSupportStaff').val(data["NoofSupportStaff"]);

        var days = data["NoofDays"];

        if (days != null && days != '') {
            var splitdays = days.toString().split(',')

            $.each(splitdays, function (coIndex, c) {

                if (c == "1") {
                    $('#mon').prop('checked', true);
                }

                if (c == "2") {
                    $('#tue').prop('checked', true);
                }

                if (c == "3") {
                    $('#wed').prop('checked', true);
                }

                if (c == "4") {
                    $('#thu').prop('checked', true);
                }

                if (c == "5") {
                    $('#fri').prop('checked', true);
                }

                if (c == "6") {
                    $('#sat').prop('checked', true);
                }

                if (c == "7") {
                    $('#sun').prop('checked', true);
                }

                // $('#chk' + c["AffiliateProgramId"]).attr('checked', 'checked');

            });
        }

        $('#OpenHours').val(data["OpenHours"]);
        $('#CloseHours').val(data["CloseHours"]);
        $('#TimeZone').val(data["TimeZone"]);

        if (data["IsAutoEnrollAffiliateProgram"] == "true" || data["IsAutoEnrollAffiliateProgram"] == true || data["IsAutoEnrollAffiliateProgram"] == "True") {
            $('#IsAutoEnrollAffiliateProgram1').attr('checked', 'checked');
        } else if (data["IsAutoEnrollAffiliateProgram"] == "false" || data["IsAutoEnrollAffiliateProgram"] == false || data["IsAutoEnrollAffiliateProgram"] == "False") {
            $('#IsAutoEnrollAffiliateProgram2').attr('checked', 'checked');
        }

        $('#SubSiteTaxReturn').val(data["SubSiteTaxReturn"]);

        if (data["SubSiteTaxReturn"] == "1") {
            $('#SubSiteTaxReturn1').attr('checked', 'checked');
        } else if (data["SubSiteTaxReturn"] == "2") {
            $('#SubSiteTaxReturn2').attr('checked', 'checked');
        } else if (data["SubSiteTaxReturn"] == "3") {
            $('#SubSiteTaxReturn3').attr('checked', 'checked');
        }


        $.each(data.SubSiteAffiliateProgramDTOs, function (coIndex, c) {
            $('#chk' + c["AffiliateProgramId"]).attr('checked', 'checked');
        });

        $.each(data.SubSitBankQuestionDTOs, function (coIndex, c) {
            var bankid = c["BankId"];
            $('#chk' + bankid).attr('checked', 'checked');
            $('#divBankQuestions' + bankid).show();
            $('#chk' + c["QuestionId"]).attr('checked', 'checked');
        });

        visibleshowhide();
        mysupportInfo();
        chkBelowinfo();


    } else {

        $('input[type=checkbox][chkname=Versicom]').attr('checked', 'checked');
        $('input[type=checkbox][chkname=versicom]').attr('checked', 'checked');

        $('#bankenrollment_content')
            .hide()
            .html('');

        $('#bankenrollment_nocontent').html('No Record Found');
    }

}

function visibleshowhide() {
    $('#dvEnrollmentInfo').hide();
    if ($('#IsuTaxManageingEnrolling2').is(":checked")) {
        $('#dvEnrollmentInfo').show();
        $('#CanSubSiteLoginToEmp').attr('disabled', 'disabled');
    }
    else {
        $('#CanSubSiteLoginToEmp').removeAttr('disabled', 'disabled');
    }
}

function mysupportInfo() {
    $('#dvmysupport').hide();


    if ($('#IsuTaxCustomerSupport2').is(':checked')) {
        $('#dvmysupport').show();
    } else {
        $('#NoofSupportStaff')
            .removeClass("error_msg")
            .val('');

        $('#divSupportDays').removeClass("error_msg");
        $('input[type=checkbox].supportdays').attr('checked', false);
        $('input[type=checkbox].supportdays').prop('checked', false);

        $('#OpenHours')
            .val('')
            .removeClass("error_msg");

        $('#CloseHours')
             .val('')
              .removeClass("error_msg");
        $('#TimeZone')
            .val('')
            .removeClass("error_msg");
    }
}

function chkBelowinfo() {

    $('#dvaffiliateInfo').hide();

    var bval = false;
    $('.chkAffiliate').each(function (e) {
        var id = $(this).attr('Id');
        if ($('#' + id).is(":checked")) {
            bval = true;
            //return true;
        }
    });

    if (bval) {
        $('#dvaffiliateInfo').show();
    }
}

// Fee Setup

function getArchiveFeeSetup(data) {

    var entityid = $('#entityid').val();


    if (data == "" || data == null || data == undefined) {
        $('#subquestion_Service').hide();
        $('#subquestion_Trans').hide();
    }

    if (data != "" && data != null && data != undefined) {
        $('#dvFeeSteupConfig_nocontent')
            .html('')
            .hide();

        $.each(data, function (rowIndex, r) {

            $('#ID').val(r["Id"]);

            if (entityid != $('#Entity_SO').val()) {//entityid != "0676dfd0-da29-41e3-a262-81cb528b796c") {

                if (r["ServiceorTransmission"] == 1) {
                    var BankTrans = $('input[type=text].bankservice');
                    $.each(r.SubSiteBankFeesDTOs, function (indx, c) {

                        $('#input_' + c["BankID"]).val(c["AmountDSK"]);
                        $('#input_' + c["BankID"] + '_MSO').val(c["AmountMSO"]);

                    });

                    if (r["IsAddOnFeeCharge"] == true) {
                        $('#rbService_BankProductYes').prop('checked', true);
                    }
                    else {
                        $('#rbService_BankProductNo').prop('checked', true);
                        $('#subquestion_Service').hide();
                    }
                    if (r["IsSameforAll"] == true) {
                        $('#rbService_SubSiteYes').prop('checked', true);
                    }
                    else {
                        $('#rbService_SubSiteNo').prop('checked', true);
                    }
                    if (r["IsSubSiteAddonFee"] == true) {
                        $('#rbService_duringEnrollingYes').prop('checked', true);
                    }
                    else {
                        $('#rbService_duringEnrollingNo').prop('checked', true);
                    }
                }
                else {
                    var BankTrans = $('input[type=text].banktrans');

                    $.each(r.SubSiteBankFeesDTOs, function (indx, c) {
                        $('#input_trns_' + c["BankID"]).val(c["AmountDSK"]);
                        $('#input_trns_' + c["BankID"] + '_MSO').val(c["AmountMSO"]);

                    });

                    if (r["IsAddOnFeeCharge"] == true) {
                        $('#rbTrans_BankProductYes').prop('checked', true);
                    }
                    else {
                        $('#rbTrans_BankProductNo').prop('checked', true);
                        $('#subquestion_Trans').hide();
                    }
                    if (r["IsSameforAll"] == true) {
                        $('#rbTrans_SubSiteYes').prop('checked', true);
                    }
                    else {
                        $('#rbTrans_SubSiteNo').prop('checked', true);
                    }
                    if (r["IsSubSiteAddonFee"] == true) {
                        $('#rbTrans_duringEnrollingYes').prop('checked', true);
                    }
                    else {
                        $('#rbTrans_duringEnrollingNo').prop('checked', true);
                    }
                }
            }
        });
    } else {
        $('#dvFeeSteupConfig_content')
           .html('')
           .hide();

        $('#dvFeeSteupConfig_nocontent')
           .html('No Record Found');
    }
}

///// Fee Reimbersment

function getArchiveFeeReimbersment(data) {

    if (data != null && data != undefined && data != '') {

        $('#dvSvTfAddon11_nocontent')
            .html('')
            .hide();

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
    } else {

        $('#dvSvTfAddon11_nocontent')
            .html('No Record Found');

        $('#dvSvTfAddon11_content')
                    .html('')
                    .hide();
    }
}


////
function getArchiveUserBanks(container1, container2, container3, container4) {

    container1.html('');
    container2.html('');
    container3.html('');
    container4.html('');
    var UserId = $('#UserId').val();

    var ismsouser = $('#ismsouser').val();
    if ($('#myid').length > 0) {
        UserId = $('#myid').val();
    }
    //var custmorUri = '/api/dropdown/userbanks?UserId=' + UserId;

    var custmorUri = '/api/Archive/subsitebank?id=' + UserId;

    ajaxHelper(custmorUri, 'GET').done(function (data) {

        $.each(data, function (rowIndex, r) {

            var TotalBankProductFees = $('#TotalBankProductFees').val();
            var TotaleFileFees = $('#TotaleFileFees').val();

            var BankSVBDesktopFee = r["BankSVBDesktopFee"];
            var BankSVBMSOFee = r["BankSVBMSOFee"];

            var BankTranDesktopFee = r["BankTranDesktopFee"];
            var BankTranMSOFee = r["BankTranMSOFee"];

            var SubDesktopFee = 0;// r["SubDesktopFee"];
            var SubMSOFee = 0;// r["SubMSOFee"];

            if (TotalBankProductFees == '' || TotalBankProductFees == null) {
                TotalBankProductFees = 0;
            }

            if (TotaleFileFees == '' || TotaleFileFees == null) {
                TotaleFileFees = 0;
            }

            if (SubDesktopFee == '' || SubDesktopFee == null) {
                SubDesktopFee = 0;
            }

            if (SubMSOFee == '' || SubMSOFee == null) {
                SubMSOFee = 0;
            }

            BankSVBDesktopFee = Number(BankSVBDesktopFee) - (Number(TotalBankProductFees)); // + Number(SubDesktopFee)
            BankSVBMSOFee = Number(BankSVBMSOFee) - (Number(TotalBankProductFees)); //+ Number(SubMSOFee)

            BankTranDesktopFee = Number(BankTranDesktopFee);// - (Number(TotaleFileFees)); //+ Number(SubDesktopFee)
            BankTranMSOFee = Number(BankTranMSOFee);// - (Number(TotaleFileFees)); // + Number(SubMSOFee)

            BankSVBDesktopFee = BankSVBDesktopFee.toFixed(2);
            BankSVBMSOFee = BankSVBMSOFee.toFixed(2);
            BankTranDesktopFee = BankTranDesktopFee.toFixed(2);
            BankTranMSOFee = BankTranMSOFee.toFixed(2);

            var DocID = r["DocumentPath"];
            if (DocID != '' && DocID != null && DocID != undefined) {

                var docpath = EMPAdminWebAPI + '/' + DocID;

                if (r["ServiceorTransmission"] == 1 || r["ServiceorTransmission"] == '1') {

                    if (ismsouser == 'true' || ismsouser == true || ismsouser == "True") {
                        container1.append('<div class="col-md-4"><div class="form-group is-empty"><label>' + r["BankName"] + ' (Desktop : $' + BankSVBDesktopFee + ')  <a href="' + docpath + '" target="_blank" title="Download"> <i class="fa fa-arrow-circle-down"></i> </a></label><input  mainfee="' + r["DesktopFee"] + '"  subfee="' + SubDesktopFee + '" id="input_' + r["BankId"] + '" feetype="desktop"  maxfee="' + r["BankSVBDesktopFee"] + '"  class="form-control bankservice decimal svbfee" name="bankservice" type="text" bankid="' + r["BankId"] + '" maxlength="8" onkeyup="fnFeeSetupSVBFee(this)"></div></div>');
                        container2.append('<div class="col-md-4"><div class="form-group is-empty"><label>' + r["BankName"] + ' (MSO : $' + BankSVBMSOFee + ')  <a href="' + docpath + '" target="_blank" title="Download"> <i class="fa fa-arrow-circle-down"></i> </a></label><input  mainfee="' + r["MSOFee"] + '" subfee="' + SubMSOFee + '"  id="input_' + r["BankId"] + '_MSO"  feetype="mso" maxfee="' + r["BankSVBMSOFee"] + '"   class="form-control decimal svbfee" type="text" bankid="' + r["BankId"] + '" maxlength="8"  onkeyup="fnFeeSetupSVBFee(this)"></div></div>');
                    }
                    else {
                        container1.append('<div class="col-md-4"><div class="form-group is-empty"><label>' + r["BankName"] + ' (Desktop : $' + BankSVBDesktopFee + ')  <a href="' + docpath + '" target="_blank" title="Download"> <i class="fa fa-arrow-circle-down"></i> </a></label><input  mainfee="' + r["DesktopFee"] + '"  subfee="' + SubDesktopFee + '" id="input_' + r["BankId"] + '" feetype="desktop"  maxfee="' + r["BankSVBDesktopFee"] + '"  class="form-control bankservice decimal svbfee" name="bankservice" type="text" bankid="' + r["BankId"] + '" maxlength="8"  onkeyup="fnFeeSetupSVBFee(this)"></div></div>');
                    }

                } else {
                    if (ismsouser == 'true' || ismsouser == true || ismsouser == "True") {
                        container3.append('<div class="col-md-4"><div class="form-group is-empty"><label>' + r["BankName"] + ' (Desktop : $' + BankTranDesktopFee + ')  <a href="' + docpath + '" target="_blank" title="Download"> <i class="fa fa-arrow-circle-down"></i> </a></label><input  mainfee="' + r["DesktopFee"] + '" subfee="' + SubDesktopFee + '" id="input_trns_' + r["BankId"] + '" feetype="desktop"  maxfee="' + r["BankTranDesktopFee"] + '" class="form-control banktrans decimal transfee" name="banktrans" type="text" bankid="' + r["BankId"] + '" maxlength="8"  onkeyup="fnFeeSetupTRANSFee(this)"></div></div>');
                        container4.append('<div class="col-md-4"><div class="form-group is-empty"><label>' + r["BankName"] + ' (MSO : $' + BankTranMSOFee + ')  <a href="' + docpath + '" target="_blank" title="Download"> <i class="fa fa-arrow-circle-down"></i> </a></label><input  mainfee="' + r["MSOFee"] + '" subfee="' + SubMSOFee + '"  id="input_trns_' + r["BankId"] + '_MSO" feetype="mso" maxfee="' + r["BankTranMSOFee"] + '" class="form-control decimal transfee" type="text" bankid="' + r["BankId"] + '" maxlength="8"   onkeyup="fnFeeSetupTRANSFee(this)"></div></div>');
                    } else {
                        container3.append('<div class="col-md-4"><div class="form-group is-empty"><label>' + r["BankName"] + ' (Desktop : $' + BankTranDesktopFee + ')  <a href="' + docpath + '" target="_blank" title="Download"> <i class="fa fa-arrow-circle-down"></i> </a></label><input  mainfee="' + r["DesktopFee"] + '" subfee="' + SubDesktopFee + '" id="input_trns_' + r["BankId"] + '" feetype="desktop"  maxfee="' + r["BankTranDesktopFee"] + '" class="form-control banktrans decimal transfee" name="banktrans" type="text" bankid="' + r["BankId"] + '" maxlength="8"   onkeyup="fnFeeSetupTRANSFee(this)"></div></div>');
                    }
                }

            } else {

                if (r["ServiceorTransmission"] == 1 || r["ServiceorTransmission"] == '1') {
                    if (ismsouser == 'true' || ismsouser == true || ismsouser == "True") {
                        container1.append('<div class="col-md-4"><div class="form-group is-empty"><label>' + r["BankName"] + ' (Desktop : $' + BankSVBDesktopFee + ')</label><input  mainfee="' + r["DesktopFee"] + '"  subfee="' + SubDesktopFee + '" id="input_' + r["BankId"] + '" feetype="desktop"  maxfee="' + r["BankSVBDesktopFee"] + '"  class="form-control bankservice decimal svbfee" name="bankservice" type="text" bankid="' + r["BankId"] + '" maxlength="8" onkeyup="fnFeeSetupSVBFee(this)"></div></div>');
                        container2.append('<div class="col-md-4"><div class="form-group is-empty"><label>' + r["BankName"] + ' (MSO : $' + BankSVBMSOFee + ')</label><input  mainfee="' + r["MSOFee"] + '" subfee="' + SubMSOFee + '" id="input_' + r["BankId"] + '_MSO"  feetype="mso" maxfee="' + r["BankSVBMSOFee"] + '"   class="form-control decimal svbfee" type="text" bankid="' + r["BankId"] + '" maxlength="8"  onkeyup="fnFeeSetupSVBFee(this)"></div></div>');
                    } else {
                        container1.append('<div class="col-md-4"><div class="form-group is-empty"><label>' + r["BankName"] + ' (Desktop : $' + BankSVBDesktopFee + ')</label><input  mainfee="' + r["DesktopFee"] + '"  subfee="' + SubDesktopFee + '" id="input_' + r["BankId"] + '" feetype="desktop"  maxfee="' + r["BankSVBDesktopFee"] + '"  class="form-control bankservice decimal svbfee" name="bankservice" type="text" bankid="' + r["BankId"] + '" maxlength="8"  onkeyup="fnFeeSetupSVBFee(this)"></div></div>');

                    }
                } else {
                    if (ismsouser == 'true' || ismsouser == true || ismsouser == "True") {
                        container3.append('<div class="col-md-4"><div class="form-group is-empty"><label>' + r["BankName"] + ' (Desktop : $' + BankTranDesktopFee + ')</label><input  mainfee="' + r["DesktopFee"] + '" subfee="' + SubDesktopFee + '" id="input_trns_' + r["BankId"] + '"  feetype="desktop"  maxfee="' + r["BankTranDesktopFee"] + '" class="form-control banktrans decimal transfee" name="banktrans" type="text" bankid="' + r["BankId"] + '" maxlength="8" onkeyup="fnFeeSetupTRANSFee(this)"></div></div>');
                        container4.append('<div class="col-md-4"><div class="form-group is-empty"><label>' + r["BankName"] + ' (MSO : $' + BankTranMSOFee + ')</label><input mainfee="' + r["MSOFee"] + '" subfee="' + SubMSOFee + '"  id="input_trns_' + r["BankId"] + '_MSO"  feetype="mso" maxfee="' + r["BankTranMSOFee"] + '" class="form-control decimal transfee" type="text" bankid="' + r["BankId"] + '" maxlength="8" onkeyup="fnFeeSetupTRANSFee(this)"></div></div>');
                    } else {
                        container3.append('<div class="col-md-4"><div class="form-group is-empty"><label>' + r["BankName"] + ' (Desktop : $' + BankTranDesktopFee + ')</label><input  mainfee="' + r["DesktopFee"] + '" subfee="' + SubDesktopFee + '" id="input_trns_' + r["BankId"] + '"  feetype="desktop"  maxfee="' + r["BankTranDesktopFee"] + '" class="form-control banktrans decimal transfee" name="banktrans" type="text" bankid="' + r["BankId"] + '" maxlength="8" onkeyup="fnFeeSetupTRANSFee(this)"></div></div>');
                    }
                }
            }

        });

        var url = '/api/SubSiteFee/';
        if (UserId != '' && UserId != null && UserId != '00000000-0000-0000-0000-000000000000') {
            ajaxHelper(url + '?Id=' + UserId, 'GET').done(function (data) {

                if (data == "" || data == null || data == undefined) {
                    $('#subquestion_Service').hide();
                    $('#subquestion_Trans').hide();
                }

                $.each(data, function (rowIndex, r) {
                    $('#ID').val(r["Id"]);
                    //if (entityid != $('#Entity_SO').val()) {//entityid != "0676dfd0-da29-41e3-a262-81cb528b796c") {

                    if (r["ServiceorTransmission"] == 1) {
                        var BankTrans = $('input[type=text].bankservice');
                        $.each(r.SubSiteBankFees, function (indx, c) {

                            $('#input_' + c["BankMaster_ID"]).val(c["BankMaxFees"]);
                            $('#input_' + c["BankMaster_ID"] + '_MSO').val(c["BankMaxFees_MSO"]);

                            //var maxfee = $('#input_' + c["BankMaster_ID"]).attr('maxfee');
                            //var myvalu = c["BankMaxFees"];

                            //if (maxfee == '' || maxfee == null) {
                            //    maxfee = 0;
                            //}

                            //if (myvalu == '' || myvalu == null) {
                            //    myvalu = 0;
                            //}

                            //var myMaxfee=0;

                            //if (maxfee > myvalu) {
                            //    myMaxfee = Number(maxfee) - Number(myvalu)
                            //} else {
                            //    myMaxfee = Number(myvalu) - Number(maxfee)
                            //}
                            //$('#input_' + c["BankMaster_ID"]).attr('maxfee', myMaxfee);

                            ////

                            //var maxfeeMSO = $('#input_' + c["BankMaster_ID"] + '_MSO').attr('maxfee');
                            //var myvaluMSO = c["BankMaxFees_MSO"];

                            //if (maxfeeMSO == '' || maxfeeMSO == null) {
                            //    maxfeeMSO = 0;
                            //}

                            //if (myvaluMSO == '' || myvaluMSO == null) {
                            //    myvaluMSO = 0;
                            //}

                            //var myMaxfeeMSO = 0;

                            //if (maxfeeMSO > myvaluMSO) {
                            //    myMaxfeeMSO = Number(maxfeeMSO) - Number(myvaluMSO)
                            //}
                            //else {
                            //    myMaxfeeMSO = Number(myvaluMSO) - Number(maxfeeMSO)
                            //}

                            //$('#input_' + c["BankMaster_ID"] + '_MSO').attr('maxfee', myMaxfeeMSO);

                        });

                        if (r["IsAddOnFeeCharge"] == true) {
                            $('#rbService_BankProductYes').prop('checked', true);
                        }
                        else {
                            $('#rbService_BankProductNo').prop('checked', true);
                            $('#subquestion_Service').hide();
                        }
                        if (r["IsSameforAll"] == true) {
                            $('#rbService_SubSiteYes').prop('checked', true);
                        }
                        else {
                            $('#rbService_SubSiteNo').prop('checked', true);
                        }
                        if (r["IsSubSiteAddonFee"] == true) {
                            $('#rbService_duringEnrollingYes').prop('checked', true);
                        }
                        else {
                            $('#rbService_duringEnrollingNo').prop('checked', true);
                        }
                    }
                    else {
                        var BankTrans = $('input[type=text].banktrans');

                        $.each(r.SubSiteBankFees, function (indx, c) {
                            $('#input_trns_' + c["BankMaster_ID"]).val(c["BankMaxFees"]);
                            $('#input_trns_' + c["BankMaster_ID"] + '_MSO').val(c["BankMaxFees_MSO"]);

                            //var maxfee = $('#input_trns_' + c["BankMaster_ID"]).attr('maxfee');
                            //var myvalu = c["BankMaxFees"];

                            //if (maxfee == '' || maxfee == null) {
                            //    maxfee = 0;
                            //}

                            //if (myvalu == '' || myvalu == null) {
                            //    myvalu = 0;
                            //}

                            //var myMaxfee = 0;

                            //if (maxfee > myvalu) {
                            //    myMaxfee = Number(maxfee) - Number(myvalu)
                            //} else {
                            //    myMaxfee = Number(myvalu) - Number(maxfee)
                            //}

                            //$('#input_trns_' + c["BankMaster_ID"]).attr('maxfee', myMaxfee);

                            ////

                            //var maxfeeMSO = $('#input_trns_' + c["BankMaster_ID"] + '_MSO').attr('maxfee');
                            //var myvaluMSO = c["BankMaxFees_MSO"];

                            //if (maxfeeMSO == '' || maxfeeMSO == null) {
                            //    maxfeeMSO = 0;
                            //}

                            //if (myvaluMSO == '' || myvaluMSO == null) {
                            //    myvaluMSO = 0;
                            //}

                            //var myMaxfeeMSO = 0;

                            //if (maxfeeMSO > myvaluMSO) {
                            //    myMaxfeeMSO = Number(maxfeeMSO) - Number(myvaluMSO)
                            //}
                            //else {
                            //    myMaxfeeMSO = Number(myvaluMSO) - Number(maxfeeMSO)
                            //}

                            //$('#input_trns_' + c["BankMaster_ID"] + '_MSO').attr('maxfee', myMaxfeeMSO);


                        });

                        if (r["IsAddOnFeeCharge"] == true) {
                            $('#rbTrans_BankProductYes').prop('checked', true);
                        }
                        else {
                            $('#rbTrans_BankProductNo').prop('checked', true);
                            $('#subquestion_Trans').hide();
                        }
                        if (r["IsSameforAll"] == true) {
                            $('#rbTrans_SubSiteYes').prop('checked', true);
                        }
                        else {
                            $('#rbTrans_SubSiteNo').prop('checked', true);
                        }
                        if (r["IsSubSiteAddonFee"] == true) {
                            $('#rbTrans_duringEnrollingYes').prop('checked', true);
                        }
                        else {
                            $('#rbTrans_duringEnrollingNo').prop('checked', true);
                        }
                    }

                });

                //getIsEnrollmentSubmit();
                ////  getIsSalesYearCheckBankDates();

                //var iretval = getIsSalesYearCheckBankDates();
                //var error_bank = $('#error_bank');
                //error_bank.html('');
                //error_bank.hide();
                //if (iretval == false || iretval == 'false' || iretval == 'False') {
                //    error_bank.show();
                //    error_bank.html('');
                //    error_bank.append('<p>The ability to update the Fees information is not available since the Cutoff date for the same is elapsed. Please contact the uTax support team. </p>');
                //    return false;
                //}

            });
        }

    });
}

