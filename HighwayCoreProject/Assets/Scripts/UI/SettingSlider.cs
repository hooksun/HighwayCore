using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

public class SettingSlider : MonoBehaviour
{
    public Slider slider;
    public TMP_InputField field;
    public GameObject Reset;
    public UnityEvent<float> OnValueChange;
    public float value{get => slider.value;}
    public float DefaultValue{get; set;}

    public void ValueChange(float amount)
    {
        amount = Mathf.Clamp(amount, slider.minValue, slider.maxValue);
        slider.value = amount;
        field.text = value.ToString("n2");
        Reset.SetActive(value != DefaultValue);
        if(OnValueChange != null)
            OnValueChange.Invoke(value);
    }
    public void SliderChange()
    {
        ValueChange(slider.value);
    }
    public void FieldChange()
    {
        float val;
        if(float.TryParse(field.text, out val))
        {
            ValueChange(val);
        }
        field.text = value.ToString();
    }
    public void ResetValue()
    {
        ValueChange(DefaultValue);
    }
}
