//var EMPAdminWebAPI = 'http://192.168.10.33:1003';
var EMPAdminWebAPI = 'http://localhost:9001/';
//var EMPAdminWebAPI_C = 'http://182.73.138.108:2001';

function ajaxHelper(uri, method, data) {

    var token = $('#Token').val();
    uri = EMPAdminWebAPI + uri;

    //var headers = {};
    //if (token != null && token != '' && token != '00000000-0000-0000-0000-000000000000') {
    //    headers.Authorization = 'Bearer ' + token;
    //}

    $('#loading_main').show();
    $('#error').hide();
    $('#error p').html('');
    return $.ajax({
        type: method,
        url: uri,
        beforeSend: function (xhr) { xhr.setRequestHeader('Token', token); },
        dataType: 'json',
        contentType: 'application/json',
        data: data ? JSON.stringify(data) : null,
        async: false,
        complete: function (res) {
            $('#loading_main').hide();
        }
    }).fail(function (jqXHR, textStatus, errorThrown) {
        $('#error').show();
        $('#error p').html(errorThrown);
    });
}

function ajaxHelperasync(uri, method, data) {

    var token = $('#Token').val();
    uri = EMPAdminWebAPI + uri;
    $('#error').hide();
    return $.ajax({
        type: method,
        url: uri,
        beforeSend: function (xhr) { xhr.setRequestHeader('Token', token); },
        dataType: 'json',
        contentType: 'application/json',
        data: data ? JSON.stringify(data) : null,
        complete: function () {
        }
    }).fail(function (jqXHR, textStatus, errorThrown) {
        if (jqXHR.status == 404) {
            var UId = $('LoginId').val();
            LogException('Not Found', uri, UId);
        }
        $('#error').show();
        // $('#error').html('<p>' + errorThrown + '</p>');
        $('#error').html('<p>An error occured executing the action. Please try again or contact the support team. </p>');
    });
}

function ajaxUploadHelper(uri, method, data) {
    uri = EMPAdminWebAPI + uri;
    return $.ajax({
        url: uri,
        type: method,
        contentType: false,
        processData: false,
        data: data,
        cache: false,
        beforeSend: function (xhr) { xhr.setRequestHeader('Token', $('#Token').val()) },
        async: false,
    }).fail(function (jqXHR, textStatus, errorThrown) {
        $('#error').show();
        $('#error p').html(errorThrown);
    });
}

$(function () {
    $('a.popup-ajax').popover({
        "html": true,
        "content": function () {
            var div_id = "tmp-id-" + $.now();
            return details_in_popup($(this).attr('href'), div_id);
        }
    });
});

function details_in_popup(link, div_id) {
    $.ajax({
        url: link,
        success: function (response) {
            $('#' + div_id).html(response);
        }
    });
    return '<div id="' + div_id + '">Loading...</div>';
}


function getEntityMaster(container) {

    var entityUri = '/api/EntityMasters/';
    //var select = $('<select />').attr('id', Id).addClass('select');

    ajaxHelper(entityUri, 'GET').done(function (data) {
        $('<option />', { value: '', text: 'Select' }).appendTo(container);
        $.each(data, function (rowIndex, r) {
            // var row = $("<option/>").appendTo(select);;
            //$.each(r, function (colIndex, c) {
            //row.append($("<td/>").text(c));
            $('<option />', { value: r["Id"], text: r["Name"] }).appendTo(container);
            // });
            //  row.append($("<td/>").html("<a href='#' onclick='UserEdit(" + r['Id'] + ")'> Detail </a>"));
        });
    });

    //  container.append(select);
    //for (var val in data) {
    //    $('<option />', { value: val, text: data[val] }).appendTo(s);
    //}
}



function getCustomerMaster(container) {

    var custmorUri = '/api/CustomerMasters/';
    // var select = $('<select />').attr('id', Id).addClass('select');

    ajaxHelper(custmorUri, 'GET').done(function (data) {
        container.append($('<option />', { value: '', text: 'Select' }));
        $.each(data, function (rowIndex, r) {
            // var row = $("<option/>").appendTo(select);;
            //$.each(r, function (colIndex, c) {
            //row.append($("<td/>").text(c));
            container.append($('<option />', { value: r["Id"], text: r["Name"] }));//.appendTo(container);
            // });
            //  row.append($("<td/>").html("<a href='#' onclick='UserEdit(" + r['Id'] + ")'> Detail </a>"));
        });
    });

    // container.append(select);
    //for (var val in data) {
    //    $('<option />', { value: val, text: data[val] }).appendTo(s);
    //}
}



function getGroupMaster(container) {

    var custmorUri = '/api/GroupMasters/';
    //var select = $('<select />').attr('id', Id).addClass('select');

    ajaxHelper(custmorUri, 'GET').done(function (data) {
        container.append($('<option />', { value: '', text: 'Select' }));
        $.each(data, function (rowIndex, r) {
            // var row = $("<option/>").appendTo(select);;
            //$.each(r, function (colIndex, c) {
            //row.append($("<td/>").text(c));
            container.append($('<option />', { value: r["Id"], text: r["Name"] }));
            //$('<option />', { value: r["Id"], text: r["Name"] }).appendTo(container);
            // });
            //  row.append($("<td/>").html("<a href='#' onclick='UserEdit(" + r['Id'] + ")'> Detail </a>"));
        });
    });

    //container.append(select);
    //for (var val in data) {
    //    $('<option />', { value: val, text: data[val] }).appendTo(s);
    //}
}



function getRoleMaster(container) {

    var custmorUri = '/api/RoleMasters/';
    // var select = $('<select />').attr('id', Id).addClass('select');
    var ul = $('<ul/>').addClass('list-unstyled padding-left-4')
    ajaxHelper(custmorUri, 'GET').done(function (data) {
        $.each(data, function (rowIndex, r) {
            // var row = $("<option/>").appendTo(select);;
            //$.each(r, function (colIndex, c) {
            //row.append($("<td/>").text(c));
            ul.append($('<li><input type="checkbox" Id="chk' + r["Id"] + '" name="chk' + r["Id"] + '" class="chkRole" value="' + r["Id"] + '" /> <span>' + r["Name"] + '</span> </li>'));
            // $('<option />', { value: r["Id"], text: r["Name"] }).appendTo(container);
            // });
            //  row.append($("<td/>").html("<a href='#' onclick='UserEdit(" + r['Id'] + ")'> Detail </a>"));
        });
        container.append(ul);
    });

    // container.append(select);
    //for (var val in data) {
    //    $('<option />', { value: val, text: data[val] }).appendTo(s);
    //}
};

function getGroupRoleMap(obj) {

    var id = $(obj).attr('id');
    var objValue = $(obj).val();

    var customerUri = '/api/GroupRoleMaps/';
    var allchkbox = $('input[type=checkbox].chkRole');

    $.each(allchkbox, function (indx, valu) {
        var chkId = $(valu).attr('id');
        // $('#' + chkId).attr('checked', '');
    });
    $('.chkRole').attr("checked", false);
    ajaxHelper(customerUri + objValue, 'GET').done(function (data) {
        $.each(data.Roles, function (coIndex, c) {
            // debugger;
            $('#chk' + c["Id"]).prop('checked', true);

            //$.each(allchkbox, function (indx, valu) {
            //    var chkId = $(valu).attr('id');
            //    if (chkId == 'chk' + c["Id"]) {
            //        $('#' + chkId).attr('checked', 'checked');
            //    }
            //});
        });
    });

}


function getUserRoleMap(Id) {

    var customerUri = '/api/UserRoleMap/';
    var allchkbox = $('input[type=checkbox].chkRole');

    ajaxHelper(customerUri + Id, 'GET').done(function (data) {
        $.each(data, function (rowIndex, r) {

            $('#chk' + r["RoleId"]).attr('checked', 'checked');
            // $.each(allchkbox, function (indx, valu) {
            //     var chkId = $(valu).attr('id');
            //     if (chkId == 'chk' + r["RoleId"]) {
            //         $('#' + chkId).attr('checked', 'checked');
            //     }
            // });
        });
    });
}





function getEntityCheckList(container) {

    var entityUri = '/api/EntityMasters/';
    var ul = $('<ul/>').addClass('list-unstyled padding-left-4')
    ajaxHelper(entityUri, 'GET').done(function (data) {

        var data2 = data;
        $.each(data, function (rowIndex, r) {
            var ParentId = r["ParentId"];
            if (ParentId==1 || ParentId == '1') { //ParentId == null || ParentId == '' || 
                ul.append($('<li><input type="checkbox" Id="chk' + r["Id"] + '" name="chk' + r["Id"] + '" class="chkEntity" value="' + r["Id"] + '" /> <span>' + r["Name"] + '</span> </li>'));
                $.each(data2, function (rowIndex2, r2) {
                    if (r2["ParentId"] == r["Id"]) {
                        ul.append($('<li style="margin-left:10px"><input type="checkbox" Id="chk' + r2["Id"] + '" name="chk' + r2["Id"] + '" class="chkEntity" value="' + r2["Id"] + '" /> <span>' + r2["Name"] + '</span> </li>'));
                    }
                });
            }

        });

        container.append(ul);
    });

}



function getAffiliateEntityMap(obj) {
    var id = $(obj).attr('id');
    var objValue = $(obj).val();

    var customerUri = '/api/AffiliateEntityMap/';
    var allchkbox = $('input[type=checkbox].chkEntity');
    ajaxHelper(customerUri + objValue, 'GET').done(function (data) {

        $.each(data.Entity, function (coIndex, c) {
            $.each(allchkbox, function (indx, valu) {
                var chkId = $(valu).attr('id');
                if (chkId == 'chk' + c["Id"]) {
                    $('#' + chkId).attr('checked', 'checked');
                }
            });
        });
    });
}


function GotoListScreen(url) {
    window.location.href = url;
}

function ActionList(url, Id, Status) {
    var active = "<i class='fa fa-circle' aria-hidden='true' title='Activated'></i>";
    var inactive = "<i class='fa fa-circle-o' aria-hidden='true'  title='Inactivated'></i>";
    var StatusCode = "";
    var actClass = "";

    if (Status == 'INA') {
        Status = 'ACT';
        StatusCode = 'Activate';
        actClass = active;
    } else if (Status == 'ACT') {
        Status = 'INA';
        StatusCode = 'Deactivate';
        actClass = inactive;
    }

    var actions = '<td><div class="btn-group dropleft"><button class="btn dropdown-toggle" aria-expanded="false" aria-haspopup="true" data-toggle="dropdown">Actions <span class="caret"></span></button>';
    actions = actions + '<ul class="dropdown-menu dropdown-menu-right" aria-labelledby="dropdownMenu" role="menu">';
    if (Status == 'INA') {
        actions = actions + '<li class="EditLink">';
        if (url == '/BankSubQuestions/create/') {
            actions = actions + '<a href="' + url + "?Id=" + Id + "&BankId=" + Id + '"><i class="fa fa-pencil-square-o" aria-hidden="true" title="Edit"></i> Edit</a>';
        }
        else {
            actions = actions + '<a href="' + url + Id + '"><i class="fa fa-pencil-square-o" aria-hidden="true" title="Edit"></i> Edit</a>';
        }
        actions = actions + '</li>';
        actions = actions + '<li class="divider"></li>';
    }
    actions = actions + '<li class="ActiveInactiveLink">';
    actions = actions + '<a href="javascript:void(0);"   onclick="fnSaveStatus(' + "'" + Id + "'" + ',' + "'" + Status + "'" + ') "> ' + actClass + ' ' + StatusCode + '  </a>';
    actions = actions + '</li>';
    actions = actions + '</ul>';
    actions = actions + '</div></td>';

    return actions;
}

function EditPopUpActionList(url, Id, Status) {
    var active = "<i class='fa fa-circle' aria-hidden='true' title='Activated'></i>";
    var inactive = "<i class='fa fa-circle-o' aria-hidden='true'  title='Inactivated'></i>";
    var StatusCode = "";
    var actClass = "";

    if (Status == 'INA') {
        Status = 'ACT';
        StatusCode = 'Activate';
        actClass = active;
    } else if (Status == 'ACT') {
        Status = 'INA';
        StatusCode = 'Deactivate';
        actClass = inactive;
    }

    var actions = actions + '<td><div class="btn-group dropleft"><button class="btn dropdown-toggle" aria-expanded="false" aria-haspopup="true" data-toggle="dropdown">Actions <span class="caret"></span></button>';
    actions = actions + '<ul class="dropdown-menu dropdown-menu-right" aria-labelledby="dropdownMenu" role="menu">';
    if (Status == 'INA') {
        actions = actions + '<li  class="EditLink">';
        //actions = actions + '<a href="' + url + Id + '"><i class="fa fa-pencil-square-o" aria-hidden="true" title="Edit"></i> Edit</a>';
        actions = actions + '<a href="javascript:void(0);"   onclick="' + url + '(' + "'" + Id + "'" + ',' + "'" + Status + "'" + ')" data-toggle="modal" data-target="#popup" title="Edit" > <i class="fa fa-pencil-square-o" aria-hidden="true" title="Edit"></i> Edit</a>';
        actions = actions + '</li>';
        actions = actions + '<li class="divider"></li>';
    }
    actions = actions + '<li class="ActiveInactiveLink">';
    actions = actions + '<a href="javascript:void(0);"   onclick="fnSaveStatus(' + "'" + Id + "'" + ',' + "'" + Status + "'" + ') "> ' + actClass + ' ' + StatusCode + '  </a>';
    actions = actions + '</li>';
    actions = actions + '</ul>';
    actions = actions + '</div></td>';

    return actions;
}



function EditActionList(url, Id) {
    var actions = '<td><div class="btn-group dropleft"><button class="btn dropdown-toggle" aria-expanded="false" aria-haspopup="true" data-toggle="dropdown">Actions <span class="caret"></span></button>';
    actions = actions + '<ul class="dropdown-menu dropdown-menu-right" aria-labelledby="dropdownMenu" role="menu">';
    actions = actions + '<li  class="EditLink">';
    actions = actions + '<a href="' + url + Id + '"><i class="fa fa-pencil-square-o" aria-hidden="true" title="Edit"></i> Edit</a>';
    actions = actions + '</li>';
    actions = actions + '</ul>';
    actions = actions + '</div></td>';
    return actions;
}

function SalesYearActionList(url, Id) {
    var actions = '<td><div class="btn-group dropleft"><button class="btn dropdown-toggle" aria-expanded="false" aria-haspopup="true" data-toggle="dropdown">Actions <span class="caret"></span></button>';
    actions = actions + '<ul class="dropdown-menu dropdown-menu-right" aria-labelledby="dropdownMenu" role="menu">';
    actions = actions + '<li>';
    actions = actions + '<a href="' + url + Id + '"><i class="fa fa-pencil-square-o" aria-hidden="true" title="View"></i> View</a>';
    actions = actions + '</li>';
    actions = actions + '</ul>';
    actions = actions + '</div></td>';
    return actions;
}

function BankActionList(url, Id, BankUrl, BankId, Status) {
    var active = "<i class='fa fa-circle' aria-hidden='true' title='Activated'></i>";
    var inactive = "<i class='fa fa-circle-o' aria-hidden='true'  title='Inactivated'></i>";
    var StatusCode = "";
    var actClass = "";

    if (Status == 'INA') {
        Status = 'ACT';
        StatusCode = 'Activate';
        actClass = active;
    } else if (Status == 'ACT') {
        Status = 'INA';
        StatusCode = 'Deactivate';
        actClass = inactive;
    }

    var actions = '<td><div class="btn-group dropleft"><button class="btn dropdown-toggle" aria-expanded="false" aria-haspopup="true" data-toggle="dropdown">Actions <span class="caret"></span></button>';
    actions = actions + '<ul class="dropdown-menu dropdown-menu-right" aria-labelledby="dropdownMenu" role="menu">';
    if (Status == 'INA') {
        actions = actions + '<li class="EditLink">';
        actions = actions + '<a href="' + url + Id + '"><i class="fa fa-pencil-square-o" aria-hidden="true" title="Edit"></i> Edit</a>';
        actions = actions + '</li>';
        actions = actions + '<li class="divider"></li>';
        actions = actions + '<li class="EditSubQuestionsLink"">';
        actions = actions + '<a href="' + BankUrl + Id + '"><i class="fa fa-bars" aria-hidden="true" title="Edit"></i>  Manage Sub Question</a>';
        actions = actions + '</li>';
        actions = actions + '<li class="divider"></li>';
    }
    actions = actions + '<li class="ActiveInactiveLink"">';
    actions = actions + '<a href="javascript:void(0);"   onclick="fnSaveStatus(' + "'" + Id + "'" + ',' + "'" + Status + "'" + ') "> ' + actClass + ' ' + StatusCode + '  </a>';
    actions = actions + '</li>';
    actions = actions + '</ul>';
    actions = actions + '</div></td>';
    return actions;
}



function EditPopUpActionListForRole(url, Id, Status, RolePermissionUrl) {
    var active = "<i class='fa fa-circle' aria-hidden='true' title='Activated'></i>";
    var inactive = "<i class='fa fa-circle-o' aria-hidden='true'  title='Deactivated'></i>";
    var StatusCode = "";
    var actClass = "";

    if (Status == 'INA') {
        Status = 'ACT';
        StatusCode = 'Activate';
        actClass = active;
    } else if (Status == 'ACT') {
        Status = 'INA';
        StatusCode = 'Deactivate';
        actClass = inactive;
    }

    var actions = actions + '<td><div class="btn-group dropleft"><button class="btn dropdown-toggle" aria-expanded="false" aria-haspopup="true" data-toggle="dropdown">Actions <span class="caret"></span></button>';
    actions = actions + '<ul class="dropdown-menu dropdown-menu-right" aria-labelledby="dropdownMenu" role="menu">';
    if (Status == 'INA') {
        actions = actions + '<li class="EditLink">';
        actions = actions + '<a href="javascript:void(0);"   onclick="' + url + '(' + "'" + Id + "'" + ',' + "'" + Status + "'" + ')" data-toggle="modal" data-target="#popup" title="Edit" > <i class="fa fa-pencil-square-o" aria-hidden="true" title="Edit"></i> Edit</a>';
        actions = actions + '</li>';
        actions = actions + '<li class="divider"></li>';
        actions = actions + '<li class="EditPermissionLink"">';
        actions = actions + '<a href="' + RolePermissionUrl + Id + '"><i class="fa fa-bars" aria-hidden="true" title="Role Permission"></i>  Manage Role Permission</a>';
        actions = actions + '</li>';
        actions = actions + '<li class="divider"></li>';
    }
    actions = actions + '<li class="ActiveInactiveLink">';
    actions = actions + '<a href="javascript:void(0);"   onclick="fnSaveStatus(' + "'" + Id + "'" + ',' + "'" + Status + "'" + ') "> ' + actClass + ' ' + StatusCode + '  </a>';
    actions = actions + '</li>';
    actions = actions + '</ul>';
    actions = actions + '</div></td>';

    return actions;
}





function getScreens(container, userId) {

    var entityUri = '/api/SitemapMasters/';
    var html = $('<ul/>').addClass('sub-menu');
    ajaxHelper(entityUri + userId, 'GET').done(function (data) {
        var data2 = data;
        $.each(data, function (rowIndex, r) {
            html.append($('<li class=\"notfor_link limanage_cities\"><a href="' + r["URL"] + '" data-toggle="tooltip" data-placement="bottom" title=""><i class="fa fa-angle-double-right"></i>' + r["Name"] + '</a></li>'));
            //$.each(data2, function (rowIndex2, r2) {
            //    if (r2["ParentId"] == r["Id"]) {
            //        ul.append($('<li style="margin-left:10px"><input type="checkbox" Id="chk' + r2["Id"] + '" name="chk' + r2["Id"] + '" class="chkEntity" value="' + r2["Id"] + '" /> <span>' + r2["Name"] + '</span> </li>'));
            //    }
            //});
        });

        container.append(html);
    });
}

function getScreensAdmin(container, userId) {

    var entityUri = '/api/AdminSitemapMasters?id=admin';
    var html = $('<ul/>').addClass('sub-menu');
    ajaxHelper(entityUri, 'GET').done(function (data) {
        var data2 = data;
        $.each(data, function (rowIndex, r) {
            html.append($('<li class=\"notfor_link limanage_cities\"><a href="' + r["URL"] + '" data-toggle="tooltip" data-placement="bottom" title=""><i class="fa fa-angle-double-right"></i>' + r["Name"] + '</a></li>'));
        });
        container.append(html);
    });
}




//http://stackoverflow.com/questions/19605150/regex-for-password-must-be-contain-at-least-8-characters-least-1-number-and-bot
function ValidatePassword(value) {
    var regex = /^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[^\w\s]).{8,}$/;
    return regex.test(value);
}

function ValidateEmail(value) {
    var pattern = new RegExp(/^([\w-\.]+@([\w-]+\.)+[\w-]{2,4})?$/);
    return pattern.test(value);
}

$(function () {
    $('input[type=number].digit, input[type=text].digit').keypress(function (event) {
        if (event.which != 8 && event.which != 0 && isNaN(String.fromCharCode(event.which))) {
            event.preventDefault(); //stop characters from entering input
        }
    });

    $('input[type=text].alphanumaric').keypress(function (event) {
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
    });
    $('input[type=text].alpha').keypress(function (event) {
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
    });

    $('input[type=number].decimal, input[type=text].decimal').keypress(function (event) {

        if (event.which != 8 && event.which != 0 && event.which != 46 && isNaN(String.fromCharCode(event.which))) {
            event.preventDefault(); //stop characters from entering input
        }

        var dotIndx = $(this).val().indexOf('.');
        if (Number(dotIndx) > 0 && event.which == 46) {
            event.preventDefault();
        }
    });
});

function getScreenPermissions(ctrlAddLink, ctrlEditLink, ctrlActiveInactiveLink, ctrlOtherLink, userId, roleId) {

    var Username = $('#UserName').val();

    if (Username.toLowerCase() != "admin") {

        ctrlAddLink.hide();
        ctrlEditLink.hide();
        ctrlActiveInactiveLink.hide();
        if (ctrlOtherLink)
            ctrlOtherLink.hide();

        var IsAllFalse = false;
        var data = $('#UserPermissions').val().split('_');
        debugger;
        var Sitemapid = $('#SiteMapID').val();
        $.each(data, function (rowIndex, row) {
            var sp = row.split(':');
            if (sp[0] == Sitemapid) {
                if (sp[1] == "Add") {
                    ctrlAddLink.show();
                }
                //debugger;
                if (sp[1] == "Edit") {
                    ctrlEditLink.show();
                    IsAllFalse = true;
                }
                if (sp[1] == "Deactivate/Activate") {
                    ctrlActiveInactiveLink.show();
                    IsAllFalse = true;
                }
                if (ctrlOtherLink != '') {
                    if (sp[1] == "Manage Bank Sub Questions") {
                        ctrlOtherLink.show();
                        IsAllFalse = true;
                    }
                    if (sp[1] == "Manage Role Permission") {
                        ctrlOtherLink.show();
                        IsAllFalse = true;
                    }
                }
            }
        });
        if (!IsAllFalse) {
            $('.btn-group').hide();
        }
    }
}


//function getScreenPermissions(ctrlAddLink, ctrlEditLink, ctrlActiveInactiveLink, ctrlEditSubQuestionsLink, userId, roleId) {
//    // var url = '/api/PermissionMasters?';
//    //var userId = $("#UserId").val();
//    //var roleId = $("#UserRoleId").val();

//    ctrlAddLink.hide();
//    ctrlAddLink.hide();
//    ctrlActiveInactiveLink.hide();
//    ctrlEditSubQuestionsLink.hide();
//    var IsAllFalse = false;
//    // ajaxHelper(url + "userId=" + userId + "&roleId=" + roleId, 'GET').done(function (data) { 

//    // site:per_site:per
//    debugger;
//    var Sitemapid = $('#SiteMapID').val();
//    var data = $('#UserPermissions').val().split('_');
//    $.each(data, function (rowIndex, row) {
//        var sp = row.split(':');

//        $.each(sp, function (rIndex, r) {

//            if (r[0] == Sitemapid) {
//                if (r[1] == "Add") {
//                    $("#AddLink").show();
//                }
//                if (r[1] == "Edit") {
//                    $("li.EditLink").show();
//                    IsAllFalse = true;
//                }
//                if (r[1] == "Deactivate/Activate") {
//                    $("li.ActiveInactiveLink").show();
//                    IsAllFalse = true;
//                }
//                if (r[1] == "Manage Bank Sub Questions") {
//                    $("li.EditSubQuestionsLink").show();
//                    IsAllFalse = true;
//                }
//            }
//        });
//    });

//    if (!IsAllFalse) {
//        $('.btn-group').hide();
//    }
//}



//function getScreenPermissions(ctrlAddLink, ctrlEditLink, ctrlActiveInactiveLink, ctrlEditPermissionLink, userId, roleId) {
//    // var url = '/api/PermissionMasters?';
//    //var userId = $("#UserId").val();
//    //var roleId = $("#UserRoleId").val();

//    ctrlAddLink.hide();
//    ctrlEditLink.hide();
//    ctrlActiveInactiveLink.hide();
//    ctrlEditPermissionLink.hide();
//    var IsAllFalse = false;
//    // ajaxHelper(url + "userId=" + userId + "&roleId=" + roleId, 'GET').done(function (data) {
//    var data = $('#UserPermissions').val().split('_');
//    $.each(data, function (rowIndex, r) {

//        if (r == "Add") {
//            ctrlAddLink.show();
//        }

//        if (r == "Edit") {
//            ctrlEditLink.show();
//            IsAllFalse = true;
//        }

//        if (r == "Deactivate/Activate") {
//            ctrlActiveInactiveLink.show();
//            IsAllFalse = true;
//        }

//        if (r == "Manage Role Permission") {
//            ctrlEditPermissionLink.show();
//            IsAllFalse = true;
//        }
//    });
//    // });

//    if (!IsAllFalse) {
//        $('.btn-group').hide();
//    }
//}




function getDocument(Id,Container) {
    var booksUri = '/api/Document/Download';
    ajaxHelper(booksUri + "?Id=" + Id, 'GET').done(function (data) {
        Container.attr('href', EMPAdminWebAPI + "/" + data);
        Container.show();
    });
}
