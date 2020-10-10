﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using ExtensionsMethods;
using Rage.Native;

public static class Vehicles
{
    private static string ConfigFileName = "Plugins\\LosSantosRED\\Vehicles.xml";
    public static List<VehicleInfo> VehiclesList;
    public enum Manufacturer
    {
        Albany = 1,
        Annis = 2,
        Benefactor = 3,
        Burgerfahrzeug = 4,
        Bollokan = 5,
        Bravado = 6,
        Brute = 7,
        Buckingham = 8,
        Canis = 9,
        Chariot = 10,
        Cheval = 11,
        Classique = 12,
        Coil = 13,
        Declasse = 14,
        Dewbauchee = 15,
        Dinka = 16,
        DUDE = 17,
        Dundreary = 18,
        Emperor = 19,
        Enus = 20,
        Fathom = 21,
        Gallivanter = 22,
        Grotti = 23,
        HVY = 24,
        Hijak = 25,
        Imponte = 26,
        Invetero = 27,
        JackSheepe = 28,
        Jobuilt = 29,
        Karin = 30,
        KrakenSubmersibles = 31,
        Lampadati = 32,
        LibertyChopShop = 33,
        LibertyCityCycles = 34,
        MaibatsuCorporation = 35,
        Mammoth = 36,
        MTL = 37,
        Nagasaki = 38,
        Obey = 39,
        Ocelot = 40,
        Overflod = 41,
        Pegassi = 42,
        Pfister = 43,
        Principe = 44,
        Progen = 45,
        ProLaps = 46,
        RUNE = 47,
        Schyster = 48,
        Shitzu = 49,
        Speedophile = 50,
        Stanley = 51,
        SteelHorse = 52,
        Truffade = 53,
        Ubermacht = 54,
        Vapid = 55,
        Vulcar = 56,
        Vysser = 57,
        Weeny = 58,
        WesternCompany = 59,
        WesternMotorcycleCompany = 60,
        Willard = 61,
        Zirconium = 62,
        Unknown = 63,
    }
    public enum VehicleClass
    {
        Compacts = 0,
        Sedans = 1,
        SUVs = 2,
        Coupes = 3,
        Muscle = 4,
        SportsClassics = 5,
        Sports = 6,
        Super = 7,
        Motorcycles = 8,
        OffRoad = 9,
        Industrial = 10,
        Utility = 11,
        Vans = 12,
        Cycles = 13,
        Boats = 14,
        Helicopters = 15,
        Planes = 16,
        Service = 17,
        Emergency = 18,
        Military = 19,
        Commercial = 20,
        Trains = 21,


        Unknown = 24,//unofficial
        Trailer = 25,//unofficial

    }
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
            VehiclesList = General.DeserializeParams<VehicleInfo>(ConfigFileName);
        }
        else
        {
            DefaultConfig();
            General.SerializeParams(VehiclesList, ConfigFileName);
        }
    }

    public static Color GetVehicleColor( VehicleExt VehicleToDescribe)
    {
        if (VehicleToDescribe.VehicleEnt.Exists())
        {
            Color BaseColor = Extensions.GetBaseColor(VehicleToDescribe.DescriptionColor);
            return BaseColor;
        }
        else
        {
            return Color.White;
        }
    }
    public static string GetMakeName(VehicleExt VehicleToDescribe)
    {
        if (VehicleToDescribe.VehicleEnt.Exists())
        {
            string MakeName;
            unsafe
            {
                IntPtr ptr = NativeFunction.CallByHash<IntPtr>(0xF7AF4F159FF99F97, VehicleToDescribe.VehicleEnt.Model.Hash);
                MakeName = Marshal.PtrToStringAnsi(ptr);
            }
            unsafe
            {
                IntPtr ptr2 = NativeFunction.CallByHash<IntPtr>(0x7B5280EBA9840C72, MakeName);
                MakeName = Marshal.PtrToStringAnsi(ptr2);
            }
            if (MakeName == "CARNOTFOUND" || MakeName == "NULL")
                return null;
            else
                return MakeName;
        }
        else
        {
            return null;
        }

    }
    public static string GetModelName(VehicleExt VehicleToDescribe)
    {
        if (VehicleToDescribe.VehicleEnt.Exists())
        {
            string ModelName;
            unsafe
            {
                IntPtr ptr = NativeFunction.CallByName<IntPtr>("GET_DISPLAY_NAME_FROM_VEHICLE_MODEL", VehicleToDescribe.VehicleEnt.Model.Hash);
                ModelName = Marshal.PtrToStringAnsi(ptr);
            }
            unsafe
            {
                IntPtr ptr2 = NativeFunction.CallByHash<IntPtr>(0x7B5280EBA9840C72, ModelName);
                ModelName = Marshal.PtrToStringAnsi(ptr2);
            }
            if (ModelName == "CARNOTFOUND" || ModelName == "NULL")
                return null;
            else
                return ModelName;
        }
        else
        {
            return null;
        }
    }
    public static string GetClassName(VehicleExt VehicleToDescribe)
    {
        int ClassInt = NativeFunction.CallByName<int>("GET_VEHICLE_CLASS", VehicleToDescribe.VehicleEnt);
        return GetVehicleTypeSubtitle((VehicleClass)ClassInt);
    }
    public static VehicleInfo GetVehicleInfo(VehicleExt Vehicle)
    {
        if (VehiclesList == null)
            Initialize();
        VehicleInfo ToReturn = VehiclesList.Where(x => x.Hash == Vehicle.VehicleEnt.Model.Hash).FirstOrDefault();
        return ToReturn;
    }
    public static string GetVehicleTypeSubtitle(VehicleClass vehicleClass)
    {
        if (vehicleClass == VehicleClass.Compacts)
            return "Compact";
        else if (vehicleClass == VehicleClass.Coupes)
            return "Coupe";
        else if (vehicleClass == VehicleClass.Commercial)
            return "Commercial Vehicle";
        else if (vehicleClass == VehicleClass.Cycles)
            return "Bicycle";
        else if (vehicleClass == VehicleClass.Emergency)
            return "Emergency Vehicle";
        else if (vehicleClass == VehicleClass.Helicopters)
            return "Helicopter";
        else if (vehicleClass == VehicleClass.Industrial)
            return "Industrial Vehicle";
        else if (vehicleClass == VehicleClass.Military)
            return "Military Vehicle";
        else if (vehicleClass == VehicleClass.Motorcycles)
            return "Motorcycle";
        else if (vehicleClass == VehicleClass.Muscle)
            return "Muscle Car";
        else if (vehicleClass == VehicleClass.OffRoad)
            return "Off-Road Vehicle";
        else if (vehicleClass == VehicleClass.Planes)
            return "Plane";
        else if (vehicleClass == VehicleClass.Sedans)
            return "Sedan";
        else if (vehicleClass == VehicleClass.Service)
            return "Service Vehicle";
        else if (vehicleClass == VehicleClass.Sports)
            return "Sports Car";
        else if (vehicleClass == VehicleClass.SportsClassics)
            return "Classic Sports Car";
        else if (vehicleClass == VehicleClass.Super)
            return "Super Car";
        else if (vehicleClass == VehicleClass.SUVs)
            return "SUV";
        else if (vehicleClass == VehicleClass.Utility)
            return "Utility Vehicle";
        else if (vehicleClass == VehicleClass.Vans)
            return "Van";
        else
            return vehicleClass.ToString();
    }
    public static void DefaultConfig()
    {
        VehiclesList = new List<VehicleInfo>
        {
            new VehicleInfo("adder", 0xB779A091, Manufacturer.Truffade, VehicleClass.Super, DispatchScannerFiles.model.ADDER01.FileName),
            new VehicleInfo("airbus", 0x4C80EB0E, Manufacturer.Brute, VehicleClass.Service),
            new VehicleInfo("airtug", 0x5D0AAC8F, Manufacturer.Unknown, VehicleClass.Utility, DispatchScannerFiles.model.AIRTUG01.FileName),
            new VehicleInfo("akula", 0x46699F47, Manufacturer.Buckingham, VehicleClass.Helicopters),
            new VehicleInfo("akuma", 0x63ABADE7, Manufacturer.Dinka, VehicleClass.Motorcycles, DispatchScannerFiles.model.AKUMA01.FileName),
            new VehicleInfo("alpha", 0x2DB8D1AA, Manufacturer.Albany, VehicleClass.Sports),
            new VehicleInfo("alphaz1", 0xA52F6866, Manufacturer.Buckingham, VehicleClass.Planes),
            new VehicleInfo("ambulance", 0x45D56ADA, Manufacturer.Brute, VehicleClass.Emergency),
            new VehicleInfo("annihilator", 0x31F0B376, Manufacturer.WesternCompany, VehicleClass.Helicopters),
            new VehicleInfo("apc", 0x2189D250, Manufacturer.HVY, VehicleClass.Military),
            new VehicleInfo("ardent", 0x97E5533, Manufacturer.Ocelot, VehicleClass.SportsClassics),
            new VehicleInfo("armytanker", 0xB8081009, Manufacturer.Unknown, VehicleClass.Trailer),
            new VehicleInfo("armytrailer2", 0x9E6B14D6, Manufacturer.Unknown, VehicleClass.Trailer),
            new VehicleInfo("asea", 0x94204D89, Manufacturer.Declasse, VehicleClass.Sedans, DispatchScannerFiles.model.ASEA01.FileName),
            new VehicleInfo("asea2", 0x9441D8D5, Manufacturer.Declasse, VehicleClass.Sedans, DispatchScannerFiles.model.ASEA01.FileName),
            new VehicleInfo("asterope", 0x8E9254FB, Manufacturer.Karin, VehicleClass.Sedans, DispatchScannerFiles.model.ASTEROPE01.FileName),
            new VehicleInfo("autarch", 0xED552C74, Manufacturer.Overflod, VehicleClass.Super),
            new VehicleInfo("avarus", 0x81E38F7F, Manufacturer.LibertyCityCycles, VehicleClass.Motorcycles),
            new VehicleInfo("avenger", 0x81BD2ED0, Manufacturer.Mammoth, VehicleClass.Planes),
            new VehicleInfo("bagger", 0x806B9CC3, Manufacturer.WesternMotorcycleCompany, VehicleClass.Motorcycles, DispatchScannerFiles.model.BAGGER01.FileName),
            new VehicleInfo("baletrailer", 0xE82AE656, Manufacturer.Unknown, VehicleClass.Trailer),
            new VehicleInfo("baller", 0xCFCA3668, Manufacturer.Gallivanter, VehicleClass.SUVs, DispatchScannerFiles.model.BALLER01.FileName),
            new VehicleInfo("baller2", 0x8852855, Manufacturer.Gallivanter, VehicleClass.SUVs, DispatchScannerFiles.model.BALLER01.FileName),
            new VehicleInfo("baller3", 0x6FF0F727, Manufacturer.Gallivanter, VehicleClass.SUVs, DispatchScannerFiles.model.BALLER01.FileName),
            new VehicleInfo("baller4", 0x25CBE2E2, Manufacturer.Gallivanter, VehicleClass.SUVs, DispatchScannerFiles.model.BALLER01.FileName),
            new VehicleInfo("baller5", 0x1C09CF5E, Manufacturer.Gallivanter, VehicleClass.SUVs, DispatchScannerFiles.model.BALLER01.FileName),
            new VehicleInfo("baller6", 0x27B4E6B0, Manufacturer.Gallivanter, VehicleClass.SUVs, DispatchScannerFiles.model.BALLER01.FileName),
            new VehicleInfo("banshee", 0xC1E908D2, Manufacturer.Bravado, VehicleClass.Sports, DispatchScannerFiles.model.BANSHEE01.FileName),
            new VehicleInfo("banshee2", 0x25C5AF13, Manufacturer.Bravado, VehicleClass.Super, DispatchScannerFiles.model.BANSHEE01.FileName),
            new VehicleInfo("barracks", 0xCEEA3F4B, Manufacturer.HVY, VehicleClass.Military, DispatchScannerFiles.model.BARRACKS01.FileName),
            new VehicleInfo("barracks2", 0x4008EABB, Manufacturer.HVY, VehicleClass.Military, DispatchScannerFiles.model.BARRACKS01.FileName),
            new VehicleInfo("barracks3", 0x2592B5CF, Manufacturer.HVY, VehicleClass.Military, DispatchScannerFiles.model.BARRACKS01.FileName),
            new VehicleInfo("barrage", 0xF34DFB25, Manufacturer.HVY, VehicleClass.Military),
            new VehicleInfo("bati", 0xF9300CC5, Manufacturer.Pegassi, VehicleClass.Motorcycles, DispatchScannerFiles.model.BATI01.FileName),
            new VehicleInfo("bati2", 0xCADD5D2D, Manufacturer.Pegassi, VehicleClass.Motorcycles, DispatchScannerFiles.model.BATI01.FileName),
            new VehicleInfo("benson", 0x7A61B330, Manufacturer.Vapid, VehicleClass.Commercial, DispatchScannerFiles.model.BENSON01.FileName),
            new VehicleInfo("besra", 0x6CBD1D6D, Manufacturer.WesternCompany, VehicleClass.Planes),
            new VehicleInfo("bestiagts", 0x4BFCF28B, Manufacturer.Grotti, VehicleClass.Sports),
            new VehicleInfo("bf400", 0x5283265, Manufacturer.Nagasaki, VehicleClass.Motorcycles),
            new VehicleInfo("bfinjection", 0x432AA566, Manufacturer.Burgerfahrzeug, VehicleClass.OffRoad, DispatchScannerFiles.model.BFINJECTION01.FileName),
            new VehicleInfo("biff", 0x32B91AE8, Manufacturer.HVY, VehicleClass.Commercial, DispatchScannerFiles.model.BIFF01.FileName),
            new VehicleInfo("bifta", 0xEB298297, Manufacturer.Burgerfahrzeug, VehicleClass.OffRoad),
            new VehicleInfo("bison", 0xFEFD644F, Manufacturer.Bravado, VehicleClass.Vans, DispatchScannerFiles.model.BISON01.FileName),
            new VehicleInfo("bison2", 0x7B8297C5, Manufacturer.Bravado, VehicleClass.Vans, DispatchScannerFiles.model.BISON01.FileName),
            new VehicleInfo("bison3", 0x67B3F020, Manufacturer.Bravado, VehicleClass.Vans, DispatchScannerFiles.model.BISON01.FileName),
            new VehicleInfo("bjxl", 0x32B29A4B, Manufacturer.Karin, VehicleClass.SUVs, DispatchScannerFiles.model.BJXL01.FileName),
            new VehicleInfo("blade", 0xB820ED5E, Manufacturer.Grotti, VehicleClass.Muscle),
            new VehicleInfo("blazer", 0x8125BCF9, Manufacturer.Nagasaki, VehicleClass.OffRoad, DispatchScannerFiles.model.BLAZER01.FileName),
            new VehicleInfo("blazer2", 0xFD231729, Manufacturer.Nagasaki, VehicleClass.OffRoad, DispatchScannerFiles.model.BLAZER01.FileName),
            new VehicleInfo("blazer3", 0xB44F0582, Manufacturer.Nagasaki, VehicleClass.OffRoad, DispatchScannerFiles.model.BLAZER01.FileName),
            new VehicleInfo("blazer4", 0xE5BA6858, Manufacturer.Nagasaki, VehicleClass.OffRoad, DispatchScannerFiles.model.BLAZER01.FileName),
            new VehicleInfo("blazer5", 0xA1355F67, Manufacturer.Nagasaki, VehicleClass.OffRoad, DispatchScannerFiles.model.BLAZER01.FileName),
            new VehicleInfo("blimp", 0xF7004C86, Manufacturer.Unknown, VehicleClass.Planes),
            new VehicleInfo("blimp2", 0xDB6B4924, Manufacturer.Unknown, VehicleClass.Planes),
            new VehicleInfo("blimp3", 0xEDA4ED97, Manufacturer.Unknown, VehicleClass.Planes),
            new VehicleInfo("blista", 0xEB70965F, Manufacturer.Dinka, VehicleClass.Compacts, DispatchScannerFiles.model.BLISTA01.FileName),
            new VehicleInfo("blista2", 0x3DEE5EDA, Manufacturer.Dinka, VehicleClass.Sports, DispatchScannerFiles.model.BLISTA01.FileName),
            new VehicleInfo("blista3", 0xDCBC1C3B, Manufacturer.Dinka, VehicleClass.Sports, DispatchScannerFiles.model.BLISTA01.FileName),
            new VehicleInfo("bmx", 0x43779C54, Manufacturer.Unknown, VehicleClass.Cycles),
            new VehicleInfo("boattrailer", 0x1F3D44B5, Manufacturer.Unknown, VehicleClass.Trailer),
            new VehicleInfo("bobcatxl", 0x3FC5D440, Manufacturer.Vapid, VehicleClass.Vans, DispatchScannerFiles.model.BOBCATXL01.FileName),
            new VehicleInfo("bodhi2", 0xAA699BB6, Manufacturer.Canis, VehicleClass.OffRoad, DispatchScannerFiles.model.BODHI01.FileName),
            new VehicleInfo("bombushka", 0xFE0A508C, Manufacturer.Unknown, VehicleClass.Planes),
            new VehicleInfo("boxville", 0x898ECCEA, Manufacturer.Brute, VehicleClass.Vans, DispatchScannerFiles.model.BOXVILLE01.FileName),
            new VehicleInfo("boxville2", 0xF21B33BE, Manufacturer.Brute, VehicleClass.Vans, DispatchScannerFiles.model.BOXVILLE01.FileName),
            new VehicleInfo("boxville3", 0x07405E08, Manufacturer.Brute, VehicleClass.Vans, DispatchScannerFiles.model.BOXVILLE01.FileName),
            new VehicleInfo("boxville4", 0x1A79847A, Manufacturer.Brute, VehicleClass.Vans, DispatchScannerFiles.model.BOXVILLE01.FileName),
            new VehicleInfo("boxville5", 0x28AD20E1, Manufacturer.Brute, VehicleClass.Vans, DispatchScannerFiles.model.BOXVILLE01.FileName),
            new VehicleInfo("brawler", 0xA7CE1BC5, Manufacturer.Coil, VehicleClass.OffRoad),
            new VehicleInfo("brickade", 0xEDC6F847, Manufacturer.HVY, VehicleClass.Service),
            new VehicleInfo("brioso", 0x5C55CB39, Manufacturer.Grotti, VehicleClass.Compacts),
            new VehicleInfo("bruiser", 0x27D79225, Manufacturer.Benefactor, VehicleClass.OffRoad),
            new VehicleInfo("bruiser2", 0x9B065C9E, Manufacturer.Benefactor, VehicleClass.OffRoad),
            new VehicleInfo("bruiser3", 0x8644331A, Manufacturer.Benefactor, VehicleClass.OffRoad),
            new VehicleInfo("btype", 0x6FF6914, Manufacturer.Albany, VehicleClass.SportsClassics),
            new VehicleInfo("btype2", 0xCE6B35A4, Manufacturer.Albany, VehicleClass.SportsClassics),
            new VehicleInfo("btype3", 0xDC19D101, Manufacturer.Albany, VehicleClass.SportsClassics),
            new VehicleInfo("buccaneer", 0xD756460C, Manufacturer.Albany, VehicleClass.Muscle, DispatchScannerFiles.model.BUCCANEER01.FileName),
            new VehicleInfo("buccaneer2", 0xC397F748, Manufacturer.Albany, VehicleClass.Muscle, DispatchScannerFiles.model.BUCCANEER01.FileName),
            new VehicleInfo("buffalo", 0xEDD516C6, Manufacturer.Bravado, VehicleClass.Sports, DispatchScannerFiles.model.BUFFALO01.FileName),
            new VehicleInfo("buffalo2", 0x2BEC3CBE, Manufacturer.Bravado, VehicleClass.Sports, DispatchScannerFiles.model.BUFFALO01.FileName),
            new VehicleInfo("buffalo3", 0xE2C013E, Manufacturer.Bravado, VehicleClass.Sports, DispatchScannerFiles.model.BUFFALO01.FileName),
            new VehicleInfo("bulldozer", 0x7074F39D, Manufacturer.HVY, VehicleClass.Industrial),
            new VehicleInfo("bullet", 0x9AE6DDA1, Manufacturer.Vapid, VehicleClass.Super, DispatchScannerFiles.model.BULLET01.FileName),
            new VehicleInfo("burrito", 0xAFBB2CA4, Manufacturer.Declasse, VehicleClass.Vans, DispatchScannerFiles.model.BURRITO01.FileName),
            new VehicleInfo("burrito2", 0xC9E8FF76, Manufacturer.Declasse, VehicleClass.Vans, DispatchScannerFiles.model.BURRITO01.FileName),
            new VehicleInfo("burrito3", 0x98171BD3, Manufacturer.Declasse, VehicleClass.Vans, DispatchScannerFiles.model.BURRITO01.FileName),
            new VehicleInfo("burrito4", 0x353B561D, Manufacturer.Declasse, VehicleClass.Vans, DispatchScannerFiles.model.BURRITO01.FileName),
            new VehicleInfo("burrito5", 0x437CF2A0, Manufacturer.Declasse, VehicleClass.Vans, DispatchScannerFiles.model.BURRITO01.FileName),
            new VehicleInfo("bus", 0xD577C962, Manufacturer.Brute, VehicleClass.Service, DispatchScannerFiles.model.BUS01.FileName),
            new VehicleInfo("buzzard", 0x2F03547B, Manufacturer.Nagasaki, VehicleClass.Helicopters),
            new VehicleInfo("buzzard2", 0x2C75F0DD, Manufacturer.Nagasaki, VehicleClass.Helicopters),
            new VehicleInfo("cablecar", 0xC6C3242D, Manufacturer.Unknown, VehicleClass.Trailer),
            new VehicleInfo("caddy", 0x44623884, Manufacturer.Nagasaki, VehicleClass.Utility, DispatchScannerFiles.model.CADDY01.FileName),
            new VehicleInfo("caddy2", 0xDFF0594C, Manufacturer.Nagasaki, VehicleClass.Utility, DispatchScannerFiles.model.CADDY01.FileName),
            new VehicleInfo("caddy3", 0xD227BDBB, Manufacturer.Nagasaki, VehicleClass.Utility, DispatchScannerFiles.model.CADDY01.FileName),
            new VehicleInfo("camper", 0x6FD95F68, Manufacturer.Brute, VehicleClass.Vans, DispatchScannerFiles.model.CAMPER01.FileName),

            new VehicleInfo("caracara2", 0xAF966F3C, Manufacturer.Vapid, VehicleClass.OffRoad),

            new VehicleInfo("carbonizzare", 0x7B8AB45F, Manufacturer.Grotti, VehicleClass.Sports, DispatchScannerFiles.model.CARBONIZZARE01.FileName),
            new VehicleInfo("carbonrs", 0xABB0C0, Manufacturer.Nagasaki, VehicleClass.Motorcycles, DispatchScannerFiles.model.CARBONRS01.FileName),
            new VehicleInfo("cargobob", 0xFCFCB68B, Manufacturer.WesternCompany, VehicleClass.Helicopters),
            new VehicleInfo("cargobob2", 0x60A7EA10, Manufacturer.WesternCompany, VehicleClass.Helicopters),
            new VehicleInfo("cargobob3", 0x53174EEF, Manufacturer.WesternCompany, VehicleClass.Helicopters),
            new VehicleInfo("cargobob4", 0x78BC1A3C, Manufacturer.WesternCompany, VehicleClass.Helicopters),
            new VehicleInfo("cargoplane", 0x15F27762, Manufacturer.Unknown, VehicleClass.Planes),
            new VehicleInfo("casco", 0x3822BDFE, Manufacturer.Lampadati, VehicleClass.SportsClassics),
            new VehicleInfo("cavalcade", 0x779F23AA, Manufacturer.Albany, VehicleClass.SUVs, DispatchScannerFiles.model.CAVALCADE01.FileName),
            new VehicleInfo("cavalcade2", 0xD0EB2BE5, Manufacturer.Albany, VehicleClass.SUVs, DispatchScannerFiles.model.CAVALCADE01.FileName),
            new VehicleInfo("cerberus", 0xD039510B, Manufacturer.MTL, VehicleClass.Commercial),
            new VehicleInfo("cerberus2", 0x287FA449, Manufacturer.MTL, VehicleClass.Commercial),
            new VehicleInfo("cerberus3", 0x71D3B6F0, Manufacturer.MTL, VehicleClass.Commercial),
            new VehicleInfo("cheburek", 0xC514AAE0, Manufacturer.RUNE, VehicleClass.SportsClassics),
            new VehicleInfo("cheetah", 0xB1D95DA0, Manufacturer.Grotti, VehicleClass.Super, DispatchScannerFiles.model.CHEETAH01.FileName),
            new VehicleInfo("cheetah2", 0xD4E5F4D, Manufacturer.Grotti, VehicleClass.SportsClassics, DispatchScannerFiles.model.CHEETAH01.FileName),
            new VehicleInfo("chernobog", 0xD6BC7523, Manufacturer.HVY, VehicleClass.Military),
            new VehicleInfo("chimera", 0x675ED7, Manufacturer.Nagasaki, VehicleClass.Motorcycles),
            new VehicleInfo("chino", 0x14D69010, Manufacturer.Vapid, VehicleClass.Muscle),
            new VehicleInfo("chino2", 0xAED64A63, Manufacturer.Vapid, VehicleClass.Muscle),
            new VehicleInfo("cliffhanger", 0x17420102, Manufacturer.WesternMotorcycleCompany, VehicleClass.Motorcycles),
            new VehicleInfo("clique", 0xA29F78B0, Manufacturer.Vapid, VehicleClass.Muscle),
            new VehicleInfo("coach", 0x84718D34, Manufacturer.HVY, VehicleClass.Service, DispatchScannerFiles.model.COACH01.FileName),
            new VehicleInfo("cog55", 0x360A438E, Manufacturer.Enus, VehicleClass.Sedans, DispatchScannerFiles.model.COG5501.FileName),
            new VehicleInfo("cog552", 0x29FCD3E4, Manufacturer.Enus, VehicleClass.Sedans, DispatchScannerFiles.model.COG5501.FileName),
            new VehicleInfo("cogcabrio", 0x13B57D8A, Manufacturer.Enus, VehicleClass.Coupes, DispatchScannerFiles.model.COGCABRIO01.FileName),
            new VehicleInfo("cognoscenti", 0x86FE0B60, Manufacturer.Enus, VehicleClass.Sedans, DispatchScannerFiles.model.COGNOSCENTI01.FileName),
            new VehicleInfo("cognoscenti2", 0xDBF2D57A, Manufacturer.Enus, VehicleClass.Sedans, DispatchScannerFiles.model.COGNOSCENTI01.FileName),
            new VehicleInfo("comet2", 0xC1AE4D16, Manufacturer.Pfister, VehicleClass.Sports, DispatchScannerFiles.model.COMET01.FileName),
            new VehicleInfo("comet3", 0x877358AD, Manufacturer.Pfister, VehicleClass.Sports, DispatchScannerFiles.model.COMET01.FileName),
            new VehicleInfo("comet4", 0x5D1903F9, Manufacturer.Pfister, VehicleClass.Sports, DispatchScannerFiles.model.COMET01.FileName),
            new VehicleInfo("contender", 0x28B67ACA, Manufacturer.Vapid, VehicleClass.SUVs),
            new VehicleInfo("coquette", 0x67BC037, Manufacturer.Invetero, VehicleClass.Sports, DispatchScannerFiles.model.COQUETTE01.FileName),
            new VehicleInfo("coquette2", 0x3C4E2113, Manufacturer.Invetero, VehicleClass.SportsClassics, DispatchScannerFiles.model.COQUETTE01.FileName),
            new VehicleInfo("coquette3", 0x2EC385FE, Manufacturer.Invetero, VehicleClass.Muscle, DispatchScannerFiles.model.COQUETTE01.FileName),
            new VehicleInfo("cruiser", 0x1ABA13B5, Manufacturer.Unknown, VehicleClass.Cycles),
            new VehicleInfo("crusader", 0x132D5A1A, Manufacturer.Canis, VehicleClass.Military),
            new VehicleInfo("cuban800", 0xD9927FE3, Manufacturer.WesternCompany, VehicleClass.Planes),
            new VehicleInfo("cutter", 0xC3FBA120, Manufacturer.HVY, VehicleClass.Industrial),
            new VehicleInfo("cyclone", 0x52FF9437, Manufacturer.Coil, VehicleClass.Super),
            new VehicleInfo("daemon", 0x77934CEE, Manufacturer.WesternMotorcycleCompany, VehicleClass.Motorcycles, DispatchScannerFiles.model.DAEMON01.FileName),
            new VehicleInfo("daemon2", 0xAC4E93C9, Manufacturer.WesternMotorcycleCompany, VehicleClass.Motorcycles, DispatchScannerFiles.model.DAEMON01.FileName),
            new VehicleInfo("deathbike", 0xFE5F0722, Manufacturer.WesternMotorcycleCompany, VehicleClass.Motorcycles),
            new VehicleInfo("deathbike2", 0x93F09558, Manufacturer.WesternMotorcycleCompany, VehicleClass.Motorcycles),
            new VehicleInfo("deathbike3", 0xAE12C99C, Manufacturer.WesternMotorcycleCompany, VehicleClass.Motorcycles),
            new VehicleInfo("defiler", 0x30FF0190, Manufacturer.Shitzu, VehicleClass.Motorcycles),
            new VehicleInfo("deluxo", 0x586765FB, Manufacturer.Imponte, VehicleClass.SportsClassics),
            new VehicleInfo("deveste", 0x5EE005DA, Manufacturer.Principe, VehicleClass.Sports),
            new VehicleInfo("deviant", 0x4C3FFF49, Manufacturer.Schyster, VehicleClass.Muscle),
            new VehicleInfo("diablous", 0xF1B44F44, Manufacturer.Principe, VehicleClass.Motorcycles),
            new VehicleInfo("diablous2", 0x6ABDF65E, Manufacturer.Principe, VehicleClass.Motorcycles),
            new VehicleInfo("dilettante", 0xBC993509, Manufacturer.Karin, VehicleClass.Compacts, DispatchScannerFiles.model.DILETTANTE01.FileName),
            new VehicleInfo("dilettante2", 0x64430650, Manufacturer.Karin, VehicleClass.Compacts, DispatchScannerFiles.model.DILETTANTE01.FileName),
            new VehicleInfo("dinghy", 0x3D961290, Manufacturer.Nagasaki, VehicleClass.Boats, DispatchScannerFiles.model.DINGHY01.FileName),
            new VehicleInfo("dinghy2", 0x107F392C, Manufacturer.Nagasaki, VehicleClass.Boats, DispatchScannerFiles.model.DINGHY01.FileName),
            new VehicleInfo("dinghy3", 0x1E5E54EA, Manufacturer.Nagasaki, VehicleClass.Boats, DispatchScannerFiles.model.DINGHY01.FileName),
            new VehicleInfo("dinghy4", 0x33B47F96, Manufacturer.Nagasaki, VehicleClass.Boats, DispatchScannerFiles.model.DINGHY01.FileName),
            new VehicleInfo("dloader", 0x698521E3, Manufacturer.Bravado, VehicleClass.OffRoad),
            new VehicleInfo("docktrailer", 0x806EFBEE, Manufacturer.Unknown, VehicleClass.Trailer),
            new VehicleInfo("docktug", 0xCB44B1CA, Manufacturer.Unknown, VehicleClass.Utility),
            new VehicleInfo("dodo", 0xCA495705, Manufacturer.Mammoth, VehicleClass.Planes),
            new VehicleInfo("dominator", 0x4CE68AC, Manufacturer.Vapid, VehicleClass.Muscle, DispatchScannerFiles.model.DOMINATOR01.FileName),
            new VehicleInfo("dominator2", 0xC96B73D9, Manufacturer.Vapid, VehicleClass.Muscle, DispatchScannerFiles.model.COQUETTE01.FileName),
            new VehicleInfo("dominator3", 0xC52C6B93, Manufacturer.Vapid, VehicleClass.Muscle, DispatchScannerFiles.model.DOMINATOR01.FileName),
            new VehicleInfo("dominator4", 0xD6FB0F30, Manufacturer.Vapid, VehicleClass.Muscle, DispatchScannerFiles.model.DOMINATOR01.FileName),
            new VehicleInfo("dominator5", 0xAE0A3D4F, Manufacturer.Vapid, VehicleClass.Muscle, DispatchScannerFiles.model.DOMINATOR01.FileName),
            new VehicleInfo("dominator6", 0xB2E046FB, Manufacturer.Vapid, VehicleClass.Muscle, DispatchScannerFiles.model.DOMINATOR01.FileName),
            new VehicleInfo("double", 0x9C669788, Manufacturer.Dinka, VehicleClass.Motorcycles, DispatchScannerFiles.model.DOUBLE01.FileName),

            new VehicleInfo("drafter", 0x28EAB80F, Manufacturer.Obey, VehicleClass.Sports),


            new VehicleInfo("dubsta", 0x462FE277, Manufacturer.Benefactor, VehicleClass.SUVs, DispatchScannerFiles.model.DUBSTA01.FileName),
            new VehicleInfo("dubsta2", 0xE882E5F6, Manufacturer.Benefactor, VehicleClass.SUVs, DispatchScannerFiles.model.DUBSTA01.FileName),
            new VehicleInfo("dubsta3", 0xB6410173, Manufacturer.Benefactor, VehicleClass.OffRoad, DispatchScannerFiles.model.DUBSTA01.FileName),
            new VehicleInfo("dukes", 0x2B26F456, Manufacturer.Imponte, VehicleClass.Muscle, DispatchScannerFiles.model.DUKES01.FileName),
            new VehicleInfo("dukes2", 0xEC8F7094, Manufacturer.Imponte, VehicleClass.Muscle, DispatchScannerFiles.model.DUKES01.FileName),
            new VehicleInfo("dump", 0x810369E2, Manufacturer.HVY, VehicleClass.Industrial),
            new VehicleInfo("dune", 0x9CF21E0F, Manufacturer.Burgerfahrzeug, VehicleClass.OffRoad),
            new VehicleInfo("dune2", 0x1FD824AF, Manufacturer.Burgerfahrzeug, VehicleClass.OffRoad),
            new VehicleInfo("dune3", 0x711D4738, Manufacturer.Burgerfahrzeug, VehicleClass.OffRoad),
            new VehicleInfo("dune4", 0xCEB28249, Manufacturer.Burgerfahrzeug, VehicleClass.OffRoad),
            new VehicleInfo("dune5", 0xED62BFA9, Manufacturer.Burgerfahrzeug, VehicleClass.OffRoad),
            new VehicleInfo("duster", 0x39D6779E, Manufacturer.WesternCompany, VehicleClass.Planes),

            new VehicleInfo("dynasty", 0, Manufacturer.Weeny, VehicleClass.Sports),

            new VehicleInfo("elegy", 0xBBA2261, Manufacturer.Annis, VehicleClass.Sports, DispatchScannerFiles.model.ELEGY01.FileName),
            new VehicleInfo("elegy2", 0xDE3D9D22, Manufacturer.Annis, VehicleClass.Sports, DispatchScannerFiles.model.ELEGY01.FileName),
            new VehicleInfo("emperor", 0xD7278283, Manufacturer.Albany, VehicleClass.Sedans, DispatchScannerFiles.model.EMPEROR01.FileName),
            new VehicleInfo("emperor2", 0x8FC3AADC, Manufacturer.Albany, VehicleClass.Sedans, DispatchScannerFiles.model.EMPEROR01.FileName),
            new VehicleInfo("emperor3", 0xB5FCF74E, Manufacturer.Albany, VehicleClass.Sedans, DispatchScannerFiles.model.EMPEROR01.FileName),
            new VehicleInfo("enduro", 0x6882FA73, Manufacturer.Dinka, VehicleClass.Motorcycles),
            new VehicleInfo("entityxf", 0xB2FE5CF9, Manufacturer.Overflod, VehicleClass.Super, DispatchScannerFiles.model.ENTITYXF01.FileName),
            new VehicleInfo("esskey", 0x794CB30C, Manufacturer.Pegassi, VehicleClass.Motorcycles),
            new VehicleInfo("exemplar", 0xFFB15B5E, Manufacturer.Dewbauchee, VehicleClass.Coupes, DispatchScannerFiles.model.EXEMPLAR01.FileName),
            new VehicleInfo("f620", 0xDCBCBE48, Manufacturer.Ocelot, VehicleClass.Coupes, DispatchScannerFiles.model.F62001.FileName),
            new VehicleInfo("faction", 0x81A9CDDF, Manufacturer.Willard, VehicleClass.Muscle, DispatchScannerFiles.model.FACTION01.FileName),
            new VehicleInfo("faction2", 0x95466BDB, Manufacturer.Willard, VehicleClass.Muscle, DispatchScannerFiles.model.FACTION01.FileName),
            new VehicleInfo("faction3", 0x866BCE26, Manufacturer.Willard, VehicleClass.Muscle, DispatchScannerFiles.model.FACTION01.FileName),
            new VehicleInfo("faggio", 0x9229E4EB, Manufacturer.Principe, VehicleClass.Motorcycles, DispatchScannerFiles.model.FAGGIO01.FileName),
            new VehicleInfo("faggio2", 0x350D1AB, Manufacturer.Principe, VehicleClass.Motorcycles, DispatchScannerFiles.model.FAGGIO01.FileName),
            new VehicleInfo("faggio3", 0xB328B188, Manufacturer.Principe, VehicleClass.Motorcycles, DispatchScannerFiles.model.FAGGIO01.FileName),
            new VehicleInfo("fbi", 0x432EA949, Manufacturer.Bravado, VehicleClass.Emergency, DispatchScannerFiles.model.POLICECAR01.FileName),
            new VehicleInfo("fbi2", 0x9DC66994, Manufacturer.Declasse, VehicleClass.Emergency, DispatchScannerFiles.model.POLICECAR01.FileName),
            new VehicleInfo("fcr", 0x25676EAF, Manufacturer.Pegassi, VehicleClass.Motorcycles, DispatchScannerFiles.model.DOMINATOR01.FileName),
            new VehicleInfo("fcr2", 0xD2D5E00E, Manufacturer.Pegassi, VehicleClass.Motorcycles),
            new VehicleInfo("felon", 0xE8A8BDA8, Manufacturer.Lampadati, VehicleClass.Coupes, DispatchScannerFiles.model.FELON01.FileName),
            new VehicleInfo("felon2", 0xFAAD85EE, Manufacturer.Lampadati, VehicleClass.Coupes, DispatchScannerFiles.model.FELON01.FileName),
            new VehicleInfo("feltzer2", 0x8911B9F5, Manufacturer.Benefactor, VehicleClass.Sports, DispatchScannerFiles.model.FELTZER01.FileName),
            new VehicleInfo("feltzer3", 0xA29D6D10, Manufacturer.Benefactor, VehicleClass.SportsClassics, DispatchScannerFiles.model.FELTZER01.FileName),
            new VehicleInfo("firetruk", 0x73920F8E, Manufacturer.MTL, VehicleClass.Emergency, DispatchScannerFiles.model.FIRETRUCK01.FileName),
            new VehicleInfo("fixter", 0xCE23D3BF, Manufacturer.Unknown, VehicleClass.Cycles),
            new VehicleInfo("flatbed", 0x50B0215A, Manufacturer.MTL, VehicleClass.Industrial),
            new VehicleInfo("fmj", 0x5502626C, Manufacturer.Vapid, VehicleClass.Super),
            new VehicleInfo("forklift", 0x58E49664, Manufacturer.HVY, VehicleClass.Utility),
            new VehicleInfo("fq2", 0xBC32A33B, Manufacturer.Fathom, VehicleClass.SUVs, DispatchScannerFiles.model.FQ201.FileName),
            new VehicleInfo("freecrawler", 0xFCC2F483, Manufacturer.Canis, VehicleClass.OffRoad),
            new VehicleInfo("freight", 0x3D6AAA9B, Manufacturer.Unknown, VehicleClass.Trains),
            new VehicleInfo("freightcar", 0x0AFD22A6, Manufacturer.Unknown, VehicleClass.Trains),
            new VehicleInfo("freightcont1", 0x36DCFF98, Manufacturer.Unknown, VehicleClass.Trains),
            new VehicleInfo("freightcont2", 0x0E512E79, Manufacturer.Unknown, VehicleClass.Trains),
            new VehicleInfo("freightgrain", 0x264D9262, Manufacturer.Unknown, VehicleClass.Trains),
            new VehicleInfo("frogger", 0x2C634FBD, Manufacturer.MaibatsuCorporation, VehicleClass.Helicopters),
            new VehicleInfo("frogger2", 0x742E9AC0, Manufacturer.MaibatsuCorporation, VehicleClass.Helicopters),
            new VehicleInfo("fugitive", 0x71CB2FFB, Manufacturer.Cheval, VehicleClass.Sedans, DispatchScannerFiles.model.FUGITIVE01.FileName),
            new VehicleInfo("furoregt", 0xBF1691E0, Manufacturer.Lampadati, VehicleClass.Sports),
            new VehicleInfo("fusilade", 0x1DC0BA53, Manufacturer.Schyster, VehicleClass.Sports, DispatchScannerFiles.model.FUSILADE01.FileName),
            new VehicleInfo("futo", 0x7836CE2F, Manufacturer.Karin, VehicleClass.Sports, DispatchScannerFiles.model.FUTO01.FileName),
            new VehicleInfo("gargoyle", 0x2C2C2324, Manufacturer.WesternMotorcycleCompany, VehicleClass.Motorcycles),
            new VehicleInfo("gauntlet", 0x94B395C5, Manufacturer.Bravado, VehicleClass.Muscle, DispatchScannerFiles.model.GAUNTLET01.FileName),
            new VehicleInfo("gauntlet2", 0x14D22159, Manufacturer.Bravado, VehicleClass.Muscle, DispatchScannerFiles.model.GAUNTLET01.FileName),

            new VehicleInfo("gauntlet3", 0x2B0C4DCD, Manufacturer.Bravado, VehicleClass.Muscle, DispatchScannerFiles.model.GAUNTLET01.FileName),
            new VehicleInfo("gauntlet4", 0x734C5E50, Manufacturer.Bravado, VehicleClass.Muscle, DispatchScannerFiles.model.GAUNTLET01.FileName),

            new VehicleInfo("gburrito", 0x97FA4F36, Manufacturer.Declasse, VehicleClass.Vans, DispatchScannerFiles.model.BURRITO01.FileName),
            new VehicleInfo("gburrito2", 0x11AA0E14, Manufacturer.Declasse, VehicleClass.Vans, DispatchScannerFiles.model.BURRITO01.FileName),
            new VehicleInfo("glendale", 0x47A6BC1, Manufacturer.Benefactor, VehicleClass.Sedans),
            new VehicleInfo("gp1", 0x4992196C, Manufacturer.Progen, VehicleClass.Super),
            new VehicleInfo("graintrailer", 0x3CC7F596, Manufacturer.Unknown, VehicleClass.Trailer),
            new VehicleInfo("granger", 0x9628879C, Manufacturer.Declasse, VehicleClass.SUVs, DispatchScannerFiles.model.GRANGER01.FileName),
            new VehicleInfo("gresley", 0xA3FC0F4D, Manufacturer.Bravado, VehicleClass.SUVs, DispatchScannerFiles.model.GRESLEY01.FileName),
            new VehicleInfo("gt500", 0x8408F33A, Manufacturer.Grotti, VehicleClass.SportsClassics),
            new VehicleInfo("guardian", 0x825A9F4C, Manufacturer.Vapid, VehicleClass.Industrial),
            new VehicleInfo("habanero", 0x34B7390F, Manufacturer.Emperor, VehicleClass.SUVs, DispatchScannerFiles.model.HABANERO01.FileName),
            new VehicleInfo("hakuchou", 0x4B6C568A, Manufacturer.Shitzu, VehicleClass.Motorcycles),
            new VehicleInfo("hakuchou2", 0xF0C2A91F, Manufacturer.Shitzu, VehicleClass.Motorcycles),
            new VehicleInfo("halftrack", 0xFE141DA6, Manufacturer.Bravado, VehicleClass.Military),
            new VehicleInfo("handler", 0x1A7FCEFA, Manufacturer.Unknown, VehicleClass.Industrial),
            new VehicleInfo("hauler", 0x5A82F9AE, Manufacturer.Jobuilt, VehicleClass.Commercial),
            new VehicleInfo("hauler2", 0x171C92C4, Manufacturer.Jobuilt, VehicleClass.Commercial),
            new VehicleInfo("havok", 0x89BA59F5, Manufacturer.Nagasaki, VehicleClass.Helicopters),

            new VehicleInfo("hellion", 0xEA6A047F, Manufacturer.Unknown, VehicleClass.OffRoad),

            new VehicleInfo("hermes", 0xE83C17, Manufacturer.Albany, VehicleClass.Muscle),
            new VehicleInfo("hexer", 0x11F76C14, Manufacturer.LibertyCityCycles, VehicleClass.Motorcycles),
            new VehicleInfo("hotknife", 0x239E390, Manufacturer.Unknown, VehicleClass.Muscle, DispatchScannerFiles.model.HOTKNIFE01.FileName),
            new VehicleInfo("hotring", 0x42836BE5, Manufacturer.Unknown, VehicleClass.Sports),
            new VehicleInfo("howard", 0xC3F25753, Manufacturer.Unknown, VehicleClass.Planes),
            new VehicleInfo("hunter", 0xFD707EDE, Manufacturer.Unknown, VehicleClass.Helicopters),
            new VehicleInfo("huntley", 0x1D06D681, Manufacturer.Enus, VehicleClass.SUVs),
            new VehicleInfo("hydra", 0x39D6E83F, Manufacturer.Unknown, VehicleClass.Planes),
            new VehicleInfo("impaler", 0xB2E046FB, Manufacturer.Unknown, VehicleClass.Muscle),
            new VehicleInfo("impaler2", 0x3C26BD0C, Manufacturer.Unknown, VehicleClass.Muscle),
            new VehicleInfo("impaler3", 0x8D45DF49, Manufacturer.Unknown, VehicleClass.Muscle),
            new VehicleInfo("impaler4", 0x9804F4C7, Manufacturer.Unknown, VehicleClass.Muscle),
            new VehicleInfo("imperator", 0x1A861243, Manufacturer.Unknown, VehicleClass.Muscle),
            new VehicleInfo("imperator2", 0x619C1B82, Manufacturer.Unknown, VehicleClass.Muscle),
            new VehicleInfo("imperator3", 0xD2F77E37, Manufacturer.Unknown, VehicleClass.Muscle),
            new VehicleInfo("infernus", 0x18F25AC7, Manufacturer.Pegassi, VehicleClass.Super, DispatchScannerFiles.model.INFERNUS01.FileName),
            new VehicleInfo("infernus2", 0xAC33179C, Manufacturer.Pegassi, VehicleClass.Sports, DispatchScannerFiles.model.INFERNUS01.FileName),
            new VehicleInfo("ingot", 0xB3206692, Manufacturer.Vulcar, VehicleClass.Sedans, DispatchScannerFiles.model.INGOT01.FileName),
            new VehicleInfo("innovation", 0xF683EACA, Manufacturer.LibertyCityCycles, VehicleClass.Motorcycles),
            new VehicleInfo("insurgent", 0x9114EADA, Manufacturer.HVY, VehicleClass.OffRoad),
            new VehicleInfo("insurgent2", 0x7B7E56F0, Manufacturer.HVY, VehicleClass.OffRoad),
            new VehicleInfo("insurgent3", 0x8D4B7A8A, Manufacturer.HVY, VehicleClass.OffRoad),
            new VehicleInfo("intruder", 0x34DD8AA1, Manufacturer.Unknown, VehicleClass.Sedans, DispatchScannerFiles.model.INTRUDER01.FileName),
            new VehicleInfo("issi2", 0xB9CB3B69, Manufacturer.Weeny, VehicleClass.Compacts, DispatchScannerFiles.model.ISSI01.FileName),
            new VehicleInfo("issi3", 0x378236E1, Manufacturer.Weeny, VehicleClass.Compacts, DispatchScannerFiles.model.ISSI01.FileName),
            new VehicleInfo("issi4", 0x256E92BA, Manufacturer.Weeny, VehicleClass.Compacts, DispatchScannerFiles.model.ISSI01.FileName),
            new VehicleInfo("issi5", 0x5BA0FF1E, Manufacturer.Weeny, VehicleClass.Compacts, DispatchScannerFiles.model.ISSI01.FileName),
            new VehicleInfo("issi6", 0x49E25BA1, Manufacturer.Weeny, VehicleClass.Compacts, DispatchScannerFiles.model.ISSI01.FileName),

            new VehicleInfo("issi7", 0x6E8DA4F7, Manufacturer.Weeny, VehicleClass.Sports, DispatchScannerFiles.model.ISSI01.FileName),

            new VehicleInfo("italigtb", 0x85E8E76B, Manufacturer.Progen, VehicleClass.Super),
            new VehicleInfo("italigtb2", 0xE33A477B, Manufacturer.Progen, VehicleClass.Super),
            new VehicleInfo("italigto", 0xEC3E3404, Manufacturer.Progen, VehicleClass.Sports),
            new VehicleInfo("jackal", 0xDAC67112, Manufacturer.Unknown, VehicleClass.Coupes, DispatchScannerFiles.model.JACKAL01.FileName),
            new VehicleInfo("jb700", 0x3EAB5555, Manufacturer.Dewbauchee, VehicleClass.SportsClassics, DispatchScannerFiles.model.JB70001.FileName),
            new VehicleInfo("jester", 0xB2A716A3, Manufacturer.Dinka, VehicleClass.Sports),
            new VehicleInfo("jester2", 0xBE0E6126, Manufacturer.Dinka, VehicleClass.Sports),

            new VehicleInfo("jester3", 0xF330CB6A, Manufacturer.Dinka, VehicleClass.Sports),

            new VehicleInfo("jet", 0x3F119114, Manufacturer.Unknown, VehicleClass.Planes),
            new VehicleInfo("jetmax", 0x33581161, Manufacturer.Shitzu, VehicleClass.Boats),
            new VehicleInfo("journey", 0xF8D48E7A, Manufacturer.Zirconium, VehicleClass.Vans, DispatchScannerFiles.model.JOURNEY01.FileName),

            new VehicleInfo("jugular", 0xF38C4245, Manufacturer.Ocelot, VehicleClass.Sports),

            new VehicleInfo("kalahari", 0x5852838, Manufacturer.Canis, VehicleClass.OffRoad),
            new VehicleInfo("khamelion", 0x206D1B68, Manufacturer.Hijak, VehicleClass.Sports, DispatchScannerFiles.model.KHAMELION01.FileName),
            new VehicleInfo("khanjali", 0xAA6F980A, Manufacturer.Unknown, VehicleClass.Military),
            new VehicleInfo("kuruma", 0xAE2BFE94, Manufacturer.Karin, VehicleClass.Sports),
            new VehicleInfo("kuruma2", 0x187D938D, Manufacturer.Karin, VehicleClass.Sports),
            new VehicleInfo("landstalker", 0x4BA4E8DC, Manufacturer.Dundreary, VehicleClass.SUVs, DispatchScannerFiles.model.LANDSTALKER01.FileName),
            new VehicleInfo("lazer", 0xB39B0AE6, Manufacturer.Jobuilt, VehicleClass.Planes),
            new VehicleInfo("le7b", 0xB6846A55, Manufacturer.Unknown, VehicleClass.Super),
            new VehicleInfo("lectro", 0x26321E67, Manufacturer.Principe, VehicleClass.Motorcycles),
            new VehicleInfo("lguard", 0x1BF8D381, Manufacturer.Unknown, VehicleClass.Emergency),
            new VehicleInfo("limo2", 0xF92AEC4D, Manufacturer.Unknown, VehicleClass.Sedans),

            new VehicleInfo("locust", 0xC7E55211, Manufacturer.Ocelot, VehicleClass.Sports),

            new VehicleInfo("lurcher", 0x7B47A6A7, Manufacturer.Albany, VehicleClass.Muscle),
            new VehicleInfo("luxor", 0x250B0C5E, Manufacturer.Unknown, VehicleClass.Planes),
            new VehicleInfo("luxor2", 0xB79F589E, Manufacturer.Unknown, VehicleClass.Planes),
            new VehicleInfo("lynx", 0x5DCA7C9A, Manufacturer.Unknown, VehicleClass.Sports),
            new VehicleInfo("mamba", 0x9CFFFC56, Manufacturer.Unknown, VehicleClass.SportsClassics),
            new VehicleInfo("mammatus", 0x97E55D11, Manufacturer.Unknown, VehicleClass.Planes),
            new VehicleInfo("manana", 0x81634188, Manufacturer.Albany, VehicleClass.SportsClassics, DispatchScannerFiles.model.MANANA01.FileName),
            new VehicleInfo("manchez", 0xA5325278, Manufacturer.Unknown, VehicleClass.Motorcycles),
            new VehicleInfo("marquis", 0xC1CE1183, Manufacturer.Unknown, VehicleClass.Boats),
            new VehicleInfo("marshall", 0x49863E9C, Manufacturer.Cheval, VehicleClass.OffRoad),
            new VehicleInfo("massacro", 0xF77ADE32, Manufacturer.Dewbauchee, VehicleClass.Sports),
            new VehicleInfo("massacro2", 0xDA5819A3, Manufacturer.Dewbauchee, VehicleClass.Sports),
            new VehicleInfo("maverick", 0x9D0450CA, Manufacturer.Buckingham, VehicleClass.Helicopters),
            new VehicleInfo("menacer", 0x79DD18AE, Manufacturer.HVY, VehicleClass.OffRoad),
            new VehicleInfo("mesa", 0x36848602, Manufacturer.Canis, VehicleClass.SUVs, DispatchScannerFiles.model.MESA01.FileName),
            new VehicleInfo("mesa2", 0xD36A4B44, Manufacturer.Canis, VehicleClass.SUVs, DispatchScannerFiles.model.MESA01.FileName),
            new VehicleInfo("mesa3", 0x84F42E51, Manufacturer.Canis, VehicleClass.OffRoad, DispatchScannerFiles.model.MESA01.FileName),
            new VehicleInfo("microlight", 0x96E24857, Manufacturer.Nagasaki, VehicleClass.Planes),
            new VehicleInfo("miljet", 0x9D80F93, Manufacturer.Unknown, VehicleClass.Planes),
            new VehicleInfo("minivan", 0xED7EADA4, Manufacturer.Unknown, VehicleClass.Vans, DispatchScannerFiles.model.MINIVAN01.FileName),
            new VehicleInfo("minivan2", 0xBCDE91F0, Manufacturer.Unknown, VehicleClass.Vans, DispatchScannerFiles.model.MINIVAN01.FileName),
            new VehicleInfo("mixer", 0xD138A6BB, Manufacturer.Unknown, VehicleClass.Industrial, DispatchScannerFiles.model.MIXER01.FileName),
            new VehicleInfo("mixer2", 0x1C534995, Manufacturer.Unknown, VehicleClass.Industrial, DispatchScannerFiles.model.MIXER01.FileName),
            new VehicleInfo("mogul", 0xD35698EF, Manufacturer.Mammoth, VehicleClass.Planes),
            new VehicleInfo("molotok", 0x5D56F01B, Manufacturer.Unknown, VehicleClass.Planes),
            new VehicleInfo("monroe", 0xE62B361B, Manufacturer.Unknown, VehicleClass.SportsClassics, DispatchScannerFiles.model.MONROE01.FileName),
            new VehicleInfo("monster", 0xCD93A7DB, Manufacturer.Unknown, VehicleClass.OffRoad),
            new VehicleInfo("monster3", 0x669EB40A, Manufacturer.Unknown, VehicleClass.OffRoad),
            new VehicleInfo("monster4", 0x32174AFC, Manufacturer.Unknown, VehicleClass.OffRoad),
            new VehicleInfo("monster5", 0xD556917C, Manufacturer.Unknown, VehicleClass.OffRoad),
            new VehicleInfo("moonbeam", 0x1F52A43F, Manufacturer.Unknown, VehicleClass.Muscle),
            new VehicleInfo("moonbeam2", 0x710A2B9B, Manufacturer.Unknown, VehicleClass.Muscle),
            new VehicleInfo("mower", 0x6A4BD8F6, Manufacturer.JackSheepe, VehicleClass.Utility),
            new VehicleInfo("mule", 0x35ED670B, Manufacturer.MaibatsuCorporation, VehicleClass.Commercial, DispatchScannerFiles.model.MULE01.FileName),
            new VehicleInfo("mule2", 0xC1632BEB, Manufacturer.MaibatsuCorporation, VehicleClass.Commercial, DispatchScannerFiles.model.MULE01.FileName),
            new VehicleInfo("mule3", 0x85A5B471, Manufacturer.MaibatsuCorporation, VehicleClass.Commercial, DispatchScannerFiles.model.MULE01.FileName),
            new VehicleInfo("mule4", 0x73F4110E, Manufacturer.MaibatsuCorporation, VehicleClass.Commercial, DispatchScannerFiles.model.MULE01.FileName),
            new VehicleInfo("nemesis", 0xDA288376, Manufacturer.Principe, VehicleClass.Motorcycles, DispatchScannerFiles.model.NEMESIS01.FileName),

            new VehicleInfo("neo", 0x9F6ED5A2, Manufacturer.Unknown, VehicleClass.Sports),

            new VehicleInfo("neon", 0x91CA96EE, Manufacturer.Pfister, VehicleClass.Sports),
            new VehicleInfo("nero", 0x3DA47243, Manufacturer.Truffade, VehicleClass.Super),
            new VehicleInfo("nero2", 0x4131F378, Manufacturer.Truffade, VehicleClass.Super),
            new VehicleInfo("nightblade", 0xA0438767, Manufacturer.LibertyChopShop, VehicleClass.Motorcycles),
            new VehicleInfo("nightshade", 0x8C2BD0DC, Manufacturer.Unknown, VehicleClass.Muscle),
            new VehicleInfo("nightshark", 0x19DD9ED1, Manufacturer.Unknown, VehicleClass.OffRoad),
            new VehicleInfo("nimbus", 0xB2CF7250, Manufacturer.Unknown, VehicleClass.Planes),
            new VehicleInfo("ninef", 0x3D8FA25C, Manufacturer.Obey, VehicleClass.Sports, DispatchScannerFiles.model.NINEF01.FileName),
            new VehicleInfo("ninef2", 0xA8E38B01, Manufacturer.Obey, VehicleClass.Sports, DispatchScannerFiles.model.NINEF01.FileName),
            new VehicleInfo("nokota", 0x3DC92356, Manufacturer.Unknown, VehicleClass.Planes),

            new VehicleInfo("novak", 0x92F5024E, Manufacturer.Lampadati, VehicleClass.SUVs),

            new VehicleInfo("omnis", 0xD1AD4937, Manufacturer.Obey, VehicleClass.Sports),
            new VehicleInfo("oppressor", 0x34B82784, Manufacturer.Pegassi, VehicleClass.Motorcycles),
            new VehicleInfo("oppressor2", 0x7B54A9D3, Manufacturer.Pegassi, VehicleClass.Motorcycles),
            new VehicleInfo("oracle", 0x506434F6, Manufacturer.Ubermacht, VehicleClass.Coupes, DispatchScannerFiles.model.ORACLE01.FileName),
            new VehicleInfo("oracle2", 0xE18195B2, Manufacturer.Ubermacht, VehicleClass.Coupes, DispatchScannerFiles.model.ORACLE01.FileName),
            new VehicleInfo("osiris", 0x767164D6, Manufacturer.Unknown, VehicleClass.Super),

            new VehicleInfo("paragon", 0xE550775B, Manufacturer.Enus, VehicleClass.Sports),
            new VehicleInfo("paragon2", 0x546D8EEE, Manufacturer.Enus, VehicleClass.Sports),

            new VehicleInfo("packer", 0x21EEE87D, Manufacturer.MTL, VehicleClass.Commercial, DispatchScannerFiles.model.PACKER01.FileName),
            new VehicleInfo("panto", 0xE644E480, Manufacturer.Benefactor, VehicleClass.Compacts),
            new VehicleInfo("paradise", 0x58B3979C, Manufacturer.Bravado, VehicleClass.Vans),
            new VehicleInfo("pariah", 0x33B98FE2, Manufacturer.Ocelot, VehicleClass.Sports),
            new VehicleInfo("patriot", 0xCFCFEB3B, Manufacturer.Mammoth, VehicleClass.SUVs, DispatchScannerFiles.model.PATRIOT01.FileName),
            new VehicleInfo("patriot2", 0xE6E967F8, Manufacturer.Mammoth, VehicleClass.SUVs, DispatchScannerFiles.model.PATRIOT01.FileName),
            new VehicleInfo("pbus", 0x885F3671, Manufacturer.Unknown, VehicleClass.Emergency),
            new VehicleInfo("pbus2", 0x149BD32A, Manufacturer.Unknown, VehicleClass.Emergency),
            new VehicleInfo("pcj", 0xC9CEAF06, Manufacturer.Shitzu, VehicleClass.Motorcycles, DispatchScannerFiles.model.PCJ60001.FileName),
            new VehicleInfo("penetrator", 0x9734F3EA, Manufacturer.Unknown, VehicleClass.Super),
            new VehicleInfo("penumbra", 0xE9805550, Manufacturer.MaibatsuCorporation, VehicleClass.Sports, DispatchScannerFiles.model.PENUMBRA01.FileName),
            new VehicleInfo("peyote", 0x6D19CCBC, Manufacturer.Unknown, VehicleClass.SportsClassics, DispatchScannerFiles.model.PEYOTE01.FileName),

            new VehicleInfo("peyote2", 0x9472CD24, Manufacturer.Unknown, VehicleClass.Muscle, DispatchScannerFiles.model.PEYOTE01.FileName),

            new VehicleInfo("pfister811", 0x92EF6E04, Manufacturer.Pfister, VehicleClass.Super),
            new VehicleInfo("phantom", 0x809AA4CB, Manufacturer.Jobuilt, VehicleClass.Commercial, DispatchScannerFiles.model.PHANTOM01.FileName),
            new VehicleInfo("phantom2", 0x9DAE1398, Manufacturer.Jobuilt, VehicleClass.Commercial, DispatchScannerFiles.model.PHANTOM01.FileName),
            new VehicleInfo("phantom3", 0xA90ED5C, Manufacturer.Jobuilt, VehicleClass.Commercial, DispatchScannerFiles.model.PHANTOM01.FileName),
            new VehicleInfo("phoenix", 0x831A21D5, Manufacturer.Unknown, VehicleClass.Muscle, DispatchScannerFiles.model.PHOENIX01.FileName),
            new VehicleInfo("picador", 0x59E0FBF3, Manufacturer.Cheval, VehicleClass.Muscle, DispatchScannerFiles.model.PICADOR01.FileName),
            new VehicleInfo("pigalle", 0x404B6381, Manufacturer.Lampadati, VehicleClass.SportsClassics),
            new VehicleInfo("police", 0x79FBB0C5, Manufacturer.Unknown, VehicleClass.Emergency, DispatchScannerFiles.model.POLICECAR01.FileName),
            new VehicleInfo("police2", 0x9F05F101, Manufacturer.Bravado, VehicleClass.Emergency, DispatchScannerFiles.model.POLICECAR01.FileName),
            new VehicleInfo("police3", 0x71FA16EA, Manufacturer.Unknown, VehicleClass.Emergency, DispatchScannerFiles.model.POLICECAR01.FileName),
            new VehicleInfo("police4", 0x8A63C7B9, Manufacturer.Unknown, VehicleClass.Emergency, DispatchScannerFiles.model.POLICECAR01.FileName),
            new VehicleInfo("policeb", 0xFDEFAEC3, Manufacturer.Unknown, VehicleClass.Emergency, DispatchScannerFiles.model.POLICECAR01.FileName),
            new VehicleInfo("policeold1", 0xA46462F7, Manufacturer.Unknown, VehicleClass.Emergency, DispatchScannerFiles.model.POLICECAR01.FileName),
            new VehicleInfo("policeold2", 0x95F4C618, Manufacturer.Albany, VehicleClass.Emergency, DispatchScannerFiles.model.POLICECAR01.FileName),
            new VehicleInfo("policet", 0x1B38E955, Manufacturer.Unknown, VehicleClass.Emergency, DispatchScannerFiles.model.POLICETRANSPORT01.FileName),
            new VehicleInfo("polmav", 0x1517D4D9, Manufacturer.Buckingham, VehicleClass.Emergency, DispatchScannerFiles.model.POLICEMAVERICK01.FileName),
            new VehicleInfo("pony", 0xF8DE29A8, Manufacturer.Unknown, VehicleClass.Vans, DispatchScannerFiles.model.PONY01.FileName),
            new VehicleInfo("pony2", 0x38408341, Manufacturer.Unknown, VehicleClass.Vans, DispatchScannerFiles.model.PONY01.FileName),
            new VehicleInfo("pounder", 0x7DE35E7D, Manufacturer.MTL, VehicleClass.Commercial, DispatchScannerFiles.model.POUNDER01.FileName),
            new VehicleInfo("pounder2", 0x6290F15B, Manufacturer.MTL, VehicleClass.Commercial, DispatchScannerFiles.model.POUNDER01.FileName),
            new VehicleInfo("prairie", 0xA988D3A2, Manufacturer.Bollokan, VehicleClass.Compacts, DispatchScannerFiles.model.PRAIRIE01.FileName),
            new VehicleInfo("pranger", 0x2C33B46E, Manufacturer.Albany, VehicleClass.Emergency, DispatchScannerFiles.model.GRANGER01.FileName),
            new VehicleInfo("predator", 0xE2E7D4AB, Manufacturer.Unknown, VehicleClass.Emergency, DispatchScannerFiles.model.PREDATOR01.FileName),
            new VehicleInfo("premier", 0x8FB66F9B, Manufacturer.Unknown, VehicleClass.Sedans, DispatchScannerFiles.model.PREMIER01.FileName),
            new VehicleInfo("primo", 0xBB6B404F, Manufacturer.Albany, VehicleClass.Sedans, DispatchScannerFiles.model.PRIMO01.FileName),
            new VehicleInfo("primo2", 0x86618EDA, Manufacturer.Albany, VehicleClass.Sedans, DispatchScannerFiles.model.PRIMO01.FileName),
            new VehicleInfo("proptrailer", 0x153E1B0A, Manufacturer.Unknown, VehicleClass.Trailer),
            new VehicleInfo("prototipo", 0x7E8F677F, Manufacturer.Grotti, VehicleClass.Super),
            new VehicleInfo("pyro", 0xAD6065C0, Manufacturer.Buckingham, VehicleClass.Planes),
            new VehicleInfo("radi", 0x9D96B45B, Manufacturer.Unknown, VehicleClass.SUVs, DispatchScannerFiles.model.RADI01.FileName),
            new VehicleInfo("raiden", 0xA4D99B7D, Manufacturer.Coil, VehicleClass.Sports),
            new VehicleInfo("raketrailer", 0x174CB172, Manufacturer.Unknown, VehicleClass.Trailer),
            new VehicleInfo("rallytruck", 0x829A3C44, Manufacturer.Unknown, VehicleClass.Service),
            new VehicleInfo("rancherxl", 0x6210CBB0, Manufacturer.Unknown, VehicleClass.OffRoad, DispatchScannerFiles.model.RANCHERXL01.FileName),
            new VehicleInfo("rancherxl2", 0x7341576B, Manufacturer.Unknown, VehicleClass.OffRoad, DispatchScannerFiles.model.RANCHERXL01.FileName),
            new VehicleInfo("rapidgt", 0x8CB29A14, Manufacturer.Dewbauchee, VehicleClass.Sports, DispatchScannerFiles.model.RAPIDGT01.FileName),
            new VehicleInfo("rapidgt2", 0x679450AF, Manufacturer.Dewbauchee, VehicleClass.Sports, DispatchScannerFiles.model.RAPIDGT01.FileName),
            new VehicleInfo("rapidgt3", 0x7A2EF5E4, Manufacturer.Dewbauchee, VehicleClass.SportsClassics, DispatchScannerFiles.model.RAPIDGT01.FileName),
            new VehicleInfo("raptor", 0xD7C56D39, Manufacturer.Burgerfahrzeug, VehicleClass.Sports),
            new VehicleInfo("ratbike", 0x6FACDF31, Manufacturer.Unknown, VehicleClass.Motorcycles),
            new VehicleInfo("ratloader", 0xD83C13CE, Manufacturer.Bravado, VehicleClass.Muscle, DispatchScannerFiles.model.RATLOADER01.FileName),
            new VehicleInfo("ratloader2", 0xDCE1D9F7, Manufacturer.Bravado, VehicleClass.Muscle, DispatchScannerFiles.model.RATLOADER01.FileName),
            new VehicleInfo("rcbandito", 0xEEF345EC, Manufacturer.Unknown, VehicleClass.OffRoad),
            new VehicleInfo("reaper", 0xDF381E5, Manufacturer.Unknown, VehicleClass.Super),
            new VehicleInfo("rebel", 0xB802DD46, Manufacturer.Unknown, VehicleClass.OffRoad, DispatchScannerFiles.model.REBEL01.FileName),
            new VehicleInfo("rebel2", 0x8612B64B, Manufacturer.Unknown, VehicleClass.OffRoad, DispatchScannerFiles.model.REBEL01.FileName),
            new VehicleInfo("regina", 0xFF22D208, Manufacturer.Dundreary, VehicleClass.Sedans, DispatchScannerFiles.model.REGINA01.FileName),
            new VehicleInfo("rentalbus", 0xBE819C63, Manufacturer.Unknown, VehicleClass.Service),
            new VehicleInfo("retinue", 0x6DBD6C0A, Manufacturer.Unknown, VehicleClass.SportsClassics),
            new VehicleInfo("revolter", 0xE78CC3D9, Manufacturer.Ubermacht, VehicleClass.Sports),
            new VehicleInfo("rhapsody", 0x322CF98F, Manufacturer.Unknown, VehicleClass.Compacts),
            new VehicleInfo("rhino", 0x2EA68690, Manufacturer.Unknown, VehicleClass.Military, DispatchScannerFiles.model.RHINO01.FileName),
            new VehicleInfo("riata", 0xA4A4E453, Manufacturer.Unknown, VehicleClass.OffRoad),
            new VehicleInfo("riot", 0xB822A1AA, Manufacturer.Brute, VehicleClass.Emergency, DispatchScannerFiles.model.RIOT01.FileName),
            new VehicleInfo("riot2", 0x9B16A3B4, Manufacturer.Brute, VehicleClass.Emergency, DispatchScannerFiles.model.RIOT01.FileName),
            new VehicleInfo("ripley", 0xCD935EF9, Manufacturer.Unknown, VehicleClass.Utility),
            new VehicleInfo("rocoto", 0x7F5C91F1, Manufacturer.Obey, VehicleClass.SUVs, DispatchScannerFiles.model.ROCOTO01.FileName),
            new VehicleInfo("rogue", 0xC5DD6967, Manufacturer.WesternCompany, VehicleClass.Planes),
            new VehicleInfo("romero", 0x2560B2FC, Manufacturer.Chariot, VehicleClass.Sedans, DispatchScannerFiles.model.HEARSE01.FileName),

            new VehicleInfo("rrocket", 0x36A167E0, Manufacturer.Unknown, VehicleClass.Motorcycles),

            new VehicleInfo("rubble", 0x9A5B1DCC, Manufacturer.Unknown, VehicleClass.Industrial),
            new VehicleInfo("ruffian", 0xCABD11E8, Manufacturer.Unknown, VehicleClass.Motorcycles),
            new VehicleInfo("ruiner", 0xF26CEFF9, Manufacturer.Imponte, VehicleClass.Muscle, DispatchScannerFiles.model.RUINER01.FileName),
            new VehicleInfo("ruiner2", 0x381E10BD, Manufacturer.Imponte, VehicleClass.Muscle, DispatchScannerFiles.model.RUINER01.FileName),
            new VehicleInfo("ruiner3", 0x2E5AFD37, Manufacturer.Imponte, VehicleClass.Muscle, DispatchScannerFiles.model.RUINER01.FileName),
            new VehicleInfo("rumpo", 0x4543B74D, Manufacturer.Bravado, VehicleClass.Vans),
            new VehicleInfo("rumpo2", 0x961AFEF7, Manufacturer.Bravado, VehicleClass.Vans),
            new VehicleInfo("rumpo3", 0x57F682AF, Manufacturer.Bravado, VehicleClass.Vans),
            new VehicleInfo("ruston", 0x2AE524A8, Manufacturer.Hijak, VehicleClass.Sports),
            new VehicleInfo("sabregt", 0x9B909C94, Manufacturer.Declasse, VehicleClass.Muscle),
            new VehicleInfo("sabregt2", 0xD4EA603, Manufacturer.Declasse, VehicleClass.Muscle),
            new VehicleInfo("sadler", 0xDC434E51, Manufacturer.Unknown, VehicleClass.Utility),
            new VehicleInfo("sadler2", 0x2BC345D1, Manufacturer.Unknown, VehicleClass.Utility),
            new VehicleInfo("sanchez2", 0xA960B13E, Manufacturer.MaibatsuCorporation, VehicleClass.Motorcycles),
            new VehicleInfo("sanctus", 0x58E316C7, Manufacturer.LibertyCityCycles, VehicleClass.Motorcycles),
            new VehicleInfo("sandking", 0xB9210FD0, Manufacturer.Vapid, VehicleClass.OffRoad),
            new VehicleInfo("sandking2", 0x3AF8C345, Manufacturer.Vapid, VehicleClass.OffRoad),
            new VehicleInfo("savage", 0xFB133A17, Manufacturer.Unknown, VehicleClass.Helicopters),
            new VehicleInfo("savestra", 0x35DED0DD, Manufacturer.Unknown, VehicleClass.SportsClassics),
            new VehicleInfo("sc1", 0x5097F589, Manufacturer.Ubermacht, VehicleClass.Super),
            new VehicleInfo("scarab", 0xBBA2A2F7, Manufacturer.Unknown, VehicleClass.Military),
            new VehicleInfo("scarab2", 0x5BEB3CE0, Manufacturer.Unknown, VehicleClass.Military),
            new VehicleInfo("scarab3", 0xDD71BFEB, Manufacturer.Unknown, VehicleClass.Military),
            new VehicleInfo("schafter2", 0xB52B5113, Manufacturer.Benefactor, VehicleClass.Sedans),
            new VehicleInfo("schafter3", 0xA774B5A6, Manufacturer.Benefactor, VehicleClass.Sports),
            new VehicleInfo("schafter4", 0x58CF185C, Manufacturer.Benefactor, VehicleClass.Sports),
            new VehicleInfo("schafter5", 0xCB0E7CD9, Manufacturer.Benefactor, VehicleClass.Sports),
            new VehicleInfo("schafter6", 0x72934BE4, Manufacturer.Benefactor, VehicleClass.Sports),
            new VehicleInfo("schlagen", 0xE1C03AB0, Manufacturer.Benefactor, VehicleClass.Sports),
            new VehicleInfo("schwarzer", 0xD37B7976, Manufacturer.Benefactor, VehicleClass.Sports),
            new VehicleInfo("scorcher", 0xF4E1AA15, Manufacturer.Unknown, VehicleClass.Cycles),
            new VehicleInfo("scramjet", 0xD9F0503D, Manufacturer.Declasse, VehicleClass.Super),
            new VehicleInfo("scrap", 0x9A9FD3DF, Manufacturer.Unknown, VehicleClass.Utility),
            new VehicleInfo("seabreeze", 0xE8983F9F, Manufacturer.WesternCompany, VehicleClass.Planes),
            new VehicleInfo("seashark", 0xC2974024, Manufacturer.Speedophile, VehicleClass.Boats),
            new VehicleInfo("seashark2", 0xDB4388E4, Manufacturer.Speedophile, VehicleClass.Boats),
            new VehicleInfo("seashark3", 0xED762D49, Manufacturer.Speedophile, VehicleClass.Boats),
            new VehicleInfo("seasparrow", 0xD4AE63D9, Manufacturer.Unknown, VehicleClass.Helicopters),
            new VehicleInfo("seminole", 0x48CECED3, Manufacturer.Canis, VehicleClass.SUVs),
            new VehicleInfo("sentinel", 0x50732C82, Manufacturer.Ubermacht, VehicleClass.Coupes),
            new VehicleInfo("sentinel2", 0x3412AE2D, Manufacturer.Ubermacht, VehicleClass.Coupes),
            new VehicleInfo("sentinel3", 0x41D149AA, Manufacturer.Ubermacht, VehicleClass.Sports),
            new VehicleInfo("serrano", 0x4FB1A214, Manufacturer.Benefactor, VehicleClass.SUVs),
            new VehicleInfo("seven70", 0x97398A4B, Manufacturer.Unknown, VehicleClass.Sports),
            new VehicleInfo("shamal", 0xB79C1BF5, Manufacturer.Buckingham, VehicleClass.Planes),
            new VehicleInfo("sheava", 0x30D3F6D8, Manufacturer.Unknown, VehicleClass.Super),
            new VehicleInfo("sheriff", 0x9BAA707C, Manufacturer.Unknown, VehicleClass.Emergency),
            new VehicleInfo("sheriff2", 0x72935408, Manufacturer.Unknown, VehicleClass.Emergency),
            new VehicleInfo("shotaro", 0xE7D2A16E, Manufacturer.Nagasaki, VehicleClass.Motorcycles),
            new VehicleInfo("skylift", 0x3E48BF23, Manufacturer.Unknown, VehicleClass.Helicopters),
            new VehicleInfo("slamvan", 0x2B7F9DE3, Manufacturer.Vapid, VehicleClass.Muscle),
            new VehicleInfo("slamvan2", 0x31ADBBFC, Manufacturer.Vapid, VehicleClass.Muscle),
            new VehicleInfo("slamvan3", 0x42BC5E19, Manufacturer.Vapid, VehicleClass.Muscle),
            new VehicleInfo("slamvan4", 0x8526E2F5, Manufacturer.Vapid, VehicleClass.Muscle),
            new VehicleInfo("slamvan5", 0x163F8520, Manufacturer.Vapid, VehicleClass.Muscle),
            new VehicleInfo("slamvan6", 0x67D52852, Manufacturer.Vapid, VehicleClass.Muscle),
            new VehicleInfo("sovereign", 0x2C509634, Manufacturer.Unknown, VehicleClass.Motorcycles),
            new VehicleInfo("specter", 0x706E2B40, Manufacturer.Dewbauchee, VehicleClass.Sports),
            new VehicleInfo("specter2", 0x400F5147, Manufacturer.Dewbauchee, VehicleClass.Sports),
            new VehicleInfo("speeder", 0xDC60D2B, Manufacturer.Unknown, VehicleClass.Boats),
            new VehicleInfo("speeder2", 0x1A144F2A, Manufacturer.Unknown, VehicleClass.Boats),
            new VehicleInfo("speedo", 0xCFB3870C, Manufacturer.Unknown, VehicleClass.Vans),
            new VehicleInfo("speedo2", 0x2B6DC64A, Manufacturer.Unknown, VehicleClass.Vans),
            new VehicleInfo("speedo4", 0xD17099D, Manufacturer.Unknown, VehicleClass.Vans),
            new VehicleInfo("squalo", 0x17DF5EC2, Manufacturer.Shitzu, VehicleClass.Boats),
            new VehicleInfo("stafford", 0x1324E960, Manufacturer.Enus, VehicleClass.Sedans),
            new VehicleInfo("stalion", 0x72A4C31E, Manufacturer.Declasse, VehicleClass.Muscle),
            new VehicleInfo("stalion2", 0xE80F67EE, Manufacturer.Declasse, VehicleClass.Muscle),
            new VehicleInfo("stanier", 0xA7EDE74D, Manufacturer.Unknown, VehicleClass.Sedans),
            new VehicleInfo("starling", 0x9A9EB7DE, Manufacturer.Unknown, VehicleClass.Planes),
            new VehicleInfo("stinger", 0x5C23AF9B, Manufacturer.Unknown, VehicleClass.SportsClassics),
            new VehicleInfo("stingergt", 0x82E499FA, Manufacturer.Unknown, VehicleClass.SportsClassics),
            new VehicleInfo("stockade", 0x6827CF72, Manufacturer.Unknown, VehicleClass.Commercial),
            new VehicleInfo("stockade3", 0xF337AB36, Manufacturer.Unknown, VehicleClass.Commercial),
            new VehicleInfo("stratum", 0x66B4FC45, Manufacturer.Zirconium, VehicleClass.Sedans),
            new VehicleInfo("streiter", 0x67D2B389, Manufacturer.Benefactor, VehicleClass.Sports),
            new VehicleInfo("stretch", 0x8B13F083, Manufacturer.Albany, VehicleClass.Sedans),
            new VehicleInfo("strikeforce", 0x64DE07A1, Manufacturer.Unknown, VehicleClass.Planes),
            new VehicleInfo("stromberg", 0x34DBA661, Manufacturer.Ocelot, VehicleClass.SportsClassics),
            new VehicleInfo("stunt", 0x81794C70, Manufacturer.Unknown, VehicleClass.Planes),
            new VehicleInfo("submersible", 0x2DFF622F, Manufacturer.KrakenSubmersibles, VehicleClass.Boats),
            new VehicleInfo("submersible2", 0xC07107EE, Manufacturer.KrakenSubmersibles, VehicleClass.Boats),
            new VehicleInfo("sultan", 0x39DA2754, Manufacturer.Karin, VehicleClass.Sports),
            new VehicleInfo("sultanrs", 0xEE6024BC, Manufacturer.Karin, VehicleClass.Super),
            new VehicleInfo("suntrap", 0xEF2295C9, Manufacturer.Unknown, VehicleClass.Boats),
            new VehicleInfo("superd", 0x42F2ED16, Manufacturer.Enus, VehicleClass.Sedans),
            new VehicleInfo("supervolito", 0x2A54C47D, Manufacturer.Unknown, VehicleClass.Helicopters),
            new VehicleInfo("supervolito2", 0x9C5E5644, Manufacturer.Unknown, VehicleClass.Helicopters),
            new VehicleInfo("surano", 0x16E478C1, Manufacturer.Benefactor, VehicleClass.Sports),
            new VehicleInfo("surfer", 0x29B0DA97, Manufacturer.Burgerfahrzeug, VehicleClass.Vans),
            new VehicleInfo("surfer2", 0xB1D80E06, Manufacturer.Burgerfahrzeug, VehicleClass.Vans),
            new VehicleInfo("surge", 0x8F0E3594, Manufacturer.Cheval, VehicleClass.Sedans),
            new VehicleInfo("swift", 0xEBC24DF2, Manufacturer.Unknown, VehicleClass.Helicopters),
            new VehicleInfo("swift2", 0x4019CB4C, Manufacturer.Unknown, VehicleClass.Helicopters),
            new VehicleInfo("swinger", 0x1DD4C0FF, Manufacturer.Ocelot, VehicleClass.SportsClassics),
            new VehicleInfo("t20", 0x6322B39A, Manufacturer.Progen, VehicleClass.Super),
            new VehicleInfo("taco", 0x744CA80D, Manufacturer.Unknown, VehicleClass.Vans),
            new VehicleInfo("tailgater", 0xC3DDFDCE, Manufacturer.Obey, VehicleClass.Sedans),
            new VehicleInfo("tampa", 0x39F9C898, Manufacturer.Declasse, VehicleClass.Muscle),
            new VehicleInfo("tampa2", 0xC0240885, Manufacturer.Declasse, VehicleClass.Sports),
            new VehicleInfo("tampa3", 0xB7D9F7F1, Manufacturer.Declasse, VehicleClass.Muscle),
            new VehicleInfo("tanker", 0xD46F4737, Manufacturer.Unknown, VehicleClass.Trailer),
            new VehicleInfo("tanker2", 0x74998082, Manufacturer.Unknown, VehicleClass.Trailer),
            new VehicleInfo("tankercar", 0x22EDDC30, Manufacturer.Unknown, VehicleClass.Trains),
            new VehicleInfo("taxi", 0xC703DB5F, Manufacturer.Declasse, VehicleClass.Service),
            new VehicleInfo("technical", 0x83051506, Manufacturer.Unknown, VehicleClass.OffRoad),
            new VehicleInfo("technical2", 0x4662BCBB, Manufacturer.Unknown, VehicleClass.OffRoad),
            new VehicleInfo("technical3", 0x50D4D19F, Manufacturer.Unknown, VehicleClass.OffRoad),
            new VehicleInfo("tempesta", 0x1044926F, Manufacturer.Unknown, VehicleClass.Super),
            new VehicleInfo("terbyte", 0x897AFC65, Manufacturer.Benefactor, VehicleClass.Commercial),
            new VehicleInfo("thrust", 0x6D6F8F43, Manufacturer.Unknown, VehicleClass.Motorcycles),
            new VehicleInfo("thruster", 0x58CDAF30, Manufacturer.Unknown, VehicleClass.Military),
            new VehicleInfo("tiptruck", 0x2E19879, Manufacturer.Unknown, VehicleClass.Industrial),
            new VehicleInfo("tiptruck2", 0xC7824E5E, Manufacturer.Unknown, VehicleClass.Industrial),
            new VehicleInfo("titan", 0x761E2AD3, Manufacturer.Unknown, VehicleClass.Planes),
            new VehicleInfo("torero", 0x59A9E570, Manufacturer.Unknown, VehicleClass.SportsClassics),
            new VehicleInfo("tornado", 0x1BB290BC, Manufacturer.Declasse, VehicleClass.SportsClassics),
            new VehicleInfo("tornado2", 0x5B42A5C4, Manufacturer.Declasse, VehicleClass.SportsClassics),
            new VehicleInfo("tornado3", 0x690A4153, Manufacturer.Declasse, VehicleClass.SportsClassics),
            new VehicleInfo("tornado4", 0x86CF7CDD, Manufacturer.Declasse, VehicleClass.SportsClassics),
            new VehicleInfo("tornado5", 0x94DA98EF, Manufacturer.Declasse, VehicleClass.SportsClassics),
            new VehicleInfo("tornado6", 0xA31CB573, Manufacturer.Declasse, VehicleClass.SportsClassics),
            new VehicleInfo("toro", 0x3FD5AA2F, Manufacturer.Unknown, VehicleClass.Boats),
            new VehicleInfo("toro2", 0x362CAC6D, Manufacturer.Unknown, VehicleClass.Boats),
            new VehicleInfo("toros", 0xBA5334AC, Manufacturer.Unknown, VehicleClass.SUVs),
            new VehicleInfo("tourbus", 0x73B1C3CB, Manufacturer.Unknown, VehicleClass.Service),
            new VehicleInfo("towtruck", 0xB12314E0, Manufacturer.Vapid, VehicleClass.Utility),
            new VehicleInfo("towtruck2", 0xE5A2D6C6, Manufacturer.Vapid, VehicleClass.Utility),
            new VehicleInfo("tr2", 0x7BE032C6, Manufacturer.Unknown, VehicleClass.Trailer),
            new VehicleInfo("tr3", 0x6A59902D, Manufacturer.Unknown, VehicleClass.Trailer),
            new VehicleInfo("tr4", 0x7CAB34D0, Manufacturer.Unknown, VehicleClass.Trailer),
            new VehicleInfo("tractor", 0x61D6BA8C, Manufacturer.Stanley, VehicleClass.Utility),
            new VehicleInfo("tractor2", 0x843B73DE, Manufacturer.Stanley, VehicleClass.Utility),
            new VehicleInfo("tractor3", 0x562A97BD, Manufacturer.Stanley, VehicleClass.Utility),
            new VehicleInfo("trailerlogs", 0x782A236D, Manufacturer.Unknown, VehicleClass.Unknown),
            new VehicleInfo("trailers", 0xCBB2BE0E, Manufacturer.Unknown, VehicleClass.Trailer),
            new VehicleInfo("trailers2", 0xA1DA3C91, Manufacturer.Unknown, VehicleClass.Trailer),
            new VehicleInfo("trailers3", 0x8548036D, Manufacturer.Unknown, VehicleClass.Trailer),
            new VehicleInfo("trailersmall", 0x2A72BEAB, Manufacturer.Unknown, VehicleClass.Trailer),
            new VehicleInfo("trailersmall2", 0x8FD54EBB, Manufacturer.Unknown, VehicleClass.Military),
            new VehicleInfo("trash", 0x72435A19, Manufacturer.Jobuilt, VehicleClass.Service),
            new VehicleInfo("trash2", 0xB527915C, Manufacturer.Jobuilt, VehicleClass.Service),
            new VehicleInfo("trflat", 0xAF62F6B2, Manufacturer.Unknown, VehicleClass.Trailer),
            new VehicleInfo("tribike", 0x4339CD69, Manufacturer.Unknown, VehicleClass.Cycles),
            new VehicleInfo("tribike2", 0xB67597EC, Manufacturer.Unknown, VehicleClass.Cycles),
            new VehicleInfo("tribike3", 0xE823FB48, Manufacturer.Unknown, VehicleClass.Cycles),
            new VehicleInfo("trophytruck", 0x612F4B6, Manufacturer.Unknown, VehicleClass.OffRoad),
            new VehicleInfo("trophytruck2", 0xD876DBE2, Manufacturer.Unknown, VehicleClass.OffRoad),
            new VehicleInfo("tropic", 0x1149422F, Manufacturer.Unknown, VehicleClass.Boats),
            new VehicleInfo("tropic2", 0x56590FE9, Manufacturer.Unknown, VehicleClass.Boats),
            new VehicleInfo("tropos", 0x707E63A4, Manufacturer.Lampadati, VehicleClass.Sports),
            new VehicleInfo("tug", 0x82CAC433, Manufacturer.Unknown, VehicleClass.Boats),
            new VehicleInfo("tula", 0x3E2E4F8A, Manufacturer.Mammoth, VehicleClass.Planes),
            new VehicleInfo("tulip", 0x56D42971, Manufacturer.Unknown, VehicleClass.Muscle),
            new VehicleInfo("turismo2", 0xC575DF11, Manufacturer.Grotti, VehicleClass.SportsClassics),
            new VehicleInfo("turismor", 0x185484E1, Manufacturer.Grotti, VehicleClass.Super),
            new VehicleInfo("tvtrailer", 0x967620BE, Manufacturer.Unknown, VehicleClass.Trailer),
            new VehicleInfo("tyrus", 0x7B406EFB, Manufacturer.Progen, VehicleClass.Super),
            new VehicleInfo("utillitruck", 0x1ED0A534, Manufacturer.Unknown, VehicleClass.Utility),
            new VehicleInfo("utillitruck2", 0x34E6BF6B, Manufacturer.Unknown, VehicleClass.Utility),
            new VehicleInfo("utillitruck3", 0x7F2153DF, Manufacturer.Unknown, VehicleClass.Utility),
            new VehicleInfo("vacca", 0x142E0DC3, Manufacturer.Unknown, VehicleClass.Super),
            new VehicleInfo("vader", 0xF79A00F7, Manufacturer.Shitzu, VehicleClass.Motorcycles),
            new VehicleInfo("vagner", 0x7397224C, Manufacturer.Dewbauchee, VehicleClass.Super),
            new VehicleInfo("valkyrie", 0xA09E15FD, Manufacturer.Buckingham, VehicleClass.Helicopters),
            new VehicleInfo("valkyrie2", 0x5BFA5C4B, Manufacturer.Buckingham, VehicleClass.Helicopters),
            new VehicleInfo("vamos", 0xFD128DFD, Manufacturer.Unknown, VehicleClass.Muscle),
            new VehicleInfo("velum", 0x9C429B6A, Manufacturer.Jobuilt, VehicleClass.Planes),
            new VehicleInfo("velum2", 0x403820E8, Manufacturer.Jobuilt, VehicleClass.Planes),
            new VehicleInfo("verlierer2", 0x41B77FA4, Manufacturer.Bravado, VehicleClass.Sports),
            new VehicleInfo("vestra", 0x4FF77E37, Manufacturer.Unknown, VehicleClass.Planes),
            new VehicleInfo("vigero", 0xCEC6B9B7, Manufacturer.Unknown, VehicleClass.Muscle),
            new VehicleInfo("vigilante", 0xB5EF4C33, Manufacturer.Unknown, VehicleClass.Super),
            new VehicleInfo("vindicator", 0xAF599F01, Manufacturer.Unknown, VehicleClass.Motorcycles),
            new VehicleInfo("virgo", 0xE2504942, Manufacturer.Dundreary, VehicleClass.Muscle),
            new VehicleInfo("virgo2", 0xCA62927A, Manufacturer.Dundreary, VehicleClass.Muscle),
            new VehicleInfo("virgo3", 0xFDFFB0, Manufacturer.Dundreary, VehicleClass.Muscle),
            new VehicleInfo("viseris", 0xE8A8BA94, Manufacturer.Lampadati, VehicleClass.SportsClassics),
            new VehicleInfo("visione", 0xC4810400, Manufacturer.Grotti, VehicleClass.Super),
            new VehicleInfo("volatol", 0x1AAD0DED, Manufacturer.Unknown, VehicleClass.Planes),
            new VehicleInfo("volatus", 0x920016F1, Manufacturer.Buckingham, VehicleClass.Helicopters),
            new VehicleInfo("voltic", 0x9F4B77BE, Manufacturer.Coil, VehicleClass.Super),
            new VehicleInfo("voltic2", 0x3AF76F4A, Manufacturer.Coil, VehicleClass.Super),
            new VehicleInfo("voodoo", 0x779B4F2D, Manufacturer.Unknown, VehicleClass.Muscle),
            new VehicleInfo("voodoo2", 0x1F3766E3, Manufacturer.Unknown, VehicleClass.Muscle),
            new VehicleInfo("vortex", 0xDBA9DBFC, Manufacturer.Unknown, VehicleClass.Motorcycles),
            new VehicleInfo("warrener", 0x51D83328, Manufacturer.Vulcar, VehicleClass.Sedans),
            new VehicleInfo("washington", 0x69F06B57, Manufacturer.Albany, VehicleClass.Sedans),
            new VehicleInfo("wastelander", 0x8E08EC82, Manufacturer.MTL, VehicleClass.Service),
            new VehicleInfo("windsor", 0x5E4327C8, Manufacturer.Enus, VehicleClass.Coupes),
            new VehicleInfo("windsor2", 0x8CF5CAE1, Manufacturer.Enus, VehicleClass.Coupes),
            new VehicleInfo("wolfsbane", 0xDB20A373, Manufacturer.Unknown, VehicleClass.Motorcycles),
            new VehicleInfo("xa21", 0x36B4A8A9, Manufacturer.Ocelot, VehicleClass.Super),
            new VehicleInfo("xls", 0x47BBCF2E, Manufacturer.Benefactor, VehicleClass.SUVs),
            new VehicleInfo("xls2", 0xE6401328, Manufacturer.Benefactor, VehicleClass.SUVs),
            new VehicleInfo("yosemite", 0x6F946279, Manufacturer.Unknown, VehicleClass.Muscle),
            new VehicleInfo("youga", 0x03E5F6B8, Manufacturer.Bravado, VehicleClass.Vans),
            new VehicleInfo("youga2", 0x3D29CD2B, Manufacturer.Bravado, VehicleClass.Vans),
            new VehicleInfo("zentorno", 0xAC5DF515, Manufacturer.Pegassi, VehicleClass.Super),
            new VehicleInfo("zion", 0xBD1B39C3, Manufacturer.Ubermacht, VehicleClass.Coupes),
            new VehicleInfo("zion2", 0xB8E2AE18, Manufacturer.Ubermacht, VehicleClass.Coupes),
            new VehicleInfo("zion3", 0x6F039A67, Manufacturer.Ubermacht, VehicleClass.SportsClassics),
            new VehicleInfo("zombiea", 0xC3D7C72B, Manufacturer.LibertyChopShop, VehicleClass.Motorcycles),
            new VehicleInfo("zombieb", 0xDE05FB87, Manufacturer.LibertyChopShop, VehicleClass.Motorcycles),
            new VehicleInfo("zr380", 0x20314B42, Manufacturer.Unknown, VehicleClass.Sports),
            new VehicleInfo("zr3802", 0xBE11EFC6, Manufacturer.Unknown, VehicleClass.Sports),
            new VehicleInfo("zr3803", 0xA7DCC35C, Manufacturer.Unknown, VehicleClass.Sports),
            new VehicleInfo("ztype", 0x2D3BD401, Manufacturer.Truffade, VehicleClass.SportsClassics)
        };
    }
    public class VehicleInfo
    {
        public string Name { get; set; }
        public uint Hash { get; set; }
        public Manufacturer Manufacturer { get; set; }
        public VehicleClass VehicleClass { get; set; }
        public string ModelScannerFile { get; set; } = "";
        public VehicleInfo()
        {

        }
        public VehicleInfo(string _Name, uint _Hash)
        {
            Name = _Name;
            Hash = _Hash;
            Manufacturer = Manufacturer.Unknown;
            VehicleClass = VehicleClass.Unknown;

        }
        public VehicleInfo(string _Name, uint _Hash, Manufacturer _Manufacturer, VehicleClass _VehicleClass)
        {
            Name = _Name;
            Hash = _Hash;
            Manufacturer = _Manufacturer;
            VehicleClass = _VehicleClass;

        }
        public VehicleInfo(string _Name, uint _Hash, Manufacturer _Manufacturer, VehicleClass _VehicleClass, string _ModelScannerFile)
        {
            Name = _Name;
            Hash = _Hash;
            Manufacturer = _Manufacturer;
            VehicleClass = _VehicleClass;
            ModelScannerFile = _ModelScannerFile;
        }
    }
}

