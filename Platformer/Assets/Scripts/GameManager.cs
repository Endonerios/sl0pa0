using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public enum GameState
    {
        None,
        Menu,
        Load,
        Play,
        Pause,
    }

    [SerializeField] GameState CurrentState;
    [SerializeField] int activeRoom;
    [SerializeField] PlayerController PlayerPrefab;

    [SerializeField] int MaxRooms;
    [SerializeField] RoomController[] RoomVars;
    List<RoomController> SpawnedRooms = new List<RoomController>();
    List<int> used_index = new List<int>();
    [SerializeField] RoomController StartRoom;
    [SerializeField] RoomController EndRoom;

    public PlayerController PlayerInstance { get; private set; }
    public static GameManager instance { get; private set; }

    private void Start()
    {
        instance = this;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            switch (CurrentState)
            {
                case GameState.Play:
                    SwitchState(GameState.Pause);
                    break;
                case GameState.Pause:
                    SwitchState(GameState.Play);
                    break;
            }
        }
    }

    public void SwitchState(GameState new_state)
    {
        Debug.Log($"Switching state from {CurrentState} to {new_state}!");
        switch (new_state)
        {
            case GameState.None:
                //UIManager.instance.SwitchState(UIManager.UIState.LevelMenu);
                CurrentState = new_state;
                break;
            case GameState.Menu:
                Time.timeScale = 1f;
                UIManager.instance.SwitchState(UIManager.UIState.LevelMenu);
                //Write code for deletion all spawned objects
                CurrentState = new_state;
                break;
            case GameState.Load:
                UIManager.instance.SwitchState(UIManager.UIState.Load);
                LoadLevel();
                Respawn();
                CurrentState = new_state;

                SwitchState(GameState.Play);
                break;
            case GameState.Play:
                //PlayerInstance.transform.position = SpawnedRooms[activeRoom].spawn.GetComponentInChildren<TriggerZone>().transform.position;
                UIManager.instance.SwitchState(UIManager.UIState.GameUI);
                Time.timeScale = 1f;
                CurrentState = new_state;
                break;
            case GameState.Pause:
                UIManager.instance.SwitchState(UIManager.UIState.GamePause);
                Time.timeScale = 0f;
                CurrentState = new_state;
                break;
        }
        
    }

    public void Respawn()
    {
        if (PlayerInstance == null)
        {
            PlayerInstance = Instantiate(PlayerPrefab);
        }
        else
        {
            PlayerInstance.transform.position = SpawnedRooms[activeRoom].spawn.GetComponentInChildren<TriggerZone>().transform.position;
        }
    }

    public void SpawnTriggered(SpawnPoint new_sp)
    {
        if (new_sp != SpawnedRooms[activeRoom].spawn)
        {
            SpawnedRooms[activeRoom].CloseExit();
            if (CheckLevelComplete())
            {
                LevelComplete();
                return;
            }
            activeRoom++;
        }
    }

    public bool CheckLevelComplete()
    {
        if (activeRoom > MaxRooms)
        {
            return true;
        }
        return false;
    }

    public void LevelComplete()
    {

    }

    public void OnCoinPickUp(Coin coin)
    {
        SpawnedRooms[activeRoom].CoinPickedUp(coin);
    }

    public void ResetLevel()
    {
        foreach (RoomController room in SpawnedRooms)
        {
            room.ResetRoom();
        }
        activeRoom = 0;
        Respawn();
    }



    void SpawnRoom(RoomController? new_room = null)
    {
        int new_index = 0;
        while (new_room == null)
        {
            new_index = Random.Range(0, RoomVars.Length);
            new_room = RoomVars[new_index];
            if (used_index.Count > 0)
            {
                foreach (int index in used_index)
                {
                    if (index == new_index)
                    {
                        new_room = null;
                        break;
                    }
                }
            }
        }
        used_index.Add(new_index);
        SpawnedRooms.Add(PlaceRoom(new_room));
    }

    RoomController PlaceRoom(RoomController new_room)
    {
        if (SpawnedRooms.Count > 0)
        {
            return Instantiate(new_room, SpawnedRooms[SpawnedRooms.Count-1].GetConnectionPoint() + new Vector3(12, 1), Quaternion.identity);
        }
        else
        {
            return Instantiate(new_room, Vector2.zero, Quaternion.identity);
        }
    }

    public void LoadLevel()
    {
        if (activeRoom < 1)
        {
            activeRoom = 0;
        }
        SpawnRoom(StartRoom);
        //PlaceRoom(StartRoom);
        while (MaxRooms > used_index.Count)
        {
            SpawnRoom();
        }
        PlaceRoom(EndRoom);
    }
}
