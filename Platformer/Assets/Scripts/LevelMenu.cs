using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelMenu : MonoBehaviour
{
    public void PlayButtonPressed()
    {
        GameManager.instance.SwitchState(GameManager.GameState.Load);
    }
    public void BackButtonPressed()
    {
        //GameManager.instance.SwitchState(GameManager.GameState.None);
    }
}
