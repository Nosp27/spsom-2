using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TestUpgradeInstall : MonoBehaviour
{
    [SerializeField] private Ship ship;
    [SerializeField] private GameObject upg;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.vKey.wasPressedThisFrame)
        {
            GameObject upgInstance = Instantiate(upg, ship.transform);
            upgInstance.SetActive(true);
            upgInstance.GetComponent<BaseUpgradeModule>().Install(ship);
        }
    }
}
