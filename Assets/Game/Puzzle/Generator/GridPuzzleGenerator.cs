using System.Collections.Generic;
using UnityEngine;

public class GridPuzzleGenerator : IPuzzleGenerator {
    public List<PuzzlePieceData> Generate(
        int columns,
        int rows,
        Vector2 imageSize
    ) {
        var pieces = new List<PuzzlePieceData>(columns * rows);
        var pieceWidth = imageSize.x / columns;
        var pieceHeight = imageSize.y / rows;

        for (var y = 0; y < rows; y++)
        for (var x = 0; x < columns; x++) {
            var pieceData = CreateRectanglePiece(
                x,
                y,
                pieceWidth,
                pieceHeight,
                columns,
                rows
            );
            pieces.Add(pieceData);
        }

        return pieces;
    }

    PuzzlePieceData CreateRectanglePiece(
        int gridX, int gridY,
        float pieceWidth, float pieceHeight,
        int columns, int rows
    ) {
        var mesh = new Mesh { name = $"Piece_{gridX}_{gridY}" };

        var positions = new Vector3[] {
            new(-pieceWidth / 2, -pieceHeight / 2, 0),
            new(pieceWidth / 2, -pieceHeight / 2, 0),
            new(pieceWidth / 2, pieceHeight / 2, 0),
            new(-pieceWidth / 2, pieceHeight / 2, 0)
        };

        var uvX = (float)gridX / columns;
        var uvY = (float)gridY / rows;
        var uvWidth = 1f / columns;
        var uvHeight = 1f / rows;

        var uvs = new Vector2[] {
            new(uvX, uvY),
            new(uvX + uvWidth, uvY),
            new(uvX + uvWidth, uvY + uvHeight),
            new(uvX, uvY + uvHeight)
        };

        mesh.vertices = positions;
        mesh.uv = uvs;
        mesh.triangles = new[] { 0, 2, 1, 0, 3, 2 };
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        var centerPosition = new Vector2(
            (gridX + 0.5f) * pieceWidth,
            (gridY + 0.5f) * pieceHeight
        );

        var bounds = new Bounds(
            centerPosition,
            new(pieceWidth, pieceHeight, 0)
        );

        return new() {
            Mesh = mesh,
            CenterPosition = centerPosition,
            UVRegion = new(uvX, uvY, uvWidth, uvHeight),
            Bounds = bounds
        };
    }
}