using System;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class JigsawPuzzleGenerator : IPuzzleGenerator {
    // Generation controls (can later be wired to settings/inspector data).
    public bool EnableRandomness { get; set; } = true;
    public int RandomSeed { get; set; } = 11;
    public int EdgeDetail { get; set; } = 8;

    // Ratios are relative to min(pieceWidth, pieceHeight).
    public float TabDepthRatio { get; set; } = 0.22f;
    public float TabWidthRatio { get; set; } = 0.46f;
    public float TabRoundnessExponent { get; set; } = 1.1f;

    // Randomness strengths (0 disables that variation).
    public float DepthJitter { get; set; } = 0.20f;
    public float WidthJitter { get; set; } = 0.12f;
    public float CenterJitter { get; set; } = 0.08f;


    public List<PuzzlePieceData> Generate(int columns, int rows, Vector2 imageSize) {
        if (columns <= 0 || rows <= 0)
            throw new ArgumentOutOfRangeException(nameof(columns), "Columns and rows must be > 0.");

        var pieces = new List<PuzzlePieceData>(columns * rows);
        var pieceWidth = imageSize.x / columns;
        var pieceHeight = imageSize.y / rows;

        var plan = BuildConnectionPlan(columns, rows, pieceWidth, pieceHeight);

        for (var y = 0; y < rows; y++)
        for (var x = 0; x < columns; x++) {
            var piece = CreatePiece(x, y, imageSize, pieceWidth, pieceHeight, plan[y, x]);
            pieces.Add(piece);
        }

        return pieces;
    }

    PieceConnections[,] BuildConnectionPlan(int columns, int rows, float pieceWidth, float pieceHeight) {
        var plan = new PieceConnections[rows, columns];

        for (var y = 0; y < rows; y++)
        for (var x = 0; x < columns; x++)
            plan[y, x] = PieceConnections.Flat;

        var baseScale = Mathf.Min(pieceWidth, pieceHeight);
        var baseDepth = Mathf.Clamp(TabDepthRatio, 0.05f, 0.40f) * baseScale;
        var baseWidth = Mathf.Clamp(TabWidthRatio, 0.20f, 0.80f);

        // Create horizontal edges (right/left connections)
        for (var y = 0; y < rows; y++)
        for (var x = 0; x < columns - 1; x++) {
            var edge = CreateEdgeProfile(baseDepth, baseWidth, x, y, true);
            plan[y, x].Right = edge;
            plan[y, x + 1].Left = edge.Inverted();
        }

        // Create vertical edges (top/bottom connections)
        for (var y = 0; y < rows - 1; y++)
        for (var x = 0; x < columns; x++) {
            var edge = CreateEdgeProfile(baseDepth, baseWidth, x, y, false);
            plan[y, x].Top = edge;
            plan[y + 1, x].Bottom = edge.Inverted();
        }

        return plan;
    }

    EdgeProfile CreateEdgeProfile(float baseDepth, float baseWidth, int x, int y, bool isHorizontal) {
        // Create a deterministic random based on position, not sequence
        // Use a hash to seed the random for consistent profiles at the same position
        var seed = (RandomSeed + x * 73856093) ^ (y * 19349663) ^ (isHorizontal ? 1 : 0);
        var random = new Random(seed);

        var tabDirection =
            random.NextDouble() < 0.5 ? -1 : 1;


        var depth = baseDepth * (1f + (EnableRandomness ? RandomRange(random, -DepthJitter, DepthJitter) : 0f));
        var width = baseWidth * (1f + (EnableRandomness ? RandomRange(random, -WidthJitter, WidthJitter) : 0f));
        var centerOffset = EnableRandomness ? RandomRange(random, -CenterJitter, CenterJitter) : 0f;

        return new() {
            Kind = EdgeKind.Connector,
            Direction = tabDirection,
            Depth = Mathf.Max(0.001f, depth),
            Span = Mathf.Clamp(width, 0.18f, 0.82f),
            CenterOffset = Mathf.Clamp(centerOffset, -0.20f, 0.20f),
            RoundnessExponent = Mathf.Clamp(TabRoundnessExponent, 0.6f, 3f),
            ParameterReversed = false
        };
    }

    PuzzlePieceData CreatePiece(
        int gridX,
        int gridY,
        Vector2 imageSize,
        float pieceWidth,
        float pieceHeight,
        PieceConnections connections
    ) {
        var boundary = BuildBoundaryPoints(pieceWidth, pieceHeight, connections);
        EnsureClockwise(boundary);

        // Geometry never changes shape based on outline width - the outline is a
        // purely fragment-shader based effect so it never distorts the texture or
        // moves the piece's silhouette (which must stay fixed for pieces to fit
        // together).
        //
        // Each boundary vertex stores "distance from boundary" = 0. Each fan
        // triangle gets its OWN copy of the center vertex (rather than one shared
        // center), storing the local spoke length (average distance of its two
        // boundary corners to the center). Linearly interpolating this scalar
        // across the triangle gives a smooth, continuous, per-fragment estimate
        // of "how far inward from the edge" a pixel is, in local (world) units -
        // this lets the fragment shader threshold against _OutlineWidth directly,
        // and keeps the border roughly constant width even where tabs/sockets make
        // the spoke length vary a lot around the piece.
        var boundaryCount = boundary.Count;
        var centerStart = boundaryCount;
        var totalVertexCount = boundaryCount * 2;

        var vertices = new Vector3[totalVertexCount];
        var uvs = new Vector2[totalVertexCount];
        var uv2s = new Vector2[totalVertexCount];

        var pieceOrigin = new Vector2(gridX * pieceWidth, gridY * pieceHeight);
        var centerPixelX = pieceOrigin.x + pieceWidth * 0.5f;
        var centerPixelY = pieceOrigin.y + pieceHeight * 0.5f;
        var centerUv = new Vector2(centerPixelX / imageSize.x, centerPixelY / imageSize.y);

        var spokeLengths = new float[boundaryCount];
        for (var i = 0; i < boundaryCount; i++) spokeLengths[i] = boundary[i].magnitude;

        for (var i = 0; i < boundaryCount; i++) {
            var local = boundary[i];

            var pixelX = pieceOrigin.x + local.x + pieceWidth * 0.5f;
            var pixelY = pieceOrigin.y + local.y + pieceHeight * 0.5f;

            // Boundary vertex: fixed position, distance-from-boundary = 0.
            vertices[i] = new(local.x, local.y, 0f);
            uvs[i] = new(pixelX / imageSize.x, pixelY / imageSize.y);
            uv2s[i] = new(0f, 0f);

            // Per-triangle center vertex copy: fixed at the piece center, but
            // carries this triangle's own local spoke length so the outline
            // stays roughly constant width even near tabs/sockets.
            var next = (i + 1) % boundaryCount;
            var spokeLength = (spokeLengths[i] + spokeLengths[next]) * 0.5f;

            var centerIdx = centerStart + i;
            vertices[centerIdx] = Vector3.zero;
            uvs[centerIdx] = centerUv;
            uv2s[centerIdx] = new(spokeLength, 0f);
        }

        var triangles = new int[boundaryCount * 3];
        var t = 0;
        for (var i = 0; i < boundaryCount; i++) {
            var next = (i + 1) % boundaryCount;
            triangles[t++] = i;
            triangles[t++] = next;
            triangles[t++] = centerStart + i;
        }

        var mesh = new Mesh { name = $"JigsawPiece_{gridX}_{gridY}" };
        mesh.vertices = vertices;
        mesh.uv = uvs;
        mesh.uv2 = uv2s;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        var centerPosition = new Vector2((gridX + 0.5f) * pieceWidth, (gridY + 0.5f) * pieceHeight);

        var uvRegion = BuildUvRegion(uvs);
        var localBounds = mesh.bounds;
        var worldBounds = new Bounds(
            new(centerPosition.x + localBounds.center.x, centerPosition.y + localBounds.center.y, 0f),
            localBounds.size
        );

        return new() {
            Mesh = mesh,
            CenterPosition = centerPosition,
            UVRegion = uvRegion,
            Bounds = worldBounds
        };
    }

    List<Vector2> BuildBoundaryPoints(float width, float height, PieceConnections connections) {
        var points = new List<Vector2>(4 + EdgeDetail * 4);

        var bottomLeft = new Vector2(-width * 0.5f, -height * 0.5f);
        var bottomRight = new Vector2(width * 0.5f, -height * 0.5f);
        var topRight = new Vector2(width * 0.5f, height * 0.5f);
        var topLeft = new Vector2(-width * 0.5f, height * 0.5f);

        AddEdgePoints(points, bottomLeft, bottomRight, Vector2.down, connections.Bottom, true, true);
        AddEdgePoints(points, bottomRight, topRight, Vector2.right, connections.Right, false, true);
        AddEdgePoints(points, topRight, topLeft, Vector2.up, connections.Top, false, true);
        // Don't include the end point for the last edge to avoid duplicate vertex at start/end
        AddEdgePoints(points, topLeft, bottomLeft, Vector2.left, connections.Left, false, false);

        return points;
    }

    void AddEdgePoints(List<Vector2> points, Vector2 start, Vector2 end,
        Vector2 outwardNormal, EdgeProfile edge,
        bool includeStart, bool includeEnd) {
        var detail = edge.Kind == EdgeKind.Flat ? 1 : Mathf.Max(2, EdgeDetail);

        for (var i = 0; i <= detail; i++) {
            if (i == 0 && !includeStart) continue;
            if (i == detail && !includeEnd) continue;

            var t = i / (float)detail;
            var basePoint = Vector2.Lerp(start, end, t);
            var offset = edge.GetOffset(t);
            points.Add(basePoint + outwardNormal * offset);
        }
    }

    static void EnsureClockwise(List<Vector2> points) {
        if (SignedArea(points) > 0f) points.Reverse();
    }


    static Rect BuildUvRegion(IReadOnlyList<Vector2> uvs) {
        var minX = float.MaxValue;
        var minY = float.MaxValue;
        var maxX = float.MinValue;
        var maxY = float.MinValue;

        for (var i = 0; i < uvs.Count; i++) {
            var uv = uvs[i];
            minX = Mathf.Min(minX, uv.x);
            minY = Mathf.Min(minY, uv.y);
            maxX = Mathf.Max(maxX, uv.x);
            maxY = Mathf.Max(maxY, uv.y);
        }

        return Rect.MinMaxRect(minX, minY, maxX, maxY);
    }

    static float SignedArea(IReadOnlyList<Vector2> points) {
        var area = 0f;
        for (var i = 0; i < points.Count; i++) {
            var a = points[i];
            var b = points[(i + 1) % points.Count];
            area += a.x * b.y - b.x * a.y;
        }

        return area * 0.5f;
    }


    static float RandomRange(Random random, float min, float max) => min + (float)random.NextDouble() * (max - min);

    internal enum EdgeKind {
        Flat,
        Connector
    }

    internal struct EdgeProfile {
        public EdgeKind Kind;
        public int Direction; // +1 tab, -1 socket (relative to edge outward normal)
        public float Depth;
        public float Span;
        public float CenterOffset;
        public float RoundnessExponent;
        public bool ParameterReversed; // When true, use (1-t) instead of t for adjacent piece traversal

        public float GetOffset(float t) {
            if (Kind == EdgeKind.Flat || Span <= 0f || Depth <= 0f) return 0f;

            // Reverse the parameter if this edge is on an opposite side of an adjacent piece
            if (ParameterReversed) t = 1f - t;

            var center = 0.5f + CenterOffset;
            var halfSpan = Span * 0.5f;
            var from = center - halfSpan;
            var to = center + halfSpan;

            if (t <= from || t >= to) return 0f;

            var u = (t - from) / (to - from);
            var smoothBump = Mathf.Sin(u * Mathf.PI);
            if (!Mathf.Approximately(RoundnessExponent, 1f)) smoothBump = Mathf.Pow(smoothBump, RoundnessExponent);

            return Direction * Depth * smoothBump;
        }

        public EdgeProfile Inverted() =>
            new() {
                Kind = Kind,
                Direction = -Direction,
                Depth = Depth,
                Span = Span,
                CenterOffset = CenterOffset,
                RoundnessExponent = RoundnessExponent,
                ParameterReversed = !ParameterReversed
            };

        public static EdgeProfile Flat =>
            new() {
                Kind = EdgeKind.Flat,
                Direction = 0,
                Depth = 0f,
                Span = 0f,
                CenterOffset = 0f,
                RoundnessExponent = 1f,
                ParameterReversed = false
            };
    }

    internal struct PieceConnections {
        internal EdgeProfile Top;
        internal EdgeProfile Right;
        internal EdgeProfile Bottom;
        internal EdgeProfile Left;

        public static PieceConnections Flat =>
            new() {
                Top = EdgeProfile.Flat,
                Right = EdgeProfile.Flat,
                Bottom = EdgeProfile.Flat,
                Left = EdgeProfile.Flat
            };
    }
}