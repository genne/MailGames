ko.bindingHandlers["showModal"] = {
    update: function (elem, valueAccessor) {
        if (valueAccessor()) {
            $(elem).modal("show");
        }
        else {
            $(elem).modal("hide");
        }
    }
}

ko.bindingHandlers["switch"] = {
    init: function (element, valueAccessor, allBindings, viewModel, bindingContext) {

        var innerBindingContext = bindingContext.extend({ "switch": valueAccessor });
        ko.applyBindingsToDescendants(innerBindingContext, element);

        return { controlsDescendantBindings: true };
    },
}
ko.virtualElements.allowedBindings["switch"] = true;
ko.virtualElements.allowedBindings["case"] = true;

ko.bindingHandlers["case"] = {
    update: function (elem, valueAccessor, allBindings, viewModel, bindingContext) {
        var value = ko.utils.unwrapObservable(valueAccessor());
        var switchValue = ko.utils.unwrapObservable(bindingContext["switch"]());
        $(ko.virtualElements.childNodes(elem)).toggle(switchValue == value);
    }
}

module Game {

    export function activateMoveButton(url: string) {
        $(".code-move-button").attr("href", url).removeClass("disabled");
    };

    export function call(url: string) {
        activateMoveButton(url);
        $(".code-move-button").click();
    }

    export function resetMoveButton() {
        $(".code-move-button").addClass("disabled").removeAttr("href");
    }

    export function bindMoveButton(callback: (url: string) => void) {
        $(".code-move-button").click(function (ev)
        {
            ev.preventDefault();
            callback($(".code-move-button").attr("href"));
        });
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

        $(".code-move-button").click(function (ev) {
            window.setTimeout(function () {
                resetMoveButton();
            });
        });
    }

    export var app;

    export function setViewModel(viewModel: GameViewModel, data: GameModel) {
        app = viewModel;
        viewModel.updateModel(data);
        ko.applyBindings(viewModel, $("#wrap")[0]);
        Game.bindMoveButton(function (url: string) {
            $.post(url, function (data) {
                viewModel.updateModel(data);
            });
        });
    }
}

interface GamePosition {
    X: number;
    Y: number;
}

enum GamePlayer {
    FirstPlayer,
    SecondPlayer
}


enum GameState {
    YourTurn,
    OpponentsTurn,
    PlayerWon,
    OpponentWon,
    Tie
}

enum Activity {
    Active,
    Passive,
    PassiveLostGame,
    GameNeverStarted
}

interface GameModel {
    State: GameState;
    OpponentActivity: Activity;
}

class GameViewModel {
    state = ko.observable();
    opponentActivity = ko.observable();
    gameOver;

    updateModel(model: GameModel) {
        this.state(model.State);
        this.opponentActivity(model.OpponentActivity);
        updateNumWaitingGames();
    }

    constructor() {
        var self = this;
        this.gameOver = ko.computed(function () {
            return self.state() >= GameState.PlayerWon;
        });
    }
}

Game.setup();

var headerViewModel = {
    numWaitingGames: ko.observable()
};

ko.applyBindings(headerViewModel, $("#header")[0]);

function updateNumWaitingGames() {
    $.get("/home/numWaitingGames", function (data: string) {
        headerViewModel.numWaitingGames(parseInt(data));
    });
}

function updateNumWaitingGamesLoop() {
    updateNumWaitingGames();
    window.setTimeout(updateNumWaitingGamesLoop, 10000);
}

updateNumWaitingGamesLoop();