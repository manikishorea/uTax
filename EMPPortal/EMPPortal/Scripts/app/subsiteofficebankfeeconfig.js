var cansubmitValid = true

function getTotalFeeSVBAndTran(UserId) {
    var url = '/api/SubSiteOffice/BankFeesConfig';
    if (UserId != '' && UserId != null && UserId != '00000000-0000-0000-0000-000000000000') {
        ajaxHelper(url + '?UserId=' + UserId, 'GET', null, false).done(function (data) {

            if (data != null) {
                $.each(data, function (indx, valu) {
                    if (valu["ServiceorTransmission"] == "1") {

                    }

                    if (valu["ServiceorTransmitter"] == "2") {
                        $('#hdnTotalSVBFees').val(valu["AmountDSK"]);
                    }
                    if (valu["ServiceorTransmitter"] == "3") {
                        $('#hdnTotalTransFees').val(valu["AmountDSK"]);
                    }
                });
            }

        });
    }
}

function fnSaveServiceBureau() {
    // var req = {};

    var UserId = $('#UserId').val();

    var RefId;
    if ($('#entityid').val() != $('#myentityid').val()) {
        RefId = $('#myid').val();
    } else {
        RefId = UserId;
    }


    var cansubmit = true;
    var error = $('#error');
    error.html('');
    error.hide();

    var success = $('#success');
    success.html('');
    success.hide();
    var Entities_Bank = [];
    var Bankservice = $('input[type=text].ServiceBank');

    $.each(Bankservice, function (indx, valu) {

        var id = $(valu).attr("id");
        var bankid = $(valu).attr("bankid");

        var amount = 0;// $('#' + id).val();
        var amount_MSO = 0;// $('#' + id + '_MSO').val();

        var AmountType = $(valu).attr("AmountType");
        $('#' + id).removeClass("error_msg");

        if (AmountType == '0') {

            amount = $('#' + id).val();
            amount_MSO = 0;

            if (amount == "") {
                $('#' + id).addClass("error_msg");
                $('#' + id).attr('title', 'Please enter Amount');
                cansubmit = false;
            }
        } else {
            amount_MSO = $('#' + id).val();
            amount = 0;
            if (amount_MSO == "") {
                $('#' + id).addClass("error_msg");
                $('#' + id).attr('title', 'Please enter Amount');
                cansubmit = false;
            }
        }

        var bQu = $('input[name=bank' + bankid + ']:checked').attr('value');

        Entities_Bank.push({
            UserId: UserId,
            RefId: RefId,
            BankID: bankid,
            ServiceorTransmitter: 1,
            AmountDSK: amount,
            AmountMSO: amount_MSO,
            QuestionID: bQu
        });
    });

    if (!SVBFeesValid()) {
        cansubmit = false;
    }

    if (!cansubmit || !cansubmitValid) {
        error.show();
        error.append('<p> Please correct the error(s). </p>');
        return false;
    }

    if (cansubmit && cansubmitValid) {
        error.hide();
        error.html('');
        var Uri = '/api/SubSiteOffice/SaveOfficeBankFeeConfig';
        ajaxHelper(Uri, 'POST', Entities_Bank).done(function (data, status) {
            if (data != '' && data != null && data != '00000000-0000-0000-0000-000000000000') {

                success.show();
                success.append('<p> Fee Configuration saved successfully </p>');

                //ResetSubSiteActivation();
                SaveConfigStatusActive('done'); //, 'subsite', 'reset'

                // getSubSiteConfigStatus();


                $('#liSubOfficeSVBFee').removeClass('active');
                $('#SubOfficeSVBFee').removeClass('active');

                $('#liSubOfficeTranFee').addClass('active');
                $('#SubOfficeTranFee').addClass('active').addClass('in');

                if (($('#hdnTranNoBank').val() == '1' || $('#hdnTranNoBank').val() == 1) && ($('#hdnSVBNoBank').val() == '0' || $('#hdnSVBNoBank').val() == 0)) {
                    $('#divTansmNextSave').show();
                }



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

function NextLink() {
    var status = SaveConfigStatusActive('done');

    if (status == true || status == 'true') {
        if ($('#entityid').val() != $('#myentityid').val()) {
            window.location.href = "/SubSiteOfficeConfiguration/ActivateMyAccount?Id=" + $('#myid').val() + "&ParentId=" + $('#myparentid').val() + '&entitydisplayid=' + $('#myentitydisplayid').val() + '&ptype=subconfig&entityid=' + $('#myentityid').val();
        } else {
            window.location.href = "/SubSiteOfficeConfiguration/ActivateMyAccount";
        }
    }
    else {
        return false;
    }
}

function fnSaveTransmitter(SaveType) {

    var UserId = $('#UserId').val();

    //if ($('#entitydisplayid').val() != $('#myentitydisplayid').val()) {
    //    parentid = $('#SubSite_ParentId').val();
    //    UserId = $('#h_subsiteId').val();
    //}

    var RefId;
    if ($('#entityid').val() != $('#myentityid').val()) {
        RefId = $('#myid').val();
    } else {
        RefId = UserId;
    }



    var cansubmit = true;
    var error = $('#error');
    error.html('');
    error.hide();

    var success = $('#success');
    success.html('');
    success.hide();
    var Entities_Bank = [];
    var Bankservice = $('input[type=text].TransmitterBank');
    $.each(Bankservice, function (indx, valu) {

        var id = $(valu).attr("id");
        var bankid = $(valu).attr("bankid");

        var amount = 0;// $('#' + id).val();
        var amount_MSO = 0;// $('#' + id + '_MSO').val();

        var AmountType = $(valu).attr("AmountType");
        $('#' + id).removeClass("error_msg");
        if (AmountType == '0') {
            amount = $('#' + id).val();
            amount_MSO = 0;
            if (amount == "") {
                $('#' + id).addClass("error_msg");
                $('#' + id).attr('title', 'Please enter Amount');
                cansubmit = false;
            }
        } else {
            amount_MSO = $('#' + id).val();
            amount = 0;
            if (amount_MSO == "") {
                $('#' + id).addClass("error_msg");
                $('#' + id).attr('title', 'Please enter Amount');
                cansubmit = false;
            }
        }

        var bQu = $('input[name=bank' + bankid + ']:checked').attr('value');


        Entities_Bank.push({
            UserId: UserId,
            RefId: RefId,
            BankID: bankid,
            ServiceorTransmitter: 2,
            AmountDSK: amount,
            AmountMSO: amount_MSO,
            QuestionID: bQu
        });

    });

    if (!TransFeesValid()) {
        cansubmit = false;
    }

    if (!cansubmit) {
        error.show();
        error.append('<p> Please correct the error(s). </p>');
        return false;
    }
    if (cansubmit && cansubmitValid) {
        error.hide();
        error.html('');
        var Uri = '/api/SubSiteOffice/SaveOfficeBankFeeConfig';
        ajaxHelper(Uri, 'POST', Entities_Bank).done(function (data, status) {
            if (data != '' && data != null && data != '00000000-0000-0000-0000-000000000000') {

                success.show();
                success.append('<p> Fee Configuration saved successfully </p>');
                var ActiveMyAccountStatus = $('#ActiveMyAccountStatus').val();

                SaveConfigStatusActive('done'); //, 'subsite', 'reset'
                //ResetSubSiteActivation();

                if (SaveType == 1) {

                    //  window.location.href = "/SubSiteOfficeConfiguration/ActivateMyAccount";
                    if (ActiveMyAccountStatus == '0' || ActiveMyAccountStatus == 0) {
                        if ($('#entityid').val() != $('#myentityid').val()) {
                            window.location.href = "/SubSiteOfficeConfiguration/ActivateMyAccount?Id=" + $('#myid').val() + "&ParentId=" + $('#myparentid').val() + '&entitydisplayid=' + $('#myentitydisplayid').val() + '&ptype=subconfig&entityid=' + $('#myentityid').val();
                        } else {
                            window.location.href = "/SubSiteOfficeConfiguration/ActivateMyAccount";
                        }
                    } else {
                        if ($('#entityid').val() != $('#myentityid').val()) {
                            window.location.href = "/SubSiteOfficeConfiguration/Dashboard?Id=" + $('#myid').val() + "&ParentId=" + $('#myparentid').val() + '&entitydisplayid=' + $('#myentitydisplayid').val() + '&ptype=subconfig&entityid=' + $('#myentityid').val();
                        } else {
                            window.location.href = "/Enrollment/OfficeInformation";
                        }
                    }
                }
                    //else if (SaveType == 2) {
                    //    window.location.href = "/SubSiteOfficeConfiguration/ActivateAccount";
                    //}
                else {
                    getConfigStatus();
                    GetCustomerNotesInfo($('#dvcustomernotes'));
                }

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

var xBankFees = [];
function GetSubSiteBankFee(MyUserId) {

    var url = '/api/SubSiteOffice/OfficeBankFeeConfig';
    if (MyUserId != '' && MyUserId != null && MyUserId != '00000000-0000-0000-0000-000000000000') {
        ajaxHelper(url + '?id=' + MyUserId, 'GET', null, false).done(function (data) {
            $.each(data, function (coIndex, c) {

                var bankid = c["BankID"];
                if (c["ServiceorTransmitter"] == 1) {
                    $('#hdnSVBNoBank').val(0);
                    $('#rb_' + c["QuestionID"]).attr('checked', 'checked');

                    $('#txt_' + bankid).val(c["AmountDSK"]);
                    $('#txt_' + bankid + '_MSO').val(c["AmountMSO"]);

                    xBankFees.push({
                        BankID: c["BankID"],
                        AmountDSK: c["AmountDSK"],
                        AmountMSO: c["AmountMSO"],
                        ServiceorTransmitter: 1
                    });
                }
                else {
                    $('#hdnTranNoBank').val(0);
                    $('#rbtrn_' + c["QuestionID"]).attr('checked', 'checked');
                    $('#txttrns_' + bankid).val(c["AmountDSK"]);
                    $('#txttrns_' + bankid + '_MSO').val(c["AmountMSO"]);

                    xBankFees.push({
                        BankID: c["BankID"],
                        AmountDSK: c["AmountDSK"],
                        AmountMSO: c["AmountMSO"],
                        ServiceorTransmitter: 2
                    });
                }
            });

            if (($('#hdnTranNoBank').val() == '1' || $('#hdnTranNoBank').val() == 1) && ($('#hdnSVBNoBank').val() == '1' || $('#hdnSVBNoBank').val() == 1)) {
                $('#divTansmNextSave').show();
            }


        });
    }

    //if(!IsMyRecordSaved)
    //{
    //    getMainSiteFeeInformation(parentid);
    //}
}

function EditServiceBureau() {
    $('#dvServiceBureau input[type=text]').removeAttr("readonly");
    $('#dvServiceBureau input[type=radio]').removeAttr("disabled");
    $('#btnServiceBureau').removeAttr('disabled');
}

function EditTransmitter() {
    $('#dvTransmitter input[type=text]').removeAttr("readonly");
    $('#dvTransmitter input[type=radio]').removeAttr("disabled");
    $('#btnTransmitter').removeAttr('disabled');
}

function getMainSiteFeeInformation(Id) {

    $('#divSVBSave').show();
    $('#divTansmSave').show();
    $('#spanSvbTansSave').show();
    $('#spanNextLink').hide();
    var IsNextVisibleOne = false;
    var IsNextVisibleTwo = false;
    $('#hdnTranNoBank').val(0);

    $('#spanSvbTansSave').show();
    $('#hdnSVBVisible').val(0);
    $('#hdnTranVisible').val(0);
    //$('#spanNextLink').show();
    //$('#divTansmSave').hide();

    var url = '/api/SubSiteFee/';
    if (Id != '' && Id != null && Id != '00000000-0000-0000-0000-000000000000') {
        ajaxHelper(url + '?Id=' + Id, 'GET').done(function (data) {

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


        });

        if (IsNextVisibleOne && IsNextVisibleTwo) {
            $('#spanNextLink').show();
            $('#spanSvbTansSave').hide();
        }
    }
}

function SVBFeesValid() {
    // var hdnFeeAmount = $('.hdnFeeAmount');
    var cansubmitFeeValid = true
    // error.html('');
    // error.hide();

    var ServiceBank = $('input[type=text].ServiceBank');

    $.each(ServiceBank, function (indx, valu) {

        $(valu).removeClass("error_msg");
        var thisval = $(valu).val();

        var bankfee = $(valu).attr('bankfee');

        var maxfee = $(valu).attr('maxfee');

        var TotalFeeAmount = maxfee;

        if (thisval != '' && thisval != null && thisval != undefined) {

            if (bankfee == '' || bankfee == null || bankfee == undefined) {
                bankfee = 0;
            }

            if (maxfee == '' || maxfee == null || maxfee == undefined) {
                maxfee = 0;
            }

            var AddOnFees = 0;
            if (Number(bankfee) < Number(maxfee)) {
                AddOnFees = Number(maxfee) - Number(bankfee);
            }
            else {
                AddOnFees = Number(bankfee) - Number(maxfee);
            }

            $(valu).removeClass("error_msg");
            if (Number(thisval) > Number(AddOnFees)) {
                $(valu).addClass("error_msg");
                $(valu).attr('title', 'Add-On Fees is more than bank fees');
                cansubmitFeeValid = false;
            }

            //if (!cansubmitFeeValid) {
            //    error.show();
            //    error.append('<p> Please correct the error(s). </p>');
            //    cansubmitFeeValid = false;
            //}
        } else {
            $(valu).addClass("error_msg");
            $(valu).attr('title', 'Please enter bank fees');

        }
    });

    return cansubmitFeeValid;
}

function TransFeesValid() {
    // var hdnFeeAmount = $('.hdnFeeAmount');
    var cansubmitFeeValid = true
    // error.html('');
    // error.hide();

    var TransmitterBank = $('input[type=text].TransmitterBank');
    $.each(TransmitterBank, function (indx, valu) {
        $(valu).removeClass("error_msg");

        var thisval = $(valu).val();

        var bankfee = 0;// $(valu).attr('bankfee');

        var maxfee = $(valu).attr('maxfee');

        var TotalFeeAmount = maxfee;

        if (thisval != '' && thisval != null && thisval != undefined) {

            if (bankfee == '' || bankfee == null || bankfee == undefined) {
                bankfee = 0;
            }

            if (maxfee == '' || maxfee == null || maxfee == undefined) {
                maxfee = 0;
            }

            var AddOnFees = 0;
            if (Number(bankfee) < Number(maxfee)) {
                AddOnFees = Number(maxfee) - Number(bankfee);
            }
            else {
                AddOnFees = Number(bankfee) - Number(maxfee);
            }

            $(valu).removeClass("error_msg");
            if (Number(thisval) > Number(AddOnFees)) {
                $(valu).addClass("error_msg");
                $(valu).attr('title', 'Add-On Fees is more than bank fees');
                cansubmitFeeValid = false;
            }

            //if (!cansubmitFeeValid) {
            //    error.show();
            //    error.append('<p> Please correct the error(s). </p>');
            //    cansubmitFeeValid = false;
            //}
        } else {
            $(valu).addClass("error_msg");
            $(valu).attr('title', 'Please enter bank fees');

        }
    });

    return cansubmitFeeValid;
}

function getSubsiteOfficeInfoForFees(Id, parentId) {
    //var Id = $('#UserId').val();
    $('#issubsitemsouser').val('');
    $('#formid').val('active');
    var url = '/api/SubSiteOffice/SubSiteOfficeConfig';
    if (Id != '' && Id != null && Id != '00000000-0000-0000-0000-000000000000' && parentId != '' && parentId != null && parentId != '00000000-0000-0000-0000-000000000000') {
        ajaxHelper(url + '?Id=' + Id + '&parentId=' + parentId, 'GET', null, false).done(function (data) {
            
            if (data != null && data != '' && data != undefined) {
                //if (data["iIsSubSiteSendTaxReturn"] == 1) {

                //}
                //else if (data["iIsSubSiteSendTaxReturn"] == 2) {

                //}
                //else if (data["iIsSubSiteSendTaxReturn"] == 3) {

                //}

                ////$('#ID').val(data["Id"]);

                //if (data["EFINListedOtherOffice"] == true || data["EFINListedOtherOffice"] == 'true') {

                //}
                //else if (data["EFINListedOtherOffice"] == false || data["EFINListedOtherOffice"] == 'false') {

                //}


                //if (data["SiteOwnthisEFIN"] == true || data["SiteOwnthisEFIN"] == 'true') {
                //}
                //else if (data["SiteOwnthisEFIN"] == false || data["SiteOwnthisEFIN"] == 'false') {
                //}

                //if (data["SiteOwnthisEFIN"] != null && data["SiteOwnthisEFIN"] != '') {
                //}

                //var SOorSSorEFIN = data["SOorSSorEFIN"];
                //if (SOorSSorEFIN != null && SOorSSorEFIN != '') {
                //    if (SOorSSorEFIN == 1) {
                //    }
                //    else if (SOorSSorEFIN == 2) {
                //    }
                //    else {
                //    }
                //}

                //if (data["SubSiteSendTaxReturn"] == true || data["SubSiteSendTaxReturn"] == 'true') {

                //}
                //else if (data["SubSiteSendTaxReturn"] == false || data["SubSiteSendTaxReturn"] == 'false') {

                //}

                if (data["SiteanMSOLocation"] == true || data["SiteanMSOLocation"] == 'true') {
                    $('#issubsitemsouser').val(1);
                }
                else if (data["SiteanMSOLocation"] == false || data["SiteanMSOLocation"] == 'false') {
                    $('#issubsitemsouser').val(0);
                }
            }
        });
    } else {
        $('#issubsitemsouser').val(0);
    }
}

function fnSaveServiceAndTranFeeForSubSite(SaveType, poptype) {
    // var req = {};
    var hdnSVBVisible = $('#hdnSVBVisible').val();
    var hdnTranVisible = $('#hdnTranVisible').val();
    $('#savetype').val(SaveType);

    if ($('#myentityid').val() == $('#Entity_SO').val() || $('#myentityid').val() == $('#Entity_SOME').val()) {
        hdnSVBVisible = 1;
        hdnTranVisible = 1;
    }

    var UserId = $('#UserId').val();

    var RefId;
    if ($('#entityid').val() != $('#myentityid').val()) {
        RefId = $('#myid').val();
    } else {
        RefId = UserId;
    }

    var cansubmit = true;
    var error = $('#error');
    error.html('');
    error.hide();

    var success = $('#success');
    success.html('');
    success.hide();



    var Entities_Bank = [];
    var Bankservice = $('input[type=text].ServiceBank');
    //if (hdnSVBVisible == '1' || hdnSVBVisible == 1) {
        $.each(Bankservice, function (indx, valu) {
            var id = $(valu).attr("id");
            var bankid = $(valu).attr("bankid");

            var amount = 0;// $('#' + id).val();
            var amount_MSO = 0;// $('#' + id + '_MSO').val();

            var bankfee = $(valu).attr("bankfee");
            var maxfee = $(valu).attr("maxfee");
            var TotalAmount = Number(maxfee) - Number(bankfee);

            var AmountType = $(valu).attr("AmountType");
            $('#' + id).removeClass("error_msg");

            if (AmountType == '0') {

                amount = $('#' + id).val();
                amount_MSO = 0;

                if (amount == "") {
                    $('#' + id).addClass("error_msg");
                    $('#' + id).attr('title', 'Please enter Amount');
                    cansubmit = false;
                }
            } else {
                amount_MSO = $('#' + id).val();
                amount = 0;
                if (amount_MSO == "") {
                    $('#' + id).addClass("error_msg");
                    $('#' + id).attr('title', 'Please enter Amount');
                    cansubmit = false;
                }
            }

            var bQu = $('input[name=bank' + bankid + ']:checked').attr('value');
            Entities_Bank.push({
                UserId: UserId,
                RefId: RefId,
                BankID: bankid,
                ServiceorTransmitter: 1,
                AmountDSK: amount,
                AmountMSO: amount_MSO,
                QuestionID: bQu,
                TotalAmount: TotalAmount
            });
        });

        if (!SVBFeesValid()) {
            cansubmit = false;
        }
    //}

    var BankserviceTran = $('input[type=text].TransmitterBank');

    //if (hdnTranVisible == '1' || hdnTranVisible == 1) {
        $.each(BankserviceTran, function (indx, valu) {

            var id = $(valu).attr("id");
            var bankid = $(valu).attr("bankid");

            var bankfee = $(valu).attr("bankfee");
            var maxfee = $(valu).attr("maxfee");
            var TotalAmount = Number(maxfee) - Number(bankfee);

            var amount = 0;// $('#' + id).val();
            var amount_MSO = 0;// $('#' + id + '_MSO').val();

            var AmountType = $(valu).attr("AmountType");
            $('#' + id).removeClass("error_msg");
            if (AmountType == '0') {
                amount = $('#' + id).val();
                amount_MSO = 0;
                if (amount == "") {
                    $('#' + id).addClass("error_msg");
                    $('#' + id).attr('title', 'Please enter Amount');
                    cansubmit = false;
                }
            } else {
                amount_MSO = $('#' + id).val();
                amount = 0;
                if (amount_MSO == "") {
                    $('#' + id).addClass("error_msg");
                    $('#' + id).attr('title', 'Please enter Amount');
                    cansubmit = false;
                }
            }

            var bQu = $('input[name=bank' + bankid + ']:checked').attr('value');
            Entities_Bank.push({
                UserId: UserId,
                RefId: RefId,
                BankID: bankid,
                ServiceorTransmitter: 2,
                AmountDSK: amount,
                AmountMSO: amount_MSO,
                QuestionID: bQu,
                TotalAmount: TotalAmount
            });

        });

        if (!TransFeesValid()) {
            cansubmit = false;
        }
   // }

    if (!cansubmit) {
        error.show();
        error.append('<p> Please correct the error(s). </p>');
        return false;
    }

    //if ((hdnSVBVisible == '0' || hdnSVBVisible == 0) && (hdnTranVisible == 0 || hdnTranVisible == '0')) {
    //    var ActiveMyAccountStatus = $('#ActiveMyAccountStatus').val();
    //    if (SaveType == 1) {
    //        if (ActiveMyAccountStatus == '0' || ActiveMyAccountStatus == 0) {
    //            if ($('#entityid').val() != $('#myentityid').val()) {
    //                window.location.href = "/SubSiteOfficeConfiguration/ActivateMyAccount?Id=" + $('#myid').val() + "&ParentId=" + $('#myparentid').val() + '&entitydisplayid=' + $('#myentitydisplayid').val() + '&ptype=subconfig&entityid=' + $('#myentityid').val();
    //            } else {
    //                window.location.href = "/SubSiteOfficeConfiguration/ActivateMyAccount";
    //            }
    //        } else {
    //            if ($('#entityid').val() != $('#myentityid').val()) {
    //                window.location.href = "/SubSiteOfficeConfiguration/Dashboard?Id=" + $('#myid').val() + "&ParentId=" + $('#myparentid').val() + '&entitydisplayid=' + $('#myentitydisplayid').val() + '&ptype=subconfig&entityid=' + $('#myentityid').val();
    //            } else {
    //                window.location.href = "/Enrollment/OfficeInformation";
    //            }
    //        }
    //    }
    //    else {
    //        getConfigStatus();
    //        GetCustomerNotesInfo($('#dvcustomernotes'));
    //    }

    //    return true;
    //}





    if (cansubmit) {
        $('#isenrollapr').val(0);

        if ($('#myentityid').val() != $('#Entity_SO').val() && $('#myentityid').val() != $('#Entity_SOME').val()) {
            if (poptype == 0) {
                var resultBankEnroll = fnGetEnrollmentStatusAndAddOnFee(Entities_Bank,xBankFees, 0);
                
                if (resultBankEnroll == 1) {
                    $('#div_subsiteenrollbankstatus').modal('show');
                }
            }
        }

        if ($('#isenrollapr').val() == 0) {
            error.hide();
            error.html('');
            var Uri = '/api/SubSiteOffice/SaveOfficeBankFeeConfig';
            ajaxHelper(Uri, 'POST', Entities_Bank, false).done(function (data, status) {

                if (data != '' && data != null && data != '00000000-0000-0000-0000-000000000000') {

                    success.show();
                    success.append('<p> Fee Configuration saved successfully </p>');
                    var ActiveMyAccountStatus = $('#ActiveMyAccountStatus').val();

                    SaveConfigStatusActive('done');

                    if ($('#myentityid').val() == $('#Entity_SO').val() || $('#myentityid').val() == $('#Entity_SOME').val()) {
                        checkSOSOMEActivation();

                        if ($('#entityid').val() != $('#myentityid').val()) {
                            window.location.href = "/SubSiteOfficeConfiguration/Dashboard?Id=" + $('#myid').val() + "&ParentId=" + $('#myparentid').val() + '&entitydisplayid=' + $('#myentitydisplayid').val() + '&ptype=subconfig&entityid=' + $('#myentityid').val();
                        } else {
                            window.location.href = "/Enrollment/OfficeInformation";
                        }

                    } else {

                        if (poptype == 1) {
                            UpdateFeeAfterApproved();
                        }
                    }



                    if ($('#entityid').val() != $('#myentityid').val()) {
                        UpdateOfficeManagement($('#myid').val());
                    } else {
                        UpdateOfficeManagement($('#UserId').val());
                    }

                    //ResetSubSiteActivation();

                    if ($('#myentityid').val() != $('#Entity_SO').val() && $('#myentityid').val() != $('#Entity_SOME').val()) {
                        if (SaveType == 1) {
                            if (ActiveMyAccountStatus == '0' || ActiveMyAccountStatus == 0) {
                                if ($('#entityid').val() != $('#myentityid').val()) {
                                    window.location.href = "/SubSiteOfficeConfiguration/ActivateMyAccount?Id=" + $('#myid').val() + "&ParentId=" + $('#myparentid').val() + '&entitydisplayid=' + $('#myentitydisplayid').val() + '&ptype=subconfig&entityid=' + $('#myentityid').val();
                                } else {
                                    window.location.href = "/SubSiteOfficeConfiguration/ActivateMyAccount";
                                }
                            } else {
                                if ($('#entityid').val() != $('#myentityid').val()) {
                                    window.location.href = "/SubSiteOfficeConfiguration/Dashboard?Id=" + $('#myid').val() + "&ParentId=" + $('#myparentid').val() + '&entitydisplayid=' + $('#myentitydisplayid').val() + '&ptype=subconfig&entityid=' + $('#myentityid').val();
                                } else {
                                    window.location.href = "/Enrollment/OfficeInformation";
                                }
                            }
                        }
                        else {
                            getConfigStatus();
                            GetCustomerNotesInfo($('#dvcustomernotes'));
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
        }
    }
}

function fnGetEnrollmentStatusAndAddOnFee(Entities_Bank2, xBankFees, type) {

    $('#isenrollapr').val(0);
    var isenrollapr = 0;

    if (type != 0) {
        fnSaveServiceAndTranFeeForSubSite($('#savetype').val(), 1);
        return true;
    }

    var IsChanged = false;
    $.each(Entities_Bank2, function (indx, valu) {
        var ChangedBank = xBankFees.filter(function (i) { return i.BankID == valu.BankID && i.AmountDSK == valu.AmountDSK && i.AmountMSO == valu.AmountMSO && i.ServiceorTransmitter == valu.ServiceorTransmitter })[0];
        if (!ChangedBank) {
            IsChanged = true;
        }
    });

    if (!IsChanged) {
        return 0;
    }


    var UserId = $('UserId').val();
    if ($('#entityid').val() != $('#myentityid').val()) {
        UserId = $('#myid').val();
    }

    var url = '/api/SubSiteOffice/SubSiteFeeConfigAfterApproved'
    var $table = $('#tbl_innerbankstatus');
    $("#tbl_innerbankstatus > tbody").remove();
    var $tbody = $('<tbody/>');
    var $tr = $('<tr/>');
    var result = ajaxHelper(url + '?UserId=' + UserId, 'GET', null, false).done(function (data, status) {
        if (data != undefined && data != null) {

            $.each(data, function (indx, valu) {

                var SVBFee = Entities_Bank2.filter(function (i) {
                    return i.BankID == valu.BankId && i.ServiceorTransmitter == 1
                })[0];

                var TransFee = Entities_Bank2.filter(function (i) {
                    return i.BankID == valu.BankId && i.ServiceorTransmitter == 2
                })[0];

                if (valu.StatusCode == 'APR' && ((valu.ServiceBureauBankAmount < (SVBFee != undefined ? (SVBFee.TotalAmount - SVBFee.AmountDSK) : 0) || valu.ServiceBureauBankAmount < SVBFee != undefined ? (SVBFee.TotalAmount - SVBFee.AmountMSO) : 0)
                    || (valu.TransmissionBankAmount == (TransFee != undefined ? (TransFee.TotalAmount - TransFee.AmountDSK) : 0) || valu.TransmissionBankAmount == (TransFee != undefined ? (TransFee.TotalAmount - TransFee.AmountMSO) : 0)))) {

                    var $tr = $('<tr/>');
                    var $td1 = $('<td/>');
                    var $td2 = $('<td/>');
                    var $td3 = $('<td/>');

                    $td1.append(valu.BankName);
                    if (valu.IsTransmissionFee) {
                        $td2.append(valu.ServiceBureauBankAmount);
                    } else {
                        $td2.append('No Add-on');
                    }
                    if (valu.IsServiceBureauFee) {
                        $td3.append(valu.TransmissionBankAmount);
                    } else {
                        $td3.append('No Add-on');
                    }

                    $tr.append($td1);
                    $tr.append($td2);
                    $tr.append($td3);
                    isenrollapr = 1;
                    $('#isenrollapr').val(1);
                    // $('#div_innerbankstatus').html("Bank Name : " + data.BankName + "| Service Bureau Add-On Fee : " + data.ServiceBureauBankAmount + "|  Transmission Add-On Fee : " + data.TransmissionBankAmount);

                    $tbody.append($tr);

                    //return false;
                }
            });

            $table.append($tbody);
            $('#div_innerbankstatus').append($table);
            // $('#div_subsiteenrollbankstatus').modal('show');
        }
        return true;
    });

    return isenrollapr;
    // return true;
}

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