function fnGetUserInfoBySalesYear() {
    var error = $('#error');
    error.html('');
    error.hide();
    $('#SalesYearID').removeClass('error_msg').attr('title', '');
    if (!$('#SalesYearID').val()) {
        $('#dv_data').hide();
        $('#SalesYearID').addClass('error_msg').attr('title', 'Please Select Sales Year.');
        error.show();
        error.append('<p> Please Select Sales Year. </p>');
        return;
    }

    var SalesYearGroupId = $('#SalesYearGroupId').val();
    var myentitydisplayid = $('#myentitydisplayid').val();
    var IsAddEFIN = false;
    if (myentitydisplayid == $('#BaseEntity_AESS').val()) {
        IsAddEFIN = true;
    }

    $('#dv_data').show();

    var Id = $('#UserId').val();
    var url = '/api/Archive/CustomerInfo?SYGrpId=' + SalesYearGroupId + '&SalesYearID=' + $('#SalesYearID').val() + '&IsAddEFIN=' + IsAddEFIN;
    if (Id != '' && Id != null && Id != '00000000-0000-0000-0000-000000000000') {
        ajaxHelper(url, 'GET').done(function (data) {

            $('#UserId').val(data.Id);
            $('#parentid').val(data.ParentId);
            //$('#entityid').val(data.EntityId);
            //$('#entitydisplayid').val(data.DisplayId);
           // $('#supparentid').val(data.ParentId);
            // $('#myentitydisplayid').val(data.DisplayId);
            $('#IsVerified').val(data.IsVerified);
            $('#ismsouser').val(data.IsMSOUser);

            if ($('#myentityid').val() == $('#Entity_MO').val() || $('#myentityid').val() == $('#Entity_SVB').val()) {
                ArcMainSite();
            } else {
                ArchiveSubSite(data.Id, data.ParentId)
                fnHideMSOLoc();
            }

            ArchiveEnrollment(data.Id, data.ParentId, data.EntityId);
            //ArcEnrollment();

           setTimeout(function () {
            $('input').attr('disabled', 'disabled');
            $('textarea').attr('disabled', 'disabled');
            $('select').attr('disabled', 'disabled');
            $('select#SalesYearID').removeAttr('disabled');
            $('#myTabContentoffice').css('pointer-events', 'none');
           
           $('#dvofficeConfig').css('pointer-events', 'none');
            $('#dvaffiliateConfig').css('pointer-events', 'none');
            $('#dvfeereim').css('pointer-events', 'none');
            $('#dvbankselection').css('pointer-events', 'none');
            $('#dvGeneral1').css('pointer-events', 'none');
            
        });

        });
    }
}

function ArcMainSite() {
    ArchiveMainSite();
}

function ArcEnrollment() {

    getTooltip();

    ArchiveEnrollment();

    //  GetCustomerInfomationForMainOffice();

    var erotype = getEroType($('#entitydisplayid').val());
    if (erotype) {
        if (erotype == 'Single Office') {
            $('#divBankProduct').hide();
        }
    }

    getBankSelected();

    setTimeout(function () {
        var d = new Date();
        d.setYear(d.getFullYear() - 18);

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

            $('#txt_OwnerDOB').datepicker({
                format: "mm/dd/yyyy",
                endDate: d
            });
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

            $('#txt_ownerDOB, #txt_modalDOB').datepicker({
                format: "mm/dd/yyyy",
                endDate: d
            });

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
            $('#txt_offmngrDOB, #txt_ownerDOB, #txt_businessdate, #txt_efinownerDOB, #txt_offContactDOB').datepicker({
                format: "mm/dd/yyyy",
                endDate: d
            })
            $('#txt_eroagreeddate, #txt_eroapplncmptd').datetimepicker();
            $('#txt_officephone, #txt_cellphone, #txt_ownerphone, #txt_efinownerphone, #txt_efinownermobile, #txt_offmngeCellPhone').mask("999-999-9999");
        }

        getBankEnrollmentData();

    });

    setTimeout(function () {
        $('ul.bankenroll-ul-tab').find('a').attr('data-toggle', 'tab');
        $('div.bankenroll-div-save').remove();

    }, 100);

    if (bankstatus == 'APR' || bankstatus == 'RDY' || bankstatus == 'SUB')
        $('#btnBankEnrollSubmit').hide();

}

function ArcSubSite() {
    ArchiveSubSite();
}