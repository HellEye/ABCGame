using System;
using System.Linq;
using UnityEngine.AddressableAssets;
using UnityEngine.UIElements;

[UxmlElement]
public partial class MinigameCardView : VisualElement {
    LevelMapping data;


    public MinigameCardView() {
        var handle = Addressables.LoadAssetAsync<VisualTreeAsset>("MinigameCard.uxml");


        handle.Completed += handle => {
            Root = handle.Result.Instantiate().Children().FirstOrDefault();
            Add(Root);
            var button = Root.Q<Button>("Card");
            AddToClassList("minigame-card-view-root");
            button.clicked += () => Clicked?.Invoke(this);
            button.AddToClassList("click-sound");
        };
        RegisterCallback<DetachFromPanelEvent>(_ => {
            handle.Release();
        });
    }


    public VisualElement Root { get; private set; }

    public LevelMapping Data {
        get => data;
        set {
            data = value;
            SetData(value);
        }
    }

    public event Action<MinigameCardView> Clicked;


    void SetData(LevelMapping data) => dataSource = data;
}