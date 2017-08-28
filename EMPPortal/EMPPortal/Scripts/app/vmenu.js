$(function () {
    var status = $('#formid').val();

    var formname = $('#formid').attr('formid');

    $('a.vmenu')
        .removeClass('active')
        .removeClass('done');

    $('a#' + formname)
        .addClass(status);
});