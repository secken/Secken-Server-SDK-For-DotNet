﻿

$(function () {
    var isPass = false;

    function verify(eid) {
        var obj = {
            eid: eid,
            Action: "CheckYcAuthResult"
        }
        obj.t = (new Date()).getTime();
        $.getJSON("/YcAuth.ashx", obj, function (result) {
            if (!isPass) {
                if (result.code != "0") {
                    if (result.code == "-4") {
                        show = false;
                        if (result.status == "603") {
                            stateChange("二维码过期,请<span class='hightLightSpan'>刷新</span>页面后重试")
                            bindRefresh();
                            isPass = true;
                        } else if (result.status != "602") {
                            stateChange("系统服务错误！");
                            isPass = true;
                        }
                    } else if (result.code == "-3") {
                        stateChange("登录失败,请<span class='hightLightSpan'>刷新</span>页面后重试")
                        bindRefresh();
                        isPass = true;
                    }
                    if (!isPass) {
                        setTimeout(verify(eid), 2000);
                    }
                } else {
                    isPass = true;
                    window.location.href = "/loginsuccess.html";
                }
            }
        });
    };

    stateChange("Is Loading QrCode...");
    var obj = {
        uid: "secken",
        Action: "AskYangAuthPush"
    }
    obj.t = (new Date()).getTime();
    $.getJSON("/YcAuth.ashx", obj, function (result) {
        if (result.code == "0") {
            eid = result.event_id;
            $("#qr_img").attr("src", "/static/image/loading.gif");
            hideState();
            verify(eid);
        } else if (result.code == "-1") {
            stateChange("获取失败!");
        }
    });


    function bindRefresh() {
        $(".hightLightSpan").bind("click", function () {
            window.location.reload();
        });
    }
})

function stateChange(tip) {
    $(".qrCodeOverdue").html(tip);
    $(".qrCodeOverdue").show();
}

function hideState() {
    $(".qrCodeOverdue").hide();
}