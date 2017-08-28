var unsaved = false;

$(document).ready(function () {
    var btn;

    $(":input").change(function () { //trigers change in all input fields including text type
        unsaved = true;
    });

    $(":input").keyup(function () { //trigers change in all input fields including text type
        unsaved = true;
    });

    $(":button.selectpicker ").change(function () { //trigers change in all input fields including text type
        unsaved = true;
    });



    $('button[type=button]').click(function (event) {
        var pp = $(this).find('button[type=button]');
        btn = $(this).attr('id');
        if (unsaved) {
            if (btn == 'CancelButton' || btn == 'CancelButton1' || btn == 'btnBack') {  //Popup Cancel
                if (confirm("This page is asking you to confirm that you want to leave - data you have entered may not be saved.")) {
                    unsaved = false;
                }
                else {
                    return false;
                }
            }
            else {  //SUbmit
                unsaved = false;
            }
        }
    });

    $('input[type=submit]').click(function (event) {

        unsaved = false;
    });

    function unloadPage() {
        if (unsaved || unsaveddrop) {
            //return "You have unsaved changes on this page. Do you want to leave this page and discard your changes or stay on this page?";
            return "This page is asking you to confirm that you want to leave - data you have entered may not be saved.";
        }
    }
    window.onbeforeunload = unloadPage;
});
