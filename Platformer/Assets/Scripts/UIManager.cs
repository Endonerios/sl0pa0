using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public enum UIState
    {
        Load,
        MainMenu,
        LevelMenu,
        Options,
        GameUI,
        GamePause,
        LevelComplete,
    }

    [SerializeField] UIState CurrentState;
    [SerializeField] GameObject load;
    [SerializeField] MainMenu mainMenu;
    [SerializeField] GameObject levelMenu;
    [SerializeField] GameObject options;
    [SerializeField] GameUI gameUI;
    [SerializeField] GameObject gamePause;
    [SerializeField] GameObject levelComplete;
    public static UIManager instance { get; private set; }

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        SwitchState(UIState.MainMenu);
    }

    public void SwitchState(UIState new_state)
    {
        switch (new_state)
        {
            case UIState.Load:
                load.gameObject.SetActive(true);
                mainMenu.gameObject.SetActive(false);
                levelMenu.gameObject.SetActive(false);
                options.gameObject.SetActive(false);
                gameUI.gameObject.SetActive(false);
                gamePause.gameObject.SetActive(false);
                levelComplete.gameObject.SetActive(false);
                break;
            case UIState.MainMenu:
                load.gameObject.SetActive(false);
                mainMenu.gameObject.SetActive(true);
                levelMenu.gameObject.SetActive(false);
                options.gameObject.SetActive(false);
                gameUI.gameObject.SetActive(false);
                gamePause.gameObject.SetActive(false);
                levelComplete.gameObject.SetActive(false);
                break;
            case UIState.LevelMenu:
                load.gameObject.SetActive(false);
                mainMenu.gameObject.SetActive(false);
                levelMenu.gameObject.SetActive(true);
                options.gameObject.SetActive(false);
                gameUI.gameObject.SetActive(false);
                gamePause.gameObject.SetActive(false);
                levelComplete.gameObject.SetActive(false);
                break;
            case UIState.Options:
                load.gameObject.SetActive(false);
                mainMenu.gameObject.SetActive(false);
                levelMenu.gameObject.SetActive(false);
                options.gameObject.SetActive(true);
                gameUI.gameObject.SetActive(false);
                gamePause.gameObject.SetActive(false);
                levelComplete.gameObject.SetActive(false);
                break;
            case UIState.GameUI:
                load.gameObject.SetActive(false);
                mainMenu.gameObject.SetActive(false);
                levelMenu.gameObject.SetActive(false);
                options.gameObject.SetActive(false);
                gameUI.gameObject.SetActive(true);
                gamePause.gameObject.SetActive(false);
                levelComplete.gameObject.SetActive(false);
                break;
            case UIState.GamePause:
                load.gameObject.SetActive(false);
                mainMenu.gameObject.SetActive(false);
                levelMenu.gameObject.SetActive(false);
                options.gameObject.SetActive(false);
                gameUI.gameObject.SetActive(false);
                gamePause.gameObject.SetActive(true);
                levelComplete.gameObject.SetActive(false);
                break;
            case UIState.LevelComplete:
                load.gameObject.SetActive(false);
                mainMenu.gameObject.SetActive(false);
                levelMenu.gameObject.SetActive(false);
                options.gameObject.SetActive(false);
                gameUI.gameObject.SetActive(false);
                gamePause.gameObject.SetActive(false);
                levelComplete.gameObject.SetActive(true);
                break;
        }
        CurrentState = new_state;
    }
}
