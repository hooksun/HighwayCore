using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputManager : MonoBehaviour
{
    public void Fire(InputAction.CallbackContext ctx)
    {
        if(ctx.performed)
            return;
        //FunctionName(ctx.started); // bool parameter, called twice, when pressed (true) and when let go (false)
    }
    public void SecondaryFire(InputAction.CallbackContext ctx)
    {
        if(ctx.performed)
            return;
        //FunctionName(ctx.started); // bool parameter, called twice, when pressed (true) and when let go (false)
    }
    public void Reload(InputAction.CallbackContext ctx)
    {
        if(!ctx.started)
            return;
        //FunctionName(); // called once when pressed
    }
    public void WeaponSlot1(InputAction.CallbackContext ctx)
    {
        if(!ctx.started)
            return;
        //FunctionName(0); // called once when pressed
    }
    public void WeaponSlot2(InputAction.CallbackContext ctx)
    {
        if(!ctx.started)
            return;
        //FunctionName(1); // called once when pressed
    }
    public void WeaponSlot3(InputAction.CallbackContext ctx)
    {
        if(!ctx.started)
            return;
        //FunctionName(2); // called once when pressed
    }
    public void Pickup(InputAction.CallbackContext ctx)
    {
        if(!ctx.started)
            return;
        //FunctionName(); // called once when pressed
    }
}
