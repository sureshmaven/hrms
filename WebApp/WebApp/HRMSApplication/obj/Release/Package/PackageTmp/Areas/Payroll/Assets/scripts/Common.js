var g_pagereload_seconds = 2800;


function showGlobalModal(msg) {
    //sample msg = "I#Required Field#Please enter end data."
    //sample msg = "E#Save Error#No leave balance. Please contact admin team."
    //sample msg = "S#Leave Applied#User leave applied."
    var arr = msg.split('#');

    if (arr[0] === "I") {
        $(".modal-header2").css("background-color", "#fbb44c");
        $(".alert-btn").css('border', '1px solid #fbb44c');
    } else if (arr[0] === "E") {
        $(".modal-header2").css("background-color", "#e41212");
        $(".alert-btn").css('border', '1px solid #e41212');
    } else if (arr[0] === "S") {
        $(".modal-header2").css("background-color", "#357935");
        $(".alert-btn").css('border', '1px solid #357935');
    }
    $("#gHrmModal .modal-title").text(arr[1]);       
    $("#gHrmModal .modal-body").text(arr[2]);
    $('#gHrmModal').modal('show');

}

function showGlobalModalPayslip(msg) {
    //sample msg = "I#Required Field#Please enter end data."
    //sample msg = "E#Save Error#No leave balance. Please contact admin team."
    //sample msg = "S#Leave Applied#User leave applied."
    var arr = msg.split('#');

    if (arr[0] === "I") {
        $(".modal-header2").css("background-color", "#fbb44c");
        $(".alert-btn").css('border', '1px solid #fbb44c');
    } else if (arr[0] === "E") {
        $(".modal-header2").css("background-color", "#e41212");
        $(".alert-btn").css('border', '1px solid #e41212');
    } else if (arr[0] === "S") {
        $(".modal-header2").css("background-color", "#357935");
        $(".alert-btn").css('border', '1px solid #357935');
    }
    $("#gHrmModal .modal-title").text(arr[1]);
    var a = arr[2].split(",").join( "," +'<br>');
    $("#gHrmModal .modal-body").html(a);
    $('#gHrmModal').modal('show');

}
function showGlobalModal_For_monthend(msg) {
    var arr = msg.split('#');
    if (arr[0] === "I") {
        $(".modal-header2").css("background-color", "#fbb44c");
        $(".alert-btn").css('border', '1px solid #fbb44c');
    } else if (arr[0] === "E") {
        $(".modal-header2").css("background-color", "#e41212");
        $(".alert-btn").css('border', '1px solid #e41212');
    } else if (arr[0] === "S") {
        $(".modal-header2").css("background-color", "#357935");
        $(".alert-btn").css('border', '1px solid #357935');
    }
    $("#gHrmModal_For_MonthEnd .modal-title").text(arr[1]);
    $("#gHrmModal_For_MonthEnd .modal-body").text(arr[2]);
    $('#gHrmModal_For_MonthEnd').modal('show');
}


function custom_confirm(message, callback) {
    //  put message into the body of modal
    $('#confirmModal').on('show.bs.modal', function (event) {
        //  store current modal reference
        var modal = $(this);

        //  update modal body help text
        modal.find('.modal-body #modal-help-text').text(message);
    });

    //  show modal ringer custom confirmation
    $('#confirmModal').modal('show');

    $('#confirmCancel').off().on('click', function () {
        // close window
        $('#confirmModal').modal('hide');
        // callback
        callback(false);
    });
}


//function showGlobalModaldelete(message, onConfirm) {
//    // 
//    var fClose = function () {
//        modal.modal("hide");
//    };
//    var message = "Do You Want to Delete";

//    var modal = $("#confirmModal");
//    modal.modal("show");
//    $("#confirmMessage").empty().append(message);
//    $("#confirmOk").unbind().one('click', onConfirm).one('click', true);
//    $("#confirmCancel").unbind().one("click", fClose);
//}


$.fn.serializeAndEncode = function () {
    return $.map(this.serializeArray(), function (val) {
        return [val.name, encodeURIComponent(val.value)].join('=');
    }).join('&');
};

////start dvLoading
$.fn.SetOverlayHeightWidth = function () {
    $(this).height($(document).height());
    $(this).width($(document).width());
};
//window.onload = function (e) {
//    $(".overlayforaddnote").show().setoverlayheightwidth();
//    $('#dvloading').fadein(500);
//    $("#dvloading").show();
//    settimeout(function () {
//        ajaxspinnerhide();
//    }, 400);
//};
$(function () {
$('.navbar-nav li').click(function () {
    $(".overlayforaddNote").show().SetOverlayHeightWidth();
    $('#dvLoading').fadeIn(500);
    $("#dvLoading").show();
    setTimeout(function () {
        ajaxSpinnerHide();
    }, 500);
    });
    $('#leftmenu li').click(function () {
    $(".overlayforaddNote").show().SetOverlayHeightWidth();
    $('#dvLoading').fadeIn(500);
    $("#dvLoading").show();
    setTimeout(function () {
        ajaxSpinnerHide();
    }, 500);
});
});
//$('#respMenu li').click(function () {
//    $(".overlayforaddNote").show().SetOverlayHeightWidth();
//    $('#dvLoading').fadeIn(500);
//    $("#dvLoading").show();
//    setTimeout(function () {
//        ajaxSpinnerHide();
//    }, 2000);
//});
//$("form").submit(function () {
//    $(".overlayforaddNote").show().SetOverlayHeightWidth();
//    $('#dvLoading').fadeIn(500);
//    $("#dvLoading").show();
//});
//function pageSpinnerHide() {
//    $(".overlayforaddNote").show().SetOverlayHeightWidth();
//    $("#dvLoading").show();
//    $('#dvLoading').fadeOut(500);
//    setTimeout(function () { $(".overlayforaddNote").hide() }, 500);
//}
function ajaxSpinnerShow() {
    $(".overlayforaddNote").show().SetOverlayHeightWidth();
    $('#dvLoading').fadeIn(500);
    $("#dvLoading").show();
}
function ajaxSpinnerHide() {
    $('#dvLoading').fadeOut(500);
    setTimeout(function () {
        $("#dvLoading").hide();
    }, 500);
    setTimeout(function () {
        $(".overlayforaddNote").hide();
    }, 700);
}

////end dvLoading
////start datepicker
$(function () {
$('#months').datepicker({
    minViewMode: 1,
    autoclose: true,
    format: 'M-yyyy'
    });
});
$(function () {
    $('.input-group.date').datepicker({
        format: 'dd-mm-yyyy',
        todayHighlight: true,
        autoclose: true
    });
});
////end datepicker
////start tooltip
$(document).ready(function () {
    $('[data-toggle="tooltip"]').tooltip();
});
////end tooltip
