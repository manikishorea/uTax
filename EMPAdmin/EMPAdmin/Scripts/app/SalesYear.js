function getAllSalesYear() {

    var booksUri = '/api/SalesYear/';
    $("#table_SalesYear > tbody").remove();
    var table = $('#table_SalesYear').append('<tbody/>'); //$(document.body)
    ajaxHelper(booksUri, 'GET').done(function (data) {
        $.each(data, function (rowIndex, r) {
            var row = $("<tr/>").appendTo(table);
            row.append($("<td/>").text(r["SalesYear"]));
            var jsonDate = r["ApplicableFromDate"];
            var currentTime = new Date(jsonDate);
            var month = currentTime.getMonth() + 1;
            var day = currentTime.getDate();
            var year = currentTime.getFullYear();
            var date = day + "/" + month + "/" + year;
            row.append($("<td/>").text(date));
            //row.append($("<td/>").text(r["ApplicableFromDate"]));
            // row.append($("<td/>").text(r["ApplicableToDate"]));
            var TojsonDate = r["ApplicableToDate"];
            if (TojsonDate != null) {
                var TocurrentTime = new Date(jsonDate);
                var Tomonth = TocurrentTime.getMonth() + 1;
                var Today = TocurrentTime.getDate();
                var Toyear = TocurrentTime.getFullYear();
                var Todate = Today + "/" + Tomonth + "/" + Toyear;
                row.append($("<td/>").text(Todate));
            }
            else { row.append($("<td/>").text("")); }

            if (r["DateType"] == true) {
                row.append(EditActionList('/SalesYear/create/', r["Id"]));
            }
            else {
                row.append(SalesYearActionList('/SalesYear/create/', r["Id"]));
            }
        });
    });
    GridFunctionality();
}

function getAllBank() {
    var booksUri = '/api/BankMaster/';
    $("#tablebank > tbody").remove();
    var table = $('#tablebank').append('<tbody/>'); //$(document.body)
    ajaxHelper(booksUri, 'GET').done(function (data) {
        $.each(data, function (rowIndex, r) {
            var row = $("<tr/>").appendTo(table);;
            row.append($("<td/>").text(r["BankName"]));
            row.append($("<td/>").html("<input type='text' class='cutdt' id='" + r["Id"] + "'/>"));
            // row.append($("<td/>").text(r["StatusCode"] == "ACT" ? "Activate" : "Deactivate"));
            // row.append(BankActionList('/bank/create/', r["Id"], '/banksubquestions/index/', r["Id"], r["StatusCode"]));
        });
    });
    $(".office-table .table").trigger('footable_initialize');
}

function getSalesYear(Id) {

    var Uri = '/api/SalesYear/';
    if (Id != '' && Id != null && Id != '00000000-0000-0000-0000-000000000000') {
        ajaxHelper(Uri + Id, 'GET').done(function (data) {

            $('#Id').val(data["Id"])
            $('#SalesYear').val(data["SalesYear"])

            var jsonDate = data["ApplicableFromDate"];
            var currentTime = new Date(jsonDate);
            var month = currentTime.getMonth() + 1;
            var day = currentTime.getDate();
            var year = currentTime.getFullYear();
            var date = month + "/" + day + "/" + year;
            $('#ApplicableFromDate').val(date);
            //$('#hidApplicableFromDate').val(date);
            $('#hidApplicableFromDate').val(data["ApplicableFromDate"])
            // $('#ApplicableFromDate').val(data["ApplicableFromDate"])
            $('#Description').val(data["Description"])
            var datatype = data["DateType"]
            if (datatype == true) {
                document.getElementById("Scheduled").checked = true;
                // $('input[name=ApplicableFrom]:checked', '#Scheduled') 
                $("#ApplicableFromDate").show();
                $("#btn_SalesYear").show();
            }
            else {
                $("#ApplicableFromDate").hide();
                $("#ApplicableFromDate").val("");
                $("#btn_SalesYear").hide();
            }
            var Bankdata = data["BankInfoList"];
            if (Bankdata.length > 0) {
                $.each(Bankdata, function (colIndex, c) {
                    var cdate = Getdatefrmt(c["CutOfDate"]);
                    var id_Bank = c["BankID"];
                    $('#' + id_Bank).val(cdate)
                });
            }
            //getEntityList();
        });
    }
}
function Getdatefrmt(dtfrmt) {
    dtfrmt = new Date(dtfrmt);
    var day = dtfrmt.getDate();
    var mon = dtfrmt.getMonth() + 1;
    var yer = dtfrmt.getFullYear();
    return mon + '/' + day + '/' + yer;
}
function fnSaveSalesYear() {
    $('div.crosspop').hide();



    $('#success').hide();
    $('#success p').html('');
    var req = {};
    var cansubmit = true;

    $('#SalesYear').removeClass("error_msg");
    var intYear = /^[12][0-9]{3}$/;
    if ($.trim($('#SalesYear').val()) == "") {
        $('#SalesYear').addClass("error_msg");
        $('#SalesYear').attr('title', 'Please enter Sales Year');
        cansubmit = false;
    }
    var ApplicableFromDate = $.trim($('#ApplicableFromDate').val());
    var DateType = 0;
    if (ApplicableFromDate == '') {
        ApplicableFromDate = new Date;
        DateType = 0;
    }
    else {
        ApplicableFromDate = $.trim($('#ApplicableFromDate').val());
        DateType = 1;
    }

    $('input[type=text].cutdt').removeClass("error_msg");
    $('input[type=text].cutdt').removeAttr('title');

    var cutdtText = $('input[type=text].cutdt');

    $.each(cutdtText, function (indx, valu) {
        var Id = $(valu).attr('id');
        $('#' + Id).removeClass("error_msg");
        $('#' + Id).removeAttr("title");
        if ($('#' + Id).val() == "") {
            $('#' + Id).addClass("error_msg");
            $('#' + Id).attr('title', 'Please enter Notes');
            cansubmit = false;
        }
    });




    $('#Description').removeClass("error_msg");
    if ($.trim($('#Description').val()) == "") {
        $('#Description').addClass("error_msg");
        $('#Description').attr('title', 'Please enter Notes');
        cansubmit = false;
    }
    if (cansubmit) {
        req.Id = $('#Id').val();
        req.SalesYear = $('#SalesYear').val();
        req.ApplicableFromDate = ApplicableFromDate;
        req.Description = $('#Description').val();
        req.DateType = DateType;

        var BankInfo_List = [];

        $('.cutdt').each(function (indx, val) {
            var bankid = $(val).attr("id");
            BankInfo_List.push({
                BankID: bankid,
                CutOfDate: $(val).val()
            });
        });
        req.BankInfoList = BankInfo_List;

        var IsSuccess = false;
        var conformResult = confirm("Are you sure you wish to create a new Sales Year?");
        if (conformResult) {

          

            var Uri = '/api/SalesYear/';
            ajaxHelper(Uri, 'POST', req).done(function (data, status) {
                if (status == 'success') {

                    $('#btn_SalesYear')
                        .attr('disabled', 'disabled')
                        .css('pointer-events', 'none');

                    $('#Id').val(data)
                    $('#success').show();
                    if (req.Id == data) {
                        $('#success p').html('Sales Year Master updated successfully.');
                        IsSuccess = true;
                    } else {
                        $('#success p').html('Sales Year created successfully.');
                        IsSuccess = true;
                    }
                }
            });

            if (IsSuccess) {
                $('#popupProgress').modal('toggle');
                GetArchiveDataCount();
            }
        }
    }
}

function GetArchiveDataCount() {
    var Uri = '/api/ArchiveProcess?status=init&tokenid=' + $("#Token").val();
    ajaxHelperasync(Uri, 'GET').done(function (data, status) {
        if (status == 'success') {
            var message = "Total records need to process : " + data;
            $('#totalRecords').val(data);
            $('#lblStatusMessage').text(message);
            $('#chkInit').prop('checked', 'checked').prop('disabled', 'disabled');

            myInterval = setInterval(function () { GetArchiveDataStatus() }, 1500);
            SetArchiveDataInformation();
        }
    });
}

function SetArchiveDataInformation() {
    var Uri = '/api/ArchiveProcess?tokenid=' + $("#Token").val() + '&salesyearid=' + $('#Id').val();
    ajaxHelperasync(Uri, 'POST').done(function (data) {
        // $('#lblStatusMessage').text(data);
        if (data == "success") {
            $('#chkActivate').prop('checked', 'checked').prop('disabled', 'disabled');
            clearInterval(myInterval);
        }
        // $('#popupProgress').modal("hide");
    });
}

function GetArchiveDataStatus() {
    var Uri = '/api/ArchiveProcess?status=start&tokenid=' + $("#Token").val();
    ajaxHelperasync(Uri, 'GET').done(function (data, status) {

        if (status == 'success') {
            var totalRecord = $('#totalRecords').val();
            var message = "Total records processed : " + data + " of " + totalRecord;

            if (Number(totalRecord) == Number(data)) {
                $('#chkActivate').prop('checked', 'checked').prop('disabled', 'disabled');
                clearInterval(myInterval);
                $('div.crosspop').show();
            }

            $('#lblStatusMessage').text(message);
            $('#chkProcess').prop('checked', 'checked').prop('disabled', 'disabled');
        }
    });
}

function getEntityList() {
    var booksUri = '/api/CustomerEntity/';
    $("#table_EntitiesSummary > tbody").remove();
    var table = $('#table_EntitiesSummary').append('<tbody/>'); //$(document.body)
    ajaxHelper(booksUri, 'GET').done(function (data) {
        $.each(data, function (rowIndex, r) {
            // debugger;
            var row = $("<tr/>").appendTo(table);

            row.append($("<td/>").text(r["Name"]));
            row.append($("<td/>").text(r["EntityCount"]));
            row.append($("<td/>").html('<input type="checkbox"  onclick="return false;" onkeydown="return false;"/>'));

        });
    });
}

function FnClearInter() {
    clearInterval(myInterval);
    $('#popupProgress').modal("hide");
}

function fnClose() {
    $('#popupProgress').modal("hide");
}

//function fnSaveStatus(Id, Status) {
//    var statuscode = '';
//    if (Status == "INA") {
//        statuscode = "Deactivate";
//    }
//    else {

//        statuscode = "Activate"
//    }
//    var conformResult = confirm("Are you sure you wish to  " + statuscode + "  the specific SalesYear \n and no longer have it available for the users to select ?");
//    if (conformResult == true) {

//        // alert(Id +","+ Status)
//        var req = {};
//        var cansubmit = true;

//        if (cansubmit) {

//            req.Id = Id;
//            req.SalesYear = $('#SalesYear').val();
//            req.StatusCode = Status;

//            var reqUri = '/api/SalesYear/';
//            ajaxHelper(reqUri + Id, 'PUT', req).done(function (data) {
//                // window.location.href = "/bank/";
//                getAllSalesYear();
//            });
//        }
//    }
//    else {
//        return false;
//    }
//}