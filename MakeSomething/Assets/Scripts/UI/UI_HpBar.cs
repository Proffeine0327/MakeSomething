using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_HpBar : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI current;
    [SerializeField] private TextMeshProUGUI max;
    [SerializeField] private Image bar;

    void Update()
    {
        current.text = Player.currentPlayer.CurrentHp.ToString();
        max.text = Player.currentPlayer.MaxHp.ToString();

        bar.fillAmount = (float)(Player.currentPlayer.CurrentHp) / (float)(Player.currentPlayer.MaxHp);
    }
}
