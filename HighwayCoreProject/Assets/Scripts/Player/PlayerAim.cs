using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAim : PlayerBehaviour
{
    public Camera MainCam, WeaponCam;
    public float sensitivity, HeadRotateSpeed;
    public float fov, weaponFov, scopeMulti, fovTime;

    Vector3 direction, headRotateOffset, headTargetOffset;
    float currFov, currWeaponFov, currMulti, fovSpeed, weaponFovSpeed;

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
        fov = MainCam.fieldOfView;
        weaponFov = WeaponCam.fieldOfView;
        currFov = fov;
        currWeaponFov = weaponFov;
        currMulti = 1f;
        fovSpeed = fov * (1 - scopeMulti) / fovTime;
        weaponFovSpeed = weaponFov * (1 - scopeMulti) / fovTime;
    }

    void OnDisable()
    {
        MainCam.fieldOfView = fov;
        WeaponCam.fieldOfView = weaponFov;
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

        currFov = Mathf.MoveTowards(currFov, fov * currMulti, fovSpeed * Time.deltaTime);
        currWeaponFov = Mathf.MoveTowards(currWeaponFov, weaponFov * currMulti, weaponFovSpeed * Time.deltaTime);
        MainCam.fieldOfView = currFov;
        WeaponCam.fieldOfView = currWeaponFov;
    }

    public void RotateHead(Vector3 offset)
    {
        headTargetOffset = offset;
    }

    public void ScopeIn(bool scope)
    {
        currMulti = (scope?scopeMulti:1f);
        fovSpeed = fov * (1 - scopeMulti) / fovTime;
        weaponFovSpeed = weaponFov * (1 - scopeMulti) / fovTime;
    }
    

    void hideCursor(){
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
