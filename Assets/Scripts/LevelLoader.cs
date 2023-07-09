using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    // Enumerator for Level / Scenes: 
    //      0 -> MainMenu
    //      1 -> LinusLevel
    //      2 -> DomaiLevel
    public enum Level { MainMenu, LinusLevel, DomaiLevel }

    // Has the player successfully finished the level?
    public static Dictionary<Level, bool> finishedLevel = new Dictionary<Level, bool>() {
        { Level.LinusLevel, false },
        { Level.DomaiLevel, false },
    };

    // Level Checkmarks
    [SerializeField] GameObject checkMarkLinusLevel;
    [SerializeField] GameObject checkMarkDomaiLevel;

    // Audio
    [SerializeField] AudioSource bgmSrc;
    [SerializeField] AudioSource sfxSrc;

    // Start is called before the first frame update
    void Start()
    {
        // At start the player doesn't finished any level
        checkMarkLinusLevel.gameObject.SetActive(finishedLevel[Level.LinusLevel]);
        checkMarkDomaiLevel.gameObject.SetActive(finishedLevel[Level.DomaiLevel]);
    }

    // Start Game at UI Click in Main Menu.
    // int level = 
    //      1  -> LinusLevel
    //      2  -> DomaiLevel
    // Everthing else will return in a resetting of the game.
    // => Loading Main Menu again.
    public void startLevel(int level) 
    {
        sfxSrc.PlayOneShot(sfxSrc.clip);
        try {
            SceneManager.LoadScene(((Level)level).ToString());
        } catch {
            SceneManager.LoadScene(Level.MainMenu.ToString());
        }
    }

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
}
