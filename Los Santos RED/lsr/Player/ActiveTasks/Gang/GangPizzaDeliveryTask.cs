﻿using ExtensionsMethods;
using LosSantosRED.lsr.Helper;
using LosSantosRED.lsr.Interface;
using Rage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LosSantosRED.lsr.Player.ActiveTasks
{
    public class GangPizzaDeliveryTask
    {
        private ITaskAssignable Player;
        private ITimeReportable Time;
        private IGangs Gangs;
        private PlayerTasks PlayerTasks;
        private IPlacesOfInterest PlacesOfInterest;
        private List<DeadDrop> ActiveDrops = new List<DeadDrop>();
        private ISettingsProvideable Settings;
        private IEntityProvideable World;
        private ICrimes Crimes;
        private IModItems ModItems;
        private IShopMenus ShopMenus;
        private Gang HiringGang;
        private DeadDrop DeadDrop;
        private PlayerTask CurrentTask;
        private int GameTimeToWaitBeforeComplications;
        private bool HasAddedComplications;
        private bool WillAddComplications;
        private int MoneyToRecieve;
        //private int MoneyToPickup;
        private GangDen HiringGangDen;
        private ModItem ItemToDeliver;
        private Restaurant ClosestPlace;
        private int NumberOfItemsToDeliver;

        private bool HasDen => HiringGangDen != null;

        public GangPizzaDeliveryTask(ITaskAssignable player, ITimeReportable time, IGangs gangs, PlayerTasks playerTasks, IPlacesOfInterest placesOfInterest, List<DeadDrop> activeDrops, ISettingsProvideable settings, IEntityProvideable world, ICrimes crimes, IModItems modItems, IShopMenus shopMenus)
        {
            Player = player;
            Time = time;
            Gangs = gangs;
            PlayerTasks = playerTasks;
            PlacesOfInterest = placesOfInterest;
            ActiveDrops = activeDrops;
            Settings = settings;
            World = world;
            Crimes = crimes;
            ModItems = modItems;
            ShopMenus = shopMenus;
        }
        public void Setup()
        {

        }
        public void Dispose()
        {
            if (DeadDrop != null)
            {
                DeadDrop.Dispose();
            }
        }
        public void Start(Gang ActiveGang)
        {
            HiringGang = ActiveGang;
            if (PlayerTasks.CanStartNewTask(HiringGang?.ContactName))
            {
                GetHiringDen();
                if (HasDen)
                {
                    GetRequiredPayment();
                    SendInitialInstructionsMessage();
                    AddTask();
                }
                else
                {
                    SendTaskAbortMessage();
                }
            }
        }
        private void GetHiringDen()
        {
            HiringGangDen = PlacesOfInterest.PossibleLocations.GangDens.FirstOrDefault(x => x.AssociatedGang?.ID == HiringGang.ID);
        }
        private void GetRequiredPayment()
        {
            int PaymentAmount = RandomItems.GetRandomNumberInt(100, 250).Round(10);
            List<string> PossibleItems = new List<string>() { 
                "10 inch Cheese Pizza","10 inch Pepperoni Pizza","10 inch Supreme Pizza"
                ,"12 inch Cheese Pizza","12 inch Pepperoni Pizza","12 inch Supreme Pizza"
                ,"18 inch Cheese Pizza","18 inch Pepperoni Pizza","18 inch Supreme Pizza"
                ,"Small Cheese Pizza","Small Pepperoni Pizza","Small Supreme Pizza"
                ,"Medium Cheese Pizza","Medium Pepperoni Pizza","Medium Supreme Pizza"
                ,"Large Cheese Pizza","Large Pepperoni Pizza","Large Supreme Pizza" 
          };

            string ChosenItem = PossibleItems.PickRandom();
            ItemToDeliver = null;

            ClosestPlace = null;


            if (ChosenItem != "")
            {
                ItemToDeliver = ModItems.Get(ChosenItem);
                ClosestPlace = PlacesOfInterest.PossibleLocations.Restaurants.Where(x => x.Menu?.Items.Any(y => y.ModItemName == ChosenItem) == true).OrderBy(x => x.DistanceToPlayer).FirstOrDefault();
            }
            if (ItemToDeliver != null)
            {
                NumberOfItemsToDeliver = RandomItems.GetRandomNumberInt(1, 3);
                MoneyToRecieve = PaymentAmount;
                EntryPoint.WriteToConsole($"GANG Pizza Pickup Item: {ItemToDeliver.Name} Number: {NumberOfItemsToDeliver} Payment: {MoneyToRecieve}");
            }
            if (MoneyToRecieve <= 0)
            {
                MoneyToRecieve = 250;
            }
            MoneyToRecieve = MoneyToRecieve.Round(10);
        }
        private void AddTask()
        {
            PlayerTasks.AddQuickTask(HiringGang.ContactName, MoneyToRecieve, 200, 0, -200, 1, "Pizza Pickup for Gang");
            HiringGangDen.ExpectedItem = ItemToDeliver;
            HiringGangDen.ExpectedItemAmount = NumberOfItemsToDeliver;

            CurrentTask = PlayerTasks.GetTask(HiringGang.ContactName);
            CurrentTask.IsReadyForPayment = true;


            GameTimeToWaitBeforeComplications = RandomItems.GetRandomNumberInt(3000, 10000);
            HasAddedComplications = false;
            WillAddComplications = false;// RandomItems.RandomPercent(Settings.SettingsManager.TaskSettings.RivalGangHitComplicationsPercentage);
        }
        private void SendInitialInstructionsMessage()
        {
            List<string> Replies = new List<string>() {
                    $"Go pickup {(NumberOfItemsToDeliver == 1 ? "a" : "1" )} {ItemToDeliver.Name}{(NumberOfItemsToDeliver == 1 ? "" : "s" )}. Make it quick. Bring it to the {HiringGang.DenName} on {HiringGangDen.FullStreetAddress} in less than an hour. ${MoneyToRecieve}.",
                    $"Go get {(NumberOfItemsToDeliver == 1 ? "a" : "1" )} {ItemToDeliver.Name}{(NumberOfItemsToDeliver == 1 ? "" : "s" )}. Don't take long. Get back to the {HiringGang.DenName} on {HiringGangDen.FullStreetAddress} in less than an hour. ${MoneyToRecieve}.",
                    $"Need you to pickup {(NumberOfItemsToDeliver == 1 ? "a" : "1" )} {ItemToDeliver.Name}{(NumberOfItemsToDeliver == 1 ? "" : "s" )}. We need it quick. Take it to the {HiringGang.DenName} on {HiringGangDen.FullStreetAddress} in less than an hour. ${MoneyToRecieve}.",
                    };
            string reply = Replies.PickRandom();
            if (ClosestPlace != null)
            {
                reply += $" You should be able to get it at {ClosestPlace.Name}.";
            }
            Player.CellPhone.AddPhoneResponse(HiringGang.ContactName, HiringGang.ContactIcon, reply);
        }
        private void SendTaskAbortMessage()
        {
            List<string> Replies = new List<string>() {
                    "Nothing yet, I'll let you know",
                    "I've got nothing for you yet",
                    "Give me a few days",
                    "Not a lot to be done right now",
                    "We will let you know when you can do something for us",
                    "Check back later.",
                    };
            Player.CellPhone.AddPhoneResponse(HiringGang.ContactName, Replies.PickRandom());
        }
    }
}
