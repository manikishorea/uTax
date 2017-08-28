function getAllFeeMaster() {
    var booksUri = '/api/FeeMaster/';
    $("#table_FeeMaster > tbody").remove();
    var table = $('#table_FeeMaster').append('<tbody/>'); //$(document.body)
    ajaxHelper(booksUri, 'GET').done(function (data) {
        $.each(data, function (rowIndex, r) {
            var row = $("<tr/>").appendTo(table);;
            row.append($("<td/>").text(r["Name"]));
            row.append($("<td/>").text(r["FeeType"]));
            row.append($("<td/>").text(r["FeesForName"]));
            // row.append($("<td/>").text(r["ActivatedDate"]));


            var jsonDate = r["ActivatedDate"];
            var currentTime = new Date(jsonDate);
            var month = currentTime.getMonth() + 1;
            var day = currentTime.getDate();
            var year = currentTime.getFullYear();
            var date = day + "/" + month + "/" + year;
            row.append($("<td/>").text(date));

            var JsonDeDate = r["DeActivatedDate"];
            if (JsonDeDate != null) {
                var currentDeTime = new Date(JsonDeDate);
                var Demonth = currentDeTime.getMonth() + 1;
                var Deday = currentDeTime.getDate();
                var Deyear = currentDeTime.getFullYear();
                var date1 = Deday + "/" + Demonth + "/" + Deyear;
                row.append($("<td/>").text(date1));
            }
            else {
                row.append($("<td/>").text(""));
            }
            row.append($("<td/>").text(r["StatusCode"] == "ACT" ? "Activate" : "Deactivate"));
            row.append(ActionList('/FeeMaster/create/', r["Id"], r["StatusCode"]));
        });
    });
    $(".office-table .table").trigger('footable_initialize');
}

function getFeeDetail(Id) {

    var Uri = '/api/FeeMaster/';
    var allchkbox = $('input[type=checkbox].chkEntity');
    if (Id != '' && Id != null && Id != '00000000-0000-0000-0000-000000000000') {
        ajaxHelper(Uri + Id, 'GET').done(function (data) {
            $('#Id').val(data["Id"]);
            $('#FeesName').val(data["Name"]);
            var FeeType = data["FeeTypeId"];
            if (FeeType == 1) {
                $('input[type=radio][name=FeesType]');
                $("#fixedamount").prop("checked", true);
                $("#displayshowhide").show();
            }
            else if (FeeType == 2) {
                $("#useramount").prop("checked", true);
                $("#displayshowhide").hide();
                $("#dvdisplaySalesforce").hide();
            }
            else {
                $("#SalesForce").prop("checked", true);
                $("#displayshowhide").hide();
                $("#dvdisplaySalesforce").show();
                $('#txtSalesForce').val(data["SalesforceFeesFieldID"]);
            }

            var FeesFor = data["FeesFor"];
            if (FeesFor == 1) {
                $("#Othersfees").prop("checked", true);
            }
            else if (FeesFor == 2) {
                $("#ServiceBureauFees").prop("checked", true);
            }
            else {
                $("#TransmissionFees").prop("checked", true);
            }


            $('#Amount').val(data["Amount"]);

            var NatureOfFees = data["FeeNatureId"];
            if (NatureOfFees == 1) {
                $('#mandatory').prop("checked", true);
            }

            $('#NoteForUser').val(data["NoteForUser"]);
            $('#Notes').val(data["Note"]);
            var FeeCategoryID = data["FeeCategoryID"];
            if (FeeCategoryID == 1) {
                $('#rbBankProduct').prop('checked', true);
            }
            else {
                $('#rbeFile').prop('checked', true);
            }


            if (data["IsIncludedMaxAmtCalculation"] == true) {
                //$('#chkIncludemaxbankfeeamt').prop('checked', true);
                $('#IncludemaxbankfeeamtYes').prop('checked', true);
            }
            else
            {
                //$('#chkIncludemaxbankfeeamt').prop('checked', false);
                $('#IncludemaxbankfeeamtNo').prop('checked', false);
            }


            $.each(data.Entities, function (coIndex, c) {
                $('#chk' + c["Id"]).attr('checked', 'checked');
            });
        });
    }
}

function fnSaveFeeMaster() {
    $('#error').hide();
    $('#error p').html('');
    $('#success').hide();
    $('#success p').html('');
    var req = {};
    var cansubmit = true;

    $('#FeesName').removeClass("error_msg");
    if ($.trim($('#FeesName').val()) == "") {
        $('#FeesName').addClass("error_msg");
        $('#FeesName').attr('title', 'Please enter Fees Name');
        cansubmit = false;
    }

    var FeeType = $('input[name=FeesType]:checked').val();
    var FeesFor = $('input[name=FeesFor]:checked').val();

    //debugger;
    $('#Amount').removeClass("error_msg");
    var value = $.trim($('#Amount').val());
    if (value != "") {
        var intRegex = /^[0-9]+([,.][0-9]+)?$/g;

        if (!intRegex.test(value)) {
            $('#Amount').addClass("error_msg");
            $('#Amount').attr('title', 'Please Enter Amount');
            cansubmit = false;
        }
    }
    else { $('#Amount').val(0); }
    if (FeeType == "SalesForce") {
        if ($.trim($('#txtSalesForce').val()) == "") {
            $('#txtSalesForce').addClass("error_msg");
            $('#txtSalesForce').attr('title', 'Please enter Salesforce Information');
            cansubmit = false;
        }
    }

  
    $('#div_Includemaxbankfeeamt').removeClass("error_msg");
    if (!$('#IncludemaxbankfeeamtYes').is(':checked') && !$('#IncludemaxbankfeeamtNo').is(':checked')) {

        $('#div_Includemaxbankfeeamt').addClass("error_msg");
        $('#div_Includemaxbankfeeamt').attr('title', 'Please check at least one radio');
        cansubmit = false;
    }
   // var Includemaxbankfeeamt = $('#IncludemaxbankfeeamtYes').is(':checked');;

    var NatureOfFees = $('input[name=NatureOfFees]:checked').val();

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

    var FEECategroy = 1;
    if ($('#rbBankProduct').is(':checked'))
    { FEECategroy = 1; }
    else { FEECategroy = 2; }

    $('#NoteForUser').removeClass("error_msg");
    if ($.trim($('#NoteForUser').val()) == "") {
        $('#NoteForUser').addClass("error_msg");
        $('#NoteForUser').attr('title', 'Please enter Note For User');
        cansubmit = false;
    }
    $('#Notes').removeClass("error_msg");
    if ($.trim($('#Notes').val()) == "") {
        $('#Notes').addClass("error_msg");
        $('#Notes').attr('title', 'Please enter Notes');
        cansubmit = false;
    }
    if (cansubmit) {

        req.Id = $('#Id').val();
        req.Name = $('#FeesName').val();
        req.FeeType = FeeType;
        req.FeesFor = FeesFor;
        //req.FeeTypeId = $('# ').val();
        req.Amount = $('#Amount').val();
        req.FeeNature = NatureOfFees;
        // req.FeeNatureId = $('# ').val();
        req.NoteForUser = $('#NoteForUser').val();
        req.Note = $('#Notes').val();

        req.FeeCategoryID = FEECategroy;
        req.IsIncludedMaxAmtCalculation = $('#IncludemaxbankfeeamtYes').is(':checked');
        req.SalesforceFeesFieldID = $('#txtSalesForce').val();

        req.Entities = Entities;

        var Uri = '/api/FeeMaster/';
        ajaxHelper(Uri, 'POST', req).done(function (data, textStatus) {
            if (textStatus == 'notmodified') {
                $('#error p').html('Fees Master is already exist.');
                $('#error').show();
            } else if (textStatus == 'notccceptable') {
                $('#error p').html('Fees Master has not saved.');
                $('#error').show();
            } else {
                $('#Id').val(data)
                $('#success').show();

                if (req.Id == data) {
                    $('#success p').html('Fees Master updated successfully.');
                } else {
                    $('#success p').html('Fees Master created successfully.');
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
    var conformResult = confirm("Are you sure you wish to  " + statuscode + "  the specific Fees Master \n and no longer have it available for the users to select ?");
    if (conformResult == true) {

        // alert(Id +","+ Status)
        var req = {};
        var cansubmit = true;

        if (cansubmit) {

            req.Id = Id;
            req.BankName = $('#BankName').val();
            req.StatusCode = Status;
            var reqUri = '/api/FeeMaster/';
            ajaxHelper(reqUri + Id, 'PUT', req).done(function (data, textStatus) {
                if (textStatus == 'notmodified') {
                }
                else {
                    $('#success').show();
                    $('#success p').html('This Bank ' + statuscode + 'd successfully.');
                    getAllFeeMaster();
                }
            });
        }
    }
    else {
        return false;
    }
}