using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputManager : MonoBehaviour
{
    //public WeaponSystem weapon // The Compoenent that handles weapon inputs
    public void Fire(InputAction.CallbackContext ctx)
    {
        if(ctx.performed)
            return;
        //weapon.FunctionName(ctx.started); // bool parameter, called twice, when pressed (true) and when let go (false)
    }
    public void SecondaryFire(InputAction.CallbackContext ctx)
    {
        if(ctx.performed)
            return;
        //weapon.FunctionName(ctx.started); // bool parameter, called twice, when pressed (true) and when let go (false)
    }
    public void Reload(InputAction.CallbackContext ctx)
    {
        if(!ctx.started)
            return;
        //weapon.FunctionName(); // called once when pressed
    }
    public void WeaponSlot1(InputAction.CallbackContext ctx)
    {
        if(!ctx.started)
            return;
        //weapon.FunctionName(0); // called once when pressed
    }
    public void WeaponSlot2(InputAction.CallbackContext ctx)
    {
        if(!ctx.started)
            return;
        //weapon.FunctionName(1); // called once when pressed
    }
    public void WeaponSlot3(InputAction.CallbackContext ctx)
    {
        if(!ctx.started)
            return;
        //weapon.FunctionName(2); // called once when pressed
    }
    public void Pickup(InputAction.CallbackContext ctx)
    {
        if(!ctx.started)
            return;
        //weapon.FunctionName(); // called once when pressed
    }
}
