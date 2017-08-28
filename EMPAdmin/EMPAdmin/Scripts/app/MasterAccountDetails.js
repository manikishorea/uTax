var Id = 0;

$(function () {
    $('#success').hide();
    $('#success p').html('');
    jQuery(".office-table .table").footable();
    $(".search_icon").click(function () {
        $(".search-block").slideToggle();
    });
    getxlinkDetails();
});

function getxlinkDetails() {
    //debugger;
    var uri = '/api/CrosslinkDetails/getxlinkdetails';
    $("#table_MasterAccount > tbody").remove();
    var table = $('#table_MasterAccount').append('<tbody/>');
    ajaxHelper(uri, 'GET').done(function (data) {
        if (data) {
            $.each(data, function (rowIndex, r) {
                var row = $("<tr/>").appendTo(table);;
                row.append($("<td/>").text(r["CLAccountId"]));
                row.append($("<td/>").text(r["CLLogin"]));
                row.append($("<td/>").text(r["status"]));

                var activei = '', activetext = '';
                if (r["status"] != "Active") {
                    activei = "<i class='fa fa-circle' aria-hidden='true' title='Activated'></i>";
                    activetext = 'Activate';
                }
                else {
                    activei = "<i class='fa fa-circle-o' aria-hidden='true'  title='Inactivate'></i>";
                    activetext = 'Deactivate';
                }
                var actions = '<td><div class="btn-group dropleft"><button class="btn dropdown-toggle" aria-expanded="false" aria-haspopup="true" data-toggle="dropdown">Actions <span class="caret"></span></button>';
                actions = actions + '<ul class="dropdown-menu dropdown-menu-right" aria-labelledby="dropdownMenu" role="menu">';
                if (activetext == 'Deactivate')
                    actions = actions + '<li class="EditLink"><a data-target="#popupNewUser" data-toggle="modal" onclick="EditAccount(\'' + r["MasterId"] + '\',\'' + r["Password"] + '\',\'' + r["CLAccountId"] + '\',\'' + r["CLLogin"] + '\',\'' + r["CLAccountPassword"] + '\',' + r["Id"] + ')"><i class="fa fa-pencil-square-o" aria-hidden="true" title="Edit"></i> Edit</a></li>';
                actions = actions + '<li class="divider"></li>';
                actions = actions + '<li class="ActiveInactiveLink">';
                actions = actions + '<a href="javascript:void(0);" onclick="ActiveInactiveAccount(' + r.Id + ',\'' + r.status + '\',\'' + activetext + '\',\'' + r.CLAccountId + '\') ">' + activei + '' + activetext + '</a>';
                actions = actions + '</li>';
                actions = actions + '</ul>';
                actions = actions + '</div></td>';
                row.append(actions);
            });
        }
    })
    $(".office-table .table").trigger('footable_initialize');
}

function fnUpdatexlink() {
    $('#popup_error').hide();
    $('#popup_success').hide();
    $('#error').hide();
    $('#success').hide();

    var req = {};
    req.MasterId = $.trim($('#txt_masterId').val())
    req.Password = $.trim($('#txt_password').val());
    req.CLAccountId = $.trim($('#txt_claccountid').val());
    req.CLLogin = $.trim($('#txt_cllogin').val());
    req.CLAccountPassword = $.trim($('#txt_claccountpassword').val());

    req.UserId = $('#UserId').val();
    req.Id = Id;
    $('input').removeClass('error_msg').attr('title', '');

    var _continue = true;
    //if (req.MasterId.trim() == '') {
    //    $('#txt_masterId').addClass('error_msg');
    //    $('#txt_masterId').attr('title', 'Please enter Master Identifier');
    //    _continue = false;
    //}

    //if (req.Password.trim() == '') {
    //    $('#txt_password').addClass('error_msg');
    //    $('#txt_password').attr('title', 'Please enter Password');
    //    _continue = false;
    //}


    if (req.CLAccountId.trim() == '') {
        $('#txt_claccountid').addClass('error_msg');
        $('#txt_claccountid').attr('title', 'Please enter CrossLink Account ID');
        _continue = false;
    }

    if (req.CLLogin.trim() == '') {
        $('#txt_cllogin').addClass('error_msg');
        $('#txt_cllogin').attr('title', 'Please enter Login');
        _continue = false;
    }

    if (req.CLAccountPassword.trim() == '') {
        $('#txt_claccountpassword').addClass('error_msg');
        $('#txt_claccountpassword').attr('title', 'Please enter CrossLink Account ID Password');
        _continue = false;
    }

    if (!_continue)
        return;

    var uri = '/api/CrosslinkDetails/updatexlinkdetails';
    ajaxHelper(uri, 'POST', req).done(function (data, status) {
        if (data == 'Exists') {
            $('#popup_error').show();
            $('#popup_error p').html('The Master Identifier is already exists.');
        }
        else if (data == 'Success') {
            $('#success').show();
            $('#success p').html('The Master Identifier ' + $('#btnSave').text() + 'ed successfully.');
            $('#btnCancel').click();
            getxlinkDetails();
        }
    })
}

function EditAccount(Username, Password, claccountid, cllogin, claccountpassword, AId) {
    $('input').removeClass('error_msg').attr('title', '');
    $('#txt_masterId').val('');
    $('#txt_password').val('');
    $('#txt_claccountid').val('');
    $('#txt_cllogin').val('');
    $('#txt_claccountpassword').val('');

    Id = AId;
    $('#txt_masterId').val(Username);
    $('#txt_password').val(Password);

    if (claccountid != null && claccountid != 'null' && claccountid != undefined) {
        $('#txt_claccountid').val(claccountid);
    }


    if (cllogin != null && cllogin != 'null' && cllogin != undefined) {
        $('#txt_cllogin').val(cllogin);
    }

    if (claccountpassword != null && claccountpassword != 'null' && claccountpassword != undefined) {
        $('#txt_claccountpassword').val(claccountpassword);
    }
    $('#btnSave').text('Update');
    $('#popup_error').hide();
    $('#popup_success').hide();
    $('#error').hide();
    $('#success').hide();
}

function NewUser() {
    Id = 0;
    $('#txt_masterId').val('');
    $('#txt_password').val('');
    $('#txt_claccountid').val('');
    $('#txt_cllogin').val('');
    $('#txt_claccountpassword').val('');
    $('#btnSave').text('Save');
    $('input').removeClass('error_msg').attr('title', '');
    $('#popup_error').hide();
    $('#popup_success').hide();
    $('#error').hide();
    $('#success').hide();
}

function ActiveInactiveAccount(Id, Status, text, UserName) {
    $('#error').hide();
    $('#success').hide();
    $('#popup_error').hide();
    $('#popup_success').hide();
    var confres = confirm('Are you sure want to ' + text + ' the record');
    if (confres) {
        var req = {};
        req.Id = Id;
        req.status = Status;
        req.UserId = $('#UserId').val();
        req.MasterId = UserName;

        var uri = '/api/CrosslinkDetails';
        ajaxHelper(uri, 'PUT', req).done(function (data, status) {
            if (data == 'success') {
                $('#success').show();
                $('#success p').html('The Master Identifier ' + text + 'ed successfully.');
                getxlinkDetails();
            }
            else if (data == 'Exists') {
                $('#error').show();
                $('#error p').html('The Master Identifier is already exists with username.');
            }
        })
    }
}
