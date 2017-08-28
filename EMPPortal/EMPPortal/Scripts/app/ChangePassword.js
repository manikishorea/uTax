function fnChangePassword() {

    var req = {};
    var cansubmit = true;
    var error = $('#error');
    $('#error').html('');

    var inputPassword = $.trim($('#inputPassword').val());
    var inputConformPassword = $.trim($('#inputConformPassword').val());

    if (inputPassword == '') {
        $('#inputPassword').attr('title', 'Please Enter New Password');
        $('#inputPassword').addClass("error_msg");
     //   error.append('<p> Please Enter New Password </p>');
        cansubmit = false;
    }

    if (inputPassword != '') {
        if (inputPassword != inputConformPassword) {
            $('#inputPassword').attr('title', 'New password and Confirm password are not matching');
            $('#inputPassword').addClass("error_msg");
          //  error.append('<p> New password and Confirm password are not matching </p>');
            cansubmit = false;
        }
    }

    if (cansubmit) {

        error.hide();
        error.html('');

        req.Id = $('#UserId').val();
        req.UserId = $('#UserId').val();
        req.EMPPassword = $('#inputPassword').val();
        var Uri = '/api/ChangePassword';

        ajaxHelper(Uri, 'POST', req).done(function (data) {
            if (data == true || data == 'True' || data == 'true') {
                //window.location.href = '/CustomerInformation/';
                UpdateOfficeManagement(req.UserId);
                SkipPassword();
            }
            else {
                error.show();
                error.append('<p> Password not updated.</p>');
            }

        });

    } else {
        error.show();
    }
}

function SkipPassword() {
    $.ajax({
        type: 'GET',
        url: '/ChangePassword/SkipPassword?CustomerId=' + $('#UserId').val(),
        success: function (res) {
            if (res)
                window.location.href = res;
        }
    })
   
}
