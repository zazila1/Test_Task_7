using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public static GameController Instance = null;

    [SerializeField] private UIEndPanel _UiEndPanel;

    [SerializeField] private List<SoldierController> _Team1;
    [SerializeField] private List<SoldierController> _Team2;

    private float _StartTime;
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
            Destroy(gameObject);
    }

    
    private void Start()
    {
        //StartGame();
    }

    public void StartGame()
    {
        foreach (var soldierController in _Team1)
        {
            soldierController.Activate();
        }

        foreach (var soldierController in _Team2)
        {
            soldierController.Activate();
        }

        _StartTime = Time.time;
    }

    public void SoldierDied(SoldierController soldier)
    {
        if (soldier.Team == Team.TEAM_1)
        {
            _Team1.Remove(soldier);
        }
        else if (soldier.Team == Team.TEAM_2)
        {
            _Team2.Remove(soldier);
        }
        
        CheckForGameEnded();
    }

    private void CheckForGameEnded()
    {
        if (_Team1.Count <= 0)
        {
            EndGame(Team.TEAM_2);
        }
        else if (_Team2.Count <= 0)
        {
            EndGame(Team.TEAM_1);
        }
    }

    private void EndGame(Team winner)
    {
        _UiEndPanel.ShowEndGamePanel(winner, Time.time - _StartTime);
    }

    public void RestartScene()
    {
        SceneManager.LoadScene(0);
    }
    
}
