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
    public GameObject DeathMenu;
    public CanvasGroup ScopeUI;
    public float ScopeAppear;
    public UISlider HealthBar, JetpackFuel;
    public UIFade JetpackFuelFade, ObjectiveFade, HitMarker;
    
    UIClass[] UIClasses;
    float fps;

    void Awake()
    {
        instance = this;
        UIClasses = new UIClass[]{HealthBar, JetpackFuel, JetpackFuelFade, ObjectiveFade, HitMarker};
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
        
    }

    public static void SetObjectiveText(string text)
    {
        instance.ObjectiveText.text = text;
        instance.ObjectiveFade.SetValue();
    }

    const float crosshairMulti = 1400f;
    public static void SetCrosshairSpread(float amount)
    {
        instance.Crosshair.sizeDelta = Vector2.one * amount * crosshairMulti;
    }

    public static void SetHitMarker(bool crit = false)
    {
        instance.HitMarker.SetValue();
        Player.ActivePlayer.PlayHitAudio();
    }

    public static void SetDamageDirection(Vector3 direction)
    {
        DamageMarker damage = DamageMarkerPool.GetObject();
        damage.transform.localPosition = Vector3.zero;
        damage.Activate(direction);
    }

    public static void SetScope(float amount)
    {
        instance.ScopeUI.alpha = Mathf.InverseLerp(instance.ScopeAppear, 1f, amount);
    }

    public static void SetJetpackFuel(float amount)
    {
        instance.JetpackFuel.SetValue(amount);
        instance.JetpackFuelFade.SetValue();
    }

    public static void SetJetpackAirJumpCost(float amount)
    {
        
    }

    public static void SetScore(float score, float highscore, bool setDeath = true)
    {
        instance.ScoreText.text = "Score: " + score.ToString();
        instance.HighscoreText.text = (score > highscore?"New Highscore!":"Highscore: " + highscore.ToString());
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