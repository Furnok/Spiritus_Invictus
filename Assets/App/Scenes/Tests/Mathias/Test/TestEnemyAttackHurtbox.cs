using System.Collections.Generic;
using UnityEngine;

public class TestEnemyAttackHutbox : MonoBehaviour, IDamageable
{
    [Header("Settings")]
    [SerializeField] float _delayToRespawn;
    [SerializeField] float _maxHealth;

    float _currentHealth;
    
    [Header("References")]
    //[SerializeField] GameObject _visuals;
    //[SerializeField] GameObject _colliders;
    [SerializeField] List<MeshRenderer> _meshRenderers;
    [SerializeField] CapsuleCollider _hitbox;
    [SerializeField] CapsuleCollider _hurtbox;
    [SerializeField] CapsuleCollider _enemyColisionBox;
    [SerializeField] GameObject _enemyMotor;

    //[Header("Input")]

    [Header("Output")]
    [SerializeField] private RSE_OnEnemyTargetDied rseOnEnemyTargetDied;

    void Awake()
    {
        _currentHealth = _maxHealth;
    }

    public void TakeDamage(float ammount)
    {
        _currentHealth -= ammount;
        if(_currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        //_visuals.SetActive(false);
        //_colliders.SetActive(false);
        rseOnEnemyTargetDied.Call(_enemyMotor);
        _hurtbox.enabled = false;
        _enemyColisionBox.enabled = false;
        _enemyMotor.SetActive(false);
        if (_hitbox != null)
        {
            _hitbox.enabled = false;
        }

        if (_meshRenderers != null && _meshRenderers.Count > 0)
        {
            foreach (var renderer in _meshRenderers)
            {
                renderer.enabled = false;
            }
        }

        StartCoroutine(S_Utils.Delay(_delayToRespawn, () =>
        {
            Respawn();
        }));
    }

    void Respawn()
    {
        //_visuals.SetActive(true);
        //_colliders.SetActive(true);
        _hurtbox.enabled = true;
        _enemyColisionBox.enabled = true;
        _enemyMotor.SetActive(true);

        if (_hitbox != null)
        {
            _hitbox.enabled = true;

        }

        if (_meshRenderers != null && _meshRenderers.Count > 0)
        {
            foreach (var renderer in _meshRenderers)
            {
                renderer.enabled = true;
            }
        }
        _currentHealth = _maxHealth;
    }

}