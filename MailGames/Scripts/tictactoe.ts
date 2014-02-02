interface TicTacToeModel extends GameModel {
    Colors: GamePlayer[];
    LastMove: GamePosition;
}

class TicTacToeCell {
    player = ko.observable();
    lastmove;

    constructor(lastmove: KnockoutObservable<number>, myPos: number) {
        this.lastmove = ko.computed(function () {
            return lastmove() == myPos;
        });
    }
}

class TicTacToeViewModel extends GameViewModel {
    cells: TicTacToeCell[] = [];
    size: number;
    lastmove = ko.observable();

    posToIndex(x: number, y: number): number {
        return x * this.size + y;
    }

    getCell(x: number, y: number): TicTacToeCell {
        return this.cells[this.posToIndex(x, y)];
    }

    updateModel(model: TicTacToeModel) {
        super.updateModel(model);
        if (model.LastMove != null)
            this.lastmove(this.posToIndex(model.LastMove.X, model.LastMove.Y));
        this.size = Math.sqrt(model.Colors.length);
        for (var i = 0; i < model.Colors.length; i++) {
            if (this.cells[i] == null) this.cells[i] = new TicTacToeCell(this.lastmove, i);
            this.cells[i].player(model.Colors[i]);
        }
    }
}

Game.setViewModel(new TicTacToeViewModel(), JSON.parse($("#tictactoeboard").attr("data-model")));