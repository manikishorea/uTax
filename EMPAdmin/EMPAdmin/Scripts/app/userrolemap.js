$(document).ready(function () {
    getAllUser();
});

function getAllUser() {
    var booksUri = '/api/UserRoleMap/';
    var table = $('#table_user'); //$(document.body)
    ajaxHelper(booksUri, 'GET').done(function (data) {
        $.each(data, function (rowIndex, r) {
            var row = $("<tr/>").appendTo(table);;
          //  $.each(r, function (colIndex, c) {
            row.append($("<td/>").text(r["Name"]));
            row.append($("<td/>").text(r["UserName"]));
            row.append($("<td/>").text(r["RoleName"]));
            row.append($("<td/>").text(r["StatusCode"]));
          //  });
            row.append($("<td/>").html("<a href='/userrolesmap/create/" + r['UserId'] + "'> Detail </a>"));
        });
    });
}


$(document).ready(function () {
    var Id = $('#Id').val();
    getEntityMaster($('#div_entity'), 'EntityId');
    getCustomerMaster($('#div_customer'), 'CustomerId');
    getUserRoleMapDetail(Id);

});

function getUserRoleMapDetail(Id) {

    var userUri = '/api/UserRoleMap/';
    if (Id != '' && Id != null && Id != '00000000-0000-0000-0000-000000000000') {
        //  var table = $('<table/>').addClass('table').attr('id', 'table_bookdetail'); //$(document.body)
        ajaxHelper(userUri + Id, 'GET').done(function (data) {
            $('#UserId').val(data["UserId"])
            $('#RoleName').val(data["RoleName"])
            $('#StatusCode').val($('#StatusCode').val())
        });
    }
}


function fnSaveUser() {
    $('#success').hide();
    $('#success p').html('');
    var Dto = [];
    var Guid = '00000000-0000-0000-0000-000000000000'
    var UserId = $('#UserId').val();
    var allchkbox = $('input[type=radio].chkquery');

    $.each(allchkbox, function (indx, valu) {

        var id = $(valu).attr('id');
        Dto.push({
            UserId: UserId,
            RoleId: id,
            Id: Guid
        });
    });

    var userUri = '/api/UserRoleMap/';
    ajaxHelper(userUri, 'POST', Dto).done(function (data) {
        $('#Id').val(data["Id"]);
        $('#Id').val(data["Id"])
        $('#success').show();

        if (req.Id == data["Id"]) {
            $('#success p').html('User Role updated successfully.');
        } else {
            $('#success p').html('User Role created successfully.');
        }
    });
}
