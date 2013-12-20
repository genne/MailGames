/// <reference path="typings/fb/fb.d.ts" />

var friendSelector = $("[name='playedOpponent']");
friendSelector.change(function (ev) {
    if (friendSelector.val() == "") {
        $("#opponentsmail-group").slideDown(null, function () {
            $("#opponentsmail-group input").focus();
        });
    }
    else {
        $("#opponentsmail-group").slideUp();
    }
});

function getSelectedOption(selector : JQuery) : JQuery {
    return selector.find("option[value='" + selector.val() + "']");
}

function getSelectedGameName() {
    var gameTypeSelector = $("select[name='gameType']");
    return getSelectedOption(gameTypeSelector).text();
}

function getSelectedPlayerFBUserId() : number {
    var playerSelector = $("select[name='playedOpponent']");
    var id = getSelectedOption(playerSelector).attr("data-fb-id");
    if (id != "") return parseInt(id);
    return null;
}

var submitButton = $("#startgame-button");
submitButton.click(function (ev) {
    var selectedPlayerFBUserId = getSelectedPlayerFBUserId();
    if (selectedPlayerFBUserId != null) {
        ev.preventDefault();
        var nameOfGame = getSelectedGameName();
        FB.ui({
            method: 'apprequests',
            message: 'I want to play ' + nameOfGame + ' against you!',
            to: [selectedPlayerFBUserId]
        }, function (args) {
            if (args.request != null) {
                var form = submitButton.closest("form");
                form.submit();
            }
        });
    }
});

//window.setTimeout(function () {

//    FB.ui({
//        method: 'apprequests',
//        message: 'Test',
//        to: [692791562]
//    }, function (args) {
//            console.log("Good!", args);
//        });
//}, 3000);