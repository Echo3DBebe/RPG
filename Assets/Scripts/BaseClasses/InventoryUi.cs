﻿using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUi : IUi
{
    public static InventoryUi containerUi;
    public static InventoryUi playerUi;

    public InventoryState Mode = InventoryState.AllItems;
    public UiState UiMode;

    public Item FocusedItem;

    public GameObject slot;
    public GameObject inventoryUi;
    public GameObject ActionBar;
    public GameObject AmountUi;
    public GameObject CategoryPrefab;
    public GameObject InventoryBar;
    public GameObject ContainerButton;
    public GameObject PlayerButton;
    public GameObject PlayerCanvas;

    public Transform ItemDetailsLocation;
    public Transform InventroyHolder;

    public Slider AmountBar;

    public List<SlotsActions> Slots;

    public string[] Instructions;

    public Text ItemNameText;
    public Text InstructionText;
    public Text ArmourText;
    public Text GoldText;
    public Text AmountText;
    public Text CarryWeigthText;

    public SlotsActions Focus;

    [SerializeField] private Inventory inventory;

    protected GameObject WeaponsBanner;
    protected GameObject ArmourBanner;
    protected GameObject SpellsBanner;
    protected GameObject RuneBanner;
    protected GameObject PotionsBanner;
    protected GameObject ResourcesBanner;
    protected GameObject MiscBanner;


    private void OnEnable()
    {
        if (UiMode == UiState.Player)
        {
            playerUi = this;
        }
        else
        {
            containerUi = this;
        }
    }

    private void Update()
    {
        if (UiMode == UiState.Player)
        {
            PlayerState playerMode = Player.player.GetMode();

            if (Input.GetButtonDown("E") && Focus != null)
            {
                if (playerMode == PlayerState.InInventoy)
                {
                    StartCoroutine(SetEquipment('E', 0));
                }
                else if (playerMode == PlayerState.InContainer)
                {
                    Player.player.Inventory.TransferItem(Focus.item, (int)AmountBar.value);
                }
            }

            if (Input.GetButtonDown("R") && Focus != null && playerMode == PlayerState.InInventoy)
            {
                StartCoroutine(SetEquipment('R', 0));
            }

        }
        else
        {
            //needs to be updated for if in a store
            //check gold amount vs item value
            /*if (Input.GetButtonDown("E") && Focus != null)
            {
                inventory.TransferItem(FocusedItem, 1);
            }*/
            //logic for moving items from a container or npc when pressing e
            //needs updating
        }
    }

    public void SetInventroy(bool State)
    {
        inventoryUi.SetActive(State);
        InventoryBar.SetActive(State);
        Cursor.visible = State;
        PlayerCanvas.SetActive(!State);
        ActionBar.SetActive(State);

        TurnItemDetailsOff();

        if (State == true)
        {
            CallSetInventory(0);
        }

        if (Focus != null)
        {
            Focus = null;
        }
    }

    public override void Set()
    {
        List<Item> AllItems = new List<Item>();
        int[] StartIds;

        if (UiMode == UiState.Player)
        {
            AllItems = Player.player.Inventory.AllItems;
            StartIds = Player.player.Inventory.StartIds;
        }
        else
        {
            AllItems = Player.player.GetHit().GetComponent<Inventory>().AllItems;
            StartIds = Player.player.GetHit().GetComponent<Inventory>().StartIds;
        }

        int LoopStart = 0;
        int LoopEnd = 0;

        if (InventroyHolder.childCount > 0)
        {
            int children = InventroyHolder.childCount;

            for (int i = 0; i < children; i++)
            {
                Destroy(InventroyHolder.GetChild(i).gameObject);
            }

            Slots.Clear();
        }

        if (UiMode == UiState.Container)
        {
            if (!inventory.gameObject.CompareTag(GlobalValues.ContainerTag))
            {
                GoldText.text = "0";
            }
            else
            {
                GoldText.text = "";
            }
        }

        if (UiMode == UiState.Player)
        {
            GoldText.text = "0";
        }

        InstructionText.text = "";

        if (WeaponsBanner != null)
        {
            Destroy(WeaponsBanner);
            WeaponsBanner = null;
        }

        if (ArmourBanner != null)
        {
            Destroy(ArmourBanner);
            ArmourBanner = null;
        }

        if (SpellsBanner != null)
        {
            Destroy(SpellsBanner);
            SpellsBanner = null;
        }

        if (RuneBanner != null)
        {
            Destroy(RuneBanner);
            RuneBanner = null;
        }

        if (PotionsBanner != null)
        {
            Destroy(PotionsBanner);
            PotionsBanner = null;
        }

        if (ResourcesBanner != null)
        {
            Destroy(ResourcesBanner);
            ResourcesBanner = null;
        }

        if (MiscBanner != null)
        {
            Destroy(MiscBanner);
            MiscBanner = null;
        }

        StringBuilder sb;

        switch (Mode)
        {
            case InventoryState.AllItems:
                LoopStart = 0;
                LoopEnd = AllItems.Count;
                break;
            case InventoryState.Weapons:
                LoopStart = 0;
                LoopEnd = StartIds[0];
                break;
            case InventoryState.Armour:
                LoopStart = StartIds[0];
                LoopEnd = StartIds[1];
                break;
            case InventoryState.Spells:
                LoopStart = StartIds[1];
                LoopEnd = StartIds[2];
                break;
            case InventoryState.Runes:
                LoopStart = StartIds[2];
                LoopEnd = StartIds[3];
                break;
            case InventoryState.Potions:
                LoopStart = StartIds[3];
                LoopEnd = StartIds[4];
                break;
            case InventoryState.Resources:
                LoopStart = StartIds[4];
                LoopEnd = StartIds[5];
                break;
            case InventoryState.Misc:
                LoopStart = StartIds[5];
                LoopEnd = AllItems.Count;
                break;
        }

        for (int i = LoopStart; i < LoopEnd; i++)
        {
            Item Item = AllItems[i].GetComponent<Item>();

            if (AllItems[i].CompareTag(GlobalValues.GoldTag))
            {
                if (UiMode == UiState.Container && Player.player.GetHit().CompareTag(GlobalValues.NPCTag))
                {
                    GoldText.text = Item.Amount.ToString("n0");
                }
                else if (UiMode == UiState.Player)
                {
                    GoldText.text = Item.Amount.ToString("n0");
                }

                if (Player.player.GetMode() == PlayerState.InStore)
                {
                    continue;
                }
            }

            if (AllItems[i].gameObject.GetComponent<IEquipable>() != null &&
                Player.player.GetMode() == PlayerState.InStore)
            {
                if (AllItems[i].GetComponent<IEquipable>().IsEquiped)
                {
                    continue;
                }
            }

            Text MiscText;
            int id;

            switch (AllItems[i].tag)
            {
                case GlobalValues.WeaponTag:
                    if (WeaponsBanner == null)
                    {
                        WeaponsBanner = Instantiate(CategoryPrefab, InventroyHolder);
                        WeaponsBanner.GetComponent<Text>().text = "Weapons\n___________________________________________________";
                    }

                    if (Mode == InventoryState.AllItems || Mode == InventoryState.Weapons)
                    {
                        id = SpawnItemInventorySlot(Item);
                        MiscText = Slots[id].transform.GetChild(2).gameObject.GetComponent<Text>();

                        WeaponHolder Weapon = AllItems[i].GetComponent<WeaponHolder>();

                        sb = new StringBuilder(Weapon.CurrentDurability.ToString("n0"));
                        sb.Append('/');
                        sb.Append(Weapon.MaxDurability.ToString("n0"));

                        MiscText.text = sb.ToString();
                    }
                    break;
                case GlobalValues.ArmourTag:
                    if (ArmourBanner == null)
                    {
                        ArmourBanner = Instantiate(CategoryPrefab, InventroyHolder);
                        ArmourBanner.GetComponent<Text>().text = "Armour\n___________________________________________________";
                    }

                    if (Mode == InventoryState.AllItems || Mode == InventoryState.Armour)
                    {
                        id = SpawnItemInventorySlot(Item);
                        MiscText = Slots[id].transform.GetChild(2).gameObject.GetComponent<Text>();

                        ArmourHolder armour = AllItems[i].GetComponent<ArmourHolder>();

                        sb = new StringBuilder(armour.CurrentDurability.ToString("n0"));
                        sb.Append('/');
                        sb.Append(armour.MaxDurability.ToString("n0"));

                        MiscText.text = sb.ToString();
                    }
                    break;
                case GlobalValues.SpellTag:
                    if (SpellsBanner == null)
                    {
                        SpellsBanner = Instantiate(CategoryPrefab, InventroyHolder);
                        SpellsBanner.GetComponent<Text>().text = "Spells\n___________________________________________________";
                    }

                    if (Mode == InventoryState.AllItems || Mode == InventoryState.Spells)
                    {
                        id = SpawnItemInventorySlot(Item);
                        MiscText = Slots[id].transform.GetChild(2).gameObject.GetComponent<Text>();

                        SpellHolder Spell = AllItems[i].GetComponent<SpellHolder>();

                        //MiscText.text = Spell.ManaCost.ToString("n0");
                    }
                    break;
                case GlobalValues.RuneTag:
                    if (RuneBanner == null)
                    {
                        RuneBanner = Instantiate(CategoryPrefab, InventroyHolder);
                        RuneBanner.GetComponent<Text>().text = "Runes\n___________________________________________________";
                    }

                    if (Mode == InventoryState.AllItems || Mode == InventoryState.Runes)
                    {
                        id = SpawnItemInventorySlot(Item);
                        MiscText = Slots[id].transform.GetChild(2).gameObject.GetComponent<Text>();
                        MiscText.text = "";
                    }

                    break;
                case GlobalValues.PotionTag:
                    if (PotionsBanner == null)
                    {
                        PotionsBanner = Instantiate(CategoryPrefab, InventroyHolder);
                        PotionsBanner.GetComponent<Text>().text = "Potions\n___________________________________________________";
                    }

                    if (Mode == InventoryState.AllItems || Mode == InventoryState.Potions)
                    {
                        id = SpawnItemInventorySlot(Item);
                        MiscText = Slots[id].transform.GetChild(2).gameObject.GetComponent<Text>();
                        MiscText.text = "";
                    }

                    break;
                case GlobalValues.ResourceTag:
                    if (ResourcesBanner == null)
                    {
                        ResourcesBanner = Instantiate(CategoryPrefab, InventroyHolder);
                        ResourcesBanner.GetComponent<Text>().text = "Resources\n___________________________________________________";
                    }

                    if (Mode == InventoryState.AllItems || Mode == InventoryState.Resources)
                    {
                        id = SpawnItemInventorySlot(Item);
                        MiscText = Slots[id].transform.GetChild(2).gameObject.GetComponent<Text>();
                        MiscText.text = "";
                    }
                    break;
                default://Gold | Misc | Keys
                    if (MiscBanner == null)
                    {
                        MiscBanner = Instantiate(CategoryPrefab, InventroyHolder);
                        MiscBanner.GetComponent<Text>().text = "Misc\n___________________________________________________";
                    }

                    if (Mode == InventoryState.AllItems || Mode == InventoryState.Misc)
                    {
                        id = SpawnItemInventorySlot(Item);
                        MiscText = Slots[id].transform.GetChild(2).gameObject.GetComponent<Text>();

                        MiscText.text = "";
                    }
                    break;
            }
        }
    }

    public override void Clear()
    {
        int children = InventroyHolder.childCount;

        if (children == 0)
        {
            return;
        }

        if (!inventoryUi.activeSelf)
        {
            inventoryUi.SetActive(true);
        }

        for (int i = 0; i < children; i++)
        {
            Destroy(InventroyHolder.GetChild(i).gameObject);
        }

        Slots.Clear();

        if (WeaponsBanner != null)
        {
            Destroy(WeaponsBanner);
        }

        if (ArmourBanner != null)
        {
            Destroy(ArmourBanner);
        }

        if (SpellsBanner != null)
        {
            Destroy(SpellsBanner);
        }

        if (ResourcesBanner != null)
        {
            Destroy(ResourcesBanner);
        }

        if (MiscBanner != null)
        {
            Destroy(MiscBanner);
        }

        TurnItemDetailsOff();

        InstructionText.text = "";

        FocusedItem = null;
        Focus = null;

        if (InventoryBar != null)
        {
            InventoryBar.SetActive(false);
        }
    }

    public override void Close()
    {
        SetInventroy(false);
    }

    private int SpawnItemInventorySlot(Item Item)
    {
        StringBuilder WeightString = new StringBuilder();

        int ItemWeight = Item.Weight;
        int BeforeDecimal = ItemWeight / 100;
        int AfterDecimal = ItemWeight - BeforeDecimal * 100;

        if (BeforeDecimal > 999)
        {
            int AfterComma = BeforeDecimal / 1000;

            BeforeDecimal = AfterComma * 1000 - BeforeDecimal;

            WeightString.Append(AfterComma);
            WeightString.Append(',');

            if (BeforeDecimal > 10)
            {
                WeightString.Append("00");
            }
            else if (BeforeDecimal > 100)
            {
                WeightString.Append('0');
            }
        }

        WeightString.Append(BeforeDecimal);

        if (AfterDecimal != 0)
        {
            WeightString.Append('.');

            if (AfterDecimal < 10)
            {
                WeightString.Append('0');
            }

            WeightString.Append(AfterDecimal);
        }

        GameObject NewSlot = Instantiate(this.slot, InventroyHolder);

        NewSlot.GetComponent<Image>().color = Item.Rarity;

        SlotsActions slot = NewSlot.GetComponent<SlotsActions>();
        slot.UI = this;
        slot.item = Item;
        Slots.Add(slot);

        int id = Slots.Count - 1;

        Text NameText = Slots[id].transform.GetChild(0).GetComponent<Text>();
        Text WeightText = Slots[id].transform.GetChild(1).GetComponent<Text>();
        Text MiscText = Slots[id].transform.GetChild(2).GetComponent<Text>();
        Text ValueText = Slots[id].transform.GetChild(3).GetComponent<Text>();

        if (Item.Amount > 1)
        {
            StringBuilder sb = new StringBuilder(Item.Name);
            sb.Append(" (");
            sb.Append(Item.Amount.ToString("n0"));
            sb.Append(")");

            NameText.text = sb.ToString();
        }
        else
        {
            NameText.text = Item.Name;
        }

        WeightText.text = WeightString.ToString();

        ValueText.text = Item.Value.ToString("n0");

        return id;
    }

    public void CallSetInventory(InventoryState state)
    {
        Mode = state;
        Set();

        if (UiMode == UiState.Player)
        {
            if (Player.player.GetMode() != PlayerState.InStore)
            {
                playerUi.SetPlayerEquipedIndicators();
            }

            playerUi.UpDateWeight();
        }
    }

    public void OpenCloseInventory(bool State)
    {
        if (UiMode == UiState.Container)
        {
            if (State)
            {
                inventory = Player.player.GetHit().GetComponent<Inventory>();

                CallSetInventory(0);
            }
            else
            {
                Clear();
            }

            if (Player.player.GetMode() == PlayerState.InStore)
            {
                PlayerButton.transform.GetChild(0).gameObject.GetComponent<Text>().text = "Sell";
                ContainerButton.transform.GetChild(0).gameObject.GetComponent<Text>().text = "Buy";
            }
            else if (Player.player.GetMode() == PlayerState.InContainer)
            {
                PlayerButton.transform.GetChild(0).gameObject.GetComponent<Text>().text = WorldStateTracker.Tracker.PlayerName;
                ContainerButton.transform.GetChild(0).gameObject.GetComponent<Text>().text = inventory.name;
            }
        }

        inventoryUi.SetActive(State);
        ActionBar.SetActive(State);
    }

    public void SetFocus(SlotsActions Slot, int ClickSource, Item item, PlayerState playerMode)
    {
        if (Focus != null && Focus == Slot)
        {
            if (FocusedItem.GetComponent<Item>().Amount >= 4)
            {
                AmountUi.SetActive(true);
                AmountBar.maxValue = FocusedItem.GetComponent<Item>().Amount;
            }
            else
            {
                CallAddItem(ClickSource, 1);
            }

            return;
        }

        //QuestInfoUi.SetActive(false);

        Focus = Slot;
        FocusedItem = item;

        if (UiMode == UiState.Player)
        {
            if (playerMode == PlayerState.InContainer)
            {
                InstructionText.text = Instructions[3];
            }
            else if (playerMode == PlayerState.InStore)
            {
                InstructionText.text = Instructions[5];
            }
            else if (playerMode == PlayerState.InInventoy)
            {
                if (item.GetComponent<IEquipable>() != null)
                {
                    if (item.GetComponent<IEquipable>().IsEquiped)
                    {
                        InstructionText.text = Instructions[1];
                    }
                    else
                    {
                        InstructionText.text = Instructions[0];
                    }
                }
                else if (FocusedItem.CompareTag(GlobalValues.PotionTag))
                {
                    InstructionText.text = Instructions[7];
                }
                else
                {
                    InstructionText.text = Instructions[2];
                }
            }
        }
        else if (UiMode == UiState.Container)
        {
            InstructionText.text = Instructions[4];
        }
        else
        {
            //Store
            InstructionText.text = Instructions[6];
        }

        if (ItemDetailsLocation.childCount != 0)
        {
            Destroy(ItemDetailsLocation.GetChild(0).gameObject);
        }

        Helper.helper.CreateItemDetails(FocusedItem, ItemDetailsLocation);
    }

    public void TurnItemDetailsOff()
    {
        if (ItemDetailsLocation.childCount != 0)
        {
            Destroy(ItemDetailsLocation.GetChild(0).gameObject);
        }
    }

    public void AddAmount()
    {
        AmountBar.value++;
        SetAmountText();
    }

    public void SubAmount()
    {
        AmountBar.value--;
        SetAmountText();
    }

    public void SetAmountText()
    {
        StringBuilder sb = new StringBuilder("How many?");
        sb.Append("\n");
        sb.Append(AmountBar.value);

        AmountText.text = sb.ToString();
    }

    public void ConfirmAmount()
    {
        CallAddItem(0, (int)AmountBar.value);
        CancelAmount();
    }

    public void CancelAmount()
    {
        AmountUi.SetActive(false);
        AmountBar.value = 1;
    }

    public void CallAddItem(int ClickSource, int amount)
    {
        if (UiMode == UiState.Player)
        {
            PlayerState playerMode = Player.player.GetMode();

            if (playerMode == PlayerState.InStore)
            {
                Player.player.Inventory.SellItem(FocusedItem, amount);
                Player.player.CalculateSpeed();
            }
            else if (playerMode == PlayerState.InInventoy)
            {
                if (FocusedItem.GetComponent<IEquipable>() != null)
                {
                    StartCoroutine(SetEquipment('E', ClickSource));
                }
                else if (FocusedItem.CompareTag(GlobalValues.PotionTag))
                {
                    FocusedItem.GetComponent<Consumable>().Action();
                }
            }
            else if (playerMode == PlayerState.InContainer)
            {
                Player.player.Inventory.TransferItem(FocusedItem, amount);
                Player.player.CalculateSpeed();
            }
        }
        else
        {
            if (inventory.Mode == UiState.Container)
            {
                inventory.TransferItem(FocusedItem, amount);
                return;
            }
            else //if (Container.Mode == UiState.Store)
            {
                inventory.Trade(FocusedItem, amount);
                return;
            }
        }
    }

    public IEnumerator SetEquipment(Char KeyTracker, int ClickSource)
    {
        if (KeyTracker == 'E')
        {
            IEquipable item = FocusedItem.GetComponent<IEquipable>();

            Item heldItem;

            if (item is WeaponHolder || item is SpellHolder)
            {
                for (int i = 0; i < 2; i++)
                {
                    heldItem = Player.player.GetHeldItem(i);

                    if (i == ClickSource && heldItem != null &&
                        heldItem != FocusedItem)
                    {
                        Player.player.UnequipItem(heldItem);
                    }
                    else if (i != ClickSource && heldItem == FocusedItem &&
                        heldItem != Player.player.GetHeldItem(1))
                    {
                        Player.player.UnequipItem(FocusedItem);
                    }
                }
            }
            else if (item is ArmourHolder holder)
            {
                int id = (int)holder.ArmourType;

                ArmourHolder armour = Player.player.GetEquipedArmour(id);

                if (armour != null &&
                    FocusedItem != armour.gameObject)
                {
                    Player.player.UnequipItem(armour);
                }
            }

            if (item.IsEquiped)
            {
                Player.player.UnequipItem(FocusedItem);
                goto CleanUpUi;
            }

            Player.player.EquipItem(FocusedItem, ClickSource);

        CleanUpUi:
            yield return new WaitForEndOfFrame();

            Player.player.Inventory.UpdateInventory();

            CallSetInventory(Mode);
            SetPlayerEquipedIndicators();

            Focus = null;

            yield break;
        }
        else if (KeyTracker == 'R')
        {
            Item item = FocusedItem.GetComponent<Item>();

            item.SpawnItem();

            Player.player.Inventory.RemoveItem(FocusedItem, item.Amount, false);
            SetPlayerEquipedIndicators();
            Focus = null;

            yield break;
        }
        else
        {
            Debug.Log("Invalid key " + KeyTracker);
        }
    }

    public void SetPlayerEquipedIndicators()
    {
        List<Item> AllItems = Player.player.Inventory.AllItems;

        if (AllItems.Count == 0 || Slots.Count == 0)
        {
            return;
        }

        for (int i = 0; i < AllItems.Count; i++)
        {
            Item rightHand = Player.player.GetHeldItem(0);
            Item leftHand = Player.player.GetHeldItem(1);

            if (AllItems[i].CompareTag(GlobalValues.WeaponTag) || AllItems[i].CompareTag(GlobalValues.SpellTag))
            {
                if (AllItems[i] == rightHand)
                {
                    if (rightHand == leftHand)
                    {
                        Slots[i].EquipedIndicator.SetActive(true);
                        Slots[i].transform.GetChild(4).GetChild(0).GetComponent<Text>().text = "LR";
                    }
                    else
                    {
                        Slots[i].EquipedIndicator.SetActive(true);
                        Slots[i].transform.GetChild(4).GetChild(0).GetComponent<Text>().text = "R";
                    }
                }
                else if (AllItems[i] == leftHand)
                {
                    Slots[i].EquipedIndicator.SetActive(true);
                    Slots[i].gameObject.transform.GetChild(4).GetChild(0).GetComponent<Text>().text = "L";
                }
                else
                {
                    Slots[i].EquipedIndicator.SetActive(false);
                    Slots[i].transform.GetChild(4).GetChild(0).GetComponent<Text>().text = "";
                }
            }

            if (AllItems[i].CompareTag(GlobalValues.ArmourTag))
            {
                ArmourHolder armour = AllItems[i].GetComponent<ArmourHolder>();

                int ArmourID = (int)armour.ArmourType;

                if (Player.player.GetEquipedArmour(ArmourID) == null)
                {
                    continue;
                }

                if (armour == Player.player.GetEquipedArmour(ArmourID))
                {
                    Slots[i].GetComponent<SlotsActions>().EquipedIndicator.SetActive(true);
                    Slots[i].transform.GetChild(4).GetChild(0).GetComponent<Text>().text = "";
                }
                else
                {
                    Slots[i].GetComponent<SlotsActions>().EquipedIndicator.SetActive(false);
                    Slots[i].transform.GetChild(4).GetChild(0).GetComponent<Text>().text = "";
                }
            }
        }
    }

    public void ChangeView(bool Action)
    {
        if (playerUi.inventoryUi.activeSelf == Action)
        {
            return;
        }

        if (Action == true)
        {
            playerUi.Set();
        }

        playerUi.inventoryUi.SetActive(Action);
        playerUi.InventoryBar.SetActive(Action);

        inventoryUi.SetActive(!Action);

        TurnItemDetailsOff();

        playerUi.TurnItemDetailsOff();

        playerUi.Focus = null;

        Focus = null;
    }

    public void UpDateWeight()
    {
        StringBuilder WeightText = new StringBuilder();

        StringBuilder[] WeightString = new StringBuilder[2];

        for (int i = 0; i < 2; i++)
        {
            int Weight;

            WeightString[i] = new StringBuilder();

            if (i == 0)
            {
                Weight = Player.player.Inventory.CurrentCarryWeight;
            }
            else
            {
                Weight = Player.player.Inventory.MaxCarryWeight;
            }

            int BeforeDecimal = Weight / 100;
            int AfterDecimal = Weight - BeforeDecimal * 100;

            if (BeforeDecimal > 999)
            {
                int AfterComma = BeforeDecimal / 1000;

                BeforeDecimal = AfterComma * 1000 - BeforeDecimal;

                WeightString[i].Append(AfterComma);
                WeightString[i].Append(',');

                if (BeforeDecimal > 10)
                {
                    WeightString[i].Append("00");
                }
                else if (BeforeDecimal > 100)
                {
                    WeightString[i].Append('0');
                }
            }

            WeightString[i].Append(BeforeDecimal);

            if (AfterDecimal != 0)
            {
                WeightString[i].Append('.');

                if (AfterDecimal < 10)
                {
                    WeightString[i].Append("0");
                }

                WeightString[i].Append(AfterDecimal);
            }

            WeightText.Append(WeightString[i].ToString());

            if (i == 0)
            {
                WeightText.Append(" / ");
            }
        }

        CarryWeigthText.text = WeightText.ToString();
    }
}
