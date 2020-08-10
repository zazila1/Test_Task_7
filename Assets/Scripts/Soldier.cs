using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class Soldier : MonoBehaviour, IDamagable
{
    [SerializeField] private Team _Team;
    [SerializeField] private SoldierParams _SoldierParams;
    [SerializeField] private LayerMask _EnemyLayerMask;
    
    private LaserVisual _LaserVisual;
    
    private float _SearchRadius = 100f;
    private Collider[] _OverlapResults = new Collider[6];

    private GameObject _CurrentTarget = null;
    private bool _CanShoot;
    private float _PassiveEnemySearchingDelay = 1f;

    private void Awake()
    {
        _LaserVisual = GetComponent<LaserVisual>();
    }

    private void Start()
    {
        // SetEnemysLayerMask();

        StartCoroutine(PassiveSearching());
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
    
    public IEnumerator Shooting()
    {
        Debug.Log(gameObject.name + " / Shooting");
        IDamagable targetDamagable = _CurrentTarget.GetComponent<IDamagable>();
        while (_CurrentTarget != null)
        {
            transform.LookAt(_CurrentTarget.transform);
            _LaserVisual.DrawLaser(_CurrentTarget.transform);
            targetDamagable.GetDamage(_SoldierParams._Damage * Time.deltaTime);
            yield return null;
        }

        _LaserVisual.StopLaser();
        StartCoroutine(PassiveSearching());
    }

    public IEnumerator PassiveSearching()
    {
        Debug.Log(gameObject.name + " / PassiveSearching");
        
        Debug.Log(gameObject.name + " / " + _CurrentTarget);
        if (_CurrentTarget == null && SearchClosestEnemy(ref _CurrentTarget))
        {
            StartCoroutine(Shooting());
        }
        yield return new WaitForSeconds(_PassiveEnemySearchingDelay);
        
        StartCoroutine(PassiveSearching());
    }

    private void Die()
    {
        Destroy(gameObject);
    }

    [CanBeNull]
    private bool SearchClosestEnemy(ref GameObject enemy)
    {
        Debug.Log(gameObject.name + " / enemy " + enemy);
        Debug.Log(gameObject.name + " / SearchClosestEnemy");
        Collider closestEnemy = null;
        float distanceToClosestEnemy = Single.PositiveInfinity;

        int searchedEnemys =
        Physics.OverlapSphereNonAlloc(transform.position, _SearchRadius, _OverlapResults, _EnemyLayerMask);
    
        Debug.Log(gameObject.name + " / _OverlapResults count " + _OverlapResults.Length);
        for (int i = 0; i < searchedEnemys; i++)
        {
            if (!_OverlapResults[i].CompareTag("Soldier"))
            {
                continue;
            }
            Debug.Log(gameObject.name + " / " + transform.position + " / " + _OverlapResults[i].transform.position);
            float distance = Vector3.Distance(transform.position, _OverlapResults[i].transform.position);

            if (distance < distanceToClosestEnemy)
            {
                distanceToClosestEnemy = distance;
                closestEnemy = _OverlapResults[i];
            }
        }
        

        if (closestEnemy == null)
        {
            Debug.Log(gameObject.name + " / SearchClosestEnemy false");
            return false;
        }
        else
        {
            Debug.Log(gameObject.name + " / SearchClosestEnemy true");
            enemy = closestEnemy.gameObject;
            return true;
        }
    }

    // private void SetEnemysLayerMask()
    // {
    //     _EnemyLayerMask = LayerMask.NameToLayer(_Team == Team.TEAM_1 ? "Team2" : "Team1");
    // }

}
