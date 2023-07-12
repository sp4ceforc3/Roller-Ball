using TMPro;
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic; 
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    // GameObject the Plane can collide of.
    // Ground   = Ground coliders game object
    // Item     = Collactable to refuel.
    // Obstacle = Obstacle and ceiling game object
    // Goal        = Colider that indicates when a level is finished.
    public enum CollisionObjects { Ground, Item, Obstacle, Goal }

    // UI
    [SerializeField] GameObject endScreen;
    [SerializeField] GameObject restartButton;
    [SerializeField] TextMeshProUGUI playTime;
    [SerializeField] TextMeshProUGUI recordTime;
    [SerializeField] TextMeshProUGUI endText;

    // Audio
    [SerializeField] AudioSource sfxSrc;
    [SerializeField] AudioSource bgmSrc;
    [SerializeField] AudioClip winningSound;
    [SerializeField] AudioClip losingSound;
    [SerializeField] AudioClip collectSound;

    // Gamestates
    public enum GameState { Playing, Won, Lose }
    public GameState gameState;

    // Item
    public int cntItems = 0;
    [SerializeField] public int totalCntItems = 2;
    [SerializeField] TextMeshProUGUI cntItemsText;

    // Levelname
    private LevelLoader.Level levelName;

    // GameObjects in World with a Collider2D component
    private Collider2D[] colliders;

    // PlayTime in Seconds
    private string playTimeS;

    // Start is called before the first frame update
    void Start()
    {
        gameState = GameState.Playing;
        // levelName = name of current scene
        if (SceneManager.GetActiveScene().name != "LevelTemplate")
            levelName = (LevelLoader.Level)System.Enum.Parse(typeof(LevelLoader.Level), SceneManager.GetActiveScene().name);
        float record = PlayerPrefs.GetFloat("highscore"+SceneManager.GetActiveScene().name);
        if (record != 0.00)
            recordTime.text = "Record: "+record.ToString("0.00")+"s";
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.R))
            MainMenu();
        if(Input.GetKeyDown(KeyCode.Space) && (gameState != GameState.Playing))
            RestartGame();
        if (gameState == GameState.Playing) {
            playTimeS = Time.timeSinceLevelLoad.ToString("0.00");  
            playTime.text = "Time: "+playTimeS+"s";
        }
    }

    // Stop the game and show endscreen UI
    public void LoadEndScreen(GameState state) 
    {
        gameState = state;
        bgmSrc.mute = true;
        
        if (state == GameState.Won) {
            endText.text = "You win!";
            sfxSrc.PlayOneShot(winningSound);
            SetHighScore();
            LevelLoader.finishedLevel[levelName] = true;
        } else {
            endText.text = "You lose!";
            sfxSrc.PlayOneShot(losingSound);
            restartButton.SetActive(true);
        }
        
        endScreen.SetActive(true);

        // deactivate all colliders beside ground
        GameObject world = GameObject.Find("World");
        colliders = world.GetComponentsInChildren<Collider2D>();
        foreach (Collider2D collider in colliders) {
            if (collider.gameObject.tag != nameof(CollisionObjects.Ground))
                collider.enabled = false;
        }
    }

    // Handle Item collection
    public void HandleItem(GameObject item) 
    {
        sfxSrc.PlayOneShot(collectSound);
        cntItems += 1;
        cntItemsText.text = $"Collected Items: {cntItems.ToString()}";
        if (totalCntItems == cntItems)
            cntItemsText.color = new Color(0, 255, 0, 255);
        Destroy(item);
    }

    // Restart Gane = Reset Scene
    public void RestartGame() => SceneManager.LoadScene(SceneManager.GetActiveScene().name);

    // Main Menu
    public void MainMenu() => SceneManager.LoadScene("MainMenu");

    // Quit Completly
    // Quit the game in Unity Editor too:
    /*    https://stackoverflow.com/questions/70437401/cannot-finish-the-game-in-unity-using-application-quit 
          https://answers.unity.com/questions/161858/startstop-playmode-from-editor-script.html
    */
    public void Quit() 
    {
        #if UNITY_STANDALONE
            Application.Quit();
        #endif
        #if UNITY_EDITOR
            // UnityEditor.EditorisPlaying = false; // newer version than 2021.3
            UnityEditor.EditorApplication.isPlaying = false; // for version 2021.3
        #endif
    }

    private void SetHighScore() {
        float record = PlayerPrefs.GetFloat("highscore"+SceneManager.GetActiveScene().name);
        float current = float.Parse(playTimeS);
        if (record > current || record == 0.00)
            PlayerPrefs.SetFloat("highscore"+SceneManager.GetActiveScene().name, current);
    }
}
