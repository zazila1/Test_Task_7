using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Soldier : MonoBehaviour, IDamagable
{
    [SerializeField] private SoldierParams _SoldierParams;
    
    public void GetDamage(int damageValue)
    {
        throw new System.NotImplementedException();
    }
}
