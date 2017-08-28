// Drop Down Custom Code in Model Window Start Here
$(window).load(function () {
    var win_height = $(window).height();
    $('.modal-body').css('max-height', win_height - 250);
    $('.modal-body').find('.bootstrap-select').on('click', function () {       
        var d_width = $(this).width();
        var p_position = $(this).offset();
        $(this).find('.dropdown-menu').css({ 'min-width': d_width, 'width': d_width });
        $(this).find('.dropdown-menu').css({ 'top': p_position.top -42});
    });
    setInterval(function () {
        $('.modal-body').find('.multiselect').bind('click', function () {
            var d_width = $(this).width();
            var p_position = $(this).offset();
            $(this).next('.multiselect-container').css({ 'min-width': d_width + 32, 'width': d_width + 32 });
            $(this).next('.multiselect-container').css({ 'top': p_position.top - 39 });
        });
    }, 1000);
});
// Drop Down Custom Code in Model Window End Here
$(document).ready(function () {
    $('.modal-body').find('.multiselect').bind('click', function () {
        var d_width = $(this).width();
        var p_position = $(this).offset();
        $(this).next('.multiselect-container').css({ 'min-width': d_width + 32, 'width': d_width + 32 });
        $(this).next('.multiselect-container').css({ 'top': p_position.top - 39 });
    });
   
    //$('.paginate_button.previous').html('');
    //$('.paginate_button.next').html('');
    
    $('#dealer_docs').click(function () {
        setTimeout(function () {
            $('#document_grid').resize();
        }, 300);
    });
    $('.help_t_right a').click(function () {
        $(this).addClass('show_right');
        $('.hel_right_txt, .bg_t_f').show()
    });
    // Help Box Script Start Here 
    setTimeout(function () {
        $('.help_s_m button.selectpicker').on('click', function () {
            //$(document).on('click', ".table_main .selectpicker", function() {
            $('.arrow_box').addClass('arrow_r');
            $('.body_main').addClass('body_r_s');
            $('.help_m_w').addClass('box_open');
            var label_txt = $.trim($(this).parents('.bootstrap-select').prev().prev('label').text());
            $('.des_h_e h4 span').each(function () {
                var head_txt = $.trim($(this).text());
                if (label_txt === head_txt) {
                    $('.des_h_e').css('margin-left', '-150px');
                    $('.des_h_e').hide();
                    $(this).parents('.des_h_e').show();
                    $('.des_h_e').animate({
                        marginLeft: '0px',
                    }, 300);
                }
            });
        });
    }, 1000);
    $('.help_s_m input[type="text"], .help_s_m input[type="radio"]').on('click', function () {
        $('.arrow_box').addClass('arrow_r');
        $('.body_main').addClass('body_r_s');
        $('.help_m_w').addClass('box_open');

    });
    $('.help_s_m input[type="text"]').click(function () {
        var label_txt = $.trim($(this).prev('label').text());
        $('.des_h_e h4 span').each(function () {
            var head_txt = $.trim($(this).text());
            if (label_txt === head_txt) {
                $('.des_h_e').css('margin-left', '-150px');
                $('.des_h_e').hide();
                $(this).parents('.des_h_e').show();
                $('.des_h_e').animate({
                    marginLeft: '0px'
                }, 300);
            }
        });
    });
    $('.help_s_m input[type="radio"]').click(function () {
        var label_txt = $.trim($(this).parent('.rad_btn').find('label.r_find').text());
        $('.des_h_e h4 span').each(function () {
            var head_txt = $.trim($(this).text());
            if (label_txt === head_txt) {
                $('.des_h_e').css('margin-left', '-150px');
                $('.des_h_e').hide();
                $(this).parents('.des_h_e').show();
                $('.des_h_e').animate({
                    marginLeft: '0px'
                }, 300);
            }
        });
    });
    $('#full_text').click(function () {
        $('.des_h_e').show();
    });
    $('.arrow_box').on('click', function () {
        $('.des_h_e').show();
        $('.body_main').toggleClass('body_r_s');
        $('.help_m_w').toggleClass('box_open');
        $('.arrow_box').toggleClass('arrow_r');
    });
    // Help Box Script End Here 


});
$('.close_btn').on('click', function () {
    $('.hel_right_txt, .bg_t_f').hide();
});
$(document).keyup(function (e) {
    if (e.keyCode == 27) {
        $('.hel_right_txt, .bg_t_f').hide();
    }
});
$(document).ready(function () {

    $('[data-toggle="tooltip"]').tooltip()
    // Dealer Menu Active Class In Mobile and Desktop
    var loc = window.location.href;
    if (/UpdateNextAction/.test(loc)) {
        /* $('.top_nav_main .makes a').addClass('active');
        $('.left_nav .customer_manage a').addClass('active'); */
    }

    // Dealer Aministration Menu Active Class
    /*if (/DealerCreditCardDetails/.test(loc) || /ManageSalesExecutives/.test(loc) || /DealerLocations/.test(loc)) {
        $('#menu .m_administration a').addClass('active');
        $('.left_nav .admin a').addClass('active');
    }*/
    /* if (/AddCreditCard/.test(loc)) {
         $('.top_nav_main .credit_card a').addClass('active');
     }*/
    //Dealer Re-Exam Requests Menu Active Class
    //if (/Region/.test(loc)) {
    //    $('.master_link').parent('li').addClass('open');
    //    $('.master_link').next('.sub-menu').show();
    //    $('.regions_link').addClass('open');
    //}
    if (/RoleMasters/.test(loc)) {
        $('.master_link').parent('li').addClass('open');
        $('.master_link').next('.sub-menu').show();
        $('.sub-menu').addClass('open active');
    }
    if (/GroupRoleMaps/.test(loc)) {
        $('.master_link').parent('li').addClass('open');
        $('.master_link').next('.sub-menu').show();
        $('.sub-menu').addClass('open active');
    }
    if (/user/.test(loc)) {
        $('.master_link').parent('li').addClass('open');
        $('.master_link').next('.sub-menu').show();
        $('.sub-menu').addClass('open active');
    }
    if (/phonetype/.test(loc)) {
        $('.master_link').parent('li').addClass('open');
        $('.master_link').next('.sub-menu').show();
        $('.sub-menu').addClass('open active');
    }
    if (/contactpersontitle/.test(loc)) {
        $('.master_link').parent('li').addClass('open');
        $('.master_link').next('.sub-menu').show();
        $('.sub-menu').addClass('open active');
    }
    if (/tooltip/.test(loc)) {
        $('.master_link').parent('li').addClass('open');
        $('.master_link').next('.sub-menu').show();
        $('.sub-menu').addClass('open active');
    }
    if (/Bank/.test(loc)) {
        $('.master_link').parent('li').addClass('open');
        $('.master_link').next('.sub-menu').show();
        $('.sub-menu').addClass('open active');
    }
   
    if (/banksubquestions/.test(loc)) {
        $('.master_link').parent('li').addClass('open');
        $('.master_link').next('.sub-menu').show();
        $('.sub-menu').addClass('open active');
    }

    if (/affiliateprogram/.test(loc)) {
        $('.master_link').parent('li').addClass('open');
        $('.master_link').next('.sub-menu').show();
        $('.sub-menu').addClass('open active');
    }
    if (/apiintegration/.test(loc)) {
        $('.master_link').parent('li').addClass('open');
        $('.master_link').next('.sub-menu').show();
        $('.sub-menu').addClass('open active');
    }
    //if (/Roles/.test(loc)) {
    //    $('.master_link').parent('li').addClass('open');
    //    $('.master_link').next('.sub-menu').show();
    //    $('.roles_link').addClass('open');
    //}
    //if (/EmailTemplates/.test(loc)) {
    //    $('.master_link').parent('li').addClass('open');
    //    $('.master_link').next('.sub-menu').show();
    //    $('.email_link').addClass('open');
    //}
    //if (/SMSTemplates/.test(loc)) {
    //    $('.master_link').parent('li').addClass('open');
    //    $('.master_link').next('.sub-menu').show();
    //    $('.sms_link').addClass('open');
    //}
    //if (/ManagePermissions/.test(loc)) {
    //    $('.contacts').addClass('open');
    //    $('.employees_link').removeClass('open');
    //    $('.master_link').parent('li').removeClass('open');
    //    $('.master_link').next('.sub-menu').hide();
    //}
    //if (/UpdateCustomerPurchaseInformation/.test(loc)) {
    //    $('.process_link').parent('li').addClass('open');
    //    $('.process_link').next('.sub-menu').show();
    //    $('.in_audit').addClass('open');
    //}
    //if (/UpdatePurchaseInfromationByQC/.test(loc)) {
    //    $('.process_link').parent('li').addClass('open');
    //    $('.process_link').next('.sub-menu').show();
    //    $('.qc_audit').addClass('open');
    //}
    //if (/UpdateCustomerServiceRequest/.test(loc)) {
    //    $('.process_link').parent('li').addClass('open');
    //    $('.process_link').next('.sub-menu').show();
    //    $('.customer_link').addClass('open');
    //}
    //if (/UpdateRexamRequest/.test(loc)) {
    //    $('.process_link').parent('li').addClass('open');
    //    $('.process_link').next('.sub-menu').show();
    //    $('.re-exam_link').addClass('open');
    //}
    //if (/AddNewReexamRequest/.test(loc)) {
    //    $('.process_link').parent('li').addClass('open');
    //    $('.process_link').next('.sub-menu').show();
    //    $('.re-exam_link').addClass('open');
    //}
    //if (/ManageCharityOrganization/.test(loc)) {
    //    $('.master_link').parent('li').addClass('open');
    //    $('.master_link').next('.sub-menu').show();
    //    $('.config_link').next('.config_link').addClass('open');
    //}
    //if (/CustomerComplaint/.test(loc)) {
    //    $('.complaint_management').parents('li').addClass('open');
    //}
    //if (/AddNewDealerRegistration/.test(loc)) {
    //    $('#dealership_management').addClass('open');
    //    $('#dealership_management').find('.sub-menu').show();
    //    $('.new_dealer').addClass('open');
    //}

    ////Begin - Added by Nikhil on 29 Dec 2015
    ////alert(loc);
    //if (/RejectedVendorDetails/.test(loc)) {
    //    $('#vendorselect_management').addClass('open');
    //    $('#vendorselect_management').find('.sub-menu').show();
    //    $('.lnkRejected_vendor').addClass('open');
    //}

    //if (/VendorDetails1/.test(loc)) {
    //    $('#vendorselect_management').addClass('open');
    //    $('#vendorselect_management').find('.sub-menu').show();
    //    $('.lnkregister_vendor').addClass('open');
    //}

    //if (/CommoditiesAssociateSpecification/.test(loc)) {
    //    $('#Commodity_Management').addClass('open');
    //    $('#Commodity_Management').find('.sub-menu').show();
    //    $('.view_Commodity').addClass('open');
    //}
    //if (/ScreenPermission/.test(loc)) {
    //    $('#Userselect_Management').addClass('open');
    //    $('#Userselect_Management').find('.sub-menu').show();
    //    $('.limanage_roles').addClass('open');
    //}
    //if (/ManageCities/.test(loc)) {
    //    $('#Manageselect_City').addClass('open');
    //    $('#Manageselect_City').find('.sub-menu').show();
    //    $('.limanage_cities').addClass('open');
    //}
    //if (/ManageCounties/.test(loc)) {
    //    $('#Manageselect_City').addClass('open');
    //    $('#Manageselect_City').find('.sub-menu').show();
    //    $('.limanage_cities').addClass('open');
    //}

    //if (/VendorScreenPermission/.test(loc)) {
    //    $('#Manageselect_user').addClass('open');
    //    $('#Manageselect_user').find('.sub-menu').show();
    //    $('.limanage_users').addClass('open');
    //}
    //if (/LineOfBusinessDocs/.test(loc)) {
    //    $('#menu_document').addClass('open');
    //    $('#menu_document').find('.sub-menu').show();
    //    $('.lobcls').addClass('open');
    //}
    //End - Added by Nikhil on 29 Dec 2015   

    var data_table_no = $('.dataTables_wrapper table').find('.dataTables_empty');
    if ($(data_table_no).text() == "No data available in table") {
        $('.dataTables_wrapper').find('.dataTables_paginate').hide();
        $('.dataTables_info').hide();
        $('.dataTables_wrapper').find('.dataTables_empty').addClass('icon_remove');
    }

    $('#Makes_filter input').addClass('form-control grid_search');

    //$('.input-group.date').click(function(){
    //$('.input-group.date .floating-label').css({"font-size": "10px", "top" : "-10px", "opacity" : "1", "color" : "#2E90E8"}); 
    //});
    //$('.input-group.date').each(function () {
    //$(this).click(function () {
    //$(this).parent().find('.input-group.date .floating-label').css({ "font-size": "10px", "top": "-10px", "opacity": "1", "color": "#2E90E8" });
    //});
    //});
    //$('#Makes_length select').addClass('selectpicker');
    $('.dataTables_filter input').addClass('form-control');

    //$('table.res_table').footable();
    //    $('.selectpicker').selectpicker();
    $('nav#menu').mmenu({
        extensions: ['effect-slide', 'pageshadow'],
        header: true,
        searchfield: true,
        counters: true,
        footer: {
            add: true,
            content: '<a href="mmenu.frebsite.nl">Visit website &rsaquo;</a>'
        }
    });

    //$('.form-control').each(function () {
    //$(this).focus(function () {
    //$(this).parent().find('.floating-label').css({ 'font-size': '10px', 'opacity': '1', 'top': '-10px'});
    //});
    //});

});

// Table Tools Script Start Here
$('.search_grid').on('click', function () {
    $(this).toggleClass('active');
    $('body').find('.dataTables_filter').toggleClass('show');
    $('.find_location, .grid_result').removeClass('active');
    $('.location_dp_down, .search_main').removeClass('show');
});
$('.find_location').on('click', function () {
    $(this).toggleClass('active');
    $('.location_dp_down').toggleClass('show');

    $('.grid_result, .search_grid').removeClass('active');
    $('.dataTables_filter, .search_main').removeClass('show');
});
$('.grid_result').on('click', function () {
    $(this).toggleClass('active');
    $('.search_main').toggleClass('show');

    $('.find_location, .search_grid').removeClass('active');
    $('.dataTables_filter, .location_dp_down').removeClass('show');

});

//**********************************BEGIN MAIN MENU********************************
$('.page-sidebar li > a').on('click', function (e) {
    if ($(this).next().hasClass('sub-menu') == false) {
        return;
    }
    var parent = $(this).parent().parent();
    parent.children('li.open').children('a').children('.arrow').removeClass('open');
    parent.children('li.open').children('a').children('.arrow').removeClass('active');
    parent.children('li.open').children('.sub-menu').slideUp(200);
    parent.children('li').removeClass('open');
    //  parent.children('li').removeClass('active');
    var sub = $(this).next();
    if (sub.is(":visible")) {
        $('.arrow', $(this)).removeClass("open");
        $(this).parent().removeClass("active");
        sub.slideUp(200, function () {
            handleSidenarAndContentHeight();
        });
    } else {
        $('.arrow', $(this)).addClass("open");
        $(this).parent().addClass("open");
        sub.slideDown(200, function () {
            handleSidenarAndContentHeight();
        });
    }

    e.preventDefault();
});
//Auto close open menus in Condensed menu
if ($('.page-sidebar').hasClass('mini')) {
    var elem = $('.page-sidebar ul');
    elem.children('li.open').children('a').children('.arrow').removeClass('open');
    elem.children('li.open').children('a').children('.arrow').removeClass('active');
    elem.children('li.open').children('.sub-menu').slideUp(200);
    elem.children('li').removeClass('open');
}
//**********************************END MAIN MENU********************************

//***********************************BEGIN Main Menu Toggle *****************************   
$('#layout-condensed-toggle').on('click', function () {
    $.sidr('close', 'sidr');
    if ($('#main-menu').attr('data-inner-menu') == '1') {
        //Do nothing
        console.log("Menu is already condensed");
    }
    else {
        if ($('#main-menu').hasClass('mini')) {
            $('body').removeClass('grey');
            $('#main-menu').removeClass('mini');
            $('.page-content').removeClass('condensed');
            $('.scrollup').removeClass('to-edge');
            $('.header-seperation').show();
            //Bug fix - In high resolution screen it leaves a white margin
            //$('.header-seperation').css('height','61px');
            $('.footer-widget').show();
        }
        else {
            $('body').addClass('grey');
            $('#main-menu').addClass('mini');
            $('.page-content').addClass('condensed');
            $('.scrollup').addClass('to-edge');
            //$('.header-seperation').hide();
            $('.footer-widget').hide();
        }
    }
});
//***********************************END Main Menu Toggle ***************************** 


var handleSidenarAndContentHeight = function () {
    var content = $('.page-content');
    var sidebar = $('.page-sidebar');

    if (!content.attr("data-height")) {
        content.attr("data-height", content.height());
    }

    if (sidebar.height() > content.height()) {
        content.css("min-height", sidebar.height() + 120);
    } else {
        content.css("min-height", content.attr("data-height"));
    }
}
$('.panel-group').on('hidden.bs.collapse', function (e) {
    $(this).find('.panel-heading').not($(e.target)).addClass('collapsed');
})




/* $(document).on('click touchstart', function (e) {
     e.stopPropagation();
     var container = $(".search_grid, .dataTables_filter");
     //check if the clicked area is dropDown or not
     if (container.has(e.target).length === 0) {
         $('.search_grid').removeClass('active');
         $('.dataTables_filter').removeClass('show');   

     }

     var location = $(".find_location, .location_dp_down");
     //check if the clicked area is dropDown or not
     if (location.has(e.target).length === 0) {
         $('.find_location').removeClass('active');
         $('.location_dp_down').removeClass('show');    
     }

     var grid_result = $(".grid_result, .search_main");
     //check if the clicked area is dropDown or not
     if (grid_result.has(e.target).length === 0) {
         $('.grid_result').removeClass('active');
         $('.search_main').removeClass('show');    
     }

 });

*/
$(window).resize(function () {
    location_drop_change();
    $('.find_location').removeClass('active');
    $('.location_dp_down').removeClass('show');
});
$(document).ready(function () {
    $('.location_dp_down #location_dropdown').remove();
    location_drop_change();
});
function location_drop_change() {
    var window_widht = $(window).width();
    if (window_widht <= 767) {
        $('#location_dropdown').appendTo($(".location_dp_down"));
        $('.selectpicker[data-id="location_dropdown"]').parent('.bootstrap-select').appendTo($(".location_dp_down"));
    }
    else {
        $('.selectpicker[data-id="location_dropdown"]').parent('.bootstrap-select').insertAfter($(".location_change .rpad10.hidden-sm"));
        $('#location_dropdown').insertAfter($(".location_change .rpad10.hidden-sm"));
    }
}
$('#location_dropdown').change(function () {
    $('table.res_table').footable();
    $('.find_location').removeClass('active');
    $('.location_dp_down').removeClass('show');
});


$(document).ready(function () {
    var url = window.location.pathname,
    urlRegExp = new RegExp(url.replace(/\/$/, '') + "$"); // create regexp to match current url pathname and remove trailing slash if present as it could collide with the link in navigation in case trailing slash wasn't present there
    $('#main-menu-wrapper li a').each(function () {
        if (urlRegExp.test(this.href.replace(/\/$/, ''))) {
            $(this).parent('li').addClass('open');
            $(this).parents('.sub-menu').parent('li').addClass('open');
        }
    });


    $('.mm-listview > li > a').each(function () {
        if (urlRegExp.test(this.href.replace(/\/$/, ''))) {
            $(this).addClass('active ');
            //$(this).parents('.sub-menu').parent('li').addClass('active');
        }
    });

    var menu_sub = $('#main-menu-wrapper li');
    $('#main-menu-wrapper li').each(function () {
        if ($(this).hasClass("open")) {
            $(this).find('.sub-menu').show();
        }
    });
    //$('.sub-menu').hide();
    //$('textarea').attr('maxlength', '1500');

    //Fix z-index youtube video embedding
    $('iframe').each(function () {
        var url = $(this).attr("src");
        $(this).attr("src", url + "?wmode=transparent");
    });
});