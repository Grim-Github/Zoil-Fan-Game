using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Money : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI moneyText;
    public int money;

    public void AddMoney(int amount)
    {
        money += amount;
        moneyText.text = "MONEY : " + money.ToString() + "$";
    }


}
