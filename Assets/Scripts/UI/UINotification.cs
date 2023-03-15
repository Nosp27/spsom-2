using System;
using System.Collections.Generic;
using System.Reflection;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UINotification : MonoBehaviour
{
    [SerializeField] private Text m_MessageBox;
    [SerializeField] private Image m_Icon;
    [SerializeField] private Image m_Border;
    [SerializeField] private RectTransform m_ContentContainer;
    [SerializeField] private Color successColor = Color.green;
    [SerializeField] private Color failedColor = Color.red;

    private bool m_TweenLock;
    private Queue<Func<Tween>> tweenQueue = new Queue<Func<Tween>>();

    private void Start()
    {
        m_ContentContainer.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (!m_TweenLock && tweenQueue.Count > 0)
        {
            m_TweenLock = true;
            tweenQueue.Dequeue().Invoke().onComplete += ReleaseTweenLock;
        }
    }

    public static UINotification Add(NotificationManager manager, string text, Sprite icon = null)
    {
        UINotification notification = Instantiate(manager.notificationPrefab).GetComponent<UINotification>();
        notification.Init(text, icon);
        notification.m_ContentContainer.gameObject.SetActive(false);
        return notification;
    }

    Tween Slide(bool slideIn)
    {
        float leftBorderWorld = m_ContentContainer.position.x - m_ContentContainer.rect.size.x / 2;
        Vector2 currentPosition = m_ContentContainer.localPosition;
        float slideStartDelta = Screen.width - leftBorderWorld;

        Vector3 from = currentPosition + Vector2.right * slideStartDelta;
        Vector3 to = currentPosition;
        if (!slideIn)
        {
            (from, to) = (to, from);
        }

        m_ContentContainer.localPosition = from;
        m_ContentContainer.gameObject.SetActive(true);
        return m_ContentContainer.DOLocalMove(to, 0.6f);
    }

    void ReleaseTweenLock()
    {
        m_TweenLock = false;
    }

    public void Init(String message, Sprite icon = null)
    {
        m_MessageBox.text = message;
        if (icon != null)
        {
            m_Icon.sprite = icon;
            m_Icon.enabled = true;
        }
        else
        {
            // m_Icon.enabled = false;
        }
    }

    public void Bind()
    {
        print("Bind called");
        tweenQueue.Enqueue(() => Slide(true));
    }

    public void MarkDone(bool success)
    {
        tweenQueue.Enqueue(() =>
        {
            var tween = m_Border.DOColor(success ? successColor : failedColor, 0.3f);
            tween.onComplete += () => Slide(false);
            return tween;
        });
    }
}