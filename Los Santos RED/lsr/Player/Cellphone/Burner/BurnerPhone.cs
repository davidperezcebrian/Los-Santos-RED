﻿using iFruitAddon2;
using LosSantosRED.lsr.Interface;
using Rage;
using Rage.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;


public class BurnerPhone
{
    private ICellPhoneable Player;
    private ITimeReportable Time;
    private int globalScaleformID;
    private bool isPhoneActive;
    private int CurrentApp;
    private int CurrentColumn;
    private int CurrentRow;
    private int CurrentIndex;
    private bool isVanillaPhoneDisabled;
    private int prevCurrentRow;
    private int prevCurrentColumn;
    private int prevCurrentApp;
    private int prevCurrentIndex;
    private bool DisplayingTextMessage = false;
    private ISettingsProvideable Settings;
    private bool IsDisplayingCall;

    public BurnerPhone(ICellPhoneable player, ITimeReportable time, ISettingsProvideable settings)
    {
        Player = player;
        Time = time;
        Settings = settings;
    }

    public void Setup()
    {
        NativeFunction.Natives.DESTROY_MOBILE_PHONE();
        globalScaleformID = NativeFunction.Natives.REQUEST_SCALEFORM_MOVIE<int>("cellphone_ifruit");
        while(!NativeFunction.Natives.HAS_SCALEFORM_MOVIE_LOADED<bool>(globalScaleformID))
        {
            GameFiber.Yield();
        }
        //SetHomeMenuApp(globalScaleformID, 0, 2, "Texts",Player.CellPhone.TextList.Where(x=>!x.IsRead).Count(), 100);
        //SetHomeMenuApp(globalScaleformID, 1, 5, "Contacts", 0, 100);
        //SetHomeMenuApp(globalScaleformID, 2, 12, "To-Do List", 0, 100);
        //SetHomeMenuApp(globalScaleformID, 3, 59, "Mobile Radio", 0, 100);
        //SetHomeMenuApp(globalScaleformID, 4, 6, "Eyefind", 0, 100);
        //SetHomeMenuApp(globalScaleformID, 5, 8, "Unknown App", 0, 100);
        //SetHomeMenuApp(globalScaleformID, 6, 24, "Settings", 0, 100);
        //SetHomeMenuApp(globalScaleformID, 7, 1, "Snapmatic", 0, 100);
        //SetHomeMenuApp(globalScaleformID, 8, 57, "SecuroServ", 0, 100);


        SetHomeScreen();
    }
    public void Update()
    {
        if(isPhoneActive)
        {
            DetectInput();
            UpdatePhone();


            if(prevCurrentRow != CurrentRow)
            {
                EntryPoint.WriteToConsole($"CurrentRow Changed from {prevCurrentRow} to {CurrentRow}");
                Game.DisplaySubtitle($"CurrentRow Changed from {prevCurrentRow} to {CurrentRow}");
                prevCurrentRow = CurrentRow;
            }
            if (prevCurrentColumn != CurrentColumn)
            {
                EntryPoint.WriteToConsole($"CurrentColumn Changed from {prevCurrentColumn} to {CurrentColumn}");
                Game.DisplaySubtitle($"CurrentColumn Changed from {prevCurrentColumn} to {CurrentColumn}");
                prevCurrentColumn = CurrentColumn;
            }
            if (prevCurrentApp != CurrentApp)
            {
                EntryPoint.WriteToConsole($"CurrentApp Changed from {prevCurrentApp} to {CurrentApp}");
                Game.DisplaySubtitle($"CurrentApp Changed from {prevCurrentApp} to {CurrentApp}");
                prevCurrentApp = CurrentApp;
            }
            if (prevCurrentIndex != CurrentIndex)
            {
                EntryPoint.WriteToConsole($"CurrentIndex Changed from {prevCurrentIndex} to {CurrentIndex}");
                Game.DisplaySubtitle($"CurrentIndex Changed from {prevCurrentIndex} to {CurrentIndex}");
                prevCurrentIndex = CurrentIndex;
            }
        }
    }
    public void ClosePhone()
    {
        isPhoneActive = false;
        NativeFunction.Natives.DESTROY_MOBILE_PHONE();

        NativeFunction.Natives.PLAY_SOUND_FRONTEND(-1, "Put_Away", "Phone_SoundSet_Michael", 1);

        Game.DisableControlAction(0, GameControl.Sprint, false);


        Tools.Scripts.StartScript("cellphone_flashhand", 1424);
        Tools.Scripts.StartScript("cellphone_controller", 1424);

    }
    public void OpenPhone()
    {

        SetHomeMenuApp(globalScaleformID, 0, 2, "Texts", Player.CellPhone.TextList.Where(x => !x.IsRead).Count(), 100);
        SetHomeMenuApp(globalScaleformID, 1, 5, "Contacts", 0, 100);


        isPhoneActive = true;

        CurrentApp = 1;
        CurrentColumn = 0;
        CurrentRow = 0;
        CurrentIndex = 0;
        DisplayingTextMessage = false;

        NativeFunction.Natives.CREATE_MOBILE_PHONE(0);

        //NativeFunction.Natives.SET_MOBILE_PHONE_POSITION(0f, 5f, -60f);
        //NativeFunction.Natives.SET_MOBILE_PHONE_ROTATION(-90f,0f,0f);
        //NativeFunction.Natives.SET_MOBILE_PHONE_SCALE(250f);


        NativeFunction.Natives.SET_MOBILE_PHONE_POSITION(Settings.SettingsManager.CellphoneSettings.BurnerCellPositionX, Settings.SettingsManager.CellphoneSettings.BurnerCellPositionY, Settings.SettingsManager.CellphoneSettings.BurnerCellPositionZ);
        NativeFunction.Natives.SET_MOBILE_PHONE_ROTATION(-90f, 0f, 0f);
        NativeFunction.Natives.SET_MOBILE_PHONE_SCALE(Settings.SettingsManager.CellphoneSettings.BurnerCellScale);

        NativeFunction.Natives.BEGIN_SCALEFORM_MOVIE_METHOD(globalScaleformID, "DISPLAY_VIEW");
        NativeFunction.Natives.xC3D0841A0CC546A6(1);
        NativeFunction.Natives.xC3D0841A0CC546A6(0);
        NativeFunction.Natives.END_SCALEFORM_MOVIE_METHOD();

        //CurrentIndex = GetSelectedIndex();


        NativeFunction.Natives.PLAY_SOUND_FRONTEND(-1, "Pull_Out", "Phone_SoundSet_Michael", 1);

        Game.DisableControlAction(0, GameControl.Sprint, true);


        isVanillaPhoneDisabled = true;
        Tools.Scripts.TerminateScript("cellphone_flashhand");
        Tools.Scripts.TerminateScript("cellphone_controller");


    }
    private void DetectInput()
    {
        DisabledVanillaControls();
        if (CurrentApp == 1)
        {
            HandleHomeInput();
        }
        else if (CurrentApp == 2)
        {
            HandleMessagesInput();
        }
        else if(CurrentApp == 3)
        {
            HandleContactsInput();
        }
    }
    private void DisabledVanillaControls()
    {
        Game.DisableControlAction(3, GameControl.CellphoneUp, true);
        Game.DisableControlAction(3, GameControl.CellphoneDown, true);
        Game.DisableControlAction(3, GameControl.CellphoneLeft, true);
        Game.DisableControlAction(3, GameControl.CellphoneRight, true);
        Game.DisableControlAction(3, GameControl.CellphoneSelect, true);
        Game.DisableControlAction(3, GameControl.CellphoneCancel, true);
    }
    private void HandleHomeInput()
    {
        if (NativeFunction.Natives.x91AEF906BCA88877<bool>(3, 172))//UP
        {
            EntryPoint.WriteToConsole("Burner Phone: Pressed UP");
            NavigateMenu(1);
            MoveFinger(1);
            CurrentRow = CurrentRow - 1;
            //CurrentIndex = GetSelectedIndex();
        }
        else if (NativeFunction.Natives.x91AEF906BCA88877<bool>(3, 173))//DOWN
        {
            EntryPoint.WriteToConsole("Burner Phone: Pressed DOWN");
            NavigateMenu(3);
            MoveFinger(2);
            CurrentRow = CurrentRow + 1;
            //CurrentIndex = GetSelectedIndex();
        }
        else if (NativeFunction.Natives.x91AEF906BCA88877<bool>(3, 174))//LEFT
        {
            EntryPoint.WriteToConsole("Burner Phone: Pressed LEFT");
            NavigateMenu(4);
            MoveFinger(4);
            CurrentColumn = CurrentColumn - 1;
            //CurrentIndex = GetSelectedIndex();
        }
        else if (NativeFunction.Natives.x91AEF906BCA88877<bool>(3, 175))//UP
        {
            EntryPoint.WriteToConsole("Burner Phone: Pressed RIGHT");
            NavigateMenu(2);
            MoveFinger(4);
            CurrentColumn = CurrentColumn + 1;
            //CurrentIndex = GetSelectedIndex();
        }
        CurrentColumn = CurrentColumn % 2;
        CurrentRow = CurrentRow % 1;
        CurrentIndex = GetCurrentIndex(CurrentColumn + 1, CurrentRow + 1);


        //CurrentIndex = GetSelectedIndex();

        if (NativeFunction.Natives.x91AEF906BCA88877<bool>(3, 176))//SELECT
        {
            EntryPoint.WriteToConsole($"Burner Phone: Pressed SELECT CurrentIndex {CurrentIndex} OpenApp {CurrentIndex + 1}");
            MoveFinger(5);
            NativeFunction.Natives.PLAY_SOUND_FRONTEND(-1, "Menu_Accept", "Phone_SoundSet_Michael", 1);
            OpenApp(CurrentIndex + 1);
        }
        if (NativeFunction.Natives.x305C8DCD79DA8B0F<bool>(3, 177))//CLOSE
        {
            EntryPoint.WriteToConsole("Burner Phone: Pressed CLOSE");
            ClosePhone();
        }
    }
    private void HandleMessagesInput()
    {
        if (NativeFunction.Natives.x91AEF906BCA88877<bool>(3, 172) && !DisplayingTextMessage)//UP
        {
            EntryPoint.WriteToConsole("Burner Phone: Pressed UP APP MESSAGES");
            MoveFinger(1);
            NavigateMenu(1);
            //CurrentIndex = GetSelectedIndex();
            CurrentRow = CurrentRow - 1;
        }
        else if (NativeFunction.Natives.x91AEF906BCA88877<bool>(3, 173) && !DisplayingTextMessage)//DOWN
        {
            EntryPoint.WriteToConsole("Burner Phone: Pressed DOWN APP MESSAGES");
            MoveFinger(2);
            NavigateMenu(3);
            //CurrentIndex = GetSelectedIndex();
            CurrentRow = CurrentRow + 1;
        }

        int TotalMessages = Player.CellPhone.TextList.Count();
        if (TotalMessages > 0)
        {
            if (CurrentRow > TotalMessages - 1)
            {
                CurrentRow = 0;
            }
        }
        if (CurrentRow < 0)
        {
            CurrentRow = 0;
        }
        if (NativeFunction.Natives.x91AEF906BCA88877<bool>(3, 176) && !DisplayingTextMessage)//SELECT
        {
            EntryPoint.WriteToConsole($"Burner Phone: Pressed MESSAGES CurrentIndex {CurrentIndex} OpenApp {CurrentIndex + 1}");
            MoveFinger(5);
            NativeFunction.Natives.PLAY_SOUND_FRONTEND(-1, "Menu_Accept", "Phone_SoundSet_Michael", 1);
            DisplayingTextMessage = true;
            DisplayTextUI(Player.CellPhone.TextList.Where(x => x.Index == CurrentRow).FirstOrDefault());
            Game.DisplaySubtitle($"SELECTED {CurrentRow}");
        }
        if (NativeFunction.Natives.x305C8DCD79DA8B0F<bool>(3, 177))//CLOSE
        {

            if (DisplayingTextMessage)
            {
                EntryPoint.WriteToConsole("Burner Phone: Pressed CLOSE APP READING MESSAGES");
                NativeFunction.Natives.PLAY_SOUND_FRONTEND(-1, "Menu_Back", "Phone_SoundSet_Michael", 1);
                DisplayingTextMessage = false;
                OpenApp(2);
            }
            else
            {
                EntryPoint.WriteToConsole("Burner Phone: Pressed CLOSE APP MESSAGES");

                NativeFunction.Natives.PLAY_SOUND_FRONTEND(-1, "Menu_Back", "Phone_SoundSet_Michael", 1);


                NativeFunction.Natives.BEGIN_SCALEFORM_MOVIE_METHOD(globalScaleformID, "DISPLAY_VIEW");
                NativeFunction.Natives.xC3D0841A0CC546A6(1);
                NativeFunction.Natives.xC3D0841A0CC546A6(0);
                NativeFunction.Natives.END_SCALEFORM_MOVIE_METHOD();


                GameFiber.Sleep(200);
                CurrentColumn = 0;
                CurrentRow = 0;
                CurrentIndex = 0;
                CurrentApp = 1;
            }
        }
    }
    private void HandleContactsInput()
    {
        if (NativeFunction.Natives.x91AEF906BCA88877<bool>(3, 172) && !IsDisplayingCall)//UP
        {
            EntryPoint.WriteToConsole("Burner Phone: Pressed UP APP CONTACTS");
            MoveFinger(1);
            NavigateMenu(1);
            //CurrentIndex = GetSelectedIndex();
            CurrentRow = CurrentRow - 1;
        }
        else if (NativeFunction.Natives.x91AEF906BCA88877<bool>(3, 173) && !IsDisplayingCall)//DOWN
        {
            EntryPoint.WriteToConsole("Burner Phone: Pressed DOWN APP CONTACTS");
            MoveFinger(2);
            NavigateMenu(3);
            //CurrentIndex = GetSelectedIndex();
            CurrentRow = CurrentRow + 1;
        }
        if (NativeFunction.Natives.x91AEF906BCA88877<bool>(3, 176) && !IsDisplayingCall)//SELECT
        {
            EntryPoint.WriteToConsole($"Burner Phone: Pressed SELECT CurrentIndex {CurrentIndex} OpenApp {CurrentIndex + 1}");
            MoveFinger(5);
            NativeFunction.Natives.PLAY_SOUND_FRONTEND(-1, "Menu_Accept", "Phone_SoundSet_Michael", 1);
            Game.DisplaySubtitle($"SELECTED {CurrentRow}");
            iFruitContact contact = Player.CellPhone.ContactList.Where(x => x.Index == CurrentRow).FirstOrDefault();
            if (contact != null)
            {
                IsDisplayingCall = true;
                contact.Call();
                DisplayCallUI(contact.Name, "CELL_211", contact.Icon.Name.SetBold(contact.Bold));
            }

        }
        int TotalContacts = Player.CellPhone.ContactList.Count();
        if (TotalContacts > 0)
        {
            if(CurrentRow > TotalContacts-1)
            {
                CurrentRow = 0;
            }
        }
        if (CurrentRow < 0)
        {
            CurrentRow = 0;
        }
        if (NativeFunction.Natives.x305C8DCD79DA8B0F<bool>(3, 177))//CLOSE
        {
            EntryPoint.WriteToConsole("Burner Phone: Pressed CLOSE APP CONTACTS");
            if (IsDisplayingCall)
            {
                NativeFunction.Natives.PLAY_SOUND_FRONTEND(-1, "Menu_Back", "Phone_SoundSet_Michael", 1);
                IsDisplayingCall = false;
                NativeFunction.Natives.TASK_USE_MOBILE_PHONE(Game.LocalPlayer.Character, false);
            }
            NativeFunction.Natives.PLAY_SOUND_FRONTEND(-1, "Menu_Back", "Phone_SoundSet_Michael", 1);
            NativeFunction.Natives.BEGIN_SCALEFORM_MOVIE_METHOD(globalScaleformID, "DISPLAY_VIEW");
            NativeFunction.Natives.xC3D0841A0CC546A6(1);
            NativeFunction.Natives.xC3D0841A0CC546A6(1);
            NativeFunction.Natives.END_SCALEFORM_MOVIE_METHOD();
            GameFiber.Sleep(200);
            CurrentColumn = 1;
            CurrentRow = 0;
            CurrentIndex = 1;
            CurrentApp = 1;
        }
    }
    public void DisplayCallUI(string contactName, string statusText = "CELL_211", string picName = "CELL_300")
    {
        string dialText;// = Game.GetGXTEntry(statusText); // "DIALING..." translated in current game's language
        unsafe
        {
            IntPtr ptr2 = NativeFunction.CallByHash<IntPtr>(0x7B5280EBA9840C72, statusText);
            dialText = Marshal.PtrToStringAnsi(ptr2);
        }

        NativeFunction.Natives.BEGIN_SCALEFORM_MOVIE_METHOD(globalScaleformID, "SET_DATA_SLOT");
        NativeFunction.Natives.xC3D0841A0CC546A6(4);
        NativeFunction.Natives.xC3D0841A0CC546A6(0);
        NativeFunction.Natives.xC3D0841A0CC546A6(3);

        NativeFunction.Natives.BEGIN_TEXT_COMMAND_SCALEFORM_STRING("STRING");
        NativeFunction.Natives.x761B77454205A61D(contactName, -1);       //UI::_ADD_TEXT_COMPONENT_APP_TITLE
        NativeFunction.Natives.END_TEXT_COMMAND_SCALEFORM_STRING();

        NativeFunction.Natives.BEGIN_TEXT_COMMAND_SCALEFORM_STRING("CELL_2000");
        NativeFunction.Natives.ADD_TEXT_COMPONENT_SUBSTRING_PLAYER_NAME(picName);
        NativeFunction.Natives.END_TEXT_COMMAND_SCALEFORM_STRING();

        NativeFunction.Natives.BEGIN_TEXT_COMMAND_SCALEFORM_STRING("STRING");
        NativeFunction.Natives.x761B77454205A61D(dialText, -1);      //UI::_ADD_TEXT_COMPONENT_APP_TITLE
        NativeFunction.Natives.END_TEXT_COMMAND_SCALEFORM_STRING();

        NativeFunction.Natives.END_SCALEFORM_MOVIE_METHOD();

        NativeFunction.Natives.BEGIN_SCALEFORM_MOVIE_METHOD(globalScaleformID, "DISPLAY_VIEW");
        NativeFunction.Natives.xC3D0841A0CC546A6(4);
        NativeFunction.Natives.END_SCALEFORM_MOVIE_METHOD();
    }
    private void OpenApp(int Index)
    {
        CurrentApp = Index;
        if(Index == 2)//Messages
        {
            OpenMessagesApp();
        }
        else if (Index == 3)//Contacts
        {
            OpenContactsApp();
        }
    }
    private void OpenContactsApp()
    {
        NativeFunction.Natives.BEGIN_SCALEFORM_MOVIE_METHOD(globalScaleformID, "SET_DATA_SLOT_EMPTY");
        NativeFunction.Natives.xC3D0841A0CC546A6(2);//2
        NativeFunction.Natives.END_SCALEFORM_MOVIE_METHOD();
        foreach (iFruitContact contact in Player.CellPhone.ContactList)
        {
            EntryPoint.WriteToConsole($"Adding Contact {contact.Name} {contact.Index}");
            DrawContact(contact);
        }
        NativeFunction.Natives.BEGIN_SCALEFORM_MOVIE_METHOD(globalScaleformID, "DISPLAY_VIEW");
        NativeFunction.Natives.xC3D0841A0CC546A6(2);
        NativeFunction.Natives.xC3D0841A0CC546A6(0);
        NativeFunction.Natives.END_SCALEFORM_MOVIE_METHOD();

    }
    private void DrawContact(iFruitContact contact)
    {
        NativeFunction.Natives.BEGIN_SCALEFORM_MOVIE_METHOD(globalScaleformID, "SET_DATA_SLOT");
        NativeFunction.Natives.xC3D0841A0CC546A6(2);
        NativeFunction.Natives.xC3D0841A0CC546A6(contact.Index);
        NativeFunction.Natives.xC3D0841A0CC546A6(0);
        NativeFunction.Natives.BEGIN_TEXT_COMMAND_SCALEFORM_STRING("STRING");
        NativeFunction.Natives.ADD_TEXT_COMPONENT_SUBSTRING_PLAYER_NAME(contact.Name);
        NativeFunction.Natives.END_TEXT_COMMAND_SCALEFORM_STRING();
        NativeFunction.Natives.BEGIN_TEXT_COMMAND_SCALEFORM_STRING("CELL_999");
        NativeFunction.Natives.END_TEXT_COMMAND_SCALEFORM_STRING();
        NativeFunction.Natives.BEGIN_TEXT_COMMAND_SCALEFORM_STRING("CELL_2000");
        NativeFunction.Natives.ADD_TEXT_COMPONENT_SUBSTRING_PLAYER_NAME(contact.Icon.Name.SetBold(true));
        NativeFunction.Natives.END_TEXT_COMMAND_SCALEFORM_STRING();
        NativeFunction.Natives.END_SCALEFORM_MOVIE_METHOD();    
    }
    private void OpenMessagesApp()
    {
        NativeFunction.Natives.BEGIN_SCALEFORM_MOVIE_METHOD(globalScaleformID, "SET_DATA_SLOT_EMPTY");
        NativeFunction.Natives.xC3D0841A0CC546A6(6);//2
        NativeFunction.Natives.END_SCALEFORM_MOVIE_METHOD();
        foreach (iFruitText text in Player.CellPhone.TextList)
        {
            EntryPoint.WriteToConsole($"Adding Message {text.Name} {text.Index}");
            DrawMessage(text);
        }
        NativeFunction.Natives.BEGIN_SCALEFORM_MOVIE_METHOD(globalScaleformID, "DISPLAY_VIEW");
        NativeFunction.Natives.xC3D0841A0CC546A6(6);
        NativeFunction.Natives.xC3D0841A0CC546A6(CurrentRow);
        NativeFunction.Natives.END_SCALEFORM_MOVIE_METHOD();
    }
    private void DrawMessage(iFruitText text)
    {

        NativeFunction.Natives.BEGIN_SCALEFORM_MOVIE_METHOD(globalScaleformID, "SET_DATA_SLOT");
        NativeFunction.Natives.xC3D0841A0CC546A6(6);//2
        NativeFunction.Natives.xC3D0841A0CC546A6(text.Index);
        NativeFunction.Natives.xC3D0841A0CC546A6(text.HourSent);
        NativeFunction.Natives.xC3D0841A0CC546A6(text.MinuteSent);

        if (text.IsRead)
        {
            NativeFunction.Natives.xC3D0841A0CC546A6(34);
        }
        else
        {
            NativeFunction.Natives.xC3D0841A0CC546A6(33);
        }

        NativeFunction.Natives.BEGIN_TEXT_COMMAND_SCALEFORM_STRING("STRING");
        NativeFunction.Natives.ADD_TEXT_COMPONENT_SUBSTRING_PLAYER_NAME(text.Name);
        NativeFunction.Natives.END_TEXT_COMMAND_SCALEFORM_STRING();

        NativeFunction.Natives.BEGIN_TEXT_COMMAND_SCALEFORM_STRING("STRING");
        NativeFunction.Natives.ADD_TEXT_COMPONENT_SUBSTRING_PLAYER_NAME(text.Message);
        NativeFunction.Natives.END_TEXT_COMMAND_SCALEFORM_STRING();

        NativeFunction.Natives.END_SCALEFORM_MOVIE_METHOD();
        
    }
    private int GetCurrentIndex(int column, int row)
    {
        if (row == 1 && column == 1)
            return 1;
        else if(row == 1 && column == 2)
            return 2;
        //else if(row == 1 && column == 3)
        //    return 3;
        //else if(row == 2 && column == 1)
        //    return 4;
        //else if(row == 2 && column == 2)
        //    return 5;
        //else if(row == 2 && column == 3)
        //    return 6;
        //else if(row == 3 && column == 1)
        //    return 7;
        //else if(row == 3 && column == 2)
        //    return 8;
        //else if(row == 3 && column == 3)
        //    return 9;
        else
            return 1;
    }
    private int GetSelectedIndex()
    {
        NativeFunction.Natives.BEGIN_SCALEFORM_MOVIE_METHOD(globalScaleformID, "GET_CURRENT_SELECTION");
        int num = NativeFunction.Natives.END_SCALEFORM_MOVIE_METHOD_RETURN_VALUE<int>();
        while (!NativeFunction.Natives.x768FF8961BA904D6<bool>(num))         //UI::_GET_SCALEFORM_MOVIE_FUNCTION_RETURN_BOOL
        {
            GameFiber.Wait(0);
        }
        int data = NativeFunction.Natives.x2DE7EFA66B906036<int>(num);       //UI::_GET_SCALEFORM_MOVIE_FUNCTION_RETURN_INT
        return data+1;
    }
    private void NavigateMenu(int index)
    {
        NativeFunction.Natives.BEGIN_SCALEFORM_MOVIE_METHOD(globalScaleformID, "SET_INPUT_EVENT");
        NativeFunction.Natives.xC3D0841A0CC546A6(index);
        NativeFunction.Natives.END_SCALEFORM_MOVIE_METHOD();
        NativeFunction.Natives.PLAY_SOUND_FRONTEND(-1, "Menu_Navigate", "Phone_SoundSet_Michael", 1);
    }
    private void MoveFinger(int index)
    {
        NativeFunction.Natives.x95C9E72F3D7DEC9B(index);
    }
    private void UpdatePhone()
    {
        NativeFunction.Natives.BEGIN_SCALEFORM_MOVIE_METHOD(globalScaleformID, "SET_TITLEBAR_TIME");
        NativeFunction.Natives.xC3D0841A0CC546A6(Time.CurrentHour);
        NativeFunction.Natives.xC3D0841A0CC546A6(Time.CurrentMinute);

        NativeFunction.Natives.BEGIN_TEXT_COMMAND_SCALEFORM_STRING("STRING");
        NativeFunction.Natives.ADD_TEXT_COMPONENT_SUBSTRING_PLAYER_NAME(Time.CurrentDateTime.ToString("ddd"));
        NativeFunction.Natives.END_TEXT_COMMAND_SCALEFORM_STRING();

        NativeFunction.Natives.END_SCALEFORM_MOVIE_METHOD();

        NativeFunction.Natives.BEGIN_SCALEFORM_MOVIE_METHOD(globalScaleformID, "SET_SIGNAL_STRENGTH");
        NativeFunction.Natives.xC3D0841A0CC546A6(5);//1-5
        NativeFunction.Natives.END_SCALEFORM_MOVIE_METHOD();

        unsafe
        {
            int lol = 0;
            NativeFunction.CallByName<bool>("GET_MOBILE_PHONE_RENDER_ID", &lol);
            NativeFunction.Natives.SET_TEXT_RENDER_ID(lol);
        }


        NativeFunction.Natives.DRAW_SCALEFORM_MOVIE(globalScaleformID, 0.1f, 0.18f, 0.2f, 0.35f, 255, 255, 255, 255, 0);
        NativeFunction.Natives.SET_TEXT_RENDER_ID(1);
    }
    private void SetHomeMenuApp(int scaleform, int index, int icon, string name, int notifications, int opactiy)
    {
        NativeFunction.Natives.BEGIN_SCALEFORM_MOVIE_METHOD(scaleform, "SET_DATA_SLOT");
        NativeFunction.Natives.xC3D0841A0CC546A6(1);

        NativeFunction.Natives.xC3D0841A0CC546A6(index);

        NativeFunction.Natives.xC3D0841A0CC546A6(icon);

        NativeFunction.Natives.xC3D0841A0CC546A6(notifications);

        NativeFunction.Natives.BEGIN_TEXT_COMMAND_SCALEFORM_STRING("STRING");
        NativeFunction.Natives.ADD_TEXT_COMPONENT_SUBSTRING_PLAYER_NAME(name);
        NativeFunction.Natives.END_TEXT_COMMAND_SCALEFORM_STRING();

        NativeFunction.Natives.xC3D0841A0CC546A6(opactiy);

        NativeFunction.Natives.END_SCALEFORM_MOVIE_METHOD();
    }
    private void SetHomeScreen()
    {
        NativeFunction.Natives.BEGIN_SCALEFORM_MOVIE_METHOD(globalScaleformID, "SET_THEME");
        NativeFunction.Natives.xC3D0841A0CC546A6(1);
        NativeFunction.Natives.END_SCALEFORM_MOVIE_METHOD();

        NativeFunction.Natives.BEGIN_SCALEFORM_MOVIE_METHOD(globalScaleformID, "SET_SLEEP_MODE");
        NativeFunction.Natives.xC3D0841A0CC546A6(0);
        NativeFunction.Natives.END_SCALEFORM_MOVIE_METHOD();
    }
    public void DisplayTextUI(iFruitText text)
    {
        if (text != null)
        {
            text.IsRead = true;
            NativeFunction.Natives.BEGIN_SCALEFORM_MOVIE_METHOD(globalScaleformID, "SET_DATA_SLOT");
            NativeFunction.Natives.xC3D0841A0CC546A6(7);
            NativeFunction.Natives.xC3D0841A0CC546A6(0);

            NativeFunction.Natives.BEGIN_TEXT_COMMAND_SCALEFORM_STRING("STRING");
            NativeFunction.Natives.ADD_TEXT_COMPONENT_SUBSTRING_PLAYER_NAME(text.Name);       //UI::_ADD_TEXT_COMPONENT_APP_TITLE
            NativeFunction.Natives.END_TEXT_COMMAND_SCALEFORM_STRING();

            NativeFunction.Natives.BEGIN_TEXT_COMMAND_SCALEFORM_STRING("STRING");
            NativeFunction.Natives.ADD_TEXT_COMPONENT_SUBSTRING_PLAYER_NAME(text.Message);       //UI::_ADD_TEXT_COMPONENT_APP_TITLE
            NativeFunction.Natives.END_TEXT_COMMAND_SCALEFORM_STRING();

            NativeFunction.Natives.BEGIN_TEXT_COMMAND_SCALEFORM_STRING("STRING");
            NativeFunction.Natives.ADD_TEXT_COMPONENT_SUBSTRING_PLAYER_NAME("CHAR_BLANK_ENTRY");       //UI::_ADD_TEXT_COMPONENT_APP_TITLE
            NativeFunction.Natives.END_TEXT_COMMAND_SCALEFORM_STRING();

            NativeFunction.Natives.END_SCALEFORM_MOVIE_METHOD();

            NativeFunction.Natives.BEGIN_SCALEFORM_MOVIE_METHOD(globalScaleformID, "DISPLAY_VIEW");
            NativeFunction.Natives.xC3D0841A0CC546A6(7);
            NativeFunction.Natives.END_SCALEFORM_MOVIE_METHOD();

            SetHomeMenuApp(globalScaleformID, 0, 2, "Texts", Player.CellPhone.TextList.Where(x => !x.IsRead).Count(), 100);
        }
    }


}

