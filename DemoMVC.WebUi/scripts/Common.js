var ACTIVE_REQ_CNT = 0;
var kendoGridRowNumber = 0;

function fnShowWaitImage() {
    //document.getElementById('loaderContainer').style.display = 'block';
    $(".page-loader").css("display", "block");
}

function fnHideWaitImage() {
    //document.getElementById('loaderContainer').style.display = 'none';
    $(".page-loader").css("display", "hide");
}
function fnCallAjaxHttpGetEvent(url, param, isAsync, showLoader, successCallback) {
    var args = Array.prototype.slice.call(arguments).slice(5);
    return $.ajax({
        async: isAsync,
        type: "GET",
        contentType: "application/json",
        url: url,
        cache: false,
        data: param,
        beforeSend: function () {
            if (showLoader) {
                ACTIVE_REQ_CNT++;
                fnShowWaitImage();
            }
        },
        success: function (data, textStatus, jqXHR) {
            var callbackArgs = [];
            callbackArgs.push(data)
            callbackArgs = callbackArgs.concat(args);
            try {
                successCallback.apply(this, callbackArgs);
            } catch (ex) {
                console.error(ex);
            }
        },
        failure: function (response) {            
            console.error(response);
        },
        complete: function () {            
            if (showLoader) {
                ACTIVE_REQ_CNT--;
                ACTIVE_REQ_CNT === 0 && fnHideWaitImage();
            }
        }
    });
}

function fnCallAjaxHttpPostEvent(url, postData, isAsync, showLoader, successCallback) {
    var args = Array.prototype.slice.call(arguments).slice(5);
    return $.ajax({
        async: isAsync,
        type: "POST",
        contentType: "application/json",
        url: url,
        cache: false,
        data: JSON.stringify(postData),
        beforeSend: function (jqXHR) {
            if (showLoader) {
                ACTIVE_REQ_CNT++;
                fnShowWaitImage();
            }
        },
        success: function (data, textStatus, jqXHR) {
            var callbackArgs = [];
            callbackArgs.push(data)
            callbackArgs = callbackArgs.concat(args);
            try {
                successCallback.apply(this, callbackArgs);
            } catch (ex) {
                console.error(ex);
            }
        },
        failure: function (response) {
            console.error(response);
        },
        complete: function () {
            if (showLoader) {
                ACTIVE_REQ_CNT--;
                ACTIVE_REQ_CNT === 0 && fnHideWaitImage();
            }
        }
    });
}

function fnNumericFilter(args) {
    args.element.kendoNumericTextBox({ format: "#", decimals: 0, spinners: false });
}

function fnColumnMenuInit(e) {
    var menu = e.container.find(".k-menu").data("kendoMenu");
    menu.element.find(".k-filter-item").hide();
    menu.element.find(".k-sort-desc").hide();
    menu.element.find(".k-sort-asc").hide();
}

function fnGetGridStateSuccessEvent(data, grid) {
    try {
        var origionalGridSettings = grid.getOptions();
        if (data && data.gridState != null && data.gridState.GridSettings) {
            var gridStateOptions = JSON.parse(data.gridState.GridSettings);
            var origionalColumns = Object.assign([], origionalGridSettings.columns);
            var savedColumns = Object.assign([], gridStateOptions.columns);
            var newColumns = [];
            var foundColumnsFiled = [];
            if (savedColumns && savedColumns.length) {
                savedColumns.forEach(function (column) {
                    var indexInArray = origionalColumns.findIndex(function (x) { return x.field === column.field });
                    var origionalColumn = origionalColumns.find(function (x) { return x.field === column.field });
                    if (indexInArray !== -1) {
                        foundColumnsFiled.push(column.field);
                        origionalColumn.width = column.width;
                        origionalColumn.hidden = column.hidden;
                        if (typeof column.locked !== 'undefined') {
                            origionalColumn.locked = column.locked;
                        }
                        newColumns.push(origionalColumn);
                        origionalColumns.splice(indexInArray, 1);
                    }
                });
            }

            origionalColumns = Object.assign([], origionalGridSettings.columns);
            var notFoundColumnsInDb = origionalColumns.filter(function (x) { return foundColumnsFiled.indexOf(x.field) === -1 });
            if (notFoundColumnsInDb && notFoundColumnsInDb.length) {
                notFoundColumnsInDb.forEach(function (column) {
                    var indexInArray = origionalColumns.findIndex(function (x) { return x.field === column.field });
                    var origionalColumn = origionalColumns.find(function (x) { return x.field === column.field });
                    if (indexInArray !== -1) {
                        newColumns.splice(indexInArray, 0, origionalColumn);
                        origionalColumns.splice(indexInArray, 1);
                    }
                });
            }

            origionalGridSettings.columns = newColumns;

            var savedDataSource = Object.assign([], gridStateOptions.dataSource);
            //origionalGridSettings.dataSource.page = savedDataSource.page;           PageNo not require save in state (Jaimik Patel - 11 Aug 2018)
            origionalGridSettings.dataSource.pageSize = savedDataSource.pageSize;

            grid.setOptions(origionalGridSettings);
            $('.clearFiltersDiv').show();
        }

    } catch (e) {
    }
    grid.dataSource.read();
}

function getNextKendoGridCounter() {
    return ++kendoGridRowNumber;
}

function fnKendoGridCommonDataBinding() {
    kendoGridRowNumber = (this.dataSource.page() - 1) * this.dataSource.pageSize();
}

function fnSetActiveMenu() {     
    var currentFormAccessCode = $('#hdnFormAccessCode').val();
    var activeLink = $('.head_menu_list li[data-facode="' + currentFormAccessCode + '"]');
    activeLink.addClass('active');
    var parentLink = activeLink.parent().closest('li');
    if (parentLink.length) {
        parentLink.addClass('active');
    }
}

//function resizeFixed() {
//    var grid = $('#' + $('#hdnKendoGridId').val()).data('kendoGrid');
//    if (grid) {
//        var wrapper = grid.wrapper;
//        var header = wrapper.find(".k-grid-header");
//        var paddingRight = parseInt(header.css("padding-right"));
//        header.css("width", wrapper.width() - paddingRight);
//    }
//}

//function scrollFixed() {
//    var grid = $('#' + $('#hdnKendoGridId').val()).data('kendoGrid');
//    if (grid) {
//        var wrapper = grid.wrapper;
//        var header = wrapper.find(".k-grid-header");
//        var offset = $(this).scrollTop();
//        var tableOffsetTop = wrapper.offset().top;
//        var tableOffsetBottom = tableOffsetTop + wrapper.height() - header.height();
//        if (offset < tableOffsetTop || offset > tableOffsetBottom) {
//            header.removeClass("fixed-header");
//        } else if (offset >= tableOffsetTop && offset <= tableOffsetBottom && !header.hasClass("fixed")) {
//            header.addClass("fixed-header");
//        }
//    }
//}

function fnKendoGridDataBoundEvent() {
    $('#gridPageLoader').remove();
    $(window).trigger('resize');
    try {
        this._adjustLockedHorizontalScrollBar();
    } catch (e) { }
    //$('.page-loader').remove();
}

function fnRestrictNegativeNumberTemp(evt) {
    var inputKeyCode = (evt.which) ? evt.which : evt.keyCode
    // Allow: backspace, delete, tab, escape, enter and .
    if ($.inArray(inputKeyCode, [46, 8, 9, 27, 13, 110, 190]) !== -1 ||
        // Allow: Ctrl/cmd+A
        (inputKeyCode == 65 && (evt.ctrlKey === true || evt.metaKey === true)) ||
        // Allow: Ctrl/cmd+C
        (inputKeyCode == 67 && (evt.ctrlKey === true || evt.metaKey === true)) ||
        // Allow: Ctrl/cmd+X
        (inputKeyCode == 88 && (evt.ctrlKey === true || evt.metaKey === true)) ||
        // Allow: home, end, left, right
        (inputKeyCode >= 35 && inputKeyCode <= 39)) {
        // let it happen, don't do anything
        return;
    }

    // Ensure that it is a number and stop the keypress
    if ((evt.shiftKey || (inputKeyCode < 48 || inputKeyCode > 57)) && (inputKeyCode < 96 || inputKeyCode > 105)) {
        evt.preventDefault();
    }
}

function fnRestrictNegativeNumber(evt) {
    var key = evt.which || evt.keyCode;
    var currentCharString = String.fromCharCode(key);
    if (currentCharString == '.')
        return false;
    // Allow: backspace, delete, tab, escape, enter and .
    if ($.inArray(key, [46, 8, 9, 27, 13, 110, 190]) !== -1 ||
        // Allow: Ctrl/cmd+A
        (key == 65 && (evt.ctrlKey === true || evt.metaKey === true)) ||
        // Allow: Ctrl/cmd+C
        (key == 67 && (evt.ctrlKey === true || evt.metaKey === true)) ||
        // Allow: Ctrl/cmd+X
        (key == 88 && (evt.ctrlKey === true || evt.metaKey === true)) ||
        // Allow: home, end, left, right
        (key >= 35 && key <= 39)) {
        // let it happen, don't do anything
        return;
    }
    var currentChar = parseInt(String.fromCharCode(key), 10);
    var selStart = this.selectionStart;
    var selEnd = this.selectionEnd;
    if (selStart > selEnd) {
        var temp = selStart;
        selStart = selEnd;
        selEnd = temp;
    }
    var nextValue;
    if (!isNaN(currentChar)) {
        nextValue = this.value.substring(0, selStart) + currentChar + this.value.substr(selEnd);
    }
    else if (key == 46 && this.value.indexOf(".") == -1) {
        nextValue = this.value.substring(0, selStart) + "." + this.value.substr(selEnd);
    }
    return currentChar !== '.' && (nextValue != undefined && (parseFloat(nextValue, 10) > 0 && (!nextValue.split(".")[1] || nextValue.split(".")[1].length < 3)));
}

function fnRestrictNegativeDecimal(evt) {
    var key = evt.which || evt.keyCode;
    var currentCharString = String.fromCharCode(key);
    if (this.value.indexOf(".") !== -1 && currentCharString == '.')
        return false;
    // Allow: backspace, delete, tab, escape, enter and .
    if ($.inArray(key, [46, 8, 9, 27, 13, 110, 190]) !== -1 ||
        // Allow: Ctrl/cmd+A
        (key == 65 && (evt.ctrlKey === true || evt.metaKey === true)) ||
        // Allow: Ctrl/cmd+C
        (key == 67 && (evt.ctrlKey === true || evt.metaKey === true)) ||
        // Allow: Ctrl/cmd+X
        (key == 88 && (evt.ctrlKey === true || evt.metaKey === true)) ||
        // Allow: home, end, left, right
        (key >= 35 && key <= 39)) {
        // let it happen, don't do anything
        return;
    }
    var currentChar = parseInt(String.fromCharCode(key), 10);
    var selStart = this.selectionStart;
    var selEnd = this.selectionEnd;
    if (selStart > selEnd) {
        var temp = selStart;
        selStart = selEnd;
        selEnd = temp;
    }
    var nextValue;
    if (!isNaN(currentChar)) {
        nextValue = this.value.substring(0, selStart) + currentChar + this.value.substr(selEnd);
    }
    else if (key == 46 && this.value.indexOf(".") == -1) {
        nextValue = this.value.substring(0, selStart) + "." + this.value.substr(selEnd);
    }
    return nextValue != undefined && (parseFloat(nextValue, 10) > 0 && (!nextValue.split(".")[1] || nextValue.split(".")[1].length < 3));
}

function EnableDisableSaveControl(enable) {
    if (!enable) {
        if ($("#btnsave")) {
            $("#btnsave").attr('disabled', 'disabled');
            $("#btnsave").addClass('disabled');
        }
    }
    else {
        if ($("#btnsave")) {
            $("#btnsave").removeAttr('disabled');
            $("#btnsave").removeClass('disabled');
        }
    }
}

//Email Validation Start

function ValidateForm() {    
    var validflag = true;
    var mailTo = $("#MailTo");
    //var mailCC = $("#MailCC");
    var subject = $("#Subject");
    var mailbody = $("#MailBody");

    if ($(mailTo).val() != null && $(mailTo).val() != '' && $(mailTo).val() != undefined) {
        var mailToValue = $(mailTo).val().trim();
    }    
    //var mailCCValue = "";
    //if ($(mailCC).val() != null && $(mailCC).val() != '' && $(mailCC).val() != undefined)
    //{
    //    mailCCValue =  $(mailCC).val().trim();
    //}

    var subjectValue = $(subject).val().trim();
    var mailbodyValue = $(mailbody).val().trim();

    $('.mailto_error').text('');
    $('.mailcc_error').text('');
    $('.subject_error').text('');
    $('.mailbody_error').text('');

    if (mailToValue == '') {
        $('.mailto_error').text('To is required.');
        validflag = false;
    }
    else {
        if (!validateMultipleEmailIds(mailToValue)) {
            $('.mailto_error').text('Invalid email Id.');
            validflag = false;
        }
    }

    //if (mailCCValue != '' && !validateMultipleEmailIds(mailCCValue)) {
    //    $('.mailcc_error').text('Invalid email Id.');
    //    validflag = false;
    //}

    if (subjectValue == '') {
        $('.subject_error').text('Subject is required.');
        validflag = false;
    }
    if (mailbodyValue == '') {
        $('.mailbody_error').text('MailBody is required.');
        validflag = false;
    }

    return validflag;
}

function validateMultipleEmailIds(value) {
    if (value != null && value != '' && value != undefined)
    {
        var emailValue = value.replace(/;/g, ",");
        var result = emailValue.split(",");
        for (var i = 0; i < result.length; i++)
            if (!emailvalidation(result[i]))
                return false;
        return true;
    }
    return true;
}

function emailvalidation(field) {

    var regex = /\b[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,4}\b/i;
    if (field != "")
        return (regex.test(field)) ? true : false;
    else
        return true;
}

//Email Validation End

function MailBodyCustomToolbar() {
    CKEDITOR.replace('MailBody', {
        toolbar: [

            ['Cut', 'Copy', 'Paste', 'PasteText', 'PasteFromWord', '-', 'Undo', 'Redo'],			// Defines toolbar group without name.
            //'/',																					// Line break - next group will be placed in new line.
            { name: 'basicstyles', items: ['Bold', 'Italic'] }
        ]
    });
}

$(document).ajaxStart(function () {
    $(".page-loader").css("display", "block");
});

$(document).ajaxComplete(function () {
    $(".page-loader").css("display", "none");
});

var successElement = $("#alertid");
if (successElement) {
    setTimeout(function () { successElement.hide(); }, 4000);
}
function otpValidation() {
    var otpcheck = $("#OTP").val();
    var isvalid = true;
    if (otpcheck == "" || otpcheck == '' || otpcheck == null) {
        isvalid = false;
        $("[data-valmsg-for='OTP']").html("<span class='field-validation-error'>OTP is required</span>");
    }
    else if (otpcheck.length > 4 || otpcheck.length < 4) {
        isvalid = false;
        $("[data-valmsg-for='OTP']").html("<span class='field-validation-error'>OTP is having 4 digits</span>");
    }
    else {
        $("[data-valmsg-for='OTP']").html('');
    }
    return isvalid;
}
function ConvertToCommaSeparatedValue(num) {
    var n1, n2;
    n1 = num.split('.');
    n2 = n1[1] || null;
    n1 = n1[0].replace(/(\d)(?=(\d\d)+\d$)/g, "$1,");
    num = n2 ? n1 + '.' + n2 : n1;
    return num;
}
function DisplayNumberCommaSeparated(num) {    
    if (num != null) {
        if (num != 0) {
            var number2 = num;
            var test = number2.toLocaleString('en-IN', { maximumFractionDigits: 3 });
            return test;
        }
        else {
            return "";
        }
    }
    else {
        return "";
    }
}
function error_handler(e) {
    if (e.errors) {
        var message = "";
        $.each(e.errors, function (key, value) {
            if ('errors' in value) {
                $.each(value.errors, function () {
                    message += this + "\n";
                });
            }
        });
        $("#CodeExistValidation").show();
        $("#CodeExistValidation").html(message);
        grid.one("dataBinding", function (e) {
            e.preventDefault();
        });
    }
}


function Alphanumeric_Only(n) {
    var t;
    try {
        if (window.event) {
            t = window.event.keyCode;
        }
        else if (n) {
            t = n.which;
        }
        else {
            return !0;
        }
        return (t > 64 && t < 91) || (t > 96 && t < 123) || (t > 47 && t < 58) || t == 8 || t == 47 || t == 92 || n.code == "Tab" || n.code == "ArrowLeft" || n.code == "ArrowRight" || n.code == "Minus" ? !0 : !1;
    } catch (i) {
        alert(i.Description);
    }
}

function AllowNumbersOnly(evt) {
    var charCode = (evt.which) ? evt.which : evt.keyCode;
    if (charCode != 46 && charCode > 31
        && (charCode < 48 || charCode > 57))
        return false;

    return true;
}


function fnGetDateFormat(datePeriod, isFullFormat, isLocalConversionRequired)
{
    //console.log(isLocalConversionRequired); //undefined if not passed as parameter 
    isLocalConversionRequired = isLocalConversionRequired === undefined ? false : isLocalConversionRequired;
    if (isLocalConversionRequired == true) {
        //convert UTC to local time zone of user's browser
        console.log("Is Daylight saving time ? " + moment().isDST());
        //if (datePeriod)
        //    datePeriod = new Date(datePeriod).addHours(get_current_user_UTC_offSet());
        if (moment().isDST())
            datePeriod = new Date(datePeriod.getTime() + (get_current_user_UTC_offSet() * 60 * 60 * 1000));
        else
            datePeriod = new Date(datePeriod.getTime() - datePeriod.getTimezoneOffset() * 60  *1000);
    }
    if (isFullFormat == true)
        return moment(datePeriod).format("MM/DD/YYYY hh:mm A");
    else
        return moment(datePeriod).format("MM/DD/YYYY");
}

function get_current_user_UTC_offSet() {
    //return /\((.*)\)/.exec(new Date().toString())[1];
    var d = new Date();
    var n = d.getTimezoneOffset();
    var offSetInHours = n / -60;
    console.log(offSetInHours);
    return offSetInHours;
}