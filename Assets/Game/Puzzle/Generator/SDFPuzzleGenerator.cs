using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

// Generates a grid of square pieces. Knobs/sockets are not geometry: each piece is a quad
// large enough to contain its knobs, and the shader draws the shape from the edge data.
public class SDFPuzzleGenerator : IPuzzleGenerator {
    public float BaseSpan { get; set; } = 0.30f; // Knob width as a fraction of the side it sits on.
    public float SpanJitter { get; set; } = 0.05f; // +/- absolute variation of width.
    public float CenterJitter { get; set; } = 0.08f; // +/- absolute shift of knob centre.

    public float MinOffsetRatio { get; set; } =
        0.7f; // Circle centre distance from the square's edge, as a fraction of the radius.

    public float MaxOffsetRatio { get; set; } = 0.9f;

    public List<PuzzlePieceData> Generate(int columns, int rows, Vector2 imageSize) {
        if (columns <= 0 || rows <= 0)
            throw new ArgumentOutOfRangeException(nameof(columns), "Columns and rows must be > 0.");

        var cellSize = new Vector2(imageSize.x / columns, imageSize.y / rows);

        // Seams between neighbouring cells. Each seam is generated once and used by both pieces.
        // verticalSeams[y, x] sits between cell (x, y) and (x + 1, y), its knob runs along the cell height.
        // horizontalSeams[y, x] sits between cell (x, y) and (x, y + 1), its knob runs along the cell width.
        var verticalSeams = new SeamShape[rows, Mathf.Max(columns - 1, 0)];
        for (var y = 0; y < rows; y++)
        for (var x = 0; x < columns - 1; x++)
            verticalSeams[y, x] = CreateSeam(cellSize.y);

        var horizontalSeams = new SeamShape[Mathf.Max(rows - 1, 0), columns];
        for (var y = 0; y < rows - 1; y++)
        for (var x = 0; x < columns; x++)
            horizontalSeams[y, x] = CreateSeam(cellSize.x);

        // The quad must fit the deepest knob, so every piece gets the same margin.
        var margin = 0f;
        foreach (var seam in verticalSeams) margin = Mathf.Max(margin, seam.Depth);
        foreach (var seam in horizontalSeams) margin = Mathf.Max(margin, seam.Depth);
        var quadSize = cellSize + Vector2.one * (margin * 2f);

        // All pieces share the same quad mesh.
        var quadMesh = CreateQuadMesh(quadSize);

        var pieces = new List<PuzzlePieceData>(columns * rows);
        for (var y = 0; y < rows; y++)
        for (var x = 0; x < columns; x++) {
            var centerPosition = new Vector2((x + 0.5f) * cellSize.x, (y + 0.5f) * cellSize.y);

            // Image UV rect covered by the quad (the cell plus its margin).
            var quadMin = new Vector2(x * cellSize.x - margin, y * cellSize.y - margin);
            var quadMax = quadMin + quadSize;
            var uvRegion = Rect.MinMaxRect(
                quadMin.x / imageSize.x,
                quadMin.y / imageSize.y,
                quadMax.x / imageSize.x,
                quadMax.y / imageSize.y
            );

            pieces.Add(
                new() {
                    GridPosition = new(x, y),
                    CenterPosition = centerPosition,
                    CellSize = cellSize,
                    Margin = margin,
                    // The lower/left piece of a seam uses +1 (its outward side), the other piece uses -1.
                    Left = x > 0 ? FromSeam(verticalSeams[y, x - 1], -1) : PuzzleEdgeData.Flat,
                    Right = x < columns - 1 ? FromSeam(verticalSeams[y, x], +1) : PuzzleEdgeData.Flat,
                    Bottom = y > 0 ? FromSeam(horizontalSeams[y - 1, x], -1) : PuzzleEdgeData.Flat,
                    Top = y < rows - 1 ? FromSeam(horizontalSeams[y, x], +1) : PuzzleEdgeData.Flat,
                    Mesh = quadMesh,
                    UVRegion = uvRegion,
                    Bounds = new(
                        new(centerPosition.x, centerPosition.y, 0f),
                        new(cellSize.x, cellSize.y, 0f)
                    )
                });
        }

        return pieces;
    }

    SeamShape CreateSeam(float sideLength) {
        // The knob's width is a fraction of the side it sits on. The circle centre sits
        // offset (0.7-0.9 radii) outside the square's edge, so the knob is more than a semicircle
        // and has a neck. The tip is at Radius + offset from the edge.
        var radius = Mathf.Clamp(BaseSpan + Random.Range(-SpanJitter, SpanJitter), 0.2f, 0.5f) * sideLength * 0.5f;
        var offset = radius * Random.Range(MinOffsetRatio, MaxOffsetRatio);
        return new() {
            Bulge = Random.value < 0.5f ? -1 : 1,
            Center = 0.5f + Random.Range(-CenterJitter, CenterJitter),
            Radius = radius,
            Depth = radius + offset
        };
    }

    static PuzzleEdgeData FromSeam(SeamShape seam, int side) =>
        new() {
            Tab = seam.Bulge * side,
            Center = seam.Center,
            Radius = seam.Radius,
            Depth = seam.Depth
        };

    static Mesh CreateQuadMesh(Vector2 size) {
        var half = size * 0.5f;
        var mesh = new Mesh { name = "PuzzlePieceQuad" };
        // Vertices in clockwise order as seen from the camera (Unity front faces are clockwise).
        mesh.vertices = new[] {
            new Vector3(-half.x, -half.y, 0f), // bottom-left
            new Vector3(-half.x, half.y, 0f), // top-left
            new Vector3(half.x, half.y, 0f), // top-right
            new Vector3(half.x, -half.y, 0f) // bottom-right
        };
        mesh.uv = new[] {
            new Vector2(0f, 0f),
            new Vector2(0f, 1f),
            new Vector2(1f, 1f),
            new Vector2(1f, 0f)
        };
        mesh.triangles = new[] { 0, 1, 2, 0, 2, 3 };
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        return mesh;
    }

    // The shape shared by both pieces on either side of a seam.
    struct SeamShape {
        public int Bulge; // +1: knob points toward the positive axis (right / up). -1: opposite.
        public float Center;
        public float Radius;
        public float Depth;
    }
}