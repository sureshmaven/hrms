function getColumnNumber(Id, Value) {
   
    return '<input type="number" class = "form-control" name="' + Id + '" value="' + Value + '" onkeydown = "javascript: return event.keyCode == 69 || event.keyCode == 189? false : true"  required pattern="/^-?\d+\.?\d*$/" onKeyPress="if(this.value.length==20) return false;" />';
}
function getColumnNumberloans(Id, Value) {

    return '<input type="number" class = "form-control" name="' + Id + '" value="' + Value + '" onkeydown = "javascript: return event.keyCode == 69 || event.keyCode == 189? false : true"  required pattern="/^-?\d+\.?\d*$/" onKeyPress="if(this.value.length==20) return false;" />';
}
function getpfColumnNumber(Id, Value) {
 
    return '<input type="number" class = "form-control" style="width: 180px;float: left;margin: 0 6px 0 0;" name="' + Id + '" value="' + Value + '" onkeydown = "javascript: return event.keyCode == 69 || event.keyCode == 189? false : true" onkeypress="javascript:return isNumber(event)" required/>';
}
function getColumnNumberCountValue(Id, Value) {
    return '<input type="number" class ="form-control value" name="' + Id + '" value="' + Value + '" onkeyup="calcTotal()" onblur="calcTotal4()" onkeydown = "javascript: return event.keyCode == 69 || event.keyCode == 189? false : true" onkeypress="javascript:return isNumber(event)" required/>';
}

function getColumnTextbox(Id, Value) {
    return '<input type="text" class = "form-control" name="' + Id + '" value="' + Value + '"onkeypress="javascript : return alpahaNumeric(event)" required " />';
}

function getColumnTextboxWithSpecialChars(Id, Value) {
    return '<input type="text" class = "form-control" name="' + Id + '" value="' + Value + '"  />';
}

function getColumnTextArea(Id, Value) {
    return '<textarea class = "form-control" name="' + Id + '" value="' + Value + '" rows="5" cols="2" readonly>' + Value+'</textarea>';
}
function getColumnTextAreaWithoutReadOnly(Id, Value) {
    return '<textarea class = "form-control" name="' + Id + '" value="' + Value + '" rows="5" cols="2" maxlength="1000">' + Value + '</textarea>';
}
function getColumnButton(Id, Value) {

    if (Value == "Add") {
        return '<button class="btn btn-danger" type="button" name="' + Id + '" id="' + Id + '" onclick="deleteEntireRow(this.id)" value="Delete"><i class="glyphicon glyphicon-trash"></i></button>';
    } else {
        return '<button class="btn btn-danger" type="button" name="' + Id + '" id="' + Id + '" onclick="deleteRow(this.id)" value="' + Value + '"><i class="glyphicon glyphicon-trash"></i></button>';
    }
}
function getColumnButtonForHfc(Id, Value) {
    if (Value == "Add") {
        return '<button class="btn btn-danger" type="button" name="' + Id + '" id="' + Id + '" onclick="deleteEntireRowHfc(this.id)" value="Delete"><i class="glyphicon glyphicon-trash"></i></button>';
    } else {
        return '<button class="btn btn-danger" type="button" name="' + Id + '" id="' + Id + '" onclick="deleteRowHfc(this.id)" value="' + Value + '"><i class="glyphicon glyphicon-trash"></i></button>';
    }
}
function getColumnButton1(Id, Value) {
    return '<button class="btn" type="button" name="' + Id + '" id="' + Id + '" onclick="getAnualSalary(this.id)" value=' + Value + '>' + Value +'</button>';
}
function getColumnCheckBox(Id, Value) {
    return '<input type="checkbox" class="selectRow" name="' + Id + '" id="' + Id + '" onclick="checkboxRow(this.id)" value="' + Value + '" /><label for="' + Id + '"><span></span></label>';
}
function getColumnDate(Id, Value) {
    return '<input type=date (yyyy-mm-dd) class = "form-control input-group.date" name="' + Id + '" value="' + Value + '" dateformat="dd-MMM-yyyy"   />';
}

function getColumnDateCheck(Id, Value) {
    return '<input type=date (yyyy-mm-dd) class = "form-control" name="' + Id + '" value="' + Value + '" onchange="fun();" />';
}

function getColumngridcompDate(Id, Value, min) {
    return '<input type=date (yyyy-mm-dd) class = "form-control" name="' + Id + '" min="' + min+'" value="' + Value + '" />';
}

function getColumnNumberMulti(Name, Value) {

    return '<input type="number" name="' + Name + '" value="' + Value + '" required />';
}

function getColumnTextboxMulti(Name, Value) {

    return '<input type="text" name="' + Name + '" value="' + Value + '" required />';
}

function getColumnDateMulti(Name, Value) {
    return '<input type="date" name="' + Name + '" value="' + Value + '" />';
}
function getHiddenField(Id, Value) {
    return '<input type="hidden" class = "form-control" name="' + Id + '" value="' + Value + '" />';
}

function getColumnReadOnlyAL(Id, Value) {
    
    return '<input class = "form-control" name="' + Id + '" value="' + Value + '" readonly/>';

}
function getColumnReadOnlywithWhiteshade(Id, Value) {
    
    return '<input class = "form-control" name="' + Id + '" value="' + Value + '" readonly style="background-color: #fff;color: #1f1d1d !important" />';

}

function getColumnReadOnly(Id, Value) {
    return '<input class = "form-control" name="' + Id + '" value="' + Value + '" readonly/>';

}

function getColumnReadOnlyAllowance(Id, Value) {
    return '<input class = "input_without_brd" name="' + Id + '" value="' + Value + '" readonly/>';

}
function getColumnDateReadOnly(Id, Value) {
    return '<input type=date (yyyy-mm-dd) class = "form-control" name="' + Id + '" value="' + Value + '" readonly/>';
}
//function getColumnTextbox() {
//    return '<input type="text" name="" value="" required />';
//}
function getColumnDropdown(Id, Value, arrData) {
     
    var returndata = '<select class="form-control" size="1" name="' + Id + '">';
    returndata += '<option  value="Select">Select</option>';
    arrData.map(function (x) {
        if (x === Value)
            returndata += '<option value="' + x + '" selected="selected">' + x + '</option >';
        else
            returndata += '<option value="' + x + '">' + x + '</option >';
    });
    returndata += '</select>';

    return returndata;
}

function getColumnDropdownForEmpMaster(Id, Value, arrData) {
   
    var returndata = '<select class="form-control" size="1" name="' + Id + '">';
    arrData.map(function (x) {
        if (x === Value)
            returndata += '<option value="' + x + '" selected="selected">' + x + '</option >';
        else
            returndata += '<option value="' + x + '">' + x + '</option >';
    });
    returndata += '</select>';

    return returndata;
}

function getModifiedDataFromJDT(updateList, orgList) {
    var arresult = [];
    var arrupdate = updateList.split('&');
    arrupdate.map(function (x) {
        var arrkeys = x.split('=');
        orgList.map(function (o) {
            if (o.Id == arrkeys[0] && o.Value != arrkeys[1]) {
                arresult.push(o.row_type + '^' + x + '^' + o.Value);
            }
        });
    });

    var sRet = '';
    if (arresult.length > 0) {
        sRet = arresult.join('~');
    }

    console.log(arresult, sRet);
    return sRet;
}

function getModifiedDataFromGrid(updateList, orgList) {
    var arresult = [];
    var arrList = updateList.split('&');
    arrList.map(function (x) {
        var arrkeys = x.split('=');
        orgList.map(function (o) {
            if (o.Id == arrkeys[0] && o.Value != arrkeys[1]) {
                arresult.push(x);
            }
        });
    });
    return arresult;
}

function getUpdateDataAsObject(arrData) {
    var arrObj = [];
    arrData.forEach(function (element) {
        var objEf = {};
        var strEf = element.split("=");
        objEf.Id = strEf[0];
        objEf.Value = strEf[1];
        arrObj.push(objEf);

    });
    return arrObj;
}

function isNumber(evt) {
    var iKeyCode = (evt.which) ? evt.which : evt.keyCode
    if (iKeyCode != 46 && iKeyCode > 31 && (iKeyCode < 48 || iKeyCode > 57))
        return false;

    return true;
} 
// Text box allws only alphabets numbers and spaces 
function alpahaNumeric(evt) {
    
    var regex = new RegExp("^[a-zA-Z0-9 ]+$");
    var str = String.fromCharCode(!evt.charCode ? evt.which : evt.charCode);
    if (regex.test(str)) {
        return true;
    }
    evt.preventDefault();
    return false;
}


 $('.input-group.date').datepicker({
        format: 'dd-mm-yyyy',
        todayHighlight: true,
        autoclose: true
});

