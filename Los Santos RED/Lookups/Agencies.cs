﻿using ExtensionsMethods;
using Rage;
using Rage.Native;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

public static class Agencies
{
    private static string ConfigFileName = "Plugins\\LosSantosRED\\Agencies.xml";
    public static List<Agency> AgenciesList { get; set; }

    public static void Initialize()
    {
        ReadConfig();
    }
    public static void Dispose()
    {

    }
    public static void ReadConfig()
    {
        if (File.Exists(ConfigFileName))
        {
            AgenciesList = LosSantosRED.DeserializeParams<Agency>(ConfigFileName);
        }
        else
        {
            DefaultConfig();
            LosSantosRED.SerializeParams(AgenciesList, ConfigFileName);
        }
    }
    private static void DefaultConfig()
    {

        //Peds
        List<Agency.ModelInformation> StandardCops = new List<Agency.ModelInformation>() {
            new Agency.ModelInformation("s_m_y_cop_01", true,85,85),
            new Agency.ModelInformation("s_f_y_cop_01", false,15,15) };
        List<Agency.ModelInformation> ExtendedStandardCops = new List<Agency.ModelInformation>() {
            new Agency.ModelInformation("s_m_y_cop_01", true,85,85),
            new Agency.ModelInformation("s_f_y_cop_01", false,10,10),
            new Agency.ModelInformation("ig_trafficwarden", true,5,5) };
        List<Agency.ModelInformation> ParkRangers = new List<Agency.ModelInformation>() {
            new Agency.ModelInformation("s_m_y_ranger_01", true,75,75),
            new Agency.ModelInformation("s_f_y_ranger_01", false,25,25) };
        List<Agency.ModelInformation> SheriffPeds = new List<Agency.ModelInformation>() {
            new Agency.ModelInformation("s_m_y_sheriff_01", true,75,75),
            new Agency.ModelInformation("s_f_y_sheriff_01", false,25,25) };
        List<Agency.ModelInformation> SWAT = new List<Agency.ModelInformation>() {
            new Agency.ModelInformation("s_m_y_swat_01", true, 0,100) };
        List<Agency.ModelInformation> PoliceAndSwat = new List<Agency.ModelInformation>() {
            new Agency.ModelInformation("s_m_y_cop_01", true,70,0),
            new Agency.ModelInformation("s_f_y_cop_01", false,30,0),
            new Agency.ModelInformation("s_m_y_swat_01", true, 0,100) };
        List<Agency.ModelInformation> DOAPeds = new List<Agency.ModelInformation>() {
            new Agency.ModelInformation("u_m_m_doa_01", true,100,100) };
        List<Agency.ModelInformation> IAAPeds = new List<Agency.ModelInformation>() {
            new Agency.ModelInformation("s_m_m_fibsec_01", true,100,100) };
        List<Agency.ModelInformation> SAHPPeds = new List<Agency.ModelInformation>() {
            new Agency.ModelInformation("s_m_y_hwaycop_01", true,100,100) };
        List<Agency.ModelInformation> MilitaryPeds = new List<Agency.ModelInformation>() {
            new Agency.ModelInformation("s_m_y_armymech_01", true,25,0),
            new Agency.ModelInformation("s_m_m_marine_01", true,50,0),
            new Agency.ModelInformation("s_m_m_marine_02", true,0,0),
            new Agency.ModelInformation("s_m_y_marine_01", true,25,0),
            new Agency.ModelInformation("s_m_y_marine_02", true,0,0),
            new Agency.ModelInformation("s_m_y_marine_03", true,0,100),
            new Agency.ModelInformation("s_m_m_pilot_02", true,0,0),
            new Agency.ModelInformation("s_m_y_pilot_01", true,0,0) };
        List<Agency.ModelInformation> FIBPeds = new List<Agency.ModelInformation>() {
            new Agency.ModelInformation("s_m_m_fibsec_01", true,55,70),
            new Agency.ModelInformation("s_m_m_fiboffice_01", true,15,0),
            new Agency.ModelInformation("s_m_m_fiboffice_02", true,15,0),
            new Agency.ModelInformation("u_m_m_fibarchitect", true,10,0),
            new Agency.ModelInformation("s_m_y_swat_01", true, 5,30) { RequiredVariation = new PedVariation(new List<PedComponent>() { new PedComponent(10, 0, 1,0) },new List<PropComponent>() { new PropComponent(0, 0, 0) }) } };
        List<Agency.ModelInformation> PrisonPeds = new List<Agency.ModelInformation>() {
            new Agency.ModelInformation("s_m_m_prisguard_01", true,100,100) };
        List<Agency.ModelInformation> SecurityPeds = new List<Agency.ModelInformation>() {
            new Agency.ModelInformation("s_m_m_security_01", true,100,100) };
        List<Agency.ModelInformation> CoastGuardPeds = new List<Agency.ModelInformation>() {
            new Agency.ModelInformation("s_m_y_uscg_01", true,100,100) };
        List<Agency.ModelInformation> NOOSEPeds = new List<Agency.ModelInformation>() {
            new Agency.ModelInformation("s_m_y_swat_01", true, 100,100) { RequiredVariation = new PedVariation(new List<PedComponent>() { new PedComponent(10, 0, 0,0) },new List<PropComponent>() { new PropComponent(0, 0, 0) }) } };

        //Vehicles
        List<Agency.VehicleInformation> UnmarkedVehicles = new List<Agency.VehicleInformation>() {
            new Agency.VehicleInformation("police4", 100, 100) };
        List<Agency.VehicleInformation> ParkRangerVehicles = new List<Agency.VehicleInformation>() {
            new Agency.VehicleInformation("pranger", 100, 100) };
        List<Agency.VehicleInformation> FIBVehicles = new List<Agency.VehicleInformation>() {
            new Agency.VehicleInformation("fbi", 70, 70),
            new Agency.VehicleInformation("fbi2", 30, 30) };
        List<Agency.VehicleInformation> NOOSEVehicles = new List<Agency.VehicleInformation>() {
            new Agency.VehicleInformation("fbi", 70, 70) {MaxWantedLevelSpawn = 3 },
            new Agency.VehicleInformation("fbi2", 30, 30) {MaxWantedLevelSpawn = 3 },
            new Agency.VehicleInformation("riot", 0, 45) { MinWantedLevelSpawn = 4 ,MaxWantedLevelSpawn = 4, AllowedPedModels = new List<string>() { "s_m_y_swat_01" } },
            new Agency.VehicleInformation("riot2", 0, 45) { MinWantedLevelSpawn = 5, AllowedPedModels = new List<string>() { "s_m_y_swat_01" } },
            new Agency.VehicleInformation("annihilator", 0, 100) { MinWantedLevelSpawn = 4 ,MaxWantedLevelSpawn = 5, AllowedPedModels = new List<string>() { "s_m_y_swat_01" },IsHelicopter = true,MinOccupants = 3,MaxOccupants = 4 }};
        List<Agency.VehicleInformation> HighwayPatrolVehicles = new List<Agency.VehicleInformation>() {
            new Agency.VehicleInformation("policeb", 70, 70) { IsMotorcycle = true, MaxOccupants = 1 },
            new Agency.VehicleInformation("police4", 30, 30) };
        List<Agency.VehicleInformation> PrisonVehicles = new List<Agency.VehicleInformation>() {
            new Agency.VehicleInformation("policet", 70, 70),
            new Agency.VehicleInformation("police4", 30, 30) };
        List<Agency.VehicleInformation> LSPDVehiclesVanilla = new List<Agency.VehicleInformation>() {
            new Agency.VehicleInformation("police", 25,20) { Liveries = new List<int>() { 0,1,2,3,4,5 } },
            new Agency.VehicleInformation("police2", 25, 20) { Liveries = new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7 } },
            new Agency.VehicleInformation("police3", 25, 20) { Liveries = new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7 } },
            new Agency.VehicleInformation("police4", 10,5),
            new Agency.VehicleInformation("fbi2", 15,10),
            new Agency.VehicleInformation("policet", 0, 25) { MinWantedLevelSpawn = 3} };
        List<Agency.VehicleInformation> LSSDVehiclesVanilla = new List<Agency.VehicleInformation>() {
            new Agency.VehicleInformation("sheriff", 50, 50){ Liveries = new List<int> { 0, 1, 2, 3 } },
            new Agency.VehicleInformation("sheriff2", 50, 50) { Liveries = new List<int> { 0, 1, 2, 3 } } };
        List<Agency.VehicleInformation> LSPDVehicles = LSPDVehiclesVanilla;
        List<Agency.VehicleInformation> SAHPVehicles = HighwayPatrolVehicles;
        List<Agency.VehicleInformation> LSSDVehicles = LSSDVehiclesVanilla;
        List<Agency.VehicleInformation> BCSOVehicles = LSSDVehiclesVanilla;
        List<Agency.VehicleInformation> VWHillsLSSDVehicles = new List<Agency.VehicleInformation>() {
            new Agency.VehicleInformation("sheriff2", 100, 100) { Liveries = new List<int> { 0, 1, 2, 3 } } };
        List<Agency.VehicleInformation> ChumashLSSDVehicles = new List<Agency.VehicleInformation>() {
            new Agency.VehicleInformation("sheriff2", 100, 100) { Liveries = new List<int> { 0, 1, 2, 3 } } };
        List<Agency.VehicleInformation> RHPDVehicles = new List<Agency.VehicleInformation>() {
            new Agency.VehicleInformation("police2", 85, 50) { Liveries = new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7 } },
            new Agency.VehicleInformation("fbi", 15,25),
            new Agency.VehicleInformation("policet", 0, 25) { MinWantedLevelSpawn = 3} };
        List<Agency.VehicleInformation> VPPDVehicles = new List<Agency.VehicleInformation>() {
            new Agency.VehicleInformation("police3", 100, 75) { Liveries = new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7 } },
            new Agency.VehicleInformation("policet", 0, 25) { MinWantedLevelSpawn = 3} };
        List<Agency.VehicleInformation> ChumashLSPDVehicles = new List<Agency.VehicleInformation>() {
            new Agency.VehicleInformation("police3", 100, 75) { Liveries = new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7 } },
            new Agency.VehicleInformation("policet", 0, 25) { MinWantedLevelSpawn = 3} };
        List<Agency.VehicleInformation> EastLSPDVehicles = new List<Agency.VehicleInformation>() {
            new Agency.VehicleInformation("police", 100,75) { Liveries = new List<int>() { 0,1,2,3,4,5 } },
            new Agency.VehicleInformation("policet", 0, 25) { MinWantedLevelSpawn = 3} };
        List<Agency.VehicleInformation> VWPDVehicles = new List<Agency.VehicleInformation>() {
            new Agency.VehicleInformation("police", 100,75) { Liveries = new List<int>() { 0,1,2,3,4,5 } },
            new Agency.VehicleInformation("policet", 0, 25) { MinWantedLevelSpawn = 3} };
        List<Agency.VehicleInformation> PoliceHeliVehicles = new List<Agency.VehicleInformation>() {
            new Agency.VehicleInformation("polmav", 0,100) { Liveries = new List<int>() { 0 }, MinWantedLevelSpawn = 3,MaxWantedLevelSpawn = 4,IsHelicopter = true,MinOccupants = 3,MaxOccupants = 3 } };


        List<Agency.VehicleInformation> ArmyVehicles = new List<Agency.VehicleInformation>() {
            new Agency.VehicleInformation("crusader", 75,50) { Liveries = new List<int>() { 0 }, IsHelicopter = false,MinOccupants = 1,MaxOccupants = 2,MaxWantedLevelSpawn = 4 },
            new Agency.VehicleInformation("barracks", 25,50) { Liveries = new List<int>() { 0 }, IsHelicopter = false,MinOccupants = 1,MaxOccupants = 5,MaxWantedLevelSpawn = 4 },
            new Agency.VehicleInformation("rhino", 0,100) { Liveries = new List<int>() { 0 }, IsHelicopter = false,MinOccupants = 1,MaxOccupants = 2,MinWantedLevelSpawn = 5 },
        };


        //Weapons
        List<Agency.IssuedWeapon> AllWeapons = new List<Agency.IssuedWeapon>()
        {
            // Pistols
            new Agency.IssuedWeapon("weapon_pistol", true, new Agency.WeaponVariation()),
            new Agency.IssuedWeapon("weapon_pistol", true, new Agency.WeaponVariation(0, new List<string> { "Flashlight" })),
            new Agency.IssuedWeapon("weapon_pistol", true, new Agency.WeaponVariation(0,new List<string> { "Extended Clip" })),
            new Agency.IssuedWeapon("weapon_pistol", true, new Agency.WeaponVariation(0,new List<string> { "Flashlight","Extended Clip" })),

            new Agency.IssuedWeapon("weapon_pistol_mk2", true ,new Agency.WeaponVariation()),
            new Agency.IssuedWeapon("weapon_pistol_mk2", true, new Agency.WeaponVariation(0,new List<string> { "Flashlight" })),
            new Agency.IssuedWeapon("weapon_pistol_mk2", true, new Agency.WeaponVariation(0,new List<string> { "Extended Clip" })),
            new Agency.IssuedWeapon("weapon_pistol_mk2", true, new Agency.WeaponVariation(0, new List<string> { "Flashlight","Extended Clip" })),

            new Agency.IssuedWeapon("weapon_combatpistol", true, new Agency.WeaponVariation()),
            new Agency.IssuedWeapon("weapon_combatpistol", true, new Agency.WeaponVariation(0, new List<string> { "Flashlight" })),
            new Agency.IssuedWeapon("weapon_combatpistol", true, new Agency.WeaponVariation(0,new List<string> { "Extended Clip" })),
            new Agency.IssuedWeapon("weapon_combatpistol", true, new Agency.WeaponVariation(0, new List<string> { "Flashlight","Extended Clip" })),

            new Agency.IssuedWeapon("weapon_heavypistol", true, new Agency.WeaponVariation()),
            new Agency.IssuedWeapon("weapon_heavypistol", true, new Agency.WeaponVariation(0,new List<string> { "Etched Wood Grip Finish" })),
            new Agency.IssuedWeapon("weapon_heavypistol", true, new Agency.WeaponVariation(0,new List<string> { "Flashlight","Extended Clip" })),
            new Agency.IssuedWeapon("weapon_heavypistol", true, new Agency.WeaponVariation(0,new List<string> { "Extended Clip" })),

            // Shotguns
            new Agency.IssuedWeapon("weapon_pumpshotgun", false, new Agency.WeaponVariation()),
            new Agency.IssuedWeapon("weapon_pumpshotgun", false, new Agency.WeaponVariation(0,new List<string> { "Flashlight" })),

            new Agency.IssuedWeapon("weapon_pumpshotgun_mk2", false, new Agency.WeaponVariation()),
            new Agency.IssuedWeapon("weapon_pumpshotgun_mk2", false, new Agency.WeaponVariation(0, new List<string> { "Flashlight" })),
            new Agency.IssuedWeapon("weapon_pumpshotgun_mk2", false, new Agency.WeaponVariation(0, new List<string> { "Holographic Sight" })),
            new Agency.IssuedWeapon("weapon_pumpshotgun_mk2", false, new Agency.WeaponVariation(0, new List<string> { "Flashlight","Holographic Sight" })),

            // ARs
            new Agency.IssuedWeapon("weapon_carbinerifle", false, new Agency.WeaponVariation()),
            new Agency.IssuedWeapon("weapon_carbinerifle", false, new Agency.WeaponVariation(0,new List<string> { "Grip","Flashlight" })),
            new Agency.IssuedWeapon("weapon_carbinerifle", false, new Agency.WeaponVariation(0, new List<string> { "Scope", "Grip","Flashlight" })),
            new Agency.IssuedWeapon("weapon_carbinerifle", false, new Agency.WeaponVariation(0,new List<string> { "Scope", "Grip","Flashlight","Extended Clip" })),

            new Agency.IssuedWeapon("weapon_carbinerifle_mk2", false, new Agency.WeaponVariation()),
            new Agency.IssuedWeapon("weapon_carbinerifle_mk2", false, new Agency.WeaponVariation(0, new List<string> { "Holographic Sight","Grip","Flashlight" })),
            new Agency.IssuedWeapon("weapon_carbinerifle_mk2", false, new Agency.WeaponVariation(0, new List<string> { "Holographic Sight", "Grip","Extended Clip" })),
            new Agency.IssuedWeapon("weapon_carbinerifle_mk2", false, new Agency.WeaponVariation(0, new List<string> { "Large Scope", "Grip","Flashlight","Extended Clip" })),
        };

        List<Agency.IssuedWeapon> BestWeapons = new List<Agency.IssuedWeapon>()
        {
            new Agency.IssuedWeapon("weapon_pistol_mk2", true, new Agency.WeaponVariation(0,new List<string> { "Flashlight" })),
            new Agency.IssuedWeapon("weapon_pistol_mk2", true, new Agency.WeaponVariation(0,new List<string> { "Extended Clip" })),
            new Agency.IssuedWeapon("weapon_pistol_mk2", true, new Agency.WeaponVariation(0, new List<string> { "Flashlight","Extended Clip" })),
            new Agency.IssuedWeapon("weapon_carbinerifle_mk2", false, new Agency.WeaponVariation(0, new List<string> { "Holographic Sight","Grip","Flashlight" })),
            new Agency.IssuedWeapon("weapon_carbinerifle_mk2", false, new Agency.WeaponVariation(0, new List<string> { "Holographic Sight", "Grip","Extended Clip" })),
            new Agency.IssuedWeapon("weapon_carbinerifle_mk2", false, new Agency.WeaponVariation(0, new List<string> { "Large Scope", "Grip","Flashlight","Extended Clip" })),
        };

        List<Agency.IssuedWeapon> LimitedWeapons = new List<Agency.IssuedWeapon>()
        {
            new Agency.IssuedWeapon("weapon_heavypistol", true, new Agency.WeaponVariation()),
            new Agency.IssuedWeapon("weapon_revolver", true, new Agency.WeaponVariation()),
            new Agency.IssuedWeapon("weapon_heavypistol", true, new Agency.WeaponVariation(0,new List<string> { "Flashlight" })),
            new Agency.IssuedWeapon("weapon_pumpshotgun", false, new Agency.WeaponVariation()),
            new Agency.IssuedWeapon("weapon_pumpshotgun", false, new Agency.WeaponVariation(0,new List<string> { "Flashlight" })),

        };

        AgenciesList = new List<Agency>
        {
            new Agency("~b~", "LSPD", "Los Santos Police Department", "Blue", Agency.Classification.Police, true, true, StandardCops, LSPDVehicles, "LS ",AllWeapons),
            new Agency("~b~", "LSPD-ASD", "Los Santos Police Department - Air Support Division", "White", Agency.Classification.Police, true, false, PoliceAndSwat, PoliceHeliVehicles, "ASD ",BestWeapons) {MinWantedLevelSpawn = 3,MaxWantedLevelSpawn = 4 },
            new Agency("~y~", "SAHP", "San Andreas Highway Patrol", "Yellow", Agency.Classification.Police, true, true, SAHPPeds, SAHPVehicles, "HP ",LimitedWeapons) {SpawnsOnHighway = true },
            new Agency("~p~", "LSIAPD", "Los Santos International Airport Police Department", "LightBlue", Agency.Classification.Police, true, true, StandardCops, LSPDVehicles, "LSA ",AllWeapons),
            new Agency("~b~", "VPPD", "Vespucci Police Department", "DarkBlue", Agency.Classification.Police, false, true, StandardCops, VPPDVehicles, "VP ",AllWeapons),
            new Agency("~b~", "RHPD", "Rockford Hills Police Department", "LightBlue", Agency.Classification.Police, false, true, StandardCops, RHPDVehicles, "RH ",AllWeapons),
            new Agency("~b~", "LSPD-VW", "Los Santos Police - Vinewood Division", "Blue", Agency.Classification.Police, false, true, ExtendedStandardCops, VWPDVehicles, "LSV ",LimitedWeapons),
            new Agency("~b~", "LSPD-ELS", "Los Santos Police - East Los Santos Division", "Blue", Agency.Classification.Police, false, true, ExtendedStandardCops, EastLSPDVehicles, "LSE ",LimitedWeapons),
            new Agency("~b~", "LSPD-CH", "Los Santos Police - Chumash Division", "Blue", Agency.Classification.Police, false, true, StandardCops, ChumashLSPDVehicles, "LSC ",AllWeapons),
            new Agency("~r~", "LSSD", "Los Santos County Sheriff", "Red", Agency.Classification.Sheriff, true, true, SheriffPeds, LSSDVehicles, "LSCS ",LimitedWeapons),
            new Agency("~r~", "BCSO", "Blaine County Sheriffs Office", "DarkRed", Agency.Classification.Sheriff, false, true, SheriffPeds, BCSOVehicles, "BCS ",LimitedWeapons),
            new Agency("~r~", "LSSD-VW", "Los Santos Sheriff - Vinewood Division", "Red", Agency.Classification.Sheriff, false, true, SheriffPeds, VWHillsLSSDVehicles, "LSCS ",LimitedWeapons),
            new Agency("~r~", "LSSD-CH", "Los Santos Sheriff - Chumash Division", "Red", Agency.Classification.Sheriff, false, true, SheriffPeds, ChumashLSSDVehicles, "LSCS ",LimitedWeapons),
            new Agency("~g~", "SAPR", "San Andreas Park Ranger", "Green", Agency.Classification.Federal, true, true, ParkRangers, ParkRangerVehicles, "",AllWeapons),
            new Agency("~p~", "DOA", "Drug Observation Agency", "Purple", Agency.Classification.Federal, true, true, DOAPeds, UnmarkedVehicles, "DOA ",AllWeapons) { CanDispatchFromAnywhere = true },
            new Agency("~p~", "FIB", "Federal Investigation Bureau", "Purple", Agency.Classification.Federal, true, true, FIBPeds, FIBVehicles, "FIB ",BestWeapons) { CanDispatchFromAnywhere = true },
            new Agency("~p~", "IAA", "International Affairs Agency", "Purple", Agency.Classification.Federal, true, false, IAAPeds, UnmarkedVehicles, "IAA ",AllWeapons),
            new Agency("~u~", "ARMY", "Army", "Black", Agency.Classification.Federal, true, false, MilitaryPeds, ArmyVehicles, "",BestWeapons) {IsArmy = true },
            new Agency("~r~", "NOOSE", "National Office of Security Enforcement", "DarkRed", Agency.Classification.Federal, true, false, NOOSEPeds, NOOSEVehicles, "",BestWeapons) { CanDispatchFromAnywhere = true,MinWantedLevelSpawn = 3 },
            new Agency("~o~", "PRISEC", "Private Security", "White", Agency.Classification.Security, true, true, SecurityPeds, UnmarkedVehicles, "",LimitedWeapons),
            new Agency("~p~", "LSPA", "Port Authority of Los Santos", "LightGray", Agency.Classification.Security, true, true, SecurityPeds, UnmarkedVehicles, "LSPA ",LimitedWeapons),
            new Agency("~o~", "SASPA", "San Andreas State Prison Authority", "Orange", Agency.Classification.Other, true, true, PrisonPeds, PrisonVehicles, "SASPA ",AllWeapons),
            new Agency("~s~", "UNK", "Unknown Agency", "White", Agency.Classification.Other, true, false, null, null, "",null),
            new Agency("~o~", "SACG", "San Andreas Coast Guard", "DarkOrange", Agency.Classification.Other, true, false, CoastGuardPeds, UnmarkedVehicles, "SACG ",LimitedWeapons),
        };
      
    }
    public static Agency GetAgencyFromPed(Ped Cop)
    {
        if (!Cop.IsPoliceArmy())
            return null;
        if (Cop.IsArmy())
            return AgenciesList.Where(x => x.IsArmy).FirstOrDefault();
        else if (Cop.IsPolice())
        {
            Agency ToReturn;
            if (Cop.Model.Name.ToLower() == "s_m_y_swat_01")//Swat depends on unit insignias
             {
                ToReturn = GetAgencyFromSwat(Cop);
            }
            else if (Cop.Model.Name.ToLower() == "s_m_m_security_01")//Security depends on where they are
            {
                ToReturn = GetAgencyFromSecurity(Cop);
            }
            else
            {
                ToReturn = GetPedAgencyFromZone(Cop);
            }
            return ToReturn;
        }
        else
            return null;
    }
    private static Agency GetPedAgencyFromZone(Ped Cop)
    {
        Zone ZoneFound = Zones.GetZoneAtLocation(Cop.Position);
        Agency ZoneAgency = null;
        if (ZoneFound != null)
        {
            foreach (ZoneAgency MyAgency in ZoneFound.ZoneAgencies)
            {
                if (MyAgency.AssociatedAgency != null && MyAgency.AssociatedAgency.CopModels != null && MyAgency.AssociatedAgency.CopModels.Any())
                {
                    if (MyAgency.AssociatedAgency.CopModels.Any(x => x.ModelName.ToLower() == Cop.Model.Name.ToLower()))
                    {
                        ZoneAgency = MyAgency.AssociatedAgency;
                        break;
                    }
                }
            }
        }

        if (ZoneAgency == null)
            ZoneAgency = AgenciesList.Where(x => x.CopModels.Any(y => y.ModelName.ToLower() == Cop.Model.Name.ToLower())).FirstOrDefault();
        return ZoneAgency;
    }
    public static void ChangeLivery(Vehicle CopCar, Agency AssignedAgency)
    {
        Agency.VehicleInformation MyVehicle = null;
        if (AssignedAgency != null && AssignedAgency.Vehicles != null)
        {
            MyVehicle = AssignedAgency.Vehicles.Where(x => x.ModelName.ToLower() == CopCar.Model.Name.ToLower()).FirstOrDefault();
        }
        if (MyVehicle == null || MyVehicle.Liveries == null || !MyVehicle.Liveries.Any())
        {
            ChangeToDefaultLivery(CopCar);
            return;
        }
        //Debugging.WriteToLog("ChangeLivery", string.Format("Agency {0}, {1}, {2}", AssignedAgency.Initials, CopCar.Model.Name,string.Join(",", MyVehicle.Liveries.Select(x => x.ToString()))));
        int NewLiveryNumber = MyVehicle.Liveries.PickRandom();
        NativeFunction.CallByName<bool>("SET_VEHICLE_LIVERY", CopCar, NewLiveryNumber);
        
        CopCar.LicensePlate = AssignedAgency.LicensePlatePrefix + LosSantosRED.RandomString(8 - AssignedAgency.LicensePlatePrefix.Length);
    }
    public static void CheckandChangeLivery(Vehicle CopCar)
    {
        Zone ZoneFound = Zones.GetZoneAtLocation(CopCar.Position);
        Agency.VehicleInformation MyVehicle = null;
        Agency ZoneAgency = null;
        if (ZoneFound != null)
        {
            foreach (ZoneAgency MyAgency in ZoneFound.ZoneAgencies)
            {
                if (MyAgency.AssociatedAgency != null && MyAgency.AssociatedAgency.Vehicles != null && MyAgency.AssociatedAgency.Vehicles.Any())
                {
                    if (MyAgency.AssociatedAgency.Vehicles.Any(x => x.ModelName == CopCar.Model.Name.ToLower()))
                    {
                        ZoneAgency = MyAgency.AssociatedAgency;
                        break;
                    }
                }
            }
            if (ZoneAgency != null && ZoneAgency.Vehicles != null)
            {
                MyVehicle = ZoneAgency.Vehicles.Where(x => x.ModelName.ToLower() == CopCar.Model.Name.ToLower()).FirstOrDefault();
            }
        }
        if (MyVehicle == null || MyVehicle.Liveries == null || !MyVehicle.Liveries.Any())
        {
            ChangeToDefaultLivery(CopCar);
            return;
        }

        int LiveryNumber = NativeFunction.CallByName<int>("GET_VEHICLE_LIVERY", CopCar);
        int NewLiveryNumber = MyVehicle.Liveries.PickRandom();
        NativeFunction.CallByName<bool>("SET_VEHICLE_LIVERY", CopCar, NewLiveryNumber);
        if(ZoneAgency != null)
        {
            CopCar.LicensePlate = ZoneAgency.LicensePlatePrefix + LosSantosRED.RandomString(8 - ZoneAgency.LicensePlatePrefix.Length);
        }
        
    }
    public static void ChangeToDefaultLivery(Vehicle CopCar)
    {
        List<Agency.VehicleInformation> LSPDVehiclesVanilla = new List<Agency.VehicleInformation>() {
                new Agency.VehicleInformation("police", 25,25) {Liveries = new List<int>() { 0,1,2,3,4,5 } },
                new Agency.VehicleInformation("police2", 25, 25) {Liveries = new List<int>() { 0,1,2,3,4,5,6,7 } },
                new Agency.VehicleInformation("police3", 25, 25) {Liveries = new List<int>() { 0,1,2,3,4,5,6,7 } },
                new Agency.VehicleInformation("police4", 10, 10),
                new Agency.VehicleInformation("fbi2", 15, 15) };

        List<Agency.VehicleInformation> LSSDVehiclesVanilla = new List<Agency.VehicleInformation>() {
                new Agency.VehicleInformation("sheriff", 50, 50) { Liveries = new List<int> { 0, 1, 2, 3 } },
                new Agency.VehicleInformation("sheriff2", 50, 50) { Liveries = new List<int> { 0, 1, 2, 3 } } };


        Agency.VehicleInformation MyVehicle = LSPDVehiclesVanilla.Where(x => x.ModelName.ToLower() == CopCar.Model.Name.ToLower()).FirstOrDefault();
        if (MyVehicle == null)
            MyVehicle = LSSDVehiclesVanilla.Where(x => x.ModelName.ToLower() == CopCar.Model.Name.ToLower()).FirstOrDefault();

        if (MyVehicle == null)
            return;

        int LiveryNumber = NativeFunction.CallByName<int>("GET_VEHICLE_LIVERY", CopCar);
        int NewLiveryNumber = MyVehicle.Liveries.PickRandom();
        NativeFunction.CallByName<bool>("SET_VEHICLE_LIVERY", CopCar, NewLiveryNumber);
    }
    private static Agency GetAgencyFromSwat(Ped Cop)
    {
        string ModelNameToFind = Cop.Model.Name;
        return AgenciesList.Where(x => x.CopModels.Any(y => y.ModelName.ToLower() == ModelNameToFind.ToLower())).FirstOrDefault();
    }
    private static Agency GetAgencyFromSecurity(Ped Cop)
    {
        Zone PedZone = Zones.GetZoneAtLocation(Cop.Position);
        string ModelNameToFind = Cop.Model.Name;
        if (PedZone != null && PedZone.ZoneAgencies.Any())
        {
            return AgenciesList.Where(x => x.CopModels.Any(y => y.ModelName.ToLower() == ModelNameToFind.ToLower())).FirstOrDefault();//PedZone.ZoneAgencies.Where(x => x.AssociatedAgency != null && x.AssociatedAgency.CopModels.Any(y => y.ModelName.ToLower() == ModelNameToFind.ToLower())).OrderBy(x => x.Priority).FirstOrDefault().AssociatedAgency;
        }
        else
        {
            
            return AgenciesList.Where(x => x.CopModels.Any(y => y.ModelName.ToLower() == ModelNameToFind.ToLower())).FirstOrDefault();
        }
    }
    public static void PrintAgencies()
    {
        foreach(Agency MyAgency in AgenciesList)
        {
            Debugging.WriteToLog("AgencyPrint", string.Format("Name: {0}", MyAgency.FullName));
        }
    }
 }
[Serializable()]
public class Agency
{
    public string ColorPrefix = "~s~";
    public string Initials;
    public string FullName;
    public List<ModelInformation> CopModels;
    public List<VehicleInformation> Vehicles;
    public string AgencyColorString = "White";
    public bool IsVanilla = false;
    public Classification AgencyClassification;
    public bool CanSpawnAmbient = false;
    public string LicensePlatePrefix;
    public bool SpawnsOnHighway = false;
    public bool CanDispatchFromAnywhere = false;
    public bool IsArmy = false;
    public uint MinWantedLevelSpawn = 0;
    public uint MaxWantedLevelSpawn = 5;
    public List<IssuedWeapon> IssuedWeapons = new List<IssuedWeapon>();
    public bool IsDefault = false;
    public bool CanSpawn
    {
        get
        {
            if (LosSantosRED.PlayerWantedLevel >= MinWantedLevelSpawn && LosSantosRED.PlayerWantedLevel <= MaxWantedLevelSpawn)
                return true;
            else
                return false;
        }
    }
    public bool HasMotorcycles
    {
        get
        {
            return Vehicles.Any(x => x.IsMotorcycle);
        }
    }
    public bool HasSpawnableHelicopters
    {
        get
        {
            return Vehicles.Any(x => x.IsHelicopter && x.CanCurrentlySpawn);
        }
    }
    public Color AgencyColor
    {
        get
        {
            return Color.FromName(AgencyColorString);
        }
    }
    public string ColoredInitials
    {
        get
        {
            return ColorPrefix + Initials;
        }
    }
    public bool CanCheckTrafficViolations
    {
        get
        {
            if (AgencyClassification == Classification.Police || AgencyClassification == Classification.Federal || AgencyClassification == Classification.Sheriff)
                return true;
            else
                return false;
        }
    }
    public enum Classification
    {
        Police = 0,
        Sheriff = 1,
        Federal = 2,
        Security = 3,
        Other = 4,
    }
    public VehicleInformation GetVehicleInfo(Vehicle CopCar)
    {
        return Vehicles.Where(x => x.ModelName.ToLower() == CopCar.Model.Name.ToLower()).FirstOrDefault();
    }
    public VehicleInformation GetRandomVehicle(bool IsMotorcycle,bool IsHelicopter)
    {
        if (Vehicles == null || !Vehicles.Any())
            return null;

        List<VehicleInformation> ToPickFrom = Vehicles.Where(x => x.IsMotorcycle == IsMotorcycle && x.IsHelicopter == IsHelicopter && x.CanCurrentlySpawn).ToList();     
        int Total = ToPickFrom.Sum(x => x.CurrentSpawnChance);
       // Debugging.WriteToLog("GetRandomVehicle", string.Format("Total Chance {0}, Items {1}", Total, string.Join(",",ToPickFrom.Select( x => x.ModelName + " " + x.CanCurrentlySpawn + "  " + x.CurrentSpawnChance))));
        int RandomPick = LosSantosRED.MyRand.Next(0, Total);
        foreach (VehicleInformation Vehicle in ToPickFrom)
        {
            int SpawnChance = Vehicle.CurrentSpawnChance;
            if (RandomPick < SpawnChance)
            {
                return Vehicle;
            }
            RandomPick -= SpawnChance;
        }
        return null;
    }

    public ModelInformation GetRandomPed(List<string> RequiredModels)
    {
        if (CopModels == null || !CopModels.Any())
            return null;

        List<ModelInformation> ToPickFrom = CopModels.Where(x => LosSantosRED.PlayerWantedLevel >= x.MinWantedLevelSpawn && LosSantosRED.PlayerWantedLevel <= x.MaxWantedLevelSpawn).ToList();
        if(RequiredModels != null && RequiredModels.Any())
        {
            ToPickFrom = ToPickFrom.Where(x => RequiredModels.Contains(x.ModelName.ToLower())).ToList();
        }

        int Total = ToPickFrom.Sum(x => x.CurrentSpawnChance);
        Debugging.WriteToLog("GetRandomPed", string.Format("Total Chance {0}, Total Items {1}", Total, ToPickFrom.Count()));
        int RandomPick = LosSantosRED.MyRand.Next(0, Total);
        foreach (ModelInformation Cop in ToPickFrom)
        {
            int SpawnChance = Cop.CurrentSpawnChance;
            if (RandomPick < SpawnChance)
            {
                return Cop;
            }
            RandomPick -= SpawnChance;
        }
        return null;
    }

    public Agency()
    {

    }
    public Agency(string _ColorPrefix, string _Initials, string _FullName, string _AgencyColorString, Classification _AgencyClassification, bool _IsVanilla,bool _CanSpawnAmbient, List<ModelInformation> _CopModels, List<VehicleInformation> _Vehicles,string _LicensePlatePrefix, List<IssuedWeapon> _IssuedWeapons)
    {
        ColorPrefix = _ColorPrefix;
        Initials = _Initials;
        FullName = _FullName;
        CopModels = _CopModels;
        AgencyColorString = _AgencyColorString;
        IsVanilla = _IsVanilla;
        Vehicles = _Vehicles;
        AgencyClassification = _AgencyClassification;
        CanSpawnAmbient = _CanSpawnAmbient;
        LicensePlatePrefix = _LicensePlatePrefix;
        IssuedWeapons = _IssuedWeapons;
    }
    public class ModelInformation
    {
        public string ModelName;
        public int AmbientSpawnChance = 0;
        public int WantedSpawnChance = 0;
        public bool IsMale = true;
        public int MinWantedLevelSpawn = 0;
        public int MaxWantedLevelSpawn = 5;
        public PedVariation RequiredVariation;
        public bool CanCurrentlySpawn
        {
            get
            {
                if (LosSantosRED.PlayerIsWanted)
                {
                    if (LosSantosRED.PlayerWantedLevel >= MinWantedLevelSpawn && LosSantosRED.PlayerWantedLevel <= MaxWantedLevelSpawn)
                        return WantedSpawnChance > 0;
                    else
                        return false;
                }
                else
                    return AmbientSpawnChance > 0;
            }
        }
        public int CurrentSpawnChance
        {
            get
            {
                if (LosSantosRED.PlayerIsWanted)
                {
                    if (LosSantosRED.PlayerWantedLevel >= MinWantedLevelSpawn && LosSantosRED.PlayerWantedLevel <= MaxWantedLevelSpawn)
                        return WantedSpawnChance;
                    else
                        return 0;
                }
                else
                    return AmbientSpawnChance;
            }
        }

        public ModelInformation()
        {

        }
        public ModelInformation(string _ModelName, bool _isMale, int ambientSpawnChance, int wantedSpawnChance)
        {
            ModelName = _ModelName;
            IsMale = _isMale;
            AmbientSpawnChance = ambientSpawnChance;
            WantedSpawnChance = wantedSpawnChance;
        }
    }
    public class VehicleInformation
    {
        public string ModelName;
        public int AmbientSpawnChance = 0;
        public int WantedSpawnChance = 0;
        public bool IsMotorcycle = false;
        public bool IsHelicopter = false;
        public int MinOccupants = 1;
        public int MaxOccupants = 2;
        public int MinWantedLevelSpawn = 0;
        public int MaxWantedLevelSpawn = 5;
        public List<string> AllowedPedModels = new List<string>();//only ped models can spawn in this, if emptyt any ambient spawn can
        public List<int> Liveries = new List<int>();
        public bool CanCurrentlySpawn
        {
            get
            {
                if (LosSantosRED.PlayerIsWanted)
                {
                    if (LosSantosRED.PlayerWantedLevel >= MinWantedLevelSpawn && LosSantosRED.PlayerWantedLevel <= MaxWantedLevelSpawn)
                        return WantedSpawnChance > 0;
                    else
                        return false;
                }
                else
                    return AmbientSpawnChance > 0;
            }
        }
        public int CurrentSpawnChance
        {
            get
            {
                if (LosSantosRED.PlayerIsWanted)
                {
                    if (LosSantosRED.PlayerWantedLevel >= MinWantedLevelSpawn && LosSantosRED.PlayerWantedLevel <= MaxWantedLevelSpawn)
                        return WantedSpawnChance;
                    else
                        return 0;
                }
                else
                    return AmbientSpawnChance;
            }
        }
        public VehicleInformation()
        {

        }
        public VehicleInformation(string modelName, int ambientSpawnChance, int wantedSpawnChance)
        {
            ModelName = modelName;
            AmbientSpawnChance = ambientSpawnChance;
            WantedSpawnChance = wantedSpawnChance;
        }
    }
    public class IssuedWeapon
    {
        public string ModelName;
        public bool IsPistol = false;
        public WeaponVariation MyVariation = new WeaponVariation();
        public IssuedWeapon()
        {

        }
        public IssuedWeapon(string _ModelName, bool _IsPistol, WeaponVariation _MyVariation)
        {
            ModelName = _ModelName;
            IsPistol = _IsPistol;
            MyVariation = _MyVariation;
        }

    }
    public class WeaponVariation
    {
        public string Name;
        public int Tint;
        public List<string> Components = new List<string>();
        public WeaponVariation()
        {

        }
        public WeaponVariation(string _name, int _Tint, List<string> _Components)
        {
            Name = _name;
            Tint = _Tint;
            Components = _Components;
        }
        public WeaponVariation(string _name, int _Tint)
        {
            Name = _name;
            Tint = _Tint;
        }
        public WeaponVariation(int _Tint, List<string> _Components)
        {
            Tint = _Tint;
            Components = _Components;
        }
    }
}

