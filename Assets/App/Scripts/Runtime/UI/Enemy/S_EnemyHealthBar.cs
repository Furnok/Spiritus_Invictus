using Unity.Behavior;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class S_EnemyHealthBar : MonoBehaviour
{
    //[Header("Settings")]

    [Header("References")]
    [SerializeField] Slider healthBar;
    [SerializeField] S_EnemyHealth S_EnemyHealth;
    [SerializeField]  SSO_EnemyHealth ssoEnemyHealthMax;

    //[Header("Inputs")]

    //[Header("Outputs")]
    private void OnEnable()
    {
        S_EnemyHealth.onUpdateEnemyHealth.AddListener(UpdateHealthBar);
    }
    private void OnDisable()
    {
        S_EnemyHealth.onUpdateEnemyHealth.RemoveListener(UpdateHealthBar);
    }
    private void Start()
    {
        healthBar.maxValue = ssoEnemyHealthMax.Value;
        healthBar.value = ssoEnemyHealthMax.Value;
    }
    private void Update()
    {
        healthBar.gameObject.transform.LookAt(Camera.main.transform);
    }
    void UpdateHealthBar(float healthValue)
    {
        healthBar.value = healthValue;
    }
}