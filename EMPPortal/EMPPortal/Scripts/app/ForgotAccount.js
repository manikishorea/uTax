function fnCheckUser() {
    var UserId = $('#UserId').val();
    var Uri = '/api/ForgotAccount/Password';
    ajaxHelper(Uri + '?id=' + UserId, 'GET').done(function (data) {
    });
}

function getQuestionMaster(container1, container2, container3) {
    //  var id = localStorage.getItem('Id');
    //  $('#Id').val(id);

    var custmorUri = '/api/SecurityAnswer';
    ajaxHelper(custmorUri, 'GET').done(function (data) {

        container1.append($('<option />', { value: '', text: 'Select' }));
        $.each(data, function (rowIndex, r) {
            container1.append($('<option />', { value: r["Id"], text: r["Question"] }));
            //$("<td/>").html("<a href='#' onclick='UserEdit(" + r['Id'] + ")'> Detail </a>"));
        });

        container2.append($('<option />', { value: '', text: 'Select' }));
        $.each(data, function (rowIndex, r) {
            container2.append($('<option />', { value: r["Id"], text: r["Question"] }));
            //$("<td/>").html("<a href='#' onclick='UserEdit(" + r['Id'] + ")'> Detail </a>"));
        });

        container3.append($('<option />', { value: '', text: 'Select' }));
        $.each(data, function (rowIndex, r) {
            container3.append($('<option />', { value: r["Id"], text: r["Question"] }));
            //$("<td/>").html("<a href='#' onclick='UserEdit(" + r['Id'] + ")'> Detail </a>"));
        });
    });
}

function fnForgotPassword() {
    $('#success').hide();
    $('#success').html('');

    $('#error').hide();
    $('#error').html('');
    var req = {};
    var cansubmit = true;

    var Questions = [];

    if (cansubmit) {

        Questions.push({
            QuestionId: $('#Question1').val(),
            Answer: $('#Answer1').val()
        });

        Questions.push({
            QuestionId: $('#Question2').val(),
            Answer: $('#Answer2').val(),
        });


        Questions.push({
            QuestionId: $('#Question3').val(),
            Answer: $('#Answer3').val(),
        });

        req.UserName = $('#UserId').val();
        req.QuestionsLst = Questions;
        // req.Status = $('#status').val();

        var Uri = '/api/ForgotAccount';
        ajaxHelper(Uri, 'POST', req).done(function (data) {

            if (data == 'true' || data == 'True' || data == true) {
                $('#success').show();
                $('#success').html('<p>Security Question Answers matched Successfully.</p>');
                window.location.href = '/ChangePassword/ResetPassword?UserId=' + $('#UserId').val();
            } else {
                $('#error').show();
                $('#error').html('<p>Security Question Answers not matched, Please contact Customer Support</p>');
            }
        });
    }
}