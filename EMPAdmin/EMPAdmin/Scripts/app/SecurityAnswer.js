function getQuestionMaster(container) {
    
    var custmorUri = '/api/SecurityAnswer';
    ajaxHelper(custmorUri, 'GET').done(function (data) {
        container.append($('<option />', { value: '', text: 'Select' }));
        $.each(data, function (rowIndex, r) {
            container.append($('<option />', { value: r["QuestionId"], text: r["Question"] }));
           //$("<td/>").html("<a href='#' onclick='UserEdit(" + r['Id'] + ")'> Detail </a>"));
        });
    });

   
}

//}
//function getPhoneTypeDetail(Id) {

//    var Uri = '/api/PhoneTypeMaster/';

//    if (Id != '' && Id != null && Id!='00000000-0000-0000-0000-000000000000') {
//        ajaxHelper(Uri + Id, 'GET').done(function (data) {
//            $('#Id').val(data["Id"])
//            $('#PhoneType').val(data["PhoneType"])
//            $('#Description').val(data["Description"])
//            //debugger;
//            $('#Phonetype_popup').html('Edit Phone Type');
//        });
//    }
//}

function fnSaveSecurityAnswer() {
    //debugger;
    $('#popup_success').hide();
    $('#popup_success p').html('');
    var req = {};
    var cansubmit = true;

     var Question = {};
     $('#QuestionId1').removeClass("error_msg");
    
    if ($.trim($('#QuestionId :selected').val()) == "") {
        $('#QuestionId').addClass("error_msg");
        $('#QuestionId').attr('title', 'Please select Question');
        cansubmit = false;
    }
    else {
        Question.Id = $('#QuestionId').val();
    }



    $('#Answer').removeClass("error_msg");
    if ($.trim($('#Answer').val()) == "") {
        $('#Answer').addClass("error_msg");
        $('#Answer').attr('title', 'Please enter Answer');
        cansubmit = false;
    }
  
    if (cansubmit) {
       
        req.Id = $('#Id').val();
        req.QuestionId = $('#QuestionId').val();
        req.Answer = $('#Answer').val();

        var Uri = '/api/SecurityAnswer';
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
function fnSaveStatus(Id, Status) {
    var statuscode = '';
    if (Status == "INA") {
        statuscode = "Deactivate";
    }
    else {

        statuscode = "Activate"
    }

    var conformResult = confirm("Are you sure you wish to " + statuscode + "  the specific Phone Type \n and no longer have it available for the users to select ?");
    if (conformResult == true) {


        // alert(Id +","+ Status)
        var req = {};
        var cansubmit = true;

        if (cansubmit) {

            req.Id = Id;
            req.PhoneType = $('#PhoneType').val();
            req.StatusCode = Status;

            var reqUri = '/api/PhoneTypeMaster/';
            ajaxHelper(reqUri + Id, 'PUT', req).done(function (data) {
                getAllPhoneType();
            });
        }
    }
    else {
        return false;
    }
}

function fnCancle() {
    $('input[type=text]')
        .removeClass('error_msg')
        .val('');
    $('#Description').val('');
    $('#popup_success').hide();
    $('#popup_success p').html('');
}