$(function () {
    getCustomerBanks();
});

function getCustomerBanks() {
    var CustomerId = $('#UserId').val();
    var EntityId = $('#entityid').val();
    if ($('#entityid').val() != $('#myentityid').val()) {
        CustomerId = $('#myid').val();
        EntityId = $('#myentityid').val();
    }

    var _showunlock = false;
    var bankUri = '/api/BankSelection/getCutomerBanks?CustomerId=' + CustomerId + '&EntityId=' + EntityId;
    ajaxHelper(bankUri, 'GET').done(function (data) {
        if (data) {
            if (data.Status) {
                $('#tbd_banks').empty();
                var trdata = '';
                $.each(data.Banks, function (item, value) {
                    trdata += '<tr><td>' + value.BankName + '</td><td>' + value.Submission + '</td><td>' + value.Acceptance + '</td><td>' + value.BankStatus + '</td><td>';

                    var enrurl = '';
                    if ($('#entityid').val() != $('#myentityid').val()) {
                        enrurl = '/Enrollment/BankSelectionFeeDetails?Id=' + CustomerId + '&entitydisplayid=' + $('#myentitydisplayid').val() + '&ParentId=' + $('#myparentid').val() + '&entityid=' + $('#myentityid').val() + '&ptype=enrollment&bankid=' + value.BankId;
                    }
                    else
                        enrurl = '/Enrollment/BankSelectionFeeDetails?bankid=' + value.BankId;
                    if (value.Default == 1) {
                        trdata += '<a href="' + enrurl + '">Update Enrollment</a>';
                    }
                    else if (value.IsApproved) {
                        trdata += '<a onclick="SetDefaultBank(\'' + value.BankId + '\')">Set as Default</a>';
                    }
                    //if (value.IsApproved) {
                    //    _showunlock = true;
                    //    //trdata += '<a href="' + enrurl + '">Update Enrollment</a>';
                    //    if (!value.IsDefault) {
                    //        trdata += '<a onclick="SetDefaultBank(\'' + value.BankId + '\')">Set as Default</a>';
                    //    }
                    //}
                    //if (!value.IsSubmitted)
                    //    trdata += '<a href="' + enrurl + '">Submit Enrollment</a>';
                    //if (value.IsSubmitted)
                    //    trdata += '<a href="' + enrurl + '">View Enrollment</a>';
                    trdata += '</td></tr>';
                })
                $('#tbd_banks').html(trdata);
            }
        }
        if (_showunlock && $('#entityid').val() == $('#Entity_uTax').val()) {
            $('#dv_unlock').show();
        }
    })
}

function SaveDefaultBankPopup() {

    var bankid = $('#hdnbankid').val();
    var UserId = $('#UserId').val();
    var CustomerId = $('#UserId').val();
    if ($('#myentityid').val() != $('#entityid').val()) {
        CustomerId = $('#myid').val();
    }

    var bankUri = '/api/BankSelection/SetDefaultBank?CustomerId=' + CustomerId + '&UserId=' + UserId + '&BankId=' + bankid;
    ajaxHelper(bankUri, 'POST').done(function (data) {
        window.location.reload();
    });
}

function SetDefaultBank(bankid) {
    $('#hdnbankid').val(bankid);
    $('#popupDefaultBank').modal('show');
}