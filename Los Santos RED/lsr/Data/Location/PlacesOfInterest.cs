﻿using LosSantosRED.lsr;
using LosSantosRED.lsr.Helper;
using LosSantosRED.lsr.Interface;
using Rage;
using Rage.Native;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class PlacesOfInterest : IPlacesOfInterest
{
    private readonly string ConfigFileName = "Plugins\\LosSantosRED\\Locations.xml";
    private List<GameLocation> LocationsList;
    private List<MenuItem> ToolMenu;
    private List<MenuItem> CheapHotelMenu;
    private List<MenuItem> ExpensiveHotelMenu;
    private List<MenuItem> HookerMenu;
    private List<MenuItem> ConvenienceStoreMenu;
    private List<MenuItem> TwentyFourSevenMenu;
    private List<MenuItem> ConvenienceAndLiquorStoreMenu;
    private List<MenuItem> FancyDeliMenu;
    private List<MenuItem> FancyFishMenu;
    private List<MenuItem> FancyGenericMenu;
    private List<MenuItem> GrainOfTruthMenu;
    private List<MenuItem> FruitVineMenu;
    private List<MenuItem> GasStationMenu;
    private List<MenuItem> RonMenu;
    private List<MenuItem> XeroMenu;
    private List<MenuItem> LTDMenu;
    private List<MenuItem> GenericMenu;
    private List<MenuItem> ChihuahuaHotDogMenu;
    private List<MenuItem> BeefyBillsMenu;
    private List<MenuItem> PizzaMenu;
    private List<MenuItem> DonutMenu;
    private List<MenuItem> StoreMenu;
    private List<MenuItem> FruitMenu;
    private List<MenuItem> UpNAtomMenu;
    private List<MenuItem> TacoFarmerMenu;
    private List<MenuItem> HeadShopMenu;
    private List<MenuItem> LiquorStoreMenu;
    private List<MenuItem> BarMenu;
    private List<MenuItem> CoffeeMenu;
    private List<MenuItem> SandwichMenu;
    private List<MenuItem> BiteMenu;
    private List<MenuItem> TacoBombMenu;
    private List<MenuItem> BurgerShotMenu;
    private List<MenuItem> WigwamMenu;
    private List<MenuItem> ViceroyMenu;
    private List<MenuItem> CluckinBellMenu;
    private List<MenuItem> AlDentesMenu;
    private List<MenuItem> BenefactorGallavanterMenu;
    private List<MenuItem> NoodleMenu;
    private List<MenuItem> WeedMenu;
    private List<MenuItem> WeedAndCigMenu;
    private List<MenuItem> WeedDealerMenu;

    public PlacesOfInterest()
    {

    }

    public void ReadConfig()
    {
        if (File.Exists(ConfigFileName))
        {
            LocationsList = Serialization.DeserializeParams<GameLocation>(ConfigFileName);
        }
        else
        {
            DefaultConfig();
            Serialization.SerializeParams(LocationsList, ConfigFileName);
        }
    }
    public List<GameLocation> GetAllPlaces()
    {
        return LocationsList;
    }
    public List<GameLocation> GetAllStores()
    {
        return LocationsList.Where(x => x.CanTransact).ToList();
    }
    public GameLocation GetClosestLocation(Vector3 Position,LocationType Type)
    {
        return LocationsList.Where(x => x.Type == Type).OrderBy(s => Position.DistanceTo2D(s.EntrancePosition)).FirstOrDefault();
    }
    public List<GameLocation> GetLocations(LocationType Type)
    {
        return LocationsList.Where(x => x.Type == Type).ToList();
    }
    private void CreateMenus()
    {
        CreateGenericMenus();
        CreateSpecificMenus();
    }
    private void CreateGenericMenus()
    {
        ToolMenu = new List<MenuItem>() {
            new MenuItem("Screwdriver",19),
            new MenuItem("Hammer", 15),
            new MenuItem("Drill", 50),
            new MenuItem("Pliers", 20),
            new MenuItem("Shovel", 60),
            new MenuItem("Wrench", 24),};
        CheapHotelMenu = new List<MenuItem>() {
            new MenuItem("Room: Single Twin",99),
            new MenuItem("Room: Single Queen", 130),
            new MenuItem("Room: Double Queen", 150),
            new MenuItem("Room: Single King", 160), };
        ExpensiveHotelMenu = new List<MenuItem>() {
            new MenuItem("Room: Single Queen", 189),
            new MenuItem("Room: Double Queen", 220),
            new MenuItem("Room: Single King", 250),
            new MenuItem("Room: Delux", 280), };
        HookerMenu = new List<MenuItem>() {
            new MenuItem("Handy", 50),
            new MenuItem("Head", 75),
            new MenuItem("Half And Half", 150),
            new MenuItem("Full",200),};
        ConvenienceStoreMenu = new List<MenuItem>() {
            new MenuItem("Hot Dog", 5),
            new MenuItem("Burger",3),
            new MenuItem("Phat Chips", 2),
            new MenuItem("Donut", 1),
            new MenuItem("Redwood Regular", 30),
            new MenuItem("Redwood Mild", 32),
            new MenuItem("Debonaire", 35),
            new MenuItem("Debonaire Menthol", 38),
            new MenuItem("Caradique", 35),
            new MenuItem("69 Brand", 40),
            new MenuItem("Estancia Cigar", 50),
            new MenuItem("Lighter", 5),
            new MenuItem("Can of eCola", 1),
            new MenuItem("Can of Sprunk", 1),
            new MenuItem("Bottle of PiBwasser", 3),
            new MenuItem("Bottle of Jakeys", 3),
            new MenuItem("Cup of Coffee", 2),
            new MenuItem("Bottle of Raine Water", 2) };
        SandwichMenu = new List<MenuItem>() {
            new MenuItem("Ham and Cheese Sandwich", 2),
            new MenuItem("Turkey Sandwich", 2),
            new MenuItem("Tuna Sandwich", 2),
            new MenuItem("Phat Chips", 2),
            new MenuItem("Bottle of Raine Water", 2),
            new MenuItem("Can of eCola", 1),
            new MenuItem("Can of Sprunk", 1) };
        HeadShopMenu = new List<MenuItem>() {
            new MenuItem("Joint", 25),
            new MenuItem("Redwood Regular", 30),
            new MenuItem("Redwood Mild", 32),
            new MenuItem("Debonaire", 35),
            new MenuItem("Debonaire Menthol", 38),
            new MenuItem("Caradique", 35),
            new MenuItem("69 Brand", 40),
            new MenuItem("Estancia Cigar", 50),
            new MenuItem("Lighter", 5),
            new MenuItem("Bottle of Raine Water", 2) };
        LiquorStoreMenu = new List<MenuItem>() {
            new MenuItem("40 oz", 5),
            new MenuItem("Bottle of Barracho", 3),
            new MenuItem("Bottle of PiBwasser", 3),
            new MenuItem("Bottle of Blarneys", 3),
            new MenuItem("Bottle of Logger", 3,1),
            new MenuItem("Bottle of Patriot", 3,1),
            new MenuItem("Bottle of Pride", 3),
            new MenuItem("Bottle of Stronz", 4),
            new MenuItem("Bottle of A.M.", 4),
            new MenuItem("Bottle of Jakeys", 4),
            new MenuItem("Bottle of Dusche", 4) };
        BarMenu = new List<MenuItem>() {
            new MenuItem("Burger", 5),
            new MenuItem("Hot Dog", 5),
            new MenuItem("Bottle of Raine Water", 2),
            new MenuItem("Cup of eCola", 2),
            new MenuItem("40 oz", 5),
            new MenuItem("Bottle of Barracho", 4),
            new MenuItem("Bottle of PiBwasser", 4),
            new MenuItem("Bottle of Blarneys", 5),
            new MenuItem("Bottle of Logger", 5),
            new MenuItem("Bottle of Patriot", 5),
            new MenuItem("Bottle of Pride", 4),
            new MenuItem("Bottle of Stronz", 5),
            new MenuItem("Bottle of A.M.", 4),
            new MenuItem("Bottle of Jakeys", 5),
            new MenuItem("Bottle of Dusche", 5) };
        CoffeeMenu = new List<MenuItem>() {
            new MenuItem("Cup of Coffee", 2),
            new MenuItem("Donut", 5),
            new MenuItem("Bottle of Raine Water", 2) };
        GenericMenu = new List<MenuItem>() {
            new MenuItem("Burger",3),
            new MenuItem("Phat Chips", 2),
            new MenuItem("Can of eCola", 1),
            new MenuItem("Can of Sprunk", 1),
            new MenuItem("Bottle of Raine Water", 2) };
        PizzaMenu = new List<MenuItem>() {
            new MenuItem("Slice of Pizza", 3),
            new MenuItem("Cup of Sprunk", 2),
            new MenuItem("Bottle of A.M.", 3),
            new MenuItem("Bottle of PiBwasser", 3),
            new MenuItem("Bottle of Barracho", 4),
            new MenuItem("Bottle of Blarneys", 4),
            new MenuItem("Bottle of Jakeys", 3),
            new MenuItem("Bottle of Stronz", 4),
            new MenuItem("Bottle of Dusche", 3) };
        DonutMenu = new List<MenuItem>() {
            new MenuItem("Hot Dog", 5),
            new MenuItem("Phat Chips", 2),
            new MenuItem("Donut", 1),
            new MenuItem("Can of eCola", 1),
            new MenuItem("Cup of eCola", 2),
            new MenuItem("Cup of Coffee", 3) };
        StoreMenu = new List<MenuItem>() {
            new MenuItem("Joint", 25),
            new MenuItem("Redwood Regular", 30),
            new MenuItem("Redwood Mild", 32),
            new MenuItem("Debonaire", 35),
            new MenuItem("Estancia Cigar", 50),
            new MenuItem("Cup of Sprunk", 2),
            new MenuItem("Banana", 3),
            new MenuItem("Donut", 1),
            new MenuItem("Hot Pretzel", 2),
            new MenuItem("40 oz", 5),
            new MenuItem("Bottle of Barracho",3),
            new MenuItem("Bottle of PiBwasser", 3),
            new MenuItem("Bottle of Blarneys",3),
            new MenuItem("Bottle of Logger", 4),
            new MenuItem("Bottle of Patriot",4),
            new MenuItem("Bottle of Pride", 3),
            new MenuItem("Bottle of Stronz", 4) };
        FruitMenu = new List<MenuItem>() {
            new MenuItem("Banana", 2),
            new MenuItem("Orange", 2),
            new MenuItem("Apple", 2),
            new MenuItem("Nuts", 2),
            new MenuItem("Bottle of Raine Water", 2),
            new MenuItem("Can of eCola", 1),
            new MenuItem("Can of Sprunk", 1) };
        GasStationMenu = new List<MenuItem>() {
            new MenuItem("Hot Dog", 5),
            new MenuItem("Burger",3),
            new MenuItem("Phat Chips", 2),
            new MenuItem("Donut", 1),
            new MenuItem("Redwood Regular", 30),
            new MenuItem("Redwood Mild", 32),
            new MenuItem("Debonaire", 35),
            new MenuItem("Estancia Cigar", 50),
            new MenuItem("Lighter", 5),
            new MenuItem("Can of eCola", 1),
            new MenuItem("Can of Sprunk", 1),
            new MenuItem("Bottle of PiBwasser", 3),
            new MenuItem("Bottle of Jakeys", 3),
            new MenuItem("Cup of Coffee", 2),
            new MenuItem("Bottle of Raine Water", 2) };
        ConvenienceAndLiquorStoreMenu = new List<MenuItem>() {
            new MenuItem("Phat Chips", 2),
            new MenuItem("Donut", 1),
            new MenuItem("Redwood Regular", 30),
            new MenuItem("Redwood Mild", 32),
            new MenuItem("Debonaire", 35),
            new MenuItem("Debonaire Menthol", 38),
            new MenuItem("Caradique", 35),
            new MenuItem("69 Brand", 40),
            new MenuItem("Estancia Cigar", 50),
            new MenuItem("Lighter", 5),
            new MenuItem("Can of eCola", 1),
            new MenuItem("Can of Sprunk", 1),
            new MenuItem("Bottle of Barracho", 3),
            new MenuItem("Bottle of PiBwasser", 3),
            new MenuItem("Bottle of Blarneys", 3),
            new MenuItem("Bottle of Logger", 3),
            new MenuItem("Bottle of Patriot", 3),
            new MenuItem("Bottle of Pride", 3),
            new MenuItem("Bottle of Stronz", 4),
            new MenuItem("Bottle of A.M.", 4),
            new MenuItem("Bottle of Jakeys", 4),
            new MenuItem("Bottle of Dusche", 4),
            new MenuItem("Cup of Coffee", 2),
            new MenuItem("Bottle of Raine Water", 2) };
        FancyDeliMenu = new List<MenuItem>() {
            new MenuItem("Chicken Club Salad",10),
            new MenuItem("Spicy Seafood Gumbo",14),
            new MenuItem("Muffaletta",8),
            new MenuItem("Zucchini Garden Pasta",9),
            new MenuItem("Pollo Mexicano",12),
            new MenuItem("Italian Cruz Po'boy",19),
            new MenuItem("Chipotle Chicken Panini",10),
            new MenuItem("Bottle of Raine Water",2),
            new MenuItem("Cup of eCola",2),
            new MenuItem("Cup of Sprunk",2),};
        FancyFishMenu = new List<MenuItem>() {
            new MenuItem("Coconut Crusted Prawns",12),
            new MenuItem("Crab and Shrimp Louie",10),
            new MenuItem("Open-Faced Crab Melt",28),
            new MenuItem("King Salmon",48),
            new MenuItem("Ahi Tuna",44),
            new MenuItem("Key Lime Pie",13),
            new MenuItem("Bottle of Raine Water",2), };
        FancyGenericMenu = new List<MenuItem>() {
            new MenuItem("Smokehouse Burger",10),
            new MenuItem("Chicken Critters Basket",7),
            new MenuItem("Prime Rib 16 oz",22),
            new MenuItem("Bone-In Ribeye",25),
            new MenuItem("Grilled Pork Chops",14),
            new MenuItem("Grilled Shrimp",15),
            new MenuItem("Bottle of Raine Water",2),
            new MenuItem("Cup of eCola",2),
            new MenuItem("Cup of Sprunk",2),};
        NoodleMenu = new List<MenuItem>() {
            new MenuItem("Juek Suk tong Mandu",8),
            new MenuItem("Hayan Jam Pong",9),
            new MenuItem("Sal Gook Su Jam Pong",12),
            new MenuItem("Chul Pan Bokkeum Jam Pong",20),
            new MenuItem("Deul Gae Udon",12),
            new MenuItem("Dakgogo Bokkeum Bap",9),
            new MenuItem("Bottle of Raine Water",2),
            new MenuItem("Cup of eCola",2),
            new MenuItem("Cup of Sprunk",2),};
        WeedMenu = new List<MenuItem>() {
            new MenuItem("White Widow Preroll",2),
            new MenuItem("OG Kush Preroll",3),
            new MenuItem("Northern Lights Preroll",3),
            new MenuItem("White Widow Gram",7),
            new MenuItem("OG Kush Gram",8),
            new MenuItem("Northern Lights Gram",9),
            new MenuItem("Bong",25),
            new MenuItem("Lighter",5),};
        WeedAndCigMenu = new List<MenuItem>() {
            new MenuItem("White Widow Preroll",2),
            new MenuItem("OG Kush Preroll",3),
            new MenuItem("Northern Lights Preroll",3),
            new MenuItem("White Widow Gram",7),
            new MenuItem("OG Kush Gram",8),
            new MenuItem("Northern Lights Gram",9),
            new MenuItem("Bong",25),
            new MenuItem("Redwood Regular", 30),
            new MenuItem("Redwood Mild", 32),
            new MenuItem("Debonaire", 35),
            new MenuItem("Debonaire Menthol", 38),
            new MenuItem("Caradique", 35),
            new MenuItem("69 Brand", 40),
            new MenuItem("Estancia Cigar", 50),
            new MenuItem("Lighter",5),};
        WeedDealerMenu = new List<MenuItem>() {
            new MenuItem("Gram of Schwag",6, 1),
            new MenuItem("Gram of Mids",9, 3),
            new MenuItem("Gram of Dank",12, 4),
            new MenuItem("Joint",3, 1)};
    }
    private void CreateSpecificMenus()
    {
        BurgerShotMenu = new List<MenuItem>
        {
            new MenuItem("Money Shot Meal", 7),
            new MenuItem("The Bleeder Meal", 4),
            new MenuItem("Torpedo Meal", 6),
            new MenuItem("Meat Free Meal", 5),
            new MenuItem("Freedom Fries", 2),
            new MenuItem("Liter of eCola", 2),
            new MenuItem("Liter of Sprunk", 2),
            new MenuItem("Bottle of Raine Water", 2),
            new MenuItem("Double Shot Coffee", 2) };
        UpNAtomMenu = new List<MenuItem>() {
            new MenuItem("Triple Burger", 4),
            new MenuItem("Bacon Triple Cheese Melt", 3),
            new MenuItem("Jumbo Shake", 5),
            new MenuItem("Bacon Burger", 2),
            new MenuItem("French Fries", 2),
            new MenuItem("Cup of eCola", 2),
            new MenuItem("Cup of Sprunk", 2),
            new MenuItem("Cup of Coffee", 3),
            new MenuItem("Bottle of Raine Water", 5) };
        BeefyBillsMenu = new List<MenuItem>() {
            new MenuItem("Burger", 3),
            new MenuItem("Megacheese Burger", 2),
            new MenuItem("Double Burger", 2),
            new MenuItem("Kingsize Burger", 2),
            new MenuItem("Bacon Burger", 2),
            new MenuItem("French Fries", 2),
            new MenuItem("Can of eCola", 1),
            new MenuItem("Can of Sprunk", 1),
            new MenuItem("Bottle of Raine Water", 2) };
        ChihuahuaHotDogMenu = new List<MenuItem>() {
            new MenuItem("Hot Dog", 5, 2),
            new MenuItem("Hot Sausage", 5),
            new MenuItem("Hot Pretzel", 2),
            new MenuItem("3 Mini Pretzels", 3),
            new MenuItem("Nuts", 2),
            new MenuItem("Can of Sprunk", 1, 1),
            new MenuItem("Bottle of Raine Water", 2, 1) };
        TacoFarmerMenu = new List<MenuItem>() {
            new MenuItem("Taco", 2),
            new MenuItem("Can of eCola", 1),
            new MenuItem("Can of Sprunk", 1),
            new MenuItem("Cup of Coffee", 3),
            new MenuItem("Bottle of Raine Water", 2) };
        BiteMenu = new List<MenuItem>() {
            new MenuItem("Gut Buster Sandwich", 9),
            new MenuItem("Ham and Tuna Sandwich", 7),
            new MenuItem("Chef's Salad", 4),
            new MenuItem("Cup of eCola", 1),
            new MenuItem("Cup of Sprunk", 1),
            new MenuItem("Bottle of Raine Water", 2) };
        TacoBombMenu = new List<MenuItem> {
            new MenuItem("Breakfast Burrito",4),
            new MenuItem("Deep Fried Salad",7),
            new MenuItem("Beef Bazooka",8),
            new MenuItem("Chimichingado Chiquito",5),
            new MenuItem("Cheesy Meat Flappers",6),
            new MenuItem("Volcano Mudsplatter Nachos",7),
            new MenuItem("Can of eCola", 1),
            new MenuItem("Can of Sprunk", 1),
            new MenuItem("Bottle of Raine Water", 2) };
        WigwamMenu = new List<MenuItem>() {
            new MenuItem("Wigwam Burger", 3),
            new MenuItem("Wigwam Cheeseburger", 2),
            new MenuItem("Big Wig Burger", 5),
            new MenuItem("French Fries", 2),
            new MenuItem("Cup of eCola", 1),
            new MenuItem("Cup of Sprunk", 1),
            new MenuItem("Bottle of Raine Water", 2) };
        ViceroyMenu = new List<MenuItem>() {
            new MenuItem("City View King",354),
            new MenuItem("City View Deluxe King", 378),
            new MenuItem("Partial Ocean View King", 392),
            new MenuItem("Ocean View King", 423),
            new MenuItem("City View Two Bedded Room", 456),
            new MenuItem("Grande King", 534),
            new MenuItem("Grande Ocean View King", 647),
            new MenuItem("Empire Suite", 994),
            new MenuItem("Monarch Suite", 1327), };
        CluckinBellMenu = new List<MenuItem>() {
            new MenuItem("Cluckin' Little Meal",2),
            new MenuItem("Cluckin' Big Meal",6),
            new MenuItem("Cluckin' Huge Meal",12),
            new MenuItem("Wing Piece",7),
            new MenuItem("Little Peckers",8),
            new MenuItem("Balls & Rings",4),
            new MenuItem("Fries",2),
            new MenuItem("Fowlburger",5),
            new MenuItem("Cup Of Coffee",3),
            new MenuItem("Cup of eCola",2),
            new MenuItem("Cup of Sprunk",2), };
        AlDentesMenu = new List<MenuItem>() {
            new MenuItem("Slice of Pizza", 3, 2),
            new MenuItem("Cup of Sprunk", 2, 1),
            new MenuItem("Bottle of A.M.", 3, 1),
            new MenuItem("Bottle of PiBwasser", 3),
            new MenuItem("Bottle of PiBwasser", -1,2),
            new MenuItem("Bottle of Barracho", 4),
            new MenuItem("Bottle of Blarneys", 4),
            new MenuItem("Bottle of Jakeys", 3),
            new MenuItem("Bottle of Stronz", 4),
            new MenuItem("Bottle of Dusche", 3) };
        BenefactorGallavanterMenu = new List<MenuItem>() {
            new MenuItem("Gallivanter Baller",67000),
            new MenuItem("Gallivanter Baller II",90000),
            new MenuItem("Gallivanter Baller LE",149000),
            new MenuItem("Gallivanter Baller LE LWB",247000),
            new MenuItem("Benefactor Schafter",65000),
            new MenuItem("Benefactor Schafter V12",112000),
            new MenuItem("Benefactor Feltzer",145000),
            new MenuItem("Benefactor Schwartzer",48000),
            new MenuItem("Benefactor Surano",110000),
            new MenuItem("Benefactor Serrano",60000),
            new MenuItem("Benefactor Dubsta",110000),
            new MenuItem("Benefactor Dubsta 2",120000),
            new MenuItem("Benefactor XLS",151000),
            new MenuItem("Benefactor Streiter",156000),
            new MenuItem("Benefactor Schlagen GT",500000),
            new MenuItem("Benefactor Krieger",750000),
        };
        TwentyFourSevenMenu = ConvenienceStoreMenu;
        GrainOfTruthMenu = ConvenienceAndLiquorStoreMenu;
        FruitVineMenu = ConvenienceAndLiquorStoreMenu;
        RonMenu = GasStationMenu;
        XeroMenu = GasStationMenu;
        LTDMenu = GasStationMenu;
    }
    private void DefaultConfig()
    {
        CreateMenus();
        LocationsList = new List<GameLocation>
        {
            //Hospital
            new GameLocation(new Vector3(364.7124f, -583.1641f, 28.69318f), 280.637f, LocationType.Hospital, "Pill Box Hill Hospital",""),
            new GameLocation(new Vector3(338.208f, -1396.154f, 32.50927f), 77.07102f, LocationType.Hospital, "Central Los Santos Hospital",""),
            new GameLocation(new Vector3(1842.057f, 3668.679f, 33.67996f), 228.3818f, LocationType.Hospital, "Sandy Shores Hospital",""),
            new GameLocation(new Vector3(-244.3214f, 6328.575f, 32.42618f), 219.7734f, LocationType.Hospital, "Paleto Bay Hospital",""),

            //Grave
            new GameLocation(new Vector3(-1654.301f, -148.7047f, 59.91496f), 299.5774f, LocationType.Grave, "Grave 1",""),

            //Police
            new GameLocation(new Vector3(358.9726f, -1582.881f, 29.29195f), 323.5287f, LocationType.Police, "Davis Police Station",""),
            new GameLocation(new Vector3(1858.19f, 3679.873f, 33.75724f), 218.3256f, LocationType.Police, "Sandy Shores Police Station",""),
            new GameLocation(new Vector3(-437.973f, 6021.403f, 31.49011f), 316.3756f, LocationType.Police, "Paleto Bay Police Station",""),
            new GameLocation(new Vector3(440.0835f, -982.3911f, 30.68966f), 47.88088f, LocationType.Police, "Mission Row Police Station",""),
            new GameLocation(new Vector3(815.8774f, -1290.531f, 26.28391f), 74.91704f, LocationType.Police, "La Mesa Police Station",""),
            new GameLocation(new Vector3(642.1356f, -3.134667f, 82.78872f), 215.299f, LocationType.Police, "Vinewood Police Station",""),
            new GameLocation(new Vector3(-557.0687f, -134.7315f, 38.20231f), 214.5968f, LocationType.Police, "Rockford Hills Police Station",""),
            new GameLocation(new Vector3(-1093.817f, -807.1993f, 19.28864f), 22.23846f, LocationType.Police, "Vespucci Police Station",""),
            new GameLocation(new Vector3(-1633.314f, -1010.025f, 13.08503f), 351.7007f, LocationType.Police, "Del Perro Police Station",""),
            new GameLocation(new Vector3(-1311.877f, -1528.808f, 4.410581f), 233.9121f, LocationType.Police, "Vespucci Beach Police Station",""),
            
            
            //Stores

            //Liquor
            new GameLocation(new Vector3(-1226.09f, -896.166f, 12.4057f), 22.23846f,new Vector3(-1221.119f, -908.5667f, 12.32635f), 33.35855f, LocationType.LiquorStore, "Rob's Liquors","Thats My Name, Don't Rob Me!") { Menu = LiquorStoreMenu, OpenTime = 4, CloseTime = 22 },
            new GameLocation(new Vector3(-2966.361f, 390.1463f, 15.04331f), 88.73301f,new Vector3(-2966.361f, 390.1463f, 15.04331f), 88.73301f, LocationType.LiquorStore, "Rob's Liquors", "Thats My Name, Don't Rob Me!"){ Menu = LiquorStoreMenu, OpenTime = 4, CloseTime = 22 },
            new GameLocation(new Vector3(-1208.469f, -1384.053f, 4.085135f), 68.08948f, LocationType.LiquorStore, "Steamboat Beers", "Steamboat Beers") { Menu = LiquorStoreMenu },
            new GameLocation(new Vector3(-1106.07f, -1287.686f, 5.421459f), 161.3398f, LocationType.LiquorStore, "Vespucci Liquor Market", "Vespucci Liquor Market") { Menu = LiquorStoreMenu },
            new GameLocation(new Vector3(-1486.196f, -377.7115f, 40.16343f), 133.357f,new Vector3(-1486.196f, -377.7115f, 40.16343f), 133.357f, LocationType.LiquorStore, "Rob's Liquors", "Rob's Liquors") { Menu = LiquorStoreMenu, OpenTime = 4, CloseTime = 22 },
            new GameLocation(new Vector3(-697.8242f, -1182.286f, 10.71113f), 132.7831f, LocationType.LiquorStore, "Liquor Market", "Liquor Market") { Menu = LiquorStoreMenu, OpenTime = 4, CloseTime = 22 },
            new GameLocation(new Vector3(-882.7062f, -1155.351f, 5.162508f), 215.8305f, LocationType.LiquorStore, "Liquor Hole", "Liquor Hole") { Menu = LiquorStoreMenu, OpenTime = 4, CloseTime = 22,BannerImage = "liquorhole.png" },
            new GameLocation(new Vector3(456.5478f, 130.5207f, 99.28537f), 162.9724f, LocationType.LiquorStore, "Vinewood Liquor", "Vinewood Liquor") { Menu = LiquorStoreMenu, OpenTime = 4, CloseTime = 22 },
            new GameLocation(new Vector3(1391.861f, 3606.275f, 34.98093f), 199.2899f,new Vector3(1391.861f, 3606.275f, 34.98093f), 199.2899f, LocationType.LiquorStore, "Liquor Ace", "Liquor Ace") { Menu = LiquorStoreMenu, OpenTime = 4, CloseTime = 22 },
            new GameLocation(new Vector3(1952.552f, 3840.833f, 32.17612f), 298.8575f, LocationType.LiquorStore, "Sandy Shores Liquor", "Sandy Shores Liquor") { Menu = LiquorStoreMenu, OpenTime = 4, CloseTime = 22 },            
            new GameLocation(new Vector3(2455.443f, 4058.518f, 38.06472f), 250.6311f, LocationType.LiquorStore, "Liquor Market 24/7", "Liquor Market"){ Menu = LiquorStoreMenu, OpenTime = 0, CloseTime = 24 },
            new GameLocation(new Vector3(2481.348f, 4100.31f, 38.13171f), 249.6295f, LocationType.LiquorStore, "Liquor Store", "Liquor Store"){ Menu = LiquorStoreMenu, OpenTime = 4, CloseTime = 22 },
            new GameLocation(new Vector3(2566.804f, 4274.108f, 41.98908f), 239.0765f, LocationType.LiquorStore, "Grape Smuggler's Liquor", "Grape Liquor"){ Menu = LiquorStoreMenu, OpenTime = 4, CloseTime = 22 },
            new GameLocation(new Vector3(98.82836f, -1308.733f, 29.27369f), 121.1285f, LocationType.LiquorStore, "The Brewer's Drop", ""){ Menu = LiquorStoreMenu, OpenTime = 4, CloseTime = 22 },
            new GameLocation(new Vector3(169.9896f, -1336.975f, 29.30038f), 284.8854f, LocationType.LiquorStore, "Liquor Beer & Wine", ""){ Menu = LiquorStoreMenu, OpenTime = 4, CloseTime = 22 },
            new GameLocation(new Vector3(-43.36616f, -1042.139f, 28.33997f), 76.74245f, LocationType.LiquorStore, "Downtown Liquor.Deli", ""){ Menu = LiquorStoreMenu, OpenTime = 4, CloseTime = 22 },
            new GameLocation(new Vector3(382.3881f, -1076.69f, 29.42185f), 267.7594f, LocationType.LiquorStore, "Downtown Liquor.Deli", ""){ Menu = LiquorStoreMenu, OpenTime = 4, CloseTime = 22 },
            new GameLocation(new Vector3(-57.89392f, -91.45875f, 57.75766f), 118.0359f, LocationType.LiquorStore, "Liquor", ""){ Menu = LiquorStoreMenu, OpenTime = 4, CloseTime = 22 },
            new GameLocation(new Vector3(456.2009f, -2059.208f, 23.9267f), 274.6541f, LocationType.LiquorStore, "South LS Liquor", ""){ Menu = LiquorStoreMenu, OpenTime = 4, CloseTime = 22 },
            new GameLocation(new Vector3(129.7891f, -1643.382f, 29.29159f), 38.8853f, LocationType.LiquorStore, "Liquor", ""){ Menu = LiquorStoreMenu, OpenTime = 4, CloseTime = 22 },
            new GameLocation(new Vector3(463.9041f, -1852.109f, 27.79801f), 3.177461f, LocationType.LiquorStore, "Liquor Store", ""){ Menu = LiquorStoreMenu, OpenTime = 4, CloseTime = 22 },
            new GameLocation(new Vector3(-156.2888f, 6327.238f, 31.58083f), 316.2542f, LocationType.LiquorStore, "Del Vecchio Liquor", ""){ Menu = LiquorStoreMenu, OpenTime = 4, CloseTime = 22 },
           
            
            
            //HeadShop
            new GameLocation(new Vector3(-1191.582f, -1197.779f, 7.617113f), 146.801f, LocationType.Headshop, "Pipe Dreams", "Pipe Dreams") {Menu = HeadShopMenu },
            new GameLocation(new Vector3(65.60603f, -137.4155f, 55.11251f), 214.0327f, LocationType.Headshop, "Pipe Dreams", "") {Menu = HeadShopMenu },
            new GameLocation(new Vector3(278.8327f, -1027.653f, 29.21136f), 184.1326f, LocationType.Headshop, "Pipe Down Cigars", "") {Menu = HeadShopMenu },
            new GameLocation(new Vector3(-1154.942f, -1373.176f, 5.061489f), 305.589f, LocationType.Headshop, "Amnesiac Smoke Shop", "Amnesiac Smoke Shop") {Menu = WeedAndCigMenu },

            //Dispensary
            new GameLocation(new Vector3(-1161.365f, -1427.646f, 4.623186f), 31.50553f, LocationType.Dispensary, "Doctor Kush", "Doctor kush") {Menu = WeedMenu },
            new GameLocation(new Vector3(-502.4879f, 32.92564f, 44.71512f), 179.9803f, LocationType.Dispensary, "Serenity Wellness", "Serenity Wellness") {Menu = WeedMenu },
            new GameLocation(new Vector3(169.5722f, -222.869f, 54.23643f), 342.0811f, LocationType.Dispensary, "High Time", "") {Menu = WeedMenu },
            new GameLocation(new Vector3(-1381.142f, -941.0327f, 10.17387f), 126.4558f, LocationType.Headshop, "Seagrass Herbals", "Seagrass Herbals") {Menu = WeedMenu },

            //Convenience
            new GameLocation(new Vector3(547f, 2678f, 41f), 22.23846f, LocationType.ConvenienceStore, "24/7","As fast as you"){ Menu = TwentyFourSevenMenu, OpenTime = 0, CloseTime = 24,BannerImage = "247.png"},
            new GameLocation(new Vector3(-3236.767f,1005.609f,12.33137f), 122.6316f, LocationType.ConvenienceStore, "24/7","As fast as you"){ Menu = TwentyFourSevenMenu, OpenTime = 0, CloseTime = 24,BannerImage = "247.png" },
            new GameLocation(new Vector3(-578.0112f, -1012.898f, 22.32503f), 359.4114f, LocationType.ConvenienceStore, "24/7", "As fast as you"){ Menu = TwentyFourSevenMenu, OpenTime = 0, CloseTime = 24,BannerImage = "247.png" },
            new GameLocation(new Vector3(-696.9965f, -858.7673f, 23.69209f), 85.51252f, LocationType.ConvenienceStore, "24/7", "24/7"){ Menu = TwentyFourSevenMenu, OpenTime = 0, CloseTime = 24,BannerImage = "247.png" },
            new GameLocation(new Vector3(152.5101f, 237.4131f, 106.9718f), 165.2823f, LocationType.ConvenienceStore, "24/7", "As fast as you"){ Menu = TwentyFourSevenMenu, OpenTime = 0, CloseTime = 24,BannerImage = "247.png" },
            new GameLocation(new Vector3(201.8985f, -26.30606f, 69.90953f), 249.8224f, LocationType.ConvenienceStore, "24/7", "As fast as you"){ Menu = TwentyFourSevenMenu, OpenTime = 0, CloseTime = 24,BannerImage = "247.png" },
            new GameLocation(new Vector3(528.017f, -152.1372f, 57.20173f), 44.64286f, LocationType.ConvenienceStore, "24/7", "As fast as you"){ Menu = TwentyFourSevenMenu, OpenTime = 0, CloseTime = 24,BannerImage = "247.png" },


            new GameLocation(new Vector3(2560f, 385f, 107f), 22.23846f,new Vector3(2555.339f, 380.9034f, 108.6229f), 347.3629f, LocationType.ConvenienceStore, "24/7","As fast as you"){ Menu = TwentyFourSevenMenu, OpenTime = 0, CloseTime = 24,BannerImage = "247.png" },
            new GameLocation(new Vector3(24.39647f, -1345.484f, 29.49702f), 252.9084f,new Vector3(24.39647f, -1345.484f, 29.49702f), 252.9084f, LocationType.ConvenienceStore, "24/7", "As fast as you"){ Menu = TwentyFourSevenMenu, OpenTime = 0, CloseTime = 24,BannerImage = "247.png" },
            new GameLocation(new Vector3(-3039.787f, 584.1979f, 7.908929f), 12.80189f,new Vector3(-3039.787f, 584.1979f, 7.908929f), 12.80189f, LocationType.ConvenienceStore, "24/7", "As fast as you"){ Menu = TwentyFourSevenMenu, OpenTime = 0, CloseTime = 24,BannerImage = "247.png" },
            new GameLocation(new Vector3(372.6485f, 327.0293f, 103.5664f), 257.6475f,new Vector3(372.6485f, 327.0293f, 103.5664f), 257.6475f, LocationType.ConvenienceStore, "24/7", "As fast as you"){ Menu = TwentyFourSevenMenu, OpenTime = 0, CloseTime = 24,BannerImage = "247.png" },

            new GameLocation(new Vector3(-45.89098f, -1757.345f, 29.42101f), 52.66933f,new Vector3(-45.89098f, -1757.345f, 29.42101f), 52.66933f, LocationType.ConvenienceStore, "LtD", "A one-stop shop!"){ Menu = LTDMenu,OpenTime = 0, CloseTime = 24 },
            



            new GameLocation(new Vector3(-1264.064f, -1162.599f, 6.764161f), 161.218f, LocationType.ConvenienceStore, "Fruit Of The Vine", "Fruit Of The Vine"){ Menu = FruitVineMenu },
            new GameLocation(new Vector3(-1270.649f, -304.9037f, 37.06938f), 257.2106f, LocationType.ConvenienceStore, "Fruit Of The Vine", "Fruit Of The Vine") { Menu = FruitVineMenu },
            new GameLocation(new Vector3(164.9962f, 351.1263f, 109.6859f), 4.847032f, LocationType.ConvenienceStore, "Fruit Of The Vine", "Fruit Of The Vine") { Menu = FruitVineMenu },
            new GameLocation(new Vector3(-144.3732f, -65.01408f, 54.60635f), 159.0404f, LocationType.ConvenienceStore, "Fruit Of The Vine", "") { Menu = FruitVineMenu },
            new GameLocation(new Vector3(-1412.015f, -320.1292f, 44.37897f), 92.48502f, LocationType.ConvenienceStore, "The Grain Of Truth", "The Grain Of Truth") { Menu = GrainOfTruthMenu },
            new GameLocation(new Vector3(-1370.819f, -684.5463f, 25.01069f), 214.6929f, LocationType.ConvenienceStore, "The Grain Of Truth", "The Grain Of Truth") { Menu = GrainOfTruthMenu },

            new GameLocation(new Vector3(1707.748f, 4792.387f, 41.98377f), 90.42564f, LocationType.ConvenienceStore, "Supermarket", "Supermarket"){ Menu = ConvenienceStoreMenu },
            new GameLocation(new Vector3(-1539.045f, -900.472f, 10.16951f), 129.0318f, LocationType.ConvenienceStore, "Del Perro Food Market","No Robberies Please!"){ Menu = ConvenienceStoreMenu },
            new GameLocation(new Vector3(-1359.607f, -963.3494f, 9.699487f), 124.3222f, LocationType.ConvenienceStore, "A&R Market", "A&R Market"){ Menu = ConvenienceStoreMenu },
            new GameLocation(new Vector3(53.28459f, -1478.863f, 29.28546f), 187.4217f, LocationType.ConvenienceStore, "Gabriela's Market", ""){ Menu = ConvenienceStoreMenu },
            new GameLocation(new Vector3(-551.5444f, -855.8014f, 28.28332f), 2.39254f, LocationType.ConvenienceStore, "Save-A-Cent", "Save-A-Cent"){ Menu = ConvenienceStoreMenu, OpenTime = 0, CloseTime = 24 },
            new GameLocation(new Vector3(-1312.64f, -1181.899f, 4.890057f), 271.5434f, LocationType.ConvenienceStore, "Beach Buddie", "Beach Buddie"){ Menu = ConvenienceStoreMenu },
            new GameLocation(new Vector3(-661.5522f, -915.5651f, 24.61216f), 260.1033f, LocationType.ConvenienceStore, "Convenience Store", "Convenience Store"){ Menu = ConvenienceStoreMenu },
            new GameLocation(new Vector3(393.8511f, -804.2294f, 29.29397f), 272.0436f, LocationType.ConvenienceStore, "Food Market", ""){ Menu = ConvenienceStoreMenu },
            new GameLocation(new Vector3(410.4163f, -1910.432f, 25.45381f), 88.30214f, LocationType.ConvenienceStore, "Long Pig Mini Mart", ""){ Menu = ConvenienceStoreMenu },
            new GameLocation(new Vector3(-170.9145f, -1449.77f, 31.64507f), 50.18372f, LocationType.ConvenienceStore, "Cert-L Market", ""){ Menu = ConvenienceStoreMenu },
            new GameLocation(new Vector3(-3152.666f, 1110.093f, 20.87176f), 245.5282f, LocationType.ConvenienceStore, "Nelsons", ""){ Menu = ConvenienceStoreMenu },
            //Gas
            new GameLocation(new Vector3(166.2001f, -1553.691f, 29.26175f), 218.9514f, LocationType.GasStation, "Ron", "") { Menu = RonMenu, OpenTime = 4, CloseTime = 22, CameraPosition = new Vector3(175.2995f, -1593.878f, 39.27175f), CameraDirection = new Vector3(-0.1031758f, 0.9726905f, -0.2079136f), CameraRotation = new Rotator(-12.00011f, 0f, 6.054868f) },
            new GameLocation(new Vector3(2676.595f, 3280.101f, 55.24113f), 325.0921f,new Vector3(2676.595f, 3280.101f, 55.24113f), 325.0921f, LocationType.GasStation, "24/7", "As fast as you") { Menu = TwentyFourSevenMenu,OpenTime = 0, CloseTime = 24 ,BannerImage = "247.png"},
            new GameLocation(new Vector3(-705.7453f, -913.6598f, 19.21559f), 83.75771f,new Vector3(-705.7453f, -913.6598f, 19.21559f), 83.75771f, LocationType.GasStation, "LtD", "A one-stop shop!"){ Menu = LTDMenu,OpenTime = 0, CloseTime = 24 },
            new GameLocation(new Vector3(1698.044f, 4922.526f, 42.06367f), 314.3236f,new Vector3(1698.044f, 4922.526f, 42.06367f), 314.3236f, LocationType.GasStation, "LtD", "A one-stop shop!"){ Menu = LTDMenu,OpenTime = 0, CloseTime = 24 },

            new GameLocation(new Vector3(-1427.998f, -268.4702f, 46.2217f), 132.4002f, LocationType.GasStation, "Ron", "Ron"){ Menu = RonMenu, OpenTime = 4, CloseTime = 22 },
            new GameLocation(new Vector3(2559.112f, 373.5359f, 108.6211f), 265.8011f, LocationType.GasStation, "Ron", "") { Menu = RonMenu, OpenTime = 4, CloseTime = 22 },
            new GameLocation(new Vector3(-1429.33f,-270.8909f,46.2077f), 325.7301f, LocationType.GasStation, "Ron","") { Menu = RonMenu, OpenTime = 4, CloseTime = 22 },
            new GameLocation(new Vector3(-2544.116f, 2315.928f, 33.21614f), 3.216755f, LocationType.GasStation, "Ron", ""){ Menu = RonMenu, OpenTime = 4, CloseTime = 22 },
            new GameLocation(new Vector3(1725f, 6410f, 35f), 22.23846f, LocationType.GasStation, "24/7","As fast as you") { Menu = TwentyFourSevenMenu, OpenTime = 0, CloseTime = 24,BannerImage = "247.png" },
            
            new GameLocation(new Vector3(-1817.871f,787.0063f,137.917f), 89.38248f, LocationType.GasStation, "LtD","A one-stop shop!"){ Menu = LTDMenu,OpenTime = 0, CloseTime = 24 },     
            new GameLocation(new Vector3(-531.5529f, -1220.763f, 18.455f), 347.6858f, LocationType.GasStation, "Xero", ""){ Menu = XeroMenu,OpenTime = 0, CloseTime = 24,BannerImage = "xero.png" },
            new GameLocation(new Vector3(289.5112f, -1266.584f, 29.44076f), 92.24692f, LocationType.GasStation, "Xero", ""){ Menu = XeroMenu,OpenTime = 0, CloseTime = 24,BannerImage = "xero.png" },
            new GameLocation(new Vector3(-92.79028f, 6409.667f, 31.64035f), 48.08112f, LocationType.GasStation, "Xero", ""){ Menu = XeroMenu,OpenTime = 0, CloseTime = 24,BannerImage = "xero.png" },
            new GameLocation(new Vector3(46.75933f, 2789.635f, 58.10043f), 139.5097f, LocationType.GasStation, "Xero", ""){ Menu = XeroMenu,OpenTime = 0, CloseTime = 24,BannerImage = "xero.png" },
            new GameLocation(new Vector3(160.4977f,6635.249f,31.61175f), 70.88637f, LocationType.GasStation, "Dons Country Store & Gas","Country Manners!") { Menu = GasStationMenu },
            new GameLocation(new Vector3(266.2746f,2599.669f,44.7383f), 231.9223f, LocationType.GasStation, "Harmony General Store & Gas","Always in Harmony!") { Menu = GasStationMenu },
            new GameLocation(new Vector3(1039.753f,2666.26f,39.55253f), 143.6208f, LocationType.GasStation, "Grande Senora Cafe & Gas","Extra Grande!") { Menu = GasStationMenu },
            new GameLocation(new Vector3(2001.239f, 3779.786f, 32.18078f), 208.5214f, LocationType.GasStation, "Sandy's Gas", "And Full Service!"){ Menu = GasStationMenu, OpenTime = 4, CloseTime = 22 },
            new GameLocation(new Vector3(646.0997f, 267.417f, 103.2494f), 58.99448f, LocationType.GasStation, "Globe Oil", "Globe Oil") { Menu = GasStationMenu },
            new GameLocation(new Vector3(1776.308f, 3327.483f, 41.43329f), 328.0875f, LocationType.GasStation, "Flywheels Gas", "Flywheels Gas") { Menu = GasStationMenu },   

            //Food Stand
            new GameLocation(new Vector3(403.3527f, 106.0655f, 101.4575f), 241.199f,new Vector3(403.3527f, 106.0655f, 101.4575f), 241.199f, LocationType.FoodStand, "Beefy Bills Burger Bar", "Extra BEEFY!"){ Menu = BeefyBillsMenu,BannerImage = "beefybills.png" },
            new GameLocation(new Vector3(245.8918f, 161.5893f, 104.9487f), 3.803493f,new Vector3(245.8918f, 161.5893f, 104.9487f), 3.803493f, LocationType.FoodStand, "Beefy Bills Burger Bar", "Extra BEEFY!"){ Menu = BeefyBillsMenu,BannerImage = "beefybills.png" }, 
            new GameLocation(new Vector3(-1268.011f, -1432.715f, 4.353373f), 134.2259f,new Vector3(-1268.011f, -1432.715f, 4.353373f), 134.2259f, LocationType.FoodStand, "Beefy Bills Burger Bar","Extra BEEFY!"){ Menu = BeefyBillsMenu,BannerImage = "beefybills.png" }, 
            new GameLocation(new Vector3(-1232.426f, -1485.006f, 4.362638f), 137.5475f,new Vector3(-1232.426f, -1485.006f, 4.362638f), 137.5475f, LocationType.FoodStand, "Beefy Bills Burger Bar", "Extra BEEFY!"){ Menu = BeefyBillsMenu,BannerImage = "beefybills.png" },
            new GameLocation(new Vector3(821.2138f, -2977.05f, 6.02066f), 272.7679f,new Vector3(821.2138f, -2977.05f, 6.02066f), 272.7679f, LocationType.FoodStand, "Beefy Bills Burger Bar", "Extra BEEFY!"){ Menu = BeefyBillsMenu,BannerImage = "beefybills.png" },
            
            new GameLocation(new Vector3(240.8329f, 167.2296f, 105.0605f), 167.5996f,new Vector3(240.8329f, 167.2296f, 105.0605f), 167.5996f, LocationType.FoodStand, "Chihuahua Hot Dogs", "Vegan? No. Meat? Yes."){ Menu = ChihuahuaHotDogMenu,BannerImage = "chihuahuahotdogs.png" },
            new GameLocation(new Vector3(-1516.382f, -952.5892f, 9.278718f), 317.7292f,new Vector3(-1516.382f, -952.5892f, 9.278718f), 317.7292f, LocationType.FoodStand, "Chihuahua Hot Dogs", "Vegan? No. Meat? Yes.") { Menu = ChihuahuaHotDogMenu,BannerImage = "chihuahuahotdogs.png" },
            new GameLocation(new Vector3(1604.818f, 3822.332f, 34.69806f), 200.7076f,new Vector3(1607.818f, 3822.332f, 34.69806f), 200.7076f, LocationType.FoodStand, "Chihuahua Hot Dog", "Vegan? No. Meat? Yes."){ Menu = ChihuahuaHotDogMenu,BannerImage = "chihuahuahotdogs.png" },
            new GameLocation(new Vector3(-1248.932f, -1474.449f, 4.277946f), 306.3787f,new Vector3(-1248.932f, -1474.449f, 4.277946f), 306.3787f, LocationType.FoodStand, "Chihuahua Hot Dogs", "Vegan? No. Meat? Yes."){ Menu = ChihuahuaHotDogMenu,BannerImage = "chihuahuahotdogs.png" },
            new GameLocation(new Vector3(821.8197f, -2973.398f, 6.020657f), 276.5136f,new Vector3(821.8197f, -2973.398f, 6.020657f), 276.5136f, LocationType.FoodStand, "Chihuahua Hot Dogs", "Vegan? No. Meat? Yes."){ Menu = ChihuahuaHotDogMenu,BannerImage = "chihuahuahotdogs.png" },
            new GameLocation(new Vector3(-1219.656f, -1504.36f, 4.36032f), 98.7149f,new Vector3(-1219.656f, -1504.36f, 4.36032f), 98.7149f, LocationType.FoodStand, "Chihuahua Hot Dogs", "Vegan? No. Meat? Yes."){ Menu = ChihuahuaHotDogMenu,BannerImage = "chihuahuahotdogs.png" },

            new GameLocation(new Vector3(2106.954f, 4947.911f, 40.95187f), 319.9109f,new Vector3(2106.954f, 4947.911f, 40.95187f), 319.9109f, LocationType.FoodStand, "Attack-A-Taco", "Heavy Shelling!") { Menu = TacoFarmerMenu },
            new GameLocation(new Vector3(-1148.969f, -1601.963f, 4.390241f), 35.73399f,new Vector3(-1145.969f, -1602.963f, 4.390241f), 35.73399f, LocationType.FoodStand, "Gyro Day", "Gyro Day") { Menu = GenericMenu },
            new GameLocation(new Vector3(1604.578f, 3828.483f, 34.4987f), 142.3778f,new Vector3(1604.578f, 3828.483f, 34.4987f), 142.3778f, LocationType.FoodStand, "Tough Nut Donut", "Our DoNuts are Crazy!"){ Menu = DonutMenu },
            new GameLocation(new Vector3(1087.509f, 6510.788f, 21.0551f), 185.487f,new Vector3(1087.509f, 6510.788f, 21.0551f), 185.487f, LocationType.FoodStand, "Roadside Fruit", "Should Be OK To Eat") { Menu = FruitMenu },
            new GameLocation(new Vector3(2526.548f, 2037.936f, 19.82413f), 263.8982f,new Vector3(2526.548f, 2037.936f, 19.82413f), 263.8982f, LocationType.FoodStand, "Roadside Fruit", "Should Be OK To Eat") { Menu = FruitMenu },
            new GameLocation(new Vector3(1263.013f, 3548.566f, 35.14751f), 187.8834f,new Vector3(1263.013f, 3548.566f, 35.14751f), 187.8834f, LocationType.FoodStand, "Roadside Fruit", "Should Be OK To Eat") { Menu = FruitMenu },
            new GameLocation(new Vector3(1675.873f, 4883.532f, 42.06379f), 57.34329f,new Vector3(1675.873f, 4883.532f, 42.06379f), 57.34329f, LocationType.FoodStand, "Grapeseed Fruit", "Grapeseed Fruit") { Menu = FruitMenu },


            new GameLocation(new Vector3(-27.02787f, -1578.65f, 29.29078f), 183.333f,new Vector3(-27.02787f, -1578.65f, 29.29078f), 183.333f, LocationType.DrugDealer, "", ""){ Menu = WeedDealerMenu, VendorModels = new List<string>() { "g_m_y_famdnf_01","g_m_y_famca_01","g_m_y_famfor_01" } },




            //Bar
            new GameLocation(new Vector3(224.5178f, 336.3819f, 105.5973f), 340.0694f, LocationType.Bar, "Pitchers", "Pitchers") { Menu = BarMenu },
            new GameLocation(new Vector3(219.5508f, 304.9488f, 105.5861f), 250.1051f, LocationType.Bar, "Singletons", "Singletons") { Menu = BarMenu },
            new GameLocation(new Vector3(1982.979f, 3053.596f, 47.21508f), 226.3188f,new Vector3(1982.979f, 3053.596f, 47.21508f), 226.3188f, LocationType.Bar, "Yellow Jacket Inn", "Yellow Jacket Inn") { Menu = BarMenu },
            new GameLocation(new Vector3(-262.8396f, 6291.08f, 31.49327f), 222.9271f, LocationType.Bar, "The Bay Bar", ""){ Menu = BarMenu },

            //Restaurant

                //Fancy
                new GameLocation(new Vector3(-1487.163f, -308.0127f, 47.02639f), 231.5184f, LocationType.Restaurant, "Las Cuadras Restaurant", "Las Cuadras Restaurant")  {Menu = FancyDeliMenu },
                new GameLocation(new Vector3(-1473.121f, -329.6028f, 44.81668f), 319.3725f, LocationType.Restaurant, "Las Cuadras Deli", "Las Cuadras Deli")  {Menu = FancyDeliMenu },
                new GameLocation(new Vector3(-1221.254f, -1095.873f, 8.115647f), 111.3174f, LocationType.Restaurant, "Prawn Vivant", "Prawn Vivant") {Menu = FancyFishMenu },
                new GameLocation(new Vector3(-1256.581f, -1079.491f, 8.398257f), 339.5968f, LocationType.Restaurant, "Surfries Diner", "Surfries Diner") {Menu = FancyDeliMenu },
                new GameLocation(new Vector3(-1247.504f, -1105.777f, 8.109305f), 289.9685f, LocationType.Restaurant, "The Split Kipper Fish", "The Split Kipper Fish") {Menu = FancyFishMenu },
                new GameLocation(new Vector3(-1230.032f, -1174.862f, 7.700727f), 330.9398f, LocationType.Restaurant, "Pot Heads Seafood", "Pot Heads Seafood") {Menu = FancyFishMenu },
                new GameLocation(new Vector3(-1111.103f, -1454.387f, 5.582287f), 304.9954f, LocationType.Restaurant, "Coconut Cafe", "Coconut Cafe") {Menu = FancyDeliMenu },
                new GameLocation(new Vector3(-1037.587f, -1397.168f, 5.553192f), 76.84702f, LocationType.Restaurant, "La Spada", "La Spada") {Menu = FancyFishMenu },
                new GameLocation(new Vector3(-1129.253f, -1373.276f, 5.056143f), 164.9213f, LocationType.Restaurant, "Marlin's Cafe", "Marlin's Cafe") {Menu = FancyDeliMenu },
                new GameLocation(new Vector3(2561.869f, 2590.851f, 38.08311f), 294.6638f, LocationType.Restaurant, "Rex's Diner", "Rex's Diner") {Menu = FancyDeliMenu },
                new GameLocation(new Vector3(2697.521f, 4324.264f, 45.98642f), 41.67124f, LocationType.Restaurant, "Park View Diner", "Park View Diner") {Menu = FancyGenericMenu },
                new GameLocation(new Vector3(-1389.63f, -744.4225f, 24.62544f), 127.01f, LocationType.Restaurant, "Haute", "Haute") {Menu = FancyDeliMenu },
                new GameLocation(new Vector3(-1392.125f, -732.2938f, 24.64698f), 37.13289f, LocationType.Restaurant, "Les Bianco's", "Les Bianco's") {Menu = FancyGenericMenu },
                new GameLocation(new Vector3(-1420.425f, -709.2584f, 24.60311f), 126.5269f, LocationType.Restaurant, "Pescado Rojo", "Pescado Rojo") {Menu = FancyFishMenu },
                new GameLocation(new Vector3(-1335.517f, -660.6063f, 26.51026f), 212.5534f, LocationType.Restaurant, "The Fish Net", "The Fish Net") {Menu = FancyFishMenu },
                new GameLocation(new Vector3(-238.903f, -777.356f, 34.09171f), 71.47642f, LocationType.Restaurant, "Cafe Redemption", "") {Menu = FancyDeliMenu },
                new GameLocation(new Vector3(370.4181f, -1027.565f, 29.33361f), 184.4234f, LocationType.Restaurant, "Ground & Pound Cafe", "") {Menu = FancyGenericMenu },
                new GameLocation(new Vector3(502.6628f, 113.1527f, 96.62571f), 164.3104f, LocationType.Restaurant, "Jazz Desserts", ""){Menu = FancyGenericMenu },
                new GameLocation(new Vector3(16.29523f, -166.5288f, 55.82795f), 341.2336f, LocationType.Restaurant, "The Fish Net", ""){Menu = FancyFishMenu },
                new GameLocation(new Vector3(281.4706f, -800.5342f, 29.31682f), 227.4364f, LocationType.Restaurant, "Pescado Azul", ""){Menu = FancyFishMenu },
                new GameLocation(new Vector3(-630.0808f, -2266.577f, 5.933444f), 242.2394f, LocationType.Restaurant, "Poppy House", ""){Menu = FancyGenericMenu },
                new GameLocation(new Vector3(-122.6395f, 6389.315f, 32.17757f), 44.91608f, LocationType.Restaurant, "Mojito Inn", ""){Menu = FancyGenericMenu },

                //Sandwiches
                new GameLocation(new Vector3(-1249.812f, -296.1564f, 37.35062f), 206.9039f, LocationType.Restaurant, "Bite!", "Have It Our Way") {Menu = BiteMenu,OpenTime = 0, CloseTime = 24,BannerImage = "bite.png" },
                new GameLocation(new Vector3(-1539.498f, -427.3804f, 35.59194f), 233.1319f, LocationType.Restaurant, "Bite!", "Have It Our Way") {Menu = BiteMenu,OpenTime = 0, CloseTime = 24,BannerImage = "bite.png" },
                new GameLocation(new Vector3(229.5384f, -22.3363f, 74.98735f), 160.0777f, LocationType.Restaurant, "Bite!", "Have It Our Way") {Menu = BiteMenu,OpenTime = 0, CloseTime = 24,BannerImage = "bite.png" },
                new GameLocation(new Vector3(-240.7315f, -346.1899f, 30.02782f), 47.8591f, LocationType.Restaurant, "Bite!", "Have It Our Way") {Menu = BiteMenu,OpenTime = 0, CloseTime = 24,BannerImage = "bite.png" },
                new GameLocation(new Vector3(-263.1924f, -904.2821f, 32.3108f), 338.4021f, LocationType.Restaurant, "Bite!", "Have It Our Way") {Menu = BiteMenu,OpenTime = 0, CloseTime = 24,BannerImage = "bite.png" },
                new GameLocation(new Vector3(385.958f, -1010.523f, 29.41794f), 271.5127f, LocationType.Restaurant, "Bite!", "Have It Our Way") {Menu = BiteMenu,OpenTime = 0, CloseTime = 24,BannerImage = "bite.png" },
                new GameLocation(new Vector3(100.5837f, 209.4958f, 107.9911f), 342.4262f, LocationType.Restaurant, "The Pink Sandwich", "The Pink Sandwich") {Menu = SandwichMenu,OpenTime = 0, CloseTime = 24 },

                //Asian
                new GameLocation(new Vector3(-798.0056f, -632.0029f, 29.02696f), 169.2606f, LocationType.Restaurant, "S.Ho", "S.Ho Noodles") {Menu = NoodleMenu,BannerImage ="sho.png" },
                new GameLocation(new Vector3(-638.5052f, -1249.646f, 11.81044f), 176.4081f, LocationType.Restaurant, "S.Ho", "S.Ho Noodles") {Menu = NoodleMenu,BannerImage ="sho.png" },
                new GameLocation(new Vector3(-700.9553f, -884.5563f, 23.79126f), 41.62328f, LocationType.Restaurant, "S.Ho", "S.Ho Noodles") {Menu = NoodleMenu,BannerImage ="sho.png" },
                new GameLocation(new Vector3(-1229.61f, -285.7077f, 37.73843f), 205.5755f, LocationType.Restaurant, "Noodle Exchange", "You Won't Want To Share!") {Menu = NoodleMenu },
                new GameLocation(new Vector3(-1199.53f, -1162.439f, 7.696731f), 107.0593f, LocationType.Restaurant, "Noodle Exchange", "You Won't Want To Share!") {Menu = NoodleMenu },
                new GameLocation(new Vector3(-655.6034f, -880.3672f, 24.67554f), 265.7094f, LocationType.Restaurant, "Wook Noodle House", "Wook Noodle House") {Menu = NoodleMenu },
                new GameLocation(new Vector3(-680.4404f, -945.5441f, 20.93157f), 180.6927f, LocationType.Restaurant, "Wook Noodle House", "Wook Noodle House") {Menu = NoodleMenu },
                new GameLocation(new Vector3(-654.8373f, -885.7593f, 24.67703f), 273.4168f, LocationType.Restaurant, "Park Jung Restaurant", "Park Jung Restaurant") {Menu = GenericMenu },
                new GameLocation(new Vector3(-661.5396f, -907.5895f, 24.60632f), 278.5222f, LocationType.Restaurant, "Hwan Cafe", "Hwan Cafe") {Menu = GenericMenu },
                new GameLocation(new Vector3(-163.0659f, -1440.267f, 31.42698f), 55.5593f, LocationType.Restaurant, "Wok It Off", "") {Menu = GenericMenu },

                //Italian
                new GameLocation(new Vector3(-1182.659f, -1410.577f, 4.499721f), 215.9843f, LocationType.Restaurant, "Al Dentes", "Al Dentes") {Menu = AlDentesMenu },
                new GameLocation(new Vector3(-213.0357f, -40.15178f, 50.04371f), 157.8173f, LocationType.Restaurant, "Al Dentes", "Al Dentes") {Menu = AlDentesMenu },
                new GameLocation(new Vector3(-1393.635f, -919.5128f, 11.24511f), 89.35195f, LocationType.Restaurant, "Al Dentes", "Al Dentes"){ Menu = AlDentesMenu },
                new GameLocation(new Vector3(215.2669f, -17.14256f, 74.98737f), 159.7144f, LocationType.Restaurant, "Pizza This...", "Pizza This...") {Menu = PizzaMenu },
                new GameLocation(new Vector3(538.3118f, 101.4798f, 96.52515f), 159.4801f, LocationType.Restaurant, "Pizza This...", "Pizza This...") {Menu = PizzaMenu },
                new GameLocation(new Vector3(443.7377f, 135.1464f, 100.0275f), 161.2897f, LocationType.Restaurant, "Guidos Takeout 24/7", "Guidos Takeout 24/7") {Menu = PizzaMenu },
                new GameLocation(new Vector3(-1320.907f, -1318.505f, 4.784881f), 106.5257f, LocationType.Restaurant, "Pebble Dash Pizza", "Pebble Dash Pizza"){ Menu = PizzaMenu },
                new GameLocation(new Vector3(-1334.007f, -1282.623f, 4.835985f), 115.3464f, LocationType.Restaurant, "Slice N Dice Pizza","Slice UP!"){ Menu = PizzaMenu},
                new GameLocation(new Vector3(-1296.815f, -1387.3f, 4.544102f), 112.4694f, LocationType.Restaurant, "Sharkies Bites","Take A Bite Today!"){ Menu = PizzaMenu },
                new GameLocation(new Vector3(-1342.607f, -872.2929f, 16.87064f), 312.7196f, LocationType.Restaurant, "Giovanni's Italian", "Giovanni's Italian"){ Menu = PizzaMenu },

                //Burger
                new GameLocation(new Vector3(-1535.117f, -454.0615f, 35.92439f), 319.1095f, LocationType.Restaurant, "Wigwam", "No need for reservations") { Menu = WigwamMenu ,BannerImage = "wigwam.png"},
                new GameLocation(new Vector3(-860.8414f, -1140.393f, 7.39234f), 171.7175f, LocationType.Restaurant, "Wigwam", "No need for reservations") { Menu = WigwamMenu,BannerImage = "wigwam.png" },
                new GameLocation(new Vector3(-1540.86f, -454.866f, 40.51906f), 321.1314f, LocationType.Restaurant, "Up-N-Atom", "Never Frozen, Often Microwaved") {Menu = UpNAtomMenu,OpenTime = 0, CloseTime = 24,BannerImage = "upnatom.png"},
                new GameLocation(new Vector3(81.31124f, 275.1125f, 110.2102f), 162.7602f, LocationType.Restaurant, "Up-N-Atom", "Never Frozen, Often Microwaved") {Menu = UpNAtomMenu,OpenTime = 0, CloseTime = 24,BannerImage = "upnatom.png"},
                new GameLocation(new Vector3(1591.054f, 6451.071f, 25.31714f), 158.0088f, LocationType.Restaurant, "Up-N-Atom Diner", "Never Frozen, Often Microwaved") {Menu = UpNAtomMenu,OpenTime = 0, CloseTime = 24,BannerImage = "upnatom.png"},
                new GameLocation(new Vector3(-1183.638f, -884.3126f, 13.79987f), 303.1936f, LocationType.Restaurant, "Burger Shot", "Burger Shot") {Menu = BurgerShotMenu },


                new GameLocation(new Vector3(-512.6821f, -683.3517f, 33.18555f), 3.720508f, LocationType.Restaurant, "Snr. Buns", "Snr. Buns") {Menu = GenericMenu },
                new GameLocation(new Vector3(-526.9481f, -679.6907f, 33.67113f), 35.17997f, LocationType.Restaurant, "Snr. Muffin", "Snr. Muffin") {Menu = GenericMenu },//???
                
                new GameLocation(new Vector3(125.9558f, -1537.896f, 29.1772f), 142.693f, LocationType.Restaurant, "La Vaca Loca", "") {Menu = BeefyBillsMenu, CameraPosition = new Vector3(137.813f, -1561.211f, 37.43506f), CameraDirection = new Vector3(-0.1290266f, 0.9696004f, -0.2079113f), CameraRotation = new Rotator(-11.99998f, -2.182118E-07f, 7.579925f) },

                //Bagels&Donuts
                new GameLocation(new Vector3(-1318.507f, -282.2458f, 39.98732f), 115.4663f, LocationType.Restaurant, "Dickies Bagels", "Holy Dick!") { Menu = CoffeeMenu },
                new GameLocation(new Vector3(-1204.364f, -1146.246f, 7.699615f), 109.2444f, LocationType.Restaurant, "Dickies Bagels", "Holy Dick!") { Menu = CoffeeMenu },
                new GameLocation(new Vector3(354.0957f, -1028.134f, 29.33102f), 182.3497f, LocationType.Restaurant, "Rusty Browns", "") { Menu = CoffeeMenu,BannerImage = "rustybrowns.png" },

                //Coffee
                    //Bean Machine
                    new GameLocation(new Vector3(-1283.567f, -1130.118f, 6.795891f), 143.1178f, LocationType.Restaurant, "The Bean Machine Coffee", "The Bean Machine Coffee") { Menu = CoffeeMenu,BannerImage = "beanmachine.png" },
                    new GameLocation(new Vector3(-1549.39f, -435.5105f, 35.88667f), 234.6563f, LocationType.Restaurant, "The Bean Machine Coffee", "The Bean Machine Coffee") { Menu = CoffeeMenu,BannerImage = "beanmachine.png" },
                    new GameLocation(new Vector3(-835.4522f, -610.4766f, 29.02697f), 142.0655f, LocationType.Restaurant, "The Bean Machine Coffee", "The Bean Machine Coffee") { Menu = CoffeeMenu,BannerImage = "beanmachine.png" },
                    new GameLocation(new Vector3(-602.2112f, -1105.766f, 22.32427f), 273.8795f, LocationType.Restaurant, "The Bean Machine Coffee", "The Bean Machine Coffee") { Menu = CoffeeMenu,BannerImage = "beanmachine.png" },
                    new GameLocation(new Vector3(-659.5289f, -814.0433f, 24.53778f), 232.0023f, LocationType.Restaurant, "The Bean Machine Coffee", "The Bean Machine Coffee") { Menu = CoffeeMenu ,BannerImage = "beanmachine.png"},
                    new GameLocation(new Vector3(-687.0801f, -855.6792f, 23.89398f), 0.2374549f, LocationType.Restaurant, "The Bean Machine Coffee", "The Bean Machine Coffee") { Menu = CoffeeMenu,BannerImage = "beanmachine.png" },
                    new GameLocation(new Vector3(-1345.296f, -609.976f, 28.61888f), 304.4266f, LocationType.Restaurant, "The Bean Machine Coffee", "The Bean Machine Coffee") { Menu = CoffeeMenu,BannerImage = "beanmachine.png" },
                    new GameLocation(new Vector3(-270.3488f, -977.3488f, 31.21763f), 164.5747f, LocationType.Restaurant, "The Bean Machine", "The Bean Machine Coffee") { Menu = CoffeeMenu,BannerImage = "beanmachine.png" },
                    new GameLocation(new Vector3(127.9072f, -1028.778f, 29.43674f), 336.4557f, LocationType.Restaurant, "The Bean Machine", "") { Menu = CoffeeMenu,BannerImage = "beanmachine.png" },
           
                new GameLocation(new Vector3(-1206.975f, -1135.029f, 7.693257f), 109.1408f, LocationType.Restaurant, "Cool Beans Coffee Co", "Cool Beans Coffee Co")  { Menu = CoffeeMenu },
                new GameLocation(new Vector3(-1278.833f, -876.438f, 11.9303f), 123.2498f, LocationType.Restaurant, "Cool Beans Coffee Co", "Cool Beans Coffee Co") { Menu = CoffeeMenu },  
                new GameLocation(new Vector3(-1108.847f, -1355.264f, 5.035112f), 206.1676f, LocationType.Restaurant, "Crucial Fix Coffee", "Crucial Fix Coffee") { Menu = CoffeeMenu },
                new GameLocation(new Vector3(189.0311f, -231.234f, 54.07472f), 340.4597f, LocationType.Restaurant, "Crucial Fix Coffee", "") { Menu = CoffeeMenu },
                new GameLocation(new Vector3(273.174f, -833.0611f, 29.41237f), 185.6476f, LocationType.Restaurant, "Crucial Fix Coffee", "") { Menu = CoffeeMenu },
                new GameLocation(new Vector3(-576.6631f, -677.8674f, 32.36259f), 306.9058f, LocationType.Restaurant, "Hit-N-Run Coffee", "Hit-N-Run Coffee"){ Menu = CoffeeMenu },
                new GameLocation(new Vector3(-1253.337f, -296.6488f, 37.31522f), 206.5786f, LocationType.Restaurant, "{java.update();}", "Enjoy Hot Coffee") { Menu = CoffeeMenu },
                new GameLocation(new Vector3(-509.1889f, -22.9895f, 45.60899f), 354.7263f, LocationType.Restaurant, "Little Teapot", "Little Teapot") { Menu = CoffeeMenu },

                //Mexican
                new GameLocation(new Vector3(10.96682f, -1605.874f, 29.3931f), 229.8729f, LocationType.Restaurant, "The Taco Farmer", "Open All Hours!") {Menu = TacoFarmerMenu },
                new GameLocation(new Vector3(-1168.281f, -1267.279f, 6.198249f), 111.9682f, LocationType.Restaurant, "Taco Libre", "Taco Libre") {Menu = TacoFarmerMenu },
                new GameLocation(new Vector3(-657.5089f, -679.4656f, 31.46727f), 317.9819f, LocationType.Restaurant, "Taco Bomb", "My taco looks so tasty!") {Menu = TacoBombMenu,BannerImage = "tacobomb.png" },
                new GameLocation(new Vector3(-1196.981f, -791.5534f, 16.40427f), 134.7115f, LocationType.Restaurant, "Taco Bomb", "My taco looks so tasty!") {Menu = TacoBombMenu,BannerImage = "tacobomb.png" },
                new GameLocation(new Vector3(-1553.112f, -439.9938f, 40.51905f), 228.7506f, LocationType.Restaurant, "Taco Bomb", "My taco looks so tasty!") {Menu = TacoBombMenu,BannerImage = "tacobomb.png" },
                new GameLocation(new Vector3(99.21678f, -1419.307f, 29.42156f), 323.9604f, LocationType.Restaurant, "Aguila Burrito", "") {Menu = TacoFarmerMenu },

                //Ice Cream
                new GameLocation(new Vector3(-1193.966f, -1543.693f, 4.373522f), 124.3727f, LocationType.Restaurant, "The Sundae Post", "The Sundae Post") {Menu = DonutMenu },
                new GameLocation(new Vector3(-1171.529f, -1435.118f, 4.461945f), 32.60835f, LocationType.Restaurant, "Ice Maiden", "Ice Maiden") {Menu = GenericMenu },

                //Juice and Smoothies
                new GameLocation(new Vector3(-1137.926f, -1624.695f, 4.410712f), 127.6497f, LocationType.Restaurant, "Vitamin Seaside Juice Bar", "Vitamin Seaside Juice Bar"){ Menu = FruitMenu },
                new GameLocation(new Vector3(-1187.057f, -1536.73f, 4.379496f), 32.85152f, LocationType.Restaurant, "Muscle Peach Juice Bar", "Muscle Peach Juice Bar"){ Menu = FruitMenu },
                new GameLocation(new Vector3(-1236.263f, -287.9641f, 37.63038f), 205.8273f, LocationType.Restaurant, "Limey's Juice and Smoothies", "No Limes About It") {Menu = GenericMenu },
                new GameLocation(new Vector3(250.3842f, -1026.535f, 29.25663f), 124.0668f, LocationType.Restaurant, "Limey's Juice and Smoothies", "") {Menu = GenericMenu },
                new GameLocation(new Vector3(-1182.576f, -1248.037f, 6.991587f), 110.7639f, LocationType.Restaurant, "Limey's Juice Bar", "No Limes About It") {Menu = GenericMenu },
                new GameLocation(new Vector3(-471.7646f, -18.46761f, 45.75837f), 357.2652f, LocationType.Restaurant, "Fruit Machine", "Fruit Machine"){Menu = GenericMenu },
                new GameLocation(new Vector3(2741.563f, 4413.093f, 48.62326f), 201.5914f, LocationType.Restaurant, "Big Juice Stand", "Big Juice Stand"){Menu = GenericMenu },
                new GameLocation(new Vector3(1791.592f, 4594.844f, 37.68291f), 182.8134f, LocationType.Restaurant, "Alamo Fruit", "Alamo Fruit"){ Menu = FruitMenu },

                //Chicken
                new GameLocation(new Vector3(-584.761f, -872.753f, 25.91489f), 353.0746f, LocationType.Restaurant, "Lucky Plucker", "Lucky Plucker") {Menu = GenericMenu },
                new GameLocation(new Vector3(2580.543f, 464.6521f, 108.6232f), 176.5548f, LocationType.Restaurant, "Bishop's Chicken", "Bishop's Chicken") {Menu = GenericMenu },
                new GameLocation(new Vector3(169.3292f, -1634.163f, 29.29167f), 35.89598f, LocationType.Restaurant, "Bishop's Chicken", "") {Menu = GenericMenu },
                new GameLocation(new Vector3(133.0175f, -1462.702f, 29.35705f), 48.47223f, LocationType.Restaurant, "Lucky Plucker", "") {Menu = GenericMenu },
                new GameLocation(new Vector3(-138.4921f, -256.509f, 43.59497f), 290.1001f, LocationType.Restaurant, "Cluckin' Bell", "") {Menu = CluckinBellMenu },
                new GameLocation(new Vector3(-184.9376f, -1428.169f, 31.47968f), 33.8636f, LocationType.Restaurant, "Cluckin' Bell", "") {Menu = CluckinBellMenu },
                //General    
                new GameLocation(new Vector3(-1222.546f, -807.5845f, 16.59777f), 305.3918f, LocationType.Restaurant, "Lettuce Be", "Lettuce Be") {Menu = GenericMenu },
                new GameLocation(new Vector3(-1196.705f, -1167.969f, 7.695099f), 108.4535f, LocationType.Restaurant, "Lettuce Be", "Lettuce Be") {Menu = GenericMenu },
                new GameLocation(new Vector3(-1535.082f, -422.2449f, 35.59194f), 229.4618f, LocationType.Restaurant, "Chihuahua Hot Dogs", "Vegan? No. Meat? Yes."){ Menu = ChihuahuaHotDogMenu },
                new GameLocation(new Vector3(49.24896f, -1000.381f, 29.35741f), 335.6092f, LocationType.Restaurant, "Chihuahua Hot Dogs", "Vegan? No. Meat? Yes."){ Menu = ChihuahuaHotDogMenu },
                new GameLocation(new Vector3(-1271.224f, -1200.703f, 5.366248f), 70.19876f, LocationType.Restaurant, "The Nut Buster", "The Nut Buster") {Menu = GenericMenu },
                new GameLocation(new Vector3(166.2677f, -1450.995f, 29.24164f), 142.858f, LocationType.Restaurant, "Ring Of Fire Chili House", "") {Menu = GenericMenu },


            //Drive Thru
            new GameLocation(new Vector3(95.41846f, 285.0295f, 110.2042f), 251.8247f, LocationType.DriveThru, "Up-N-Atom Burger", "Never Frozen, Often Microwaved") {Menu = UpNAtomMenu,OpenTime = 0, CloseTime = 24, BannerImage = "upnatom.png"},
            new GameLocation(new Vector3(15.48935f, -1595.832f, 29.28254f), 319.2816f, LocationType.DriveThru, "The Taco Farmer", "Open All Hours!") {Menu = TacoFarmerMenu },
            new GameLocation(new Vector3(-576.9321f, -880.5195f, 25.70123f), 86.01214f, LocationType.DriveThru, "Lucky Plucker", "Lucky Plucker") {Menu = GenericMenu },
            new GameLocation(new Vector3(2591.213f, 478.8892f, 108.6423f), 270.9569f,LocationType.DriveThru, "Bishop's Chicken", "Bishop's Chicken") {Menu = GenericMenu },
            new GameLocation(new Vector3(144.34f, -1541.275f, 28.36799f), 139.819f, LocationType.DriveThru, "La Vaca Loca", "") {Menu = BeefyBillsMenu },
            new GameLocation(new Vector3(145.3499f, -1460.568f, 28.71129f), 49.75111f, LocationType.DriveThru, "Lucky Plucker", "") {Menu = GenericMenu },

            //Bank
            new GameLocation(new Vector3(-813.9924f, -1114.698f, 11.18181f), 297.7995f, LocationType.Bank, "Fleeca Bank", "Fleeca Bank"),
            new GameLocation(new Vector3(-350.1604f, -45.84864f, 49.03682f), 337.4063f, LocationType.Bank, "Fleeca Bank", "Fleeca Bank"),
            new GameLocation(new Vector3(-1318f, -831.5065f, 16.97263f), 125.3848f, LocationType.Bank, "Maze Bank", "Maze Bank"),
            new GameLocation(new Vector3(150.9058f, -1036.347f, 29.33961f), 340.9843f, LocationType.Bank, "Fleeca", ""),
            new GameLocation(new Vector3(315.2256f, -275.1059f, 53.92431f), 345.6797f, LocationType.Bank, "Fleeca", ""),
            new GameLocation(new Vector3(-3142.849f, 1131.727f, 20.84295f), 247.9002f, LocationType.Bank, "Blaine County Savings", ""),
            new GameLocation(new Vector3(-2966.905f, 483.1484f, 15.6927f), 86.25156f, LocationType.Bank, "Fleeca", ""),

            //Hotel
            new GameLocation(new Vector3(-1183.073f, -1556.673f, 5.036984f), 122.3785f, LocationType.Hotel, "Vespucci Hotel", "Vespucci Hotel") {Menu = CheapHotelMenu,OpenTime = 0, CloseTime = 24 },
            new GameLocation(new Vector3(-1343.127f, -1091.096f, 6.936333f), 299.9456f, LocationType.Hotel, "Venetian", "Venetian") {Menu = ExpensiveHotelMenu,OpenTime = 0, CloseTime = 24 },
            new GameLocation(new Vector3(-1309.048f, -931.2507f, 13.35856f), 23.25741f, LocationType.Hotel, "Crown Jewel Hotel", "Crown Jewel Hotel") {Menu = CheapHotelMenu,OpenTime = 0, CloseTime = 24 },
            new GameLocation(new Vector3(-1660.706f, -533.756f, 36.02398f), 141.6077f, LocationType.Hotel, "Banner", "Banner") {Menu = ExpensiveHotelMenu,OpenTime = 0, CloseTime = 24,CameraPosition = new Vector3(-1660.326f, -566.6978f, 39.62436f), CameraRotation = new Rotator(-11.99999f, 1.091059E-07f, 4.528234f) },//, new Vector3(0f, 0f, 0f), //Camera Position LocationName: bannerhotel
            new GameLocation(new Vector3(-1856.868f, -347.9391f, 49.83775f), 141.5183f, LocationType.Hotel, "Von Krastenburg", "Von Krastenburg") {Menu = ExpensiveHotelMenu,OpenTime = 0, CloseTime = 24 },//needs zoom out
            new GameLocation(new Vector3(-1356.452f, -791.2153f, 20.24218f), 129.4868f, LocationType.Hotel, "Hedera", "Hedera") {Menu = ExpensiveHotelMenu,OpenTime = 0, CloseTime = 24 },//needs zoom out
            new GameLocation(new Vector3(-2007.835f, -314.862f, 32.09708f), 46.05545f, LocationType.Hotel, "The Jetty", "The Jetty") {Menu = ExpensiveHotelMenu,OpenTime = 0, CloseTime = 24 },//needs zoome out
            new GameLocation(new Vector3(-823.0718f, -1223.552f, 7.365416f), 54.09635f, LocationType.Hotel, "The Viceroy", ""){Menu = ViceroyMenu,BannerImage = "viceroy.png",OpenTime = 0, CloseTime = 24, CameraPosition = new Vector3(-847.939f, -1207.791f, 7.15155f), CameraDirection = new Vector3(0.9588153f, -0.1468293f, 0.2431342f), CameraRotation = new Rotator(14.0716f, 0f, -98.70642f) },//needs zoome out
            new GameLocation(new Vector3(-287.0405f, -1060.003f, 27.20538f), 252.0524f, LocationType.Hotel, "Banner", "") {Menu = ExpensiveHotelMenu, CameraPosition = new Vector3(-233.506f, -1048.275f, 34.58431f), CameraDirection = new Vector3(-0.9516708f, -0.2260422f, -0.2079124f), CameraRotation = new Rotator(-12.00004f, 0f, 103.3614f) },
            new GameLocation(new Vector3(68.509f, -958.8935f, 29.80383f), 161.9325f, LocationType.Hotel, "The Emissary", ""){Menu = ExpensiveHotelMenu, CameraPosition = new Vector3(81.53342f, -1010.819f, 63.66661f), CameraDirection = new Vector3(-0.1635272f, 0.9643815f, -0.2079113f), CameraRotation = new Rotator(-11.99998f, 2.182118E-07f, 9.623925f) },

            new GameLocation(new Vector3(313.3858f, -225.0208f, 54.22117f), 160.1122f, LocationType.Hotel, "Pink Cage", ""){Menu = CheapHotelMenu },
            new GameLocation(new Vector3(307.3867f, -727.7486f, 29.31678f), 254.8814f, LocationType.Hotel, "Alesandro Hotel", ""){Menu = CheapHotelMenu },
            new GameLocation(new Vector3(-702.4747f, -2274.476f, 13.45538f), 225.7683f, LocationType.Hotel, "Opium Nights", "") {Menu = ExpensiveHotelMenu},
            new GameLocation(new Vector3(379.4438f, -1781.435f, 29.46008f), 47.01642f, LocationType.Hotel, "Motel & Beauty", "") {Menu = CheapHotelMenu},
            new GameLocation(new Vector3(570.0554f, -1745.989f, 29.22319f), 260.0757f, LocationType.Hotel, "Billings Gate Motel", ""){Menu = CheapHotelMenu},
            new GameLocation(new Vector3(-104.5376f, 6315.921f, 31.57622f), 141.414f, LocationType.Hotel, "Dream View Motel", "Mostly Bug Free!"){Menu = CheapHotelMenu},

            //Pharmacy
            new GameLocation(new Vector3(114.2954f, -4.942202f, 67.82149f), 195.4308f, LocationType.Pharmacy, "Pop's Pills", ""),
            new GameLocation(new Vector3(68.94705f, -1570.043f, 29.59777f), 50.85398f, LocationType.Pharmacy, "Dollar Pills", ""),
            new GameLocation(new Vector3(326.7227f, -1074.448f, 29.47332f), 359.3641f, LocationType.Pharmacy, "Family Pharmacy", ""),

            //Hardware
            new GameLocation(new Vector3(2747.406f, 3473.213f, 55.67021f), 249.8152f, LocationType.HardwareStore, "You Tool", "You Tool") {Menu = ToolMenu,BannerImage = "youtool.png", CameraPosition = new Vector3(2780.472f, 3473.511f, 73.06239f), CameraDirection = new Vector3(-0.9778581f, -0.02382228f, -0.2079087f), CameraRotation = new Rotator(-11.99983f, 0f, 91.39555f) },
            new GameLocation(new Vector3(339.4021f, -776.9934f, 29.2665f), 68.51967f, LocationType.HardwareStore, "Krapea", ""){Menu = ToolMenu },
            new GameLocation(new Vector3(-10.88182f, 6499.395f, 31.50508f), 44.30542f, LocationType.HardwareStore, "Bay Hardware", ""){Menu = ToolMenu },
            new GameLocation(new Vector3(-3153.697f, 1053.398f, 20.88735f), 338.4756f, LocationType.HardwareStore, "Hardware", ""){Menu = ToolMenu },

            //Car Dealer
            //new GameLocation(new Vector3(-69.16984f, 63.42498f, 71.89044f), 150.3918f, LocationType.CarDealer, "Benefactor/Gallivanter", "") { BannerImage = "benefactorgallivanter.png", Menu = BenefactorGallavanterMenu, CameraPosition = new Vector3(231.7523f, -993.08f, -97.99996f), CameraDirection = new Vector3(-0.9534805f, 0.1368595f, -0.2685973f), CameraRotation = new Rotator(-15.58081f, 0f, 81.83174f),ItemPosition = new Vector3(226.205f, -992.613f, -98.99996f), ItemHeading = 177.2006f },
            new GameLocation(new Vector3(-69.16984f, 63.42498f, 71.89044f), 150.3918f, LocationType.CarDealer, "Benefactor/Gallivanter", "") { BannerImage = "benefactorgallivanter.png", Menu = BenefactorGallavanterMenu, 
                CameraPosition = new Vector3(231.7523f, -993.08f, -97.99996f), CameraDirection = new Vector3(-0.9534805f, 0.1368595f, -0.2685973f), CameraRotation = new Rotator(-15.58081f, 0f, 81.83174f),
                ItemPreviewPosition = new Vector3(226.205f, -992.613f, -98.99996f), ItemPreviewHeading = 177.2006f,
                ItemDeliveryPosition = new Vector3(-83.40893f, 80.80059f, 71.08399f), ItemDeliveryHeading = 150.8571f




            },

            new GameLocation(new Vector3(-176.7741f, -1158.648f, 23.81366f), 359.6327f, LocationType.CarDealer, "Vapid", ""),
            new GameLocation(new Vector3(286.8117f, -1148.615f, 29.29189f), 0.5211872f, LocationType.CarDealer, "Sanders Motorcycles", ""),
            new GameLocation(new Vector3(-247.2263f, 6213.266f, 31.93902f), 143.0866f, LocationType.CarDealer, "Melmut's European Autos", ""),









            //PawnShop
            new GameLocation(new Vector3(412.5109f, 314.9815f, 103.1327f), 207.4105f, LocationType.PawnShop, "F.T. Pawn", "") {Menu = ToolMenu },


            //Garage
            new GameLocation(new Vector3(226.205f, -992.613f, -98.99996f), 150.3918f, LocationType.Garage, "Underground Garage", "")

        };






        /*            new GameLocation(new Vector3(2683.969f,3282.098f,55.24052f), 89.53969f, LocationType.GasStation, "24/7 Supermarket Grande Senora" ,"As fast as you"),//,new List<Vector3>() { new Vector3(2678.073f, 3265.522f, 54.7076f),new Vector3(2681.173f, 3262.774f, 54.70736f) }),
            new GameLocation(new Vector3(1725f, 6410f, 35f), 22.23846f, LocationType.GasStation, "24/7 Mount Chilliad (Gas)","As fast as you"),//,new List<Vector3>() { new Vector3(1706.173f, 6412.223f, 32.22713f), new Vector3(1701.657f, 6414.528f, 32.1186f), new Vector3(1697.71f, 6416.565f, 32.08189f),new Vector3(1706.173f, 6412.223f, 32.22713f)
                                                                                                                                                //, new Vector3(1697.869f,6420.53f,32.05283f), new Vector3(1701.852f,6418.417f,32.05503f), new Vector3(1705.852f,6416.659f,32.05479f) }),
            new GameLocation(new Vector3(-1429.33f,-270.8909f,46.2077f), 325.7301f, LocationType.GasStation, "Ron Morningwood","Good Morningwood!"),//,new List<Vector3>() { new Vector3(-1428.23f,-277.0434f,45.79089f), new Vector3(-1436.362f,-267.6647f,45.79237f),
                                                                                                                                                         // new Vector3(-1440.16f,-270.164f,45.79181f),new Vector3(-1431.339f,-280.2279f,45.79009f),
                                                                                                                                                         // new Vector3(-1434.153f,-282.5898f,45.79139f), new Vector3(-1442.416f,-273.0687f,45.7986f),
                                                                                                                                                         // new Vector3(-1446.231f,-276.2419f,45.80196f),new Vector3(-1437.798f,-285.5625f,45.77643f) }),
            new GameLocation(new Vector3(160.4977f,6635.249f,31.61175f), 70.88637f, LocationType.GasStation, "Dons Country Store & Gas","Country Manners!"),//,new List<Vector3>() { new Vector3(188.7231f,6607.43f,31.84954f), new Vector3(184.8491f,6606.112f,31.85245f), new Vector3(181.3225f,6605.81f,31.84829f)
                                                                                                                                               // , new Vector3(178.1896f,6604.389f,31.89782f), new Vector3(174.0998f,6604.168f,31.84834f), new Vector3(171.1875f,6603.454f,32.04737f) }),
            new GameLocation(new Vector3(266.2746f,2599.669f,44.7383f), 231.9223f, LocationType.GasStation, "Harmony General Store & Gas","Always in Harmony!"), //,new List<Vector3>() { new Vector3(262.5423f, 2610.143f, 44.3814f),new Vector3(265.0521f, 2604.807f, 44.38421f) }),

            new GameLocation(new Vector3(1039.753f,2666.26f,39.55253f), 143.6208f, LocationType.GasStation, "Grande Senora Cafe & Gas","Extra Grande!"),//,new List<Vector3>() { new Vector3(1043.293f,2677.189f,38.90083f), new Vector3(1035.182f,2677.444f,38.90271f), new Vector3(1034.835f,2670.396f,38.9343f)
                                                                                                                                                //, new Vector3(1043.761f,2670.147f,38.94082f), new Vector3(1043.412f,2672.185f,38.95105f), new Vector3(1034.338f,2672.41f,38.94936f) }),


            new GameLocation(new Vector3(-1817.871f,787.0063f,137.917f), 89.38248f, LocationType.GasStation, "LTD Richmond Glen",""),//,new List<Vector3>() { new Vector3(-1804.758f,792.6874f,138.5142f), new Vector3(-1809.721f,798.1087f,138.5137f),
                                                                                                                                                        //  new Vector3(-1807.576f,801.251f,138.5144f),new Vector3(-1802.39f,796.0137f,138.5141f),
                                                                                                                                                       //   new Vector3(-1798.391f,798.9479f,138.5154f), new Vector3(-1802.964f,805.196f,138.5681f),
                                                                                                                                                       //   new Vector3(-1800.726f,807.3672f,138.515f),new Vector3(-1796.539f,803.0259f,138.5148f),
                                                                                                                                                        //  new Vector3(-1792.259f,804.6542f,138.5133f),new Vector3(-1796.816f,810.0197f,138.5144f),
                                                                                                                                                       //   new Vector3(-1794.411f,813.2302f,138.5146f),new Vector3(-1789.586f,808.2133f,138.5163f)}),*/
    }
}

