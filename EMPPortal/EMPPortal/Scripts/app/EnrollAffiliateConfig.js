function fnGetEnrollAffiliateConfig(Id) {
    // getAffiliateEnroll($('#divAffiliates'));
    //  getAffiliateEnroll_Sub($('#divAffiliates'));
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
                    var IsAutoEnrollAffiliateProgram = c["IsAutoEnrollAffiliateProgram"];
                    if (IsAutoEnrollAffiliateProgram) {
                        $('#chkAE' + c["AffiliateProgramId"]).attr('disabled', 'disabled');
                    }
                    if (chktext == 'iProtect') {
                        $('#diviProtect').show();
                    }
                    $('#chkAE' + c["AffiliateProgramId"] + '_charge').val(c["AffiliateProgramCharge"]);
                });
            } else {
                IsMainConfig = true;
            }
        });
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

function fnGetEnrollOfficeConfig(UserId) {
    //var ParentId = $('#parentid').val();

    //var Cid = getUrlVars()["Id"];
    //if (Cid) {
    //    UserId = Cid;
    //    ParentId = $('#myparentid').val();// localStorage.getItem("EnrollparentId");
    //}

    var url = '/api/EnrollmentOfficeConfig/Self';
    $('#IsMainSiteTransmitTaxReturn').val(0);

    if (UserId != '' && UserId != null && UserId != '00000000-0000-0000-0000-000000000000') {
        ajaxHelper(url + '?id=' + UserId, 'GET').done(function (data) {

            if (data != null && data != '' && data != undefined) {
                $('#Id').val(data["Id"]);

                if (data["IsMainSiteTransmitTaxReturn"] == 'true' || data["IsMainSiteTransmitTaxReturn"] == 'True' || data["IsMainSiteTransmitTaxReturn"] == true) {
                    $('#IsMainSiteTransmitTaxReturn1').html('Yes');
                    $('#IsMainSiteTransmitTaxReturn').val(1);

                } else if (data["IsMainSiteTransmitTaxReturn"] == 'false' || data["IsMainSiteTransmitTaxReturn"] == 'False' || data["IsMainSiteTransmitTaxReturn"] == false) {
                    $('#IsMainSiteTransmitTaxReturn1').html('No');
                    $('#IsMainSiteTransmitTaxReturn').val(0);
                }


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

        //var Id = $('#Id').val();
        //if (Id == '' || Id == null || Id == '00000000-0000-0000-0000-000000000000') {
        //    fnGetEnrollOfficeMainConfig(ParentId);
        //}
    }
}

function fnGetEnrollOfficeMainConfig(UserId) {

    //var Cid = getUrlVars()["Id"];
    //if (Cid) {
    //    UserId = Cid;
    //    ParentId = $('#myparentid').val();
    //   // ParentId = localStorage.getItem("EnrollparentId");
    //}
    var url = '/api/EnrollmentOfficeConfig/MainConfig';
    if (UserId != '' && UserId != null && UserId != '00000000-0000-0000-0000-000000000000') {
        ajaxHelper(url + '?id=' + UserId, 'GET').done(function (data) {

            if (data) {
                if (data["IsMainSiteTransmitTaxReturn"] == 'true' || data["IsMainSiteTransmitTaxReturn"] == 'True' || data["IsMainSiteTransmitTaxReturn"] == true) {
                    $('#IsMainSiteTransmitTaxReturn1').html('Yes');
                    $('#IsMainSiteTransmitTaxReturn').val(1);

                } else if (data["IsMainSiteTransmitTaxReturn"] == 'false' || data["IsMainSiteTransmitTaxReturn"] == 'False' || data["IsMainSiteTransmitTaxReturn"] == false) {
                    $('#IsMainSiteTransmitTaxReturn1').html('No');
                    $('#IsMainSiteTransmitTaxReturn').val(0);
                }
            }
        });
    }
}

function fnSaveEnrollOfficeConfig(type) {
    var req = {};
    var cansubmit = true;

    var error = $('#error');
    error.html('');
    error.hide();

    var success = $('#success');
    success.html('');
    success.hide();

    req.IsMainSiteTransmitTaxReturn = false;
    if ($('#IsMainSiteTransmitTaxReturn').val() == 1) {
        req.IsMainSiteTransmitTaxReturn = true;
    }

    req.NoofTaxProfessionals = $.trim($('#NoofTaxProfessionals').val());
    req.IsSoftwareOnNetwork = $('#IsSoftwareOnNetwork1').is(':checked');
    req.NoofComputers = $.trim($('#NoofComputers').val());
    req.PreferredLanguage = $('#PreferredLanguage1').is(':checked') ? 1 : 2;


    $('#NoofTaxProfessionals').removeClass("error_msg");
    if (req.NoofTaxProfessionals == "") {
        $('#NoofTaxProfessionals').addClass("error_msg");
        $('#NoofTaxProfessionals').attr('title', 'Please enter Tax Professionals count');
        cansubmit = false;
    }

    $('#NoofComputers').removeClass("error_msg");
    if (req.NoofComputers == "") {
        $('#NoofComputers').addClass("error_msg");
        $('#NoofComputers').attr('title', 'Please enter Computers count');
        cansubmit = false;
    }


    //$('#NoofTaxProfessionals').removeClass('error_msg');
    if (req.NoofTaxProfessionals != '' && req.NoofTaxProfessionals != null && req.NoofTaxProfessionals != undefined) {
        if (Number(req.NoofTaxProfessionals) > 50) {

            $('#NoofTaxProfessionals').addClass('error_msg');
            $('#NoofTaxProfessionals').attr('title', 'Tax professionals maximum allowed is 50');
            cansubmit = false;
        }

        if (Number(req.NoofTaxProfessionals) < 1) {
            $('#NoofTaxProfessionals').addClass('error_msg');
            $('#NoofTaxProfessionals').attr('title', 'Tax professionals minimum allowed is 1');
            cansubmit = false;
        }
    }


    //$('#NoofComputers').removeClass('error_msg');
    if (req.NoofComputers != '' && req.NoofComputers != null && req.NoofComputers != undefined) {

        if (Number(req.NoofComputers) > 99) {

            $('#NoofComputers').addClass('error_msg');
            $('#NoofComputers').attr('title', 'Computer maximum allowed is 99');
            cansubmit = false;
        }

        if (Number(req.NoofComputers) < 1) {
            $('#NoofComputers').addClass('error_msg');
            $('#NoofComputers').attr('title', 'Computer minimum allowed is 1');
            cansubmit = false;
        }
    }


    if (!cansubmit) {
        error.show();
        error.append('<p> Please correct the error(s). </p>');
        return false;
    }

    if (cansubmit) {

        error.hide();
        error.html('');

        req.Id = $('#Id').val();
        req.UserId = $('#UserId').val();
        req.CustomerId = $('#UserId').val();

        if ($('#entityid').val() != $('#myentityid').val()) {
            req.CustomerId = $('#myid').val();
        }


        //var entitydisplayid = "";
        //if (getUrlVars()["entitydisplayid"]) {

        //    entitydisplayid = getUrlVars()["entitydisplayid"];
        //}

        var Uri = '/api/EnrollmentOfficeConfig';
        ajaxHelper(Uri, 'POST', req,false).done(function (data, status) {

            if (data == -1) {
                error.show();
                error.append('<p>  Record not saved. </p>');
            }
            else {
                success.show();
                success.append('<p> Record saved successfully. </p>');
                SaveConfigStatusActive('done');
                UpdateOfficeManagement(req.CustomerId);
                if (type == 1) {

                    if ($('#entityid').val() != $('#myentityid').val()) {
                        window.location.href = '/Enrollment/AffiliateConfiguration?Id=' + req.CustomerId + '&entitydisplayid=' + $('#myentitydisplayid').val() + '&ParentId=' + $('#myparentid').val() + '&entityid=' + $('#myentityid').val() + '&ptype=enrollment';
                    } else {
                        window.location.href = '/Enrollment/AffiliateConfiguration';
                    }
                }

                getConfigStatus();
            }
            //if (data == 'true' || data == 'True' || data == true) {
            //    success.show();
            //    success.append('<p> Record saved successfully. </p>');
            //    return true;
            //}
            //else {
            //    error.show();
            //    error.append('<p>  Record not saved. </p>');
            //    return false;
            //}
        });
    }
}

function fnSaveEnrollAffiliateConfig(type) {

    var req = {};
    var cansubmit = true;

    var error = $('#error');
    error.html('');
    error.hide();

    var success = $('#success');
    success.html('');
    success.hide();

    var bval = false;
    $('.chkAEAffiliate').each(function (e) {
        var id = $(this).attr('Id');
        var isChecked = $(this).is(':checked');
        var chkname = $(this).attr('chkname');
        if (chkname == 'iProtect' && isChecked) {
            bval = true;
        }
    });


    $('#chk25a4379b-4df1-4a65-aec1-30dcd587eeb7_charge').removeClass('error_msg');
    if (bval) {

        var AffiliateProgramCharge = $('#chk25a4379b-4df1-4a65-aec1-30dcd587eeb7_charge').val();
        $('#chk25a4379b-4df1-4a65-aec1-30dcd587eeb7_charge').removeClass("error_msg");
        if (AffiliateProgramCharge == "") {
            $('#chk25a4379b-4df1-4a65-aec1-30dcd587eeb7_charge').addClass("error_msg");
            $('#chk25a4379b-4df1-4a65-aec1-30dcd587eeb7_charge').attr('title', 'Please enter fee');
            cansubmit = false;
        }

        if (AffiliateProgramCharge != '' && AffiliateProgramCharge != null && AffiliateProgramCharge != undefined) {
            if (Number(AffiliateProgramCharge) > 50) {

                $('#chk25a4379b-4df1-4a65-aec1-30dcd587eeb7_charge').addClass('error_msg');
                $('#chk25a4379b-4df1-4a65-aec1-30dcd587eeb7_charge').attr('title', 'The maximum fee allowed is $50');
                cansubmit = false;
            }

            if (Number(AffiliateProgramCharge) < 0) {

                $('#chk25a4379b-4df1-4a65-aec1-30dcd587eeb7_charge').addClass('error_msg');
                $('#chk25a4379b-4df1-4a65-aec1-30dcd587eeb7_charge').attr('title', 'The minimum fee allowed is $0');
                cansubmit = false;
            }
        }
    }

    var Affiliates = [];
    var IsAffiliateChecked = false;
    var allchkbox = $('input[type=checkbox].chkAEAffiliate');

    var CustomerId = $('#UserId').val();

    if ($('#entityid').val() != $('#myentityid').val()) {
        CustomerId = $('#myid').val();
    }

    $.each(allchkbox, function (indx, valu) {
        var chkId = $(valu).attr('id');
        var ProgramId = $('#' + chkId).val()
        var ProgramCharge = $('#' + chkId + '_charge').val()
        if ($(valu).is(':checked')) {
            IsAffiliateChecked = true;
            Affiliates.push({
                AffiliateProgramId: ProgramId,
                AffiliateProgramCharge: ProgramCharge,
                CustomerId: CustomerId
            });
        }
    });

    req.Affiliates = Affiliates;

    if (!cansubmit) {
        error.show();
        error.append('<p> Please correct the error(s). </p>');
        return false;
    }

    if (cansubmit) {

        error.hide();
        error.html('');

        req.UserId = $('#UserId').val();



        var entitydisplayid = getUrlVars()["entitydisplayid"];

        var Uri = '/api/EnrollmentAffiliateConfig';
        ajaxHelper(Uri, 'POST', req,false).done(function (data, status) {

            if (data == 'true' || data == 'True' || data == true) {
                success.show();
                success.append('<p> Record saved successfully. </p>');
                SaveConfigStatusActive('done');

                if (type == 1) {

                    //if ($('#site067c03a3-34f1-4143-beae-35327a8fca44').length <= 0) { // doesn't have the permission to enrollment (EFIN NO )
                    //    getConfigStatus();
                    //    return;
                    //}
                    //

                    if ($('#site2f7d1b90-78aa-4a93-85ec-81cd8b10a545').parent().next('li').length == 0) {
                        return false;
                    }

                    if ($('#entityid').val() != $('#myentityid').val()) {

                        // var erotype = localStorage.getItem("EnrollEROType");
                        var erotype = getEroType($('#myentityid').val());
                        if (erotype == 'Single Office')
                            window.location.href = '/Enrollment/BankSelectionFeeDetails?Id=' + CustomerId + '&entitydisplayid=' + entitydisplayid + '&ParentId=' + $('#myparentid').val() + '&entityid=' + $('#myentityid').val() + '&ptype=enrollment';
                        else
                            window.location.href = '/Enrollment/BankSelectionFeeDetails?Id=' + CustomerId + '&entitydisplayid=' + entitydisplayid + '&ParentId=' + $('#myparentid').val() + '&entityid=' + $('#myentityid').val() + '&ptype=enrollment';
                    }
                    else {
                        var erotypeid = $('#entityid').val();
                        var erotype = getEroType(erotypeid);
                        if (erotype == 'Single Office')
                            window.location.href = '/Enrollment/BankSelectionFeeDetails';
                        else
                            window.location.href = '/Enrollment/BankSelectionFeeDetails';
                    }
                }
                getConfigStatus();
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

function getEroType(id) {
    switch (id) {
        case '1':
            return 'uTax';
        case '2':
            return 'Single Office';
        case '3':
            return 'SOME';
        case '4':
            return 'Multi-Office';
        case '5':
            return 'Multi-Office Sub-Office';
        case '6':
            return 'Service Bureau - SVB';
        case '7':
            return 'SVB Sub-Office';
        case '8':
            return 'SOME-SubSite';
        default:
            return '';

    }
}

