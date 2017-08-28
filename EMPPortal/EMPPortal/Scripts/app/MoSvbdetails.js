function GetCustomerInfomationForMainOffice() {

    var Id = '';
    if ($('#entityid').val() != $('#myentityid').val()) {
        Id = $('#myid').val();
    }
    else
        Id = $('#UserId').val();

    var url = '/api/CustomerInformation';
    if (Id != '' && Id != null && Id != '00000000-0000-0000-0000-000000000000') {
        ajaxHelper(url + '?id=' + Id, 'GET').done(function (data) {
            $('#spCompanyName').text(data["CompanyName"]);
            localStorage.setItem("spHead", data["CompanyName"]);
            localStorage.setItem("spEntityID", data["EntityId"]);
            $('#spBusinessOwnerName').text(data["BusinessOwnerFirstName"]);
            $('#spBusinessOwnerLastName').text(data["BusinessOwnerLastName"]);
            $('#spPhysicalAddress').text(data["PhysicalAddress1"]);
            $('#spCitystatezip').text(data["PhysicalCity"] + ' ' + data["PhysicalState"] + ', ' + data["PhysicalZipcode"]);
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
            $('#ismsouser').val(data["IsMSOUser"]);
            $('#IsVerified').val(data["IsVerified"]);
            // localStorage.setItem("IsVerified", data["IsVerified"]);


            var Sitemapid = $('#formid').attr('sitemapid');
            // $('#rbuTaxNotCollectingSVBFeeYes').removeAttr('checked');
            // $('#rbuTaxNotCollectingSVBFeeNo').removeAttr('checked');
            if (Sitemapid == "98a706d7-031f-4c5d-8cc4-d32cc7658b63") {
                if (data.IsNotCollectingFee == 'false' || data.IsNotCollectingFee == false || data.IsNotCollectingFee == 'False') {
                    //  $('#rbuTaxNotCollectingSVBFeeNo').prop('checked', true);
                    $('#uTaxNotCollectingSVBFee').html('No');

                } else {
                    // $('#rbuTaxNotCollectingSVBFeeYes').prop('checked', true);
                    $('#uTaxNotCollectingSVBFee').html('Yes');
                }

                //$('#rbuTaxNotCollectingSVBFeeNo').prop('disabled', true);
                // $('#rbuTaxNotCollectingSVBFeeYes').prop('disabled', true);
            }

            $('#spEFIN').html(data["EFINStatusText"]);
            //alert(data["IsVerified"]);
            //fnVerifiedLinksStatus();

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

            // to show "Hold" button
            if (data["StatusCode"] == 'ACT' && ($('#entityid').val() != $('#myentityid').val() || $('#entityid').val() == '5' || $('#entityid').val() == '9') && !data.IsEnrollSubmitted) {
                $('#a_hold').toggleClass('hide');
                if (data["IsHold"]) {
                    $('#a_hold').text('Site on Hold');
                }
            }
            else
                $('#a_hold').remove();

        });

        //  SaveConfigStatusActive('done');
    }
    //var LoginId = $('#LoginId').val();
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

function GetCustomerInfo_MainOfficeinEDIT() {

    if ($('#entityid').val() != $('#myentityid').val()) {
        var Id = $('#myid').val();
        window.location.href = '/Configuration/Create?Id=' + Id + '&entitydisplayid=' + $('#myentitydisplayid').val() + '&entityid=' + $('#myentityid').val() + '&ParentId=' + $('#myparentid').val() + '&ptype=config';
    } else {
        var Id = $('#UserId').val();
        window.location.href = '/Configuration/Create/' + Id;
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

function fnSaveCustomerInformation_MoSvb() {
    var req = {};
    var cansubmit = true;
    $('#EFINStatus').prop('disabled', false);

    var error = $('#error');
    error.html('');
    error.hide();

    var success = $('#success');
    success.html('');
    success.hide();
    req.CompanyName = $.trim($('#CompanyName').val());
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

    req.EFINStatus = $.trim($('#EFINStatus').val());
    $('#CompanyName').removeClass("error_msg");
    if (req.CompanyName == "") {
        $('#CompanyName').addClass("error_msg");
        $('#CompanyName').attr('title', 'Please enter Company Name');
        cansubmit = false;
    }

    //$('#AccountStatus').removeClass("error_msg");
    //if (req.AccountStatus == "") {
    //    $('#AccountStatus').addClass("error_msg");
    //    $('#AccountStatus').attr('title', 'Please enter AccountStatus Id');
    //    cansubmit = false;
    //}

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

    //$('#AlternatePhone').removeClass("error_msg");
    //if (req.AlternatePhone == "") {
    //    $('#AlternatePhone').addClass("error_msg");
    //    $('#AlternatePhone').attr('title', 'Please enter Alternate Phone');
    //    cansubmit = false;
    //}

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

    //$('#SupportNotificationemail').removeClass("error_msg");
    //if (req.SupportNotificationemail == "") {
    //    $('#SupportNotificationemail').addClass("error_msg");
    //    $('#SupportNotificationemail').attr('title', 'Please enter Support Notification email');
    //    cansubmit = false;
    //}

    //$('#EROType').removeClass("error_msg");
    //if (req.EROType == "") {
    //    $('#EROType').addClass("error_msg");
    //    $('#EROType').attr('title', 'Please enter ERO Type');
    //    cansubmit = false;
    //}

    //$('#AlternativeContact').removeClass("error_msg");
    //if (req.AlternativeContact == "") {
    //    $('#AlternativeContact').addClass("error_msg");
    //    $('#AlternativeContact').attr('title', 'Please enter Alternative Contact');
    //    cansubmit = false;
    //}
    $('#EFINStatus').removeClass("error_msg");
    $('#EFIN').removeClass("error_msg");
    var EFIN = $.trim(req.EFIN);
    if ($.trim(req.EFINStatus) == "0" || $.trim(req.EFINStatus) == "") {
        $('#EFINStatus').addClass("error_msg");
        $('#EFINStatus').attr('title', 'Please select EFIN Status');
        cansubmit = false;
    }
    else if ($.trim(req.EFINStatus) == '16' || $.trim(req.EFINStatus) == '19') {

        if (EFIN == "" || EFIN == null) {
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
    else if (req.PhysicalZipcode.length!=5) {
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
        $('#PhysicalStateID').attr('title', 'Please select Physical State');
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
        $('#ShippingStateID').attr('title', 'Please select Shipping State');
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

    if (!cansubmit) {
        error.show();
        error.append('<p> Please correct the error(s). </p>');
        return false;
    }

    if (cansubmit) {
        error.hide();
        error.html('');
        req.UserId = $('#UserId').val();
        req.Id = $('#Id').val();
        req.SiteMapId = $('#formid').attr('sitemapid');

        var Uri = '/api/CustomerInformation/Save';
        ajaxHelper(Uri, 'POST', req, false).done(function (data, status) {
            $("html, body").animate({ scrollTop: 0 }, "slow");

            if (data.StatusCode == -1) {
                error.show();
                error.append('<p> This EFIN is already associated another user </p>');
                return false;
            }
            else if (data.StatusCode == 0) {
                error.show();
                error.append('<p>  Record not saved. </p>');
                return false;
            }
            else {
                SaveConfigStatusActive('done');
                UpdateEFINAfterApproved($('#OldEFIN').val());
                UpdateOfficeManagement(req.Id);
                success.show();
                success.append('<p> Record saved successfully. </p>');

                if ($('#myentityid').val() == $('#Entity_SO').val() || $('#myentityid').val() == $('#Entity_SOME').val()) {
                    checkSOSOMEActivation();
                }

                window.location.href = document.referrer;
                return true;
            }
            //SaveConfigStatusActive('done');
        });
    }

}

function fnIsShippingSameAsPhysical_MoSvb() {

    var ShippingAddressSameAsPhysicalAddress = $('#ShippingAddressSameAsPhysicalAddress').is(':checked');
    if (ShippingAddressSameAsPhysicalAddress) {
        $('#ShippingAddress1').val($('#PhysicalAddress1').val());
        $('#ShippingAddress2').val($('#PhysicalAddress2').val());
        $('#ShippingZipcode').val($('#PhysicalZipcode').val());
        $('#ShippingCity').val($('#PhysicalCity').val());

        $('#ShippingStateID').val($('#PhysicalStateID').val());
    }
}

function getBankAndQuestions_MOSvb(container, entityid) {
    container.html('');
    var custmorUri = '/api/dropdown/banksubquestion?entityid=' + entityid;
    var $table = $('<table/>').addClass('table table-striped table-hover table-bordered');//; <table class="table table-striped table-hover table-bordered">
    var $tbody = $('<tbody />');
    var IsDataExist = false;
    ajaxHelper(custmorUri, 'GET', null, false).done(function (data) {

        if (data != null && data != '' && data != undefined) {

            $.each(data, function (colIndex, c) {
                var bankid = c["BankId"];
                var bankname = c["BankName"];
                var DocID = c["DocumentPath"];
                var $tr = $('<tr/>');
                var $td1 = $('<td/>');

                if (DocID != '' && DocID != null && DocID != undefined) {
                    var docpath = EMPAdminWebAPI + '/' + DocID;
                    $td1.append('<div class="form-group"><div class="checkbox"><label><input type="checkbox" Id="chk' + bankid + '"  name="chk' + bankid + '" class="chkBank" value="' + bankid + '" onchange="getBankQuestionsShow(this)"  bankname="' + bankname + '" bankid="' + bankid + '"> <span class="checkbox-material"><span class="check"></span></span> ' + bankname + ' <a href="' + docpath + '" target="_blank" title="Download"> <i class="fa fa-arrow-circle-down"></i> </a></label></div></div>'); //<span class="checkbox-material"><span class="check"></span></span>
                } else {
                    $td1.append('<div class="form-group"><div class="checkbox"><label><input type="checkbox" Id="chk' + bankid + '"  name="chk' + bankid + '" class="chkBank" value="' + bankid + '" onchange="getBankQuestionsShow(this)"  bankname="' + bankname + '" bankid="' + bankid + '"> <span class="checkbox-material"><span class="check"></span></span> ' + bankname + ' </label></div></div>'); //<span class="checkbox-material"><span class="check"></span></span>
                }
                var $td2 = $('<td/>')
                var $head = $('<div/>').addClass('form-group bank-details');
                if (c.Questions.length > 0) {
                    $head.attr('id', 'divBankQuestions' + bankid).attr('style', 'display:none');
                    $head.append('<label>How will your Sub-sites print their ' + bankname + ' Checks?</label>');//  container.append('<div/>').addClass('form-group').attr('id', 'divBankQuestions'+bankid);// class="form-group" id="divBankQuestions"><p>How will your Sub-sites print their ' + name + ' Checks?</p>');
                }
                $.each(c.Questions, function (rowIndex, r) {
                    IsDataExist = true;
                    $head.append('<div class="radio"><label><input type="radio" Id="chk' + r["Id"] + '"  name="bank' + bankid + '" class="rbBank' + bankid + '" value="' + r["Id"] + '" checked="" /><span class="circle"></span><span class="check"></span> ' + r["Name"] + '</label></div>');
                });

                if (IsDataExist) {
                    $td2.append($head);
                }
                $tr.append($td1);
                $tr.append($td2);
                $tbody.append($tr);
            });
            $table.append($tbody);
            container.append($table);
        }
    });
}

function getAffiliate_MoSvb(container, entityid) {
    container.html('');
    var custmorUri = '/api/dropdown/affiliate?entityid=' + entityid;
    //var ul = $('<ul/>').addClass('list-unstyled')
    ajaxHelper(custmorUri, 'GET', null, false).done(function (data) {
        $.each(data, function (rowIndex, r) {
            var DocID = r["Description"];
            if (DocID != '' && DocID != null && DocID != undefined) {
                //var booksUri = '/api/Document/Download';
                //ajaxDownloader(booksUri + "?Id=" + DocID, 'GET').done(function (result) {
                //    // $("a#tcpathabcd").attr('href', EMPAdminWebAPI + "/" + data);
                var docpath = EMPAdminWebAPI + '/' + DocID;
                container.append('<label><input type="checkbox" Id="chk' + r["Id"] + '"  name="chk' + r["Id"] + '" class="chkAffiliate" value="' + r["Id"] + '" onclick="chkBelowinfo(this)" chkname="' + r["Name"] + '"> <span class="checkbox-material"><span class="check"></span></span> ' + r["Name"] + ' <a href="' + docpath + '" target="_blank" title="Download"> <i class="fa fa-arrow-circle-down"></i> </a> </label>');//<span class="checkbox-material"><span class="check"></span></span>

            }
            else {
                container.append('<label><input type="checkbox" Id="chk' + r["Id"] + '"  name="chk' + r["Id"] + '" class="chkAffiliate" value="' + r["Id"] + '" onclick="chkBelowinfo(this)"  chkname="' + r["Name"] + '"> <span class="checkbox-material"><span class="check"></span></span> ' + r["Name"] + '</label>'); //<span class="checkbox-material"><span class="check"></span></span>
            }
        });
        //container.append(ul);
    });
}

function getFees_Mosvb(container1, container2, entityid, userid) {

    container1.html('');
    container2.html('');
    var custmorUri = '/api/dropdown/Fees?entityid=' + entityid + '&userid=' + userid;

    container1.append('<thead><tr><th>Bank Product Fees</th> <th>Amount</th></tr></thead><tbody>');
    container2.append('<thead><tr><th>e-File Fees Amount</th> <th>Amount</th></tr></thead><tbody>');

    var TotalBankProductFees = 0;
    var TotaleFileFees = 0;

    ajaxHelper(custmorUri, 'GET', null, false).done(function (data) {
        $.each(data, function (rowIndex, r) {
            if (r["FeeCategoryID"] == '1') {
                if (r["IsEdit"] == true) {
                    container1.append('<tr><td>' + r["Name"] + '</td><td><span>$</span> <span class="hdnBankFee padding-left-3" feefor="' + r["FeeFor"] + '"  id="cst-amount_' + r["Id"] + '" value="' + r["Amount"] + '" >' + r["Amount"] + '</span>  <div class="cst-edit"><div class="col-md-6"><input type="text" id="input_' + r["Id"] + '" value=' + r["Amount"] + ' class="form-control decimal" maxlength="8"></div><span class="cst-option"><a href="#" class="bankfee" onclick="SaveTransmetterFee(this)" id="' + r["Id"] + '" ><i class="fa fa-check" aria-hidden="true"></i></a><i class="fa fa-times cst-close" aria-hidden="true"  onclick="fn_close(this)" id="close_' + r["Id"] + '" closeid="' + r["Id"] + '" ></i></span></div><i class="fa fa-pencil-square-o cst-edit-btn" aria-hidden="true" onclick="editclick(this)" id="edit_' + r["Id"] + '" editid="' + r["Id"] + '"></i></td></tr>');
                    //TotalBankProductFees = TotalBankProductFees + Number(r["Amount"]);
                }
                else {
                    container1.append('<tr><td>' + r["Name"] + '</td><td><span>$</span> <span class="hdnBankFee padding-left-3" feefor="' + r["FeeFor"] + '">' + r["Amount"] + '</span></td></tr>');
                }

            }
            else {
                if (r["IsEdit"] == true) {
                    container2.append('<tr><td>' + r["Name"] + '</td><td>span>$</span> <span class="hdnTranFee" feefor="' + r["FeeFor"] + '" id="cst-amount_' + r["Id"] + '">' + r["Amount"] + '</span>  <div class="cst-edit"><div class="col-md-6"><input type="text" id="input_' + r["Id"] + '" value=' + r["Amount"] + ' class="form-control decimal" maxlength="8"></div><span class="cst-option"><a href="#" class="tranfee" onclick="SaveTransmetterFee(this)" id="' + r["Id"] + '"><i class="fa fa-check" aria-hidden="true"></i></a><i class="fa fa-times cst-close" aria-hidden="true"  onclick="fn_close(this)" id="close_' + r["Id"] + '" closeid="' + r["Id"] + '" ></i></span></div><i class="fa fa-pencil-square-o cst-edit-btn" aria-hidden="true" onclick="editclick(this)" id="edit_' + r["Id"] + '" editid="' + r["Id"] + '"></i></td></tr>');
                    //TotaleFileFees = TotaleFileFees + Number(r["Amount"]);
                }
                else {
                    container2.append('<tr><td>' + r["Name"] + '</td><td><span>$</span> <span class="hdnTranFee" feefor="' + r["FeeFor"] + '">' + r["Amount"] + '</span> </td></tr>');
                    //TotaleFileFees = TotaleFileFees + Number(r["Amount"]);
                }
            }

            if (r["FeeFor"] == '2') {
                TotalBankProductFees = TotalBankProductFees + Number(r["Amount"]);
            }

            if (r["FeeFor"] == '3') {
                TotaleFileFees = TotaleFileFees + Number(r["Amount"]);
            }

        });

        $('#TotalBankProductFees').val(TotalBankProductFees);
        $('#TotaleFileFees').val(TotaleFileFees);

        container1.append('</tbody>');
        container2.append('</tbody>');
    });
}

function getUserBanks_MoSvb(container1, container2, container3, container4) {

    container1.html('');
    container2.html('');
    container3.html('');
    container4.html('');
    var UserId;
    if ($('#entityid').val() != $('#myentityid').val()) {
        UserId = $('#myid').val();
    } else {
        UserId = $('#UserId').val();
    }
    var custmorUri = '/api/dropdown/userbanks?UserId=' + UserId;
    ajaxHelper(custmorUri, 'GET').done(function (data) {

        $.each(data, function (rowIndex, r) {

            var DocID = r["DocumentPath"];
            if (DocID != '' && DocID != null && DocID != undefined) {
                var docpath = EMPAdminWebAPI + '/' + DocID;
                container1.append('<div class="col-md-4"><div class="form-group is-empty"><label>' + r["BankName"] + ' (Desktop : ' + r["FeeDesktop"] + ')  <a href="' + docpath + '" target="_blank" title="Download"> <i class="fa fa-arrow-circle-down"></i> </a></label><input id="input_' + r["BankId"] + '" feetype="desktop"  feedsk="' + r["FeeDesktop"] + '"  class="form-control bankservice decimal svbfee" name="bankservice" type="text" bankid="' + r["BankId"] + '" maxlength="8"></div></div>');
                container2.append('<div class="col-md-4"><div class="form-group is-empty"><label>' + r["BankName"] + ' (MSO : ' + r["FeeMSO"] + ')  <a href="' + docpath + '" target="_blank" title="Download"> <i class="fa fa-arrow-circle-down"></i> </a></label><input id="input_' + r["BankId"] + '_MSO"  feetype="mso" feemso="' + r["FeeMSO"] + '"   class="form-control decimal svbfee" type="text" bankid="' + r["BankId"] + '" maxlength="8"></div></div>');

                container3.append('<div class="col-md-4"><div class="form-group is-empty"><label>' + r["BankName"] + ' (Desktop : ' + r["TranFeeDesktop"] + ')  <a href="' + docpath + '" target="_blank" title="Download"> <i class="fa fa-arrow-circle-down"></i> </a></label><input id="input_trns_' + r["BankId"] + '" feetype="desktop"  tranfeedsk="' + r["TranFeeDesktop"] + '" class="form-control banktrans decimal transfee" name="banktrans" type="text" bankid="' + r["BankId"] + '" maxlength="8"></div></div>');
                container4.append('<div class="col-md-4"><div class="form-group is-empty"><label>' + r["BankName"] + ' (MSO : ' + r["TranFeeMSO"] + ')  <a href="' + docpath + '" target="_blank" title="Download"> <i class="fa fa-arrow-circle-down"></i> </a></label><input id="input_trns_' + r["BankId"] + '_MSO" feetype="mso" tranfeemso="' + r["TranFeeMSO"] + '" class="form-control decimal transfee" type="text" bankid="' + r["BankId"] + '" maxlength="8"></div></div>');

            } else {
                container1.append('<div class="col-md-4"><div class="form-group is-empty"><label>' + r["BankName"] + ' (Desktop : ' + r["FeeDesktop"] + ')</label><input id="input_' + r["BankId"] + '" feetype="desktop"  feedsk="' + r["FeeDesktop"] + '"  class="form-control bankservice decimal svbfee" name="bankservice" type="text" bankid="' + r["BankId"] + '" maxlength="8"></div></div>');
                container2.append('<div class="col-md-4"><div class="form-group is-empty"><label>' + r["BankName"] + ' (MSO : ' + r["FeeMSO"] + ')</label><input id="input_' + r["BankId"] + '_MSO"  feetype="mso" feemso="' + r["FeeMSO"] + '"   class="form-control decimal svbfee" type="text" bankid="' + r["BankId"] + '" maxlength="8"></div></div>');

                container3.append('<div class="col-md-4"><div class="form-group is-empty"><label>' + r["BankName"] + ' (Desktop : ' + r["TranFeeDesktop"] + ')</label><input id="input_trns_' + r["BankId"] + '"  feetype="desktop"  tranfeedsk="' + r["TranFeeDesktop"] + '" class="form-control banktrans decimal transfee" name="banktrans" type="text" bankid="' + r["BankId"] + '" maxlength="8"></div></div>');
                container4.append('<div class="col-md-4"><div class="form-group is-empty"><label>' + r["BankName"] + ' (MSO : ' + r["TranFeeMSO"] + ')</label><input id="input_trns_' + r["BankId"] + '_MSO"  feetype="mso" tranfeemso="' + r["TranFeeMSO"] + '" class="form-control decimal transfee" type="text" bankid="' + r["BankId"] + '" maxlength="8"></div></div>');
            }

        });
    });
}

function getIsSalesYearCheckBankDates() {
    var url = '/api/SubSiteFee/IsSalesYearBankLst';
    var Id;
    if ($('#entityid').val() != $('#myentityid').val()) {
        Id = $('#myid').val();
    }
    else {
        Id = $('#UserId').val();
    }
    var _flag = true;
    var _flag_A = false;
    if (Id != '' && Id != null && Id != '00000000-0000-0000-0000-000000000000') {
        ajaxHelper(url + '?Id=' + Id, 'POST').done(function (data) {
            $.each(data, function (ind, valu) {
                var Bankid = valu["BankId"];
                var Active = valu["Active"];
                if (Active == false) {
                    var stitle = 'The ability to update the Fees information is not available since the Cutoff date for the same is elapsed. Please contact the uTax support team. ';
                    $('#input_' + Bankid).prop('disabled', 'disabled').prop('title', stitle);
                    $('#input_' + Bankid + '_MSO').prop('disabled', 'disabled').prop('title', stitle);
                    $('#input_trns_' + Bankid).prop('disabled', 'disabled').prop('title', stitle);
                    $('#input_trns_' + Bankid + '_MSO').prop('disabled', 'disabled').prop('title', stitle);
                    $('#btnEdit').prop('disabled', 'disabled');

                    $('#input_' + Bankid).val() != "" ? 0 : $('#input_' + Bankid).val(0);
                    $('#input_' + Bankid + '_MSO').val() != "" ? 0 : $('#input_' + Bankid + '_MSO').val(0);
                    $('#input_trns_' + Bankid).val() != "" ? 0 : $('#input_trns_' + Bankid).val(0);
                    $('#input_trns_' + Bankid + '_MSO').val() != "" ? 0 : $('#input_trns_' + Bankid + '_MSO').val(0);
                    _flag = false;
                } else {
                    $('#input_' + Bankid).removeAttr('disabled');
                    $('#input_' + Bankid + '_MSO').removeAttr('disabled');
                    $('#input_trns_' + Bankid).removeAttr('disabled');
                    $('#input_trns_' + Bankid + '_MSO').removeAttr('disabled');
                    $('#btnEdit').removeAttr('disabled');
                    _flag_A = true;
                }
            });
        })
    }
    if (_flag_A)
        return _flag_A;
    else
        return _flag;
}

function getEnrollmentStatus() {
    var Id = '';
    if (getUrlVars()["Id"])
        Id = getUrlVars()["Id"];
    else
        Id = $('#UserId').val();

    var uri = '/api/EnrollmentBankSelection/getEnrollmentStatusInfo?CustomerId=' + Id;//+ '&bankid=' + bankid;
    ajaxHelper(uri, 'GET').done(function (data) {
        if (data) {
            if (data.status) {
                $('#spn_bank').text(data.BankName);
                $('#spn_date').text(data.SubmitedDate);
                $('#spn_bankstatus').text(data.SubmissionStaus);
                $('.IsSubmitted').show();

                if (data.ShowUnlock && $('#entityid').val() == '1' && _unlockEnroll) {
                    $('#dv_unlock').show();
                }
                if ((data.SubmissionStaus == 'Submitted' || data.SubmissionStaus == 'Pending') && $('#entityid').val() == '1') { //data.SubmissionStaus == 'Pending' ||
                    $('#a_enrollInfo').show();
                }
                else
                    $('#a_enrollInfo').hide();
                //if (data.ShowBankselection)
                //    $('#a_bankselection').show();
            }
        }
    })
}

function unlockEnrollment() {
    var success = $('#success');
    success.html('');
    success.hide();
    if ($('#txt_unlockreason').val().trim() == '') {
        $('#txt_unlockreason').addClass("error_msg");
        $('#txt_unlockreason').attr('title', 'Please enter Reason');
        return;
    }
    else {
        $('#txt_unlockreason').removeClass("error_msg");
        $('#txt_unlockreason').attr('title', '');
    }

    var Id = $('#myid').val();
    var request = {};
    request.CustomerId = Id;
    request.UserId = $('#UserId').val();
    request.Reason = $('#txt_unlockreason').val();
    var uri = '/api/EnrollmentBankSelection/unlockEnrollment';
    ajaxHelper(uri, 'POST', request).done(function (data) {
        if (data) {
            UpdateOfficeManagement(request.CustomerId);
            success.html('<p>Unlocked successfully</p>');
            success.show();
            $('.IsSubmitted').hide();
            $('#dv_unlock').hide();
            $('#popupUnlock').modal('hide');
        }
    });
}

function ClearMessage() {
    $('#txt_unlockreason').val('');
    $('#txt_unlockreason').removeClass("error_msg");
    $('#txt_unlockreason').attr('title', '');
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
    else {
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

//function HoldUnhold() {
//    var msg = '';
//    if ($('#a_hold').text() == 'Site is Active')
//        msg = 'Are you sure you want to place this site on hold?\nBy placing this site on Hold, you will be restricting this site’s access to transmitting tax returns and receiving customer support.';
//    else
//        msg = 'Do you want to remove this site from being on hold?\nBy re-activating this site, you are granting permission to transmit tax returns to the filing center.';

//    var conres = confirm(msg)
//    if (conres) {
//        var Id = '';
//        if ($('#entityid').val() != $('#myentityid').val()) {
//            Id = $('#myid').val();
//        }
//        else
//            Id = $('#UserId').val();

//        var uri = '/api/CustomerInformation/HoldUnHold?CustomerId=' + Id + '&UserId=' + $('#UserId').val();
//        ajaxHelper(uri, 'POST', null, true).done(function (res) {
//            if (res) {
//                if ($('#a_hold').text() == 'Site is Active') {
//                    $('#success').html('<p>Site has been holded.</p>');
//                    $('#success').show();
//                    $('#a_hold').text('Site on Hold');
//                }
//                else {
//                    $('#success').html('<p>Site has been unholded.</p>');
//                    $('#success').show();
//                    $('#a_hold').text('Site is Active');
//                }
//                UpdateOfficeManagement(Id);
//            }
//        })
//    }
//}

