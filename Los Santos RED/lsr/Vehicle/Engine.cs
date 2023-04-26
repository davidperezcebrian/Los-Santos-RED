﻿using ExtensionsMethods;
using LosSantosRED.lsr;
using LosSantosRED.lsr.Interface;
using LSR.Vehicles;
using Rage;
using Rage.Native;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


public class Engine
{
    private float Health = 1000f;
    private VehicleExt VehicleToMonitor;
    private ISettingsProvideable Settings;
    private uint GameTimeLastToggleEngine;

    public bool IsRunning { get; private set; }
    public bool CanToggle => VehicleToMonitor.Vehicle.Exists() && VehicleToMonitor.Vehicle.Speed < 4f && !VehicleToMonitor.Vehicle.MustBeHotwired;
    public Engine(VehicleExt vehicleToMonitor, ISettingsProvideable settings)
    {
        VehicleToMonitor = vehicleToMonitor;
        Settings = settings;
        if (vehicleToMonitor.Vehicle.Exists())
        {
            Health = vehicleToMonitor.Vehicle.EngineHealth;
            IsRunning = vehicleToMonitor.Vehicle.IsEngineOn;
        }
    }
    public void Update(IDriveable driver)
    {
        UpdateDamage(driver);
        UpdateState();
    }
    public void Toggle()
    {
        if (Game.GameTime - GameTimeLastToggleEngine >= 1500)
        {
            Toggle(!IsRunning);
            GameTimeLastToggleEngine = Game.GameTime;
        }
    }
    public void Toggle(bool DesiredStatus)
    {     
        if (CanToggle)
        {
            IsRunning = DesiredStatus;
            Update(null);
        }
    }
    private void UpdateDamage(IDriveable driver)
    {
        if (Health > VehicleToMonitor.Vehicle.EngineHealth)
        {
            float Difference = Health - VehicleToMonitor.Vehicle.EngineHealth;
            bool Collided = NativeFunction.Natives.HAS_ENTITY_COLLIDED_WITH_ANYTHING<bool>(VehicleToMonitor.Vehicle);
            if (Settings.SettingsManager.VehicleSettings.ScaleEngineDamage)
            {
                float ScaledDamage = Health - Settings.SettingsManager.VehicleSettings.ScaleEngineDamageMultiplier * Difference;
                if (ScaledDamage <= -4000f)
                {
                    ScaledDamage = -4000f;
                }
                VehicleToMonitor.Vehicle.EngineHealth = ScaledDamage;
                Health = ScaledDamage;
                driver?.OnVehicleEngineHealthDecreased(ScaledDamage, Collided);
            }
            else
            {
                driver?.OnVehicleEngineHealthDecreased(Difference, Collided);
                Health = VehicleToMonitor.Vehicle.EngineHealth;
            }
        }
    }
    private void UpdateState()
    {
        if (Settings.SettingsManager.VehicleSettings.AllowSetEngineState)
        {
            if(VehicleToMonitor.IsHotWireLocked)
            {
                if(!VehicleToMonitor.HasShowHotwireLockPrompt)
                {
                    VehicleToMonitor.HasShowHotwireLockPrompt = true;
                    Game.DisplayHelp("Screwdriver required to hotwire");
                }
                
                VehicleToMonitor.Vehicle.MustBeHotwired = false;
                VehicleToMonitor.Vehicle.IsDriveable = false;
                VehicleToMonitor.Vehicle.IsEngineOn = false;
                IsRunning = false;
              //  EntryPoint.WriteToConsole($"PLAYER EVENT: VEHICLE SET NOT DRIVEABLE 1");
                return;
            }

            if(VehicleToMonitor.IsDisabled && Settings.SettingsManager.VehicleSettings.DisableAircraftWithoutLicense)
            {
                VehicleToMonitor.Vehicle.MustBeHotwired = false;
                VehicleToMonitor.Vehicle.IsDriveable = false;
                VehicleToMonitor.Vehicle.IsEngineOn = false;
                IsRunning = false;
               // EntryPoint.WriteToConsole($"PLAYER EVENT: VEHICLE SET NOT DRIVEABLE 2");
                return;
            }



            if (VehicleToMonitor.Vehicle.IsEngineStarting)
            {
                IsRunning = true;
            }
            if (!IsRunning)
            {
                VehicleToMonitor.Vehicle.IsDriveable = false;
                VehicleToMonitor.Vehicle.IsEngineOn = false;
                //EntryPoint.WriteToConsole($"PLAYER EVENT: VEHICLE SET NOT DRIVEABLE 3");
            }
            else
            {
                VehicleToMonitor.Vehicle.IsDriveable = true;
                VehicleToMonitor.Vehicle.IsEngineOn = true;
                //EntryPoint.WriteToConsole($"PLAYER EVENT: VEHICLE SET DRIVEABLE");
            }
        }
        else
        {
            if (VehicleToMonitor.Vehicle.IsEngineStarting || VehicleToMonitor.Vehicle.IsEngineOn)
            {
                IsRunning = true;
            }
        }
    }

    public void Synchronize()
    {
        if(VehicleToMonitor.Vehicle.Exists())
        {
            IsRunning = VehicleToMonitor.Vehicle.IsEngineOn;
        }
    }
}
