/// <reference path="typings/jquery/jquery.d.ts"/>
/// <reference path="typings/bootstrap/bootstrap.d.ts"/>
/// <reference path="game.ts"/>
/// <reference path="typings/knockout/knockout.d.ts"/>
var __extends = this.__extends || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    __.prototype = b.prototype;
    d.prototype = new __();
};
//var columns = [0, 1, 2, 3, 4, 5, 6, 7];
//var originalRows = columns;
function getColName(c) {
    return String.fromCharCode('A'.charCodeAt(0) + c);
}

function getRowName(r) {
    return 7 - r + 1;
}

function getPositionName(position) {
    return getColName(position.X) + getRowName(position.Y);
}

var PieceType;
(function (PieceType) {
    PieceType[PieceType["Pawn"] = 0] = "Pawn";
    PieceType[PieceType["Knight"] = 1] = "Knight";
    PieceType[PieceType["Bishop"] = 2] = "Bishop";
    PieceType[PieceType["Rook"] = 3] = "Rook";
    PieceType[PieceType["Queen"] = 4] = "Queen";
    PieceType[PieceType["King"] = 5] = "King";
})(PieceType || (PieceType = {}));

function getPieceClasses(player, piece, attacked, selectable, selected) {
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

//if (model.PlayerColor == GamePlayer.SecondPlayer) {
//    originalRows = originalRows.reverse();
//}
var PieceViewModel = (function () {
    function PieceViewModel(model, app, piece, selectable, attacked, c, r) {
        this.selectable = selectable;
        this.attacked = attacked;
        var self = this;
        this.player = piece.GamePlayer;
        this.piece = piece.PieceType;
        this.canSelect = this.player == app.currentColor();
        var selected = positionToInt(c, r);
        this.pieceClasses = ko.computed(function () {
            return getPieceClasses(self.player, self.piece, attacked, selectable, app.selectedSource() == selected);
        });
        this.selectPiece = function () {
            if (!self.canSelect)
                return;
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
        };
    }
    return PieceViewModel;
})();

function findPiece(model, app, c, r) {
    var piece = model.Cells[c * 8 + r];
    if (piece == null)
        return null;
    var selectable = app.currentColor() == app.playerColor() && piece.GamePlayer == app.currentColor();
    var attacked = piece.PieceType == PieceType.King && piece.GamePlayer == model.AttackedKing;
    return new PieceViewModel(model, app, piece, selectable, attacked, c, r);
}

function positionToInt(c, r) {
    return c * 10 + r;
}

function isLastMove(model, c, r) {
    var intPos = positionToInt(c, r);
    return model.OpponentMoves.indexOf(intPos) >= 0;
}

function getRowsAndColumns(model) {
    var rows = [0, 1, 2, 3, 4, 5, 6, 7];
    if (model.PlayerColor == GamePlayer.SecondPlayer)
        rows.reverse();
    return rows;
}

function getMoveUrl(app) {
    return "/chess/move?board=" + app.id + "&from=" + app.selectedSource() + "&to=" + app.selectedTarget();
}

function createRows(model, app) {
    var columns = getRowsAndColumns(model);
    var rows = columns;
    return $.map(rows, function (r) {
        return {
            cells: $.map(columns, function (c) {
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
                        if (!this.availableTarget())
                            return;
                        app.selectedTarget(positionToInt(c, r));
                        app.upgradePawnMode(app.shouldUpgradePawn);
                        if (!app.upgradePawnMode()) {
                            var moveUrl = getMoveUrl(app);
                            Game.activateMoveButton(moveUrl);
                        }
                    },
                    updateModel: function (model) {
                        this.piece(findPiece(model, app, c, r));
                        this.lastMove(isLastMove(model, c, r));
                    }
                };
            }),
            rowName: getRowName(r)
        };
    });
}

function createCols(model) {
    return $.map(getRowsAndColumns(model), function (c) {
        return { colName: getColName(c) };
    });
}

function createMoves(model, app) {
    return $.map(model.Moves, function (obj) {
        return createMove(model, app, obj);
    });
}

function createMove(model, app, move) {
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
    };
}

function createCapturedPieces(model, app) {
    return $.map(model.CapturedPieces, function (c) {
        return new PieceViewModel(model, app, c, false, false, null, null);
    });
}

function getPointsForPieceType(pieceType) {
    switch (pieceType) {
        case PieceType.Pawn:
            return 1;
        case PieceType.Bishop:
        case PieceType.Knight:
            return 3;
        case PieceType.Rook:
            return 5;
        case PieceType.Queen:
            return 9;
    }
    return 0;
}

function getPoints(app) {
    var progress = 0;
    $.each(app.pieces(), function (i, piece) {
        var points = getPointsForPieceType(piece.piece);
        if (piece.player == app.playerColor()) {
            progress += points;
        } else {
            progress -= points;
        }
    });
    return progress;
}

function getProgress(app) {
    var progress = getPoints(app);
    return Math.max(0, Math.min(100, 50 + progress * 50 / 9));
}

var ChessViewModel = (function (_super) {
    __extends(ChessViewModel, _super);
    function ChessViewModel(model) {
        _super.call(this);
        this.capturedPieces = ko.observableArray();
        this.moves = ko.observableArray();
        this.availableTargets = ko.observableArray();
        this.selectedTarget = ko.observable();
        this.selectedSource = ko.observable();
        this.upgradePawnMode = ko.observable(false);
        this.selectedMove = ko.observable();
        this.shouldUpgradePawn = false;
        this.playerColor = ko.observable();
        this.currentColor = ko.observable();
        var self = this;
        this.rows = createRows(model, this);
        this.cols = createCols(model);
        this.pieces = ko.computed(function () {
            return getPieces(self);
        });
        this.progress = ko.computed(function () {
            return getProgress(self);
        });
        this.dangerProgress = ko.computed(function () {
            return Math.max(0, (50 - self.progress()) * 2) + "%";
        });
        this.successProgress = ko.computed(function () {
            return Math.max(0, (self.progress() - 50) * 2) + "%";
        });
        this.points = ko.computed(function () {
            return getPoints(self);
        });
    }
    ChessViewModel.prototype.updateModel = function (model) {
        _super.prototype.updateModel.call(this, model);

        this.id = model.Id;

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
    };

    ChessViewModel.prototype.convertPawnTo = function (convertTo) {
        this.upgradePawnMode(false);
        Game.call(getMoveUrl(this) + "&convertPawnTo=" + convertTo);
    };
    return ChessViewModel;
})(GameViewModel);

function getPieces(app) {
    var pieces = [];
    $.each(app.rows, function (i, row) {
        $.each(row.cells, function (i, cell) {
            if (cell.piece() != null) {
                pieces.push(cell.piece());
            }
        });
    });
    return pieces;
}

var initialModel = JSON.parse($("#chesstable").attr("data-model"));
Game.setViewModel(new ChessViewModel(initialModel), initialModel);
