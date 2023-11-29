﻿using LosSantosRED.lsr.Helper;
using LosSantosRED.lsr.Interface;
using Rage;
using RAGENativeUI;
using RAGENativeUI.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.AxHost;

public class DebugMoneySubMenu : DebugSubMenu
{
    private ISettingsProvideable Settings;
    private ICrimes Crimes;
    private ITaskerable Tasker;
    private IEntityProvideable World;
    private IWeapons Weapons;
    private IModItems ModItems;
    private ITimeControllable Time;
    private IRadioStations RadioStations;
    private INameProvideable Names;
    private ModDataFileManager ModDataFileManager;
    public DebugMoneySubMenu(UIMenu debug, MenuPool menuPool, IActionable player, ISettingsProvideable settings, ICrimes crimes, ITaskerable tasker, IEntityProvideable world, IWeapons weapons, IModItems modItems, ITimeControllable time,
        IRadioStations radioStations, INameProvideable names, ModDataFileManager modDataFileManager) : base(debug, menuPool, player)
    {
        Settings = settings;
        Crimes = crimes;
        Tasker = tasker;
        World = world;
        Weapons = weapons;
        ModItems = modItems;
        Time = time;
        RadioStations = radioStations;
        Names = names;
        ModDataFileManager = modDataFileManager;
    }
    public override void AddItems()
    {
        UIMenu moneyItemsSubMenu = MenuPool.AddSubMenu(Debug, "Money Menu");
        moneyItemsSubMenu.SetBannerType(EntryPoint.LSRedColor);
        Debug.MenuItems[Debug.MenuItems.Count() - 1].Description = "Change various money items.";
        UIMenuItem GiveMoney = new UIMenuItem("Give On Hand Cash", "Give the player $50K on hand cash");
        GiveMoney.Activated += (menu, item) =>
        {
            Player.BankAccounts.GiveMoney(50000, false);
            menu.Visible = false;
        };
        UIMenuItem SetMoney = new UIMenuItem("Set On Hand Cash", "Sets the current player cash on hand");
        SetMoney.Activated += (menu, item) =>
        {
            if (int.TryParse(NativeHelper.GetKeyboardInput(""), out int moneyToSet))
            {
                Player.BankAccounts.SetCash(moneyToSet);
            }
            menu.Visible = false;
        };
        UIMenuItem RemoveAccountsMenu = new UIMenuItem("Remove Accounts", "Removes all the banks accounts for the current character");
        RemoveAccountsMenu.Activated += (menu, item) =>
        {
            Player.BankAccounts.Reset();
            menu.Visible = false;
        };
        moneyItemsSubMenu.AddItem(GiveMoney);
        moneyItemsSubMenu.AddItem(SetMoney);
        moneyItemsSubMenu.AddItem(RemoveAccountsMenu);
        UIMenuListScrollerItem<BankAccount> RemoveSpecificAccountMenu = new UIMenuListScrollerItem<BankAccount>("Remove Account", "Removes the selected bank account", Player.BankAccounts.BankAccountList);
        RemoveSpecificAccountMenu.Activated += (menu, item) =>
        {
            Player.BankAccounts.Remove(RemoveSpecificAccountMenu.SelectedItem);
            menu.Visible = false;
        };
        moneyItemsSubMenu.AddItem(RemoveSpecificAccountMenu);

        UIMenuListScrollerItem<Bank> AddSpecificAccountMenu = new UIMenuListScrollerItem<Bank>("Add Account", "Add a new account from the selected bank", ModDataFileManager.PlacesOfInterest.PossibleLocations.Banks);
        AddSpecificAccountMenu.Activated += (menu, item) =>
        {
            Player.BankAccounts.CreateRandomAccount(AddSpecificAccountMenu.SelectedItem);
            menu.Visible = false;
        };
        moneyItemsSubMenu.AddItem(AddSpecificAccountMenu);

    }
}

