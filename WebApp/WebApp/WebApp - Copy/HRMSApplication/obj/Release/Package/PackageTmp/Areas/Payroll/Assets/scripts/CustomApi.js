
function toasterOptions() {
    toastr.options = {
        "closeButton": false,
        "debug": false,
        "newestOnTop": false,
        "progressBar": true,
        "positionClass": 'toast-bottom-right',
        "preventDuplicates": true,
        "onclick": null,
        "showDuration": "100",
        "hideDuration": "1000",
        "timeOut": "2500",
        "extendedTimeOut": "1000",
        "showEasing": "swing",
        "hideEasing": "linear",
        "showMethod": "show",
        "hideMethod": "hide"
    };
}

function CommonApiPOST(url, payload, cbFunc) {
    ajaxSpinnerShow(); //$("#dvLoading").show();
    $.ajax({
        type: "POST",
        url: url,
        contentType: "application/json;",
        data: payload,
        success: function (response) {
            ajaxSpinnerHide(); //$("#dvLoading").hide();
            var Msg = response.split('#');
            if (response.startsWith('I')) {
                toasterOptions();
                toastr.warning(Msg[2], Msg[1]);
                cbFunc(true);
            } else if (response.startsWith('E')) { 
                //todo - modal popup
                showGlobalModal(response);
                //toasterOptions();
                //toastr.error(Msg[2], Msg[1]);
                cbFunc(false);
            }
        },
        failure: function (response) {
            ajaxSpinnerHide(); //$("#dvLoading").hide();
            showGlobalModal(response);
            //todo - modal popup
            //toasterOptions();
            //toastr.error(response);
            cbFunc(false);
        }
    });
}


function CommonApiGET(url, cbFunc) {
    ajaxSpinnerShow(); //$("#dvLoading").show();
    $.ajax({
        type: "GET",
        url: url,
        dataType: 'json',
        success: function (resp, textstatus) {
            ajaxSpinnerHide(); //$("#dvLoading").hide();
            if (textstatus == 'success') {
                cbFunc(resp);
            } else {
                //todo - modal popup
                //toasterOptions();
               // toastr.error(response);
                showGlobalModal(response);
            }
        },
        failure: function (response) {
            ajaxSpinnerHide(); //$("#dvLoading").hide();

            //todo - modal popup
            //toasterOptions();
           // toastr.error(response);
        }
    });
}


function CommonApiGET_NoSpinner(url, cbFunc) {
    $.ajax({
        type: "GET",
        url: url,
        dataType: 'json',
        success: function (resp, textstatus) {
            if (textstatus == 'success') {
                cbFunc(resp);
            } else {
                //todo - modal popup
                //toasterOptions();
                // toastr.error(response);
                showGlobalModal(response);
            }
        },
        failure: function (response) {

            //todo - modal popup
            //toasterOptions();
            // toastr.error(response);
        }
    });
}