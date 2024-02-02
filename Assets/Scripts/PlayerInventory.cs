using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [SerializeField] public Transform[] itemHandTransforms;
    [SerializeField] public bool occupiedHand = false;
    [SerializeField] public int amountOfItems = 0;


    public void DispatchOneItem()
    {
        itemHandTransforms[amountOfItems - 1].gameObject.SetActive(false);
        amountOfItems--;
        occupiedHand = false;
    }

    private void VerifyInventory()
    {
        var amount = 0;
        foreach (var item in itemHandTransforms)
        {
            if(item.gameObject.activeSelf)
            {
                amount++;
            }
        }
        amountOfItems = amount;
        if(amountOfItems == itemHandTransforms.Length)
        {
            occupiedHand = true;
        }
    }

    private void Update()
    {
        VerifyInventory();
    }

}
