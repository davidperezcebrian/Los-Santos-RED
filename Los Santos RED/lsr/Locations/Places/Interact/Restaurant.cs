﻿using LosSantosRED.lsr.Interface;
using Rage;
using RAGENativeUI.Elements;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

public class Restaurant : InteractableLocation
{
    private LocationCamera StoreCamera;
    private IActivityPerformable Player;
    private IModItems ModItems;
    private IEntityProvideable World;
    private ISettingsProvideable Settings;
    private IWeapons Weapons;
    private ITimeControllable Time;
    private Transaction Transaction;
    public Restaurant() : base()
    {

    }

    //[XmlIgnore]
    //public ShopMenu Menu { get; set; }
    //public string MenuID { get; set; }


    public FoodType FoodType { get; set; } = FoodType.Generic;

    public override string TypeName { get; set; } = "Restaurant";
    public override int MapIcon { get; set; } = 621;
    public override Color MapIconColor { get; set; } = Color.White;
    public override float MapIconScale { get; set; } = 1.0f;
    public override string ButtonPromptText { get; set; }
    public Restaurant(Vector3 _EntrancePosition, float _EntranceHeading, string _Name, string _Description, string menuID,FoodType foodType) : base(_EntrancePosition, _EntranceHeading, _Name, _Description)
    {
        MenuID = menuID;
        FoodType = foodType;
        ButtonPromptText = $"Dine at {Name}";
    }
    public override void OnInteract(ILocationInteractable player, IModItems modItems, IEntityProvideable world, ISettingsProvideable settings, IWeapons weapons, ITimeControllable time)
    {
        Player = player;
        ModItems = modItems;
        World = world;
        Settings = settings;
        Weapons = weapons;
        Time = time;

        if (CanInteract)
        {
            Player.IsInteractingWithLocation = true;
            CanInteract = false;
            Player.IsTransacting = true;
            GameFiber.StartNew(delegate
            {
                StoreCamera = new LocationCamera(this, Player);
                StoreCamera.Setup();

                CreateInteractionMenu();
                Transaction = new Transaction(MenuPool, InteractionMenu, Menu, this);
                Transaction.CreateTransactionMenu(Player, modItems, world, settings, weapons, time);

                InteractionMenu.Visible = true;
                InteractionMenu.OnItemSelect += InteractionMenu_OnItemSelect;
                Transaction.ProcessTransactionMenu();

                Transaction.DisposeTransactionMenu();
                DisposeInteractionMenu();

                StoreCamera.Dispose();

                Player.IsInteractingWithLocation = false;
                Player.IsTransacting = false;
                CanInteract = true;
            }, "RestaurantInteract");
        }
    }
    private void InteractionMenu_OnItemSelect(RAGENativeUI.UIMenu sender, UIMenuItem selectedItem, int index)
    {
        if (selectedItem.Text == "Buy")
        {
            Transaction?.SellMenu?.Dispose();
            Transaction?.PurchaseMenu?.Show();
        }
        else if (selectedItem.Text == "Sell")
        {
            Transaction?.PurchaseMenu?.Dispose();
            Transaction?.SellMenu?.Show();
        }
    }
    public override List<Tuple<string, string>> DirectoryInfo(int currentHour, float distanceTo)
    {
        List<Tuple<string, string>> BaseList = base.DirectoryInfo(currentHour, distanceTo).ToList();
        BaseList.Add(Tuple.Create(FoodType.ToString(), ""));
        return BaseList;

    }
}

