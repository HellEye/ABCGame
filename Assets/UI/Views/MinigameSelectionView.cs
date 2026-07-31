using System;
using System.Collections.Generic;
using UnityEngine;
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

        ResponsiveUIManager.Instance.LayoutChanged += RefreshLayout;
    }

    public event Action<LevelMapping> CardClicked;

    public void SetCards(LevelMapping[] cards, Sprite heartSprite, Sprite cornerSprite) {
        cardData.Clear();
        cardData.AddRange(cards);

        BuildCards(heartSprite, cornerSprite);

        RefreshLayout();
    }

    void BuildCards(Sprite heartSprite, Sprite cornerSprite) {
        var layout = ResponsiveUIManager.Instance.CurrentLayout;

        cardGrid.Clear();
        cardViews.Clear();

        for (var i = 0; i < cardData.Count; i++) {
            var index = i;

            var card = new MinigameCardView();
            card.InitMinigameCardView(cardTemplate);

            card.Data = cardData[index];
            card.SetFrame(cornerSprite, heartSprite);

            card.Clicked += _ => {
                CardClicked?.Invoke(card.Data);
            };

            // cardViews.Add(card);

            cardGrid.Add(card);

            var halfSpacing = layout.spacing * 0.5f;

            card.Root.style.marginLeft = halfSpacing;
            card.Root.style.marginRight = halfSpacing;
            card.Root.style.marginTop = halfSpacing;
            card.Root.style.marginBottom = halfSpacing;
        }
    }

    void RefreshLayout() {
        if (cardContainer.resolvedStyle.width <= 0)
            return;

        var layout = ResponsiveUIManager.Instance.CurrentLayout;

        var availableWidth =
            cardContainer.resolvedStyle.width -
            layout.horizontalMargin * 2;

        var cardWidth =
            ResponsiveUIManager.Instance
                .CalculateCardWidth(availableWidth);

        foreach (var card in cardViews) card.ApplyLayout(cardWidth, layout);

        RefreshPage();
    }

    void RefreshPage() {
        var layout = ResponsiveUIManager.Instance.CurrentLayout;

        var cardsPerPage = layout.cardsPerRow * 2;

        var first = currentPage * cardsPerPage;
        var last = first + cardsPerPage;

        for (var i = 0; i < cardViews.Count; i++)
            cardViews[i].Visible =
                i >= first &&
                i < last;

        //leftArrow.SetEnabled(currentPage > 0);
        //rightArrow.SetEnabled(last < cardViews.Count);
    }

    void NextPage() {
        currentPage++;

        RefreshPage();
    }

    void PreviousPage() {
        if (currentPage > 0)
            currentPage--;

        RefreshPage();
    }
}