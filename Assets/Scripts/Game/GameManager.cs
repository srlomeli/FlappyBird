using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static bool hasStarted;
    [SerializeField] private GameObject startPannel;
    [SerializeField] private GameObject gameoverPannel;
    [SerializeField] private GameObject InGamePannel;

    public static Action OnGameStart;
    public static Action OnRestart;

    private void OnEnable()
    {
        PlaneController.OnGameOver += OnDeath;
    }

    private void OnDisable()
    {
        PlaneController.OnGameOver -= OnDeath;
    }

    private void Awake()
    {
        hasStarted = false;
        LoadSceneManager.Check();
        startPannel.SetActive(true);
    }

    private void Update()
    {
        if (!hasStarted &&Input.anyKeyDown)
        {
            AudioManager.PlayClick();

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            hasStarted = true;
            OnGameStart?.Invoke();
            startPannel.SetActive(false);
            InGamePannel.SetActive(true);
        }
    }

    public void OnDeath()
    {
        gameoverPannel.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void OnClickRestart()
    {
        Restart();
    }

    private async void Restart()
    {
        await SceneManager.UnloadSceneAsync(LoadSceneManager.GAME_SCENE);
        hasStarted = false;
        await SceneManager.LoadSceneAsync(LoadSceneManager.GAME_SCENE, LoadSceneMode.Additive);
        WorldGenerator.instance.OnRestart();
    }
}
