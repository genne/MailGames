// Module
module Game {

    export function activateMoveButton(url: string) {
        $(".code-move-button").attr("href", url).removeClass("disabled");
    };

    export function resetMoveButton() {
        $(".code-move-button").addClass("disabled").removeAttr("href");
    }

    export function setup() {
        var current = null;
        $(".code-use-move-button a").click(function (ev) {
            ev.preventDefault();
            if (current != null) current.removeClass("selected");
            current = $(ev.currentTarget);
            current.addClass("selected");
            activateMoveButton(current.attr("href"));
        });
    }
}

Game.setup();