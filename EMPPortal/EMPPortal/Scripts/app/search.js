$(document).ready(function () {
    $('#table_user').dataTable({
        "aoColumns": [
            null,
            null
        ]
        , iDisplayLength: 4,
        columnDefs: [{
            "targets": 'no-sort',
            "orderable": false
        }]
    });
});