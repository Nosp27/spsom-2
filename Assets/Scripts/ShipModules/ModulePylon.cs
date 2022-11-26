using System;
using JetBrains.Annotations;
using UnityEngine;

public class ModulePylon : MonoBehaviour
{
    [SerializeField] private ShipModule m_InstalledModule;
    public ShipModule InstalledModule => m_InstalledModule;

    public GameObject modulePrefab;

    public bool Engaged => (m_InstalledModule != null);

    public bool Install(GameObject module)
    {
        modulePrefab = module;
        m_InstalledModule = Instantiate(
            modulePrefab, transform.position, transform.rotation, transform
        ).GetComponent<ShipModule>();
        InstalledModule.Install();
        return true;
    }

    public bool Uninstall()
    {
        if (!m_InstalledModule)
            return false;
        m_InstalledModule.Uninstall();
        Destroy(m_InstalledModule.gameObject);
        m_InstalledModule = null;
        modulePrefab = null;
        return true;
    }
}