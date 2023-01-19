﻿using ExtensionsMethods;
using LosSantosRED.lsr.Interface;
using Rage;
using Rage.Native;
using System.Collections.Generic;

public class Firefighter : PedExt
{
    private uint GameTimeSpawned;
    public Firefighter(Ped pedestrian, ISettingsProvideable settings, int health, Agency agency, bool wasModSpawned, ICrimes crimes, IWeapons weapons, string name, IEntityProvideable world) : base(pedestrian,settings, crimes, weapons, name, "FireFighter", world)
    {
        Health = health;
        AssignedAgency = agency;
        WasModSpawned = wasModSpawned;
        if (WasModSpawned)
        {
            GameTimeSpawned = Game.GameTime;
        }
    }
    public Agency AssignedAgency { get; set; } = new Agency();
    public uint HasBeenSpawnedFor => Game.GameTime - GameTimeSpawned;
    public override bool KnownsDrugAreas => false;
    //public bool WasModSpawned { get; private set; }
}