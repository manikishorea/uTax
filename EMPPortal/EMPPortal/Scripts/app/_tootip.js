function getTooltip() {
    $('a.ttip').hide();
    var sitemapid = $('#formid').attr('formid');
    sitemapid = sitemapid.toString().replace('site', '');
    var custmorUri = '/api/dropdown/tooltip?sitemapid=' + sitemapid;
    ajaxHelper(custmorUri, 'GET').done(function (data) {
        $.each(data, function (rowIndex, r) {
            $('#ttip' + r["Id"])
                .attr('data-original-title', r["Tooltip"])
                .show();
        });
    });
}