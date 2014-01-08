// Module
var Game;
(function (Game) {
    function activateMoveButton(url) {
        $(".code-move-button").attr("href", url).removeClass("disabled");
    }
    Game.activateMoveButton = activateMoveButton;
    ;

    function resetMoveButton() {
        $(".code-move-button").addClass("disabled").removeAttr("href");
    }
    Game.resetMoveButton = resetMoveButton;

    function setup() {
        var current = null;
        $(".code-use-move-button a").click(function (ev) {
            ev.preventDefault();
            if (current != null)
                current.removeClass("selected");
            current = $(ev.currentTarget);
            current.addClass("selected");
            activateMoveButton(current.attr("href"));
        });

        $(".code-move-button").click(function (ev) {
            window.setTimeout(function () {
                resetMoveButton();
            });
        });
    }
    Game.setup = setup;
})(Game || (Game = {}));

Game.setup();
