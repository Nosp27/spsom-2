using System;
using UnityEngine;

namespace ShipModules
{
    public class ModulePylon : MonoBehaviour
    {
        public GameObject content { get; private set; }

        public void Install(GameObject module)
        {
            if (content != null)
                throw new Exception("This pylon already has installed a module");
            
            content = module;
            content.transform.SetParent(transform);
            content.transform.localPosition = Vector3.zero;
            content.SetActive(true);
        }
    
        public GameObject Uninstall()
        {
            content.SetActive(false);
            content.transform.SetParent(null);
            GameObject _content = content;
            content = null;
            return _content;
        }
    }
}