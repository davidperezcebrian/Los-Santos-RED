﻿using ExtensionsMethods;
using Rage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

public static class Zones
{
    private static string ConfigFileName = "Plugins\\LosSantosRED\\Zones.xml";

    public static List<Zone> ZoneList = new List<Zone>();

    public static void Initialize()
    {
        ReadConfig();
    }
    public static void ReadConfig()
    {
        if (File.Exists(ConfigFileName))
        {
            ZoneList = General.DeserializeParams<Zone>(ConfigFileName);
        }
        else
        {
            DefaultConfig();
            General.SerializeParams(ZoneList, ConfigFileName);
        }
    }
    private static void DefaultConfig()
    {

        //Blaine
        List<ZoneAgency> StandardBlaineAgencies = new List<ZoneAgency>() {
            new ZoneAgency("LSSD-BC", 0, 85, 75),
            new ZoneAgency("DOA", 1, 15, 5),
            new ZoneAgency("LSSD", 2, 0, 5),
            new ZoneAgency("NOOSE", 3, 0, 10),
            new ZoneAgency("LSSD-ASD", 4, 0, 5),
            new ZoneAgency("PRISEC", 5, 0, 0)};

        List< ZoneAgency > BlaineParkRangerAgencies = new List<ZoneAgency>() {
            new ZoneAgency("SAPR", 0, 51, 20),
            new ZoneAgency("LSSD-BC", 1, 49, 65),
            new ZoneAgency("NOOSE", 2, 0, 10),
            new ZoneAgency("LSSD-ASD", 3, 0, 5),
            new ZoneAgency("PRISEC", 4, 0, 0)};

        List<ZoneAgency> BlainePrisonAgencies = new List<ZoneAgency>() {
            new ZoneAgency("SASPA",0, 100, 70),
            new ZoneAgency("LSSD-BC", 1, 0, 10),
            new ZoneAgency("NOOSE", 2, 0, 15),
            new ZoneAgency("LSSD-ASD", 3, 0, 5),
            new ZoneAgency("PRISEC", 4, 0, 0)};

        List<ZoneAgency> StandardCityAgencies = new List<ZoneAgency>() {
            new ZoneAgency("LSPD", 0, 94, 80),
            new ZoneAgency("FIB", 1, 4, 4),
            new ZoneAgency("DOA", 2, 1, 1),
            new ZoneAgency("NOOSE", 3, 0, 10),
            new ZoneAgency("LSPD-ASD", 4, 0, 5),
            new ZoneAgency("PRISEC", 5, 1, 0)};

        List<ZoneAgency> DavisAgencies = new List<ZoneAgency>() {
            new ZoneAgency("LSSD-DV", 0, 50, 40),
            new ZoneAgency("LSPD", 1, 50, 40),
            new ZoneAgency("NOOSE", 2, 0, 15),
            new ZoneAgency("LSPD-ASD", 4, 0, 5) };

        List<ZoneAgency> SecurityAgencies = new List<ZoneAgency>() {
            new ZoneAgency("PRISEC", 0, 100, 50),
            new ZoneAgency("NOOSE", 1, 0, 50) };

        List<string> VespucciAreaUnits = new List<string>() { DispatchScannerFiles.attention_all_area_units.VespucciAreaUnits.FileName, DispatchScannerFiles.attention_all_area_units.VespucciAreaUnits2.FileName };
        List<ZoneAgency> VespucciAgencies = new List<ZoneAgency>() {
            new ZoneAgency("LSPD-DP", 0, 84, 70),
            new ZoneAgency("LSPD", 1, 10, 10),
            new ZoneAgency("FIB", 2, 5, 5),
            new ZoneAgency("NOOSE", 3, 0, 10),
            new ZoneAgency("LSPD-ASD", 4, 0, 5),
            new ZoneAgency("PRISEC", 5, 0, 0)};

        List<string> CentralAreaUnits = new List<string>() { DispatchScannerFiles.attention_all_area_units.CentralUnits.FileName, DispatchScannerFiles.attention_all_area_units.CentralUnits1.FileName, DispatchScannerFiles.attention_all_area_units.CentralAreaUnits.FileName };
        List<string> EastLosSantosAreaUnits = new List<string>() { DispatchScannerFiles.attention_all_area_units.EastLosSantosUnits1.FileName, DispatchScannerFiles.attention_all_area_units.EastLosSantosUnits1.FileName, DispatchScannerFiles.attention_all_area_units.EastLosSantos2.FileName };
        List<ZoneAgency> EastLosSantosAgencies = new List<ZoneAgency>() {
            new ZoneAgency("LSPD-ELS", 0, 85, 70),
            new ZoneAgency("LSPD", 1, 10, 10),
            new ZoneAgency("FIB", 2, 5, 5),
            new ZoneAgency("NOOSE", 3, 0, 10),
            new ZoneAgency("LSPD-ASD", 4, 0, 5),
            new ZoneAgency("PRISEC", 5, 0, 0)};

        List<string> VinewoodAreaUnits = new List<string>() { DispatchScannerFiles.attention_all_area_units.VinewoodAreaUnits.FileName, DispatchScannerFiles.attention_all_area_units.VinewoodUnits2.FileName };
        List<ZoneAgency> VinewoodAgencies = new List<ZoneAgency>() {
            new ZoneAgency("LSPD-VW", 0, 75, 70),
            new ZoneAgency("LSSD-VW", 1, 10, 10),
            new ZoneAgency("FIB", 2, 10, 5),
            new ZoneAgency("NOOSE", 3, 0, 10),
            new ZoneAgency("LSPD-ASD", 4, 0, 5),
            new ZoneAgency("PRISEC", 5, 0, 0)};

        List<ZoneAgency> PortAgencies = new List<ZoneAgency>() {
            new ZoneAgency("LSPA", 0, 95, 80),
            new ZoneAgency("LSPD", 1, 5, 5),
            new ZoneAgency("NOOSE", 2, 0, 10),
            new ZoneAgency("LSPD-ASD", 3, 0, 5),
            new ZoneAgency("PRISEC", 4, 0, 0)};

        List<string> PortAreaUnits = new List<string>() { DispatchScannerFiles.attention_all_area_units.PortOfLosSantosUnits.FileName, DispatchScannerFiles.attention_all_area_units.PortOfLosSantosUnits1.FileName, DispatchScannerFiles.attention_all_area_units.PortOfLosSantosUnits2.FileName };

        List<ZoneAgency> RockfordHillsAgencies = new List<ZoneAgency>() {
            new ZoneAgency("LSPD-RH", 0, 85, 70),
            new ZoneAgency("FIB", 1, 10, 10),
            new ZoneAgency("LSPD", 2, 5, 5),
            new ZoneAgency("NOOSE", 3, 0, 10),
            new ZoneAgency("LSPD-ASD", 4, 0, 5),
            new ZoneAgency("PRISEC", 5, 0, 0)};

        List<string> VinewoodHillsAreaUnits = new List<string>() { DispatchScannerFiles.attention_all_area_units.VinewoodAreaUnits.FileName, DispatchScannerFiles.attention_all_area_units.VinewoodUnits2.FileName };

        List<ZoneAgency> VinewoodHillsAgencies = new List<ZoneAgency>() {
            new ZoneAgency("LSSD-VW", 0, 85, 70),
            new ZoneAgency("DOA", 1, 15, 15),
            new ZoneAgency("NOOSE", 2, 0, 10),
            new ZoneAgency("LSPD-ASD", 3, 0, 5),
            new ZoneAgency("PRISEC", 4, 0, 0)};

        List<string> ChumashsAreaUnits = new List<string>() { DispatchScannerFiles.attention_all_area_units.ChumashUnits.FileName, DispatchScannerFiles.attention_all_area_units.ChumashUnits1.FileName, DispatchScannerFiles.attention_all_area_units.ChumashUnits2.FileName };
        List<ZoneAgency> ChumashAgencies = new List<ZoneAgency>() {
            new ZoneAgency("LSSD-CH", 0, 85, 70),
            new ZoneAgency("LSPD-CH", 1, 10, 5),
            new ZoneAgency("DOA", 2, 5, 5),
            new ZoneAgency("NOOSE", 3, 0, 15),
            new ZoneAgency("LSPD-ASD", 4, 0, 5),
            new ZoneAgency("PRISEC", 5, 0, 0)};

        List<ZoneAgency> StandardSheriffAgencies = new List<ZoneAgency>() {
            new ZoneAgency("LSSD", 0, 85, 70),
            new ZoneAgency("DOA", 1, 15, 5),
            new ZoneAgency("NOOSE", 2, 0, 15),
            new ZoneAgency("LSSD-ASD", 3, 0, 10),
            new ZoneAgency("PRISEC", 4, 0, 0)};

        List<ZoneAgency> OceanAgencies = new List<ZoneAgency>() {
            new ZoneAgency("SACG", 0, 95, 80),
            new ZoneAgency("LSPD", 1, 5, 5),
            new ZoneAgency("NOOSE", 2, 0, 15),
            new ZoneAgency("PRISEC", 3, 0, 0)
        };

        List<ZoneAgency> AirportAgencies = new List<ZoneAgency>() {
            new ZoneAgency("LSIAPD", 0, 95, 80),
            new ZoneAgency("LSPD", 1, 5, 5),
            new ZoneAgency("NOOSE", 2, 0, 10),
            new ZoneAgency("LSPD-ASD", 3, 0, 5),
            new ZoneAgency("PRISEC", 4, 1, 0),
            new ZoneAgency("SACG", 5, 1, 0)};

        List<ZoneAgency> ArmyAgencies = new List<ZoneAgency>() {
            new ZoneAgency("ARMY", 0, 100, 100) };

        ZoneList = new List<Zone>
        {
            //One Off
            new Zone("OCEANA", "Pacific Ocean", DispatchScannerFiles.areas.TheOcean.FileName, County.PacificOcean,"Pacific Coast") { ZoneAgencies = OceanAgencies },   

            //North Blaine
            new Zone("PROCOB", "Procopio Beach", DispatchScannerFiles.areas.ProcopioBeach.FileName, County.BlaineCounty,"Chiliad") { ZoneAgencies = StandardBlaineAgencies },
            new Zone("MTCHIL", "Mount Chiliad", DispatchScannerFiles.areas.MountChiliad.FileName, County.BlaineCounty,"Chiliad") { ZoneAgencies = BlaineParkRangerAgencies },
            new Zone("MTGORDO", "Mount Gordo", DispatchScannerFiles.areas.MountGordo.FileName, County.BlaineCounty,"Chiliad") { ZoneAgencies = BlaineParkRangerAgencies },
            new Zone("PALETO", "Paleto Bay", DispatchScannerFiles.areas.PaletoBay.FileName, County.BlaineCounty,"Chiliad", new List<string>() { DispatchScannerFiles.attention_all_area_units.PaletoaBayUnits.FileName, DispatchScannerFiles.attention_all_area_units.PaletoBayUnits.FileName, DispatchScannerFiles.attention_all_area_units.PaletoBayUnits3.FileName }, "Paleto Bay Units") { ZoneAgencies = StandardBlaineAgencies },
            new Zone("PALCOV", "Paleto Cove", DispatchScannerFiles.areas.PaletoBay.FileName, County.BlaineCounty,"Chiliad") { ZoneAgencies = StandardBlaineAgencies },
            new Zone("PALFOR", "Paleto Forest", DispatchScannerFiles.areas.PaletoForest.FileName, County.BlaineCounty,"Chiliad") { ZoneAgencies = StandardBlaineAgencies },
            new Zone("CMSW", "Chiliad Mountain State Wilderness", DispatchScannerFiles.areas.ChilliadMountainStWilderness.FileName, County.BlaineCounty,"Chiliad") { ZoneAgencies = BlaineParkRangerAgencies },
            new Zone("CALAFB", "Calafia Bridge", "", County.BlaineCounty,"Chiliad") { ZoneAgencies = StandardBlaineAgencies },
            new Zone("GALFISH", "Galilee", "", County.BlaineCounty,"Chiliad") { ZoneAgencies = StandardBlaineAgencies },
            new Zone("ELGORL", "El Gordo Lighthouse", DispatchScannerFiles.areas.MountGordo.FileName, County.BlaineCounty,"Chiliad") { ZoneAgencies = StandardBlaineAgencies },
            new Zone("GRAPES", "Grapeseed", DispatchScannerFiles.areas.Grapeseed.FileName, County.BlaineCounty,"Chiliad") { ZoneAgencies = StandardBlaineAgencies },
            new Zone("BRADP", "Braddock Pass", DispatchScannerFiles.areas.BraddockPass.FileName, County.BlaineCounty,"Chiliad") { ZoneAgencies = StandardBlaineAgencies },
            new Zone("BRADT", "Braddock Tunnel", DispatchScannerFiles.areas.TheBraddockTunnel.FileName, County.BlaineCounty,"Chiliad") { ZoneAgencies = StandardBlaineAgencies },
            new Zone("CCREAK", "Cassidy Creek", "", County.BlaineCounty,"Chiliad") { ZoneAgencies = StandardBlaineAgencies },

            //Blaine
            new Zone("ALAMO", "Alamo Sea", DispatchScannerFiles.areas.TheAlamaSea.FileName, County.BlaineCounty,"Midlands") { ZoneAgencies = StandardBlaineAgencies },
            new Zone("ARMYB", "Fort Zancudo", DispatchScannerFiles.areas.FtZancudo.FileName, County.BlaineCounty,"Midlands") { IsRestrictedDuringWanted = true, ZoneAgencies = ArmyAgencies },
            new Zone("CANNY", "Raton Canyon", DispatchScannerFiles.areas.RatonCanyon.FileName, County.BlaineCounty,"Midlands") { ZoneAgencies = StandardBlaineAgencies },
            
            new Zone("DESRT", "Grand Senora Desert", DispatchScannerFiles.areas.GrandeSonoranDesert.FileName, County.BlaineCounty,"Midlands") { ZoneAgencies = StandardBlaineAgencies },
      
            new Zone("HUMLAB", "Humane Labs and Research", "", County.BlaineCounty,"Midlands") { ZoneAgencies = StandardBlaineAgencies },
            new Zone("JAIL", "Bolingbroke Penitentiary", DispatchScannerFiles.areas.BoilingBrookPenitentiary.FileName, County.BlaineCounty,"Midlands") { ZoneAgencies = BlainePrisonAgencies,IsRestrictedDuringWanted = true },
            new Zone("LAGO", "Lago Zancudo", DispatchScannerFiles.areas.LagoZancudo.FileName, County.BlaineCounty,"Midlands") { ZoneAgencies = StandardBlaineAgencies },
            new Zone("MTJOSE", "Mount Josiah", DispatchScannerFiles.areas.MtJosiah.FileName, County.BlaineCounty,"Midlands") { ZoneAgencies = StandardBlaineAgencies },
            new Zone("NCHU", "North Chumash", DispatchScannerFiles.areas.NorthChumash.FileName, County.BlaineCounty,"Midlands") { ZoneAgencies = StandardBlaineAgencies },   
            new Zone("SANCHIA", "San Chianski Mountain Range", "", County.BlaineCounty,"Midlands") { ZoneAgencies = BlaineParkRangerAgencies },
            new Zone("SANDY", "Sandy Shores", DispatchScannerFiles.areas.SandyShores.FileName, County.BlaineCounty,"Midlands", new List<string>() { DispatchScannerFiles.attention_all_area_units.SandyShoreUnits.FileName, DispatchScannerFiles.attention_all_area_units.SandyShoresUnits2.FileName, DispatchScannerFiles.attention_all_area_units.SanyShoreUnits3.FileName }, "Sandy Shores Units") { ZoneAgencies = StandardBlaineAgencies },
            new Zone("SLAB", "Slab City", DispatchScannerFiles.areas.SlabCity.FileName, County.BlaineCounty,"Midlands") { ZoneAgencies = StandardBlaineAgencies },
            new Zone("ZANCUDO", "Zancudo River", DispatchScannerFiles.areas.ZancudoRiver.FileName, County.BlaineCounty,"Midlands", new List<string>() { DispatchScannerFiles.attention_all_area_units.ZancudoRiverUnits.FileName }, "Zancudo River Units") { ZoneAgencies = BlaineParkRangerAgencies },
            new Zone("ZQ_UAR", "Davis Quartz", DispatchScannerFiles.areas.DavisCourts.FileName, County.BlaineCounty,"Midlands") { ZoneAgencies = StandardBlaineAgencies },

            //Vespucci
            new Zone("BEACH", "Vespucci Beach", DispatchScannerFiles.areas.VespucciBeach.FileName, County.CityOfLosSantos,"Vespucci", VespucciAreaUnits, "Vespucci Area Units") { ZoneAgencies = VespucciAgencies },
            new Zone("DELBE", "Del Perro Beach", DispatchScannerFiles.areas.DelPierroBeach.FileName, County.CityOfLosSantos,"Vespucci", VespucciAreaUnits, "Vespucci Area Units") { ZoneAgencies = VespucciAgencies },
            new Zone("DELPE", "Del Perro", DispatchScannerFiles.areas.DelPierro.FileName, County.CityOfLosSantos,"Vespucci", VespucciAreaUnits, "Vespucci Area Units") { ZoneAgencies = VespucciAgencies },
            new Zone("VCANA", "Vespucci Canals", DispatchScannerFiles.areas.VespucciCanal.FileName, County.CityOfLosSantos,"Vespucci", VespucciAreaUnits, "Vespucci Area Units") { ZoneAgencies = VespucciAgencies },
            new Zone("VESP", "Vespucci Metro", DispatchScannerFiles.areas.Vespucci.FileName, County.CityOfLosSantos,"Vespucci", VespucciAreaUnits, "Vespucci Area Units") { ZoneAgencies = VespucciAgencies },
            new Zone("LOSPUER", "La Puerta", DispatchScannerFiles.areas.LaPuertes.FileName, County.CityOfLosSantos,"Vespucci", VespucciAreaUnits, "Vespucci Area Units") { ZoneAgencies = VespucciAgencies },
            new Zone("PBLUFF", "Pacific Bluffs", DispatchScannerFiles.areas.PacificBluffs.FileName, County.CityOfLosSantos,"Vespucci", VespucciAreaUnits, "Vespucci Area Units") { ZoneAgencies = VespucciAgencies },
            new Zone("DELSOL", "Puerto Del Sol", DispatchScannerFiles.areas.PuertoDelSoul.FileName, County.CityOfLosSantos,"Vespucci", VespucciAreaUnits, "Vespucci Area Units") { ZoneAgencies = VespucciAgencies },

            //Central
            new Zone("BANNING", "Banning", DispatchScannerFiles.areas.Banning.FileName, County.CityOfLosSantos,"Central", CentralAreaUnits, "Central Area Units") { ZoneAgencies = DavisAgencies },
            new Zone("CHAMH", "Chamberlain Hills", DispatchScannerFiles.areas.ChamberlainHills.FileName, County.CityOfLosSantos,"Central", CentralAreaUnits, "Central Area Units") { ZoneAgencies = DavisAgencies },
            new Zone("DAVIS", "Davis", DispatchScannerFiles.areas.Davis.FileName, County.CityOfLosSantos,"Central", CentralAreaUnits, "Central Area Units") { ZoneAgencies = DavisAgencies },
            new Zone("DOWNT", "Downtown", DispatchScannerFiles.areas.Downtown.FileName, County.CityOfLosSantos,"Central", CentralAreaUnits, "Central Area Units") { ZoneAgencies = StandardCityAgencies },
            new Zone("PBOX", "Pillbox Hill", DispatchScannerFiles.areas.PillboxHill.FileName, County.CityOfLosSantos,"Central", CentralAreaUnits, "Central Area Units") { ZoneAgencies = StandardCityAgencies },
            new Zone("RANCHO", "Rancho", DispatchScannerFiles.areas.Rancho.FileName, County.CityOfLosSantos,"Central", CentralAreaUnits, "Central Area Units") { ZoneAgencies = DavisAgencies },
            new Zone("SKID", "Mission Row", DispatchScannerFiles.areas.MissionRow.FileName, County.CityOfLosSantos,"Central", CentralAreaUnits, "Central Area Units") { ZoneAgencies = StandardCityAgencies },
            new Zone("STAD", "Maze Bank Arena", DispatchScannerFiles.areas.MazeBankArena.FileName, County.CityOfLosSantos,"Central", CentralAreaUnits, "Central Area Units") { ZoneAgencies = SecurityAgencies },
            new Zone("STRAW", "Strawberry", DispatchScannerFiles.areas.Strawberry.FileName, County.CityOfLosSantos,"Central", CentralAreaUnits, "Central Area Units") { ZoneAgencies = DavisAgencies },
            new Zone("TEXTI", "Textile City", DispatchScannerFiles.areas.TextileCity.FileName, County.CityOfLosSantos,"Central", CentralAreaUnits, "Central Area Units") { ZoneAgencies = StandardCityAgencies },
            new Zone("LEGSQU", "Legion Square", "", County.CityOfLosSantos,"Central", CentralAreaUnits, "Central Area Units") { ZoneAgencies = StandardCityAgencies },

            //East LS
            new Zone("CYPRE", "Cypress Flats", DispatchScannerFiles.areas.CypressFlats.FileName, County.CityOfLosSantos,"East Los Santos", EastLosSantosAreaUnits, "East Los Santos Units") { ZoneAgencies = EastLosSantosAgencies },
            new Zone("LMESA", "La Mesa", DispatchScannerFiles.areas.LaMesa.FileName, County.CityOfLosSantos,"East Los Santos", EastLosSantosAreaUnits, "East Los Santos Units") { ZoneAgencies = EastLosSantosAgencies },
            new Zone("MIRR", "Mirror Park", DispatchScannerFiles.areas.MirrorPark.FileName, County.CityOfLosSantos,"East Los Santos", EastLosSantosAreaUnits, "East Los Santos Units") { ZoneAgencies = EastLosSantosAgencies },
            new Zone("MURRI", "Murrieta Heights", DispatchScannerFiles.areas.MuriettaHeights.FileName, County.CityOfLosSantos,"East Los Santos", EastLosSantosAreaUnits, "East Los Santos Units") { ZoneAgencies = EastLosSantosAgencies },
            new Zone("EBURO", "El Burro Heights", DispatchScannerFiles.areas.ElBerroHights.FileName, County.CityOfLosSantos,"East Los Santos", EastLosSantosAreaUnits, "East Los Santos Units") { ZoneAgencies = EastLosSantosAgencies },//was county

            //Vinewood
            new Zone("ALTA", "Alta", DispatchScannerFiles.areas.Alta.FileName, County.CityOfLosSantos,"Vinewood", VinewoodAreaUnits, "Vinewood Units") { ZoneAgencies = VinewoodAgencies },
            new Zone("DTVINE", "Downtown Vinewood", DispatchScannerFiles.areas.DowntownVinewood.FileName, County.CityOfLosSantos,"Vinewood", VinewoodAreaUnits, "Vinewood Units") { ZoneAgencies = VinewoodAgencies },
            new Zone("EAST_V", "East Vinewood", DispatchScannerFiles.areas.EastVinewood.FileName, County.CityOfLosSantos,"Vinewood", VinewoodAreaUnits, "Vinewood Units") { ZoneAgencies = VinewoodAgencies },
            new Zone("HAWICK", "Hawick", "", County.CityOfLosSantos,"Vinewood", VinewoodAreaUnits, "Vinewood Units") { ZoneAgencies = VinewoodAgencies },
            new Zone("HORS", "Vinewood Racetrack", DispatchScannerFiles.areas.TheRaceCourse.FileName, County.CityOfLosSantos,"Vinewood", VinewoodAreaUnits, "Vinewood Units") { ZoneAgencies = VinewoodAgencies },
            new Zone("VINE", "Vinewood", DispatchScannerFiles.areas.Vinewood.FileName, County.CityOfLosSantos,"Vinewood", VinewoodAreaUnits, "Vinewood Units") { ZoneAgencies = VinewoodAgencies },
            new Zone("WVINE", "West Vinewood", DispatchScannerFiles.areas.WestVinewood.FileName, County.CityOfLosSantos,"Vinewood", VinewoodAreaUnits, "Vinewood Units") { ZoneAgencies = VinewoodAgencies },

            //PortOfLosSantos
            new Zone("ELYSIAN", "Elysian Island", DispatchScannerFiles.areas.ElysianIsland.FileName, County.CityOfLosSantos,"Port Area", PortAreaUnits, "Port of Los Santos Units") { ZoneAgencies = PortAgencies },
            new Zone("ZP_ORT", "Port of South Los Santos", DispatchScannerFiles.areas.PortOfSouthLosSantos.FileName, County.CityOfLosSantos,"Port Area", PortAreaUnits, "Port of Los Santos Units") { ZoneAgencies = PortAgencies },
            new Zone("TERMINA", "Terminal", DispatchScannerFiles.areas.Terminal.FileName, County.CityOfLosSantos,"Port Area", PortAreaUnits, "Port of Los Santos Units") { ZoneAgencies = PortAgencies },
            new Zone("ZP_ORT", "Port of South Los Santos", DispatchScannerFiles.areas.PortOfSouthLosSantos.FileName, County.CityOfLosSantos,"Port Area", PortAreaUnits, "Port of Los Santos Units") { ZoneAgencies = PortAgencies },
            new Zone("AIRP", "Los Santos International Airport", DispatchScannerFiles.areas.LosSantosInternationalAirport.FileName, County.CityOfLosSantos,"Port Area") { IsRestrictedDuringWanted = true, ZoneAgencies = AirportAgencies },

            //Rockford Hills
            new Zone("BURTON", "Burton", DispatchScannerFiles.areas.Burton.FileName, County.CityOfLosSantos,"Rockford Hills") { ZoneAgencies = RockfordHillsAgencies },
            new Zone("GOLF", "GWC and Golfing Society", DispatchScannerFiles.areas.TheGWCGolfingSociety.FileName, County.CityOfLosSantos,"Rockford Hills") { ZoneAgencies = SecurityAgencies },
            new Zone("KOREAT", "Little Seoul", DispatchScannerFiles.areas.LittleSeoul.FileName, County.CityOfLosSantos,"Rockford Hills") { ZoneAgencies = RockfordHillsAgencies },
            new Zone("MORN", "Morningwood", DispatchScannerFiles.areas.MorningWood.FileName, County.CityOfLosSantos,"Rockford Hills") { ZoneAgencies = RockfordHillsAgencies },
            new Zone("MOVIE", "Richards Majestic", DispatchScannerFiles.areas.RichardsMajesticStudio.FileName, County.CityOfLosSantos,"Rockford Hills") { ZoneAgencies = SecurityAgencies },
            new Zone("RICHM", "Richman", DispatchScannerFiles.areas.Richman.FileName, County.CityOfLosSantos,"Rockford Hills") { ZoneAgencies = RockfordHillsAgencies },
            new Zone("ROCKF", "Rockford Hills", DispatchScannerFiles.areas.RockfordHills.FileName, County.CityOfLosSantos,"Rockford Hills") { ZoneAgencies = RockfordHillsAgencies },         

            //Vinewood Hills
            new Zone("CHIL", "Vinewood Hills", DispatchScannerFiles.areas.VinewoodHills.FileName, County.LosSantosCounty,"Vinewood Hills", VinewoodHillsAreaUnits, "Vinewood Hills Units") { ZoneAgencies = VinewoodHillsAgencies },
            new Zone("GREATC", "Great Chaparral", DispatchScannerFiles.areas.GreatChapparalle.FileName, County.LosSantosCounty,"Vinewood Hills", VinewoodHillsAreaUnits, "Vinewood Hills Units") { ZoneAgencies = VinewoodHillsAgencies },
            new Zone("BAYTRE", "Baytree Canyon", DispatchScannerFiles.areas.BayTreeCanyon.FileName, County.LosSantosCounty,"Vinewood Hills") { ZoneAgencies = VinewoodHillsAgencies },
            new Zone("RGLEN", "Richman Glen", DispatchScannerFiles.areas.RichmanGlenn.FileName, County.LosSantosCounty,"Vinewood Hills", VinewoodHillsAreaUnits, "Vinewood Hills Units") { ZoneAgencies = VinewoodHillsAgencies },
            new Zone("TONGVAV", "Tongva Valley", DispatchScannerFiles.areas.TongvaValley.FileName, County.LosSantosCounty,"Vinewood Hills", VinewoodHillsAreaUnits, "Vinewood Hills Units") { ZoneAgencies = VinewoodHillsAgencies },
            new Zone("HARMO", "Harmony", DispatchScannerFiles.areas.Harmony.FileName, County.LosSantosCounty,"Vinewood Hills") { ZoneAgencies = VinewoodHillsAgencies },
            new Zone("RTRAK", "Redwood Lights Track", DispatchScannerFiles.areas.TheRedwoodLightsTrack.FileName, County.LosSantosCounty,"Vinewood Hills") { ZoneAgencies = VinewoodHillsAgencies },
           
            //Chumash
            new Zone("BANHAMC", "Banham Canyon Dr", "", County.LosSantosCounty,"Chumash", ChumashsAreaUnits, "Chumash Units") { ZoneAgencies = ChumashAgencies },
            new Zone("BHAMCA", "Banham Canyon", "", County.LosSantosCounty,"Chumash", ChumashsAreaUnits, "Chumash Units") { ZoneAgencies = ChumashAgencies },
            new Zone("CHU", "Chumash", DispatchScannerFiles.areas.Chumash.FileName, County.LosSantosCounty,"Chumash", ChumashsAreaUnits, "Chumash Units") { ZoneAgencies = ChumashAgencies },
            new Zone("TONGVAH", "Tongva Hills", DispatchScannerFiles.areas.TongaHills.FileName, County.LosSantosCounty,"Chumash", ChumashsAreaUnits, "Chumash Units") { ZoneAgencies = ChumashAgencies },
           
            //Tataviam 
            new Zone("LACT", "Land Act Reservoir", "", County.LosSantosCounty,"Tataviam") { ZoneAgencies = StandardSheriffAgencies },
            new Zone("LDAM", "Land Act Dam", "", County.LosSantosCounty,"Tataviam") { ZoneAgencies = StandardSheriffAgencies },
            new Zone("NOOSE", "N.O.O.S.E", "", County.LosSantosCounty,"Tataviam") { IsRestrictedDuringWanted = true, ZoneAgencies = StandardSheriffAgencies },
            new Zone("PALHIGH", "Palomino Highlands", DispatchScannerFiles.areas.PalominoHighlands.FileName, County.LosSantosCounty,"Tataviam") { ZoneAgencies = StandardSheriffAgencies },
            new Zone("PALMPOW", "Palmer - Taylor Power Station", DispatchScannerFiles.areas.PalmerTaylorPowerStation.FileName, County.LosSantosCounty,"Tataviam") { ZoneAgencies = StandardSheriffAgencies },    
            new Zone("SANAND", "San Andreas", DispatchScannerFiles.areas.SanAndreas.FileName, County.LosSantosCounty,"San Andreas") { ZoneAgencies = StandardSheriffAgencies },
            new Zone("TATAMO", "Tataviam Mountains", DispatchScannerFiles.areas.TatathiaMountains.FileName, County.LosSantosCounty,"Tataviam") { ZoneAgencies = StandardSheriffAgencies },
            new Zone("WINDF", "Ron Alternates Wind Farm", DispatchScannerFiles.areas.RonAlternatesWindFarm.FileName, County.LosSantosCounty,"Tataviam") { ZoneAgencies = StandardSheriffAgencies },
    };
        
    }
    public static Zone GetZoneAtLocation(Vector3 ZonePosition)
    {        
        string zoneName = string.Empty;
        unsafe
        {
            IntPtr ptr = Rage.Native.NativeFunction.CallByName<IntPtr>("GET_NAME_OF_ZONE", ZonePosition.X, ZonePosition.Y, ZonePosition.Z);

            zoneName = Marshal.PtrToStringAnsi(ptr);
        }
        Zone ListResult = ZoneList.Where(x => x.GameName.ToUpper() == zoneName.ToUpper()).FirstOrDefault();
        if(ListResult == null)
        {
            if (ZonePosition.IsInLosSantosCity())
                return new Zone("UNK_LSCITY", "Los Santos", "", County.CityOfLosSantos,"San Andreas");
            else
                return new Zone("UNK_LSCOUNTY", "Los Santos County", "", County.LosSantosCounty, "San Andreas");
        }
        else
        {
            return ListResult;
        }
    }
    public static string GetZoneStringAtLocation(Vector3 ZonePosition)
    {
        string zoneName;
        unsafe
        {
            IntPtr ptr = Rage.Native.NativeFunction.CallByName<IntPtr>("GET_NAME_OF_ZONE", ZonePosition.X, ZonePosition.Y, ZonePosition.Z);

            zoneName = Marshal.PtrToStringAnsi(ptr);
        }
        return zoneName;
    }
    public static string GetFormattedZoneName(Zone MyZone,bool WithCounty)
    {
        if (WithCounty)
        {
            string CountyName = "San Andreas";
            if (MyZone.ZoneCounty == County.BlaineCounty)
                CountyName = "Blaine County";
            else if (MyZone.ZoneCounty == County.CityOfLosSantos)
                CountyName = "City of Los Santos";
            else if (MyZone.ZoneCounty == County.LosSantosCounty)
                CountyName = "Los Santos County";

            return MyZone.TextName + ", " + MyZone.AreaName + ", " + CountyName;
        }
        else
        {
            return MyZone.TextName;
        }

    }
    //public ZoneAgency GetRandomVehicle(bool IsMotorcycle)
    //{
    //    if (Vehicles == null || !Vehicles.Any())
    //        return null;

    //    List<VehicleInformation> ToPickFrom = Vehicles.Where(x => x.IsMotorcycle == IsMotorcycle && LosSantosRED.PlayerWantedLevel >= x.MinWantedLevelSpawn && LosSantosRED.PlayerWantedLevel <= x.MaxWantedLevelSpawn).ToList();
    //    int Total = ToPickFrom.Sum(x => x.CurrentSpawnChance);
    //    int RandomPick = LosSantosRED.MyRand.Next(0, Total);
    //    foreach (VehicleInformation Vehicle in ToPickFrom)
    //    {
    //        int SpawnChance = Vehicle.CurrentSpawnChance;
    //        if (RandomPick < SpawnChance)
    //        {
    //            return Vehicle;
    //        }
    //        RandomPick -= SpawnChance;
    //    }
    //    return null;
    //}
}
[Serializable()]
public class Zone
{
    public Zone()
    {

    }
    public Zone(string _GameName, string _TextName, string _ScannerValue,County _ZoneCounty, string _AreaName)
    {
        GameName = _GameName;
        TextName = _TextName;
        ScannerValue = _ScannerValue;
        ZoneCounty = _ZoneCounty;
        AreaName = _AreaName;
    }
    public Zone(string _GameName, string _TextName, string _ScannerValue, County _ZoneCounty, string _AreaName, List<string> _DispatchUnitAudio, string _DispatchUnitName)
    {
        GameName = _GameName;
        TextName = _TextName;
        ScannerValue = _ScannerValue;
        ZoneCounty = _ZoneCounty;
        AreaName = _AreaName;
        DispatchUnitAudio = _DispatchUnitAudio;
        DispatchUnitName = _DispatchUnitName;
    }
    public string DispatchUnitName { get; set; }
    public string GameName { get; set; }
    public string TextName { get; set; }
    public string AreaName { get; set; }
    public County ZoneCounty { get; set; }
    public List<string> DispatchUnitAudio { get; set; } = new List<string>();
    public string ScannerValue { get; set; }
    public List<ZoneAgency> ZoneAgencies { get; set; }
    public bool IsRestrictedDuringWanted { get; set; } = false;
    public Agency GetRandomAgency()
    {
        if (ZoneAgencies == null || !ZoneAgencies.Any())
            return null;

        List<ZoneAgency> ToPickFrom = ZoneAgencies.Where(x => x.CanCurrentlySpawn).ToList();
        int Total = ToPickFrom.Sum(x => x.CurrentSpawnChance);
        int RandomPick = General.MyRand.Next(0, Total);
        foreach (ZoneAgency ZA in ToPickFrom)
        {
            int SpawnChance = ZA.CurrentSpawnChance;
            if (RandomPick < SpawnChance)
            {
                return ZA.AssociatedAgency;
            }
            RandomPick -= SpawnChance;
        }
        return null;
    }

    public Agency MainZoneAgency
    {
        get
        {
            if (HasAgencies)
                return ZoneAgencies.OrderBy(x => x.Priority).FirstOrDefault().AssociatedAgency;
            else
                return null;
        }
    }
    public bool HasAgencies
    {
        get
        {
            if (ZoneAgencies != null && ZoneAgencies.Any())
                return true;
            else
                return false;
        }
    }
}
public class ZoneAgency
{
    public string AssociatedAgencyName;
    public int Priority;
    public int AmbientSpawnChance = 0;
    public int WantedSpawnChance = 0;
    public ZoneAgency()
    {

    }
    public ZoneAgency(string associatedAgencyName, int priority, int ambientSpawnChance, int wantedSpawnChance)
    {
        AssociatedAgencyName = associatedAgencyName;
        Priority = priority;
        AmbientSpawnChance = ambientSpawnChance;
        WantedSpawnChance = wantedSpawnChance;
    }
    public bool CanCurrentlySpawn
    {
        get
        {
            if (PlayerState.IsWanted)
            {
                if (AssociatedAgency.CanSpawn)
                {
                    return WantedSpawnChance > 0;
                }
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
            if (PlayerState.IsWanted)
                return WantedSpawnChance;
            else
                return AmbientSpawnChance;
        }
    }
    public Agency AssociatedAgency
    {
        get
        {
           return Agencies.AgenciesList.Where(x => x.Initials == AssociatedAgencyName).FirstOrDefault();
        }
    }
}
public enum County
{
    CityOfLosSantos = 0,
    LosSantosCounty = 1,
    BlaineCounty = 2,
    PacificOcean = 3,
}
