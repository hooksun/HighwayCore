using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAim : PlayerBehaviour
{
    public float sensitivity, HeadRotateSpeed;

    Vector3 direction, headRotateOffset, headTargetOffset;

    public void AimInput(InputAction.CallbackContext ctx)
    {
        hideCursor();

        Vector2 input = ctx.ReadValue<Vector2>();

        input.x *= sensitivity;
        input.y *= -sensitivity;

        direction += new Vector3(input.y, input.x, 0f) * Time.deltaTime;

        direction.x = Mathf.Clamp(direction.x, -90f, 90f);
    }

    void Update()
    {
        transform.rotation = Quaternion.Euler(Vector3.up * direction.y);
        player.Head.localRotation = Quaternion.Euler(Vector3.right * direction.x);

        headRotateOffset = Vector3.MoveTowards(headRotateOffset, headTargetOffset, HeadRotateSpeed * Time.deltaTime);
        if(headRotateOffset != Vector3.zero)
        {
            player.Head.Rotate(headRotateOffset);
        }
    }

    public void RotateHead(Vector3 offset)
    {
        headTargetOffset = offset;
    }

    void hideCursor(){
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
