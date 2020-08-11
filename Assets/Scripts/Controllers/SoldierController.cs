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
    
    private LaserVisualController _LaserVisualController;
    private NavMeshAgent _NavMeshAgent;
    
    private float _SearchRadius = 100f;
    private Collider[] _OverlapResults = new Collider[3];

    private GameObject _CurrentTarget = null;
    private bool _TargetInRange = false;
    private float _PassiveEnemySearchingDelay = 1f;
    private float walkRangeOffset = 0.9f; // подходим ближе на 10% чем радиус атаки

    private bool _isActive = false;

    public Team Team => _Team;

    private void Awake()
    {
        _LaserVisualController = GetComponent<LaserVisualController>();
        _NavMeshAgent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        _NavMeshAgent.speed = _SoldierParams._Speed;

        StartCoroutine(PassiveSearching());
    }

    public void GetDamage(float damageValue)
    {
        _SoldierParams._Health -= damageValue;

        if (_SoldierParams._Health <= 0 && _isActive)
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

            float distance = Vector3.Distance(transform.position, _CurrentTarget.transform.position);
            if(distance >= _SoldierParams._AttackRange * walkRangeOffset)
            {
                _NavMeshAgent.SetDestination(_CurrentTarget.transform.position);
                
                _TargetInRange = false || distance <= _SoldierParams._AttackRange;
            }
            else 
            {
                _TargetInRange = true;
                _NavMeshAgent.SetDestination(transform.position);
            }
        
            if (_TargetInRange)
            {
                _LaserVisualController.DrawLaser(_CurrentTarget.transform);
                targetDamagable.GetDamage(_SoldierParams._Damage * Time.deltaTime);
            }
            else
            {
                _LaserVisualController.StopLaser();
            }
            
            yield return null;
        }
       

        _LaserVisualController.StopLaser();
        StartCoroutine(PassiveSearching());
    }

    public void Activate()
    {
        _isActive = true;
    }

    public IEnumerator PassiveSearching()
    {
        if (_isActive && _CurrentTarget == null && SearchClosestEnemy(ref _CurrentTarget))
        {
            StartCoroutine(WalkAndShoot());
        }
        yield return new WaitForSeconds(_PassiveEnemySearchingDelay);
        
        StartCoroutine(PassiveSearching());
    }

    private void Die()
    {
        _isActive = false;
        StopAllCoroutines();
        
        GameController.Instance.SoldierDied(this);
        Destroy(gameObject);
    }

    private bool SearchClosestEnemy(ref GameObject enemy)
    {
        Collider closestEnemy = null;
        float distanceToClosestEnemy = Single.PositiveInfinity;

        int searchedEnemysCount =
        Physics.OverlapSphereNonAlloc(transform.position, _SearchRadius, _OverlapResults, _EnemyLayerMask);
    
        for (int i = 0; i < searchedEnemysCount; i++)
        {
            if (!_OverlapResults[i].CompareTag("Soldier"))
            {
                continue;
            }
            float distance = Vector3.Distance(transform.position, _OverlapResults[i].transform.position);

            if (distance < distanceToClosestEnemy)
            {
                distanceToClosestEnemy = distance;
                closestEnemy = _OverlapResults[i];
            }
        }
        

        if (closestEnemy == null)
        {
            return false;
        }
        else
        {
            enemy = closestEnemy.gameObject;
            return true;
        }
    }

}
