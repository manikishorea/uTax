getEFINStatus($('#EFINStatus'), 2);

function SavePartner() {
    var success = $('#success');
    success.html('');
    success.hide();

    var err = $('#error');
    err.html('');
    err.hide();

    var info = {};
    info.CompanyName = $('#txt_company').val();
    info.FName = $('#txt_FName').val();
    info.LName = $('#txt_LName').val();
    info.Phone = $('#txt_Phone').val();
    info.Email = $('#txt_Email').val();
    info.EFINStatus = $('#EFINStatus').val();
    info.EFIN = $('#EFIN').val();
    info.Address1 = $('#txt_address1').val();
    info.Address2 = $('#txt_address2').val();
    info.City = $('#txt_city').val();
    info.State = $('#txt_state').val();
    info.Zip = $('#txt_zip').val();
    info.MasterID = $('#MasterIdentifier').val();
    info.OfficePortalUrl = 'https://www.mytaxofficeportal.com';

    ajaxHelper('/api/CustomerInformation/AddSO', 'POST', info).done(function (res) {
        if (res) {
            success.html('<p>Site created successfully. It is available in Office Management.</p>');
            success.show();
        }
        else {
            err.html('<p>Error occured. Please try again later.</p>');
            success.show();
        }
    })
}