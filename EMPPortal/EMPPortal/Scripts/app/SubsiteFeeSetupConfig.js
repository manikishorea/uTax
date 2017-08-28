function getFeeReimbursementInformation() {

    var url = '/api/SubSiteFee/';
    var entityid = $('#entityid').val();

    var Id;
    if ($('#entityid').val() != $('#myentityid').val()) {
        Id = $('#myid').val();
        entityid = $('#myentityid').val();
        // entityid = localStorage.getItem("spEntityID");
        getFees_Mosvb($('#tdFeeFixed'), $('#tdFeeUser'), entityid, Id);
        // getUserBanks_MoSvb($('#divBankAddOn'), $('#divBankAddOn_MSO'), $('#divBankAddOn_trns'), $('#divBankAddOn_trns_MSO'));
        getUserBanks($('#divBankAddOn'), $('#divBankAddOn_MSO'), $('#divBankAddOn_trns'), $('#divBankAddOn_trns_MSO'));
    }
    else {
        Id = $('#UserId').val();
        // entityid = $('#entityid').val();
        getFees($('#tdFeeFixed'), $('#tdFeeUser'));
        getUserBanks($('#divBankAddOn'), $('#divBankAddOn_MSO'), $('#divBankAddOn_trns'), $('#divBankAddOn_trns_MSO'));
    }

    //if (Id != '' && Id != null && Id != '00000000-0000-0000-0000-000000000000') {
    //    ajaxHelper(url + '?Id=' + Id, 'GET').done(function (data) {

    //        if (data == "" || data == null || data == undefined) {
    //            $('#subquestion_Service').hide();
    //            $('#subquestion_Trans').hide();
    //        }

    //        $.each(data, function (rowIndex, r) {
    //            $('#ID').val(r["Id"]);
    //            //if (entityid != $('#Entity_SO').val()) {//entityid != "0676dfd0-da29-41e3-a262-81cb528b796c") {

    //                if (r["ServiceorTransmission"] == 1) {
    //                    var BankTrans = $('input[type=text].bankservice');
    //                    $.each(r.SubSiteBankFees, function (indx, c) {

    //                        $('#input_' + c["BankMaster_ID"]).val(c["BankMaxFees"]);
    //                        $('#input_' + c["BankMaster_ID"] + '_MSO').val(c["BankMaxFees_MSO"]);

    //                        //var maxfee = $('#input_' + c["BankMaster_ID"]).attr('maxfee');
    //                        //var myvalu = c["BankMaxFees"];

    //                        //if (maxfee == '' || maxfee == null) {
    //                        //    maxfee = 0;
    //                        //}

    //                        //if (myvalu == '' || myvalu == null) {
    //                        //    myvalu = 0;
    //                        //}

    //                        //var myMaxfee=0;

    //                        //if (maxfee > myvalu) {
    //                        //    myMaxfee = Number(maxfee) - Number(myvalu)
    //                        //} else {
    //                        //    myMaxfee = Number(myvalu) - Number(maxfee)
    //                        //}
    //                        //$('#input_' + c["BankMaster_ID"]).attr('maxfee', myMaxfee);

    //                        ////

    //                        //var maxfeeMSO = $('#input_' + c["BankMaster_ID"] + '_MSO').attr('maxfee');
    //                        //var myvaluMSO = c["BankMaxFees_MSO"];

    //                        //if (maxfeeMSO == '' || maxfeeMSO == null) {
    //                        //    maxfeeMSO = 0;
    //                        //}

    //                        //if (myvaluMSO == '' || myvaluMSO == null) {
    //                        //    myvaluMSO = 0;
    //                        //}

    //                        //var myMaxfeeMSO = 0;

    //                        //if (maxfeeMSO > myvaluMSO) {
    //                        //    myMaxfeeMSO = Number(maxfeeMSO) - Number(myvaluMSO)
    //                        //}
    //                        //else {
    //                        //    myMaxfeeMSO = Number(myvaluMSO) - Number(maxfeeMSO)
    //                        //}

    //                        //$('#input_' + c["BankMaster_ID"] + '_MSO').attr('maxfee', myMaxfeeMSO);

    //                    });

    //                    if (r["IsAddOnFeeCharge"] == true) {
    //                        $('#rbService_BankProductYes').prop('checked', true);
    //                    }
    //                    else {
    //                        $('#rbService_BankProductNo').prop('checked', true);
    //                        $('#subquestion_Service').hide();
    //                    }
    //                    if (r["IsSameforAll"] == true) {
    //                        $('#rbService_SubSiteYes').prop('checked', true);
    //                    }
    //                    else {
    //                        $('#rbService_SubSiteNo').prop('checked', true);
    //                    }
    //                    if (r["IsSubSiteAddonFee"] == true) {
    //                        $('#rbService_duringEnrollingYes').prop('checked', true);
    //                    }
    //                    else {
    //                        $('#rbService_duringEnrollingNo').prop('checked', true);
    //                    }
    //                }
    //                else {
    //                    var BankTrans = $('input[type=text].banktrans');

    //                    $.each(r.SubSiteBankFees, function (indx, c) {
    //                        $('#input_trns_' + c["BankMaster_ID"]).val(c["BankMaxFees"]);
    //                        $('#input_trns_' + c["BankMaster_ID"] + '_MSO').val(c["BankMaxFees_MSO"]);

    //                        //var maxfee = $('#input_trns_' + c["BankMaster_ID"]).attr('maxfee');
    //                        //var myvalu = c["BankMaxFees"];

    //                        //if (maxfee == '' || maxfee == null) {
    //                        //    maxfee = 0;
    //                        //}

    //                        //if (myvalu == '' || myvalu == null) {
    //                        //    myvalu = 0;
    //                        //}

    //                        //var myMaxfee = 0;

    //                        //if (maxfee > myvalu) {
    //                        //    myMaxfee = Number(maxfee) - Number(myvalu)
    //                        //} else {
    //                        //    myMaxfee = Number(myvalu) - Number(maxfee)
    //                        //}

    //                        //$('#input_trns_' + c["BankMaster_ID"]).attr('maxfee', myMaxfee);

    //                        ////

    //                        //var maxfeeMSO = $('#input_trns_' + c["BankMaster_ID"] + '_MSO').attr('maxfee');
    //                        //var myvaluMSO = c["BankMaxFees_MSO"];

    //                        //if (maxfeeMSO == '' || maxfeeMSO == null) {
    //                        //    maxfeeMSO = 0;
    //                        //}

    //                        //if (myvaluMSO == '' || myvaluMSO == null) {
    //                        //    myvaluMSO = 0;
    //                        //}

    //                        //var myMaxfeeMSO = 0;

    //                        //if (maxfeeMSO > myvaluMSO) {
    //                        //    myMaxfeeMSO = Number(maxfeeMSO) - Number(myvaluMSO)
    //                        //}
    //                        //else {
    //                        //    myMaxfeeMSO = Number(myvaluMSO) - Number(maxfeeMSO)
    //                        //}

    //                        //$('#input_trns_' + c["BankMaster_ID"] + '_MSO').attr('maxfee', myMaxfeeMSO);


    //                    });

    //                    if (r["IsAddOnFeeCharge"] == true) {
    //                        $('#rbTrans_BankProductYes').prop('checked', true);
    //                    }
    //                    else {
    //                        $('#rbTrans_BankProductNo').prop('checked', true);
    //                        $('#subquestion_Trans').hide();
    //                    }
    //                    if (r["IsSameforAll"] == true) {
    //                        $('#rbTrans_SubSiteYes').prop('checked', true);
    //                    }
    //                    else {
    //                        $('#rbTrans_SubSiteNo').prop('checked', true);
    //                    }
    //                    if (r["IsSubSiteAddonFee"] == true) {
    //                        $('#rbTrans_duringEnrollingYes').prop('checked', true);
    //                    }
    //                    else {
    //                        $('#rbTrans_duringEnrollingNo').prop('checked', true);
    //                    }
    //                }

    //        });

    //        getIsEnrollmentSubmit();
    //    });
    //}
}


function fnSaveFeeSetupConfig(SaveType) {

    var entityid = $('#entityid').val();

    if ($('#entityid').val() != $('#myentityid').val()) {
        entityid = localStorage.getItem("spEntityID");
    }

    var entitydisplayid = $('#entitydisplayid').val();
    var req = {};

    var success = $('#success');
    success.html('');
    success.hide();
    var cansubmit = true;
    $('*').removeClass("error_msg");
    $('*').attr('title', '');

    if ($('input[name=sample1]:checked').length == 0) {
        $('#dvFirst').addClass("error_msg");
        $('#dvFirst').attr('title', 'Please Select Yes or No');
        cansubmit = false;
    }

    else if ($('input[name=sample4]:checked').length == 0) {
        $('#dvFifth').addClass("error_msg");
        $('#dvFifth').attr('title', 'Please Select Yes or No');
        cansubmit = false;
    }

    var Entities_Bank = [];
    var Entities_Bank_Trans = [];
    if ($('#rbService_BankProductYes').is(':checked')) {
        //if ($('input[name=sample2]:checked').length == 0) {
        //    $('#dvSecound').addClass("error_msg");
        //    $('#dvSecound').attr('title', 'Please Select Yes or No');
        //    cansubmit = false;
        //}
        //else
        if ($('input[name=sample3]:checked').length == 0) {
            $('#dvThird').addClass("error_msg");
            $('#dvThird').attr('title', 'Please Select Yes or No');
            cansubmit = false;
        }

        var Bankservice = $('input[type=text].bankservice');//input[name=bankservice]'
        $.each(Bankservice, function (indx, valu) {

            //var id = $(valu).attr("id");
            //var feetype = $(valu).attr("feetype");
            //var bankid = $('#' + id).attr("bankid");

            //var bankvalue = 0;// $('#' + id).val();
            //var bankvalue_MSO = 0;

            //// if (feetype == "desktop") {
            //bankvalue = $('#' + id).val();
            //$('#' + id).removeClass("error_msg");
            //if (bankvalue == '' || bankvalue == null) { // || bankvalue == 0 || bankvalue == '0'
            //    $('#' + id).addClass("error_msg");
            //    $('#' + id).attr('title', 'Please enter Bank Amount');
            //    cansubmit = false;
            //}

            ////bankvalue_MSO = 0;

            ////} else {

            //bankvalue_MSO = $('#' + id + '_MSO').val()
            //$('#' + id).removeClass("error_msg");
            //if (bankvalue_MSO == '' || bankvalue_MSO == null) { //|| bankvalue_MSO == 0 || bankvalue_MSO == '0'
            //    $('#' + id + '_MSO').addClass("error_msg");
            //    $('#' + id + '_MSO').attr('title', 'Please enter Bank Amount');
            //    cansubmit = false;
            //}
            //bankvalue = 0;
            //// }


            var id = $(valu).attr("id");
            var feetype = $(valu).attr("feetype");
            var bankvalue = $('#' + id).val();
            var bankid = $('#' + id).attr("bankid");

            $('#' + id).removeClass("error_msg");
            if (bankvalue == '' || bankvalue == null) { //|| bankvalue == 0 || bankvalue == '0'
                $('#' + id).addClass("error_msg");
                $('#' + id).attr('title', 'Please enter Bank Amount');
                cansubmit = false;
            }

            var msoExist = $('#' + id + '_MSO').length;

            var bankvalue_MSO = 0;

            if (msoExist > 0) {
                bankvalue_MSO = $('#' + id + '_MSO').val()
                $('#' + id + '_MSO').removeClass("error_msg");
                if (bankvalue_MSO == '' || bankvalue_MSO == null) { //|| bankvalue_MSO == 0 || bankvalue_MSO == '0'
                    $('#' + id + '_MSO').addClass("error_msg");
                    $('#' + id + '_MSO').attr('title', 'Please enter Bank Amount');
                    cansubmit = false;
                }
            }


            Entities_Bank.push({
                BankMaster_ID: bankid,
                BankMaxFees: bankvalue,
                BankMaxFees_MSO: bankvalue_MSO,
                ServiceorTransmission: 1,
            });

        });

        if (!SVBFeesValid()) {
            cansubmit = false;
        }
    }

    if ($('#rbTrans_BankProductYes').is(':checked')) {
        //if ($('input[name=sample5]:checked').length == 0) {
        //    $('#dvSixth').addClass("error_msg");
        //    $('#dvSixth').attr('title', 'Please Select Yes or No');
        //    cansubmit = false;
        //}
        //else
        if ($('input[name=sample6]:checked').length == 0) {
            $('#dvSeven').addClass("error_msg");
            $('#dvSeven').attr('title', 'Please Select Yes or No');
            cansubmit = false;
        }

        var BankTrans = $('input[type=text].banktrans'); //[name=banktrans]
        $.each(BankTrans, function (indx, valu) {

            var id = $(valu).attr("id");
            var feetype = $(valu).attr("feetype");
            var bankvalue = $('#' + id).val();
            var bankid = $('#' + id).attr("bankid");

            $('#' + id).removeClass("error_msg");
            if (bankvalue == '' || bankvalue == null) { //|| bankvalue == 0 || bankvalue == '0'
                $('#' + id).addClass("error_msg");
                $('#' + id).attr('title', 'Please enter Bank Amount');
                cansubmit = false;
            }

            var msoExist = $('#' + id + '_MSO').length;

            var bankvalue_MSO = 0;

            if (msoExist > 0) {
                bankvalue_MSO = $('#' + id + '_MSO').val()
                $('#' + id + '_MSO').removeClass("error_msg");
                if (bankvalue_MSO == '' || bankvalue_MSO == null) { //|| bankvalue_MSO == 0 || bankvalue_MSO == '0'
                    $('#' + id + '_MSO').addClass("error_msg");
                    $('#' + id + '_MSO').attr('title', 'Please enter Bank Amount');
                    cansubmit = false;
                }
            }

            Entities_Bank_Trans.push({
                BankMaster_ID: bankid,
                BankMaxFees: bankvalue,
                BankMaxFees_MSO: bankvalue_MSO,
                ServiceorTransmission: 2,
            });
        });

        if (!TransFeesValid()) {
            cansubmit = false;
        }
    }

    var ref_id, user_id;

    if ($('#entityid').val() != $('#myentityid').val()) {
        ref_id = $('#myid').val();
        user_id = $('#UserId').val();
    }
    else {
        ref_id = $('#UserId').val();
        user_id = $('#UserId').val();
    }



    // if (entityid != $('#Entity_SO').val()) {
    var Entities = [];
    Entities.push({
        Id: $('#ID').val(),
        refId: ref_id,
        UserId: user_id,
        IsAddOnFeeCharge: $('#rbService_BankProductYes').is(':checked'),
        IsSameforAll: true,//$('#rbService_SubSiteYes').is(':checked'),
        IsSubSiteAddonFee: $('#rbService_duringEnrollingYes').is(':checked'),
        ServiceorTransmission: 1,
        SubSiteBankFees: Entities_Bank
    });

    Entities.push({
        Id: $('#ID').val(),
        refId: ref_id,
        UserId: user_id,
        IsAddOnFeeCharge: $('#rbTrans_BankProductYes').is(':checked'),
        IsSameforAll: true, //$('#rbTrans_SubSiteYes').is(':checked'),
        IsSubSiteAddonFee: $('#rbTrans_duringEnrollingYes').is(':checked'),
        ServiceorTransmission: 2,
        SubSiteBankFees: Entities_Bank_Trans
    });
    req.SubsiteFees = Entities;
    //}

    if (!cansubmit) {
        error.show();
        error.html('');
        error.append('<p> Please correct the error(s). </p>');
        $("html, body").animate({ scrollTop: 0 }, "slow");
        return false;
    }

    if (cansubmit) {
        error.hide();
        error.html('');

        var Uri = '/api/SubSiteFee/Fees';
        req.UserId = user_id;
        req.refId = ref_id;
        ajaxHelper(Uri, 'POST', req, false).done(function (data, status) {

            if (data == '1' || data == 1) {
                success.show();
                success.append('<p> uTax & Transmitter Fees saved Successfully </p>');

                //ResetMainSiteActivation();
                SaveConfigStatusActive('done') //, 'mainsite', 'reset'


                if ($('#entityid').val() != $('#myentityid').val()) {
                    UpdateOfficeManagement($('#myid').val());
                } else {
                    UpdateOfficeManagement($('#UserId').val());
                }

                var ActiveMyAccountStatus = $('#ActiveMyAccountStatus').val();
                var iretval = false;
                var uTaxNotCollectingSBFee = $('#uTaxNotCollectingSBFee').val();

                var IsAddOnFeeCharge = $('#rbService_BankProductYes').is(':checked');
                var IsSubSiteAddonFee = $('#rbService_duringEnrollingYes').is(':checked');

                var IsTranAddOnFeeCharge = $('#rbTrans_BankProductYes').is(':checked');
                var IsTranSubSiteAddonFee = $('#rbTrans_duringEnrollingYes').is(':checked');

                if (uTaxNotCollectingSBFee == false || uTaxNotCollectingSBFee == 'false' || uTaxNotCollectingSBFee == 'False') {

                    if (IsAddOnFeeCharge == true || IsTranAddOnFeeCharge == true) {
                        iretval = true;
                    }

                    if (!iretval) {
                        if (IsSubSiteAddonFee == true || IsTranSubSiteAddonFee == true) {
                            iretval = true;
                        } else {
                            iretval = false;
                        }
                    }

                } else {

                    if (IsTranAddOnFeeCharge == true) {
                        iretval = true;
                    }
                    else {
                        iretval = false;
                    }

                    if (iretval) {
                        if (IsTranSubSiteAddonFee == true) {
                            iretval = true;
                        }
                    }
                }


                if (SaveType == 1) {
                    //if ($('#entityid').val() == $('#myentityid').val()) {
                    //  getSitemap($('#divmenu'));
                    //}

                    // var sitelength = $("#site60025459-7568-4a77-b152-f81904aaaa63").length;

                    //if (sitelength > 0) {
                    debugger;
                    if ($('#site60025459-7568-4a77-b152-f81904aaaa63').length == 0) {
                        if ($('#item6df12cef-8bdb-4d84-9057-095590ea0a79 #site0eda5d25-591c-4e01-a845-fb580572cff5').length >= 1) {
                            window.location.href = $('#item6df12cef-8bdb-4d84-9057-095590ea0a79 #site0eda5d25-591c-4e01-a845-fb580572cff5').attr('href');
                            return;
                        }
                        else if ($('#item6df12cef-8bdb-4d84-9057-095590ea0a79 #site0eda5d25-591c-4e01-a845-fb580572cfe8').length >= 1) {
                            window.location.href = $('#item6df12cef-8bdb-4d84-9057-095590ea0a79 #site0eda5d25-591c-4e01-a845-fb580572cfe8').attr('href');
                            return;
                        }
                    }

                    var cango = false;
                    ajaxHelper('/api/SubSiteFee/getFeeLink?CustomerId=' + ref_id, 'POST', null, false).done(function (res) {
                        debugger;
                        cango = res;
                    })

                    if (cango) {
                        if ($('#entityid').val() != $('#myentityid').val()) {
                            window.location.href = "/Configuration/ServiceBureauTransmission?Id=" + $('#myid').val() + '&entitydisplayid=' + $('#myentitydisplayid').val() + '&ptype=config&entityid=' + $('#myentityid').val();
                        } else {
                            window.location.href = "/Configuration/ServiceBureauTransmission";
                        }
                    }
                    else {

                        if (ActiveMyAccountStatus == '0' || ActiveMyAccountStatus == 0) {
                            if ($('#entityid').val() != $('#myentityid').val()) {
                                window.location.href = "/Configuration/ActivateInformation?Id=" + $('#myid').val() + '&entitydisplayid=' + $('#myentitydisplayid').val() + '&ptype=config&entityid=' + $('#myentityid').val();
                            } else {
                                window.location.href = "/Configuration/ActivateInformation";
                            }
                        } else {

                            if ($('#entityid').val() != $('#myentityid').val()) {
                                window.location.href = "/Configuration/Dashboard?Id=" + $('#myid').val() + '&entitydisplayid=' + $('#myentitydisplayid').val() + '&ptype=config&entityid=' + $('#myentityid').val();
                            } else {
                                window.location.href = "/Configuration/Dashboard";
                            }
                        }
                    }

                } else {
                    // getFeeReimbursementInformation();
                    getConfigStatus();
                }

                $("html, body").animate({ scrollTop: 0 }, "slow");
                return true;
            }
            else if (data == '-2' || data == -2) {
                error.show();
                error.append('<p> Sub-Office already activated you can not saved. </p>');
                return false;
            }
            else {
                error.show();
                error.append('<p>  Record not saved. </p>');
                return false;
            }
        });
    }
}


function SaveCustomerAssociateFee() {
    var req = {};

    req.UserId = $.trim($('#UserId').val());
    req.refId = $.trim($('#UserId').val());

    if ($('#entityid').val() != $('#myentityid').val()) {
        req.refId = $.trim($('#myid').val());
    }

    var Uri = '/api/SubSiteFee/CustomerAssociateFees';
    ajaxHelper(Uri, 'POST', req).done(function (data, status) {
        if (data == 'true' || data == 'True' || data == true) {
            return true;
        }
        else {
            return false;
        }
    });
}

function SaveTransmetterFee(obj) {

    cansubmit = true;
    //var error = $('#error');
    //error.html('');
    //error.hide();
    var success = $('#success');
    success.html('');
    success.hide();

    var req = {};
    var id = $(obj).attr('id');


    req.refId = $.trim($('#UserId').val());
    req.FeeMaster_ID = id;
    req.Amount = $('#input_' + id).val();
    req.UserId = $.trim($('#UserId').val());

    $('#input_' + id).removeClass("error_msg");
    if (req.Amount == '' || req.Amount == null || req.Amount == 0 || req.Amount == '0') {
        $('#input_' + id).addClass("error_msg");
        $('#input_' + id).attr('title', 'Please enter Fee Amount');
        cansubmit = false;
    }

    if (!cansubmit) {
        error.html('');
        error.show();
        error.append('<p> Please correct the error(s). </p>');
        $("html, body").animate({ scrollTop: 0 }, "slow");
        return false;
    }
    if (cansubmit) {
        var Uri = '/api/SubSiteFee/UpdateCustomerAssociateFees';
        ajaxHelper(Uri, 'POST', req).done(function (data, status) {
            if (data == 'true' || data == 'True' || data == true) {
                success.show();
                success.append('<p> Record saved successfully. </p>');
                $('#cst-amount_' + id).text(req.Amount);
                //  $('#cst-amount_' + id).show();

                getConfigStatus();
                return true;
            }
            else {
                error.show();
                error.append('<p>  Record not saved. </p>');
                return false;
            }
        });


        var cls = $(obj).attr('class');

        if (cls == 'bankfee') {
            var hdnBankFee = $('.hdnBankFee');
            // var hdnBankFee = $('input[type=hidden][name=feesetup][feefor=2]');
            var TotalBankFee = 0;
            $.each(hdnBankFee, function (indx, valu) {
                var feefor = $(valu).attr('feefor');
                if (feefor == '2') {
                    var FeeAmount = $(valu).html();

                    if (FeeAmount == '' || FeeAmount == null || FeeAmount == undefined) {
                        FeeAmount = 0;
                    }
                    TotalBankFee = Number(TotalBankFee) + Number(FeeAmount);
                }
            });

            $('#TotalBankProductFees').val(TotalBankFee);
        }

        if (cls == 'tranfee') {
            var hdnTranFee = $('.hdnTranFee');
            //var hdnBankFee = $('input[type=hidden][name=feesetup][feefor=3]');
            var TotalTranFee = 0;
            $.each(hdnTranFee, function (indx, valu) {

                var feefor = $(valu).attr('feefor');
                if (feefor == '3') {
                    var FeeAmount = $(valu).html();

                    if (FeeAmount == '' || FeeAmount == null || FeeAmount == undefined) {
                        FeeAmount = 0;
                    }
                    TotalTranFee = Number(TotalTranFee) + Number(FeeAmount);
                }
            });

            $('#TotaleFileFees').val(TotalTranFee);
        }
    }

}

function editclick(obj) {
    var id = $(obj).attr('id');
    var editid = $(obj).attr('editid');
    var edit = $('#edit_' + id).val();
    $('#cst-amount_' + editid).hide();
}

function fn_close(obj) {
    var closeid = $(obj).attr('closeid');
    $('#cst-amount_' + closeid).show();
}


function SVBFeesValid() {
    var svbfee = $('input[type=text].svbfee:visible');

    var cansubmitvalid = true
    $.each(svbfee, function (indx, valu) {
        var TotalFeeAmount = $('#TotalBankProductFees').val();

        $(valu).removeClass("error_msg");

        var thisval = $(valu).val();

        var regexp = /^\d+(\.\d{1,2})?$/; ///^\d+\.\d{1,2}$/;
        var result = regexp.test(thisval)

        if (result) {
            var mainfee = $(valu).attr('mainfee');
            var subfee = $(valu).attr('subfee');
            var maxfee = $(valu).attr('maxfee');

            if (thisval != '' && thisval != null && thisval != undefined) {

                if (maxfee == '' || maxfee == null || maxfee == undefined) {
                    maxfee = 0;
                }

                if (subfee == '' || subfee == null || subfee == undefined) {
                    subfee = 0;
                }

                if (TotalFeeAmount == '' || TotalFeeAmount == null || TotalFeeAmount == undefined) {
                    TotalFeeAmount = 0;
                }

                TotalFeeAmount = Number(TotalFeeAmount) + Number(subfee);

                var AddOnFees = 0;
                if (Number(TotalFeeAmount) < Number(maxfee)) {
                    AddOnFees = Number(maxfee) - Number(TotalFeeAmount);
                }
                else {
                    AddOnFees = Number(TotalFeeAmount) - Number(maxfee);
                }

                $(valu).removeClass("error_msg");
                if (Number(thisval) > Number(AddOnFees)) {
                    $(valu).addClass("error_msg");
                    $(valu).attr('title', 'Maximum possible for this bank is : $' + AddOnFees);
                    cansubmitvalid = false;
                }
            } else {
                $(valu).addClass("error_msg");
                $(valu).attr('title', 'Please provide the bank fee');
            }
        }
        else {
            $(valu).addClass("error_msg");
            $(valu).attr('title', 'Please provide the correct value');
            cansubmitvalid = false;
        }

    });

    return cansubmitvalid;
}

function TransFeesValid() {
    var transfee = $('input[type=text].transfee:visible');
    var cansubmitvalid = true

    $.each(transfee, function (indx, valu) {
        $(valu).removeClass("error_msg");


        var TotalFeeAmount = 0;// $('#TotaleFileFees').val();
        var thisval = $(valu).val();
        var regexp = /^\d+(\.\d{1,2})?$/; ///^\d+\.\d{1,2}$/;
        var result = regexp.test(thisval)

        if (result) {
            var mainfee = $(valu).attr('mainfee');
            var subfee = $(valu).attr('subfee');
            var maxfee = $(valu).attr('maxfee');

            if (thisval != '' && thisval != null && thisval != undefined) {


                if (subfee == '' || subfee == null || subfee == undefined) {
                    subfee = 0;
                }

                if (maxfee == '' || maxfee == null || maxfee == undefined) {
                    maxfee = 0;
                }

                if (TotalFeeAmount == '' || TotalFeeAmount == null || TotalFeeAmount == undefined) {
                    TotalFeeAmount = 0;
                }

                TotalFeeAmount = Number(TotalFeeAmount) + Number(subfee);

                var AddOnFees = 0;

                if (Number(TotalFeeAmount) < Number(maxfee)) {
                    AddOnFees = Number(maxfee) - Number(TotalFeeAmount);
                }
                else {
                    AddOnFees = Number(TotalFeeAmount) - Number(maxfee);
                }


                if (Number(thisval) > Number(AddOnFees)) {
                    $(valu).addClass("error_msg");
                    $(valu).attr('title', 'Maximum possible for this bank is : $' + AddOnFees);
                    cansubmitvalid = false;
                }
            } else {
                $(valu).addClass("error_msg");
                $(valu).attr('title', 'Please provide the bank fee');
            }
        } else {
            $(valu).addClass("error_msg");
            $(valu).attr('title', 'Please provide the correct value');
            cansubmitvalid = false;
        }
    });

    return cansubmitvalid;
}

//function getBankEnrollmentStatus() {
//    var url = '/api/EnrollmentBankSelection/IsBankEnrollmentSubmitted?CustomerId=' + CustomerId;
//    ajaxHelper(url, 'GET').done(function (data) {
//        if (data) {
//            DeactivateSelection();
//        }
//    })
//}
