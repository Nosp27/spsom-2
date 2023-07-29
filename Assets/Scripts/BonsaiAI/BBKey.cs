using UnityEngine;

namespace BonsaiAI
{
    [CreateAssetMenu(fileName = "BonsaiBBKey", menuName = "Bonsai/Blackboard Key")]
    public class BBKey : ScriptableObject
    {
        public static implicit operator string(BBKey k) => k.name;
    }
}
