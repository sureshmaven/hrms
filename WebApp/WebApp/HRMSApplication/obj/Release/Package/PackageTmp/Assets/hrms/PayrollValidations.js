//Monthly attendence Search

function MonthlyAttendencesearch() {

    var lempid = $("#companyName").val();
   
    if (lempid == "") {

        showGlobalModal("I#Required#Please Enter EmpCode.");
        $("#companyName").focus();
        return false;
    }
 

    else {
       
        return true;

    }

}
//only numbers
function isNumber(evt) {

    evt = (evt) ? evt : window.event;
    var charCode = (evt.which) ? evt.which : evt.keyCode;
    if (charCode > 31 && (charCode < 48 || charCode > 57)) {
        alert("Only Numbers Allowed");
        return false;
    }
    return true;
}
//Monthly Attendence Submit
function MonthlyAttendences() {

    var lempid = $("#companyName").val();
    var lstatus = $("#Status").val();
    var lStatusdate = $("#Statusdate").val();
    var lop = $("#LOPdays").val();
    var absent = $("#Absentdays").val();
    var working = $("#Workingdays").val();
    
    
    
    if (lempid == "") {

        showGlobalModal("I#Required#Please Enter EmpCode.");
        $("#companyName").focus();
        return false;
    }
  
    else if (lstatus == "") {
        showGlobalModal("I#Required#Please select Status.");
        $("#Status").focus();
        return false;
    }
    else if ((lstatus != "Deputation") && lStatusdate == "") {
        showGlobalModal("I#Required#Please select StatusDate.");
        $("#Status").focus();
        return false;
    }
    else if (lop == "") {
        showGlobalModal("I#Required#Please enter LOP days.");
        $("#Lopdays").focus();
        return false;
    }
  
    else if (absent == "") {
        showGlobalModal("I#Required#Please enter Absent days.");
        $("#Absentdays").focus();
        return false;
    }
    else if (working == "") {
        showGlobalModal("I#Required#Please enter Working days.");
        $("#Workingdays").focus();
        return false;
    }
    else {

        return true;

    }

}



function addmonthdetails() {
    var pdate = $("#pdate").val();
    var DaSlabs = $("#DaSlabs").val();
    var DaPoints = $("#DaPoints").val();
    var DaPercent = $("#DaPercent").val();

    if (pdate == "") {

        showGlobalModal("I#Required#Please Enter Payment Date.");
        $("#pdate").focus();
        return false;
    }

    else if (DaSlabs == "") {
        showGlobalModal("I#Required#Please Enter DaSlabs.");
        $("#DaSlabs").focus();
        return false;
    }
    else if (DaPoints == "") {
        showGlobalModal("I#Required#Please Enter DaPoints.");
        $("#DaPoints").focus();
        return false;
    }
    else if (DaPercent == "") {
        showGlobalModal("I#Required#Please enter DaPercent.");
        $("#DaPercent").focus();
        return false;
    }

    else if (DaPercent == "") {
        showGlobalModal("I#Required#Please enter DaPercent.");
        $("#Absentdays").focus();
        return false;
    }
  
    else {

        return true;

    }
}
document.addEventListener("DOMContentLoaded", pageSpinnerHide);