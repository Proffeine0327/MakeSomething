using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_HpBar : MonoBehaviour
{
    [SerializeField] private Image layer;
    [SerializeField] private Image bar;
    [SerializeField] private BaseEnemy enemy;

    RectTransform rt;

    private void Start() 
    {
        rt = GetComponent<RectTransform>();    
    }

    void Update()
    {
        rt.anchoredPosition = Camera.main.WorldToScreenPoint(enemy.transform.position + (Vector3.down * 1));
        bar.fillAmount =  (float)enemy.CurrentHp / (float)enemy.MaxHp;
    }

    public void SetEnemy(BaseEnemy enemy) => this.enemy = enemy;
}
