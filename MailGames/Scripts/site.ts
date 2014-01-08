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

function updateGrids() {
    $(".code-grid").each(function (i, e) {
        var o = $(e);
        o.width("auto");
        var width = o.parent().width();
        var excludeGridSelector = ".code-exclude-grid";
        width -= o.find("tr").first().find(excludeGridSelector).outerWidth(true);
        var tdSelector = "td:not(" + excludeGridSelector + ")";
        var numCols = o.find("tr").first().find(tdSelector).length;
        var cellSize = (width / numCols) - 4;
        o.find(tdSelector).width(cellSize).height(cellSize).css("font-size", cellSize / 1.5);
    });
}

$(window).resize(function () {
    updateGrids();
});
$(function() {
    updateGrids();
});

$("select[data-value]").each(function (i, o) {
    $(o).val($(o).attr("data-value"));
});