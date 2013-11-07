/// <reference path="typings/jquery/jquery.d.ts"/>

$(".code-nice-date").each(function(i, e) {
    var dateString = $(e).text();
    if (dateString == "") {
        $(e).text("never");
        return;
    }
    var d = Date.parse(dateString);
    var serverTime = Date.parse($("body").attr("data-servertime"));
    var now = new Date().getTime();
    var serverTimeDiff = serverTime - now;

    var update = function() {
        var diff = now - d + serverTimeDiff;

        var suffix = "ago";
        if (diff < 0) {
            diff = -diff;
            suffix = "left";
        }

        diff = diff / 1000; // seconds
        var text = "";
        if (diff < 60) {
            text = "second";
        } else {
            diff /= 60;
            if (diff < 60) {
                text = "minute";
            } else {
                diff /= 60;
                if (diff < 24) {
                    text = "hour";
                } else {
                    diff /= 24;
                    if (diff < 7) {
                        text = "day";
                    } else {
                        diff /= 7;
                        text = "week";
                    }
                }
            }
        }

        if (diff >= 2) text += "s"; // Plural
        $(e).text(Math.floor(diff) + " " + text + " " + suffix);
        window.setTimeout(update, 1000);
    };
    update();
});