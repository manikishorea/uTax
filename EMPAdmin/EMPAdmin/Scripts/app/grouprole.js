function getAllGroupRoles() {

    var booksUri = '/api/GroupRoleMaps/';

    $("#table_grouprole > tbody").remove();
    var table = $('#table_grouprole').append('<tbody/>'); //$(document.body)
    ajaxHelper(booksUri, 'GET').done(function (data) {
        $.each(data, function (rowIndex, r) {
            var row = $("<tr/>").appendTo(table);;

            // row.append($("<td/>").text(r["Name"]));
            row.append($("<td/>").text(r["GroupName"]));
            var rolename = '<ul class="list-unstyled">';
            $.each(r.Roles, function (colIndex, c) {
                rolename = rolename + '<li>' + c["Name"] + '</li>';
            });
            rolename = rolename + '</ul>';
            row.append($("<td/>").html(rolename));

            //row.append('<td><a href="/GroupRoleMaps/create/' + r["GroupId"] + '"> <i class="fa fa-pencil-square-o" aria-hidden="true" title="Edit"></i> </a> &nbsp;&nbsp;  <a href="javascript:void(0);"  onclick="fnSaveStatus(' + "'" + r["GroupId"] + "'" + ',' + "'" + Status + "'" + ') "> ' + actClass + ' </a></td>');
            //r["StatusCode"]=="ACT"?"Activate":"Deactivate";
            row.append($("<td/>").text(r["StatusCode"] == "ACT" ? "Activate" : "Deactivate"));
            row.append(ActionList('/GroupRoleMaps/create/', r["GroupId"], r["StatusCode"]));
        });
    });
    $(".office-table .table").trigger('footable_initialize');
   // GridFunctionality();
}


function getGroupRolesByGroupId(Id) {

    //$('input[type=checkbox].chkRole').removeAttr('checked');
    var customerUri = '/api/GroupRoleMaps/';
    var allchkbox = $('input[type=checkbox].chkRole');
    ajaxHelper(customerUri + Id, 'GET').done(function (data) {

        $.each(data.Roles, function (coIndex, c) {

            $('#chk' + c["Id"]).attr('checked', 'checked');
            //  $.each(allchkbox, function (indx, valu) {
            //      var chkId = $(valu).attr('id');
            //      if (chkId == 'chk' + c["Id"]) {
            //         $('#' + chkId).attr('checked', 'checked');
            //     }
            //  });
        });

        $('#GroupName').val(data["GroupName"]);
    });
}


function fnSaveGroupRole() {
    $('#error').hide();
    $('#error p').html('');

    $('#success').hide();
    $('#success p').html('');
    var dto = {};
    var cansubmit = true;
    $('#GroupName').removeClass("error_msg");
    if ($.trim($('#GroupName').val()) == "") {
        $('#GroupName').addClass("error_msg");
        $('#GroupName').attr('title', 'Please enter Group');
        cansubmit = false;
    }
    dto.Id = $('#Id').val();
    dto.UserId = $('#UserId').val();
    dto.GroupName = $('#GroupName').val();
    var Roles = [];

    var IsRoleChecked = false;
    var allchkbox = $('input[type=checkbox].chkRole');
    $.each(allchkbox, function (indx, valu) {
        var chkId = $(valu).attr('id');
        var valR = $('#' + chkId).val()
        if ($(valu).is(':checked')) {
            IsRoleChecked = true;
            Roles.push({
                Id: valR
            });
        }
    });

    dto.Roles = Roles;
    $('#RoleIds').removeClass("error_msg");
    if (!IsRoleChecked) {
        $('#RoleIds').addClass("error_msg");
        $('#RoleIds').attr('title', 'Please select at least one role');
        cansubmit = false;
    }
    if (cansubmit) {
        var userUri = '/api/GroupRoleMaps';
        ajaxHelper(userUri, 'POST', dto).done(function (data, textStatus) {
            if (textStatus == 'notmodified') {
                $('#error p').html('Group is already exist.');
                $('#error').show();
            } else if (textStatus == 'notccceptable') {
                $('#error p').html('Group has not saved.');
                $('#error').show();
            } else {
                var dataId = data;
                $('#Id').val(dataId)
                $('#success').show();
                if (dto.Id == dataId) {
                    $('#success p').html('Group Role(s) updated successfully.');
                } else {
                    $('#success p').html('Group Role(s) created successfully.');
                }
            }
        });
    }
}

function fnSaveStatus(Id, Status) {
    $('#error').hide();
    $('#error p').html('');
    $('#success').hide();
    $('#success p').html('');
    var statuscode = '';
    if (Status == "INA") {
        statuscode = "Deactivate";
    }
    else {
        statuscode = "Activate"
    }
    var conformResult = confirm("Are you sure you wish to  " + statuscode + "  the specific Group Role \n and no longer have it available for the users to select ?");
    if (conformResult == true) {


        var req = {};
        var cansubmit = true;

        if (cansubmit) {

            req.Id = Id;
            req.PhoneType = $('#PhoneType').val();
            req.StatusCode = Status;

            var reqUri = '/api/GroupRoleMaps/';
            ajaxHelper(reqUri + Id, 'PUT', req).done(function (datajq, textStatus) {
                //window.location.href = "/GroupRoleMaps/";
                $('#error').show();

                if (textStatus == 'notmodified') {
                    $('#error p').html("Group can not be Deactivated. It's associated with user");
                } else {
                    $('#success').show();
                    $('#success p').html('This Group ' + statuscode + 'd successfully.');
                    getAllGroupRoles();
                }
            });
        }
    }
    else {
        return false;
    }
}