//Create Employee,Insert Employee, View Profile feild Validations
function RegFunction() {
    debugger;
    var FN = $("#FirstName").val();
    var LN = $("#LastName").val();
    var Pwd = $("#pw1").val();
    var cPwd = $("#pw2").val();
    var male = $("#m").val();
    var female = $("#f").val();
    var single = $("#r1").val();
    var married = $("#r2").val();
    var DOB = $("#DOB").val();
    var Spouse = $("#SpouseName").val();
    var FatherName = $("#FatherName").val();
    var MotherName = $("#MotherName").val();
    var MobileNo = $("#MobileNumber").val();
    var HomeNo = $("#HomeNumber").val();
    var PresentAdd = $("#PresentAddress").val();
    var PermanentAdd = $("#PermanentAddress").val();
    var Grad = $("#Graduation").val();
    var PostGrad = $("#PostGraduation").val();
    var Category = $("#Category").val();
    var PersonalEmailId = $("#PersonalEmailId").val();
    var EmergName = $("#EmergencyName").val();
    var EmergCnctNo = $("#EmergencyContactNo").val();
    var shotname = $("#ShortName").val();

    var AadharNo = $("#AadharCardNo").val();
    var PanNo = $("#PanCardNo").val();
    var BG = $("#BloodGroup").val();
    var EmpId = $("#empidd").val();
    var Br = $("#profeBranch option:selected").text();
    var JoinDesig = $("#JoinedDesignation").val();
    var CurrDesig = $("#desg1").val();
    var Dept = $("#profeDept option:selected").text();
    var Role = $("#Role").val();
    var OffEmailId = $("#OfficalEmailId").val();
    var profqual = $("#ProfessionalQualifications").val();
    var DOJ = $("#DOJ").val();

    var RelR = $("#Exit_type").val(); // newly added on 06/06/2020
    var todaydate = new Date(); // newly added on 08/06/2020
    var twoDigitMonth = (todaydate.getMonth() + 1) + ""; if (twoDigitMonth.length == 1) twoDigitMonth = "0" + twoDigitMonth; // newly added on 08/06/2020
    var twoDigitDate = todaydate.getDate() + ""; if (twoDigitDate.length == 1) twoDigitDate = "0" + twoDigitDate; // newly added on 08/06/2020
    var currentDatess = twoDigitDate + "-" + twoDigitMonth + "-" + todaydate.getFullYear(); // newly added on 08/06/2020

    var RelDate = $("#RelievingDate").val();
    var RetDate = $("#RetirementDate").val();
    var relreason = $("#RelievingReason").val();
    var totexp = $("TotalExperience").val();
    var controllingdept = $("#HDept").val();
    var controllingbranch = $("#HbranchCon").val();
    var controllingdesignation = $("#Hdesignationcon").val();
    var controllingmanager = $("#HMN").val();
    var sanctiondept = $("#DeptSan").val();
    var sanctiondesignation = $("#designationSan").val();
    var sanctionmanager = $("#mag").val();
    var profheadoffice = $("#head").val();
    var profbranch = $("#bran").val();
    var controlheadoffice = $("#head1").val();
    var controlbranch = $("#bran1").val();
    var image = $("#file").val();
    var regexp = /^[a-zA-Z][\w\.-]*[a-zA-Z0-9]@[a-zA-Z0-9][\w\.-]*[a-zA-Z0-9]\.[a-zA-Z][a-zA-Z\.]*[a-zA-Z]$/;//email
    var phone = /^(?:(?:\+|0{0,2})91(\s*[\ -]\s*)?|[0]?)?[789]\d{9}|(\d[ -]?){10}\d$/;
    var AlphaNum = /^[a-z0-9]+$/i;
    var regpan = /^([a-zA-Z]){5}([0-9]){4}([a-zA-Z]){1}?$/;
    if (FN == "") {
        //window.alert("Please Enter First Name.");
        showGlobalModal("I#Required#Please Enter First Name.");
        $("#FirstName").focus();
        return false;
    }
    else if (FN != "" && FN.length < 1) {
        // window.alert("Please Enter Mimimum 1 characters For First Name.");
        showGlobalModal("I#Required#Please Enter Mimimum 1 characters For First Name.");
        $("#FirstName").focus();
        return false;
    }
    else if (LN == "") {
        //window.alert("Please Enter the Last Name.");
        showGlobalModal("I#Required#Please Enter the Last Name.");
        $("#LastName").focus();
        return false;
    }
    else if (LN != "" && LN.length < 1) {
        // window.alert("Please Enter Minimum 1 characters For Last Name.");
        showGlobalModal("I#Required#Please Enter Minimum 1 characters For Last Name.");
        $("#LastName").focus();
        return false;
    }
    else if (Pwd == "") {
        //window.alert("Please Enter Password.");
        showGlobalModal("I#Required#Please Enter Password.");
        $("#pw1").focus();
        return false;
    }
    else if (Pwd != "" && Pwd.length < 6) {
        // window.alert("Please Enter Minimum 6 characters For Password.");
        showGlobalModal("E#Required#Please Enter Minimum 6 characters For Password.");
        $("#pw1").focus();
        return false;
    }
    else if (cPwd == "") {
        // window.alert("Please Enter Confirm Password.");
        showGlobalModal("I#Required#Please Enter Confirm Password.");
        $("#pw2").focus();
        return false;
    }

    if (Pwd != cPwd) {
        //alert('Your Password and Confirm Password do not match.');
        showGlobalModal("E#Required#Your Password and Confirm Password do not match.");
        $("#pw2").focus();
        return false;
    }
    else if (document.getElementById("m").checked == false && document.getElementById("f").checked == false) {
        //alert("Please choose your Gender: Male or Female.");
        showGlobalModal("I#Required#Please choose your Gender: Male or Female.");
        $("#Gender").focus();
        return false;
    }
    else if (DOB == "") {
        // window.alert("Please Enter Date Of Birth.");
        showGlobalModal("I#Required#Please Enter Date Of Birth.");
        $("#DOB").focus();
        return false;
    }
    else if (document.getElementById("r1").checked == false && document.getElementById("r2").checked == false) {
        //alert("Please choose your Martial Status.");
        showGlobalModal("I#Required#Please choose your Martial Status.");
        $("#MartialStatus").focus();
        return false;
    }
    else if (FatherName == "") {
        //window.alert("Please Enter Father's Name.");
        showGlobalModal("I#Required#Please Enter Father's Name.");
        $("#FatherName").focus();
        return false;
    }
    else if (FatherName !== "" && FatherName.length < 3) {
        //  window.alert("Please enter minimum 3 characters for FatherName.");
        showGlobalModal("I#Required#Please enter minimum 3 characters for FatherName.");
        $("#FatherName").focus();
        return false;
    }
    else if (FatherName !== "" && FatherName.length > 35) {
        //window.alert("Please enter Father Name in 35 characters only.");
        showGlobalModal("I#Required#Please enter Father Name in 35 characters only.");
        $("#FatherName").focus();
        return false;
    }

    else if (MotherName == "") {
        //window.alert("Please Enter Mother's Name.");
        showGlobalModal("I#Required#Please Enter Mother's Name.");
        $("#MotherName").focus();
        return false;
    }
    else if (MotherName !== "" && MotherName.length < 3) {
        // window.alert("Please enter minimum 3 characters for Mother Name.");
        showGlobalModal("I#Required#Please enter minimum 3 characters for Mother Name.");
        $("#MotherName").focus();
        return false;
    }
    else if (MotherName !== "" && MotherName.length > 35) {
        //window.alert("Please enter Mother Name in  35 characters only.");
        showGlobalModal("I#Required#Please enter Mother Name in  35 characters only.");
        $("#MotherName").focus();
        return false;
    }
    else if (MobileNo == "") {
        //window.alert("Please Enter Mobile Number. ");
        showGlobalModal("I#Required#Please Enter Mobile Number.");
        $("#MobileNumber").focus();
        return false;
    }
    else if (MobileNo !== "" && MobileNo.length < 10) {
        //window.alert("Please enter Correct Mobile Number.");
        showGlobalModal("E#Required#Please enter Correct Mobile Number.");
        $("#MobileNumber").focus();
        return false;
    }
    //else if (!mobileno.match(phone)) {
    //    window.alert("please enter valid mobile number.");
    //    $("#mobilenumber").focus();
    //    return false;
    //}
    else if (PresentAdd == "") {
        //window.alert("Please Enter Present Address. ");
        showGlobalModal("I#Required#Please Enter Present Address.");
        $("#PresentAddress").focus();
        return false;
    }
    else if (PermanentAdd == "") {
        // window.alert("Please Enter Permanent Address. ");
        showGlobalModal("I#Required#Please Enter Permanent Address.");
        $("#PermanentAddress").focus();
        return false;
    }
    else if (Grad == "") {
        // window.alert("Please Enter the type of Graduation.");
        showGlobalModal("I#Required#Please Enter the type of Graduation.");
        $("#Graduation").focus();
        return false;
    }
    else if (Grad != "" && Grad.length < 2) {
        //window.alert("Please enter minimum of 2 characters.");
        showGlobalModal("I#Required#Please enter minimum of 2 characters.");
        $("#Graduation").focus();
        return false;
    }
    else if (document.getElementById("Category").value == "") {
        // window.alert("Please Select Category.");
        showGlobalModal("I#Required#Please Select Category.");
        $("#Category").focus();
        return false;
    }
    else if (PersonalEmailId == "") {
        //window.alert("Please Enter Personal EmailId");
        showGlobalModal("I#Required#Please Enter Personal EmailId.");
        $("#PersonalEmailId").focus();
        return false;
    }
    else if (!PersonalEmailId.match(regexp)) {
        //window.alert("Please Enter Valid Email Address");
        showGlobalModal("E#Required#Please Enter Valid Email Address.");
        $("#PersonalEmailId").focus();
        return false;
    }
    else if (AadharNo == "") {
        // window.alert("Please Enter Aadhar Number.");
        showGlobalModal("I#Required#Please Enter Aadhar Number.");
        $("#AadharCardNo").focus();
        return false;
    }
    else if (AadharNo != "" && AadharNo.length < 12) {
        //window.alert("Invalid Aadhar Number.");
        showGlobalModal("E#Required#Invalid Aadhar Number.");
        $("#AadharCardNo").focus();
        return false;
    }
    else if (PanNo == "") {
        // window.alert("Please Enter the PAN Number.");
        showGlobalModal("I#Required#Please Enter the PAN Number.");
        $("#PanCardNo").focus();
        return false;
    }
    else if (PanNo != "" && !PanNo.match(regpan)) {
        //  window.alert("Please Enter Valid PAN Number.");
        showGlobalModal("E#Required#Please Enter Valid PAN Number.");
        $("#PanCardNo").focus();
        return false;
    }
    else if (PanNo != "" && PanNo.length < 10) {
        // window.alert("Invalid PAN Number.");
        showGlobalModal("E#Required#Invalid PAN Number.");
        $("#PanCardNo").focus();
        return false;
    }
    else if (BG == "") {
        //window.alert("Please Enter Blood Group.");
        showGlobalModal("I#Required#Please Enter Blood Group.");
        $("#BloodGroup").focus();
        return false;
    }
    else if (EmergName == "") {
        // window.alert("Please Enter Emergency Contact Name");
        showGlobalModal("I#Required#Please Enter Emergency Contact Name.");
        $("#EmergencyName").focus();
        return false;
    }
    else if (EmergName !== "" && EmergName.length < 3) {
        // window.alert("Please Enter minimum of 3 characters for Emergency contact Name ");
        showGlobalModal("I#Required#Please Enter minimum of 3 characters for Emergency contact Name.");
        $("#EmergencyName").focus();
        return false;
    }
    else if (EmergName !== "" && EmergName.length > 35) {
        // window.alert("Please enter below 35 characters only");
        showGlobalModal("I#Required#Please enter below 35 characters only.");
        $("EmergencyName").focus();
        return false;
    }

    else if (EmergCnctNo == "") {
        //window.alert("Please Enter Emergency Contact Number");
        showGlobalModal("I#Required#Please Enter Emergency Contact Number.");
        $("#EmergencyContactNo").focus();
        return false;
    }
    else if (shotname == "") {
        // window.alert("Please Enter Employee ShortName");
        showGlobalModal("I#Required#Please Enter Employee ShortName.");
        $("#ShortName").focus();
        return false;
    }
    else if (EmpId == "") {
        // window.alert("Please Enter Employee Code.");
        showGlobalModal("I#Required#Please Enter Employee Code.");
        $("#empidd").focus();
        return false;
    }
    else if (EmpId == "0" || EmpId == "00" || EmpId == "000" || EmpId == "0000" || EmpId == "00000" || EmpId == "000000" || EmpId == "0000000" || EmpId == "00000000" || EmpId == "000000000" || EmpId == "0000000000") {
        // window.alert("EmpId Cannot be zero.");
        showGlobalModal("E#Required#EmpId Cannot be zero.");
        $("#empidd").focus();
        return false;
    }
    else if (document.getElementById("head").checked == true && Dept == "Select Department") {
                showGlobalModal("I#Required#Please Select Department.");
                $("#Department").focus();
                return false;
        }

    else if (document.getElementById("bran").checked == true && Br == "Select Branch") {
        showGlobalModal("I#Required#Please Select Branch.");
        $("#Branch").focus();
        return false;

    }
    else if (JoinDesig == "")
    {
        showGlobalModal("I#Required#Please Select join desigantion.");
        $("#JoinedDesignation").focus();
        return false;
    }
    else if (CurrDesig == "" || CurrDesig=="0")
    {
        showGlobalModal("I#Required#Please Select current desigantion.");
        $("#desg1").focus();
        return false;
    }
    else if (Role == "")
    {
        showGlobalModal("I#Required#Please Select Role.");
        $("#Role").focus();
        return false;
    }
    else if (DOJ == "") {
        // window.alert("Please Enter DateOfJoining. ");
        showGlobalModal("I#Required#Please Enter DateOfJoining.");
        $("#DOJ").focus();
        return false;
    }
    //newly added on 06/06/2020
    else if (RelR == "") {
        showGlobalModal("I#Required#Please Select Relieving Reason.");
        $("#Exit_type").focus();
        return false;
    }

    //end
    else if (RetDate == "") {
        // window.alert("Please Enter Retirement Date");
        showGlobalModal("I#Required#Please Enter Retirement Date.");
        $("#RetirementDate").focus();
        return false;
    }
    //newly added on 08/06/2020
    else if (RetDate > currentDatess && RelR == "Death") {
            alert("Please select Today's Date or Lower Date");
            document.getElementById("RetirementDate").focus();
            document.getElementById("RetirementDate").value = '';
            return false;
    }
    
    else if (document.getElementById("head").checked == false && document.getElementById("bran").checked == false) {
        // window.alert("Please choose your Branch.");
        showGlobalModal("I#Required#Please choose your Branch.");
        $("#Branch_Value1").focus();
        return false;
    }
    //else if (document.getElementById("head").checked == true && document.getElementById("Department").value == "") {
    //    //window.alert("Please Select Department.");
    //    showGlobalModal("I#Required#Please Select Department.");
    //    $("#Department").focus();
    //    return false;
    //}
    //else if (document.getElementById("bran").checked == true && document.getElementById("Branch").value == "") {
    //    //window.alert("Please Select Branch.");
    //    showGlobalModal("I#Required#Please Select Branch.");
    //    $("#Branch").focus();
    //    return false;
    //}
   
    else if (OffEmailId != "" && !OffEmailId.match(regexp)) {
        // window.alert("Please Enter Valid Email Address.");
        showGlobalModal("E#Required#Please Enter Valid Email Address.");
        $("#OfficalEmailId").focus();
        return false;
    }
    else if (document.getElementById("head1").checked == false && document.getElementById("bran1").checked == false) {
        // window.alert("Please Choose your Branch.");
        showGlobalModal("I#Required#Please Choose your Branch.");
        $("#Branch_Value_2").focus();
        return false;
    }
        
    else if (document.getElementById("head1").checked == true && controllingdept == "") {
        //window.alert("Please Select Department in Controlling authority.");
        showGlobalModal("I#Required#Please Select Department in Controlling authority.");
        $("#HDept").focus();
        return false;
    }
    else if (document.getElementById("head1").checked == true && controllingdesignation == "0") {
        //window.alert("Please Select Designation in Controlling authority.");
        showGlobalModal("I#Required#Please Select Designation in Controlling authority.");
        $("#Hdesignationcon").focus();
        return false;
    }
    else if (document.getElementById("head1").checked == true && controllingmanager == "0") {
        // window.alert("Please Select Manager Name in Controlling authority");
        showGlobalModal("I#Required#Please Select Manager Name in Controlling authority.");
        $("#HMN").focus();
        return false;
    }
    else if (document.getElementById("bran1").checked == true && controllingbranch == "") {
        //  window.alert("Please Select Branch in Controlling authority.");
        showGlobalModal("I#Required#Please Select Branch in Controlling authority.");
        $("#HbranchCon").focus();
        return false;
    }
    else if (document.getElementById("bran1").checked == true && controllingdesignation == "0") {
        //window.alert("Please Select Designation in Controlling authority.");
        showGlobalModal("I#Required#Please Select Designation in Controlling authority.");
        $("#Hdesignationcon").focus();
        return false;
    }
    else if (document.getElementById("bran1").checked == true && controllingmanager == "0") {
        // window.alert("Please Select Manager Name in Controlling authority");
        showGlobalModal("I#Required#Please Select Manager Name in Controlling authority.");
        $("#HMN").focus();
        return false;
    }
    else if (sanctiondept == "") {
        //window.alert("Please Select Department in Sanction authority.");
        showGlobalModal("I#Required#Please Select Department in Sanction authority.");
        $("#DeptSan").focus();
        return false;
    }
    else if (sanctiondesignation == "0") {
        // window.alert("Please Select Designation in Sanction authority.");
        showGlobalModal("I#Required#Please Select Designation in Sanction authority.");
        $("#designationSan").focus();
        return false;
    }
    else if (sanctionmanager == "0") {
        //window.alert("Please Select Manager Name in Sanction authority");
        showGlobalModal("I#Required#Please Select Manager Name in Sanction authority.");
        $("#mag").focus();
        return false;
    }
    
    else {
        $('#divLoader').show();
        return true;
    }
}
function RegFunction1() {
    debugger;
    var FN = $("#FirstName").val();
    var LN = $("#LastName").val();
    var Pwd = $("#pw1").val();
    var cPwd = $("#pw2").val();
    var male = $("#m").val();
    var female = $("#f").val();
    var single = $("#r1").val();
    var married = $("#r2").val();
    var DOB = $("#DOB").val();
    var Spouse = $("#SpouseName").val();
    var FatherName = $("#FatherName").val();
    var MotherName = $("#MotherName").val();
    var MobileNo = $("#MobileNumber").val();
    var HomeNo = $("#HomeNumber").val();
    var PresentAdd = $("#PresentAddress").val();
    var PermanentAdd = $("#PermanentAddress").val();
    var Grad = $("#Graduation").val();
    var PostGrad = $("#PostGraduation").val();
    var Category = $("#Category").val();
    var PersonalEmailId = $("#PersonalEmailId").val();
    var EmergName = $("#EmergencyName").val();
    var EmergCnctNo = $("#EmergencyContactNo").val();
    var shotname = $("#ShortName").val();

    var AadharNo = $("#AadharCardNo").val();
    var PanNo = $("#PanCardNo").val();
    var BG = $("#BloodGroup").val();
    var EmpId = $("#empidd").val();
    var Br = $("#Branch").val();
    var JoinDesig = $("#JoinedDesignation").val();
    var CurrDesig = $("#desg1").val();
    var Dept = $("#Department").val();
    var Role = $("#Role").val();
    var OffEmailId = $("#OfficalEmailId").val();
    var profqual = $("#ProfessionalQualifications").val();
    var DOJ = $("#DOJ").val();

    var RelDate = $("#RelievingDate").val();
    var RetDate = $("#RetirementDate").val();
    var relreason = $("#RelievingReason").val();
    var totexp = $("TotalExperience").val();
    var controllingdept = $("#HDept").val();
    var controllingbranch = $("#HbranchCon").val();
    var controllingdesignation = $("#Hdesignationcon").val();
    var controllingmanager = $("#HMN").val();
    var sanctiondept = $("#DeptSan").val();
    var sanctiondesignation = $("#designationSan").val();
    var sanctionmanager = $("#mag").val();
    var profheadoffice = $("#head").val();
    var profbranch = $("#bran").val();
    var controlheadoffice = $("#head1").val();
    var controlbranch = $("#bran1").val();
    var image = $("#file").val();
    var shifttiming = $("#shiftid").val();
    var regexp = /^[a-zA-Z][\w\.-]*[a-zA-Z0-9]@[a-zA-Z0-9][\w\.-]*[a-zA-Z0-9]\.[a-zA-Z][a-zA-Z\.]*[a-zA-Z]$/;//email
    var phone = /^(?:(?:\+|0{0,2})91(\s*[\ -]\s*)?|[0]?)?[789]\d{9}|(\d[ -]?){10}\d$/;
    var AlphaNum = /^[a-z0-9]+$/i;
    var regpan = /^([a-zA-Z]){5}([0-9]){4}([a-zA-Z]){1}?$/;
    if (FN == "") {
        window.alert("Please Enter First Name.");
        $("#FirstName").focus();
        return false;
    }
    else if (FN != "" && FN.length < 1) {
        window.alert("Please Enter Mimimum 1 characters For First Name.");
        $("#FirstName").focus();
        return false;
    }
    else if (LN == "") {
        window.alert("Please Enter the Last Name.");
        $("#LastName").focus();
        return false;
    }
    else if (LN != "" && LN.length < 1) {
        window.alert("Please Enter Minimum 1 characters For Last Name.");
        $("#LastName").focus();
        return false;
    }
    else if (Pwd == "") {
        window.alert("Please Enter Password.");
        $("#pw1").focus();
        return false;
    }
    else if (Pwd != "" && Pwd.length < 6) {
        window.alert("Please Enter Minimum 6 characters For Password.");
        $("#pw1").focus();
        return false;
    }
    else if (cPwd == "") {
        window.alert("Please Enter Confirm Password.");
        $("#pw2").focus();
        return false;
    }

    if (Pwd != cPwd) {
        alert('Your Password and Confirm Password do not match.');
        $("#pw2").focus();
        return false;
    }
    else if (document.getElementById("m").checked == false && document.getElementById("f").checked == false) {
        alert("Please choose your Gender: Male or Female.");
        $("#Gender").focus();
        return false;
    }
    else if (DOB == "") {
        window.alert("Please Enter Date Of Birth.");
        $("#DOB").focus();
        return false;
    }
    else if (document.getElementById("r1").checked == false && document.getElementById("r2").checked == false) {
        alert("Please choose your Martial Status.");
        $("#MartialStatus").focus();
        return false;
    }
    else if (FatherName == "") {
        window.alert("Please Enter Father's Name.");
        $("#FatherName").focus();
        return false;
    }
    else if (FatherName !== "" && FatherName.length < 3) {
        window.alert("Please enter minimum 3 characters for FatherName.");
        $("#FatherName").focus();
        return false;
    }
    else if (FatherName !== "" && FatherName.length > 35) {
        window.alert("Please enter Father Name in 35 characters only.");
        $("#FatherName").focus();
        return false;
    }

    else if (MotherName == "") {
        window.alert("Please Enter Mother's Name.");
        $("#MotherName").focus();
        return false;
    }
    else if (MotherName !== "" && MotherName.length < 3) {
        window.alert("Please enter minimum 3 characters for Mother Name.");
        $("#MotherName").focus();
        return false;
    }
    else if (MotherName !== "" && MotherName.length > 35) {
        window.alert("Please enter Mother Name in  35 characters only.");
        $("#MotherName").focus();
        return false;
    }
    else if (MobileNo == "") {
        window.alert("Please Enter Mobile Number. ");
        $("#MobileNumber").focus();
        return false;
    }
    else if (MobileNo !== "" && MobileNo.length < 10) {
        window.alert("Please enter Correct Mobile Number.");
        $("#MobileNumber").focus();
        return false;
    }
    //else if (!mobileno.match(phone)) {
    //    window.alert("please enter valid mobile number.");
    //    $("#mobilenumber").focus();
    //    return false;
    //}
    else if (PresentAdd == "") {
        window.alert("Please Enter Present Address. ");
        $("#PresentAddress").focus();
        return false;
    }
    else if (PermanentAdd == "") {
        window.alert("Please Enter Permanent Address. ");
        $("#PermanentAddress").focus();
        return false;
    }
    else if (Grad == "") {
        window.alert("Please Enter the type of Graduation.");
        $("#Graduation").focus();
        return false;
    }
    else if (Grad != "" && Grad.length < 2) {
        window.alert("Please enter minimum of 2 characters.");
        $("#Graduation").focus();
        return false;
    }
    else if (document.getElementById("Category").value == "") {
        window.alert("Please Select Category.");
        $("#Category").focus();
        return false;
    }
    else if (PersonalEmailId == "") {
        window.alert("Please Enter Personal EmailId");
        $("#PersonalEmailId").focus();
        return false;
    }
    else if (!PersonalEmailId.match(regexp)) {
        window.alert("Please Enter Valid Email Address");
        $("#PersonalEmailId").focus();
        return false;
    }
    else if (EmergName == "") {
        window.alert("Please Enter Emergency Contact Name");
        $("#EmergencyName").focus();
        return false;
    }
    else if (EmergName !== "" && EmergName.length < 3) {
        window.alert("Please Enter minimum of 3 characters for Emergency contact Name ");
        $("#EmergencyName").focus();
        return false;
    }
    else if (EmergName !== "" && EmergName.length > 35) {
        window.alert("Please enter below 35 characters only");
        $("EmergencyName").focus();
        return false;
    }

    else if (EmergCnctNo == "") {
        window.alert("Please Enter Emergency Contact Number");
        $("#EmergencyContactNo").focus();
        return false;
    }
    else if (shotname == "") {
        window.alert("Please Enter Employee ShortName");
        $("#ShortName").focus();
        return false;
    }
    else if (document.getElementById("DOJ").value == "") {
        window.alert("Please Enter DateOfJoining. ");
        $("#DOJ").focus();
        return false;
    }

    else if (RetDate == "") {
        window.alert("Please Enter Retirement Date");
        $("#RetirementDate").focus();
        return false;
    }
    //else if (!EmergCnctNo.match(phone)) {
    //    window.alert("Please enter Valid Emergency Contact Number ");
    //    $("#EmergencyContantNo").focus();
    //    return false;
    //}

    else if (AadharNo == "") {
        window.alert("Please Enter Aadhar Number.");
        $("#AadharCardNo").focus();
        return false;
    }
    else if (AadharNo != "" && AadharNo.length < 12) {
        window.alert("Invalid Aadhar Number.");
        $("#AadharCardNo").focus();
        return false;
    }
    else if (PanNo == "") {
        window.alert("Please Enter the PAN Number.");
        $("#PanCardNo").focus();
        return false;
    }
    else if (PanNo != "" && !PanNo.match(regpan)) {
        window.alert("Please Enter Valid PAN Number.");
        $("#PanCardNo").focus();
        return false;
    }
    else if (PanNo != "" && PanNo.length < 10) {
        window.alert("Invalid PAN Number.");
        $("#PanCardNo").focus();
        return false;
    }
    else if (BG == "") {
        window.alert("Please Enter Blood Group.");
        $("#BloodGroup").focus();
        return false;
    }
    else if (EmpId == "") {
        window.alert("Please Enter Employee Code.");
        $("#empidd").focus();
        return false;
    }
    else if (shifttiming == "") {
        window.alert("Please Enter Shift Timing.");
        $("#shiftid").focus();
        return false;
    }
    //else if (EmpId.startsWith(0)) {
    //    window.alert("Please Enter valid Employee Code.");
    //    $("#empidd").focus();
    //    $("#empidd").val('');
    //    return false;
    //}
    else if (document.getElementById("head").checked == false && document.getElementById("bran").checked == false) {
        window.alert("Please choose your Branch.");
        $("#Branch_Value1").focus();
        return false;
    }
    else if (document.getElementById("head").checked == true && document.getElementById("Department").value == "") {
        window.alert("Please Select Department.");
        $("#Department").focus();
        return false;
    }
    else if (document.getElementById("bran").checked == true && document.getElementById("Branch").value == "") {
        window.alert("Please Select Branch.");
        $("#Branch").focus();
        return false;
    }
    else if (document.getElementById("JoinedDesignation").value == "") {
        window.alert("Please Select Joined Designation.");
        $("#JoinedDesignation").focus();
        return false;
    }
    else if (document.getElementById("desg1").value == "" || document.getElementById("desg1").value == 0) {
        window.alert("Please Select Current Designation.");
        $("#desg1").focus();
        return false;
    }
    else if (document.getElementById("Role").value == "") {
        window.alert("Please Select Role.");
        $("#Role").focus();
        return false;
    }
    else if (OffEmailId != "" && !OffEmailId.match(regexp)) {
        window.alert("Please Enter Valid Email Address.");
        $("#OfficalEmailId").focus();
        return false;
    }

    else if (DOJ == "") {
        window.alert("Please Enter DateOfJoining. ");
        $("#DOJ").focus();
        return false;
    }
    else if (document.getElementById("head1").checked == false && document.getElementById("bran1").checked == false) {
        window.alert("Please Choose your Branch.");
        $("#Branch_Value_2").focus();
        return false;
    }
    else if (document.getElementById("head1").checked == true && document.getElementById("HDept").value == "") {
        window.alert("Please Select Department in Controlling authority.");
        $("#HDept").focus();
        return false;
    }
    else if (document.getElementById("bran1").checked == true && document.getElementById("HbranchCon").value == "") {
        window.alert("Please Select Branch in Controlling authority.");
        $("#HbranchCon").focus();
        return false;
    }
    else if (document.getElementById("Hdesignationcon").value == 0) {
        window.alert("Please Select Designation in Controlling authority.");
        $("#Hdesignationcon").focus();
        return false;
    }
    else if (document.getElementById("HMN").value == 0) {
        window.alert("Please Select Manager Name in Controlling authority");
        $("#HMN").focus();
        return false;
    }
    else if (document.getElementById("DeptSan").value == "") {
        window.alert("Please Select Department in Sanction authority.");
        $("#DeptSan").focus();
        return false;
    }
    else if (document.getElementById("designationSan").value == 0) {
        window.alert("Please Select Designation in Sanction authority.");
        $("#designationSan").focus();
        return false;
    }
    else if (document.getElementById("mag").value == 0) {
        window.alert("Please Select Manager Name in Sanction authority");
        $("#mag").focus();
        return false;
    }
    else {
        $('#divLoader').show();
        return true;
    }
}
// validation for LeaveHistory in MyLeave for admin
function leavehistoryadmin() {
    var fromdate = $("#SDate").val();
    var todate = $("#EDate").val();
    var leavetype = $("#leaveid").val();
    if (fromdate == "") {
        // window.alert("Please Select From Date. ");
        showGlobalModal("I#Required#Please Select From Date.");
        $("#SDate").focus();
        return false;
    }
    else if (todate == "") {
        // window.alert("Please Select To Date.");
        showGlobalModal("I#Required#Please Select To Date.");
        $("#EDate").focus();
        return false;
    }
    else if (document.getElementById("leaveid").value == "") {
        //window.alert("Please Select Leave Type.");
        showGlobalModal("I#Required#Please Select Leave Type.");
        $("#leaveid").focus();
        return false;
    }
    else {
        // $('#divLoader').show();
        return true;

    }
}



//Deputationmyhistoty
// validation for LeaveHistory in MyLeave for admin
function Deputationhistoryadmin() {
    var fromdate = $("#SDate").val();
    var todate = $("#EDate").val();

    if (fromdate == "") {
        // window.alert("Please Select From Date. ");
        showGlobalModal("I#Required#Please Select Start Date.");
        $("#SDate").focus();
        return false;
    }
    else if (todate == "") {

        // window.alert("Please Select To Date.");
        showGlobalModal("I#Required#Please Select Start Date.");
        $("#EDate").focus();
        return false;
    }

    else {

        return true;

    }

}
//Banks(Masters) feild Validations
function Banks() {
    var BN = $("#baName").val();
    var Address1 = $("#lbanksAddr1").val();
    var Address2 = $("#lbanksAddr2").val();
    var City = $("#lbankscity").val();
    var Phone1 = $("#lbanksphone1").val();
    var Phone2 = $("#lbanksphone2").val();
    var Phone3 = $("#lbanksphone3").val();
    var character = /^[^<>;]+$/;
    if (BN == "") {
        //window.alert("Please Enter the Bank Name.");
        showGlobalModal("I#Required#Please Enter the Bank Name.");
        $("#baName").focus();
        return false;
    }
    if (BN != "" && BN.length < 3) {
        //window.alert("Please Enter Minimum 3 characters for Bank Name.");
        showGlobalModal("I#Required#Please Enter Minimum 3 characters for Bank Name.");
        $("#baName").focus();
        return false;
    }
    if (BN != "" && BN.length > 35) {
        //window.alert("Please Enter Maximum 35 characters only for Bank Name.");
        showGlobalModal("I#Required#Please Enter Maximum 35 characters only for Bank Name.");
        $("#baName").focus();
        return false;
    }
    else if (Address1 == "") {
        //window.alert("Please Enter AddressLine 1.");
        showGlobalModal("I#Required#Please Enter AddressLine 1.");
        $("#lbanksAddr1").focus();
        return false;
    }
    else if (Address2 == "") {
        //window.alert("Please Enter AddressLine 2.");
        showGlobalModal("I#Required#Please Enter AddressLine 2.");
        $("#lbanksAddr2").focus();
        return false;
    }
    else if (City == "") {
        //window.alert("Please Enter City.");
        showGlobalModal("I#Required#Please Enter City.");
        $("#lbankscity").focus();
        return false;
    }
    else if (City != "" && City.length < 3) {
        //window.alert("Please Enter Minimum 3 characters for City.");
        showGlobalModal("I#Required#Please Enter Minimum 3 characters for City.");
        $("#lbankscity").focus();
        return false;
    }
    else if (City != "" && City.length > 35) {
        //window.alert("Please Enter Maximum 35 characters only for City.");
        showGlobalModal("I#Required#Please Enter Maximum 35 characters only for City.");
        $("#lbankscity").focus();
        return false;
    }
    else if (Phone1 == "") {
        //window.alert("Please Enter PhoneNumber.");
        showGlobalModal("I#Required#Please Enter PhoneNumber.");
        $("#lbanksphone1").focus();
        return false;
    }
    else if (Phone1 != "" && Phone1.length < 10) {
        //window.alert("Please Enter Valid Mobile Number.");
        showGlobalModal("I#Required#Please Enter Valid Mobile Number.");
        $("#lbanksphone1").focus();
        return false;
    }
    else if (Phone2 != "" && Phone2.length < 10) {
        //window.alert("Please Enter Valid Mobile Number.");
        showGlobalModal("I#Required#Please Enter Valid Mobile Number.");
        $("#lbanksphone2").focus();
        return false;
    }
    else if (Phone3 != "" && Phone3.length < 10) {

        //window.alert("Please Enter Valid Mobile Number");
        showGlobalModal("I#Required#Please Enter Valid Mobile Number.");
        $("#lbanksphone3").focus();
        return false;
    }
    else if (image == "") {
        //window.alert("Please Upload Photo.");
        showGlobalModal("I#Required#Please Upload Photo.");
        $("#file").focus();
        return false;
    }

    else {
        $('#divLoader').show();
        return true;
    }

}
//Branches(Masters) feild Validations
function Branches() {
    var BN = $("#bname").val();
    var BrN = $("#brName").val();
    var BC = $("#bc").val();
    var Address1 = $("#add1").val();
    var Address2 = $("#add2").val();
    var City = $("#cit").val();
    var Phone1 = $("#lbranchPhoneNo1").val();
    var Phone2 = $("#lbranchPhoneNo2").val();
    var Phone3 = $("#lbranchPhoneNo3").val();
    if (document.getElementById("bname").value == "") {
        // window.alert("Please Select BankName");
        showGlobalModal("I#Required#Please Select BankName.");
        $("#bname").focus();
        return false;
    }
    else if (BrN == "") {
        // window.alert("Please Enter Branch Name.");
        showGlobalModal("I#Required#Please Enter Branch Name.");
        $("#brName").focus();
        return false;
    }
    else if (BrN != "" && BrN.length < 3) {
        //window.alert("Please Enter Minimum of 3 characters for Branch Name.");
        showGlobalModal("I#Required#Please Enter Minimum of 3 characters for Branch Name.");
        $("#brName").focus();
        return false;
    }
    else if (BrN != "" && BrN.length > 35) {
        //  window.alert("Please Enter Maximum of 35 characters  only for Branch Name.");
        showGlobalModal("I#Required#Please Enter Maximum of 35 characters  only for Branch Name.");
        $("#brName").focus();
        return false;
    }
    //else if (BC == "") {
    //    window.alert("Please Enter Branch Code. ");
    //    $("#bc").focus(); 
    //    return false;
    //}
    //else if (BC != "" && BC.length < 3) {
    //    window.alert("Please Enter Minimum of 3 characters for Branch Code.");
    //    $("#bc").focus();
    //    return false;
    //}
    //else if (BC != "" && BC.length > 35) {
    //    window.alert("Please Enter Maximum of 35 characters only for Branch Code.");
    //    $("#bc").focus();
    //    return false;
    //}
    else if (Address1 == "") {
        //window.alert("Please Enter AddressLine 1.");
        showGlobalModal("I#Required#Please Enter AddressLine 1.");
        $("#add1").focus();
        return false;
    }
    else if (Address2 == "") {
        // window.alert("Please Enter AddressLine 2.");
        showGlobalModal("I#Required#Please Enter AddressLine 2.");
        $("#add2").focus();
        return false;
    }
    else if (City == "") {
        // window.alert("Please Enter City.");
        showGlobalModal("I#Required#Please Enter City.");
        $("#cit").focus();
        return false;
    }
    else if (City != "" && City.length < 3) {
        //window.alert("Please Enter Minimum of 3 characters for City. ");
        showGlobalModal("I#Required#Please Enter Minimum of 3 characters for City.");
        $("#cit").focus();
        return false;
    }
    else if (City != "" && City.length > 35) {
        //window.alert("Please Enter Maximum of 35 characters only for City.");
        showGlobalModal("I#Required#Please Enter Maximum of 35 characters only for City.");
        $("#cit").focus();
        return false;
    }
    else if (Phone1 == "") {
        // window.alert("Please Enter PhoneNumber.");
        showGlobalModal("I#Required#Please Enter PhoneNumber.");
        $("#lbanksphone1").focus();
        return false;
    }
    else if (Phone1 != "" && Phone1.length < 10) {
        //window.alert("Please Enter Valid Mobile Number.");
        showGlobalModal("I#Required#Please Enter Valid Mobile Number.");
        $("#lbranchPhoneNo1").focus();
        return false;
    }
    else if (Phone2 != "" && Phone2.length < 10) {
        // window.alert("Please Enter Valid Mobile Number.");
        showGlobalModal("I#Required#Please Enter Valid Mobile Number.");
        $("#lbranchPhoneNo2").focus();
        return false;
    }
    else if (Phone3 != "" && Phone3.length < 10) {
        //window.alert("Please Enter Valid Mobile Number.");
        showGlobalModal("I#Required#Please Enter Valid Mobile Number.");
        $("#lbranchPhoneNo3").focus();
        return false;
    }
    else {
        $('#divLoader').show();
        return true;
    }

}
//Edit Branches(Masters) feild Validations
function EditBranches() {
    var BN = $("#BankName").val();
    var BrN = $("#brName").val();
    var BC = $("#bc").val();
    var Address1 = $("#add1").val();
    var Address2 = $("#add2").val();
    var City = $("#cit").val();
    var Phone1 = $("#PhoneNo1").val();
    var Phone2 = $("#PhoneNo2").val();
    var Phone3 = $("#PhoneNo3").val();
    if (document.getElementById("BankName").value == "") {
        window.alert("Please Select BankName");
        $("#BankName").focus();
        return false;
    }
    else if (BrN == "") {
        window.alert("Please Enter Branch Name.");
        $("#brName").focus();
        return false;
    }
    //else if (BC == "") {
    //    window.alert("Please Enter Branch Code. ");
    //    $("#bc").focus();
    //    return false;
    //}
    else if (Address1 == "") {
        window.alert("Please Enter AddressLine 1.");
        $("#add1").focus();
        return false;
    }
    else if (Address2 == "") {
        window.alert("Please Enter AddressLine 2.");
        $("#add2").focus();
        return false;
    }
    else if (City == "") {
        window.alert("Please Enter City.");
        $("#cit").focus();
        return false;
    }
    else if (Phone1 == "") {
        window.alert("Please Enter PhoneNumber.");
        $("#PhoneNo1").focus();
        return false;
    }
    else if (Phone1 != "" && Phone1.length < 10) {
        window.alert("Please Enter Valid Phone Number.");
        $("#PhoneNo1").focus();
        return false;
    }
    else if (Phone2 != "" && Phone2.length < 10) {
        window.alert("Please Enter Valid Phone Number.");
        $("#PhoneNo2").focus();
        return false;
    }
    else if (Phone3 != "" && Phone3.length < 10) {
        window.alert("Please Enter Valid Phone Number.");
        $("#PhoneNo3").focus();
        return false;
    }
    else {

        return true;
    }

}
//Edit Banks(Masters) feild Validations
function BanksEdit() {
    var BN = $("#Name").val();
    var Address1 = $("#AddressLine1").val();
    var Address2 = $("#AddressLine2").val();
    var City = $("#City").val();
    var Phone1 = $("#lbanksphone1").val();
    var Phone2 = $("#lbanksphone2").val();
    var Phone3 = $("#lbanksphone3").val();
    if (BN == "") {
        window.alert("Please Enter the Bank Name.");
        $("#Name").focus();
        return false;
    }
    else if (Address1 == "") {
        window.alert("Please Enter AddressLine 1.");
        $("#AddressLine1").focus();
        return false;
    }
    else if (Address2 == "") {
        window.alert("Please Enter AddressLine 2.");
        $("#AddressLine2").focus();
        return false;
    }
    else if (City == "") {
        window.alert("Please Enter City.");
        $("#City").focus();
        return false;
    }
    else if (Phone1 == "") {
        window.alert("Please Enter Phone Number.");
        $("#lbanksphone1").focus();
        return false;
    }
    else if (Phone1 != "" && Phone1.length < 10) {
        window.alert("Please Enter Valid Phone Number.");
        $("#lbanksphone1").focus();
        return false;
    }
    else if (Phone2 != "" && Phone2.length < 10) {
        window.alert("Please Enter Valid Phone Number.");
        $("#lbanksphone1").focus();
        return false;
    }
    else if (Phone3 != "" && Phone3.length < 10) {
        window.alert("Please Enter Valid Phone Number.");
        $("#lbanksphone1").focus();
        return false;
    }
    else {

        return true;
    }

}
//Departments(Masters) feild Validations
function department() {
    var code = $("#c").val();
    var name = $("#na").val();
    if (code == "") {
        // window.alert("Please Enter the Department Code.");
        showGlobalModal("I#Required#Please Enter the Department Code.");
        $("#c").focus();
        return false;
    }
    else if (name == "") {
        //window.alert("Please Enter the Department Name.");
        showGlobalModal("I#Required#Please Enter the Department Name.");
        $("#na").focus();
        return false;
    }
    else {
        $('#divLoader').show();
        return true;
    }

}
//Designations(Masters) feild Validations
function designation() {
    var code = $("#cod").val();
    var name = $("#nam").val();
    if (code == "") {
        //window.alert("Please Enter the Designation Code.");
        showGlobalModal("I#Required#Please Enter the Designation Code");
        $("#cod").focus();
        return false;
    }
    else if (name == "") {
        // window.alert("Please Enter the Designation Name.");
        showGlobalModal("I#Required#Please Enter the Designation Name");
        $("#nam").focus();
        return false;
    }
    else {
        $('#divLoader').show();
        return true;
    }

}
//Roles(Masters) feild Validations
function role() {
    var id = $("#i").val();
    var name = $("#n").val();

    if (name == "") {
        //window.alert("Please Enter the Role Name.");
        showGlobalModal("I#Required#Please Enter the Role Name.");
        $("#n").focus();
        return false;
    }
    else {
        $('#divLoader').show();
        return true;
    }

}
//LeaveType(Masters) feild Validations
function LeaveType() {
    var type = $("#typ").val();
    var code = $("#code").val();
    if (type == "") {
        //window.alert("Please Enter the Leave Type.");
        showGlobalModal("I#Required#Please Enter the Leave Type.");
        $("#typ").focus();
        return false;
    }
    else if (code == "") {
        // window.alert("Please Enter the Leave Code.");
        showGlobalModal("I#Required#Please Enter the Leave Code.");
        $("#code").focus();
        return false;
    }
    else {
        $('#divLoader').show();
        return true;
    }

}

function PayrollAlerts() {

    var UANno = $("#UANno").val();
    var BAno = $("#BAno").val();
    var FixedGross = $("#FixedGross").val();

    if (UANno == "") {
        window.alert("Please Enter UAN PF Number.");
        //showGlobalModal("I#RequiredPlease Enter the Leave Type.");
        $("#UANno").focus();
        return false;
    }
    else if (BAno == "") {
        window.alert("Please Enter Bank Account Number.");
        //showGlobalModal("I#Required#Please Enter the Leave Code.");
        $("#BAno").focus();
        return false;
    }
    else if (FixedGross == "") {
        window.alert("Please Enter Fixed Gross.");
        //showGlobalModal("I#Required#Please Enter the Leave Code.");
        $("#FixedGross").focus();
        return false;
    }
    else {
        $('#divLoader').show();
        return true;
    }

}





//Edit Designations(Masters) feild Validations
function desigedit() {

    var code = $("#Code").val();
    var name = $("#Name").val();
    if (code == "") {
        window.alert("Please Enter the Designation Code.");
        $("#Code").focus();
        return false;
    }
    else if (name == "") {
        window.alert("Please Enter the Designation Name.");
        $("#Name").focus();
        return false;
    }

    else {

        return true;
    }

}
//Edit Departments(Masters) feild Validations
function deptedit() {
    var code = $("#Code").val();
    var name = $("#Name").val();
    if (code == "") {
        window.alert("Please Enter the Department Code.");
        $("#Code").focus();
        return false;
    }
    else if (name == "") {
        window.alert("Please Enter the Department Name.");
        $("#Name").focus();
        return false;
    }
    else {

        return true;
    }

}
//Edit Roles(Masters) feild Validations
function roleedit() {
    var name = $("#Name").val();
    if (name == "") {
        window.alert("Please Enter the Role Name.");
        $("#Name").focus();
        return false;
    }
    else {

        return true;
    }

}
// Holiday List field Validations
function holiday() {
    var occ = $("#Occas").val();
    var d = $("#dat").val();
    if (occ == "") {
        //window.alert("Please Enter the Occassion.");
        showGlobalModal("I#Required#Please Enter the Occassion.");
        $("#Occas").focus();
        return false;
    }
    else if (d == "") {
        //window.alert("Please Enter the Date.");
        showGlobalModal("I#Required#Please Enter the Date.");
        $("#dat").focus();
        return false;
    }
    else {
        $('#divLoader').show();
        return true;
    }
}
// Holiday List field Validations
function holiedit() {
    var occ = $("#Occas").val();
    var date = $("#dat").val();
    if (occ == "") {
        window.alert("Please Enter the Occassion.");
        $("#Occas").focus();
        return false;
    }
    else if (date == "") {
        window.alert("Please Enter the Date.");
        $("#dat").focus();
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
//No Characters,No Numbers,No symbols
function nokeyword(evt) {

    evt = (evt) ? evt : window.event;
    var charCode = (evt.which) ? evt.which : evt.keyCode;
    if (charCode > 31 && charCode < 125) {
        alert("Please Select from Picker");
        return false;
    }
    return true;
}

//Forgot Password
function forget() {

    var Member = $("#OfficalEmailId").val();
    var regex = /^([0-9a-zA-Z]([-_\\.]*[0-9a-zA-Z]+)*)@@([0-9a-zA-Z]([-_\\.]*[0-9a-zA-Z]+)*)[\\.]([a-zA-Z]{2,9})$/;

    if (Member == "") {
        window.alert("Please Enter Emp Id or Email. ");
        $("#OfficalEmailId").focus();
        return false;
    }
    //else if (Member != "" && Member.length < 6 ) {
    //    window.alert("Please Enter Valid Employee ID");
    //    $("#PersonalEmailId").focus();
    //    return false;
    //}



    //else if (!regex.test(Member)) {
    //    window.alert("Enter valid Email Id");
    //    $("#PersonalEmailId").focus();
    //    return false;
    //}
    else {

        return true;
    }


};
// Compare date1
function myDateFunction1() {

    var Joindate = document.getElementById("DOJ").value;
    var Retdate = document.getElementById("RetirementDate").value;
    if ((Joindate) >= (Retdate)) {
        alert('Retirement Date Should be More than DOJ Date');
        document.getElementById("RetirementDate").focus();
        document.getElementById("RetirementDate").value = '';
        return false;
    }

    else
        return true;
}
// Compare date2
function myDateFunction2() {

    var Joindate = document.getElementById("DOJ").value;
    var Reldate = document.getElementById("RelievingDate").value;

    if ((Joindate) >= (Reldate)) {
        alert('Releving Date Should be More than DOJ  Date');
        document.getElementById("RelievingDate").focus();
        document.getElementById("RelievingDate").value = '';
        return false;
    }
    else
        return true;
}
// Alphanumeric
function Alphanumeric(str) {
    var code, i, len;
    for (i = 0, len = str.length; i < len; i++) {
        code = str.charCodeAt(i);
        if (!(code > 47 && code < 58) && // numeric (0-9)
            !(code > 64 && code < 91) && // upper alpha (A-Z)
            !(code > 96 && code < 123)) { // lower alpha (a-z)
            return false;
        }
    }

    return true;
};
// Only Characters
function checkNum(event) {
    if ((event.keyCode > 64 && event.keyCode < 91) || (event.keyCode > 96 && event.keyCode < 123) || event.keyCode == 8 || event.keyCode == 32 || event.keyCode == 190)
        return true;
    else {
        alert("Please Enter Only Characters.");
        return false;
    }

}
//Only Charaters and Special Characters Validtaion
function checkNum1(evt) {
    evt = (evt) ? evt : window.evt;
    var charCode = (evt.which) ? evt.which : evt.keyCode;
    if ((charCode >= 65 && charCode <= 90) || (charCode >= 97 && charCode <= 122) || charCode == 32 || charCode == 190 || !(charCode > 47 && charCode < 58))
        return true;
    else {
        alert("Please Enter Only Characters.");
        return false;
    }

}

//image validation
function Checkfiles() {
    var fup = document.getElementById('file');
    var fileName = fup.value;
    var ext = fileName.substring(fileName.lastIndexOf('.') + 1);
    if (ext == "JPEG" || ext == "jpeg" || ext == "jpg" || ext == "JPG" || ext == "png" || ext == "PNG") {
        return true;
    }
    else {
        alert("Upload JPEG,JPG,PNG image format only.");
        fup.focus();
        document.getElementById("file").value = "";
        return false;
    }
}
// LTC for Apply Leave in MyLeave for admin and employee
function LTCapply() {

    var stat = $("#status").val();
    var startdate = $("#sdate").val();
    var enddate = $("#edate").val();
    var purpose = $("#reason1").val();
    var descrip = $("#subject").val();


    if (startdate == "") {
        window.alert("Please Select Start Date");
        $("#sdate").focus();
        return false;
    }
    else if (enddate == "") {
        window.alert("Please Select End Date");
        $("#edate").focus();
        return false;
    }
    else if (document.getElementById("subject").value == "") {
        window.alert("Please Enter Subject.");
        $("#subject").focus();
        return false;
    }
    else if (document.getElementById("reason1").value == "") {
        window.alert("Please Enter Reason");
        $("#reason1").focus();
        return false;
    }
    else {

        $('.subleave1').attr("disabled", true);
        document.Index.submit();
    }
}
function LTCapply2() {

    var fromdate = $("#SDate").val();
    var todate = $("#EDate").val();

    if (fromdate == "") {
        //window.alert("Please Select Start Date");
        showGlobalModal("I#Required#Please Select Start Date.");
        $("#SDate").focus();
        return false;
    }
    else if (todate == "") {
        //window.alert("Please Select End Date");
        showGlobalModal("I#Required#Please Select End Date.");
        $("#EDate").focus();
        return false;
    }
    else {

        $('.subleave1').attr("disabled", true);
        document.Index.submit();
    }
}
function LTCapply2() {

    var fromdate = $("#SDate").val();
    var todate = $("#EDate").val();

    if (fromdate == "") {
        //window.alert("Please Select Start Date");
        showGlobalModal("I#Required#Please Select Start Date.");
        $("#SDate").focus();
        return false;
    }
    else if (todate == "") {
        //  window.alert("Please Select End Date");
        showGlobalModal("I#Required#Please Select End Date.");
        $("#EDate").focus();
        return false;
    }
    else {

        $('.subleave1').attr("disabled", true);
        document.Index.submit();
    }
}
function LTCapply3() {

    var fromdate = $("#Stdate1").val();
    var todate = $("#Endate1").val();

    if (fromdate == "") {
        window.alert("Please Select Start Date");
        $("#Stdate1").focus();
        return false;
    }
    else if (todate == "") {
        window.alert("Please Select End Date");
        $("#Endate1").focus();
        return false;
    }
    else {

        $('.subleave1').attr("disabled", true);
        document.Index.submit();
    }
}
// Deputation for Apply Leave in MyLeave for admin and employee
function deputationapply() {

    var from = $("#Visitingfrom").val();
    var to = $("#VistorTo").val();
    var startdate = $("#sdate").val();
    var enddate = $("#edate").val();
    var purpose = $("#Purpose").val();
    var descrip = $("#reason1").val();

    if (to == "") {
        // window.alert("Please Enter Deputation Location.");
        showGlobalModal("I#Required#Please Enter Deputation Location.");
        $("#VistorTo").focus();
        return false;
    }
    else if (startdate == "") {
        // window.alert("Please Select Start Date");
        showGlobalModal("I#Required#Please Select Start Date.");
        $("#sdate").focus();
        return false;
    }
    else if (enddate == "") {
        //window.alert("Please Select End Date");
        showGlobalModal("I#Required#Please Select End Date.");
        $("#edate").focus();
        return false;
    }
    else if (document.getElementById("Purpose").value == "") {
        // window.alert("Please Select Purpose.");
        showGlobalModal("I#Required#Please Select Purpose.");
        $("#Purpose").focus();
        return false;
    }
    else if (descrip == "") {
        //window.alert("Please Enter the Description.");
        showGlobalModal("I#Required#Please Enter the Description.");
        $("#reason1").focus();
        return false;
    }
    else {

        $('.subleave1').attr("disabled", true);
        document.Index.submit();
    }
}
// validation for Apply Leave in MyLeave for admin and employee
function leaveapply() {

    var leavetype = $("#Type").val();
    var startdate = $("#sdate").val();
    var enddate = $("#edate").val();

    var deldate = $("#delivery").val();
    var reasn = $("#reason").val();
    if (document.getElementById("Type").value == "") {
        //window.alert("Please Select Leave Type.");
        showGlobalModal("I#Required#Please Select Leave Type.");
        $("#Type").focus();
        return false;
    }
    else if (startdate == "") {
        //window.alert("Please Select Start Date. ");
        showGlobalModal("I#Required#Please Select Start Date.");
        $("#sdate").focus();
        return false;
    }
    else if (enddate == "") {
        //window.alert("Please Select End Date.");
        showGlobalModal("I#Required#Please Select End Date.");
        $("#edate").focus();
        return false;
    }

    else if (document.getElementById("Type").value == "5" && deldate == "") {
        // window.alert("Please Select Delivery Date.");
        showGlobalModal("I#Required#Please Select Delivery Date.");
        $("#delivery").focus();
        return false;
    }
    else if (reasn == "") {

        //window.alert("Please Enter Reason.");
        showGlobalModal("I#Required#Please Enter Reason.");
        $("#reason").focus();
        return false;
    }
    else {
        $('#divLoader').show();
        $('.subleave').attr("disabled", true);
        document.LeaveRequest.submit();
        //return true;
    }
}

function leaveapplyforother() {

    var empcode = $("#empcode").val();
    var leavetype = $("#Type").val();
    var startdate = $("#sdate").val();
    var enddate = $("#edate").val();
    var reasn = $("#reason").val();
    if (empcode == "") {
        showGlobalModal("I#Required#Please enter Empcode.");
        document.getElementById("empcode").focus();
        return false;
    }
    if (document.getElementById("Type").value == "") {
        //window.alert("Please Select Leave Type.");
        showGlobalModal("I#Required#Please Select Leave Type.");
        $("#Type").focus();
        return false;
    }
    else if (startdate == "") {
        //window.alert("Please Select Start Date. ");
        showGlobalModal("I#Required#Please Select Start Date.");
        $("#sdate").focus();
        return false;
    }
    else if (enddate == "") {
        //window.alert("Please Select End Date.");
        showGlobalModal("I#Required#Please Select End Date.");
        $("#edate").focus();
        return false;
    }


    else if (reasn == "") {

        //window.alert("Please Enter Reason.");
        showGlobalModal("I#Required#Please Enter Reason.");
        $("#reason").focus();
        return false;
    }
    else {
        $('#divLoader').show();
        $('.subleave').attr("disabled", true);
        document.Leaveforemployee.submit();
        //return true;
    }
}

// validation for Leave Cancellation
function leaveapply1() {

    var startdate = $("#csdate").val();
    var enddate = $("#cedate").val();
    var abc = $("#REASON1").val();
    if (startdate == "") {
        window.alert("Please Enter Cancel StartDate.");
        $("#csdate").focus();
        return false;
    }
    else if (enddate == "") {
        window.alert("Please Enter Cancel EndDate.");
        $("#cedate").focus();
        return false;
    }
    else if (abc == "") {
        window.alert("Please Enter Reason.");
        $("#REASON1").focus();
        return false;
    }
    else {
        return true;
    }
}
//Validation for Leave Cancellation(full)

function leaveapply2() {
    var abc = $("#Reason123").val();
    if (abc == "") {
        window.alert("Please Enter Reason.");
        $("#Reason123").focus();
        return false;
    }
    else {
        return true;
    }
}

// validation for LeaveHistory in MyLeave for admin
function leavehistory() {
    var fromdate = $("#SDate").val();
    var todate = $("#EDate").val();
    var leavetype = $("#leaveid").val();
    if (document.getElementById("lApplied").checked == true && fromdate == "") {
        window.alert("Please Select From Date. ");
        $("#SDate").focus();
        return false;
    }
    else if (document.getElementById("lRequest").checked == true && fromdate == "") {
        window.alert("Please Select From Date. ");
        $("#SDate").focus();
        return false;
    }
    else if (document.getElementById("lApplied").checked == true && todate == "") {
        window.alert("Please Select To Date.");
        $("#EDate").focus();
        return false;
    }
    else if (document.getElementById("lRequest").checked == true && todate == "") {
        window.alert("Please Select To Date.");
        $("#EDate").focus();
        return false;
    }
    else if (document.getElementById("lApplied").checked == true && document.getElementById("leaveid").value == "") {
        window.alert("Please Select Leave Type.");
        $("#leaveid").focus();
        return false;
    }
    else if (document.getElementById("lRequest").checked == true && document.getElementById("leaveid").value == "") {
        window.alert("Please Select Leave Type.");
        $("#leaveid").focus();
        return false;
    }
    else {
        return true;
    }
}
// Validation for Employee LeaveHistory in leave for admin
function Empleavehist() {

    var fromdate1 = $("#SDate1").val();
    var todate1 = $("#EDate1").val();
    var leavetype1 = $("#leaveid1").val();
    var Status = $("#Status").val();
    if (fromdate1 == "") {
        window.alert("Please Select From Date. ");
        $("#SDate1").focus();
        return false;
    }
    else if (todate1 == "") {
        window.alert("Please Select To Date.");
        $("#EDate1").focus();
        return false;
    }
    else if (document.getElementById("leaveid1").value == "") {
        window.alert("Please Select Leave Type.");
        $("#leaveid1").focus();
        return false;
    }
    else if (document.getElementById("Status").value == "") {
        window.alert("Please Select Status.");
        $("#Status").focus();
        return false;
    }
    else {
        return true;
    }
}

// validation for LeaveHistory in MyLeave for Employee
function empleavehistory() {
    var fromdate = $("#SDate").val();
    var todate = $("#EDate").val();
    var leavetype = $("#LeaveType").val();
    if (fromdate == "") {
        window.alert("Please Select From Date. ");
        $("#SDate").focus();
        return false;
    }
    else if (todate == "") {
        window.alert("Please Select To Date.");
        $("#EDate").focus();
        return false;
    }
    else if (document.getElementById("LeaveType").value == "") {
        window.alert("Please Select Leave Type.");
        $("#LeaveType").focus();
        return false;
    }
    else {
        return true;
    }
}
//In compare My Leaves Starting and End Date
function applyleavecompare() {

    var startdate = document.getElementById("sdate").value;
    var enddate = document.getElementById("edate").value;

    if ((Date.parse(enddate)) < (Date.parse(startdate))) {
        alert('Leave Starting Date Should be More than Leave Ending Date.');
        document.getElementById("edate").focus();
        document.getElementById("edate").value = '';
        return false;
    }
    else
        return true;
}
//Credit in Masters
function Credit() {
    // debugger;
    var branch = $("#branch").val();
    var department = $("#depart").val();
    var designation = $("#desig").val();
    var employee = $("#mag").val();
    var leavetype = $("#type").val();
    var noofcredit = $("#creditday").val();
    var txtEmpId = $("#txtEmpId").val();
    var noofdebit = $("#debitday").val();
    var comments = $("#cbcomments").val();
    //if (document.getElementById("head").checked == true && document.getElementById("depart").value ==0) {
    //    //window.alert("Please Select Department");
    //    showGlobalModal("I#Required#Please Select Department.");
    //    $("#depart").focus();
    //    return false;
    //}
    //else if (document.getElementById("bran").checked == true && document.getElementById("branch").value == 0) {
    //   // window.alert("Please Select Branch.");
    //    showGlobalModal("I#Required#Please Select Branch.");
    //    $("#branch").focus();
    //    return false;
    //}
    //else if (document.getElementById("desig").value == 0) {
    //   // window.alert("Please Select Designation.");
    //    showGlobalModal("I#Required#Please Select Designation.");
    //    $("#desig").focus();
    //    return false;
    //}
    //else if (document.getElementById("mag").value == 0) {
    //   // window.alert("Please Select Employee.");
    //    showGlobalModal("I#Required#Please Select Employee.");
    //    $("#mag").focus();
    //    return false;
    //}
    //else 
    if (document.getElementById("txtEmpId").value == 0) {
        // window.alert("Please Select Employee.");
        showGlobalModal("I#Required#Please Enter Employee ID.");
        $("#txtEmpId").focus();
        return false;
    }
    else
        if (document.getElementById("type").value == 0) {
            // window.alert("Please Select Leave Type.");
            showGlobalModal("I#Required#Please Select Leave Type.");
            $("#type").focus();
            return false;
        }

        else if (document.getElementById("C").checked == true && noofcredit == "") {
            // window.alert("Please Enter Number of Credit LeaveDays.");
            showGlobalModal("I#Required#Please Enter Number of Credit LeaveDays.");
            $("#creditday").focus();
            return false;
        }
        else if (document.getElementById("C").checked == true && (noofcredit == "0" || noofcredit == "00")) {
            // window.alert("Please Enter Valid Credit LeaveDays.");
            showGlobalModal("I#Required#Please Enter Valid Credit LeaveDays.");
            $("#creditday").focus();
            $("#creditday").val('');
            return false;
        }
        else if (document.getElementById("D").checked == true && noofdebit == "") {
            //window.alert("Please Enter Number of Debit LeaveDays.");
            showGlobalModal("I#Required#Please Enter Number of Debit LeaveDays.");
            $("#debitday").focus();
            return false;
        }
        else if (document.getElementById("D").checked == true && (noofdebit == "0" || noofdebit == "00")) {
            // window.alert("Please Enter Valid Debit LeaveDays.");
            showGlobalModal("I#Required#Please Enter Valid Debit LeaveDays.");
            $("#debitday").focus();
            $("#debitday").val('');
            return false;
        }
        else if (comments == "") {
            //window.alert("Please Enter Comments.");
            showGlobalModal("I#Required#Please Enter Comments.");
            $("#cbcomments").focus();
            return false;
        }
        else {
            //$('#divLoader').show();
            //return true;
            $('.subleave').attr("disabled", true);
            document.creditdebit.submit();
        }

}
// validation for Report Branch Lists
function reports() {
    var Branch = $("#BranchName").val();
    if (document.getElementById("BranchName").value == "") {
        window.alert("Please Select Branch.");
        $("#BranchName").focus();
        return false;
    }
    else {
        return true;
    }
}
// validation for Branch count
function branchcount() {

    var name = $("#bname").val();
    var sc = $("#Staffcount").val();
    var Category = $("#Category").val();
    var ar = $("#range").val();
    if (name == "") {
        window.alert("Please Select Branch Name.");
        $("#bname").focus();

        $("#Category").val('');
        $("#Staffcount").val('');
        $("#range").val('');

        return false;
    }
    else if (Category == "") {
        window.alert("Please Select Branch Category.");
        $("#Category").focus();
        return false;
    }
    else if (sc == "") {
        window.alert("Please Enter Staff Count.");
        $("#Staffcount").focus();
        return false;
    }
    else if (sc == 0) {
        window.alert("Staff Count Cannot be Zero.Please Enter Valid Staff Count.");
        $("#Staffcount").focus();
        $("#Staffcount").val('');
        return false;
    }
    else if (ar == "") {
        window.alert("Please Enter Amount Range.");
        $("#range").focus();
        return false;
    }
    else if (ar == ".") {

        window.alert("Please Enter Valid Amount Range.");
        $("#range").focus();
        $("#range").val('');
        return false;
    }
    else {
        $('#divLoader').show();
        return true;
    }
}
// validation for Branch count Edit
function branchcountedit() {
    var name1 = $("#b").val();
    var sc1 = $("#Staffcount").val();
    var Category = $("#Category").val();
    var ar1 = $("#amount").val();
    if (document.getElementById("b").value == "") {
        window.alert("Please Select Branch Name.");
        $("#b").focus();
        return false;
    }
    else if (document.getElementById("Category").value == "") {
        window.alert("Please Select Branch Category.");
        $("#Category").focus();
        return false;
    }
    else if (sc1 == "") {
        window.alert("Please Enter Staff Count.");
        $("#Staffcount").focus();
        return false;
    }
    else if (sc1 == 0) {
        window.alert("Staff Count Cannot be Zero.Please Enter Valid Staff Count.");
        $("#Staffcount").focus();
        $("#Staffcount").val('');
        return false;
    }
    else if (ar1 == "") {
        window.alert("Please Enter Amount Range.");
        $("#amount").focus();
        return false;
    }
    else if (ar == ".") {
        window.alert("Please Enter Valid Amount Range.");
        $("#range").focus();
        $("#range").val('');
        return false;
    }
    else {
        return true;
    }
}
//For current Location in transfers
function empidsearch() {

    var DepEmpid = $("#companyName").val();


    if (DepEmpid == "") {

        //  window.alert("Please Enter EmpId.");

        $("#companyName").focus();
        return false;
    }

    else {
        return true;
    }
}
//For WorkLocation In Transfers
function Worklocation() {

    var newdept = $("#dHDept").val();
    //var newbranch = $("#dHbranchCon").val();
    //var startdate = $("#EffectiveFrom").val();
    //var startdatep = $("#EffectiveFromP").val();
    //var startdatet = $("#EffectiveFromT").val();
    //var startdatept = $("#NewDesignationPT").val();
    var enddate = $("#EffectiveTo").val();

    var designation = $("#newdesig").val();
    var transdept = $("#tHbranchCon").val();
    var cate = $("#newcat").val();
    var transbranch = $("#tHDept").val();
    var promotransdesig = $("#ptHDesig").val();
    var DepEmpid = $("#companyName").val();
    var depustart = $("#Depuationfrom").val();
    var promostart = $("#Promotionfrom").val();
    var promoend = $("#increment").val();
    var PTstart = $("#PTdate").val();
    var transferstart = $("#transferdate").val();
    var temp = $("#temp").val();
    var perm = $("#perm").val();
    var promo_val = document.getElementById('Promotionfrom').val;

    if (DepEmpid == "") {

        //  window.alert("Please Enter EmpId.");
        showGlobalModal("I#Required#Please Enter EmpId.");

        $("#companyName").focus();
        return false;
    }
    else if (document.getElementById("Transfer").checked == true && (document.getElementById("temp").checked == false && document.getElementById("perm").checked == false)) {
        // window.alert("Please Select Temporary/Permanent.");
        showGlobalModal("I#Required#Please Select Temporary/Permanent.");
        $("#temp").focus();
        $("#perm").focus();
        return false;

    }
    //else  if (document.getElementById("Deputation").checked == true && document.getElementById("dhead2").checked == true && document.getElementById("dHDept").value == "") {
    //       window.alert("Please Select New Department");
    //       $("#dHDept").focus();
    //       return false;
    //   }
    //   else if (document.getElementById("Deputation").checked == true && document.getElementById("dbran2").checked == true && document.getElementById("dHbranchCon").value == "") {
    //       window.alert("Please Select New Branch.");
    //       $("#dHbranchCon").focus();
    //       return false;
    //   }  
    else if (document.getElementById("Promotion").checked == true && document.getElementById("newcat").value == 0) {
        // window.alert("Please Select New Designation.");
        showGlobalModal("I#Required#Please Select New Category.");
        $("#newcat").focus();
        return false;
    }
    else if (document.getElementById("Promotion").checked == true && document.getElementById("newdesig").value == 0) {
        // window.alert("Please Select New Designation.");
        showGlobalModal("I#Required#Please Select New Designation.");
        $("#newdesig").focus();
        return false;
    }

    else if (document.getElementById("Transfer").checked == true && document.getElementById("thead2").checked == true && document.getElementById("tHDept").value == "") {
        //window.alert("Please Select New Department.");
        showGlobalModal("I#Required#Please Select New Department.");
        $("#tHDept").focus();
        return false;
    }
    else if (document.getElementById("Transfer").checked == true && document.getElementById("tbran2").checked == true && document.getElementById("tHbranchCon").value == "") {
        //window.alert("Please Select New Branch.");
        showGlobalModal("I#Required#Please Select New Branch.");
        $("#tHbranchCon").focus();
        return false;
    }
    else if (document.getElementById("PromotionTransfer").checked == true && document.getElementById("pthead2").checked == true && document.getElementById("ptHDept").value == "") {
        //window.alert("Please Select New Department.");
        showGlobalModal("I#Required#Please Select New Department.");
        $("#ptHDept").focus();
        return false;
    }
    else if (document.getElementById("PromotionTransfer").checked == true && document.getElementById("ptbran2").checked == true && document.getElementById("ptHbranchCon").value == "") {
        //window.alert("Please Select New Branch.");
        showGlobalModal("I#Required#Please Select New Branch.");
        $("#ptHbranchCon").focus();
        return false;
    }
    else if (document.getElementById("PromotionTransfer").checked == true && document.getElementById("ptHDesig").value == 0) {
        //window.alert("Please Select New Designation.");
        showGlobalModal("I#Required#Please Select New Designation.");
        $("#ptHDesig").focus();
        return false;
    }
    //
    else if (document.getElementById("PromotionTransfer").checked == true && document.getElementById("PTdate").value == 0) {
        // window.alert("Please Select Effective From ");
        showGlobalModal("I#Required#Please Select Effective From.");
        $("#PTdate").focus();
        return false;
    }


    else if (document.getElementById("Promotion").checked == 'true' && promo_val == "") {
        // window.alert("Please Select Effective From.");
        showGlobalModal("I#Required#Please Select Effective From.");
        $("#PTdate").focus();
        return false;
    }

    //else if (startdate == "") {
    //    window.alert("Please Select Effective From.");
    //    $("#EffectiveFrom").focus();      
    //    return false;
    //}
    //else if (startdatep == "") {
    //    window.alert("Please Select Effective From1");
    //    $("#EffectiveFromP").focus();       
    //    return false;
    //}
    //else if (startdatet == "") {
    //    window.alert("Please Select Effective From2");
    //    $("#EffectiveFromT").focus();   
    //    return false;
    //}
    //else if (startdatept == "") {
    //    window.alert("Please Select Effective From.");
    //    $("#EffectiveFromPT").focus();
    //    return false;
    //}
    //else if (document.getElementById("Deputation").checked == true &&  depustart == "dd/mm/yyyy"){
    //    window.alert("Please Select Effective From");
    //    $("#Depuationfrom").focus();
    //    return false;

    //}
    else if (document.getElementById("Promotion").checked == true && promostart == " ") {
        //  window.alert("Please Select Effective From");
        showGlobalModal("I#Required#Please Select New promotion Date.");
        $("#Promotionfrom").focus();
        return false;

    }
    else if (document.getElementById("Promotion").checked == true && promoend == " ") {
        //  window.alert("Please Select Effective From");
        showGlobalModal("I#Required#Please Select New Increment Date .");
        $("#increment").focus();
        return false;

    }
    else if (document.getElementById("PromotionTransfer").checked == true && PTstart == "") {
        //window.alert("Please Select Effective From");
        showGlobalModal("I#Required#Please Select Effective From.");
        $("#PTdate").focus();
        return false;

    }
    else if (document.getElementById("Transfer").checked == true && transferstart == "") {
        // window.alert("Please Select Effective From.");
        showGlobalModal("I#Required#Please Select Effective From.");
        $("#transferdate").focus();
        return false;

    }


    else {
        $('#divLoader').show();
        return true;
    }
}
//OD History validation
function ODhistoryadmin() {
    var fromdate = $("#Stdate1").val();
    var todate = $("#Endate1").val();
    if (fromdate == "") {
        window.alert("Please Select From Date. ");
        $("#Stdate1").focus();
        return false;
    }
    else if (todate == "") {
        window.alert("Please Select To Date.");
        $("#Endate1").focus();
        return false;
    }
    else {
        return true;
    }
}
//OD Masters
function ODMasters() {
    var type = $("#ODType").val();

    if (type == "") {
        // window.alert("Please Enter OD Type. ");
        showGlobalModal("I#Required#Please Enter OD Type.");
        $("#ODType").focus();
        return false;
    }
    else {
        $('#divLoader').show();
        return true;
    }
}
//only numbers and decimals
function numericOnly(evt) {
    var keyCodeEntered = (event.which) ? event.which : (window.event.keyCode) ? window.event.keyCode : -1;
    if ((keyCodeEntered >= 48) && (keyCodeEntered <= 57)) {
        return true;
    }
    // '.' decimal point...
    else if (keyCodeEntered == 46) {
        // Allow only 1 decimal point ('.')...
        if ((evt.value) && (evt.value.indexOf('.') >= 0))
            return false;
        else
            return true;
    }
    return false;
}

// validation for Report Category Lists
function creport() {
    var c = $("#Category").val();
    if (document.getElementById("Category").value == 0) {
        window.alert("Please Select Category.");
        $("#Category").focus();
        return false;
    }
    else {
        return true;
    }
}

// validation for Report Cadre Lists
function dreport() {
    var c = $("#name").val();
    if (document.getElementById("name").value == 0) {
        window.alert("Please Select Designations.");
        $("#name").focus();
        return false;
    }
    else {
        return true;
    }
}

// validation for Report Department List
function vreport() {
    var dept = $("#BranchName").val();
    if (document.getElementById("BranchName").value == 0) {
        window.alert("Please Select Department.");
        $("#BranchName").focus();
        return false;
    }
    else {
        return true;
    }
}

// validation for Report DOB List
function dob() {

    var month = $("#Month").val();

    if (document.getElementById("Month").value == "") {
        window.alert("Please Select Month.");
        $("#Month").focus();

        return false;

    }
    else {

        return true;

    }
}

function onlyAlphabets(evt) {
    try {
        if (window.event) {
            var charCode = window.event.keyCode;
        }
        else if (evt) {
            var charCode = evt.which;
        }

        if ((charCode != 62 && charCode != 60))
            return true;
        else
            return false;
    }
    catch (err) {
        alert(err.Description);
    }
}

//Validation for Leave Approval list
function approve() {
    var OfficerName = $("#OfficerName").val();
    var Authority = $("#searchby").val();
    if (Authority == "") {
        window.alert("Please Select Authority. ");
        $("#searchby").focus();
        return false;
    }
    else if (OfficerName == "") {
        window.alert("Please Select Officer Name. ");
        $("#OfficerName").focus();
        return false;
    }
    else
        return true;
}




function profilechange() {
    var empid = $("#companyName").val();
    if (empid == "") {
        window.alert("Please Enter Empid.");
        $("#companyName").focus();
        return false;
    }
    else {
        return true;
    }
}
//Validation for LTC.
function LeaveTravel() {
    var availment = $("#C").val();
    var blockperiod = $("#Bperiod").val();
    var LeaveType = $("#Type").val();
    var visits = $("#Placeofvisit").val();
    var Sdate = $("#sdate").val();
    var Edate = $("#edate").val();
    var transport = $("#Mtrans").val();
    var Subject = $("#subjects").val();
    var traveladvance = $("#TravelAdvances").val();
    var Parentformcheck = $("#Form1CheckBox").val();
    var spouseformcheck = $("#Form2CheckBox").val();
    var spouseworkingavail = $("#EmpWorking").val();
    var spouseaddressavail = $("#EmpAddress").val();
    var spousedateavail = $("#formdatespouse").val();
    var spouseplaceavail = $("#spousePlace").val();
    var relation = $("#Relation__").val();
    var lrname = $("#rname").val();
    var lrage = $("#rage").val();
    var lrocc = $("#roocu").val();
    var lformdate = $("#formdate").val();
    var lplace = $("#Place").val();
    var x = document.getElementsByName("relation[]");
    if (document.getElementById("C").checked == true) {
        if (blockperiod == "") {
            // window.alert("Please Select Block Period.");
            showGlobalModal("I#Required#Please Select Block Period.");
            $("#Bperiod").focus();
            return false;
        }
        else if (LeaveType == "") {
            //window.alert("Please Select Leave Type.");
            showGlobalModal("I#Required#Please Select Leave Type.");
            $("#Type").focus();
            return false;
        }
        else if (document.getElementById("C").checked == true && visits == "") {
            //window.alert("Please Enter Place of Visits.")
            showGlobalModal("I#Required#Please Enter Place of Visits.");
            $("#Placeofvisit").focus();
            return false;
        }
        else if (Sdate == "") {
            // window.alert("Please Enter Start Date.");
            showGlobalModal("I#Required#Please Enter Start Date.");
            $("#sdate").focus();
            return false;
        }
        else if (Edate == "") {
            //window.alert("Please Enter End Date.");
            showGlobalModal("I#Required#Please Enter End Date.");
            $("#edate").focus();
            return false;
        }
        else if (transport == "") {
            // window.alert("Please Enter Mode Of Transport.");
            showGlobalModal("I#Required#Please Enter Mode Of Transport.");
            $("#Mtrans").focus();
            return false;
        }

        else if (Subject == "") {
            // window.alert("Please Enter the Subject.");
            showGlobalModal("I#Required#Please Enter the Subject.");
            $("#subjects").focus();
            return false;
        }
        else if (traveladvance == "") {
            // window.alert("Please Enter Travel Advance.");
            showGlobalModal("I#Required#Please Enter Travel Advance.");
            $("#TravelAdvances").focus();
            return false;
        }
        else if (traveladvance == "0" || traveladvance == "00" || traveladvance == "000" || traveladvance == "0000" || traveladvance == "00000" || traveladvance == "000000" || traveladvance == "0000000" || traveladvance == "00000000" || traveladvance == "000000000" || traveladvance == "0000000000") {
            // window.alert("Please Enter Valid Travel Advance.");
            showGlobalModal("I#Required#Please Enter Valid Travel Advance.");
            $("#TravelAdvances").focus();
            $("#TravelAdvances").val('');
            return false;
        }
        else if (relation == "Select" || relation == "" || relation == null) {
            //window.alert("Please Select Relation.");
            showGlobalModal("I#Required#Please Select Relation.");
            $("#Relation__").focus();
            return false;
        }
        else if (lrname == "" || lrname == null) {
            //window.alert("Please Enter Name.");
            showGlobalModal("I#Required#Please Enter Name.");
            $("#rname").focus();
            return false;
        }

        else if (lrage == "" || lrage == null) {
            // window.alert("Please Enter Age.");
            showGlobalModal("I#Required#Please Enter Age.");
            $("#rage").focus();
            return false;
        }
        else if (lrocc == "" || lrocc == null) {
            // window.alert("Please Enter Occupation.");
            showGlobalModal("I#Required#Please Enter Occupation.");
            $("#roocu").focus();
            return false;
        }

        //else if (relation == "Spouse" && spouseworking == "") {

        //    window.alert("Please Enter Occupation");
        //    $("#EmpWorking").focus();
        //    return false;
        //}
        //else if (relation == "Spouse" && spouseaddress == "") {
        //    window.alert("Please Enter Address");
        //    $("#EmpAddress").focus();
        //    return false;
        //}
        else if (relation == "Spouse" && document.getElementById("Form2CheckBox").checked == false) {
            //window.alert("Please Check checkbox");
            showGlobalModal("I#Required#Please Check checkbox.");
            $("#Form2CheckBox").focus();
            return false;
        }
        else if (relation == "Spouse" && spousedateavail == "") {

            // window.alert("Please Enter Date");
            showGlobalModal("I#Required#Please Enter Date.");
            $("#formdatespouse").focus();
            return false;
        }
        else if (relation == "Spouse" && spouseplaceavail == "") {

            // window.alert("Please Enter Place");
            showGlobalModal("I#Required#Please Enter Place.");
            $("#spousePlace").focus();
            return false;
        }
        else if ((relation == "Mother" || relation == "Father" || relation == "Children") && document.getElementById("Form1CheckBox").checked == false) {
            //window.alert("Please Check checkbox");
            showGlobalModal("I#Required#Please Check checkbox.");
            $("#Form1CheckBox").focus();
            return false;
        }
        else if ((relation == "Mother" || relation == "Father" || relation == "Children") && lformdate == "") {
            //window.alert("Please Enter Date");
            showGlobalModal("I#Required#Please Enter Date.");
            $("#formdate").focus();
            return false;
        }
        else if ((relation == "Mother" || relation == "Father" || relation == "Children") && lplace == "") {
            //window.alert("Please Enter Place");
            showGlobalModal("I#Required#Please Enter Place.");
            $("#Place").focus();
            return false;
        }
        var i;
        for (i = 0; i < x.length - 1; i++) {
            if (x[i].value == '' || x[i].value == null) {
                //alert("please Select relation");
                showGlobalModal("I#Required#please Select relation.");
                return false;
            }
        }
        var y = document.getElementsByName("name[]");
        for (i = 0; i < y.length - 1; i++) {
            if (y[i].value == '' || y[i].value == null) {
                // alert("please enter Name");  
                showGlobalModal("I#Required#please enter Name.");
                return false;
            }
        }
        var z = document.getElementsByName("RelationAge[]");
        for (i = 0; i < z.length - 1; i++) {
            if (z[i].value == '' || z[i].value == null) {
                //alert("please enter Age");
                showGlobalModal("I#Required#please enter Age.");
                return false;
            }
        }
        var p = document.getElementsByName("Occupation[]");
        for (i = 0; i < p.length - 1; i++) {
            if (p[i].value == '' || p[i].value == null) {
                //alert("please enter Occupation");
                showGlobalModal("I#Required#please enter Occupation.");
                return false;
            }
        }
        for (i = 0; i < x.length; i++) {
            if ((x[i].value == "Mother" || x[i].value == "Father" || x[i].value == "Children") && document.getElementById("Form1CheckBox").checked == false) {

                //window.alert("Please Check checkbox");
                showGlobalModal("I#Required#Please Check checkbox.");
                $("#Form1CheckBox").focus();
                return false;
            }
            else if ((x[i].value == "Mother" || x[i].value == "Father" || x[i].value == "Children") && lformdate == "") {

                // window.alert("Please Enter Date");
                showGlobalModal("I#Required#Please Enter Date.");
                $("#formdate").focus();
                return false;
            }
            else if ((x[i].value == "Mother" || x[i].value == "Father" || x[i].value == "Children") && lplace == "") {

                // window.alert("Please Enter Place");
                showGlobalModal("I#Required#Please Enter Place.");
                $("#Place").focus();
                return false;
            }

            else if (x[i].value == "Spouse" && spouseworkingavail == "") {

                // window.alert("Please Enter Office Name");
                showGlobalModal("I#Required#Please Enter Place.");
                $("#EmpWorking").focus();
                return false;
            }
            else if (x[i].value == "Spouse" && spouseaddressavail == "") {

                // window.alert("Please Enter Office Address");
                showGlobalModal("I#Required#Please Enter Office Address.");
                $("#EmpAddress").focus();
                return false;
            }
            else if (x[i].value == "Spouse" && document.getElementById("Form2CheckBox").checked == false) {

                //window.alert("Please Check checkbox");
                showGlobalModal("I#Required#Please Check checkbox.");
                $("#Form2CheckBox").focus();
                return false;
            }
            else if (x[i].value == "Spouse" && spousedateavail == "") {

                // window.alert("Please Enter Date");
                showGlobalModal("I#Required#Please Enter Date.");
                $("#formdatespouse").focus();
                return false;
            }
            else if (x[i].value == "Spouse" && spouseplaceavail == "") {

                //window.alert("Please Enter Place");
                showGlobalModal("I#Required#Please Enter Place.");
                $("#spousePlace").focus();
                return false;
            }
        }

    }
    //    else {
    //        $('#divLoader').show();
    //        return true;
    //    } 
    else if (document.getElementById("D").checked == true) {
        if (blockperiod == "") {
            //window.alert("Please Select Block Period.");
            showGlobalModal("I#Required#Please Select Block Period.");
            $("#Bperiod").focus();
            return false;
        }
    }
    else {
        return true;
    }
}
function CovidForm() {
    debugger;
    var x = document.getElementsByName("relation[]");

    var i;
    var y = document.getElementsByName("name[]");
    for (i = 0; i < y.length; i++) {
        if (y[i].value == '' || y[i].value == null) {
            showGlobalModal("I#Required#Please enter Name.");
            return false;
        }
    }

    var p = document.getElementsByName("Gender[]");
    for (i = 0; i < p.length; i++) {
        if (p[i].value == '' || p[i].value == null) {
            //alert("please enter Occupation");
            showGlobalModal("I#Required#Please select Gender.");
            return false;
        }
    }
    for (i = 0; i < x.length; i++) {
        if (x[i].value == '' || x[i].value == null) {
            showGlobalModal("I#Required#Please Select relation.");
            return false;
        }
    }

    var z = document.getElementsByName("RelationAge[]");
    for (i = 0; i < z.length; i++) {
        if (z[i].value == '' || z[i].value == null) {
            //alert("please enter Age");
            showGlobalModal("I#Required#Please enter Age.");
            return false;
        }
    }
    var p = document.getElementsByName("address[]");
    for (i = 0; i < p.length; i++) {
        if (p[i].value == '' || p[i].value == null) {
            //alert("please enter Occupation");
            showGlobalModal("I#Required#Please enter address.");
            return false;
        }
    }
    var p = document.getElementsByName("diabetes[]");
    for (i = 0; i < p.length; i++) {
        if (p[i].value == '' || p[i].value == null) {
            //alert("please enter Occupation");
            showGlobalModal("I#Required#Please select Diabetic status.");
            return false;
        }
    }
    var p = document.getElementsByName("hbp[]");
    for (i = 0; i < p.length; i++) {
        if (p[i].value == '' || p[i].value == null) {
            //alert("please enter Occupation");
            showGlobalModal("I#Required#Please select Hypertension status.");
            return false;
        }
    }
    return true;
}
function checkEmail(str) {

    var re = /^(([^<>()[\]\\.,;:\s@\"]+(\.[^<>()[\]\\.,;:\s@\"]+)*)|(\".+\"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
    if (!re.test(str))
        alert("Please enter a valid email address");
}

//Validation for profile update.
function ProfileSubmit() {

    var Empid = $("#companyName").val();
    var activeTab = $(".tab-content").find(".active");
    var id = activeTab.attr('id');
    var officialemailid = $("#OffEdetails").val();
    var personalemailid = $("#PerEdetails").val();
    var regexp = /^[a-zA-Z][\w\.-]*[a-zA-Z0-9]@[a-zA-Z0-9][\w\.-]*[a-zA-Z0-9]\.[a-zA-Z][a-zA-Z\.]*[a-zA-Z]$/;//email
    // console.log(id);
    //  alert(id);
    if (Empid == "") {
        //window.alert("Please Enter Employee Id.");
        showGlobalModal("I#Required#Please Enter Employee Id.");
        $("#companyName").focus();
        return false;
    }
    if (id == "cht1") {
        if ($("#PermAdddetails").val() == "" && $("#PresAdddetails").val() == "") {
            //alert("Please Enter New Details");
            showGlobalModal("I#Required#Please Enter New Details.");
            return false;
        }
    }
    if (id == "premanentval") {
        if ($("#MobileNdetails").val() == "" && $("#HomeNdetails").val() == "" && $("#EmerNdetails").val() == "" && $("#OffEdetails").val() == "" && $("#PerEdetails").val() == "" && $("#ProfGdetails").val() == "" && $("#Pgraddetails").val() == "" && $("#Graddetails").val() == "") {
            //alert("Please Enter New Details");
            showGlobalModal("I#Required#Please Enter New Details.");
            return false;
        }
        else if (officialemailid != "" && !officialemailid.match(regexp)) {
            //window.alert("Please Enter Valid Email Address."); 
            showGlobalModal("I#Required#Please Enter Valid Email Address.");
            $("#OffEdetails").focus();
            return false;
        }
        else if (personalemailid != "" && !personalemailid.match(regexp)) {
            //window.alert("Please Enter Valid Email Address.");
            showGlobalModal("I#Required#Please Enter Valid Email Address.");
            $("#PerEdetails").focus();
            return false;
        }
    }
    else {
        $('#divLoader').show();
        return true;
    }
}

//Validation for WorkDairy
function wd() {
    var workdate = $("#workdate").val();
    var name = $("#name").val();
    var description = $("#subjects").val();
    if (workdate == "") {
        window.alert("Please Enter Work Date.");
        $("#workdate").focus();
        return false;
    }
    else if (name == "") {
        window.alert("Please Enter Work Name.");
        $("#name").focus();
        return false;
    }
    else if (description == "") {
        window.alert("Please Enter Work Description.");
        $("#subjects").focus();
        return false;
    }
}
function WDsubmit() {
    var date = $("#workdate").val();
    var x = document.getElementsByName("workname[]");
    var y = document.getElementsByName("workdes[]");
    var i;

    if (date == "") {
        // window.alert("Please Select WorkDate from DatePicker");
        showGlobalModal("I#Required#Please Select WorkDate from DatePicker.");
        $("#workdate").focus();
        return false;
    }
    else {
        for (i = 0; i < x.length; i++) {
            if (x[i].value == "Please Select") {
                //alert("Please Select Work Name");
                showGlobalModal("I#Required#Please Select Work Name.");
                return false;
            }
        }

        for (i = 0; i < y.length; i++) {

            if (y[i].value == '' || y[i].value == null) {
                //alert("Please Enter Work Description");
                showGlobalModal("I#Required#Please Enter Work Description.");
                return false;
            }
        }

    }
    $('#divLoader').show();
}
function approvalsearch() {
    var workdate = $("#Wdate").val();
    var empid = $("#EmpId").val();
    if (empid == "") {
        window.alert("Please Enter EmpId");
        $("#EmpId").focus();
        return false;
    }
    else if (workdate == "") {
        window.alert("Please Select WorkDate from DatePicker");
        $("#Wdate").focus();
        return false;
    }
    else {
        return true;
    }

}
//function WDsubmit() {
//    var date = $("#workdate").val();
//    var x = document.getElementsByName("workname[]");
//    var y = document.getElementsByName("workdes[]");
//    var othertextbox = document.getElementsByName("others[]");
//    var i;

//    if (date == "") {
//        window.alert("Please Enter Work Date");
//        $("#workdate").focus();
//        return false;
//    }
//    else
//    {
//        for (i = 0; i < x.length; i++) {
//            if (x[i].value == "Please Select") {
//                alert("Please Enter Work Name");
//                return false;
//            }       
//            }
//        for (i = 0; i < othertextbox.length; i++) {
//            if (x[i].value == "Others" && othertextbox[i].value == "" ) {
//                alert("Please Enter Other Work Name");
//                return false;
//            }  
//        }
//        for (i = 0; i < y.length; i++) {

//            if (y[i].value == '' || y[i].value == null) {
//                alert("Please Enter Work Description");
//                return false;
//            }
//        }              
//    }

//    $('#divLoader').show();
//}
function myleaveapp() {

    var employeeid = $("#EmpId").val();
    var status = $("#searchby").val();
    //if (employeeid == "") {
    //    showGlobalModal("I#Required#Please Enter Employee Id.");
    //    //window.alert("Please Enter Employee Id.");
    //    $("#EmpId").focus();
    //    return false;
    //}
    // else  
    if (status == "") {
        //window.alert("Please Select Status.");
        showGlobalModal("I#Required#Please Select Status.");
        $("#searchby").focus();
        return false;
    }
}

function SearchLeaveBal() {
    var EmpId = $("#EmployeeCode").val();
    var FN = $("#FirstName").val();
    var LN = $("#LastName").val();
    var Branch = $("#Branch").val();
    var Department = $("#Department").val();

    if (EmpId == "" && FN == "" && LN == "" && Branch == "" && Department == "") {
        window.alert("Please fill data,.");
        $("#EmployeeCode").focus();
        return false;
    }
}
function Sanctionapprovalsearch() {
    var workdate = $("#Wdate").val();
    var empid = $("#EmpId").val();
    var todate = $("#tdate").val();
    //if (empid == "") {
    //    window.alert("Please Enter EmpId");
    //    $("#EmpId").focus();
    //    return false;
    //}
    //else

    if (workdate == "") {
        window.alert("Please Select FromDate");
        $("#Wdate").focus();
        return false;
    }
    else if (todate == "") {
        window.alert("Please Select ToDate");
        $("#tdate").focus();
        return false;
    }
    else {
        return true;
    }



}

function timesheetapply() {

    var empcode = $("#empcode").val();
    var entrytime = $("#entime").val();
    var exittime = $("#extime").val();
    var requestdate = $("#edates").val();
    var requestdesc = $("#rdes").val();
    if (empcode == "") {
        showGlobalModal("I#Required#Please enter Empcode.");
        document.getElementById("empcode").focus();
        return false;
    }
    else if (requestdate == "" || requestdate == undefined) {
        //window.alert("Please Enter Request Date.");
        showGlobalModal("I#Required#Please Enter Request Date.");
        $("#edates").focus();
        return false;
    }
    else if (entrytime == "") {
        showGlobalModal("I#Required#Please select Entry Time.");

        document.getElementById("entime").focus();
        return false;
    }
    else if (exittime == "") {
        showGlobalModal("I#Required#Please select Exit Time.");
        document.getElementById("extime").focus();
        return false;
    }
    else if (document.getElementById("Type").value == "") {
        // window.alert("Please Select Reason Type.");
        showGlobalModal("I#Required#Please Select Reason Type.");
        document.getElementById("Type").focus();
        return false;
    }

    //else if (requestdesc == "") {
    //    //window.alert("Please Enter Request Description.");
    //    showGlobalModal("I#Required#Please Enter Request Description.");
    //    $("#rdes").focus();
    //    return false;
    // }  
}

function timesheetapply1() {

    var empcode = $("#empcode").val();
    var entrytime = $("#entime").val();
    var exittime = $("#extime").val();
    var requestdate = $("#edate").val();
    var requestdesc = $("#rdes").val();
    if (empcode == "") {
        showGlobalModal("I#Required#Please enter Empcode.");
        document.getElementById("empcode").focus();
        return false;
    }
    else if (requestdate == "" || requestdate == undefined) {
        //window.alert("Please Enter Request Date.");
        showGlobalModal("I#Required#Please Enter Request Date.");
        $("#edate").focus();
        return false;
    }
    else if (entrytime == "") {
        showGlobalModal("I#Required#Please select Entry Time.");

        document.getElementById("entime").focus();
        return false;
    }
    else if (exittime == "") {
        showGlobalModal("I#Required#Please select Exit Time.");
        document.getElementById("extime").focus();
        return false;
    }
    else if (document.getElementById("Type").value == "") {
        // window.alert("Please Select Reason Type.");
        showGlobalModal("I#Required#Please Select Reason Type.");
        document.getElementById("Type").focus();
        return false;
    }

    //else if (requestdesc == "") {
    //    //window.alert("Please Enter Request Description.");
    //    showGlobalModal("I#Required#Please Enter Request Description.");
    //    $("#rdes").focus();
    //    return false;
    // }  
}

function memocreate() {
    var empcode = $("#empcode").val();
    var Empname = $("#shortname").val();
    var Noofdays = $("#Noofdays").val();
    var ClarificationDueDate = $("#cDuedate").val();
    //var memotype = $("#cars").val();
    //var memotype = $("#cars").val();
    var memotype = $("#cars").val();
    //$("memotype").val() = memotype;
    var Memodetails = $("#Memodetails").val();
    if (empcode == "") {
        showGlobalModal("I#Required#Please enter Empcode.");
        document.getElementById("empcode").focus();
        return false;
    }
    if (Empname == "") {
        showGlobalModal("I#Required#Please Search EmpCode.");
        document.getElementById("empcode").focus();
        return false;
    }
    else if (Noofdays == "" || Noofdays == undefined) {
        //window.alert("Please Enter Request Date.");
        showGlobalModal("I#Required#Please Enter Noofdays.");
        $("#Noofdays").focus();
        return false;
    }
    else if (ClarificationDueDate == "" || ClarificationDueDate == undefined) {
        //window.alert("Please Enter Request Date.");
        showGlobalModal("I#Required#Please Enter Clarification Due Date.");
        $("#cDuedate").focus();
        return false;
    }
    else if (memotype == "" || memotype == "Select") {
        //window.alert("Please Enter Request Date.");
        showGlobalModal("I#Required#Please select Memo type.");
        $("#cars").focus();
        return false;
    }

    else if (Memodetails == "" || Memodetails == undefined) {
        //window.alert("Please Enter Request Date.");
        showGlobalModal("I#Required#Please Enter Memodetails.");
        $("#Memodetails").focus();
        return false;
    }
    return true;

}

//function memoupdate() {
//    var memotype = $("#cars").val();
//    if (memotype == "" || memotype == "Select") {
//        //window.alert("Please Enter Request Date.");
//        showGlobalModal("I#Required#Please select Memo type.");
//        $("#cars").focus();
//        return false;
//    }

    
//    return true;
//}

function isNumbercheck(evt) {
    evt = (evt) ? evt : window.event;
    var charCode = (evt.which) ? evt.which : evt.keyCode;
    if (charCode > 31 && (charCode < 48 || charCode > 57) && charCode != 45 && charCode != 43) {
        alert("Only Numbers Allowed");
        return false;
    }
    return true;
}
//start dvLoading
$.fn.SetOverlayHeightWidth = function () {
    $(this).height($(document).height());
    $(this).width($(document).width());
};
$('.navbar-nav li').click(function () {
    $(".overlayforaddNote").show().SetOverlayHeightWidth();
    $('#dvLoading').fadeIn(500);
    $("#dvLoading").show();
});
$('.nav-tabs li').click(function () {
    $(".overlayforaddNote").show().SetOverlayHeightWidth();
    $('#dvLoading').fadeIn(500);
    $("#dvLoading").show();
    setTimeout(function () {
        ajaxSpinnerHide();
    }, 2000);
});

$('.report-list li').click(function () {
    $(".overlayforaddNote").show().SetOverlayHeightWidth();
    $('#dvLoading').fadeIn(500);
    $("#dvLoading").show();
});
$('#button1id').click(function () {
    $(".overlayforaddNote").show().SetOverlayHeightWidth();
    $('#dvLoading').fadeIn(500);
    $("#dvLoading").show();
    setTimeout(function () {
        $(".overlayforaddNote").hide();
        $("#dvLoading").hide();
    }, 3000);
});
$("form").submit(function () {
    $(".overlayforaddNote").show().SetOverlayHeightWidth();
    $('#dvLoading').fadeIn(500);
    $("#dvLoading").show();
});
function pageSpinnerHide() {
    $(".overlayforaddNote").show().SetOverlayHeightWidth();
    $("#dvLoading").show();
    $('#dvLoading').fadeOut(500);
    setTimeout(function () { $(".overlayforaddNote").hide() }, 500);
}
function ajaxSpinnerShow() {
    $(".overlayforaddNote").show().SetOverlayHeightWidth();
    $('#dvLoading').fadeIn(500);
    $("#dvLoading").show();
}
function ajaxSpinnerHide() {
    $('#dvLoading').fadeOut(500);
    setTimeout(function () {
        $(".overlayforaddNote").hide();
        $("#dvLoading").hide();
    }, 500);

}

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
    $(".modal-title").text(arr[1]);
    $(".modal-body").text(arr[2]);
    $('#gHrmModal').modal('show');

}


// add new code 

function TimesheetHistory() {
    var fromdate = $("#SDate").val();
    var todate = $("#EDate").val();

    if (fromdate == "") {
        // window.alert("Please Select From Date. ");
        showGlobalModal("I#Required#Please Select From Date.");
        $("#SDate").focus();
        return false;
    }
    else if (todate == "") {

        // window.alert("Please Select To Date.");
        showGlobalModal("I#Required#Please Select To Date.");
        $("#EDate").focus();
        return false;
    }

    else {

        return true;

    }

}



function TimesheetApproval() {
    var fromdate = $("#SDate").val();
    var todate = $("#EDate").val();

    if (fromdate == "") {
        // window.alert("Please Select From Date. ");
        showGlobalModal("I#Required#Please Select From Date.");
        $("#SDate").focus();
        return false;
    }
    else if (todate == "") {

        // window.alert("Please Select To Date.");
        showGlobalModal("I#Required#Please Select To Date.");
        $("#EDate").focus();
        return false;
    }

    else {

        return true;

    }

}

document.addEventListener("DOMContentLoaded", pageSpinnerHide);
