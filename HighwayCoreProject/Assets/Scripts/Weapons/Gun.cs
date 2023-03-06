using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public GunData data;
    public Transform firePoint, muzzleFlash;
    public float flashTime = 0.05f;

    public void Reset()
    {
        muzzleFlash.gameObject.SetActive(false);
    }

    public void ShootGfx()
    {
        if(!UIManager.scoping)
            StartCoroutine(MuzzleFlash());
    }

    IEnumerator MuzzleFlash()
    {
        muzzleFlash.Rotate(Vector3.forward * Random.Range(0f, 360f));
        muzzleFlash.gameObject.SetActive(true);
        yield return new WaitForSeconds(flashTime);
        muzzleFlash.gameObject.SetActive(false);
    }
}
