function getAllTooltip() {
    var booksUri = '/api/TooltipMaster/';
    $("#table_tooltip > tbody").remove();
    var table = $('#table_tooltip').append('<tbody/>');
    ajaxHelper(booksUri, 'GET').done(function (data) {
        $.each(data, function (rowIndex, r) {
            var row = $("<tr/>").appendTo(table);;
            row.append($("<td/>").text(r["SitemapTitle"]));
            row.append($("<td/>").text(r["Field"]));
            row.append($("<td/>").text(r["ToolTipText"]));
            row.append($("<td/>").text(r["IsUIVisible"]? "Visible" : "Hidden"));
            row.append(EditActionList('/Tooltip/create/', r["Id"]));
        });
    });
    $(".office-table .table").trigger('footable_initialize');
}


function getTooltipDetail(Id) {

    var Uri = '/api/TooltipMaster/';

    if (Id != '' && Id != null && Id != '00000000-0000-0000-0000-000000000000') {
        ajaxHelper(Uri + Id, 'GET').done(function (data) {
            $('#Id').val(data["Id"])
            $('#lblField').html(data["Field"])
            $('#Field').val(data["Field"])
            $('#ToolTipText').val(data["ToolTipText"])
            $('#lblSitemapTitle').html(data["SitemapTitle"])
            $('#Description').val(data["Description"])

            if (data["IsUIVisible"] == 'true' || data["IsUIVisible"] == true || data["IsUIVisible"] == 'True') {
                $('#IsUIVisible').attr('checked', 'checked');
            }
            $('#SitemapId').val(data["SitemapId"])

        });

    }
}

function fnSaveTooltip() {

    $('#success').hide();
    $('#success p').html('');

    var req = {};
    var cansubmit = true;

    $('#ToolTipText').removeClass("error_msg");
    if ($.trim($('#ToolTipText').val()) == "") {
        $('#ToolTipText').addClass("error_msg");
        $('#ToolTipText').attr('title', 'Please enter Tooltip Text');
        cansubmit = false;
    }

    if (cansubmit) {

        req.Id = $('#Id').val();
        req.Field = $('#Field').val();
        req.ToolTipText = $('#ToolTipText').val();
        req.Description = $('#Description').val();
        req.SitemapId = $('#SitemapId').val();
        req.UserId = $('#UserId').val();
        req.IsUIVisible = $("#IsUIVisible").is(':checked');

        var Uri = '/api/TooltipMaster/';
        ajaxHelper(Uri, 'POST', req).done(function (data) {

            $('#Id').val(data["Id"])
            $('#success').show();

            if (req.Id == data["Id"]) {
                $('#success p').html('Tooltip updated successfully.');
            } else {
                $('#success p').html('Tooltip created successfully.');
            }
        });
    }
}


function fnSaveStatus(Id, Status) {
    var conformResult = confirm("Are you sure you wish to Deactivate the specific ToolTip \n and no longer have it available for the users to select ?");
    if (conformResult == true) {

        // alert(Id +","+ Status)
        var req = {};
        var cansubmit = true;

        if (cansubmit) {

            req.Id = Id;
            req.Field = $('#Field').val();
            req.StatusCode = Status;

            var reqUri = '/api/TooltipMaster/';
            ajaxHelper(reqUri + Id, 'PUT', req).done(function (data) {
                //window.location.href = "/tooltip/";
                getAllTooltip();
            });
        }
    }
    else {
        return false;
    }
}