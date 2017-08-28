var subsitebankfees = [];
var mainfees = [];
var selectedBank = '00000000-0000-0000-0000-000000000000';
var selectedSvbAmt = 0, selectedTranAmt = 0;
var selectedsvbOpt = 0, selectedTranOpt = 0;
var originalbank = '00000000-0000-0000-0000-000000000000';
var _svbaddongee = 0, _transaddonfee = 0;
var IsLocked = false;

function getEnrollmentFees(container1, container2) {

    container1.html('');
    container2.html('');
    var entityid = $('#entityid').val();
    var userid = $('#UserId').val();
    var parentid = "";// $('#parentid').val();

    if ($('#entityid').val() != $('#myentityid').val()) {
        userid = $('#myid').val();
        parentid = $('#myparentid').val();
        entityid = $('#myentityid').val();
    }

    //var bankid = getUrlVars()["bankid"];

    //var Cid = getUrlVars()["Id"];

    //if ($('#entityid').val() != $('#myentityid').val()) {
    //    userid = Cid;
    //    entityid = $('#myentityid').val() //localStorage.getItem("EnrollEntityId");
    //    parentid = $('#myparentid').val();//localStorage.getItem("EnrollparentId");
    //}

    //if ($('#entitydisplayid').val() == $('#Entity_SOMESubSite').val()) {
    //    parentid = $('#supparentid').val();
    //}



    var bankfeeuri = '/api/EnrollmentBankSelection/getBankFee?CustomerId=' + userid;// + '&bankid=' + bankid;
    ajaxHelper(bankfeeuri, 'GET', null, false).done(function (res) {
        if (res) {
            subsitebankfees = res;
            subsitebankfees.push({ BankName: '', BankId: '00000000-0000-0000-0000-000000000000', BankCode: '', SvbAmount: 0, TransAmount: 0 });
        }
    })

    var custmorUri = '/api/EnrollmentBankSelection/GetBankandFeesInfo?entityid=' + entityid + '&userid=' + userid + '&parentid=' + parentid;// + '&bankid=' + bankid;

    container1.append('<thead><tr><th>Bank Product Fees</th> <th>Amount</th></tr></thead><tbody>');
    container2.append('<thead><tr><th>Bank Amount</th> <th>Amount</th></tr></thead><tbody>');

    var TotalBankProductFees = 0;
    var TotaleFileFees = 0;

    ajaxHelper(custmorUri, 'GET', null, false).done(function (data) {
        $.each(data, function (rowIndex, r) {
            if (r["FeeCategoryID"] == '1') {
                var feeid = '';
                if (r["Name"] == 'Service Bureau Fees')
                    feeid = 'spn_svbamt';
                else if (r["Name"] == 'Transmission Fees') {
                    feeid = 'spn_trsnamt';
                    r["Name"] = 'Transmitter Fees';
                }
                container1.append('<tr><td>' + r["Name"] + '</td><td><span>$</span> <span class="hdnBankFee padding-left-3" id=' + feeid + '>' + r["Amount"] + '</span></td></tr>');
                TotalBankProductFees = TotalBankProductFees + Number(r["Amount"]);
            }
            else {
                container2.append('<tr id="bankSelection_' + r["Id"] + '"><td>' + r["Name"] + '</td><td><span>$</span> <span class="hdnTranFee">' + r["Amount"] + '</span> </td></tr>');
                TotaleFileFees = TotaleFileFees + Number(r["Amount"]);
            }
        });

        $('#TotalBankProductFees').val(TotalBankProductFees);
        $('#TotaleFileFees').val(TotaleFileFees);

        container1.append('</tbody>');
        container2.append('</tbody>');
    });

}

function getBankQuestions(container) {

    container.html('');
    var entityid = $('#entityid').val();
    var CustomerId = $('#UserId').val();
    var ParentId = $('#parentid').val();
    var Cid = getUrlVars()["Id"];
    //var BankId = getUrlVars()["bankid"];


    if ($('#entityid').val() != $('#myentityid').val()) {
        entityid = $('#myentityid').val();
        CustomerId = $('#myid').val();
    }

    var custmorUri = '/api/dropdown/banksubquestionForSelection?entityid=' + entityid + '&CustomerId=' + CustomerId;//BankId=' + BankId + '&
    var IsDataExist = false;
    var $divBank = $("<div/>");
    var $divBankQues = $("<div/>");
    ajaxHelper(custmorUri, 'GET', null, false).done(function (data) {

        if (data != null && data != '' && data != undefined) {

            $divBank.append('&nbsp;<label><input type="radio" Id="chkBE00000000-0000-0000-0000-000000000000" name="chkBankEnroll" value="00000000-0000-0000-0000-000000000000" onchange="getBankQuestionsShow_Enroll(this)" bankname="none" bankid="00000000-0000-0000-0000-000000000000"><span class="circle"></span><span class="check"></span> None </label>');

            $.each(data, function (colIndex, c) {

                var bankid = c["BankId"];
                var bankname = c["BankName"];
                var DocID = c["DocumentPath"];

                if (DocID != '' && DocID != null && DocID != undefined) {
                    var docpath = EMPAdminWebAPI + '/' + DocID;
                    $divBank.append('&nbsp;<label Id="spn_' + bankid + '"><input type="radio" Id="chkBE' + bankid + '" name="chkBankEnroll"  value="' + bankid + '" onchange="GetEnrollmentBankSelection(\'' + bankid + '\')"  bankname="' + bankname + '" bankid="' + bankid + '"><span class="circle"></span><span class="check"></span>' + bankname + '<a href="' + docpath + '" target="_blank" title="Download"> <i class="fa fa-arrow-circle-down"></i> </a></label>');
                } else {
                    $divBank.append('&nbsp;<label Id="spn_' + bankid + '"><input type="radio" Id="chkBE' + bankid + '" name="chkBankEnroll" value="' + bankid + '" onchange="GetEnrollmentBankSelection(\'' + bankid + '\')"  bankname="' + bankname + '" bankid="' + bankid + '"><span class="circle"></span><span class="check"></span> ' + bankname + '</label>');
                }
                var $head = $('<div/>').addClass('form-group bank-details').css('margin', '0px;').css('padding', '0px;');
                var IsSubQues = false;
                if (c.Questions.length > 0) {
                    $head.attr('id', 'divBankQuestions' + bankid).attr('style', 'display:none');;
                    $head.append('<label>How will you print your ' + bankname + ' Checks?</label>');
                    IsSubQues = true;
                }
                if (bankname.toLowerCase() == 'tpg') {
                    $head.append('<div class="radio"><label><input type="radio" Id="Qchka29b3547-8954-4036-9bd3-312f1d6a3faa" option="1"  name="bankTPG" class="rbBank' + bankid + '" value="a29b3547-8954-4036-9bd3-312f1d6a3faa"  /><span class="circle"></span><span class="check"></span> From TPG\'s Website </label></div>');
                    $head.append('<div class="radio"><label><input type="radio" Id="Qchka29b3547-8954-4036-9bd3-312f1d6a3fba" option="2"  name="bankTPG" class="rbBank' + bankid + '" value="a29b3547-8954-4036-9bd3-312f1d6a3fba"  /><span class="circle"></span><span class="check"></span> From within the tax software </label></div>');
                    IsSubQues = true;
                }
                else {
                    $.each(c.Questions, function (rowIndex, r) {
                        var Options = r["Description"];
                        $head.append('<div class="radio"><label><input type="radio" Id="Qchk' + r["Id"] + '" option="' + Options + '"  name="bank' + bankid + '" class="rbBank' + bankid + '" value="' + r["Id"] + '"  /><span class="circle"></span><span class="check"></span> ' + r["Name"] + '</label></div>');
                        IsSubQues = true;
                    });

                    // $('#dv_bankSelection').hide();
                }

                if (IsSubQues) {
                    $divBankQues.append($head)
                }
                //if (c.Questions.length > 0) {

                //}
            });

            container.append($divBank);
            container.append($divBankQues);

            //getIsSalesYearCheckBankDates_Enrollment();
        }
        else {
            $('tr[id^="bankSelection_"]').hide();
            $('#dv_bankSelection').hide();
            setTimeout(function () {
                $('#site0feeb0fe-d0e7-4370-8733-dd5f7d2041fc').hide();
            });
            $('#dvNotDataPageSVB').show();
            $('#dv_save').hide();
        }


    });
}

var svblimit = 0;
var translimit = 0;
var _showtrans = true, _showsvb = true;
var _isSubmitted = false;

function getBankQuestionsShow_Enroll(obj) { //chkBE2fb91ef5-efc4-4baf-abd7-ea05a8c100cc
    $('#spn_svblimit').text(' ($ 0)');
    $('#spn_translimit').text(' ($ 0)');
    var Isreadonly = $(obj).attr('isreadonly');

    if (Isreadonly == 1) {
        var stitle = 'The ability to update the Fees information is not available since the Cutoff date for the same is elapsed. Please contact the uTax support team. ';

        //$('#rbTransmissionYes').attr('disabled', 'disabled').attr('title', stitle);
        //$('#rbServiceBureauYes').attr('disabled', 'disabled').attr('title', stitle);
        //$('#lblYesSubmis').attr('title', stitle);
        //$('#lblYesTrans').attr('title', stitle);
        //$('#rbServiceBureauNo').prop('checked', true);
        //$('#rbTransmissionNo').prop('checked', true);

        // commented by mani to resolve cutoff issue

        $('#divsvbfees').hide();
        $('#divtransfees').hide();
    } else {
        //$('#rbTransmissionYes').prop('checked', true).removeAttr('disabled').attr('title', '');
        //$('#rbServiceBureauYes').prop('checked', true).removeAttr('disabled').attr('title', '');
        // commented by mani to resolve cutoff issue

        //$('#rbServiceBureauNo').prop('checked', false);
        //$('#rbTransmissionNo').prop('checked', false);
        $('#divsvbfees').show();
        $('#divtransfees').show();
    }

    $('#dv_bankfees').hide();
    $('div[id^="divBankQuestions"]').hide();
    var bankid = $(obj).attr('value');
    var name = $(obj).attr('bankname');
    var erotypeid = $('#entitydisplayid').val();
    var erotype = getEroType(erotypeid);
    var ParentId = $('#parentid').val();
    var CustomerId = $('#UserId').val();
    $('#divsvbfees').hide();
    $('#divtransfees').hide();
    var svbtransamt = subsitebankfees.filter(function (i) { return i.BankId == bankid })[0];
    if (svbtransamt) {
        var svbamt = $('#spn_svbamt').text();
        var tramt = $('#spn_trsnamt').text();
        $('#spn_svbamt').text(getRoundoff(parseFloat(svbamt) + svbtransamt.SvbAmount));
        $('#spn_trsnamt').text(getRoundoff(parseFloat(tramt) + svbtransamt.TransAmount));
    }
    var oldbankamt = subsitebankfees.filter(function (i) { return i.BankId == selectedBank })[0];
    if (oldbankamt) {
        var svbamt = $('#spn_svbamt').text();
        var tramt = $('#spn_trsnamt').text();
        var svbaddon = $('#txtService').val();
        var trasaddon = $('#txtTransmission').val();
        if (!$('#site067c03a3-34f1-4143-beae-35327a8fca44').hasClass('done')) {
            svbaddon = 0;
            trasaddon = 0;
        }
        if (svbaddon == '')
            svbaddon = 0;
        if (trasaddon == '')
            trasaddon = 0;

        $('#spn_svbamt').text(getRoundoff(parseFloat(svbamt) - oldbankamt.SvbAmount - parseFloat(svbaddon)));
        $('#spn_trsnamt').text(getRoundoff(parseFloat(tramt) - oldbankamt.TransAmount - parseFloat(trasaddon)));
        if ($('#site067c03a3-34f1-4143-beae-35327a8fca44').hasClass('done') && originalbank == bankid) {
            svbamt = $('#spn_svbamt').text();
            tramt = $('#spn_trsnamt').text();
            $('#spn_svbamt').text(getRoundoff(parseFloat(svbamt) + selectedSvbAmt));
            $('#spn_trsnamt').text(getRoundoff(parseFloat(tramt) + selectedTranAmt));
        }
    }
    selectedBank = bankid;


    if ($('#entityid').val() != $('#myentityid').val()) {
        CustomerId = $('#myid').val();
        ParentId = $('#myparentid').val();
    }


    var _showfee = false;

    $('input[name="rbServiceBureau"]').prop('checked', false);
    $('input[name="rbTransmission"]').prop('checked', false);

    //if (erotype == 'Multi-Office' || erotype == 'Service Bureau - SVB') {
    //    $('#dv_bankfees').hide();
    //}
    //else {
    if (bankid != 'none' && bankid != '00000000-0000-0000-0000-000000000000') {
        _showfee = true;
        var uri = '/api/EnrollmentBankSelection/getFeeLimit?CustomerId=' + CustomerId + '&ParentId=' + ParentId + '&BankId=' + bankid;
        ajaxHelper(uri, 'GET', null, false).done(function (data) {
            if (data) {
                if (data.Status) {
                    svblimit = data.SvbFeeLimit;
                    translimit = data.TransFeeLimit;
                    _showsvb = data.ShowSvb;
                    _showtrans = data.ShoeTransmission;

                    //if (Isreadonly != 1 && svblimit > 0)
                    //    $('#divsvbfees').show();
                    //else
                    //    $('#divsvbfees').hide();

                    //if (Isreadonly != 1 && svblimit > 0)
                    //    $('#divtransfees').show();
                    //else
                    //    $('#divtransfees').hide();

                    $('#spn_svblimit').text(' ($ ' + data.SvbFeeLimit + ')');
                    $('#spn_translimit').text(' ($ ' + data.TransFeeLimit + ')');
                }
            }
        })
        $('#dv_bankfees').show();
    }
    else {
        $('#chkBE00000000-0000-0000-0000-000000000000').prop('checked', true);
        $('tr[id^="bankSelection_"]').hide();
        $('#dv_bankfees').hide();
    }
    //}
    if ($('#chkBE' + bankid).is(':checked')) {
        $('#h_BankID').val(bankid);
        $('#divBankQuestions' + bankid).show();
        $('#dvBankServiceInfo').html('');
        $('#dvBankTransmissionInfo').html('');

        $('#dvBankServiceInfo').append('<div class="col-md-12"><div> <input type="text" id="txtService" name="txtService" onkeypress="Onlydecimal(event)" class="form-control"></div></div>');
        $('#dvBankTransmissionInfo').append('<div class="col-md-12"><div> <input type="text" id="txtTransmission" name="txtTransmission" onkeypress="Onlydecimal(event)" class="form-control"></div></div>');
    }

    $('tr[id^="bankSelection_"]').hide();
    $('#bankSelection_' + bankid).show();

    if (_showsvb)
        $('#dv_ServiceBureu').show();
    else
        $('#dv_ServiceBureu').hide();

    if (_showtrans)
        $('#dv_Transmission').show();
    else
        $('#dv_Transmission').hide();

    if (originalbank == bankid) {
        if (selectedsvbOpt == 1) {
            $('#rbServiceBureauYes').prop('checked', true);
            if (svblimit > 0) {
                $('#txtService').val(selectedSvbAmt);
                $('#divsvbfees').show();
            }
        }
        else if (selectedsvbOpt == 2) {
            $('#rbServiceBureauNo').prop('checked', true);
            $('#divsvbfees').hide();
        }
        if (selectedTranOpt == 1) {
            $('#rbTransmissionYes').prop('checked', true);
            if (translimit > 0) {
                $('#txtTransmission').val(selectedTranAmt);
                $('#divtransfees').show();
            }
        }
        else if (selectedTranOpt == 2) {
            $('#rbTransmissionNo').prop('checked', true);
            $('#divtransfees').hide();
        }
    }


    //GetEnrollmentBankSelection(bankid);

}

function Onlydecimal(event, Id) {
    if (event.which != 8 && event.which != 0 && event.which != 46 && isNaN(String.fromCharCode(event.which))) {
        event.preventDefault(); //stop characters from entering input
    }

    var dotIndx = $('#' + Id).val().indexOf('.');
    if (Number(dotIndx) > 0 && event.which == 46) {
        event.preventDefault();
    }
}

function fnSaveEnrollmentBankSelection(type) {

    var req = {};
    var success = $('#success');
    success.html('');
    success.hide();
    var error = $('#error');
    error.html('');
    error.hide();
    var cansubmit = true;
    $('*').removeClass("error_msg");
    $('*').attr('title', '');

    //if ($('#chk00000000-0000-0000-0000-000000000000').prop('checked')) {
    //    error.show();
    //    error.append('Please select any one bank.\t\t\t');
    //    cansubmit = false;
    //}

    //if ($('input[name=chkBank]:checked').length == 0) {
    //    error.show();
    //    error.append('Please select at least one bank.\t\t\t');
    //    cansubmit = false;
    //}

    if ($('#dv_bankfees').css('display') != 'none') {

        if (_showsvb) {
            if ($('input[name=rbServiceBureau]:checked').length == 0) {
                $('#dvServiceBVal').addClass("error_msg");
                //error.show();
                //error.append('Please select Yes or No for Service Bureau Fees.\t\t\t');
                cansubmit = false;
            }
            if ($('#txtService').val().trim() == "" && $('#rbServiceBureauYes').prop('checked') && svblimit > 0) {
                $('#txtService').addClass("error_msg");
                $('#txtService').attr('title', 'Please enter amount');
                cansubmit = false;
            }

            if ($('#txtService').val().trim() != "" && $('#rbServiceBureauYes').prop('checked') && parseFloat($('#txtService').val().trim()) > svblimit) {
                $('#txtService').addClass("error_msg");
                $('#txtService').attr('title', 'Please enter less than  or equal to ' + svblimit);
                cansubmit = false;
            }
        }

        if (_showtrans) {
            if ($('input[name=rbTransmission]:checked').length == 0) {
                //error.show();
                //error.append('Please select Yes or No for Transmission Fees.\t\t\t');
                $('#dvTransmVal').addClass("error_msg");
                cansubmit = false;
            }
            if ($('#txtTransmission').val().trim() == "" && $('#rbTransmissionYes').prop('checked') && translimit > 0) {
                $('#txtTransmission').addClass("error_msg");
                $('#txtTransmission').attr('title', 'Please enter amount');
                cansubmit = false;
            }

            if ($('#txtTransmission').val().trim() != "" && $('#rbTransmissionYes').prop('checked') && parseFloat($('#txtTransmission').val().trim()) > translimit) {
                $('#txtTransmission').addClass("error_msg");
                $('#txtTransmission').attr('title', 'Please enter less than or equal to ' + translimit);
                cansubmit = false;
            }
        }
    }

    req.BankId = $('#h_BankID').val();
    var que = 0;
    //sub questions
    $.each($('input[type=radio].rbBank' + req.BankId), function (indx, valu) {
        var id = $(valu).attr('id');
        if ($('#' + id).is(':checked')) {
            que = $('#' + id).attr('value');
        }
    });

    if (que == 0 && $('input[type=radio].rbBank' + req.BankId).length > 0) {
        cansubmit = false;
        error.show();
        $('#divBankQuestions' + req.BankId).addClass('error_msg');
    }

    if (cansubmit) {
        req.UserId = $('#UserId').val();
        req.CustomerId = $('#UserId').val();
        req.QuestionId = que;
        req.IsServiceBureauFee = $('#rbServiceBureauYes').is(':Checked');
        req.ServiceBureauBankAmount = $('#txtService').val();
        req.IsTransmissionFee = $('#rbTransmissionYes').is(':Checked');
        req.TransmissionBankAmount = $('#txtTransmission').val();
        var Cid = getUrlVars()["Id"];
        var entitydisplayid = getUrlVars()["entitydisplayid"];

        if (Cid) {
            req.CustomerId = Cid;
        }

        if (entitydisplayid) {
            entitydisplayid = getUrlVars()["entitydisplayid"];
        }



        var Uri = '/api/EnrollmentBankSelection/SaveBankandFeesInfo';
        ajaxHelper(Uri, 'POST', req, false).done(function (data, status) {
            $("html, body").animate({ scrollTop: 0 }, "slow");
            if (data == 'true' || data == 'True' || data == true) {



                success.show();
                success.append('<p> Record saved successfully. </p>');
                SaveConfigStatusActive('done', req.BankId);
                //SaveConfigStatusActive("done", req.BankId);
                //if (req.BankId != "00000000-0000-0000-0000-000000000000") {
                //    SaveConfigStatusActive("done", req.BankId);
                //} else {
                //    SaveConfigStatusActive("none", req.BankId);
                //}

                //UpdateOfficeManagement(req.CustomerId)

                if (req.BankId != "00000000-0000-0000-0000-000000000000") {

                    if (type == 1) {

                        if ($('#entityid').val() != $('#myentityid').val()) {
                            window.location.href = '/Enrollment/BankEnrollment?Id=' + req.CustomerId + '&entitydisplayid=' + entitydisplayid + '&ParentId=' + $('#myparentid').val() + '&entityid=' + $('#myentityid').val() + '&ptype=enrollment&bankid=' + req.BankId;
                        }
                        else {
                            window.location.href = '/Enrollment/BankEnrollment?bankid=' + req.BankId;

                        }
                    }
                    else if (type == 0) {
                        BankUrlAppend(req.BankId);
                    }
                }
                else {

                    if (type == 1) {
                        if ($('#entityid').val() != $('#myentityid').val()) {
                            if ($('#site0eda5d25-591c-4e01-a845-fb580572cff5').length > 0)
                                window.location.href = $('#site0eda5d25-591c-4e01-a845-fb580572cff5').attr('href');
                            else if ($('#site0eda5d25-591c-4e01-a845-fb580572cfe8').length > 0)
                                window.location.href = $('#site0eda5d25-591c-4e01-a845-fb580572cfe8').attr('href');
                            else
                                window.location.href = '/Enrollment/EnrollmentSummary?Id=' + req.CustomerId + '&entitydisplayid=' + entitydisplayid + '&ParentId=' + $('#myparentid').val() + '&entityid=' + $('#myentityid').val() + '&ptype=enrollment';
                        }
                        else {
                            if ($('#site0eda5d25-591c-4e01-a845-fb580572cff5').length > 0)
                                window.location.href = $('#site0eda5d25-591c-4e01-a845-fb580572cff5').attr('href');
                            else if ($('#site0eda5d25-591c-4e01-a845-fb580572cfe8').length > 0)
                                window.location.href = $('#site0eda5d25-591c-4e01-a845-fb580572cfe8').attr('href');
                            else
                                window.location.href = '/Enrollment/EnrollmentSummary';
                        }
                    }
                }
                $('#site067c03a3-34f1-4143-beae-35327a8fca44').addClass('done');
                //getConfigStatus();
                return true;
            }
            else {
                error.show();
                error.append('<p>  Record not saved. </p>');
                return false;
            }
        });
    }
    else {
        $("html, body").animate({ scrollTop: 0 }, "slow");
        error.show();
        error.append('<p>  Please correct error(s). </p>');
    }

}

function GetEnrollmentBankSelection(bankid) {
    $('*').removeClass("error_msg");
    $('*').attr('title', '');
    $('#dv_aprsave').hide();
    $('#dv_save').show();
    $('#p_stagingsvb').hide();
    $('#p_stagingtrans').hide();

    fnRBSVB(0);
    fnRBTRANS(0);
    var UserId = $('#UserId').val();
    var parentid = $('#parentid').val();
    var entityid = $('#entityid').val();

    //var bankid = getUrlVars()["bankid"];

    var IsStaging = false;
    $('div[id^="divBankQuestions"]').hide();

    if ($('#entityid').val() != $('#myentityid').val()) {
        UserId = $('#myid').val();
        parentid = $('#myparentid').val();
    }
    if (getUrlVars()["Staging"])
        IsStaging = true;

    if (bankid == '' || bankid == null || bankid == undefined) {
        bankid = '00000000-0000-0000-0000-000000000000';
    }

    //if (bankid == '00000000-0000-0000-0000-000000000000') {
    //    $('#chkBE00000000-0000-0000-0000-000000000000').prop('checked', true);
    //    $('tr[id^="bankSelection_"]').hide();
    //    return;
    //}

    var custmorUri = '/api/EnrollmentBankSelection/EnrollmentBankSelection?userid=' + UserId + '&Parentid=' + parentid + '&IsStaging=' + IsStaging + '&bankid=' + bankid;
    ajaxHelper(custmorUri, 'GET', null, false).done(function (data) {
        

        if (data != null && data != '' && data != undefined && data.length > 0) {
            if (data[0].BankId != '00000000-0000-0000-0000-000000000000' && data[0].IsAvailable == 1) {
                bankid = data[0]["BankId"];
                $('#chkBE' + bankid).prop('checked', true);


                var svbtransamt = subsitebankfees.filter(function (i) { return i.BankId == bankid })[0];
                if (svbtransamt) {
                    var svbamt = $('#spn_svbamt').text();
                    var tramt = $('#spn_trsnamt').text();
                    $('#spn_svbamt').text(getRoundoff(parseFloat(svbamt) + svbtransamt.SvbAmount + data[0]["ServiceBureauBankAmount"]));
                    $('#spn_trsnamt').text(getRoundoff(parseFloat(tramt) + svbtransamt.TransAmount + data[0]["TransmissionBankAmount"]));
                }
                else {
                    var svbamt = $('#spn_svbamt').text();
                    var tramt = $('#spn_trsnamt').text();
                    $('#spn_svbamt').text(getRoundoff(parseFloat(svbamt) + data[0]["ServiceBureauBankAmount"]));
                    $('#spn_trsnamt').text(getRoundoff(parseFloat(tramt) + data[0]["TransmissionBankAmount"]));
                }
                selectedBank = bankid;
                originalbank = bankid;

                $('tr[id^="bankSelection_"]').hide();
                $('#bankSelection_' + bankid).show();

                $('#h_BankID').val(bankid);
                $('#divBankQuestions' + bankid).show();
                var name = $('#spn_' + bankid).text();

                $('#dvBankServiceInfo').html('');
                $('#dvBankTransmissionInfo').html('');
                $('#dvBankServiceInfo').append('<div class="col-md-12"><div><input type="text" id="txtService" value="' + data[0]["ServiceBureauBankAmount"] + '" name="txtService" onkeypress="Onlydecimal(event)" class="form-control"></div></div>');
                $('#dvBankTransmissionInfo').append('<div class="col-md-12"><div><input type="text" id="txtTransmission" value="' + data[0]["TransmissionBankAmount"] + '" name="txtTransmission" onkeypress="Onlydecimal(event)" class="form-control"></div></div>');


                $('#Qchk' + data[0]["QuestionId"]).prop('checked', true);

                if (data[0]["IsServiceBureauFee"] == true) {
                    $('#rbServiceBureauYes').prop('checked', true);
                    getfnRBSVB(1, data[0]["ServiceBureauBankAmount"]);
                }
                else {
                    $('#rbServiceBureauNo').prop('checked', true);
                }

                if (data[0]["IsTransmissionFee"] == true) {
                    $('#rbTransmissionYes').prop('checked', true);
                    getfnRBTRANS(1, data[0]["TransmissionBankAmount"]);
                }
                else {
                    $('#rbTransmissionNo').prop('checked', true);
                }

                selectedSvbAmt = parseFloat(data[0]["ServiceBureauBankAmount"]);
                selectedTranAmt = parseFloat(data[0]["TransmissionBankAmount"]);
                selectedsvbOpt = data[0]["IsServiceBureauFee"] ? 1 : 2;
                selectedTranOpt = data[0]["IsTransmissionFee"] ? 1 : 2;


                $('#dv_ServiceBureu, #dv_Transmission').hide();
                $('#h_dvService, #h_Trasnmission').val(0);

                if (data[0]["IsDVServiceBureauFee"] == true) {
                    $('#dv_ServiceBureu').show(); $('#h_dvService').val(1);
                }
                if (data[0]["IsDVTransmissionFee"] == true) {
                    $('#dv_Transmission').show(); $('#h_Trasnmission').val(1);
                }

                var bankopts = $('input[name=bank' + bankid + ']');
                if (data[0]["TPGOptions"] == 1 || data[0]["TPGOptions"] == 2) {
                    $.each(bankopts, function (indx, valu) {
                        var opts = $(valu).attr('option');
                        var optid = $(valu).attr('id');
                        if (data[0]["TPGOptions"] == opts)
                            $('#' + optid).prop('checked', true);
                    });
                    $('input[name=bank' + bankid + ']').attr('disabled', 'disabled');
                }
                else if (data[0]["TPGOptions"] == 3) {
                    $('input[name=bank' + bankid + ']').attr('disabled', 'disabled');
                    $.each(bankopts, function (indx, valu) {
                        $('input[name=bank' + bankid + ']').removeAttr('disabled', 'disabled');
                        var opts = $(valu).attr('option');
                        var optid = $(valu).attr('id');
                        if (data[0]["TPGOptions"] == opts) {
                            $('#' + optid).attr('disabled', 'disabled');
                        }
                    });
                }
                else {
                    $('input[name=bank' + bankid + ']').removeAttr('disabled', 'disabled');
                }

                var uri = '/api/EnrollmentBankSelection/getFeeLimit?CustomerId=' + UserId + '&ParentId=' + parentid + '&BankId=' + bankid;
                ajaxHelper(uri, 'GET', null, false).done(function (data) {
                    if (data) {
                        if (data.Status) {
                            svblimit = data.SvbFeeLimit;
                            translimit = data.TransFeeLimit;
                            _showsvb = data.ShowSvb;
                            _showtrans = data.ShoeTransmission;

                            $('#spn_svblimit').text(' ($ ' + data.SvbFeeLimit + ')');
                            $('#spn_translimit').text(' ($ ' + data.TransFeeLimit + ')');
                            if ($('#rbServiceBureauYes').prop('checked') && data.SvbFeeLimit > 0)
                                $('#divsvbfees').show();
                            if ($('#rbTransmissionYes').prop('checked') && data.TransFeeLimit > 0)
                                $('#divtransfees').show();
                        }
                    }
                })
                $('#dv_bankfees').show();
            }
            else {
                //$('#chkBE00000000-0000-0000-0000-000000000000').prop('checked', true);
                //$('tr[id^="bankSelection_"]').hide();


                if (bankid != '00000000-0000-0000-0000-000000000000' && data[0].IsAvailable == 2) {
                    getBankQuestionsShow_Enroll($('#chkBE' + bankid)); //chkBE2fb91ef5-efc4-4baf-abd7-ea05a8c100cc

                } else {
                    $('#chkBE00000000-0000-0000-0000-000000000000').prop('checked', true);
                    $('tr[id^="bankSelection_"]').hide();
                }
            }
            
            if (bankid != '00000000-0000-0000-0000-000000000000' && data[0].IsAvailable == 1) {
                getBankEnrollmentStatusSelection(UserId, bankid);
            }
        }
        else {
            // $('#chkBE00000000-0000-0000-0000-000000000000').prop('checked', true);
            //$('tr[id^="bankSelection_"]').hide();

            // if (data[0].IsAvailable == 2) {
            //     getBankQuestionsShow_Enroll($('#chkBE' + bankid)); //chkBE2fb91ef5-efc4-4baf-abd7-ea05a8c100cc
            // } else {
            $('#chkBE00000000-0000-0000-0000-000000000000').prop('checked', true);
            $('tr[id^="bankSelection_"]').hide();
            // }
        }

        //var uri = '/api/EnrollmentBankSelection/GetUnlockedBanks?userid=' + UserId;
        //ajaxHelper(uri, 'GET').done(function (data) {
        //    $.each(data, function (item, value) {
        //        $('#chkBE' + value).attr('disabled', 'disabled');
        //        $('#spn_' + value).attr('title', 'Not allowed as it was unlocked');
        //    })
        //});

        //var uri = '/api/EnrollmentBankSelection/GetRejectedBanks?userid=' + UserId;
        //ajaxHelper(uri, 'GET').done(function (data) {
        //    $.each(data, function (item, value) {
        //        $('#chkBE' + value).attr('disabled', 'disabled');
        //        $('#spn_' + value).attr('title', '');
        //    })
        //});
    });


    if (_showsvb)
        $('#dv_ServiceBureu').show();
    else
        $('#dv_ServiceBureu').hide();

    if (_showtrans)
        $('#dv_Transmission').show();
    else
        $('#dv_Transmission').hide();

    getOtherBankStatus(bankid, UserId);
    getIsSalesYearCheckBankDates_Enrollment();
}

function getIsSalesYearCheckBankDates_Enrollment() {
    var url = '/api/SubSiteFee/IsSalesYearBankLst';
    var Id;
    if ($('#entityid').val() != $('#myentityid').val()) {
        //if ($('#myentitydisplayid').val() == $('#Entity_MO').val() || $('#myentitydisplayid').val() == $('#Entity_SVB').val())
        //{ Id = $('#myid').val(); }
        //else { Id = $('#myparentid').val(); }

        Id = $('#myid').val();
    }
    else {
        Id = $('#UserId').val();
    }

    var error = $('#error_bank');
    error.html('');
    error.hide();
    var _flag = true;
    var _flag_A = false;
    if (Id != '' && Id != null && Id != '00000000-0000-0000-0000-000000000000') {
        ajaxHelper(url + '?Id=' + Id, 'POST').done(function (data) {
            $.each(data, function (ind, valu) {
                var Bankid = valu["BankId"];
                var Active = valu["Active"];
                if (Active == false) {
                    var stitle = 'The ability to update the Fees information is not available since the Cutoff date for the same is elapsed. Please contact the uTax support team. ';
                    //$('#chk' + Bankid).attr('disabled', 'disabled').attr('title', stitle);
                    //$('#chk' + Bankid + '_MSO').attr('disabled', 'disabled').attr('title', stitle);
                    //$('#spn_' + Bankid).attr('title', stitle);
                    //$('#spn_' + Bankid + '_MSO').attr('title', stitle);
                    $('#chkBE' + Bankid).attr('IsReadOnly', '1').attr('title', stitle);
                    $('#chkBE' + Bankid).prop('disabled', true).attr('title', stitle);
                    $('#spn_' + Bankid).attr('title', stitle);

                    _flag = false;
                } else {
                    if (!_isSubmitted) {
                        //$('#chk' + Bankid).removeAttr('disabled');
                        //$('#chk' + Bankid + '_MSO').removeAttr('disabled');
                        //$('#spn_' + Bankid).attr('title', '');
                        //$('#spn_' + Bankid + '_MSO').attr('title', '');
                        $('#chkBE' + Bankid).attr('IsReadOnly', '2').attr('title', '');
                        _flag_A = true;
                    }
                }
            });
        })
    }
    if (!_flag_A) {
        if (_flag == false || _flag == 'false' || _flag == 'False') {
            error.show();
            error.append('<p>The ability to update the Fees information is not available since the Cutoff date for the same is elapsed. Please contact the uTax support team. </p>');
            return false;
        }
    }
}

function fnRBSVB(obj) {

    $('#divsvbfees').hide();
    if (obj == 1 && svblimit > 0) {
        $('#divsvbfees').show();
    }
    if (obj == 0)
        $('#txtService').val('');
}

function fnRBTRANS(obj) {
    $('#divtransfees').hide();
    if (obj == 1 && translimit > 0) {
        $('#divtransfees').show();
    }
    if (obj == 0)
        $('#txtTransmission').val('');
}

function getfnRBSVB(obj, amount) {
    $('#divsvbfees').hide();
    if (obj == 1 && amount > 0) {
        $('#divsvbfees').show();
    }
}

function getfnRBTRANS(obj, amount) {
    $('#divtransfees').hide();
    if (obj == 1 && amount > 0) {
        $('#divtransfees').show();
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

function getBankEnrollmentStatusSelection(CustomerId, bankid) {

    var url = '/api/EnrollmentBankSelection/getBankEnrollmentStatus?CustomerId=' + CustomerId + '&bankid=' + bankid;
    ajaxHelper(url, 'GET', null, false).done(function (data) {
        
        if (data) {
            switch (data.SubmissionStaus) {
                case 'RDY':
                    //DeactivateSelection();
                    break;
                case 'SUB':
                    DeactivateSelection();
                    break;
                case 'APR':
                    //ApprovedBankEnrollmentChange();
                    break;
                case 'REJ':
                case 'CAN':
                    //$('#chkBE' + data.BankId).attr('disabled', 'disabled');
                    break;
                case 'PEN':
                    DeactivateSelection();
                    break;
                case 'DEN':
                    //$('#chkBE' + data.BankId).attr('disabled', 'disabled');
                    break;
                default:
                    break;
            }

            
            if (data.SubmissionStaus == 'APR') {
                
                var islock = false;
                var unlockuri = '/api/BankSelection/CheckUnlock?CustomerId=' + CustomerId;
                ajaxHelper(unlockuri, 'POST', null, false).done(function (res) {
                    
                    if (!res) {
                        islock = true;
                        $('#dv_save').hide();
                        if ($('#dv_bankfees').css('display') != 'none') {
                            $('#dv_aprsave').show();
                            $('#btn_submitenr').hide();
                        }
                        $('input').attr('disabled', 'disabled');
                        //$('.bank-selection-addon-edit').show();
                        // $('div#dv_bankSelection input').attr('disabled', 'disabled');
                    }
                    else {
                        $('#dv_save').show();
                        $('#dv_aprsave').hide();
                        //$('.bank-selection-addon-edit').hide();
                        $('input').removeAttr('disabled');
                        $('a.btn-info').css('pointer-events', '');
                        $('a.btn-info').removeAttr('disabled');
                    }
                })

                IsLocked = !islock;

                if (islock) {
                    var uri = '/api/EnrollmentBankSelection/getStagingAddon?CustomerId=' + CustomerId + '&BankId=' + bankid;
                    ajaxHelper(uri, 'GET').done(function (res) {
                        
                        if (res) {
                            if (res.IsSvbFee) {
                                if ($('#rbServiceBureauYes').prop('checked')) {
                                    $('#p_stagingsvb').html('Previously changed to <span>' + res.SvbAmount + '</span>').show();
                                }
                                else {
                                    $('#p_stagingsvb').html('Previously changed to <span>Yes</span> and added <span>' + res.SvbAmount + '</span>').show();
                                }
                            }
                            else {
                                if ($('#rbServiceBureauYes').prop('checked')) {
                                    $('#p_stagingsvb').html('Previously changed to <span>No</span>').show();
                                }
                            }

                            if (res.IsTransmissionFee) {
                                if ($('#rbTransmissionYes').prop('checked')) {
                                    $('#p_stagingtrans').html('Previously changed to <span>' + res.TransmissionAmount + '</span>').show();
                                }
                                else {
                                    $('#p_stagingtrans').html('Previously changed to <span>Yes</span> and added <span>' + res.TransmissionAmount + '</span>').show();
                                }
                            }
                            else {
                                if ($('#rbTransmissionYes').prop('checked')) {
                                    $('#p_stagingtrans').html('Previously changed to <span>No</span>').show();
                                }
                            }
                        }
                    })
                }
            }

            if (data.SubmissionCount > 0) {
                $('input').attr('disabled', 'disabled');
                $('a.btn-info').css('pointer-events', 'none');
                $('a.btn-info').attr('disabled', 'disabled');
            }
        }
    })
}

function DeactivateSelection() {
    _isSubmitted = true;
    setTimeout(function () {
        $('input').attr('disabled', 'disabled');
        $('a.btn-info').css('pointer-events', 'none');
        $('a.btn-info').attr('disabled', 'disabled');
    })
}

function ApprovedBankEnrollmentChange() {
    $('#dv_save').hide();
    if ($('#dv_bankfees').css('display') != 'none')
        $('#dv_aprsave').show();
    $('.bank-selection-addon-edit').show();
    setTimeout(function () {
        // $('input').attr('disabled', 'disabled');
    });
}

function SubmitEnrollmentBankFee() {
    if ($('#dv_ServiceBureu input').attr('disabled') == 'disabled' && $('#dv_Transmission input').attr('disabled') == 'disabled')
        return;

    var req = {};
    var success = $('#success');
    success.html('');
    success.hide();
    var error = $('#error');
    error.html('');
    error.hide();
    var cansubmit = true;
    $('*').removeClass("error_msg");
    $('*').attr('title', '');

    if (_showsvb) {
        if ($('input[name=rbServiceBureau]:checked').length == 0) {
            $('#dvServiceBVal').addClass("error_msg");
            cansubmit = false;
        }
        if ($('#txtService').val().trim() == "" && $('#rbServiceBureauYes').prop('checked') && svblimit > 0) {
            $('#txtService').addClass("error_msg");
            $('#txtService').attr('title', 'Please enter amount');
            cansubmit = false;
        }
        if ($('#txtService').val().trim() != "" && $('#rbServiceBureauYes').prop('checked') && parseFloat($('#txtService').val().trim()) > svblimit) {
            $('#txtService').addClass("error_msg");
            $('#txtService').attr('title', 'Please enter less than  or equal to ' + svblimit);
            cansubmit = false;
        }
    }

    if (_showtrans) {
        if ($('input[name=rbTransmission]:checked').length == 0) {
            $('#dvTransmVal').addClass("error_msg");
            cansubmit = false;
        }
        if ($('#txtTransmission').val().trim() == "" && $('#rbTransmissionYes').prop('checked') && translimit > 0) {
            $('#txtTransmission').addClass("error_msg");
            $('#txtTransmission').attr('title', 'Please enter amount');
            cansubmit = false;
        }
        if ($('#txtTransmission').val().trim() != "" && $('#rbTransmissionYes').prop('checked') && parseFloat($('#txtTransmission').val().trim()) > translimit) {
            $('#txtTransmission').addClass("error_msg");
            $('#txtTransmission').attr('title', 'Please enter less than or equal to ' + translimit);
            cansubmit = false;
        }
    }

    if (cansubmit) {
        req.UserId = $('#LoginId').val();
        req.CustomerId = $('#UserId').val();
        req.IsSvbFee = $('#rbServiceBureauYes').is(':Checked');
        req.SvbAmount = $('#txtService').val();
        req.IsTransmissionFee = $('#rbTransmissionYes').is(':Checked');
        req.TransmissionAmount = $('#txtTransmission').val();
        req.BankId = getUrlVars()["bankid"];

        var Cid = getUrlVars()["Id"];
        var entitydisplayid = '';

        if (Cid) {
            req.CustomerId = Cid;
            entitydisplayid = getUrlVars()["entitydisplayid"];
        }


        var Uri = '/api/EnrollmentBankSelection/UpdateAddonforEnrollment';
        ajaxHelper(Uri, 'POST', req).done(function (data) {
            if (data) {
                if (data.Status) {
                    if (Cid)
                        window.location.href = '/Enrollment/enrollmentsummary?Id=' + req.CustomerId + '&entitydisplayid=' + $('#myentitydisplayid').val() + '&ParentId=' + $('#myparentid').val() + '&entityid=' + $('#myentityid').val() + '&ptype=enrollment&Staging=' + data.StagingId + '&bankid=' + req.BankId;
                    else
                        window.location.href = '/Enrollment/EnrollmentSummary?Staging=' + data.StagingId + '&bankid=' + req.BankId;
                }
                else {

                }
            }
        });
    }
    else {
        $("html, body").animate({ scrollTop: 0 }, "slow");
        error.show();
        error.append('<p>  Please correct error(s). </p>');
    }
}

function EnableAddon() {
    $('#dv_ServiceBureu input').removeAttr('disabled');
    $('#dv_Transmission input').removeAttr('disabled');
    $('#btn_submitenr').show();
}

function getOtherBankStatus(bankid, CustomerId) {
    var uri = '/api/BankSelection/getOtherBankStatus?CustomerId=' + CustomerId + '&bankid=' + bankid;
    ajaxHelper(uri, 'GET').done(function (res) {
        if (res) {
            $.each(res, function (item, value) {
                $('#chkBE' + value).attr('disabled', 'disabled');
            })
        }
    })
}