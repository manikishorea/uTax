var success = $('#success');
var error = $('#error');
ResetSuccessError();
var banktpye = '';
var isDataAvailable = true;

function getBankSelected() {
    var CustId = $('#UserId').val();
    var Cid = getUrlVars()["Id"];
    if (Cid) {
        CustId = Cid;
    }
    //$.blockUI({ message: 'getting details...' });
    var bankid = getUrlVars()["bankid"];
    var Uri = '/api/EnrollmentBankSelection/GetBankSelectedByCustomer?CustomerId=' + CustId + '&bankid=' + bankid;
    ajaxHelper(Uri, 'GET', null, false).done(function (data, status) {
        if (data == 'S') {
            banktpye = 'S';
            $.ajax({
                type: 'POST',
                url: '/Enrollment/BankObjectsTPG',
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
                url: '/Enrollment/BankObjectsRB',
                data: {},
                async: false,
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
                url: '/Enrollment/BankObjectsRA',
                data: {},
                async: false,
                success: function (res) {
                    $('#dv_Enrollment').html('');
                    $('#dv_Enrollment').append(res);
                }
            })
        }
    });
    if (banktpye == 'S') {
        var containers = [];
        containers.push($('#ddl_officestate'));
        containers.push($('#ddl_shipstate'));
        containers.push($('#ddl_ownerstate'));
        containers.push($('#ddl_owneridstate'));
        getStateMasterCodeMultiple(containers);

        $('#ddl_officestate').val(0);
        $('#ddl_shipstate').val(0);
        $('#ddl_ownerstate').val(0);
        $('#ddl_BankLY').val(0);
        $('#ddl_accttype').val(0);
        $('#ddl_owneridstate').val(0);

        $('#txt_OfficeTel').mask("999-999-9999");
        $('#txt_OwnerTel').mask("999-999-9999");

        //$('#txt_OwnerDOB').datepicker({
        //    format: "mm/dd/yyyy",
        //    endDate: d
        //});
    }
    else if (banktpye == 'V') {
        var containers = [];
        containers.push($('#ddl_officestate'));
        containers.push($('#ddl_mailstate'));
        containers.push($('#ddl_ownerstate'));
        containers.push($('#ddl_issuestate'));
        containers.push($('#ddl_shipstate'));
        containers.push($('#ddl_IRSstate'));
        containers.push($('#ddl_modalstate'));
        containers.push($('#ddl_modalidstate'));
        containers.push($('#ddl_incorporatestate'));
        getStateMasterCodeMultiple(containers);

        $('#ddl_officestate').val(0);
        $('#ddl_mailstate').val(0);
        $('#ddl_ownerstate').val(0);
        $('#ddl_issuestate').val(0);
        $('#ddl_shipstate').val(0);
        $('#ddl_IRSstate').val(0);
        $('#ddl_corporationtype').val(0);
        $('#ddl_hasassociated').val(0);
        $('#ddl_prevclient').val(0);
        $('#ddl_bankaccounttype').val(0);
        $('#ddl_modalstate').val(0);
        $('#ddl_modalidstate').val(0);
        $('#ddl_incorporatestate').val(0);

        //$('#txt_ownerDOB, #txt_modalDOB').datepicker({
        //    format: "mm/dd/yyyy",
        //    endDate: d
        //});

        $('#txt_ownercell, #txt_contactphone, #txt_modalphone').mask("999-999-9999");
        $('#txt_ownerphone, #txt_officephone').mask("999-999-9999");
    }
    else if (banktpye == 'R') {
        var containers = [];
        containers.push($('#ddl_officestate'));
        containers.push($('#ddl_altofficestate'));
        containers.push($('#ddl_mailstate'));
        containers.push($('#ddl_fullstate'));
        containers.push($('#ddl_ownerstate'));
        containers.push($('#ddl_efinownerstate'));
        containers.push($('#ddl_idstate'));
        getStateMasterCodeMultiple(containers);
        $('select').val(0);
        //$('#txt_offmngrDOB, #txt_ownerDOB, #txt_businessdate, #txt_efinownerDOB, #txt_offContactDOB').datepicker({
        //    format: "mm/dd/yyyy",
        //    endDate: d
        //})
        $('#txt_eroagreeddate, #txt_eroapplncmptd').datetimepicker();
        $('#txt_officephone, #txt_cellphone, #txt_ownerphone, #txt_efinownerphone, #txt_efinownermobile, #txt_offmngeCellPhone').mask("999-999-9999");
    }
    
    getBankEnrollmentData();

    //$.unblockUI();
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

function getBankEnrollmentData() {
    var CustId = $('#UserId').val(); //c
    var Cid = getUrlVars()["Id"];
    if (Cid) {
        CustId = Cid;
    }
    setTimeout(function () {
        $.blockUI({ message: '<img src="../content/images/loading-img.gif"/>' });
        if (banktpye == 'S') {
            var Uri = '/api/EnrollmentBankSelection/GetTPGBankObjects?CustomerId=' + CustId;
            ajaxHelper(Uri, 'GET').done(function (data, status) {
                if (data) {
                    $('#spnBankStatus').text(data.BankStatus);
                    //Tab  - 1
                    $('#spCompanyName').text(data.CompanyName);
                    $('#spManagFirstName').text(data.ManagerFirstName);
                    $('#spManagLastName').text(data.ManagerLastName);
                    $('#spManageEmailAddress').text(data.ManagerEmail);
                    $('#spOfficeAddress').text(data.OfficeAddress);
                    $('#spOfficeCityStateZip').text(data.OfficeCity + ', ' + data.OfficeState + ',' + data.OfficeZip);
                    $('#spOfficeTelephone').text(data.OfficeTelephone);
                    $('#spOfficeFax').text(data.OfficeFax);
                    $('#spShippingAddress').text(data.ShippingAddress);
                    $('#spShippingCityStateZip').text(data.ShippingCity + ', ' + data.ShippingState + ',' + data.ShippingZip);
                    //Tab  - 2
                    $('#spEFINTitle').text(data.EfinOwnerTitle);
                    $('#spEFINFN').text(data.OwnerFirstName);
                    $('#spEFINLN').text(data.OwnerLastName);
                    $('#spEFINEIN').text(data.OwnerEIN);
                    $('#spEFINSSN').text(data.OwnerSSn);
                    $('#spEFINIDNo').text(data.EfinIDNumber);
                    $('#spEFINTeleNo').text(data.OwnerTelephone);
                    $('#spEFINIDState').text(data.EfinIdState);
                    $('#spEFINOwnersAdd').text(data.OwnerAddress);
                    $('#spEFINCityStateZip').text(data.OwnerCity + ',' + data.OwnerState + ',' + data.OwnerZip);
                    $('#SPEFINDOB').text(data.OwnerDOB);
                    $('#spEFINEmailAddress').text(data.OwnerEmail);
                    //Tab  - 3
                    $('#chk_feeall').text(data.SbfeeAll);
                    $('#spTransmission').text(data.Addonfee);
                    $('#spSVBFee').text(data.ServiceBureaufee);
                    $('#spDocumentPreprationFee').text(data.Addonfee);
                    $('#spDocumentPreprationFee').text(data.DocPrepFee);
                    //Tab  - 4
                    $('#spBankUsedLastYear').text(data.LastYearBank);
                    $('#spPriorYearEFIN').text(data.LastYearEFIN);
                    $('#spPriorYear').text(data.LastYearVolume);
                    $('#spPrioryearBank').text(data.BankProductFund);
                    //Tab  - 5
                    $('#spBankname').text(data.BankName);
                    $('#spAccountName').text(data.AccountName);
                    $('#spOfficeAcct').text(data.AccountType);
                    $('#spOfficeRTN').text(data.OfficeRTN);
                    $('#spOfficeDAN').text(data.OfficeDAN);

                    for (var key in data.LatestAppRawXml) {
                        $('#tbl_app').append('<tr><td>' + key + '</td><td>' + data.LatestAppRawXml[key] + '</td></tr>');
                    }
                    for (var key in data.TPGRawXml) {
                        $('#tbl_bank').append('<tr><td>' + key + '</td><td>' + data.TPGRawXml[key] + '</td></tr>');
                    }
                    for (var key in data.EfinRawXml) {
                        $('#tbl_efin').append('<tr><td>' + key + '</td><td>' + data.EfinRawXml[key] + '</td></tr>');
                    }

                    //if (data.AgreeBank) {
                    //    $('#chk_agreebank').attr('disabled', 'disabled');
                    //}
                }
                else
                    isDataAvailable = false;
                $.unblockUI();
            })
        }
        else if (banktpye == 'V') {
            var Uri = '/api/EnrollmentBankSelection/getRABankObjectInfo?CustomerId=' + CustId;
            ajaxHelper(Uri, 'GET').done(function (data, status) {
                if (data) {
                    $('#spnBankStatus').text(data.BankStatus);
                    //Tab - 1
                    $('#spEFINOwnerTitle').text(data.OwnerTitle);
                    $('#spEFINFN').text(data.OwnerFirstName);
                    $('#spEFINLN').text(data.OwnerLastName);
                    $('#spEFINSSN').text(data.OwnerSSN);
                    $('#spEFINDOB').text(data.OwnerDOB);
                    $('#spEFINEmailID').text(data.OwnerEmail);
                    $('#spEFINMobile').text(data.OwnerCellPhone);
                    $('#spEFINPhone').text(data.OwnerHomePhone);
                    $('#spEFINOwnerIDNo').text(data.OwnerStateIssuedIdNumber);
                    $('#spEFINIDStateabbrev').text(data.OwnerIssuingState);
                    $('#spEFINIssuedtoAdd').text(data.OwnerAddress);
                    $('#spEFICityStateZip').text(data.OwnerCity + ',' + data.OwnerState + ',' + data.OwnerZipCode);

                    //Tab - 2
                    $('#spEROCompany').text(data.EROOfficeName);
                    $('#spMainFN').text(data.MainContactFirstName);
                    $('#spMainLN').text(data.MainContactLastName);
                    $('#SPMainPhone').text(data.MainContactPhone);
                    $('#spOfffocePhone').text(data.EROOfficePhone);
                    $('#spOfficeStAdd').text(data.EROOfficeAddress);
                    $('#spOfficeCityStateZip').text(data.EROOfficeCity + ',' + data.EROOfficeState + ',' + data.EROOfficeZipCoce);
                    $('#spMailingadd').text(data.EROMaillingAddress);
                    $('#spMailCityStateZip').text(data.EROMailingCity + ',' + data.EROMailingState + ',' + data.EROMailingZipcode);
                    $('#spShippingAddress').text(data.EROShippingAddress);
                    $('#spShippingCityStateZip').text(data.EROShippingCity + ',' + data.EROShippingState + ',' + data.EROShippingZip);
                    $('#spBusineeEIN').text(data.BusinessEIN);
                    $('#spCorportBusiness').text(data.CorporationType);
                    $('#spStateofIncorp').text(data.StateOfIncorporation);
                    $('#spPreviousYear').text(data.PreviousYearVolume);
                    $('#spPreviousYearBN').text(data.PreviousBankName);
                    $('#spExpectedCurrent').text(data.ExpectedCurrentYearVolume);
                    // $('#spPreviousYear').text(data.ExpectedCurrentYearVolume); Pending from DB ddl 
                    $('#spYearEfiling').text(data.NoofYearsExperience);
                    $('#chk_textmessages').text(data.TextMessages);
                    $('#chk_hasassociated').text(data.LegalIssues);

                    //Tab - 3 
                    $('#chk_feeall').text(data.SbFeeall);
                    $('#spTransmission').text(data.TransmissionAddon);
                    $('#spServiceBureau').text(data.SbFee);
                    $('#spElectronic').text(data.ElectronicFee);
                    $('#spBankName').text(data.BankName);
                    $('#spAccountName').text(data.AccountName);
                    $('#spBankAccount').text(data.BankAccountType);
                    $('#spBankrouting').text(data.BankRoutingNumber);
                    $('#spBankAccountNumber').text(data.BankAccountNumber);

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
                    fnEFINOwnerInfo_table(EFINOwnerInfo);


                    for (var key in data.LatestAppRawXml) {
                        $('#tbl_app').append('<tr><td>' + key + '</td><td>' + data.LatestAppRawXml[key] + '</td></tr>');
                    }
                    for (var key in data.RARawXml) {
                        $('#tbl_bank').append('<tr><td>' + key + '</td><td>' + data.RARawXml[key] + '</td></tr>');
                    }
                    for (var key in data.EfinRawXml) {
                        $('#tbl_efin').append('<tr><td>' + key + '</td><td>' + data.EfinRawXml[key] + '</td></tr>');
                    }
                }
                else
                    isDataAvailable = false;
                $.unblockUI();
            })
        }
        else if (banktpye == 'R') {
            var Uri = '/api/EnrollmentBankSelection/getRBBankObjectInfo?CustomerId=' + CustId;
            ajaxHelper(Uri, 'GET').done(function (data, status) {
                if (data) {
                    $('#spnBankStatus').text(data.BankStatus);
                    $('#txt_officename').text(data.OfficeName);
                    $('#txt_officeaddress').text(data.OfficePhysicalAddress);
                    $('#txt_officecity').text(data.OfficePhysicalCity);
                    $('#ddl_officestate').text(data.OfficePhysicalState);
                    $('#txt_officezip').text(data.OfficePhysicalZip);
                    $('#txt_offcntFN').text(data.OfficeContactFirstName);
                    $('#txt_offcntLN').text(data.OfficeContactLastName);
                    $('#txt_offcntSSN').text(data.OfficeContactSSN);
                    $('#txt_officephone').text(data.OfficePhoneNumber);
                    $('#txt_cellphone').text(data.CellPhoneNumber);
                    $('#txt_email').text(data.EmailAddress);
                    $('#txt_offmngrFN').text(data.OfficeManagerFirstName);
                    $('#txt_offmngeLN').text(data.OfficeManageLastName);
                    $('#txt_offmngrSSN').text(data.OfficeManagerSSN);
                    $('#txt_offmngrDOB').text(data.OfficeManagerDOB);
                    $('#txt_offContactDOB').text(data.OfficeContactDOB ? data.OfficeContactDOB : '');
                    $('#txt_offmngrPhoneNo').text(data.OfficeManagerPhone);
                    $('#txt_offmngeCellPhone').text(data.OfficeManagerCellNo);
                    $('#txt_offManagerEmail').text(data.OfficeManagerEmail);
                    $('#txt_offmngrSSN').text(data.OfficeManagerSSN);
                    $('#txt_mailaddress').text(data.MailingAddress);
                    $('#txt_mailcity').text(data.MailingCity);
                    $('#ddl_mailstate').text(data.MailingState);
                    $('#txt_mailzip').text(data.MailingZip);
                    $('#txt_fulladdress').text(data.FulfillmentShippingAddress);
                    $('#txt_fullcity').text(data.FulfillmentShippingCity);
                    $('#ddl_fullstate').text(data.FulfillmentShippingState);
                    $('#txt_fullzip').text(data.FulfillmentShippingZip);
                    $('#txt_website').text(data.WebsiteAddress ? data.WebsiteAddress : '');
                    $('#txt_yearsinbusiness').text(data.YearsinBusiness);
                    $('#txt_noofbankprdcts').text(data.NoofBankProductsLastYear);
                    $('#ddl_prevbank').text(data.PreviousBankProductFacilitator);
                    $('#txt_actnoofbankprdcts').text(data.ActualNoofBankProductsLastYear);
                    $('#txt_ownerFN').text(data.OwnerFirstName);
                    $('#txt_ownerLN').text(data.OwnerLastName);
                    $('#txt_ownerSSN').text(data.OwnerSSN);
                    $('#txt_ownerDOB').text(data.OnwerDOB);
                    $('#txt_ownerphone').text(data.OwnerHomePhone);
                    $('#txt_owneraddress').text(data.OwnerAddress);
                    $('#txt_ownercity').text(data.OwnerCity);
                    $('#ddl_ownerstate').text(data.OwnerState);
                    $('#txt_ownerzip').text(data.OwnerZip);
                    $('#ddl_legalentity').text(data.LegarEntityStatus);
                    $('#ddl_llcmembership').text(data.LLCMembershipRegistration);
                    $('#txt_businessEIN').text(data.BusinessEIN);
                    $('#txt_efinownerFN').text(data.EFINOwnerFirstName);
                    $('#txt_efinownerLN').text(data.EFINOwnerLastName);
                    $('#txt_efinownerSSN').text(data.EFINOwnerSSN);
                    $('#txt_efinownerDOB').text(data.EFINOwnerDOB);
                    $('#ddl_multioffice').text(data.IsMultiOffice);
                    $('#ddl_productsoffering').text(data.ProductsOffering);
                    $('#ddl_ptin').text(data.IsPTIN);
                    $('#ddl_processlaw').text(data.IsAsPerProcessLaw);
                    $('#ddl_complaince').text(data.IsAsPerComplainceLaw);
                    $('#ddl_consumerlending').text(data.ConsumerLending);
                    $('#txt_noofpersoneel').text(data.NoofPersoneel);
                    $('#ddl_advertiseapprvl').text(data.AdvertisingApproval);
                    $('#ddl_lockeddocs').text(data.IsLockedStore_Documents);
                    $('#ddl_lockedcardschecks').text(data.IsLockedStore_Checks);
                    $('#ddl_lockedoffice').text(data.IsLocked_Office);
                    $('#ddl_limitaccess').text(data.IsLimitAccess);
                    $('#ddl_plandispose').text(data.PlantoDispose);
                    $('#ddl_logintoemployees').text(data.LoginAccesstoEmployees);
                    $('#ddl_antivirus').text(data.AntivirusRequired);
                    $('#ddl_firewall').text(data.HasFirewall);
                    $('#ddl_onlinetraining').text(data.OnlineTraining);
                    $('#ddl_pwdrqd').text(data.PasswordRequired);
                    $('#txt_accountname').text(data.CheckingAccountName);
                    $('#txt_bankroutingno').text(data.BankRoutingNumber);
                    $('#txt_confirmbankroutingno').text(data.BankRoutingNumber);
                    $('#txt_bankaccountno').text(data.BankAccountNumber);
                    $('#txt_confirmbankaccountno').text(data.BankAccountNumber);
                    $('#ddl_accounttype').text(data.BankAccountType);
                    $('#txt_efintitle').text(data.EFINOwnerTitle);
                    $('#txt_efinownerAddress').text(data.EFINOwnerAddress);
                    $('#txt_efinownercity').text(data.EFINOwnerCity);
                    $('#ddl_efinownerstate').text(data.EFINOwnerState);
                    $('#txt_efinownerZip').text(data.EFINOwnerZip);
                    $('#txt_efinownerphone').text(data.EFINOwnerPhone);
                    $('#txt_efinownermobile').text(data.EFINOwnerMobile ? data.EFINOwnerMobile : '');
                    $('#txt_efinowneremail').text(data.EFINOwnerEmail);
                    $('#txt_efinowneridnumber').text(data.EFINOwnerIDNumber);
                    $('#ddl_idstate').text(data.EFINOwnerIDState);
                    $('#txt_efinownerein').text(data.EFINOwnerEIN);
                    $('#ddl_wifipwd').text(data.SupportOS);
                    $('#txt_bankname').text(data.BankName);
                    $('#chk_fbfee').text(data.SBFeeonAll);
                    $('#txt_sbfee').text(data.SBFee);
                    $('#txt_transmitfee').text(data.TransimissionAddon);
                    $('#txt_sbfee').attr('title', data.ServiceBureaufeeTitle);
                    $('#txt_transmitfee').attr('title', data.AddonfeeTitle);
                    $('#ddl_cardprogram').text(data.PrepaidCardProgram);


                    for (var key in data.LatestAppRawXml) {
                        $('#tbl_app').append('<tr><td>' + key + '</td><td>' + data.LatestAppRawXml[key] + '</td></tr>');
                    }
                    for (var key in data.RBRawXml) {
                        $('#tbl_bank').append('<tr><td>' + key + '</td><td>' + data.RBRawXml[key] + '</td></tr>');
                    }
                    for (var key in data.EfinRawXml) {
                        $('#tbl_efin').append('<tr><td>' + key + '</td><td>' + data.EfinRawXml[key] + '</td></tr>');
                    }
                }
                else
                    isDataAvailable = false;
                $.unblockUI();
            })
        }

        //getBankEnrollmentStatus(CustId);
    })
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

function fnEFINOwnerInfo_table(EFINOwnerInfo) {
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

function AcceptBank() {
    ResetSuccessError();
    var confRes = confirm('Are you sure want to approve the Bank Enrollment ?');
    if (confRes) {
        var custId = getUrlVars()["Id"];
        var bankid = getUrlVars()["bankid"];
        var UserId = $('#LoginId').val();
        if (custId) {
            var Uri = '/api/EnrollmentBankSelection/AcceptBankEnrollment?CustomerId=' + custId + '&UserId=' + UserId + '&bankid=' + bankid;
            ajaxHelper(Uri, 'POST').done(function (data, status) {
                if (data) {
                    UpdateOfficeManagement(request.CustomerId);
                    success.html('<p>Bank Enrollment has been approved successfully.</p>');
                    success.show();
                    $('#spnBankStatus').text('Accepted');
                    $('#dv_accrej').hide();
                }
            })
        }
    }
}

function RejectBank() {
    var confRes = confirm('Are you sure want to reject the Bank Enrollment ?');
    if (confRes) {
        var custId = getUrlVars()["Id"];
        var UserId = $('#LoginId').val();
        var bankid = getUrlVars()["bankid"];
        if (custId) {
            var Uri = '/api/EnrollmentBankSelection/RejectBankEnrollment?CustomerId=' + custId + '&UserId=' + UserId + '&bankid=' + bankid;
            ajaxHelper(Uri, 'POST').done(function (data, status) {
                if (data) {
                    UpdateOfficeManagement(request.CustomerId);
                    success.html('<p>Bank Enrollment has been rejected successfully.</p>');
                    success.show();
                    $('#spnBankStatus').text('Rejected');
                    $('#dv_accrej').hide();
                }
            })
        }
    }
}

function ResetSuccessError() {
    success.html('');
    success.hide();
    error.html('');
    error.hide();
}