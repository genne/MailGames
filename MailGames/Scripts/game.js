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
    }
    Game.setup = setup;
})(Game || (Game = {}));

Game.setup();
