using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    static UIManager instance;

    public RectTransform Crosshair;
    public TextMeshProUGUI AmmoText, ReserveText, HealthText, ObjectiveText, ScoreText, HighscoreText, FPSText;
    public float FPSInterval;
    public GameObject[] Guns;
    public GameObject DeathMenu;
    public CanvasGroup ScopeUI;
    public float ScopeAppear;
    public UISlider HealthBar, JetpackFuel;
    public UIFade JetpackFuelFade, ObjectiveFade, HitMarker, CritMarker;
    
    UIClass[] UIClasses;
    float fps;

    void Awake()
    {
        instance = this;
        UIClasses = new UIClass[]{HealthBar, JetpackFuel, JetpackFuelFade, ObjectiveFade, HitMarker, CritMarker};
    }

    void Update()
    {
        if(fps <= 0f)
        {
            FPSText.text = "FPS: " + (1f/Time.unscaledDeltaTime).ToString("0.00");
            fps += FPSInterval;
        }
        fps -= Time.unscaledDeltaTime;

        if(Time.deltaTime == 0f)
            return;

        foreach(UIClass ui in UIClasses)
            ui.Update(Time.deltaTime);
    }
    
    public static void SetHealth(float amount)
    {
        instance.HealthBar.SetValue(amount);
        instance.HealthText.text = ((int)amount).ToString();
    }

    public static void SetAmmo(int amount)
    {
        instance.AmmoText.text = amount.ToString();
    }
    public static void SetReserve(int amount)
    {
        instance.ReserveText.text = amount.ToString();
    }

    public static void SetWeapon(int gun)
    {
        for(int i = 0; i < instance.Guns.Length; i++)
        {
            instance.Guns[i].SetActive(i == gun);
        }
    }

    public static void SetObjectiveText(string text)
    {
        instance.ObjectiveText.text = text;
        instance.ObjectiveFade.SetValue();
    }

    const float crosshairMulti = 2200f;
    public static void SetCrosshairSpread(float amount)
    {
        instance.Crosshair.sizeDelta = Vector2.one * amount * crosshairMulti;
    }

    public static void SetHitMarker(bool crit = false)
    {
        if(crit)
            instance.CritMarker.SetValue();
        else
            instance.HitMarker.SetValue();
        Player.ActivePlayer.PlayHitAudio(crit);
    }

    public static void SetDamageDirection(Vector3 direction)
    {
        DamageMarker damage = DamageMarkerPool.GetObject();
        damage.transform.localPosition = Vector3.zero;
        damage.Activate(direction);
    }

    public static bool scoping;
    public static void SetScope(float amount)
    {
        instance.ScopeUI.alpha = Mathf.InverseLerp(instance.ScopeAppear, 1f, amount);
        scoping = amount >= instance.ScopeAppear;
        instance.Crosshair.gameObject.SetActive(!scoping);
    }

    public static void SetJetpackFuel(float amount, bool updateFade = true)
    {
        instance.JetpackFuel.SetValue(amount);
        if(updateFade)
            instance.JetpackFuelFade.SetValue();
    }

    public static void SetJetpackAirJumpCost(float amount)
    {
        
    }

    public static void SetScore(float score, float highscore, bool setDeath = true)
    {
        instance.ScoreText.text = "Score: " + score.ToString() + "m";
        instance.HighscoreText.text = (score > highscore?"New Highscore!":"Highscore: " + highscore.ToString() + "m");
        SetDeathMenu(setDeath);
    }
    public static void SetDeathMenu(bool active) => instance.DeathMenu.SetActive(active);
}

public abstract class UIClass
{
    public virtual void SetValue(float val = 0f){}
    public virtual void Update(float delta){}
}

[System.Serializable]
public class UISlider : UIClass
{
    public Slider slider;
    public float speed;
    float value;

    public override void SetValue(float val = 0f)
    {
        value = val;
    }
    public override void Update(float delta)
    {
        slider.value = Mathf.MoveTowards(slider.value, value, speed * delta);
    }
}

[System.Serializable]
public class UIFade : UIClass
{
    public CanvasGroup canvasGroup;
    public float maxAlpha = 1f;
    public float uptime, fadeTime;
    float cooldown;

    public override void SetValue(float val = 0f)
    {
        cooldown = uptime + fadeTime;
    }
    public override void Update(float delta)
    {
        canvasGroup.alpha = Mathf.InverseLerp(0f, fadeTime, cooldown) * maxAlpha;
        if(cooldown > 0f)
            cooldown -= delta;
    }
}