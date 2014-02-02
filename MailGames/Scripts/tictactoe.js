var __extends = this.__extends || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    __.prototype = b.prototype;
    d.prototype = new __();
};
var TicTacToeCell = (function () {
    function TicTacToeCell(lastmove, myPos) {
        this.player = ko.observable();
        this.lastmove = ko.computed(function () {
            return lastmove() == myPos;
        });
    }
    return TicTacToeCell;
})();

var TicTacToeViewModel = (function (_super) {
    __extends(TicTacToeViewModel, _super);
    function TicTacToeViewModel() {
        _super.apply(this, arguments);
        this.cells = [];
        this.lastmove = ko.observable();
    }
    TicTacToeViewModel.prototype.posToIndex = function (x, y) {
        return x * this.size + y;
    };

    TicTacToeViewModel.prototype.getCell = function (x, y) {
        return this.cells[this.posToIndex(x, y)];
    };

    TicTacToeViewModel.prototype.updateModel = function (model) {
        _super.prototype.updateModel.call(this, model);
        if (model.LastMove != null)
            this.lastmove(this.posToIndex(model.LastMove.X, model.LastMove.Y));
        this.size = Math.sqrt(model.Colors.length);
        for (var i = 0; i < model.Colors.length; i++) {
            if (this.cells[i] == null)
                this.cells[i] = new TicTacToeCell(this.lastmove, i);
            this.cells[i].player(model.Colors[i]);
        }
    };
    return TicTacToeViewModel;
})(GameViewModel);

Game.setViewModel(new TicTacToeViewModel(), JSON.parse($("#tictactoeboard").attr("data-model")));
