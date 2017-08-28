function getSubSiteOfficeBankFee(parentId) {
    var url = '/api/SubSiteOffice/SubSiteOfficeBankFee';
    if (parentId != '' && parentId != null && parentId != '00000000-0000-0000-0000-000000000000') {
        ajaxHelper(url + '?parentId=' + parentId, 'GET').done(function (data) {

            if (data != null) {
                $.each(data, function (indx, valu) {

                    if (valu["ServiceorTransmission"] == "1") {

                    }

                    if (valu["ServiceorTransmitter"] == "2") {
                        $('#hdnTotalSVBFees').val(valu["Amount"]);
                    }
                    if (valu["ServiceorTransmitter"] == "3") {
                        $('#hdnTotalTransFees').val(valu["Amount"]);
                    }

                });
            }

        });
    }
}


function fnSavesubsiteOfficeConfig(SaveType) {
    var sretval = getValidUSerorNot($('#txtEFINOwnerSite').val());

    var req = {};
    var cansubmit = true;
    var error = $('#error');
    error.html('');
    error.hide();
    var success = $('#success');
    success.html('');
    success.hide();

    req.Id = $.trim($('#ID').val());
    var hdnEFINStatus = $('#hdnEFINStatus').val();

    if ($('#entityid').val() != $('#myentityid').val()) {
        req.refId = $.trim($('#myid').val());
    } else {
        req.refId = $.trim($('#UserId').val());
    }

    //req.EFINListedOtherOffice = $('#rbEFINlistedYes').is(':checked');
    //var rbEFINlisted = $('input[name=rbEFINlisted]').is(':checked');
    //var rbEFIN = $('input[name=rbEFIN]').is(':checked');
    var rbSOorSSorEFIN = $('input[name=rbSOorSSorEFIN]').is(':checked');
    var rbTaxreturn = $('input[name=rbTaxreturn]').is(':checked');
    var rbMSO = $('input[name=rbMSO]').is(':checked');
    var rbBS = $('input[name=rbBusSw]:checked');
    var rbse = $('input[name=shEfin]:checked');

    //$('#divEFINlisted').removeClass("error_msg");
    //$('#divEFIN').removeClass("error_msg");
    $('*').removeClass("error_msg");
    $('*').attr('title', '');
    //$('#divTaxreturn').removeClass("error_msg");
    //$('#divMSO').removeClass("error_msg");

    //if (!rbEFINlisted) {
    //    $('#divEFINlisted').addClass("error_msg");
    //    $('#divEFINlisted').attr('title', '');
    //    cansubmit = false;

    //} else if (req.EFINListedOtherOffice) {
    //    if (!rbEFIN) {
    //        $('#divEFIN').addClass("error_msg");
    //        $('#divEFIN').attr('title', '');
    //        cansubmit = false;
    //    }
    //}
    debugger;
    if (rbBS.length == 0) {
        $('#dvBSW').addClass("error_msg");
        $('#dvBSW').attr('title', 'Please select Yes or No');
        cansubmit = false;
    }
    if (hdnEFINStatus != '21' && hdnEFINStatus != '20') {
        req.SiteOwnthisEFIN = true;
        req.EFINListedOtherOffice = true;
    }
    if (hdnEFINStatus != '21' && hdnEFINStatus != '20' && hdnEFINStatus != '17' && hdnEFINStatus != '18') {
        if (rbse.length == 0) {
            $('#dvSharingEfin').addClass("error_msg");
            $('#dvSharingEfin').attr('title', 'Please select Yes or No');
            cansubmit = false;
        }
    }
    else {
        if (hdnEFINStatus != '18' && hdnEFINStatus != '20') {
            req.EFINOwnerSite = $('#txtEFINOwnerSite').val();
            req.SiteOwnthisEFIN = false;
            req.EFINListedOtherOffice = true;
            if (!req.EFINOwnerSite) {
                $('#txtEFINOwnerSite').addClass("error_msg");
                $('#txtEFINOwnerSite').attr('title', 'Please enter the User ID of the EFIN owner\'s site');
                cansubmit = false;
            }
            else if (sretval == false) {
                $('#txtEFINOwnerSite').addClass("error_msg");
                $('#txtEFINOwnerSite').attr('title', 'Please enter the valid User ID of the EFIN owner\'s site');
                cansubmit = false;
            }
        }
    }
    if (!rbSOorSSorEFIN) {
        $('#divSOorSSorEFIN').addClass("error_msg");
        $('#divSOorSSorEFIN').attr('title', '');
        cansubmit = false;
    }
    if (!rbTaxreturn) {
        $('#divTaxreturn').addClass("error_msg");
        $('#divTaxreturn').attr('title', '');
        cansubmit = false;
    }
    if (!rbMSO) {
        if ($('#ismsouser').val() == 'true' || $('#ismsouser').val() == true) {
            $('#divMSO').addClass("error_msg");
            $('#divMSO').attr('title', '');
            cansubmit = false;
        }
    }

    //if (cansubmit) {
    //    //if (!req.EFINListedOtherOffice) {
    //    //    req.SiteOwnthisEFIN = false;
    //    //    req.EFINOwnerSite = "";
    //    //}
    //    //else {
    //    //    req.SiteOwnthisEFIN = $('#rbEFINYes').is(':checked');
    //    //    if (!req.SiteOwnthisEFIN) {
    //    //        req.EFINOwnerSite = $('#txtEFINOwnerSite').val();
    //    //        $('#txtEFINOwnerSite').removeClass("error_msg");
    //    //        $('#txtEFINOwnerSite').attr('title', '');
    //    //        if ($('#txtEFINOwnerSite').val() == "") {
    //    //            $('#txtEFINOwnerSite').addClass("error_msg");
    //    //            $('#txtEFINOwnerSite').attr('title', 'Please enter the User ID of the EFIN owner\'s site');
    //    //            cansubmit = false;
    //    //        }
    //    //        else {
    //    //            //var sretval = getValidUSerorNot($('#txtEFINOwnerSite').val());
    //    //            // alert(sretval);
    //    //            if (sretval == false) {
    //    //                $('#txtEFINOwnerSite').addClass("error_msg");
    //    //                $('#txtEFINOwnerSite').attr('title', 'Please enter the valid User ID of the EFIN owner\'s site');
    //    //                cansubmit = false;
    //    //            }
    //    //        }
    //    //    }
    //    //    else {
    //    //        req.EFINOwnerSite = "";
    //    //    }
    //    //    //$('#txtEFINOwnerSite').attr('title', '');
    //    //    //if ($('#txtEFINOwnerSite').val() == "") {
    //    //    //    $('#txtEFINOwnerSite').addClass("error_msg");
    //    //    //    $('#txtEFINOwnerSite').attr('title', 'Please enter the User ID of the EFIN owner(s) site');
    //    //    //    cansubmit = false;
    //    //    //}
    //    //}
    //}

    // req.SingleOfficeorMainSite = $('#rbsingmainYes').is(':checked');
    req.SOorSSorEFIN = $('input[name=rbSOorSSorEFIN]:checked').val();
    req.SubSiteSendTaxReturn = $('#rbTaxreturnYes').is(':checked');
    req.SiteanMSOLocation = $('#rbMSOYes').is(':checked');

    if (!cansubmit) {
        error.show();
        error.append('<p> Please correct the error(s). </p>');
        return false;
    }

    if (cansubmit) {
        error.hide();
        error.html('');
        req.Id = $('#ID').val();
        req.UserId = $('#UserId').val();
        req.IsBusinessSoftware = $('#rbBSYes').prop('checked');
        req.IsSharingEFIN = $('#rbseYes').prop('checked');

        var Uri = '/api/SubSiteOffice/SaveSubSiteOfficeConfig';
        ajaxHelper(Uri, 'POST', req, false).done(function (data, status) {

            if (data != '' && data != null && data != '00000000-0000-0000-0000-000000000000') {
                SaveConfigStatusActive('done');

                if ($('#myentityid').val() == $('#Entity_SO').val() || $('#myentityid').val() == $('#Entity_SOME').val()) {
                    checkSOSOMEActivation();
                }

                //ResetSubSiteActivation();
                UpdateOfficeManagement(req.refId);
                $('#ID').val(data);
                success.show();
                success.append('<p> Sub-Site Office Configuration saved Successfully </p>');
                if (SaveType == 1) {
                    if ($('#entityid').val() != $('#myentityid').val()) {
                        window.location.href = "/SubSiteOfficeConfiguration/SubSiteOfficeFeeConfig?Id=" + $('#myid').val() + "&ParentId=" + $('#myparentid').val() + '&entitydisplayid=' + $('#myentitydisplayid').val() + '&ptype=subconfig&entityid=' + $('#myentityid').val();
                    } else {
                        window.location.href = "/SubSiteOfficeConfiguration/SubSiteOfficeFeeConfig";
                    }
                } else {
                    getConfigStatus();
                }

                return true;
            }
            else {
                error.show();
                error.append('<p> Sub-Site Office Configuration not saved. </p>');
                return false;
            }
        });
    }
}

function getValidUSerorNot(sendVal) {

    if (sendVal != '' && sendVal != undefined && sendVal != null) {
        var isexist = false;
        var ownuser = "";
        if ($('#entityid').val() != $('#myentityid').val()) {

            ownuser = localStorage.getItem("ch_EMPUserId");
            var custmorUri = '/api/SubSiteOffice/ValidEFINUserFromuTax?OwnID=' + ownuser + '&ParentID=' + sendVal + '&CustomerId=' + $('#myid').val();

            ajaxHelper(custmorUri, 'GET', null, false).done(function (data) {
                isexist = data;
            });
            return isexist;
        } else {

            ownuser = $('#UserName').val();

            var custmorUri = '/api/SubSiteOffice/ValidEFINUser?OwnID=' + ownuser + '&ParentID=' + sendVal + '&CustomerId=' + $('#id').val();

            ajaxHelper(custmorUri, 'GET', null, false).done(function (data) {
                isexist = data;
            });

            return isexist;
        }
    }
}

///Customer Related Methods
function GetCustomerNotesInfo(continer) {
    var Id;
    if ($('#entityid').val() != $('#myentityid').val()) {
        Id = $('#myid').val();
    } else {
        Id = $('#UserId').val();
    }
    var url = '/api/SubSiteOffice/GetCustomerNote';
    if (Id != '' && Id != null && Id != '00000000-0000-0000-0000-000000000000') {
        ajaxHelper(url + '?Id=' + Id, 'GET').done(function (data) {
            if (data != null && data != '' && data != undefined) {
                //$('#ID').val(data["Id"]);
                var $ul = $('<table/>').addClass('table table-striped table-bordered table-hover');
                $ul.append('<tr> <th style="width:80%">Notes</th><th style="width:20%">Date</th> </tr>');
                $.each(data, function (index, value) {
                    $ul.append('<tr> <td>' + value["Note"] + ' </td><td> ' + value["CreatedDate"] + ' </td> </tr>');
                });
                continer.append($ul);
            }
        });
    }
}

function fnSaveCustomerNotes(SaveType) {
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
    req.Note = $('#txtNotes').val();
    $('#txtNotes').removeClass("error_msg");
    if ($('#txtNotes').val() == "") {
        $('#txtNotes').addClass("error_msg");
        $('#txtNotes').attr('title', 'Please enter Customer Notes');
        cansubmit = false;
    }

    if (!cansubmit) {
        error.show();
        error.append('<p> Please correct the error(s). </p>');
        return false;
    }
    if (cansubmit) {
        error.hide();
        error.html('');
        req.Id = $('#ID').val();
        req.UserId = $('#UserId').val();
        var Uri = '/api/SubSiteOffice/SaveCustomerNotes';
        ajaxHelper(Uri, 'POST', req, false).done(function (data, status) {
            if (data != '' && data != null && data != '00000000-0000-0000-0000-000000000000') {
                $('#ID').val(data);
                success.show();
                success.append('<p> Customer Notes saved Successfully </p>');

                SaveConfigStatusActive('done');

                if (SaveType == 1) {
                    if ($('#entityid').val() != $('#myentityid').val()) {
                        window.location.href = "/SubSiteOfficeConfiguration/ActivateMyAccount?Id=" + $('#myid').val() + "&ParentId=" + $('#myparentid').val() + '&entitydisplayid=' + $('#myentitydisplayid').val() + '&ptype=subconfig&entityid=' + $('#myentityid').val();
                    } else {
                        window.location.href = "/SubSiteOfficeConfiguration/ActivateMyAccount/";
                    }
                } else {
                    getConfigStatus();
                }

                $('#txtNotes').val('');
                $('#dvcustomernotes').html('');
                GetCustomerNotesInfo($('#dvcustomernotes'));
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

function getCustomerInformation(Id) {
    var url = '/api/CustomerInformation/';
    if (Id != '' && Id != null && Id != '00000000-0000-0000-0000-000000000000') {
        ajaxHelper(url + '?id=' + Id, 'GET').done(function (data) {
            if (data) {
                $('#Id').val(Id);
                $('#CompanyName').val(data["CompanyName"]);
                $('#AccountStatus').val(data["AccountStatus"]);
                $('#SalesforceParentID').val(data["SalesforceParentID"]);
                $('#MasterIdentifier').val(data["MasterIdentifier"]);
                $('#BusinessOwnerFirstName').val(data["BusinessOwnerFirstName"]);
                $('#OfficePhone').val(data["OfficePhone"]);
                $('#AlternatePhone').val(data["AlternatePhone"]);
                $('#Primaryemail').val(data["Primaryemail"]);
                $('#SupportNotificationemail').val(data["SupportNotificationemail"]);
                $('#EROType').val(data["EROType"]);
                $('#AlternativeContact').val(data["AlternativeContact"]);
                $('#EFIN').val(data["EFIN"]);
                $('#PhysicalAddress1').val(data["PhysicalAddress1"]);
                $('#PhysicalAddress2').val(data["PhysicalAddress2"]);
                $('#PhysicalZipcode').val(data["PhysicalZipcode"]);
                $('#PhysicalCity').val(data["PhysicalCity"]);
                //$('#PhysicalState').val(data["PhysicalState"]);
                $('#PhysicalStateCode').val(data["PhysicalState"]);
                $('#ShippingAddressSameAsPhysicalAddress').prop('checked', false);
                var shippingSameAsPhysical = data["ShippingAddressSameAsPhysicalAddress"];
                if (shippingSameAsPhysical == 'true' || shippingSameAsPhysical == 'True' || shippingSameAsPhysical == true) {
                    $('#ShippingAddressSameAsPhysicalAddress').prop('checked', true);
                }

                $('#ShippingAddress1').val(data["ShippingAddress1"]);
                $('#ShippingAddress2').val(data["ShippingAddress2"]);
                $('#ShippingZipcode').val(data["ShippingZipcode"]);
                $('#ShippingCity').val(data["ShippingCity"]);
                //$('#ShippingState').val(data["ShippingState"]);
                $('#ShippingStateCode').val(data["ShippingState"]);



                $('#PhoneTypeId').val(data['PhoneTypeId']);
                $('#TitleId').val(data['TitleId']);

                getEFINStatus($('#EFINStatus'), data['EntityId']);
                $('#EFINStatus').val(data['EFINStatus']);
                $('#EFIN').val(data["EFIN"]);
                if (data['EFINStatus'] == '16' || data['EFINStatus'] == '19') {
                    $('#EFINStatus').attr('readonly', 'readonly');
                    //$('#EFIN').attr('readonly', 'readonly');
                    $('#EFINStatus').css('pointer-events', 'none');
                    $('#EFINStatus').prop('disabled', true);
                }
                // getCountry();
                // getState($('#PhysicalState'), $('#ShippingState'));
                // getCity($('#PhysicalCity'), $('#ShippingCity'));
                // getZipCode($('#PhysicalZipcode'), $('#ShippingZipcode'));
            }
        });
        // SaveConfigStatusActive('done');
    }
}

//function getPhoneType(container) {

//    var custmorUri = '/api/dropdown/phonetype';
//    ajaxHelper(custmorUri, 'GET').done(function (data) {
//        container.append($('<option />', { value: '', text: 'Select' }));
//        $.each(data, function (rowIndex, r) {
//            container.append($('<option />', { value: r["Id"], text: r["Name"] }));
//        });
//    });
//   // container.val(valu);
//}


//function getAlternativeTitle(container, valu) {
//    var custmorUri = '/api/dropdown/Alternativetitle';
//    ajaxHelper(custmorUri, 'GET').done(function (data) {
//        container.append($('<option />', { value: '', text: 'Select' }));
//        $.each(data, function (rowIndex, r) {
//            container.append($('<option />', { value: r["Id"], text: r["Name"] }));
//        });
//    });
//    container.val(valu);
//}

function fnValidConfigStatus() {
    var canSubmit = true;
    var req = {};
    var error = $('#error');
    error.html('');
    error.hide();
    var success = $('#success');
    success.html('');
    success.hide();

    if (!SVBFeesValid()) {
        canSubmit = false;
    }


    if (!TransFeesValid()) {
        canSubmit = false;
    }

    if (!canSubmit) {
        error.show();

        error.append('<p>One or more error(s) have been identified in the Service Bureau and\or Transmission Fee sections. Please have the same resolved to proceed to activate your account.</p>');
        return false;
    }

    if (canSubmit) {
        $('#bank-confimation').modal('show');
    }
}
///till here

function fnSaveConfigStatus() {

    //  var h_subsiteId = $('#h_subsiteId').val();
    var IsActivate = false;

    $.blockUI({ message: '<img src="../../content/images/loading-img.gif"/>' });
    setTimeout(function () {

        if ($('#entityid').val() != $('#myentityid').val()) {
            var StatusCode = $('#StatusCode').val();
            if (StatusCode == "INP") {
                var Status = fnActiveCuseromerInfo();
                IsActivate = true;
            }
        }


        if (!IsActivate) {
            var error = $('#error');
            error.html('');
            error.hide();
            var success = $('#success');
            success.html('');
            success.hide();

            var req = {};
            req.UserId = $.trim($('#UserId').val());
            req.CustomerId = $.trim($('#UserId').val());

            var UserId = $.trim($('#UserId').val());
            var CustomerId = $.trim($('#UserId').val());

            var parentid = $('#ParentId').val();
            if ($('#entityid').val() != $('#myentityid').val()) {
                parentid = $('#myparentid').val();

                req.CustomerId = $('#myid').val();
                CustomerId = $.trim($('#myid').val());
            }


            var Uri = '/api/CustomerInformation/CustomerConfig?CustomerId=' + CustomerId + '&UserId=' + UserId + '&SiteMapID=7A2C166C-C2EF-47C0-AA5F-E4950F9FF369';
            ajaxHelper(Uri, 'POST', null, false).done(function (data) {

                $("html, body").animate({ scrollTop: 0 }, "slow");

                if (data > 0) {
                    var message = '';
                    if (data == 1)
                        message = '<p> Activated this information saved Successfully </p>';
                    else if (data == 2)
                        message = '<p> Activated this information saved Successfully </p><p> New Crosslink and EMP User has been created. </p>';
                    else if (data == 3)
                        message = '<p> Activated this information saved Successfully</p><p>Crosslink and EMP User has not created. It will be availble in New Customer Signup</p>'
                    else if (data == 4) {                        
                        message = '<p> User account activation processed and saved sucessfully.</p><p>Crosslink UserId has been created. But Password has not created. It will be availble in New Customer Signup</p>'
                        error.show();
                        error.append(message);
                    }
                    if (data != 4) {
                        success.show();
                        success.append(message);
                    }
                    UpdateOfficeManagement(CustomerId);


                    $("html, body").animate({ scrollTop: 0 }, "slow");
                    if (data == 1) {
                        if ($('#entityid').val() != $('#myentityid').val()) {
                            window.location.href = "/SubSiteOfficeConfiguration/Dashboard?Id=" + CustomerId + "&ParentId=" + parentid + "&entitydisplayid=" + $('#myentitydisplayid').val() + '&ptype=subconfig&entityid=' + $('#myentityid').val();
                        } else {
                            window.location.href = "/SubSiteOfficeConfiguration/Dashboard";
                        }
                    }

                    //getSubSiteConfigStatus();
                    //return true;
                }
                else {
                    error.show();
                    error.append('<p>  Record not saved. </p>');
                    // return false;
                }
            });
        }
        $.unblockUI();
    }, 500);
}


function getMainSiteFeeInformation(Id) {
    $('#divSVBSave').show();
    $('#divTansmSave').show();
    $('#spanNextLink').show();

    var url = '/api/SubSiteFee/';
    if (Id != '' && Id != null && Id != '00000000-0000-0000-0000-000000000000') {
        ajaxHelper(url + '?Id=' + Id, 'GET', null, false).done(function (data) {

            $.each(data, function (rowIndex, r) {

                if (r["ServiceorTransmission"] == 1) {

                    if (r["IsAddOnFeeCharge"] == true) {

                    }
                    else {
                        $('#divSVBSave').hide();
                        $('#dvServiceBureau input[type=text]').attr("disabled");
                        $('#dvServiceBureau input[type=radio]').attr("disabled");
                    }

                    if (r["IsSameforAll"] == true) {
                        $('#divSVBSave').hide();
                        $('#dvServiceBureau input[type=text]').attr("disabled");
                        $('#dvServiceBureau input[type=radio]').attr("disabled");
                    }
                    else {
                        $('#spanNextLink').hide();
                        $('#dvServiceBureau input[type=text]').removeAttr("disabled");
                        $('#dvServiceBureau input[type=radio]').removeAttr("disabled");
                        $('#dvServiceBureau input[type=text]').val('');
                    }

                    if (r["IsSubSiteAddonFee"] == true) {

                    }
                    else {

                    }
                }
                else {

                    if (r["IsAddOnFeeCharge"] == true) {

                    }
                    else {
                        $('#divTansmSave').hide();
                        $('#dvTransmitter input[type=text]').attr("disabled");
                        $('#dvTransmitter input[type=radio]').attr("disabled");
                    }

                    if (r["IsSameforAll"] == true) {
                        $('#divTansmSave').hide();
                        $('#dvTransmitter input[type=text]').attr("disabled");
                        $('#dvTransmitter input[type=radio]').attr("disabled");
                    }
                    else {
                        $('#spanNextLink').hide();
                        $('#dvTransmitter input[type=text]').removeAttr("disabled");
                        $('#dvTransmitter input[type=radio]').removeAttr("disabled");
                        $('#dvTransmitter input[type=text]').val('');
                    }

                    if (r["IsSubSiteAddonFee"] == true) {

                    }
                    else {

                    }
                }
            });
        });
    }
}



