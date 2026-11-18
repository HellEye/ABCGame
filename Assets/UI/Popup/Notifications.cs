using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Notification {
    public string message;
    public string title;
}

public class Notifications : MonoBehaviour {
    [SerializeField] UIDocument document;
    readonly Queue<Notification> notifications = new();

    int i;
    Label messageLabel;
    Popup popup;
    Label titleLabel;

    public void Start() {
        var root = document.rootVisualElement;
        popup = root.Q<Popup>("NotificationPopup");
        titleLabel = root.Q<Label>("NotificationTitle");
        messageLabel = root.Q<Label>("NotificationMessage");

        popup.OnClose += () => {
            if (notifications.Count > 0) ShowNotifications();
        };
        root.Q<Button>("NotificationCloseButton").clicked += () => popup.Close();
    }

    void OnNotificationAdded() {
        if (!popup.IsOpen) ShowNotifications();
    }

    void ShowNotifications() {
        if (notifications.Count <= 0) return;
        var notification = notifications.Dequeue();
        titleLabel.text = notification.title;
        messageLabel.text = notification.message;
        popup.Open();
    }

    [ContextMenu("Test Notification")]
    public void TestNotification() =>
        Show(
            new() {
                title = "Test Notification",
                message = $"This is a test notification {i++}."
            });

    public void Show(Notification notification) {
        notifications.Enqueue(notification);
        OnNotificationAdded();
    }

    public void Show(string title, string message) =>
        Show(
            new() {
                title = title,
                message = message
            });
}