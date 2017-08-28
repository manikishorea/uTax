function getSubSiteConfig(RefId) {
    var IsDone = true;
    $('#formid').val('active');
    // getTimeZones($('#TimeZone'));
    $('#chk732ee2fd-f7c3-45be-809c-bb8d99dfbc1f').prop('checked', true);
    //MO or SVB Details
    if ($('#entityid').val() != $('#myentityid').val()) {
       // var entity = localStorage.getItem("spEntityID");
        getBankAndQuestions_MOSvb($('#divBanks'), $('#myentityid').val());
        getAffiliate_MoSvb($('#divAffiliates'), $('#myentityid').val());
    } else {
        getBankAndQuestions($('#divBanks'));
        getAffiliate($('#divAffiliates'));
    }

    $('#refId').val(RefId);
    var url = '/api/SubSiteConfig/Get';
    if (RefId != '' && RefId != null && RefId != '00000000-0000-0000-0000-000000000000') {
        ajaxHelper(url + '?id=' + RefId, 'GET').done(function (data) {
            
            if (data != null && data != undefined && data != '') {
                $('#Id').val(data["Id"]);
                $('#refId').val(data["refId"]);
                if (data["CanSubSiteLoginToEmp"] == true)
                    $('#CanSubSiteLoginToEmp').prop('checked', true);
                else
                    $('#CanSubSiteLoginToEmp').prop('checked', false);
                if (data.EnrollmentEmails)
                    $('#rdb_enrYes').prop('checked', true);
                else
                    $('#rdb_enrNo').prop('checked', true);

                IsDone = false;
                if (data["IsuTaxManageingEnrolling"] == "true" || data["IsuTaxManageingEnrolling"] == true || data["IsuTaxManageingEnrolling"] == "True") {
                    $('#IsuTaxManageingEnrolling1').attr('checked', 'checked');
                    IsDone = true;
                } else if (data["IsuTaxManageingEnrolling"] == "false" || data["IsuTaxManageingEnrolling"] == false || data["IsuTaxManageingEnrolling"] == "False") {
                    $('#IsuTaxManageingEnrolling2').attr('checked', 'checked');
                    IsDone = true;
                }

                if (data["IsuTaxPortalEnrollment"] == "true" || data["IsuTaxPortalEnrollment"] == true || data["IsuTaxPortalEnrollment"] == "True") {
                    $('#IsuTaxPortalEnrollment1').attr('checked', 'checked');
                    IsDone = true;
                } else if (data["IsuTaxPortalEnrollment"] == "false" || data["IsuTaxPortalEnrollment"] == false || data["IsuTaxPortalEnrollment"] == "False") {
                    $('#IsuTaxPortalEnrollment2').attr('checked', 'checked');
                    IsDone = true;
                }

                if (data["IsuTaxManageOnboarding"] == "true" || data["IsuTaxManageOnboarding"] == true || data["IsuTaxManageOnboarding"] == "True") {
                    $('#IsuTaxManageOnboarding1').attr('checked', 'checked');
                    IsDone = true;
                } else if (data["IsuTaxManageOnboarding"] == "false" || data["IsuTaxManageOnboarding"] == false || data["IsuTaxManageOnboarding"] == "False") {
                    $('#IsuTaxManageOnboarding2').attr('checked', 'checked');
                    IsDone = true;
                }

                if (data["IsuTaxCustomerSupport"] == "true" || data["IsuTaxCustomerSupport"] == true || data["IsuTaxCustomerSupport"] == "True") {
                    $('#IsuTaxCustomerSupport1').attr('checked', 'checked');
                    IsDone = true;
                } else if (data["IsuTaxCustomerSupport"] == "false" || data["IsuTaxCustomerSupport"] == false || data["IsuTaxCustomerSupport"] == "False") {
                    $('#IsuTaxCustomerSupport2').attr('checked', 'checked');
                    IsDone = true;
                }

                if (data["IsSubSiteEFINAllow"] == "true" || data["IsSubSiteEFINAllow"] == true || data["IsSubSiteEFINAllow"] == "True") {
                    $('#IsSubSiteEFINAllow1').attr('checked', 'checked');
                    $('#IsSubSiteEFINAllow2').attr('disabled', 'disabled');
                    IsDone = true;
                } else if (data["IsSubSiteEFINAllow"] == "false" || data["IsSubSiteEFINAllow"] == false || data["IsSubSiteEFINAllow"] == "False") {
                    $('#IsSubSiteEFINAllow2').attr('checked', 'checked');
                    IsDone = true;
                }

                $('#NoofSupportStaff').val(data["NoofSupportStaff"]);

                var days = data["NoofDays"];

                if (days != null && days != '') {
                    var splitdays = days.toString().split(',')

                    IsDone = true;
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
                    IsDone = true;
                } else if (data["IsAutoEnrollAffiliateProgram"] == "false" || data["IsAutoEnrollAffiliateProgram"] == false || data["IsAutoEnrollAffiliateProgram"] == "False") {
                    $('#IsAutoEnrollAffiliateProgram2').attr('checked', 'checked');
                    IsDone = true;
                }

                $('#SubSiteTaxReturn').val(data["SubSiteTaxReturn"]);

                if (data["SubSiteTaxReturn"] == "1") {
                    $('#SubSiteTaxReturn1').attr('checked', 'checked');
                } else if (data["SubSiteTaxReturn"] == "2") {
                    $('#SubSiteTaxReturn2').attr('checked', 'checked');
                } else if (data["SubSiteTaxReturn"] == "3") {
                    $('#SubSiteTaxReturn3').attr('checked', 'checked');
                }


                $.each(data.Affiliates, function (coIndex, c) {
                    $('#chk' + c["AffiliateProgramId"]).attr('checked', 'checked');
                });

                $.each(data.SubSiteBankQuestions, function (coIndex, c) {
                    var bankid = c["BankId"];
                    $('#chk' + bankid).attr('checked', 'checked');
                    $('#divBankQuestions' + bankid).show();
                    $('#chk' + c["QuestionId"]).attr('checked', 'checked');
                });

                visibleshowhide();
                mysupportInfo();
                chkBelowinfo();

                if (IsDone) {
                    $('#formid').val('active');
                }
            } else {

                $('input[type=checkbox][chkname=Versicom]').attr('checked', 'checked');
                $('input[type=checkbox][chkname=versicom]').attr('checked', 'checked');
            }


            getIsEnrollmentSubmit();
           // chkBelowinfo();
        });
    }
}

function fnSaveBankService() {

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

    if ($('input[name=IsuTaxManageingEnrolling]:checked').length == 0) {
        $('#dvFirst').addClass("error_msg");
        $('#dvFirst').attr('title', 'Please Select Yes or No');
        cansubmit = false;
    }
    else if ($('input[name=IsSubSiteEFINAllow]:checked').length == 0) {
        $('#dvthird').addClass("error_msg");
        $('#dvthird').attr('title', 'Please Select Yes or No');
        cansubmit = false;
    }
    //else if ($('input[name=   ]:checked').length == 0) {
    //    $('#dv_enremail').addClass("error_msg");
    //    $('#dv_enremail').attr('title', 'Please Select Yes or No');
    //    cansubmit = false;
    //}

    if ($('#entityid').val() != $('#myentityid').val()) {
        req.refId = $('#myid').val();
    } else {
        req.refId = $.trim($('#refId').val());
    }

    req.IsSubSiteEFINAllow = false;
    if ($('#IsSubSiteEFINAllow1').is(':checked')) {
        req.IsSubSiteEFINAllow = true;
    }

    req.IsuTaxManageingEnrolling = false;
    if ($('#IsuTaxManageingEnrolling1').is(':checked')) {
        req.IsuTaxManageingEnrolling = true;
    }
    else {
        if ($('input[name=IsuTaxPortalEnrollment]:checked').length == 0) {
            $('#dvsecond').addClass("error_msg");
            $('#dvsecond').attr('title', 'Please Select Yes or No');
            cansubmit = false;
        }
    }
    req.IsuTaxPortalEnrollment = false;
    if (!req.IsuTaxManageingEnrolling) {
        if ($('#IsuTaxPortalEnrollment1').is(':checked')) {
            req.IsuTaxPortalEnrollment = true;
        }
    }

    var BankQuestions = [];
    var IsBankChecked = false;
    allchkbox = $('input[type=checkbox].chkBank');
    $.each(allchkbox, function (indx, valu) {
        var chkId = $(valu).attr('id');
        var BankId = $('#' + chkId).val()
        if ($(valu).is(':checked')) {
            var bankQueRB = $('input[name=bank' + BankId + ']');
            var questionid = bankQueRB.filter(':checked').val();
            IsBankChecked = true;
            BankQuestions.push({
                BankId: BankId,
                QuestionId: questionid
            });
        }
    });

    req.SubSiteBankQuestions = BankQuestions;

    $('#divBanks').removeClass("error_msg");
    if (!IsBankChecked) {
        $('#divBanks').addClass("error_msg");
        $('#divBanks').attr('title', 'Please provide at lease one bank');
        cansubmit = false;
    }
    if (!cansubmit) {
        error.show();
        error.append('<p> Please correct the error(s). </p>');
        $("html, body").animate({ scrollTop: 0 }, "slow");
        return false;
    }

    if (cansubmit) {

        error.hide();
        error.html('');

        req.Id = $('#Id').val();

        req.UserId = $('#UserId').val();
        req.EnrollmentEmails = $('#rdb_enrYes').prop('checked');


        var Uri = '/api/SubSiteConfig/BankService';
        ajaxHelper(Uri, 'POST', req,false).done(function (data, status) {

            $("html, body").animate({ scrollTop: 0 }, "slow");
            if (data != '' && data != null && data != '00000000-0000-0000-0000-000000000000') {
                $('#Id').val(data);
                success.show();
                success.append('<p> Main Office Sub-site configuration saved Successfully</p>');

                $('#libankenrollment_service').removeClass('active');
                $('#bankenrollment_service').removeClass('active');

                $('#lionboarding_service').addClass('active');
                $('#onboarding_service').addClass('active').addClass('in');

                $('#lisupport_service').removeClass('active');
                $('#support_service').removeClass('active');
                getConfigStatus();
                return true;
            }
            else {

                error.show();
                error.append('<p> Record not saved. </p>');
                return false;
            }

        });
    }
}

function fnSaveOnBoarding() {

    var req = {};
    var cansubmit = true;

    var error = $('#error');
    error.html('');
    error.hide();

    var success = $('#success');
    success.html('');
    success.hide();
    $('*').removeClass("error_msg");

    if ($('#entityid').val() != $('#myentityid').val()) {
        req.refId = $('#myid').val();
    } else {
        req.refId = $.trim($('#refId').val());
    }

    if ($('input[name=IsuTaxManageOnboarding]:checked').length == 0) {
        $('#dvFourth').addClass("error_msg");
        $('#dvFourth').attr('title', 'Please Select Yes or No');
        cansubmit = false;
    }

    req.IsuTaxManageOnboarding = false;
    if ($('#IsuTaxManageOnboarding1').is(':checked')) {
        req.IsuTaxManageOnboarding = true;
    }

    if (!cansubmit) {
        error.show();
        error.append('<p> Please correct the error(s). </p>');
        $("html, body").animate({ scrollTop: 0 }, "slow");
        return false;
    }

    if (cansubmit) {

        error.hide();
        error.html('');

        req.Id = $('#Id').val();
        req.UserId = $('#UserId').val();

        var Uri = '/api/SubSiteConfig/OnBoardingService';
        ajaxHelper(Uri, 'POST', req,false).done(function (data, status) {
            $("html, body").animate({ scrollTop: 0 }, "slow");
            if (data != '' && data != null && data != '00000000-0000-0000-0000-000000000000') {
                $('#Id').val(data);
                success.show();
                success.append('<p> Main Office Sub-site configuration saved Successfully </p>');

                $('#libankenrollment_service').removeClass('active');
                $('#bankenrollment_service').removeClass('active');

                $('#lionboarding_service').removeClass('active');
                $('#onboarding_service').removeClass('active');

                $('#lisupport_service').addClass('active');
                $('#support_service').addClass('active').addClass('in');
                getConfigStatus();
                return true;
            }
            else {
                error.show();
                error.append('<p> Record not saved. </p>');
                return false;
            }

        });
    }
}

function fnSaveSupport(SaveType) {

    var req = {};
    var cansubmit = true;

    var error = $('#error');
    error.html('');
    error.hide();

    var success = $('#success');
    success.html('');
    success.hide();

    if ($('#entityid').val() != $('#myentityid').val()) {
        req.refId = $('#myid').val();
    } else {
        req.refId = $.trim($('#refId').val());
    }

    $('*').removeClass("error_msg");
    if ($('input[name=IsuTaxCustomerSupport]:checked').length == 0) {
        $('#dvFifth').addClass("error_msg");
        $('#dvFifth').attr('title', 'Please Select Yes or No');
        cansubmit = false;
    }
    else if ($('input[name=SubSiteTaxReturn]:checked').length == 0) {
        $('#dvSixth').addClass("error_msg");
        $('#dvSixth').attr('title', 'Please Select Yes or No');
        cansubmit = false;
    }

    req.IsuTaxCustomerSupport = false;
    if ($('#IsuTaxCustomerSupport1').is(':checked')) {
        req.IsuTaxCustomerSupport = true;
    }

    if (!req.IsuTaxCustomerSupport) {
        req.NoofSupportStaff = $.trim($('#NoofSupportStaff').val());
        req.OpenHours = $.trim($('#OpenHours').val());
        req.CloseHours = $.trim($('#CloseHours').val());
        req.TimeZone = $.trim($('#TimeZone').val());


        var supportchk = $('input[type=checkbox].supportdays');
        var IsDaysChecked = false;
        var SupportDays = '';
        $.each(supportchk, function (indx, valu) {
            var chkId = $(valu).attr('id');
            var ProgramId = $('#' + chkId).val()
            if ($(valu).is(':checked')) {
                IsDaysChecked = true;
                SupportDays = SupportDays + ProgramId + ",";
            }
        });

        $('#divSupportDays').removeClass("error_msg");
        if (!IsDaysChecked) {
            $('#divSupportDays').addClass("error_msg");
            $('#divSupportDays').attr('title', 'Please provide at least one week days');
            cansubmit = false;
        }

        req.NoofDays = SupportDays;


        $('#NoofSupportStaff').removeClass("error_msg");
        if (Number(req.NoofSupportStaff) <= 0 || req.NoofSupportStaff == "") {
            $('#NoofSupportStaff').addClass("error_msg");
            $('#NoofSupportStaff').attr('title', 'Please enter number of Support Staff');
            cansubmit = false;
        }
        else if (Number(req.NoofSupportStaff) > 20) {
            $('#NoofSupportStaff').addClass("error_msg");
            $('#NoofSupportStaff').attr('title', 'Please enter number of Support Staff 1 to 20');
            cansubmit = false;
        }

        $('#OpenHours').removeClass("error_msg");
        if (req.OpenHours == '' || req.OpenHours == null) {
            $('#OpenHours').addClass("error_msg");
            $('#OpenHours').attr('title', 'Please enter Open Hours');
            cansubmit = false;
        }

        if (!ValidateTime(req.OpenHours)) {
            $('#OpenHours').addClass("error_msg");
            $('#OpenHours').attr('title', 'Please enter valid Open Hours');
            cansubmit = false;
        }


        $('#CloseHours').removeClass("error_msg");
        if (req.CloseHours == '' || req.CloseHours == null) {
            $('#CloseHours').addClass("error_msg");
            $('#CloseHours').attr('title', 'Please enter Close Hours');
            cansubmit = false;
        }

        if (!ValidateTime(req.CloseHours)) {
            $('#CloseHours').addClass("error_msg");
            $('#CloseHours').attr('title', 'Please enter valid Close Hours');
            cansubmit = false;
        }

        $('#TimeZone').removeClass("error_msg");
        if (req.TimeZone == '' || req.TimeZone == null) {
            $('#TimeZone').addClass("error_msg");
            $('#TimeZone').attr('title', 'Please select TimeZone');
            cansubmit = false;
        }

    }

    req.SubSiteTaxReturn = $.trim($('#SubSiteTaxReturn').val());

    var allchkbox = $('input[type=radio][name=SubSiteTaxReturn]');
    var IsTaxChecked = false;
    $.each(allchkbox, function (indx, valu) {
        var chkId = $(valu).attr('id');
        var ProgramId = $('#' + chkId).val()
        if ($(valu).is(':checked')) {
            IsTaxChecked = true;
            req.SubSiteTaxReturn = ProgramId;
        }
    });


    req.IsAutoEnrollAffiliateProgram = false;
    if (IsTaxChecked) {
        if ($('#IsAutoEnrollAffiliateProgram1').is(':checked')) {
            req.IsAutoEnrollAffiliateProgram = true;
        }
    }



    var Affiliates = [];
    var IsAffiliateChecked = false;
    var allchkbox = $('input[type=checkbox].chkAffiliate');
    $.each(allchkbox, function (indx, valu) {
        var chkId = $(valu).attr('id');
        var ProgramId = $('#' + chkId).val()
        if ($(valu).is(':checked')) {
            IsAffiliateChecked = true;
            Affiliates.push({
                AffiliateProgramId: ProgramId
            });
        }
    });
    req.Affiliates = Affiliates;

    //$('#divAffiliates').removeClass("error_msg");
    //if (!IsAffiliateChecked) {
    //    $('#divAffiliates').addClass("error_msg");
    //    $('#divAffiliates').attr('title', 'Please provide at least one affiliate program');
    //    cansubmit = false;
    //}

    if (!cansubmit) {
        error.show();
        error.append('<p> Please correct the error(s). </p>');
        $("html, body").animate({ scrollTop: 0 }, "slow");
        return false;
    }

    if (cansubmit) {

        error.hide();
        error.html('');

        req.Id = $('#Id').val();
        req.UserId = $('#UserId').val();

        var Uri = '/api/SubSiteConfig/SupportService';
        ajaxHelper(Uri, 'POST', req,false).done(function (data, status) {
            $("html, body").animate({ scrollTop: 0 }, "slow");

            if (data != '' && data != null && data != '00000000-0000-0000-0000-000000000000') {
                $('#Id').val(data);
                success.show();
                success.append('<p> Main Office Sub-site configuration saved Successfully </p>');
               
                if ($('#entityid').val() != $('#myentityid').val()) {
                    UpdateOfficeManagement($('#myid').val());
                } else {
                    UpdateOfficeManagement($('#UserId').val());
                }

                if (SaveType == 1) {
                    if ($('#entityid').val() != $('#myentityid').val()) {
                        window.location.href = "/Configuration/FeesetupConfiguration?Id=" + $('#myid').val() + '&entitydisplayid=' + $('#myentitydisplayid').val()+'&ptype=config&entityid='+$('#myentityid').val();
                    } else {
                        window.location.href = "/Configuration/FeesetupConfiguration/";
                    }
                } else {
                    UpdateOfficeManagement($('#UserId').val());
                    getConfigStatus();
                }
                return true;
            }
            else {

                error.show();
                error.append('<p> Record not saved. </p>');
                return false;
            }

        });
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
