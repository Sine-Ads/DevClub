using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Slider = UnityEngine.UI.Slider;

public class FloatingHealthBar : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private Damage damageScript;
    
    // Update is called once per frame

    private void Start()
    {
        // Get reference to Damage script on parent or same GameObject
        if (damageScript == null)
        {
            damageScript = GetComponentInParent<Damage>();
        }
    }

    void Update()
    {
        // Update health bar to match current health
        if (damageScript != null && slider != null)
        {
            slider.value = damageScript.health;
        }
    }


}


