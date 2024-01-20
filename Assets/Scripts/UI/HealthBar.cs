using DG.Tweening;
using GameControl.StateMachine;
using GameEventSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private DamageModel attachedDamageModel;
    [SerializeField] private bool trackPlayerShip;
    [SerializeField] private TextMeshProUGUI HP;
    [SerializeField] private Gradient colorShade;

    [SerializeField] private Image circle;

    private bool isVisible = true;

    private void Start()
    {
        EventLibrary.objectReceivesDamage.AddListener(ReactOnHealthChange);
        EventLibrary.shipReceivesHeal.AddListener(sdm => ReactOnHealthChange(sdm, default));
        if (trackPlayerShip)
            EventLibrary.switchPlayerShip.AddListener(
                (x, y) => ReactOnHealthChange(y.damageModel, null)
            );
    }

    private bool IsRelevant(DamageModel damageModel)
    {
        bool isTrackingPlayerShip =
            attachedDamageModel == null && trackPlayerShip &&
            damageModel == GameController.Current.PlayerShip.damageModel;
        bool isTrackingAttachedShip = damageModel != null && damageModel == attachedDamageModel;
        if (!isTrackingPlayerShip && !isTrackingAttachedShip)
            return false;
        return true;
    }

    private void ReactOnHealthChange(DamageModel damageModel, BulletHitDTO _)
    {
        if (!IsRelevant(damageModel))
            return;
        
        if ((damageModel.Health > 0) != isVisible)
        {
            SwitchVisible(damageModel.Health > 0);
        }
        SetPieAngle(361 * damageModel.Health / damageModel.MaxHealth);
        if (HP != null)
            HP.text = damageModel.Health.ToString();
    }

    private void SetPieAngle(int pieAngle)
    {
        pieAngle = Mathf.Clamp(pieAngle, 0, 361);
        circle.DOFillAmount(pieAngle / 361f, .1f);
        circle.color = colorShade.Evaluate(pieAngle / 361f);
    }

    private void SwitchVisible(bool visible)
    {
        circle.enabled = visible;
        if (HP)
        {
            HP.enabled = visible;
        }

        isVisible = visible;
    }
}