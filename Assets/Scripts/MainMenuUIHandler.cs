using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenuUIHandler : MonoBehaviour
{
    [SerializeField] private GameObject _mainMenu;
    [SerializeField] private GameObject _mainUI;
    [SerializeField] private GameObject _infoScreen;
    [SerializeField] private GameObject _loadingScreen;
    [SerializeField] private GameObject _loadingProgress;

    [SerializeField] private AudioManager _audioManager;

    public void LoadGameScene()
    {
        StartCoroutine(LoadSceneAsync(1));
    }

    public void QuitGame()
    {
        Application.Quit(0);
    }

    public void ShowInfoScreen()
    {
        _mainUI.SetActive(false);
        _infoScreen.SetActive(true);
    }

    public void HideInfoScreen()
    {
        _mainUI.SetActive(true);
        _infoScreen.SetActive(false);
    }

    IEnumerator LoadSceneAsync(int index)
    {
        _loadingScreen.SetActive(true);
        AsyncOperation load = SceneManager.LoadSceneAsync(index);
        while(!load.isDone)
        {
            _loadingProgress.GetComponent<Slider>().value = load.progress / 0.9f;
            yield return null;
        }
        _loadingScreen.SetActive(false);
    }

    private void Start()
    {
        _audioManager.PlaySound("MenuMusic");
    }
}
