


function fnSaveCustomerLogin() {
    var req = {};
    var cansubmit = true;

    var error = $('#error');
    $('#error').html('');

    $('#UserID').removeClass("error_msg");
    if ($.trim($('#UserID').val()) == "") {
        $('#UserID').addClass("error_msg");
        $('#UserID').attr('title', 'Please enter User Id');
      //  error.append('<p> Please enter User Id </p>');
        cansubmit = false;
    }

    $('#Password').removeClass("error_msg");
    if ($.trim($('#Password').val()) == "") {
        $('#Password').addClass("error_msg");
        $('#Password').attr('title', 'Please enter Password');
     //   error.append('<p> Please enter Password </p>');
        cansubmit = false;
    }

    if (cansubmit) {
        error.hide();
        //req.Id = $('#Id').val();
        req.CrossLinkUserId = $('#UserID').val();
        req.EMPPassword = $('#Password').val();

        var Uri = '/api/CustomerLogin/';
        ajaxHelper(Uri, 'POST', req).done(function (data) {

            if (data["Id"] != null) {


                //   Session["UserId"] = data["Id"];

                var idvalue = data["Id"];
                localStorage.setItem('Id', idvalue);
                window.location.href = '/ChangePassword/InitialPassword';

                //            **Get the Session variable**

                //var id= localStorage.getItem('AdminId');
                //            alert(id)

            }
            else { return false; }
        });
    } else {

        error.show();
        error.append('<p> Please provide the correct data </p>');
    }
}