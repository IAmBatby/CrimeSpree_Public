using UnityEngine;

public class MouseLook : MonoBehaviour
{
    public float mouseSensitivity = 100f;
    public Transform playerBodyTransform;

    float xRotation = 0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        if (GameManager.Instance.gameStatesToggle != GameManager.GameStates.Paused)
        {
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);

            transform.localRotation = Quaternion.Euler(xRotation, 0f, GameManager.Instance.playerController.camTiltOffset);
            //transform.localRotation = Quaternion.Euler(new Vector3(transform.localRotation.x, transform.localRotation.y, GameManager.Instance.playerController.camTiltOffset));
            playerBodyTransform.Rotate(Vector3.up * mouseX);
        }
    }
}
