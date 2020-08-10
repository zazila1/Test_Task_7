using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class Soldier : MonoBehaviour, IDamagable
{
    [SerializeField] private Team _Team;
    [SerializeField] private SoldierParams _SoldierParams;

    private float _SearchRadius = 100f;
    private Collider[] _OverlapResults = new Collider[6];
    private LayerMask _EnemyLayerMask;

    private IDamagable _CurrentTarget;
    private bool _CanShoot;
    
    
    private void Start()
    {
        SetEnemysLayerMask();
    }

    public void GetDamage(float damageValue)
    {
        _SoldierParams._Health -= damageValue;

        if (_SoldierParams._Health <= 0)
        {
            _SoldierParams._Health = 0;
            Die();
        }
    }
    
    public IEnumerator Shoot(IDamagable target)
    {
        while (target != null)
        {
            target.GetDamage(_SoldierParams._Damage * Time.deltaTime);
            yield return null;
        }
    }

    private void Die()
    {
        Destroy(transform);
    }

    [CanBeNull]
    private IDamagable SearchEnemy()
    {
        Collider closestEnemy = null;
        float distanceToClosestEnemy = Single.PositiveInfinity;
        
        if (Physics.OverlapSphereNonAlloc(transform.position, _SearchRadius, _OverlapResults, _EnemyLayerMask) > 0)
        {
            for (int i = 0; i < _OverlapResults.Length; i++)
            {
                float distance = Vector3.Distance(transform.position, _OverlapResults[i].transform.position);

                if (distance < distanceToClosestEnemy)
                {
                    distanceToClosestEnemy = distance;
                    closestEnemy = _OverlapResults[i];
                }
            }
        }

        return closestEnemy != null ? closestEnemy.GetComponent<IDamagable>() : null;
    }

    private void SetEnemysLayerMask()
    {
        if (_Team == Team.TEAM_1)
        {
            _EnemyLayerMask = LayerMask.NameToLayer("Team2");
        }
        else
        {
            _EnemyLayerMask = LayerMask.NameToLayer("Team1");
        }
    }

}
