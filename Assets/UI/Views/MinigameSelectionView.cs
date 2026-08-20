using System;
using System.Collections.Generic;
using UnityEngine.UIElements;

public class MinigameSelectionView {
    //private readonly VisualElement root;
    readonly VisualElement cardContainer;

    readonly List<LevelMapping> cardData = new();
    readonly VisualElement cardGrid;

    //private readonly Button leftArrow;
    //private readonly Button rightArrow;

    readonly VisualTreeAsset cardTemplate;
    readonly List<MinigameCardView> cardViews = new();

    int currentPage;

    public MinigameSelectionView(
        VisualElement root,
        VisualTreeAsset cardTemplate) {
        //this.root = root;
        this.cardTemplate = cardTemplate;

        cardContainer = root.Q<VisualElement>("CardContainer");
        cardGrid = root.Q<VisualElement>("CardGrid");

        //leftArrow = root.Q<Button>("LeftArrow");
        //rightArrow = root.Q<Button>("RightArrow");

        //leftArrow.clicked += PreviousPage;
        //rightArrow.clicked += NextPage;
    }

    public event Action<LevelMapping> CardClicked;

    public void SetCards(LevelMapping[] cards) {
        cardData.Clear();
        cardData.AddRange(cards);

        BuildCards();
    }

    void BuildCards() {
        cardGrid.Clear();
        cardViews.Clear();

        for (var i = 0; i < cardData.Count; i++) {
            var card = new MinigameCardView();

            card.Data = cardData[i];

            card.Clicked += _ => {
                CardClicked?.Invoke(card.Data);
            };

            cardGrid.Add(card);
        }
    }
}