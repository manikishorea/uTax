function getAllAffiliateProgram() {
    var booksUri = '/api/affiliateprogrammaster/';
    $("#table_affiliate > tbody").remove();
    var table = $('#table_affiliate').append('<tbody/>'); //$(document.body)
    ajaxHelper(booksUri, 'GET').done(function (data) {
        $.each(data, function (rowIndex, r) {
            var row = $("<tr/>").appendTo(table);;

            row.append($("<td/>").text(r["Name"]));
            row.append($("<td/>").text(r["Cost"]));
            row.append($("<td/>").text(r["StatusCode"] == "ACT" ? "Activate" : "Deactivate"));
            var jsonDate = r["ActivationDate"];
            var currentTime = new Date(jsonDate);
            var month = currentTime.getMonth() + 1;
            var day = currentTime.getDate();
            var year = currentTime.getFullYear();
            var date = day + "/" + month + "/" + year;

            row.append(ActionList('/affiliateprogram/create/', r["Id"], r["StatusCode"]));
        });
    });
    $(".office-table .table").trigger('footable_initialize');
   // GridFunctionality();
}



function getAffiliateProgramDetail(Id) {
    $('#tcpathabcd').hide();
    var Uri = '/api/affiliateprogrammaster/';
    var allchkbox = $('input[type=checkbox].chkEntity');

    if (Id != '' && Id != null && Id != '00000000-0000-0000-0000-000000000000') {
        ajaxHelper(Uri + Id, 'GET').done(function (data) {
            $('#Name').val(data["Name"])
            $('#Cost').val(data["Cost"])
            $('#h_docpath').val(data["DocumentPath"]);
            $.each(data.Entities, function (coIndex, c) {
                $('#chk' + c["Id"]).attr('checked', 'checked');
            });

            if (data["DocumentPath"] != 'NoFile') {
                getDocument(data["DocumentPath"], $('#tcpathabcd'));
            }
        });
    }
}

function fnSaveAffiliateProgram() {
    var cansubmit = true;
    if ($('#fileToUpload').val() != '') {
        if (!$('#fileToUpload').val().match(/(?:pdf|PDF)$/)) {
            $('#fileToUpload').parent('div').find('.bootstrap-filestyle .form-control').addClass("error_msg");
            $('#fileToUpload').parent('div').find('.bootstrap-filestyle .form-control').attr('title', 'Please upload file having extension (.pdf)');
            cansubmit = false;
        }
        else {
            $('#fileToUpload').parent('div').find('.bootstrap-filestyle .form-control').removeClass("error_msg");
        }
    }

    if (cansubmit) {
        var data = new FormData()
        var files = $("#fileToUpload").get(0).files;
        if (files.length > 0) {
            data.append("UploadedImage", files[0]);
        }

        data.append("UserID", $('#UserId').val());
        data.append("Id", $('#h_docpath').val());
        var url = '/api/Document/SaveAjaxDocuments';
        ajaxUploadHelper(url, 'POST', data).done(function (result) {
            $('#h_docpath').val(result);
            SaveApiData();
        });
    }
    else {
        if (cansubmit)
        { SaveApiData(); }
    }
}

function SaveApiData() {
    $('#error').hide();
    $('#error p').html('');
    $('#success').hide();
    $('#success p').html('');
    var req = {};
    var cansubmit = true;

    $('#Name').removeClass("error_msg");
    if ($.trim($('#Name').val()) == "") {
        $('#Name').addClass("error_msg");
        $('#Name').attr('title', 'Please enter Name');
        cansubmit = false;
    }

    $('#Cost').removeClass("error_msg");
    var intRegex = /^\d+$/;
    var Cost = $('#Cost').val();
    if (!intRegex.test(Cost)) {
        $('#Cost').addClass("error_msg");
        $('#Cost').attr('title', 'Please enter Cost');
        cansubmit = false;
    }


    var Entities = [];

    var IsChecked = false;
    var allchkbox = $('input[type=checkbox].chkEntity');
    $.each(allchkbox, function (indx, valu) {
        var chkId = $(valu).attr('id');
        var valE = $('#' + chkId).val()
        if ($(valu).is(':checked')) {
            IsChecked = true;
            Entities.push({
                Id: valE
            });
        }
    });

    $('#div_Entity').removeClass("error_msg");
    if (!IsChecked) {
        $('#div_Entity').addClass("error_msg");
        $('#div_Entity').attr('title', 'Please select at least one entity');
        cansubmit = false;
    }
    if (cansubmit) {
        req.Id = $('#Id').val();
        req.Name = $('#Name').val();
        req.Cost = $('#Cost').val();
        req.DocumentPath = $('#h_docpath').val();//$('#DocumentPath').val();
        req.UserId = $('#UserId').val();
        req.Entities = Entities;
        var Uri = '/api/affiliateprogrammaster/';
        ajaxHelper(Uri, 'POST', req).done(function (data, textStatus) {
            if (textStatus == 'notmodified') {
                $('#error p').html('Affiliate Program is already exist.');
                $('#error').show();
            } else if (textStatus == 'notccceptable') {
                $('#error p').html('Affiliate Program has not saved.');
                $('#error').show();
            } else {
                $('#Id').val(data)
                $('#success').show();

                if (req.Id == data) {
                    $('#success p').html('Affiliate Program updated successfully.');
                } else {
                    $('#success p').html('Affiliate Program created successfully.');
                }
                //getAffiliateProgramDetail(data);

                if (req.DocumentPath != 'NoFile') {
                    getDocument(req.DocumentPath, $('#tcpathabcd'));
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

    var conformResult = confirm("Are you sure you wish to " + statuscode + " the specific Affiliate Program and no longer have it available for the users to select ?");
    if (conformResult == true) {

        // alert(Id +","+ Status)
        var req = {};
        var cansubmit = true;
        if (cansubmit) {
            req.Id = Id;
            req.Field = $('#Field').val();
            req.StatusCode = Status;

            var reqUri = '/api/affiliateprogrammaster/';
            ajaxHelper(reqUri + Id, 'PUT', req).done(function (data, textStatus) {
                if (textStatus == 'notmodified') {
                }
                else {
                    $('#success').show();
                    $('#success p').html('This Bank ' + statuscode + 'd successfully.');
                    getAllAffiliateProgram();
                }
            });
        }
    }
    else {
        return false;
    }
}