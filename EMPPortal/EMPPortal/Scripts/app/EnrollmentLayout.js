//$(function () {
//    getSummaryActivation();
//    var Id = getUrlVars()["Id"];

//    //if (window.location.href.indexOf('CustomerInformation/Create') > 0)
//    //    Id = window.location.href.split('/').pop();

//    getEnrollmentCustomerInformation(Id);
//})

function getEnrollmentCustomerInformation(Id) {
    
    var url = '/api/CustomerInformation/';
    if (Id != '' && Id != null && Id != '00000000-0000-0000-0000-000000000000') {
        ajaxHelper(url + '?id=' + Id, 'GET',null,false).done(function (data) {
            $('#sp_Head').html(data.CompanyName);
            $('#IsVerified').val(data.IsVerified);
            $('#ismsouser').val(data.IsMSOUser);
            $('#hdnEFINStatus').val(data.EFINStatus);
           // fnVerifiedLinksStatus();
            $('#ActiveMyAccountStatus').val(data.IsActivationCompleted);
            $('#uTaxNotCollectingSBFee').val(data.IsNotCollectingFee);

            $('#SalesforceOpportunityID').val(data.SalesforceOpportunityID);

            if (data.BaseEntityId == $('#BaseEntity_AESS').val()) {

                //Office Config
                $('#divSubSiteOfficeConfigForm input').attr('disabled', 'disabled');
                $('#divSubSiteOfficeConfigForm select').attr('disabled', 'disabled');
                $('#divSubSiteOfficeConfigForm textarea').attr('disabled', 'disabled');

                $('#divSubSiteOfficeConfigForm a')
                    .attr('disabled', 'disabled')
                    .css('pointer-events', 'none');


                //Fee Setup & Config
                $('#SubOfficeSVBFee input,#SubOfficeTranFee input').attr('disabled', 'disabled');
                $('#SubOfficeSVBFee select,#SubOfficeTranFee select').attr('disabled', 'disabled');
                $('#SubOfficeSVBFee textarea,#SubOfficeTranFee textarea').attr('disabled', 'disabled');

                $('#SubOfficeSVBFee a, #SubOfficeTranFee a,#spanSvbTansSave a')
                    .attr('disabled', 'disabled')
                    .css('pointer-events', 'none');




                //Dashboard
                if ($('#entitydisplayid').val() != $('#Entity_uTax').val()) {
                    //$('#divSubSiteOfficeDashboardForm a')
                    //  .attr('disabled', 'disabled')
                    //  .css('pointer-events', 'none');
                }


                //Activate My Account
                $('#divSubSiteOfficeActivate a.btn-Edit-SubSiteOffice-Active')
                    .attr('disabled', 'disabled')
                     .css('pointer-events', 'none');


                //Customer Notes
                $('#divSubSiteOfficeCustNotesForm input').attr('disabled', 'disabled');
                $('#divSubSiteOfficeCustNotesForm textarea').attr('disabled', 'disabled');

                $('#divSubSiteOfficeCustNotesForm a')
                    .attr('disabled', 'disabled')
                    .css('pointer-events', 'none');



                //
               // $('#lblIsSubSiteEFINRB').remove();
               // $('#lblIsSubSiteEFIN').remove();

                //Office Config
                $('#divEnrollOfficeConfigForm input').attr('disabled', 'disabled');
                $('#divEnrollOfficeConfigForm select').attr('disabled', 'disabled');
                $('#divEnrollOfficeConfigForm textarea').attr('disabled', 'disabled');

                $('#divEnrollOfficeConfigForm a')
                    .attr('disabled', 'disabled')
                    .css('pointer-events', 'none');


                //Affliate Config
                $('#divEnrollAffliateForm input').attr('disabled', 'disabled');
                $('#divEnrollAffliateForm select').attr('disabled', 'disabled');
                $('#divEnrollAffliateForm textarea').attr('disabled', 'disabled');

                $('#divEnrollAffliateForm a')
                    .attr('disabled', 'disabled')
                    .css('pointer-events', 'none');


                //Dashboard
                $('#divEnrollOfficeInfoForm a')
                  .attr('disabled', 'disabled')
                  .css('pointer-events', 'none');


                //Enroll Summary Form
                $('#divEnrollSummaryForm a.btn-edit-enroll-disabled')
                    .attr('disabled', 'disabled')
                     .css('pointer-events', 'none');


                ////Fee Setup Config
                //$('#divEnrollFeeSetupConfigForm input').attr('disabled', 'disabled');
                //$('#divEnrollFeeSetupConfigForm textarea').attr('disabled', 'disabled');
                //$('#divEnrollFeeSetupConfigForm select').attr('disabled', 'disabled');

                //$('#divEnrollFeeSetupConfigForm a')
                //    .attr('disabled', 'disabled')
                //    .css('pointer-events', 'none');
            }

            
            $('#ActiveMyAccountStatus').val('0');
            if (data.IsActivationCompleted == '1' || data.IsActivationCompleted == 1) {
                $('#ActiveMyAccountStatus').val('1');
            }

            //if ($('#ActiveMyAccountStatus').val() == '1' || $('#ActiveMyAccountStatus').val() == 1) {
            //    var sitemapid = $('#formid').attr('sitemapid');
            //    if (sitemapid == "60025459-7568-4a77-b152-f81904aaaa63") {
            //        $('#divSVBFeeReimNoteActive').show();
            //    }
            //}

        });
    }
}

//function getSummaryActivation() {
    
//    var Id = getUrlVars()["Id"];
//    if (Id) {
//        var url = '/api/EnrollmentBankSelection/IsAcivated?CustomerId=' + Id;
//        ajaxHelper(url, 'POST').done(function (data) {
            
//            if (data) {
//                $('#li_summary').css('display', 'block');
//            }
//        })
//    }
//}