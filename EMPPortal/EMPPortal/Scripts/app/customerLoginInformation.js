function GetCustomerLoginInformation(CustomerOfficeId) {
    var url = '/api/CustomerLoginInformation';
    ajaxHelper(url + '?id=' + CustomerOfficeId, 'GET').done(function (data) {

        $('#lbCustomerName').text(data["CompanyName"]);
        $('#Id').val(data["Id"]);
        $('#EFIN').val(data["EFIN"]);
        $('#MasterIdentifier').val(data["MasterIdentifier"]);
        $('#CrossLinkUserId').val(data["CrossLinkUserId"]);
        $('#CrossLinkPassword').val(data["CrossLinkPassword"]);
        $('#OfficePortalUrl').val(data["OfficePortalUrl"]);
        $('#TaxOfficeUsername').val(data["TaxOfficeUsername"]);
        $('#TaxOfficePassword').val(data["TaxOfficePassword"]);
        //$('#EMPPassword').val(data["EMPPassword"]);
        $('#CustomerOfficeId').val(data["CustomerOfficeId"]);
        $('#MasterIdentifierPassword').val(data["MasterIdentifierPassword"])

        // getEFINStatus($('#EFINStatus'), data['EntityId']);
        // $('#EFINStatus').val(data["EFINStatus"]);

        $('#EFINStatus').val(data['EFINStatus']);
        //$('#EFIN').val(data["EFIN"]);
        if (data['EFINStatus'] == '16' || data['EFINStatus'] == '19') {
            //$('#EFINStatus').attr('readonly', 'readonly');
            //$('#EFIN').attr('readonly', 'readonly');
            //$('#EFINStatus').css('pointer-events', 'none');
            //$('#EFINStatus').prop('disabled', true);
        }

        if (Number(data.EFIN) > 0) {
            var EFIN = PadLeft(data.EFIN, 6);
            $('#EFIN').val(EFIN);
        }
        else {
            $('#EFIN').val('');
        }

        $('#CLAccountId').val(data.CLAccountId);
        $('#CLLogin').val(data.CLLogin);
        $('#CLAccountPassword').val(data.CLAccountPassword);

        $('#CLAccountId').attr('readonly', 'readonly')
        $('#CLLogin').attr('readonly', 'readonly')
        $('#CLAccountPassword').attr('readonly', 'readonly')

        if (data.EntityId == $('#Entity_SVB').val()) {
            $('#CLAccountId').removeAttr('readonly')
            $('#CLLogin').removeAttr('readonly')
            $('#CLAccountPassword').removeAttr('readonly')
        }

    });
}

function SaveCustomerLoginInformation() {
    $('#EFINStatus').prop('disabled', false);
    var cansubmit = true;

    var error = $('#error');
    error.html('');
    error.hide();

    var success = $('#success');
    success.html('');
    success.hide();

    var customer = {};
    customer.Id = $('#Id').val();
    customer.EFIN = $.trim($('#EFIN').val());
    customer.MasterIdentifier = $.trim($('#MasterIdentifier').val());
    customer.CrossLinkUserId = $.trim($('#CrossLinkUserId').val());
    customer.EMPUserId = $.trim($('#CrossLinkUserId').val());
    customer.CrossLinkPassword = $.trim($('#CrossLinkPassword').val());
    customer.EMPPassword = $.trim($('#CrossLinkPassword').val());
    customer.OfficePortalUrl = $.trim($('#OfficePortalUrl').val());
    customer.TaxOfficeUsername = $.trim($('#TaxOfficeUsername').val());
    customer.TaxOfficePassword = $.trim($('#TaxOfficePassword').val());
    customer.CustomerOfficeId = $.trim($('#CustomerOfficeId').val());
    customer.UserId = $('#UserId').val();
    customer.EFINStatus = $('#EFINStatus').val();

    customer.CLAccountId = $('#CLAccountId').val();
    customer.CLLogin = $('#CLLogin').val();
    customer.CLAccountPassword = $('#CLAccountPassword').val();

    //$('#EFIN').removeClass("error_msg");
    //if (customer.EFIN == "") {
    //    $('#EFIN').addClass("error_msg");
    //    $('#EFIN').attr('title', 'Please enter EFIN');
    //    cansubmit = false;
    //}
    var EFIN = customer.EFIN;
    $('#EFINStatus').removeClass("error_msg");
    $('#EFIN').removeClass("error_msg");
    if ($.trim(customer.EFINStatus) == "0" || $.trim(customer.EFINStatus) == "") {
        $('#EFINStatus').addClass("error_msg");
        $('#EFINStatus').attr('title', 'Please select EFIN Status');
        cansubmit = false;
    } else if ($.trim(customer.EFINStatus) == '16' || $.trim(customer.EFINStatus) == '19') {
        if (EFIN == "" || EFIN == "0") {
            $('#EFIN').addClass("error_msg");
            $('#EFIN').attr('title', 'Please enter Valid EFIN');
            cansubmit = false;
        }

        //if (EFIN.length < 6) {
        //    $('#EFIN').addClass("error_msg");
        //    $('#EFIN').attr('title', 'Please enter Valid 6 digit EFIN');
        //    cansubmit = false;
        //}
    }

    if (EFIN != "" && EFIN != null) {
        if (EFIN.length < 6 || Number(EFIN) < 1) {
            $('#EFIN').addClass("error_msg");
            $('#EFIN').attr('title', 'Please enter Valid 6 digit EFIN');
            cansubmit = false;
        }
    }


    $('#MasterIdentifier').removeClass("error_msg");
    if (customer.MasterIdentifier == "") {
        $('#MasterIdentifier').addClass("error_msg");
        $('#MasterIdentifier').attr('title', 'Please enter Master Identifier');
        cansubmit = false;
    }

    $('#CrossLinkUserId').removeClass("error_msg");
    if (customer.CrossLinkUserId == "") {
        $('#CrossLinkUserId').addClass("error_msg");
        $('#CrossLinkUserId').attr('title', 'Please enter CrossLink UserId');
        cansubmit = false;
    }
    if ($.trim(customer.CrossLinkUserId) == "0") {
        $('#CrossLinkUserId').addClass("error_msg");
        $('#CrossLinkUserId').attr('title', 'Please enter Valid CrossLink UserId');
        cansubmit = false;
    }

    $('#CrossLinkPassword').removeClass("error_msg");
    if (customer.CrossLinkPassword == "") {
        $('#CrossLinkPassword').addClass("error_msg");
        $('#CrossLinkPassword').attr('title', 'Please enter CrossLink Password');
        cansubmit = false;
    }

    $('#OfficePortalUrl').removeClass("error_msg");
    if (customer.OfficePortalUrl == "") {
        $('#OfficePortalUrl').addClass("error_msg");
        $('#OfficePortalUrl').attr('title', 'Please enter Office Portal Url');
        cansubmit = false;
    }

    //$('#TaxOfficeUsername').removeClass("error_msg");
    //if (customer.TaxOfficeUsername == "") {
    //    $('#TaxOfficeUsername').addClass("error_msg");
    //    $('#TaxOfficeUsername').attr('title', 'Please enter TaxOffice Username');
    //    cansubmit = false;
    //}

    //$('#TaxOfficePassword').removeClass("error_msg");
    //if (customer.TaxOfficePassword == "") {
    //    $('#TaxOfficePassword').addClass("error_msg");
    //    $('#TaxOfficePassword').attr('title', 'Please enter TaxOffice Password');
    //    cansubmit = false;
    //}

    //$('#EMPPassword').removeClass("error_msg");
    //if (customer.EMPPassword == "") {
    //    $('#EMPPassword').addClass("error_msg");
    //    $('#EMPPassword').attr('title', 'Please enter EMP Password');
    //    cansubmit = false;
    //}

    //$('#CustomerOfficeId').removeClass("error_msg");
    //if (customer.CustomerOfficeId == "") {
    //    $('#CustomerOfficeId').addClass("error_msg");
    //    $('#CustomerOfficeId').attr('title', 'Please enter CustomerOfficeId Id');
    //    cansubmit = false;
    //}

    if (!cansubmit) {
        error.show();
        error.append('<p> Please correct the error(s). </p>');
        return false;
    }

    if (cansubmit) {
        error.hide();
        error.html('');
        var url = '/api/CustomerLoginInformation/';
        ajaxHelper(url, 'POST', customer).done(function (data) {
            if (data == -1) {
                error.show();
                error.append('<p>  This EFIN already exists. </p>');
                $("html, body").animate({ scrollTop: 0 }, "slow");
            }
            else if (data == -2) {
                error.show();
                error.append('<p>  This Crosslink User Id already exists. </p>');
                $("html, body").animate({ scrollTop: 0 }, "slow");
            }
            else if (data == 0) {
                error.show();
                error.append('<p>  Record not saved. </p>');

            } else {
                UpdateOfficeManagement(customer.CustomerOfficeId);
                localStorage.setItem("CrossMsg", "Record saved successfully");;
                success.show();
                success.append('<p> Record saved successfully. </p>');
                window.location.href = "/CustomerInformation/Index";
            }
        });
    }
}
