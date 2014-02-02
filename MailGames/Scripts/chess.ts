/// <reference path="typings/jquery/jquery.d.ts"/>
/// <reference path="typings/bootstrap/bootstrap.d.ts"/>
/// <reference path="game.ts"/>
/// <reference path="typings/knockout/knockout.d.ts"/>

//var columns = [0, 1, 2, 3, 4, 5, 6, 7];
//var originalRows = columns;

function getColName(c) {
    return String.fromCharCode('A'.charCodeAt(0) + c);
}

function getRowName(r) {
    return 7 - r + 1;
}

function getPositionName(position: GamePosition) {
    return getColName(position.X) + getRowName(position.Y);
}

enum PieceType {
    Pawn,
    Knight,
    Bishop,
    Rook,
    Queen,
    King
}

function getPieceClasses(player: GamePlayer, piece: PieceType, attacked: boolean, selectable: boolean, selected: boolean) {
    var classes = [];
    var playerName = GamePlayer[player].toLowerCase();
    var pieceName = PieceType[piece].toLowerCase();
    classes.push("piece-color-" + playerName);
    classes.push("piece-" + pieceName);
    if (attacked) {
        classes.push("attacked");
    }
    if (selectable) {
        classes.push("selectable");
        if (selected)
            classes.push("selected");
    }
    return classes.join(" ");
}


interface Piece {
    GamePlayer: GamePlayer;
    PieceType: PieceType;
}

interface PieceMove {
    Piece: Piece;

    From: GamePosition;
    To: GamePosition;

    CapturedPiece: Piece;

}

interface ChessModel extends GameModel {
    Id: string;
    Cells: Piece[];
    PlayerColor: GamePlayer;
    CurrentColor: GamePlayer;
    AttackedKing: GamePlayer;
    OpponentMoves: number[];
    Moves: PieceMove[];
    CapturedPieces: Piece[];
}



//if (model.PlayerColor == GamePlayer.SecondPlayer) {
//    originalRows = originalRows.reverse();
//}

class PieceViewModel {

    player: GamePlayer;
    piece: PieceType;
    canSelect: boolean;
    pieceClasses;
    selectPiece;

    constructor(model: ChessModel, app: ChessViewModel, piece: Piece, public selectable: boolean, public attacked: boolean, c: number, r: number) {
        var self = this;
        this.player = piece.GamePlayer;
        this.piece = piece.PieceType;
        this.canSelect = this.player == app.currentColor();
        var selected = positionToInt(c, r);
        this.pieceClasses = ko.computed(function () {
            return getPieceClasses(self.player, self.piece, attacked, selectable, app.selectedSource() == selected);
        });
        this.selectPiece = function () {
            if (!self.canSelect) return;
            var rand = Math.random();
            $.post("/chess/getAvailableCells?board=" + model.Id + "&selected=" + selected + "&rand=" + rand, function (data) {
                app.availableTargets(data);
            });
            app.availableTargets([]);
            app.selectedSource(selected);
            app.selectedTarget(null);
            var pawnUpgradeRow = self.player == GamePlayer.FirstPlayer ? 1 : 6;
            app.shouldUpgradePawn = self.piece == PieceType.Pawn && r == pawnUpgradeRow;
            Game.resetMoveButton();
        }

    }

}

function findPiece(model: ChessModel, app: ChessViewModel, c: number, r: number): PieceViewModel {
    var piece: Piece = model.Cells[c * 8 + r];
    if (piece == null) return null;
    var selectable = app.currentColor() == app.playerColor() && piece.GamePlayer == app.currentColor();
    var attacked = piece.PieceType == PieceType.King && piece.GamePlayer == model.AttackedKing;
    return new PieceViewModel(model, app, piece, selectable, attacked, c, r);
}

function positionToInt(c: number, r: number) {
    return c * 10 + r;
}

function isLastMove(model: ChessModel, c: number, r: number): boolean {
    var intPos = positionToInt(c, r);
    return model.OpponentMoves.indexOf(intPos) >= 0;
}

interface Cell {
    piece: KnockoutObservable<PieceViewModel>;
    updateModel(model: ChessModel);
}

interface Row {
    cells: Cell[];
}

function getRowsAndColumns(model: ChessModel)
{
    var rows = [0, 1, 2, 3, 4, 5, 6, 7];
    if (model.PlayerColor == GamePlayer.SecondPlayer) rows.reverse();
    return rows;
}

function createRows(model: ChessModel, app: ChessViewModel): Row[] {
    var columns = getRowsAndColumns(model);
    var rows = columns;
    return $.map(rows, function (r: number) {

        return {
            cells: $.map(columns, function (c: number) {
                return {
                    piece: ko.observable(),
                    lastMove: ko.observable(),
                    availableTarget: ko.computed(function () {
                        return app.availableTargets().indexOf(positionToInt(c, r)) >= 0;
                    }),
                    selectedTarget: ko.computed(function () {
                        return app.selectedTarget() == positionToInt(c, r);
                    }),
                    selectCell: function () {
                        if (!this.availableTarget()) return;
                        app.selectedTarget(positionToInt(c, r));
                        app.upgradePawnMode(app.shouldUpgradePawn);
                        if (!app.upgradePawnMode()) {
                            var moveUrl = "/chess/move?board=" + model.Id + "&from=" + app.selectedSource() + "&to=" + app.selectedTarget();
                            Game.activateMoveButton(moveUrl);
                        }
                    },
                    updateModel: function (model: ChessModel) {
                        this.piece(findPiece(model, app, c, r));
                        this.lastMove(isLastMove(model, c, r));
                    }
                };
            }),
            rowName: getRowName(r)
        };
    });
}

function createCols(model: ChessModel) {
    return $.map(getRowsAndColumns(model), function (c) { return { colName: getColName(c) }; });
}

function createMoves(model: ChessModel, app: ChessViewModel) {
    return $.map(model.Moves, function (obj) { return createMove(model, app, obj); });
}

function createMove(model: ChessModel, app: ChessViewModel, move: PieceMove) {
    return {
        from: getPositionName(move.From),
        to: getPositionName(move.To),
        piece: new PieceViewModel(model, app, move.Piece, false, false, null, null),
        capturedPiece: move.CapturedPiece != null ? new PieceViewModel(model, app, move.CapturedPiece, false, true, null, null) : null,
        isSelected: ko.computed(function () {
            return app.selectedMove() == move;
        }),
        select: function () {
            app.selectedMove(move);
        }
    }
}

function createCapturedPieces(model: ChessModel, app: ChessViewModel) {
    return $.map(model.CapturedPieces, function (c) { return new PieceViewModel(model, app, c, false, false, null, null); });
}

function getPointsForPieceType(pieceType: PieceType) {
    switch (pieceType) {
        case PieceType.Pawn: return 1;
        case PieceType.Bishop:
        case PieceType.Knight:
            return 3;
        case PieceType.Rook: return 5;
        case PieceType.Queen: return 9;
    }
    return 0;
}

function getPoints(app: ChessViewModel): number {
    var progress = 0;
    $.each(app.pieces(), function (i, piece: PieceViewModel) {
        var points = getPointsForPieceType(piece.piece);
        if (piece.player == app.playerColor()) {
            progress += points;
        }
        else {
            progress -= points;
        }
    });
    return progress;
}

function getProgress(app: ChessViewModel) {
    var progress = getPoints(app);
    return Math.max(0, Math.min(100, 50 + progress * 50 / 9));
}

class ChessViewModel extends GameViewModel {
    capturedPieces = ko.observableArray();
    moves = ko.observableArray();
    availableTargets = ko.observableArray();
    selectedTarget = ko.observable();
    selectedSource = ko.observable();
    upgradePawnMode = ko.observable(false);
    selectedMove = ko.observable();
    shouldUpgradePawn = false;
    rows;
    cols;
    pieces : KnockoutComputed<PieceViewModel[]>;
    progress : KnockoutComputed<number>;
    points;
    dangerProgress;
    successProgress;
    playerColor = ko.observable();
    currentColor = ko.observable();

    constructor(model: ChessModel) {
        super();
        var self = this;
        this.rows = createRows(model, this);
        this.cols = createCols(model);
        this.pieces = ko.computed(function () { return getPieces(self); });
        this.progress = ko.computed(function () { return getProgress(self); });
        this.dangerProgress = ko.computed(function () { return Math.max(0, (50 - self.progress()) * 2) + "%"; });
        this.successProgress = ko.computed(function () { return Math.max(0, (self.progress() - 50) * 2) + "%"; });
        this.points = ko.computed(function () {
            return getPoints(self);
        });
    }

    updateModel(model: ChessModel) {
        super.updateModel(model);

        this.playerColor(model.PlayerColor);
        this.currentColor(model.CurrentColor);

        $.each(this.rows, function (i, row) {
            $.each(row.cells, function (j, cell) {
                cell.updateModel(model);
            });
        });
        this.moves(createMoves(model, this));
        this.capturedPieces(createCapturedPieces(model, this));

        this.selectedTarget(null);
        this.selectedSource(null);
        this.availableTargets([]);
    }

}

function getPieces(app: ChessViewModel): PieceViewModel[] {
    var pieces = [];
    $.each(app.rows, function (i, row : Row) {
        $.each(row.cells, function (i, cell: Cell) {
            if (cell.piece() != null) {
                pieces.push(cell.piece());
            }
        });
    });
    return pieces;
}

var initialModel: ChessModel = JSON.parse($("#chesstable").attr("data-model"));
Game.setViewModel(new ChessViewModel(initialModel), initialModel);
