using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAim : PlayerBehaviour
{
    public Camera MainCam, WeaponCam;
    public float HeadRotateSpeed;
    public float scopeMulti, fovTime;

    Vector3 direction, headRotateOffset, headTargetOffset;
    float weaponFov, fovRange, fovTarget, fovSpeed;

    float sensitivity{get => player.Settings.settings.sensitivity;}
    float fov{get => player.Settings.settings.fov;}

    public void AimInput(InputAction.CallbackContext ctx)
    {
        Vector2 input = ctx.ReadValue<Vector2>();

        input.x *= sensitivity;
        input.y *= -sensitivity;

        direction += new Vector3(input.y, input.x, 0f) * Time.deltaTime;

        direction.x = Mathf.Clamp(direction.x, -90f, 90f);
    }

    void Start()
    {
        hideCursor();
        weaponFov = WeaponCam.fieldOfView;
        fovSpeed = 1f / fovTime;
        fovTarget = 0f;
        fovRange = fovTarget;
    }

    void OnDisable()
    {
        MainCam.fieldOfView = fov;
        WeaponCam.fieldOfView = weaponFov;
    }

    void Update()
    {
        fovRange = Mathf.MoveTowards(fovRange, fovTarget, fovSpeed * Time.deltaTime);
        MainCam.fieldOfView = Mathf.Lerp(fov, fov * scopeMulti, fovRange);
        WeaponCam.fieldOfView = Mathf.Lerp(weaponFov, weaponFov * scopeMulti, fovRange);

        if(Time.deltaTime == 0f)
            return;

        UIManager.SetScope(fovRange);
        
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

    public void ScopeIn(bool scope, float time = 0f)
    {
        if(time <= 0f)
            time = fovTime;

        fovTarget = (scope?1f:0f);
        fovSpeed = 1f / time;
    }
    
    //public void ChangeWeaponFov(float newFov) => weaponFov = newFov;

    void hideCursor(){
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void showCursor(){
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
    }
    
    public override void Die()
    {
        //ded
        
    }
}
