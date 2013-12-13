/// <reference path="typings/jquery/jquery.d.ts"/>
/// <reference path="typings/bootstrap/bootstrap.d.ts"/>
/// <reference path="game.ts"/>

$(function () {
    var rand = Math.random();
    var currentTableCell = null;
    var currentSourcePiece = null;
    var showUpgradePawnModal = function (callbackFunction) {
        $("#pawn-upgrade-modal").modal("show");
        $(".code-convert-pawn-button").unbind("click").click(function (ev) {
            ev.preventDefault();
            callbackFunction($(ev.currentTarget).attr("data-piece-type"));
        });
    };
    var clearCurrentTableCell = function () {
        if (currentTableCell != null) {
            currentTableCell.removeClass("selected-target");
            currentTableCell = null;
        }
        Game.resetMoveButton();
    }
    var highlightAvailableCells = function (p) {
        var chessTable = p.closest(".chess-table");
        chessTable.find(".available-target").removeClass("available-target").unbind("click");
        clearCurrentTableCell();
        if (currentSourcePiece == p[0]) {
            currentSourcePiece = null;
            return;
        }
        currentSourcePiece = p[0];
        var board = chessTable.attr("data-board");
        var selected = p.closest("[data-cell]").attr("data-cell");
        var upgradePawn = p.hasClass("code-upgrade-pawn");
        $.post("/chess/getAvailableCells?board=" + board + "&selected=" + selected + "&rand=" + rand, function (data) {
            $.each(data, function (i, cell) {
                var tableCell = chessTable.find("[data-cell=" + cell + "]");
                tableCell.addClass("available-target").click(function (ev) {
                    clearCurrentTableCell();
                    currentTableCell = tableCell;
                    tableCell.addClass("selected-target");
                    var moveUrl = "/chess/move?board=" + board + "&from=" + selected + "&to=" + cell;
                    if (upgradePawn) {
                        showUpgradePawnModal(function (convertPawnTo) {
                            window.location.href = moveUrl + "&convertPawnTo=" + convertPawnTo;
                        });
                    } else {
                        Game.activateMoveButton(moveUrl);
                    }
                });
            });
        });
    };
    var selectPiece = function (p) {
        p.closest(".chess-table").find(".selected").removeClass("selected");
        p.toggleClass("selected");

        highlightAvailableCells(p);
    };
    $(".piece.selectable").click(function (ev) {
        selectPiece($(ev.currentTarget));
    });
});