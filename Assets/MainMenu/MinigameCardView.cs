using System;
using System.Linq;
using UnityEngine.AddressableAssets;
using UnityEngine.UIElements;

[UxmlElement]
public partial class MinigameCardView : VisualElement {
    VisualElement corner;
    Image cornerBackground;

    LevelMapping data;
    Image heart;

    Image thumbnail;

    Label title;

    public MinigameCardView() {
        var handle = Addressables.LoadAssetAsync<VisualTreeAsset>("MinigameCard.uxml");


        handle.Completed += handle => {
            Root = handle.Result.Instantiate().Children().FirstOrDefault();
            Add(Root);


            thumbnail = Root.Q<Image>("Thumbnail");
            AddToClassList("minigame-card-view-root");

            corner = Root.Q<VisualElement>("Corner");
            cornerBackground = Root.Q<Image>("CornerBackground");
            heart = Root.Q<Image>("Heart");

            title = Root.Q<Label>("Title");
            Root.RegisterCallback<ClickEvent>(_ => {
                Clicked?.Invoke(this);
            });
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