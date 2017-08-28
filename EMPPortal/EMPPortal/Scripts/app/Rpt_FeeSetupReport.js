function Fn_FeeSeupReport(Id) {
    var table = $('#table_FeeSetup').append('<tbody/>');
    var url = '/api/Reports/FeeSetupReport?UserId=' + Id;
    var req = {};
    var i = 1;
    $('#table_FeeSetup').DataTable().clear().draw();

    ajaxHelper(url, 'GET',null,false).done(function (data) {
        $.each(data, function (rowIndex, r) {
            $('#table_FeeSetup').DataTable().row.add([
                i,
                r["MasterID"],
                r["UserID"],
                r["ParentUserID"],
                r["CompanyName"],
                r["Efin"],
                r["uTaxFee"],
                r["SBFee"],
                r["SBAddOn"],
                r["AddonFeeSB"],
                r["AddonFeeERO"],
                r["Bank"],
                r["EnrollmentStatus"],
                r["AccountOwner"]
            ]).draw(false);
            i++;
        });
    });
}

function getMainCustomer(container) {
    container.html('');
    var custmorUri = '/api/Reports/AllMainCustomers';
    ajaxHelper(custmorUri, 'GET',null,false).done(function (data) {
        $.each(data, function (rowIndex, r) {
            container.append($('<option />', { value: r["ID"], text: r["CustomerName"] }));
        });
    });
}

function getSubCustomer(container, ParentId) {
    container.html('');
    var custmorUri = '/api/Reports/AllSubCustomers?ParentIds=' + ParentId;
    ajaxHelper(custmorUri, 'GET',null,false).done(function (data) {
        // container.append($('<option />', { value: '', text: 'Select' }));
        $.each(data, function (rowIndex, r) {
            container.append($('<option />', { value: r["ID"], text: r["CustomerName"] }));
        });
    });
}

//No Bank App Submission Report
function Fn_NoBankAppSubmissionReport(Id) {
    var table = $('#table_NoBankSubmission').append('<tbody/>');
    var url = '/api/Reports/NoBankAppSubmission?UserId=' + Id;
    var req = {};
    var i = 1;
    $('#table_NoBankSubmission').DataTable().clear().draw();
    ajaxHelper(url, 'GET',null,false).done(function (data) {
        $.each(data, function (rowIndex, r) {
            $('#table_NoBankSubmission').DataTable().row.add([
                i,
                r["MasterID"],
                r["UserID"],
                r["ParentUserID"],
                r["CompanyName"],
                r["Efin"],
                r["AccountOwner"]
            ]).draw(false);
            i++;
        });
    });
}

function getNoSubmissionMainCustomer(container) {
    container.html('');
    var custmorUri = '/api/Reports/AllNoBankSubmissionCustomerMain';
    ajaxHelper(custmorUri, 'GET',null,false).done(function (data) {
        $.each(data, function (rowIndex, r) {
            container.append($('<option />', { value: r["ID"], text: r["CustomerName"] }));
        });
    });
}
//till here 

//Last Login Report 
function Fn_LastLoginReport(Id) {
    var table = $('#table_LastLoginInfo').append('<tbody/>');
    var url = '/api/Reports/LastLoginInfo?UserID=' + Id;
    var req = {};
    $('#table_NoBankSubmission').DataTable().clear().draw();
    ajaxHelper(url, 'GET',null,false).done(function (data) {
        $.each(data, function (rowIndex, r) {
            $('#table_LastLoginInfo').DataTable().row.add([
                 r["MasterID"],r["Efin"],r["UserID"],r["ParentUserID"],r["CompanyName"],r["UserID"],r["DateTimeStamp"],r["IpAddress"]
            ]).draw(false);
        });
    });
}
//till here

//New Enrollment 
function Fn_NewEnrollmentCases(Id) {
    var table = $('#table_NewEnrollment').append('<tbody/>');
    var url = '/api/Reports/AllNewEnrollmentCases?UserId=' + Id;
    var req = {};
    $('#table_NewEnrollment').DataTable().clear().draw();
    ajaxHelper(url, 'GET').done(function (data) {
        $.each(data, function (rowIndex, r) {
            $('#table_NewEnrollment').DataTable().row.add([
                r["MasterID"],
                r["UserID"],
                r["ParentUserID"],
                r["CompanyName"],
                r["Efin"],
                r["AccountStatus"],
                r["EROType"],               
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                r["MSOUser"],
                r["DateTimeOpened"],
                r["DateTimeLastModified"]
            ]).draw(false);
        });
    });
}

function Fn_CloseEnrollmentCases(Id) {
    var table = $('#table_NewEnrollment').append('<tbody/>');
    var url = '/api/Reports/AllNewEnrollmentCases?UserId=' + Id;
    var req = {};
    var i = 1;
    $('#table_NewEnrollment').DataTable().clear().draw();
    ajaxHelper(url, 'GET').done(function (data) {
        $.each(data, function (rowIndex, r) {
            $('#table_NewEnrollment').DataTable().row.add([
                r["MasterID"],
                r["UserID"],
                r["ParentUserID"],
                r["CompanyName"],
                r["Efin"],
                r["AccountStatus"],
                r["EROType"],
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                r["MSOUser"],
                r["DateTimeOpened"],
                r["DateTimeLastModified"]
            ]).draw(false);
            i++;
        });
    });
}

function Fn_StaleEnrollmentCases(Id) {
    var table = $('#table_StaleEnrollment').append('<tbody/>');
    var url = '/api/Reports/StaleEnrollmentCases?UserId=' + Id;
    var req = {};
    var i = 1;
    $('#table_StaleEnrollment').DataTable().clear().draw();
    ajaxHelper(url, 'GET').done(function (data) {
        $.each(data, function (rowIndex, r) {
            $('#table_StaleEnrollment').DataTable().row.add([
                r["MasterID"],
                r["Efin"],
                r["UserID"],
                r["ParentUserID"],
                r["CompanyName"],                
                r["AccountStatus"],
                r["EROType"],
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                r["MSOUser"],
                r["DateTimeOpened"],
                r["DateTimeLastModified"]
            ]).draw(false);
            i++;
        });
    });
}

function Fn_CloseEnrollmentCases(Id) {
    var table = $('#table_CloseEnrollment').append('<tbody/>');
    var url = '/api/Reports/AllCloseEnrollmentCases?UserId=' + Id;
    var req = {};
    var i = 1;
    $('#table_CloseEnrollment').DataTable().clear().draw();
    ajaxHelper(url, 'GET').done(function (data) {
        $.each(data, function (rowIndex, r) {
            $('#table_CloseEnrollment').DataTable().row.add([
                r["MasterID"],
                r["UserID"],
                r["ParentUserID"],
                r["CompanyName"],
                r["Efin"],
                r["AccountStatus"],
                r["EROType"],
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                r["MSOUser"],
                r["DateTimeOpened"],
                r["DateTimeLastModified"]
            ]).draw(false);
            i++;
        });
    });
}
//Till here