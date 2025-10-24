using System.Collections;
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

    private Coroutine displayHealthBar;
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
        healthBar.gameObject.SetActive(false);
    }
    private void Update()
    {
        healthBar.gameObject.transform.LookAt(Camera.main.transform);
    }
    void UpdateHealthBar(float healthValue)
    {
        if(healthValue <= 0)
        {
            healthBar.gameObject.SetActive(false);
        }
        else
        {
            healthBar.gameObject.SetActive(true);
        }  
        healthBar.value = healthValue;

        if(displayHealthBar != null)
        {
            StopCoroutine(displayHealthBar);
            displayHealthBar = null;
        }
        displayHealthBar = StartCoroutine(DisplayHealthBar());
    }

    IEnumerator DisplayHealthBar()
    {
        yield return new WaitForSeconds(3f);
        healthBar.gameObject.SetActive(false);
        displayHealthBar = null;
    } 
}