using System;
using System.Collections.Generic;
using System.Linq;
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
    }

    public event Action<LevelMapping> CardClicked;

    public void SetCards(LevelMapping[] cards) {
        cardData.Clear();
        cardData.AddRange(Debug.isDebugBuild ? cards : cards.Where(c => !c.WIP));

        BuildCards();
    }

    void BuildCards() {
        cardGrid.Clear();
        cardViews.Clear();

        for (var i = 0; i < cardData.Count; i += 2) {
            // Group cards in pairs for columns
            var container = new VisualElement();
            container.AddToClassList("card-column");
            for (var j = 0; j < 2; j++) {
                if (i + j >= cardData.Count) {
                    // Add a dummy card to fill the space
                    // Only for odd number of cards
                    var dummyCard = new VisualElement();
                    dummyCard.AddToClassList("minigame-card-view-root");
                    container.Add(dummyCard);
                    break;
                }

                var card = new MinigameCardView {
                    Data = cardData[i + j]
                };
                card.Clicked += _ => {
                    CardClicked?.Invoke(card.Data);
                };
                container.Add(card);
            }

            cardGrid.Add(container);
        }
    }
}