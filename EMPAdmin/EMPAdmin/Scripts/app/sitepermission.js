function getAllSitePermission(Id) {
    var booksUri = '/api/SiteMapRolePermissions/';

    $("#table_sitemappermission > tbody").remove();

    var table = $('#table_sitemappermission').append('<tbody/>'); //$(document.body)
    ajaxHelper(booksUri + Id, 'GET').done(function (data) {

        var Parid = 0;
        var data2 = data;
        var ParentName = '';
        var SiteName = '';
        var SiteMapId = '';
        var SiteMapIds = [];
        $.each(data, function (rowIndex, r) {

            if (ParentName == null || ParentName == '' || ParentName != r["ParentName"]) {
                Parid++;
                ParentName = r["ParentName"];
                var $tr_main = $('<tr/>');
                $tr_main.appendTo(table);
                $tr_main.append($('<td colspan="6" class="label-default"><input class="manage_check" type="checkbox" title="Screen" id="parentChk_' + Parid + '" onchange="UpdateCkeck(' + Parid + ')" /> <b>' + r["ParentName"] + '</b></td>'));
            }

            if (SiteMapIds == null || SiteMapIds == '' || SiteMapIds.indexOf(r["SiteMapId"]) == -1) {

                SiteMapIds.push(r["SiteMapId"]);
                SiteMapId = r["SiteMapId"];
                SiteName = r["SiteName"];

                var $tr_sub = $('<tr/>').addClass("sub_check");
                $tr_sub.appendTo(table);

                $tr_sub.append('<td>&nbsp;&nbsp;' + SiteName + '</td>');
                var $td_sub = $('<td/>');
                var $ul_sub = $('<ul/>').addClass('list-unstyled padding-left-4');

                $.each(data2, function (rowIndex2, r2) {
                    if (SiteMapId == r2["SiteMapId"]) {

                        var IsChecked = r2["IsPermitted"];
                        var checked = '';
                        if (IsChecked == 'true' || IsChecked == true || IsChecked == 'True') {
                            checked = 'checked=checked';
                        }
                        $ul_sub.append('<li>' + r2["PermissionName"] + ' &nbsp;<input class="add_edit_check all_check sub_check" type="checkbox" title="' + r2["PermissionName"] + '" ' + checked + ' name="' + r2["Id"] + '" id="chksub_' + Parid + '_' + r2["Id"] + '" value="' + r2["Id"] + '"  sitemapid="' + SiteMapId + '" /></li>');
                    }
                });
                $td_sub.append($ul_sub);
                $tr_sub.append($td_sub);
            }
            // }
        });

        $('input[id^="parentChk_"]').each(function (e) {
            //debugger;
            var pid = this.id.split('_')[1];
            if ($('input[id^=chksub_' + pid + ']').length == $('input[id^=chksub_' + pid + ']:checked').length)
                $('#parentChk_' + pid).prop('checked', true);
        });


        var enrollmanagstatus = $('#chksub_3_8f696cdd-054b-453c-b34e-3fe299b7e235').prop('checked');
        if (!enrollmanagstatus) {
            $('#chksub_3_be9c75ca-4a88-43a9-851c-356ee8363f99').prop('disabled', true);
            $('#chksub_3_be9c75ca-4a88-43a9-851c-356ee8363f99').prop('checked', false);
        }
    });
}

function fnSaveSiteRolePermission() {

    $('#success').hide();
    $('#success p').html('');

    $('#error').hide();
    $('#error p').html('');

    var dto = {};
    var cansubmit = true;

    dto.RoleId = $('#RoleId').val();
    dto.UserId = $('#UserId').val();
    var SiteRolePermissions = [];

    var IsRoleChecked = false;
    var allchkbox = $('input[type=checkbox].sub_check');
    $.each(allchkbox, function (indx, valu) {
        var chkId = $(valu).attr('id');
        var sitepermissionid = $('#' + chkId).val()
        var sitemapid = $('#' + chkId).attr('sitemapid');
        if ($(valu).is(':checked')) {
            IsRoleChecked = true;
            SiteRolePermissions.push({
                Id: sitepermissionid,
                PermissionId: sitepermissionid,
                SiteMapId: sitemapid
            });
        }
    });

    dto.SiteRolePermissions = SiteRolePermissions;


    //$('#RoleIds').removeClass("error_msg");
    //if (!IsRoleChecked) {
    //    $('#RoleIds').addClass("error_msg");
    //    $('#RoleIds').attr('title', 'Please select at least one permission');
    //    cansubmit = false;
    //}


    if (cansubmit) {
        var Uri = '/api/SiteMapRolePermissions/';
        ajaxHelper(Uri, 'POST', dto).done(function (data) {

            if (data == true || data == 'true' || data == 'True') {
                $('#success').show();
                $('#success p').html('Manage Role Permission updated successfully.');
            } else {
                $('#error').show();
                $('#error p').html('Manage Role Permission not updated.');
            }
        });
    }
}

$(function () {
    var Id = $('#RoleId').val();
    getAllSitePermission(Id)

    //var initial_form_state = $('#frmscreenPermission').serialize();
    //$('#frmscreenPermission').submit(function () {
    //    initial_form_state = $('#frmscreenPermission').serialize();
    //});
    //$(window).bind('beforeunload', function (e) {
    //    var form_state = $('#frmscreenPermission').serialize();
    //    if (initial_form_state != form_state) {
    //        var message = "You have unsaved changes on this page. Do you want to leave this page and discard your changes or stay on this page?";
    //        e.returnValue = message;
    //        return message;
    //    }
    //});

    //$('#chkAllRoles').change(function () {
    //    if ($(this).is(':checked')) {
    //        $('input[type="checkbox"].sub_check').prop('checked', true);
    //    }
    //    else {
    //        $('input[type="checkbox"].sub_check').prop('checked', false);
    //    }
    //});


    $('#chksub_3_8f696cdd-054b-453c-b34e-3fe299b7e235').on('change', function (e) {
        // alert($(this).is(':checked'));
        $('#chksub_3_be9c75ca-4a88-43a9-851c-356ee8363f99').prop('disabled', true);
        $('#chksub_3_be9c75ca-4a88-43a9-851c-356ee8363f99').prop('checked', false);
        if ($(this).is(':checked')) {
            $('#chksub_3_be9c75ca-4a88-43a9-851c-356ee8363f99').prop('disabled', false);
        }
    });

});

function UpdateCkeck(id) {
    $('input[id^=chksub_' + id + ']').prop('checked', $('#parentChk_' + id).prop('checked'));

    if (id == 3 || id == '3') {
        $('#chksub_3_be9c75ca-4a88-43a9-851c-356ee8363f99').prop('disabled', true);
        $('#chksub_3_be9c75ca-4a88-43a9-851c-356ee8363f99').prop('checked', false);
        if ($('#parentChk_' + id).prop('checked')) {
            $('#chksub_3_be9c75ca-4a88-43a9-851c-356ee8363f99').prop('disabled', false);
        }
    }
}