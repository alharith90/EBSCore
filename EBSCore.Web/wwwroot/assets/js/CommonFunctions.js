// Author : 7arith
$.ajaxSetup({
    beforeSend: function (xhr) {
        xhr.setRequestHeader('__RequestVerificationToken', getHeaders()["__RequestVerificationToken"]);
        //showProgress();
    },
    complete: function () {
        //hideProgress();
        // hide progress spinner
    }
});
jQuery.fn.outerHTML = function (s) {
    return s ?
        this.before(s).remove() :
        jQuery("<p>").append(this.eq(0).clone()).html();
};
function getHeaders() {
    var token1 = $('[name=__RequestVerificationToken]');
    var headers1 = {};
    if (token1) {
        headers1["__RequestVerificationToken"] = token1.val();
        return headers1;
    } else {
        headers1;
    }
}


function ShowFullLoading() {
    $("body").addClass("Loading");
    $(".cLoading").show();
}
function HideFullLoading() {
    $("body").removeClass("Loading");
    $(".cLoading").hide();

};
//End Data Table Functions //


//Initialize DataTable
window.InitializeDataTable = (tblID, dtObject) => {
    try {
        console.log("Initializing DataTable with ID" + tblID);
        console.log(JSON.parse(dtObject));
        var DataObject = JSON.parse(dtObject);
        $('#' + tblID).DataTable({
            columns: DataObject.columns,
            data: JSON.parse(DataObject.data)
        });

    } catch (error) {
        console.error("Error initializing DataTable:", error);
        // You can also send this error back to Blazor if necessary
        throw new Error("Error initializing DataTable: " + error.message);
    }
}
//END Initialize DataTable


// Query String
function getQuerystring(key, default_) {
    if (default_ == null) default_ = "";
    key = key.replace(/[\[]/, "\\\[").replace(/[\]]/, "\\\]");
    var regex = new RegExp("[\\?&]" + key + "=([^&#]*)");
    var qs = regex.exec(window.location.href);
    if (qs == null)
        return default_;
    else
        return qs[1];
}
// Generate drop down

function getEditColumn(editText, CancelText, UpdateText) {
    return {
        width: "13%", headerAttributes: { "class": "NoPrint" },
        command: [{
            name: "edit",
            text: { edit: editText, cancel: CancelText, update: UpdateText }
        }]
    }
}
var AjaxCall = function (url, Type, data, Control) {
    var r = $.Deferred();
    $.ajax({
        url: url,
        dataType: "json",
        type: Type,
        contentType: 'application/json; charset=utf-8',
        async: true,
        processData: false,
        cache: false,
        data: data,
        success: function (returnedData) {
            //alert(returnedData);

            try {
                Control.value(returnedData);
                r.resolve();
            } catch (err) {

                $("#" + Control).val(returnedData);
                r.resolve();

            }
        }
    });
    return r;

}

function getRequiredValidator(NameOfControl, ErrorMsg) {
    return {
        requiredMsg: function (input) {
            if (input.is("[name='" + NameOfControl + "']") && (input.val() == ""
                || input.val() == null
            )) {
                input.attr("data-requiredMsg-msg", ErrorMsg);
                return false;
            }
            return true;
        }

    }
}
function getDeleteButton(kwindow, kwindowTemplate, DeleteText, ds) {
    if (ds == null) {
        ds = dataSource;
    }
    return {
        command: [
            {
                name: "Delete", text: DeleteText, imageClass: "k-font-icon k-i-trash ",
                click: function (e) {
                    //add a click event listener on the delete button
                    e.preventDefault()
                    var tr = $(e.target).closest("tr"); //get the row for deletion
                    var data = this.dataItem(tr); //get the row data so it can be referred later
                    //    alert(data);
                    // if ( != null) {
                    try {
                        resetWindows(kwindow);
                        kwindow.content(kwindowTemplate(data)); //send the row data object to the template and render it
                        kwindow.open().center();

                        $("#yesButton").click(function () {
                            ds.remove(data)  //prepare a "destroy" request 
                            //grid.dataSource.sync()  //actually send the request (might be ommited if the autoSync option is enabled in the dataSource)
                            kwindow.close();
                        })
                        $("#noButton").click(function (e) {

                            kwindow.close();
                        })
                    } catch (err) {
                        ds.remove(data)
                    }
                }
            }
        ], width: "100px", title: DeleteText,
        headerAttributes: { "class": "NoPrint", }
    }
}
function resetWindows(wd) {

    wd.title("");
    wd.wrapper.css({
        width: "400",
        height: "150px"
    });

}
function AjaxPost(varURL, varData, OnSuccess, OnError, headers) {
    if (!headers) {
        var token = $('[name=__RequestVerificationToken]').val();
        var headers = {};
        headers["__RequestVerificationToken"] = token;
    }


    $.ajax({
        url: varURL,
        dataType: "json",
        type: "POST",
        contentType: 'application/json; charset=utf-8',
        data: varData,
        headers: headers,
        async: true,
        processData: false,
        cache: false,
        success: OnSuccess,
        error: OnError
    });
}
function AjaxGet(varURL, varData, OnSuccess, OnError, headers) {
    if (!headers) {
        var token = $('[name=__RequestVerificationToken]').val();
        var headers = {};
        headers["__RequestVerificationToken"] = token;
    }
    $.ajax({
        url: varURL,
        dataType: "json",
        type: "GET",
        contentType: 'application/json; charset=utf-8',
        data: varData,
        headers: headers,
        async: true,
        processData: false,
        cache: false,
        success: OnSuccess,
        error: OnError
    });
}

function AjaxDelete(varURL, varData, OnSuccess, OnError, headers) {
    if (!headers) {
        var token = $('[name=__RequestVerificationToken]').val();
        var headers = {};
        headers["__RequestVerificationToken"] = token;
    }
    $.ajax({
        url: varURL,
        dataType: "json",
        type: "DELETE",
        contentType: 'application/json; charset=utf-8',
        data: varData,
        headers: headers,
        async: true,
        processData: false,
        cache: false,
        success: OnSuccess,
        error: OnError
    });
}
// Events 
$(function () {
    HideFullLoading();

    if (typeof flatpickr === "function") {
        flatpickr(".dtPicker", { enableTime: true, dateFormat: "d/m/Y H:i", });
        flatpickr(".dPicker", { enableTime: false, dateFormat: "d/m/Y", });
    }

    $('[data-toggle="tooltip"]').tooltip();
    $("#bBack").click(function () {
        var cssClass = $("#bBack").attr("class");
        if (cssClass.indexOf("hasPopUp") > -1) {
            try {
                parent.location.href = parent.location.href
            }
            catch (err) {

            }
            try {
            }
            catch (err) {
            }
        } else {
            var varLastURL = document.referrer;
            location.href = varLastURL;
        }
    });

    $('form').find('input[required], select[required], textarea[required]').each(function () {
        var label = $(this).parents(".form-group").find('label');
        if (label.length) {
            label.append('<i class="text-danger fa-solid fa-asterisk"></i>');
        }
    });
});

// Progress 
function showProgress() {
    $("body").append('<div class="AgencyLoading"><div><img src="/Content/Images/imgLoading.svg" /><p>Please Wait</p></div></div>');

}
function hideProgress() {
    $(".AgencyLoading").remove();
}


// To Show Tool Tip for cills in grid 
function addToolTipEvent() {
    var strControlsSelector = "[role='gridcell'] , .k-footer-template > td , .k-group-footer > td"
    $(strControlsSelector).mouseenter(function (e) {
        $(this).css("cursor", "help");
        $("#divToolTip").remove();
        var tdText = $(this).html();
        var strDiv = "<div id='divToolTip' class='customToolTip' style='position: fixed;display:none;'>" + tdText + "</div>"
        $("body").append(strDiv);
        var left = e.clientX + "px";
        var top = e.clientY + "px";
        var div = document.getElementById("divToolTip");
        div.style.left = left;
        div.style.top = top;
        $("#divToolTip").toggle();
    });
    $(strControlsSelector).mouseout(function (e) {
        $("#divToolTip").remove();
    });
}



function DownloadFile(FileData, FileExt) {
    var byteString = atob(FileData.split(",")[1]);
    // Convert that text into a byte array.
    var ab = new ArrayBuffer(byteString.length);
    var ia = new Uint8Array(ab);
    for (var i = 0; i < byteString.length; i++) {
        ia[i] = byteString.charCodeAt(i);
    }

    // Blob for saving.
    var blob = new Blob([ia], { type: FileData.split(",")[0] });

    // Tell the browser to save as report.pdf.
    saveAs(blob, "Attach." + FileExt);
}
function GenerateAcronym(phrase) {
    if (!phrase) return '';

    return phrase
        .split(' ') // Split the phrase into an array of words
        .map(word => word.charAt(0).toUpperCase()) // Get the first character of each word and convert to uppercase
        .join(''); // Join all the first characters to form the acronym
}


function hideLayout() {
    $("#header").hide();
    $("#controls").hide();
    $("#content").css("right", "0");
    $("#pageInfo").css("display", "none");
}
