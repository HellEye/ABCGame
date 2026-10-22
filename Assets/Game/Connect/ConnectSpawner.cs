using System.Collections.Generic;
using System.Linq;
using Reflex.Attributes;
using UnityEngine;
using Random = UnityEngine.Random;

public class ConnectSpawner : MonoBehaviour {
    [SerializeField] [Range(0f, 1f)] float itemX;
    [SerializeField] [Range(0f, 1f)] float labelX;
    [SerializeField] [Range(0f, 1f)] float spacingRelative;
    [SerializeField] ScreenSizeManager screenSizeManager;
    [SerializeField] [Range(1, 4)] int previewItemCount = 3;

    [Header("Prefabs")]
    [SerializeField] GameObject itemPrefab;

    [SerializeField] GameObject labelPrefab;
    [Inject] ConnectDifficulty difficulty;
    [Inject] ConnectGameManager gameManager;

    void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        var screenYTop = screenSizeManager.FromNormalizedToWorldPos(new(0, 1)).y;
        var screenYBottom = screenSizeManager.FromNormalizedToWorldPos(new(0, 0)).y;
        var itemPos = screenSizeManager.FromNormalizedToWorldPos(new(itemX, 0)).x;
        var labelPos = screenSizeManager.FromNormalizedToWorldPos(new(labelX, 0)).x;

        Gizmos.DrawLine(new(itemPos, screenYBottom, 0), new(itemPos, screenYTop, 0));
        Gizmos.DrawLine(new(labelPos, screenYBottom, 0), new(labelPos, screenYTop, 0));

        Gizmos.color = Color.green;
        var placements = GetItemPlacement(previewItemCount, itemX);
        for (var i = 0; i < previewItemCount; i++) {
            Gizmos.DrawSphere(ToWorld(placements[i]), 0.1f);
            Gizmos.DrawSphere(ToWorld(placements[i] + new Vector2(labelX - itemX, 0)), 0.1f);
        }
    }

    // Returns normalized (0-1 viewport) positions, evenly spaced vertically around the center
    List<Vector2> GetItemPlacement(int itemCount, float normalizedX) {
        var startOffset = -((itemCount - 1) * spacingRelative) / 2f;
        var res = new List<Vector2>(itemCount);
        for (var i = 0; i < itemCount; i++)
            res.Add(new(normalizedX, 0.5f + startOffset + i * spacingRelative));

        return res;
    }

    public (IEnumerable<IElement> fakeItems, IEnumerable<IElement> fakeLabels) PickFakes(IEnumerable<IElement> fakes) {
        Debug.Assert(fakes.Count() % 2 == 0, "Something went wrong with generating items, expecting an even number");
        List<IElement> fakeItems = new();
        List<IElement> fakeLabels = new();
        foreach (var (fake, i) in fakes.Indexed())
            if (i % 2 == 0) fakeItems.Add(fake);
            else fakeLabels.Add(fake);

        return (fakeItems, fakeLabels);
    }

    public void SpawnItems(IEnumerable<IElement> items, IEnumerable<IElement> fakes) {
        var (fakeItems, fakeLabels) = PickFakes(fakes);
        var itemList = items.Concat(fakeItems).ToList();
        var labelList = items.Concat(fakeLabels).ToList();
        var count = difficulty.numberOfItems + difficulty.unconnectedItems;
        Shuffle(itemList);
        CyclicShuffle(labelList);
        Debug.Log("Items " + itemList.Count + " Labels " + labelList.Count);
        var itemPlacements = GetItemPlacement(count, itemX);
        var labelPlacements = GetItemPlacement(count, labelX);
        for (var i = 0; i < count; i++) {
            SpawnItem(itemList[i], itemPlacements[i]);
            SpawnLabel(labelList[i], labelPlacements[i]);
        }
    }

    void SpawnItem(IElement element, Vector2 normalizedPos) {
        var item = Instantiate(itemPrefab, Vector3.zero, Quaternion.identity).GetComponent<Item>();
        item.Initialize(element, normalizedPos);
        gameManager.AddItem(item);
    }

    void SpawnLabel(IElement element, Vector2 normalizedPos) {
        var label = Instantiate(labelPrefab, Vector3.zero, Quaternion.identity);

        var labelRenderer = label.GetComponent<IElementRenderer>();
        if (labelRenderer == null) {
            Debug.LogError($"{labelPrefab.name} requires a component implementing {nameof(IElementRenderer)}", label);
            return;
        }

        var dropZone = label.GetComponent<DropZone>();
        dropZone.Initialize(new List<IElement> { element }, normalizedPos);

        labelRenderer.Initialize(element);
    }

    Vector3 ToWorld(Vector2 normalizedPos) => screenSizeManager.FromNormalizedToWorldPos(normalizedPos);

    static void Shuffle<T>(IList<T> list) {
        for (var i = list.Count - 1; i > 0; i--) {
            var j = Random.Range(0, i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }

    // Sattolo's algorithm: random single-cycle permutation, so no element stays in its original slot
    static void CyclicShuffle<T>(IList<T> list) {
        for (var i = list.Count - 1; i > 0; i--) {
            var j = Random.Range(0, i); // exclusive upper bound guarantees j < i
            (list[i], list[j]) = (list[j], list[i]);
        }
    }
}