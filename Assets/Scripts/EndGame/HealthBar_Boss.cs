using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar_Boss : MonoBehaviour
{
    [SerializeField] private Slider _slider;
    [SerializeField] private Gradient _gradient;
    [SerializeField] private Image _fill;

    public void SetMaxHealth(int health)
    {
        _slider.maxValue = health;
        _slider.value = health;
        _fill.color = _gradient.Evaluate(1f);
    } 

    public void SetHealth(int health)
    {
        _slider.value = health;
        _fill.color = _gradient.Evaluate(_slider.normalizedValue);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
