using UnityEngine;

namespace Factions
{
    public class FactionMember : MonoBehaviour
    {
        [SerializeField] private SpsomFaction faction;
        public SpsomFaction Faction => faction;
    }
}
