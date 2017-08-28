//Dash Board functionality
function GetSubSiteCustomerInfomationForMainOffice(Id) {
    var url = '/api/CustomerInformation';
    if (Id != '' && Id != null && Id != '00000000-0000-0000-0000-000000000000') {
        ajaxHelper(url + '?id=' + Id, 'GET').done(function (data) {

            localStorage.setItem("spHead", data["CompanyName"]);

            $('#IsVerified').val(data["IsVerified"]);
            localStorage.setItem("IsVerified", data["IsVerified"]);

            $('#spCompanyName').text(data["CompanyName"]);
            $('#spBusinessOwnerName').text(data["BusinessOwnerFirstName"]);
            $('#spBusinessOwnerLastName').text(data["BusinessOwnerLastName"]);
            $('#spPhysicalAddress').text(data["PhysicalAddress1"]);
            $('#spCitystatezip').text(data["PhysicalCity"] + ' ' + data["PhysicalState"] + ', ' + data["PhysicalZipcode"]);
            $('#spOfficePhone').text(data["OfficePhone"]);
            $('#spAlternatePhone').text(data["AlternatePhone"]);
            $('#spPrimaryemail').text(data["Primaryemail"]);
            $('#spSupportemail').text(data["SupportNotificationemail"]);
            $('#spAlternativeContact').text(data["AlternativeContact"]);
            $('#ismsouser').val(data["IsMSOUser"]);

            if (data["PhoneType"] == null)
                $('#spphoneType').text('');
            else
                $('#spphoneType').text(data["PhoneType"]);
            $('#spContactType').text(data["ContactTitle"]);


            fnParentSubSiteInformation(data["ParentId"]);

            $('#ActiveMyAccountStatus').val(data["IsActivationCompleted"]);

            $('#StatusCode').val(data["StatusCode"]);
            $('#ActivationParentId').val(data["ParentId"]);

            $('#spEFIN').html(data["EFINStatusText"]);

            // fnVerifiedLinksStatus();
            if (data.IsVerified == false || data.IsVerified == 'false' || data.IsVerified == 'False') {
                $('#btn_next').remove();
                $('#btn_edit').text('Verify');
            } else {
                $('#btn_next').show();
                $('#btn_edit').text('Edit');
            }

            if (data.StatusCode == 'ACT' && ($('#entityid').val() == '5' || $('#entityid').val() == '9' || $('#entityid').val() == '1') && !data.IsEnrollSubmitted) {
                $('#a_hold').toggleClass('hide');
                if (data["IsHold"]) {
                    $('#a_hold').text('Site on Hold');
                }
            }

            getIsEnrollmentSubmit();

        });

        // SaveConfigStatusActive('done');


        var url1 = '/api/CustomerLoginInformation';
        ajaxHelper(url1 + '?Id=' + Id, 'GET').done(function (data2) {

            //  $('#spEFIN').text(data2["EFIN"]);
            $('#spMasterident').text(data2["MasterIdentifier"]); // Master ID
            $('#spMasterUserID').text(data2["CrossLinkUserId"]); // User ID
            $('#spTransmissionpwd').text(data2["CrossLinkPassword"]); // Transmission Password
            $('#spofficeportalusername').text(data2["TaxOfficeUsername"]);
            $('#spoofficeportalpwd').text(data2["TaxOfficePassword"]);

            // $('#spemppwd').spemppwd(data["EMPPassword"]);
        });
    }

}

function fnParentSubSiteInformation(ParentID) {

    if (ParentID != null && ParentID != undefined && ParentID != '' && ParentID != '00000000-0000-0000-0000-000000000000') {
        localStorage.removeItem("ch_EMPUserId");
        //var Id = $('#parentid').val();
        var url1 = '/api/CustomerLoginInformation';
        ajaxHelper(url1 + '?Id=' + ParentID, 'GET').done(function (data) {
            $('#p_spEFIN').html(data["EFINStatusText"]);
            $('#p_spCompanyName').text(data["CompanyName"]);
            $('#p_spOwnerName').text(data["BusinessOwnerFirstName"]);
            $('#p_spOwnerLastName').text(data["BusinessOwnerLastName"]);
            $('#p_spPhysicalAddress').text(data["PhysicalAddress1"]);
            $('#p_spCityStateZip').text(data["CityStateZip"]);
            // $('#ismsouser').val(data["IsMSOUser"]);
            localStorage.setItem("ismsouser", data["IsMSOUser"]);
            localStorage.setItem("ch_EMPUserId", data["EMPUserId"]);
        });
    }
    else {
        $('#divParentAccountPanel').hide();
    }
}

function fnParentSubSite_Information(ParentID) {
    var url1 = '/api/CustomerLogin/ParentCustomerInfo';
    ajaxHelper(url1 + '?Id=' + ParentID, 'GET').done(function (data) {

        //localStorage.setItem("spHead", data["CompanyName"]);
        $('#sp_Head').text(data["CompanyName"]);

        $('#lblMasterID').text(data["MasterIdentifier"]);
        $('#lblParentAccount').text(data["SalesforceParentID"]);
        $('#lblTransmitType').html(data["TransmitType"]);
        $('#lblMSO').text((data["IsMSOUser"] == "false" || data["IsMSOUser"] == false) ? "No" : "Yes");
        $('#lblBank').html(data["Bank"]);
        $('#lblEFIN').text(data["EFIN"]);


        getEFINStatus($('#EFINStatus'), $('#Entity_SVB_SO').val());

        if (data["IsAdditionalEFINAllowed"] == true || data["IsAdditionalEFINAllowed"] == 'true' || data["IsAdditionalEFINAllowed"] == 'True') {
            $('input[type=text]').attr('readonly', 'readonly');
            $('input[type=checkbox]').attr('disabled', 'disabled');
            $('select').attr('disabled', 'disabled');
            $('input[type=text]#EFIN').removeAttr('readonly');

            $('#AdditionalEFIN').val(data["IsAdditionalEFINAllowed"]);
            getCustomerInformation_CreateSubSite(ParentID);
            $('#EFINStatus').removeAttr('disabled', 'disabled');


        }
    });
}

function getCustomerInformation_CreateSubSite(Id) {
    var url = '/api/CustomerInformation/';
    if (Id != '' && Id != null && Id != '00000000-0000-0000-0000-000000000000') {
        ajaxHelper(url + '?id=' + Id, 'GET').done(function (data) {

            //$('#Id').val(Id);

            $('#mygrandparentId').val(data["ParentId"]);
            $('#CompanyName').val(data["CompanyName"]);
            $('#AccountStatus').val(data["AccountStatus"]);
            $('#SalesforceParentID').val(data["SalesforceParentID"]);
            $('#MasterIdentifier').val(data["MasterIdentifier"]);
            $('#BusinessOwnerFirstName').val(data["BusinessOwnerFirstName"]);
            $('#BusinessOwnerLastName').val(data["BusinessOwnerLastName"]);
            $('#OfficePhone').val(data["OfficePhone"]);
            $('#AlternatePhone').val(data["AlternatePhone"]);
            $('#Primaryemail').val(data["Primaryemail"]);
            $('#SupportNotificationemail').val(data["SupportNotificationemail"]);
            $('#EROType').val(data["EROType"]);
            $('#AlternativeContact').val(data["AlternativeContact"]);
            // $('#EFIN').val(data["EFIN"]);
            $('#PhysicalAddress1').val(data["PhysicalAddress1"]);
            $('#PhysicalAddress2').val(data["PhysicalAddress2"]);
            $('#PhysicalZipcode').val(data["PhysicalZipcode"]);
            $('#PhysicalCity').val(data["PhysicalCity"]);
            $('#PhysicalStateID').val(data["PhysicalState"]);
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
            $('#ShippingStateID').val(data["ShippingState"]);

            $('#TitleId').val(data["TitleId"]);
            $('#AlternativeTypeId').val(data["AlternativeType"]);
            $('#PhoneTypeId').val(data["PhoneTypeId"]);

            $('#myentityid').val(data.EntityId);
            $('#AlternatePhoneTypeId').val(data["AlternativePhoneType"]);


        });
    }
}

function fnSaveSubSite_CustomerInformation() {
    $('select').removeAttr('disabled');
    $('#EFINStatus').prop('disabled', false);
    var req = {};
    var cansubmit = true;
    var error = $('#error');
    error.html('');
    error.hide();
    var success = $('#success');
    success.html('');
    success.hide();

    req.CompanyName = $.trim($('#CompanyName').val());
    // req.AccountStatus = $.trim($('#AccountStatus').val());
    req.SalesforceParentID = $.trim($('#SalesforceParentID').val());
    req.MasterIdentifier = $.trim($('#MasterIdentifier').val());
    req.BusinessOwnerFirstName = $.trim($('#BusinessOwnerFirstName').val());
    req.BusinessOwnerLastName = $.trim($('#BusinessOwnerLastName').val());
    req.OfficePhone = $.trim($('#OfficePhone').val());
    req.AlternatePhone = $.trim($('#AlternatePhone').val());
    req.Primaryemail = $.trim($('#Primaryemail').val());
    req.SupportNotificationemail = $.trim($('#SupportNotificationemail').val());
    req.EROType = $.trim($('#EROType').val());
    req.AlternativeContact = $.trim($('#AlternativeContact').val());
    req.EFIN = $.trim($('#EFIN').val());
    req.PhysicalAddress1 = $.trim($('#PhysicalAddress1').val());
    req.PhysicalAddress2 = $.trim($('#PhysicalAddress2').val());
    req.PhysicalZipcode = $.trim($('#PhysicalZipcode').val());
    req.PhysicalCity = $.trim($('#PhysicalCity').val());
    req.PhysicalState = $.trim($('#PhysicalStateID').val());
    req.ShippingAddressSameAsPhysicalAddress = $('#ShippingAddressSameAsPhysicalAddress').is(':checked');
    req.ShippingAddress1 = $.trim($('#ShippingAddress1').val());
    req.ShippingAddress2 = $.trim($('#ShippingAddress2').val());
    req.ShippingZipcode = $.trim($('#ShippingZipcode').val());
    req.ShippingCity = $.trim($('#ShippingCity').val());
    req.ShippingState = $.trim($('#ShippingStateID').val());
    req.PhoneTypeId = $.trim($('#PhoneTypeId').val());
    req.TitleId = $.trim($('#TitleId').val());
    req.AlternativeType = $.trim($('#AlternativeType').val());
    req.ParentId = $('#ParentID').val() == null ? "" : $('#ParentID').val();
    req.EFINStatus = $.trim($('#EFINStatus').val());
    req.AlternativePhoneType = $.trim($('#AlternatePhoneTypeId').val());

    var AdditionalEFIN = $('#AdditionalEFIN').val();
    if (AdditionalEFIN == true || AdditionalEFIN == 'True' || AdditionalEFIN == 'true') {
        req.IsAdditionalEFINSubSite = true;
    } else {
        req.IsAdditionalEFINSubSite = false;
    }

    var EFIN = req.EFIN;
    $('#EFINStatus').removeClass("error_msg");
    $('#EFIN').removeClass("error_msg");
    if ($.trim(req.EFINStatus) == "0" || $.trim(req.EFINStatus) == "") {
        $('#EFINStatus').addClass("error_msg");
        $('#EFINStatus').attr('title', 'Please select EFIN Status');
        cansubmit = false;
    }
    else if ($.trim(req.EFINStatus) == '16' || $.trim(req.EFINStatus) == '19') {

        if (EFIN == "" || EFIN == "0") {
            $('#EFIN').addClass("error_msg");
            $('#EFIN').attr('title', 'Please enter Valid EFIN');
            cansubmit = false;
        }

        //if (EFIN.length < 6) {
        //    $('#EFIN').addClass("error_msg");
        //    $('#EFIN').attr('title', 'Please enter Valid 6 digit EFIN');
        //    cansubmit = false;
        //}
    }
    if (EFIN != "" && EFIN != null) {
        if (EFIN.length < 6 || Number(EFIN) < 1) {
            $('#EFIN').addClass("error_msg");
            $('#EFIN').attr('title', 'Please enter Valid 6 digit EFIN');
            cansubmit = false;
        }
    }

    if ($("#myentityid").val() != $("#Entity_SOME").val()) {

        $('#CompanyName').removeClass("error_msg");
        if (req.CompanyName == "") {
            $('#CompanyName').addClass("error_msg");
            $('#CompanyName').attr('title', 'Please enter Company Name');
            cansubmit = false;
        }
        $('#BusinessOwnerFirstName').removeClass("error_msg");
        if (req.BusinessOwnerFirstName == "") {
            $('#BusinessOwnerFirstName').addClass("error_msg");
            $('#BusinessOwnerFirstName').attr('title', 'Please enter Business Owner First Name');
            cansubmit = false;
        }

        $('#BusinessOwnerLastName').removeClass("error_msg");
        if (req.BusinessOwnerLastName == "") {
            $('#BusinessOwnerLastName').addClass("error_msg");
            $('#BusinessOwnerLastName').attr('title', 'Please enter Business Owner Last Name');
            cansubmit = false;
        }

        $('#OfficePhone').removeClass("error_msg");
        if (req.OfficePhone == "") {
            $('#OfficePhone').addClass("error_msg");
            $('#OfficePhone').attr('title', 'Please enter Office Phone');
            cansubmit = false;
        }

        $('#Primaryemail').removeClass("error_msg");
        if (req.Primaryemail == "") {
            $('#Primaryemail').addClass("error_msg");
            $('#Primaryemail').attr('title', 'Please enter Primary Email');
            cansubmit = false;
        }
        else {
            var emailaddress = $.trim($('#Primaryemail').val());
            var IsEamil = IsEmailValidate(emailaddress);
            if (IsEamil == false) {
                $('#Primaryemail').addClass("error_msg");
                $('#Primaryemail').attr('title', 'Please enter valid Email Address');
                cansubmit = false;
            }
        }


        $('#PhysicalAddress1').removeClass("error_msg");
        if (req.PhysicalAddress1 == "") {
            $('#PhysicalAddress1').addClass("error_msg");
            $('#PhysicalAddress1').attr('title', 'Please enter Physical Address 1');
            cansubmit = false;
        }

        $('#PhysicalZipcode').removeClass("error_msg");
        if (req.PhysicalZipcode == "") {
            $('#PhysicalZipcode').addClass("error_msg");
            $('#PhysicalZipcode').attr('title', 'Please enter Physical Zipcode');
            cansubmit = false;
        }
        else if (req.PhysicalZipcode.length != 5) {
            $('#PhysicalZipcode').addClass("error_msg");
            $('#PhysicalZipcode').attr('title', 'Please enter 5-digit zip code');
            cansubmit = false;
        }

        $('#PhysicalCity').removeClass("error_msg");
        if (req.PhysicalCity == "") {
            $('#PhysicalCity').addClass("error_msg");
            $('#PhysicalCity').attr('title', 'Please enter Physical City');
            cansubmit = false;
        }

        $('#PhysicalStateID').removeClass("error_msg");
        if (req.PhysicalState == "") {
            $('#PhysicalStateID').addClass("error_msg");
            $('#PhysicalStateID').attr('title', 'Please enter Physical State');
            cansubmit = false;
        }


        $('#ShippingAddress1').removeClass("error_msg");
        if (req.ShippingAddress1 == "") {
            $('#ShippingAddress1').addClass("error_msg");
            $('#ShippingAddress1').attr('title', 'Please enter Shipping Address 1');
            cansubmit = false;
        }
        $('#ShippingZipcode').removeClass("error_msg");
        if (req.ShippingZipcode == "") {
            $('#ShippingZipcode').addClass("error_msg");
            $('#ShippingZipcode').attr('title', 'Please enter Shipping Zipcode');
            cansubmit = false;
        }
        else if (req.ShippingZipcode.length != 5) {
            $('#ShippingZipcode').addClass("error_msg");
            $('#ShippingZipcode').attr('title', 'Please enter 5-digit zip code');
            cansubmit = false;
        }

        $('#ShippingCity').removeClass("error_msg");
        if (req.ShippingCity == "") {
            $('#ShippingCity').addClass("error_msg");
            $('#ShippingCity').attr('title', 'Please enter Shipping City');
            cansubmit = false;
        }

        $('#ShippingStateID').removeClass("error_msg");
        if (req.ShippingState == "") {
            $('#ShippingStateID').addClass("error_msg");
            $('#ShippingStateID').attr('title', 'Please enter Shipping State');
            cansubmit = false;
        }

        $('#SupportNotificationemail').removeClass("error_msg");
        $('#SupportNotificationemail').attr('title', '');
        if ($.trim($('#SupportNotificationemail').val()) != "") {
            var emailaddress = $.trim($('#SupportNotificationemail').val());
            var IsEamil = IsEmailValidate(emailaddress);
            if (IsEamil == false) {
                $('#SupportNotificationemail').addClass("error_msg");
                $('#SupportNotificationemail').attr('title', 'Please enter valid Email Address');
                cansubmit = false;
            }
        }

        $('#TitleId').removeClass("error_msg");
        if (req.TitleId == "") {
            $('#TitleId').addClass("error_msg");
            $('#TitleId').attr('title', 'Please select Title');
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

        if ($('#entityid').val() != $('#myentityid').val()) {
            req.Id = $('#Id').val();
        } else {
            if ($('#Id').val() == "") {
                req.Id = '00000000-0000-0000-0000-000000000000';
            }
            else {
                req.Id = $('#Id').val();
            }
        }
        req.SiteMapId = $('#formid').attr('sitemapid');
        req.UserId = $('#UserId').val();
        var Uri = '/api/CustomerInformation/Save';
        ajaxHelper(Uri, 'POST', req, false).done(function (data, status) {
            if (data.StatusCode == -1) {
                error.show();
                error.append('<p> This EFIN is already associated another user </p>');
                return false;
            } else if (data.StatusCode == -2) {
                error.show();
                error.append('<p> Parent information not found </p>');
                return false;
            }
            else if (data.StatusCode == 0) {
                error.show();
                error.append('<p>  Record not saved. </p>');
                return false;
            }
            else if (data.StatusCode == 2) {
                $('#myid').val(data.Id);
                SaveConfigStatusActive('done');
                UpdateOfficeManagement(data.Id);
                success.show();
                success.append('<p>  Record updated successfully.  </p>');
                return false;
            }
            else if (data.StatusCode == 3) {
                $('#myid').val(data.Id);
                SaveConfigStatusActive('done');
                UpdateOfficeManagement(data.Id);
                success.show();
                success.append('<p> Record saved successfully. </p><p> New Crosslink and EMP User has been created. </p>');
                return false;
            }
            else {
                $('#myid').val(data.Id);
                SaveConfigStatusActive('done');
                UpdateOfficeManagement(data.Id);
                success.show();
                success.append('<p> Record saved successfully. </p>');

                if (req.IsAdditionalEFINSubSite == true) {

                    if ($('#myentityid').val() == $('#Entity_SOME').val()) {
                        window.location.href = "/SubSiteOfficeConfiguration/Dashboard?Id=" + data.Id + "&ParentId=" + $('#ParentID').val() + '&entitydisplayid=' + data.DisplayId + '&ptype=subconfig&entityid=' + data.EntityId;
                    } else {
                        window.location.href = "/SubSiteOfficeConfiguration/ActivateMyAccount?Id=" + data.Id + "&ParentId=" + $('#mygrandparentId').val() + '&entitydisplayid=' + data.DisplayId + '&ptype=subconfig&entityid=' + data.EntityId;
                    }
                }
                else {
                    if ($('#myentityid').val() == $('#Entity_SOME').val()) {
                        window.location.href = "/SubSiteOfficeConfiguration/Dashboard?Id=" + data.Id + "&ParentId=" + $('#ParentID').val() + '&entitydisplayid=' + data.DisplayId + '&ptype=subconfig&entityid=' + data.EntityId;
                    } else {
                        window.location.href = "/SubSiteOfficeConfiguration/Dashboard?Id=" + data.Id + "&ParentId=" + $('#ParentID').val() + '&entitydisplayid=' + data.DisplayId + '&ptype=subconfig&entityid=' + data.EntityId;
                    }
                }
            }

        });
    }
}

function fnActiveCuseromerInfo() {
    var error = $('#error');
    error.html('');
    error.hide();
    var success = $('#success');
    success.html('');
    success.hide();
    SaveConfigStatusActive('done');
    var SubParentId = $('#parentid').val();

    if ($('#ActivationParentId').length > 0) {
        SubParentId = $('#ActivationParentId').val();
    }

    var url1 = '/api/CustomerInformation/SubSiteCustomerStatus';
    ajaxHelper(url1 + '?Id=' + $('#myid').val() + '&ParentId=' + SubParentId, 'POST', null, false).done(function (data) {
        $("html, body").animate({ scrollTop: 0 }, "slow");
        if (data > 0) {

            UpdateOfficeManagement($('#myid').val());

            var message = '';
            if (data == 1)
                message = '<p> Activated this information saved Successfully </p>';
            else if (data == 2)
                message = '<p> Activated this information saved Successfully </p><p> New Crosslink and EMP User has been created. </p>';
            else if (data == 3)
                message = '<p> Activated this information saved Successfully</p><p>Crosslink and EMP User has not created. It will be availble in New Customer Signup</p>'
            else if (data == 4) {
                message = '<p> User account activation processed and saved sucessfully.</p><p>Crosslink UserId has been created. But Password has not created. It will be availble in New Customer Signup</p>';
                error.show();
                error.append(message);
            }

            if (data != 4) {
                success.show();
                success.append(message);
            }

            
            //window.location.href = "/CustomerInformation/AllCustomerInfo";
            if (data == 1) {
                if ($('#entityid').val() != $('#myentityid').val()) {
                    window.location.href = "/SubSiteOfficeConfiguration/Dashboard?Id=" + $('#myid').val() + "&ParentId=" + $('#myparentid').val() + "&entitydisplayid=" + $('#myentitydisplayid').val() + '&ptype=subconfig&entityid=' + $('#myentityid').val();
                } else {
                    window.location.href = "/SubSiteOfficeConfiguration/Dashboard";
                }
            }

            return true;
        }
        else {
            error.show();
            error.append('<p>  Record not saved. </p>');

            return false;
        }
    });

    return true;

}

function HoldUnhold() {
    $('#txt_doldDesc').attr('title', '');
    $('#txt_doldDesc').removeClass('error_msg');
    if ($('#a_hold').text() == 'Site is Active') {
        $('#lbl_doldDesc').show();
        $('#txt_doldDesc').show();
        $('#txt_doldDesc').val('');
        $('#p_hold').text('Are you sure you want to place this site on hold?');
        $('#p_holdDesc').text('By placing this site on Hold, you will be restricting this site’s access to transmitting tax returns and receiving customer support.');
    }
    else
    {
        $('#lbl_doldDesc').hide();
        $('#txt_doldDesc').hide();
        $('#p_hold').text('Do you want to remove this site from being on hold?');
        $('#p_holdDesc').text('By re-activating this site, you are granting permission to transmit tax returns to the filing center.');
    }
    $('#popupHold').modal('show');
}

function HoldUnholdPopup() {

    $('#txt_doldDesc').attr('title', '');
    $('#txt_doldDesc').removeClass('error_msg');
    if ($('#a_hold').text() == 'Site is Active' && $('#txt_doldDesc').val() == '') {
        $('#txt_doldDesc').addClass('error_msg');
        $('#txt_doldDesc').attr('title', 'Please enter Description');
        return;
    }

    var Id = '';
    if ($('#entityid').val() != $('#myentityid').val()) {
        Id = $('#myid').val();
    }
    else
        Id = $('#UserId').val();

    var uri = '/api/CustomerInformation/HoldUnHold?CustomerId=' + Id + '&UserId=' + $('#UserId').val() + '&Description=' + $('#txt_doldDesc').val();
    ajaxHelper(uri, 'POST', null, true).done(function (res) {
        if (res) {
            if ($('#a_hold').text() == 'Site is Active') {
                $('#success').html('<p>This site has been placed on hold</p>');
                $('#success').show();
                $('#a_hold').text('Site on Hold');
            }
            else {
                $('#success').html('<p>This site has been removed from Hold.</p>');
                $('#success').show();
                $('#a_hold').text('Site is Active');
            }
            $('#popupHold').modal('hide');
            UpdateOfficeManagement(Id);
        }
    })
}

////till here
//function getCustomerParent_ForAdditonalEFINSubSite(Id) {
//    var url = '/api/CustomerInformation/';
//    if (Id != '' && Id != null && Id != '00000000-0000-0000-0000-000000000000') {
//        ajaxHelper(url + '?id=' + Id, 'GET').done(function (data) {

//            $('#sup1parentid').val(data["ParentId"]);

//        });
//    }
//}
