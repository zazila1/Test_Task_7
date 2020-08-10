using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.AI;

public class SoldierController : MonoBehaviour, IDamagable
{
    [SerializeField] private Team _Team;
    [SerializeField] private SoldierParams _SoldierParams;
    [SerializeField] private LayerMask _EnemyLayerMask;
    
    private LaserVisual _LaserVisual;
    private NavMeshAgent _NavMeshAgent;
    
    private float _SearchRadius = 100f;
    private Collider[] _OverlapResults = new Collider[3];

    private GameObject _CurrentTarget = null;
    private bool _TargetInRange = false;
    private float _PassiveEnemySearchingDelay = 1f;

    private void Awake()
    {
        _LaserVisual = GetComponent<LaserVisual>();
        _NavMeshAgent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        // SetEnemysLayerMask();
        _NavMeshAgent.speed = _SoldierParams._Speed;

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
    
    public IEnumerator WalkAndShoot()
    {
        IDamagable targetDamagable = _CurrentTarget.GetComponent<IDamagable>();
        
        while(_CurrentTarget != null)
        {
            transform.LookAt(_CurrentTarget.transform);
            
            if(Vector3.Distance(transform.position, _CurrentTarget.transform.position) > _SoldierParams._AttackRange)
            {
                _TargetInRange = false;
                _NavMeshAgent.SetDestination(_CurrentTarget.transform.position);
            }
            else
            {
                _TargetInRange = true;
                _NavMeshAgent.SetDestination(transform.position);
            }
        
            if (_TargetInRange)
            {
                _LaserVisual.DrawLaser(_CurrentTarget.transform);
                targetDamagable.GetDamage(_SoldierParams._Damage * Time.deltaTime);
            }
            else
            {
                _LaserVisual.StopLaser();
            }
            
            yield return null;
        }
       

        _LaserVisual.StopLaser();
        StartCoroutine(PassiveSearching());
    }

    public IEnumerator PassiveSearching()
    {
        if (_CurrentTarget == null && SearchClosestEnemy(ref _CurrentTarget))
        {
            StartCoroutine(WalkAndShoot());
        }
        yield return new WaitForSeconds(_PassiveEnemySearchingDelay);
        
        StartCoroutine(PassiveSearching());
    }

    private void Die()
    {
        Destroy(gameObject);
    }

    private bool SearchClosestEnemy(ref GameObject enemy)
    {
        Collider closestEnemy = null;
        float distanceToClosestEnemy = Single.PositiveInfinity;

        int searchedEnemysCount =
        Physics.OverlapSphereNonAlloc(transform.position, _SearchRadius, _OverlapResults, _EnemyLayerMask);
    
        //Debug.Log(gameObject.name + " / _OverlapResults count " + _OverlapResults.Length);
        for (int i = 0; i < searchedEnemysCount; i++)
        {
            if (!_OverlapResults[i].CompareTag("Soldier"))
            {
                continue;
            }
            //Debug.Log(gameObject.name + " / " + transform.position + " / " + _OverlapResults[i].transform.position);
            float distance = Vector3.Distance(transform.position, _OverlapResults[i].transform.position);

            if (distance < distanceToClosestEnemy)
            {
                distanceToClosestEnemy = distance;
                closestEnemy = _OverlapResults[i];
            }
        }
        

        if (closestEnemy == null)
        {
            //Debug.Log(gameObject.name + " / SearchClosestEnemy false");
            return false;
        }
        else
        {
            //Debug.Log(gameObject.name + " / SearchClosestEnemy true");
            enemy = closestEnemy.gameObject;
            return true;
        }
    }

    // private void SetEnemysLayerMask()
    // {
    //     _EnemyLayerMask = LayerMask.NameToLayer(_Team == Team.TEAM_1 ? "Team2" : "Team1");
    // }

}
