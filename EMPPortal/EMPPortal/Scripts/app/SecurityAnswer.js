function getQuestionMaster(container1, container2, container3) {
    var custmorUri = '/api/SecurityAnswer';
    ajaxHelper(custmorUri, 'GET').done(function (data) {

        var questionList = [container1, container2, container3];

        $.each(questionList, function (index, value) {
            value.append($('<option />', { value: '', text: 'Please Select ' }));
            $.each(data, function (rowIndex, r) {
                value.append($('<option />', { value: r["Id"], text: r["Question"] }));
            });
        });

        getSecAnswer();
    });
}

function getSecAnswer() {
    var UserId = $('#UserId').val();
    var custmorUri = '/api/SecurityAnswer?id=';
    ajaxHelper(custmorUri + UserId, 'GET').done(function (data) {

        $.each(data, function (rowIndex, r) {
            var displayorder = r["DisplayOrder"];
            $('#QuestionId' + displayorder).val(r["QuestionId"]);
            $('#Answer' + displayorder).val(r["Answer"]);
        });

        var questions = [1, 2, 3];

        $.each(questions, function (qindex, qdata) {
            var qfiltered = questions.filter(function (a) { return a != qdata });
            $.each(qfiltered, function (findex, fdata) {
                $('#QuestionId' + qdata + ' option[value=' + $.trim($('#QuestionId' + fdata).val()) + ']').prop('disabled',true);
            });
        });
    });
}

function fnSaveSecurityAnswer() {
    
    $('#success').hide();
    var cansubmit = true;
    var error = $('#error');
    $('#error').html('');
    $('#error').hide();

    var req = {};
    // cansubmit = true;

    var Question = [];

    var QuestionIdId1 = $('#QuestionId1 :selected').val();
    var Answer1 = $('#Answer1').val();

    $('#Answer1').removeClass("error_msg");
    $('#QuestionId1').removeClass("error_msg");
    if ($.trim(QuestionIdId1) == '') {
        $('#QuestionId1').addClass("error_msg");
        $('#QuestionId1').attr('title', 'Please select Security Question 1');

        // error.append('<p> Please select Question 1 </p>');
        cansubmit = false;
    }

    if ($.trim(Answer1).length < 3) {
        $('#Answer1').addClass("error_msg");
        $('#Answer1').attr('title', 'Please enter Security Answer 1. It requires a minimum of 3 characters');

        //  error.append('<p> Please enter Answer 1 </p>');
        cansubmit = false;
    }


    $('#Answer2').removeClass("error_msg");
    $('#QuestionId2').removeClass("error_msg");
    var QuestionIdId2 = $('#QuestionId2 :selected').val();
    var Answer2 = $('#Answer2').val();

    if ($.trim(QuestionIdId2) == '') {
        $('#QuestionId2').addClass("error_msg");
        $('#QuestionId2').attr('title', 'Please select Security Question 2');

        // error.append('<p> Please select Question 3 </p>');
        cansubmit = false;
    }
    if ($.trim(Answer2).length < 3) {
        $('#Answer2').addClass("error_msg");
        $('#Answer2').attr('title', 'Please enter Security Answer 2. It requires a minimum of 3 characters');

        //  error.append('<p> Please enter Answer 2 </p>');
        cansubmit = false;
    }

    $('#Answer3').removeClass("error_msg");
    $('#QuestionIdId3').removeClass("error_msg");
    var QuestionIdId3 = $('#QuestionId3 :selected').val();
    var Answer3 = $('#Answer3').val();

    if ($.trim(QuestionIdId3) == '') {
        $('#QuestionId3').addClass("error_msg");
        $('#QuestionId3').attr('title', 'Please select Security Question 3');

        //  error.append('<p> Please select Question 3 </p>');
        cansubmit = false;
    }
    if ($.trim(Answer3).length < 3) {
        $('#Answer3').addClass("error_msg");
        $('#Answer3').attr('title', 'Please enter Security Answer 3. It requires a minimum of 3 characters');

        // error.append('<p> Please enter Answer 3</p>');
        cansubmit = false;
    }

    if ($.trim($('#QuestionId1').val()) == $.trim($('#QuestionId3').val())) {
        $('#QuestionId3').addClass("error_msg");
        $('#QuestionId3').attr('title', 'This option already selected, Please select different option');
        cansubmit = false;
    }

    if ($.trim($('#QuestionId1').val()) == $.trim($('#QuestionId2').val())) {
        $('#QuestionId2').addClass("error_msg");
        $('#QuestionId2').attr('title', 'This option already selected, Please select different option');

        cansubmit = false;
    }


    if ($.trim($('#QuestionId2').val()) == $.trim($('#QuestionId3').val())) {
        $('#QuestionId3').addClass("error_msg");
        $('#QuestionId3').attr('title', 'This option already selected, Please select different option');
        cansubmit = false;
    }


    if (!cansubmit) {
        error.show();
        error.append('<p> Please provide the correct data</p>');
    }

    if (cansubmit) {

        Question.push({
            QuestionId: QuestionIdId1,
            Answer: Answer1,
            DisplayOrder: 1
        });

        Question.push({
            QuestionId: QuestionIdId2,
            Answer: Answer2,
            DisplayOrder: 2
        });


        Question.push({
            QuestionId: QuestionIdId3,
            Answer: Answer3,
            DisplayOrder: 3
        });

        req.Id = $('#UserId').val();
        req.UserId = $('#UserId').val();
        req.QuestionsLst = Question;
        req.Status = $('#status').val();

        var Uri = '/api/SecurityAnswer';
        ajaxHelper(Uri, 'POST', req).done(function (data) {

            if (data == 'true' || data == 'True' || data == true) {
                $('#success').show();
                $('#success').html('<p>Security Question Answers Saved Successfully.</p>');

                if (req.Status != "2") {
                    window.location.href = '/ChangePassword/InitialPassword';
                }

            } else {
                $('#error').show();
                $('#error').html('<p>Security Question Answers not saved.</p>');
            }
        });
    }
}

function fnQuestionSelected(obj) {
    var cansubmit = true;
    var error = $('#error');
    $('#error').html('');
    $('#error').hide();

    $(obj).removeClass("error_msg");
    $(obj).attr('title', '');

    var questions = [1, 2, 3];

    $.each(questions, function (qindex, qdata) {
        $('#QuestionId' + qdata + ' option').removeAttr('disabled');
    });

    $.each(questions, function (qindex, qdata) {
        var questionFiltered = questions.filter(function (a) { return a != qdata });
        $.each(questionFiltered, function (rindex, rdata) {
            $('#QuestionId' + qdata + ' option[value=' + $.trim($('#QuestionId' + rdata).val()) + ']').prop('disabled', true);
        });
    });

    if ($.trim($('#QuestionId1').val()) == $.trim($('#QuestionId3').val()) && $.trim($('#QuestionId1').val()) != "" && $.trim($('#QuestionId3').val()) != "") {
        $(obj).addClass("error_msg");
        $(obj).attr('title', 'This option already selected, Please select different option');
        cansubmit = false;
    }

    if ($.trim($('#QuestionId1').val()) == $.trim($('#QuestionId2').val()) && $.trim($('#QuestionId2').val()) != "" && $.trim($('#QuestionId1').val()) != "") {
        $(obj).addClass("error_msg");
        $(obj).attr('title', 'This option already selected, Please select different option');
        cansubmit = false;
    }

    if ($.trim($('#QuestionId2').val()) == $.trim($('#QuestionId3').val()) && $.trim($('#QuestionId2').val()) != "" && $.trim($('#QuestionId3').val())!="") {
        $(obj).addClass("error_msg");
        $(obj).attr('title', 'This option already selected, Please select different option');
        cansubmit = false;
    }

    if (!cansubmit) {
        error.show();
        error.append('<p> Please provide the correct data</p>');
    }
}
