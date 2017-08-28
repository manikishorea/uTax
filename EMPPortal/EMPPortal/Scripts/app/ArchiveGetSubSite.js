function ArchiveSubSite(Id, parentid) {
    // var Id = $('#UserId').val();
    var url = '/api/Archive/SubSite?Id=' + Id + '&ParentId=' + parentid;
    if (Id != '' && Id != null && Id != '00000000-0000-0000-0000-000000000000') {
        ajaxHelper(url, 'GET').done(function (data) {
            getArchiveSubsiteOffice(data.SubSiteOfficeDTO);
            getSubsiteBankAndQuestions($('#dvServiceBureau'), $('#dvTransmitter'));
            GetArchiveSubSiteBankFee(data.SubSiteBankFeesDTOs);
            getArchivMainSiteFeeConfig(data.SubSiteFeeDTOs);
        });
    }
}

function getArchiveSubsiteOffice(data) {

    if (data != null && data != '' && data != undefined) {

        //if (data["iIsSubSiteSendTaxReturn"] == 1) {
        //    // $('#rbTaxreturnNo').attr('disabled', 'disabled');
        //    $('#rbTaxreturnYes').prop('checked', 'checked');
        //}
        //else if (data["iIsSubSiteSendTaxReturn"] == 2) {
        //    // $('#rbTaxreturnYes').attr('disabled', 'disabled');
        //    $('#rbTaxreturnNo').prop('checked', 'checked');
        //}
        //else if (data["iIsSubSiteSendTaxReturn"] == 3) {
        //    $('#rbTaxreturnYes, #rbTaxreturnNo').removeAttr('disabled');
        //}
        $('#ID').val(data["Id"]);

        if (data["EFINListedOtherOffice"] == true || data["EFINListedOtherOffice"] == 'true') {
            $("#dbsubquestions").show();
            $('#dvEFINOwner').hide();

            $('#rbEFINlistedYes').prop('checked', true);

            if (data["SiteOwnthisEFIN"] == true || data["SiteOwnthisEFIN"] == 'true') {
                $('#rbEFINYes').prop('checked', true);
               
            }
            else if (data["SiteOwnthisEFIN"] == false || data["SiteOwnthisEFIN"] == 'false') {
                $('#rbEFINNo').prop('checked', true);
                $('#txtEFINOwnerSite').val(data["EFINOwnerSite"]);
                $('#dvEFINOwner').show();
            }
        }
        else if (data["EFINListedOtherOffice"] == false || data["EFINListedOtherOffice"] == 'false') {
            $('#rbEFINlistedNo').prop('checked', true);
            $("#dbsubquestions").hide();
        }

        var SOorSSorEFIN = data["SOorSSorEFIN"];
        if (SOorSSorEFIN != null && SOorSSorEFIN != '') {
            if (SOorSSorEFIN == 1) {
                $("#rbNewSO").prop("checked", true);
            }
            else if (SOorSSorEFIN == 2) {
                $("#rbNewSS").prop("checked", true);
            }
            else {
                $("#rbNewSO").prop("disabled", true);
                $("#rbNewSS").prop("disabled", true);
                $("#rbAdditionalEFIN").prop("checked", true);
            }
        }

        if (data["SubSiteSendTaxReturn"] == true || data["SubSiteSendTaxReturn"] == 'true') {
            $('#rbTaxreturnYes').prop('checked', true);
        }
        else if (data["SubSiteSendTaxReturn"] == false || data["SubSiteSendTaxReturn"] == 'false') {
            $('#rbTaxreturnNo').prop('checked', true);
        }

        if (data["SiteanMSOLocation"] == true || data["SiteanMSOLocation"] == 'true') {
            $('#rbMSOYes').prop('checked', true);
        }
        else if (data["SiteanMSOLocation"] == false || data["SiteanMSOLocation"] == 'false') {
            $('#rbMSONo').prop('checked', true);
        }

        ////New Field
        if (data["SiteanMSOLocation"] == true || data["SiteanMSOLocation"] == 'true') {
            $('#issubsitemsouser').val(1);
        }
        else if (data["SiteanMSOLocation"] == false || data["SiteanMSOLocation"] == 'false') {
            $('#issubsitemsouser').val(0);
        }
    } else {
        $('#dvOfficeConfig')
            .html('')
            .html('No Record Found');
    }

}

///// Sub Site Fee Setup

function GetArchiveSubSiteBankFee(data) {

    var SVBScreen = false;
    var TransScreen = false;
    if (data != null && data != '' && data != undefined) {
        $.each(data, function (coIndex, c) {

            var bankid = c["BankID"];
            if (c["ServiceorTransmitter"] == 1) {
                $('#rb_' + c["QuestionID"]).attr('checked', 'checked');

                $('#txt_' + bankid).val(c["AmountDSK"]);
                $('#txt_' + bankid + '_MSO').val(c["AmountMSO"]);
                SVBScreen = true;
            }
            else {
                $('#rbtrn_' + c["QuestionID"]).attr('checked', 'checked');
                $('#txttrns_' + bankid).val(c["AmountDSK"]);
                $('#txttrns_' + bankid + '_MSO').val(c["AmountMSO"]);
                TransScreen = true;
            }
        });
    } else {
        $('#SubOfficeSVBFee_Act')
            .html('')
            .html('No Record Found');

        $('#SubOfficeTranFee_Act')
           .html('')
           .html('No Record Found');
    }

    if (!SVBScreen)
    {
        $('#SubOfficeSVBFee_Act')
         .html('')
         .html('No Record Found');
    }

    if (!TransScreen) {
        $('#SubOfficeTranFee_Act')
         .html('')
         .html('No Record Found');
    }
}


///Main Fee Config


function getArchivMainSiteFeeConfig(data) {

    $('#spanSvbTansSave').show();
    $('#spanNextLink').hide();
    var IsNextVisibleOne = false;
    var IsNextVisibleTwo = false;
    $('#hdnTranNoBank').val(0);

    $('#spanSvbTansSave').show();
    $('#hdnSVBVisible').val(0);
    $('#hdnTranVisible').val(0);

    if (data != '' && data != null && data != undefined) {

        $.each(data, function (rowIndex, r) {
            if (r["ServiceorTransmission"] == 1) {

                if (r["IsAddOnFeeCharge"] == true || r["IsAddOnFeeCharge"] == 'true') {
                    //$('input[type=text].ServiceBank').val('0');
                    $('#hdnSVBVisible').val(1);
                }
                else if (r["IsAddOnFeeCharge"] == 'false' || r["IsAddOnFeeCharge"] == false) {
                    $('#hdnSVBNoBank').val(1);
                    $('#dvNotDataPageSVB').show();
                    $('#divSVB').hide();
                    $('#divSVBSave').hide();
                    $('#dvServiceBureau input[type=text]').attr("readonly", "readonly");
                    // $('#dvServiceBureau input[type=radio]').attr("disabled", "disabled");
                    IsNextVisibleOne = true;


                }


                if (r["IsAddOnFeeCharge"] == true || r["IsAddOnFeeCharge"] == 'true') {

                    if (r["IsSameforAll"] == true) {
                        // $('#hdnSVBNoBank').val(1);
                        // $('#divSVBSave').hide();
                        $('#dvServiceBureau input[type=text]').attr("readonly", "readonly");
                        // $('#dvServiceBureau input[type=radio]').attr("disabled", "disabled");
                    }
                    else {
                        //  $('#dvServiceBureau input[type=text]').removeAttr("readonly");
                        //   $('#dvServiceBureau input[type=radio]').removeAttr("disabled");
                        //  $('#dvServiceBureau input[type=text]').val('');
                        // $('input[type=text].ServiceBank').val('');
                    }
                }

                if (r["IsSubSiteAddonFee"] == true) {
                }
                else {
                }
            }
            else {

                if (r["IsAddOnFeeCharge"] == true || r["IsAddOnFeeCharge"] == 'true') {
                    //$('input[type=text].TransmitterBank').val('0');
                    $('#hdnTranVisible').val(1);
                }
                else if (r["IsAddOnFeeCharge"] == 'false' || r["IsAddOnFeeCharge"] == false) {

                    $('#dvNotDataPageTrans').show();
                    $('#divTrans').hide();


                    $('#divTansmSave').hide();
                    $('#hdnTranNoBank').val(1);
                    $('#dvTransmitter input[type=text]').attr("readonly", "readonly");
                    // $('#dvTransmitter input[type=radio]').attr("disabled", "disabled");
                    $('#dvTransmitter input[type=text]').val('0');
                    IsNextVisibleTwo = true;
                }

                if (r["IsAddOnFeeCharge"] == true || r["IsAddOnFeeCharge"] == 'true') {

                    if (r["IsSameforAll"] == true) {
                        // $('#divTansmSave').hide();
                        // $('#hdnTranNoBank').val(1);
                        $('#dvTransmitter input[type=text]').attr("readonly", "readonly");
                        //$('#dvTransmitter input[type=radio]').attr("disabled", "disabled");
                    }
                    else {
                        //$('#dvTransmitter input[type=text]').removeAttr("readonly");
                        // $('#dvTransmitter input[type=radio]').removeAttr("disabled");
                        // $('input[type=text].TransmitterBank').val('');
                    }
                }


                if (r["IsSubSiteAddonFee"] == true) {

                }
                else {

                }
            }
        });

    }
}
