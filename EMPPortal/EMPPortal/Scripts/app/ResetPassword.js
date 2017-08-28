function fnSaveResetPassword()
{
    $('#popup_success').hide();
    $('#popup_success p').html('');
    var req = {};
    var cansubmit = true;

     
    $('#EMPPassword').removeClass("error_msg");
    if ($.trim($('#EMPPassword').val()) == "") {
        $('#EMPPassword').addClass("error_msg");
        $('#EMPPassword').attr('title', 'Please Enter  New Password');
        cansubmit = false;
    }

    if (cansubmit) {

        req.Id = $('#Id').val();
        req.QuestionId = $('#EMPPassword').val();

        var Uri =  '/api/ResetPassword';
        ajaxHelper(Uri, 'POST', req).done(function (data) {
            $('#Id').val(data["Id"])
            $('#popup_success').show();

            if (req.Id == data["Id"]) {
                $('#popup_success p').html('Phone Type updated successfully.');
            } else {
                $('#popup_success p').html('Phone Type created successfully.');
            }
            // getAllPhoneType();
        });
    }

}