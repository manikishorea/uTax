function getCustomerSearch(pagenumber,maxcount) {
    var url = '/api/CustomerSearch';
   
   $("#tbAllCustomerInfo > tbody").remove();
    var table = $('#tbAllCustomerInfo').append('<tbody/>'); //$(document.body)

    var s_Entity = $('#entityid').val();
    ajaxHelper(url + '?pageno=' + pagenumber + '&maxcount=' + maxcount, 'GET').done(function (data) {

       // alert(data.totalrecords);
        var totalrecords = data.totalrecords;
        var maxcount = $('#maxcount').val();
        $('#divtest').html('');
        var pages = Number(totalrecords) / Number(maxcount);
        var div = $('#divtest');
        for (var i = 0; i <= pages; i++)
        {
            var n= Number(i)+1;
            div.append('<a onclick="fnPagesSearch(' + n + ')" href="#">' + n + '</a> &nbsp;&nbsp;&nbsp;&nbsp;')
        }

        $.each(data.CustomerModel, function (rowIndex, r) {
            var row;
            if (r["BaseEntityId"] == $('#Entity_MO').val() || r["BaseEntityId"] == $('#Entity_SVB').val()) {
                row = $("<tr/>").addClass('success').attr('role', 'row');
            }
            else {
                row = $("<tr/>").addClass('odd').attr('role', 'row');
            }
            row.appendTo(table);
            row.append($("<td/>").text(r["ActivationStatus"]));

            var childinfo = r["ChaildCustomerInfoCount"];
            if (Number(childinfo) > 0) {
                row.append($("<td/>").addClass("CX").html('<i class="fa fa-minus-square"></i>'));
            }
            else {
                row.append($("<td/>"));
            }

            if ($('#entityid').val() != $('#Entity_MO').val() && $('#entityid').val() != $('#Entity_SVB').val()) {
                if (r["EntityId"] == $('#Entity_MO').val() || r["EntityId"] == $('#Entity_SVB').val()) {
                    if (_canmanageOfc)
                        row.append($("<td/>").html('<div class="hover"><span class="hover-blk"><a onclick="fnGetInfo(' + r["Id"] + ')"  href="/Configuration/Dashboard?Id=' + r["Id"] + '&entitydisplayid=' + r["BaseEntityId"] + '" target="_blank">' + r["CompanyName"] + '</a></span><div class="tooltip"><span>User Type:</span> ' + r["EROType"] + '</div></div>'));
                    else
                        row.append($("<td/>").html('<div class="hover" ><span class="hover-blk">' + r["CompanyName"] + '</span><div class="tooltip"><span>User Type:</span> ' + r["EROType"] + '</div></div>'));
                }
                else {
                    row.append($("<td/>").html('<div class="hover" ><span class="hover-blk">' + r["CompanyName"] + '</span><div class="tooltip"><span>User Type:</span> ' + r["EROType"] + '</div></div>'));
                }
            }
            else {
                row.append($("<td/>").html('<div class="hover" ><span class="hover-blk">' + r["CompanyName"] + '</span><div class="tooltip"><span>User Type:</span> ' + r["EROType"] + '</div></div>'));
            }
            row.append($("<td/>").text(r["SalesforceParentID"]));
            row.append($("<td/>").text(r["MasterIdentifier"]));
            row.append($("<td/>").text(r["CrossLinkUserId"]));
            row.append($("<td/>").text(r["AlternativeContact"]));
            row.append($("<td/>").text(r["EFIN"]));
            row.append($("<td/>").text(r.ActiveBank ? r.ActiveBank : ''));
            row.append($("<td/>").text(r.SubmissionDate ? r.SubmissionDate : ''));

            var _enrstatus = r.EnrollmentStatus ? r.EnrollmentStatus : "Not Started";
            var _rejectedbanks = r.RejectedBanks ? '<span>Rejected Banks</span><span>' + r.RejectedBanks + '</span>' : '';
            var _approvedbank = r.ApprovedBank ? '<span>Approved Bank:</span> <span>' + r.ApprovedBank + '</span>' : '';
            var statushtml = '';
            if (_rejectedbanks || _approvedbank)
                statushtml = '<div class="hover"><span class="hover-blk">' + _enrstatus + '</span><div class="tooltip">' + _approvedbank + ' ' + _rejectedbanks + '</div></div>';
            else
                statushtml = _enrstatus;

            row.append($("<td/>").html(statushtml));

            if (!r.IsActivated) {
                row.append($("<td/>").html(''));
            }
            else {
                if (_manageEnroll && ($('#entityid').val() != $('#Entity_MO').val() && $('#entityid').val() != $('#Entity_SVB').val()))
                    row.append($("<td/>").html('<div class="hover"><span class="hover-blk"><a href="/Enrollment/OfficeInformation?Id=' + r.Id + '&entitydisplayid=' + r["BaseEntityId"] + '&entityid=' + r["EntityId"] + '" target="_blank"><i class="fa fa-bars"></i></a></span><div class="tooltip"><span>Status:</span> ' + _enrstatus + '</div></div>'));
                else
                    row.append($("<td/>").html(''));
            }

            row.append($("<td/>").html(''));
            row.append($("<td/>").html(''));

            row.append($("<td/>").text(r["AccountStatus"]));
            var EntityDisplayId = $('#entitydisplayid').val();

            var IsActivationCompleted = r["IsActivationCompleted"];
            if (EntityDisplayId == $('#Entity_uTax').val()) {
                if (r["EntityId"] == $('#Entity_MO').val() || r["EntityId"] == $('#Entity_SVB').val()) {
                    if (_canresetpwd && _addsuboffice) {
                        if (IsActivationCompleted == '1' || IsActivationCompleted == 1) {
                            row.append($("<td/>").addClass("footable-last-column-width").html('<div class="btn-group dropleft"><button class="btn dropdown-toggle" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">Actions <span class="caret"></span></button><ul class="dropdown-menu dropdown-menu-right" role="menu" aria-labelledby="dropdownMenu"><li><a href="#" onClick="Resetpassword(\'' + r["Id"] + '\')">Reset Password</a></li><li><a href="/SubSiteOfficeConfiguration/CreateSubSiteInfo?ParentId=' + r["Id"] + '" target="_blank">Add Sub Office</a></li></ul></div>'));
                        } else {
                            row.append($("<td/>").addClass("footable-last-column-width").html('<div class="btn-group dropleft"><button class="btn dropdown-toggle" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">Actions <span class="caret"></span></button><ul class="dropdown-menu dropdown-menu-right" role="menu" aria-labelledby="dropdownMenu"><li><a href="#" onClick="Resetpassword(\'' + r["Id"] + '\')">Reset Password</a></li></ul></div>'));
                        }
                    }
                    else if (_canresetpwd && !_addsuboffice) {
                        row.append($("<td/>").addClass("footable-last-column-width").html('<div class="btn-group dropleft"><button class="btn dropdown-toggle" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">Actions <span class="caret"></span></button><ul class="dropdown-menu dropdown-menu-right" role="menu" aria-labelledby="dropdownMenu"><li><a href="#" onClick="Resetpassword(\'' + r["Id"] + '\')">Reset Password</a></li></ul></div>'));
                    }
                    else if (!_canresetpwd && _addsuboffice) {
                        if (IsActivationCompleted == '1' || IsActivationCompleted == 1) {
                            row.append($("<td/>").addClass("footable-last-column-width").html('<div class="btn-group dropleft"><button class="btn dropdown-toggle" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">Actions <span class="caret"></span></button><ul class="dropdown-menu dropdown-menu-right" role="menu" aria-labelledby="dropdownMenu"><li><a href="/SubSiteOfficeConfiguration/CreateSubSiteInfo?ParentId=' + r["Id"] + '" target="_blank">Add Sub Office</a></li></ul></div>'));
                        }
                    }
                }
                else {
                    if (_canresetpwd)
                        row.append($("<td/>").addClass("footable-last-column-width").html('<div class="btn-group dropleft"><button class="btn dropdown-toggle" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">Actions <span class="caret"></span></button><ul class="dropdown-menu dropdown-menu-right" role="menu" aria-labelledby="dropdownMenu"><li><a href="#" onClick="Resetpassword(\'' + r["Id"] + '\')">Reset Password</a></li></ul></div>'));
                }
            }
            else if (EntityDisplayId == $('#Entity_MO').val() || EntityDisplayId == $('#Entity_SVB').val()) {

                if (IsActivationCompleted == '1' || IsActivationCompleted == 1) {
                    row.append($("<td/>").addClass("footable-last-column-width").html('<div class="btn-group dropleft"><button class="btn dropdown-toggle" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">Actions <span class="caret"></span></button><ul class="dropdown-menu dropdown-menu-right" role="menu" aria-labelledby="dropdownMenu"><li><a href="/SubSiteOfficeConfiguration/CreateSubSiteInfo?ParentId=' + r["Id"] + '" target="_blank">Add Sub Office</a></li></ul></div>'));
                }
            } else {
                row.append($("<td/>").html('&nbsp;'));

            }

        });

    });
}
