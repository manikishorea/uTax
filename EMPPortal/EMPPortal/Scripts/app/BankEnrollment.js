var success = $('#success');
var error = $('#error');
ResetSuccessError();
var banktpye = '';
var bankstatus = '';
var alphaArray = ['a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z'];
var numberArray = ['1', '2', '3', '4', '5', '6', '7', '8', '9', '0'];

function getBankSelected() {

    var CustId = $('#UserId').val();

    var Cid = getUrlVars()["Id"];
    if (Cid) {
        CustId = Cid;
    }

    var bankid = getUrlVars()["bankid"];

    if (bankid != null && bankid != '' && bankid != undefined) {

        $.blockUI({ message: 'getting details...' });

        //var Uri = '/api/EnrollmentBankSelection/GetBankSelectedByCustomer?CustomerId=' + CustId + '&bankid=' + bankid;
        //ajaxHelper(Uri, 'GET', null, false).done(function (data, status) {

        if (bankid.toUpperCase() == 'A29B3547-8954-4036-9BD3-312F1D6A3DAA') {
            banktpye = 'S';
            $.ajax({
                type: 'POST',
                url: '/Enrollment/BankEnrollmentTPG',
                async: false,
                data: {},
                success: function (res) {
                    $('#dv_Enrollment').html('');
                    $('#dv_Enrollment').append(res);
                }
            })
        }
        else if (bankid.toUpperCase() == '2FB91EF5-EFC4-4BAF-ABD7-EA05A8C100CC') {
            banktpye = 'R';
            $.ajax({
                type: 'POST',
                url: '/Enrollment/BankEnrollmentRB',
                async: false,
                data: {},
                success: function (res) {
                    $('#dv_Enrollment').html('');
                    $('#dv_Enrollment').append(res);
                }
            })
        }
        else if (bankid.toUpperCase() == '5B4C7D3A-CE74-43EE-B17E-4DD5DCFD919B') {
            banktpye = 'V';
            $.ajax({
                type: 'POST',
                url: '/Enrollment/BankEnrollmentRA',
                async: false,
                data: {},
                success: function (res) {
                    $('#dv_Enrollment').html('');
                    $('#dv_Enrollment').append(res);
                }
            })
        }
        // });

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
        getBankEnrollmentStatus(CustId, bankid);
        getTooltip();
        $.unblockUI();
    }
}

function getBankEnrollmentData() {
    var CustId = $('#UserId').val(); //c
    var Cid = getUrlVars()["Id"];
    if (Cid) {
        CustId = Cid;
    }

    var IsStaging = false;
    if (getUrlVars()["Staging"])
        IsStaging = true;

    var bankid = getUrlVars()["bankid"];

    setTimeout(function () {
        //$.blockUI({ message: '<img src="../content/images/loading-img.gif"/>' });
        if (banktpye == 'S') {
            var Uri = '/api/EnrollmentBankSelection/GetTPGBankEnrollment?CustomerId=' + CustId + '&IsStaging=' + IsStaging + '&bankid=' + bankid;
            ajaxHelper(Uri, 'GET').done(function (data, status) {
                if (data) {
                    $('#txt_CompanyName').val(removeSplCompany(data.CompanyName));
                    $('#txt_ManagerFN').val(removeSplName(data.ManagerFirstName));
                    $('#txt_ManagerLN').val(removeSplName(data.ManagerLastName));
                    $('#txt_OfficeAddress').val(removeSplAddress(data.OfficeAddress));
                    $('#txt_OfficeCity').val(removeSplCity(data.OfficeCity));
                    $('#ddl_officestate').val(data.OfficeState);
                    $('#txt_OfficeZip').val(data.OfficeZip);
                    $('#txt_OfficeTel').val(data.OfficeTelephone);
                    $('#txt_OfficeFax').val(data.OfficeFax);
                    $('#txt_ShipAddress').val(removeSplAddress(data.ShippingAddress));
                    $('#txt_ShipCity').val(removeSplCity(data.ShippingCity));
                    $('#ddl_shipstate').val(data.ShippingState);
                    $('#txt_ShipZip').val(data.ShippingZip);
                    $('#txt_ManagerEmail').val(data.ManagerEmail);
                    $('#txt_OwnerEin').val(data.OwnerEIN == 0 ? '' : data.OwnerEIN);
                    $('#txt_OwnerSSN').val(data.OwnerSSn);
                    $('#txt_OwnerFN').val(removeSplName(data.OwnerFirstName));
                    $('#txt_OwnerLN').val(removeSplName(data.OwnerLastName));
                    $('#txt_OwnerAddress').val(removeSplAddress(data.OwnerAddress));
                    $('#txt_OwnerCity').val(removeSplCity(data.OwnerCity));
                    $('#ddl_ownerstate').val(data.OwnerState);
                    $('#txt_OwnerZip').val(data.OwnerZip);
                    $('#txt_OwnerTel').val(data.OwnerTelephone);
                    $('#txt_OwnerDOB').val(data.OwnerDOB);
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
                    $('#txt_sbfee').val(data.ServiceBureaufee);

                    if ($('#entityid').val() == $('#Entity_uTax').val() || $('#entityid').val() == $('#Entity_SO').val() || $('#entityid').val() == $('#Entity_SOME').val() ||
                        $('#entityid').val() == $('#Entity_MO').val() || $('#entityid').val() == $('#Entity_SVB').val()) {
                        $('#lblTranFeeSummary').attr('title', data.AddonfeeTitle);
                        $('#txt_addonfee').attr('title', data.AddonfeeTitle);
                        $('#lblSVBFeeSummary').attr('title', data.ServiceBureaufeeTitle);
                        $('#txt_sbfee').attr('title', data.ServiceBureaufeeTitle);
                    }
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
                $.unblockUI();
            })
        }
        else if (banktpye == 'V') {
            var Uri = '/api/EnrollmentBankSelection/GetRABankEnrollment?CustomerId=' + CustId + '&IsStaging=' + IsStaging + '&bankid=' + bankid;
            ajaxHelper(Uri, 'GET').done(function (data, status) {
                if (data) {
                    
                    $('#txt_owneremail').val(data.OwnerEmail);
                    $('#txt_ownerFN').val(removeSplName(data.OwnerFirstName));
                    $('#txt_ownerLN').val(removeSplName(data.OwnerLastName));
                    $('#txt_ownerSSN').val(data.OwnerSSN);
                    $('#txt_ownerDOB').val(data.OwnerDOB);
                    $('#txt_ownercell').val(data.OwnerCellPhone);
                    $('#txt_ownerphone').val(data.OwnerHomePhone);
                    $('#txt_ownerAddress').val(removeSplAddress(data.OwnerAddress));
                    $('#txt_ownerCity').val(removeSplCity(data.OwnerCity));
                    $('#ddl_ownerstate').val(data.OwnerState);
                    $('#txt_ownerzip').val(data.OwnerZipCode);
                    $('#txt_ownerissuenumber').val(data.OwnerStateIssuedIdNumber);
                    $('#ddl_issuestate').val(data.OwnerIssuingState);
                    $('#txt_companyname').val(removeSplCompany(data.EROOfficeName));
                    $('#txt_officeaddress').val(removeSplAddress(data.EROOfficeAddress));
                    $('#txt_officecity').val(removeSplCity(data.EROOfficeCity));
                    $('#ddl_officestate').val(data.EROOfficeState);
                    $('#txt_officezip').val(data.EROOfficeZipCoce);
                    $('#txt_officephone').val(data.EROOfficePhone);
                    $('#txt_mailaddress').val(removeSplAddress(data.EROMaillingAddress));
                    $('#txt_mailcity').val(removeSplCity(data.EROMailingCity));
                    $('#ddl_mailstate').val(data.EROMailingState);
                    $('#txt_mailzip').val(data.EROMailingZipcode);
                    $('#txt_shipaddress').val(removeSplAddress(data.EROShippingAddress));
                    $('#txt_shipcity').val(removeSplCity(data.EROShippingCity));
                    $('#ddl_shipstate').val(data.EROShippingState);
                    $('#txt_shipzip').val(data.EROShippingZip);
                    $('#txt_IRSaddress').val(data.IRSAddress);
                    $('#txt_IRScity').val(data.IRSCity);
                    $('#ddl_IRSstate').val(data.IRSState);
                    $('#txt_IRSzip').val(data.IRSZipcode);
                    $('#txt_pyvolume').val(data.PreviousYearVolume == 0 ? '' : data.PreviousYearVolume);
                    $('#txt_cyvolume').val(data.ExpectedCurrentYearVolume == 0 ? '' : data.ExpectedCurrentYearVolume);
                    $('#txt_pybank').val(data.PreviousBankName);
                    $('#ddl_corporationtype').val(data.CorporationType);
                    $('#txt_businessowner').val(data.CollectionofBusinessOwners);
                    $('#txt_nonbusinessowner').val(data.CollectionOfOtherOwners);
                    $('#txt_noofyears').val(data.NoofYearsExperience == 0 ? '' : data.NoofYearsExperience);
                    $('#ddl_hasassociated').val(data.HasAssociatedWithVictims);
                    $('#txt_federalnumber').val(data.BusinessFederalIDNumber);
                    $('#txt_businessein').val(data.BusinessEIN == 0 ? '' : data.BusinessEIN);
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
                    if ($('#entityid').val() == $('#Entity_uTax').val() || $('#entityid').val() == $('#Entity_SO').val() || $('#entityid').val() == $('#Entity_SOME').val() ||
                        $('#entityid').val() == $('#Entity_MO').val() || $('#entityid').val() == $('#Entity_SVB').val()) {
                        $('#txt_sbfee').attr('title', data.ServiceBureaufeeTitle);

                        $('#txt_addonfee').attr('title', data.AddonfeeTitle);
                    }
                    $('#txt_electronicfee').val(data.ElectronicFee);
                    $('#chk_agreebank').prop('checked', data.AgreeTandC);
                    $('#txt_bankname').val(data.BankName);
                    $('#txt_accountname').val(data.AccountName);
                    $('#txt_mainCntFN').val(removeSplName(data.MainContactFirstName));
                    $('#txt_mainCntLN').val(removeSplName(data.MainContactLastName));
                    $('#txt_contactphone').val(data.MainContactPhone);
                    $('#chk_textmessages').prop('checked', data.TextMessages);
                    $('#chk_hasassociated').val('' + data.LegalIssues + '');
                    $('#ddl_incorporatestate').val(data.StateOfIncorporation);

                    EFINOwnerInfo = [];

                    if (data.RAEFINOwnerInfo)
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

                }
                $.unblockUI();
            })
        }
        else if (banktpye == 'R') {
            var Uri = '/api/EnrollmentBankSelection/GetRBBankEnrollment?CustomerId=' + CustId + '&IsStaging=' + IsStaging + '&bankid=' + bankid;
            ajaxHelper(Uri, 'GET').done(function (data, status) {
                if (data) {
                    $('#txt_officename').val(removeSplCompany(data.OfficeName));
                    $('#txt_officeaddress').val(removeSplAddress(data.OfficePhysicalAddress));
                    $('#txt_officecity').val(removeSplCity(data.OfficePhysicalCity));
                    $('#ddl_officestate').val(data.OfficePhysicalState);
                    $('#txt_officezip').val(data.OfficePhysicalZip);
                    $('#txt_offcntFN').val(removeSplName(data.OfficeContactFirstName));
                    $('#txt_offcntLN').val(removeSplName(data.OfficeContactLastName));
                    $('#txt_offcntSSN').val(data.OfficeContactSSN);
                    $('#txt_officephone').val(data.OfficePhoneNumber);
                    $('#txt_cellphone').val(data.CellPhoneNumber);
                    $('#txt_faxnumber').val(data.FAXNumber);
                    $('#txt_email').val(data.EmailAddress);
                    $('#txt_offmngrFN').val(removeSplName(data.OfficeManagerFirstName));
                    $('#txt_offmngeLN').val(removeSplName(data.OfficeManageLastName));
                    $('#txt_offmngrSSN').val(data.OfficeManagerSSN);
                    $('#txt_offmngrDOB').val(data.OfficeManagerDOB);

                    $('#txt_offContactDOB').val(data.OfficeContactDOB);
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
                    $('#txt_mailaddress').val(removeSplAddress(data.MailingAddress));
                    $('#txt_mailcity').val(removeSplCity(data.MailingCity));
                    $('#ddl_mailstate').val(data.MailingState);
                    $('#txt_mailzip').val(data.MailingZip);
                    $('#txt_fulladdress').val(removeSplAddress(data.FulfillmentShippingAddress));
                    $('#txt_fullcity').val(removeSplCity(data.FulfillmentShippingCity));
                    $('#ddl_fullstate').val(data.FulfillmentShippingState);
                    $('#txt_fullzip').val(data.FulfillmentShippingZip);
                    $('#txt_website').val(data.WebsiteAddress);
                    $('#txt_yearsinbusiness').val(data.YearsinBusiness == 0 ? '' : data.YearsinBusiness);
                    $('#txt_noofbankprdcts').val(data.NoofBankProductsLastYear == 0 ? '' : data.NoofBankProductsLastYear);
                    $('#ddl_prevbank').val(data.PreviousBankProductFacilitator);
                    $('#txt_actnoofbankprdcts').val(data.ActualNoofBankProductsLastYear);
                    $('#txt_ownerFN').val(removeSplName(data.OwnerFirstName));
                    $('#txt_ownerLN').val(removeSplName(data.OwnerLastName));
                    $('#txt_ownerSSN').val(data.OwnerSSN);
                    $('#txt_ownerDOB').val(data.OnwerDOB);
                    $('#txt_ownerphone').val(data.OwnerHomePhone);
                    $('#txt_owneraddress').val(removeSplAddress(data.OwnerAddress));
                    $('#txt_ownercity').val(removeSplCity(data.OwnerCity));
                    $('#ddl_ownerstate').val(data.OwnerState);
                    $('#txt_ownerzip').val(data.OwnerZip);
                    $('#ddl_legalentity').val(data.LegarEntityStatus);
                    $('#ddl_llcmembership').val(data.LLCMembershipRegistration);
                    $('#txt_businessname').val(data.BusinessName);
                    $('#txt_businessEIN').val(data.BusinessEIN);
                    $('#txt_businessdate').val(data.BusinessIncorporation);
                    $('#txt_efinownerFN').val(removeSplName(data.EFINOwnerFirstName));
                    $('#txt_efinownerLN').val(removeSplName(data.EFINOwnerLastName));
                    $('#txt_efinownerSSN').val(data.EFINOwnerSSN);
                    $('#txt_efinownerDOB').val(data.EFINOwnerDOB);
                    $('#ddl_multioffice').val(data.IsMultiOffice);
                    $('#ddl_productsoffering').val(data.ProductsOffering);
                    $('#ddl_locationtransmit').val(data.IsOfficeTransmit);
                    $('#ddl_ptin').val(data.IsPTIN);
                    $('#ddl_processlaw').val(data.IsAsPerProcessLaw);
                    $('#ddl_complaince').val(data.IsAsPerComplainceLaw);
                    $('#ddl_consumerlending').val(data.ConsumerLending);
                    $('#txt_noofpersoneel').val(data.NoofPersoneel == 0 ? '' : data.NoofPersoneel)
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
                    $('#txt_efinownerAddress').val(removeSplAddress(data.EFINOwnerAddress));
                    $('#txt_efinownercity').val(removeSplCity(data.EFINOwnerCity));
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

                    if ($('#entityid').val() == $('#Entity_uTax').val() || $('#entityid').val() == $('#Entity_SO').val() || $('#entityid').val() == $('#Entity_SOME').val() ||
                        $('#entityid').val() == $('#Entity_MO').val() || $('#entityid').val() == $('#Entity_SVB').val()) {
                        $('#txt_sbfee').attr('title', data.ServiceBureaufeeTitle);
                        $('#txt_transmitfee').attr('title', data.AddonfeeTitle);
                    }
                    $('#ddl_cardprogram').val(data.PrepaidCardProgram);
                    if (data.TandC == true)
                        $('#chk_returns').prop('checked', true);
                }
                $.unblockUI();
            })
        }
    })
}

function saveTPGBankEnrollment(type) {

    if (bankstatus == 'SUB' && type == 1) {
        if ($('#sitea55334d1-3960-44c4-8cf1-e3ba9901f2be').length > 0)
            window.location.href = $('#sitea55334d1-3960-44c4-8cf1-e3ba9901f2be').attr('href');
        if ($('#site0eda5d25-591c-4e01-a845-fb580572cff5').length > 0)
            window.location.href = $('#site0eda5d25-591c-4e01-a845-fb580572cff5').attr('href');
        if ($('#site0eda5d25-591c-4e01-a845-fb580572cfe8').length > 0)
            window.location.href = $('#site0eda5d25-591c-4e01-a845-fb580572cfe8').attr('href');
        if ($('#site98a706d7-031f-4c5d-8cc4-d32cc7658b69').length > 0)
            window.location.href = $('#site98a706d7-031f-4c5d-8cc4-d32cc7658b69').attr('href');

        return;
    }
    else if (bankstatus == 'SUB') {
        return;
    }


    var _continue = true;

    var bankid = getUrlVars()["bankid"];

    ResetSuccessError();
    if ($('#txt_CompanyName').val().trim() == '') {
        $('#txt_CompanyName').addClass("error_msg");
        $('#txt_CompanyName').attr('title', 'Please enter Company Name');
        _continue = false;
    }
    else {
        $('#txt_CompanyName').removeClass("error_msg");
        $('#txt_CompanyName').attr('title', '');
    }

    if ($('#txt_CompanyName').val().trim() != '') {
        if ($('#txt_CompanyName').val().trim().length > 35) {
            $('#txt_CompanyName').addClass("error_msg");
            $('#txt_CompanyName').attr('title', 'Company Name max length is 35');
            _continue = false;
        }
        else {
            $('#txt_CompanyName').removeClass("error_msg");
            $('#txt_CompanyName').attr('title', '');
        }
    }

    if ($('#txt_ManagerFN').val().trim() == '') {
        $('#txt_ManagerFN').addClass("error_msg");
        $('#txt_ManagerFN').attr('title', 'Please enter Manager\'s First Name');
        _continue = false;
    }
    else {
        $('#txt_ManagerFN').removeClass("error_msg");
        $('#txt_ManagerFN').attr('title', '');
    }

    if ($('#txt_ManagerFN').val().trim() != '') {
        if ($('#txt_ManagerFN').val().trim().length > 20) {
            $('#txt_ManagerFN').addClass("error_msg");
            $('#txt_ManagerFN').attr('title', 'First Name max length is 20');
            _continue = false;
        }
        else {
            $('#txt_ManagerFN').removeClass("error_msg");
            $('#txt_ManagerFN').attr('title', '');
        }
    }

    if ($('#txt_ManagerLN').val().trim() == '') {
        $('#txt_ManagerLN').addClass("error_msg");
        $('#txt_ManagerLN').attr('title', 'Please enter Manager\'s Last Name');
        _continue = false;
    }
    else {
        $('#txt_ManagerLN').removeClass("error_msg");
        $('#txt_ManagerLN').attr('title', '');
    }

    if ($('#txt_ManagerLN').val().trim() != '') {
        if ($('#txt_ManagerLN').val().trim().length > 20) {
            $('#txt_ManagerLN').addClass("error_msg");
            $('#txt_ManagerLN').attr('title', 'Last Name max length is 20');
            _continue = false;
        }
        else {
            $('#txt_ManagerLN').removeClass("error_msg");
            $('#txt_ManagerLN').attr('title', '');
        }
    }

    if ($('#txt_OfficeAddress').val().trim() == '') {
        $('#txt_OfficeAddress').addClass("error_msg");
        $('#txt_OfficeAddress').attr('title', 'Please enter Office Address');
        _continue = false;
    }
    else {
        $('#txt_OfficeAddress').removeClass("error_msg");
        $('#txt_OfficeAddress').attr('title', '');
    }

    if ($('#txt_OfficeAddress').val().trim() != '') {
        if (isPOExist($('#txt_OfficeAddress').val())) {
            $('#txt_OfficeAddress').addClass("error_msg");
            $('#txt_OfficeAddress').attr('title', 'Office Address should not contain PO');
            _continue = false;
        }
        else {
            $('#txt_OfficeAddress').removeClass("error_msg");
            $('#txt_OfficeAddress').attr('title', '');
        }
    }

    if ($('#txt_OfficeAddress').val().trim() != '') {
        if ($('#txt_OfficeAddress').val().trim().length > 20) {
            $('#txt_OfficeAddress').addClass("error_msg");
            $('#txt_OfficeAddress').attr('title', 'Address max length is 40');
            _continue = false;
        }
        else {
            $('#txt_OfficeAddress').removeClass("error_msg");
            $('#txt_OfficeAddress').attr('title', '');
        }
    }

    if ($('#txt_OfficeCity').val().trim() == '') {
        $('#txt_OfficeCity').addClass("error_msg");
        $('#txt_OfficeCity').attr('title', 'Please enter Office City');
        _continue = false;
    }
    else {
        $('#txt_OfficeCity').removeClass("error_msg");
        $('#txt_OfficeCity').attr('title', '');
    }

    if ($('#txt_OfficeZip').val().trim() == '') {
        $('#txt_OfficeZip').addClass("error_msg");
        $('#txt_OfficeZip').attr('title', 'Please enter Office Zip Code');
        _continue = false;
    }
    else {
        $('#txt_OfficeZip').removeClass("error_msg");
        $('#txt_OfficeZip').attr('title', '');
    }

    if ($('#txt_OfficeZip').val().trim() != '') {
        var zip = $('#txt_OfficeZip').val().trim();
        if (zip.length != 5 && zip.length != 9) {
            $('#txt_OfficeZip').addClass("error_msg");
            $('#txt_OfficeZip').attr('title', 'Please enter valid Office Zip Code');
            _continue = false;
        }
        else {
            $('#txt_OfficeZip').removeClass("error_msg");
            $('#txt_OfficeZip').attr('title', '');
        }
    }

    if ($('#txt_OfficeTel').val().trim() == '') {
        $('#txt_OfficeTel').addClass("error_msg");
        $('#txt_OfficeTel').attr('title', 'Please enter Office Telephone Number');
        _continue = false;
    }
    else {
        $('#txt_OfficeTel').removeClass("error_msg");
        $('#txt_OfficeTel').attr('title', '');
    }


    if ($('#txt_OfficeTel').val().trim() != '') {
        var zip = $('#txt_OfficeTel').val().trim().replace(/-/g, '');
        if (0 < zip.indexOf(0) > 4 || 0 < zip.indexOf(1) > 4 || zip.length < 10) {
            $('#txt_OfficeTel').addClass("error_msg");
            $('#txt_OfficeTel').attr('title', 'Please enter valid Phone Number');
            _continue = false;
        }
        else {
            $('#txt_OfficeTel').removeClass("error_msg");
            $('#txt_OfficeTel').attr('title', '');
        }
    }

    if ($('#txt_ShipAddress').val().trim() == '') {
        $('#txt_ShipAddress').addClass("error_msg");
        $('#txt_ShipAddress').attr('title', 'Please enter Shipping Address');
        _continue = false;
    }
    else {
        $('#txt_ShipAddress').removeClass("error_msg");
        $('#txt_ShipAddress').attr('title', '');
    }

    if ($('#txt_ShipAddress').val().trim() != '') {
        if (isPOExist($('#txt_ShipAddress').val())) {
            $('#txt_ShipAddress').addClass("error_msg");
            $('#txt_ShipAddress').attr('title', 'Shipping Address should not contain PO');
            _continue = false;
        }
        else {
            $('#txt_ShipAddress').removeClass("error_msg");
            $('#txt_ShipAddress').attr('title', '');
        }
    }

    if ($('#txt_ShipAddress').val().trim() != '') {
        if ($('#txt_ShipAddress').val().trim().length > 20) {
            $('#txt_ShipAddress').addClass("error_msg");
            $('#txt_ShipAddress').attr('title', 'Address max length is 40');
            _continue = false;
        }
        else {
            $('#txt_ShipAddress').removeClass("error_msg");
            $('#txt_ShipAddress').attr('title', '');
        }
    }

    if ($('#txt_ShipCity').val().trim() == '') {
        $('#txt_ShipCity').addClass("error_msg");
        $('#txt_ShipCity').attr('title', 'Please enter Shipping City');
        _continue = false;
    }
    else {
        $('#txt_ShipCity').removeClass("error_msg");
        $('#txt_ShipCity').attr('title', '');
    }

    if ($('#txt_ShipZip').val().trim() == '') {
        $('#txt_ShipZip').addClass("error_msg");
        $('#txt_ShipZip').attr('title', 'Please enter Shipping Zip Code');
        _continue = false;
    }
    else {
        $('#txt_ShipZip').removeClass("error_msg");
        $('#txt_ShipZip').attr('title', '');
    }

    if ($('#txt_ShipZip').val().trim() != '') {
        var zip = $('#txt_ShipZip').val().trim();
        if (zip.length != 5 && zip.length != 9) {
            $('#txt_ShipZip').addClass("error_msg");
            $('#txt_ShipZip').attr('title', 'Please enter valid Shipping Zip code');
            _continue = false;
        }
        else {
            $('#txt_ShipZip').removeClass("error_msg");
            $('#txt_ShipZip').attr('title', '');
        }
    }

    if ($('#txt_ManagerEmail').val().trim() == '') {
        $('#txt_ManagerEmail').addClass("error_msg");
        $('#txt_ManagerEmail').attr('title', 'Please enter Manager\'s Email Address');
        _continue = false;
    }
    else {
        $('#txt_ManagerEmail').removeClass("error_msg");
        $('#txt_ManagerEmail').attr('title', '');
    }

    //if ($('#txt_OwnerEin').val().trim() == '') {
    //    $('#txt_OwnerEin').addClass("error_msg");
    //    $('#txt_OwnerEin').attr('title', 'Please enter EFIN Owner\'s EIN');
    //    _continue = false;
    //}
    //else {
    //    $('#txt_OwnerEin').removeClass("error_msg");
    //    $('#txt_OwnerEin').attr('title', '');
    //}

    if ($('#txt_OwnerEin').val().trim() != '') {
        var zip = $('#txt_OwnerEin').val().trim();
        if (zip.length < 9) {
            $('#txt_OwnerEin').addClass("error_msg");
            $('#txt_OwnerEin').attr('title', 'The EIN field can be left blank or needs to be a valid 9 digit number. Please do not provide the SSN here.');
            _continue = false;
        }
        else {
            $('#txt_OwnerEin').removeClass("error_msg");
            $('#txt_OwnerEin').attr('title', '');
        }
    }

    if ($('#txt_OwnerSSN').val().trim() == '') {
        $('#txt_OwnerSSN').addClass("error_msg");
        $('#txt_OwnerSSN').attr('title', 'The SSN field cannot be left blank and needs to be a valid 9 digit number. Please do not provide your EIN here.');
        _continue = false;
    }
    else {
        $('#txt_OwnerSSN').removeClass("error_msg");
        $('#txt_OwnerSSN').attr('title', '');
    }

    if ($('#txt_OwnerSSN').val().trim() != '') {
        var zip = $('#txt_OwnerSSN').val().trim();
        if (zip.length < 9) {
            $('#txt_OwnerSSN').addClass("error_msg");
            $('#txt_OwnerSSN').attr('title', 'The SSN field cannot be left blank and needs to be a valid 9 digit number. Please do not provide your EIN here.');
            _continue = false;
        }
        else {
            $('#txt_OwnerSSN').removeClass("error_msg");
            $('#txt_OwnerSSN').attr('title', '');
        }
    }

    if ($('#txt_OwnerFN').val().trim() == '') {
        $('#txt_OwnerFN').addClass("error_msg");
        $('#txt_OwnerFN').attr('title', 'Please enter EFIN Owner\'s First Name');
        _continue = false;
    }
    else {
        $('#txt_OwnerFN').removeClass("error_msg");
        $('#txt_OwnerFN').attr('title', '');
    }
    if ($('#txt_OwnerLN').val().trim() == '') {
        $('#txt_OwnerLN').addClass("error_msg");
        $('#txt_OwnerLN').attr('title', 'Please enter EFIN Owner\'s Last Name');
        _continue = false;
    }
    else {
        $('#txt_OwnerLN').removeClass("error_msg");
        $('#txt_OwnerLN').attr('title', '');
    }

    if ($('#txt_OwnerAddress').val().trim() == '') {
        $('#txt_OwnerAddress').addClass("error_msg");
        $('#txt_OwnerAddress').attr('title', 'Please enter EFIN Owner\'s City');
        _continue = false;
    }
    else {
        $('#txt_OwnerAddress').removeClass("error_msg");
        $('#txt_OwnerAddress').attr('title', '');
    }

    if ($('#txt_OwnerAddress').val().trim() != '') {
        if (isPOExist($('#txt_OwnerAddress').val())) {
            $('#txt_OwnerAddress').addClass("error_msg");
            $('#txt_OwnerAddress').attr('title', 'Owner Address should not contain PO');
            _continue = false;
        }
        else {
            $('#txt_OwnerAddress').removeClass("error_msg");
            $('#txt_OwnerAddress').attr('title', '');
        }
    }

    if ($('#txt_OwnerAddress').val().trim() != '') {
        if ($('#txt_OwnerAddress').val().trim().length > 20) {
            $('#txt_OwnerAddress').addClass("error_msg");
            $('#txt_OwnerAddress').attr('title', 'Address max length is 40');
            _continue = false;
        }
        else {
            $('#txt_OwnerAddress').removeClass("error_msg");
            $('#txt_OwnerAddress').attr('title', '');
        }
    }

    if ($('#txt_OwnerCity').val().trim() == '') {
        $('#txt_OwnerCity').addClass("error_msg");
        $('#txt_OwnerCity').attr('title', 'Please enter EFIN Owner\'s City');
        _continue = false;
    }
    else {
        $('#txt_OwnerCity').removeClass("error_msg");
        $('#txt_OwnerCity').attr('title', '');
    }

    if ($('#txt_OwnerZip').val().trim() == '') {
        $('#txt_OwnerZip').addClass("error_msg");
        $('#txt_OwnerZip').attr('title', 'Please enter EFIN Owner\'s Zip Code');
        _continue = false;
    }
    else {
        $('#txt_OwnerZip').removeClass("error_msg");
        $('#txt_OwnerZip').attr('title', '');
    }

    if ($('#txt_OwnerZip').val().trim() != '') {
        var zip = $('#txt_OwnerZip').val().trim();
        if (zip.length != 5 && zip.length != 9) {
            $('#txt_OwnerZip').addClass("error_msg");
            $('#txt_OwnerZip').attr('title', 'Please enter valid Owner Zip code');
            _continue = false;
        }
        else {
            $('#txt_OwnerZip').removeClass("error_msg");
            $('#txt_OwnerZip').attr('title', '');
        }
    }

    if ($('#txt_OwnerTel').val().trim() == '') {
        $('#txt_OwnerTel').addClass("error_msg");
        $('#txt_OwnerTel').attr('title', 'Please enter EFIN Owner\'s Telephone Number');
        _continue = false;
    }
    else {
        $('#txt_OwnerTel').removeClass("error_msg");
        $('#txt_OwnerTel').attr('title', '');
    }

    if ($('#txt_OwnerTel').val().trim() != '') {
        var zip = $('#txt_OwnerTel').val().trim().replace(/-/g, '');
        if (0 < zip.indexOf(0) > 4 || 0 < zip.indexOf(1) > 4 || zip.length < 10) {
            $('#txt_OwnerTel').addClass("error_msg");
            $('#txt_OwnerTel').attr('title', 'Please enter valid Telephone Number');
            _continue = false;
        }
        else {
            $('#txt_OwnerTel').removeClass("error_msg");
            $('#txt_OwnerTel').attr('title', '');
        }
    }

    if ($('#txt_OwnerDOB').val().trim() == '') {
        $('#txt_OwnerDOB').addClass("error_msg");
        $('#txt_OwnerDOB').attr('title', 'Please enter EFIN Owner\'s Date of Birth');
        _continue = false;
    }
    else {
        $('#txt_OwnerDOB').removeClass("error_msg");
        $('#txt_OwnerDOB').attr('title', '');
    }

    if ($('#txt_OwnerDOB').val().trim() != '') {
        var dob = $('#txt_OwnerDOB').val().trim();
        var d = new Date();
        d.setYear(d.getFullYear() - 18);

        if (dob.indexOf('/') > 0) {
            if (!isValidDateFormat(dob)) {
                $('#txt_OwnerDOB').addClass("error_msg");
                $('#txt_OwnerDOB').attr('title', 'Please enter valid EFIN Owner\'s Date of Birth');
                _continue = false;
            }
            else {
                $('#txt_OwnerDOB').removeClass("error_msg");
                $('#txt_OwnerDOB').attr('title', '');
                var dd = new Date(dob);
                if (dd > d) {
                    $('#txt_OwnerDOB').addClass("error_msg");
                    $('#txt_OwnerDOB').attr('title', 'EFIN Owner\'s Date of Birth must be 18 years ago');
                    _continue = false;
                }
            }
        }
        else {
            var pos = 2, yearpos = 5;
            var b = '/';
            var updob = [dob.slice(0, pos), b, dob.slice(pos)].join('');
            var _updob = [updob.slice(0, yearpos), b, updob.slice(yearpos)].join('');
            if (!isValidDateFormat(_updob)) {
                $('#txt_OwnerDOB').addClass("error_msg");
                $('#txt_OwnerDOB').attr('title', 'Please enter valid EFIN Owner\'s Date of Birth');
                _continue = false;
            }
            else {
                $('#txt_OwnerDOB').removeClass("error_msg");
                $('#txt_OwnerDOB').attr('title', '');
                var dd = new Date(_updob);
                if (dd > d) {
                    $('#txt_OwnerDOB').addClass("error_msg");
                    $('#txt_OwnerDOB').attr('title', 'EFIN Owner\'s Date of Birth must be 18 years ago');
                    _continue = false;
                }
            }
        }
    }

    if ($('#txt_OwnerEmail').val().trim() == '') {
        $('#txt_OwnerEmail').addClass("error_msg");
        $('#txt_OwnerEmail').attr('title', 'Please enter EFIN Owner\'s Email Address');
        _continue = false;
    }
    else {
        $('#txt_OwnerEmail').removeClass("error_msg");
        $('#txt_OwnerEmail').attr('title', '');
    }

    if ($('#txt_LYVolume').val().trim() == '') {
        $('#txt_LYVolume').addClass("error_msg");
        $('#txt_LYVolume').attr('title', 'Please enter Prior Year Volume');
        _continue = false;
    }
    else {
        $('#txt_LYVolume').removeClass("error_msg");
        $('#txt_LYVolume').attr('title', '');
    }
    if ($('#txt_BPF').val().trim() == '') {
        $('#txt_BPF').addClass("error_msg");
        $('#txt_BPF').attr('title', 'Please enter Prior Year Bank Products Funded');
        _continue = false;
    }
    else {
        $('#txt_BPF').removeClass("error_msg");
        $('#txt_BPF').attr('title', '');
    }

    if ($('#txt_OfficeRTN').val().trim() == '') {
        $('#txt_OfficeRTN').addClass("error_msg");
        $('#txt_OfficeRTN').attr('title', 'Please enter Office RTN');
        _continue = false;
    }
    else {
        $('#txt_OfficeRTN').removeClass("error_msg");
        $('#txt_OfficeRTN').attr('title', '');
    }

    if ($('#txt_ConfirmOfficeRTN').val().trim() == '') {
        $('#txt_ConfirmOfficeRTN').addClass("error_msg");
        $('#txt_ConfirmOfficeRTN').attr('title', 'Please enter Confirm Office RTN');
        _continue = false;
    }
    else {
        $('#txt_ConfirmOfficeRTN').removeClass("error_msg");
        $('#txt_ConfirmOfficeRTN').attr('title', '');
    }

    //if ($('#txt_ConfirmOfficeRTN').val().trim() == '') {
    //    $('#txt_ConfirmOfficeRTN').addClass("error_msg");
    //    $('#txt_ConfirmOfficeRTN').attr('title', 'Please enter Confirm Office RTN');
    //    _continue = false;
    //}
    //else {
    //    $('#txt_ConfirmOfficeRTN').removeClass("error_msg");
    //    $('#txt_ConfirmOfficeRTN').attr('title', '');
    //}

    if ($('#txt_ConfirmOfficeRTN').val() != '' && $('#txt_OfficeRTN').val() != '') {
        if ($('#txt_ConfirmOfficeRTN').val() != $('#txt_OfficeRTN').val()) {
            $('#txt_ConfirmOfficeRTN').addClass("error_msg");
            $('#txt_ConfirmOfficeRTN').attr('title', 'RTN is mismatching.');
            _continue = false;
        }
        else {
            $('#txt_ConfirmOfficeRTN').removeClass("error_msg");
            $('#txt_ConfirmOfficeRTN').attr('title', '');
        }
    }

    if ($('#txt_OfficeDAN').val().trim() == '') {
        $('#txt_OfficeDAN').addClass("error_msg");
        $('#txt_OfficeDAN').attr('title', 'Please enter Office DAN');
        _continue = false;
    }
    else {
        $('#txt_OfficeDAN').removeClass("error_msg");
        $('#txt_OfficeDAN').attr('title', '');
    }

    if ($('#txt_OfficeDAN').val().trim() != '') {
        var dan = parseInt($('#txt_OfficeDAN').val());
        if (dan <= 0) {
            $('#txt_OfficeDAN').addClass("error_msg");
            $('#txt_OfficeDAN').attr('title', 'Please enter valid DAN');
            _continue = false;
        }
        else {
            $('#txt_OfficeDAN').removeClass("error_msg");
            $('#txt_OfficeDAN').attr('title', '');
        }
    }

    if ($('#txt_ConfirmOfficeDAN').val().trim() == '') {
        $('#txt_ConfirmOfficeDAN').addClass("error_msg");
        $('#txt_ConfirmOfficeDAN').attr('title', 'Please enter Confirm Office DAN');
        _continue = false;
    }
    else {
        $('#txt_ConfirmOfficeDAN').removeClass("error_msg");
        $('#txt_ConfirmOfficeDAN').attr('title', '');
    }

    if ($('#txt_ConfirmOfficeDAN').val() != '' && $('#txt_OfficeDAN').val() != '') {
        if ($('#txt_ConfirmOfficeDAN').val() != $('#txt_OfficeDAN').val()) {
            $('#txt_ConfirmOfficeDAN').addClass("error_msg");
            $('#txt_ConfirmOfficeDAN').attr('title', 'DAN is mismatching.');
            _continue = false;
        }
        else {
            $('#txt_ConfirmOfficeDAN').removeClass("error_msg");
            $('#txt_ConfirmOfficeDAN').attr('title', '');
        }
    }

    if ($('#ddl_officestate').val() == '' || $('#ddl_officestate').val() == 0 || $('#ddl_officestate').val() == undefined) {
        $('#ddl_officestate').addClass("error_msg");
        $('#ddl_officestate').attr('title', 'Please select Office State');
        _continue = false;
    }
    else {
        $('#ddl_officestate').removeClass("error_msg");
        $('#ddl_officestate').attr('title', '');
    }

    if ($('#ddl_shipstate').val() == '' || $('#ddl_shipstate').val() == 0 || $('#ddl_shipstate').val() == undefined) {
        $('#ddl_shipstate').addClass("error_msg");
        $('#ddl_shipstate').attr('title', 'Please select Shipping State');
        _continue = false;
    }
    else {
        $('#ddl_shipstate').removeClass("error_msg");
        $('#ddl_shipstate').attr('title', '');
    }

    if ($('#ddl_ownerstate').val() == '' || $('#ddl_ownerstate').val() == 0 || $('#ddl_ownerstate').val() == undefined) {
        $('#ddl_ownerstate').addClass("error_msg");
        $('#ddl_ownerstate').attr('title', 'Please select EFIN Owner\'s State');
        _continue = false;
    }
    else {
        $('#ddl_ownerstate').removeClass("error_msg");
        $('#ddl_ownerstate').attr('title', '');
    }

    if ($('#ddl_BankLY').val() == '' || $('#ddl_BankLY').val() == 0 || $('#ddl_BankLY').val() == undefined) {
        $('#ddl_BankLY').addClass("error_msg");
        $('#ddl_BankLY').attr('title', 'Please select Bank Used Last Year');
        _continue = false;
    }
    else {
        $('#ddl_BankLY').removeClass("error_msg");
        $('#ddl_BankLY').attr('title', '');
    }

    if ($('#ddl_accttype').val() == '' || $('#ddl_accttype').val() == 0 || $('#ddl_accttype').val() == undefined) {
        $('#ddl_accttype').addClass("error_msg");
        $('#ddl_accttype').attr('title', 'Please select Office ACCT Type');
        _continue = false;
    }
    else {
        $('#ddl_accttype').removeClass("error_msg");
        $('#ddl_accttype').attr('title', '');
    }

    if ($('#txt_OwnerTitle').val().trim() == '') {
        $('#txt_OwnerTitle').addClass("error_msg");
        $('#txt_OwnerTitle').attr('title', 'Please enter EFIN Owner\'s Title');
        _continue = false;
    }
    else {
        $('#txt_OwnerTitle').removeClass("error_msg");
        $('#txt_OwnerTitle').attr('title', '');
    }

    //if ($('#txt_OwnerMobile').val().trim() == '') {
    //    $('#txt_OwnerMobile').addClass("error_msg");
    //    $('#txt_OwnerMobile').attr('title', 'Please enter EFIN Owner\'s Mobile Number');
    //    _continue = false;
    //}
    //else {
    //    $('#txt_OwnerMobile').removeClass("error_msg");
    //    $('#txt_OwnerMobile').attr('title', '');
    //}

    //if ($('#txt_OwnerMobile').val().trim() != '') {
    //    var zip = $('#txt_OwnerMobile').val().trim().replace(/-/g, '');
    //    if (0 < zip.indexOf(0) > 4 || 0 < zip.indexOf(1) > 4 || zip.length < 10) {
    //        $('#txt_OwnerMobile').addClass("error_msg");
    //        $('#txt_OwnerMobile').attr('title', 'Please enter valid Mobile Number');
    //        _continue = false;
    //    }
    //    else {
    //        $('#txt_OwnerMobile').removeClass("error_msg");
    //        $('#txt_OwnerMobile').attr('title', '');
    //    }
    //}

    if ($('#txt_OwnerIDNumber').val().trim() == '') {
        $('#txt_OwnerIDNumber').addClass("error_msg");
        $('#txt_OwnerIDNumber').attr('title', 'Please enter EFIN Owner\'s ID Number');
        _continue = false;
    }
    else {
        $('#txt_OwnerIDNumber').removeClass("error_msg");
        $('#txt_OwnerIDNumber').attr('title', '');
    }

    if ($('#ddl_owneridstate').val() == '' || $('#ddl_owneridstate').val() == 0 || $('#ddl_owneridstate').val() == undefined) {
        $('#ddl_owneridstate').addClass("error_msg");
        $('#ddl_owneridstate').attr('title', 'Please select EFIN Owner\'s ID State');
        _continue = false;
    }
    else {
        $('#ddl_owneridstate').removeClass("error_msg");
        $('#ddl_owneridstate').attr('title', '');
    }

    if ($('#txt_docprep').val().trim() == '') {
        $('#txt_docprep').addClass("error_msg");
        $('#txt_docprep').attr('title', 'Please enter Doc Prep Fee');
        _continue = false;
    }
    else {
        $('#txt_docprep').removeClass("error_msg");
        $('#txt_docprep').attr('title', '');
    }

    if ($('#txt_bankname').val().trim() == '') {
        $('#txt_bankname').addClass("error_msg");
        $('#txt_bankname').attr('title', 'Please enter Bank Name');
        _continue = false;
    }
    else {
        $('#txt_bankname').removeClass("error_msg");
        $('#txt_bankname').attr('title', '');
    }

    if ($('#txt_accountname').val().trim() == '') {
        $('#txt_accountname').addClass("error_msg");
        $('#txt_accountname').attr('title', 'Please Account Name');
        _continue = false;
    }
    else {
        $('#txt_accountname').removeClass("error_msg");
        $('#txt_accountname').attr('title', '');
    }

    if (!$('#chk_agreebank').prop('checked')) {
        $('#dvagreetc').addClass("error_msg");
        $('#dvagreetc').attr('title', 'Please Agree Terms and Conditions');
        _continue = false;
    }
    else {
        $('#dvagreetc').removeClass("error_msg");
        $('#dvagreetc').attr('title', '');
    }

    //if (!$('#chk_feeall').prop('checked')) {
    //    $('#dvfeeall').addClass("error_msg");
    //    $('#dvfeeall').attr('title', 'Please select checkbox');
    //    _continue = false;
    //}
    //else {
    //    $('#dvfeeall').removeClass("error_msg");
    //    $('#dvfeeall').attr('title', '');
    //}

    var rtn = $('#txt_OfficeRTN').val().trim();
    if (rtn != '') {
        var isValidrtn = checkABA(rtn);
        if (!isValidrtn) {
            _continue = false;
            $('#txt_OfficeRTN').addClass("error_msg");
            $('#txt_OfficeRTN').attr('title', 'Please enter valid RTN');
        }
    }

    var managerEmail = $('#txt_ManagerEmail').val().trim();
    if (managerEmail != '') {
        var validemail = validateEmail(managerEmail);
        if (!validemail) {
            _continue = false;
            $('#txt_ManagerEmail').addClass("error_msg");
            $('#txt_ManagerEmail').attr('title', 'Please enter valid Email Address');
        }
    }

    var ownerEmail = $('#txt_OwnerEmail').val().trim();
    if (ownerEmail != '') {
        var _validmail = validateEmail(ownerEmail);
        if (!_validmail) {
            _continue = false;
            $('#txt_OwnerEmail').addClass("error_msg");
            $('#txt_OwnerEmail').attr('title', 'Please enter valid Email Address');
        }
    }

    var prepfee = $('#txt_docprep').val().trim();
    if (prepfee != '') {
        if (parseFloat(prepfee) > 40) {
            _continue = false;
            $('#txt_docprep').addClass("error_msg");
            $('#txt_docprep').attr('title', 'Please enter less than or equal to 40');
        }
    }

    if (!_continue) {
        $("html, body").animate({ scrollTop: 0 }, "slow");
        error.show();
        error.append('<p>  Please correct error(s). </p>');

        if ($('#dvGeneral input.error_msg').length > 0 || $('#dvGeneral select.error_msg').length > 0 || $('#dvGeneral textarea.error_msg').length > 0) {//$('#a_general').click();
            $('#libOfficeInfo').addClass('active');
            $('#dvGeneral').addClass('active in');
            $('#libankselection').removeClass('active');
            $('#dvBank').removeClass('active');
        }
        else if ($('#dvEfin_ab input.error_msg').length > 0 || $('#dvEfin_ab select.error_msg').length > 0 || $('#dvEfin_ab textarea.error_msg').length > 0) {
            // $('#a_efinowner').click();
            $('#libOfficeConfig').addClass('active');
            $('#dvEfin_ab').addClass('active in');
            $('#libankselection').removeClass('active');
            $('#dvBank').removeClass('active in');
        }
        else if ($('#dvFee input.error_msg').length > 0 || $('#dvFee div.error_msg').length > 0) {
            //$('#a_fee').click();
            $('#liaffiliate').addClass('active');
            $('#dvFee').addClass('active in');
            $('#libankselection').removeClass('active');
            $('#dvBank').removeClass('active in');
        }
        else if ($('#dvPrior input.error_msg').length > 0 || $('#dvPrior select.error_msg').length > 0) { // $('#a_prior').click();
            $('#lifee').addClass('active');
            $('#dvPrior').addClass('active in');
            $('#libankselection').removeClass('active');
            $('#dvBank').removeClass('active in');
        }
        else {
            //$('#a_bank').click();
            $('#libankselection').addClass('active');
            $('#dvBank').addClass('active in');
            return;
        }
    }


    if (_continue) {
        var requuest = {};
        requuest.CompanyName = $('#txt_CompanyName').val().trim();
        requuest.ManagerFirstName = $('#txt_ManagerFN').val().trim();
        requuest.ManagerLastName = $('#txt_ManagerLN').val().trim();
        requuest.OfficeAddress = $('#txt_OfficeAddress').val().trim();
        requuest.OfficeCity = $('#txt_OfficeCity').val().trim();
        requuest.OfficeZip = $('#txt_OfficeZip').val().trim();
        requuest.OfficeTelephone = $('#txt_OfficeTel').val().trim();
        requuest.OfficeFax = $('#txt_OfficeFax').val().trim();
        requuest.ShippingAddress = $('#txt_ShipAddress').val().trim();
        requuest.ShippingCity = $('#txt_ShipCity').val().trim();
        requuest.ShippingZip = $('#txt_ShipZip').val().trim();
        requuest.ManagerEmail = $('#txt_ManagerEmail').val().trim();
        requuest.OwnerEIN = $('#txt_OwnerEin').val().trim();
        requuest.OwnerSSn = $('#txt_OwnerSSN').val().trim();
        requuest.OwnerFirstName = $('#txt_OwnerFN').val().trim();
        requuest.OwnerLastName = $('#txt_OwnerLN').val().trim();
        requuest.OwnerAddress = $('#txt_OwnerAddress').val().trim();
        requuest.OwnerCity = $('#txt_OwnerCity').val().trim();
        requuest.OwnerZip = $('#txt_OwnerZip').val().trim();
        requuest.OwnerTelephone = $('#txt_OwnerTel').val().trim();
        var odob = $('#txt_OwnerDOB').val();
        if (odob) {
            if (odob.indexOf('/') > 0)
                requuest.OwnerDOB = odob;
            else
                requuest.OwnerDOB = getformattedDate(odob);
        }
        requuest.OwnerEmail = $('#txt_OwnerEmail').val().trim();
        requuest.LastYearVolume = $('#txt_LYVolume').val().trim();
        requuest.LastYearEFIN = $('#txt_EFINLY').val().trim();
        requuest.BankProductFund = $('#txt_BPF').val().trim();
        requuest.OfficeRTN = $('#txt_OfficeRTN').val().trim();
        requuest.OfficeDAN = $('#txt_OfficeDAN').val().trim();
        requuest.AccountType = $('#ddl_accttype').val();
        requuest.LastYearBank = $('#ddl_BankLY').val();
        requuest.OwnerState = $('#ddl_ownerstate').val();
        requuest.ShippingState = $('#ddl_shipstate').val();
        requuest.OfficeState = $('#ddl_officestate').val();
        requuest.UserId = $('#UserId').val();
        requuest.CustomerId = $('#UserId').val();
        requuest.EfinOwnerTitle = $('#txt_OwnerTitle').val();
        requuest.EfinOwnerMobile = '';// $('#txt_OwnerMobile').val();
        requuest.EfinIDNumber = $('#txt_OwnerIDNumber').val();
        requuest.EfinIdState = $('#ddl_owneridstate').val();
        requuest.Addonfee = $('#txt_addonfee').val();
        requuest.ServiceBureaufee = $('#txt_sbfee').val();
        requuest.CheckPrint = 'R';
        requuest.AgreeBank = $('#chk_agreebank').prop('checked');
        requuest.SbfeeAll = $('#chk_feeall').prop('checked') ? 'X' : ' ';
        requuest.DocPrepFee = $('#txt_docprep').val();
        requuest.BankName = $('#txt_bankname').val();
        requuest.AccountName = $('#txt_accountname').val();
        requuest.BankId = bankid;

        var Cid = getUrlVars()["Id"];
        var entityid = $('#entityid').val();

        if (Cid) {
            entityid = $('#myentityid').val();
            requuest.CustomerId = Cid;
            entityid = getUrlVars()["myentityid"];
        }


        if ($('#entityid').val() != $('#myentityid').val()) {
            entityid = $('#myentityid').val();
            requuest.CustomerId = $('#myid').val();;
        }

        var saveURI = '/api/EnrollmentBankSelection/SaveTPGBankEnrollment';
        ajaxHelper(saveURI, 'POST', requuest, false).done(function (data, status) {
            if (data) {
                if (data.Status) {
                    SaveConfigStatusActive('done', bankid);
                    UpdateOfficeManagement(requuest.CustomerId);
                    $("html, body").animate({ scrollTop: 0 }, "slow");
                    success.show();
                    success.append('<p> Bank Enrollment details saved successfully</p>');
                    $('#site0feeb0fe-d0e7-4370-8733-dd5f7d2041fc').addClass('done');

                    if (type == 1) {

                        //if (Cid)
                        //    window.location.href = '../Enrollment/enrollmentsummary?Id=' + Cid;
                        //else
                        //    window.location.href = '../Enrollment/enrollmentsummary';

                        if ($('#entityid').val() != $('#myentityid').val()) {
                            if (entityid == $('#Entity_SO').val() || entityid == $('#Entity_SOME').val() || entityid == $('#Entity_SOME_SS').val()) {
                                var feeuri = '/api/EnrollmentBankSelection/getAddonSelection?CustomerId=' + requuest.CustomerId + '&bankid=' + bankid;
                                ajaxHelper(feeuri, 'GET').done(function (res) {
                                    if (res) {
                                        window.location.href = '/Enrollment/EnrollmentFeeReimbursement?Id=' + requuest.CustomerId + '&entitydisplayid=' + $('#myentitydisplayid').val() + '&ParentId=' + $('#myparentid').val() + '&entityid=' + $('#myentityid').val() + '&ptype=enrollment&bankid=' + bankid;
                                    }
                                    else {
                                        if ($('#site0eda5d25-591c-4e01-a845-fb580572cff5').length > 0) {
                                            window.location.href = '/PaymentOptions/efile?Id=' + requuest.CustomerId + '&entitydisplayid=' + $('#myentitydisplayid').val() + '&ParentId=' + $('#myparentid').val() + '&entityid=' + $('#myentityid').val() + '&ptype=enrollment&bankid=' + bankid;
                                        }
                                        else if ($('#site0eda5d25-591c-4e01-a845-fb580572cfe8').length > 0) {
                                            window.location.href = '/PaymentOptions/OutstandingBalance?Id=' + requuest.CustomerId + '&entitydisplayid=' + $('#myentitydisplayid').val() + '&ParentId=' + $('#myparentid').val() + '&entityid=' + $('#myentityid').val() + '&ptype=enrollment&bankid=' + bankid;
                                        }
                                        else
                                            window.location.href = '/Enrollment/enrollmentsummary?Id=' + requuest.CustomerId + '&entitydisplayid=' + $('#myentitydisplayid').val() + '&ParentId=' + $('#myparentid').val() + '&entityid=' + $('#myentityid').val() + '&ptype=enrollment&bankid=' + bankid;
                                    }
                                })
                            }

                            else {
                                if ($('#site0eda5d25-591c-4e01-a845-fb580572cff5').length > 0) {
                                    window.location.href = '/PaymentOptions/efile?Id=' + requuest.CustomerId + '&entitydisplayid=' + $('#myentitydisplayid').val() + '&ParentId=' + $('#myparentid').val() + '&entityid=' + $('#myentityid').val() + '&ptype=enrollment&bankid=' + bankid;
                                }
                                else if ($('#site0eda5d25-591c-4e01-a845-fb580572cfe8').length > 0) {
                                    window.location.href = '/PaymentOptions/OutstandingBalance?Id=' + requuest.CustomerId + '&entitydisplayid=' + $('#myentitydisplayid').val() + '&ParentId=' + $('#myparentid').val() + '&entityid=' + $('#myentityid').val() + '&ptype=enrollment&bankid=' + bankid;
                                }
                                else
                                    window.location.href = '/Enrollment/enrollmentsummary?Id=' + requuest.CustomerId + '&entitydisplayid=' + $('#myentitydisplayid').val() + '&ParentId=' + $('#myparentid').val() + '&entityid=' + $('#myentityid').val() + '&ptype=enrollment&bankid=' + bankid;
                            }
                        }
                        else {
                            if (entityid == $('#Entity_SO').val() || entityid == $('#Entity_SOME').val() || entityid == $('#Entity_SOME_SS').val()) {
                                var feeuri = '/api/EnrollmentBankSelection/getAddonSelection?CustomerId=' + requuest.CustomerId + '&bankid=' + bankid;
                                ajaxHelper(feeuri, 'GET').done(function (res) {
                                    
                                    if (res) {
                                        window.location.href = '/Enrollment/EnrollmentFeeReimbursement?bankid=' + bankid;
                                    }
                                    else {
                                        if ($('#site0eda5d25-591c-4e01-a845-fb580572cff5').length > 0) {
                                            window.location.href = '/PaymentOptions/efile?bankid=' + bankid;
                                        }
                                        else if ($('#site0eda5d25-591c-4e01-a845-fb580572cfe8').length > 0) {
                                            window.location.href = '/PaymentOptions/OutstandingBalance?bankid=' + bankid;
                                        }
                                        else
                                            window.location.href = '/Enrollment/enrollmentsummary?bankid=' + bankid;
                                    }
                                })
                            }
                            else {
                                if ($('#site0eda5d25-591c-4e01-a845-fb580572cff5').length > 0) {
                                    window.location.href = '/PaymentOptions/efile?bankid=' + bankid;
                                }
                                else if ($('#site0eda5d25-591c-4e01-a845-fb580572cfe8').length > 0) {
                                    window.location.href = '/PaymentOptions/OutstandingBalance?bankid=' + bankid;
                                }
                                else
                                    window.location.href = '/Enrollment/enrollmentsummary?bankid=' + bankid;
                            }
                        }
                    }
                    else
                        getConfigStatus();
                }
                else {
                    $("html, body").animate({ scrollTop: 0 }, "slow");
                    error.show();
                    error.append('<p> Details not saved. </p>');
                    $.each(data.Messages, function (item, value) {
                        error.append('<p> ' + value + ' </p>');
                    })
                }
            }
            else {
                $("html, body").animate({ scrollTop: 0 }, "slow");
                error.show();
                error.append('<p> Details not saved. </p>');
            }
        });
    }
}

function saveRABankEnrollment(type) {

    if (bankstatus == 'SUB' && type == 1) {
        if ($('#sitea55334d1-3960-44c4-8cf1-e3ba9901f2be').length > 0)
            window.location.href = $('#sitea55334d1-3960-44c4-8cf1-e3ba9901f2be').attr('href');
        if ($('#site0eda5d25-591c-4e01-a845-fb580572cff5').length > 0)
            window.location.href = $('#site0eda5d25-591c-4e01-a845-fb580572cff5').attr('href');
        if ($('#site0eda5d25-591c-4e01-a845-fb580572cfe8').length > 0)
            window.location.href = $('#site0eda5d25-591c-4e01-a845-fb580572cfe8').attr('href');
        if ($('#site98a706d7-031f-4c5d-8cc4-d32cc7658b69').length > 0)
            window.location.href = $('#site98a706d7-031f-4c5d-8cc4-d32cc7658b69').attr('href');

        return;
    }
    else if (bankstatus == 'SUB') {
        return;
    }


    var bankid = getUrlVars()["bankid"];

    var _continue = true;
    ResetSuccessError();
    if ($('#txt_ownertitle').val().trim() == '') {
        $('#txt_ownertitle').addClass("error_msg");
        $('#txt_ownertitle').attr('title', 'Please enter EFIN Owner Business Title');
        _continue = false;
    }
    else {
        $('#txt_ownertitle').removeClass("error_msg");
        $('#txt_ownertitle').attr('title', '');
    }

    if ($('#txt_owneremail').val().trim() == '') {
        $('#txt_owneremail').addClass("error_msg");
        $('#txt_owneremail').attr('title', 'Please enter EFIN Owner email address');
        _continue = false;
    }
    else {
        $('#txt_owneremail').removeClass("error_msg");
        $('#txt_owneremail').attr('title', '');
    }

    if ($('#txt_ownerFN').val().trim() == '') {
        $('#txt_ownerFN').addClass("error_msg");
        $('#txt_ownerFN').attr('title', 'Please enter EFIN Owner first name');
        _continue = false;
    }
    else {
        $('#txt_ownerFN').removeClass("error_msg");
        $('#txt_ownerFN').attr('title', '');
    }

    if ($('#txt_ownerFN').val().trim() != '') {
        if ($('#txt_ownerFN').val().trim().length > 20) {
            $('#txt_ownerFN').addClass("error_msg");
            $('#txt_ownerFN').attr('title', 'First Name max length is 20');
            _continue = false;
        }
        else {
            $('#txt_ownerFN').removeClass("error_msg");
            $('#txt_ownerFN').attr('title', '');
        }
    }

    if ($('#txt_ownerLN').val().trim() == '') {
        $('#txt_ownerLN').addClass("error_msg");
        $('#txt_ownerLN').attr('title', 'Please enter EFIN Owner last name');
        _continue = false;
    }
    else {
        $('#txt_ownerLN').removeClass("error_msg");
        $('#txt_ownerLN').attr('title', '');
    }

    if ($('#txt_ownerLN').val().trim() != '') {
        if ($('#txt_ownerLN').val().trim().length > 20) {
            $('#txt_ownerLN').addClass("error_msg");
            $('#txt_ownerLN').attr('title', 'Last Name max length is 20');
            _continue = false;
        }
        else {
            $('#txt_ownerLN').removeClass("error_msg");
            $('#txt_ownerLN').attr('title', '');
        }
    }

    if ($('#txt_ownerSSN').val().trim() == '') {
        $('#txt_ownerSSN').addClass("error_msg");
        $('#txt_ownerSSN').attr('title', 'Please enter EFIN Owner SSN');
        _continue = false;
    }
    else {
        $('#txt_ownerSSN').removeClass("error_msg");
        $('#txt_ownerSSN').attr('title', '');
    }

    if ($('#txt_ownerSSN').val().trim() != '') {
        if ($('#txt_ownerSSN').val().trim().length < 9) {
            $('#txt_ownerSSN').addClass("error_msg");
            $('#txt_ownerSSN').attr('title', 'Please enter valid EFIN Owner SSN');
            _continue = false;
        }
        else {
            $('#txt_ownerSSN').removeClass("error_msg");
            $('#txt_ownerSSN').attr('title', '');
        }
    }

    if ($('#txt_ownerDOB').val().trim() == '') {
        $('#txt_ownerDOB').addClass("error_msg");
        $('#txt_ownerDOB').attr('title', 'Please enter EFIN Owner Date of birth');
        _continue = false;
    }
    else {
        $('#txt_ownerDOB').removeClass("error_msg");
        $('#txt_ownerDOB').attr('title', '');
    }

    if ($('#txt_ownerDOB').val().trim() != '') {
        var dob = $('#txt_ownerDOB').val().trim();
        var d = new Date();
        d.setYear(d.getFullYear() - 18);

        if (dob.indexOf('/') > 0) {
            if (!isValidDateFormat(dob)) {
                $('#txt_ownerDOB').addClass("error_msg");
                $('#txt_ownerDOB').attr('title', 'Please enter valid EFIN Owner Date of Birth');
                _continue = false;
            }
            else {
                $('#txt_ownerDOB').removeClass("error_msg");
                $('#txt_ownerDOB').attr('title', '');
                var dd = new Date(dob);
                if (dd > d) {
                    $('#txt_ownerDOB').addClass("error_msg");
                    $('#txt_ownerDOB').attr('title', 'EFIN Owner Date of Birth must be 18 years ago');
                    _continue = false;
                }
            }
        }
        else {
            var pos = 2, yearpos = 5;
            var b = '/';
            var updob = [dob.slice(0, pos), b, dob.slice(pos)].join('');
            var _updob = [updob.slice(0, yearpos), b, updob.slice(yearpos)].join('');
            if (!isValidDateFormat(_updob)) {
                $('#txt_ownerDOB').addClass("error_msg");
                $('#txt_ownerDOB').attr('title', 'Please enter valid EFIN Owner Date of Birth');
                _continue = false;
            }
            else {
                $('#txt_ownerDOB').removeClass("error_msg");
                $('#txt_ownerDOB').attr('title', '');
                var dd = new Date(_updob);
                if (dd > d) {
                    $('#txt_ownerDOB').addClass("error_msg");
                    $('#txt_ownerDOB').attr('title', 'EFIN Owner Date of Birth must be 18 years ago');
                    _continue = false;
                }
            }
        }
    }

    //if ($('#txt_ownercell').val().trim() == '') {
    //    $('#txt_ownercell').addClass("error_msg");
    //    $('#txt_ownercell').attr('title', 'Please enter EFIN Owner cell phone');
    //    _continue = false;
    //}
    //else {
    //    $('#txt_ownercell').removeClass("error_msg");
    //    $('#txt_ownercell').attr('title', '');
    //}

    if ($('#txt_ownercell').val().trim() != '') {
        var zip = $('#txt_ownercell').val().trim().replace(/-/g, '');
        if (0 < zip.indexOf(0) > 4 || 0 < zip.indexOf(1) > 4 || zip.length < 10) {
            $('#txt_ownercell').addClass("error_msg");
            $('#txt_ownercell').attr('title', 'Please enter valid Mobile Number');
            _continue = false;
        }
        else {
            $('#txt_ownercell').removeClass("error_msg");
            $('#txt_ownercell').attr('title', '');
        }
    }

    if ($('#txt_ownerphone').val().trim() == '') {
        $('#txt_ownerphone').addClass("error_msg");
        $('#txt_ownerphone').attr('title', 'Please enter EFIN Owner phone');
        _continue = false;
    }
    else {
        $('#txt_ownerphone').removeClass("error_msg");
        $('#txt_ownerphone').attr('title', '');
    }

    if ($('#txt_ownerphone').val().trim() != '') {
        var zip = $('#txt_ownerphone').val().trim().replace(/-/g, '');
        if (0 < zip.indexOf(0) > 4 || 0 < zip.indexOf(1) > 4 || zip.length < 10) {
            $('#txt_ownerphone').addClass("error_msg");
            $('#txt_ownerphone').attr('title', 'Please enter valid Phone Number');
            _continue = false;
        }
        else {
            $('#txt_ownerphone').removeClass("error_msg");
            $('#txt_ownerphone').attr('title', '');
        }
    }

    if ($('#txt_ownerAddress').val().trim() == '') {
        $('#txt_ownerAddress').addClass("error_msg");
        $('#txt_ownerAddress').attr('title', 'Please enter EFIN Owner Address');
        _continue = false;
    }
    else {
        $('#txt_ownerAddress').removeClass("error_msg");
        $('#txt_ownerAddress').attr('title', '');
    }
    
    if ($('#txt_ownerAddress').val().trim() != '') {
        if (isPOExist($('#txt_ownerAddress').val())) {
            $('#txt_ownerAddress').addClass("error_msg");
            $('#txt_ownerAddress').attr('title', 'Owner Address should not contain PO');
            _continue = false;
        }
        else {
            $('#txt_ownerAddress').removeClass("error_msg");
            $('#txt_ownerAddress').attr('title', '');
        }
    }

    if ($('#txt_ownerAddress').val().trim() != '') {
        if ($('#txt_ownerAddress').val().trim().length > 40) {
            $('#txt_ownerAddress').addClass("error_msg");
            $('#txt_ownerAddress').attr('title', 'Last Name max length is 40');
            _continue = false;
        }
        else {
            $('#txt_ownerAddress').removeClass("error_msg");
            $('#txt_ownerAddress').attr('title', '');
        }
    }

    if ($('#txt_ownerCity').val().trim() == '') {
        $('#txt_ownerCity').addClass("error_msg");
        $('#txt_ownerCity').attr('title', 'Please enter EFIN Owner City');
        _continue = false;
    }
    else {
        $('#txt_ownerCity').removeClass("error_msg");
        $('#txt_ownerCity').attr('title', '');
    }

    if ($('#txt_ownerzip').val().trim() == '') {
        $('#txt_ownerzip').addClass("error_msg");
        $('#txt_ownerzip').attr('title', 'Please enter EFIN owner zip code');
        _continue = false;
    }
    else {
        $('#txt_ownerzip').removeClass("error_msg");
        $('#txt_ownerzip').attr('title', '');
    }

    if ($('#txt_ownerzip').val().trim() != '') {
        var zip = $('#txt_ownerzip').val().trim().replace(/-/g, '');
        if (zip.length != 5 && zip.length != 9) {
            $('#txt_ownerzip').addClass("error_msg");
            $('#txt_ownerzip').attr('title', 'Please enter valid zip code');
            _continue = false;
        }
        else {
            $('#txt_ownerzip').removeClass("error_msg");
            $('#txt_ownerzip').attr('title', '');
        }
    }

    if ($('#txt_ownerissuenumber').val().trim() == '') {
        $('#txt_ownerissuenumber').addClass("error_msg");
        $('#txt_ownerissuenumber').attr('title', 'Please enter EFIN Owner state issued ID number');
        _continue = false;
    }
    else {
        $('#txt_ownerissuenumber').removeClass("error_msg");
        $('#txt_ownerissuenumber').attr('title', '');
    }
    
    ////Old Code
    if ($('#txt_companyname').val().trim() == '') {
        $('#txt_companyname').addClass("error_msg");
        $('#txt_companyname').attr('title', 'Please enter Company Name of the ERO office');
        _continue = false;
    }
    else {
        $('#txt_companyname').removeClass("error_msg");
        $('#txt_companyname').attr('title', '');
    }

    if ($('#txt_companyname').val().trim() != '') {
        if ($('#txt_companyname').val().trim().length > 35) {
            $('#txt_companyname').addClass("error_msg");
            $('#txt_companyname').attr('title', 'Company Name max length is 35');
            _continue = false;
        }
        else {
            $('#txt_companyname').removeClass("error_msg");
            $('#txt_companyname').attr('title', '');
        }
    }

    if ($('#txt_officeaddress').val().trim() == '') {
        $('#txt_officeaddress').addClass("error_msg");
        $('#txt_officeaddress').attr('title', 'Please enter ERO office street address');
        _continue = false;
    }
    else {
        $('#txt_officeaddress').removeClass("error_msg");
        $('#txt_officeaddress').attr('title', '');
    }

    if ($('#txt_officeaddress').val().trim() != '') {
        if (isPOExist($('#txt_officeaddress').val())) {
            $('#txt_officeaddress').addClass("error_msg");
            $('#txt_officeaddress').attr('title', 'Office Address should not contain PO');
            _continue = false;
        }
        else {
            $('#txt_officeaddress').removeClass("error_msg");
            $('#txt_officeaddress').attr('title', '');
        }
    }

    if ($('#txt_officeaddress').val().trim() != '') {
        if ($('#txt_officeaddress').val().trim().length > 40) {
            $('#txt_officeaddress').addClass("error_msg");
            $('#txt_officeaddress').attr('title', 'Address max length is 40');
            _continue = false;
        }
        else {
            $('#txt_officeaddress').removeClass("error_msg");
            $('#txt_officeaddress').attr('title', '');
        }
    }

    if ($('#txt_officecity').val().trim() == '') {
        $('#txt_officecity').addClass("error_msg");
        $('#txt_officecity').attr('title', 'Please enter ERO office city');
        _continue = false;
    }
    else {
        $('#txt_officecity').removeClass("error_msg");
        $('#txt_officecity').attr('title', '');
    }

    if ($('#txt_officezip').val().trim() == '') {
        $('#txt_officezip').addClass("error_msg");
        $('#txt_officezip').attr('title', 'Please enter ERO office zip');
        _continue = false;
    }
    else {
        $('#txt_officezip').removeClass("error_msg");
        $('#txt_officezip').attr('title', '');
    }

    if ($('#txt_officezip').val().trim() != '') {
        var zip = $('#txt_officezip').val().trim().replace(/-/g, '');
        if (zip.length != 5 && zip.length != 9) {
            $('#txt_officezip').addClass("error_msg");
            $('#txt_officezip').attr('title', 'Please enter valid zip code');
            _continue = false;
        }
        else {
            $('#txt_officezip').removeClass("error_msg");
            $('#txt_officezip').attr('title', '');
        }
    }

    if ($('#txt_officephone').val().trim() == '') {
        $('#txt_officephone').addClass("error_msg");
        $('#txt_officephone').attr('title', 'Please enter ERO office phone');
        _continue = false;
    }
    else {
        $('#txt_officephone').removeClass("error_msg");
        $('#txt_officephone').attr('title', '');
    }

    if ($('#txt_officephone').val().trim() != '') {
        var zip = $('#txt_officephone').val().trim().replace(/-/g, '');
        if (0 < zip.indexOf(0) > 4 || 0 < zip.indexOf(1) > 4 || zip.length < 10) {
            $('#txt_officephone').addClass("error_msg");
            $('#txt_officephone').attr('title', 'Please enter valid Phone Number');
            _continue = false;
        }
        else {
            $('#txt_officephone').removeClass("error_msg");
            $('#txt_officephone').attr('title', '');
        }
    }

    if ($('#txt_mailaddress').val().trim() == '') {
        $('#txt_mailaddress').addClass("error_msg");
        $('#txt_mailaddress').attr('title', 'Please enter ERO mailing street address');
        _continue = false;
    }
    else {
        $('#txt_mailaddress').removeClass("error_msg");
        $('#txt_mailaddress').attr('title', '');
    }

    if ($('#txt_mailaddress').val().trim() != '') {
        if (isPOExist($('#txt_mailaddress').val())) {
            $('#txt_mailaddress').addClass("error_msg");
            $('#txt_mailaddress').attr('title', 'Mailing Address should not contain PO');
            _continue = false;
        }
        else {
            $('#txt_mailaddress').removeClass("error_msg");
            $('#txt_mailaddress').attr('title', '');
        }
    }

    if ($('#txt_mailaddress').val().trim() != '') {
        if ($('#txt_mailaddress').val().trim().length > 40) {
            $('#txt_mailaddress').addClass("error_msg");
            $('#txt_mailaddress').attr('title', 'Address max length is 40');
            _continue = false;
        }
        else {
            $('#txt_mailaddress').removeClass("error_msg");
            $('#txt_mailaddress').attr('title', '');
        }
    }

    if ($('#txt_mailcity').val().trim() == '') {
        $('#txt_mailcity').addClass("error_msg");
        $('#txt_mailcity').attr('title', 'Please enter ERO mailing city');
        _continue = false;
    }
    else {
        $('#txt_mailcity').removeClass("error_msg");
        $('#txt_mailcity').attr('title', '');
    }

    if ($('#txt_mailzip').val().trim() == '') {
        $('#txt_mailzip').addClass("error_msg");
        $('#txt_mailzip').attr('title', 'Please enter ERO mailing zip code');
        _continue = false;
    }
    else {
        $('#txt_mailzip').removeClass("error_msg");
        $('#txt_mailzip').attr('title', '');
    }

    if ($('#txt_mailzip').val().trim() != '') {
        var zip = $('#txt_mailzip').val().trim().replace(/-/g, '');
        if (zip.length != 5 && zip.length != 9) {
            $('#txt_mailzip').addClass("error_msg");
            $('#txt_mailzip').attr('title', 'Please enter valid zip code');
            _continue = false;
        }
        else {
            $('#txt_mailzip').removeClass("error_msg");
            $('#txt_mailzip').attr('title', '');
        }
    }

    if ($('#txt_shipaddress').val().trim() == '') {
        $('#txt_shipaddress').addClass("error_msg");
        $('#txt_shipaddress').attr('title', 'Please enter ERO shipping address');
        _continue = false;
    }
    else {
        $('#txt_shipaddress').removeClass("error_msg");
        $('#txt_shipaddress').attr('title', '');
    }

    if ($('#txt_shipaddress').val().trim() != '') {
        if (isPOExist($('#txt_shipaddress').val())) {
            $('#txt_shipaddress').addClass("error_msg");
            $('#txt_shipaddress').attr('title', 'Shipping Address should not contain PO');
            _continue = false;
        }
        else {
            $('#txt_shipaddress').removeClass("error_msg");
            $('#txt_shipaddress').attr('title', '');
        }
    }

    if ($('#txt_shipaddress').val().trim() != '') {
        if ($('#txt_shipaddress').val().trim().length > 40) {
            $('#txt_shipaddress').addClass("error_msg");
            $('#txt_shipaddress').attr('title', 'Address max length is 40');
            _continue = false;
        }
        else {
            $('#txt_shipaddress').removeClass("error_msg");
            $('#txt_shipaddress').attr('title', '');
        }
    }

    if ($('#txt_shipcity').val().trim() == '') {
        $('#txt_shipcity').addClass("error_msg");
        $('#txt_shipcity').attr('title', 'Please enter ERO shipping city');
        _continue = false;
    }
    else {
        $('#txt_shipcity').removeClass("error_msg");
        $('#txt_shipcity').attr('title', '');
    }

    if ($('#txt_shipzip').val().trim() == '') {
        $('#txt_shipzip').addClass("error_msg");
        $('#txt_shipzip').attr('title', 'Please enter ERO Shipping Zip');
        _continue = false;
    }
    else {
        $('#txt_shipzip').removeClass("error_msg");
        $('#txt_shipzip').attr('title', '');
    }

    if ($('#txt_shipzip').val().trim() != '') {
        var zip = $('#txt_shipzip').val().trim().replace(/-/g, '');
        if (zip.length != 5 && zip.length != 9) {
            $('#txt_shipzip').addClass("error_msg");
            $('#txt_shipzip').attr('title', 'Please enter valid zip code');
            _continue = false;
        }
        else {
            $('#txt_shipzip').removeClass("error_msg");
            $('#txt_shipzip').attr('title', '');
        }
    }

    //if ($('#txt_IRSaddress').val().trim() == '') {
    //    $('#txt_IRSaddress').addClass("error_msg");
    //    $('#txt_IRSaddress').attr('title', 'Please enter Street Address ERO used to obtain EFIN from IRS');
    //    _continue = false;
    //}
    //else {
    //    $('#txt_IRSaddress').removeClass("error_msg");
    //    $('#txt_IRSaddress').attr('title', '');
    //}

    //if ($('#txt_IRScity').val().trim() == '') {
    //    $('#txt_IRScity').addClass("error_msg");
    //    $('#txt_IRScity').attr('title', 'Please enter City of address ERO used to obtain EFIN from IRS');
    //    _continue = false;
    //}
    //else {
    //    $('#txt_IRScity').removeClass("error_msg");
    //    $('#txt_IRScity').attr('title', '');
    //}

    //if ($('#txt_IRSzip').val().trim() == '') {
    //    $('#txt_IRSzip').addClass("error_msg");
    //    $('#txt_IRSzip').attr('title', 'Please enter Zip Code of address used to obtain EFIN from IRS');
    //    _continue = false;
    //}
    //else {
    //    $('#txt_IRSzip').removeClass("error_msg");
    //    $('#txt_IRSzip').attr('title', '');
    //}

    if ($('#txt_pyvolume').val().trim() == '') {
        $('#txt_pyvolume').addClass("error_msg");
        $('#txt_pyvolume').attr('title', 'Please enter Previous year volume in whole number');
        _continue = false;
    }
    else {
        $('#txt_pyvolume').removeClass("error_msg");
        $('#txt_pyvolume').attr('title', '');
    }

    if ($('#txt_cyvolume').val().trim() == '') {
        $('#txt_cyvolume').addClass("error_msg");
        $('#txt_cyvolume').attr('title', 'Please enter Expected current year volume in whole number');
        _continue = false;
    }
    else {
        $('#txt_cyvolume').removeClass("error_msg");
        $('#txt_cyvolume').attr('title', '');
    }

    if ($('#ddl_prevbank').val() == '' || $('#ddl_prevbank').val() == undefined || $('#ddl_prevbank').val() == 0) {
        $('#ddl_prevbank').addClass("error_msg");
        $('#ddl_prevbank').attr('title', 'Please enter Previous year bank name');
        _continue = false;
    }
    else {
        $('#ddl_prevbank').removeClass("error_msg");
        $('#ddl_prevbank').attr('title', '');
    }

    //if ($('#txt_businessowner').val().trim() == '') {
    //    $('#txt_businessowner').addClass("error_msg");
    //    $('#txt_businessowner').attr('title', 'Please enter A collection of business Owners');
    //    _continue = false;
    //}
    //else {
    //    $('#txt_businessowner').removeClass("error_msg");
    //    $('#txt_businessowner').attr('title', '');
    //}

    //if ($('#txt_nonbusinessowner').val().trim() == '') {
    //    $('#txt_nonbusinessowner').addClass("error_msg");
    //    $('#txt_nonbusinessowner').attr('title', 'Please enter A collection of contacts that are neither the EFIN owner nor the Business Owner');
    //    _continue = false;
    //}
    //else {
    //    $('#txt_nonbusinessowner').removeClass("error_msg");
    //    $('#txt_nonbusinessowner').attr('title', '');
    //}

    if ($('#txt_noofyears').val().trim() == '') {
        $('#txt_noofyears').addClass("error_msg");
        $('#txt_noofyears').attr('title', 'Please enter Number of years experience the owner has with filing');
        _continue = false;
    }
    else {
        $('#txt_noofyears').removeClass("error_msg");
        $('#txt_noofyears').attr('title', '');
    }

    //if ($('#txt_businessein').val().trim() == '') {
    //    $('#txt_businessein').addClass("error_msg");
    //    $('#txt_businessein').attr('title', 'Please enter Business EIN');
    //    _continue = false;
    //}
    //else {
    //    $('#txt_businessein').removeClass("error_msg");
    //    $('#txt_businessein').attr('title', '');
    //}

    if ($('#txt_businessein').val().trim() != '') {
        var zip = $('#txt_businessein').val().trim();
        if (zip.length < 9) {
            $('#txt_businessein').addClass("error_msg");
            $('#txt_businessein').attr('title', 'Please enter 9 digit valid Business EIN');
            _continue = false;
        }
        else {
            $('#txt_businessein').removeClass("error_msg");
            $('#txt_businessein').attr('title', '');
        }
    }

    //if ($('#txt_ownerstitle').val().trim() == '') {
    //    $('#txt_ownerstitle').addClass("error_msg");
    //    $('#txt_ownerstitle').attr('title', 'Please enter EFIN Owners title');
    //    _continue = false;
    //}
    //else {
    //    $('#txt_ownerstitle').removeClass("error_msg");
    //    $('#txt_ownerstitle').attr('title', '');
    //}

    if ($('#txt_routingnumber').val().trim() == '') {
        $('#txt_routingnumber').addClass("error_msg");
        $('#txt_routingnumber').attr('title', 'Please enter Bank routing number');
        _continue = false;
    }
    else {
        $('#txt_routingnumber').removeClass("error_msg");
        $('#txt_routingnumber').attr('title', '');
    }

    if ($('#txt_confirmroutingnumber').val().trim() == '') {
        $('#txt_confirmroutingnumber').addClass("error_msg");
        $('#txt_confirmroutingnumber').attr('title', 'Please enter Confirm Bank routing number');
        _continue = false;
    }
    else {
        $('#txt_confirmroutingnumber').removeClass("error_msg");
        $('#txt_confirmroutingnumber').attr('title', '');
    }

    if ($('#txt_confirmroutingnumber').val().trim() != '' && $('#txt_routingnumber').val() != '') {
        if ($('#txt_confirmroutingnumber').val() != $('#txt_routingnumber').val()) {
            $('#txt_confirmroutingnumber').addClass("error_msg");
            $('#txt_confirmroutingnumber').attr('title', 'Bank routing number is mismatching.');
            _continue = false;
        }
        else {
            $('#txt_confirmroutingnumber').removeClass("error_msg");
            $('#txt_confirmroutingnumber').attr('title', '');
        }
    }

    if ($('#txt_accountname').val().trim() == '') {
        $('#txt_accountname').addClass("error_msg");
        $('#txt_accountname').attr('title', 'Please enter Account Name');
        _continue = false;
    }
    else {
        $('#txt_accountname').removeClass("error_msg");
        $('#txt_accountname').attr('title', '');
    }

    if ($('#txt_bankname').val().trim() == '') {
        $('#txt_bankname').addClass("error_msg");
        $('#txt_bankname').attr('title', 'Please enter Bank Name');
        _continue = false;
    }
    else {
        $('#txt_bankname').removeClass("error_msg");
        $('#txt_bankname').attr('title', '');
    }

    if ($('#txt_bankaccountno').val().trim() == '') {
        $('#txt_bankaccountno').addClass("error_msg");
        $('#txt_bankaccountno').attr('title', 'Please enter Bank account number');
        _continue = false;
    }
    else {
        $('#txt_bankaccountno').removeClass("error_msg");
        $('#txt_bankaccountno').attr('title', '');
    }

    if ($('#txt_bankaccountno').val().trim() != '') {
        var dan = parseInt($('#txt_bankaccountno').val());
        if (dan <= 0) {
            $('#txt_bankaccountno').addClass("error_msg");
            $('#txt_bankaccountno').attr('title', 'Please enter valid Bank Account Number');
            _continue = false;
        }
        else {
            $('#txt_bankaccountno').removeClass("error_msg");
            $('#txt_bankaccountno').attr('title', '');
        }
    }

    if ($('#txt_confirmbankaccountno').val().trim() == '') {
        $('#txt_confirmbankaccountno').addClass("error_msg");
        $('#txt_confirmbankaccountno').attr('title', 'Please enter Account Name');
        _continue = false;
    }
    else {
        $('#txt_confirmbankaccountno').removeClass("error_msg");
        $('#txt_confirmbankaccountno').attr('title', '');
    }

    if ($('#txt_bankaccountno').val().trim() != '' && $('#txt_confirmbankaccountno').val() != '') {
        if ($('#txt_confirmbankaccountno').val() != $('#txt_bankaccountno').val()) {
            $('#txt_confirmbankaccountno').addClass("error_msg");
            $('#txt_confirmbankaccountno').attr('title', 'Bank Account number is mismatching.');
            _continue = false;
        }
        else {
            $('#txt_confirmbankaccountno').removeClass("error_msg");
            $('#txt_confirmbankaccountno').attr('title', '');
        }
    }

    if ($('#ddl_bankaccounttype').val() == '' || $('#ddl_bankaccounttype').val() == 0 || $('#ddl_bankaccounttype').val() == undefined) {
        $('#ddl_bankaccounttype').addClass("error_msg");
        $('#ddl_bankaccounttype').attr('title', 'Please select Bank Account Type');
        _continue = false;
    }
    else {
        $('#ddl_bankaccounttype').removeClass("error_msg");
        $('#ddl_bankaccounttype').attr('title', '');
    }

    //if ($('#ddl_prevclient').val() == '' || $('#ddl_prevclient').val() == 0 || $('#ddl_prevclient').val() == undefined) {
    //    $('#ddl_prevclient').addClass("error_msg");
    //    $('#ddl_prevclient').attr('title', 'Please select Was this user a client of yours last year');
    //    _continue = false;
    //}
    //else {
    //    $('#ddl_prevclient').removeClass("error_msg");
    //    $('#ddl_prevclient').attr('title', '');
    //}

    //if ($('#ddl_hasassociated').val() == '' || $('#ddl_hasassociated').val() == 0 || $('#ddl_hasassociated').val() == undefined) {
    //    $('#ddl_hasassociated').addClass("error_msg");
    //    $('#ddl_hasassociated').attr('title', 'Please select Has anyone associated');
    //    _continue = false;
    //}
    //else {
    //    $('#ddl_hasassociated').removeClass("error_msg");
    //    $('#ddl_hasassociated').attr('title', '');
    //}

    if ($('#ddl_corporationtype').val() == '' || $('#ddl_corporationtype').val() == 0 || $('#ddl_corporationtype').val() == undefined) {
        $('#ddl_corporationtype').addClass("error_msg");
        $('#ddl_corporationtype').attr('title', 'Please select Corporation Type of the business');
        _continue = false;
    }
    else {
        $('#ddl_corporationtype').removeClass("error_msg");
        $('#ddl_corporationtype').attr('title', '');
    }

    //if ($('#ddl_IRSstate').val() == '' || $('#ddl_IRSstate').val() == 0 || $('#ddl_IRSstate').val() == undefined) {
    //    $('#ddl_IRSstate').addClass("error_msg");
    //    $('#ddl_IRSstate').attr('title', 'Please select State abbreviation of address used to obtain EFIN from IRS');
    //    _continue = false;
    //}
    //else {
    //    $('#ddl_IRSstate').removeClass("error_msg");
    //    $('#ddl_IRSstate').attr('title', '');
    //}

    if ($('#ddl_shipstate').val() == '' || $('#ddl_shipstate').val() == 0 || $('#ddl_shipstate').val() == undefined) {
        $('#ddl_shipstate').addClass("error_msg");
        $('#ddl_shipstate').attr('title', 'Please select ERO shipping state abbreviation');
        _continue = false;
    }
    else {
        $('#ddl_shipstate').removeClass("error_msg");
        $('#ddl_shipstate').attr('title', '');
    }

    if ($('#ddl_mailstate').val() == '' || $('#ddl_mailstate').val() == 0 || $('#ddl_mailstate').val() == undefined) {
        $('#ddl_mailstate').addClass("error_msg");
        $('#ddl_mailstate').attr('title', 'Please select ERO mailing state abbreviation');
        _continue = false;
    }
    else {
        $('#ddl_mailstate').removeClass("error_msg");
        $('#ddl_mailstate').attr('title', '');
    }

    if ($('#ddl_officestate').val() == '' || $('#ddl_officestate').val() == 0 || $('#ddl_officestate').val() == undefined) {
        $('#ddl_officestate').addClass("error_msg");
        $('#ddl_officestate').attr('title', 'Please select ERO office state abbreviation');
        _continue = false;
    }
    else {
        $('#ddl_officestate').removeClass("error_msg");
        $('#ddl_officestate').attr('title', '');
    }

    if ($('#ddl_issuestate').val() == '' || $('#ddl_issuestate').val() == 0 || $('#ddl_issuestate').val() == undefined) {
        $('#ddl_issuestate').addClass("error_msg");
        $('#ddl_issuestate').attr('title', 'Please select EFIN Owner ID issuing state abbreviation');
        _continue = false;
    }
    else {
        $('#ddl_issuestate').removeClass("error_msg");
        $('#ddl_issuestate').attr('title', '');
    }

    if ($('#ddl_ownerstate').val() == '' || $('#ddl_ownerstate').val() == 0 || $('#ddl_ownerstate').val() == undefined) {
        $('#ddl_ownerstate').addClass("error_msg");
        $('#ddl_ownerstate').attr('title', 'Please select EFIN owner state abbreviation');
        _continue = false;
    }
    else {
        $('#ddl_ownerstate').removeClass("error_msg");
        $('#ddl_ownerstate').attr('title', '');
    }

    //if (!$('#chk_feeall').prop('checked')) {
    //    $('#dvfeeall').addClass("error_msg");
    //    $('#dvfeeall').attr('title', 'Please select checkbox');
    //    _continue = false;
    //}
    //else {
    //    $('#dvfeeall').removeClass("error_msg");
    //    $('#dvfeeall').attr('title', '');
    //}

    if ($('#txt_electronicfee').val().trim() == '') {
        $('#txt_electronicfee').addClass("error_msg");
        $('#txt_electronicfee').attr('title', 'Please enter Electronic Filing Fee');
        _continue = false;
    }
    else {
        $('#txt_electronicfee').removeClass("error_msg");
        $('#txt_electronicfee').attr('title', '');
    }

    if (!$('#chk_agreebank').prop('checked')) {
        $('#dvtanc').addClass("error_msg");
        $('#dvtanc').attr('title', 'Please select Terms and Conditions');
        _continue = false;
    }
    else {
        $('#dvtanc').removeClass("error_msg");
        $('#dvtanc').attr('title', '');
    }

    //if (!$('#chk_textmessages').prop('checked')) {
    //    $('#dvtextmessages').addClass("error_msg");
    //    $('#dvtextmessages').attr('title', 'Please select');
    //    _continue = false;
    //}
    //else {
    //    $('#dvtextmessages').removeClass("error_msg");
    //    $('#dvtextmessages').attr('title', '');
    //}

    //if (!$('#chk_hasassociated').prop('checked')) {
    //    $('#dvlegalissues').addClass("error_msg");
    //    $('#dvlegalissues').attr('title', 'Please select');
    //    _continue = false;
    //}
    //else {
    //    $('#dvlegalissues').removeClass("error_msg");
    //    $('#dvlegalissues').attr('title', '');
    //}

    if ($('#txt_mainCntFN').val().trim() == '') {
        $('#txt_mainCntFN').addClass("error_msg");
        $('#txt_mainCntFN').attr('title', 'Please enter Main Contact First Name');
        _continue = false;
    }
    else {
        $('#txt_mainCntFN').removeClass("error_msg");
        $('#txt_mainCntFN').attr('title', '');
    }

    if ($('#txt_mainCntLN').val().trim() == '') {
        $('#txt_mainCntLN').addClass("error_msg");
        $('#txt_mainCntLN').attr('title', 'Please enter Main Contact Last Name');
        _continue = false;
    }
    else {
        $('#txt_mainCntLN').removeClass("error_msg");
        $('#txt_mainCntLN').attr('title', '');
    }

    if ($('#txt_contactphone').val().trim() == '') {
        $('#txt_contactphone').addClass("error_msg");
        $('#txt_contactphone').attr('title', 'Please enter Main Contact Phone');
        _continue = false;
    }
    else {
        $('#txt_contactphone').removeClass("error_msg");
        $('#txt_contactphone').attr('title', '');
    }

    if ($('#txt_contactphone').val().trim() != '') {
        var zip = $('#txt_contactphone').val().trim().replace(/-/g, '');
        if (0 < zip.indexOf(0) > 4 || 0 < zip.indexOf(1) > 4 || zip.length < 10) {
            $('#txt_contactphone').addClass("error_msg");
            $('#txt_contactphone').attr('title', 'Please enter valid Phone Number');
            _continue = false;
        }
        else {
            $('#txt_contactphone').removeClass("error_msg");
            $('#txt_contactphone').attr('title', '');
        }
    }

    if (($('#ddl_incorporatestate').val() == '' || $('#ddl_incorporatestate').val() == 0 || $('#ddl_incorporatestate').val() == undefined) && ($('#ddl_corporationtype').val() == 'Corporation' || $('#ddl_corporationtype').val() == 'LLC')) {
        $('#ddl_incorporatestate').addClass("error_msg");
        $('#ddl_incorporatestate').attr('title', 'Please select State of Incorporations');
        _continue = false;
    }
    else {
        $('#ddl_incorporatestate').removeClass("error_msg");
        $('#ddl_incorporatestate').attr('title', '');
    }
    if (EFINOwnerInfo.length == 0) {
        $('#dvOwners').addClass("error_msg");
        $('#dvOwners').attr('title', 'Please add Owners');
        _continue = false;
    }
    else {
        $('#dvOwners').removeClass("error_msg");
        $('#dvOwners').attr('title', '');
    }

    var rtn = $('#txt_routingnumber').val().trim();
    if (rtn != '') {
        var isValidrtn = checkABA(rtn);
        if (!isValidrtn) {
            _continue = false;
            $('#txt_routingnumber').addClass("error_msg");
            $('#txt_routingnumber').attr('title', 'Please enter valid RTN');
        }
    }

    var ownerEmail = $('#txt_owneremail').val().trim();
    if (ownerEmail != '') {
        var isemvalid = validateEmail(ownerEmail);
        if (!isemvalid) {
            $('#txt_owneremail').addClass("error_msg");
            $('#txt_owneremail').attr('title', 'Please enter valid Email Address');
            _continue = false;
        }
        else {
            $('#txt_owneremail').removeClass("error_msg");
            $('#txt_owneremail').attr('title', '');
        }
    }

    $('#dvOwners').removeClass("error_msg");
    if (EFINOwnerInfo.length == 0) {
        $('#dvOwners').addClass("error_msg");
        $('#dvOwners').attr('title', 'Please add owners');
        _continue = false;
    }

    if (EFINOwnerInfo.length > 0) {
        var totalpercentage = 0;
        for (var i = 0; i < EFINOwnerInfo.length; i++) {
            totalpercentage = totalpercentage + parseInt(EFINOwnerInfo[i].PercentageOwned);
        }

        if (totalpercentage != 100) {
            //$("html, body").animate({ scrollTop: 0 }, "slow");
            //error.show();
            //error.append('<p> Percentage Owned should be equal to 100. </p>');
            $('#dvOwners').attr('title', 'Percentage Owned should be equal to 100.');
            $('#dvOwners').addClass("error_msg");
            $('#a_ooinfo').click();
            $('#a_ooinfo').parent('li').addClass('active');
            $('#dvero').addClass('active in');

            $('#a_bankinfo').parent('li').removeClass('active');
            $('#dv_bankinfo').removeClass('active in');
            _continue = false;
        }
    }

    if (!_continue) {
        $("html, body").animate({ scrollTop: 0 }, "slow");
        error.show();
        error.append('<p>  Please correct error(s). </p>');
        if ($('#dvefinowner input.error_msg').length > 0 || $('#dvefinowner select.error_msg').length > 0 || $('#dvefinowner textarea.error_msg').length > 0 || $('#dvefinowner div.error_msg').length > 0) {
            //$('#a_efinowner').click();            
            $('#a_efinowner').parent('li').addClass('active');
            $('#dvefinowner').addClass('active in');

            $('#a_bankinfo').parent('li').removeClass('active');
            $('#dv_bankinfo').removeClass('active in');
        }
        else if ($('#dvero input.error_msg').length > 0 || $('#dvero select.error_msg').length > 0 || $('#dvero textarea.error_msg').length > 0 || $('#dvero div.error_msg').length > 0) {
            //$('#a_ooinfo').click();
            $('#a_ooinfo').parent('li').addClass('active');
            $('#dvero').addClass('active in');

            $('#a_bankinfo').parent('li').removeClass('active');
            $('#dv_bankinfo').removeClass('active in');
        }
        else if ($('#dvfee input.error_msg').length > 0 || $('#dvfee div.error_msg').length > 0 || $('#dvfee textarea.error_msg').length > 0 || $('#dvfee select.error_msg').length > 0) {
            // $('#a_feeinfo').click();
            $('#a_feeinfo').parent('li').addClass('active');
            $('#dvfee').addClass('active in');

            $('#a_bankinfo').parent('li').removeClass('active');
            $('#dv_bankinfo').removeClass('active in');
        }
        else {
            //$('#a_bankinfo').click();
            $('#a_bankinfo').parent('li').addClass('active');
            $('#dv_bankinfo').addClass('active in');
            return;
        }
    }

    if (_continue) {
        var request = {};
        request.BankId = bankid;
        request.OwnerEmail = ownerEmail;
        request.OwnerFirstName = $('#txt_ownerFN').val().trim();
        request.OwnerLastName = $('#txt_ownerLN').val().trim();
        request.OwnerSSN = $('#txt_ownerSSN').val().trim();
        var odob = $('#txt_ownerDOB').val();
        if (odob) {
            if (odob.indexOf('/') > 0)
                request.OwnerDOB = odob;
            else
                request.OwnerDOB = getformattedDate(odob);
        }
        request.OwnerCellPhone = $('#txt_ownercell').val().trim();
        request.OwnerHomePhone = $('#txt_ownerphone').val().trim();
        request.OwnerAddress = $('#txt_ownerAddress').val().trim();
        request.OwnerCity = $('#txt_ownerCity').val().trim();
        request.OwnerState = $('#ddl_ownerstate').val();
        request.OwnerZipCode = $('#txt_ownerzip').val().trim();
        request.OwnerStateIssuedIdNumber = $('#txt_ownerissuenumber').val().trim();
        request.OwnerIssuingState = $('#ddl_issuestate').val();
        request.EROOfficeName = $('#txt_companyname').val().trim();;
        request.EROOfficeAddress = $('#txt_officeaddress').val().trim();;
        request.EROOfficeCity = $('#txt_officecity').val().trim();;
        request.EROOfficeState = $('#ddl_officestate').val();
        request.EROOfficeZipCoce = $('#txt_officezip').val().trim();;
        request.EROOfficePhone = $('#txt_officephone').val().trim();;
        request.EROMaillingAddress = $('#txt_mailaddress').val().trim();;
        request.EROMailingCity = $('#txt_mailcity').val().trim();;
        request.EROMailingState = $('#ddl_mailstate').val();
        request.EROMailingZipcode = $('#txt_mailzip').val().trim();;
        request.EROShippingAddress = $('#txt_shipaddress').val().trim();;
        request.EROShippingCity = $('#txt_shipcity').val().trim();;
        request.EROShippingState = $('#ddl_shipstate').val();
        request.EROShippingZip = $('#txt_shipzip').val().trim();;
        request.IRSAddress = $('#txt_IRSaddress').val().trim();;
        request.IRSCity = $('#txt_IRScity').val().trim();;
        request.IRSState = $('#ddl_IRSstate').val();
        request.IRSZipcode = $('#txt_IRSzip').val().trim();;
        request.PreviousYearVolume = $('#txt_pyvolume').val().trim();;
        request.ExpectedCurrentYearVolume = $('#txt_cyvolume').val().trim();;
        request.PreviousBankName = $('#ddl_prevbank').val();
        request.CorporationType = $('#ddl_corporationtype').val();
        request.CollectionofBusinessOwners = '';
        request.CollectionOfOtherOwners = '';
        request.NoofYearsExperience = $('#txt_noofyears').val().trim();;
        request.HasAssociatedWithVictims = $('#ddl_hasassociated').val();
        request.BusinessFederalIDNumber = '';
        request.BusinessEIN = $('#txt_businessein').val();
        request.EFINOwnersSite = '';
        request.IsLastYearClient = $('#ddl_prevclient').val();
        request.BankRoutingNumber = $('#txt_routingnumber').val();
        request.BankAccountNumber = $('#txt_bankaccountno').val();
        request.BankAccountType = $('#ddl_bankaccounttype').val();

        request.RAEFINOwnerInfo = EFINOwnerInfo;
        request.OwnerTitle = $('#txt_ownertitle').val();
        request.SbFeeall = 'X';
        request.TransmissionAddon = $('#txt_addonfee').val();
        request.SbFee = $('#txt_sbfee').val();
        request.ElectronicFee = $('#txt_electronicfee').val();
        request.AgreeTandC = true;
        request.BankName = $('#txt_bankname').val();
        request.AccountName = $('#txt_accountname').val();
        request.MainContactFirstName = $('#txt_mainCntFN').val();
        request.MainContactLastName = $('#txt_mainCntLN').val();
        request.MainContactPhone = $('#txt_contactphone').val();
        request.TextMessages = $('#chk_textmessages').prop('checked');
        request.LegalIssues = $('#chk_hasassociated').val();
        request.StateOfIncorporation = $('#ddl_incorporatestate').val();

        request.UserId = $('#UserId').val();
        request.CustomerId = $('#UserId').val();

        var Cid = getUrlVars()["Id"];
        var entityid = $('#entityid').val();
        if (Cid) {
            request.CustomerId = Cid;
            entityid = getUrlVars()["entityid"];
        }
        var saveURI = '/api/EnrollmentBankSelection/SaveRABankEnrollment';
        ajaxHelper(saveURI, 'POST', request, false).done(function (data, status) {
            if (data) {
                if (data.Status) {
                    $("html, body").animate({ scrollTop: 0 }, "slow");
                    success.show();
                    success.append('<p> Bank Enrollment details saved successfully.</p>');

                    //$('#site0feeb0fe-d0e7-4370-8733-dd5f7d2041fc').addClass('done');

                    SaveConfigStatusActive('done', bankid);
                    UpdateOfficeManagement(request.CustomerId)
                    if (type == 1) {

                        //if (Cid)
                        //    window.location.href = '../Enrollment/enrollmentsummary?Id=' + Cid;
                        //else
                        //    window.location.href = '../Enrollment/enrollmentsummary';

                        if ($('#entityid').val() != $('#myentityid').val()) {
                            if (entityid == $('#Entity_SO').val() || entityid == $('#Entity_SOME').val() || entityid == $('#Entity_SOME_SS').val()) {
                                var feeuri = '/api/EnrollmentBankSelection/getAddonSelection?CustomerId=' + request.CustomerId + '&bankid=' + bankid;
                                ajaxHelper(feeuri, 'GET').done(function (res) {
                                    if (res) {
                                        window.location.href = '/Enrollment/EnrollmentFeeReimbursement?Id=' + request.CustomerId + '&entitydisplayid=' + $('#myentitydisplayid').val() + '&ParentId=' + $('#myparentid').val() + '&entityid=' + $('#myentityid').val() + '&ptype=enrollment&bankid=' + bankid;
                                    }
                                    else {
                                        if ($('#site0eda5d25-591c-4e01-a845-fb580572cff5').length > 0) {
                                            window.location.href = '/PaymentOptions/efile?Id=' + request.CustomerId + '&entitydisplayid=' + $('#myentitydisplayid').val() + '&ParentId=' + $('#myparentid').val() + '&entityid=' + $('#myentityid').val() + '&ptype=enrollment&bankid=' + bankid;
                                        }
                                        else if ($('#site0eda5d25-591c-4e01-a845-fb580572cfe8').length > 0) {
                                            window.location.href = '/PaymentOptions/OutstandingBalance?Id=' + request.CustomerId + '&entitydisplayid=' + $('#myentitydisplayid').val() + '&ParentId=' + $('#myparentid').val() + '&entityid=' + $('#myentityid').val() + '&ptype=enrollment&bankid=' + bankid;
                                        }
                                        else
                                            window.location.href = '/Enrollment/enrollmentsummary?Id=' + request.CustomerId + '&entitydisplayid=' + $('#myentitydisplayid').val() + '&ParentId=' + $('#myparentid').val() + '&entityid=' + $('#myentityid').val() + '&ptype=enrollment&bankid=' + bankid;
                                    }
                                })
                            }
                            else {
                                if ($('#site0eda5d25-591c-4e01-a845-fb580572cff5').length > 0) {
                                    window.location.href = '/PaymentOptions/efile?Id=' + request.CustomerId + '&entitydisplayid=' + $('#myentitydisplayid').val() + '&ParentId=' + $('#myparentid').val() + '&entityid=' + $('#myentityid').val() + '&ptype=enrollment&bankid=' + bankid;
                                }
                                else if ($('#site0eda5d25-591c-4e01-a845-fb580572cfe8').length > 0) {
                                    window.location.href = '/PaymentOptions/OutstandingBalance?Id=' + request.CustomerId + '&entitydisplayid=' + $('#myentitydisplayid').val() + '&ParentId=' + $('#myparentid').val() + '&entityid=' + $('#myentityid').val() + '&ptype=enrollment&bankid=' + bankid;
                                }
                                else
                                    window.location.href = '/Enrollment/enrollmentsummary?Id=' + request.CustomerId + '&entitydisplayid=' + $('#myentitydisplayid').val() + '&ParentId=' + $('#myparentid').val() + '&entityid=' + $('#myentityid').val() + '&ptype=enrollment&bankid=' + bankid;
                            }
                        }
                        else {
                            if (entityid == $('#Entity_SO').val() || entityid == $('#Entity_SOME').val() || entityid == $('#Entity_SOME_SS').val()) {
                                var feeuri = '/api/EnrollmentBankSelection/getAddonSelection?CustomerId=' + request.CustomerId + '&bankid=' + bankid;
                                ajaxHelper(feeuri, 'GET').done(function (res) {
                                    if (res) {
                                        window.location.href = '/Enrollment/EnrollmentFeeReimbursement?bankid=' + bankid;
                                    }
                                    else {
                                        if ($('#site0eda5d25-591c-4e01-a845-fb580572cff5').length > 0) {
                                            window.location.href = '/PaymentOptions/efile?bankid=' + bankid;
                                        }
                                        else if ($('#site0eda5d25-591c-4e01-a845-fb580572cfe8').length > 0) {
                                            window.location.href = '/PaymentOptions/OutstandingBalance?bankid=' + bankid;
                                        }
                                        else
                                            window.location.href = '/Enrollment/enrollmentsummary?bankid=' + bankid;
                                    }
                                })
                            }
                            else {
                                if ($('#site0eda5d25-591c-4e01-a845-fb580572cff5').length > 0) {
                                    window.location.href = '/PaymentOptions/efile?bankid=' + bankid;
                                }
                                else if ($('#site0eda5d25-591c-4e01-a845-fb580572cfe8').length > 0) {
                                    window.location.href = '/PaymentOptions/OutstandingBalance?bankid=' + bankid;
                                }
                                else
                                    window.location.href = '/Enrollment/enrollmentsummary?bankid=' + bankid;
                            }
                        }
                    }


                    //if (type == 1) {
                    //    if ($('#entityid').val() != $('#myentityid').val()) {
                    //        if (entityid == $('#Entity_SO').val()) {
                    //            window.location.href = '/Enrollment/EnrollmentFeeReimbursement?Id=' + Cid + '&entitydisplayid=' + $('#myentitydisplayid').val() + '&ParentId=' + $('#myparentid').val() + '&entityid=' + $('#myentityid').val() + '&ptype=enrollment';
                    //        }
                    //        else {
                    //            window.location.href = '/Enrollment/enrollmentsummary?Id=' + Cid + '&entitydisplayid=' + $('#myentitydisplayid').val() + '&ParentId=' + $('#myparentid').val() + '&entityid=' + $('#myentityid').val() + '&ptype=enrollment';
                    //        }
                    //    }
                    //    else {
                    //        if (entityid == $('#Entity_SO').val()) {
                    //            window.location.href = '/Enrollment/EnrollmentFeeReimbursement';
                    //        }
                    //        else {
                    //            window.location.href = '/Enrollment/enrollmentsummary';
                    //        }
                    //    }
                    //}

                    if (type != 1)
                        getConfigStatus();
                }
                else {
                    $("html, body").animate({ scrollTop: 0 }, "slow");
                    error.show();
                    error.append('<p> Details not saved. </p>');
                    $.each(data.Messages, function (item, value) {
                        error.append('<p> ' + value + ' </p>');
                    })
                }
            }
            else {
                $("html, body").animate({ scrollTop: 0 }, "slow");
                error.show();
                error.append('<p> Details not saved. </p>');
            }
        });
    }
}

function saveNextRBBankEnrollment(type, level) {

    if (bankstatus == 'APR' || bankstatus == 'SUB' || bankstatus == 'PEN')
        return;

    var bankid = getUrlVars()["bankid"];
    var _continue = true;
    ResetSuccessError();

    var email = $('#txt_email').val().trim();
    //_continue = validateEmail(email);
    //if (!_continue) {
    //    $('#txt_email').addClass("error_msg");
    //    $('#txt_email').attr('title', 'Please enter valid Email Address');
    //    $("html, body").animate({ scrollTop: 0 }, "slow");
    //    error.show();
    //    error.append('<p>  Please correct error </p>');
    //    return;
    //}
    if ($('#txt_efinowneremail').val().trim() != '') {
        var email1 = $('#txt_efinowneremail').val().trim();
        _continue = validateEmail(email1);
        if (!_continue) {
            $('#txt_efinowneremail').addClass("error_msg");
            $('#txt_efinowneremail').attr('title', 'Please enter valid Email Address');
            $("html, body").animate({ scrollTop: 0 }, "slow");
            error.show();
            error.append('<p>  Please correct error </p>');
            return;
        } else {
            $('#txt_efinowneremail').removeClass("error_msg");
            $('#txt_efinowneremail').attr('title', '');
        }
    }

    var url = $('#txt_website').val().trim();
    //if (url != '') {
    //    _continue = ValidateWebURL(url);
    //    if (!_continue) {
    //        $('#txt_website').addClass("error_msg");
    //        $('#txt_website').attr('title', 'Please enter valid Website Address');
    //        $("html, body").animate({ scrollTop: 0 }, "slow");
    //        error.show();
    //        error.append('<p>  Please correct error </p>');
    //        return;
    //    }
    //}
    //if (level == 6) {
    var rtn = $('#txt_bankroutingno').val().trim();
    //    _continue = checkABA(rtn);
    //    if (!_continue) {
    //        $('#txt_bankroutingno').addClass("error_msg");
    //        $('#txt_bankroutingno').attr('title', 'Please enter valid Bank Routing Number');
    //        $("html, body").animate({ scrollTop: 0 }, "slow");
    //        error.show();
    //        error.append('<p>  Please correct error </p>');
    //        return;
    //    }
    //}
    var request = {};
    request.BankId = bankid;
    request.OfficeName = $('#txt_officename').val();
    request.OfficePhysicalAddress = $('#txt_officeaddress').val();
    request.OfficePhysicalCity = $('#txt_officecity').val();
    request.OfficePhysicalState = $('#ddl_officestate').val();
    request.OfficePhysicalZip = $('#txt_officezip').val();
    request.OfficeContactFirstName = $('#txt_offcntFN').val();
    request.OfficeContactLastName = $('#txt_offcntLN').val();
    request.OfficeContactSSN = $('#txt_offcntSSN').val();
    request.OfficePhoneNumber = $('#txt_officephone').val();
    request.CellPhoneNumber = $('#txt_cellphone').val();
    request.FAXNumber = $('#txt_faxnumber').val();
    request.EmailAddress = $('#txt_email').val();

    var OfficeContactDOB = $('#txt_offmngrDOB').val();
    if (OfficeContactDOB) {
        if (OfficeContactDOB.indexOf('/') > 0 && isValidDateFormat(OfficeContactDOB))
            request.OfficeContactDOB = OfficeContactDOB;
        else if (isValidDateFormat(getformattedDate(OfficeContactDOB)))
            request.OfficeContactDOB = getformattedDate(OfficeContactDOB);
    }
    //request.OfficeContactDOB = $('#txt_offContactDOB').val();
    request.OfficeManagerPhone = $('#txt_offmngrPhoneNo').val();
    request.OfficeManagerCellNo = $('#txt_offmngeCellPhone').val();
    request.OfficeManagerEmail = $('#txt_offManagerEmail').val();
    request.OfficeManagerFirstName = $('#txt_offmngrFN').val();
    request.OfficeManageLastName = $('#txt_offmngeLN').val();
    request.OfficeManagerSSN = $('#txt_offmngrSSN').val();

    var OfficeManagerDOB = $('#txt_offmngrDOB').val();
    if (OfficeManagerDOB) {
        if (OfficeManagerDOB.indexOf('/') > 0 && isValidDateFormat(OfficeManagerDOB))
            request.OfficeManagerDOB = OfficeManagerDOB;
        else if (isValidDateFormat(getformattedDate(OfficeManagerDOB)))
            request.OfficeManagerDOB = getformattedDate(OfficeManagerDOB);
    }
    // request.OfficeManagerDOB = $('#txt_offmngrDOB').val();
    request.AltOfficeContact1FirstName = $('#txt_altocFN1').val();
    request.AltOfficeContact1LastName = $('#txt_altocLN1').val();
    request.AltOfficeContact1Email = $('#txt_altoc1Email').val();
    request.AltOfficeContact1SSn = $('#txt_altoc1SSN').val();
    request.AltOfficeContact2FirstName = $('#txt_altocFN2').val();
    request.AltOfficeContact2LastName = $('#txt_altocLN2').val();
    request.AltOfficeContact2Email = $('#txt_altoc2Email').val();
    request.AltOfficeContact2SSn = $('#txt_altoc2SSN').val();
    request.AltOfficePhysicalAddress = $('#txt_altoffadd').val();
    request.AltOfficePhysicalAddress2 = $('#txt_altoffadd2').val();
    request.AltOfficePhysicalCity = $('#txt_altoffcity').val();
    request.AltOfficePhysicalState = $('#ddl_altofficestate').val();
    request.AltOfficePhysicalZipcode = $('#txt_altoffzip').val();
    request.MailingAddress = $('#txt_mailaddress').val();
    request.MailingCity = $('#txt_mailcity').val();
    request.MailingState = $('#ddl_mailstate').val();
    request.MailingZip = $('#txt_mailzip').val();
    request.FulfillmentShippingAddress = $('#txt_fulladdress').val();
    request.FulfillmentShippingCity = $('#txt_fullcity').val();
    request.FulfillmentShippingState = $('#ddl_fullstate').val();
    request.FulfillmentShippingZip = $('#txt_fullzip').val();
    request.WebsiteAddress = $('#txt_website').val();
    request.YearsinBusiness = $('#txt_yearsinbusiness').val();
    request.NoofBankProductsLastYear = $('#txt_noofbankprdcts').val();
    request.PreviousBankProductFacilitator = $('#ddl_prevbank').val();
    request.ActualNoofBankProductsLastYear = $('#txt_actnoofbankprdcts').val();
    request.OwnerFirstName = $('#txt_ownerFN').val();
    request.OwnerLastName = $('#txt_ownerLN').val();
    request.OwnerSSN = $('#txt_ownerSSN').val();
    var odob = $('#txt_ownerDOB').val();
    if (odob) {
        if (odob.indexOf('/') > 0 && isValidDateFormat(odob))
            request.OwnerDOB = odob;
        else if (isValidDateFormat(getformattedDate(odob)))
            request.OwnerDOB = getformattedDate(odob);
    }
    request.OwnerHomePhone = $('#txt_ownerphone').val();
    request.OwnerAddress = $('#txt_owneraddress').val();
    request.OwnerCity = $('#txt_ownercity').val();
    request.OwnerState = $('#ddl_ownerstate').val();
    request.OwnerZip = $('#txt_ownerzip').val();
    request.LegarEntityStatus = $('#ddl_legalentity').val();
    request.LLCMembershipRegistration = $('#ddl_llcmembership').val();
    request.BusinessName = $('#txt_businessname').val();
    request.BusinessEIN = $('#txt_businessEIN').val();
    request.BusinessIncorporation = $('#txt_businessdate').val();
    request.EFINOwnerFirstName = $('#txt_efinownerFN').val();
    request.EFINOwnerLastName = $('#txt_efinownerLN').val();
    request.EFINOwnerSSN = $('#txt_efinownerSSN').val();


    var EFINOwnerDOB = $('#txt_efinownerDOB').val();
    if (EFINOwnerDOB) {
        if (EFINOwnerDOB.indexOf('/') > 0 && isValidDateFormat(EFINOwnerDOB))
            request.EFINOwnerDOB = EFINOwnerDOB;
        else if (isValidDateFormat(getformattedDate(EFINOwnerDOB)))
            request.EFINOwnerDOB = getformattedDate(EFINOwnerDOB);
    }
    //request.EFINOwnerDOB = $('#txt_efinownerDOB').val();
    request.IsMultiOffice = $('#ddl_multioffice').val();
    request.ProductsOffering = $('#ddl_productsoffering').val();
    request.IsOfficeTransmit = $('#ddl_locationtransmit').val();
    request.IsPTIN = $('#ddl_ptin').val();
    request.IsAsPerProcessLaw = $('#ddl_processlaw').val();
    request.IsAsPerComplainceLaw = $('#ddl_complaince').val();
    request.ConsumerLending = $('#ddl_consumerlending').val();
    request.NoofPersoneel = $('#txt_noofpersoneel').val();
    request.AdvertisingApproval = $('#ddl_advertiseapprvl').val();
    request.EROParticipation = $('#ddl_eroparticipation').val();
    request.SPAAmount = $('#txt_spaamount').val();
    request.SPADate = $('#txt_eroagreeddate').val();
    request.RetailPricingMethod = $('#ddl_retailmethod').val();
    request.IsLockedStore_Documents = $('#ddl_lockeddocs').val();
    request.IsLockedStore_Checks = $('#ddl_lockedcardschecks').val();
    request.IsLocked_Office = $('#ddl_lockedoffice').val();
    request.IsLimitAccess = $('#ddl_limitaccess').val();
    request.PlantoDispose = $('#ddl_plandispose').val();
    request.LoginAccesstoEmployees = $('#ddl_logintoemployees').val();
    request.AntivirusRequired = $('#ddl_antivirus').val();
    request.HasFirewall = $('#ddl_firewall').val();
    request.OnlineTraining = $('#ddl_onlinetraining').val();
    request.PasswordRequired = $('#ddl_pwdrqd').val();
    request.EROApplicattionDate = $('#txt_eroapplncmptd').val();
    request.EROReadTAndC = $('#ddl_eroreadrc').val();
    request.CheckingAccountName = $('#txt_accountname').val();
    request.BankRoutingNumber = $('#txt_bankroutingno').val();
    request.BankAccountNumber = $('#txt_bankaccountno').val();
    request.BankAccountType = $('#ddl_accounttype').val();


    request.EFINOwnerTitle = $('#txt_efintitle').val().trim();
    request.EFINOwnerAddress = $('#txt_efinownerAddress').val().trim();
    request.EFINOwnerCity = $('#txt_efinownercity').val().trim();
    request.EFINOwnerState = $('#ddl_efinownerstate').val();
    request.EFINOwnerZip = $('#txt_efinownerZip').val().trim();
    request.EFINOwnerPhone = $('#txt_efinownerphone').val().trim();
    request.EFINOwnerMobile = $('#txt_efinownermobile').val().trim();
    request.EFINOwnerEmail = $('#txt_efinowneremail').val().trim();
    request.EFINOwnerIDNumber = $('#txt_efinowneridnumber').val().trim();
    request.EFINOwnerIDState = $('#ddl_idstate').val();
    request.EFINOwnerEIN = $('#txt_efinownerein').val().trim();
    request.SupportOS = $('#ddl_wifipwd').val();
    request.BankName = $('#txt_bankname').val().trim();
    request.SBFeeonAll = $('#chk_fbfee').prop('checked') ? 'X' : ' ';
    request.SBFee = $('#txt_sbfee').val().trim();
    request.TransimissionAddon = $('#txt_transmitfee').val().trim();
    request.PrepaidCardProgram = $('#ddl_cardprogram').val();
    request.TandC = true;
    request.EntryLevel = level;

    request.UserId = $('#UserId').val();
    request.CustomerId = $('#UserId').val();

    var Cid = getUrlVars()["Id"];
    var entityid = $('#entityid').val();

    if (Cid) {
        request.CustomerId = Cid;
        entityid = getUrlVars()["entityid"];
    }

    var saveURI = '/api/EnrollmentBankSelection/SaveNextRBBankEnrollment';
    ajaxHelper(saveURI, 'POST', request).done(function (data, status) {
        if (data) {
            //$("html, body").animate({ scrollTop: 0 }, "slow");
            ////success.show();
            ////success.append('<p> Bank Enrollment details saved successfully.</p>');
            //$('#site0feeb0fe-d0e7-4370-8733-dd5f7d2041fc').addClass('done');
            ////UpdateOfficeManagement(request.CustomerId)

            //if (type == 1) {
            //    if ($('#entityid').val() != $('#myentityid').val()) {
            //        if (entityid == $('#Entity_SO').val() || entityid == $('#Entity_SOME').val() || entityid == $('#Entity_SOME_SS').val()) {
            //            window.location.href = '/Enrollment/EnrollmentFeeReimbursement?Id=' + request.CustomerId + '&entitydisplayid=' + $('#myentitydisplayid').val() + '&ParentId=' + $('#myparentid').val() + '&entityid=' + $('#myentityid').val() + '&ptype=enrollment';
            //        }
            //        else {
            //            window.location.href = '/Enrollment/enrollmentsummary?Id=' + request.CustomerId + '&entitydisplayid=' + $('#myentitydisplayid').val() + '&ParentId=' + $('#myparentid').val() + '&entityid=' + $('#myentityid').val() + '&ptype=enrollment';
            //        }
            //    }
            //    else {
            //        if (entityid == $('#Entity_SO').val() || entityid == $('#Entity_SOME').val() || entityid == $('#Entity_SOME_SS').val()) {
            //            window.location.href = '/Enrollment/EnrollmentFeeReimbursement';
            //        }
            //        else {
            //            window.location.href = '/Enrollment/enrollmentsummary';
            //        }
            //    }
            //}
            //getConfigStatus();
        }
        else {
            $("html, body").animate({ scrollTop: 0 }, "slow");
            error.show();
            error.append('<p> Details not saved. </p>');
        }
    });
}

function saveNextRABankEnrollment(type, level) {
    if (bankstatus == 'APR' || bankstatus == 'SUB' || bankstatus == 'PEN')
        return;

    var bankid = getUrlVars()["bankid"];
    var _continue = true;
    ResetSuccessError();
    var request = {};
    request.BankId = bankid;
    var ownerEmail = $('#txt_owneremail').val().trim();
    request.OwnerEmail = ownerEmail;
    request.OwnerFirstName = $('#txt_ownerFN').val().trim();
    request.OwnerLastName = $('#txt_ownerLN').val().trim();
    request.OwnerSSN = $('#txt_ownerSSN').val().trim();
    var odob = $('#txt_ownerDOB').val();
    if (odob) {
        if (odob.indexOf('/') > 0 && isValidDateFormat(odob))
            request.OwnerDOB = odob;
        else if (isValidDateFormat(getformattedDate(odob)))
            request.OwnerDOB = getformattedDate(odob);
    }
    //request.OwnerDOB = $('#txt_ownerDOB').val().trim();
    request.OwnerCellPhone = $('#txt_ownercell').val().trim();
    request.OwnerHomePhone = $('#txt_ownerphone').val().trim();
    request.OwnerAddress = $('#txt_ownerAddress').val().trim();
    request.OwnerCity = $('#txt_ownerCity').val().trim();
    request.OwnerState = $('#ddl_ownerstate').val();
    request.OwnerZipCode = $('#txt_ownerzip').val().trim();
    request.OwnerStateIssuedIdNumber = $('#txt_ownerissuenumber').val().trim();
    request.OwnerIssuingState = $('#ddl_issuestate').val();
    request.EROOfficeName = $('#txt_companyname').val().trim();;
    request.EROOfficeAddress = $('#txt_officeaddress').val().trim();;
    request.EROOfficeCity = $('#txt_officecity').val().trim();;
    request.EROOfficeState = $('#ddl_officestate').val();
    request.EROOfficeZipCoce = $('#txt_officezip').val().trim();;
    request.EROOfficePhone = $('#txt_officephone').val().trim();;
    request.EROMaillingAddress = $('#txt_mailaddress').val().trim();;
    request.EROMailingCity = $('#txt_mailcity').val().trim();;
    request.EROMailingState = $('#ddl_mailstate').val();
    request.EROMailingZipcode = $('#txt_mailzip').val().trim();;
    request.EROShippingAddress = $('#txt_shipaddress').val().trim();;
    request.EROShippingCity = $('#txt_shipcity').val().trim();;
    request.EROShippingState = $('#ddl_shipstate').val();
    request.EROShippingZip = $('#txt_shipzip').val().trim();;
    request.IRSAddress = $('#txt_IRSaddress').val().trim();;
    request.IRSCity = $('#txt_IRScity').val().trim();;
    request.IRSState = $('#ddl_IRSstate').val();
    request.IRSZipcode = $('#txt_IRSzip').val().trim();;
    request.PreviousYearVolume = $('#txt_pyvolume').val().trim();;
    request.ExpectedCurrentYearVolume = $('#txt_cyvolume').val().trim();;
    request.PreviousBankName = $('#ddl_prevbank').val();
    request.CorporationType = $('#ddl_corporationtype').val();
    request.CollectionofBusinessOwners = '';
    request.CollectionOfOtherOwners = '';
    request.NoofYearsExperience = $('#txt_noofyears').val().trim();;
    request.HasAssociatedWithVictims = $('#ddl_hasassociated').val();
    request.BusinessFederalIDNumber = '';
    request.BusinessEIN = $('#txt_businessein').val();
    request.EFINOwnersSite = '';
    request.IsLastYearClient = $('#ddl_prevclient').val();
    request.BankRoutingNumber = $('#txt_routingnumber').val();
    request.BankAccountNumber = $('#txt_bankaccountno').val();
    request.BankAccountType = $('#ddl_bankaccounttype').val();
    request.UserId = $('#UserId').val();
    request.CustomerId = $('#UserId').val();
    request.RAEFINOwnerInfo = EFINOwnerInfo;
    request.OwnerTitle = $('#txt_ownertitle').val();
    request.SbFeeall = 'X';
    request.TransmissionAddon = $('#txt_addonfee').val();
    request.SbFee = $('#txt_sbfee').val();
    request.ElectronicFee = $('#txt_electronicfee').val();
    request.AgreeTandC = true;
    request.BankName = $('#txt_bankname').val();
    request.AccountName = $('#txt_accountname').val();
    request.MainContactFirstName = $('#txt_mainCntFN').val();
    request.MainContactLastName = $('#txt_mainCntLN').val();
    request.MainContactPhone = $('#txt_contactphone').val();
    request.TextMessages = $('#chk_textmessages').prop('checked');
    request.LegalIssues = $('#chk_hasassociated').val();
    request.StateOfIncorporation = $('#ddl_incorporatestate').val();
    request.EntryLevel = level;
    var Cid = getUrlVars()["Id"];
    var entityid = $('#entityid').val();
    if (Cid) {
        request.CustomerId = Cid;
        entityid = getUrlVars()["entityid"];
    }
    var saveURI = '/api/EnrollmentBankSelection/SaveNextRABankEnrollment';
    ajaxHelper(saveURI, 'POST', request).done(function (data, status) {
        if (data) {
            //$("html, body").animate({ scrollTop: 0 }, "slow");
            ////success.show();
            ////success.append('<p> Bank Enrollment details saved successfully.</p>');
            //$('#site0feeb0fe-d0e7-4370-8733-dd5f7d2041fc').addClass('done');
            //// UpdateOfficeManagement(request.CustomerId)
            //if (type == 1) {
            //    if ($('#entityid').val() != $('#myentityid').val()) {
            //        if (entityid == $('#Entity_SO').val() || entityid == $('#Entity_SOME').val() || entityid == $('#Entity_SOME_SS').val()) {
            //            window.location.href = '/Enrollment/EnrollmentFeeReimbursement?Id=' + request.CustomerId + '&entitydisplayid=' + $('#myentitydisplayid').val() + '&ParentId=' + $('#myparentid').val() + '&entityid=' + $('#myentityid').val() + '&ptype=enrollment';
            //        }
            //        else {
            //            window.location.href = '/Enrollment/enrollmentsummary?Id=' + request.CustomerId + '&entitydisplayid=' + $('#myentitydisplayid').val() + '&ParentId=' + $('#myparentid').val() + '&entityid=' + $('#myentityid').val() + '&ptype=enrollment';
            //        }
            //    }
            //    else {
            //        if (entityid == $('#Entity_SO').val() || entityid == $('#Entity_SOME').val() || entityid == $('#Entity_SOME_SS').val()) {
            //            window.location.href = '/Enrollment/EnrollmentFeeReimbursement';
            //        }
            //        else {
            //            window.location.href = '/Enrollment/enrollmentsummary';
            //        }
            //    }
            //}

            //getConfigStatus();
        }
        else {
            $("html, body").animate({ scrollTop: 0 }, "slow");
            error.show();
            error.append('<p> Details not saved. </p>');
        }
    });
}

function saveRBBankEnrollment(type) {

    if (bankstatus == 'SUB' && type == 1) {
        if ($('#sitea55334d1-3960-44c4-8cf1-e3ba9901f2be').length > 0)
            window.location.href = $('#sitea55334d1-3960-44c4-8cf1-e3ba9901f2be').attr('href');
        if ($('#site0eda5d25-591c-4e01-a845-fb580572cff5').length > 0)
            window.location.href = $('#site0eda5d25-591c-4e01-a845-fb580572cff5').attr('href');
        if ($('#site0eda5d25-591c-4e01-a845-fb580572cfe8').length > 0)
            window.location.href = $('#site0eda5d25-591c-4e01-a845-fb580572cfe8').attr('href');
        if ($('#site98a706d7-031f-4c5d-8cc4-d32cc7658b69').length > 0)
            window.location.href = $('#site98a706d7-031f-4c5d-8cc4-d32cc7658b69').attr('href');

        return;
    }
    else if (bankstatus == 'SUB') {
        return;
    }

    var _continue = true;
    var bankid = getUrlVars()["bankid"];

    ResetSuccessError();

    if ($('#txt_officename').val().trim() == '') {
        $('#txt_officename').addClass("error_msg");
        $('#txt_officename').attr('title', 'Please enter Office Name');
        _continue = false;
    }
    else {
        $('#txt_officename').removeClass("error_msg");
        $('#txt_officename').attr('title', '');
    }

    if ($('#txt_officename').val().trim() != '') {
        if ($('#txt_officename').val().trim().length > 35) {
            $('#txt_officename').addClass("error_msg");
            $('#txt_officename').attr('title', 'Office Name max length is 35');
            _continue = false;
        }
        else {
            $('#txt_officename').removeClass("error_msg");
            $('#txt_officename').attr('title', '');
        }
    }

    if ($('#txt_officeaddress').val().trim() == '') {
        $('#txt_officeaddress').addClass("error_msg");
        $('#txt_officeaddress').attr('title', 'Please enter Office Physical Street Address');
        _continue = false;
    }
    else {
        $('#txt_officeaddress').removeClass("error_msg");
        $('#txt_officeaddress').attr('title', '');
    }

    if ($('#txt_officeaddress').val().trim() != '') {
        if (isPOExist($('#txt_officeaddress').val())) {
            $('#txt_officeaddress').addClass("error_msg");
            $('#txt_officeaddress').attr('title', 'Office Address should not contain PO');
            _continue = false;
        }
        else {
            $('#txt_officeaddress').removeClass("error_msg");
            $('#txt_officeaddress').attr('title', '');
        }
    }

    if ($('#txt_officeaddress').val().trim() != '') {
        if ($('#txt_officeaddress').val().trim().length > 40) {
            $('#txt_officeaddress').addClass("error_msg");
            $('#txt_officeaddress').attr('title', 'Address max length is 40');
            _continue = false;
        }
        else {
            $('#txt_officeaddress').removeClass("error_msg");
            $('#txt_officeaddress').attr('title', '');
        }
    }

    if ($('#txt_officecity').val().trim() == '') {
        $('#txt_officecity').addClass("error_msg");
        $('#txt_officecity').attr('title', 'Please enter Office Physical City');
        _continue = false;
    }
    else {
        $('#txt_officecity').removeClass("error_msg");
        $('#txt_officecity').attr('title', '');
    }

    if ($('#ddl_officestate').val() == 0 || $('#ddl_officestate').val() == '' || $('#ddl_officestate').val() == undefined) {
        $('#ddl_officestate').addClass("error_msg");
        $('#ddl_officestate').attr('title', 'Please select Office Physical State');
        _continue = false;
    }
    else {
        $('#ddl_officestate').removeClass("error_msg");
        $('#ddl_officestate').attr('title', '');
    }

    if ($('#txt_officezip').val().trim() == '') {
        $('#txt_officezip').addClass("error_msg");
        $('#txt_officezip').attr('title', 'Please enter Office Physical Zip Code');
        _continue = false;
    }
    else {
        $('#txt_officezip').removeClass("error_msg");
        $('#txt_officezip').attr('title', '');
    }

    if ($('#txt_officezip').val().trim() != '') {
        var zip = $('#txt_officezip').val().trim();
        if (zip.length != 5 && zip.length != 9) {
            $('#txt_officezip').addClass("error_msg");
            $('#txt_officezip').attr('title', 'Please enter valid Office Physical Zip Code');
            _continue = false;
        }
        else {
            $('#txt_officezip').removeClass("error_msg");
            $('#txt_officezip').attr('title', '');
        }
    }

    if ($('#txt_offcntFN').val().trim() == '') {
        $('#txt_offcntFN').addClass("error_msg");
        $('#txt_offcntFN').attr('title', 'Please enter Office Contact First Name');
        _continue = false;
    }
    else {
        $('#txt_offcntFN').removeClass("error_msg");
        $('#txt_offcntFN').attr('title', '');
    }

    if ($('#txt_offcntLN').val().trim() == '') {
        $('#txt_offcntLN').addClass("error_msg");
        $('#txt_offcntLN').attr('title', 'Please enter Office Contact Last Name');
        _continue = false;
    }
    else {
        $('#txt_offcntLN').removeClass("error_msg");
        $('#txt_offcntLN').attr('title', '');
    }

    if ($('#txt_offcntSSN').val().trim() == '') {
        $('#txt_offcntSSN').addClass("error_msg");
        $('#txt_offcntSSN').attr('title', 'Please enter Office Contact SSN');
        _continue = false;
    }
    else {
        $('#txt_offcntSSN').removeClass("error_msg");
        $('#txt_offcntSSN').attr('title', '');
    }

    if ($('#txt_offcntSSN').val().trim() != '') {
        var zip = $('#txt_offcntSSN').val().trim();
        if (zip.length < 4) {
            $('#txt_offcntSSN').addClass("error_msg");
            $('#txt_offcntSSN').attr('title', 'Please enter valid Office Contact SSN');
            _continue = false;
        }
        else {
            $('#txt_offcntSSN').removeClass("error_msg");
            $('#txt_offcntSSN').attr('title', '');
        }
    }

    if ($('#txt_officephone').val().trim() == '') {
        $('#txt_officephone').addClass("error_msg");
        $('#txt_officephone').attr('title', 'Please enter Office Phone Number');
        _continue = false;
    }
    else {
        $('#txt_officephone').removeClass("error_msg");
        $('#txt_officephone').attr('title', '');
    }

    if ($('#txt_officephone').val().trim() != '') {
        var phone = $('#txt_officephone').val().trim().replace(/-/g, '');
        if (0 < phone.indexOf(0) > 4 || 0 < phone.indexOf(1) > 4 || phone.length < 10) {
            $('#txt_officephone').addClass("error_msg");
            $('#txt_officephone').attr('title', 'Please enter valid Office Phone Number');
            _continue = false;
        }
        else {
            $('#txt_officephone').removeClass("error_msg");
            $('#txt_officephone').attr('title', '');
        }
    }

    //if ($('#txt_cellphone').val().trim() == '') {
    //    $('#txt_cellphone').addClass("error_msg");
    //    $('#txt_cellphone').attr('title', 'Please enter Office Cell Number');
    //    _continue = false;
    //}
    //else {
    //    $('#txt_cellphone').removeClass("error_msg");
    //    $('#txt_cellphone').attr('title', '');
    //}

    if ($('#txt_cellphone').val().trim() != '') {
        var phone = $('#txt_cellphone').val().trim().replace(/-/g, '');
        if (0 < phone.indexOf(0) > 4 || 0 < phone.indexOf(1) > 4 || phone.length < 10) {
            $('#txt_cellphone').addClass("error_msg");
            $('#txt_cellphone').attr('title', 'Please enter valid Cell Phone Number');
            _continue = false;
        }
        else {
            $('#txt_cellphone').removeClass("error_msg");
            $('#txt_cellphone').attr('title', '');
        }
    }

    if ($('#txt_officephone').val().trim() != '' && $('#txt_cellphone').val().trim() != '') {
        if ($('#txt_officephone').val().trim() == $('#txt_cellphone').val().trim()) {
            $('#txt_cellphone').addClass("error_msg");
            $('#txt_cellphone').attr('title', 'Office Phone number should not match cell phone number');
            _continue = false;
        }
    }

    if ($('#txt_email').val().trim() == '') {
        $('#txt_email').addClass("error_msg");
        $('#txt_email').attr('title', 'Please enter Email Address');
        _continue = false;
    }
    else {
        $('#txt_email').removeClass("error_msg");
        $('#txt_email').attr('title', '');
    }

    if ($('#txt_offmngrFN').val().trim() == '') {
        $('#txt_offmngrFN').addClass("error_msg");
        $('#txt_offmngrFN').attr('title', 'Please enter Office Manager First Name');
        _continue = false;
    }
    else {
        $('#txt_offmngrFN').removeClass("error_msg");
        $('#txt_offmngrFN').attr('title', '');
    }

    if ($('#txt_offmngeLN').val().trim() == '') {
        $('#txt_offmngeLN').addClass("error_msg");
        $('#txt_offmngeLN').attr('title', 'Please enter Office Manager Last Name');
        _continue = false;
    }
    else {
        $('#txt_offmngeLN').removeClass("error_msg");
        $('#txt_offmngeLN').attr('title', '');
    }

    if ($('#txt_offmngrSSN').val().trim() == '') {
        $('#txt_offmngrSSN').addClass("error_msg");
        $('#txt_offmngrSSN').attr('title', 'Please enter Office Manager SSN');
        _continue = false;
    }
    else {
        $('#txt_offmngrSSN').removeClass("error_msg");
        $('#txt_offmngrSSN').attr('title', '');
    }

    if ($('#txt_offmngrSSN').val().trim() != '') {
        var zip = $('#txt_offmngrSSN').val().trim();
        if (zip.length < 9) {
            $('#txt_offmngrSSN').addClass("error_msg");
            $('#txt_offmngrSSN').attr('title', 'Please enter valid Office Manager SSN');
            _continue = false;
        }
        else {
            $('#txt_offmngrSSN').removeClass("error_msg");
            $('#txt_offmngrSSN').attr('title', '');
        }
    }

    if ($('#txt_offmngrDOB').val().trim() == '') {
        $('#txt_offmngrDOB').addClass("error_msg");
        $('#txt_offmngrDOB').attr('title', 'Please enter Office Manager Date of Birth');
        _continue = false;
    }
    else {
        $('#txt_offmngrDOB').removeClass("error_msg");
        $('#txt_offmngrDOB').attr('title', '');
    }

    if ($('#txt_offmngrDOB').val().trim() != '') {
        var dob = $('#txt_offmngrDOB').val().trim();
        var d = new Date();
        d.setYear(d.getFullYear() - 18);

        if (dob.indexOf('/') > 0) {
            if (!isValidDateFormat(dob)) {
                $('#txt_offmngrDOB').addClass("error_msg");
                $('#txt_offmngrDOB').attr('title', 'Please enter valid Office Manager Date of Birth');
                _continue = false;
            }
            else {
                $('#txt_offmngrDOB').removeClass("error_msg");
                $('#txt_offmngrDOB').attr('title', '');
                var dd = new Date(dob);
                if (dd > d) {
                    $('#txt_offmngrDOB').addClass("error_msg");
                    $('#txt_offmngrDOB').attr('title', 'Office Manager Date of Birth must be 18 years ago');
                    _continue = false;
                }
            }
        }
        else {
            var pos = 2, yearpos = 5;
            var b = '/';
            var updob = [dob.slice(0, pos), b, dob.slice(pos)].join('');
            var _updob = [updob.slice(0, yearpos), b, updob.slice(yearpos)].join('');
            if (!isValidDateFormat(_updob)) {
                $('#txt_offmngrDOB').addClass("error_msg");
                $('#txt_offmngrDOB').attr('title', 'Please enter valid Office Manager Date of Birth');
                _continue = false;
            }
            else {
                $('#txt_offmngrDOB').removeClass("error_msg");
                $('#txt_offmngrDOB').attr('title', '');
                var dd = new Date(_updob);
                if (dd > d) {
                    $('#txt_offmngrDOB').addClass("error_msg");
                    $('#txt_offmngrDOB').attr('title', 'Office Manager Date of Birth must be 18 years ago');
                    _continue = false;
                }
            }
        }
    }

    if ($('#txt_mailaddress').val().trim() == '') {
        $('#txt_mailaddress').addClass("error_msg");
        $('#txt_mailaddress').attr('title', 'Please enter Mailing Address');
        _continue = false;
    }
    else {
        $('#txt_mailaddress').removeClass("error_msg");
        $('#txt_mailaddress').attr('title', '');
    }

    if ($('#txt_mailaddress').val().trim() != '') {
        if (isPOExist($('#txt_mailaddress').val())) {
            $('#txt_mailaddress').addClass("error_msg");
            $('#txt_mailaddress').attr('title', 'Mailing Address should not contain PO');
            _continue = false;
        }
        else {
            $('#txt_mailaddress').removeClass("error_msg");
            $('#txt_mailaddress').attr('title', '');
        }
    }

    if ($('#txt_mailaddress').val().trim() != '') {
        if ($('#txt_mailaddress').val().trim().length > 40) {
            $('#txt_mailaddress').addClass("error_msg");
            $('#txt_mailaddress').attr('title', 'Address max length is 40');
            _continue = false;
        }
        else {
            $('#txt_mailaddress').removeClass("error_msg");
            $('#txt_mailaddress').attr('title', '');
        }
    }

    if ($('#txt_mailcity').val().trim() == '') {
        $('#txt_mailcity').addClass("error_msg");
        $('#txt_mailcity').attr('title', 'Please enter Mailing City');
        _continue = false;
    }
    else {
        $('#txt_mailcity').removeClass("error_msg");
        $('#txt_mailcity').attr('title', '');
    }

    if ($('#ddl_mailstate').val() == '' || $('#ddl_mailstate').val() == 0 || $('#ddl_mailstate').val() == undefined) {
        $('#ddl_mailstate').addClass("error_msg");
        $('#ddl_mailstate').attr('title', 'Please select Mailing State');
        _continue = false;
    }
    else {
        $('#ddl_mailstate').removeClass("error_msg");
        $('#ddl_mailstate').attr('title', '');
    }

    if ($('#txt_mailzip').val().trim() == '') {
        $('#txt_mailzip').addClass("error_msg");
        $('#txt_mailzip').attr('title', 'Please enter Mailing Zip');
        _continue = false;
    }
    else {
        $('#txt_mailzip').removeClass("error_msg");
        $('#txt_mailzip').attr('title', '');
    }

    if ($('#txt_mailzip').val().trim() != '') {
        var zip = $('#txt_mailzip').val().trim();
        if (zip.length != 5 && zip.length != 9) {
            $('#txt_mailzip').addClass("error_msg");
            $('#txt_mailzip').attr('title', 'Please enter valid Mailing Zip');
            _continue = false;
        }
        else {
            $('#txt_mailzip').removeClass("error_msg");
            $('#txt_mailzip').attr('title', '');
        }
    }

    if ($('#txt_fulladdress').val().trim() == '') {
        $('#txt_fulladdress').addClass("error_msg");
        $('#txt_fulladdress').attr('title', 'Please enter Fulfillment Shipping Street Address');
        _continue = false;
    }
    else {
        $('#txt_fulladdress').removeClass("error_msg");
        $('#txt_fulladdress').attr('title', '');
    }

    if ($('#txt_fulladdress').val().trim() != '') {
        if (isPOExist($('#txt_fulladdress').val())) {
            $('#txt_fulladdress').addClass("error_msg");
            $('#txt_fulladdress').attr('title', 'Fulfillment Shipping Address should not contain PO');
            _continue = false;
        }
        else {
            $('#txt_fulladdress').removeClass("error_msg");
            $('#txt_fulladdress').attr('title', '');
        }
    }

    if ($('#txt_fulladdress').val().trim() != '') {
        if ($('#txt_fulladdress').val().trim().length > 40) {
            $('#txt_fulladdress').addClass("error_msg");
            $('#txt_fulladdress').attr('title', 'Address max length is 40');
            _continue = false;
        }
        else {
            $('#txt_fulladdress').removeClass("error_msg");
            $('#txt_fulladdress').attr('title', '');
        }
    }

    if ($('#txt_fullcity').val().trim() == '') {
        $('#txt_fullcity').addClass("error_msg");
        $('#txt_fullcity').attr('title', 'Please enter Fulfillment Shipping City');
        _continue = false;
    }
    else {
        $('#txt_fullcity').removeClass("error_msg");
        $('#txt_fullcity').attr('title', '');
    }

    if ($('#ddl_fullstate').val() == '' || $('#ddl_fullstate').val() == 0 || $('#ddl_fullstate').val() == undefined) {
        $('#ddl_fullstate').addClass("error_msg");
        $('#ddl_fullstate').attr('title', 'Please select Fulfillment Shipping State');
        _continue = false;
    }
    else {
        $('#ddl_fullstate').removeClass("error_msg");
        $('#ddl_fullstate').attr('title', '');
    }

    if ($('#txt_fullzip').val().trim() == '') {
        $('#txt_fullzip').addClass("error_msg");
        $('#txt_fullzip').attr('title', 'Please enter Fulfillment Shipping Zip Code');
        _continue = false;
    }
    else {
        $('#txt_fullzip').removeClass("error_msg");
        $('#txt_fullzip').attr('title', '');
    }

    if ($('#txt_fullzip').val().trim() != '') {
        var zip = $('#txt_fullzip').val().trim();
        if (zip.length != 5 && zip.length != 9) {
            $('#txt_fullzip').addClass("error_msg");
            $('#txt_fullzip').attr('title', 'Please enter valid Fulfillment Shipping Zip Code');
            _continue = false;
        }
        else {
            $('#txt_fullzip').removeClass("error_msg");
            $('#txt_fullzip').attr('title', '');
        }
    }

    if ($('#txt_yearsinbusiness').val().trim() == '') {
        $('#txt_yearsinbusiness').addClass("error_msg");
        $('#txt_yearsinbusiness').attr('title', 'Please enter Years In Business');
        _continue = false;
    }
    else {
        $('#txt_yearsinbusiness').removeClass("error_msg");
        $('#txt_yearsinbusiness').attr('title', '');
    }

    if ($('#txt_yearsinbusiness').val().trim() != '') {
        var value = parseInt($('#txt_yearsinbusiness').val());
        if (value <= 0) {
            $('#txt_yearsinbusiness').addClass("error_msg");
            $('#txt_yearsinbusiness').attr('title', 'Please enter Years In Business grater than 0');
            _continue = false;
        }
        else {
            $('#txt_yearsinbusiness').removeClass("error_msg");
            $('#txt_yearsinbusiness').attr('title', '');
        }
    }

    if ($('#txt_noofbankprdcts').val().trim() == '') {
        $('#txt_noofbankprdcts').addClass("error_msg");
        $('#txt_noofbankprdcts').attr('title', 'Please enter Number Of Bank Products Last Year');
        _continue = false;
    }
    else {
        $('#txt_noofbankprdcts').removeClass("error_msg");
        $('#txt_noofbankprdcts').attr('title', '');
    }

    if ($('#ddl_prevbank').val() == '' || $('#ddl_prevbank').val() == 0 || $('#ddl_prevbank').val() == undefined) {
        $('#ddl_prevbank').addClass("error_msg");
        $('#ddl_prevbank').attr('title', 'Please select Previous Bank Product Facilitator');
        _continue = false;
    }
    else {
        $('#ddl_prevbank').removeClass("error_msg");
        $('#ddl_prevbank').attr('title', '');
    }

    if ($('#txt_ownerFN').val().trim() == '') {
        $('#txt_ownerFN').addClass("error_msg");
        $('#txt_ownerFN').attr('title', 'Please enter Owner First Name');
        _continue = false;
    }
    else {
        $('#txt_ownerFN').removeClass("error_msg");
        $('#txt_ownerFN').attr('title', '');
    }

    if ($('#txt_ownerLN').val().trim() == '') {
        $('#txt_ownerLN').addClass("error_msg");
        $('#txt_ownerLN').attr('title', 'Please enter Owner Last Name');
        _continue = false;
    }
    else {
        $('#txt_ownerLN').removeClass("error_msg");
        $('#txt_ownerLN').attr('title', '');
    }

    if ($('#txt_ownerSSN').val().trim() == '') {
        $('#txt_ownerSSN').addClass("error_msg");
        $('#txt_ownerSSN').attr('title', 'Please enter Owner SSN');
        _continue = false;
    }
    else {
        $('#txt_ownerSSN').removeClass("error_msg");
        $('#txt_ownerSSN').attr('title', '');
    }

    if ($('#txt_ownerSSN').val().trim() != '') {
        var SSN = $('#txt_ownerSSN').val().trim();
        if (SSN.length < 9) {
            $('#txt_ownerSSN').addClass("error_msg");
            $('#txt_ownerSSN').attr('title', 'Please enter valid 9 digit SSN');
            _continue = false;
        }
        else {
            $('#txt_ownerSSN').removeClass("error_msg");
            $('#txt_ownerSSN').attr('title', '');
        }
    }

    if ($('#txt_ownerDOB').val().trim() == '') {
        $('#txt_ownerDOB').addClass("error_msg");
        $('#txt_ownerDOB').attr('title', 'Please enter Owner Date of Birth');
        _continue = false;
    }
    else {
        $('#txt_ownerDOB').removeClass("error_msg");
        $('#txt_ownerDOB').attr('title', '');
    }

    if ($('#txt_ownerDOB').val().trim() != '') {
        var dob = $('#txt_ownerDOB').val().trim();
        var d = new Date();
        d.setYear(d.getFullYear() - 18);

        if (dob.indexOf('/') > 0) {
            if (!isValidDateFormat(dob)) {
                $('#txt_ownerDOB').addClass("error_msg");
                $('#txt_ownerDOB').attr('title', 'Please enter valid Owner Date of Birth');
                _continue = false;
            }
            else {
                $('#txt_ownerDOB').removeClass("error_msg");
                $('#txt_ownerDOB').attr('title', '');
                var dd = new Date(dob);
                if (dd > d) {
                    $('#txt_ownerDOB').addClass("error_msg");
                    $('#txt_ownerDOB').attr('title', 'Owner Date of Birth must be 18 years ago');
                    _continue = false;
                }
            }
        }
        else {
            var pos = 2, yearpos = 5;
            var b = '/';
            var updob = [dob.slice(0, pos), b, dob.slice(pos)].join('');
            var _updob = [updob.slice(0, yearpos), b, updob.slice(yearpos)].join('');
            if (!isValidDateFormat(_updob)) {
                $('#txt_ownerDOB').addClass("error_msg");
                $('#txt_ownerDOB').attr('title', 'Please enter valid Owne Date of Birth');
                _continue = false;
            }
            else {
                $('#txt_ownerDOB').removeClass("error_msg");
                $('#txt_ownerDOB').attr('title', '');
                var dd = new Date(_updob);
                if (dd > d) {
                    $('#txt_ownerDOB').addClass("error_msg");
                    $('#txt_ownerDOB').attr('title', 'Owner Date of Birth must be 18 years ago');
                    _continue = false;
                }
            }
        }
    }

    if ($('#txt_ownerphone').val().trim() == '') {
        $('#txt_ownerphone').addClass("error_msg");
        $('#txt_ownerphone').attr('title', 'Please enter Owner Home Phone');
        _continue = false;
    }
    else {
        $('#txt_ownerphone').removeClass("error_msg");
        $('#txt_ownerphone').attr('title', '');
    }

    if ($('#txt_ownerphone').val().trim() != '') {
        var phone = $('#txt_ownerphone').val().trim().replace(/-/g, '');
        if (0 < phone.indexOf(0) > 4 || 0 < phone.indexOf(1) > 4 || phone.length < 10) {
            $('#txt_ownerphone').addClass("error_msg");
            $('#txt_ownerphone').attr('title', 'Please enter valid Owner Phone Number');
            _continue = false;
        }
        else {
            $('#txt_ownerphone').removeClass("error_msg");
            $('#txt_ownerphone').attr('title', '');
        }
    }

    if ($('#txt_owneraddress').val().trim() == '') {
        $('#txt_owneraddress').addClass("error_msg");
        $('#txt_owneraddress').attr('title', 'Please enter Owner Address');
        _continue = false;
    }
    else {
        $('#txt_owneraddress').removeClass("error_msg");
        $('#txt_owneraddress').attr('title', '');
    }

    if ($('#txt_owneraddress').val().trim() != '') {
        if (isPOExist($('#txt_owneraddress').val())) {
            $('#txt_owneraddress').addClass("error_msg");
            $('#txt_owneraddress').attr('title', 'Owner Address should not contain PO');
            _continue = false;
        }
        else {
            $('#txt_owneraddress').removeClass("error_msg");
            $('#txt_owneraddress').attr('title', '');
        }
    }

    if ($('#txt_owneraddress').val().trim() != '') {
        if ($('#txt_owneraddress').val().trim().length > 40) {
            $('#txt_owneraddress').addClass("error_msg");
            $('#txt_owneraddress').attr('title', 'Address max length is 40');
            _continue = false;
        }
        else {
            $('#txt_owneraddress').removeClass("error_msg");
            $('#txt_owneraddress').attr('title', '');
        }
    }

    if ($('#txt_ownercity').val().trim() == '') {
        $('#txt_ownercity').addClass("error_msg");
        $('#txt_ownercity').attr('title', 'Please enter Owner City');
        _continue = false;
    }
    else {
        $('#txt_ownercity').removeClass("error_msg");
        $('#txt_ownercity').attr('title', '');
    }

    if ($('#ddl_ownerstate').val() == '' || $('#ddl_ownerstate').val() == 0 || $('#ddl_ownerstate').val() == undefined) {
        $('#ddl_ownerstate').addClass("error_msg");
        $('#ddl_ownerstate').attr('title', 'Please select Owner State');
        _continue = false;
    }
    else {
        $('#ddl_ownerstate').removeClass("error_msg");
        $('#ddl_ownerstate').attr('title', '');
    }

    if ($('#txt_ownerzip').val().trim() == '') {
        $('#txt_ownerzip').addClass("error_msg");
        $('#txt_ownerzip').attr('title', 'Please enter Owner Zip');
        _continue = false;
    }
    else {
        $('#txt_ownerzip').removeClass("error_msg");
        $('#txt_ownerzip').attr('title', '');
    }

    if ($('#txt_ownerzip').val().trim() != '') {
        var zip = $('#txt_ownerzip').val().trim();
        if (zip.length != 5 && zip.length != 9) {
            $('#txt_ownerzip').addClass("error_msg");
            $('#txt_ownerzip').attr('title', 'Please enter valid zip code');
            _continue = false;
        }
        else {
            $('#txt_ownerzip').removeClass("error_msg");
            $('#txt_ownerzip').attr('title', '');
        }
    }

    if ($('#txt_businessEIN').val().trim() != '') {
        var zip = $('#txt_businessEIN').val().trim();
        if (zip.length < 9) {
            $('#txt_businessEIN').addClass("error_msg");
            $('#txt_businessEIN').attr('title', 'Please enter valid Business EIN');
            _continue = false;
        }
        else {
            $('#txt_businessEIN').removeClass("error_msg");
            $('#txt_businessEIN').attr('title', '');
        }
    }

    if ($('#txt_efinownerFN').val().trim() == '') {
        $('#txt_efinownerFN').addClass("error_msg");
        $('#txt_efinownerFN').attr('title', 'Please enter EFIN Holder First Name');
        _continue = false;
    }
    else {
        $('#txt_efinownerFN').removeClass("error_msg");
        $('#txt_efinownerFN').attr('title', '');
    }

    if ($('#txt_efinownerLN').val().trim() == '') {
        $('#txt_efinownerLN').addClass("error_msg");
        $('#txt_efinownerLN').attr('title', 'Please enter EFIN Holder Last Name');
        _continue = false;
    }
    else {
        $('#txt_efinownerLN').removeClass("error_msg");
        $('#txt_efinownerLN').attr('title', '');
    }

    if ($('#txt_efintitle').val().trim() == '') {
        $('#txt_efintitle').addClass("error_msg");
        $('#txt_efintitle').attr('title', 'Please enter EFIN Holder Title');
        _continue = false;
    }
    else {
        $('#txt_efintitle').removeClass("error_msg");
        $('#txt_efintitle').attr('title', '');
    }

    if ($('#txt_efinownerSSN').val().trim() == '') {
        $('#txt_efinownerSSN').addClass("error_msg");
        $('#txt_efinownerSSN').attr('title', 'Please enter EFIN Owner SSN');
        _continue = false;
    }
    else {
        $('#txt_efinownerSSN').removeClass("error_msg");
        $('#txt_efinownerSSN').attr('title', '');
    }

    if ($('#txt_efinownerSSN').val().trim() != '') {
        var zip = $('#txt_efinownerSSN').val().trim();
        if (zip.length < 9) {
            $('#txt_efinownerSSN').addClass("error_msg");
            $('#txt_efinownerSSN').attr('title', 'Please enter valid SSN');
            _continue = false;
        }
        else {
            $('#txt_efinownerSSN').removeClass("error_msg");
            $('#txt_efinownerSSN').attr('title', '');
        }
    }

    if ($('#txt_efinownerDOB').val().trim() == '') {
        $('#txt_efinownerDOB').addClass("error_msg");
        $('#txt_efinownerDOB').attr('title', 'Please enter EFIN Owner Date of Birth');
        _continue = false;
    }
    else {
        $('#txt_efinownerDOB').removeClass("error_msg");
        $('#txt_efinownerDOB').attr('title', '');
    }

    if ($('#txt_efinownerDOB').val().trim() != '') {
        var dob = $('#txt_efinownerDOB').val().trim();
        var d = new Date();
        d.setYear(d.getFullYear() - 18);

        if (dob.indexOf('/') > 0) {
            if (!isValidDateFormat(dob)) {
                $('#txt_efinownerDOB').addClass("error_msg");
                $('#txt_efinownerDOB').attr('title', 'Please enter valid EFIN Owner Date of Birth');
                _continue = false;
            }
            else {
                $('#txt_efinownerDOB').removeClass("error_msg");
                $('#txt_efinownerDOB').attr('title', '');
                var dd = new Date(dob);
                if (dd > d) {
                    $('#txt_efinownerDOB').addClass("error_msg");
                    $('#txt_efinownerDOB').attr('title', 'EFIN Owner Date of Birth must be 18 years ago');
                    _continue = false;
                }
            }
        }
        else {
            var pos = 2, yearpos = 5;
            var b = '/';
            var updob = [dob.slice(0, pos), b, dob.slice(pos)].join('');
            var _updob = [updob.slice(0, yearpos), b, updob.slice(yearpos)].join('');
            if (!isValidDateFormat(_updob)) {
                $('#txt_efinownerDOB').addClass("error_msg");
                $('#txt_efinownerDOB').attr('title', 'Please enter valid EFIN Owner Date of Birth');
                _continue = false;
            }
            else {
                $('#txt_efinownerDOB').removeClass("error_msg");
                $('#txt_efinownerDOB').attr('title', '');
                var dd = new Date(_updob);
                if (dd > d) {
                    $('#txt_efinownerDOB').addClass("error_msg");
                    $('#txt_efinownerDOB').attr('title', 'EFIN Owner Date of Birth must be 18 years ago');
                    _continue = false;
                }
            }
        }
    }

    if ($('#txt_offContactDOB').val().trim() != '') {
        var dob = $('#txt_offContactDOB').val().trim();
        var d = new Date();
        d.setYear(d.getFullYear() - 18);

        if (dob.indexOf('/') > 0) {
            if (!isValidDateFormat(dob)) {
                $('#txt_offContactDOB').addClass("error_msg");
                $('#txt_offContactDOB').attr('title', 'Please enter valid Date of Birth');
                _continue = false;
            }
            else {
                $('#txt_offContactDOB').removeClass("error_msg");
                $('#txt_offContactDOB').attr('title', '');
                var dd = new Date(dob);
                if (dd > d) {
                    $('#txt_offContactDOB').addClass("error_msg");
                    $('#txt_offContactDOB').attr('title', 'Date of Birth must be 18 years ago');
                    _continue = false;
                }
            }
        }
        else {
            var pos = 2, yearpos = 5;
            var b = '/';
            var updob = [dob.slice(0, pos), b, dob.slice(pos)].join('');
            var _updob = [updob.slice(0, yearpos), b, updob.slice(yearpos)].join('');
            if (!isValidDateFormat(_updob)) {
                $('#txt_offContactDOB').addClass("error_msg");
                $('#txt_offContactDOB').attr('title', 'Please enter valid Date of Birth');
                _continue = false;
            }
            else {
                $('#txt_offContactDOB').removeClass("error_msg");
                $('#txt_offContactDOB').attr('title', '');
                var dd = new Date(_updob);
                if (dd > d) {
                    $('#txt_offContactDOB').addClass("error_msg");
                    $('#txt_offContactDOB').attr('title', 'Date of Birth must be 18 years ago');
                    _continue = false;
                }
            }
        }
    }

    if ($('#txt_efinownerAddress').val().trim() == '') {
        $('#txt_efinownerAddress').addClass("error_msg");
        $('#txt_efinownerAddress').attr('title', 'Please enter EFIN Owner Address');
        _continue = false;
    }
    else {
        $('#txt_efinownerAddress').removeClass("error_msg");
        $('#txt_efinownerAddress').attr('title', '');
    }

    if ($('#txt_efinownerAddress').val().trim() != '') {
        if (isPOExist($('#txt_efinownerAddress').val())) {
            $('#txt_efinownerAddress').addClass("error_msg");
            $('#txt_efinownerAddress').attr('title', 'Efin Owner Address should not contain PO');
            _continue = false;
        }
        else {
            $('#txt_efinownerAddress').removeClass("error_msg");
            $('#txt_efinownerAddress').attr('title', '');
        }
    }

    if ($('#txt_efinownerAddress').val().trim() != '') {
        if ($('#txt_efinownerAddress').val().trim().length > 40) {
            $('#txt_efinownerAddress').addClass("error_msg");
            $('#txt_efinownerAddress').attr('title', 'Address max length is 40');
            _continue = false;
        }
        else {
            $('#txt_efinownerAddress').removeClass("error_msg");
            $('#txt_efinownerAddress').attr('title', '');
        }
    }

    if ($('#txt_efinownercity').val().trim() == '') {
        $('#txt_efinownercity').addClass("error_msg");
        $('#txt_efinownercity').attr('title', 'Please enter EFIN Owner City');
        _continue = false;
    }
    else {
        $('#txt_efinownercity').removeClass("error_msg");
        $('#txt_efinownercity').attr('title', '');
    }

    if ($('#ddl_efinownerstate').val() == '' || $('#ddl_efinownerstate').val() == 0 || $('#ddl_efinownerstate').val() == undefined) {
        $('#ddl_efinownerstate').addClass("error_msg");
        $('#ddl_efinownerstate').attr('title', 'Please enter EFIN Owner DOB');
        _continue = false;
    }
    else {
        $('#ddl_efinownerstate').removeClass("error_msg");
        $('#ddl_efinownerstate').attr('title', '');
    }

    if ($('#txt_efinownerZip').val().trim() == '') {
        $('#txt_efinownerZip').addClass("error_msg");
        $('#txt_efinownerZip').attr('title', 'Please enter EFIN Owner zip');
        _continue = false;
    }
    else {
        $('#txt_efinownerZip').removeClass("error_msg");
        $('#txt_efinownerZip').attr('title', '');
    }

    if ($('#txt_efinownerZip').val().trim() != '') {
        var zip = $('#txt_efinownerZip').val().trim();
        if (zip.length != 5 && zip.length != 9) {
            $('#txt_efinownerZip').addClass("error_msg");
            $('#txt_efinownerZip').attr('title', 'Please enter valid SSN');
            _continue = false;
        }
        else {
            $('#txt_efinownerZip').removeClass("error_msg");
            $('#txt_efinownerZip').attr('title', '');
        }
    }

    if ($('#txt_efinownerphone').val().trim() == '') {
        $('#txt_efinownerphone').addClass("error_msg");
        $('#txt_efinownerphone').attr('title', 'Please enter EFIN Holder Phone');
        _continue = false;
    }
    else {
        $('#txt_efinownerphone').removeClass("error_msg");
        $('#txt_efinownerphone').attr('title', '');
    }

    if ($('#txt_efinownerphone').val().trim() != '') {
        var phone = $('#txt_efinownerphone').val().trim().replace(/-/g, '');
        if (0 < phone.indexOf(0) > 4 || 0 < phone.indexOf(1) > 4 || phone.length < 10) {
            $('#txt_efinownerphone').addClass("error_msg");
            $('#txt_efinownerphone').attr('title', 'Please enter valid Phone Number');
            _continue = false;
        }
        else {
            $('#txt_efinownerphone').removeClass("error_msg");
            $('#txt_efinownerphone').attr('title', '');
        }
    }

    if ($('#txt_efinownermobile').val().trim() != '') {
        var phone = $('#txt_efinownermobile').val().trim().replace(/-/g, '');
        if (0 < phone.indexOf(0) > 4 || 0 < phone.indexOf(1) > 4 || phone.length < 10) {
            $('#txt_efinownermobile').addClass("error_msg");
            $('#txt_efinownermobile').attr('title', 'Please enter valid Mobile Number');
            _continue = false;
        }
        else {
            $('#txt_efinownermobile').removeClass("error_msg");
            $('#txt_efinownermobile').attr('title', '');
        }
    }


    // Added because of submission failed with this error
    if ($('#txt_efinowneremail').val().trim() == '') {
        $('#txt_efinowneremail').addClass("error_msg");
        $('#txt_efinowneremail').attr('title', 'Please enter EFIN Holder Email');
        _continue = false;
    }
    else {
        $('#txt_efinowneremail').removeClass("error_msg");
        $('#txt_efinowneremail').attr('title', '');
    }


    if ($('#txt_efinowneridnumber').val().trim() == '') {
        $('#txt_efinowneridnumber').addClass("error_msg");
        $('#txt_efinowneridnumber').attr('title', 'Please enter EFIN Holder Id Number');
        _continue = false;
    }
    else {
        $('#txt_efinowneridnumber').removeClass("error_msg");
        $('#txt_efinowneridnumber').attr('title', '');
    }

    //if ($('#txt_efinownerein').val().trim() == '') {
    //    $('#txt_efinownerein').addClass("error_msg");
    //    $('#txt_efinownerein').attr('title', 'Please enter EFIN Holder EIN');
    //    _continue = false;
    //}
    //else {
    //    $('#txt_efinownerein').removeClass("error_msg");
    //    $('#txt_efinownerein').attr('title', '');
    //}

    //if ($('#txt_efinownerein').val() != '' && $('#txt_businessEIN').val() != '') {
    //    if ($('#txt_efinownerein').val() != $('#txt_businessEIN').val()) {
    //        $('#txt_efinownerein').addClass("error_msg");
    //        $('#txt_efinownerein').attr('title', 'Please enter Same as Business EIN');
    //        _continue = false;
    //    }
    //    else {
    //        $('#txt_efinownerein').removeClass("error_msg");
    //        $('#txt_efinownerein').attr('title', '');
    //    }
    //}

    if ($('#txt_efinownerein').val() != '') {
        var len = $('#txt_efinownerein').val().length;
        if (len < 9) {
            $('#txt_efinownerein').addClass("error_msg");
            $('#txt_efinownerein').attr('title', 'Please enter valid 9 digit EFIN Holder EIN');
            _continue = false;
        }
        else {
            $('#txt_efinownerein').removeClass("error_msg");
            $('#txt_efinownerein').attr('title', '');
        }
    }

    if ($('#ddl_idstate').val() == '' || $('#ddl_idstate').val() == 0 || $('#ddl_idstate').val() == undefined) {
        $('#ddl_idstate').addClass("error_msg");
        $('#ddl_idstate').attr('title', 'Please enter EFIN Holder Phone');
        _continue = false;
    }
    else {
        $('#ddl_idstate').removeClass("error_msg");
        $('#ddl_idstate').attr('title', '');
    }

    if ($('#ddl_legalentity').val() == '' || $('#ddl_legalentity').val() == 0 || $('#ddl_legalentity').val() == undefined) {
        $('#ddl_legalentity').addClass("error_msg");
        $('#ddl_legalentity').attr('title', 'Please select Legal Entity Status Indicator');
        _continue = false;
    }
    else {
        $('#ddl_legalentity').removeClass("error_msg");
        $('#ddl_legalentity').attr('title', '');
    }

    if ($('#ddl_llcmembership').val() == '' || $('#ddl_llcmembership').val() == 0 || $('#ddl_llcmembership').val() == undefined) {
        $('#ddl_llcmembership').addClass("error_msg");
        $('#ddl_llcmembership').attr('title', 'Please select LLC Membership Registration');
        _continue = false;
    }
    else {
        $('#ddl_llcmembership').removeClass("error_msg");
        $('#ddl_llcmembership').attr('title', '');
    }

    if ($('#ddl_multioffice').val() == '' || $('#ddl_multioffice').val() == 0 || $('#ddl_multioffice').val() == undefined) {
        $('#ddl_multioffice').addClass("error_msg");
        $('#ddl_multioffice').attr('title', 'Please select Is EFIN part of a multi-office');
        _continue = false;
    }
    else {
        $('#ddl_multioffice').removeClass("error_msg");
        $('#ddl_multioffice').attr('title', '');
    }

    if ($('#ddl_productsoffering').val() == '' || $('#ddl_productsoffering').val() == 0 || $('#ddl_productsoffering').val() == undefined) {
        $('#ddl_productsoffering').addClass("error_msg");
        $('#ddl_productsoffering').attr('title', 'Please select What products are you offering');
        _continue = false;
    }
    else {
        $('#ddl_productsoffering').removeClass("error_msg");
        $('#ddl_productsoffering').attr('title', '');
    }

    if ($('#ddl_ptin').val() == '' || $('#ddl_ptin').val() == 0 || $('#ddl_ptin').val() == undefined) {
        $('#ddl_ptin').addClass("error_msg");
        $('#ddl_ptin').attr('title', 'Please select  PTIN, as required by the IRS');
        _continue = false;
    }
    else {
        $('#ddl_ptin').removeClass("error_msg");
        $('#ddl_ptin').attr('title', '');
    }

    if ($('#ddl_processlaw').val() == '' || $('#ddl_processlaw').val() == 0 || $('#ddl_processlaw').val() == undefined) {
        $('#ddl_processlaw').addClass("error_msg");
        $('#ddl_processlaw').attr('title', 'Please select');
        _continue = false;
    }
    else {
        $('#ddl_processlaw').removeClass("error_msg");
        $('#ddl_processlaw').attr('title', '');
    }

    if ($('#ddl_complaince').val() == '' || $('#ddl_complaince').val() == 0 || $('#ddl_complaince').val() == undefined) {
        $('#ddl_complaince').addClass("error_msg");
        $('#ddl_complaince').attr('title', 'Please select');
        _continue = false;
    }
    else {
        $('#ddl_complaince').removeClass("error_msg");
        $('#ddl_complaince').attr('title', '');
    }

    if ($('#ddl_consumerlending').val() == '' || $('#ddl_consumerlending').val() == 0 || $('#ddl_consumerlending').val() == undefined) {
        $('#ddl_consumerlending').addClass("error_msg");
        $('#ddl_consumerlending').attr('title', 'Please select');
        _continue = false;
    }
    else {
        $('#ddl_consumerlending').removeClass("error_msg");
        $('#ddl_consumerlending').attr('title', '');
    }

    if ($('#txt_noofpersoneel').val().trim() == '') {
        $('#txt_noofpersoneel').addClass("error_msg");
        $('#txt_noofpersoneel').attr('title', 'Please enter number');
        _continue = false;
    }
    else {
        $('#txt_noofpersoneel').removeClass("error_msg");
        $('#txt_noofpersoneel').attr('title', '');
    }

    if ($('#txt_noofpersoneel').val().trim() != '') {
        var num = Number($('#txt_noofpersoneel').val().trim());
        if (num <= 0) {
            $('#txt_noofpersoneel').addClass("error_msg");
            $('#txt_noofpersoneel').attr('title', 'Please enter more than 0');
            _continue = false;
        }
        else {
            $('#txt_noofpersoneel').removeClass("error_msg");
            $('#txt_noofpersoneel').attr('title', '');
        }
    }

    if ($('#ddl_advertiseapprvl').val() == '' || $('#ddl_advertiseapprvl').val() == 0 || $('#ddl_advertiseapprvl').val() == undefined) {
        $('#ddl_advertiseapprvl').addClass("error_msg");
        $('#ddl_advertiseapprvl').attr('title', 'Please select');
        _continue = false;
    }
    else {
        $('#ddl_advertiseapprvl').removeClass("error_msg");
        $('#ddl_advertiseapprvl').attr('title', '');
    }

    if ($('#ddl_lockeddocs').val() == '' || $('#ddl_lockeddocs').val() == 0 || $('#ddl_lockeddocs').val() == undefined) {
        $('#ddl_lockeddocs').addClass("error_msg");
        $('#ddl_lockeddocs').attr('title', 'Please select');
        _continue = false;
    }
    else {
        $('#ddl_lockeddocs').removeClass("error_msg");
        $('#ddl_lockeddocs').attr('title', '');
    }

    if ($('#ddl_lockedcardschecks').val() == '' || $('#ddl_lockedcardschecks').val() == 0 || $('#ddl_lockedcardschecks').val() == undefined) {
        $('#ddl_lockedcardschecks').addClass("error_msg");
        $('#ddl_lockedcardschecks').attr('title', 'Please select');
        _continue = false;
    }
    else {
        $('#ddl_lockedcardschecks').removeClass("error_msg");
        $('#ddl_lockedcardschecks').attr('title', '');
    }

    if ($('#ddl_lockedoffice').val() == '' || $('#ddl_lockedoffice').val() == 0 || $('#ddl_lockedoffice').val() == undefined) {
        $('#ddl_lockedoffice').addClass("error_msg");
        $('#ddl_lockedoffice').attr('title', 'Please select');
        _continue = false;
    }
    else {
        $('#ddl_lockedoffice').removeClass("error_msg");
        $('#ddl_lockedoffice').attr('title', '');
    }

    if ($('#ddl_limitaccess').val() == '' || $('#ddl_limitaccess').val() == 0 || $('#ddl_limitaccess').val() == undefined) {
        $('#ddl_limitaccess').addClass("error_msg");
        $('#ddl_limitaccess').attr('title', 'Please select');
        _continue = false;
    }
    else {
        $('#ddl_limitaccess').removeClass("error_msg");
        $('#ddl_limitaccess').attr('title', '');
    }

    if ($('#ddl_plandispose').val() == '' || $('#ddl_plandispose').val() == 0 || $('#ddl_plandispose').val() == undefined) {
        $('#ddl_plandispose').addClass("error_msg");
        $('#ddl_plandispose').attr('title', 'Please select');
        _continue = false;
    }
    else {
        $('#ddl_plandispose').removeClass("error_msg");
        $('#ddl_plandispose').attr('title', '');
    }

    if ($('#ddl_logintoemployees').val() == '' || $('#ddl_logintoemployees').val() == 0 || $('#ddl_logintoemployees').val() == undefined) {
        $('#ddl_logintoemployees').addClass("error_msg");
        $('#ddl_logintoemployees').attr('title', 'Please select');
        _continue = false;
    }
    else {
        $('#ddl_logintoemployees').removeClass("error_msg");
        $('#ddl_logintoemployees').attr('title', '');
    }

    if ($('#ddl_antivirus').val() == '' || $('#ddl_antivirus').val() == 0 || $('#ddl_antivirus').val() == undefined) {
        $('#ddl_antivirus').addClass("error_msg");
        $('#ddl_antivirus').attr('title', 'Please select');
        _continue = false;
    }
    else {
        $('#ddl_antivirus').removeClass("error_msg");
        $('#ddl_antivirus').attr('title', '');
    }

    if ($('#ddl_firewall').val() == '' || $('#ddl_firewall').val() == 0 || $('#ddl_firewall').val() == undefined) {
        $('#ddl_firewall').addClass("error_msg");
        $('#ddl_firewall').attr('title', 'Please select');
        _continue = false;
    }
    else {
        $('#ddl_firewall').removeClass("error_msg");
        $('#ddl_firewall').attr('title', '');
    }

    if ($('#ddl_onlinetraining').val() == '' || $('#ddl_onlinetraining').val() == 0 || $('#ddl_onlinetraining').val() == undefined) {
        $('#ddl_onlinetraining').addClass("error_msg");
        $('#ddl_onlinetraining').attr('title', 'Please select');
        _continue = false;
    }
    else {
        $('#ddl_onlinetraining').removeClass("error_msg");
        $('#ddl_onlinetraining').attr('title', '');
    }

    if ($('#ddl_wifipwd').val() == '' || $('#ddl_wifipwd').val() == 0 || $('#ddl_wifipwd').val() == undefined) {
        $('#ddl_wifipwd').addClass("error_msg");
        $('#ddl_wifipwd').attr('title', 'Please select');
        _continue = false;
    }
    else {
        $('#ddl_wifipwd').removeClass("error_msg");
        $('#ddl_wifipwd').attr('title', '');
    }

    if ($('#ddl_pwdrqd').val() == '' || $('#ddl_pwdrqd').val() == 0 || $('#ddl_pwdrqd').val() == undefined) {
        $('#ddl_pwdrqd').addClass("error_msg");
        $('#ddl_pwdrqd').attr('title', 'Please select');
        _continue = false;
    }
    else {
        $('#ddl_pwdrqd').removeClass("error_msg");
        $('#ddl_pwdrqd').attr('title', '');
    }

    if ($('#txt_accountname').val().trim() == '') {
        $('#txt_accountname').addClass("error_msg");
        $('#txt_accountname').attr('title', 'Please enter Name on Checking Account');
        _continue = false;
    }
    else {
        $('#txt_accountname').removeClass("error_msg");
        $('#txt_accountname').attr('title', '');
    }

    if ($('#txt_bankname').val().trim() == '') {
        $('#txt_bankname').addClass("error_msg");
        $('#txt_bankname').attr('title', 'Please enter Bank Name');
        _continue = false;
    }
    else {
        $('#txt_bankname').removeClass("error_msg");
        $('#txt_bankname').attr('title', '');
    }

    if ($('#txt_bankroutingno').val().trim() == '') {
        $('#txt_bankroutingno').addClass("error_msg");
        $('#txt_bankroutingno').attr('title', 'Please enter Bank Routing Number');
        _continue = false;
    }
    else {
        $('#txt_bankroutingno').removeClass("error_msg");
        $('#txt_bankroutingno').attr('title', '');
    }

    if ($('#txt_bankroutingno').val().trim() != '') {
        var rtn = $('#txt_bankroutingno').val().trim();
        if (rtn.length < 9) {
            $('#txt_bankroutingno').addClass("error_msg");
            $('#txt_bankroutingno').attr('title', 'Please enter valid 9 digit Bank Routing Number');
            _continue = false;
        }
        else {
            $('#txt_bankroutingno').removeClass("error_msg");
            $('#txt_bankroutingno').attr('title', '');
        }
    }

    if ($('#txt_confirmbankroutingno').val().trim() == '') {
        $('#txt_confirmbankroutingno').addClass("error_msg");
        $('#txt_confirmbankroutingno').attr('title', 'Please enter Confirm Banking Routing Number');
        _continue = false;
    }
    else {
        $('#txt_confirmbankroutingno').removeClass("error_msg");
        $('#txt_confirmbankroutingno').attr('title', '');
    }

    if ($('#txt_confirmbankroutingno').val() != $('#txt_bankroutingno').val()) {
        $('#txt_confirmbankroutingno').addClass("error_msg");
        $('#txt_confirmbankroutingno').attr('title', 'Banking Routing Number is mismatching');
        _continue = false;
    }
    else {
        $('#txt_confirmbankroutingno').removeClass("error_msg");
        $('#txt_confirmbankroutingno').attr('title', '');
    }

    if ($('#txt_bankaccountno').val().trim() == '') {
        $('#txt_bankaccountno').addClass("error_msg");
        $('#txt_bankaccountno').attr('title', 'Please enter Bank Account Number');
        _continue = false;
    }
    else {
        $('#txt_bankaccountno').removeClass("error_msg");
        $('#txt_bankaccountno').attr('title', '');
    }

    if ($('#txt_bankaccountno').val().trim() != '') {
        var dan = parseInt($('#txt_bankaccountno').val());
        if (dan <= 0) {
            $('#txt_bankaccountno').addClass("error_msg");
            $('#txt_bankaccountno').attr('title', 'Please enter valid Bank Account Number');
            _continue = false;
        }
        else {
            $('#txt_bankaccountno').removeClass("error_msg");
            $('#txt_bankaccountno').attr('title', '');
        }
    }

    if ($('#txt_confirmbankaccountno').val().trim() == '') {
        $('#txt_confirmbankaccountno').addClass("error_msg");
        $('#txt_confirmbankaccountno').attr('title', 'Please enter Confirm Banking Accounnt Number');
        _continue = false;
    }
    else {
        $('#txt_confirmbankaccountno').removeClass("error_msg");
        $('#txt_confirmbankaccountno').attr('title', '');
    }

    if ($('#txt_confirmbankaccountno').val() != $('#txt_bankaccountno').val()) {
        $('#txt_confirmbankaccountno').addClass("error_msg");
        $('#txt_confirmbankaccountno').attr('title', 'Banking Account Number is mismatching');
        _continue = false;
    }
    else {
        $('#txt_confirmbankaccountno').removeClass("error_msg");
        $('#txt_confirmbankaccountno').attr('title', '');
    }

    if ($('#ddl_accounttype').val() == '' || $('#ddl_accounttype').val() == 0 || $('#ddl_accounttype').val() == undefined) {
        $('#ddl_accounttype').addClass("error_msg");
        $('#ddl_accounttype').attr('title', 'Please select Bank Account Type');
        _continue = false;
    }
    else {
        $('#ddl_accounttype').removeClass("error_msg");
        $('#ddl_accounttype').attr('title', '');
    }

    if ($('#ddl_cardprogram').val() == '' || $('#ddl_cardprogram').val() == 0 || $('#ddl_cardprogram').val() == undefined) {
        $('#ddl_cardprogram').addClass("error_msg");
        $('#ddl_cardprogram').attr('title', 'Please select Prepaid Card Offers');
        _continue = false;
    }
    else {
        $('#ddl_cardprogram').removeClass("error_msg");
        $('#ddl_cardprogram').attr('title', '');
    }

    if (!$('#chk_returns').prop('checked')) {
        $('#dvTandC').addClass("error_msg");
        $('#dvTandC').attr('title', 'Please select Terms and Conditions');
        _continue = false;
    }
    else {
        $('#dvTandC').removeClass("error_msg");
        $('#dvTandC').attr('title', '');
    }

    var rtn1 = $('#txt_bankroutingno').val().trim();
    if (rtn1 != '') {
        var isValidrtn = checkABA(rtn1);
        if (!isValidrtn) {
            _continue = false;
            $('#txt_bankroutingno').addClass("error_msg");
            $('#txt_bankroutingno').attr('title', 'Please enter valid RTN');
        }
        else {
            $('#txt_bankroutingno').removeClass("error_msg");
            $('#txt_bankroutingno').attr('title', '');
        }
    }

    var email = $('#txt_email').val().trim();
    if (email != '') {
        var validmail = validateEmail(email);
        if (!validmail) {
            $('#txt_email').addClass("error_msg");
            $('#txt_email').attr('title', 'Please enter valid Email Address');
            _continue = false;
        }
        else {
            $('#txt_email').removeClass("error_msg");
            $('#txt_email').attr('title', '');
        }
    }

    if ($('#txt_offManagerEmail').val().trim() != '') {
        var emailmainage = $('#txt_offManagerEmail').val().trim();
        var validmanemail = validateEmail(emailmainage);
        if (!validmanemail) {
            $('#txt_offManagerEmail').addClass("error_msg");
            $('#txt_offManagerEmail').attr('title', 'Please enter valid Email Address');
            _continue = false;
        }
        else {
            $('#txt_offManagerEmail').removeClass("error_msg");
            $('#txt_offManagerEmail').attr('title', '');
        }
    }

    if ($('#txt_efinowneremail').val().trim() != '') {
        var email1 = $('#txt_efinowneremail').val().trim();
        var isvalid = validateEmail(email1);
        if (!isvalid) {
            $('#txt_efinowneremail').addClass("error_msg");
            $('#txt_efinowneremail').attr('title', 'Please enter valid Email Address');
            _continue = false;
        }
        else {
            $('#txt_efinowneremail').removeClass("error_msg");
            $('#txt_efinowneremail').attr('title', '');
        }
    }

    var url = $('#txt_website').val().trim();
    if (url != '') {
        var validurl = ValidateWebURL(url);
        if (!validurl) {
            $('#txt_website').addClass("error_msg");
            $('#txt_website').attr('title', 'Please enter valid Website Address');
            _continue = false;
        }
        else {
            $('#txt_website').removeClass("error_msg");
            $('#txt_website').attr('title', '');
        }
    }

    if (!_continue) {
        $("html, body").animate({ scrollTop: 0 }, "slow");
        error.show();
        error.append('<p>  Please correct error(s). </p>');
        if ($('#dvoffice input.error_msg').length > 0 || $('#dvoffice select.error_msg').length > 0 || $('#dvoffice textarea.error_msg').length > 0 || $('#dvoffice div.error_msg').length > 0) {
            // $('#a_office').click();
            $('#a_office').parent('li').addClass('active');
            $('#dvoffice').addClass('active in');
            $('#a_fee').parent('li').removeClass('active');
            $('#dv_fee').removeClass('active in');
        }
        else if ($('#dvowner input.error_msg').length > 0 || $('#dvowner select.error_msg').length > 0 || $('#dvowner textarea.error_msg').length > 0 || $('#dvowner div.error_msg').length > 0) {
            $('#a_bowner').parent('li').addClass('active');
            $('#dvowner').addClass('active in');
            $('#a_fee').parent('li').removeClass('active');
            $('#dv_fee').removeClass('active in');
        }
        else if ($('#dvefinowner input.error_msg').length > 0 || $('#dvefinowner div.error_msg').length > 0 || $('#dvefinowner textarea.error_msg').length > 0 || $('#dvefinowner select.error_msg').length > 0) {
            $('#a_eowner').parent('li').addClass('active');
            $('#dvefinowner').addClass('active in');
            $('#a_fee').parent('li').removeClass('active');
            $('#dv_fee').removeClass('active in');
        }
        else if ($('#dvGeneral input.error_msg').length > 0 || $('#dvGeneral select.error_msg').length > 0 || $('#dvGeneral textarea.error_msg').length > 0 || $('#dvGeneral div.error_msg').length > 0) {
            $('#a_general').parent('li').addClass('active');
            $('#dvGeneral').addClass('active in');
            $('#a_fee').parent('li').removeClass('active');
            $('#dv_fee').removeClass('active in');
        }
        else if ($('#dvsqus input.error_msg').length > 0 || $('#dvsqus div.error_msg').length > 0 || $('#dvsqus textarea.error_msg').length > 0 || $('#dvsqus select.error_msg').length > 0) {
            $('#a_secQtns').parent('li').addClass('active');
            $('#dvsqus').addClass('active in');
            $('#a_fee').parent('li').removeClass('active');
            $('#dv_fee').removeClass('active in');
        }
        else if ($('#dv_bankinfo input.error_msg').length > 0 || $('#dv_bankinfo select.error_msg').length > 0 || $('#dv_bankinfo textarea.error_msg').length > 0 || $('#dv_bankinfo div.error_msg').length > 0) {
            $('#a_bank').parent('li').addClass('active');
            $('#dv_bankinfo').addClass('active in');
            $('#a_fee').parent('li').removeClass('active');
            $('#dv_fee').removeClass('active in');
        }
        else {
            $('#a_fee').parent('li').addClass('active');
            $('#dv_fee').addClass('active in');
            $('#a_bank').parent('li').removeClass('active');
            $('#dv_bankinfo').removeClass('active in');
            return;
        }
    }

    if (_continue) {
        var request = {};
        request.BankId = bankid;
        request.OfficeName = $('#txt_officename').val();
        request.OfficePhysicalAddress = $('#txt_officeaddress').val();
        request.OfficePhysicalCity = $('#txt_officecity').val();
        request.OfficePhysicalState = $('#ddl_officestate').val();
        request.OfficePhysicalZip = $('#txt_officezip').val();
        request.OfficeContactFirstName = $('#txt_offcntFN').val();
        request.OfficeContactLastName = $('#txt_offcntLN').val();
        request.OfficeContactSSN = $('#txt_offcntSSN').val();
        request.OfficePhoneNumber = $('#txt_officephone').val();
        request.CellPhoneNumber = $('#txt_cellphone').val();
        request.FAXNumber = $('#txt_faxnumber').val();
        request.EmailAddress = $('#txt_email').val();

        var OfficeContactDOB = $('#txt_offmngrDOB').val();
        if (OfficeContactDOB) {
            if (OfficeContactDOB.indexOf('/') > 0 && isValidDateFormat(OfficeContactDOB))
                request.OfficeContactDOB = OfficeContactDOB;
            else if (isValidDateFormat(getformattedDate(OfficeContactDOB)))
                request.OfficeContactDOB = getformattedDate(OfficeContactDOB);
        }
        //request.OfficeContactDOB = $('#txt_offContactDOB').val();
        request.OfficeManagerPhone = $('#txt_offmngrPhoneNo').val();
        request.OfficeManagerCellNo = $('#txt_offmngeCellPhone').val();
        request.OfficeManagerEmail = $('#txt_offManagerEmail').val();

        request.OfficeManagerFirstName = $('#txt_offmngrFN').val();
        request.OfficeManageLastName = $('#txt_offmngeLN').val();
        request.OfficeManagerSSN = $('#txt_offmngrSSN').val();

        var OfficeManagerDOB = $('#txt_offmngrDOB').val();
        if (OfficeManagerDOB) {
            if (OfficeManagerDOB.indexOf('/') > 0 && isValidDateFormat(OfficeManagerDOB))
                request.OfficeManagerDOB = OfficeManagerDOB;
            else if (isValidDateFormat(getformattedDate(OfficeManagerDOB)))
                request.OfficeManagerDOB = getformattedDate(OfficeManagerDOB);
        }
        //request.OfficeManagerDOB = $('#txt_offmngrDOB').val();
        request.AltOfficeContact1FirstName = $('#txt_altocFN1').val();
        request.AltOfficeContact1LastName = $('#txt_altocLN1').val();
        request.AltOfficeContact1Email = $('#txt_altoc1Email').val();
        request.AltOfficeContact1SSn = $('#txt_altoc1SSN').val();
        request.AltOfficeContact2FirstName = $('#txt_altocFN2').val();
        request.AltOfficeContact2LastName = $('#txt_altocLN2').val();
        request.AltOfficeContact2Email = $('#txt_altoc2Email').val();
        request.AltOfficeContact2SSn = $('#txt_altoc2SSN').val();
        request.AltOfficePhysicalAddress = $('#txt_altoffadd').val();
        request.AltOfficePhysicalAddress2 = $('#txt_altoffadd2').val();
        request.AltOfficePhysicalCity = $('#txt_altoffcity').val();
        request.AltOfficePhysicalState = $('#ddl_altofficestate').val();
        request.AltOfficePhysicalZipcode = $('#txt_altoffzip').val();
        request.MailingAddress = $('#txt_mailaddress').val();
        request.MailingCity = $('#txt_mailcity').val();
        request.MailingState = $('#ddl_mailstate').val();
        request.MailingZip = $('#txt_mailzip').val();
        request.FulfillmentShippingAddress = $('#txt_fulladdress').val();
        request.FulfillmentShippingCity = $('#txt_fullcity').val();
        request.FulfillmentShippingState = $('#ddl_fullstate').val();
        request.FulfillmentShippingZip = $('#txt_fullzip').val();
        request.WebsiteAddress = $('#txt_website').val();
        request.YearsinBusiness = $('#txt_yearsinbusiness').val();
        request.NoofBankProductsLastYear = $('#txt_noofbankprdcts').val();
        request.PreviousBankProductFacilitator = $('#ddl_prevbank').val();
        request.ActualNoofBankProductsLastYear = $('#txt_actnoofbankprdcts').val();
        request.OwnerFirstName = $('#txt_ownerFN').val();
        request.OwnerLastName = $('#txt_ownerLN').val();
        request.OwnerSSN = $('#txt_ownerSSN').val();
        var odob = $('#txt_ownerDOB').val();
        if (odob) {
            if (odob.indexOf('/') > 0)
                request.OnwerDOB = odob;
            else
                request.OnwerDOB = getformattedDate(odob);
        }
        //request.OnwerDOB = $('#txt_ownerDOB').val();
        request.OwnerHomePhone = $('#txt_ownerphone').val();
        request.OwnerAddress = $('#txt_owneraddress').val();
        request.OwnerCity = $('#txt_ownercity').val();
        request.OwnerState = $('#ddl_ownerstate').val();
        request.OwnerZip = $('#txt_ownerzip').val();
        request.LegarEntityStatus = $('#ddl_legalentity').val();
        request.LLCMembershipRegistration = $('#ddl_llcmembership').val();
        request.BusinessName = $('#txt_businessname').val();
        request.BusinessEIN = $('#txt_businessEIN').val();
        request.BusinessIncorporation = $('#txt_businessdate').val();
        request.EFINOwnerFirstName = $('#txt_efinownerFN').val();
        request.EFINOwnerLastName = $('#txt_efinownerLN').val();
        request.EFINOwnerSSN = $('#txt_efinownerSSN').val();

        var EFINOwnerDOB = $('#txt_efinownerDOB').val();
        if (EFINOwnerDOB) {
            if (EFINOwnerDOB.indexOf('/') > 0 && isValidDateFormat(EFINOwnerDOB))
                request.EFINOwnerDOB = EFINOwnerDOB;
            else if (isValidDateFormat(getformattedDate(EFINOwnerDOB)))
                request.EFINOwnerDOB = getformattedDate(EFINOwnerDOB);
        }
        //request.EFINOwnerDOB = $('#txt_efinownerDOB').val();
        request.IsMultiOffice = $('#ddl_multioffice').val();
        request.ProductsOffering = $('#ddl_productsoffering').val();
        request.IsOfficeTransmit = $('#ddl_locationtransmit').val();
        request.IsPTIN = $('#ddl_ptin').val();
        request.IsAsPerProcessLaw = $('#ddl_processlaw').val();
        request.IsAsPerComplainceLaw = $('#ddl_complaince').val();
        request.ConsumerLending = $('#ddl_consumerlending').val();
        request.NoofPersoneel = $('#txt_noofpersoneel').val();
        request.AdvertisingApproval = $('#ddl_advertiseapprvl').val();
        request.EROParticipation = $('#ddl_eroparticipation').val();
        request.SPAAmount = $('#txt_spaamount').val();
        request.SPADate = $('#txt_eroagreeddate').val();
        request.RetailPricingMethod = $('#ddl_retailmethod').val();
        request.IsLockedStore_Documents = $('#ddl_lockeddocs').val();
        request.IsLockedStore_Checks = $('#ddl_lockedcardschecks').val();
        request.IsLocked_Office = $('#ddl_lockedoffice').val();
        request.IsLimitAccess = $('#ddl_limitaccess').val();
        request.PlantoDispose = $('#ddl_plandispose').val();
        request.LoginAccesstoEmployees = $('#ddl_logintoemployees').val();
        request.AntivirusRequired = $('#ddl_antivirus').val();
        request.HasFirewall = $('#ddl_firewall').val();
        request.OnlineTraining = $('#ddl_onlinetraining').val();
        request.PasswordRequired = $('#ddl_pwdrqd').val();
        request.EROApplicattionDate = $('#txt_eroapplncmptd').val();
        request.EROReadTAndC = $('#ddl_eroreadrc').val();
        request.CheckingAccountName = $('#txt_accountname').val();
        request.BankRoutingNumber = $('#txt_bankroutingno').val();
        request.BankAccountNumber = $('#txt_bankaccountno').val();
        request.BankAccountType = $('#ddl_accounttype').val();
        request.UserId = $('#UserId').val();
        request.CustomerId = $('#UserId').val();

        request.EFINOwnerTitle = $('#txt_efintitle').val().trim();
        request.EFINOwnerAddress = $('#txt_efinownerAddress').val().trim();
        request.EFINOwnerCity = $('#txt_efinownercity').val().trim();
        request.EFINOwnerState = $('#ddl_efinownerstate').val();
        request.EFINOwnerZip = $('#txt_efinownerZip').val().trim();
        request.EFINOwnerPhone = $('#txt_efinownerphone').val().trim();
        request.EFINOwnerMobile = $('#txt_efinownermobile').val().trim();
        request.EFINOwnerEmail = $('#txt_efinowneremail').val().trim();
        request.EFINOwnerIDNumber = $('#txt_efinowneridnumber').val().trim();
        request.EFINOwnerIDState = $('#ddl_idstate').val();
        request.EFINOwnerEIN = $('#txt_efinownerein').val().trim();
        request.SupportOS = $('#ddl_wifipwd').val().trim();
        request.BankName = $('#txt_bankname').val().trim();
        request.SBFeeonAll = $('#chk_fbfee').prop('checked') ? 'X' : ' ';
        request.SBFee = $('#txt_sbfee').val().trim();
        request.TransimissionAddon = $('#txt_transmitfee').val().trim();
        request.PrepaidCardProgram = $('#ddl_cardprogram').val();
        request.TandC = true;
        var Cid = getUrlVars()["Id"];
        var entityid = $('#entityid').val();

        if (Cid) {
            request.CustomerId = Cid;
            entityid = getUrlVars()["entityid"];
        }

        var saveURI = '/api/EnrollmentBankSelection/SaveRBBankEnrollment';
        ajaxHelper(saveURI, 'POST', request, false).done(function (data, status) {
            if (data) {
                if (data.Status) {
                    $("html, body").animate({ scrollTop: 0 }, "slow");
                    success.show();
                    success.append('<p> Bank Enrollment details saved successfully.</p>');
                    //$('#site0feeb0fe-d0e7-4370-8733-dd5f7d2041fc').addClass('done');
                    SaveConfigStatusActive('done', bankid);
                    UpdateOfficeManagement(request.CustomerId)
                    if (type == 1) {

                        //if (Cid)
                        //    window.location.href = '../Enrollment/enrollmentsummary?Id=' + Cid;
                        //else
                        //    window.location.href = '../Enrollment/enrollmentsummary';

                        if ($('#entityid').val() != $('#myentityid').val()) {
                            if (entityid == $('#Entity_SO').val() || entityid == $('#Entity_SOME').val() || entityid == $('#Entity_SOME_SS').val()) {
                                var feeuri = '/api/EnrollmentBankSelection/getAddonSelection?CustomerId=' + request.CustomerId + '&bankid=' + bankid;
                                ajaxHelper(feeuri, 'GET').done(function (res) {
                                    if (res) {
                                        window.location.href = '/Enrollment/EnrollmentFeeReimbursement?Id=' + request.CustomerId + '&entitydisplayid=' + $('#myentitydisplayid').val() + '&ParentId=' + $('#myparentid').val() + '&entityid=' + $('#myentityid').val() + '&ptype=enrollment&bankid=' + bankid;
                                    }
                                    else {
                                        if ($('#site0eda5d25-591c-4e01-a845-fb580572cff5').length > 0) {
                                            window.location.href = '/PaymentOptions/efile?Id=' + request.CustomerId + '&entitydisplayid=' + $('#myentitydisplayid').val() + '&ParentId=' + $('#myparentid').val() + '&entityid=' + $('#myentityid').val() + '&ptype=enrollment&bankid=' + bankid;
                                        }
                                        else if ($('#site0eda5d25-591c-4e01-a845-fb580572cfe8').length > 0) {
                                            window.location.href = '/PaymentOptions/OutstandingBalance?Id=' + request.CustomerId + '&entitydisplayid=' + $('#myentitydisplayid').val() + '&ParentId=' + $('#myparentid').val() + '&entityid=' + $('#myentityid').val() + '&ptype=enrollment&bankid=' + bankid;
                                        }
                                        else
                                            window.location.href = '/Enrollment/enrollmentsummary?Id=' + request.CustomerId + '&entitydisplayid=' + $('#myentitydisplayid').val() + '&ParentId=' + $('#myparentid').val() + '&entityid=' + $('#myentityid').val() + '&ptype=enrollment&bankid=' + bankid;
                                    }
                                })
                            }
                            else {
                                if ($('#site0eda5d25-591c-4e01-a845-fb580572cff5').length > 0) {
                                    window.location.href = '/PaymentOptions/efile?Id=' + request.CustomerId + '&entitydisplayid=' + $('#myentitydisplayid').val() + '&ParentId=' + $('#myparentid').val() + '&entityid=' + $('#myentityid').val() + '&ptype=enrollment&bankid=' + bankid;
                                }
                                else if ($('#site0eda5d25-591c-4e01-a845-fb580572cfe8').length > 0) {
                                    window.location.href = '/PaymentOptions/OutstandingBalance?Id=' + request.CustomerId + '&entitydisplayid=' + $('#myentitydisplayid').val() + '&ParentId=' + $('#myparentid').val() + '&entityid=' + $('#myentityid').val() + '&ptype=enrollment&bankid=' + bankid;
                                }
                                else
                                    window.location.href = '/Enrollment/enrollmentsummary?Id=' + request.CustomerId + '&entitydisplayid=' + $('#myentitydisplayid').val() + '&ParentId=' + $('#myparentid').val() + '&entityid=' + $('#myentityid').val() + '&ptype=enrollment&bankid=' + bankid;
                            }
                        }
                        else {
                            if (entityid == $('#Entity_SO').val() || entityid == $('#Entity_SOME').val() || entityid == $('#Entity_SOME_SS').val()) {
                                var feeuri = '/api/EnrollmentBankSelection/getAddonSelection?CustomerId=' + request.CustomerId + '&bankid=' + bankid;
                                ajaxHelper(feeuri, 'GET').done(function (res) {
                                    if (res) {
                                        window.location.href = '/Enrollment/EnrollmentFeeReimbursement?bankid=' + bankid;
                                    }
                                    else {
                                        if ($('#site0eda5d25-591c-4e01-a845-fb580572cff5').length > 0) {
                                            window.location.href = '/PaymentOptions/efile?bankid=' + bankid;
                                        }
                                        else if ($('#site0eda5d25-591c-4e01-a845-fb580572cfe8').length > 0) {
                                            window.location.href = '/PaymentOptions/OutstandingBalance?bankid=' + bankid;
                                        }
                                        else
                                            window.location.href = '/Enrollment/enrollmentsummary?bankid=' + bankid;
                                    }
                                })
                            }
                            else {
                                if ($('#site0eda5d25-591c-4e01-a845-fb580572cff5').length > 0) {
                                    window.location.href = '/PaymentOptions/efile?bankid=' + bankid;
                                }
                                else if ($('#site0eda5d25-591c-4e01-a845-fb580572cfe8').length > 0) {
                                    window.location.href = '/PaymentOptions/OutstandingBalance?bankid=' + bankid;
                                }
                                else
                                    window.location.href = '/Enrollment/enrollmentsummary?bankid=' + bankid;
                            }
                        }
                    }


                    //if (type == 1) {
                    //    if ($('#entityid').val() != $('#myentityid').val()) {
                    //        if (entityid == $('#Entity_SO').val()) {
                    //            window.location.href = '/Enrollment/EnrollmentFeeReimbursement?Id=' + Cid + '&entitydisplayid=' + $('#myentitydisplayid').val() + '&ParentId=' + $('#myparentid').val() + '&entityid=' + $('#myentityid').val() + '&ptype=enrollment';
                    //        }
                    //        else {
                    //            window.location.href = '/Enrollment/enrollmentsummary?Id=' + Cid + '&entitydisplayid=' + $('#myentitydisplayid').val() + '&ParentId=' + $('#myparentid').val() + '&entityid=' + $('#myentityid').val() + '&ptype=enrollment';
                    //        }
                    //    }
                    //    else {
                    //        if (entityid == $('#Entity_SO').val()) {
                    //            window.location.href = '/Enrollment/EnrollmentFeeReimbursement';
                    //        }
                    //        else {
                    //            window.location.href = '/Enrollment/enrollmentsummary';
                    //        }
                    //    }
                    //}
                    if (type != 1)
                        getConfigStatus();
                }
                else {
                    $("html, body").animate({ scrollTop: 0 }, "slow");
                    error.show();
                    error.append('<p> Details not saved. </p>');
                    $.each(data.Messages, function (item, value) {
                        error.append('<p> ' + value + ' </p>');
                    })
                }
            }
            else {
                $("html, body").animate({ scrollTop: 0 }, "slow");
                error.show();
                error.append('<p> Details not saved. </p>');
            }
        });
    }
}

function ResetSuccessError() {
    success.html('');
    success.hide();
    error.html('');
    error.hide();
}

function onlyAlpha(event) {
    var arr = [8, 32, 46];
    for (var i = 65; i <= 90; i++) {
        arr.push(i);
    }
    for (var i = 96; i <= 123; i++) {
        arr.push(i);
    }
    if (jQuery.inArray(event.which, arr) === -1) {
        event.preventDefault();
    }
}

function Onlyalphanumaric(event) {
    var arr = [8, 32, 46];
    for (var i = 65; i <= 90; i++) {
        arr.push(i);
    }
    for (var i = 96; i <= 123; i++) {
        arr.push(i);
    }
    for (var i = 48; i <= 57; i++) {
        arr.push(i);
    }
    if (jQuery.inArray(event.which, arr) === -1) {
        event.preventDefault();
    }
}

function Onlydigit(event) {
    if (event.which != 99 && event.which != 8 && event.which != 0 && isNaN(String.fromCharCode(event.which)) || event.key.toLowerCase() == 'spacebar' || event.key == ' ') {
        event.preventDefault(); //stop characters from entering input
    }
}

function CompanyNameRegex(event) {
    var spl = [' ', '/', '.', '+', '&', '-', 'tab', 'backspace', 'home', 'end', 'spacebar', 'arrowleft', 'arrowright', 'delete', 'arrowup', 'arrowdown'];
    if (alphaArray.indexOf(event.key.toLowerCase()) == -1 && numberArray.indexOf(event.key) == -1 && spl.indexOf(event.key.toLowerCase()) == -1)
        event.preventDefault();
}

function removeCompanyName(obj) {
    var spl = [' ', '/', '.', '+', '&', '-'];
    var cn = $(obj).val();
    if (cn) {
        var chars = cn.split('');
        var newcn = '';
        for (i = 0; i < chars.length; i++) {
            if (spl.indexOf(chars[i]) > -1 || alphaArray.indexOf(chars[i].toLowerCase()) > -1 || numberArray.indexOf(chars[i]) > -1)
                newcn += chars[i];
        }
        $(obj).val(newcn);
    }
}

function removeSplCompany(value) {
    var spl = [' ', '/', '.', '+', '&', '-'];
    if (value) {
        var chars = value.split('');
        var newcn = '';
        for (i = 0; i < chars.length; i++) {
            if (spl.indexOf(chars[i]) > -1 || alphaArray.indexOf(chars[i].toLowerCase()) > -1 || numberArray.indexOf(chars[i]) > -1)
                newcn += chars[i];
        }
        return newcn;
    }
    return '';
}

function NameRegex(event) {
    var spl = [' ', '/', '-', 'tab', 'backspace', 'home', 'end', 'spacebar', 'arrowleft', 'arrowright', 'delete', 'arrowup', 'arrowdown'];
    if (alphaArray.indexOf(event.key.toLowerCase()) == -1 && spl.indexOf(event.key.toLowerCase()) == -1)
        event.preventDefault();
}

function removeName(obj) {
    var spl = [' ', '/', '-'];
    var cn = $(obj).val();
    if (cn) {
        var chars = cn.split('');
        var newcn = '';
        for (i = 0; i < chars.length; i++) {
            if (spl.indexOf(chars[i]) > -1 || alphaArray.indexOf(chars[i].toLowerCase()) > -1)
                newcn += chars[i];
        }
        $(obj).val(newcn);
    }
}

function removeSplName(value) {
    var spl = [' ', '/', '-'];
    if (value) {
        var chars = value.split('');
        var newcn = '';
        for (i = 0; i < chars.length; i++) {
            if (spl.indexOf(chars[i]) > -1 || alphaArray.indexOf(chars[i].toLowerCase()) > -1)
                newcn += chars[i];
        }
        return newcn;
    }
    return '';
}

function AddressRegex(event) {
    
    var spl = [' ', '&', '-', '/', ',', '.', '%', 'tab', 'backspace', 'home', 'end', 'spacebar', 'arrowleft', 'arrowright', 'delete', 'arrowup', 'arrowdown'];
    if (alphaArray.indexOf(event.key.toLowerCase()) == -1 && numberArray.indexOf(event.key) == -1 && spl.indexOf(event.key.toLowerCase()) == -1)
        event.preventDefault();
}

function removeAddress(obj) {
    var spl = [' ', '&', '-', '/', ',', '.', '%'];
    var cn = $(obj).val();
    if (cn) {
        var chars = cn.split('');
        var newcn = '';
        for (i = 0; i < chars.length; i++) {
            if (spl.indexOf(chars[i]) > -1 || alphaArray.indexOf(chars[i].toLowerCase()) > -1 || numberArray.indexOf(chars[i]) > -1)
                newcn += chars[i];
        }
        $(obj).val(newcn);
    }
}

function removeSplAddress(value) {
    var spl = [' ', '&', '-', '/', ',', '.', '%'];
    if (value) {
        var chars = value.split('');
        var newcn = '';
        for (i = 0; i < chars.length; i++) {
            if (spl.indexOf(chars[i]) > -1 || alphaArray.indexOf(chars[i].toLowerCase()) > -1 || numberArray.indexOf(chars[i]) > -1)
                newcn += chars[i];
        }
        return newcn;
    }
    return '';
}

function CityRegex(event) {
    var spl = [' ', 'tab', 'backspace', 'home', 'end', 'spacebar', 'arrowleft', 'arrowright', 'delete', 'arrowup', 'arrowdown'];
    if (alphaArray.indexOf(event.key.toLowerCase()) == -1 && spl.indexOf(event.key.toLowerCase()) == -1)
        event.preventDefault();
}

function removeCity(obj) {
    var spl = [' '];
    var cn = $(obj).val();
    if (cn) {
        var chars = cn.split('');
        var newcn = '';
        for (i = 0; i < chars.length; i++) {
            if (spl.indexOf(chars[i]) > -1 || alphaArray.indexOf(chars[i].toLowerCase()) > -1)
                newcn += chars[i];
        }
        $(obj).val(newcn);
    }
}

function removeSplCity(value) {
    var spl = [' '];
    if (value) {
        var chars = value.split('');
        var newcn = '';
        for (i = 0; i < chars.length; i++) {
            if (spl.indexOf(chars[i]) > -1 || alphaArray.indexOf(chars[i].toLowerCase()) > -1)
                newcn += chars[i];
        }
        return newcn;
    }
    return '';
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

function getBankEnrollmentStatus(CustomerId, bankid) {
    var url = '/api/EnrollmentBankSelection/getBankEnrollmentStatus?CustomerId=' + CustomerId + '&bankid=' + bankid;
    ajaxHelper(url, 'GET', null, false).done(function (data) {
        if (data) {
            
            switch (data.SubmissionStaus) {
                case 'RDY':
                    //Fn_DeactivateSelection();
                    break;
                case 'SUB':
                    bankstatus = data.SubmissionStaus;
                    Fn_DisableAll();
                    break;
                case 'APR':
                    bankstatus = data.SubmissionStaus;
                    Fn_DeactivateSelection();
                    $('#p_approvedContent').show();
                    break;
                case 'REJ':
                case 'CAN':
                    bankstatus = data.SubmissionStaus;
                    break;
                case 'PEN':
                    bankstatus = data.SubmissionStaus;
                    Fn_DisableAll();
                    break;
                case 'DEN':
                    bankstatus = data.SubmissionStaus;
                    break;
                default:
                    break;
            }


            if (data.SubmissionCount > 0) {
                $('input').attr('disabled', 'disabled');
                $('select').attr('disabled', 'disabled');
                $('textarea').attr('disabled', 'disabled');
                $('a.btn-info').css('pointer-events', 'none');
                $('a.btn-info').attr('disabled', 'disabled');
                $('.btn-nextprev').css('pointer-events', 'all').removeAttr('disabled');
            }
        }
    })
}

function Fn_DisableAll() {
    $('input').attr('disabled', 'disabled');
    $('select').attr('disabled', 'disabled');
    $('textarea').attr('disabled', 'disabled');
    $('a.submitlock').attr('pointer-events', 'none').attr('disabled', 'disabled');
}

function Fn_DeactivateSelection() {
    $('.submitlock').attr('disabled', 'disabled');
    //$('a.submitlock').attr('pointer-events', 'none');
}

function fnPopupEFINOwnerInfo() {
    if (EFINOwnerInfo.length == 3) {
        return;
    }
    $("#txt_modalFN").val('');
    $("#txt_modalLN").val('');
    $("#txt_modalDOB").val('');
    $("#txt_modalphone").val('');
    $("#txt_modalSSN").val('');
    $("#txt_modalAddress").val('');
    $("#txt_modalCity").val('');
    $("#ddl_modalstate").val(0);
    $("#txt_modalzip").val('');
    $("#txt_modalidnumber").val('');
    $("#ddl_modalidstate").val(0);
    $('#txt_ownerpercent').val('');
    $('#BankEnrollRA').modal('show');
    editIndex = -1;
}

var EFINOwnerInfo = [];
var editIndex = -1;

function fnPopupEFINOwnerInfo_Save() {

    var _continue = true;

    if ($('#txt_modalFN').val().trim() == '') {
        $('#txt_modalFN').addClass("error_msg");
        $('#txt_modalFN').attr('title', 'Please enter First Name');
        _continue = false;
    }
    else {
        $('#txt_modalFN').removeClass("error_msg");
        $('#txt_modalFN').attr('title', '');
    }

    if ($('#txt_modalLN').val().trim() == '') {
        $('#txt_modalLN').addClass("error_msg");
        $('#txt_modalLN').attr('title', 'Please enter Last Name');
        _continue = false;
    }
    else {
        $('#txt_modalLN').removeClass("error_msg");
        $('#txt_modalLN').attr('title', '');
    }

    if ($('#txt_modalDOB').val().trim() == '') {
        $('#txt_modalDOB').addClass("error_msg");
        $('#txt_modalDOB').attr('title', 'Please enter Date of Birth');
        _continue = false;
    }
    else {
        $('#txt_modalDOB').removeClass("error_msg");
        $('#txt_modalDOB').attr('title', '');
    }

    if ($('#txt_modalDOB').val().trim() != '') {
        var dob = $('#txt_modalDOB').val().trim();
        var d = new Date();
        d.setYear(d.getFullYear() - 18);

        if (dob.indexOf('/') > 0) {
            if (!isValidDateFormat(dob)) {
                $('#txt_modalDOB').addClass("error_msg");
                $('#txt_modalDOB').attr('title', 'Please enter valid EFIN Owner\'s Date of Birth');
                _continue = false;
            }
            else {
                $('#txt_modalDOB').removeClass("error_msg");
                $('#txt_modalDOB').attr('title', '');
                var dd = new Date(dob);
                if (dd > d) {
                    $('#txt_modalDOB').addClass("error_msg");
                    $('#txt_modalDOB').attr('title', 'EFIN Owner\'s Date of Birth must be 18 years ago');
                    _continue = false;
                }
            }
        }
        else {
            var pos = 2, yearpos = 5;
            var b = '/';
            var updob = [dob.slice(0, pos), b, dob.slice(pos)].join('');
            var _updob = [updob.slice(0, yearpos), b, updob.slice(yearpos)].join('');
            if (!isValidDateFormat(_updob)) {
                $('#txt_modalDOB').addClass("error_msg");
                $('#txt_modalDOB').attr('title', 'Please enter valid EFIN Owner\'s Date of Birth');
                _continue = false;
            }
            else {
                $('#txt_modalDOB').removeClass("error_msg");
                $('#txt_modalDOB').attr('title', '');
                var dd = new Date(_updob);
                if (dd > d) {
                    $('#txt_modalDOB').addClass("error_msg");
                    $('#txt_modalDOB').attr('title', 'EFIN Owner\'s Date of Birth must be 18 years ago');
                    _continue = false;
                }
            }
        }
    }

    if ($('#txt_modalphone').val().trim() == '') {
        $('#txt_modalphone').addClass("error_msg");
        $('#txt_modalphone').attr('title', 'Please enter Phone Number');
        _continue = false;
    }
    else {
        $('#txt_modalphone').removeClass("error_msg");
        $('#txt_modalphone').attr('title', '');
    }

    if ($('#txt_modalSSN').val().trim() == '') {
        $('#txt_modalSSN').addClass("error_msg");
        $('#txt_modalSSN').attr('title', 'Please enter SSN');
        _continue = false;
    }
    else {
        $('#txt_modalSSN').removeClass("error_msg");
        $('#txt_modalSSN').attr('title', '');
    }

    if ($('#txt_modalSSN').val().trim() != '') {
        var len = $('#txt_modalSSN').val().length;
        if (len < 9) {
            $('#txt_modalSSN').addClass("error_msg");
            $('#txt_modalSSN').attr('title', 'Please enter valid 9 digit SSN');
            _continue = false;
        }
        else {
            $('#txt_modalSSN').removeClass("error_msg");
            $('#txt_modalSSN').attr('title', '');
        }
    }

    if ($('#txt_modalAddress').val().trim() == '') {
        $('#txt_modalAddress').addClass("error_msg");
        $('#txt_modalAddress').attr('title', 'Please enter Address');
        _continue = false;
    }
    else {
        $('#txt_modalAddress').removeClass("error_msg");
        $('#txt_modalAddress').attr('title', '');
    }

    if ($('#txt_modalAddress').val().trim() != '') {
        if (isPOExist($('#txt_modalAddress').val())) {
            $('#txt_modalAddress').addClass("error_msg");
            $('#txt_modalAddress').attr('title', 'Address should not contain PO');
            _continue = false;
        }
        else {
            $('#txt_modalAddress').removeClass("error_msg");
            $('#txt_modalAddress').attr('title', '');
        }
    }

    if ($('#txt_modalCity').val().trim() == '') {
        $('#txt_modalCity').addClass("error_msg");
        $('#txt_modalCity').attr('title', 'Please enter City');
        _continue = false;
    }
    else {
        $('#txt_modalCity').removeClass("error_msg");
        $('#txt_modalCity').attr('title', '');
    }

    if ($('#ddl_modalstate').val() == '' || $('#ddl_modalstate').val() == undefined || $('#ddl_modalstate').val() == '0') {
        $('#ddl_modalstate').addClass("error_msg");
        $('#ddl_modalstate').attr('title', 'Please select State');
        _continue = false;
    }
    else {
        $('#ddl_modalstate').removeClass("error_msg");
        $('#ddl_modalstate').attr('title', '');
    }

    if ($('#txt_modalzip').val().trim() == '') {
        $('#txt_modalzip').addClass("error_msg");
        $('#txt_modalzip').attr('title', 'Please enter Zip code');
        _continue = false;
    }
    else {
        $('#txt_modalzip').removeClass("error_msg");
        $('#txt_modalzip').attr('title', '');
    }

    if ($('#txt_modalzip').val() != '') {
        var zip = $('#txt_modalzip').val();
        if (!(zip.length == 5 || zip.length == 9)) {
            $('#txt_modalzip').addClass("error_msg");
            $('#txt_modalzip').attr('title', 'Please enter valid Zip code');
            _continue = false;
        }
        else {
            $('#txt_modalzip').removeClass("error_msg");
            $('#txt_modalzip').attr('title', '');
        }
    }

    if ($('#txt_modalidnumber').val().trim() == '') {
        $('#txt_modalidnumber').addClass("error_msg");
        $('#txt_modalidnumber').attr('title', 'Please enter ID Number');
        _continue = false;
    }
    else {
        $('#txt_modalidnumber').removeClass("error_msg");
        $('#txt_modalidnumber').attr('title', '');
    }

    if ($('#ddl_modalidstate').val() == '' || $('#ddl_modalidstate').val() == undefined || $('#ddl_modalidstate').val() == '0') {
        $('#ddl_modalidstate').addClass("error_msg");
        $('#ddl_modalidstate').attr('title', 'Please select ID State');
        _continue = false;
    }
    else {
        $('#ddl_modalidstate').removeClass("error_msg");
        $('#ddl_modalidstate').attr('title', '');
    }

    if ($('#txt_ownerpercent').val().trim() == '') {
        $('#txt_ownerpercent').addClass("error_msg");
        $('#txt_ownerpercent').attr('title', 'Please enter Percentage owned');
        _continue = false;
    }
    else {
        $('#txt_ownerpercent').removeClass("error_msg");
        $('#txt_ownerpercent').attr('title', '');
    }

    if (!_continue) {
        return;
    }

    var FirstName = $("#txt_modalFN").val();
    var LastName = $("#txt_modalLN").val();
    var Email = '';
    var DOB = '';
    var odob = $('#txt_modalDOB').val().trim();
    if (odob) {
        if (odob.indexOf('/') > 0)
            DOB = odob;
        else
            DOB = getformattedDate(odob);
    }
    //var DOB = $("#txt_modalDOB").val();
    var HomePhone = $("#txt_modalphone").val();
    var MobilePhone = '';
    var SSN = $("#txt_modalSSN").val();
    var Address = $("#txt_modalAddress").val();
    var City = $("#txt_modalCity").val();
    var State = $("#ddl_modalstate").val();
    var Zip = $("#txt_modalzip").val();
    var IDNumber = $("#txt_modalidnumber").val();
    var IDState = $("#ddl_modalidstate").val();
    var Percentage = $('#txt_ownerpercent').val();

    if (editIndex == -1) {
        var index = EFINOwnerInfo.length;

        EFINOwnerInfo.push({
            BankEnrollmentRAId: '',
            FirstName: FirstName,
            LastName: LastName,
            EmailId: Email,
            DateofBirth: DOB,
            HomePhone: HomePhone,
            MobilePhone: MobilePhone,
            SSN: SSN,
            Address: Address,
            City: City,
            StateId: State,
            ZipCode: Zip,
            IDNumber: IDNumber,
            IDState: IDState,
            PercentageOwned: Percentage
        });
    }
    else {
        var info = EFINOwnerInfo[editIndex];
        if (info) {
            info.FirstName = FirstName;
            info.LastName = LastName;
            info.EmailId = Email;
            info.DateofBirth = DOB;
            info.HomePhone = HomePhone;
            info.MobilePhone = MobilePhone;
            info.SSN = SSN;
            info.Address = Address;
            info.City = City;
            info.StateId = State;
            info.ZipCode = Zip;
            info.IDNumber = IDNumber;
            info.IDState = IDState;
            info.PercentageOwned = Percentage;
        }
    }

    fnEFINOwnerInfo_tableBind();
    $('#BankEnrollRA').modal('hide');
}

function fnEFINOwnerInfo_tableBind() {

    fnEFINOwnerInfo_table(EFINOwnerInfo);
    $('#BankEnrollRA').modal('hide');
    var totalpercentage = 0;
    for (var i = 0; i < EFINOwnerInfo.length; i++) {
        totalpercentage = totalpercentage + parseInt(EFINOwnerInfo[i].PercentageOwned);
    }
    $('#btnAddEFINOwner').removeAttr('disabled');
    $('#btnAddEFINOwner').css('pointer-events', '');
    $('#btnAddEFINOwner').css('display', 'inline-block');
    if (totalpercentage >= 100 || EFINOwnerInfo.length >= 3) {
        $('#btnAddEFINOwner').attr('disabled', 'disabled');
        $('#btnAddEFINOwner').css('pointer-events', 'none');
        $('#btnAddEFINOwner').css('display', 'none');
    }
}

function fnEFINOwnerInfo_table(EFINOwnerInfo) {
    // var tbl_EFINOwnerInfo = $('#tbl_EFINOwnerInfo');
    //var tbody = $(tbl_EFINOwnerInfo).append('<tbody/>')
    //$.each(EFINOwenerInfo, function (indx, valu) {
    //    alert(valu[0].FirstName);
    //});

    var totalpercentage = 0;
    for (var i = 0; i < EFINOwnerInfo.length; i++) {
        totalpercentage = totalpercentage + parseInt(EFINOwnerInfo[i].PercentageOwned);
    }
    $('#btnAddEFINOwner').removeAttr('disabled');
    $('#btnAddEFINOwner').css('pointer-events', '');
    $('#btnAddEFINOwner').css('display', 'inline-block');
    if (totalpercentage >= 100 || EFINOwnerInfo.length >= 3) {
        $('#btnAddEFINOwner').attr('disabled', 'disabled');
        $('#btnAddEFINOwner').css('pointer-events', 'none');
        $('#btnAddEFINOwner').css('display', 'none');
    }


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
        row.append($("<td/>").html('<a class="submitlock" style="cursor:pointer;font-size:18px;" onclick="editOwner(' + i + ')"><i class="fa fa-pencil-square" aria-hidden="true"></i></a> <a class="submitlock" style="cursor:pointer;font-size:18px;" onclick="removeOwner(' + i + ')"><i class="fa fa-times-circle" aria-hidden="true"></i></a>'));
        row.appendTo(tbody);
    }

}

function editOwner(i) {
    editIndex = i;
    var info = EFINOwnerInfo[i];
    if (info) {
        $("#txt_modalFN").val(info.FirstName);
        $("#txt_modalLN").val(info.LastName);
        $('#txt_modalDOB').val(info.DateofBirth);
        $("#txt_modalphone").val(info.HomePhone);
        $("#txt_modalSSN").val(info.SSN);
        $("#txt_modalAddress").val(info.Address);
        $("#txt_modalCity").val(info.City);
        $("#ddl_modalstate").val(info.StateId);
        $("#txt_modalzip").val(info.ZipCode);
        $("#txt_modalidnumber").val(info.IDNumber);
        $("#ddl_modalidstate").val(info.IDState);
        $('#txt_ownerpercent').val(info.PercentageOwned);
        $('#BankEnrollRA').modal('show');
    }
}

function removeOwner(i) {
    EFINOwnerInfo.splice(i, 1);
    fnEFINOwnerInfo_table(EFINOwnerInfo);
    var totalpercentage = 0;
    for (var i = 0; i < EFINOwnerInfo.length; i++) {
        totalpercentage = totalpercentage + parseInt(EFINOwnerInfo[i].PercentageOwned);
    }
    $('#btnAddEFINOwner').removeAttr('disabled');
    $('#btnAddEFINOwner').css('pointer-events', '');
    $('#btnAddEFINOwner').css('display', 'inline-block');
    if (totalpercentage >= 100) {
        $('#btnAddEFINOwner').attr('disabled', 'disabled');
        $('#btnAddEFINOwner').css('pointer-events', 'none');
        $('#btnAddEFINOwner').css('display', 'none');
    }
}

function saveTPGNextTab(level) {

    if (bankstatus == 'APR' || bankstatus == 'SUB' || bankstatus == 'PEN')
        return;

    var bankid = getUrlVars()["bankid"];

    var requuest = {};
    requuest.BankId = bankid;
    requuest.CompanyName = $('#txt_CompanyName').val().trim();
    requuest.ManagerFirstName = $('#txt_ManagerFN').val().trim();
    requuest.ManagerLastName = $('#txt_ManagerLN').val().trim();
    requuest.OfficeAddress = $('#txt_OfficeAddress').val().trim();
    requuest.OfficeCity = $('#txt_OfficeCity').val().trim();
    requuest.OfficeZip = $('#txt_OfficeZip').val().trim();
    requuest.OfficeTelephone = $('#txt_OfficeTel').val().trim();
    requuest.OfficeFax = $('#txt_OfficeFax').val().trim();
    requuest.ShippingAddress = $('#txt_ShipAddress').val().trim();
    requuest.ShippingCity = $('#txt_ShipCity').val().trim();
    requuest.ShippingZip = $('#txt_ShipZip').val().trim();
    requuest.ManagerEmail = $('#txt_ManagerEmail').val().trim();
    requuest.OwnerEIN = $('#txt_OwnerEin').val().trim();
    requuest.OwnerSSn = $('#txt_OwnerSSN').val().trim();
    requuest.OwnerFirstName = $('#txt_OwnerFN').val().trim();
    requuest.OwnerLastName = $('#txt_OwnerLN').val().trim();
    requuest.OwnerAddress = $('#txt_OwnerAddress').val().trim();
    requuest.OwnerCity = $('#txt_OwnerCity').val().trim();
    requuest.OwnerZip = $('#txt_OwnerZip').val().trim();
    requuest.OwnerTelephone = $('#txt_OwnerTel').val().trim();
    var odob = $('#txt_ownerDOB').val();
    if (odob) {
        if (odob.indexOf('/') > 0 && isValidDateFormat(odob))
            requuest.OnwerDOB = odob;
        else if (isValidDateFormat(getformattedDate(odob)))
            requuest.OnwerDOB = getformattedDate(odob);
    }
    //requuest.OwnerDOB = $('#txt_OwnerDOB').val().trim();
    requuest.OwnerEmail = $('#txt_OwnerEmail').val().trim();
    requuest.LastYearVolume = $('#txt_LYVolume').val().trim();
    requuest.LastYearEFIN = $('#txt_EFINLY').val().trim();
    requuest.BankProductFund = $('#txt_BPF').val().trim();
    requuest.OfficeRTN = $('#txt_OfficeRTN').val().trim();
    requuest.OfficeDAN = $('#txt_OfficeDAN').val().trim();
    requuest.AccountType = $('#ddl_accttype').val();
    requuest.LastYearBank = $('#ddl_BankLY').val();
    requuest.OwnerState = $('#ddl_ownerstate').val();
    requuest.ShippingState = $('#ddl_shipstate').val();
    requuest.OfficeState = $('#ddl_officestate').val();
    requuest.UserId = $('#UserId').val();
    requuest.CustomerId = $('#UserId').val();
    requuest.EfinOwnerTitle = $('#txt_OwnerTitle').val();
    requuest.EfinOwnerMobile = $('#txt_OwnerMobile').val();
    requuest.EfinIDNumber = $('#txt_OwnerIDNumber').val();
    requuest.EfinIdState = $('#ddl_owneridstate').val();
    requuest.Addonfee = $('#txt_addonfee').val();
    requuest.ServiceBureaufee = $('#txt_sbfee').val();
    requuest.CheckPrint = 'R';
    requuest.AgreeBank = $('#chk_agreebank').prop('checked');
    requuest.SbfeeAll = $('#chk_feeall').prop('checked') ? 'X' : ' ';
    requuest.DocPrepFee = $('#txt_docprep').val();
    requuest.BankName = $('#txt_bankname').val();
    requuest.AccountName = $('#txt_accountname').val();
    requuest.EntryLevel = level;

    var Cid = getUrlVars()["Id"];
    var entitydisplayid = $('#entitydisplayid').val();
    if (Cid) {
        requuest.CustomerId = Cid;
        entitydisplayid = getUrlVars()["entitydisplayid"];
    }

    var saveURI = '/api/EnrollmentBankSelection/SaveNextTPGBankEnrollment';
    ajaxHelper(saveURI, 'POST', requuest).done(function (data, status) { });
}

function TPGGeneralNext() {

    //$('*').removeClass('error_msg');
    //$('*').attr('title', '');

    if ($('#txt_OfficeTel').val() != '') {
        var number = $('#txt_OfficeTel').val();
        if (number.toLowerCase().indexOf('x') > -1) {
            $('#txt_OfficeTel').addClass("error_msg");
            $('#txt_OfficeTel').attr('title', 'Please enter valid Office Telephone Number');
            return;
        }
        else {
            $('#txt_OfficeTel').removeClass("error_msg");
            $('#txt_OfficeTel').attr('title', '');
        }
    }

    saveTPGNextTab(1);
    $('#libOfficeInfo').removeClass('active');
    $('#libOfficeConfig').addClass('active');
    $('#dvGeneral, #libOfficeInfo').removeClass('active in');
    $('#dvEfin_ab, #libOfficeConfig').addClass('active in');
}

function TPGEfinOwnerPrev() {
    $('#libOfficeInfo').addClass('active');
    $('#libOfficeConfig').removeClass('active');
    $('#dvGeneral').addClass('active in');
    $('#dvEfin_ab').removeClass('active in');
}

function TPGEfinOwnerNext() {
    //$('*').removeClass('error_msg');
    //$('*').attr('title', '');

    if ($('#txt_OwnerTel').val() != '') {
        var number = $('#txt_OwnerTel').val();
        if (number.toLowerCase().indexOf('x') > -1) {
            $('#txt_OwnerTel').addClass("error_msg");
            $('#txt_OwnerTel').attr('title', 'Please enter valid EFIN Owner\'s Telephone Number');
            return;
        }
        else {
            $('#txt_OwnerTel').removeClass("error_msg");
            $('#txt_OwnerTel').attr('title', '');
        }
    }

    saveTPGNextTab(2);
    $('#liaffiliate').addClass('active');
    $('#libOfficeConfig').removeClass('active');
    $('#dvFee').addClass('active in');
    $('#dvEfin_ab').removeClass('active in');
}

function TPGFeePrev() {
    $('#libOfficeConfig').addClass('active');
    $('#liaffiliate').removeClass('active');
    $('#dvEfin_ab').addClass('active in');
    $('#dvFee').removeClass('active in');
}

function TPGFeeNext() {
    saveTPGNextTab(3);
    $('#lifee').addClass('active');
    $('#liaffiliate').removeClass('active');
    $('#dvPrior').addClass('active in');
    $('#dvFee').removeClass('active in');
}

function TPGPriorPrev() {
    $('#liaffiliate').addClass('active');
    $('#lifee').removeClass('active');
    $('#dvFee').addClass('active in');
    $('#dvPrior').removeClass('active in');
}

function TPGPriorNext() {
    saveTPGNextTab(4);
    $('#libankselection').addClass('active');
    $('#lifee').removeClass('active');
    $('#dvBank').addClass('active in');
    $('#dvPrior').removeClass('active in')
}

function TPGBankPrev() {
    $('#lifee').addClass('active');
    $('#libankselection').removeClass('active');
    $('#dvPrior').addClass('active in');
    $('#dvBank').removeClass('active in');
}
//////RB Office Functions ////
function RBOfficeNext() {
    var _cango = true;
    //$('*').removeClass('error_msg');
    //$('*').attr('title', '');

    if ($('#txt_officephone').val() != '') {
        var number = $('#txt_officephone').val();
        if (number.toLowerCase().indexOf('x') > -1) {
            $('#txt_officephone').addClass("error_msg");
            $('#txt_officephone').attr('title', 'Please enter valid Office Phone Number');
            _cango = false;
        }
        else {
            $('#txt_officephone').removeClass("error_msg");
            $('#txt_officephone').attr('title', '');
        }
    }

    if ($('#txt_cellphone').val() != '') {
        var number = $('#txt_cellphone').val();
        if (number.toLowerCase().indexOf('x') > -1) {
            $('#txt_cellphone').addClass("error_msg");
            $('#txt_cellphone').attr('title', 'Please enter valid Cell Phone Number');
            _cango = false;
        }
        else {
            $('#txt_cellphone').removeClass("error_msg");
            $('#txt_cellphone').attr('title', '');
        }
    }

    if ($('#txt_offmngrPhoneNo').val() != '') {
        var number = $('#txt_offmngrPhoneNo').val();
        if (number.toLowerCase().indexOf('x') > -1) {
            $('#txt_offmngrPhoneNo').addClass("error_msg");
            $('#txt_offmngrPhoneNo').attr('title', 'Please enter valid Manager Phone Number');
            _cango = false;
        }
        else {
            $('#txt_offmngrPhoneNo').removeClass("error_msg");
            $('#txt_offmngrPhoneNo').attr('title', '');
        }
    }

    $('#txt_offmngeCellPhone').removeClass("error_msg");
    $('#txt_offmngeCellPhone').attr('title', '');
    if ($('#txt_offmngeCellPhone').val() != '') {
        var number = $('#txt_offmngeCellPhone').val();
        if (number.toLowerCase().indexOf('x') > -1) {
            $('#txt_offmngeCellPhone').addClass("error_msg");
            $('#txt_offmngeCellPhone').attr('title', 'Please enter valid Manager Cell Phone Number');
            _cango = false;
        }
    }

    if (!_cango)
        return;

    saveNextRBBankEnrollment(0, 1);
    $('#a_bowner').parent('li').addClass('active');
    $('#a_office').parent('li').removeClass('active');
    $('#dvowner').addClass('active in');
    $('#dvoffice').removeClass('active in');
}

function RBBONext() {
    //$('*').removeClass('error_msg');
    //$('*').attr('title', '');

    $('#txt_ownerphone').removeClass("error_msg");
    $('#txt_ownerphone').attr('title', '');
    if ($('#txt_ownerphone').val() != '') {
        var number = $('#txt_ownerphone').val();
        if (number.toLowerCase().indexOf('x') > -1) {
            $('#txt_ownerphone').addClass("error_msg");
            $('#txt_ownerphone').attr('title', 'Please enter valid Owner Home Phone');
            return;
        }
    }

    saveNextRBBankEnrollment(0, 2);

    $('#a_eowner').parent('li').addClass('active');
    $('#a_bowner').parent('li').removeClass('active');

    $('#dvefinowner').addClass('active in');
    $('#dvowner').removeClass('active in');
}

function RBBOPrev() {
    $('#a_bowner').parent('li').removeClass('active');
    $('#a_office').parent('li').addClass('active');
    $('#dvowner').removeClass('active in');
    $('#dvoffice').addClass('active in');
}

function RBEONext() {
    var _cango = true;
    //$('*').removeClass('error_msg');
    //$('*').attr('title', '');

    $('#txt_efinownerphone').removeClass("error_msg");
    $('#txt_efinownerphone').attr('title', '');
    if ($('#txt_efinownerphone').val() != '') {
        var number = $('#txt_efinownerphone').val();
        if (number.toLowerCase().indexOf('x') > -1) {
            $('#txt_efinownerphone').addClass("error_msg");
            $('#txt_efinownerphone').attr('title', 'Please enter valid EFIN Holder Phone');
            _cango = false;
        }
    }
    $('#txt_efinownermobile').removeClass("error_msg");
    $('#txt_efinownermobile').attr('title', '');
    if ($('#txt_efinownermobile').val() != '') {
        var number = $('#txt_efinownermobile').val();
        if (number.toLowerCase().indexOf('x') > -1) {
            $('#txt_efinownermobile').addClass("error_msg");
            $('#txt_efinownermobile').attr('title', 'Please enter valid EFIN Holder Mobile');
            _cango = false;
        }
    }

    if (!_cango)
        return;

    saveNextRBBankEnrollment(0, 3);
    $('#a_eowner').parent('li').removeClass('active');
    $('#a_general').parent('li').addClass('active');
    $('#dvefinowner').removeClass('active in');
    $('#dvGeneral').addClass('active in');
}

function RBEOPrev() {
    $('#a_bowner').parent('li').addClass('active');
    $('#a_eowner').parent('li').removeClass('active');
    $('#dvowner').addClass('active in');
    $('#dvefinowner').removeClass('active in');
}

function RBGenNext() {
    saveNextRBBankEnrollment(0, 4);
    $('#a_general').parent('li').removeClass('active');
    $('#a_secQtns').parent('li').addClass('active');
    $('#dvGeneral').removeClass('active in');
    $('#dvsqus').addClass('active in');
}

function RBGenPrev() {
    $('#a_eowner').parent('li').addClass('active');
    $('#a_general').parent('li').removeClass('active');
    $('#dvefinowner').addClass('active in');
    $('#dvGeneral').removeClass('active in');
}

function RBSQNext() {
    saveNextRBBankEnrollment(0, 5);
    $('#a_bank').parent('li').addClass('active');
    $('#a_secQtns').parent('li').removeClass('active');
    $('#dv_bankinfo').addClass('active in');
    $('#dvsqus').removeClass('active in');
}

function RBSQPrev() {
    $('#a_secQtns').parent('li').removeClass('active');
    $('#a_general').parent('li').addClass('active');
    $('#dvsqus').removeClass('active in');
    $('#dvGeneral').addClass('active in');
}

function RBBankNext() {
    saveNextRBBankEnrollment(0, 6);
    $('#a_fee').parent('li').addClass('active');
    $('#a_bank').parent('li').removeClass('active');
    $('#dv_fee').addClass('active in');
    $('#dv_bankinfo').removeClass('active in');
}

function RBBankPrev() {
    $('#a_bank').parent('li').removeClass('active');
    $('#a_secQtns').parent('li').addClass('active');
    $('#dv_bankinfo').removeClass('active in');
    $('#dvsqus').addClass('active in');
}

function RBFeePrev() {
    $('#a_fee').parent('li').removeClass('active');
    $('#a_bank').parent('li').addClass('active');
    $('#dv_fee').removeClass('active in');
    $('#dv_bankinfo').addClass('active in');
}
//// Refund Advantage
function RAEONext() {
    var _cango = true;
    //$('*').removeClass('error_msg');
    //$('*').attr('title', '');

    $('#txt_ownercell').removeClass("error_msg");
    $('#txt_ownercell').attr('title', '');
    if ($('#txt_ownercell').val() != '') {
        var number = $('#txt_ownercell').val();
        if (number.toLowerCase().indexOf('x') > -1) {
            $('#txt_ownercell').addClass("error_msg");
            $('#txt_ownercell').attr('title', 'Please enter valid EFIN Owner Mobile Phone');
            _cango = false;
        }
    }

    $('#txt_ownerphone').removeClass("error_msg");
    $('#txt_ownerphone').attr('title', '');
    if ($('#txt_ownerphone').val() != '') {
        var number = $('#txt_ownerphone').val();
        if (number.toLowerCase().indexOf('x') > -1) {
            $('#txt_ownerphone').addClass("error_msg");
            $('#txt_ownerphone').attr('title', 'Please enter valid EFIN Owner Phone');
            _cango = false;
        }
    }

    if (!_cango)
        return;

    saveNextRABankEnrollment(0, 1);
    // $('#a_ooinfo').click();
    $('#a_efinowner').parent('li').removeClass('active');
    $('#a_ooinfo').parent('li').addClass('active');

    $('#dvefinowner').removeClass('active in');
    $('#dvero').addClass('active in');
}

function RAOONext() {
    var _cango = true;
    //$('*').removeClass('error_msg');
    //$('*').attr('title', '');
    $('#txt_contactphone').removeClass("error_msg");
    $('#txt_contactphone').attr('title', '');
    if ($('#txt_contactphone').val() != '') {
        var number = $('#txt_contactphone').val();
        if (number.toLowerCase().indexOf('x') > -1) {
            $('#txt_contactphone').addClass("error_msg");
            $('#txt_contactphone').attr('title', 'Please enter valid Main Contact Phone');
            _cango = false;
        }
    }
    $('#txt_officephone').removeClass("error_msg");
    $('#txt_officephone').attr('title', '');
    if ($('#txt_officephone').val() != '') {
        var number = $('#txt_officephone').val();
        if (number.toLowerCase().indexOf('x') > -1) {
            $('#txt_officephone').addClass("error_msg");
            $('#txt_officephone').attr('title', 'Please enter valid Office Phone');
            _cango = false;
        }
    }

    if (!_cango)
        return;

    saveNextRABankEnrollment(0, 2);
    // $('#a_feeinfo').click();
    $('#a_ooinfo').parent('li').removeClass('active');
    $('#a_feeinfo').parent('li').addClass('active');

    $('#dvero').removeClass('active in');
    $('#dvfee').addClass('active in');
}

function RAOOPPrev() {
    // $('#a_efinowner').click();
    $('#a_efinowner').parent('li').addClass('active');
    $('#a_ooinfo').parent('li').removeClass('active');

    $('#dvero').removeClass('active in');
    $('#dvefinowner').addClass('active in');
}

function RAFeeNext() {
    saveNextRABankEnrollment(0, 3);
    // $('#a_bankinfo').click();
    $('#a_bankinfo').parent('li').addClass('active');
    $('#a_feeinfo').parent('li').removeClass('active');

    $('#dvfee').removeClass('active in');
    $('#dv_bankinfo').addClass('active in');
}

function RAFeePPrev() {
    // $('#a_ooinfo').click();
    $('#a_ooinfo').parent('li').addClass('active');
    $('#a_feeinfo').parent('li').removeClass('active');

    $('#dvfee').removeClass('active in');
    $('#dvero').addClass('active in');
}

function RABankPPrev() {
    //$('#a_feeinfo').click();
    $('#a_bankinfo').parent('li').removeClass('active');
    $('#a_feeinfo').parent('li').addClass('active');

    $('#dvfee').addClass('active in');
    $('#dv_bankinfo').removeClass('active in');
}

function getformattedDate(dob) {
    var pos = 2, yearpos = 5;
    var b = '/';
    var updob = [dob.slice(0, pos), b, dob.slice(pos)].join('');
    var _updob = [updob.slice(0, yearpos), b, updob.slice(yearpos)].join('');
    return _updob;
}
