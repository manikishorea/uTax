var CompanArr = [];

function getEnrollmentList__AB(Id) {
    //var CustId = $('#UserId').val();
    //var Cid = getUrlVars()["Id"];
    //if (Cid) {
    //    CustId = Cid;
    //}
    //var EntityId = $('#entitydisplayid').val();

    var Uri = '/api/Reports/getEnrollmentsList?strguid=' + Id;
    ajaxHelper(Uri, 'GET').done(function (res) {
        var body = '';
        $.each(res, function (value, item) {
            body += '<tr><td>' + item.Efin + '</td><td>'
                + item.UserId + '</td><td>' + item.ParentId + '</td><td>' + item.Company + '</td><td>'
                + item.Bank + '</td><td>' + item.Status + '</td><td>' + item.SBFee + '</td><td>' + item.AddonFee + '</td><td>'
                + item.SubmissionDate + '</td><td>' + item.AccountOwner + '</td><td>' + item.ErrorMessage + '</td><td>' + item.Parent + '</td></tr>';

            var company = {};
            company.Id = item.Id;
            company.Name = item.Company;
            company.Parent = item.Parent;
            company.ParentName = item.ParentId;
            CompanArr.push(company);
        });
        $('#tbd_enrollment').html(body);
        $.unblockUI();
    })
}

function getEnrollmentList(Id) {
    var table = $('#tbl_enrollmentstatus').append('<tbody/>');
    var url = '/api/Reports/getEnrollmentsList?strguid=' + Id;
    var req = {};
    $('#tbl_enrollmentstatus').DataTable().clear().draw();
    ajaxHelper(url, 'GET',null,false).done(function (data) {
        $.each(data, function (rowIndex, r) {
            $('#tbl_enrollmentstatus').DataTable().row.add([
                r["MasterId"],
                r["Efin"],
                r["UserId"],
                r["ParentId"],
                r["Company"],
                r["Bank"],
                r["Status"],
                r["SBFee"],
                r["AddonFee"],
                r["SubmissionDate"],
                r["LastModifiedDate"],
                r["LastModifiedUser"],
                r["AccountOwner"],
                r["ErrorMessage"]
            ]).draw(false);
        });
    });
}

function fillDropdown() {
    CompanArr = CompanArr.unique();
    var parentIds = CompanArr.filter(function (i) { return i.ParentName == "" });
    $('#ddl_company').append('<option value=""></option>');
    $.each(parentIds, function (item, value) {
        $('#ddl_company').append('<option value="' + value.Id + '">' + value.Name + '</option>');
    })
}

function filterCompany() {
    var value = $('#ddl_company').val();
    $('#tbl_enrollmentstatus').DataTable().column(11).search(
        value, false, true
    ).draw();
}