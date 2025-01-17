﻿using System.IO;
using System.Text;
using UnityEngine;
using TMPro;

public class Containers : Interactialbes, IInteractable, ISavable
{
    public UiState Mode;

    public bool FirstOpen = true;

    [SerializeField] private Inventory inventory; 

    public void Awake()
    {
        inventory = GetComponent<Inventory>();
        Mode = inventory.GetMode();
    }

    public override void SetUiOpen()
    {
        StringBuilder sb = new StringBuilder();

        switch (Mode)
        {
            case UiState.Container:
                sb.Append(GlobalValues.InterationKey);
                sb.Append(": ");
                sb.Append(GlobalValues.OpenText);
                sb.Append(' ');
                sb.Append(Name);

                if (inventory.Count == 0)
                {
                    sb.Append(' ');
                    sb.Append(GlobalValues.EmptyText);
                }
                
                PlayerInstructionText.text = sb.ToString();
                break;
            case UiState.Store:
                    sb.Append(GlobalValues.InterationKey);
                    sb.Append(": ");
                    sb.Append(GlobalValues.TalkText);
                    sb.Append(GlobalValues.ToText);
                    sb.Append(Name);

                    PlayerInstructionText.text = sb.ToString();
                break;
        }

        base.SetUiOpen();
    }

    public void Interact(bool State)
    {
        SetPlayerState(State);
        
        switch (Mode)
        {
            case UiState.Container:
                InventoryUi.containerUi.OpenCloseInventory(State);

                InventoryUi.playerUi.OpenCloseInventory(false);
                break;
            case UiState.Store:
                if (gameObject.GetComponent<AIController>().GetDead())
                {
                    Mode = UiState.Container;
                    Interact(State);
                    return;
                }
                if (Player.player.GetMode() == PlayerState.InStore)
                {
                    InventoryUi.containerUi.OpenCloseInventory(State);

                    InventoryUi.playerUi.OpenCloseInventory(false);
                }

                NPCDialogue.npcD.StartDialogue(State);
                break;
        }


        FirstOpen = false;
    }

    public void SetDefaultState(bool state)
    {
        inventory.SetDefaultState(state);
    }

    public bool Save(int id)
    {
        return SaveSystem.SaveContainer(inventory, id);
    }

    public bool Load(int id)
    {
        return LoadContainer(id);
    }

    private bool LoadContainer(int id)
    {
        StringBuilder path = new StringBuilder(Application.persistentDataPath);
        path.Append('/');
        path.Append(WorldStateTracker.Tracker.PlayerName);
        path.Append('/');
        path.Append(WorldStateTracker.Tracker.SaveProfile);
        path.Append(GlobalValues.LevelFolder);
        path.Append(SceneManagerOwn.Manager.SceneName);
        path.Append(GlobalValues.ContainerFolder);
        path.Append('/');
        path.Append(GetComponent<Containers>().Name);
        path.Append(id);

        StringBuilder tempPath = new StringBuilder(path.ToString());
        tempPath.Append(GlobalValues.TempExtension);

        path.Append(GlobalValues.SaveExtension);

        InventoryData data;

        if (File.Exists(tempPath.ToString()))
        {
            data = SaveSystem.LoadContainer(tempPath.ToString());
        }
        else
        {
            data = SaveSystem.LoadContainer(path.ToString());
        }

        if (data == null)
        {
            Debug.Log("containers data equals null");
        }

        inventory.Load(data);
        return true;
    }
}
