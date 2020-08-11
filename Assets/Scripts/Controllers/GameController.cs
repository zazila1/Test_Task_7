using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public static GameController Instance = null;
    
    [SerializeField] private GameObject _Team1Soldiers;
    [SerializeField] private GameObject _Team2Soldiers;

    [SerializeField] private int _Team1AliveCount;
    [SerializeField] private int _Team2AliveCount;

    public int Team1AliveCount
    {
        get => _Team1AliveCount;
        set
        {
            if (_Team1AliveCount - value <= 0)
            {
                _Team1AliveCount = 0;
                EndGame(Team.TEAM_2);
            }
        }
    }
    public int Team2AliveCount
    {
        get => _Team2AliveCount;
        set
        {
            if (_Team2AliveCount - value <= 0)
            {
                _Team2AliveCount = 0;
                EndGame(Team.TEAM_1);
            }
        }
    }
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
            Destroy(gameObject);
    }

    
    private void Start()
    {
        _Team1AliveCount = _Team1Soldiers.transform.childCount;
        _Team2AliveCount = _Team2Soldiers.transform.childCount;
    }

    public void SoldierDied(Team team)
    {
        if (team == Team.TEAM_1)
        {
            _Team1AliveCount--;
        }
        else if (team == Team.TEAM_2)
        {
            _Team2AliveCount--;
        }
    }

    private void EndGame(Team winner)
    {
        
    }

    public void RestartScene()
    {
        SceneManager.LoadScene(0);
    }
    
}
