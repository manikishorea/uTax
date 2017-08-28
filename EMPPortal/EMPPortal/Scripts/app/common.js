//var EMPPortalWebAPI = 'http://192.168.10.33:1004';
//var EMPAdminWebAPI = 'http://192.168.10.33:1003';
var EMPPortalWebAPI = 'http://27.250.24.101:2002';
var EMPAdminWebAPI = 'http://27.250.24.101:2001';
//var EMPPortalWebAPI = 'http://localhost:9002';
//var EMPAdminWebAPI = 'http://localhost:9001';

var EMPAdminWebAPI_C = 'http://27.250.24.101:2001';

var _canupdateswid = true, _canviewinfo = true;
var _canresetpwd = true, _addsuboffice = true, _canmanageOfc = true, _manageEnroll = true, _unlockEnroll = true, _archiveInfo = true,_showpassword = true;
var _feeroprtview = true, _nobankappreportviewe = true, _enrstatusreportview = true, _loginreportview = true;

//$(window).bind('load', function () {
//    $('#loading_main').show();
//    $(".btn.btn-default").show();
//    $('#btn_submitenr').hide();
//});

$(document).ajaxStart(function () {
    $('#loading_main').show();
    $(".btn.btn-default").hide();
    $('#btn_submitenr').hide();
});

$(document).ajaxComplete(function () {
    $('#loading_main').hide();
    $(".btn.btn-default").show();
    $('#btn_submitenr').hide();
});

function ajaxHelper(uri, method, data, async) {
    var xAsync = async;
    if (xAsync == undefined || xAsync == '' || xAsync == null || xAsync == true) {
        async = true;
    }
    else {
        xAsync = false;
    }

    $('*').removeClass('error_msg');
    $('*').removeClass('error_msg');
    var token = $('#Token').val();
    uri = EMPPortalWebAPI + uri;

    //var headers = {};
    //if (token != null && token != '' && token != '00000000-0000-0000-0000-000000000000') {
    //    headers.Authorization = 'Bearer ' + token;
    //}
    $('#loading_main').show();
    $('.btn.btn-default').hide();
    $('#error').hide();

    //setTimeout(function(){
    return $.ajax({
        type: method,
        url: uri,
        //beforeSend: function (xhr) {
        //    xhr.setRequestHeader('Token', token);
        //    xhr.setRequestHearder('IP', userip);
        //    headers;
        //},
        beforeSend: function (xhr) {
            $('#loading_main').show();
            xhr.setRequestHeader('Token', token);
            // xhr.setRequestHeader('X-USER-IP', userip);
        },
        dataType: 'json',
        contentType: 'application/json',
        data: data ? JSON.stringify(data) : null,
        async: xAsync,
        complete: function () {
            $('#loading_main').hide();
        }
    }).fail(function (jqXHR, textStatus, errorThrown) {
        $('#loading_main').hide();
        //localStorage.setItem('errrr', uri + ' ' + window.location.href + +jqXHR.status);
        if (jqXHR.status == 404) {
            var UId = $('LoginId').val();
            LogException('Not Found', uri, UId);
        }

        if (jqXHR.status != 406 && jqXHR.status != 0) {
            //return;
            $('#error').show();
            // $('#error').html('<p>' + errorThrown + '</p>');
            $('#error').html('<p>An error occured executing the action. Please try again or contact the support team. </p>');
        }
    });
    //})
}

function LogException(Message, Method, UserId) {

    var req = {};
    req.Message = Message;
    req.MethodName = Method;
    req.UserId = UserId;

    var token = $('#Token').val();

    $.ajax({
        type: 'POST',
        url: EMPPortalWebAPI + '/api/Error/LogException',
        beforeSend: function (xhr) { xhr.setRequestHeader('Token', token); },
        dataType: 'json',
        contentType: 'application/json',
        data: JSON.stringify(req),
        async: false,
        complete: function () {

        }
    });
}

function ajaxDownloader(uri, method, data) {

    var token = $('#Token').val();
    uri = EMPAdminWebAPI + uri;

    //var headers = {};
    //if (token != null && token != '' && token != '00000000-0000-0000-0000-000000000000') {
    //    headers.Authorization = 'Bearer ' + token;
    //}

    $('#loading_main').show();
    $('#error').hide();
    return $.ajax({
        type: method,
        url: uri,
        beforeSend: function (xhr) { xhr.setRequestHeader('Token', token); },
        dataType: 'json',
        contentType: 'application/json',
        data: data ? JSON.stringify(data) : null,
        async: false,
        complete: function () {
            $('#loading_main').hide();
        }
    }).fail(function (jqXHR, textStatus, errorThrown) {
        //  self.error(errorThrown);
        $('#error').show();
        $('#error').html('<p>' + errorThrown + '</p>');
    });
}

$(function () {
    $('a.popup-ajax').popover({
        "html": true,
        "content": function () {
            var div_id = "tmp-id-" + $.now();
            return details_in_popup($(this).attr('href'), div_id);
        }
    });
    checkLoginStatus();

});

function details_in_popup(link, div_id) {
    $.ajax({
        url: link,
        success: function (response) {
            $('#' + div_id).html(response);
        }
    });
    return '<div id="' + div_id + '">Loading...</div>';
}


function GotoListScreen(url) {
    window.location.href = url;
}

function ActionList(Id, Status, EntityId) {
    var actions = '<td><div class="btn-group dropleft"><button class="btn dropdown-toggle" aria-expanded="false" aria-haspopup="true" data-toggle="dropdown">Actions <span class="caret"></span></button>';
    actions = actions + '<ul class="dropdown-menu dropdown-menu-right" aria-labelledby="dropdownMenu" role="menu">';
    if (_canviewinfo) {
        actions = actions + '<li class="EditLink">';
        actions = actions + '<a href="/CustomerInformation/ViewCustomer?Id=' + Id + '&EntityId=' + EntityId + '"><i class="fa fa-pencil-square-o" aria-hidden="true" title="Edit"></i> View Details</a>';
        actions = actions + '</li>';
        actions = actions + '<li class="divider"></li>';
    }
    if (_canupdateswid) {
        actions = actions + '<li class="EditLink">';
        actions = actions + '<a href="/CustomerLoginInformation/Index?Id=' + Id + '&EntityId=' + EntityId + '"><i class="fa fa-pencil-square-o" aria-hidden="true" title="Edit"></i> Update Software Identification</a>';
        actions = actions + '</li>';
    }
    actions = actions + '</ul>';
    actions = actions + '</div></td>';
    return actions;
}


//http://stackoverflow.com/questions/19605150/regex-for-password-must-be-contain-at-least-8-characters-least-1-number-and-bot
function ValidatePassword(value) {
    var regex = new RegExp("^(?=.*[a - z])(?=.*[A - Z])(?=.*\d)(?=.*[$@$!%*?&])[A - Za - z\d$@$!%*?&]{ 8,}");
    return regex.test(value);
}

function IsEmailValidate(value) {
    var pattern = new RegExp(/^([\w-\.]+@([\w-]+\.)+[\w-]{2,4})?$/);
    return pattern.test(value);
}


function ValidateTime(value) {
    var regex = /^([0]?[1-9]|1[0-2]):([0-5]\d)\s?(AM|PM)?$/i;
    return regex.test(value);
}

function getPhoneType(container,container1) {
    container.html('');
    var custmorUri = '/api/dropdown/phonetype';
    ajaxHelper(custmorUri, 'GET', null, false).done(function (data) {
        container.append($('<option />', { value: '', text: 'Select' }));
        $.each(data, function (rowIndex, r) {
            container.append($('<option />', { value: r["Id"], text: r["Name"] }));
        });

        if (container1 && container1.length > 0) {
            container1.append($('<option />', { value: '', text: 'Select' }));
            $.each(data, function (rowIndex, r) {
                container1.append($('<option />', { value: r["Id"], text: r["Name"] }));
            });
        }
    });
    //container.val(valu);
}

function getTitle(container) {
    container.html('');
    var custmorUri = '/api/dropdown/title';
    ajaxHelper(custmorUri, 'GET', null, false).done(function (data) {
        container.append($('<option />', { value: '', text: 'Select' }));
        $.each(data, function (rowIndex, r) {
            container.append($('<option />', { value: r["Id"], text: r["Name"] }));
        });
    });

    //container.val(valu);
}


function getTitleForMulti(container1, container2) {
    var custmorUri = '/api/dropdown/title';
    ajaxHelper(custmorUri, 'GET', null, false).done(function (data) {

        container1.append($('<option />', { value: '', text: 'Select' }));

        if (container2.length > 0) {
            container2.append($('<option />', { value: '', text: 'Select' }));
        }

        $.each(data, function (rowIndex, r) {

            if (r.Description == "0" || r.Description == 0) {
                container1.append($('<option />', { value: r["Id"], text: r["Name"] }));
            } else if (r.Description == "1" || r.Description == 1) {
                if (container2.length > 0) {
                    container2.append($('<option />', { value: r["Id"], text: r["Name"] }));
                }
            }

        });
    });

    //container1.val(valu1);

    //if (container2.length > 0) {
    //    container2.val(valu2);
    //}
}

function getAlternativeTitle(container) {
    var custmorUri = '/api/dropdown/Alternativetitle';
    ajaxHelper(custmorUri, 'GET', null, false).done(function (data) {
        container.append($('<option />', { value: '', text: 'Select' }));
        $.each(data, function (rowIndex, r) {
            container.append($('<option />', { value: r["Id"], text: r["Name"] }));
        });
    });
    // container.val(valu);
}

function getSalesYear(container) {
    container.html('');
    var UserId = $('#UserId').val();

    if ($('#entityid').val() != $('#myentityid').val()) {
        UserId = $('#myid').val();
    }

    var custmorUri = '/api/Archive/SalesYears?Id=' + UserId;

    var i = 0;
    ajaxHelper(custmorUri, 'GET', null, false).done(function (data) {
        container.append($('<option />', { value: '', text: 'Select Sales Year' }));
        if (data != '' && data != null && data != undefined) {
            $.each(data, function (rowIndex, r) {
                container.append($('<option />', { value: r["Id"], text: r["SalesYear"] }));

                if (i == 0) {
                    if (r["Description"] != null && r["Description"] != '') {
                        $('#SalesYearGroupId').val(r["Description"]);
                    }
                    i = 1;
                }
            });
        }
    });
    // container.val(valu);
}


function getCountry(container1, container2) {
    container1.html('');
    container2.html('');
    var custmorUri = '/api/dropdown/country';
    ajaxHelper(custmorUri, 'GET').done(function (data) {
        container1.append($('<option />', { value: '', text: 'Select' }));
        $.each(data, function (rowIndex, r) {
            container1.append($('<option />', { value: r["Id"], text: r["Name"] }));
        });

        container2.append($('<option />', { value: '', text: 'Select' }));
        $.each(data, function (rowIndex, r) {
            container2.append($('<option />', { value: r["Id"], text: r["Name"] }));
        });
    });
}



function getState(container1, container2) {
    container1.html('');
    container2.html('');
    var custmorUri = '/api/dropdown/state';
    ajaxHelper(custmorUri, 'GET').done(function (data) {
        container1.append($('<option />', { value: '', text: 'Select' }));
        $.each(data, function (rowIndex, r) {
            container1.append($('<option />', { value: r["Id"], text: r["Name"] }));
        });

        container2.append($('<option />', { value: '', text: 'Select' }));
        $.each(data, function (rowIndex, r) {
            container2.append($('<option />', { value: r["Id"], text: r["Name"] }));
        });
    });
}



function getCity(container1, container2) {
    container1.html('');
    container2.html('');
    var custmorUri = '/api/dropdown/city';
    ajaxHelper(custmorUri, 'GET').done(function (data) {
        container1.append($('<option />', { value: '', text: 'Select' }));
        $.each(data, function (rowIndex, r) {
            container1.append($('<option />', { value: r["Id"], text: r["Name"] }));
        });

        container2.append($('<option />', { value: '', text: 'Select' }));
        $.each(data, function (rowIndex, r) {
            container2.append($('<option />', { value: r["Id"], text: r["Name"] }));
        });
    });
}


function getZipCode(container1, container2) {
    container1.html('');
    container2.html('');
    var custmorUri = '/api/dropdown/zipcode';
    ajaxHelper(custmorUri, 'GET').done(function (data) {
        container1.append($('<option />', { value: '', text: 'Select' }));
        $.each(data, function (rowIndex, r) {
            container1.append($('<option />', { value: r["Id"], text: r["Name"] }));
        });

        container2.append($('<option />', { value: '', text: 'Select' }));
        $.each(data, function (rowIndex, r) {
            container2.append($('<option />', { value: r["Id"], text: r["Name"] }));
        });
    });
}


function getAffiliate(container) {
    container.html('');
    var entityid = $('#entityid').val();
    var custmorUri = '/api/dropdown/affiliate?entityid=' + entityid;
    //var ul = $('<ul/>').addClass('list-unstyled')
    ajaxHelper(custmorUri, 'GET', null, false).done(function (data) {
        $.each(data, function (rowIndex, r) {
            var DocID = r["Description"];
            if (DocID != '' && DocID != null && DocID != undefined) {
                //var booksUri = '/api/Document/Download';
                //ajaxDownloader(booksUri + "?Id=" + DocID, 'GET').done(function (result) {
                //    // $("a#tcpathabcd").attr('href', EMPAdminWebAPI + "/" + data);
                var docpath = EMPAdminWebAPI + '/' + DocID;
                container.append('<label><input type="checkbox" Id="chk' + r["Id"] + '"  name="chk' + r["Id"] + '" class="chkAffiliate" value="' + r["Id"] + '" onclick="chkBelowinfo(this)" chkname="' + r["Name"] + '"> <span class="checkbox-material"><span class="check"></span></span> ' + r["Name"] + ' <a href="' + docpath + '" target="_blank" title="Download"> <i class="fa fa-arrow-circle-down"></i> </a> </label>');//<span class="checkbox-material"><span class="check"></span></span>

            }
            else {
                container.append('<label><input type="checkbox" Id="chk' + r["Id"] + '"  name="chk' + r["Id"] + '" class="chkAffiliate" value="' + r["Id"] + '" onclick="chkBelowinfo(this)"  chkname="' + r["Name"] + '"> <span class="checkbox-material"><span class="check"></span></span> ' + r["Name"] + '</label>'); //<span class="checkbox-material"><span class="check"></span></span>
            }
        });
        //container.append(ul);
    });
}


function getAffiliateArchive(container) {
    container.html('');
    var entityid = $('#entityid').val();
    if ($('#entityid').val() != $('#myentityid').val()) {
        entityid = $('#myentityid').val();
    }

    var custmorUri = '/api/dropdown/affiliate?entityid=' + entityid;
    //var ul = $('<ul/>').addClass('list-unstyled')
    ajaxHelper(custmorUri, 'GET', null, false).done(function (data) {
        $.each(data, function (rowIndex, r) {
            var DocID = r["Description"];
            if (DocID != '' && DocID != null && DocID != undefined) {
                //var booksUri = '/api/Document/Download';
                //ajaxDownloader(booksUri + "?Id=" + DocID, 'GET').done(function (result) {
                //    // $("a#tcpathabcd").attr('href', EMPAdminWebAPI + "/" + data);
                var docpath = EMPAdminWebAPI + '/' + DocID;
                container.append('<label><input type="checkbox" Id="chk' + r["Id"] + '"  name="chk' + r["Id"] + '" class="chkAffiliate" value="' + r["Id"] + '" onclick="chkBelowinfo(this)" chkname="' + r["Name"] + '"><span class="checkbox-material"><span class="check"></span></span> ' + r["Name"] + ' <a href="' + docpath + '" target="_blank" title="Download"> <i class="fa fa-arrow-circle-down"></i> </a> </label>');//<span class="checkbox-material"><span class="check"></span></span>

            }
            else {
                container.append('<label><input type="checkbox" Id="chk' + r["Id"] + '"  name="chk' + r["Id"] + '" class="chkAffiliate" value="' + r["Id"] + '" onclick="chkBelowinfo(this)"  chkname="' + r["Name"] + '"><span class="checkbox-material"><span class="check"></span></span> ' + r["Name"] + '</label>'); //
            }
        });
        //container.append(ul);
    });
}

function getAffiliateEnroll(container) {
    container.html('');
    var entityid = '';
    // var enrollentity = localStorage.getItem("EnrollEntityId");
    if ($('#entityid').val() != $('#myentityid').val()) {
        entityid = $('#myentityid').val();
    } else {
        entityid = $('#entityid').val();
    }
    var custmorUri = '/api/dropdown/affiliate?entityid=' + entityid;
    //var ul = $('<ul/>').addClass('list-unstyled')
    ajaxHelper(custmorUri, 'GET').done(function (data) {
        $.each(data, function (rowIndex, r) {
            var DocID = r["Description"];
            if (DocID != '' && DocID != null && DocID != undefined) {
                //var booksUri = '/api/Document/Download';
                //ajaxDownloader(booksUri + "?Id=" + DocID, 'GET').done(function (result) {
                //    // $("a#tcpathabcd").attr('href', EMPAdminWebAPI + "/" + data);
                var docpath = EMPAdminWebAPI + '/' + DocID;
                container.append('<label><input type="checkbox" Id="chk' + r["Id"] + '"  name="chk' + r["Id"] + '" class="chkAffiliate" value="' + r["Id"] + '" onclick="chkBelowinfo(this)" chkname="' + r["Name"] + '"> ' + r["Name"] + ' <a href="' + docpath + '" target="_blank" title="Download"> <i class="fa fa-arrow-circle-down"></i> </a> </label>');//<span class="checkbox-material"><span class="check"></span></span>

            }
            else {
                container.append('<label><input type="checkbox" Id="chk' + r["Id"] + '"  name="chk' + r["Id"] + '" class="chkAffiliate" value="' + r["Id"] + '" onclick="chkBelowinfo(this)"  chkname="' + r["Name"] + '"> ' + r["Name"] + '</label>'); //<span class="checkbox-material"><span class="check"></span></span>
            }
        });
        //container.append(ul);
    });
}



function getAffiliateEnrollSummary(container) {
    container.html('');
    var entityid = '';
    // var enrollentity = localStorage.getItem("EnrollEntityId");
    if ($('#entityid').val() != $('#myentityid').val()) {
        entityid = $('#myentityid').val();
    } else {
        entityid = $('#entityid').val();
    }


    var custmorUri = '/api/dropdown/affiliate?entityid=' + entityid;
    //var ul = $('<ul/>').addClass('list-unstyled')
    ajaxHelper(custmorUri, 'GET').done(function (data) {
        $.each(data, function (rowIndex, r) {
            var DocID = r["Description"];
            if (DocID != '' && DocID != null && DocID != undefined) {
                //var booksUri = '/api/Document/Download';
                //ajaxDownloader(booksUri + "?Id=" + DocID, 'GET').done(function (result) {
                //    // $("a#tcpathabcd").attr('href', EMPAdminWebAPI + "/" + data);
                var docpath = EMPAdminWebAPI + '/' + DocID;
                container.append('<label><input type="checkbox" Id="chk' + r["Id"] + '"  name="chk' + r["Id"] + '" class="chkAffiliate" value="' + r["Id"] + '" onclick="chkBelowinfo(this)" chkname="' + r["Name"] + '"> ' + r["Name"] + ' <a href="' + docpath + '" target="_blank" title="Download"> <i class="fa fa-arrow-circle-down"></i> </a> </label>');//<span class="checkbox-material"><span class="check"></span></span>

            }
            else {
                container.append('<label><input type="checkbox" Id="chk' + r["Id"] + '"  name="chk' + r["Id"] + '" class="chkAffiliate" value="' + r["Id"] + '" onclick="chkBelowinfo(this)"  chkname="' + r["Name"] + '"> ' + r["Name"] + '</label>'); //<span class="checkbox-material"><span class="check"></span></span>
            }
        });
        //container.append(ul);
    });
}


function getAffiliateEnroll_Sub(container) {
    container.html('');
    var entityid = '';
    // var enrollentity = localStorage.getItem("EnrollEntityId");
    var UserId = 0;
    if ($('#entityid').val() != $('#myentityid').val()) {
        entityid = $('#myentityid').val();
        UserId = $('#myid').val();

    } else {
        entityid = $('#entityid').val();
        UserId = $('#UserId').val();
    }

    var custmorUri = '/api/dropdown/affiliate_Sub?entityid=' + entityid + '&CustomerId=' + UserId;
    //var ul = $('<ul/>').addClass('list-unstyled')
    ajaxHelper(custmorUri, 'GET').done(function (data) {
        $.each(data, function (rowIndex, r) {
            var DocID = r["Description"];
            if (DocID != '' && DocID != null && DocID != undefined) {
                //var booksUri = '/api/Document/Download';
                //ajaxDownloader(booksUri + "?Id=" + DocID, 'GET').done(function (result) {
                //    // $("a#tcpathabcd").attr('href', EMPAdminWebAPI + "/" + data);
                var docpath = EMPAdminWebAPI + '/' + DocID;
                container.append('<label><input type="checkbox" Id="chkAE' + r["Id"] + '"  name="chkAE' + r["Id"] + '" class="chkAEAffiliate" value="' + r["Id"] + '" onclick="chkBelowinfo(this)" chkname="' + r["Name"] + '"> <span class="checkbox-material"><span class="check"></span></span> ' + r["Name"] + ' <a href="' + docpath + '" target="_blank" title="Download"> <i class="fa fa-arrow-circle-down"></i> </a> </label>');//<span class="checkbox-material"><span class="check"></span></span>

            }
            else {
                container.append('<label><input type="checkbox" Id="chkAE' + r["Id"] + '"  name="chkAE' + r["Id"] + '" class="chkAEAffiliate" value="' + r["Id"] + '" onclick="chkBelowinfo(this)"  chkname="' + r["Name"] + '"><span class="checkbox-material"><span class="check"></span></span> ' + r["Name"] + '</label>'); //<span class="checkbox-material"><span class="check"></span></span>
            }

            fnGetEnrollAffiliateConfig(UserId);
        });
        //container.append(ul);
    });
}

function getAffiliateEnrollSummary_Sub(container) {
    container.html('');
    var entityid = '';
    // var enrollentity = localStorage.getItem("EnrollEntityId");
    var UserId = 0;
    if ($('#entityid').val() != $('#myentityid').val()) {
        entityid = $('#myentityid').val();
        UserId = $('#myid').val();

    } else {
        entityid = $('#entityid').val();
        UserId = $('#UserId').val();
    }


    var custmorUri = '/api/dropdown/affiliate_sub?entityid=' + entityid + '&CustomerId=' + UserId;
    //var ul = $('<ul/>').addClass('list-unstyled')
    ajaxHelper(custmorUri, 'GET').done(function (data) {
        $.each(data, function (rowIndex, r) {
            var DocID = r["Description"];
            if (DocID != '' && DocID != null && DocID != undefined) {
                //var booksUri = '/api/Document/Download';
                //ajaxDownloader(booksUri + "?Id=" + DocID, 'GET').done(function (result) {
                //    // $("a#tcpathabcd").attr('href', EMPAdminWebAPI + "/" + data);
                var docpath = EMPAdminWebAPI + '/' + DocID;
                container.append('<label><input type="checkbox" Id="chkAE' + r["Id"] + '"  name="chkAE' + r["Id"] + '" class="chkAEAffiliate" value="' + r["Id"] + '" onclick="chkBelowinfo(this)" chkname="' + r["Name"] + '"><span class="checkbox-material"><span class="check"></span></span> ' + r["Name"] + ' <a href="' + docpath + '" target="_blank" title="Download"> <i class="fa fa-arrow-circle-down"></i> </a> </label>');//<span class="checkbox-material"><span class="check"></span></span>

            }
            else {
                container.append('<label><input type="checkbox" Id="chkAE' + r["Id"] + '"  name="chkAE' + r["Id"] + '" class="chkAEAffiliate" value="' + r["Id"] + '" onclick="chkBelowinfo(this)"  chkname="' + r["Name"] + '"><span class="checkbox-material"><span class="check"></span></span> ' + r["Name"] + '</label>'); //<span class="checkbox-material"><span class="check"></span></span>
            }
        });
        //container.append(ul);
    });
}

function getArchiveAffiliateEnrollment(container, customerid, entityid) {
    container.html('');
    var custmorUri = '/api/Archive/AffiliateEnrollment?entityid=' + entityid + '&customerid=' + customerid;
    //var ul = $('<ul/>').addClass('list-unstyled')
    ajaxHelper(custmorUri, 'GET', null, false).done(function (data) {
        $.each(data, function (rowIndex, r) {
            var DocID = r["Description"];
            if (DocID != '' && DocID != null && DocID != undefined) {
                //var booksUri = '/api/Document/Download';
                //ajaxDownloader(booksUri + "?Id=" + DocID, 'GET').done(function (result) {
                //    // $("a#tcpathabcd").attr('href', EMPAdminWebAPI + "/" + data);
                var docpath = EMPAdminWebAPI + '/' + DocID;
                container.append('<label><input type="checkbox" Id="chkAE' + r["Id"] + '"  name="chkAE' + r["Id"] + '" class="chkAEAffiliate" value="' + r["Id"] + '" onclick="chkBelowinfo(this)" chkname="' + r["Name"] + '"><span class="checkbox-material"><span class="check"></span></span> ' + r["Name"] + ' <a href="' + docpath + '" target="_blank" title="Download"> <i class="fa fa-arrow-circle-down"></i> </a> </label>');//<span class="checkbox-material"><span class="check"></span></span>

            }
            else {
                container.append('<label><input type="checkbox" Id="chkAE' + r["Id"] + '"  name="chkAE' + r["Id"] + '" class="chkAEAffiliate" value="' + r["Id"] + '" onclick="chkBelowinfo(this)"  chkname="' + r["Name"] + '"><span class="checkbox-material"><span class="check"></span></span> ' + r["Name"] + '</label>'); //<span class="checkbox-material"><span class="check"></span></span>
            }
        });
        //container.append(ul);
    });
}

//function getDocument(Id) {
//    var booksUri = '/api/Document/Download';
//    ajaxHelper(booksUri + "?Id=" + Id, 'GET').done(function (data) {
//        $("a#tcpathabcd").attr('href', EMPAdminWebAPI + "/" + data);
//    });
//}

function getBank(container) {
    container.html('');
    var entityid = $('#entityid').val();

    var custmorUri = '/api/dropdown/bank?entityid=' + entityid;
    //  var ul = $('<ul/>').addClass('list-unstyled')
    ajaxHelper(custmorUri, 'GET').done(function (data) {
        $.each(data, function (rowIndex, r) {

            var DocID = r["Description"];
            if (DocID != '' && DocID != null && DocID != undefined) {
                var docpath = EMPAdminWebAPI + '/' + DocID;
                container.append('<label><input type="checkbox" Id="chk' + r["Id"] + '"  name="chk' + r["Id"] + '" class="chkBank" value="' + r["Id"] + '" onchange="getBankSubQuestion(this)"  bankname="' + r["Name"] + '" bankid="' + r["Id"] + '"><span class="checkbox-material"></span> ' + r["Name"] + '</label> <div class="form-group" id="div' + r["Id"] + '">');
            } else {
                container.append('<label><input type="checkbox" Id="chk' + r["Id"] + '"  name="chk' + r["Id"] + '" class="chkBank" value="' + r["Id"] + '" onchange="getBankSubQuestion(this)"  bankname="' + r["Name"] + '" bankid="' + r["Id"] + '"><span class="checkbox-material"></span> ' + r["Name"] + '</label> <div class="form-group" id="div' + r["Id"] + '">');
            }
        });
        // container.append(ul);
    });

}

function getBankTexts(container1, container2) {
    container1.html('');
    container2.html('');
    var entityid = $('#entityid').val();
    var custmorUri = '/api/dropdown/bank?entityid=' + entityid;
    ajaxHelper(custmorUri, 'GET').done(function (data) {
        $.each(data, function (rowIndex, r) {
            // container.append('<label><input type="checkbox" Id="chk' + r["Id"] + '"  name="chk' + r["Id"] + '" class="chkBank" value="' + r["Id"] + '" onchange="getBankSubQuestion(this)"  bankname="' + r["Name"] + '"><span class="checkbox-material"></span> ' + r["Name"] + '</label> <div class="form-group" id="div' + r["Id"] + '">');
            container1.append('<div class="col-md-4"><div class="form-group is-empty"><label>' + r["Name"] + '</label><input id="input_' + r["Id"] + '" class="form-control bankservice digit" type="text" bankid="' + r["Id"] + '" maxlength="8"></div></div>');
            container2.append('<div class="col-md-4"><div class="form-group is-empty"><label>' + r["Name"] + '</label><input id="input_trns_' + r["Id"] + '" class="form-control banktrans digit" type="text" bankid="' + r["Id"] + '" maxlength="8"></div></div>');
        });
    });
}

function getBankSubQuestion(obj) {

    var bankid = $(obj).attr('value');
    var name = $(obj).attr('bankname');
    getSubQuestions(bankid, name);
}


function getFees(container1, container2) {

    container1.html('');
    container2.html('');
    // var entityid = $('#entityid').val();
    var userid = $('#UserId').val();
    var entityid = $('#entityid').val();
    if ($('#entityid').val() != $('#myentityid').val()) {
        entityid = $('#myentityid').val();
        userid = $('#myid').val();
    }

    var custmorUri = '/api/dropdown/Fees?entityid=' + entityid + '&userid=' + userid;

    container1.append('<thead><tr><th>Bank Product Fees</th> <th>Amount</th></tr></thead><tbody>');
    container2.append('<thead><tr><th>e-File Fees Amount</th> <th>Amount</th></tr></thead><tbody>');

    var TotalBankProductFees = 0;
    var TotaleFileFees = 0;

    ajaxHelper(custmorUri, 'GET', null, false).done(function (data) {
        $.each(data, function (rowIndex, r) {
            if (r["FeeCategoryID"] == '1') {
                if (r["IsEdit"] == true) {
                    container1.append('<tr><td>' + r["Name"] + '</td><td><span>$</span> <span class="hdnBankFee padding-left-3" feefor="' + r["FeeFor"] + '"  id="cst-amount_' + r["Id"] + '" value="' + r["Amount"] + '" >' + r["Amount"] + '</span>  <div class="cst-edit"><div class="col-md-6"><input type="text" id="input_' + r["Id"] + '" value=' + r["Amount"] + ' class="form-control decimal" maxlength="8"></div><span class="cst-option"><a href="#" class="bankfee" onclick="SaveTransmetterFee(this)" id="' + r["Id"] + '" ><i class="fa fa-check" aria-hidden="true"></i></a><i class="fa fa-times cst-close" aria-hidden="true"  onclick="fn_close(this)" id="close_' + r["Id"] + '" closeid="' + r["Id"] + '" ></i></span></div><i class="fa fa-pencil-square-o cst-edit-btn" aria-hidden="true" onclick="editclick(this)" id="edit_' + r["Id"] + '" editid="' + r["Id"] + '"></i></td></tr>');
                    //TotalBankProductFees = TotalBankProductFees + Number(r["Amount"]);
                }
                else {
                    container1.append('<tr><td>' + r["Name"] + '</td><td><span>$</span> <span class="hdnBankFee padding-left-3" feefor="' + r["FeeFor"] + '">' + r["Amount"] + '</span></td></tr>');
                }

            }
            else {
                if (r["IsEdit"] == true) {
                    container2.append('<tr><td>' + r["Name"] + '</td><td>span>$</span> <span class="hdnTranFee" feefor="' + r["FeeFor"] + '" id="cst-amount_' + r["Id"] + '">' + r["Amount"] + '</span>  <div class="cst-edit"><div class="col-md-6"><input type="text" id="input_' + r["Id"] + '" value=' + r["Amount"] + ' class="form-control decimal" maxlength="8"></div><span class="cst-option"><a href="#" class="tranfee" onclick="SaveTransmetterFee(this)" id="' + r["Id"] + '"><i class="fa fa-check" aria-hidden="true"></i></a><i class="fa fa-times cst-close" aria-hidden="true"  onclick="fn_close(this)" id="close_' + r["Id"] + '" closeid="' + r["Id"] + '" ></i></span></div><i class="fa fa-pencil-square-o cst-edit-btn" aria-hidden="true" onclick="editclick(this)" id="edit_' + r["Id"] + '" editid="' + r["Id"] + '"></i></td></tr>');
                    //TotaleFileFees = TotaleFileFees + Number(r["Amount"]);
                }
                else {
                    container2.append('<tr><td>' + r["Name"] + '</td><td><span>$</span> <span class="hdnTranFee" feefor="' + r["FeeFor"] + '">' + r["Amount"] + '</span> </td></tr>');
                    //TotaleFileFees = TotaleFileFees + Number(r["Amount"]);
                }
            }

            if (r["FeeFor"] == '2') {
                TotalBankProductFees = TotalBankProductFees + Number(r["Amount"]);
            }

            if (r["FeeFor"] == '3') {
                TotaleFileFees = TotaleFileFees + Number(r["Amount"]);
            }

        });

        $('#TotalBankProductFees').val(TotalBankProductFees);
        $('#TotaleFileFees').val(TotaleFileFees);

        container1.append('</tbody>');
        container2.append('</tbody>');
    });
}

$(function () {
    $('input[type=number].digit, input[type=text].digit').keypress(function (event) {
        if (event.which != 99 && event.which != 8 && event.which != 0 && isNaN(String.fromCharCode(event.which))) {
            event.preventDefault(); //stop characters from entering input
        }
    });

    $('input[type=text].alphanumaric').keypress(function (event) {

        var arr = [0, 8, 9, 32, 46];

        for (var i = 65; i <= 90; i++) {
            arr.push(i);
        }

        for (var i = 96; i <= 123; i++) {
            arr.push(i);
        }

        for (var i = 48; i <= 57; i++) {
            arr.push(i);
        }

        if (jQuery.inArray(event.which, arr) === -1) {
            event.preventDefault();
        }
    });

    $('input[type=text].alpha').keypress(function (event) {
        var arr = [0, 8, 9, 32, 46];
        for (var i = 65; i <= 90; i++) {
            arr.push(i);
        }
        for (var i = 96; i <= 123; i++) {
            arr.push(i);
        }
        if (jQuery.inArray(event.which, arr) === -1) {
            event.preventDefault();
        }
    });

    $('input[type=number].decimal, input[type=text].decimal').keypress(function (event) {
        if (event.which != 99 && event.which != 8 && event.which != 0 && event.which != 46 && isNaN(String.fromCharCode(event.which))) {
            event.preventDefault(); //stop characters from entering input
        }

        var dotIndx = $(this).val().indexOf('.');
        if (Number(dotIndx) > 0 && event.which == 46) {
            event.preventDefault();
        }
    });
});


function getTimeZones(container) {
    //container.html('');
    //container.append($('<option />', { value: '', text: 'Select' }));

    //$.getJSON("/Scripts/app/timezonesUS.json", function (data) {
    //    // var mydata = JSON.parse(data);
    //    //var items = [];
    //    //$.each(data, function (key, val) {
    //    //    items.push("<li id='" + key + "'>" + val + "</li>");
    //    //});

    //    //$("<ul/>", {
    //    //    "class": "my-new-list",
    //    //    html: items.join("")
    //    //}).appendTo("body");

    //    $.each(data, function (rowIndex, r) {

    //        if (r["value"] == 'Eastern Standard Time') {
    //            container.append($('<option />', { value: r["text"], text: r["value"] }));
    //        }
    //        if (r["value"] == 'Central Standard Time') {
    //            container.append($('<option />', { value: r["text"], text: r["value"] }));
    //        }
    //        if (r["value"] == 'Mountain Standard Time') {
    //            container.append($('<option />', { value: r["text"], text: r["value"] }));
    //        }
    //        if (r["value"] == 'Pacific Standard Time') {
    //            container.append($('<option />', { value: r["text"], text: r["value"] }));
    //        }

    //    });
    //});
}



function getSubQuestions(bankid, name) {
    var container = $('#div' + bankid);
    container.html('');
    var entityid = $('#entityid').val();
    var custmorUri = '/api/dropdown/banksubquestion?bankid=' + bankid;
    var $head = $('<div/>').addClass('form-group').attr('id', 'divBankQuestions' + bankid);
    if ($('#' + bankid).is(':checked')) {
        // var ul = $('<ul/>').addClass('list-unstyled')
        $head.append('<p>How will your Sub-sites print their ' + name + ' Checks?</p>');//  container.append('<div/>').addClass('form-group').attr('id', 'divBankQuestions'+bankid);// class="form-group" id="divBankQuestions"><p>How will your Sub-sites print their ' + name + ' Checks?</p>');
        var IsDataExist = false;
        ajaxHelper(custmorUri, 'GET').done(function (data) {
            $.each(data, function (rowIndex, r) {
                IsDataExist = true;
                var DocID = r["DocumentPath"];
                if (DocID != '' && DocID != null && DocID != undefined) {
                    var docpath = EMPAdminWebAPI + '/' + DocID;
                }
                //container.append('<div class="radio"><label><input type="radio" Id="chk' + r["Id"] + '"  name="chkBankQuestion' + bankid + '" class="chkBankQuestion" value="' + r["Id"] + '" checked=""><span class="circle"></span><span class="check"></span> ' + r["Name"] + '</label></div>');
                $head.append('<div class="radio"><label><input type="radio" Id="chk' + r["Id"] + '"  name="bank' + bankid + '" class="rbBank' + bankid + '" value="' + r["Id"] + '" checked=""><span class="circle"></span><span class="check"></span> ' + r["Name"] + '</label></div>');
                // ul.append($('<li><input type="radio" Id="chk' + r["Id"] + '" name="chkBankQuestion' + bankid + '" class="chkBankQuestion" value="' + r["Id"] + '" /> <span>' + r["Name"] + '</span> </li>'));
            });

            if (IsDataExist) {
                container.append($head);
            }
        });
    }
}



function getBankAndQuestions(container) {
    container.html('');
    //  var container = $('#div' + bankid);
    //   container.html('');
    var entityid = $('#entityid').val();
    if ($('#entityid').val() != $('#myentityid').val()) {
        entityid = $('#myentityid').val();
    }

    var custmorUri = '/api/dropdown/banksubquestion?entityid=' + entityid;
    var $table = $('<table/>').addClass('table table-striped table-hover table-bordered');//; <table class="table table-striped table-hover table-bordered">
    var $tbody = $('<tbody />');
    var IsDataExist = false;
    ajaxHelper(custmorUri, 'GET', null, false).done(function (data) {

        if (data != null && data != '' && data != undefined) {

            $.each(data, function (colIndex, c) {
                var bankid = c["BankId"];
                var bankname = c["BankName"];
                var DocID = c["DocumentPath"];
                var $tr = $('<tr/>');
                var $td1 = $('<td/>');

                if (DocID != '' && DocID != null && DocID != undefined) {
                    var docpath = EMPAdminWebAPI + '/' + DocID;
                    $td1.append('<div class="form-group"><div class="checkbox"><label><input type="checkbox" Id="chk' + bankid + '"  name="chk' + bankid + '" class="chkBank" value="' + bankid + '" onchange="getBankQuestionsShow(this)"  bankname="' + bankname + '" bankid="' + bankid + '"> <span class="checkbox-material"><span class="check"></span></span> ' + bankname + ' <a href="' + docpath + '" target="_blank" title="Download"> <i class="fa fa-arrow-circle-down"></i> </a></label></div></div>'); //<span class="checkbox-material"><span class="check"></span></span>
                } else {
                    $td1.append('<div class="form-group"><div class="checkbox"><label><input type="checkbox" Id="chk' + bankid + '"  name="chk' + bankid + '" class="chkBank" value="' + bankid + '" onchange="getBankQuestionsShow(this)"  bankname="' + bankname + '" bankid="' + bankid + '"> <span class="checkbox-material"><span class="check"></span></span> ' + bankname + ' </label></div></div>'); //<span class="checkbox-material"><span class="check"></span></span>
                }

                //  $tr.html('<td><div class="form-group bank-details"><label>How will your Sub-sites print their SBH Checks?</label><div class="radio"><label><input type="radio" id="IsuTaxPortalEnrollment1" name="IsuTaxPortalEnrollment" value="Yes" checked=""> All Sub-sites will print checks online from SBH website </label> </div>');

                // container.append('<label><input type="checkbox" Id="chk' + bankid + '"  name="chk' + bankid + '" class="chkBank" value="' + bankid + '" onchange="getBankQuestionsShow(this)"  bankname="' + bankname + '" bankid="' + bankid + '"><span class="checkbox-material"></span> ' + bankname + '</label>');
                var $td2 = $('<td/>')
                var $head = $('<div/>').addClass('form-group bank-details');
                if (c.Questions.length > 0) {
                    $head.attr('id', 'divBankQuestions' + bankid).attr('style', 'display:none');
                    $head.append('<label>How will your Sub-sites print their ' + bankname + ' Checks?</label>');//  container.append('<div/>').addClass('form-group').attr('id', 'divBankQuestions'+bankid);// class="form-group" id="divBankQuestions"><p>How will your Sub-sites print their ' + name + ' Checks?</p>');
                }

                $.each(c.Questions, function (rowIndex, r) {
                    IsDataExist = true;
                    $head.append('<div class="radio"><label><input type="radio" Id="chk' + r["Id"] + '"  name="bank' + bankid + '" class="rbBank' + bankid + '" value="' + r["Id"] + '" checked="" /><span class="circle"></span><span class="check"></span> ' + r["Name"] + '</label></div>');
                });

                if (IsDataExist) {
                    $td2.append($head);
                }
                $tr.append($td1);
                $tr.append($td2);
                $tbody.append($tr);
                //container.append('<label><input type="checkbox" Id="chk' + bankid + '"  name="chk' + bankid + '" class="chkBank" value="' + bankid + '" onchange="getBankQuestionsShow(this)"  bankname="' + bankname + '" bankid="' + bankid + '"><span class="checkbox-material"></span> ' + bankname + '</label>');
                //var $head = $('<div/>').addClass('form-group');
                //if (c.Questions.length > 0) {
                //    $head.attr('id', 'divBankQuestions' + bankid).attr('style', 'display:none');
                //    $head.append('<p>How will your Sub-sites print their ' + bankname + ' Checks?</p>');//  container.append('<div/>').addClass('form-group').attr('id', 'divBankQuestions'+bankid);// class="form-group" id="divBankQuestions"><p>How will your Sub-sites print their ' + name + ' Checks?</p>');
                //}

                //$.each(c.Questions, function (rowIndex, r) {
                //    IsDataExist = true;
                //    $head.append('<div class="radio"><label><input type="radio" Id="chk' + r["Id"] + '"  name="bank' + bankid + '" class="rbBank' + bankid + '" value="' + r["Id"] + '" checked=""><span class="circle"></span><span class="check"></span> ' + r["Name"] + '</label></div>');
                //});

                //if (IsDataExist) {
                //    container.append($head);
                //}
            });


            $table.append($tbody);
            container.append($table);
        }
    });
}

function getBankQuestionsShow(obj) {
    var bankid = $(obj).attr('value');
    var name = $(obj).attr('bankname');
    $('#divBankQuestions' + bankid).hide();
    if ($('#chk' + bankid).is(':checked'))
        $('#divBankQuestions' + bankid).show();
}


//var ConfigStatus = [];
function getConfigStatus() {

    //var ConfigStatus = [];
    //var UserId = $('#UserId').val();
    //var entityid = $('#entityid').val();

    //if ($('#entityid').val() != $('#myentityid').val()) {

    //    //$('a#btnNextFeeReim')
    //    //                 .attr('disabled', 'disabled')
    //    //                 .css('pointer-events', 'none');

    //    entityid = $('#myentityid').val();
    //    UserId = $('#myid').val();
    //}

    ////$('a#btnMainFeeSetupSaveNext')
    ////              .attr('disabled', 'disabled')
    ////              .css('pointer-events', 'none');

    ////var Cid = getUrlVars()["Id"];
    ////if (Cid) {
    ////    UserId = Cid;
    ////}
    ////var entitydisplayid = $('#entitydisplayid').val();


    //var Uri = '/api/Configuration';
    //var html = $('<ul/>');//.addClass('sub-menu');
    //var status = 'active';
    //var IsDataExist = false;

    //ajaxHelper(Uri + '?id=' + UserId, 'GET',null,false).done(function (data) {
    //    if (data != null && data != '' && data != undefined) {
    //        IsDataExist = true;
    //        $.each(data, function (rowIndex, r) {
    //            //ConfigStatus.push({
    //            //    SitemapId: r["SitemapId"],
    //            //    Status: r["Status"]
    //            //});
    //            ConfigStatus.push({
    //                SitemapId: r["SitemapId"],
    //                Status: r["Status"]
    //            });

    //            if (r["Status"] == 'done') {
    //                status = 'done';//$('#formid').val();
    //            }

    //            if (r["Status"] == 'none') {
    //                status = 'done';//$('#formid').val();
    //            }

    //            var formname = 'site' + r["SitemapId"];

    //            $('a.vmenu')
    //                .removeClass('selected')
    //            //    .removeClass('done');

    //            $('a#' + formname)
    //                .addClass(status);

    //            var formid = $('#formid').attr('formid');

    //            if (formid != formname) {
    //                $('a#' + formid)
    //                     .addClass("selected");
    //            } else {
    //                $('a#' + formname)
    //                    .addClass("selected");
    //            }
    //        });
    //    }
    //});

    //if (!IsDataExist) {

    //    $('a.vmenu')
    //              .removeClass('selected');
    //    // .removeClass('selected');

    //    var formid = $('#formid').attr('formid');
    //    $('a#' + formid)
    //         .addClass('selected');
    //    //.addClass('selected');
    //}
    //var n = 0;
    ////  $('#ActiveMyAccountStatus').val('0');
    //var one = 0, two = 0, three = 0, four = 0, five = 0;
    //var _enrollsubmitted = false;
    //var ActiveMyAccount = 0;
    //if (ConfigStatus.length > 0) {
    //    //var enrollstatus = ConfigStatus.filter(function (i) { return i.SitemapId == '0feeb0fe-d0e7-4370-8733-dd5f7d2041fc' && i.Status == 'done' });
    //    //if (enrollstatus.length <= 0) {
    //    //    $('#site98a706d7-031f-4c5d-8cc4-d32cc7658b69').hide();
    //    //}

    //    if (entityid == $('#Entity_SO').val() || entityid == $('#Entity_SOME').val() || entityid == $('#Entity_SOME_SS').val()) {
    //        var dashboardstatus = ConfigStatus.filter(function (i) { return i.SitemapId == '7c8aa474-2535-4f69-a2ae-c3794887f92d' && i.Status == 'done' });
    //        if (dashboardstatus.length <= 0 && entityid != $('#Entity_uTax').val()) {
    //            $('#site7c8aa474-2535-4f69-a2ae-c3794887f92d').text('Main Office Information');
    //        }
    //    }
    //    else {
    //        var dashboardstatus = ConfigStatus.filter(function (i) { return i.SitemapId == '98a706d7-031f-4c5d-8cc4-d32cc7658b63' && i.Status == 'done' });
    //        if (dashboardstatus.length <= 0 && entityid != $('#Entity_uTax').val()) {
    //            $('#site98a706d7-031f-4c5d-8cc4-d32cc7658b63').text('Main Office Information');
    //        }
    //    }

    //    if (entityid == $('#Entity_MO').val() || entityid == $('#Entity_SVB').val() || entityid == $('#Entity_uTax').val()) {
    //        for (var i = 0, len = ConfigStatus.length; i < len; i++) {
    //            if (ConfigStatus[i].SitemapId == "0eda5d25-591c-4e01-a845-fb580572ade8" && ConfigStatus[i].Status == "done") { //Main office Config.
    //                one = 1;
    //            }
    //            if (ConfigStatus[i].SitemapId == "68882c05-5914-4fdb-b284-e33d6c029f5a" && ConfigStatus[i].Status == "done") { //Sub - Site Config.
    //                two = 1;
    //            }
    //            if (ConfigStatus[i].SitemapId == "c81dddc4-4654-4775-a5cd-74efa99dfc90" && ConfigStatus[i].Status == "done") { //Fee Setup Config.
    //                three = 1;
    //            }
    //            if (ConfigStatus[i].SitemapId == "60025459-7568-4a77-b152-f81904aaaa63" && ConfigStatus[i].Status == "done") { // Service Transmi 
    //                four = 1;
    //            }

    //            if (ConfigStatus[i].SitemapId == "4fc65d1b-675f-4985-8022-9e8bd0ed735f" && ConfigStatus[i].Status == "done") { // Service Transmi 
    //                ActiveMyAccount = 1; 
    //                $('#ActiveMyAccountStatus').val('1');
    //            }
    //        }

    //        $('a#btnNextFeeReim')
    //                  .removeAttr('disabled')
    //                  .css('pointer-events', '');


    //        if ($('#site60025459-7568-4a77-b152-f81904aaaa63').length == 0) {
    //            four = 1;
    //        }

    //        n = Number(one) + Number(two) + Number(three) + Number(four) + Number(five);
    //        var ReimNextLink = Number(one) + Number(two) + Number(three);

    //        if (ActiveMyAccount == 0) {

    //            if (Number(ReimNextLink) < 3) {

    //                $('a#btnNextFeeReim')
    //                .attr('disabled', 'disabled')
    //                .css('pointer-events', 'none');

    //                $('a#btnSaveNextFeeReim')
    //               .attr('disabled', 'disabled')
    //               .css('pointer-events', 'none');


    //            }

    //            var FeeSetupNo = Number(one) + Number(two);
    //            if (Number(FeeSetupNo) < 2) {
    //                $('a#btnMainFeeSetupSaveNext')
    //                 .attr('disabled', 'disabled')
    //                 .css('pointer-events', 'none');
    //            } else {
    //                $('a#btnMainFeeSetupSaveNext')
    //                  .removeAttr('disabled')
    //                  .css('pointer-events', '');
    //            }

    //            if (n < 4) {
    //                $('a#btnMainActiveAccount')
    //                 .attr('disabled', 'disabled')
    //                 .css('pointer-events', 'none')
    //                 .attr('title', 'please save all configuration');

    //                $('div#divMainActiveAccount')
    //                    .addClass('activate-note')
    //                    .html('Note: please save all configuration');
    //            }


    //        }


    //        $('#site4fc65d1b-675f-4985-8022-9e8bd0ed735f').hide();
    //        if (ActiveMyAccount == 0) {
    //            if (Number(n) >= 4) {
    //                $('#site4fc65d1b-675f-4985-8022-9e8bd0ed735f').show();

    //                if ($('#entityid').val() != $('#myentityid').val()) {
    //                    $('#site4fc65d1b-675f-4985-8022-9e8bd0ed735f')
    //                        .attr('href', '/Configuration/ActivateInformation?Id=' + $('#myid').val() + '&entitydisplayid=' + $('#myentitydisplayid').val() + '&ptype=config' + '&entityid=' + $('#myentityid').val())
    //                        .removeClass('done');
    //                } else {
    //                    $('#site4fc65d1b-675f-4985-8022-9e8bd0ed735f')
    //                        .attr('href', '/Configuration/ActivateInformation')
    //                        .removeClass('done');
    //                }
    //            }
    //        }
    //    }
    //    else {
    //        for (var i = 0, len = ConfigStatus.length; i < len; i++) {

    //            if (ConfigStatus[i].SitemapId == "0eda5d25-591c-4e01-a845-fb580572ade8" && ConfigStatus[i].Status == "done") {
    //                n = n + 1;
    //            }

    //            if (ConfigStatus[i].SitemapId == "c81dddc4-4654-4775-a5cd-74efa99dfc90" && ConfigStatus[i].Status == "done") {
    //                n = n + 1;
    //            }

    //            if (ConfigStatus[i].SitemapId == "4fc65d1b-675f-4985-8022-9e8bd0ed735f" && ConfigStatus[i].Status == "done") { // Service Transmi 
    //                ActiveMyAccount = 1;
    //            }
    //        }
    //        $('#site4fc65d1b-675f-4985-8022-9e8bd0ed735f').hide();
    //        if (ActiveMyAccount == 0) {
    //            if (Number(n) >= 2) {
    //                $('#site4fc65d1b-675f-4985-8022-9e8bd0ed735f').show();
    //                if ($('#entityid').val() != $('#myentityid').val()) {
    //                    $('#site4fc65d1b-675f-4985-8022-9e8bd0ed735f')
    //                        .attr('href', '/Configuration/ActivateInformation?Id=' + $('#myid').val() + '&entitydisplayid=' + $('#myentitydisplayid').val() + '&ptype=config' + '&entityid=' + $('#myentityid').val())
    //                        .removeClass('done');
    //                } else {
    //                    $('#site4fc65d1b-675f-4985-8022-9e8bd0ed735f')
    //                        .attr('href', '/Configuration/ActivateInformation')
    //                        .removeClass('done');
    //                }
    //            }
    //        }
    //    }

    //}
    //else {
    //    $('#site4fc65d1b-675f-4985-8022-9e8bd0ed735f').hide();
    //    $('#site98a706d7-031f-4c5d-8cc4-d32cc7658b69').hide();
    //    if (entityid != '1') {
    //        if (entityid == $('#Entity_SO').val() || entityid == $('#Entity_SOME').val() || entityid == $('#Entity_SOME_SS').val())
    //            $('#site7c8aa474-2535-4f69-a2ae-c3794887f92d').text('Main Office Information');
    //        else
    //            $('#site98a706d7-031f-4c5d-8cc4-d32cc7658b63').text('Main Office Information');
    //    }
    //}

    //var sitelength = $("#site60025459-7568-4a77-b152-f81904aaaa63").length;
    //if (sitelength <= 0) {
    //    $('#dvSbTfAddon').hide();
    //    $('#liSVBAndTransFeeAddOn').hide();
    //    if (Number(n) >= 4) {
    //        $('#site4fc65d1b-675f-4985-8022-9e8bd0ed735f').show();

    //        if ($('#entityid').val() != $('#myentityid').val()) {

    //            $('#site4fc65d1b-675f-4985-8022-9e8bd0ed735f').attr('href', '/Configuration/ActivateInformation?Id=' + $('#myid').val() + '&entitydisplayid=' + $('#myentitydisplayid').val() + '&ptype=config' + '&entityid=' + $('#myentityid').val());
    //        } else {
    //            $('#site4fc65d1b-675f-4985-8022-9e8bd0ed735f').attr('href', '/Configuration/ActivateInformation');
    //        }
    //    }
    //}

    //EnrollmentMenuStatus(ConfigStatus);
}

var SiteMaps = [];
function getSitemap(container) {
    container.html('');
    if (SiteMaps.length == 0) {
        var EntityId = $('#entityid').val();
        var entitydisplayid = $('#entitydisplayid').val();
        var UserId = $('#UserId').val();
        var Type = '';
        var SiteParentId = '00000000-0000-0000-0000-000000000000';
        var BankId = '00000000-0000-0000-0000-000000000000';

        var Cid = getUrlVars()["Id"];
        if (Cid && $('#myentitydisplayid').length > 0) {
            UserId = Cid;
            entitydisplayid = $('#myentitydisplayid').val();
            EntityId = $('#myentityid').val();
            Type = getUrlVars()["ptype"];
            SiteParentId = getUrlVars()["ParentId"];;
        }

        var IsBank = getUrlVars()["bankid"];
        if (IsBank)
            BankId = IsBank;

        var IsAdded = false;
        var entityUri = '/api/Sitemap/Main?entityid=' + EntityId + '&UserId=' + UserId + '&ptype=' + Type + '&mainentity=' + $('#entityid').val() + '&BankId=' + BankId;
        var html = $('<ul/>').addClass('nav nav-pills nav-stacked left-menu').attr('id', 'stacked-menu');//.addClass('sub-menu');
        //li_UserProfile
        var UserProfile_html = $('<ul/>').addClass('dropdown-menu');
        var addNewCls = "";
        //var IsPartial = false;

        var BeforeAct = "";
        var AfterAct = "";
        var DisplayOrder = "";
        ajaxHelper(entityUri, 'GET').done(function (data) {
            var data2 = data;
            var IsExist = false;
            // if (EntityId != '8f4fca72-3d3c-4ad3-8f1c-1de3d602fd73') {

            $.each(data, function (rowIndex, r) {
                var ParentId = r["ParentId"];//'';
                addNewCls = r["DisplayClass"];

                if (ParentId == null || ParentId == '' || ParentId == '00000000-0000-0000-0000-000000000000') {
                    var _childs = data2.filter(function (i) { return i.ParentId == r.Id });
                    var _doPrcs = true;
                    if (_childs.length == 0 && r.Id == 'c333b448-b287-4c08-929b-0797f986ca6f') {
                        _doPrcs = false;
                    }
                    if (_doPrcs) {
                        addNewCls = r["DisplayClass"];
                        if (r.Id == '9c3fee6c-4075-4224-82d5-0b247ba142b1' && window.location.href.toLowerCase().indexOf('reports/') <= 0)
                            addNewCls += ' collapsed';

                        if (r["DisplayClass"] == 'profilelink') {
                            UserProfile_html.append($('<li><a href="' + r["URL"] + '" class="vmenu ' + r["DisplayClass"] + '" id="site' + r["Id"] + '" >' + r["Name"] + '</a></li>'));
                        } else {


                            var $li = $('<li/>');

                            if (EntityId == 1 && $('#formid').attr('formid') == 'siteb303a3e8-1a4f-4638-b31c-50d1b4b8db34') {

                                if (r["Id"] == 'c85ed511-a485-461e-a13f-1d1c0b48220a') {
                                    $li.append($('<a data-target="#item' + r["Id"] + '" data-toggle="collapse" data-parent="#stacked-menu"  href="' + r["URL"] + '" class=" vmenu ' + addNewCls + '" id="site' + r["Id"] + '" style="font-size:32px;" ></a>'));
                                }

                            }
                            else
                                $li.append($('<a data-target="#item' + r["Id"] + '" data-toggle="collapse" data-parent="#stacked-menu"  href="' + r["URL"] + '" class=" vmenu ' + addNewCls + '" id="site' + r["Id"] + '" >' + r["Name"] + '</a>'));

                            var collapseclass = 'nav nav-stacked  left-submenu';

                            if (r.Id == '9c3fee6c-4075-4224-82d5-0b247ba142b1' && $('#formid').attr('formid') != 'siteb303a3e8-1a4f-4638-b31c-50d1b4b8db34' &&  window.location.href.toLowerCase().indexOf('reports/') <= 0)
                                collapseclass += ' collapse';
                            else if ($('#entityid').val() == '1' && r.Id == '9c3fee6c-4075-4224-82d5-0b247ba142b1' &&  $('#formid').attr('formid') == 'siteb303a3e8-1a4f-4638-b31c-50d1b4b8db34')
                                collapseclass += ' collapse in';
                            else if($('#entityid').val()!='1' && r.Id == '9c3fee6c-4075-4224-82d5-0b247ba142b1' &&  window.location.href.toLowerCase().indexOf('reports/') <= 0)
                                collapseclass += ' collapse';
                            else
                                collapseclass += ' collapse in';

                            var $subul = $('<ul/>').addClass(collapseclass).attr('id', 'item' + r["Id"]);
                            //$subul.append('<ul class="nav nav-stacked collapse left-submenu" id="item1">');

                            $.each(data2, function (rowIndex2, r2) {


                                addNewCls = r2["DisplayClass"];
                                // IsPartial = r2["DisplayPartial"];
                                DisplayOrder = r2["DisplayOrder"];


                                SiteMaps.push({
                                    Id: r2["Id"],
                                    Name: r2["Name"],
                                    URL: r2["URL"],
                                    DisplayClass: r2["DisplayClass"]
                                });

                                if (r2["ParentId"] == r["Id"]) {

                                    if (addNewCls == 'profilelink') {
                                        UserProfile_html.append($('<li><a href="' + r2["URL"] + '" class="vmenu ' + addNewCls + '" id="site' + r2["Id"] + '" >' + r2["Name"] + '</a></li>'));
                                    } else {

                                        if (EntityId == 1 && $('#formid').attr('formid') == 'siteb303a3e8-1a4f-4638-b31c-50d1b4b8db34') {

                                            if (r2["Id"] == '6ed791bd-1909-4e7a-b6b1-b76983faef30') {
                                                $subul.append('<li><a href="' + r2["URL"] + '" class="allcust-remove-content vmenu menu-150p ' + addNewCls + '" id="site' + r2["Id"] + '" title="New Customer Signups"><span><i class="fa fa-user-plus"></i> <span style="font-size:12px;font-weight:bold;"> ' + r2["Description"] + ' </span></span></a></li>');
                                            }

                                            if (r2["Id"] == 'b303a3e8-1a4f-4638-b31c-50d1b4b8db34') {
                                                $subul.append('<li><a href="' + r2["URL"] + '" class="allcust-remove-content vmenu ' + addNewCls + '" id="site' + r2["Id"] + '"  title="Office Management"><i class="fa fa-cogs"></i></a></li>');
                                            }

                                            if (r2["Id"] == 'af155d4a-c29b-4dfe-9df6-e9765b35ec82') {
                                                $subul.append('<li><a href="' + r2["URL"] + '" class="allcust-remove-content vmenu ' + addNewCls + '" id="site' + r2["Id"] + '"  title="Reports"><i class="fa fa-file-text"></i></a></li>');
                                            }

                                            if (r2["Id"] == '1c157f32-aa0f-4689-a27b-ca154e9763c4' && !_feeroprtview) {
                                                $subul.append('<li><a href="' + r2["URL"] + '" class="allcust-remove-content vmenu ' + addNewCls + '" id="site' + r2["Id"] + '"  title="Reports"><i class="fa fa-file-text"></i></a></li>');
                                            }

                                            if (r2["Id"] == 'abb91b1e-53fd-49fb-bdf7-4f180c0edc65' && !_feeroprtview && !_enrstatusreportview) {
                                                $subul.append('<li><a href="' + r2["URL"] + '" class="allcust-remove-content vmenu ' + addNewCls + '" id="site' + r2["Id"] + '"  title="Reports"><i class="fa fa-file-text"></i></a></li>');
                                            }

                                            if (r2["Id"] == '947b9d9e-22ce-4584-946a-db1ce6f9c7a1' && !_feeroprtview && !_enrstatusreportview && !_nobankappreportviewe) {
                                                $subul.append('<li><a href="' + r2["URL"] + '" class="allcust-remove-content vmenu ' + addNewCls + '" id="site' + r2["Id"] + '"  title="Reports"><i class="fa fa-file-text"></i></a></li>');
                                            }
                                        }
                                        else {

                                            var bankid = getUrlVars()["bankid"];

                                            //if (Type == 'config' || Type == 'enrollment' || Type == 'subconfig') {
                                            //    var pid = getUrlVars()["ParentId"] ? getUrlVars()["ParentId"] : '00000000-0000-0000-0000-000000000000';
                                            //    r2["URL"] = r2["URL"] + '?Id=' + getUrlVars()["Id"] + '&entitydisplayid=' + getUrlVars()["entitydisplayid"] + '&entityid=' + getUrlVars()["entityid"] + '&ParentId=' + pid + '&ptype=' + getUrlVars()["ptype"];
                                            //}
                                            //// if (r2["ParentId"].toUpperCase() == 'A29B3547-8954-4036-9BD3-312F1D6A3DAA' || r2["ParentId"].toUpperCase() == '5B4C7D3A-CE74-43EE-B17E-4DD5DCFD919B' || r2["ParentId"].toUpperCase() == '2FB91EF5-EFC4-4BAF-ABD7-EA05A8C100CC') {
                                            //if (bankid) {
                                            //    if (r2["URL"].indexOf('?') > 0)
                                            //        r2["URL"] = r2["URL"] + '&bankid=' + bankid;
                                            //    else
                                            //        r2["URL"] = r2["URL"] + '?bankid=' + bankid;
                                            //}

                                            $subul.append('<li><a href="' + r2["URL"] + '" class="vmenu ' + addNewCls + '" id="site' + r2["Id"] + '" >' + r2["Name"] + '</a></li>');
                                        }
                                    }
                                }

                            });
                            $li.append($subul);
                        }
                    }
                }


                //if (r["DisplayClass"] == 'profilelink' && entitydisplayid == $('#Entity_SO').val()) {
                //    UserProfile_html.append($('<li><a href="' + r["URL"] + '" class="vmenu ' + r["DisplayClass"] + '" id="site' + r["Id"] + '" >' + r["Name"] + '</a></li>'));
                //}

                html.append($li);

            });

            container.append(html);
            $('#divUserProfile').append(UserProfile_html);


            // fnVerifiedLinksStatus();
            var enrconfig = SiteMaps.filter(function (i) { return i.Id == 'fc32db13-6aec-488e-bafe-19acb3399e57' });
            if (enrconfig.length == 0) {
                $('#divEnrollOfficeInfoForm #btn_next').remove();
                $('#divEnrollOfficeInfoForm #btn_next').remove();
            }


            var mainconfig = SiteMaps.filter(function (i) { return i.Id == '0eda5d25-591c-4e01-a845-fb580572ade8' });
            if (mainconfig.length == 0) {
                $('#configdashboard #btn_next').remove();
            }

            var subsiteconfig = SiteMaps.filter(function (i) { return i.Id == '2639fb0a-0caa-47cf-b315-587e7ce86aef' });
            if (subsiteconfig.length == 0) {
                $('#divSubSiteOfficeDashboardForm #btn_next').remove();
            }

            $('#UlMainActivate #liSVBAndTransFeeAddOn').hide();
            var SVBAndTransFeeAddOn = SiteMaps.filter(function (i) { return i.Id == '60025459-7568-4a77-b152-f81904aaaa63' });
            if (SVBAndTransFeeAddOn.length > 0) {
                $('#UlMainActivate #liSVBAndTransFeeAddOn').show();
            }
            var SVBAndTransFeeAddOn = SiteMaps.filter(function (i) { return i.Id == '0eda5d25-591c-4e01-a845-fb580572cff5' }); // e-file on account setup
            if (SVBAndTransFeeAddOn.length > 0) {
                $('#UlMainActivate #liEfile').show();
            }
            var SVBAndTransFeeAddOn = SiteMaps.filter(function (i) { return i.Id == '0eda5d25-591c-4e01-a845-fb580572cfe8' }); // outstand balance on account setup
            if (SVBAndTransFeeAddOn.length > 0) {
                $('#UlMainActivate #liOutstand').show();
            }

            //Activate My Account Hide condition
            var n = 0;
            var one = 0, two = 0, three = 0, four = 0, five = 0;
            var ActiveMyAccount = $('#ActiveMyAccountStatus').val();

            if (EntityId == $('#Entity_MO').val() || EntityId == $('#Entity_SVB').val() || EntityId == $('#Entity_uTax').val()) {
                // for (var i = 0, len = SiteMaps.length; i < len; i++) {
                var mainofficeconfig = SiteMaps.filter(function (i) { return i.Id == '0eda5d25-591c-4e01-a845-fb580572ade8' && i.DisplayClass == 'done' });
                if (mainofficeconfig.length > 0) { //mainofficeconfig
                    one = 1;
                }

                var subsiteconfig = SiteMaps.filter(function (i) { return i.Id == '68882c05-5914-4fdb-b284-e33d6c029f5a' && i.DisplayClass == 'done' });
                if (subsiteconfig.length > 0) { //subsiteconfig
                    two = 1;
                }

                var feesetupconfig = SiteMaps.filter(function (i) { return i.Id == 'c81dddc4-4654-4775-a5cd-74efa99dfc90' && i.DisplayClass == 'done' });
                if (feesetupconfig.length > 0) { //feesetupconfig
                    three = 1;
                }

                var svbtranbank = SiteMaps.filter(function (i) { return i.Id == '60025459-7568-4a77-b152-f81904aaaa63' && i.DisplayClass == 'done' });
                if (svbtranbank.length > 0) { //svbtranbank
                    four = 1;
                }

                var activateaccount = SiteMaps.filter(function (i) { return i.Id == '4fc65d1b-675f-4985-8022-9e8bd0ed735f' && i.DisplayClass == 'done' });
                if (activateaccount.length > 0) { //activateaccount
                    ActiveMyAccount = 1;
                }
                // }


                var sitesvbbank = SiteMaps.filter(function (i) { return i.Id == '60025459-7568-4a77-b152-f81904aaaa63' });
                if (sitesvbbank.length == 0) {
                    four = 1;
                }

                n = Number(one) + Number(two) + Number(three) + Number(four) + Number(five);
                var ReimNextLink = Number(one) + Number(two) + Number(three);

                if (ActiveMyAccount == 0) {

                    if (Number(ReimNextLink) < 3) {

                        $('a#btnNextFeeReim')
                        .attr('disabled', 'disabled')
                        .css('pointer-events', 'none');

                        $('a#btnSaveNextFeeReim')
                       .attr('disabled', 'disabled')
                       .css('pointer-events', 'none');
                    }

                    var FeeSetupNo = Number(one) + Number(two);
                    if (Number(FeeSetupNo) < 2) {
                        $('a#btnMainFeeSetupSaveNext')
                         .attr('disabled', 'disabled')
                         .css('pointer-events', 'none');
                    } else {
                        $('a#btnMainFeeSetupSaveNext')
                          .removeAttr('disabled')
                          .css('pointer-events', '');
                    }

                    if (n < 4) {
                        $('a#btnMainActiveAccount')
                         .attr('disabled', 'disabled')
                         .css('pointer-events', 'none')
                         .attr('title', 'please save all configuration');

                        $('div#divMainActiveAccount')
                            .addClass('activate-note')
                            .html('Note: please save all configuration');
                    }
                }


                $('#site4fc65d1b-675f-4985-8022-9e8bd0ed735f').hide();
                if (ActiveMyAccount == 0) {
                    if (Number(n) >= 4) {
                        $('#site4fc65d1b-675f-4985-8022-9e8bd0ed735f').show();

                        if ($('#entityid').val() != $('#myentityid').val()) {
                            $('#site4fc65d1b-675f-4985-8022-9e8bd0ed735f')
                                .attr('href', '/Configuration/ActivateInformation?Id=' + $('#myid').val() + '&entitydisplayid=' + $('#myentitydisplayid').val() + '&ptype=config' + '&entityid=' + $('#myentityid').val())
                                .removeClass('done');
                        } else {
                            $('#site4fc65d1b-675f-4985-8022-9e8bd0ed735f')
                                .attr('href', '/Configuration/ActivateInformation')
                                .removeClass('done');
                        }
                    }
                }
            }
            else {

                var m = 0;
                ActiveMyAccount = $('#ActiveMyAccountStatus').val();

                var subsiteconfig1 = SiteMaps.filter(function (i) { return i.Id == '2639fb0a-0caa-47cf-b315-587e7ce86aef' && i.DisplayClass == 'done' });
                if (subsiteconfig1.length > 0) { //subsiteconfig
                    m = m + 1;
                }

                var feesetupconfig1 = SiteMaps.filter(function (i) { return i.Id == 'd8d06578-1923-4792-bdad-153603b57068' && i.DisplayClass == 'done' });
                if (feesetupconfig1.length > 0) { //feesetupconfig
                    m = m + 1;
                }

                var activateaccount = SiteMaps.filter(function (i) { return i.Id == '7a2c166c-c2ef-47c0-aa5f-e4950f9ff369' && i.DisplayClass == 'done' });
                if (activateaccount.length > 0) { //activateaccount
                    ActiveMyAccount = 1;
                }
                //}
                debugger;
                $('#site7a2c166c-c2ef-47c0-aa5f-e4950f9ff369').hide();
                if (ActiveMyAccount == 0) {
                    if (Number(m) >= 2) {

                        $('#site7a2c166c-c2ef-47c0-aa5f-e4950f9ff369').show();
                        if ($('#entityid').val() != $('#myentityid').val()) {
                            $('#site7a2c166c-c2ef-47c0-aa5f-e4950f9ff369').attr('href', '/SubSiteOfficeConfiguration/ActivateMyAccount?Id=' + $('#myid').val() + "&ParentId=" + $('#myparentid').val() + '&entitydisplayid=' + $('#myentitydisplayid').val() + '&entityid=' + $('#myentityid').val() + '&ptype=subconfig');
                        } else {
                            $('#site7a2c166c-c2ef-47c0-aa5f-e4950f9ff369').attr('href', '/SubSiteOfficeConfiguration/ActivateMyAccount');
                        }
                    }
                    else {

                        $('a#btnSubActiveAccount')
                                 .attr('disabled', 'disabled')
                                 .css('pointer-events', 'none')
                                 .attr('title', 'please save all configuration');

                        $('div#divSubActiveAccount')
                             .addClass('activate-note')
                            .html('Note: please save all configuration');
                    }
                }

            }

            //ENROLLMENT

            //// Bank Select Selectd as None then Following Conditions

            var enrollstatus = SiteMaps.filter(function (i) { return i.Id == '067c03a3-34f1-4143-beae-35327a8fca44' && i.DisplayClass == 'none' });

            var EFINOwnerUserId = false;
            if ($('#entityid').val() != $('#myentityid').val()) {
                EFINOwnerUserId = localStorage.getItem("EFINOwnerUserId");
            } else {
                if ($('#EFINOwnerUserId').val() == 'true' || $('#EFINOwnerUserId').val() == 'True' || $('#EFINOwnerUserId').val() == true) {
                    EFINOwnerUserId = true;
                }
            }
            var CanSubSiteLoginToEmp = $('#CanSubSiteLoginToEmp').val();

            if (enrollstatus.length > 0) {
                $('#site0feeb0fe-d0e7-4370-8733-dd5f7d2041fc').remove();
                $('#sitea55334d1-3960-44c4-8cf1-e3ba9901f2be').remove();
                $('#site98a706d7-031f-4c5d-8cc4-d32cc7658b69').show();
                $('#btnBankEnrollSubmit').remove();
                $('#libankenrollment').hide();
                $('#lifee').hide();
            }

            if (CanSubSiteLoginToEmp == 'false' || CanSubSiteLoginToEmp == 'False' || CanSubSiteLoginToEmp == false) {
                $('#site067c03a3-34f1-4143-beae-35327a8fca44').remove();
                $('#site0feeb0fe-d0e7-4370-8733-dd5f7d2041fc').remove();
                $('#site98a706d7-031f-4c5d-8cc4-d32cc7658b69').remove();
                //$('#btnEnrollAffSaveNext').hide();
                $('#btnEnrollAffSaveNext').remove();
            }



            //if (entitydisplayid == $('#BaseEntity_AE').val() || entitydisplayid == $('#BaseEntity_AESS').val()) {
            //    getSubSiteConfigStatus();
            //} else {
            //    getConfigStatus();
            //}


            $('a.vmenu')
                .removeClass('selected')

            //$('a#' + formname)
            //    .addClass(status);

            var formid = $('#formid').attr('formid');
            $('a#' + formid)
                 .addClass("selected");

            //Minimising the completed menus

            setTimeout(function () {
                if ($('#entityid').val() != 1) {
                    var mainofficedone = true;
                    $('#item6df12cef-8bdb-4d84-9057-095590ea0a79 li').each(function (e) {
                        var anchorClass = $(this)[0].children[0].className;
                        if (anchorClass.indexOf('done') == -1)
                            mainofficedone = false;
                    })
                    $('#item6df12cef-8bdb-4d84-9057-095590ea0a79 li').each(function (e) {
                        var anchorClass = $(this)[0].children[0].className;
                        if (anchorClass.indexOf('selected') >0)
                            mainofficedone = false;
                    })
                    if (mainofficedone) {
                        $('#site6df12cef-8bdb-4d84-9057-095590ea0a79').addClass('collapsed').attr('aria-expanded', 'false');
                        $('#item6df12cef-8bdb-4d84-9057-095590ea0a79').removeClass('in').attr('aria-expanded', 'false').css('height', '0px');
                    }

                    var enrolldone = true;
                    $('#itemc333b448-b287-4c08-929b-0797f986ca6f li').each(function (e) {
                        var anchorClass = $(this)[0].children[0].className;
                        if (anchorClass.indexOf('done') == -1)
                            enrolldone = false;
                    })
                    $('#itemc333b448-b287-4c08-929b-0797f986ca6f li').each(function (e) {
                        var anchorClass = $(this)[0].children[0].className;
                        if (anchorClass.indexOf('selected') >0)
                            enrolldone = false;
                    })
                    if (enrolldone) {
                        $('#itemc333b448-b287-4c08-929b-0797f986ca6f').addClass('collapsed').attr('aria-expanded', 'false');
                        $('#itemc333b448-b287-4c08-929b-0797f986ca6f').removeClass('in').attr('aria-expanded', 'false').css('height', '0px');
                    }
                }
            })
        });
    }
}

function getSubSitemap(container) {
    container.html('');
    if (SiteMaps.length == 0) {
        var EntityId = $('#entityid').val();
        var UserId = $('#UserId').val();
        var IsAdded = false;
        var entityUri = '/api/Sitemap/SubSiteMap?entityid=' + EntityId + '&CustomerId=' + UserId;
        var html = $('<ul/>');//.addClass('sub-menu');
        //li_UserProfile
        var UserProfile_html = $('<ul/>').addClass('dropdown-menu');
        var addNewCls = "";
        //var IsPartial = false;
        ajaxHelper(entityUri, 'GET').done(function (data) {
            var data2 = data;
            var IsExist = false;

            // if (EntityId != '8f4fca72-3d3c-4ad3-8f1c-1de3d602fd73') {

            $.each(data, function (rowIndex, r) {
                addNewCls = r["DisplayClass"];//'';
                // IsPartial = r["DisplayPartial"];//'';
                IsExist = false;
                IsAdded = true;

                //if (r["Id"] == "1f2a6418-f9bc-4878-ab0b-2fa004c01c01") {
                //    addNewCls = 'active';
                //}

                if (IsAdded) {
                    SiteMaps.push({
                        Id: r["Id"],
                        Name: r["Name"],
                        URL: r["URL"],
                    });
                    if (addNewCls == 'profilelink') {
                        UserProfile_html.append($('<li><a href="' + r["URL"] + '" class="vmenu ' + addNewCls + '" id="site' + r["Id"] + '" >' + r["Name"] + '</a></li>'));
                    }
                    else {
                        html.append($('<li><a href="' + r["URL"] + '" class="vmenu ' + addNewCls + '" id="site' + r["Id"] + '" >' + r["Name"] + '</a></li>'));
                    }
                }

            });
            //}
            //else {
            //        html.append($('<li><a href="/CustomerInformation" class="vmenu active" id="site6ed791bd-1909-4e7a-b6b1-b76983faef30">New Customer Signups</a></li>'));
            //    }
            container.append(html);
            $('#divUserProfile').append(UserProfile_html);
        });
    }
}

function getSubSiteConfigStatus() {
    //var SubSiteConfigStatus = [];
    //var UserId;

    //if ($('#entityid').val() != $('#myentityid').val()) {
    //    UserId = $('#myid').val();
    //}
    //else {
    //    UserId = $('#UserId').val();
    //}

    //var Uri = '/api/Configuration';
    //var html = $('<ul/>');//.addClass('sub-menu');
    //var status = 'active';
    //var IsDataExist = false;
    //ajaxHelper(Uri + '?id=' + UserId, 'GET', null, false).done(function (data) {
    //    if (data != null && data != '' && data != undefined) {
    //        IsDataExist = true;
    //        $.each(data, function (rowIndex, r) {

    //            SubSiteConfigStatus.push({
    //                SitemapId: r["SitemapId"],
    //                Status: r["Status"]
    //            });

    //            if (r["Status"] == 'done') {
    //                var status = 'done';//$('#formid').val();
    //            }

    //            var formname = 'site' + r["SitemapId"];

    //            $('a.vmenu')
    //                .removeClass('selected')
    //            //    .removeClass('done');

    //            $('a#' + formname)
    //                .addClass(status);

    //            var formid = $('#formid').attr('formid');

    //            if (formid != formname) {
    //                $('a#' + formid)
    //                     .addClass("selected");
    //            } else {
    //                $('a#' + formname)
    //                    .addClass("selected");
    //            }
    //        });
    //    }
    //});

    //if (!IsDataExist) {

    //    $('a.vmenu')
    //              .removeClass('selected');
    //    // .removeClass('selected');

    //    var formid = $('#formid').attr('formid');
    //    $('a#' + formid)
    //         .addClass('selected');
    //}
    //var ActiveMyAccount = 0;
    //// $('#ActiveMyAccountStatus').val('0');
    //var n = 0; //this is for Sub Site configuration side

    //var enrollstatus = SubSiteConfigStatus.filter(function (i) { return i.SitemapId == '0feeb0fe-d0e7-4370-8733-dd5f7d2041fc' && i.Status == 'done' });
    //if (enrollstatus.length <= 0) {
    //    $('#site98a706d7-031f-4c5d-8cc4-d32cc7658b69').hide();
    //}
    //var entityid = $('#entityid').val();
    //var dashboardstatus = SubSiteConfigStatus.filter(function (i) { return i.SitemapId == '1f2a6418-f9bc-4878-ab0b-2fa004c01c01' && i.Status == 'done' });
    //if (dashboardstatus.length <= 0 && entityid != $('#Entity_uTax').val()) {
    //    $('#site98a706d7-031f-4c5d-8cc4-d32cc7658b63').text('Main Office Information');
    //}
    //// alert(SubSiteConfigStatus.length + "Lent");
    //if (SubSiteConfigStatus.length > 0) {
    //    for (var i = 0, len = SubSiteConfigStatus.length; i < len; i++) {

    //        if (SubSiteConfigStatus[i].SitemapId == "1f2a6418-f9bc-4878-ab0b-2fa004c01c01" && SubSiteConfigStatus[i].Status == "done") {
    //            n = n + 1;
    //        }
    //        if (SubSiteConfigStatus[i].SitemapId == "2639fb0a-0caa-47cf-b315-587e7ce86aef" && SubSiteConfigStatus[i].Status == "done") {
    //            n = n + 1;
    //        }
    //        if (SubSiteConfigStatus[i].SitemapId == "d8d06578-1923-4792-bdad-153603b57068" && SubSiteConfigStatus[i].Status == "done") {
    //            n = n + 1;
    //        }

    //        if (SubSiteConfigStatus[i].SitemapId == "7a2c166c-c2ef-47c0-aa5f-e4950f9ff369" && SubSiteConfigStatus[i].Status == "done") { // Service Transmi 
    //            ActiveMyAccount = 1;

    //            $('#ActiveMyAccountStatus').val('1');
    //        }
    //    }
    //}

    //$('#site7a2c166c-c2ef-47c0-aa5f-e4950f9ff369').hide();
    //if (ActiveMyAccount == 0) {
    //    if (Number(n) >= 2) {

    //        $('#site7a2c166c-c2ef-47c0-aa5f-e4950f9ff369').show();

    //        if ($('#entityid').val() != $('#myentityid').val()) {

    //            $('#site7a2c166c-c2ef-47c0-aa5f-e4950f9ff369').attr('href', '/SubSiteOfficeConfiguration/ActivateMyAccount?Id=' + $('#myid').val() + "&ParentId=" + $('#myparentid').val() + '&entitydisplayid=' + $('#myentitydisplayid').val() + '&entityid=' + $('#myentityid').val() + '&ptype=subconfig');

    //        } else {

    //            $('#site7a2c166c-c2ef-47c0-aa5f-e4950f9ff369').attr('href', '/SubSiteOfficeConfiguration/ActivateMyAccount');
    //        }
    //    } else {

    //        $('a#btnSubActiveAccount')
    //                 .attr('disabled', 'disabled')
    //                 .css('pointer-events', 'none')
    //                 .attr('title', 'please save all configuration');

    //        $('div#divSubActiveAccount')
    //             .addClass('activate-note')
    //            .html('Note: please save all configuration');
    //    }
    //}

    //EnrollmentMenuStatus(SubSiteConfigStatus);

}

function getTooltip() {
    $('a.ttip').hide();
    var sitemapid = $('#formid').attr('formid');
    sitemapid = sitemapid.toString().replace('site', '');
    var custmorUri = '/api/dropdown/tooltip?sitemapid=' + sitemapid;
    ajaxHelper(custmorUri, 'GET').done(function (data) {
        $.each(data, function (rowIndex, r) {
            if (r)
                $('#ttip' + r["Id"])
                    .attr('data-original-title', r["Tooltip"])
                    .show();
        });
    });
}

function getUserBanksOld(container1, container2, container3, container4) {  //24/09/2016 Comments Common fee concept

    container1.html('');
    container2.html('');
    container3.html('');
    container4.html('');

    var UserId = $('#UserId').val();
    var custmorUri = '/api/dropdown/userbanks?UserId=' + UserId;

    ajaxHelper(custmorUri, 'GET').done(function (data) {

        $.each(data, function (rowIndex, r) {

            var DocID = r["DocumentPath"];
            if (DocID != '' && DocID != null && DocID != undefined) {
                var docpath = EMPAdminWebAPI + '/' + DocID;
                container1.append('<div class="col-md-4"><div class="form-group is-empty"><label>' + r["BankName"] + ' (Maximum available for Desktop : ' + r["FeeDesktop"] + ')  <a href="' + docpath + '" target="_blank" title="Download"> <i class="fa fa-arrow-circle-down"></i> </a></label><input id="input_' + r["BankId"] + '" feetype="desktop"  feedsk="' + r["FeeDesktop"] + '"  class="form-control bankservice decimal svbfee" name="bankservice" type="text" bankid="' + r["BankId"] + '" maxlength="8"></div></div>');
                container2.append('<div class="col-md-4"><div class="form-group is-empty"><label>' + r["BankName"] + ' (Maximum available for MSO : ' + r["FeeMSO"] + ')  <a href="' + docpath + '" target="_blank" title="Download"> <i class="fa fa-arrow-circle-down"></i> </a></label><input id="input_' + r["BankId"] + '_MSO"  feetype="mso" feemso="' + r["FeeMSO"] + '"   class="form-control decimal svbfee" type="text" bankid="' + r["BankId"] + '" maxlength="8"></div></div>');

                container3.append('<div class="col-md-4"><div class="form-group is-empty"><label>' + r["BankName"] + ' (Maximum available for Desktop : ' + r["TranFeeDesktop"] + ')  <a href="' + docpath + '" target="_blank" title="Download"> <i class="fa fa-arrow-circle-down"></i> </a></label><input id="input_trns_' + r["BankId"] + '" feetype="desktop"  tranfeedsk="' + r["TranFeeDesktop"] + '" class="form-control banktrans decimal transfee" name="banktrans" type="text" bankid="' + r["BankId"] + '" maxlength="8"></div></div>');
                container4.append('<div class="col-md-4"><div class="form-group is-empty"><label>' + r["BankName"] + ' (Maximum available for MSO : ' + r["TranFeeMSO"] + ')  <a href="' + docpath + '" target="_blank" title="Download"> <i class="fa fa-arrow-circle-down"></i> </a></label><input id="input_trns_' + r["BankId"] + '_MSO" feetype="mso" tranfeemso="' + r["TranFeeMSO"] + '" class="form-control decimal transfee" type="text" bankid="' + r["BankId"] + '" maxlength="8"></div></div>');

            } else {
                container1.append('<div class="col-md-4"><div class="form-group is-empty"><label>' + r["BankName"] + ' (Maximum available for Desktop : ' + r["FeeDesktop"] + ')</label><input id="input_' + r["BankId"] + '" feetype="desktop"  feedsk="' + r["FeeDesktop"] + '"  class="form-control bankservice decimal svbfee" name="bankservice" type="text" bankid="' + r["BankId"] + '" maxlength="8"></div></div>');
                container2.append('<div class="col-md-4"><div class="form-group is-empty"><label>' + r["BankName"] + ' (Maximum available for MSO : ' + r["FeeMSO"] + ')</label><input id="input_' + r["BankId"] + '_MSO"  feetype="mso" feemso="' + r["FeeMSO"] + '"   class="form-control decimal svbfee" type="text" bankid="' + r["BankId"] + '" maxlength="8"></div></div>');

                container3.append('<div class="col-md-4"><div class="form-group is-empty"><label>' + r["BankName"] + ' (Maximum available for Desktop : ' + r["TranFeeDesktop"] + ')</label><input id="input_trns_' + r["BankId"] + '"  feetype="desktop"  tranfeedsk="' + r["TranFeeDesktop"] + '" class="form-control banktrans decimal transfee" name="banktrans" type="text" bankid="' + r["BankId"] + '" maxlength="8"></div></div>');
                container4.append('<div class="col-md-4"><div class="form-group is-empty"><label>' + r["BankName"] + ' (Maximum available for MSO : ' + r["TranFeeMSO"] + ')</label><input id="input_trns_' + r["BankId"] + '_MSO"  feetype="mso" tranfeemso="' + r["TranFeeMSO"] + '" class="form-control decimal transfee" type="text" bankid="' + r["BankId"] + '" maxlength="8"></div></div>');
            }

        });
    });
}

function getUserBanks(container1, container2, container3, container4) {

    container1.html('');
    container2.html('');
    container3.html('');
    container4.html('');
    var UserId = $('#UserId').val();

    var ismsouser = $('#ismsouser').val();
    if ($('#myid').length > 0) {
        UserId = $('#myid').val();
    }
    //var custmorUri = '/api/dropdown/userbanks?UserId=' + UserId;

    var custmorUri = '/api/dropdown/subsitebank?id=' + UserId;

    ajaxHelper(custmorUri, 'GET').done(function (data) {

        $.each(data, function (rowIndex, r) {

            var TotalBankProductFees = $('#TotalBankProductFees').val();
            var TotaleFileFees = $('#TotaleFileFees').val();

            var BankSVBDesktopFee = r["BankSVBDesktopFee"];
            var BankSVBMSOFee = r["BankSVBMSOFee"];

            var BankTranDesktopFee = r["BankTranDesktopFee"];
            var BankTranMSOFee = r["BankTranMSOFee"];

            var SubDesktopFee = 0;// r["SubDesktopFee"];
            var SubMSOFee = 0;// r["SubMSOFee"];

            if (TotalBankProductFees == '' || TotalBankProductFees == null) {
                TotalBankProductFees = 0;
            }

            if (TotaleFileFees == '' || TotaleFileFees == null) {
                TotaleFileFees = 0;
            }

            if (SubDesktopFee == '' || SubDesktopFee == null) {
                SubDesktopFee = 0;
            }

            if (SubMSOFee == '' || SubMSOFee == null) {
                SubMSOFee = 0;
            }

            BankSVBDesktopFee = Number(BankSVBDesktopFee) - (Number(TotalBankProductFees)); // + Number(SubDesktopFee)
            BankSVBMSOFee = Number(BankSVBMSOFee) - (Number(TotalBankProductFees)); //+ Number(SubMSOFee)

            BankTranDesktopFee = Number(BankTranDesktopFee);// - (Number(TotaleFileFees)); //+ Number(SubDesktopFee)
            BankTranMSOFee = Number(BankTranMSOFee);// - (Number(TotaleFileFees)); // + Number(SubMSOFee)

            BankSVBDesktopFee = BankSVBDesktopFee.toFixed(2);
            BankSVBMSOFee = BankSVBMSOFee.toFixed(2);
            BankTranDesktopFee = BankTranDesktopFee.toFixed(2);
            BankTranMSOFee = BankTranMSOFee.toFixed(2);

            var DocID = r["DocumentPath"];
            if (DocID != '' && DocID != null && DocID != undefined) {

                var docpath = EMPAdminWebAPI + '/' + DocID;

                if (r["ServiceorTransmission"] == 1 || r["ServiceorTransmission"] == '1') {

                    if (ismsouser == 'true' || ismsouser == true || ismsouser == "True") {
                        container1.append('<div class="col-md-4"><div class="form-group is-empty"><label>' + r["BankName"] + ' (Maximum available for Desktop : $' + BankSVBDesktopFee + ')  <a href="' + docpath + '" target="_blank" title="Download"> <i class="fa fa-arrow-circle-down"></i> </a></label><input  mainfee="' + r["DesktopFee"] + '"  subfee="' + SubDesktopFee + '" id="input_' + r["BankId"] + '" feetype="desktop"  maxfee="' + r["BankSVBDesktopFee"] + '"  class="form-control bankservice decimal svbfee" name="bankservice" type="text" bankid="' + r["BankId"] + '" maxlength="8" onkeyup="fnFeeSetupSVBFee(this)"></div></div>');
                        container2.append('<div class="col-md-4"><div class="form-group is-empty"><label>' + r["BankName"] + ' (Maximum available for MSO : $' + BankSVBMSOFee + ')  <a href="' + docpath + '" target="_blank" title="Download"> <i class="fa fa-arrow-circle-down"></i> </a></label><input  mainfee="' + r["MSOFee"] + '" subfee="' + SubMSOFee + '"  id="input_' + r["BankId"] + '_MSO"  feetype="mso" maxfee="' + r["BankSVBMSOFee"] + '"   class="form-control decimal svbfee" type="text" bankid="' + r["BankId"] + '" maxlength="8"  onkeyup="fnFeeSetupSVBFee(this)"></div></div>');
                    }
                    else {
                        container1.append('<div class="col-md-4"><div class="form-group is-empty"><label>' + r["BankName"] + ' (Maximum available for Desktop : $' + BankSVBDesktopFee + ')  <a href="' + docpath + '" target="_blank" title="Download"> <i class="fa fa-arrow-circle-down"></i> </a></label><input  mainfee="' + r["DesktopFee"] + '"  subfee="' + SubDesktopFee + '" id="input_' + r["BankId"] + '" feetype="desktop"  maxfee="' + r["BankSVBDesktopFee"] + '"  class="form-control bankservice decimal svbfee" name="bankservice" type="text" bankid="' + r["BankId"] + '" maxlength="8"  onkeyup="fnFeeSetupSVBFee(this)"></div></div>');
                    }

                } else {
                    if (ismsouser == 'true' || ismsouser == true || ismsouser == "True") {
                        container3.append('<div class="col-md-4"><div class="form-group is-empty"><label>' + r["BankName"] + ' (Maximum available for Desktop : $' + BankTranDesktopFee + ')  <a href="' + docpath + '" target="_blank" title="Download"> <i class="fa fa-arrow-circle-down"></i> </a></label><input  mainfee="' + r["DesktopFee"] + '" subfee="' + SubDesktopFee + '" id="input_trns_' + r["BankId"] + '" feetype="desktop"  maxfee="' + r["BankTranDesktopFee"] + '" class="form-control banktrans decimal transfee" name="banktrans" type="text" bankid="' + r["BankId"] + '" maxlength="8"  onkeyup="fnFeeSetupTRANSFee(this)"></div></div>');
                        container4.append('<div class="col-md-4"><div class="form-group is-empty"><label>' + r["BankName"] + ' (Maximum available for MSO : $' + BankTranMSOFee + ')  <a href="' + docpath + '" target="_blank" title="Download"> <i class="fa fa-arrow-circle-down"></i> </a></label><input  mainfee="' + r["MSOFee"] + '" subfee="' + SubMSOFee + '"  id="input_trns_' + r["BankId"] + '_MSO" feetype="mso" maxfee="' + r["BankTranMSOFee"] + '" class="form-control decimal transfee" type="text" bankid="' + r["BankId"] + '" maxlength="8"   onkeyup="fnFeeSetupTRANSFee(this)"></div></div>');
                    } else {
                        container3.append('<div class="col-md-4"><div class="form-group is-empty"><label>' + r["BankName"] + ' (Maximum available for Desktop : $' + BankTranDesktopFee + ')  <a href="' + docpath + '" target="_blank" title="Download"> <i class="fa fa-arrow-circle-down"></i> </a></label><input  mainfee="' + r["DesktopFee"] + '" subfee="' + SubDesktopFee + '" id="input_trns_' + r["BankId"] + '" feetype="desktop"  maxfee="' + r["BankTranDesktopFee"] + '" class="form-control banktrans decimal transfee" name="banktrans" type="text" bankid="' + r["BankId"] + '" maxlength="8"   onkeyup="fnFeeSetupTRANSFee(this)"></div></div>');
                    }
                }

            } else {

                if (r["ServiceorTransmission"] == 1 || r["ServiceorTransmission"] == '1') {
                    if (ismsouser == 'true' || ismsouser == true || ismsouser == "True") {
                        container1.append('<div class="col-md-4"><div class="form-group is-empty"><label>' + r["BankName"] + ' (Maximum available for Desktop : $' + BankSVBDesktopFee + ')</label><input  mainfee="' + r["DesktopFee"] + '"  subfee="' + SubDesktopFee + '" id="input_' + r["BankId"] + '" feetype="desktop"  maxfee="' + r["BankSVBDesktopFee"] + '"  class="form-control bankservice decimal svbfee" name="bankservice" type="text" bankid="' + r["BankId"] + '" maxlength="8" onkeyup="fnFeeSetupSVBFee(this)"></div></div>');
                        container2.append('<div class="col-md-4"><div class="form-group is-empty"><label>' + r["BankName"] + ' (Maximum available for MSO : $' + BankSVBMSOFee + ')</label><input  mainfee="' + r["MSOFee"] + '" subfee="' + SubMSOFee + '" id="input_' + r["BankId"] + '_MSO"  feetype="mso" maxfee="' + r["BankSVBMSOFee"] + '"   class="form-control decimal svbfee" type="text" bankid="' + r["BankId"] + '" maxlength="8"  onkeyup="fnFeeSetupSVBFee(this)"></div></div>');
                    } else {
                        container1.append('<div class="col-md-4"><div class="form-group is-empty"><label>' + r["BankName"] + ' (Maximum available for Desktop : $' + BankSVBDesktopFee + ')</label><input  mainfee="' + r["DesktopFee"] + '"  subfee="' + SubDesktopFee + '" id="input_' + r["BankId"] + '" feetype="desktop"  maxfee="' + r["BankSVBDesktopFee"] + '"  class="form-control bankservice decimal svbfee" name="bankservice" type="text" bankid="' + r["BankId"] + '" maxlength="8"  onkeyup="fnFeeSetupSVBFee(this)"></div></div>');

                    }
                } else {
                    if (ismsouser == 'true' || ismsouser == true || ismsouser == "True") {
                        container3.append('<div class="col-md-4"><div class="form-group is-empty"><label>' + r["BankName"] + ' (Maximum available for Desktop : $' + BankTranDesktopFee + ')</label><input  mainfee="' + r["DesktopFee"] + '" subfee="' + SubDesktopFee + '" id="input_trns_' + r["BankId"] + '"  feetype="desktop"  maxfee="' + r["BankTranDesktopFee"] + '" class="form-control banktrans decimal transfee" name="banktrans" type="text" bankid="' + r["BankId"] + '" maxlength="8" onkeyup="fnFeeSetupTRANSFee(this)"></div></div>');
                        container4.append('<div class="col-md-4"><div class="form-group is-empty"><label>' + r["BankName"] + ' (Maximum available for MSO : $' + BankTranMSOFee + ')</label><input mainfee="' + r["MSOFee"] + '" subfee="' + SubMSOFee + '"  id="input_trns_' + r["BankId"] + '_MSO"  feetype="mso" maxfee="' + r["BankTranMSOFee"] + '" class="form-control decimal transfee" type="text" bankid="' + r["BankId"] + '" maxlength="8" onkeyup="fnFeeSetupTRANSFee(this)"></div></div>');
                    } else {
                        container3.append('<div class="col-md-4"><div class="form-group is-empty"><label>' + r["BankName"] + ' (Maximum available for Desktop : $' + BankTranDesktopFee + ')</label><input  mainfee="' + r["DesktopFee"] + '" subfee="' + SubDesktopFee + '" id="input_trns_' + r["BankId"] + '"  feetype="desktop"  maxfee="' + r["BankTranDesktopFee"] + '" class="form-control banktrans decimal transfee" name="banktrans" type="text" bankid="' + r["BankId"] + '" maxlength="8" onkeyup="fnFeeSetupTRANSFee(this)"></div></div>');
                    }
                }
            }

        });

        var url = '/api/SubSiteFee/';
        if (UserId != '' && UserId != null && UserId != '00000000-0000-0000-0000-000000000000') {
            ajaxHelper(url + '?Id=' + UserId, 'GET').done(function (data) {

                if (data == "" || data == null || data == undefined) {
                    //$('#subquestion_Service').hide();
                    //$('#subquestion_Trans').hide();
                }

                $.each(data, function (rowIndex, r) {
                    $('#ID').val(r["Id"]);
                    //if (entityid != $('#Entity_SO').val()) {//entityid != "0676dfd0-da29-41e3-a262-81cb528b796c") {

                    if (r["ServiceorTransmission"] == 1) {
                        var BankTrans = $('input[type=text].bankservice');
                        $.each(r.SubSiteBankFees, function (indx, c) {

                            $('#input_' + c["BankMaster_ID"]).val(c["BankMaxFees"]);
                            $('#input_' + c["BankMaster_ID"] + '_MSO').val(c["BankMaxFees_MSO"]);

                            //var maxfee = $('#input_' + c["BankMaster_ID"]).attr('maxfee');
                            //var myvalu = c["BankMaxFees"];

                            //if (maxfee == '' || maxfee == null) {
                            //    maxfee = 0;
                            //}

                            //if (myvalu == '' || myvalu == null) {
                            //    myvalu = 0;
                            //}

                            //var myMaxfee=0;

                            //if (maxfee > myvalu) {
                            //    myMaxfee = Number(maxfee) - Number(myvalu)
                            //} else {
                            //    myMaxfee = Number(myvalu) - Number(maxfee)
                            //}
                            //$('#input_' + c["BankMaster_ID"]).attr('maxfee', myMaxfee);

                            ////

                            //var maxfeeMSO = $('#input_' + c["BankMaster_ID"] + '_MSO').attr('maxfee');
                            //var myvaluMSO = c["BankMaxFees_MSO"];

                            //if (maxfeeMSO == '' || maxfeeMSO == null) {
                            //    maxfeeMSO = 0;
                            //}

                            //if (myvaluMSO == '' || myvaluMSO == null) {
                            //    myvaluMSO = 0;
                            //}

                            //var myMaxfeeMSO = 0;

                            //if (maxfeeMSO > myvaluMSO) {
                            //    myMaxfeeMSO = Number(maxfeeMSO) - Number(myvaluMSO)
                            //}
                            //else {
                            //    myMaxfeeMSO = Number(myvaluMSO) - Number(maxfeeMSO)
                            //}

                            //$('#input_' + c["BankMaster_ID"] + '_MSO').attr('maxfee', myMaxfeeMSO);

                        });

                        //if (r["IsAddOnFeeCharge"] == true) {
                        //    $('#rbService_BankProductYes').prop('checked', true);
                        //}
                        //else {
                        //    $('#rbService_BankProductNo').prop('checked', true);
                        //    $('#subquestion_Service').hide();
                        //}
                        if (r["IsSameforAll"] == true) {
                            $('#rbService_SubSiteYes').prop('checked', true);
                        }
                        else {
                            $('#rbService_SubSiteNo').prop('checked', true);
                        }
                        if (r["IsSubSiteAddonFee"] == true) {
                            $('#rbService_duringEnrollingYes').prop('checked', true);
                        }
                        else {
                            $('#rbService_duringEnrollingNo').prop('checked', true);
                        }
                    }
                    else {
                        var BankTrans = $('input[type=text].banktrans');

                        $.each(r.SubSiteBankFees, function (indx, c) {
                            $('#input_trns_' + c["BankMaster_ID"]).val(c["BankMaxFees"]);
                            $('#input_trns_' + c["BankMaster_ID"] + '_MSO').val(c["BankMaxFees_MSO"]);

                            //var maxfee = $('#input_trns_' + c["BankMaster_ID"]).attr('maxfee');
                            //var myvalu = c["BankMaxFees"];

                            //if (maxfee == '' || maxfee == null) {
                            //    maxfee = 0;
                            //}

                            //if (myvalu == '' || myvalu == null) {
                            //    myvalu = 0;
                            //}

                            //var myMaxfee = 0;

                            //if (maxfee > myvalu) {
                            //    myMaxfee = Number(maxfee) - Number(myvalu)
                            //} else {
                            //    myMaxfee = Number(myvalu) - Number(maxfee)
                            //}

                            //$('#input_trns_' + c["BankMaster_ID"]).attr('maxfee', myMaxfee);

                            ////

                            //var maxfeeMSO = $('#input_trns_' + c["BankMaster_ID"] + '_MSO').attr('maxfee');
                            //var myvaluMSO = c["BankMaxFees_MSO"];

                            //if (maxfeeMSO == '' || maxfeeMSO == null) {
                            //    maxfeeMSO = 0;
                            //}

                            //if (myvaluMSO == '' || myvaluMSO == null) {
                            //    myvaluMSO = 0;
                            //}

                            //var myMaxfeeMSO = 0;

                            //if (maxfeeMSO > myvaluMSO) {
                            //    myMaxfeeMSO = Number(maxfeeMSO) - Number(myvaluMSO)
                            //}
                            //else {
                            //    myMaxfeeMSO = Number(myvaluMSO) - Number(maxfeeMSO)
                            //}

                            //$('#input_trns_' + c["BankMaster_ID"] + '_MSO').attr('maxfee', myMaxfeeMSO);


                        });

                        //if (r["IsAddOnFeeCharge"] == true) {
                        //    $('#rbTrans_BankProductYes').prop('checked', true);
                        //}
                        //else {
                        //    $('#rbTrans_BankProductNo').prop('checked', true);
                        //    $('#subquestion_Trans').hide();
                        //}
                        if (r["IsSameforAll"] == true) {
                            $('#rbTrans_SubSiteYes').prop('checked', true);
                        }
                        else {
                            $('#rbTrans_SubSiteNo').prop('checked', true);
                        }
                        if (r["IsSubSiteAddonFee"] == true) {
                            $('#rbTrans_duringEnrollingYes').prop('checked', true);
                        }
                        else {
                            $('#rbTrans_duringEnrollingNo').prop('checked', true);
                        }
                    }

                });

                $('#rbService_BankProductYes').prop('checked', true);
                $("#rbTrans_BankProductYes").prop('checked', true);

                getIsEnrollmentSubmit();
                //  getIsSalesYearCheckBankDates();

                var iretval = getIsSalesYearCheckBankDates();
                var error_bank = $('#error_bank');
                error_bank.html('');
                error_bank.hide();
                if (iretval == false || iretval == 'false' || iretval == 'False') {
                    error_bank.show();
                    error_bank.html('');
                    error_bank.append('<p>The ability to update the Fees information is not available since the Cutoff date for the same is elapsed. Please contact the uTax support team. </p>');
                    return false;
                }

            });
        }

    });
}

//This is get the State Master Details
function getStateMaster(container) {
    container.html('');
    var entityid = $('#entityid').val();
    var custmorUri = '/api/dropdown/States';
    ajaxHelper(custmorUri, 'GET').done(function (data) {
        $.each(data, function (rowIndex, r) {
            container.append('<option value="' + r["StateID"] + '">' + r["StateName"] + ' (' + r["StateCode"] + ')</option>"');
        });
    });
}

//function getStateMasterCode(container) {
//    container.html('');
//    var entityid = $('#entityid').val();
//    var custmorUri = '/api/dropdown/States';
//    ajaxHelper(custmorUri, 'GET').done(function (data) {
//        $.each(data, function (rowIndex, r) {
//            container.append('<option value="' + r["StateCode"] + '">' + r["StateName"] + ' (' + r["StateCode"] + ')</option>"');
//        });
//    });
//}

function getStateMasterCode(container1, container2) {
    container1.html('');
    container2.html('');
    var entityid = $('#entityid').val();
    var custmorUri = '/api/dropdown/States';
    ajaxHelper(custmorUri, 'GET', null, false).done(function (data) {
        $.each(data, function (rowIndex, r) {
            container1.append('<option value="' + r["StateCode"] + '">' + r["StateName"] + ' (' + r["StateCode"] + ')</option>"');
            container2.append('<option value="' + r["StateCode"] + '">' + r["StateName"] + ' (' + r["StateCode"] + ')</option>"');
        });
    });
}

function getStateMasterCodeMultiple(containers) {
    var entityid = $('#entityid').val();
    var custmorUri = '/api/dropdown/States';
    ajaxHelper(custmorUri, 'GET', null, false).done(function (data) {
        for (i = 0; i < containers.length; i++) {
            var container = containers[i];
            container.html('');
            $.each(data, function (rowIndex, r) {
                container.append('<option value="' + r["StateCode"] + '">' + r["StateName"] + ' (' + r["StateCode"] + ')</option>"');
            });
        }
    });
}

function Onlydigit(event) {
    if (event.which != 99 && event.which != 8 && event.which != 0 && isNaN(String.fromCharCode(event.which)) || event.key.toLowerCase() == 'spacebar' || event.key == ' ') {
        event.preventDefault(); //stop characters from entering input
    }
}

function validateEmail(email) {
    var re = /^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
    return re.test(email);
}

function ValidateWebURL(url) {
    var re = /(https?:\/\/(?:www\.|(?!www))[^\s\.]+\.[^\s]{2,}|www\.[^\s]+\.[^\s]{2,})/
    return re.test(url);
}

var luhnChk = (function (arr) {
    return function (ccNum) {
        var
            len = ccNum.length,
            bit = 1,
            sum = 0,
            val;

        while (len) {
            val = parseInt(ccNum.charAt(--len), 10);
            sum += (bit ^= 1) ? arr[val] : val;
        }

        return sum && sum % 10 === 0;
    };
}([0, 2, 4, 6, 8, 1, 3, 5, 7, 9]));

function checkABA(s) {

    var i, n, t;

    // First, remove any non-numeric characters.

    t = "";
    for (i = 0; i < s.length; i++) {
        c = parseInt(s.charAt(i), 10);
        if (c >= 0 && c <= 9)
            t = t + c;
    }

    // Check the length, it should be nine digits.

    if (t.length != 9)
        return false;

    // Now run through each digit and calculate the total.

    n = 0;
    for (i = 0; i < t.length; i += 3) {
        n += parseInt(t.charAt(i), 10) * 3
          + parseInt(t.charAt(i + 1), 10) * 7
          + parseInt(t.charAt(i + 2), 10);
    }

    // If the resulting sum is an even multiple of ten (but not zero),
    // the aba routing number is good.

    if (n != 0 && n % 10 == 0)
        return true;
    else
        return false;
}

function getUrlVars() {
    var vars = [], hash;
    var hashes = window.location.href.slice(window.location.href.indexOf('?') + 1).split('&');
    for (var i = 0; i < hashes.length; i++) {
        hash = hashes[i].split('=');
        vars.push(hash[0]);
        var paramvalue = hash[1];
        if (hash[1])
            if (hash[1].indexOf('#') > 0) {
                paramvalue = hash[1].split('#')[0];
            }
        vars[hash[0]] = paramvalue;
    }
    return vars;
}

function MyHierarchy(UserId) {
    //var UserId = $("#UserId").val();
    $('#hdnSubSiteActiveStatus').val(0);
    var Uri = '/api/dropdown/Hierarchy';
    ajaxHelper(Uri + '?id=' + UserId, 'GET').done(function (data, status) {

        $.each(data, function (indx, r) {

            if (r["ActiveStatus"] == '1' || r["ActiveStatus"] == 1) {

                $('#hdnSubSiteActiveStatus').val(1);
                $('input[type=text].svbfee').attr('readonly', 'readonly');
                $('input[type=text].transfee').attr('readonly', 'readonly');

                //$('#divBankAddOn input[type=text]').attr('readonly', 'readonly');
                //$('#divBankAddOn_MSO input[type=text]').attr('readonly', 'readonly');
                //$('#divBankAddOn input[type=text]').attr('readonly', 'readonly');
                //$('#divBankAddOn_MSO input[type=text]').attr('readonly', 'readonly');
            }

        });
    });
}

function fnVerifiedLinksStatus() {
    //  var IsVerified = false;
    var IsVerified = $('#IsVerified').val();
    //alert('sd');
    // alert(IsVerified);
    //alert(IsVerified);
    //if ($('#entityid').val() != $('#myentityid').val()) {

    //    $('#btn_edit').text('Edit');
    //    $('#btn_next').show();
    //    IsVerified = true;
    //}
    //else {
    //  var IsVerified = localStorage.getItem('IsVerified');

    //if ($('#ActiveMyAccountStatus').val() == "1" || $('#ActiveMyAccountStatus').val() == 1) {
    //    IsVerified = true;
    //    $('#IsVerified').val(true);
    //}

    var formid = $('#formid').attr('sitemapid');

    if (IsVerified == true || IsVerified == 'true' || IsVerified == 'True') {
        IsVerified = true;
        $('#btn_edit').text('Edit');
        // $('#btn_next').show();
        //if ($('#entityid').val() == $('#Entity_SO').val() || $('#entityid').val() == $('#Entity_SOME').val() || $('#entityid').val() == $('#Entity_SOME_SS').val() ||
        //    $('#myentityid').val() == $('#Entity_SO').val() || $('#myentityid').val() == $('#Entity_SOME').val() || $('#myentityid').val() == $('#Entity_SOME_SS').val()) {
        //    if(formid=='7c8aa474-2535-4f69-a2ae-c3794887f92d')
        //        $('#btn_next').show();
        //}
        //else
        //    $('#btn_next').show();

        //$('#divEnrollOfficeInfoForm #btn_next').hide();
        //var hdnEFINStatus = $('#hdnEFINStatus').val();
        //if (hdnEFINStatus == '16' || hdnEFINStatus == '19' || hdnEFINStatus == '21') {
        //    $('#divEnrollOfficeInfoForm #btn_next').show();
        //}

    } else {
        $('#btn_edit').text('Verify');
        //  $('#btn_next').hide();
        IsVerified = false;
    }


    //  }


    //setTimeout(function () {
    //    if (!IsVerified) {
    //        //Office
    //        $('#site0eda5d25-591c-4e01-a845-fb580572ade8').hide();
    //        $('#site68882c05-5914-4fdb-b284-e33d6c029f5a').hide();
    //        $('#sitec81dddc4-4654-4775-a5cd-74efa99dfc90').hide();
    //        $('#site60025459-7568-4a77-b152-f81904aaaa63').hide();
    //        $('#site4fc65d1b-675f-4985-8022-9e8bd0ed735f').hide();

    //        //Sub-Site
    //        $('#site2639fb0a-0caa-47cf-b315-587e7ce86aef').hide();
    //        $('#sited8d06578-1923-4792-bdad-153603b57068').hide();
    //        $('#sited8d06578-1923-4792-bdad-153603b57068').hide();

    //        //Enrollment
    //        $('#sitefc32db13-6aec-488e-bafe-19acb3399e57').hide();
    //        $('#site2f7d1b90-78aa-4a93-85ec-81cd8b10a545').hide();
    //        $('#sitea55334d1-3960-44c4-8cf1-e3ba9901f2be').hide();
    //        $('#site067c03a3-34f1-4143-beae-35327a8fca44').hide();
    //        $('#site0feeb0fe-d0e7-4370-8733-dd5f7d2041fc').hide();
    //        $('#site98a706d7-031f-4c5d-8cc4-d32cc7658b69').hide();
    //        $('#site0eda5d25-591c-4e01-a845-fb580572cfe8').hide();
    //        $('#site0eda5d25-591c-4e01-a845-fb580572cff5').hide();
    //    }
    //})
}

function SaveConfigStatusActive(status, bankid, type, reset) {

    var UserId = $('#UserId').val();
    var CustomerId = $('#UserId').val();

    if ($('#entityid').val() != $('#myentityid').val()) {
        CustomerId = $('#myid').val();
    }

    var ActiveLinkSiteMapId = '';

    if (type == 'subsite' && reset == 'reset') {
        ActiveLinkSiteMapId = "7a2c166c-c2ef-47c0-aa5f-e4950f9ff369";
    } else if (type == 'mainsite' && reset == 'reset') {
        ActiveLinkSiteMapId = "4fc65d1b-675f-4985-8022-9e8bd0ed735f";
    }

    var FormSiteMapId = $('#formid').attr('sitemapid');

    if (bankid == undefined || bankid == null || bankid == '') {
        bankid = '00000000-0000-0000-0000-000000000000'
    }

    var Uri = '/api/Configuration/Save';
    ajaxHelper(Uri + '?CustomerId=' + CustomerId + '&UserId=' + UserId + '&SiteMapID=' + FormSiteMapId + '&resettype=' + reset + '&ActiveLinkSiteMapID=' + ActiveLinkSiteMapId + '&status=' + status + '&bankid=' + bankid, 'POST', null, false).done(function (data, status) {

    });
    return true;
}

function ResetSubSiteActivation() {

    var UserId = $('#UserId').val();
    var CustomerId = $('#UserId').val();

    if ($('#entityid').val() != $('#myentityid').val()) {
        CustomerId = $('#myid').val();
    }

    var ActiveLinkSiteMapId = "7a2c166c-c2ef-47c0-aa5f-e4950f9ff369";

    var Uri = '/api/Configuration/ResetActivation';
    ajaxHelper(Uri + '?CustomerId=' + CustomerId + '&UserId=' + UserId + '&ActiveLinkSiteMapID=' + ActiveLinkSiteMapId, 'POST', null, false).done(function (data, status) {
    });

    if (CustomerId != "") {
        GetSubSiteCustomerInfomationForMainOffice(CustomerId);
    }
}

function ResetMainSiteActivation() {

    var UserId = $('#UserId').val();
    var CustomerId = $('#UserId').val();

    if ($('#entityid').val() != $('#myentityid').val()) {
        CustomerId = $('#myid').val();
    }

    var ActiveLinkSiteMapId = "4fc65d1b-675f-4985-8022-9e8bd0ed735f";

    var Uri = '/api/Configuration/ResetActivation';
    ajaxHelper(Uri + '?CustomerId=' + CustomerId + '&UserId=' + UserId + '&ActiveLinkSiteMapID=' + ActiveLinkSiteMapId, 'POST', null, false).done(function (data, status) {

    });
}

function fnHideOffMgmt() {
    if ($('#siteb303a3e8-1a4f-4638-b31c-50d1b4b8db34').length > 0) {
        $('#siteb303a3e8-1a4f-4638-b31c-50d1b4b8db34').hide();
        clearInterval(hideoffcmgmt);
    }
}

function fnHideNewcustomers() {
    if ($('#site6ed791bd-1909-4e7a-b6b1-b76983faef30').length > 0) {
        $('#site6ed791bd-1909-4e7a-b6b1-b76983faef30').hide();
        clearInterval(hidenewcustomer);
    }
}

var hideoffcmgmt = null;
var hidenewcustomer = null;

function getPermissions() {

    var entityid = $('#entityid').val();

    if (entityid == $('#Entity_uTax').val()) {

        var UserId = $('#LoginId').val();
        var uri = '/api/Sitemap/GetUserRolePermissions?UserId=' + UserId;
        ajaxHelper(uri, 'GET', null, false).done(function (data) {
            if (data) {

                var _viewom = true, _viewncs = true;

                if (data.OfficeManamgement) {
                    if (!data.OfficeManamgement.ViewPermission) {
                        _viewom = false;
                        hideoffcmgmt = setInterval(fnHideOffMgmt, 500);
                    }
                    _canresetpwd = data.OfficeManamgement.ResetPassword;
                    _addsuboffice = data.OfficeManamgement.AddSubOffice;
                    _canmanageOfc = data.OfficeManamgement.OfficeMgmt;
                    _manageEnroll = data.OfficeManamgement.EnrollmentMgmt;
                    _unlockEnroll = data.OfficeManamgement.UnlockEnrollment;
                    _archiveInfo = data.OfficeManamgement.ArchiveInfo;
                    _showpassword = data.OfficeManamgement.ShowPassword;
                }

                if (data.NewCustomer) {
                    if (!data.NewCustomer.ViewPermission) {
                        _viewncs = false;
                        hidenewcustomer = setInterval(fnHideNewcustomers, 500);
                    }
                    _canupdateswid = data.NewCustomer.Updatesw;
                    _canviewinfo = data.NewCustomer.ViewCustomerInfo;
                }

                if (data.ReportPermissions) {
                    _feeroprtview = data.ReportPermissions.FeeReport;
                    _nobankappreportviewe = data.ReportPermissions.NoBankApp;
                    _enrstatusreportview = data.ReportPermissions.Enrollstatus;
                    _loginreportview = data.ReportPermissions.LoginReport;
                }

                if (!_viewom && !_viewncs && !_feeroprtview && !_nobankappreportviewe && !_enrstatusreportview && !_loginreportview) {
                    if (window.location.href.toLowerCase().indexOf('nopermissions') < 0) {
                        window.location.href = '/CustomerInformation/NoPermissions';
                        return;
                    }
                }

                //if (data.NewCustomer) {
                //    if (!data.NewCustomer.ViewPermission) {
                //        if (window.location.href.indexOf('AllCustomerInfo') < 0 && window.location.href.toLowerCase().indexOf('reports') < 0) {
                //            if (data.OfficeManamgement.ViewPermission)
                //                window.location.href = '/CustomerInformation/AllCustomerInfo';
                //            else {
                //                if (data.ReportPermissions.FeeReport)
                //                    window.location.href = '/Reports/Index';
                //                if (data.ReportPermissions.NoBankApp)
                //                    window.location.href = '/Reports/NoBankAppSubmission';
                //                if (data.ReportPermissions.Enrollstatus)
                //                    window.location.href = '/Reports/EnrollmentStatus';
                //                if (data.ReportPermissions.LoginReport)
                //                    window.location.href = '/Reports/LastLoginInfo';
                //            }
                //        }
                //        setTimeout(function () {
                //            $('#site6ed791bd-1909-4e7a-b6b1-b76983faef30').hide();
                //        })
                //    }
                //    _canupdateswid = data.NewCustomer.Updatesw;
                //    _canviewinfo = data.NewCustomer.ViewCustomerInfo;
                //}
                //if (data.OfficeManamgement) {
                //    if (!data.OfficeManamgement.ViewPermission) {
                //        setTimeout(function () {
                //            $('#siteb303a3e8-1a4f-4638-b31c-50d1b4b8db34').hide();
                //        })
                //    }
                //    _canresetpwd = data.OfficeManamgement.ResetPassword;
                //    _addsuboffice = data.OfficeManamgement.AddSubOffice;
                //    _canmanageOfc = data.OfficeManamgement.OfficeMgmt;
                //    _manageEnroll = data.OfficeManamgement.EnrollmentMgmt;
                //}

                //if (data.ReportPermissions) {
                //    _feeroprtview = data.ReportPermissions.FeeReport;
                //    _nobankappreportviewe = data.ReportPermissions.NoBankApp;
                //    _enrstatusreportview = data.ReportPermissions.Enrollstatus;
                //    _loginreportview = data.ReportPermissions.LoginReport;
                //}
            }
        })
    }
    else {
        var uri = '/api/Sitemap/GetPartnerPermissions';
        ajaxHelper(uri, 'GET', null, false).done(function (data) {
            if (data) {

                if (data.OfficeManamgement) {                    
                    _showpassword = data.OfficeManamgement.ShowPassword;
                }
            }
        })
    }
}

function EnrollmentMenuStatus(ConfigStatus) {
    if (ConfigStatus.length > 0) {

        // Bank Select Selectd as None then Following Conditions
        var enrollstatus = ConfigStatus.filter(function (i) { return i.SitemapId == '067c03a3-34f1-4143-beae-35327a8fca44' && i.Status == 'none' });

        //var EFINOwnerUserId = false;
        ////var enrollstatus = ConfigStatus.filter(function (i) { return i.SitemapId == 'c333b448-b287-4c08-929b-0797f986ca6f' && i.Status == 'none' });
        //btn_next
        if ($('#entityid').val() != $('#myentityid').val()) {
            EFINOwnerUserId = localStorage.getItem("EFINOwnerUserId");
        } else {
            if ($('#EFINOwnerUserId').val() == 'true' || $('#EFINOwnerUserId').val() == 'True' || $('#EFINOwnerUserId').val() == true) {
                EFINOwnerUserId = true;
            }
        }
        var CanSubSiteLoginToEmp = $('#CanSubSiteLoginToEmp').val();

        if (enrollstatus.length > 0) {
            $('#site0feeb0fe-d0e7-4370-8733-dd5f7d2041fc').remove();
            $('#sitea55334d1-3960-44c4-8cf1-e3ba9901f2be').remove();
            $('#site98a706d7-031f-4c5d-8cc4-d32cc7658b69').show();
            $('#btnBankEnrollSubmit').remove();
            $('#libankenrollment').hide();
            $('#lifee').hide();
        }

        //EFINOwnerUserId == true || EFINOwnerUserId == 'true' || EFINOwnerUserId == 'True' || 
        if (CanSubSiteLoginToEmp == 'false' || CanSubSiteLoginToEmp == 'False' || CanSubSiteLoginToEmp == false) {
            $('#site067c03a3-34f1-4143-beae-35327a8fca44').remove();
            $('#site0feeb0fe-d0e7-4370-8733-dd5f7d2041fc').remove();
            $('#site98a706d7-031f-4c5d-8cc4-d32cc7658b69').remove();
            $('#btnEnrollAffSaveNext').hide();
            $('#btnEnrollAffSaveNext').remove();
        }

    }
}

function SubSiteMenuStatus(ConfigStatus) {
    var ActiveMyAccount = 0;
    var n = 0;

    if (ConfigStatus.length > 0) {
        for (var i = 0, len = ConfigStatus.length; i < len; i++) {

            if (ConfigStatus[i].SitemapId == "1f2a6418-f9bc-4878-ab0b-2fa004c01c01" && ConfigStatus[i].Status == "done") {
                n = n + 1;
            }
            if (ConfigStatus[i].SitemapId == "2639fb0a-0caa-47cf-b315-587e7ce86aef" && ConfigStatus[i].Status == "done") {
                n = n + 1;
            }
            if (ConfigStatus[i].SitemapId == "d8d06578-1923-4792-bdad-153603b57068" && ConfigStatus[i].Status == "done") {
                n = n + 1;
            }

            if (ConfigStatus[i].SitemapId == "7a2c166c-c2ef-47c0-aa5f-e4950f9ff369" && ConfigStatus[i].Status == "done") { // Service Transmi 
                ActiveMyAccount = 1;
                $('#ActiveMyAccountStatus').val('1');
            }
        }
    }

    $('#site7a2c166c-c2ef-47c0-aa5f-e4950f9ff369').hide();
    if (ActiveMyAccount == 0) {
        if (Number(n) >= 3) {
            $('#site7a2c166c-c2ef-47c0-aa5f-e4950f9ff369').show();
            $('#site7a2c166c-c2ef-47c0-aa5f-e4950f9ff369').attr('href', '/SubSiteOfficeConfiguration/ActivateMyAccount');
        }
    }
}

function SetLogoutSession() {
    localStorage.setItem("IsLoggedout", "true");
}
//    //var UserId = 0;
//    //if ($('#entitydisplayid').val() != $('#myentitydisplayid').val()) {
//    //    UserId = $('#MoSvb_ParentId').val();
//    //} else {
//    //    UserId = $('#UserId').val();
//    //}

//    //var Uri = '/api/CustomerLogin/TokenKill?TokenID=' + $('#Token').val();
//    //ajaxHelper(Uri, 'POST').done(function (data, status) {

//    //});
//    // window.location.href = '/CustomerLogin/';
//    //ajaxHelper('CustomerLogin/Logout', 'POST').done(function (data) {
//    //    window.location.href = '/CustomerLogin/';
//    //});
//}

function checkLoginStatus() {
    setInterval(function () {
        var isLoggedout = localStorage.getItem("IsLoggedout");
        if (isLoggedout) {
            if (isLoggedout == 'true') {
                $('#popupLogout').modal('show');
            }
        }
    }, 5000);
}

function Closetab() {
    window.close();
    window.location.href = '/CustomerLogin/Index';
}

function fnEFINReadonly() {

    var entityid = $('#entityid').val();
    var myentityid = $('#myentityid').val();

    var Entity_uTax = $('#Entity_uTax').val();

    if (entityid != Entity_uTax) {
        if (entityid == myentityid) {
            //$('#divConfigCustInfoCreateForm input[type=text]#EFIN').attr('readonly', 'readonly');
            $('input[type=text]#EFIN').attr('readonly', 'readonly');
            $('select#EFINStatus').attr('disabled', 'disabled');
        }
    }
    else {
        if ($('#myentitydisplayid').val() == $('#BaseEntity_AESS').val()) {
            $('input[type=text]').attr('readonly', 'readonly');
            $('select').attr('disabled', 'disabled');
            $('input[type=checkbox]').attr('disabled', 'disabled');
            $('input[type=text]#EFIN').removeAttr('readonly');
            $('select#EFINStatus').removeAttr('disabled', 'disabled');
        }
    }
}

function getIsEnrollmentSubmit() {
    var url = '/api/SubSiteFee/IsEnrollmentSubmit';
    var Id;
    if ($('#entityid').val() != $('#myentityid').val()) {

        if ($('#myid').length > 0) {
            Id = $('#myid').val();
        } else {
            Id = $('#myid').val();// $('#SubSite_ParentId').val();
        }
    }
    else {
        Id = $('#UserId').val();
    }

    var _flag = true;
    if (Id != '' && Id != null && Id != '00000000-0000-0000-0000-000000000000') {
        ajaxHelper(url + '?Id=' + Id, 'POST').done(function (data) {
            if (data == 'true' || data == 'True' || data == true) {
                _flag = true;

                //Main Configuration
                $('#divMainOfficeConfigForm input').attr('disabled', 'disabled');
                $('#divMainOfficeConfigSave a').attr('disabled', 'disabled');
                $('#divMainOfficeConfigSave a').css('pointer-events', 'none');
                $('#dvBSW input').removeAttr('disabled');


                //SubSite Configuration
                $('#divSubSiteConfigForm input').attr('disabled', 'disabled');
                $('#divSubSiteConfigForm a.btn-info').attr('disabled', 'disabled');
                $('#divSubSiteConfigForm a.btn-info').css('pointer-events', 'none');

                //Fee Setup & Config
                $('#divFeeSetupConfigForm input').attr('disabled', 'disabled');
                $('#divFeeSetupConfigForm a.btn-info').attr('disabled', 'disabled');
                $('#divFeeSetupConfigForm a.btn-info').css('pointer-events', 'none');


                //$('#divFeeSetupConfigForm').attr('disabled', 'disabled');

                //  SubSIte Office COnfig
                $('#divSubSiteOfficeConfigForm input').attr('disabled', 'disabled');
                $('#divSubSiteOfficeConfigForm a').attr('disabled', 'disabled');
                $('#divSubSiteOfficeConfigForm a').css('pointer-events', 'none');


                // SubSIte Office Fee COnfig //12072016
                //$('#divSubSiteOfficeFeeConfigForm input').attr('disabled', 'disabled');
                //$('#divSubSiteOfficeFeeConfigForm a').attr('disabled', 'disabled');
                //$('#divSubSiteOfficeFeeConfigForm a').css('pointer-events', 'none');

                // SubSIte Office Dash
                $('#divSubSiteOfficeDashboardForm input').attr('disabled', 'disabled');
                $('#divSubSiteOfficeDashboardForm a').attr('disabled', 'disabled');
                $('#divSubSiteOfficeDashboardForm a').css('pointer-events', 'none');



            }
            else {
                _flag = false;
            }
        })
    }
    return _flag;
}


//function getIsEnrollmentSubmit() {
//    var url = '/api/SubSiteFee/IsEnrollmentSubmit';
//    var Id;
//    if ($('#entitydisplayid').val() != $('#myentitydisplayid').val()) {
//        Id = $('#MoSvb_ParentId').val();
//    }
//    else {
//        Id = $('#UserId').val();
//    }
//    var _flag = true;
//    if (Id != '' && Id != null && Id != '00000000-0000-0000-0000-000000000000') {
//        ajaxHelper(url + '?Id=' + Id, 'POST').done(function (data) {
//            if (data == 'true' || data == 'True' || data == true) {
//                $('input').attr('disabled', 'disabled');
//                $('a.btn-info').attr('disabled', 'disabled');
//                $('a.btn-info').css('pointer-events', 'none');
//            }
//            else {
//                _flag = false;
//            }
//        })
//    }
//    return _flag;
//}



//function getIsEnrollmentSubmit() {
//    var url = '/api/SubSiteFee/IsEnrollmentSubmit';
//    var Id;
//    if ($('#entitydisplayid').val() != $('#myentitydisplayid').val()) {
//        Id = $('#MoSvb_ParentId').val();
//    }
//    else {
//        Id = $('#UserId').val();
//    }
//    var _flag = true;
//    if (Id != '' && Id != null && Id != '00000000-0000-0000-0000-000000000000') {
//        ajaxHelper(url + '?Id=' + Id, 'POST').done(function (data) {
//            if (data == 'true' || data == 'True' || data == true) {
//                $('input').attr('disabled', 'disabled');
//                $('a.btn-info').attr('disabled', 'disabled');
//                $('a.btn-info').css('pointer-events', 'none');
//            }
//            else {
//                _flag = false;
//            }
//        })
//    }
//    return _flag;
//}



function fnSaveuTaxNotCollectingSVBFee(obj) {

    var uTaxNotCollectingSVBFee = obj;
    var UserId = 0;
    if ($('#entityid').val() != $('#myentityid').val()) {
        UserId = $('#myid').val();
    } else {
        UserId = $('#UserId').val();
    }

    var param = 'uTaxNotCollectingSVBFee=' + uTaxNotCollectingSVBFee + '&Id=' + UserId;
    var Uri = '/api/CustomerInformation/SaveuTaxNotCollectingSVBFee';
    ajaxHelper(Uri + '?' + param, 'POST').done(function (data, status) {

    });
}

Array.prototype.contains = function (v) {
    for (var i = 0; i < this.length; i++) {
        if (this[i].Parent === v) return true;
    }
    return false;
};

Array.prototype.unique = function () {
    var arr = [];
    for (var i = 0; i < this.length; i++) {
        if (!arr.contains(this[i].Parent)) {
            arr.push(this[i]);
        }
    }
    return arr;
}

//////SUB SITE BANK AND SUB QUESTIONS

function getSubsiteBankAndQuestions(container, container1) {

    var issubsitemsouser = $('#issubsitemsouser').val();

    container.html('');
    container1.html('');

    $('#divSVB').hide();
    $('#divTrans').hide();

    // $('#dvNotDataPageSVB').show();
    // $('#dvNotDataPageTrans').show();

    $('#hdnSVBNoBank').val(1);
    $('#hdnTranNoBank').val(1);

    //var parentid, custmorUri;

    //if ($('#entityid').val() != $('#myentityid').val()) {
    //    parentid = $('#myparentid').val();
    //}
    //else {
    //    parentid = $('#parentid').val();
    //    if ($('#entitydisplayid').val() == $('#Entity_SOMESubSite').val()) {
    //        parentid = $('#supparentid').val();
    //    }
    //}

    var CustomerId = $('#UserId').val();
    if ($('#entityid').val() != $('#myentityid').val()) {
        CustomerId = $('#myid').val();
    }

    if (issubsitemsouser != '' && (issubsitemsouser == '0' || issubsitemsouser == '1')) {
        custmorUri = '/api/dropdown/subsitebank?id=' + CustomerId;

        var $table = $('<table/>').addClass('table table-striped table-hover table-bordered');//; <table class="table table-striped table-hover table-bordered">
        var $tbody = $('<tbody />');
        var $table1 = $('<table/>').addClass('table table-striped table-hover table-bordered');//; <table class="table table-striped table-hover table-bordered">
        var $tbody1 = $('<tbody />');
        var IsDataExist = false;

        ajaxHelper(custmorUri, 'GET', null, false).done(function (data) {
            if (data != null && data != '' && data != undefined) {
                $.each(data, function (colIndex, c) {
                    if (c["ServiceorTransmission"] == 1 || c["ServiceorTransmission"] == 0) {
                        $('#divSVB').show();
                        $('#dvNotDataPageSVB').hide();
                        $('#hdnSVBNoBank').val(0);

                        var SVBDesktopFee = c["BankSVBDesktopFee"];
                        var SVBMSOFee = c["BankSVBMSOFee"];

                        var DesktopFee = c["DesktopFee"];
                        var MSOFee = c["MSOFee"];
                        //GD-11042016
                        //var SubDesktopFee = c["SubDesktopFee"];
                        //var SubMSOFee = c["SubMSOFee"];

                        var MainSiteFee = $('#hdnTotalSVBFees').val();


                        if (SVBDesktopFee == '' || SVBDesktopFee == null || SVBDesktopFee == undefined) {
                            SVBDesktopFee = 0;
                        }

                        if (DesktopFee == '' || DesktopFee == null || DesktopFee == undefined) {
                            DesktopFee = 0;
                        }

                        if (MainSiteFee == '' || MainSiteFee == null || MainSiteFee == undefined) {
                            MainSiteFee = 0;
                        }

                        var BankFee = Number(MainSiteFee); // Number(DesktopFee) + 

                        var DispSVBDesktopFee = Number(SVBDesktopFee) - Number(BankFee);



                        if (SVBMSOFee == '' || SVBMSOFee == null || SVBMSOFee == undefined) {
                            SVBMSOFee = 0;
                        }

                        if (MSOFee == '' || MSOFee == null || MSOFee == undefined) {
                            MSOFee = 0;
                        }

                        if (MainSiteFee == '' || MainSiteFee == null || MainSiteFee == undefined) {
                            MainSiteFee = 0;
                        }

                        var MSOBankFee = Number(MainSiteFee); //Number(MSOFee) +

                        var DispSVBMSOFee = Number(SVBMSOFee) - Number(MSOBankFee);

                        //if (c["ServiceorTransmission"] != 1) {
                        //    DesktopFee = 0;
                        //    MSOFee = 0;
                        //}

                        DispSVBMSOFee = DispSVBMSOFee.toFixed(2);
                        DispSVBDesktopFee = DispSVBDesktopFee.toFixed(2);

                        var bankid = c["BankId"];
                        var bankname = c["BankName"];
                        var isSameforAll = c["IsSameforAll"];

                        var $tr = $('<tr/>');
                        var $td1 = $('<td style="width:20%;"/>');
                        var $td2 = $('<td style="width:30%;"/>');

                        $td1.append('<div class="form-group"><label>' + bankname + '</label></div>');

                        var $divbank = $('<div/>').addClass('form-group');

                        if (issubsitemsouser == '0' || issubsitemsouser == 0) {
                            $divbank.append('<div class="fee-amount"><span>Desktop : ($' + DispSVBDesktopFee + ') </span> <input readonly="readonly" maxfee="' + SVBDesktopFee + '" bankfee="' + BankFee + '" type="text" Id="txt_' + bankid + '"  name="txt_' + bankid + '" value="' + DesktopFee + '" class="form-control ServiceBank SVBBank decimal" maxlength="8" AmountType="0" bankid=' + bankid + '></div>');
                        } else {
                            $divbank.append('<div class="fee-amount"><span>MSO : ($' + DispSVBMSOFee + ') </span> <input  readonly="readonly" maxfee="' + SVBMSOFee + '" bankfee="' + MSOBankFee + '" type="text" Id="txt_' + bankid + '_MSO"  name="txt_' + bankid + '_MSO" value="' + MSOFee + '"  class="form-control ServiceBank decimal" maxlength="8" AmountType="1" bankid=' + bankid + '></div>');
                        }

                        $td2.append($divbank);
                        var $td3 = $('<td style="width:50%;"/>')
                        var $head = $('<div/>').addClass('form-group bank-details');
                        if (c.Questions.length > 0) {
                            $head.attr('id', 'divBankQuestions' + bankid);
                            $head.append('<label>How will your Sub-sites print their ' + bankname + ' Checks?</label>');
                        }
                        $.each(c.Questions, function (rowIndex, r) {
                            IsDataExist = true;
                            $head.append('<div class="radio"><label><input type="radio" Id="rb_' + r["Id"] + '"  name="bank' + bankid + '" class="rbBank' + bankid + '" value="' + r["Id"] + '" checked /><span class="circle"></span><span class="check"></span> ' + r["Name"] + '</label></div>');
                        });
                        if (IsDataExist) {
                            $td3.append($head);
                        }
                        $tr.append($td1);
                        $tr.append($td2);
                        $tr.append($td3);
                        $tbody.append($tr);
                    }
                });

                $table.append($tbody);
                container.append($table);
                ///this below transmitter
                $.each(data, function (colIndex, c) {
                    if (c["ServiceorTransmission"] == 2 || c["ServiceorTransmission"] == 0) {
                        $('#divTrans').show();
                        $('#dvNotDataPageTrans').hide();
                        $('#hdnTranNoBank').val(0);

                        var TranDesktopFee = c["BankTranDesktopFee"];
                        var TranMSOFee = c["BankTranMSOFee"];

                        var bankid = c["BankId"];
                        var bankname = c["BankName"];
                        var isSameforAll = c["IsSameforAll"];

                        var DesktopFee = c["DesktopFee"];
                        var MSOFee = c["MSOFee"];

                        //GD-11042016
                        //var SubDesktopFee = c["SubDesktopFee"];
                        //var SubMSOFee = c["SubMSOFee"];


                        var MainSiteFee = 0;// $('#hdnTotalTransFees').val();

                        if (TranDesktopFee == '' || TranDesktopFee == null || TranDesktopFee == undefined) {
                            TranDesktopFee = 0;
                        }

                        if (DesktopFee == '' || DesktopFee == null || DesktopFee == undefined) {
                            DesktopFee = 0;
                        }

                        if (MainSiteFee == '' || MainSiteFee == null || MainSiteFee == undefined) {
                            MainSiteFee = 0;
                        }

                        var TranBankFee = Number(MainSiteFee); //Number(DesktopFee) + 

                        var DispTranDesktopFee = Number(TranDesktopFee) - Number(TranBankFee);

                        if (TranMSOFee == '' || TranMSOFee == null || TranMSOFee == undefined) {
                            TranMSOFee = 0;
                        }

                        if (MSOFee == '' || MSOFee == null || MSOFee == undefined) {
                            MSOFee = 0;
                        }

                        if (MainSiteFee == '' || MainSiteFee == null || MainSiteFee == undefined) {
                            MainSiteFee = 0;
                        }

                        var TranMSOBankFee = Number(MainSiteFee); //Number(MSOFee) +

                        var DispTranMSOFee = Number(TranMSOFee) - Number(TranMSOBankFee);


                        DispTranDesktopFee = DispTranDesktopFee.toFixed(2);
                        DispTranMSOFee = DispTranMSOFee.toFixed(2);

                        var $tr = $('<tr/>');
                        var $td1 = $('<td style="width:20%;"/>');
                        var $td2 = $('<td style="width:30%;"/>');
                        var $divbank = $('<div/>').addClass('form-group');

                        $td1.append('<div class="form-group"><label>' + bankname + '</label></div>');

                        if (issubsitemsouser == '0' || issubsitemsouser == 0) {
                            $divbank.append('<div class="fee-amount"><span> Desktop : ($' + DispTranDesktopFee + ') </span> <input  readonly="readonly" maxfee="' + TranDesktopFee + '" bankfee="' + TranBankFee + '" type="text" Id="txttrns_' + bankid + '"  name="txttrns_' + bankid + '"  value="' + DesktopFee + '" class="form-control TransmitterBank decimal TransBank" maxlength="8" AmountType="0" bankid=' + bankid + '></div>');
                        } else {
                            $divbank.append('<div class="fee-amount"><span>MSO : ($' + DispTranMSOFee + ') </span> <input  readonly="readonly"  maxfee="' + TranMSOFee + '" bankfee="' + TranMSOBankFee + '" type="text" Id="txttrns_' + bankid + '_MSO"  name="txttrns_' + bankid + '_MSO" value="' + MSOFee + '" class="form-control TransmitterBank decimal" maxlength="8" AmountType="1" bankid=' + bankid + '></div>');
                        }

                        $td2.append($divbank);
                        var $td3 = $('<td style="width:50%;"/>')
                        //var $head = $('<div/>').addClass('form-group bank-details');
                        //if (c.Questions.length > 0) {
                        //    $head.attr('id', 'divBankQuestions' + bankid);
                        //    $head.append('<label>How will your Sub-sites print their ' + bankname + ' Checks?</label>');
                        //}
                        //$.each(c.Questions, function (rowIndex, r) {
                        //    IsDataExist = true;
                        //    $head.append('<div class="radio"><label><input type="radio" Id="rbtrn_' + r["Id"] + '"  name="bankTrn' + bankid + '" class="rbBankTrn' + bankid + '" value="' + r["Id"] + '" checked/><span class="circle"></span><span class="check"></span> ' + r["Name"] + '</label></div>');
                        //});
                        //if (IsDataExist) {
                        //    $td3.append($head);
                        //}
                        $tr.append($td1);
                        $tr.append($td2);
                        $tr.append($td3);
                        $tbody1.append($tr);
                    }
                });
                $table1.append($tbody1);
                container1.append($table1);
                ///till here
                getIsSalesYearCheckBankDates_SubSite();
            }


        });

        IsNextLinkActive();
    }
    else {
        $('#divSVB').hide();
        $('#divTrans').hide();
        $('#divTansmNextSave').hide();

        $('#dvNotDataPageSVB')
            .show()
            .html('Please save office configuration first');

        $('#dvNotDataPageTrans')
            .show()
            .html('Please save office configuration first');
    }


}

function IsNextLinkActive() {

    $("#spanNextLink").hide();
    if (($('#hdnSVBNoBank').val() == '1' || $('#hdnSVBNoBank').val() == 1) && ($('#hdnTranNoBank').val() == '1' || $('#hdnTranNoBank').val() == 1)) {
        $("#spanNextLink").show();
    }
}

function isValidDateFormat(date) {
    var matches = /^(\d{1,2})[-\/](\d{1,2})[-\/](\d{4})$/.exec(date);
    if (matches == null) return false;
    var d = matches[2];
    var m = matches[1] - 1;
    var y = matches[3];
    var composedDate = new Date(y, m, d);
    return composedDate.getDate() == d &&
            composedDate.getMonth() == m &&
            composedDate.getFullYear() == y;
}

//function fnHideMSOLoc() {
//    var ismsouser = $('#ismsouser').val();
//    if (ismsouser == 'false' || ismsouser == false) {
//        $('#divSubSiteMSOLoc').hide();
//    }
//}

function fnHideMSOLoc() {
    var ismsouser = $('#ismsouser').val();
    $('#divSubSiteMSOLoc').hide();
    if (ismsouser == 'true' || ismsouser == true) {
        $('#divSubSiteMSOLoc').show();
    }
}

function DashboardNext(obj) {
    if (obj == "subsite") {
        if ($('#btn_edit').text().toUpperCase() == 'VERIFY')
            return;
        var nextlnk = $('#site2639fb0a-0caa-47cf-b315-587e7ce86aef').attr('href');
        if (nextlnk)
            window.location.href = nextlnk;
    }
    else if (obj == "mainsite") {
        if ($('#btn_edit').text().toUpperCase() == 'VERIFY')
            return;
        var nextlnk = $('#site0eda5d25-591c-4e01-a845-fb580572ade8').attr('href');
        if (nextlnk)
            window.location.href = nextlnk;
    }
    else {
        if ($('#btn_edit').text().toUpperCase() == 'VERIFY')
            return;

        var nextlink = $('#sitefc32db13-6aec-488e-bafe-19acb3399e57').attr('href');
        if (nextlink)
            window.location.href = nextlink;
    }
}

function getRoundoff(number) {
    return Math.round(number * 100) / 100;
}

function isPOExist(address) {
    var po = 'PO BOX';
    var po1 = 'P O BOX';
    var po2 = 'P.O.';
    var po3 = 'P. O.';
    var po4 = 'P. O. BOX';
    var po5 = 'P. O BOX';

    if (address.toUpperCase().indexOf(po) >= 0 || address.toUpperCase().indexOf(po4) >= 0 || address.toUpperCase().indexOf(po1) >= 0 ||
        address.toUpperCase().indexOf(po2) >= 0 || address.toUpperCase().indexOf(po3) >= 0 || address.toUpperCase().indexOf(po5) >= 0)
        return true;
    else
        return false;
}

function UpdateOfficeManagement(UserId) {
    var SalesYearId = "";
    var url = '/api/OfficeManagement/update';
    ajaxHelper(url + '?id=' + UserId + '&salesyearid=' + SalesYearId, 'POST', null, false).done(function (data) {
        //alert(data);
    });
}

function getEFINStatus(container, myentityid) {
    //var myentityid = $('#myentityid').val();
    if (myentityid != '0' && myentityid != '' && myentityid != undefined && myentityid != null) {
        var url = '/api/dropdown/EFINStatus?entityid=' + myentityid;
        container.html('');
        ajaxHelper(url, 'GET', null, false).done(function (data) {
            container.append($('<option />', { value: '', text: 'Select' }));
            $.each(data, function (rowIndex, r) {
                container.append($('<option />', { value: r["Id"], text: r["Name"] }));
            });
        });
    }
}

function UpdateEFINAfterApproved(OldEFIN) {

    var UserId = $('#UserId').val();
    var CustomerId = $('#UserId').val();

    if ($('#entityid').val() != $('#myentityid').val()) {
        CustomerId = $('#myid').val();
    }

    if (OldEFIN != '' && OldEFIN != '0' && OldEFIN != undefined && OldEFIN != null) {
        var Uri = '/api/Configuration/UpdateEFINAfterApproved';
        ajaxHelper(Uri + '?CustomerId=' + CustomerId + '&UserId=' + UserId + '&OldEFIN=' + OldEFIN, 'POST', null, false).done(function (data, status) {
            return true;
        });
    }

    return true;
}

function UpdateFeeAfterApproved() {

    var UserId = $('#UserId').val();
    var CustomerId = $('#UserId').val();

    if ($('#entityid').val() != $('#myentityid').val()) {
        CustomerId = $('#myid').val();
    }

    var Uri = '/api/Configuration/UpdateFeeAfterApproved';
    ajaxHelper(Uri + '?CustomerId=' + CustomerId + '&UserId=' + UserId, 'POST', null, false).done(function (data, status) {
        return true;
    });

    return true;
}

function checkApprovedBankEnrollment() {

    var UserId = $('#UserId').val();
    var CustomerId = $('#UserId').val();

    if ($('#entityid').val() != $('#myentityid').val()) {
        CustomerId = $('#myid').val();
    }

    var Uri = '/api/Configuration/checkApprovedBankEnrollment';
    ajaxHelper(Uri + '?CustomerId=' + CustomerId, 'GET', null, false).done(function (data, status) {
        return data;
    });

    return false;
}

function checkSOSOMEActivation() {

    var UserId = $('#UserId').val();
    var CustomerId = $('#UserId').val();

    if ($('#entityid').val() != $('#myentityid').val()) {
        CustomerId = $('#myid').val();
    }

    var Uri = '/api/Configuration/GetSOSOMEActivation';
    ajaxHelper(Uri + '?CustomerId=' + CustomerId + '&UserId=' + UserId, 'POST', null, false).done(function (data, status) {
        return data;
    });
    return false;
}

function PadLeft(n, p, c) {
    var pad_char = typeof c !== 'undefined' ? c : '0';
    var pad = new Array(1 + p).join(pad_char);
    return (pad + n).slice(-pad.length);
}
//var fu = paddy(14, 5); // 00014
//var bar = paddy(2, 4, '#'); // ###2

function BankUrlAppend(bankid) {
    var vmenu = $('a.vmenu');
    $.each(vmenu, function (indx, valu) {
        var href = $(this).prop('href');
        href = updateQueryStringParameter(href, 'bankid', bankid)
        $(this).prop('href', href);
    });
    return false;
}

function updateQueryStringParameter(uri, key, value) {
    var re = new RegExp("([?&])" + key + "=.*?(&|$)", "i");
    var separator = uri.indexOf('?') !== -1 ? "&" : "?";
    if (uri.match(re)) {
        return uri.replace(re, '$1' + key + "=" + value + '$2');
    }
    else {
        return uri + separator + key + "=" + value;
    }
}

function GetActivityBankStatus(divActivityBankStatus) {
    var CustomerId = $('#UserId').val();
    if ($('#entityid').val() != $('#myentityid').val()) {
        CustomerId = $('#myid').val();
    }

    var Uri = '/api/BankSelection/getActivityBankStatus';
    ajaxHelper(Uri + '?CustomerId=' + CustomerId, 'POST', null, false).done(function (data, status) {

        if (data.ActiveBank != '' && data.ActiveBank != null && data.ActiveBank != undefined
            && data.EditingBank != '' && data.EditingBank != null && data.EditingBank != undefined) {
            divActivityBankStatus.html('Active Bank : ' + data.ActiveBank + '<br/>Last Edited : ' + data.EditingBank);
            divActivityBankStatus.show();
        }
    });
}

function OnlyDigits(event) {
    if (event.which != 99 && event.which != 8 && event.which != 0 && isNaN(String.fromCharCode(event.which)) || event.key.toLowerCase() == 'spacebar' || event.key == ' '){
        event.preventDefault();
    }
}





function getIsSalesYearCheckBankDates_SubSite() {
    var url = '/api/SubSiteFee/IsSalesYearBankLst';
    var Id;

    //if ($('#SubSite_ParentId').val() != "") {
    //    Id = $('#SubSite_ParentId').val();
    //}
    //else {
    //    Id = $('#UserId').val();
    //}
    if ($('#entityid').val() != $('#myentityid').val()) {
        Id = $('#myid').val();
    }
    else {
        Id = $('#UserId').val();
    }


    var error = $('#error_bank');
    error.html('');
    error.hide();

    var _flag = true;
    var _flag_A = false;
    if (Id != '' && Id != null && Id != '00000000-0000-0000-0000-000000000000') {
        ajaxHelper(url + '?Id=' + Id, 'POST').done(function (data) {
            $.each(data, function (ind, valu) {
                var Bankid = valu["BankId"];
                var Active = valu["Active"];
                if (Active == false) {
                    var stitle = 'The ability to update the Fees information is not available since the Cutoff date for the same is elapsed. Please contact the uTax support team. ';
                    $('#txt_' + Bankid).prop('disabled', 'disabled').attr('title', stitle);
                    $('#txt_' + Bankid + '_MSO').prop('disabled', 'disabled').attr('title', stitle);
                    $('#txttrns_' + Bankid).prop('disabled', 'disabled').attr('title', stitle);
                    $('#txttrns_' + Bankid + '_MSO').prop('disabled', 'disabled').attr('title', stitle);

                    $('#txt_' + Bankid).val() != "" ? 0 : $('#txt_' + Bankid).val(0);
                    $('#txt_' + Bankid + '_MSO').val() != "" ? 0 : $('#txt_' + Bankid + '_MSO').val(0);
                    $('#txttrns_' + Bankid).val() != "" ? 0 : $('#txttrns_' + Bankid).val(0);
                    $('#txttrns_' + Bankid + '_MSO').val() != "" ? 0 : $('#txttrns_' + Bankid + '_MSO').val(0);
                    _flag = false;
                } else {
                    $('#txt_' + Bankid).removeAttr('disabled');
                    $('#txt_' + Bankid + '_MSO').removeAttr('disabled');
                    $('#txttrns_' + Bankid).removeAttr('disabled');
                    $('#txttrns_' + Bankid + '_MSO').removeAttr('disabled');
                    _flag_A = true;
                }
            });
        })
    }
    if (!_flag_A) {
        if (_flag == false || _flag == 'false' || _flag == 'False') {
            error.show();
            error.append('<p>The ability to update the Fees information is not available since the Cutoff date for the same is elapsed. Please contact the uTax support team. </p>');
            return false;
        }
    }
}
