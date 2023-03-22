using UnityEngine;

namespace UI.Inventory
{
    public abstract class AbstractSlotGuiListener : MonoBehaviour
    {
        public abstract void Run();
        public abstract void End();
    }
}