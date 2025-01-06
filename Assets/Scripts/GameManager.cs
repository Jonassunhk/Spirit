using System;
using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private string[] storyboard = new string[20];
    public static GameManager Instance;

    public GameState State;
    public GameState prevState;
    public LightingManager lightingManager;
    public UIManager UIManager; 
    public startGameManager startGameManager;
    public Light hallwaySun;
    public AnimationMovementController animationMovementController;
    public GameObject hallwaySpawn, gymSpawn, classroomSpawn, poolSpawn, dialogueLocations;
    bool gameIntro = false;

    public static event Action<GameState> OnGameStateChanged;

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        UpdateGameState(GameState.Classroom);
        animationMovementController.respawn(State);
        storyboard[1] = "I am the spirit of Adrian. I woke up and found myself detached from Adrian¡¯s body. Where is he? Use arrow keys to move me around.";
        storyboard[2] = "My headphones played a gentle tune of whale calls. The backpack filled with schoolwork pained my shoulders. I continued to walk forward, searching and reminiscing.";
        storyboard[3] = "Where is Adrian?¡± I questioned. I must find him as soon as possible. A person cannot live long without his spirit.";
        storyboard[4] = "I see the red lights in front. Red lights are dangerous ¡ª  they roam around school, sucking away the spirit of students.";
        storyboard[5] = "Use the spacebar to jump across the red light.";
        storyboard[6] = "The whale is a magnificent creature. It withstands tremendous pressure and lives in a dark loneliness for most of their life, but humans only see their immense grandeur and question, ¡®How did they get so big?¡¯";
        storyboard[7] = "The platform is too high to cross, but I have to rescue Adrian. There is very little time: the rain intensifies. How do I get across?";
        storyboard[8] = "Will there be enough time? Will I save him? What happens if I fail? I'm afraid. I want to quit, but I don¡¯t have a choice.";
        storyboard[9] = "Another red light. A big one that flickers with a certain pattern. I have to keep going.";
        storyboard[10] = "One false step and I¡¯m back to square one. I feel the pressure building. The music in my headphones no longer soothes me.";
        storyboard[11] = "I cross over the first step. Two more to go. The pressure reached its peak, and the trembling numbed my legs.";
        storyboard[12] = "One more to go. The pressure I feel is both agonizing and enlivening. I wonder how much pressure Adrian had to go through every day.";
        storyboard[13] = "I have reached the end, but Adrian is still missing. I have to continue. It was nice traveling with you, and I¡¯m sure we¡¯ll meet again.";
       
    }

    public void updateDialogue(int number) // dialogue needs to be updated
    {
        UIManager.showStoryboard(storyboard[number]); // call UI Manager to update dialogue
    }


    public void UpdateGameState(GameState newState)
   {
       // if (newState <= State && newState != GameState.MainScreen && newState != GameState.PlayerDeath) return;

        prevState = State;
        State = newState;

        switch(newState)
        {
            case GameState.MainScreen:
                hallwaySun.enabled = true;
                StartCoroutine(HandleMainScreen());
                break;
            case GameState.PlayerDeath:
                Debug.Log("Here");
                StartCoroutine(HandlePlayerDeath());
                break;
            case GameState.Hallway:
                hallwaySun.enabled = true;
                if (!gameIntro)
                {
                    gameIntro = true;
                    StartCoroutine(startGameManager.startGameStartScene());
                }
                break;
            case GameState.Gym:
                hallwaySun.enabled = true;
                break;
            case GameState.Classroom:
                hallwaySun.enabled = false;
                break;
            case GameState.Pool:
                hallwaySun.enabled = false;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
        }

        OnGameStateChanged?.Invoke(newState);
   }

    IEnumerator HandleMainScreen()
    {
        //animationMovementController.respawn(State);
        animationMovementController.canUpdate = false;
        UIManager.ShowGameTitle();
        yield return new WaitForSeconds(6);
        UIManager.RemoveGameTitle();
        animationMovementController.canUpdate = true;
    }
        
    IEnumerator HandlePlayerDeath()
    {
        Debug.Log("Here2");
        StartCoroutine(UIManager.FadeToOpaque(2f));
        yield return new WaitForSeconds(2);
        animationMovementController.respawn(prevState);
        UpdateGameState(prevState);
        StartCoroutine(UIManager.FadeToTransparent(2f));
        yield return new WaitForSeconds(2);
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
