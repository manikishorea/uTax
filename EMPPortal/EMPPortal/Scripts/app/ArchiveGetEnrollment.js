function ArchiveEnrollment(Id, ParentId, EntityId) {
    // var Id = $('#UserId').val();
    var url = '/api/Archive/Enrollment?Id=' + Id + '&parentid=' + ParentId;
    if (Id != '' && Id != null && Id != '00000000-0000-0000-0000-000000000000') {
        ajaxHelper(url, 'GET').done(function (data) {


            getArchivedCustomerInfo(data.CustomerInformationDTO);
            getArchivedLoginInfo(data.CustomerLoginInformationDTO);
            getArchivedParentInfo(data.ParentInformationDTO);

            var entityid = $('#myentityid').val();
            getArchiveAffiliateEnrollment($('#divEnrollAffiliates'), Id, EntityId);

            $('#diviProtect').hide();

            getArchiveEnrollOfficeConfig(data.EnrollmentOfficeConfigDTO);
            getArchiveEnrollAffiliate(data.EnrollmentAffiliateConfigDTOs);

            debugger;
            getArchiveEnrollBankQuestions($('#divEnrollBanks'));
            GetArchiveEnrollmentBankSelection(data.EnrollmentBankSelectDTOs);

            // $('#libankselection').hide();
            // $('#dvbankselection').hide();

            $('#libankenrollment').hide();
            // $('#dvbankenrollment').hide();

            $('#lifee').hide();
            // $('#dvfeereim').hide();

            $('#liefilingpaymentopt').hide();
            // $('#dvefilingpaymentopt').hide();

            $('#lioutstandingbalance').hide();
            //$('#dvoutstandingbalance').hide();

            $('#lienrollbankstatus').hide();
            // $('#dvenrollbankstatus').hide();

            if (data.EnrollmentBankSelectDTOs.length > 0) {


                // $('#dvbankenrollment').show();

                $('#liefilingpaymentopt').show();
                // $('#dvefilingpaymentopt').show();

                $('#lioutstandingbalance').show();
                // $('#dvoutstandingbalance').show();

                $('#lienrollbankstatus').show();
                // $('#dvenrollbankstatus').show();


                if (data.EnrollmentBankSelectDTOs[0].BankCode == "S") {
                    getArchiveBankEnrollmentData("S", data.TPGBankEnrollmentDTO);
                    $('#libankenrollment').show();
                }
                else if (data.EnrollmentBankSelectDTOs[0].BankCode == "R") {
                    getArchiveBankEnrollmentData("R", data.RBBankEnrollmentDTO);
                    $('#libankenrollment').show();
                }
                else {
                    getArchiveBankEnrollmentData("V", data.RABankEnrollmentDTO);
                    $('#libankenrollment').show();
                }

                if (data.EnrollmentBankSelectDTOs[0].ServiceBureauBankAmount != 0 && data.EnrollmentBankSelectDTOs[0].ServiceBureauBankAmount != 0) {

                    if (entityid == $('#Entity_SO').val() || entityid == $('#Entity_SOME').val()) {
                        getEnrollFeeReimbersment(data.EnrollmentFeeReimbursementDTO);
                        $('#lifee').show();
                        //  $('#dvfeereim').show();
                    }
                }

                if (entityid == $('#Entity_SO').val() || entityid == $('#Entity_SOME').val() || entityid == $('#Entity_MO').val() || entityid == $('#Entity_SVB').val()) {

                    if (data.eFilePaymentInfoDTO.status) {
                        getArchiveeFilePaymentInfo(data.eFilePaymentInfoDTO);
                    }
                    else {
                        $('#liefilingpaymentopt').hide();
                        $('#dvefilingpaymentopt').hide();
                    }

                    if (data.OutstandingPaymentInfoDTO.status) {
                        getArchiveOutStandingPaymentInfo(data.OutstandingPaymentInfoDTO);
                    }
                    else {
                        $('#lioutstandingbalance').hide();
                        $('#dvoutstandingbalance').hide();
                    }
                }
                else {
                    $('#liefilingpaymentopt').hide();
                    //  $('#dvefilingpaymentopt').hide();
                    $('#lioutstandingbalance').hide();
                    //  $('#dvoutstandingbalance').hide();
                }

            }


            getCustomerBanks(data.CustomerBanksResponseDTO);
        });
    }
}

//// Enroll Config
function getArchiveEnrollOfficeConfig(data) {

    if (data != null && data != '' && data != undefined) {
        $('#dvofficeConfig_nocontent')
                    .html('')
                    .hide();

        $('#Id').val(data["Id"]);

        if (data["IsMainSiteTransmitTaxReturn"] == 'true' || data["IsMainSiteTransmitTaxReturn"] == 'True' || data["IsMainSiteTransmitTaxReturn"] == true) {
            $('#IsMainSiteTransmitTaxReturn1').html('Yes');
            $('#IsMainSiteTransmitTaxReturn').val(1);

        } else if (data["IsMainSiteTransmitTaxReturn"] == 'false' || data["IsMainSiteTransmitTaxReturn"] == 'False' || data["IsMainSiteTransmitTaxReturn"] == false) {
            $('#IsMainSiteTransmitTaxReturn1').html('No');
            $('#IsMainSiteTransmitTaxReturn').val(0);
        }


        $('#NoofTaxProfessionals').val(data["NoofTaxProfessionals"]);

        if (data["IsSoftwareOnNetwork"] == 'true' || data["IsSoftwareOnNetwork"] == 'True' || data["IsSoftwareOnNetwork"] == true) {
            $('#IsSoftwareOnNetwork1').prop('checked', true);

        } else if (data["IsSoftwareOnNetwork"] == 'false' || data["IsSoftwareOnNetwork"] == 'False' || data["IsSoftwareOnNetwork"] == false) {
            $('#IsSoftwareOnNetwork2').prop('checked', true);
        }


        $('#NoofComputers').val(data["NoofComputers"]);

        if (data["PreferredLanguage"] == 'true' || data["PreferredLanguage"] == 'True' || data["PreferredLanguage"] == true || data["PreferredLanguage"] == 1) {
            $('#PreferredLanguage1').prop('checked', true);

        } else if (data["PreferredLanguage"] == 'false' || data["PreferredLanguage"] == 'False' || data["PreferredLanguage"] == false || data["PreferredLanguage"] == 2) {
            $('#PreferredLanguage2').prop('checked', true);
        }
    } else {

        $('#dvofficeConfig_nocontent')
          .html('No Record Found');

        $('#dvofficeConfig_content')
                    .html('')
                    .hide();
    }

}


//// Enroll Affiliate
//
function getArchiveEnrollAffiliate(data) {
    if (data != null && data != '' && data != undefined) {

        $('#dvaffiliateConfig_nocontent')
                   .html('')
                   .hide();

        $.each(data, function (coIndex, c) {

            $('#chkAE' + c["AffiliateProgramId"]).attr('checked', 'checked');
            var chktext = $('#chkAE' + c["AffiliateProgramId"]).attr('chkname');
            var IsAutoEnrollAffiliateProgram = c["IsAutoEnrollAffiliateProgram"];
            if (IsAutoEnrollAffiliateProgram) {
                $('#chkAE' + c["AffiliateProgramId"]).attr('disabled', 'disabled');
            }
            if (chktext == 'iProtect') {
                $('#diviProtect').show();
            }
            $('#chkAE' + c["AffiliateProgramId"] + '_charge').val(c["AffiliateProgramCharge"]);
        });
    } else {

        $('#dvaffiliateConfig_content')
               .html('')
               .hide();

        $('#dvaffiliateConfig_nocontent')
               .html('No Record Found');
    }
}


//// Fee Reim
function getEnrollFeeReimbersment(data) {

    if (data != null && data != '' && data != undefined) {

        $('#dvfeereim_nocontent')
                 .html('')
                 .hide();

        $('#ID').val(data["ID"]);
        $('#txtNameofAccount').val(data["AccountName"]);
        $('#txtBankName').val(data["BankName"]);

        if (data["AccountType"] == true) {
            $('#rbAccountTypeYes').prop('checked', true);
        }
        else {
            $('#rbAccountTypeNo').prop('checked', true);
        }
        $('#txtRTN').val(data["RTN"]);
        $('#txtConfirmRTN').val(data["RTN"]);

        $('#txtBankAccount').val(data["BankAccountNo"]);
        $('#txtConfirmBankAccount').val(data["BankAccountNo"]);

        if (data["IsAuthorize"] == true) {
            $('#chkAughorize').prop('checked', true);
        } else {
            $('#chkAughorize').prop('checked', false);
        }
    } else {

        $('#dvfeereim_content')
             .html('')
             .hide();

        $('#dvfeereim_nocontent')
               .html('No Record Found');
    }
}


//// Bank Selection
//

function getArchiveEnrollBankQuestions(container) {
    container.html('');
    var entityid = $('#entityid').val();
    var CustomerId = $('#UserId').val();

    if ($('#entityid').val() != $('#myentityid').val()) {
        CustomerId = $('#myid').val();
        entityid = $('#myentityid').val();
    }

    getArchiveEnrollmentFees($('#tdFeeFixedEnroll'), $('#tdFeeUserEnroll'));

    var custmorUri = '/api/dropdown/banksubquestionForSelection?entityid=' + entityid + '&CustomerId=' + CustomerId;
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
                    $divBank.append('&nbsp;<label Id="spn_' + bankid + '"><input type="radio" Id="chkBE' + bankid + '" name="chkBankEnroll"  value="' + bankid + '" onchange="getBankQuestionsShow_Enroll(this)"  bankname="' + bankname + '" bankid="' + bankid + '"><span class="circle"></span><span class="check"></span>' + bankname + '<a href="' + docpath + '" target="_blank" title="Download"> <i class="fa fa-arrow-circle-down"></i> </a></label>');
                } else {
                    $divBank.append('&nbsp;<label Id="spn_' + bankid + '"><input type="radio" Id="chkBE' + bankid + '" name="chkBankEnroll" value="' + bankid + '" onchange="getBankQuestionsShow_Enroll(this)"  bankname="' + bankname + '" bankid="' + bankid + '"><span class="circle"></span><span class="check"></span> ' + bankname + '</label>');
                }
                var $head = $('<div/>').addClass('form-group bank-details');

                if (c.Questions.length > 0) {
                    $head.attr('id', 'divBankQuestions' + bankid).attr('style', 'display:none');
                    $head.append('<label>How will you print your ' + bankname + ' Checks?</label>');
                }
                if (bankname.toLowerCase() == 'tpg') {
                    $head.append('<div class="radio"><label><input type="radio" Id="Qchka29b3547-8954-4036-9bd3-312f1d6a3faa" option="1"  name="bankTPG" class="rbBank' + bankid + '" value="a29b3547-8954-4036-9bd3-312f1d6a3faa"  /><span class="circle"></span><span class="check"></span> From TPG\'s Webiste </label></div>');
                    $head.append('<div class="radio"><label><input type="radio" Id="Qchka29b3547-8954-4036-9bd3-312f1d6a3fba" option="2"  name="bankTPG" class="rbBank' + bankid + '" value="a29b3547-8954-4036-9bd3-312f1d6a3fba"  /><span class="circle"></span><span class="check"></span> From within the tax software </label></div>');
                }
                else {
                    $.each(c.Questions, function (rowIndex, r) {
                        var Options = r["Description"];
                        $head.append('<div class="radio"><label><input type="radio" Id="Qchk' + r["Id"] + '" option="' + Options + '"  name="bank' + bankid + '" class="rbBank' + bankid + '" value="' + r["Id"] + '"  /><span class="circle"></span><span class="check"></span> ' + r["Name"] + '</label></div>');
                    });
                }
                $divBankQues.append($head)

            });

            container.append($divBank);
            container.append($divBankQues);
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

function GetArchiveEnrollmentBankSelection(data) {
    fnRBSVB(0);
    fnRBTRANS(0);
    var UserId = $('#UserId').val();
    var parentid = $('#parentid').val();

    if (data != null && data != '' && data != undefined && data.length > 0) {

        $('#dvbankselection_nocontent')
             .html('')
             .hide();

        if (data[0].BankId != '00000000-0000-0000-0000-000000000000') {

            getArchiveBankSelected(data[0].BankCode);

            var bankid = data[0]["BankId"];
            $('#chkBE' + bankid).prop('checked', true);

            $('tr[id^="bankSelection_"]').hide();
            $('#bankSelection_' + bankid).show();

            $('#h_BankID').val(bankid);
            $('#divEnrollBanks #divBankQuestions' + bankid).show();
            var name = $('#spn_' + bankid).text();

            $('#dvBankServiceInfo').append('<div class="col-md-4"><div class="form-group"><label>' + name + '</label><input type="text" id="txtService" value="' + data[0]["ServiceBureauBankAmount"] + '" name="txtService" class="form-control"></div></div>');
            $('#dvBankTransmissionInfo').append('<div class="col-md-4"><div class="form-group"><label>' + name + '</label><input type="text" id="txtTransmission" value="' + data[0]["TransmissionBankAmount"] + '" name="txtTransmission" class="form-control"></div></div>');


            $('#Qchk' + data[0]["QuestionId"]).prop('checked', true);

            if (data[0]["IsServiceBureauFee"] == true) {
                $('#rbServiceBureauYes').prop('checked', true);
                getfnRBSVB(1);
            }
            else {
                $('#rbServiceBureauNo').prop('checked', true);
            }

            if (data[0]["IsTransmissionFee"] == true) {
                $('#rbTransmissionYes').prop('checked', true);
                getfnRBTRANS(1);
            }
            else {
                $('#rbTransmissionNo').prop('checked', true);
            }

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

            $('#dv_bankfees').show();

        }
        else {
            $('#chkBE00000000-0000-0000-0000-000000000000').prop('checked', true);
            $('tr[id^="bankSelection_"]').hide();
        }

        //$('#divBankQuestions' + bankid).show();
    }
    else {
        $('#chkBE00000000-0000-0000-0000-000000000000').prop('checked', true);
        $('tr[id^="bankSelection_"]').hide();


        $('#dvbankselection_content')
             .html('')
             .hide();

        $('#dvbankselection_nocontent')
               .html('No Record Found');
    }

    // getBankEnrollmentStatus(UserId);
}

function getArchiveBankEnrollmentStatus(CustomerId) {
    var url = '/api/Archive/getBankEnrollmentStatus?CustomerId=' + CustomerId;
    ajaxHelper(url, 'GET').done(function (data) {
        if (data) {
            switch (data.SubmissionStaus) {
                case 'RDY':
                    DeactivateSelection();
                    break;
                case 'SUB':
                    DeactivateSelection();
                    break;
                case 'APR':
                    DeactivateSelection();
                    break;
                case 'REJ':
                    $('#chkBE' + data.BankId).attr('disabled', 'disabled');
                    break;
                case 'PEN':
                    DeactivateSelection();
                    break;
                case 'DEN':
                case 'CAN':
                    $('#chkBE' + data.BankId).attr('disabled', 'disabled');
                    break;
                default:
                    break;
            }
        }
    })
}

function fnRBSVB(obj) {

    $('#divsvbfees').hide();
    if (obj == 1 && svblimit > 0) {
        $('#divsvbfees').show();
    }
}

function fnRBTRANS(obj) {
    $('#divtransfees').hide();
    if (obj == 1 && translimit > 0) {
        $('#divtransfees').show();
    }
}

function getfnRBSVB(obj) {
    $('#divsvbfees').hide();
    if (obj == 1) {
        $('#divsvbfees').show();
    }
}

function getfnRBTRANS(obj) {
    $('#divtransfees').hide();
    if (obj == 1) {
        $('#divtransfees').show();
    }
}

function getArchiveEnrollmentFees(container1, container2) {

    container1.html('');
    container2.html('');
    var entityid = $('#entityid').val();
    var userid = $('#UserId').val();
    var parentid = '';
    if ($('#entityid').val() != $('#myentityid').val()) {
        userid = $('#myid').val();
        parentid = $('#myparentid').val();
        entityid = $('#myentityid').val();
    }

    var bankfeeuri = '/api/Archive/getBankFee?CustomerId=' + userid;// + '&bankid=' + bankid;
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
                else if (r["Name"] == 'Transmission Fees')
                    feeid = 'spn_trsnamt';
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


//// Get Bank Enrollment Data

function getArchiveBankSelected(data) {


    $.blockUI({ message: 'getting details...' });

    //var Uri = '/api/EnrollmentBankSelection/GetBankSelectedByCustomer?CustomerId=' + CustId;
    //ajaxHelper(Uri, 'GET').done(function (data, status) {

    if (data == 'S') {
        banktpye = 'S';
        $.ajax({
            type: 'POST',
            url: '/Enrollment/ArcBankEnrollmentTPG',
            async: false,
            data: {},
            success: function (res) {
                $('#dv_Enrollment').html('');
                $('#dv_Enrollment').append(res);
            }
        })
    }
    else if (data == 'R') {
        banktpye = 'R';
        $.ajax({
            type: 'POST',
            url: '/Enrollment/ArcBankEnrollmentRB',
            async: false,
            data: {},
            success: function (res) {
                $('#dv_Enrollment').html('');
                $('#dv_Enrollment').append(res);
            }
        })
    }
    else if (data == 'V') {
        banktpye = 'V';
        $.ajax({
            type: 'POST',
            url: '/Enrollment/ArcBankEnrollmentRA',
            async: false,
            data: {},
            success: function (res) {
                $('#dv_Enrollment').html('');
                $('#dv_Enrollment').append(res);
            }
        })
    }
    // });
    var containers = [];
    containers.push($('#ddl_officestate'));
    containers.push($('#ddl_shipstate'));
    containers.push($('#ddl_ownerstate'));
    containers.push($('#ddl_owneridstate'));

    if (data == 'R') {

        containers.push($('#ddl_mailstate'));
        containers.push($('#ddl_fullstate'));
        containers.push($('#ddl_efinownerstate'));
        containers.push($('#ddl_idstate'));
    }

    getStateMasterCodeMultiple(containers);
    //  getBankEnrollmentStatus(CustId);

    $.unblockUI();

}


function getBankEnrollmentStatus(CustomerId, bankid) {
    var url = '/api/EnrollmentBankSelection/getBankEnrollmentStatus?CustomerId=' + CustomerId + '&bankid=' + bankid;
    ajaxHelper(url, 'GET').done(function (data) {
        if (data) {

            switch (data.SubmissionStaus) {
                case 'RDY':
                    if (!isDataAvailable) {
                        $('#p_pendingmsg').show();
                        $('#dv_accrej').hide();
                    }
                    break;
                case 'SUB':
                    break;
                case 'APR':
                    break;
                case 'REJ':
                case 'CAN':
                    break;
                case 'PEN':
                    break;
                case 'DEN':
                    break;
                default:
                    break;
            }
        }
    })
}

function getArchiveBankEnrollmentData(bankcode, data) {

    if (data) {

        setTimeout(function () {

            if (bankcode == 'S') {
                // var Uri = '/api/EnrollmentBankSelection/GetTPGBankEnrollment?CustomerId=' + CustId;
                //  ajaxHelper(Uri, 'GET').done(function (data, status) {
                if (data) {
                    $('#txt_CompanyName').val(data.CompanyName);
                    $('#txt_ManagerFN').val(data.ManagerFirstName);
                    $('#txt_ManagerLN').val(data.ManagerLastName);
                    $('#txt_OfficeAddress').val(data.OfficeAddress);
                    $('#txt_OfficeCity').val(data.OfficeCity);
                    $('#ddl_officestate').val(data.OfficeState);
                    $('#txt_OfficeZip').val(data.OfficeZip);
                    $('#txt_OfficeTel').val(data.OfficeTelephone);
                    $('#txt_OfficeFax').val(data.OfficeFax);
                    $('#txt_ShipAddress').val(data.ShippingAddress);
                    $('#txt_ShipCity').val(data.ShippingCity);
                    $('#ddl_shipstate').val(data.ShippingState);
                    $('#txt_ShipZip').val(data.ShippingZip);
                    $('#txt_ManagerEmail').val(data.ManagerEmail);
                    $('#txt_OwnerEin').val(data.OwnerEIN);
                    $('#txt_OwnerSSN').val(data.OwnerSSn);
                    $('#txt_OwnerFN').val(data.OwnerFirstName);
                    $('#txt_OwnerLN').val(data.OwnerLastName);
                    $('#txt_OwnerAddress').val(data.OwnerAddress);
                    $('#txt_OwnerCity').val(data.OwnerCity);
                    $('#ddl_ownerstate').val(data.OwnerState);
                    $('#txt_OwnerZip').val(data.OwnerZip);
                    $('#txt_OwnerTel').val(data.OwnerTelephone);
                    if (data.OwnerDOB)
                        $('#txt_OwnerDOB').datepicker('update', new Date(data.OwnerDOB));
                    else
                        $('#txt_OwnerDOB').val('');
                    $('#txt_OwnerEmail').val(data.OwnerEmail);
                    $('#ddl_BankLY').val(data.LastYearBank);
                    $('#txt_EFINLY').val(data.LastYearEFIN);
                    $('#txt_LYVolume').val(data.LastYearVolume);
                    $('#txt_BPF').val(data.BankProductFund);
                    $('#txt_OfficeRTN').val(data.OfficeRTN);
                    $('#txt_ConfirmOfficeRTN').val(data.OfficeRTN);
                    $('#txt_OfficeDAN').val(data.OfficeDAN);
                    $('#txt_ConfirmOfficeDAN').val(data.OfficeDAN);
                    $('#ddl_accttype').val(data.AccountType);
                    $('#txt_OwnerTitle').val(data.EfinOwnerTitle);
                    //$('#txt_OwnerMobile').val(data.EfinOwnerMobile);
                    $('#txt_OwnerIDNumber').val(data.EfinIDNumber);
                    $('#ddl_owneridstate').val(data.EfinIdState);
                    $('#txt_addonfee').val(data.Addonfee);
                    $('#txt_addonfee').attr('title', data.AddonfeeTitle);
                    $('#txt_sbfee').val(data.ServiceBureaufee);
                    $('#txt_sbfee').attr('title', data.ServiceBureaufeeTitle);

                    $('#lblTranFeeSummary').attr('title', data.AddonfeeTitle);
                    $('#lblSVBFeeSummary').attr('title', data.ServiceBureaufeeTitle);

                    $('#chk_agreebank').prop('checked', data.AgreeBank);
                    if (data.SbfeeAll == 'X')
                        $('#chk_feeall').prop('checked', true);
                    else
                        $('#chk_feeall').prop('checked', false);
                    $('#txt_docprep').val(data.DocPrepFee);
                    $('#txt_bankname').val(data.BankName);
                    $('#txt_accountname').val(data.AccountName);
                    if (data.AgreeBank) {
                        $('#chk_agreebank').attr('disabled', 'disabled');
                    }
                }
                //})
            }
            else if (bankcode == 'V') {
                //var Uri = '/api/EnrollmentBankSelection/GetRABankEnrollment?CustomerId=' + CustId;
                //ajaxHelper(Uri, 'GET').done(function (data, status) {
                if (data) {
                    $('#txt_owneremail').val(data.OwnerEmail);
                    $('#txt_ownerFN').val(data.OwnerFirstName);
                    $('#txt_ownerLN').val(data.OwnerLastName);
                    $('#txt_ownerSSN').val(data.OwnerSSN);
                    if (data.OwnerDOB)
                        $('#txt_ownerDOB').datepicker('update', new Date(data.OwnerDOB));
                    else
                        $('#txt_ownerDOB').val();
                    $('#txt_ownercell').val(data.OwnerCellPhone);
                    $('#txt_ownerphone').val(data.OwnerHomePhone);
                    $('#txt_ownerAddress').val(data.OwnerAddress);
                    $('#txt_ownerCity').val(data.OwnerCity);
                    $('#ddl_ownerstate').val(data.OwnerState);
                    $('#txt_ownerzip').val(data.OwnerZipCode);
                    $('#txt_ownerissuenumber').val(data.OwnerStateIssuedIdNumber);
                    $('#ddl_issuestate').val(data.OwnerIssuingState);
                    $('#txt_companyname').val(data.EROOfficeName);
                    $('#txt_officeaddress').val(data.EROOfficeAddress);
                    $('#txt_officecity').val(data.EROOfficeCity);
                    $('#ddl_officestate').val(data.EROOfficeState);
                    $('#txt_officezip').val(data.EROOfficeZipCoce);
                    $('#txt_officephone').val(data.EROOfficePhone);
                    $('#txt_mailaddress').val(data.EROMaillingAddress);
                    $('#txt_mailcity').val(data.EROMailingCity);
                    $('#ddl_mailstate').val(data.EROMailingState);
                    $('#txt_mailzip').val(data.EROMailingZipcode);
                    $('#txt_shipaddress').val(data.EROShippingAddress);
                    $('#txt_shipcity').val(data.EROShippingCity);
                    $('#ddl_shipstate').val(data.EROShippingState);
                    $('#txt_shipzip').val(data.EROShippingZip);
                    $('#txt_IRSaddress').val(data.IRSAddress);
                    $('#txt_IRScity').val(data.IRSCity);
                    $('#ddl_IRSstate').val(data.IRSState);
                    $('#txt_IRSzip').val(data.IRSZipcode);
                    $('#txt_pyvolume').val(data.PreviousYearVolume);
                    $('#txt_cyvolume').val(data.ExpectedCurrentYearVolume);
                    $('#txt_pybank').val(data.PreviousBankName);
                    $('#ddl_corporationtype').val(data.CorporationType);
                    $('#txt_businessowner').val(data.CollectionofBusinessOwners);
                    $('#txt_nonbusinessowner').val(data.CollectionOfOtherOwners);
                    $('#txt_noofyears').val(data.NoofYearsExperience);
                    $('#ddl_hasassociated').val(data.HasAssociatedWithVictims);
                    $('#txt_federalnumber').val(data.BusinessFederalIDNumber);
                    $('#txt_businessein').val(data.BusinessEIN);
                    $('#txt_ownerstitle').val(data.EFINOwnersSite);
                    $('#ddl_prevclient').val(data.IsLastYearClient);
                    $('#txt_routingnumber').val(data.BankRoutingNumber);
                    $('#txt_confirmroutingnumber').val(data.BankRoutingNumber);
                    $('#txt_bankaccountno').val(data.BankAccountNumber);
                    $('#txt_confirmbankaccountno').val(data.BankAccountNumber);
                    $('#ddl_bankaccounttype').val(data.BankAccountType);

                    $('#txt_ownertitle').val(data.OwnerTitle);
                    if (data.SbFeeall == 'X')
                        $('#chk_feeall').prop('checked', true);

                    $('#txt_addonfee').val(data.TransmissionAddon);
                    $('#txt_sbfee').val(data.SbFee);
                    $('#txt_sbfee').attr('title', data.ServiceBureaufeeTitle);

                    $('#txt_addonfee').attr('title', data.AddonfeeTitle);

                    $('#txt_electronicfee').val(data.ElectronicFee);
                    $('#chk_agreebank').prop('checked', data.AgreeTandC);
                    $('#txt_bankname').val(data.BankName);
                    $('#txt_accountname').val(data.AccountName);
                    $('#txt_mainCntFN').val(data.MainContactFirstName);
                    $('#txt_mainCntLN').val(data.MainContactLastName);
                    $('#txt_contactphone').val(data.MainContactPhone);
                    $('#chk_textmessages').prop('checked', data.TextMessages);
                    $('#chk_hasassociated').prop('checked', data.LegalIssues);
                    $('#ddl_incorporatestate').val(data.StateOfIncorporation);

                    EFINOwnerInfo = [];

                    $.each(data.RAEFINOwnerInfo, function (indx, valu) {
                        EFINOwnerInfo.push({
                            BankEnrollmentRAId: valu.BankEnrollmentRAId,
                            FirstName: valu.FirstName,
                            LastName: valu.LastName,
                            Email: valu.Email,
                            DateofBirth: valu.DateofBirth,
                            HomePhone: valu.HomePhone,
                            MobilePhone: valu.MobilePhone,
                            SSN: valu.SSN,
                            Address: valu.Address,
                            City: valu.City,
                            StateId: valu.StateId,
                            ZipCode: valu.ZipCode,
                            IDNumber: valu.IDNumber,
                            IDState: valu.IDState,
                            PercentageOwned: valu.PercentageOwned
                        });
                    });

                    ArchiveEFINOwnerInfo_table(EFINOwnerInfo);

                }
                //})
            }
            else if (bankcode == 'R') {
                //var Uri = '/api/EnrollmentBankSelection/GetRBBankEnrollment?CustomerId=' + CustId;
                //ajaxHelper(Uri, 'GET').done(function (data, status) {
                if (data) {
                    $('#txt_officename').val(data.OfficeName);
                    $('#txt_officeaddress').val(data.OfficePhysicalAddress);
                    $('#txt_officecity').val(data.OfficePhysicalCity);
                    $('#ddl_officestate').val(data.OfficePhysicalState);
                    $('#txt_officezip').val(data.OfficePhysicalZip);
                    $('#txt_offcntFN').val(data.OfficeContactFirstName);
                    $('#txt_offcntLN').val(data.OfficeContactLastName);
                    $('#txt_offcntSSN').val(data.OfficeContactSSN);
                    $('#txt_officephone').val(data.OfficePhoneNumber);
                    $('#txt_cellphone').val(data.CellPhoneNumber);
                    $('#txt_faxnumber').val(data.FAXNumber);
                    $('#txt_email').val(data.EmailAddress);
                    $('#txt_offmngrFN').val(data.OfficeManagerFirstName);
                    $('#txt_offmngeLN').val(data.OfficeManageLastName);
                    $('#txt_offmngrSSN').val(data.OfficeManagerSSN);
                    if (data.OfficeManagerDOB)
                        $('#txt_offmngrDOB').datepicker('update', new Date(data.OfficeManagerDOB));
                    else
                        $('#txt_offmngrDOB').val('');


                    if (data.OfficeContactDOB)
                        $('#txt_offContactDOB').datepicker('update', new Date(data.OfficeContactDOB));
                    else
                        $('#txt_offContactDOB').val('');
                    $('#txt_offmngrPhoneNo').val(data.OfficeManagerPhone);
                    $('#txt_offmngeCellPhone').val(data.OfficeManagerCellNo);
                    $('#txt_offManagerEmail').val(data.OfficeManagerEmail);

                    $('#txt_offmngrSSN').val(data.OfficeManagerSSN);
                    $('#txt_altocFN1').val(data.AltOfficeContact1FirstName);
                    $('#txt_altocLN1').val(data.AltOfficeContact1LastName);
                    $('#txt_altoc1Email').val(data.AltOfficeContact1Email);
                    $('#txt_altoc1SSN').val(data.AltOfficeContact1SSn);
                    $('#txt_altocFN2').val(data.AltOfficeContact2FirstName);
                    $('#txt_altocLN2').val(data.AltOfficeContact2LastName);
                    $('#txt_altoc2Email').val(data.AltOfficeContact2Email);
                    $('#txt_altoc2SSN').val(data.AltOfficeContact2SSn);
                    $('#txt_altoffadd').val(data.AltOfficePhysicalAddress);
                    $('#txt_altoffadd2').val(data.AltOfficePhysicalAddress2);
                    $('#txt_altoffcity').val(data.AltOfficePhysicalCity);
                    $('#ddl_altofficestate').val(data.AltOfficePhysicalState);
                    $('#txt_altoffzip').val(data.AltOfficePhysicalZipcode);
                    $('#txt_mailaddress').val(data.MailingAddress);
                    $('#txt_mailcity').val(data.MailingCity);
                    $('#ddl_mailstate').val(data.MailingState);
                    $('#txt_mailzip').val(data.MailingZip);
                    $('#txt_fulladdress').val(data.FulfillmentShippingAddress);
                    $('#txt_fullcity').val(data.FulfillmentShippingCity);
                    $('#ddl_fullstate').val(data.FulfillmentShippingState);
                    $('#txt_fullzip').val(data.FulfillmentShippingZip);
                    $('#txt_website').val(data.WebsiteAddress);
                    $('#txt_yearsinbusiness').val(data.YearsinBusiness);
                    $('#txt_noofbankprdcts').val(data.NoofBankProductsLastYear);
                    $('#ddl_prevbank').val(data.PreviousBankProductFacilitator);
                    $('#txt_actnoofbankprdcts').val(data.ActualNoofBankProductsLastYear);
                    $('#txt_ownerFN').val(data.OwnerFirstName);
                    $('#txt_ownerLN').val(data.OwnerLastName);
                    $('#txt_ownerSSN').val(data.OwnerSSN);
                    if (data.OnwerDOB)
                        $('#txt_ownerDOB').datepicker('update', new Date(data.OnwerDOB));
                    else
                        $('#txt_ownerDOB').val('');
                    $('#txt_ownerphone').val(data.OwnerHomePhone);
                    $('#txt_owneraddress').val(data.OwnerAddress);
                    $('#txt_ownercity').val(data.OwnerCity);
                    $('#ddl_ownerstate').val(data.OwnerState);
                    $('#txt_ownerzip').val(data.OwnerZip);
                    $('#ddl_legalentity').val(data.LegarEntityStatus);
                    $('#ddl_llcmembership').val(data.LLCMembershipRegistration);
                    $('#txt_businessname').val(data.BusinessName);
                    $('#txt_businessEIN').val(data.BusinessEIN);
                    if (data.BusinessIncorporation)
                        $('#txt_businessdate').datepicker('update', new Date(data.BusinessIncorporation));
                    else
                        $('#txt_businessdate').val('');
                    $('#txt_efinownerFN').val(data.EFINOwnerFirstName);
                    $('#txt_efinownerLN').val(data.EFINOwnerLastName);
                    $('#txt_efinownerSSN').val(data.EFINOwnerSSN);
                    if (data.EFINOwnerDOB)
                        $('#txt_efinownerDOB').datepicker('update', new Date(data.EFINOwnerDOB));
                    else
                        $('#txt_efinownerDOB').val('');
                    $('#ddl_multioffice').val(data.IsMultiOffice);
                    $('#ddl_productsoffering').val(data.ProductsOffering);
                    $('#ddl_locationtransmit').val(data.IsOfficeTransmit);
                    $('#ddl_ptin').val(data.IsPTIN);
                    $('#ddl_processlaw').val(data.IsAsPerProcessLaw);
                    $('#ddl_complaince').val(data.IsAsPerComplainceLaw);
                    $('#ddl_consumerlending').val(data.ConsumerLending);
                    $('#txt_noofpersoneel').val(data.NoofPersoneel);
                    $('#ddl_advertiseapprvl').val(data.AdvertisingApproval);
                    $('#ddl_eroparticipation').val(data.EROParticipation);
                    $('#txt_spaamount').val(data.SPAAmount);
                    $('#txt_eroagreeddate').val(data.SPADate);
                    $('#ddl_retailmethod').val(data.RetailPricingMethod);
                    $('#ddl_lockeddocs').val(data.IsLockedStore_Documents);
                    $('#ddl_lockedcardschecks').val(data.IsLockedStore_Checks);
                    $('#ddl_lockedoffice').val(data.IsLocked_Office);
                    $('#ddl_limitaccess').val(data.IsLimitAccess);
                    $('#ddl_plandispose').val(data.PlantoDispose);
                    $('#ddl_logintoemployees').val(data.LoginAccesstoEmployees);
                    $('#ddl_antivirus').val(data.AntivirusRequired);
                    $('#ddl_firewall').val(data.HasFirewall);
                    $('#ddl_onlinetraining').val(data.OnlineTraining);
                    $('#ddl_pwdrqd').val(data.PasswordRequired);
                    if (data.EROApplicattionDate)
                        $('#txt_eroapplncmptd').val(data.EROApplicattionDate);
                    else
                        $('#txt_eroapplncmptd').val();
                    $('#ddl_eroreadrc').val(data.EROReadTAndC);
                    $('#txt_accountname').val(data.CheckingAccountName);
                    $('#txt_bankroutingno').val(data.BankRoutingNumber);
                    $('#txt_confirmbankroutingno').val(data.BankRoutingNumber);
                    $('#txt_bankaccountno').val(data.BankAccountNumber);
                    $('#txt_confirmbankaccountno').val(data.BankAccountNumber);
                    $('#ddl_accounttype').val(data.BankAccountType);
                    $('#txt_efintitle').val(data.EFINOwnerTitle);
                    $('#txt_efinownerAddress').val(data.EFINOwnerAddress);
                    $('#txt_efinownercity').val(data.EFINOwnerCity);
                    $('#ddl_efinownerstate').val(data.EFINOwnerState);
                    $('#txt_efinownerZip').val(data.EFINOwnerZip);
                    $('#txt_efinownerphone').val(data.EFINOwnerPhone);
                    $('#txt_efinownermobile').val(data.EFINOwnerMobile);
                    $('#txt_efinowneremail').val(data.EFINOwnerEmail);
                    $('#txt_efinowneridnumber').val(data.EFINOwnerIDNumber);
                    $('#ddl_idstate').val(data.EFINOwnerIDState);
                    $('#txt_efinownerein').val(data.EFINOwnerEIN);
                    $('#ddl_wifipwd').val(data.SupportOS);
                    $('#txt_bankname').val(data.BankName);
                    if (data.SBFeeonAll == 'X')
                        $('#chk_fbfee').prop('checked', true);
                    $('#txt_sbfee').val(data.SBFee);
                    $('#txt_transmitfee').val(data.TransimissionAddon);

                    $('#txt_sbfee').attr('title', data.ServiceBureaufeeTitle);
                    $('#txt_transmitfee').attr('title', data.AddonfeeTitle);

                    $('#ddl_cardprogram').val(data.PrepaidCardProgram);
                    if (data.TandC == true)
                        $('#chk_returns').prop('checked', true);
                }
                // });
            }


            $('input').attr('disabled', 'disabled');
            $('textarea').attr('disabled', 'disabled');
            $('select').attr('disabled', 'disabled');
            $('select#SalesYearID').removeAttr('disabled');

        });
    } else {

        $('#dv_Enrollment').html('');

        $('#dv_Enrollment')
               .html('No Record Found');
    }
}

function getLegalStatus(type) {

    switch (type) {
        case 'C':
            return 'Corporation';
        case 'P':
            return 'Partnership';
        case 'L':
            return 'Limited Liability Corporation (LLC)';
        case 'T':
            return 'Limited Liability Partnership';
        case 'S':
            return 'Sole Proprietorship';
        case 'W':
            return 'Personal Service Corporation';
        default:
            return type;
    }

}

function ArchiveEFINOwnerInfo_table(EFINOwnerInfo) {
    $("#tbl_EFINOwnerInfo > tbody").remove();
    var tbody = $('#tbl_EFINOwnerInfo').append('<tbody/>');
    for (var i = 0, len = EFINOwnerInfo.length; i < len; i++) {
        var row = $('<tr/>');
        row.append($("<td/>").html(EFINOwnerInfo[i].FirstName));
        row.append($("<td/>").html(EFINOwnerInfo[i].LastName));
        //row.append($("<td/>").html(EFINOwnerInfo[i].EmailId));
        row.append($("<td/>").html(EFINOwnerInfo[i].DateofBirth));
        //row.append($("<td/>").html(EFINOwnerInfo[i].MobilePhone));
        row.append($("<td/>").html(EFINOwnerInfo[i].SSN));
        row.append($("<td/>").html(EFINOwnerInfo[i].HomePhone));
        row.append($("<td/>").html(EFINOwnerInfo[i].Address));
        row.append($("<td/>").html(EFINOwnerInfo[i].City));
        row.append($("<td/>").html(EFINOwnerInfo[i].StateId));
        row.append($("<td/>").html(EFINOwnerInfo[i].ZipCode));
        row.append($("<td/>").html(EFINOwnerInfo[i].IDNumber));
        row.append($("<td/>").html(EFINOwnerInfo[i].IDState));
        row.append($("<td/>").html(EFINOwnerInfo[i].PercentageOwned));
        //row.append($("<td/>").html('<a class="submitlock" sttyle="cursor:pointer;font-size:20px;" onclick="removeOwner(' + i + ')"><i class="fa fa-times-circle" aria-hidden="true"></i></a>'));
        row.appendTo(tbody);

    }
}


//////Payment Option


function getArchiveeFilePaymentInfo(data) {

    if (data.status) {
        Id = data.Id;
        IsFeeReimbursement = data.IsFeeReimbursement;
        IsEnrollment = data.IsEnrollment;
        if (data.PaymentType == 1) {
            $('#rbefileCC0').prop('checked', true);
            $('#dbBankSubQuestion0').hide();
            $('#liefileCC0').show();
            $('#liefileBA0').hide();
            if (data.CreditCard) {
                if (data.CreditCard.status) {
                    $('#txtCardholderName0').val(data.CreditCard.CardHolderName);
                    $('#txtBillingAddress0').val(data.CreditCard.Address);
                    $('#txtCardNumber0').val(data.CreditCard.CardNumber);
                    $('#txtExpiration0').val(data.CreditCard.Expiration);
                    $('#txtZip0').val(data.CreditCard.ZipCode);
                    $("select[name=ddlstate0] option[value=" + data.CreditCard.StateId + "]").attr('selected', 'selected');
                    $('#txtCity0').val(data.CreditCard.City);
                    if (data.CreditCard.CardType == 1)
                        $('#rbmastercard0').prop('checked', true);
                    else if (data.CreditCard.CardType == 2)
                        $('#rbvisa0').prop('checked', true);
                    else if (data.CreditCard.CardType == 3)
                        $('#rbamericanexpress0').prop('checked', true);
                    $('#chkAughorizeCC0').prop('checked', true);
                }
            }

        }
        else if (data.PaymentType == 2) {
            $('#rbefileBA0').prop('checked', true);
            $('#liefileCC0').hide();
            $('#liefileBA0').show();
            if ($('#sitea55334d1-3960-44c4-8cf1-e3ba9901f2be').length > 0 || $('#site60025459-7568-4a77-b152-f81904aaaa63').length > 0 || IsFeeReimbursement)
                $('#dbBankSubQuestion0').show();
            if (data.IsSameBankAccount == 1)
                $('#rbBankProdYes0').prop('checked', true);
            else
                $('#rbBankProdNo0').prop('checked', true);

            if (data.ACH) {
                if (data.ACH.status) {
                    $('#txtNameofAccount0').val(data.ACH.AccountName);
                    $('#txtBankName0').val(data.ACH.BankName);
                    $('#txtRTN0, #txtConfirmRTN0').val(data.ACH.RTN);
                    $('#txtBankAccount0, #txtConfirmBankAccount0').val(data.ACH.AccountNumber);
                    if (data.ACH.AccountType == 1)
                        $('#rbAccountTypeYes0').prop('checked', true);
                    else if (data.ACH.AccountType == 2)
                        $('#rbAccountTypeNo0').prop('checked', true);
                    $('#chkAughorizeACH0').prop('checked', true);

                }
                //else if (data.IsSameBankAccount == 1) {
                //    var bankURI = '/api/CustomerPaymentOptions/GetCustomerBankDetails?UserId=' + UserId + '&EntityId=' + EntityId + '&CustId=' + UserId + '&BankId=' + bankid;
                //    ajaxHelper(bankURI, 'GET').done(function (data) {
                //        if (data)
                //            if (data.status) {
                //                $('#txtNameofAccount').val(data.AccountName);
                //                $('#txtBankName').val(data.BankName);
                //                $('#txtRTN, #txtConfirmRTN').val(data.RTN);
                //                $('#txtBankAccount, #txtConfirmBankAccount').val(data.AccountNumber);
                //                if (data.AccountType == 1)
                //                    $('#rbAccountTypeYes').prop('checked', true);
                //                else if (data.AccountType == 2)
                //                    $('#rbAccountTypeNo').prop('checked', true);
                //                else
                //                    $('input[name="AccountType"]').prop('checked', false);
                //            }

                //    });
                //}
            }
        }

        $('#tb_feeSummary0').html('');
        $.each(data.Fees, function (item, value) {

            $('#tb_feeSummary0').append('<tr><td>' + value.Fee + '</td><td><span>$</span> <span class="hdnTranFee">' + value.Amount + '</span></td></tr>');
        })

        bankDetails = data.BankDetails;

        // efilePaymentChanges(data.type);
    }
    else {
        $('#liefileCC0').hide();
        $('#liefileBA0').hide();
    }

    // });
}

function getArchiveOutStandingPaymentInfo(data) {

    if (data.status) {
        Id = data.Id;
        IsFeeReimbursement = data.IsFeeReimbursement;
        IsEnrollment = data.IsEnrollment;
        if (data.PaymentType == 1) {
            $('#rbefileCC1').prop('checked', true);
            $('#dbBankSubQuestion1').hide();
            $('#liefileCC1').show();
            $('#liefileBA1').hide();
            if (data.CreditCard) {
                if (data.CreditCard.status) {
                    $('#txtCardholderName1').val(data.CreditCard.CardHolderName);
                    $('#txtBillingAddress1').val(data.CreditCard.Address);
                    $('#txtCardNumber1').val(data.CreditCard.CardNumber);
                    $('#txtExpiration1').val(data.CreditCard.Expiration);
                    $('#txtZip1').val(data.CreditCard.ZipCode);
                    $("select[name=ddlstate1] option[value=" + data.CreditCard.StateId + "]").attr('selected', 'selected');
                    $('#txtCity1').val(data.CreditCard.City);
                    if (data.CreditCard.CardType == 1)
                        $('#rbmastercard1').prop('checked', true);
                    else if (data.CreditCard.CardType == 2)
                        $('#rbvisa1').prop('checked', true);
                    else if (data.CreditCard.CardType == 3)
                        $('#rbamericanexpress1').prop('checked', true);
                    $('#chkAughorizeCC1').prop('checked', true);
                }
            }

        }
        else if (data.PaymentType == 2) {
            $('#rbefileBA1').prop('checked', true);
            $('#liefileCC1').hide();
            $('#liefileBA1').show();

            if ($('#sitea55334d1-3960-44c4-8cf1-e3ba9901f2be').length > 0 || $('#site60025459-7568-4a77-b152-f81904aaaa63').length > 0 || IsFeeReimbursement)
                $('#dbBankSubQuestion1').show();
            if (data.IsSameBankAccount == 1)
                $('#rbBankProdYes1').prop('checked', true);
            else
                $('#rbBankProdNo1').prop('checked', true);

            if (data.ACH) {
                if (data.ACH.status) {
                    $('#txtNameofAccount1').val(data.ACH.AccountName);
                    $('#txtBankName1').val(data.ACH.BankName);
                    $('#txtRTN1, #txtConfirmRTN1').val(data.ACH.RTN);
                    $('#txtBankAccount1, #txtConfirmBankAccount1').val(data.ACH.AccountNumber);
                    if (data.ACH.AccountType == 1)
                        $('#rbAccountTypeYes1').prop('checked', true);
                    else if (data.ACH.AccountType == 2)
                        $('#rbAccountTypeNo1').prop('checked', true);
                    $('#chkAughorizeACH1').prop('checked', true);
                }
                //else if (data.IsSameBankAccount == 1) {
                //    var bankURI = '/api/CustomerPaymentOptions/GetCustomerBankDetails?UserId=' + UserId + '&EntityId=' + EntityId + '&CustId=' + UserId + '&BankId=' + bankid;
                //    ajaxHelper(bankURI, 'GET').done(function (data) {
                //        if (data)
                //            if (data.status) {
                //                $('#txtNameofAccount').val(data.AccountName);
                //                $('#txtBankName').val(data.BankName);
                //                $('#txtRTN, #txtConfirmRTN').val(data.RTN);
                //                $('#txtBankAccount, #txtConfirmBankAccount').val(data.AccountNumber);
                //                if (data.AccountType == 1)
                //                    $('#rbAccountTypeYes').prop('checked', true);
                //                else if (data.AccountType == 2)
                //                    $('#rbAccountTypeNo').prop('checked', true);
                //                else
                //                    $('input[name="AccountType"]').prop('checked', false);
                //            }

                //    });
                //}
            }
        }

        $('#tb_feeSummary1').html('');
        $.each(data.Fees, function (item, value) {

            $('#tb_feeSummary1').append('<tr><td>' + value.Fee + '</td><td><span>$</span> <span class="hdnTranFee">' + value.Amount + '</span></td></tr>');
        })

        bankDetails = data.BankDetails;

        // efilePaymentChanges(data.type);
    }
    else {
        $('#liefileCC1').hide();
        $('#liefileBA1').hide();
    }


    // });
}

function efilePaymentChanges(type) {
    if (type == 'cc')
        $('#dbBankSubQuestion').hide();
    else {
        if ($('#sitea55334d1-3960-44c4-8cf1-e3ba9901f2be').length > 0 || $('#site60025459-7568-4a77-b152-f81904aaaa63').length > 0 || IsFeeReimbursement)
            $('#dbBankSubQuestion').show();
    }
}


///Bank Status

function getCustomerBanks(data) {
    //var CustomerId = $('#UserId').val();
    //var EntityId = $('#entityid').val();
    //if ($('#entityid').val() != $('#myentityid').val()) {
    //    CustomerId = $('#myid').val();
    //    EntityId = $('#myentityid').val();
    //}

    //var _showunlock = false;
    //var bankUri = '/api/BankSelection/getCutomerBanks?CustomerId=' + CustomerId + '&EntityId=' + EntityId;
    //ajaxHelper(bankUri, 'GET').done(function (data) {

    if (data) {
        if (data.Status) {
            $('#tbd_banks').empty();
            var trdata = '';

            $.each(data.Banks, function (item, value) {
                trdata += '<tr><td>' + value.BankName + '</td><td>' + value.Submission + '</td><td>' + value.Acceptance + '</td><td>' + value.BankStatus + '</td></tr>';
            })

            $('#tbd_banks').html(trdata);
        } else {
            $('#lienrollbankstatus').hide();
            $('#dvenrollbankstatus').hide();
        }
    }
}

/////
function getArchivedCustomerInfo(data) {
    if (data) {
        $('#IsVerified').val(data["IsVerified"]);
        $('#spCompanyName').text(data["CompanyName"]);
        $('#spBusinessOwnerName').text(data["BusinessOwnerFirstName"]);
        $('#spBusinessOwnerLastName').text(data["BusinessOwnerLastName"]);
        $('#spPhysicalAddress').text(data["PhysicalAddress1"]);
        $('#spCitystatezip').text(data["PhysicalCity"] + ', ' + data["PhysicalState"] + ' ' + data["PhysicalZipcode"]);
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

        $('#StatusCode').val(data["StatusCode"]);
        $('#ActivationParentId').val(data["ParentId"]);
        $('#spEFIN').html(data["EFINStatusText"]);


        //Enroll
        $('#IsVerified1').val(data["IsVerified"]);
        $('#spCompanyName1').text(data["CompanyName"]);
        $('#spBusinessOwnerName1').text(data["BusinessOwnerFirstName"]);
        $('#spBusinessOwnerLastName1').text(data["BusinessOwnerLastName"]);
        $('#spPhysicalAddress1').text(data["PhysicalAddress1"]);
        $('#spCitystatezip1').text(data["PhysicalCity"] + ', ' + data["PhysicalState"] + ' ' + data["PhysicalZipcode"]);
        $('#spOfficePhone1').text(data["OfficePhone"]);
        $('#spAlternatePhone1').text(data["AlternatePhone"]);
        $('#spPrimaryemail1').text(data["Primaryemail"]);
        $('#spSupportemail1').text(data["SupportNotificationemail"]);
        $('#spAlternativeContact1').text(data["AlternativeContact"]);
        $('#ismsouser1').val(data["IsMSOUser"]);

        if (data["PhoneType"] == null)
            $('#spphoneType1').text('');
        else
            $('#spphoneType1').text(data["PhoneType"]);

        $('#spContactType1').text(data["ContactTitle"]);

        $('#StatusCode1').val(data["StatusCode"]);
        $('#ActivationParentId1').val(data["ParentId"]);
        $('#spEFIN1').html(data["EFINStatusText"]);
    }
}

function getArchivedLoginInfo(data) {
    if (data) {
        $('#spMasterident').text(data["MasterIdentifier"]); // Master ID
        $('#spMasterUserID').text(data["CrossLinkUserId"]); // User ID
        $('#spTransmissionpwd').text(data["CrossLinkPassword"]); // Transmission Password
        $('#spofficeportalusername').text(data["TaxOfficeUsername"]);
        $('#spoofficeportalpwd').text(data["TaxOfficePassword"]);

        //Enroll
        $('#spMasterident1').text(data["MasterIdentifier"]); // Master ID
        $('#spMasterUserID1').text(data["CrossLinkUserId"]); // User ID
        $('#spTransmissionpwd1').text(data["CrossLinkPassword"]); // Transmission Password
        $('#spofficeportalusername1').text(data["TaxOfficeUsername"]);
        $('#spoofficeportalpwd1').text(data["TaxOfficePassword"]);
    }
}

function getArchivedParentInfo(data) {
    if (data.Id != '' && data.Id != null && data.Id != '00000000-0000-0000-0000-000000000000') {
        $('#divParentAccountPanel').show();
        $('#p_spEFIN').html(data["EFINStatusText"]);
        $('#p_spCompanyName').text(data["CompanyName"]);
        $('#p_spOwnerName').text(data["BusinessOwnerFirstName"]);
        $('#p_spOwnerLastName').text(data["BusinessOwnerLastName"]);
        $('#p_spPhysicalAddress').text(data["PhysicalAddress1"]);
        $('#p_spCityStateZip').text(data["PhysicalCity"] + ' ' + data["PhysicalState"] + ', ' + data["PhysicalZipcode"]);

        //Enroll
        $('#divParentAccountPanel1').show();
        $('#p_spEFIN1').html(data["EFINStatusText"]);
        $('#p_spCompanyName1').text(data["CompanyName"]);
        $('#p_spOwnerName1').text(data["BusinessOwnerFirstName"]);
        $('#p_spOwnerLastName1').text(data["BusinessOwnerLastName"]);
        $('#p_spPhysicalAddress1').text(data["PhysicalAddress1"]);
        $('#p_spCityStateZip1').text(data["PhysicalCity"] + ' ' + data["PhysicalState"] + ', ' + data["PhysicalZipcode"]);
    } else {
        $('#divParentAccountPanel').hide();
        $('#divParentAccountPanel1').hide();
    }
}

function getArchivedEnrollmentStatus(data) {
    //  var uri = '/api/EnrollmentBankSelection/getEnrollmentStatusInfo?CustomerId=' + Id;//+ '&bankid=' + bankid;
    //ajaxHelper(uri, 'GET').done(function (data) {
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
    //})
}
