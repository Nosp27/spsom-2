using System.Collections.Generic;
using FMODUnity;
using UnityEngine;

public class NotificationManager : MonoBehaviour
{
    public GameObject notificationPrefab;
    [SerializeField] private EventReference successEvent;
    
    [SerializeField] private Transform smallNotificationsContainer;

    private HashSet<UINotification> notificationRefs = new HashSet<UINotification>();
    private int counter;

    public void RegisterNotification(UINotification notification)
    {
        notification.transform.SetParent(smallNotificationsContainer);
        notification.transform.localScale = Vector3.one;
        notificationRefs.Add(notification);
        notification.Bind();
    }
    
    public void RemoveNotification(UINotification notification, bool success)
    {
        if (!notificationRefs.Remove(notification))
            return;
        notification.MarkDone(success);
        Destroy(notification.gameObject, 1f);
        if (success)
            RuntimeManager.PlayOneShot(successEvent);
    }
}
