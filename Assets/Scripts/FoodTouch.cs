using TMPro;
using UnityEngine;

public class FoodTouch : MonoBehaviour
{
    PlayerInventory inventory;

    private void Awake()
    {
        inventory = GameObject.FindAnyObjectByType<PlayerInventory>();
    }


    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.transform.GetComponent<Food>() != null)
        {
            if (inventory.occupiedHand == false)
            {
                inventory.itemHandTransforms[inventory.amountOfItems].gameObject.SetActive(true);
                inventory.itemHandTransforms[inventory.amountOfItems].GetComponentInChildren<TextMeshPro>().text = hit.transform.GetComponent<Food>().foodName;
                inventory.itemHandTransforms[inventory.amountOfItems].GetComponentInChildren<TextMeshPro>().color = hit.transform.GetComponent<Food>().foodColor;

                Destroy(hit.gameObject);
            }
        }

    }
}
