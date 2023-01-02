using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerSettings", menuName = "Player/Settings")]
public class PlayerSettings : ScriptableObject
{
    public float sensitivity, fov;
}
