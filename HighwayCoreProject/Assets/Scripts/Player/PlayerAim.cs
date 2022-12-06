using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAim : PlayerBehaviour
{
    public Camera MainCam, WeaponCam;
    public float sensitivity, HeadRotateSpeed;
    [Range(30f, 140f)]
    public float fov, scopedFov;
    public float fovTime;

    Vector3 direction, headRotateOffset, headTargetOffset;
    float currFov, targetFov, fovSpeed;

    public void AimInput(InputAction.CallbackContext ctx)
    {
        Vector2 input = ctx.ReadValue<Vector2>();

        input.x *= sensitivity;
        input.y *= -sensitivity;

        direction += new Vector3(input.y, input.x, 0f) * Time.deltaTime;

        direction.x = Mathf.Clamp(direction.x, -90f, 90f);
    }

    void OnEnable()
    {
        hideCursor();
        currFov = fov;
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

        currFov = Mathf.MoveTowards(currFov, targetFov, fovSpeed * Time.deltaTime);
        MainCam.fieldOfView = currFov;
        WeaponCam.fieldOfView = currFov;
    }

    public void RotateHead(Vector3 offset)
    {
        headTargetOffset = offset;
    }

    public void ScopeIn(bool scope)
    {
        targetFov = (scope?scopedFov:fov);
        fovSpeed = (fov - scopedFov) / fovTime;
    }
    

    void hideCursor(){
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
