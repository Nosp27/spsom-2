using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ShipModuleSlot : UISlot
{
    public ModulePylon Pylon { get; private set; }

    [SerializeField] Color EngagedColor;
    [SerializeField] Color FreeColor;
    [SerializeField] Sprite Icon;

    public GameObject ModulePrefab => Pylon.modulePrefab;
    public bool Engaged => Pylon && Pylon.Engaged;
    
    private Image imageComponent;

    private void Start()
    {
        imageComponent = GetComponent<Image>();
        imageComponent.sprite = Icon;
    }

    public override void AddAction(UnityAction a)
    {
        GetComponentInChildren<Button>().onClick.AddListener(a);
    }
    
    public void AttachPylon(ModulePylon pylon)
    {
        if (imageComponent == null)
        {
            Start();
        }

        Pylon = pylon;
        print($"Engaged: {Engaged}");
        imageComponent.color = Engaged ? EngagedColor : FreeColor;
    }

    public override void Highlight(bool enabled)
    {
        Shadow shadow = GetComponent<Shadow>();
        if (shadow != null)
            shadow.enabled = enabled;
    }

    public bool Install(ShipModule module)
    {
        bool result = Pylon.Install(module.gameObject);
        imageComponent.color = Pylon.Engaged ? EngagedColor : FreeColor;
        return result;
    }

    public bool Uninstall()
    {
        bool result = Pylon.Uninstall();
        imageComponent.color = Pylon.Engaged ? EngagedColor : FreeColor;
        return result;
    }
}