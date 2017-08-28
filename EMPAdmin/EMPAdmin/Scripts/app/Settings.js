function getuTaxSettings() {
    var url = '/api/Settings/GetSettings';
    ajaxHelperasync(url, 'GET', null).done(function (res) {
        if (res) {
            $('#chk_ActCrt').prop('checked', res.IsAccountCreation);
        }
    })
}

function saveSettings() {
    var req = {};
    req.UserId = $('#UserId').val();
    req.IsAccountCreation = $('#chk_ActCrt').prop('checked');

    var url = '/api/Settings/SaveSettings';
    ajaxHelperasync(url, 'POST', req).done(function (res) {
        if (res) {
            if (res == 1) {
                $('#success p').html('Settings saved successfully.');
                $('#success').show();
            }
            else {
                $('#error p').html('Settings not saved.');
                $('#error').show();
            }
        }
    });
}

$(function(){

    getuTaxSettings();

})