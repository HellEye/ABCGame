using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MinigameSelectionView
{
    public event Action<int> CardClicked;

    //private readonly VisualElement root;
    private readonly VisualElement cardContainer;
    private readonly VisualElement cardGrid;

    //private readonly Button leftArrow;
    //private readonly Button rightArrow;

    private readonly VisualTreeAsset cardTemplate;

    private readonly List<LevelMapping> cardData = new();
    private readonly List<MinigameCardView> cardViews = new();

    private int currentPage;

    public MinigameSelectionView(
        VisualElement root,
        VisualTreeAsset cardTemplate)
    {
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

    public void SetCards(LevelMapping[] cards)
    {
        cardData.Clear();
        cardData.AddRange(cards);

        BuildCards();

        RefreshLayout();
    }

    private void BuildCards()
    {
        var layout = ResponsiveUIManager.Instance.CurrentLayout;
        
        cardGrid.Clear();
        cardViews.Clear();

        for (int i = 0; i < cardData.Count; i++)
        {
            int index = i;

            var card = new MinigameCardView(cardTemplate);

            card.SetData(cardData[index]);

            card.Clicked += _ =>
            {
                CardClicked?.Invoke(index);
            };

            cardViews.Add(card);

            cardGrid.Add(card.Root);
            
            float halfSpacing = layout.spacing * 0.5f;

            card.Root.style.marginLeft = halfSpacing;
            card.Root.style.marginRight = halfSpacing;
            card.Root.style.marginTop = halfSpacing;
            card.Root.style.marginBottom = halfSpacing;
        }
    }

    private void RefreshLayout()
    {
        if (cardContainer.resolvedStyle.width <= 0)
            return;

        var layout = ResponsiveUIManager.Instance.CurrentLayout;

        float availableWidth =
            cardContainer.resolvedStyle.width -
            layout.horizontalMargin * 2;

        float cardWidth =
            ResponsiveUIManager.Instance
                .CalculateCardWidth(availableWidth);

        foreach (var card in cardViews)
        {
            card.ApplyLayout(cardWidth, layout);
        }

        RefreshPage();
    }

    private void RefreshPage()
    {
        var layout = ResponsiveUIManager.Instance.CurrentLayout;

        int cardsPerPage = layout.cardsPerRow * 2;

        int first = currentPage * cardsPerPage;
        int last = first + cardsPerPage;

        for (int i = 0; i < cardViews.Count; i++)
        {
            cardViews[i].Visible =
                i >= first &&
                i < last;
        }

        //leftArrow.SetEnabled(currentPage > 0);
        //rightArrow.SetEnabled(last < cardViews.Count);
    }

    private void NextPage()
    {
        currentPage++;

        RefreshPage();
    }

    private void PreviousPage()
    {
        if (currentPage > 0)
            currentPage--;

        RefreshPage();
    }
}