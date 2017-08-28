function getAllCustomerInformation(i) {
    var msg = localStorage.getItem("CrossMsg");
    localStorage.removeItem("CrossMsg");

    if (msg != '' && msg != null && msg != undefined) {
        var success = $('#success');
        success.html('');
        success.hide();
        success.show();
        success.append('<p> ' + msg + ' </p>');
    }
    //$('#tbAllCustomerInfo').html('');
    var Status = $.trim($('#StatusID').val());
    var SiteType = $.trim($('#SiteTypeID').val());
    var BankPartner = $.trim($('#BankPartnerID').val());
    var Enrollment = $.trim($('#EnrollmentID').val());
    var OnBoard = $.trim($('#OnBoardStatusID').val());
    var SearchType = 0;
    if (i == 0) {
        if ($('#IsCustomerName').is(':checked')) { SearchType = 1; }
        else if ($('#IsUseID').is(':checked')) { SearchType = 2; }
        else if ($('#IsEFIN').is(':checked')) { SearchType = 3; }
        else if ($('#IsContactName').is(':checked')) { SearchType = 4; }
        else if ($('#IsTelephone').is(':checked')) { SearchType = 5; }
        else if ($('#IsMasterID').is(':checked')) { SearchType = 6; }

        if ($('#txtSearch').val().trim() == '') {
            SearchType = 0;
            //alert('Please Select any option.');
        }
        Status = '';
        SiteType = '';
        BankPartner = '';
        Enrollment = '';
        OnBoard = '';
    }
    var url = '/api/CustomerInformation/CustomerSearchInfo?UserName=' + $('#UserId').val() + '&Status=' + Status + '&SiteType=' + SiteType
        + '&BankPartner=' + BankPartner + '&EnrollmentStatus=' + Enrollment + '&OnBoardingStatus=' + OnBoard + '&SearchText=' + $('#txtSearch').val() + '&SearchType=' + SearchType;

    //$("#tbAllCustomerInfo > tbody").remove();
    //var table = $('#tbAllCustomerInfo').append('<tbody/>'); //$(document.body)
    $('#tbCustomersBody').empty();
    var s_Entity = $('#entityid').val();
    var tableContent = '';
    ajaxHelper(url, 'GET').done(function (data) {
        $.each(data, function (rowIndex, r) {
           
            var row;
            if (r["DisplayId"] == 4 || r["DisplayId"] == 6) {
                tableContent += '<tr class="success" role="row">';
                //row = $("<tr/>").addClass('success').attr('role', 'row');
            }
            else {
                tableContent += '<tr class="odd" role="row">';
                //row = $("<tr/>").addClass('odd').attr('role', 'row');
            }
            //row.appendTo(tableContent);
            //row.append($("<td/>").text(r["ActivationStatus"]));
            tableContent += '<td>' + r.ActivationStatus + '</td>';

            if (r.ChaildCustomerInfo.length > 0) {
                tableContent += '<td class="CX"><i class="fa fa-plus-square"></i></td>'
                //row.append($("<td/>").addClass("CX").html('<i class="fa fa-minus-square"></i>'));
            }
            else {
                //row.append($("<td/>"));
                tableContent += '<td></td>'
            }

            var company = '';
            if ($('#entitydisplayid').val() != 4 && $('#entitydisplayid').val() != 6) {
                if (r["DisplayId"] == 4 || r["DisplayId"] == 6) {
                    if (_canmanageOfc)
                        company = '<div class="hover"><span class="hover-blk"><a onclick="fnGetInfo(' + r["Id"] + ')"  href="/Configuration/Dashboard?Id=' + r["Id"] + '&entitydisplayid=' + r["DisplayId"] + '" target="_blank">' + r["CompanyName"] + '</a></span><div class="tooltip"><span>User Type:</span> ' + r["EROType"] + '</div></div>';
                        //company("<td/>").html('<div class="hover"><span class="hover-blk"><a onclick="fnGetInfo(' + r["Id"] + ')"  href="/Configuration/Dashboard?Id=' + r["Id"] + '&entitydisplayid=' + r["DisplayId"] + '" target="_blank">' + r["CompanyName"] + '</a></span><div class="tooltip"><span>User Type:</span> ' + r["EROType"] + '</div></div>'));
                    else
                        company = '<div class="hover" ><span class="hover-blk">' + r["CompanyName"] + '</span><div class="tooltip"><span>User Type:</span> ' + r["EROType"] + '</div></div>';
                }
                else {
                    company = '<div class="hover" ><span class="hover-blk">' + r["CompanyName"] + '</span><div class="tooltip"><span>User Type:</span> ' + r["EROType"] + '</div></div>';
                }
            }
            else {
                company = '<div class="hover" ><span class="hover-blk">' + r["CompanyName"] + '</span><div class="tooltip"><span>User Type:</span> ' + r["EROType"] + '</div></div>';
            }
            tableContent += '<td>' + company + '</td>';
            tableContent += '<td>' + r["SalesforceParentID"] + '</td>';
            tableContent += '<td>' + r["MasterIdentifier"] + '</td>';
            tableContent += '<td>' + r["CrossLinkUserId"] + '</td>';
            tableContent += '<td>' + r["AlternativeContact"] + '</td>';
            tableContent += '<td>' + r["EFIN"] + '</td>';
            var ackBank = r.ActiveBank ? r.ActiveBank : ""
            tableContent += '<td>' + ackBank + '</td>';
            var subDate = r.SubmissionDate ? r.SubmissionDate : "";
            tableContent += '<td>' + subDate + '</td>';

            var _enrstatus = r.EnrollmentStatus ? r.EnrollmentStatus : "Not Started";
            var _rejectedbanks = r.RejectedBanks ? '<span>Rejected Banks</span><span>' + r.RejectedBanks + '</span>' : '';
            var _approvedbank = r.ApprovedBank ? '<span>Approved Bank:</span> <span>' + r.ApprovedBank + '</span>' : '';
            var statushtml = '';
            if (_rejectedbanks || _approvedbank)
                statushtml = '<div class="hover"><span class="hover-blk">' + _enrstatus + '</span><div class="tooltip">' + _approvedbank + ' ' + _rejectedbanks + '</div></div>';
            else
                statushtml = _enrstatus;

            tableContent += '<td>' + statushtml + '</td>';

            if (!r.IsActivated) {
                tableContent += '<td></td>';
            }
            else {
                if (_manageEnroll)
                    tableContent += '<td><div class="hover"><span class="hover-blk"><a href="/Enrollment/OfficeInformation?Id=' + r.Id + '&entitydisplayid=' + r["DisplayId"] + '&entityid=' + r["EntityId"] + '" target="_blank"><i class="fa fa-bars"></i></a></span><div class="tooltip"><span>Status:</span> ' + _enrstatus + '</div></div></td>';
                else
                    tableContent += '<td></td>';
            }

            //if (r["TotalServiceFee"] != '' && r["TotalServiceFee"] != '0') {
            //    row.append($("<td/>").html('<div class="hover" ><span class="hover-blk">' + r["TotalServiceFee"] + '</span><div class="tooltip"><span>Service Bureau Fee Summary:</span> ' + r["ServiceTooltip"] + '</div></div>'));

            //} else {
            //    row.append($("<td/>").html(r["TotalServiceFee"]));
            //}
            //if (r["TotalTransFee"] != '' && r["TotalTransFee"] != '0') {
            //    row.append($("<td/>").html('<div class="hover"><span class="hover-blk">' + r["TotalTransFee"] + '</span><div class="tooltip"> <span>Transmission Fee Summary:</span> ' + r["TransTooltip"] + '</div></div>'));
            //}
            //else {
            //    row.append($("<td/>").html(r["TotalTransFee"]));
            // }

            tableContent += '<td></td>';
            tableContent += '<td></td>';
            tableContent += '<td>' + r["AccountStatus"] + '</td>';

            var EntityDisplayId = $('#entitydisplayid').val();
            var IsActivationCompleted = r["IsActivationCompleted"];
            if (EntityDisplayId == $('#Entity_uTax').val()) {
                if (r["DisplayId"] == '4' || r["DisplayId"] == '6') {
                    if (_canresetpwd && _addsuboffice) {
                        if (IsActivationCompleted == '1' || IsActivationCompleted == 1) {
                            tableContent += '<td class="footable-last-column-width"><div class="btn-group dropleft"><button class="btn dropdown-toggle" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">Actions <span class="caret"></span></button><ul class="dropdown-menu dropdown-menu-right" role="menu" aria-labelledby="dropdownMenu"><li><a href="#" onClick="Resetpassword(\'' + r["Id"] + '\')">Reset Password</a></li><li><a href="/SubSiteOfficeConfiguration/CreateSubSiteInfo?ParentId=' + r["Id"] + '" target="_blank">Add Sub Office</a></li></ul></div></td>';
                        } else {
                            tableContent += '<td class="footable-last-column-width"><div class="btn-group dropleft"><button class="btn dropdown-toggle" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">Actions <span class="caret"></span></button><ul class="dropdown-menu dropdown-menu-right" role="menu" aria-labelledby="dropdownMenu"><li><a href="#" onClick="Resetpassword(\'' + r["Id"] + '\')">Reset Password</a></li></ul></div></td>';
                        }
                    }
                    else if (_canresetpwd && !_addsuboffice) {
                        tableContent += '<td class="footable-last-column-width"><div class="btn-group dropleft"><button class="btn dropdown-toggle" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">Actions <span class="caret"></span></button><ul class="dropdown-menu dropdown-menu-right" role="menu" aria-labelledby="dropdownMenu"><li><a href="#" onClick="Resetpassword(\'' + r["Id"] + '\')">Reset Password</a></li></ul></div></td>';
                    }
                    else if (!_canresetpwd && _addsuboffice) {
                        if (IsActivationCompleted == '1' || IsActivationCompleted == 1) {
                            tableContent += '<td class="footable-last-column-width"><div class="btn-group dropleft"><button class="btn dropdown-toggle" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">Actions <span class="caret"></span></button><ul class="dropdown-menu dropdown-menu-right" role="menu" aria-labelledby="dropdownMenu"><li><a href="/SubSiteOfficeConfiguration/CreateSubSiteInfo?ParentId=' + r["Id"] + '" target="_blank">Add Sub Office</a></li></ul></div></td>';
                        }
                    }
                }
                else {
                    if (_canresetpwd)
                        tableContent += '<td class="footable-last-column-width"><div class="btn-group dropleft"><button class="btn dropdown-toggle" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">Actions <span class="caret"></span></button><ul class="dropdown-menu dropdown-menu-right" role="menu" aria-labelledby="dropdownMenu"><li><a href="#" onClick="Resetpassword(\'' + r["Id"] + '\')">Reset Password</a></li></ul></div></td>';
                }
            }
            else if (EntityDisplayId == $('#Entity_MO').val() || EntityDisplayId == $('#Entity_SVB').val()) {

                if (IsActivationCompleted == '1' || IsActivationCompleted == 1) {
                    tableContent += '<td class="footable-last-column-width"><div class="btn-group dropleft"><button class="btn dropdown-toggle" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">Actions <span class="caret"></span></button><ul class="dropdown-menu dropdown-menu-right" role="menu" aria-labelledby="dropdownMenu"><li><a href="/SubSiteOfficeConfiguration/CreateSubSiteInfo?ParentId=' + r["Id"] + '" target="_blank">Add Sub Office</a></li></ul></div></td>';
                }
            } else {
                tableContent += '<td></td>';
            }
            tableContent += '</tr>';


            if (r.ChaildCustomerInfo.length > 0) {
                tableContent += '<tr class="sub" style="display:none;"><td></td><td colspan="15"><table class="tableData"><tr class=""/>';
                //var row_s = $("<tr/>").addClass('sub');
                //row_s.appendTo(table);
                //row_s.append($("<td/>"));
                //var row_td = $("<td/>").attr("colspan", 15);
                //row_s.append(row_td);

                //var table_sub = $('<table/>').addClass('tableData');
                //var row_trh = $("<tr class=''/>");
                //row_trh.appendTo(table_sub);

                tableContent += '<th>Status</th>';
                tableContent += '<th></th>';
                tableContent += '<th>Company</th>';
                tableContent += '<th>Parent ID</th>';
                tableContent += '<th>Master Identifier</th>';
                tableContent += '<th>User ID</th>';
                tableContent += '<th>Contact</th>';
                tableContent += '<th>EFIN</th>';
                tableContent += '<th>Active Bank</th>';
                tableContent += '<th>Submission Date</th>';
                tableContent += '<th>Enr. Status</th>';
                tableContent += '<th>User Type</th>';
                tableContent += '<th>View/Edit Enrollment</th>';
                tableContent += '<th>Service Fee</th>';
                tableContent += '<th>Transmission Fee</th>';
                tableContent += '<th>On Boarding Status</th>';
                tableContent += '<th>Action</th></tr>';
                $.each(r.ChaildCustomerInfo, function (r_index, rs) {
                    tableContent += '<tr>';
                    //var row_tr = $("<tr/>");
                    //row_tr.appendTo(table_sub);
                    tableContent += '<td>' + rs["ActivationStatus"] + '</td>';
                    if ((rs["IsAdditionalEFINAllowed"] == 'true' || rs["IsAdditionalEFINAllowed"] == 'True' || rs["IsAdditionalEFINAllowed"] == true) && (["IsActivationCompleted"] == 'true' || rs["IsActivationCompleted"] == 'True' || rs["IsActivationCompleted"] == true)) {

                        if (rs.ChaildCustomerInfo.length > 0) {
                            tableContent += '<td class="CX"><i class="fa fa-minus-square"></i></td>';
                        } else {
                            tableContent += '<td><td/>';
                        }

                    } else {
                        tableContent += '<td><td/>';
                    }

                    if (_canmanageOfc)
                        tableContent += '<td><a href="/SubSiteOfficeConfiguration/Dashboard?Id=' + rs["Id"] + '&ParentId=' + r["Id"] + '&entitydisplayid=' + rs["DisplayId"] + '" target="_blank">' + rs["CompanyName"] + '</a></td>';
                    else
                        tableContent += '<td>' + rs.CompanyName + '</td>';
                    tableContent += '<td>' + rs["SalesforceParentID"] + '</td>';
                    tableContent += '<td>' + rs["MasterIdentifier"] + '</td>';
                    tableContent += '<td>' + rs["CrossLinkUserId"] + '</td>';
                    tableContent += '<td>' + rs["AlternativeContact"] + '</td>';
                    tableContent += '<td>' + rs["EFIN"] + '</td>';
                    var _chldackbank = rs.ActiveBank ? rs.ActiveBank : '';
                    tableContent += '<td>' + _chldackbank + '</td>';
                    var _chldsdate = rs.SubmissionDate ? rs.SubmissionDate : '';
                    tableContent += '<td>' + _chldsdate + '</td>';

                    var _childenrstatus = r.EnrollmentStatus ? r.EnrollmentStatus : "Not Started";
                    var _childrejectedbanks = rs.RejectedBanks ? '<span>Rejected Banks</span><span>' + rs.RejectedBanks + '</span>' : '';
                    var _childapprovedbank = rs.ApprovedBank ? '<span>Approved Bank:</span> <span>' + rs.ApprovedBank + '</span>' : '';
                    var statushtml = '';
                    if (_childrejectedbanks || _childapprovedbank)
                        statushtml = '<div class="hover"><span class="hover-blk">' + _childenrstatus + '</span><div class="tooltip">' + _childapprovedbank + ' ' + _childrejectedbanks + '</div></div>';
                    else
                        statushtml = _childenrstatus;


                    tableContent += '<td>' + statushtml + '</td>';
                    tableContent += '<td>' + rs["EROType"] + '</td>'


                    if (rs.IsActivationCompleted == 1 && _manageEnroll) {
                        if (rs.IsActivated || (rs.IsActivated && EntityDisplayId != $('#Entity_uTax').val())) {

                            tableContent += '<td><div class="hover"><span class="hover-blk"><a href="/Enrollment/OfficeInformation?Id=' + rs.Id + '&ParentId=' + rs["ParentId"] + '&entitydisplayid=' + rs["DisplayId"] + '&entityid=' + rs["EntityId"] + '" target="_blank"><i class="fa fa-bars"></i></a></span><div class="tooltip"><span>Status:</span> ' + _childenrstatus + '</div></div></td>';

                        }
                        else
                            tableContent += '<td><i class="fa fa-bars" title=""></i></td>';
                    }
                    else {
                        tableContent += '<td></td>';
                    }

                    //  row_tr.append($("<td/>").text(rs["TotalServiceFee"]));
                    //  row_tr.append($("<td/>").text(rs["TotalTransFee"]));
                    // row_tr.append($("<td/>").html('<span class="cst-tooltip"><a href="#" data-toggle="tooltip" data-placement="right"  data-original-title="' + rs["ServiceTooltip"] + '">' + rs["TotalServiceFee"] + '</a></span>'));
                    // row_tr.append($("<td/>").html('<span class="cst-tooltip"><a href="#" data-toggle="tooltip" data-placement="right"  data-original-title="' + rs["TransTooltip"] + '">' + rs["TotalTransFee"] + '</a></span>'));

                    if (rs["TotalServiceFee"] != '' && rs["TotalServiceFee"] != '0') {
                        tableContent += '<td><div class="hover" ><span class="hover-blk">' + rs["TotalServiceFee"] + '</span><div class="tooltip"><span>Service Bureau Fee Summary:</span> ' + rs["ServiceTooltip"] + '</div></div></td>';

                    } else {
                        tableContent += '<td>' + rs["TotalServiceFee"] + '</td>';
                    }
                    if (rs["TotalTransFee"] != '' && rs["TotalTransFee"] != '0') {
                        tableContent += '<td><div class="hover"><span class="hover-blk">' + rs["TotalTransFee"] + '</span><div class="tooltip"> <span>Transmission Fee Summary:</span> ' + rs["TransTooltip"] + '</div></div></td>';
                    }
                    else {
                        tableContent += '<td>' + rs["TotalTransFee"] + '</td>';
                    }
                    tableContent += '<td>' + rs["AccountStatus"] + '</td>';



                    if ((rs["IsAdditionalEFINAllowed"] == 'true' || rs["IsAdditionalEFINAllowed"] == 'True' || rs["IsAdditionalEFINAllowed"] == true) && (["IsActivationCompleted"] == 'true' || rs["IsActivationCompleted"] == 'True' || rs["IsActivationCompleted"] == true)) {
                        tableContent += '<td class="footable-last-column-width"><div class="btn-group dropleft"><button class="btn dropdown-toggle" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">Actions <span class="caret"></span></button><ul class="dropdown-menu dropdown-menu-right" role="menu" aria-labelledby="dropdownMenu"><li><a href="/SubSiteOfficeConfiguration/CreateSubSiteInfo?ParentId=' + rs["Id"] + '" target="_blank">Add Sub Office</a></li></ul></div></td>';
                        //row_tr.append($("<td/>").addClass("footable-last-column-width").html('<div class="btn-group dropleft"><button class="btn dropdown-toggle" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">Actions <span class="caret"></span></button><ul class="dropdown-menu dropdown-menu-right" role="menu" aria-labelledby="dropdownMenu"><li><a href="/SubSiteOfficeConfiguration/CreateSubSiteInfo?ParentId=' + rs["Id"] + '" target="_blank">Add Sub Office</a></li></ul></div>'));
                    } else {
                        tableContent += '<td></td>';
                    }
                    tableContent += '</tr>';

                    if ((rs["IsAdditionalEFINAllowed"] == 'true' || rs["IsAdditionalEFINAllowed"] == 'True' || rs["IsAdditionalEFINAllowed"] == true) && (["IsActivationCompleted"] == 'true' || rs["IsActivationCompleted"] == 'True' || rs["IsActivationCompleted"] == true)) {
                        var childinfo2 = rs["ChaildCustomerInfo"];
                        if (childinfo2.length > 0) {
                            tableContent += '<tr class="sub" style="display:none;">';
                            //var row_s2 = $("<tr/>").addClass('sub');
                            //row_s2.appendTo(table_sub);
                            tableContent += '<td></td>';
                            //row_s2.append($("<td/>"));
                            tableContent += '<td colspan="14"><table class="tableData table_SOMESubSite"><tr class="">';
                            //var row_td2 = $("<td/>").attr("colspan", 14);
                            //row_s2.append(row_td2);

                            //var table_sub2 = $('<table/>').addClass('tableData table_SOMESubSite');
                            //var row_trh2 = $("<tr class=''/>");
                            //row_trh2.appendTo(table_sub2);
                            tableContent += '<th>Status</th>';
                            tableContent += '<th>Company</th>';
                            tableContent += '<th>Parent ID</th>';
                            tableContent += '<th>Master Identifier</th>';
                            tableContent += '<th>User ID</th>';
                            tableContent += '<th>Contact</th>';
                            tableContent += '<th>EFIN</th>';
                            tableContent += '<th>User Type</th>';
                            tableContent += '<th>View/Edit Enrollment</th>';
                            tableContent += '<th>Service Fee</th>';
                            tableContent += '<th>Transmission Fee</th>';
                            tableContent += '<th>On Boarding Status</th></tr>';

                            $.each(childinfo2, function (r_index2, rs2) {
                                tableContent += '<tr>';
                                tableContent += '<td>' + rs2["ActivationStatus"] + '</td>';
                                tableContent += '<td><a href="/SubSiteOfficeConfiguration/Dashboard?Id=' + rs2["Id"] + '&ParentId=' + rs["ParentId"] + '&entitydisplayid=' + rs2["DisplayId"] + '" target="_blank">' + rs2["CompanyName"] + '</a></td>';
                                tableContent += '<td>' +rs2["SalesforceParentID"]+ '</td>';
                                tableContent += '<td>' +rs2["MasterIdentifier"]+ '</td>';
                                tableContent += '<td>' +rs2["CrossLinkUserId"]+ '</td>';
                                tableContent += '<td>' +rs2["AlternativeContact"]+ '</td>';
                                tableContent += '<td>' +rs2["EFIN"]+ '</td>';
                                tableContent += '<td>' + rs2["EROType"] + '</td>';
                                // row_tr2.append($("<td/>").text(rs2["TotalServiceFee"]));
                                // row_tr2.append($("<td/>").text(rs2["TotalTransFee"]));

                                // row_tr2.append($("<td/>").html('<span class="cst-tooltip"><a href="#" data-toggle="tooltip" data-placement="right"  data-original-title="' + rs2["ServiceTooltip"] + '">' + rs2["TotalServiceFee"] + '</a></span>'));
                                //row_tr2.append($("<td/>").html('<span class="cst-tooltip"><a href="#" data-toggle="tooltip" data-placement="right"  data-original-title="' + rs2["TransTooltip"] + '">' + rs2["TotalTransFee"] + '</a></span>'));

                                if (!rs2.IsActivationCompleted) {
                                    tableContent += '<td></td>';
                                }
                                else {
                                    if (_manageEnroll)
                                        tableContent += '<td><div class="hover"><span class="hover-blk"><a href="/Enrollment/OfficeInformation?Id=' + rs2.Id + '&ParentId=' + rs2.ParentId + '&entitydisplayid=' + rs2["DisplayId"] + '&entityid=' + rs2["EntityId"] + '" target="_blank"><i class="fa fa-bars"></i></a></span><div class="tooltip"><span>Status:</span> ' + _enrstatus + '</div></div></td>'
                                    else
                                        tableContent += '<td></td>';
                                }

                                if (rs2["TotalServiceFee"] != '' && rs2["TotalServiceFee"] != '0') {
                                    tableContent += '<td><div class="hover"> <span class="hover-blk"> ' + rs2["TotalServiceFee"] + '</span><div class="tooltip"><span>Service Bureau Fee Summary:</span> ' + rs2["ServiceTooltip"] + '</div></div></td>';

                                } else {
                                    tableContent += '<td>'+rs2["TotalServiceFee"] + '</td>';
                                }
                                if (rs2["TotalTransFee"] != '' && rs2["TotalTransFee"] != '0') {
                                    tableContent += '<td><div class="hover"> <span class="hover-blk">' + rs2["TotalTransFee"] + '</span><div class="tooltip"> <span>Transmission Fee Summary:</span> ' + rs2["TransTooltip"] + '</div></div></td>';
                                }
                                else {
                                    tableContent += '<td>'+rs2["TotalTransFee"] + '</td>';
                                }

                                tableContent += '<td>' + rs2["AccountStatus"] + '</td></tr>';
                            });
                            tableContent += '</table></td></tr>';
                            //row_td2.append(table_sub2);
                        }
                    }
                });
                // row_td.append(table_sub);
                tableContent += '</table></td></tr>';
            }
        });
        
        //console.log(tableContent);
        $("#tbCustomersBody").html( tableContent);

    });

    // $('table').footable();
}

function getAllCustomerInfo() {
    var msg = localStorage.getItem("CrossMsg");
    localStorage.removeItem("CrossMsg");

    if (msg != '' && msg != null && msg != undefined) {
        var success = $('#success');
        success.html('');
        success.hide();
        success.show();
        success.append('<p> ' + msg + ' </p>');
    }
    //$('#tbAllCustomerInfo').html('');
    var url = '/api/CustomerInformation/CustomerAllInfo?UserName=' + $('#UserId').val();
    $("#tbAllCustomerInfo > tbody").remove();
    var table = $('#tbAllCustomerInfo').append('<tbody/>'); //$(document.body)
    var s_Entity = $('#entityid').val();
    ajaxHelper(url, 'GET').done(function (data) {
        $.each(data, function (rowIndex, r) {
            var row;
            if (r["DisplayId"] == 4 || r["DisplayId"] == 6) {
                row = $("<tr/>").addClass('success').attr('role', 'row');
            }
            else {
                row = $("<tr/>").addClass('odd').attr('role', 'row');
            }
            row.appendTo(table);
            row.append($("<td/>").text(r["ActivationStatus"]));

            var childinfo = r["ChaildCustomerInfoCount"];
            if (childinfo > 0) {
                row.append($("<td/>").addClass("CX").html('<i class="fa fa-minus-square"></i>'));
            }
            else {
                row.append($("<td/>"));
            }

            if ($('#entitydisplayid').val() != 4 && $('#entitydisplayid').val() != 6) {
                if (r["DisplayId"] == 4 || r["DisplayId"] == 6) {
                    if (_canmanageOfc)
                        row.append($("<td/>").html('<div class="hover"><span class="hover-blk"><a href="../Configuration/Dashboard?MoSvbParentId=' + r["Id"] + '" target="_blank">' + r["CompanyName"] + '</a></span><div class="tooltip"><span>User Type:</span> ' + r["EROType"] + '</div></div>'));
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
            row.append($("<td/>").text(''));
            row.append($("<td/>").text(''));
            row.append($("<td/>").text(''));
            if (!r.IsActivated) {
                row.append($("<td/>").html(''));
            }
            else {
                if (_manageEnroll)
                    row.append($("<td/>").html('<a href="../Enrollment/OfficeInformation?Id=' + r.Id + '" target="_blank"><i class="fa fa-bars"></i></a>'));
                else
                    row.append($("<td/>").html(''));
            }

            if (r["TotalServiceFee"] != '' && r["TotalServiceFee"] != '0') {
                row.append($("<td/>").html('<div class="hover" ><span class="hover-blk">' + r["TotalServiceFee"] + '</span><div class="tooltip"><span>Service Bureau Fee Details:</span> ' + r["ServiceTooltip"] + '</div></div>'));

            } else {
                row.append($("<td/>").html(r["TotalServiceFee"]));
            }
            if (r["TotalTransFee"] != '' && r["TotalTransFee"] != '0') {
                row.append($("<td/>").html('<div class="hover"><span class="hover-blk">' + r["TotalTransFee"] + '</span><div class="tooltip"> <span>Service Bureau Fee Details:</span> ' + r["TransTooltip"] + '</div></div>'));
            }
            else {
                row.append($("<td/>").html(r["TotalTransFee"]));
            }
            row.append($("<td/>").text(r["AccountStatus"]));
            var EntityDisplayId = $('#entitydisplayid').val();

            var IsActivationCompleted = r["IsActivationCompleted"];
            if (EntityDisplayId == $('#Entity_uTax').val()) {//  if (s_Entity == '8f4fca72-3d3c-4ad3-8f1c-1de3d602fd73') {

                if (r["DisplayId"] == '4' || r["DisplayId"] == '6') {
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



function GetRecentlyCreate() {
    var url = '/api/OfficeManagement/RecentlyCreate?UserID=' + $('#UserId').val();
    ajaxHelper(url, 'GET').done(function (data) {
        var tbCreate = $('#tblCreate');
        $.each(data, function (rind, rc) {
            if (rc) {
                var row = $("<tr/>").appendTo(tbCreate);
                row.append($("<td/>").text(rc["Name"]));
                row.append($("<td/>").text(rc["updateDatetime"]));
            }
        });
    });
}

function GetRecentlyUpdate() {
    var url = '/api/OfficeManagement/RecentlyUpdate?UserID=' + $('#UserId').val();
    ajaxHelper(url, 'GET').done(function (data) {
        var tbUpdate = $('#tblUpdate');
        $.each(data, function (rind, ru) {
            if (ru) {
                var row = $("<tr/>").appendTo(tbUpdate);
                row.append($("<td/>").text(ru["Name"]));
                row.append($("<td/>").text(ru["updateDatetime"]));
            }
        });
    });
}

function Resetpassword(uid) {
    $('#success').hide();
    //var data = '{ UserId : ' + uid + '}' //{ firstName: "Denny"}
    $('#success p').html('');
    var conformResult = confirm("Please confirm if you wish to reset the customer's EMP password to be the same as the Transmission Password ?");
    if (conformResult == true) {
        var custmorUri = '/api/ChangePassword/Reset';
        ajaxHelper(custmorUri, 'POST', { UserId: uid }).done(function (data) {
            $('#success').show();
            $('#success').append('<p>Password has been reset to the Transmission Password </p>');
        });
    }
}
//This is Get the Status for multi - ddl
function getMultiSelectStatus(container) {
    container.html('');
    var entityid = $('#entityid').val();
    var custmorUri = '/api/dropdown/Status';
    ajaxHelper(custmorUri, 'GET').done(function (data) {
        $.each(data, function (rowIndex, r) {
            container.append('<option value="' + r["Name"] + '">' + r["DisplayText"] + '</option>"');
        });
    });
}
//This is Get the Site type for multi - ddl
function getMultiSelectSiteType(container) {
    container.html('');
    var entityid = $('#entityid').val();
    var custmorUri = '/api/dropdown/Entities';
    ajaxHelper(custmorUri, 'GET').done(function (data) {
        $.each(data, function (rowIndex, r) {
            container.append('<option value="' + r["Id"] + '">' + r["Name"] + '</option>"');
        });
    });
}


function getMultiSelectBank(container) {
    container.html('');
    var entityid = $('#entityid').val();
    var custmorUri = '/api/dropdown/bank?entityid=' + entityid;
    ajaxHelper(custmorUri, 'GET').done(function (data) {
        $.each(data, function (rowIndex, r) {
            container.append('<option value="' + r["Id"] + '">' + r["Name"] + '</option>"');
        });
    });
}

//This is Get the Enrollment Status for multi - ddl
function getMultiSelectEnrollmentStatus(container) {
    container.html('');
    var entityid = $('#entityid').val();
    var custmorUri = '/api/dropdown/EnrollmentStatus';
    ajaxHelper(custmorUri, 'GET').done(function (data) {
        $.each(data, function (rowIndex, r) {
            container.append('<option value="' + r["Name"] + '">' + r["DisplayText"] + '</option>"');
        });
    });
}

//This is Get the OnBoarding Status for multi - ddl
function getMultiSelectOnBoardingStatus(container) {
    container.html('');
    var entityid = $('#entityid').val();
    var custmorUri = '/api/dropdown/OnBoardingStatus';
    ajaxHelper(custmorUri, 'GET').done(function (data) {
        $.each(data, function (rowIndex, r) {
            container.append('<option value="' + r["Name"] + '">' + r["DisplayText"] + '</option>"');
        });
    });
}

function fnDisplay(i) {
    getAllCustomerInformation(i);
    setTimeout(function () {
        $('#tbAllCustomerInfo tr.sub').css('display', 'none');
        $('.fa-minus-square').addClass('fa-plus-square');
        $('.fa-plus-square').removeClass('fa-minus-square');
    })
}

function fnDisplayClear() {
    $('#txtSearch').val('');
    //$('#IsCustomerName').prop('checked', true);
    $('#StatusID').val(0).multiselect('refresh');
    $('#SiteTypeID').val(0).multiselect('refresh');
    $('#BankPartnerID').val(0).multiselect('refresh');
    $('#EnrollmentID').val(0).multiselect('refresh');
    $('#OnBoardStatusID').val(0).multiselect('refresh');
}


var _newwindows = [];

function openOfficeMgmt(id) {
    var win = window.open('http://www.google.com', '_target');
    _newwindows.push(win);
    localStorage.setItem("_newwindows", JSON.stringify(_newwindows));


}
