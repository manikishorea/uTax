function getAllCustomerInformation() {

    var msg = localStorage.getItem("CrossMsg");
    localStorage.removeItem("CrossMsg");

    if (msg != '' && msg != null && msg != undefined) {
        var success = $('#success');
        success.html('');
        success.hide();
        success.show();
        success.append('<p> ' + msg + ' </p>');
    }

    var url = '/api/CustomerInformation/GetNewCustomers';
    $("#table_CustomerInformation > tbody").remove();
    var table = $('#table_CustomerInformation').append('<tbody/>'); //$(document.body)
    var table2 = $('#table_ManualCustomer').append('<tbody/>'); //$(document.body)


    ajaxHelper(url, 'GET').done(function (data) {
        var newcust = data.filter(function (a) { return a.EntityId == 5 || a.EntityId == 9 });
        var mancust = data.filter(function (a) { return a.EntityId != 5 && a.EntityId != 9 });
        if (newcust) {
            $('#loading_main').show();
            $.each(newcust, function (rowIndex, r) {
                var row = $("<tr/>").addClass('odd').attr('role', 'row');
                row.appendTo(table);
                row.append($("<td/>").text(r["EROType"]));
                row.append($("<td/>").text(r["CompanyName"]));
                row.append($("<td/>").text(r["BusinessOwnerFirstName"]));
                row.append($("<td/>").text(r["OfficePhone"]));
                row.append($("<td/>").text(r["Primaryemail"]));
                row.append(ActionList(r["Id"], r["StatusCode"], r["EntityId"]));
            });

            $('#loading_main').hide();
        }

        if (mancust) {
            $('#loading_main').show();
            $.each(mancust, function (rowIndex, r) {
                var row = $("<tr/>").addClass('odd').attr('role', 'row');
                row.appendTo(table2);
                row.append($("<td/>").text(r["EROType"]));
                row.append($("<td/>").text(r["CompanyName"]));
                row.append($("<td/>").text(r["BusinessOwnerFirstName"]));
                row.append($("<td/>").text(r["OfficePhone"]));
                row.append($("<td/>").text(r["Primaryemail"]));
                row.append(ActionList(r["Id"], r["StatusCode"], r["EntityId"]));
            });

            $('#loading_main').hide();
        }
    });
}

function getCustomerInformation(Id) {
    var url = '/api/CustomerInformation/';
    if (Id != '' && Id != null && Id != '00000000-0000-0000-0000-000000000000') {
        ajaxHelper(url + '?id=' + Id, 'GET').done(function (data) {

            var entityid = $('#entityid').val();
            if ($('#entityid').val() != $('#myentityid').val()) {
                entityid = $('#myentityid').val();
            }



            $('#Id').val(Id);
            $('#CompanyName').val(data["CompanyName"]);
            if (data["CompanyName"] && (entityid == $('#Entity_SO').val() || entityid == $('#Entity_MO').val() || entityid == $('#Entity_SVB').val()))
                $('#CompanyName').attr('disabled', 'disabled');
            else
                $('#CompanyName').removeAttr('disabled');
            // $('#AccountStatus').val(data["AccountStatus"]);
            // $('#SalesforceParentID').val(data["SalesforceParentID"]);
            // $('#MasterIdentifier').val(data["MasterIdentifier"]);
            $('#BusinessOwnerFirstName').val(data["BusinessOwnerFirstName"]);
            if (data["BusinessOwnerFirstName"] && (entityid == $('#Entity_SO').val() || entityid == $('#Entity_MO').val() || entityid == $('#Entity_SVB').val()))
                $('#BusinessOwnerFirstName').attr('disabled', 'disabled');
            else
                $('#BusinessOwnerFirstName').removeAttr('disabled');

            $('#BusinessOwnerLastName').val(data["BusinessOwnerLastName"]);
            if (data["BusinessOwnerLastName"] && (entityid == $('#Entity_SO').val() || entityid == $('#Entity_MO').val() || entityid == $('#Entity_SVB').val()))
                $('#BusinessOwnerLastName').attr('disabled', 'disabled');
            else
                $('#BusinessOwnerLastName').removeAttr('disabled');

            $('#OfficePhone').val(data["OfficePhone"]);
            $('#AlternatePhone').val(data["AlternatePhone"]);
            $('#Primaryemail').val(data["Primaryemail"]);
            $('#SupportNotificationemail').val(data["SupportNotificationemail"]);
            $('#EROType').val(data["EROType"]);
            $('#AlternativeContact').val(data["AlternativeContact"]);

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
            $('#AlternatePhoneTypeId').val(data["AlternativePhoneType"]);
            $('#PhoneTypeId').val(data["PhoneTypeId"]);
            //  getTitleForMulti($('#TitleId'), data['TitleId'],$('#AlternativeTypeId'), data['AlternativeType']);
            // getAlternativeTitle($('#AlternativeTypeId'), data['AlternativeType']);
            // getPhoneType($('#PhoneTypeId'), data['PhoneTypeId']);

            //getEFINStatus($('#EFINStatus'), data['EntityId']);
            $('#EFINStatus').val(data['EFINStatus']);

            // removed as per new cr - mani // changed as per "FW: Phase 1 Final Review" mail
            if (data['EFINStatus'] != '16' && data['EFINStatus'] != '19') {
                //$('#EFINStatus').attr('readonly', 'readonly');
                $('#EFIN').attr('readonly', 'readonly');
                //$('#EFINStatus').css('pointer-events', 'none');
                //$('#EFINStatus').prop('disabled', true);
            }

            if (Number(data.EFIN) > 0) {
                var EFIN = PadLeft(data.EFIN, 6);
                $('#EFIN').val(EFIN);
            }
            else {
                $('#EFIN').val('');
            }

            if (data['ParentId'] == "" || data['ParentId'] == null || data['ParentId'] == undefined) {
                // $('#divParentInfo').hide();
                $('#divParentInfo').show();
                fnParentInfor(data['Id']);
            } else {
                $('#divParentInfo').show();
                fnParentInfor(data['ParentId']);
            }

            $('#OldEFIN').val($('#EFIN').val());
        });
    }
}

$('#EFINStatus').change(function (e) {
    if($(this).val()!='16'&& $(this).val()!='19')
        $('#EFIN').attr('readonly', 'readonly');
    else
        $('#EFIN').removeAttr('readonly');
})

function getCustomerLoginInfo(Id) {
    var Login = localstorage.getItem("RegisterCustomer")
    if (Login) {
        var IsExist = Login.filter(function (i) { return i.id == Id })[0];

        if (!IsExist) {
            var url = '/api/CustomerLogin/GetCustomer';
            if (Id != '' && Id != null && Id != '00000000-0000-0000-0000-000000000000') {

                ajaxHelper(url + '?id=' + Id, 'GET').done(function (data) {
                    Login.push({
                        id: id,
                        entitydisplayid: data.EntityDisplayId,
                        IsMSOUser: data.IsMSOUser,
                        IsActivationCompleted: data.IsActivationCompleted,
                        IsEnrollmentSubmit: data.IsEnrollmentSubmit,
                        EntityId: data.EntityID
                    });
                    localstorage.getItem("uTax", Login)
                });
            }
        }
    }
}

function GetCustomerInfomationForMainOfficeinEDIT() {

    if ($('#entityid').val() != $('#myentityid').val()) {
        window.location.href = "/CustomerInformation/Create?Id=" + getUrlVars()["Id"] + '&ParentId=' + getUrlVars()["ParentId"] + '&entitydisplayid=' + getUrlVars()["entitydisplayid"] + '&entityid=' + getUrlVars()["entityid"] + '&ptype=' + getUrlVars()["ptype"];
    }
    else {
        window.location.href = '/CustomerInformation/Create?Id=' + $('#UserId').val();
    }
}

function GetCustomerInfomationForSubSiteOffice() {

    if ($('#entityid').val() != $('#myentityid').val()) {
        var Id = $('#myid').val();
        var parentid = $('#myparentid').val();
        window.location.href = '/SubSiteOfficeConfiguration/Create?Id=' + Id + '&ParentId=' + parentid + '&type=1' + '&entitydisplayid=' + $('#myentitydisplayid').val() + '&ptype=' + getUrlVars()["ptype"] + '&entityid=' + getUrlVars()["entityid"];
    } else {
        var Id = $('#UserId').val();
        window.location.href = '/SubSiteOfficeConfiguration/Create?Id=' + Id + '&type=0';;
    }
}

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
            $('#spCitystatezip').text(data["PhysicalCity"] + ', ' + data["PhysicalState"] + ' ' +  data["PhysicalZipcode"]);
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
            $('#IsVerified').val(data["IsVerified"]);
            localStorage.setItem("IsVerified", data["IsVerified"]);

            $('#spEFIN').html(data["EFINStatusText"]);



            fnVerifiedLinksStatus();
            getIsEnrollmentSubmit();


            if (data.IsVerified == false || data.IsVerified == 'false' || data.IsVerified == 'False') {
                $('#btn_next').remove();
                $('#btn_edit').text('Verify');
            } else {
                $('#btn_next').show();
                $('#btn_edit').text('Edit');
            }

            if (data["EFINStatus"] == '17' || data["EFINStatus"] == '18' || data["EFINStatus"] == '20') {
                $('#btn_next').remove();
            }

        });
    }

    var url1 = '/api/CustomerLoginInformation';
    ajaxHelper(url1 + '?Id=' + Id, 'GET').done(function (data) {
        //$('#spEFIN').text(data["EFIN"]);
        $('#spMasterident').text(data["MasterIdentifier"]); // Master ID
        $('#spMasterUserID').text(data["CrossLinkUserId"]); // User ID
        $('#spTransmissionpwd').text(data["CrossLinkPassword"]); // Transmission Password
        $('#spofficeportalusername').text(data["TaxOfficeUsername"]);
        $('#spoofficeportalpwd').text(data["TaxOfficePassword"]);
        // $('#spemppwd').spemppwd(data["EMPPassword"]);
    });
}

function getBankEnrollmentStatusofCustomer(Id) {
    ajaxHelper('/api/EnrollmentBankSelection/getBankEnrollmentStatusofCustomer?CustomerId=' + Id, 'GET', null, true).done(function (res) {
        if (res) {
            if (res.DisableEfin) {
                $('#EFINStatus, #EFIN').css('pointer-events', 'none');
            }
            else if (res.IsUnlocked) {
                $('#EFINStatus, #EFIN').css('pointer-events', 'all');
            }
        }
    })
}

function fnIsShippingSameAsPhysical() {

    var ShippingAddressSameAsPhysicalAddress = $('#ShippingAddressSameAsPhysicalAddress').is(':checked');
    if (ShippingAddressSameAsPhysicalAddress) {
        $('#ShippingAddress1').val($('#PhysicalAddress1').val());
        $('#ShippingAddress2').val($('#PhysicalAddress2').val());
        $('#ShippingZipcode').val($('#PhysicalZipcode').val());
        $('#ShippingCity').val($('#PhysicalCity').val());

        $('#ShippingStateCode').val($('#PhysicalStateCode').val());
    }
}

function fnIsShippingSameAsPhysical() {

    var ShippingAddressSameAsPhysicalAddress = $('#ShippingAddressSameAsPhysicalAddress').is(':checked');
    if (ShippingAddressSameAsPhysicalAddress) {
        $('#ShippingAddress1').val($('#PhysicalAddress1').val());
        $('#ShippingAddress2').val($('#PhysicalAddress2').val());
        $('#ShippingZipcode').val($('#PhysicalZipcode').val());
        $('#ShippingCity').val($('#PhysicalCity').val());

        $('#ShippingStateID').val($('#PhysicalStateID').val());
    }
}

function fnSaveCustomerInformation() {

    $('select').removeAttr('disabled', 'disabled');
    $('input[type=checkbox]').removeAttr('disabled', 'disabled');
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
    // req.SalesforceParentID = $.trim($('#SalesforceParentID').val());
    // req.MasterIdentifier = $.trim($('#MasterIdentifier').val());
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
    req.AlternativeType = $.trim($('#AlternativeTypeId').val());
    req.ParentId = $('#ParentID').val() == null ? "" : $('#ParentID').val();
    req.EFINStatus = $.trim($('#EFINStatus').val());
    req.AlternativePhoneType = $.trim($('#AlternatePhoneTypeId').val());

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

    var EFIN = $.trim(req.EFIN)
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

    //$('#EFIN').removeClass("error_msg");
    //if (req.EFIN == "") {
    //    $('#EFIN').addClass("error_msg");
    //    $('#EFIN').attr('title', 'Please enter EFIN');
    //    cansubmit = false;
    //}

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
        $('#ShippingAddress1').attr('title', 'Please enter Physical Address 1');
        cansubmit = false;
    }

    $('#ShippingZipcode').removeClass("error_msg");
    if (req.ShippingZipcode == "") {
        $('#ShippingZipcode').addClass("error_msg");
        $('#ShippingZipcode').attr('title', 'Please enter Physical Zipcode');
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
        $('#ShippingCity').attr('title', 'Please enter Physical City');
        cansubmit = false;
    }

    $('#ShippingStateID').removeClass("error_msg");
    if (req.ShippingState == "") {
        $('#ShippingStateID').addClass("error_msg");
        $('#ShippingStateID').attr('title', 'Please enter Physical State');
        cansubmit = false;
    }

    if ($("#myentityid").val() != $("#Entity_SOME").val() && $("#myentityid").val() != $("#Entity_SO").val() && $("#myentityid").val() != $("#Entity_SOME_SS").val()) {

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

        $('#TitleId').removeClass("error_msg");
        if (req.TitleId == "") {
            $('#TitleId').addClass("error_msg");
            $('#TitleId').attr('title', 'Please select Title');
            cansubmit = false;
        }
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


    if (!cansubmit) {
        error.show();
        error.append('<p> Please correct the error(s). </p>');
        return false;
    }
    if (cansubmit) {
        error.hide();
        error.html('');
        if ($('#entityid').val() != $('#myentityid').val()) {
            req.UserId = $('#UserId').val();
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

                SaveConfigStatusActive('done');
                UpdateEFINAfterApproved($('#OldEFIN').val());

                var iscreation = false;
                var iscreated = 0;
                if ($('#myentityid').val() == $('#Entity_SO').val() || $('#myentityid').val() == $('#Entity_SOME').val()) {
                    iscreation = true;
                    checkSOSOMEActivation();
                    iscreated = checkActivationandCreateUser();
                }

                UpdateOfficeManagement(data.Id);
                success.show();
                success.append('<p>  Record updated successfully.  </p>');
                if (iscreation && (iscreated == 1)) {
                    success.append('<p> Crosslink and EMP Portal credentials were created. </p>');
                    // return;
                }
                else if (iscreation && (iscreated == -1)) {
                    success.append('<p> Crosslink and EMP Portal credentials were not created. It is available in New Customer Signup</p>');
                    // return;
                }

                window.location.href = document.referrer;
                // getConfigStatus();
                // return false;
            }
            else {

                var iscreation = false;
                var iscreated = 0;
                SaveConfigStatusActive('done');
                UpdateEFINAfterApproved($('#OldEFIN').val());

                if ($('#myentityid').val() == $('#Entity_SO').val() || $('#myentityid').val() == $('#Entity_SOME').val()) {
                    iscreation = true;
                    checkSOSOMEActivation();
                    iscreated = checkActivationandCreateUser();
                }

                UpdateOfficeManagement(data.Id);
                success.show();
                success.append('<p> Record saved successfully. </p>');
                if (iscreation && (iscreated == 1)) {
                    success.append('<p> Crosslink and EMP Portal credentials were created. </p>');
                }
                else if (iscreation && (iscreated == -1)) {
                    success.append('<p> Crosslink and EMP Portal credentials were not created. It is available in New Customer Signup</p>');
                }

                // return true;
            }
        });
    }
}

function checkActivationandCreateUser() {
    var customerid = '';
    if ($('#entityid').val() != $('#myentityid').val()) {
        customerid = $('#myid').val();
    } else {
        customerid = $('#id').val();
    }

    var _created = 0;

    var uri = '/api/Crosslink/checkActivationandCreateUser?CustomerId=' + customerid+'&UserId='+$('#UserId').val();
    ajaxHelper(uri, 'POST', null, false).done(function (res) {
        if (res) {
            _created = res;
        }
    })

    return _created;
}

function fnSaveSubSiteInformation() {
    var req = {};
    var cansubmit = true;

    var error = $('#error');
    error.html('');
    error.hide();

    var success = $('#success');
    success.html('');
    success.hide();
    req.EFIN = $.trim($('#txtEFIN').val());
    req.CompanyName = $.trim($('#txtCompanyName').val());
    req.BusinessOwnerFirstName = $.trim($('#txtOwnerName').val());
    req.PhysicalAddress1 = $.trim($('#txtPhysicalAdd').val());
    req.PhysicalCity = $.trim($('#txtCity').val());
    req.PhysicalState = $.trim($('#StateCode').val());
    req.PhysicalZipcode = $.trim($('#txtZip').val());
    req.OfficePhone = $.trim($('#txtOfficePhone').val());
    req.AlternatePhone = $.trim($('#txtAlternatePhone').val());
    req.Primaryemail = $.trim($('#txtPrimaryEmail').val());
    req.AlternativeContact = $.trim($('#txtAlternateContact').val());
    req.OfficePortalUrl = 'https://www.mytaxofficeportal.com';
    req.MasterIdentifier = $.trim($('#MasterIdentifier').val());
    req.SalesforceParentID = $.trim($('#SFParentId').val());

    $('*').removeClass("error_msg");
    if (req.EFIN == "") {
        $('#txtEFIN').addClass("error_msg");
        $('#txtEFIN').attr('title', 'Please enter EFIN');
        cansubmit = false;
    }
    if (req.CompanyName == "") {
        $('#txtCompanyName').addClass("error_msg");
        $('#txtCompanyName').attr('title', 'Please enter Company Name');
        cansubmit = false;
    }
    if (req.BusinessOwnerFirstName == "") {
        $('#txtOwnerName').addClass("error_msg");
        $('#txtOwnerName').attr('title', 'Please enter Owner Name');
        cansubmit = false;
    }
    if (req.Primaryemail == "") {
        $('#txtPrimaryEmail').addClass("error_msg");
        $('#txtPrimaryEmail').attr('title', 'Please enter Primary Email');
        cansubmit = false;
    }
    else {
        var emailaddress = $.trim($('#txtPrimaryEmail').val());
        var IsEamil = IsEmailValidate(emailaddress);
        if (IsEamil == false) {
            $('#txtPrimaryEmail').addClass("error_msg");
            $('#txtPrimaryEmail').attr('title', 'Please enter valid Email Address');
            cansubmit = false;
        }
    }
    if (req.OfficePhone == "") {
        $('#txtOfficePhone').addClass("error_msg");
        $('#txtOfficePhone').attr('title', 'Please enter Office Phone');
        cansubmit = false;
    }

    if (req.PhysicalAddress1 == "") {
        $('#txtPhysicalAdd').addClass("error_msg");
        $('#txtPhysicalAdd').attr('title', 'Please enter Address');
        cansubmit = false;
    }
    if (req.PhysicalCity == "") {
        $('#txtCity').addClass("error_msg");
        $('#txtCity').attr('title', 'Please enter City');
        cansubmit = false;
    }
    if (req.PhysicalState == "") {
        $('#txtState').addClass("error_msg");
        $('#txtState').attr('title', 'Please enter State');
        cansubmit = false;
    }
    if (req.PhysicalZipcode == "") {
        $('#txtZip').addClass("error_msg");
        $('#txtZip').attr('title', 'Please enter Zipcode');
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
        req.ParentId = $('#ParentID').val();
        req.UserId = $('#UserId').val();
        var Uri = '/api/CustomerInformation/CustomerSubSiteInfo';
        ajaxHelper(Uri, 'POST', req).done(function (data, status) {
            if (data == -1) {
                error.show();
                error.append('<p> This EFIN is already exist</p>');
                $("html, body").animate({ scrollTop: 0 }, "slow");
            }
            else if (data == 0) {
                error.show();
                error.append('<p>  Record not saved. </p>');
                $("html, body").animate({ scrollTop: 0 }, "slow");
            } else {
                success.show();
                success.append('<p> Sub-Site Information saved Successfully. </p>');
                $("html, body").animate({ scrollTop: 0 }, "slow");
            }
        });
    }
}

function fnSubSiteInformation() {
    var Id = $('#ParentID').val();
    var url1 = '/api/CustomerLoginInformation';
    ajaxHelper(url1 + '?Id=' + Id, 'GET').done(function (data) {
        $('#spEFIN').text(data["EFIN"]);
        $('#lblMasterID').text(data["MasterIdentifier"]); // Master ID
        $('#MasterID').text(data["MasterIdentifier"]);
        $('#lblUserID').text(data["CrossLinkUserId"]); // User ID
        $('#lblTransPwd').text('N/A');
        $('#lblCompanyName').text(data["CompanyName"]);

        $('#MasterIdentifier').val(data["MasterIdentifier"]);
        $('#SFParentId').val(data["SalesforceParentID"]);
        //$('#lblTransPwd').text(data["CrossLinkPassword"]); // Transmission Password

        //$('#spofficeportalusername').text(data["OfficePortalUrl"]);
        //$('#spoofficeportalpwd').text(data["TaxOfficePassword"]);
        // $('#spemppwd').spemppwd(data["EMPPassword"]);
    });
}

function fnParentSubSiteInformation() {
    var Id = $('#parentid').val();
    var url1 = '/api/CustomerLoginInformation';
    ajaxHelper(url1 + '?Id=' + Id, 'GET').done(function (data) {
        $('#p_spEFIN').text(data["EFIN"]);
        $('#p_spCompanyName').text(data["CompanyName"]);
        $('#p_spOwnerName').text(data["BusinessOwnerFirstName"]);
        $('#p_spOwnerLastName').text(data["BusinessOwnerLastName"]);
        $('#p_spPhysicalAddress').text(data["PhysicalAddress1"]);
        $('#p_spCityStateZip').text(data["CityStateZip"]);
    });
}

function goBack() {
    window.history.back();
}

function fnParentInfor(ParentID) {
    //var Id = $('#parentid').val();
    var url1 = '/api/CustomerLoginInformation';
    ajaxHelper(url1 + '?Id=' + ParentID, 'GET').done(function (data) {
        localStorage.setItem("spHead", data["CompanyName"]);
        $('#lblMasterID').text(data["MasterIdentifier"]);
        $('#lblParentAccount').text(data["SalesforceParentID"]);
        $('#lblTransmitType').html(data["TransmitType"]);
        $('#lblMSO').text((data["IsMSOUser"] == "false" || data["IsMSOUser"] == false) ? "No" : "Yes");
        $('#lblBank').html(data["Bank"]);
        $('#lblEFIN').text(data["EFIN"]);
    });
}


//function GetCustomerInfomationForMainOfficeVerification() {
//    var Id = '';
//    if (getUrlVars()["Id"])
//        Id = getUrlVars()["Id"];
//    else
//        Id = $('#UserId').val();

//    var url = '/api/CustomerInformation';
//    if (Id != '' && Id != null && Id != '00000000-0000-0000-0000-000000000000') {
//        ajaxHelper(url + '?id=' + Id, 'GET').done(function (data) {
//            $('#IsVerified').val(data["IsVerified"]);
//            localStorage.setItem("IsVerified", data["IsVerified"]);
//        });
//    }
//}

function fnSaveEnrollCustomerInformation() {

    $('select').removeAttr('disabled', 'disabled');
    $('input[type=checkbox]').removeAttr('disabled', 'disabled');
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
    // req.SalesforceParentID = $.trim($('#SalesforceParentID').val());
    // req.MasterIdentifier = $.trim($('#MasterIdentifier').val());
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
    req.AlternativeType = $.trim($('#AlternativeTypeId').val());
    req.ParentId = $('#ParentID').val() == null ? "" : $('#ParentID').val();
    req.EFINStatus = $.trim($('#EFINStatus').val());
    req.AlternativePhoneType = $.trim($('#AlternatePhoneTypeId').val());

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

    var EFIN = $.trim(req.EFIN)
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

    //$('#EFIN').removeClass("error_msg");
    //if (req.EFIN == "") {
    //    $('#EFIN').addClass("error_msg");
    //    $('#EFIN').attr('title', 'Please enter EFIN');
    //    cansubmit = false;
    //}

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

    $('#TitleId').removeClass("error_msg");
    if (req.TitleId == "") {
        $('#TitleId').addClass("error_msg");
        $('#TitleId').attr('title', 'Please select Title');
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


    if (!cansubmit) {
        error.show();
        error.append('<p> Please correct the error(s). </p>');
        return false;
    }
    if (cansubmit) {
        error.hide();
        error.html('');

        var siteid = '';

        if ($('#entityid').val() != $('#myentityid').val()) {
            req.UserId = $('#UserId').val();
            req.Id = $('#Id').val();

            if (getUrlVars()["ptype"] == 'enrollment')
                siteid = '7c8aa474-2535-4f69-a2ae-c3794887f92d';
            else if (getUrlVars()["ptype"] == 'config')
                siteid = '98a706d7-031f-4c5d-8cc4-d32cc7658b63';
        } else {

            if ($('#Id').val() == "") {
                req.Id = '00000000-0000-0000-0000-000000000000';
            }
            else {
                req.Id = $('#Id').val();
            }
        }

        req.SiteMapId = siteid ? siteid : $('#formid').attr('sitemapid');
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

                SaveConfigStatusActive('done');
                UpdateEFINAfterApproved($('#OldEFIN').val());
                UpdateOfficeManagement(data.Id);
                success.show();
                success.append('<p>  Record updated successfully.  </p>');

                if ($('#myentityid').val() == $('#Entity_SO').val() || $('#myentityid').val() == $('#Entity_SOME').val()) {
                    checkSOSOMEActivation();
                }

                window.location.href = document.referrer;
                // getConfigStatus();
                // return false;
            }
            else {

                SaveConfigStatusActive('done');
                UpdateEFINAfterApproved($('#OldEFIN').val());
                UpdateOfficeManagement(data.Id);
                success.show();
                success.append('<p> Record saved successfully. </p>');

                if ($('#myentityid').val() == $('#Entity_SO').val() || $('#myentityid').val() == $('#Entity_SOME').val()) {
                    checkSOSOMEActivation();
                }

                // return true;
            }


        });
    }
}