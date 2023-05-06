using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HealthUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI currentHP;
    [SerializeField] private Gradient healthGradient;
    [SerializeField] private Image line;

    private PlayerController player;

    private void Awake()
    {
        player = FindObjectOfType<PlayerController>();
        player.OnPlayerDamage += Player_OnPlayerDamage;
    }

    private void Start()
    {
        currentHP.text = player.Health.ToString();
        line.color = healthGradient.Evaluate(player.Health / player.MaxHealth);
    }

    private void Player_OnPlayerDamage(object sender, System.EventArgs e)
    {
        currentHP.text = player.Health.ToString();
        line.color = healthGradient.Evaluate(player.Health / player.MaxHealth);
    }
}
