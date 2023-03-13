using UnityEngine;

[RequireComponent(typeof(ColliderTrigger))]
public class QuestLocation : MonoBehaviour
{
    public string LocationName => locationName;
    public ColliderTrigger LocationTrigger => m_LocationTrigger;
    
    [SerializeField] private string locationName;
    
    private ColliderTrigger m_LocationTrigger;

    private void Start()
    {
        m_LocationTrigger = GetComponent<ColliderTrigger>();
        m_LocationTrigger.SetCriteria(ColliderTrigger.CRITERIA_PLAYER_SHIP);
        FindObjectOfType<LocationManager>().Bind(this);
    }
}