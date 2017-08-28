function getMainOfficeInformation() {
    var Id;
    var _entityid;
    if ($('#entityid').val() != $('#myentityid').val()) {
        Id = $('#myid').val();
        _entityid = $('#myentityid').val();
    } else {
        Id = $('#UserId').val();
        _entityid = $('#entityid').val();
    }
    var url = '/api/MainOffice/';
    if (Id != '' && Id != null && Id != '00000000-0000-0000-0000-000000000000') {
        ajaxHelper(url + '?Id=' + Id, 'GET').done(function (data) {
            if (data != null && data != '' && data != undefined) {
                if (data.Id == '00000000-0000-0000-0000-000000000000') {
                    if (data.IsBusinessSoftware)
                        if (_entityid == 2 || _entityid == 5 || _entityid == 9)
                            $('#rbBSYes').attr('checked', true);
                }
                else {
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

                    if (data["IsBusinessSoftware"]) {
                        $('#rbBSYes').attr('checked', true);
                    }
                    else {
                        $('#rbBSNo').attr('checked', true);
                    }

                    if (data["IsSharingEFIN"])
                        $('#rbseYes').attr('checked', true);
                    else
                        $('#rbseNo').attr('checked', true);
                }
            }

            getIsEnrollmentSubmit();
        });
    }
}


function fnSaveMainOfficeConfig(SaveType) {
    var req = {};
    var cansubmit = true;
    var error = $('#error');
    error.html('');
    error.hide();

    var success = $('#success');
    success.html('');
    success.hide();
    req.Id = $.trim($('#ID').val());
    if ($('#entityid').val() != $('#myentityid').val()) {
        req.refId = $('#myid').val();
    } else {
        req.refId = $('#UserId').val();
    }
    $('*').removeClass("error_msg");
    $('*').attr('title', '');

    if ($('input[name=rbtaxreturns]:checked').length == 0) {
        $('#dvFirst').addClass("error_msg");
        $('#dvFirst').attr('title', 'Please Select Yes or No');
        cansubmit = false;
    }
    else if ($('input[name=sample4]:checked').length == 0) {
        $('#dvFourth').addClass("error_msg");
        $('#dvFourth').attr('title', 'Please Select Yes or No');
        cansubmit = false;
    }
    else if ($('input[name=rbBusSw]:checked').length == 0) {
        $('#dvBSW').addClass("error_msg");
        $('#dvBSW').attr('title', 'Please Select Yes or No');
        cansubmit = false;
    }
    else if ($('input[name=shEfin]:checked').length == 0 && $('#hdnEFINStatus').val() == '16') {
        $('#dvSharingEfin').addClass("error_msg");
        $('#dvSharingEfin').attr('title', 'Please Select Yes or No');
        cansubmit = false;
    }

    req.IsSiteTransmitTaxReturns = $('#rbTaxReturnYes').is(':checked');
    if (!req.IsSiteTransmitTaxReturns) {
        req.IsSiteOfferBankProducts = false;
        req.TaxProfessionals = 0;
        req.IsSoftwarebeInstalledNetwork = false;
        req.ComputerswillruninSoftware = 0;
    }
    else {
        if ($('input[name=sample2]:checked').length == 0) {
            $('#dvSec').addClass("error_msg");
            $('#dvSec').attr('title', 'Please Select Yes or No');
            cansubmit = false;
        }
        else if ($('input[name=sample3]:checked').length == 0) {
            $('#dvThird').addClass("error_msg");
            $('#dvThird').attr('title', 'Please Select Yes or No');
            cansubmit = false;
        }
        req.IsSiteOfferBankProducts = $('#rbBankProdYes').is(':checked');
        req.TaxProfessionals = $('#TaxProfessionals').val();
        $('#TaxProfessionals').removeClass("error_msg");
        if (Number(req.TaxProfessionals) < 0 || req.TaxProfessionals == "") {
            $('#TaxProfessionals').addClass("error_msg");
            $('#TaxProfessionals').attr('title', 'Please enter valid number');
            cansubmit = false;
        }
        else if (Number(req.TaxProfessionals) < 1 || Number(req.TaxProfessionals) > 50) {
            $('#TaxProfessionals').addClass("error_msg");
            $('#TaxProfessionals').attr('title', 'It should accept between 1 to 50');
            cansubmit = false;
        }

        req.IsSoftwarebeInstalledNetwork = $('#rbsoftwareYes').is(':checked');

        req.ComputerswillruninSoftware = $('#computersoftware').val();
        $('#computersoftware').removeClass("error_msg");
        if (Number(req.ComputerswillruninSoftware) < 0 || req.ComputerswillruninSoftware == "") {
            $('#computersoftware').addClass("error_msg");
            $('#computersoftware').attr('title', 'Please enter valid number');
            cansubmit = false;
        }
        else if (Number(req.ComputerswillruninSoftware) < 1 || Number(req.ComputerswillruninSoftware) > 99) {
            $('#computersoftware').addClass("error_msg");
            $('#computersoftware').attr('title', 'It should accept between 1 to 99');
            cansubmit = false;
        }
    }
    req.PreferredSupportLanguage = $('#rbsupportYes').is(':checked') ? 1 : 2;

    req.IsBusinessSoftware = $('#rbBSYes').prop('checked');
    req.IsSharingEFIN = $('#rbseYes').prop('checked');

    if (!cansubmit) {
        error.show();
        error.append('<p> Please correct the error(s). </p>');
        $("html, body").animate({ scrollTop: 0 }, "slow");
        return false;
    }

    if (cansubmit) {
        error.hide();
        error.html('');
        req.Id = $('#ID').val();
        req.UserId = $('#UserId').val();
        var Uri = '/api/MainOffice/';

        $.blockUI({ message: '<img src="../../content/images/loading-img.gif"/>' });
        setTimeout(function () {
            ajaxHelper(Uri, 'POST', req, false).done(function (data, status) {
                $("html, body").animate({ scrollTop: 0 }, "slow");
                if (data == 'true' || data == 'True' || data == true) {
                    success.show();
                    success.append('<p> Main Office Configuration saved Successfully </p>');

                    SaveConfigStatusActive('done');


                    UpdateOfficeManagement(req.refId);

                    if (SaveType == 1) {
                        if ($('#entityid').val() != $('#myentityid').val()) {
                            window.location.href = "/Configuration/SubsiteConfiguration?Id=" + $('#myid').val() + '&entitydisplayid=' + $('#myentitydisplayid').val() + '&ptype=config&entityid=' + $('#myentityid').val();
                        } else {
                            window.location.href = "/Configuration/SubsiteConfiguration/";
                        }
                    } else {
                        getConfigStatus();
                    }

                    // return true;
                }
                else {
                    error.show();
                    error.append('<p>  Record not saved. </p>');
                    //return false;
                }
                $.unblockUI();
            });
        }, 500)
    }
}
