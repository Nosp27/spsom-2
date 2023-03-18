using System;
using System.Collections.Generic;
using GameControl.StateMachine;
using UnityEditor;
using UnityEngine;
using UnityEngine.Animations;


public abstract class BaseQuest : MonoBehaviour
{
    [Serializable]
    class SerializedPair<T, K>
    {
        public T Item1;
        public K Item2;
    }

    public Ship playerShip => GameController.Current.PlayerShip;
    private StateMachine m_QuestStateMachine;
    protected NotificationManager notificationManager;
    protected LocationManager locationManager;
    protected Banner banner;
    [SerializeField] private List<SerializedPair<string, GameObject>> questItemsList;
    [SerializeField] private GameObject questMarker;
    public Dictionary<string, GameObject> questItems { get; private set; }

    protected abstract StateMachine InitStateMachine();
    
    void Start()
    {
        questItems = new Dictionary<string, GameObject>();
        foreach (var item in questItemsList)
        {
            questItems[item.Item1] = item.Item2;
        }
        questItemsList.Clear();
        
        notificationManager = FindObjectOfType<NotificationManager>();
        banner = notificationManager.GetComponent<Banner>();
        locationManager = FindObjectOfType<LocationManager>();
        m_QuestStateMachine = InitStateMachine();
    }
    
    void Update()
    {
        m_QuestStateMachine.Tick();
    }

    protected virtual void Complete()
    {
        banner.Show("Quest Done", 1.7f);
    }
    
    protected virtual void Fail()
    {
        banner.Show("Quest Failed", 1.7f);
    }
    
    void OnDrawGizmosSelected()
    {
        foreach (var item in questItemsList)
        {
            Vector3 position = item.Item2.transform.position;
            Gizmos.color = Color.yellow;
            Handles.color = Color.yellow;
            Gizmos.DrawWireSphere(position, 10);
            Handles.Label(position + Vector3.up * 20, item.Item1);
        }
    }

    protected void SetMarker(Transform target)
    {
        questMarker.GetComponent<PositionTracker>().target = target;
        questMarker.SetActive(target != null);
    }

    protected Transform QuestItemTransform(string item_id)
    {
        GameObject item = questItems[item_id];
        if (item == null)
            return null;
        return item.transform;
    }
}
