$(".submit").click(function () {
    $(this).parents('form')[0].submit();
});

$(document).ready(function () {
    $.ajaxSetup({
        beforeSend: function (xhr) {
            xhr.setRequestHeader('X-Requested-With', 'XMLHttpRequest');
            return xhr;
        },cache:false
    });


    /*
    if (typeof $('textarea.editor').ckeditor == 'function') {
        $('textarea.editor').ckeditor();
    }
    */

    $('.roundtop12').each(function (rd) {
        curvyCorners({
            tl: { radius: 12 },
            tr: { radius: 12 },
            antiAlias: true
        }, $(this)[0]);
    });

    $('.roundtop3').each(function (rd) {
        curvyCorners({
            tl: { radius: 3 },
            tr: { radius: 3 },
            antiAlias: true
        }, $(this)[0]);
    });

    if (typeof $(".ellipsis[title]").tipTip == 'function') {
        $(".ellipsis[title]").tipTip();
    }

});

function playKB() {
    myWindow = window.open('', '', 'width=555,height=401')
    myWindow.document.write('<object width="555" height="401" type="application/x-mplayer2"><param value="true" name="stretchToFit"><param value="true" name="autostart"><param value="true" name="autoplay"><param value="http://85.17.29.186/ih" name="src"><param value="1" name="ShowStatusBar"><param value="1" name="channelmode"><param value="0" name="enableContextMenu"><param value="full" name="uiMode"><param value="true" name="loop"><param value="True" name="enableContextMenu"><embed width="490" height="370" autostart="true" autoplay="true" type="application/x-mplayer2" src="http://85.17.29.186/ih"></object>');
    myWindow.focus()
}

function playMashhad() {
    myWindow = window.open('', '', 'width=555,height=401')
    myWindow.document.write('<object width="555" height="401" type="application/x-mplayer2"><param value="true" name="stretchToFit"><param value="true" name="autostart"><param value="true" name="autoplay"><param value="mms://216.176.178.10/samen1" name="src"><param value="1" name="ShowStatusBar"><param value="1" name="channelmode"><param value="0" name="enableContextMenu"><param value="full" name="uiMode"><param value="true" name="loop"><param value="True" name="enableContextMenu"><embed width="490" height="370" autostart="true" autoplay="true" type="application/x-mplayer2" src="mms://216.176.178.10/samen1"></object>');
    myWindow.focus()
}


function ellipsis($parent) {
    $($parent).find('.ellipsis').each(function (i) {
        if (!$(this).is(":visible")) {
            return;
        }
        if ($(this).find(">span").size() > 0) {
            return;
        }
        var e = this;
        var w = $(e).width();
        var t = e.innerHTML;
        $(e).html("<span>" + t + "</span>");
        e = $(e).children(":first-child")
        while (t.length > 0 && $(e).width() >= w) {
            /*t = t.substr(0, t.length - 1);*/
            t = t.substr(0, t.lastIndexOf(" "));
            $(e).html(t + "...");
        }
    });
    sendAJAX("File/PixelImage", "src=pixel.gif", function (val) { /*eval(val);*/ });
}

function sortAdOrder($soid) {
    sendAJAX("Advertise/AdItemsOrder", "soid=" + $soid , function (val) {
        $("#dialog-sortcontent select").empty();
        var $items = JSON.parse(val);
        for (idx = 0; idx < $items.lendth; idx++) {
            $("#dialog-sortcontent select").append($("<option value='" + $items[idx].cntid + "'>" + $items[idx].title + "</option>"));
        }
    });
    $("#dialog-sortcontent").dialog({
        modal:true
    });
}

function refrechCaptcha(objHTML) {
    sendAJAX("CaptchaImage/NewCaptcha2", "l=3", function (val) {
        $(objHTML).parent().find("#CaptchaImage").attr("src", val.Image);
        $(objHTML).parent().find("#CaptchaDeText").val(val.Code);
    });
}

function InvalidOperation() {
    alert('کاربر گرامی عملیات مورد نظر تعریف نشده و یا شما مجوز کافی برای انجام آنرا ندارید');
}

function showMessage(msg) {
    /*$("div#header").prepend("<div class='Message'><span>" + msg + "</span></div>");*/
}

function sendAJAX(url, data, onSuccess, onError, onComplete) {
    $.ajax({
        type: "GET",
        url: "/" + url,
        contentType: "application/json; charset=utf-8",
        data: data,
        cache: false,
        success: function (result) {
            if (result.succeed) {
                var retVal = result.value != undefined ? result.value : "";
                if (typeof onSuccess == 'function') {
                    onSuccess(retVal);
                }
            }
            else {
                if (typeof onError == 'function') {
                    onError(result);
                }
            }
            if (result.msg != undefined && result.msg != '') {
                showMessage(result.msg);
            }
        },
        error: function (result) { if (typeof onError == 'function') { onError(result); } else { /*showMessage("Error in Connection...");*/ } },
        complete: function () { if (typeof onComplete == 'function') { onComplete(); } }
    });
}

function RebindCSRFToken(target) {
    if ($(target).find('>input:hidden[name="__RequestVerificationToken"]').size() == 0) {
        $(target).append($('<input type="hidden" value="' + $('input:hidden[name="__RequestVerificationToken"]').val() + '" name="__RequestVerificationToken">'));
    }
}

function postAJAX(url, data, onSuccess, onError, onComplete) {
    var dataparam = JSON.parse(data);
    /*var dataparam = JSON.parse(data.replace(/\'/g, "\""));*/
    dataparam.__RequestVerificationToken = $('input:hidden[name="__RequestVerificationToken"]').val();
    $.ajax({
        type: "POST",
        url: "/" + url,
        /*contentType: "application/json; charset=utf-8",
        dataType: "json",*/
        data: dataparam,
        cache: false,
        success: function (result) {
            if (result.succeed) {
                var retVal = result.value != undefined ? result.value : "";
                if (typeof onSuccess == 'function') {
                    onSuccess(retVal);
                }
            }
            else {
                if (typeof onError == 'function') {
                    onError(result);
                }
            }
            if (result.msg != undefined && result.msg != '') {
                /*showMessage(result.msg);*/
            }
        },
        error: function () { if (typeof onError == 'function') { onError(result); } else { /*showMessage("Error in Connection...");*/ } },
        complete: function () { if (typeof onComplete == 'function') { onComplete(); } }
    });
}

function getCookie(c_name) {
    var i, x, y, ARRcookies = document.cookie.split(";");
    for (i = 0; i < ARRcookies.length; i++) {
        x = ARRcookies[i].substr(0, ARRcookies[i].indexOf("="));
        y = ARRcookies[i].substr(ARRcookies[i].indexOf("=") + 1);
        x = x.replace(/^\s+|\s+$/g, "");
        if (x == c_name) {
            return unescape(y);
        }
    }
}

function setCookie(c_name, value, exdays) {
    var exdate = new Date();
    exdate.setDate(exdate.getDate() + exdays);
    var c_value = escape(value) + ((exdays == null) ? "" : "; expires=" + exdate.toUTCString());
    document.cookie = c_name + "=" + c_value + ";path=/";
}

function getFlashMovieObject(movieName) {
    if (window.document[movieName]) {
        return window.document[movieName];
    }
    if (navigator.appName.indexOf("Microsoft Internet") == -1) {
        if (document.embeds && document.embeds[movieName])
            return document.embeds[movieName];
    }
    else /* if (navigator.appName.indexOf("Microsoft Internet")!=-1)*/
    {
        return document.getElementById(movieName);
    }
}

function validateEmail(elementValue) {
    var emailPattern = /^[a-zA-Z0-9._-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,4}$/;
    return emailPattern.test(elementValue);
}


function convertTicksToDate(ticks) {
    var epochMicrotimeDiff = 621355968000000000;
    var tickDate = new Date((ticks - epochMicrotimeDiff) / 10000);
    timezondiff = new Date().getTimezoneOffset()
    finaldate=new Date(tickDate.valueOf() + timezondiff * 60000)
    return finaldate;
}

function convertTicksToPersianDate(ticks) {
    var pdate = persianDate(convertTicksToDate(ticks)).pDate;
    return pdate.year+"/"+pdate.month+"/"+pdate.day+" "+pdate.hours+":"+pdate.minutes+":"+pdate.seconds

}

function handelbarfunchelperinit() {
    Handlebars.registerHelper("ticksToPDate", function (ticks) {
        return convertTicksToPersianDate(ticks);
    });
    Handlebars.registerHelper('ifCond', function (v1, v2, options) {
        if (v1 === v2) {
            return options.fn(this);
        }
        return options.inverse(this);
    });
    Handlebars.registerHelper('replace', function (find, replace, options) {
        var string = options.fn(this);
        return string.replace(find, replace);
    });
}

function loader(objhtml) {
    $(objhtml).html($('<div class="loader04"></div>'));
}
function disloader(objhtml) {
    $(objhtml).html('');
}