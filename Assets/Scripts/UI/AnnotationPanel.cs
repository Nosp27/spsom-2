using GameEventSystem;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class AnnotationPanel : MonoBehaviour
    {
        private bool panelEnabled;
        [SerializeField] private GameObject panelObject;
        [SerializeField] private Text annotationText;
    
        void Start()
        {
            panelObject.SetActive(false);
            panelEnabled = false;
            EventLibrary.cursorHoverTargetChanged.AddListener(HoverTargetChangedHandler);
        }

        void HoverTargetChangedHandler(GameObject newHoverTarget)
        {
            AnnotationTarget target = null;
            bool displayNotification = newHoverTarget != null && newHoverTarget.TryGetComponent(out target);

            if (displayNotification != panelEnabled)
            {
                panelObject.SetActive(displayNotification);
                panelEnabled = displayNotification;
                annotationText.text = target?.Annotation;
            }
        }
    }
}
