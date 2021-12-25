﻿using LosSantosRED.lsr;
using LosSantosRED.lsr.Interface;
using Rage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Civilians
{
    private IEntityProvideable World;
    private IPoliceRespondable PoliceRespondable;
    private IPerceptable Perceptable;
    private uint GameTimeLastUpdatedPeds;

    public Civilians(IEntityProvideable world, IPoliceRespondable policeRespondable, IPerceptable perceptable)
    {
        World = world;
        PoliceRespondable = policeRespondable;
        Perceptable = perceptable;
    }
    public int PersistentCount
    {
        get
        {
            return World.CivilianList.Count(x => x.Pedestrian.IsPersistent);
        }
    }
    public void ResetWitnessedCrimes()
    {
        World.CivilianList.ForEach(x => x.PlayerCrimesWitnessed.Clear());
    }
    public void Update()
    {
        foreach (PedExt ped in World.CivilianList.OrderBy(x => x.GameTimeLastUpdated))
        {
            try
            {
                ped.Update(Perceptable, PoliceRespondable, Vector3.Zero, World);
                GameFiber.Yield();
            }
            catch (Exception e)
            {
                EntryPoint.WriteToConsole("Error" + e.Message + " : " + e.StackTrace, 0);
                Game.DisplayNotification("CHAR_BLANK_ENTRY", "CHAR_BLANK_ENTRY", "~o~Error", "Los Santos ~r~RED", "Los Santos ~r~RED ~s~ Error Updating Civilian Data");
            }
        }
        foreach (PedExt ped in World.MerchantList.OrderBy(x => x.GameTimeLastUpdated))
        {
            try
            {
                ped.Update(Perceptable, PoliceRespondable, Vector3.Zero, World);
                GameFiber.Yield();
            }
            catch (Exception e)
            {
                EntryPoint.WriteToConsole("Error" + e.Message + " : " + e.StackTrace, 0);
                Game.DisplayNotification("CHAR_BLANK_ENTRY", "CHAR_BLANK_ENTRY", "~o~Error", "Los Santos ~r~RED", "Los Santos ~r~RED ~s~ Error Updating Merchant Data");
            }
        }
        EntryPoint.WriteToConsole($"Civilians.Update Ran Time Since {Game.GameTime - GameTimeLastUpdatedPeds}", 5);
        GameTimeLastUpdatedPeds = Game.GameTime;
    }
}
