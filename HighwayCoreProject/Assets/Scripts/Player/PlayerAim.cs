using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAim : MonoBehaviour
{
    public Transform cam;
    
    public float sensitivity;

    Vector3 direction;

    public void AimInput(InputAction.CallbackContext ctx)
    {
        hideCursor();

        Vector2 input = ctx.ReadValue<Vector2>();

        input.x *= sensitivity;
        input.y *= -sensitivity;

        direction += new Vector3(input.y, input.x, 0f) * Time.deltaTime;

        direction.x = Mathf.Clamp(direction.x, -90f, 90f);

        transform.rotation = Quaternion.Euler(Vector3.up * direction.y);
        cam.transform.localRotation = Quaternion.Euler(Vector3.right * direction.x);
    }

    void hideCursor(){
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
