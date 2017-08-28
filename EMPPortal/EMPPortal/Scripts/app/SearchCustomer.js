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

            var childinfo = r["ChaildCustomerInfo"];
            if (childinfo.length > 0) {
                row.append($("<td/>").addClass("CX").html('<i class="fa fa-minus-square"></i>'));
            }
            else {
                row.append($("<td/>"));
            }

            if ($('#entitydisplayid').val() != 4 && $('#entitydisplayid').val() != 6) {
                if (r["DisplayId"] == 4 || r["DisplayId"] == 6) {
                    if (_canmanageOfc)
                        row.append($("<td/>").html('<div class="hover"><span class="hover-blk"><a onclick="fnGetInfo(' + r["Id"] + ')"  href="/Configuration/Dashboard?Id=' + r["Id"] + '&entitydisplayid=' + r["DisplayId"] + '" target="_blank">' + r["CompanyName"] + '</a></span><div class="tooltip"><span>User Type:</span> ' + r["EROType"] + '</div></div>'));
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
                if (_manageEnroll && ($('#entitydisplayid').val() != $('#Entity_MO').val() && $('#entitydisplayid').val() != $('#Entity_SVB').val()))
                    row.append($("<td/>").html('<div class="hover"><span class="hover-blk"><a href="/Enrollment/OfficeInformation?Id=' + r.Id + '&entitydisplayid=' + r["DisplayId"] + '&entityid=' + r["EntityId"] + '" target="_blank"><i class="fa fa-bars"></i></a></span><div class="tooltip"><span>Status:</span> ' + _enrstatus + '</div></div>'));
                else
                    row.append($("<td/>").html(''));
            }
            row.append($("<td/>").html(''));
            row.append($("<td/>").html(''));

            row.append($("<td/>").text(r["AccountStatus"]));
            var EntityDisplayId = $('#entitydisplayid').val();

            var IsActivationCompleted = r["IsActivationCompleted"];
            if (EntityDisplayId == $('#Entity_uTax').val()) {
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

            if (childinfo.length > 0) {
                var row_s = $("<tr/>").addClass('sub');
                row_s.appendTo(table);
                row_s.append($("<td/>"));
                var row_td = $("<td/>").attr("colspan", 15);
                row_s.append(row_td);

                var table_sub = $('<table/>').addClass('tableData');
                var row_trh = $("<tr class=''/>");
                row_trh.appendTo(table_sub);
                row_trh.append($("<td/>").text("Status"));
                row_trh.append($("<th/>").text(""));
                row_trh.append($("<th/>").html("Company"));
                row_trh.append($("<th/>").text("Parent ID"));
                row_trh.append($("<th/>").text("Master Identifier"));
                row_trh.append($("<th/>").text("User ID"));
                row_trh.append($("<th/>").text("Contact"));
                row_trh.append($("<th/>").text("EFIN"));
                row_trh.append($("<th/>").text("Active Bank"));
                row_trh.append($("<th/>").text("Submission Date"));
                row_trh.append($("<th/>").text("Enr. Status"));
                row_trh.append($("<th/>").text("User Type"));
                row_trh.append($("<th/>").text("View/Edit Enrollment"));
                row_trh.append($("<th/>").text("Service Fee"));
                row_trh.append($("<th/>").text("Transmission Fee"));
                row_trh.append($("<th/>").text("On Boarding Status"));
                row_trh.append($("<th/>").text("Action"));
                $.each(childinfo, function (r_index, rs) {
                    var row_tr = $("<tr/>");
                    row_tr.appendTo(table_sub);
                    row_tr.append($("<td/>").text(rs["ActivationStatus"]));
                    if ((rs["IsAdditionalEFINAllowed"] == 'true' || rs["IsAdditionalEFINAllowed"] == 'True' || rs["IsAdditionalEFINAllowed"] == true)) { //&& (rs["IsActivationCompleted"] == 'true' || rs["IsActivationCompleted"] == 'True' || rs["IsActivationCompleted"] == true)

                        var childinfo2 = rs["ChaildCustomerInfo"];

                        if (childinfo2.length > 0) {
                            row_tr.append($("<td/>").addClass("CX").html('<i class="fa fa-minus-square"></i>'));
                        } else {
                            row_tr.append($("<td/>"));
                        }

                    } else {
                        row_tr.append($("<td/>"));
                    }

                    if (_canmanageOfc)
                        row_tr.append($("<td/>").html('<a href="/SubSiteOfficeConfiguration/Dashboard?Id=' + rs["Id"] + '&ParentId=' + r["Id"] + '&entitydisplayid=' + rs["DisplayId"] + '" target="_blank">' + rs["CompanyName"] + '</a>'));
                    else
                        row_tr.append($("<td/>").text(rs.CompanyName));
                    row_tr.append($("<td/>").text(rs["SalesforceParentID"]));
                    row_tr.append($("<td/>").text(rs["MasterIdentifier"]));
                    row_tr.append($("<td/>").text(rs["CrossLinkUserId"] + " (" + rs["EMPPassword"] + ")"));
                    row_tr.append($("<td/>").text(rs["AlternativeContact"]));
                    row_tr.append($("<td/>").text(rs["EFIN"]));
                    row_tr.append($("<td/>").text(rs.ActiveBank ? rs.ActiveBank : ''));
                    row_tr.append($("<td/>").text(rs.SubmissionDate ? rs.SubmissionDate : ''));

                    var _childenrstatus = r.EnrollmentStatus ? r.EnrollmentStatus : "Not Started";
                    var _childrejectedbanks = rs.RejectedBanks ? '<span>Rejected Banks</span><span>' + rs.RejectedBanks + '</span>' : '';
                    var _childapprovedbank = rs.ApprovedBank ? '<span>Approved Bank:</span> <span>' + rs.ApprovedBank + '</span>' : '';
                    var statushtml = '';
                    if (_childrejectedbanks || _childapprovedbank)
                        statushtml = '<div class="hover"><span class="hover-blk">' + _childenrstatus + '</span><div class="tooltip">' + _childapprovedbank + ' ' + _childrejectedbanks + '</div></div>';
                    else
                        statushtml = _childenrstatus;


                    row_tr.append($("<td/>").html(statushtml));
                    row_tr.append($("<td/>").text(rs["EROType"]));


                    if (rs.IsActivationCompleted == 1 && _manageEnroll) {
                        if (rs.IsActivated || (rs.IsActivated && EntityDisplayId != $('#Entity_uTax').val())) {

                            row_tr.append($("<td/>").html('<div class="hover"><span class="hover-blk"><a href="/Enrollment/OfficeInformation?Id=' + rs.Id + '&ParentId=' + rs["ParentId"] + '&entitydisplayid=' + rs["DisplayId"] + '&entityid=' + rs["EntityId"] + '" target="_blank"><i class="fa fa-bars"></i></a></span><div class="tooltip"><span>Status:</span> ' + _childenrstatus + '</div></div>'));

                        }
                        else
                            row_tr.append($("<td/>").html('<i class="fa fa-bars" title=""></i>'));
                    }
                    else {
                        row_tr.append($("<td/>").html(''));
                    }

                    //  row_tr.append($("<td/>").text(rs["TotalServiceFee"]));
                    //  row_tr.append($("<td/>").text(rs["TotalTransFee"]));
                    // row_tr.append($("<td/>").html('<span class="cst-tooltip"><a href="#" data-toggle="tooltip" data-placement="right"  data-original-title="' + rs["ServiceTooltip"] + '">' + rs["TotalServiceFee"] + '</a></span>'));
                    // row_tr.append($("<td/>").html('<span class="cst-tooltip"><a href="#" data-toggle="tooltip" data-placement="right"  data-original-title="' + rs["TransTooltip"] + '">' + rs["TotalTransFee"] + '</a></span>'));

                    if (rs["TotalServiceFee"] != '' && rs["TotalServiceFee"] != '0') {
                        row_tr.append($("<td/>").html('<div class="hover" ><span class="hover-blk">' + rs["TotalServiceFee"] + '</span><div class="tooltip"><span>Service Bureau Fee Summary:</span> ' + rs["ServiceTooltip"] + '</div></div>'));

                    } else {
                        row_tr.append($("<td/>").html(rs["TotalServiceFee"]));
                    }
                    if (rs["TotalTransFee"] != '' && rs["TotalTransFee"] != '0') {
                        row_tr.append($("<td/>").html('<div class="hover"><span class="hover-blk">' + rs["TotalTransFee"] + '</span><div class="tooltip"> <span>Transmission Fee Summary:</span> ' + rs["TransTooltip"] + '</div></div>'));
                    }
                    else {
                        row_tr.append($("<td/>").html(rs["TotalTransFee"]));
                    }
                    row_tr.append($("<td/>").text(rs["AccountStatus"]));



                    if ((rs["IsAdditionalEFINAllowed"] == 'true' || rs["IsAdditionalEFINAllowed"] == 'True' || rs["IsAdditionalEFINAllowed"] == true) && (rs["IsActivationCompleted"] == 'true' || rs["IsActivationCompleted"] == 'True' || rs["IsActivationCompleted"] == true)) {
                        row_tr.append($("<td/>").addClass("footable-last-column-width").html('<div class="btn-group dropleft"><button class="btn dropdown-toggle" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">Actions <span class="caret"></span></button><ul class="dropdown-menu dropdown-menu-right" role="menu" aria-labelledby="dropdownMenu"><li><a href="#" onClick="Resetpassword(\'' + rs["Id"] + '\')">Reset Password</a></li><li><a href="/SubSiteOfficeConfiguration/CreateSubSiteInfo?ParentId=' + rs["Id"] + '" target="_blank">Add Sub Office</a></li></ul></div>'));
                    } else {
                        row_tr.append($("<td/>").addClass("footable-last-column-width").html('<div class="btn-group dropleft"><button class="btn dropdown-toggle" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">Actions <span class="caret"></span></button><ul class="dropdown-menu dropdown-menu-right" role="menu" aria-labelledby="dropdownMenu"><li><a href="#" onClick="Resetpassword(\'' + rs["Id"] + '\')">Reset Password</a></li></ul></div>'));
                    }

                    if ((rs["IsAdditionalEFINAllowed"] == 'true' || rs["IsAdditionalEFINAllowed"] == 'True' || rs["IsAdditionalEFINAllowed"] == true)) { //&& (rs["IsActivationCompleted"] == 'true' || rs["IsActivationCompleted"] == 'True' || rs["IsActivationCompleted"] == true)
                        var childinfo2 = rs["ChaildCustomerInfo"];
                        if (childinfo2.length > 0) {
                            var row_s2 = $("<tr/>").addClass('sub');
                            row_s2.appendTo(table_sub);
                            row_s2.append($("<td/>"));
                            var row_td2 = $("<td/>").attr("colspan", 14);
                            row_s2.append(row_td2);

                            var table_sub2 = $('<table/>').addClass('tableData table_SOMESubSite');
                            var row_trh2 = $("<tr class=''/>");
                            row_trh2.appendTo(table_sub2);
                            row_trh2.append($("<td/>").text("Status"));
                            row_trh2.append($("<th/>").text("Company"));
                            row_trh2.append($("<th/>").text("Parent ID"));
                            row_trh2.append($("<th/>").text("Master Identifier"));
                            row_trh2.append($("<th/>").text("User ID"));
                            row_trh2.append($("<th/>").text("Contact"));
                            row_trh2.append($("<th/>").text("EFIN"));
                            row_trh2.append($("<th/>").text("User Type"));
                            row_trh2.append($("<th/>").text("View/Edit Enrollment"));
                            row_trh2.append($("<th/>").text("Service Fee"));
                            row_trh2.append($("<th/>").text("Transmission Fee"));
                            row_trh2.append($("<th/>").text("On Boarding Status"));
                            row_trh2.append($("<th/>").text("Action"));
                            $.each(childinfo2, function (r_index2, rs2) {
                                var row_tr2 = $("<tr/>");
                                row_tr2.appendTo(table_sub2);
                                row_tr2.append($("<td/>").text(rs2["ActivationStatus"]));
                                row_tr2.append($("<td/>").html('<a href="/SubSiteOfficeConfiguration/Dashboard?Id=' + rs2["Id"] + '&ParentId=' + rs["ParentId"] + '&entitydisplayid=' + rs2["DisplayId"] + '" target="_blank">' + rs2["CompanyName"] + '</a>'));
                                row_tr2.append($("<td/>").text(rs2["SalesforceParentID"]));
                                row_tr2.append($("<td/>").text(rs2["MasterIdentifier"]));
                                row_tr2.append($("<td/>").text(rs2["CrossLinkUserId"] + " (" + rs2["EMPPassword"] + ")"));
                                row_tr2.append($("<td/>").text(rs2["AlternativeContact"]));
                                row_tr2.append($("<td/>").text(rs2["EFIN"]));
                                row_tr2.append($("<td/>").text(rs2["EROType"]));
                                // row_tr2.append($("<td/>").text(rs2["TotalServiceFee"]));
                                // row_tr2.append($("<td/>").text(rs2["TotalTransFee"]));

                                // row_tr2.append($("<td/>").html('<span class="cst-tooltip"><a href="#" data-toggle="tooltip" data-placement="right"  data-original-title="' + rs2["ServiceTooltip"] + '">' + rs2["TotalServiceFee"] + '</a></span>'));
                                //row_tr2.append($("<td/>").html('<span class="cst-tooltip"><a href="#" data-toggle="tooltip" data-placement="right"  data-original-title="' + rs2["TransTooltip"] + '">' + rs2["TotalTransFee"] + '</a></span>'));

                                if (!rs2.IsActivationCompleted) {
                                    row_tr2.append($("<td/>").html(''));
                                }
                                else {
                                    if (_manageEnroll)
                                        row_tr2.append($("<td/>").html('<div class="hover"><span class="hover-blk"><a href="/Enrollment/OfficeInformation?Id=' + rs2.Id + '&ParentId=' + rs.ParentId + '&entitydisplayid=' + rs2["DisplayId"] + '&entityid=' + rs2["EntityId"] + '" target="_blank"><i class="fa fa-bars"></i></a></span><div class="tooltip"><span>Status:</span> ' + _enrstatus + '</div></div>'));
                                    else
                                        row_tr2.append($("<td/>").html(''));
                                }

                                if (rs2["TotalServiceFee"] != '' && rs2["TotalServiceFee"] != '0') {
                                    row_tr2.append($("<td/>").html('<div class="hover"> <span class="hover-blk"> ' + rs2["TotalServiceFee"] + '</span><div class="tooltip"><span>Service Bureau Fee Summary:</span> ' + rs2["ServiceTooltip"] + '</div></div>'));

                                } else {
                                    row_tr2.append($("<td/>").html(rs2["TotalServiceFee"]));
                                }
                                if (rs2["TotalTransFee"] != '' && rs2["TotalTransFee"] != '0') {
                                    row_tr2.append($("<td/>").html('<div class="hover"> <span class="hover-blk">' + rs2["TotalTransFee"] + '</span><div class="tooltip"> <span>Transmission Fee Summary:</span> ' + rs2["TransTooltip"] + '</div></div>'));
                                }
                                else {
                                    row_tr2.append($("<td/>").html(rs2["TotalTransFee"]));
                                }

                                row_tr2.append($("<td/>").text(rs2["AccountStatus"]));
                                row_tr2.append($("<td/>").addClass("footable-last-column-width").html('<div class="btn-group dropleft"><button class="btn dropdown-toggle" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">Actions <span class="caret"></span></button><ul class="dropdown-menu dropdown-menu-right" role="menu" aria-labelledby="dropdownMenu"><li><a href="#" onClick="Resetpassword(\'' + rs2["Id"] + '\')">Reset Password</a></li></ul></div>'));

                            });
                            row_td2.append(table_sub2);
                        }
                    }
                });
                row_td.append(table_sub);
            }
        });




        setTimeout(function () {
            FooTable.get('#tbAllCustomerInfo').pageSize($('#change-page-size').val());
            $('.footable').data('page-size', $('#change-page-size').val());
            $('.footable').trigger('footable_initialized');

            $('.footable-page a').click(function () {
                setTimeout(function () {
                    $('#tbAllCustomerInfo tr.sub').css('display', 'none');
                    $('.fa-minus-square').addClass('fa-plus-square');
                    $('.fa-plus-square').removeClass('fa-minus-square');
                })
            })
        })

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
    var url = '/api/OfficeManagement/RecentlyCreate?UserID=' + $('#UserId').val() + '&Mainentity=' + $('#entityid').val();
    ajaxHelper(url, 'GET').done(function (data) {
        var tbCreate = $('#tblCreate');
        $.each(data, function (rind, rc) {
            if (rc) {
                var row = $("<tr/>").appendTo(tbCreate);
                var anchref = rc.Ptype == 'config' ? '/Configuration/Dashboard?' : '/SubSiteOfficeConfiguration/Dashboard?';
                var ancQs = anchref+'Id=' + rc.Id + '&ParentId=' + (rc.ParentId ? rc.ParentId : '00000000-0000-0000-0000-000000000000') + '&entitydisplayid=' + rc.EntityDisplayId + '&entityid=' + rc.EntityId + '&ptype=' + rc.Ptype;
                var ancCompany = '<a href="' + ancQs + '" target="_blank">' + rc.Name + '</a>'
                row.append($("<td/>").html(ancCompany));
                row.append($("<td/>").text(rc["Date"]));
            }
        });
    });
}

function GetRecentlyUpdate() {
    var url = '/api/OfficeManagement/RecentlyUpdate?UserID=' + $('#UserId').val() + '&Mainentity=' + $('#entityid').val();
    ajaxHelper(url, 'GET').done(function (data) {
        var tbUpdate = $('#tblUpdate');
        $.each(data, function (rind, ru) {
            if (ru) {
                var row = $("<tr/>").appendTo(tbUpdate);
                var anchref = ru.Ptype == 'config' ? '/Configuration/Dashboard?' : '/SubSiteOfficeConfiguration/Dashboard?';
                var ancQs = anchref + 'Id=' + ru.Id + '&ParentId=' + (ru.ParentId ? ru.ParentId : '00000000-0000-0000-0000-000000000000') + '&entitydisplayid=' + ru.EntityDisplayId + '&entityid=' + ru.EntityId + '&ptype=' + ru.Ptype;
                var ancCompany = '<a href="' + ancQs + '" target="_blank">' + ru.Name + '</a>'
                row.append($("<td/>").html(ancCompany));
                row.append($("<td/>").text(ru["Date"]));
            }
        });
    });
}

function Resetpassword(uid) {

    var req = {};
    req.CustomerOfficeId = uid;
    req.UserId = $('#UserId').val();

    $('#success').hide();
    //var data = '{ UserId : ' + uid + '}' //{ firstName: "Denny"}
    $('#success p').html('');
    var conformResult = confirm("Please confirm if you wish to reset the customer's EMP password to be the same as the Transmission Password ?");
    if (conformResult == true) {
        var custmorUri = '/api/ChangePassword/ResetPassword';
        ajaxHelper(custmorUri, 'POST', req).done(function (data) {
            if (data == 'true' || data == 'True' || data == true) {
                $('#success').show();
                $('#success').append('<p>Password has been reset to the Transmission Password </p>');
                // getAllCustomerInfo();
            }
        });
    }
}
//This is Get the Status for multi - ddl
function getMultiSelectStatus(container) {
    container.html('');
    var entityid = $('#entityid').val();
    var custmorUri = '/api/dropdown/Status';
    ajaxHelper(custmorUri, 'GET', null, false).done(function (data) {
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
    ajaxHelper(custmorUri, 'GET', null, false).done(function (data) {

        $.each(data, function (rowIndex, r) {
            if (r)
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

function getMultiSelectBankFilter(container) {
    container.html('');
    var entityid = $('#entityid').val();
    var custmorUri = '/api/dropdown/bank?entityid=' + entityid;
    ajaxHelper(custmorUri, 'GET', null, false).done(function (data) {
        $.each(data, function (rowIndex, r) {
            container.append('<option value="' + r["Name"] + '">' + r["Name"] + '</option>"');
        });
    });
}

//This is Get the Enrollment Status for multi - ddl
function getMultiSelectEnrollmentStatus(container) {
    container.html('');
    var entityid = $('#entityid').val();
    var custmorUri = '/api/dropdown/EnrollmentStatus';
    ajaxHelper(custmorUri, 'GET', null, false).done(function (data) {
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
    ajaxHelper(custmorUri, 'GET', null, false).done(function (data) {
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


function ActiveAccount(uid) {

    var req = {};
    req.CustomerId = uid;
    req.UserId = $('#UserId').val();

    $('#success').hide();
    //var data = '{ UserId : ' + uid + '}' //{ firstName: "Denny"}
    $('#success p').html('');
    var conformResult = confirm("Please confirm if you wish to activate the customer's account?");
    if (conformResult == true) {
        var custmorUri = '/api/Configuration/ActiveAccount?CustomerId=' + uid + '&UserId=' + $('#UserId').val();
        ajaxHelper(custmorUri, 'POST', req).done(function (data) {
            if (data == 'true' || data == 'True' || data == true) {
                $('#success').show();
                $('#success').append('<p>Account has been activated</p>');
            }
        });
    }
}