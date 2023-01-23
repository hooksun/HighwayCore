using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageMarker : MonoBehaviour
{
    public UIFade fade;

    Vector3 direction;
    public void Activate(Vector3 dir)
    {
        direction = dir;
        transform.rotation = Quaternion.Euler(Vector3.back*Quaternion.LookRotation(Quaternion.Inverse(Player.ActivePlayer.Head.rotation) * direction).eulerAngles.y);
        fade.SetValue();
    }

    void Update()
    {
        fade.Update(Time.deltaTime);
        transform.rotation = Quaternion.Euler(Vector3.back*Quaternion.LookRotation(Quaternion.Inverse(Player.ActivePlayer.Head.rotation) * direction).eulerAngles.y);
        if(fade.canvasGroup.alpha <= 0f)
            gameObject.SetActive(false);
    }
}
