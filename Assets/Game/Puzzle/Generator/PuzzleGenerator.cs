using System.Collections.Generic;
using UnityEngine;

// One side of a puzzle piece.
// Shared sides (knob on one piece, socket on its neighbour) carry identical Center/Radius/Depth,
// only Tab is mirrored. Center is measured along the world axis of the side
// (bottom->top for left/right sides, left->right for top/bottom sides), so both pieces agree.
public struct PuzzleEdgeData {
    public int Tab; // +1 knob, -1 socket, 0 flat. Relative to this piece's outward normal.
    public float Center; // 0..1 position of the knob centre along the side.
    public float Radius; // World units. Radius of the knob circle.
    public float Depth; // World units the knob tip protrudes beyond the cell side (Radius + centre offset).

    public static PuzzleEdgeData Flat => new() { Tab = 0, Center = 0.5f, Radius = 0f, Depth = 0f };

    public Vector4 ToVector4() => new(Tab, Center, Radius, Depth);
}

public class PuzzlePieceData {
    public static readonly int CellSizeId = Shader.PropertyToID("_CellSize");
    public static readonly int MarginId = Shader.PropertyToID("_Margin");
    public static readonly int UVRectId = Shader.PropertyToID("_UVRect");
    public static readonly int TopEdgeId = Shader.PropertyToID("_EdgeTop");
    public static readonly int RightEdgeId = Shader.PropertyToID("_EdgeRight");
    public static readonly int BottomEdgeId = Shader.PropertyToID("_EdgeBottom");
    public static readonly int LeftEdgeId = Shader.PropertyToID("_EdgeLeft");
    public PuzzleEdgeData Bottom;
    public Bounds Bounds; // Cell bounds, for colliders.
    public Vector2 CellSize; // Logical cell size, without knobs.
    public Vector2 CenterPosition; // Cell centre in puzzle space (0..imageSize).

    public Vector2Int GridPosition;
    public PuzzleEdgeData Left;
    public float Margin; // Extra space around the cell that the quad covers for knobs.
    public Mesh Mesh; // Quad covering CellSize + 2 * Margin, UVs 0..1.
    public PuzzleEdgeData Right;
    public PuzzleEdgeData Top;
    public Rect UVRegion; // Image UV rect the quad maps to.

    // Sets the shader inputs for this piece. Shader property names are defined here
    // so the shader can match them later.
    public void ApplyTo(MaterialPropertyBlock block) {
        block.SetVector(CellSizeId, CellSize);
        block.SetFloat(MarginId, Margin);
        block.SetVector(UVRectId, new(UVRegion.xMin, UVRegion.yMin, UVRegion.xMax, UVRegion.yMax));
        block.SetVector(TopEdgeId, Top.ToVector4());
        block.SetVector(RightEdgeId, Right.ToVector4());
        block.SetVector(BottomEdgeId, Bottom.ToVector4());
        block.SetVector(LeftEdgeId, Left.ToVector4());
    }
}

public interface IPuzzleGenerator {
    List<PuzzlePieceData> Generate(
        int columns,
        int rows,
        Vector2 imageSize
    );
}