ko.bindingHandlers["showModal"] = {
    update: function (elem, valueAccessor) {
        if (valueAccessor()) {
            $(elem).modal("show");
        }
    }
};

ko.bindingHandlers["switch"] = {
    init: function (element, valueAccessor, allBindings, viewModel, bindingContext) {
        var innerBindingContext = bindingContext.extend({ "switch": valueAccessor });
        ko.applyBindingsToDescendants(innerBindingContext, element);

        return { controlsDescendantBindings: true };
    }
};
ko.virtualElements.allowedBindings["switch"] = true;
ko.virtualElements.allowedBindings["case"] = true;

ko.bindingHandlers["case"] = {
    update: function (elem, valueAccessor, allBindings, viewModel, bindingContext) {
        var value = ko.utils.unwrapObservable(valueAccessor());
        var switchValue = ko.utils.unwrapObservable(bindingContext["switch"]());
        $(ko.virtualElements.childNodes(elem)).toggle(switchValue == value);
    }
};

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

    function bindMoveButton(callback) {
        $(".code-move-button").click(function (ev) {
            ev.preventDefault();
            callback($(".code-move-button").attr("href"));
        });
    }
    Game.bindMoveButton = bindMoveButton;

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

    Game.app;

    function setViewModel(viewModel, data) {
        Game.app = viewModel;
        viewModel.updateModel(data);
        ko.applyBindings(viewModel, $("#wrap")[0]);
        Game.bindMoveButton(function (url) {
            $.post(url, function (data) {
                viewModel.updateModel(data);
            });
        });
    }
    Game.setViewModel = setViewModel;
})(Game || (Game = {}));

var GamePlayer;
(function (GamePlayer) {
    GamePlayer[GamePlayer["FirstPlayer"] = 0] = "FirstPlayer";
    GamePlayer[GamePlayer["SecondPlayer"] = 1] = "SecondPlayer";
})(GamePlayer || (GamePlayer = {}));

var GameState;
(function (GameState) {
    GameState[GameState["YourTurn"] = 0] = "YourTurn";
    GameState[GameState["OpponentsTurn"] = 1] = "OpponentsTurn";
    GameState[GameState["PlayerWon"] = 2] = "PlayerWon";
    GameState[GameState["OpponentWon"] = 3] = "OpponentWon";
    GameState[GameState["Tie"] = 4] = "Tie";
})(GameState || (GameState = {}));

var Activity;
(function (Activity) {
    Activity[Activity["Active"] = 0] = "Active";
    Activity[Activity["Passive"] = 1] = "Passive";
    Activity[Activity["PassiveLostGame"] = 2] = "PassiveLostGame";
    Activity[Activity["GameNeverStarted"] = 3] = "GameNeverStarted";
})(Activity || (Activity = {}));

var GameViewModel = (function () {
    function GameViewModel() {
        this.state = ko.observable();
        this.opponentActivity = ko.observable();
        var self = this;
        this.gameOver = ko.computed(function () {
            return self.state() >= GameState.PlayerWon;
        });
    }
    GameViewModel.prototype.updateModel = function (model) {
        this.state(model.State);
        this.opponentActivity(model.OpponentActivity);
        updateNumWaitingGames();
    };
    return GameViewModel;
})();

Game.setup();

var headerViewModel = {
    numWaitingGames: ko.observable()
};

ko.applyBindings(headerViewModel, $("#header")[0]);

function updateNumWaitingGames() {
    $.get("/home/numWaitingGames", function (data) {
        headerViewModel.numWaitingGames(parseInt(data));
    });
}

function updateNumWaitingGamesLoop() {
    updateNumWaitingGames();
    window.setTimeout(updateNumWaitingGamesLoop, 10000);
}

updateNumWaitingGamesLoop();
