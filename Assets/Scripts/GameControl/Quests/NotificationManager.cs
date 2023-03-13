using System.Collections.Generic;
using UnityEngine;

public class NotificationManager : MonoBehaviour
{
    [SerializeField] private GameObject notificationPrefab;

    private Dictionary<int, UINotification> notificationRefs = new Dictionary<int, UINotification>();
    private int counter;

    public int AddNotification(string message, Sprite icon = null)
    {
        GameObject notificationGameObject = Instantiate(notificationPrefab, transform);
        UINotification notification = notificationGameObject.GetComponent<UINotification>();
        notificationRefs[++counter] = notification;
        notification.Init(message, icon);
        return counter;
    }
    
    public void RemoveNotification(int index, bool success)
    {
        print($"Remove {index}");
        UINotification notification;
        if (!notificationRefs.TryGetValue(index, out notification))
            return;
        notificationRefs.Remove(index);
        notification.MarkDone(success);
        Destroy(notification.gameObject, 1f);
    }
}
