using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Money : MonoBehaviour
{
    [Header("Money Main")]
    public int money;

    [Header("Money Components")]
    [SerializeField] private TextMeshProUGUI moneyText;


    public void AddMoney(int amount)
    {
        money += amount;
        moneyText.text =  money.ToString() + "$";
    }


}
