using System.Collections.Generic;
using UnityEngine;

public class PuzzleManager : MonoBehaviour {
    [SerializeField] Texture2D image;
    [SerializeField] Vector2Int size;
    [SerializeField] Vector2 imageSize;
    [SerializeField] PuzzlePiece piecePrefab;
    [SerializeField] Material pieceMaterial;
    readonly IPuzzleGenerator generator = new JigsawPuzzleGenerator();
    List<(PuzzlePiece piece, PuzzlePieceData data)> pieces;

    void Start() {
        Debug.Assert(size.x > 0, "Puzzle size x must be positive");
        Debug.Assert(size.y > 0, "Puzzle size y must be positive");
        var puzzles = generator.Generate(size.x, size.y, imageSize);
        pieces = new(size.x * size.y);
        pieceMaterial.mainTexture = image;
        foreach (var puzzle in puzzles) {
            var newPiece = Instantiate(piecePrefab, transform);
            newPiece.transform.position = puzzle.CenterPosition - imageSize / 2;
            newPiece.Init(pieceMaterial, puzzle);
            pieces.Add((newPiece, puzzle));
        }
    }
}