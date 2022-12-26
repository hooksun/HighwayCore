using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    static UIManager instance;

    public TextMeshProUGUI AmmoText, HealthText, ObjectiveText;

    void Awake() => instance = this;

    public static void SetHealth(float amount)
    {

    }

    public static void SetAmmo(int mag, int reserve)
    {

    }

    public static void SetWeapon(int gun)
    {
        
    }

    public static void SetObjectiveText(string text)
    {

    }

    public static void SetCrosshairSpread(float amount)
    {
        
    }

    public static void SetHitMarker(bool crit = false)
    {

    }

    public static void SetDamageDirection(Vector2 direction)
    {
        
    }

    public static void SetJetpackFuel(float amount)
    {

    }

    public static void SetJetpackAirJumpCost(float amount)
    {
        
    }
}
