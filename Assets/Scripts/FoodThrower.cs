using TMPro;
using UnityEngine;

public class FoodThrower : MonoBehaviour
{
    [SerializeField] private GameObject throwObject;
    [SerializeField] private PlayerInventory playerInventory;
    [SerializeField] private CharacterController playerController;
    [SerializeField] private float throwForce = 20;


    private void Awake()
    {
        playerInventory = GetComponent<PlayerInventory>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if(playerInventory.amountOfItems == 0)
            {
                return;
            }

            GameObject thrownObject = Instantiate(throwObject, Camera.main.transform.forward + Camera.main.transform.position, Quaternion.identity);
            thrownObject.GetComponentInChildren<TextMeshPro>().text = playerInventory.itemHandTransforms[playerInventory.amountOfItems - 1].GetComponentInChildren<TextMeshPro>().text;
            thrownObject.GetComponentInChildren<TextMeshPro>().color = playerInventory.itemHandTransforms[playerInventory.amountOfItems - 1].GetComponentInChildren<TextMeshPro>().color;
            playerInventory.DispatchOneItem();

            thrownObject.GetComponent<Rigidbody>().AddForce(Camera.main.transform.forward * throwForce , ForceMode.Impulse);
        }
    }
}
