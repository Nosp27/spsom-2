using GameEventSystem;
using UnityEngine;

public class ScoreCounter : MonoBehaviour
{
    private int score;

    private void Start()
    {
        EventLibrary.shipKills.AddListener(ShipKillsListener);
    }

    private void ShipKillsListener(Ship kills, DamageModel killed)
    {
        if (kills == GameController.Current.PlayerShip)
        {
            score += killed.MaxHealth;
        }
    }
}
