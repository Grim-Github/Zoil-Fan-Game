using UnityEngine;

public class EnterUIZone : MonoBehaviour
{
    private QMovement playerMovement;
    private void Awake()
    {
        playerMovement = GameObject.FindGameObjectWithTag("Player").GetComponent<QMovement>();  
    }

    public void EnterZone()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        playerMovement.ToggleCameraMovement(false);
    }

    public void ExitZone()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        playerMovement.ToggleCameraMovement(true);
    }
}
