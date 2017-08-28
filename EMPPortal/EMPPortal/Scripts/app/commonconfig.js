function getSubsiteOfficeInformation(Id, parentId) {
    //var Id = $('#UserId').val();

    var _entityid;
    if ($('#entityid').val() != $('#myentityid').val()) {
        _entityid = $('#myentityid').val();
    } else {
        _entityid = $('#entityid').val();
    }

    var url = '/api/SubSiteOffice/SubSiteOfficeConfig';
    if (Id != '' && Id != null && Id != '00000000-0000-0000-0000-000000000000') {
        ajaxHelper(url + '?Id=' + Id + '&parentId=' + parentId, 'GET').done(function (data) {
            if (data != null && data != '' && data != undefined) {

                if (data["iIsSubSiteSendTaxReturn"] == 1) {
                    // $('#rbTaxreturnNo').attr('disabled', 'disabled');
                    $('#rbTaxreturnYes').prop('checked', 'checked');
                }
                else if (data["iIsSubSiteSendTaxReturn"] == 2) {
                    // $('#rbTaxreturnYes').attr('disabled', 'disabled');
                    $('#rbTaxreturnNo').prop('checked', 'checked');
                }
                else if (data["iIsSubSiteSendTaxReturn"] == 3) {
                    if (window.location.href.toLowerCase().indexOf('activatemyaccount') == -1)
                        $('#rbTaxreturnYes, #rbTaxreturnNo').removeAttr('disabled');
                }

                $('#ID').val(data["Id"]);

                if (data["EFINListedOtherOffice"] == true || data["EFINListedOtherOffice"] == 'true') {
                    $('#rbEFINlistedYes').prop('checked', true);
                    localStorage.removeItem("EFINOwnerUserId")

                    if (data["SiteOwnthisEFIN"] == true || data["SiteOwnthisEFIN"] == 'true') {
                        localStorage.removeItem("EFINOwnerUserId")
                        $('#rbEFINYes').prop('checked', true);
                    }
                    else if (data["SiteOwnthisEFIN"] == false || data["SiteOwnthisEFIN"] == 'false') {

                        localStorage.setItem("EFINOwnerUserId", true);

                        $('#rbEFINNo').prop('checked', true);

                        $('#txtEFINOwnerSite').val(data["EFINOwnerSite"]);
                    }
                    $("#dbsubquestions").show();
                }
                else if (data["EFINListedOtherOffice"] == false || data["EFINListedOtherOffice"] == 'false') {
                    $('#rbEFINlistedNo').prop('checked', true);
                    $("#dbsubquestions").hide();
                    localStorage.removeItem("EFINOwnerUserId")
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
                        if ($('#ActiveMyAccountStatus').val() == '1' || $('#ActiveMyAccountStatus').val() == 1) {
                            $("#rbNewSO").prop("disabled", true);
                            $("#rbNewSS").prop("disabled", true);
                        }

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

                if (data.IsBusinessSoftware)
                    $('#rbBSYes').attr('checked', true);
                else
                    $('#rbBSNo').attr('checked', true);

                if (!data.Id && data.IsBusinessSoftware && _entityid == 2) {
                    $('#rbBSYes').attr('checked', true);
                }


                var hdnEFINStatus = $('#hdnEFINStatus').val();
                if (hdnEFINStatus != '21' && hdnEFINStatus != 20 && hdnEFINStatus != '17') {
                    if (data.IsSharingEFIN)
                        $('#rbseYes').attr('checked', true);
                    else
                        $('#rbseNo').attr('checked', true);
                }

                if (hdnEFINStatus == "21") {
                    $('#dvEFINOwner').removeClass('hide');
                    if (!$('#dvShareEfinQtn').hasClass('hide'))
                        $('#dvShareEfinQtn').addClass('hide');
                }
                if (hdnEFINStatus == "18" || hdnEFINStatus == "20" || hdnEFINStatus == "17") {
                    $('#dvShareEfinQtn').addClass('hide');
                }
            }

            if (data.Id == null || data.Id == undefined) {
                SubSiteSOChecked();
            }

            funEFINInfo();
        });
    }
}

var _myaccountID = '';

function getAccountId() {
    var Id = '';
    if ($('#entityid').val() != $('#myentityid').val()) {
        Id = $('#myid').val();
    }
    else {
        Id = $('#UserId').val();
    }

    ajaxHelper('/api/SubSiteOffice/GetAccountId?Id=' + Id, 'GET', null, false)
        .done(function (res) {
            _myaccountID = res;
        })
}

function SubSiteSOChecked() {

    if ($('#SalesforceOpportunityID').val()) {
        if ($('#myentityid').val() == $('#Entity_SVB_SO').val() || $('#myentityid').val() == $('#Entity_MO_SO').val() || $('#myentityid').val() == $('#Entity_SVB_MO_SO').val()) {

            $('#rbNewSO').prop('checked', true);

            $('#rbNewSS')
                .prop('disabled', true)
                .prop('checked', false);

            $('#rbAdditionalEFIN')
              .prop('disabled', true)
              .prop('checked', false);
        }

        if ($('#myentityid').val() == $('#Entity_SVB_MO').val()) {

            $('#rbNewSO')
                .prop('disabled', true)
                .prop('checked', false);

            $('#rbNewSS').prop('checked', true);


            $('#rbAdditionalEFIN')
              .prop('disabled', true)
              .prop('checked', false);
        }
    }
}