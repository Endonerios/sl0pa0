using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PauseUI : MonoBehaviour
{
    public TMP_Text StatsText;

    public void RespawnPressed()
    {
        GameManager.instance.Respawn();
        GameManager.instance.SwitchState(GameManager.GameState.Play);
    }

    public void ResetPressed()
    {
        GameManager.instance.ResetLevel();
        GameManager.instance.SwitchState(GameManager.GameState.Play);
    }

    public void LevelMenuPressed()
    {
        GameManager.instance.SwitchState(GameManager.GameState.Menu);
    }
}
