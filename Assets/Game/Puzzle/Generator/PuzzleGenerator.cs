using System.Collections.Generic;
using UnityEngine;

public class PuzzlePieceData {
    public Mesh Mesh { get; set; }
    public Vector2 CenterPosition { get; set; }
    public Rect UVRegion { get; set; }
    public Bounds Bounds { get; set; }
}

public interface IPuzzleGenerator {
    List<PuzzlePieceData> Generate(
        int columns,
        int rows,
        Vector2 imageSize
    );
}