var win_height = $(window).height();
$('.modal-body').css('max-height', win_height - 250);
$('.modal-body').find('.bootstrap-select').on('click', function () {
    var d_width = $(this).width();
    var p_position = $(this).offset();
    $(this).find('.dropdown-menu').css({ 'min-width': d_width, 'width': d_width });
    $(this).find('.dropdown-menu').css({ 'top': p_position.top - 42 });
});
setInterval(function () {
    $('.modal-body').find('.multiselect').bind('click', function () {
        var d_width = $(this).width();
        var p_position = $(this).offset();
        $(this).next('.multiselect-container').css({ 'min-width': d_width + 32, 'width': d_width + 32 });
        $(this).next('.multiselect-container').css({ 'top': p_position.top - 39 });
    });
}, 1000);