using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.Rendering;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameState State;
    public GameState prevState;
    public LightingManager lightingManager;
    public AnimationMovementController animationMovementController;
    public GameObject hallwaySpawn, gymSpawn, classroomSpawn, poolSpawn;
    

    public static event Action<GameState> OnGameStateChanged;

    private void Awake()
    {
        Instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        UpdateGameState(GameState.Pool);
    }

   public void UpdateGameState(GameState newState)
   {
        if (newState <= State) return;

        prevState = State;
        State = newState;

        switch(newState)
        {
            case GameState.MainScreen: 
                break;
            case GameState.PlayerDeath:
                Debug.Log("Here");
                StartCoroutine(HandlePlayerDeath());
                break;
            case GameState.Hallway: 
                break;
            case GameState.Gym: 
                break;
            case GameState.Classroom: 
                break;
            case GameState.Pool: 
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
        }

        OnGameStateChanged?.Invoke(newState);
   }

    IEnumerator HandlePlayerDeath()
    {
        Debug.Log("Here2");
        lightingManager.onDeath();
        yield return new WaitForSeconds(3);
        animationMovementController.respawn(prevState);
        UpdateGameState(prevState);
        lightingManager.onRespawn();
        yield return new WaitForSeconds(3);
    }
}

public enum GameState
{
    MainScreen,
    PlayerDeath,
    Hallway,
    Gym,
    Classroom,
    Pool
}
