/// <reference path="typings/fb/fb.d.ts" />

var friendSelector = $("[name='playedOpponent']");
friendSelector.change(function (ev) {
    if (friendSelector.val() == "") {
        $("#opponentsmail-group").stop().slideDown(null, function () {
            $("#opponentsmail-group input").focus();
        });
    }
    else {
        $("#opponentsmail-group").stop().slideUp();
    }
}).change();

function getSelectedOption(selector : JQuery) : JQuery {
    return selector.find("option[value='" + selector.val() + "']");
}

var gameTypeSelector = $("select[name='gameType']");
gameTypeSelector.change(function (ev) {
    var computerOpponent = $("#computer-opponent");
    computerOpponent.toggle(gameTypeSelector.val() == "Chess" || gameTypeSelector.val() == "CrissCross" || gameTypeSelector.val() == "TicTacToe" || gameTypeSelector.val() == "Othello");
    if (computerOpponent.is(":selected")) {
        computerOpponent.closest("select").val("").change();
    }
}).change();

function getSelectedGameName() {
    return getSelectedOption(gameTypeSelector).text();
}

function getSelectedPlayerFBUserId() : number {
    var playerSelector = $("select[name='playedOpponent']");
    var id = getSelectedOption(playerSelector).attr("data-fb-id");
    if (id != "" && id != null) return parseInt(id);
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
