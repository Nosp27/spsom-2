using System.Collections.Generic;
using GameControl.StateMachine;
using UnityEngine;

public abstract class BaseQuest : MonoBehaviour
{
    private Stack<int> notificationIndices = new Stack<int>();
    private StateMachine m_QuestStateMachine;
    private NotificationManager m_NotificationManager;
    protected LocationManager locationManager;

    protected abstract StateMachine InitStateMachine();
    
    void Start()
    {
        m_NotificationManager = FindObjectOfType<NotificationManager>();
        locationManager = FindObjectOfType<LocationManager>();
        m_QuestStateMachine = InitStateMachine();
    }
    
    void Update()
    {
        m_QuestStateMachine.Tick();
    }

    protected int Notify(string text, bool removeOlderNotifications=true)
    {
        if (removeOlderNotifications && notificationIndices.Count > 0)
        {
            for (int i = 0; i<notificationIndices.Count; i++)
            {
                int index = notificationIndices.Pop();
                m_NotificationManager.RemoveNotification(index, true);
            }
        }

        int idx = m_NotificationManager.AddNotification(text);
        notificationIndices.Push(idx);
        return idx;
    }
    
    protected void PopNotification(bool success)
    {
        print("POP");
        if (notificationIndices.Count == 0)
        {
            print("EMPTY");
            return;
        }

        int index = notificationIndices.Pop();
        m_NotificationManager.RemoveNotification(index, success);
    }

    protected virtual void Complete()
    {
        
    }
    
    protected virtual void Fail()
    {
        
    }
    
    protected class NotificationState : IState
    {
        private string action;
        private string text;
        private BaseQuest quest;
        
        private NotificationState(BaseQuest quest, string action, string text)
        {
            this.action = action;
            this.text = text;
            this.quest = quest;
        }

        public static NotificationState Add(BaseQuest quest, string text)
        {
            return new NotificationState(quest, "SHOW", text);
        }
        
        public static NotificationState Fail(BaseQuest quest)
        {
            return new NotificationState(quest, "FAIL", "");
        }
        
        public static NotificationState Success(BaseQuest quest)
        {
            return new NotificationState(quest, "SUCCESS", "");
        }
        
        public void Tick()
        {
            // Skip
        }

        public void OnEnter()
        {
            if (action == "SHOW")
                quest.Notify(text);
            if (action == "FAIL")
                quest.PopNotification(false);
            if (action == "SUCCESS")
                quest.PopNotification(true);
        }

        public void OnExit()
        {
        }

        public bool Done()
        {
            return true;
        }
    }
}
