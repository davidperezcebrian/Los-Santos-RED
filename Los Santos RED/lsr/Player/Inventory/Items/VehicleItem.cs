﻿using LosSantosRED.lsr.Helper;
using LosSantosRED.lsr.Interface;
using LSR.Vehicles;
using Rage;
using Rage.Native;
using RAGENativeUI;
using RAGENativeUI.Elements;
using System.Drawing;
using System.Linq;


public class VehicleItem : ModItem
{
    private int PrimaryColor = 0;
    private int SecondaryColor = 0;
    private Color SellPrimaryColor = Color.Black;
    private Color SellSecondaryColor = Color.Black;
    private int Livery1 = -1;

    public bool RequiresDLC { get; set; } = false;
    public string ModelName { get; set; }
    public uint ModelHash { get; set; }
    public VehicleItem()
    {
    }

    public VehicleItem(string name, string description, ItemType itemType) : base(name, description, itemType)
    {

    }
    public VehicleItem(string name, ItemType itemType) : base(name, itemType)
    {

    }
    public VehicleItem(string name, bool requiresDLC, ItemType itemType) : base(name, itemType)
    {
        RequiresDLC = requiresDLC;
    }
    public VehicleItem(string name, string description, bool requiresDLC, ItemType itemType) : base(name, description, itemType)
    {
        RequiresDLC = requiresDLC;
    }
    public override void Setup(PhysicalItems physicalItems, IWeapons weapons)
    {
        //ModelItem = new PhysicalItem(ModelItemID, Game.GetHashKey(ModelItemID), ePhysicalItemType.Vehicle);
        ModelItem = new PhysicalItem(ModelName, ModelHash == 0 ? Game.GetHashKey(ModelName) : ModelHash, ePhysicalItemType.Vehicle);
        MenuCategory = NativeHelper.VehicleClassName(Game.GetHashKey(ModelItem.ModelName));
    }
    public override void CreateSellMenuItem(Transaction Transaction, MenuItem menuItem, UIMenu sellMenuRNUI, ISettingsProvideable settings, ILocationInteractable player, bool isStealing, IEntityProvideable world)
    {
        PrimaryColor = 0;
        SecondaryColor = 0;
        SellPrimaryColor = Color.Black;
        SellSecondaryColor = Color.Black;
        string formattedSalesPrice = menuItem.SalesPrice.ToString("C0");
        string MakeName = NativeHelper.VehicleMakeName(Game.GetHashKey(ModelItem.ModelName));
        string ClassName = NativeHelper.VehicleClassName(Game.GetHashKey(ModelItem.ModelName));
        string ModelName = NativeHelper.VehicleModelName(Game.GetHashKey(ModelItem.ModelName));
        string description;
        if (Description.Length >= 200)
        {
            description = Description.Substring(0, 200) + "...";//menu cant show more than 225?, need some for below
        }
        else
        {
            description = Description;
        }
        description += "~n~~s~";
        if (MakeName != "")
        {
            description += $"~n~Manufacturer: ~b~{MakeName}~s~";
        }
        if (ModelName != "")
        {
            description += $"~n~Model: ~g~{ModelName}~s~";
        }
        if (ClassName != "")
        {
            description += $"~n~Class: ~p~{ClassName}~s~";
        }
        if (RequiresDLC)
        {
            description += $"~n~~b~DLC Vehicle";
        }


        bool enabled = false;
        VehicleExt ownedVersion = player.VehicleOwnership.OwnedVehicles.FirstOrDefault(x => x.Vehicle.Exists() && x.Vehicle.Model.Hash == Game.GetHashKey(ModelItem.ModelName));

        if (ownedVersion != null)
        {
            SellPrimaryColor = ownedVersion.Vehicle.PrimaryColor;
            SellSecondaryColor = ownedVersion.Vehicle.SecondaryColor;
            enabled = true;
        }


        UIMenu VehicleMenu = null;
        bool FoundCategoryMenu = false;


        UIMenu VehicleSubMenu = sellMenuRNUI.Children.Where(x => x.Value.SubtitleText.ToLower() == "vehicles").FirstOrDefault().Value;
        UIMenu ToCheckFirst = sellMenuRNUI;
        if (VehicleSubMenu != null)
        {
            ToCheckFirst = VehicleSubMenu;
        }
        UIMenu CategoryMenu = ToCheckFirst.Children.Where(x => x.Value.SubtitleText == MenuCategory).FirstOrDefault().Value;
        if(CategoryMenu != null)
        {
            FoundCategoryMenu = true;
            VehicleMenu = Transaction.MenuPool.AddSubMenu(CategoryMenu, menuItem.ModItemName);
            CategoryMenu.MenuItems[CategoryMenu.MenuItems.Count() - 1].Description = description;
            CategoryMenu.MenuItems[CategoryMenu.MenuItems.Count() - 1].RightLabel = formattedSalesPrice;
            CategoryMenu.MenuItems[CategoryMenu.MenuItems.Count() - 1].Enabled = enabled;
            EntryPoint.WriteToConsole($"Added Vehicle {Name} To SubMenu {CategoryMenu.SubtitleText}", 5);
        }
        //foreach (UIMenu uimen in Transaction.MenuPool.ToList())
        //{
        //    if (uimen.SubtitleText == ClassName && uimen.ParentMenu == sellMenuRNUI)
        //    {
        //        FoundCategoryMenu = true;
        //        VehicleMenu = Transaction.MenuPool.AddSubMenu(uimen, menuItem.ModItemName);
        //        uimen.MenuItems[uimen.MenuItems.Count() - 1].Description = description;
        //        uimen.MenuItems[uimen.MenuItems.Count() - 1].RightLabel = formattedSalesPrice;
        //        EntryPoint.WriteToConsole($"Added Vehicle {Name} To SubMenu {uimen.SubtitleText}", 5);
        //        break;
        //    }
        //}
        if (!FoundCategoryMenu && VehicleMenu == null)
        {
            VehicleMenu = Transaction.MenuPool.AddSubMenu(sellMenuRNUI, menuItem.ModItemName);
            sellMenuRNUI.MenuItems[sellMenuRNUI.MenuItems.Count() - 1].Description = description;
            sellMenuRNUI.MenuItems[sellMenuRNUI.MenuItems.Count() - 1].RightLabel = formattedSalesPrice;
            sellMenuRNUI.MenuItems[sellMenuRNUI.MenuItems.Count() - 1].Enabled = enabled;
            EntryPoint.WriteToConsole($"Added Vehicle {Name} To Main Buy Menu", 5);
        }
        if (Transaction.HasBannerImage)
        {
            VehicleMenu.SetBannerType(Transaction.BannerImage);
        }
        else if (Transaction.RemoveBanner)
        {
            VehicleMenu.RemoveBanner();
        }
        description = Description;
        if (description == "")
        {
            description = $"List Price {formattedSalesPrice}";
        }

        UIMenuItem Sell = new UIMenuItem($"Sell", "Select to sell this vehicle") { RightLabel = formattedSalesPrice, Enabled = enabled };
        Sell.Activated += (sender, selectedItem) =>
        {
            if(SellVehicle(Transaction, menuItem, player, settings, world))
            {
                player.BankAccounts.GiveMoney(menuItem.SalesPrice);
                Transaction.MoneySpent += menuItem.SalesPrice;
                sender.Visible = false;
            }
        };
        VehicleMenu.AddItem(Sell);
    }
    private bool SellVehicle(Transaction transaction, MenuItem CurrentMenuItem, ILocationInteractable player, ISettingsProvideable settings, IEntityProvideable world)
    {
        VehicleExt toSell = player.VehicleOwnership.OwnedVehicles.Where(x => x.Vehicle.Exists() && x.Vehicle.Model.Hash == Game.GetHashKey(ModelItem.ModelName)).OrderBy(x => x.Vehicle.DistanceTo2D(player.Position)).FirstOrDefault();
        if (toSell != null)
        {
            player.VehicleOwnership.RemoveOwnershipOfVehicle(toSell);
            transaction.OnItemSold(this, CurrentMenuItem, 1);
            return true;
        }
        else
        {
            transaction.PlayErrorSound();
            transaction.DisplayMessage("~r~Sale Failed", "We are sorry, we are unable to complete this transation");
            return false;
        }
    }
    public override void CreatePurchaseMenuItem(Transaction Transaction, MenuItem menuItem, UIMenu purchaseMenu, ISettingsProvideable settings, ILocationInteractable player, bool isStealing, IEntityProvideable world)
    {
        PrimaryColor = 0;
        SecondaryColor = 0;
        if(RequiresDLC && !settings.SettingsManager.PlayerOtherSettings.AllowDLCVehiclesInStores)
        {
            return;
        }

        string formattedPurchasePrice = menuItem.PurchasePrice.ToString("C0");
        if (menuItem.PurchasePrice == 0)
        {
            formattedPurchasePrice = "FREE";
        }

        string MakeName = NativeHelper.VehicleMakeName(Game.GetHashKey(ModelItem.ModelName));
        string ClassName = NativeHelper.VehicleClassName(Game.GetHashKey(ModelItem.ModelName));
        string ModelName = NativeHelper.VehicleModelName(Game.GetHashKey(ModelItem.ModelName));
        string description;
        if (Description.Length >= 200)
        {
            description = Description.Substring(0, 200) + "...";//menu cant show more than 225?, need some for below
        }
        else
        {
            description = Description;
        }

        description += "~n~~s~";
        if (MakeName != "")
        {
            description += $"~n~Manufacturer: ~b~{MakeName}~s~";
        }
        if (ModelName != "")
        {
            description += $"~n~Model: ~g~{ModelName}~s~";
        }
        if (ClassName != "")
        {
            description += $"~n~Class: ~p~{ClassName}~s~";
        }
        if (RequiresDLC)
        {
            description += $"~n~~b~DLC Vehicle";
        }
        UIMenu VehicleMenu = null;
        bool FoundCategoryMenu = false;

        UIMenu VehicleSubMenu = purchaseMenu.Children.Where(x => x.Value.SubtitleText.ToLower() == "vehicles").FirstOrDefault().Value;
        UIMenu ToCheckFirst = purchaseMenu;
        if (VehicleSubMenu != null)
        {
            ToCheckFirst = VehicleSubMenu;
        }
        UIMenu CategoryMenu = ToCheckFirst.Children.Where(x => x.Value.SubtitleText == MenuCategory).FirstOrDefault().Value;
        if (CategoryMenu != null)
        {
            FoundCategoryMenu = true;
            VehicleMenu = Transaction.MenuPool.AddSubMenu(CategoryMenu, menuItem.ModItemName);
            CategoryMenu.MenuItems[CategoryMenu.MenuItems.Count() - 1].Description = description;
            CategoryMenu.MenuItems[CategoryMenu.MenuItems.Count() - 1].RightLabel = formattedPurchasePrice;
            EntryPoint.WriteToConsole($"Added Vehicle {Name} To SubMenu {CategoryMenu.SubtitleText}", 5);
        }
        //foreach (UIMenu uimen in Transaction.MenuPool.ToList())
        //{
        //    if (uimen.SubtitleText == ClassName)
        //    {
        //        FoundCategoryMenu = true;
        //        VehicleMenu = Transaction.MenuPool.AddSubMenu(uimen, menuItem.ModItemName);
        //        uimen.MenuItems[uimen.MenuItems.Count() - 1].Description = description;
        //        uimen.MenuItems[uimen.MenuItems.Count() - 1].RightLabel = formattedPurchasePrice;
        //        EntryPoint.WriteToConsole($"Added Vehicle {Name} To SubMenu {uimen.SubtitleText}", 5);
        //        break;
        //    }
        //}
        if (!FoundCategoryMenu && VehicleMenu == null)
        {
            VehicleMenu = Transaction.MenuPool.AddSubMenu(purchaseMenu, menuItem.ModItemName);
            purchaseMenu.MenuItems[purchaseMenu.MenuItems.Count() - 1].Description = description;
            purchaseMenu.MenuItems[purchaseMenu.MenuItems.Count() - 1].RightLabel = formattedPurchasePrice;
            EntryPoint.WriteToConsole($"Added Vehicle {Name} To Main Buy Menu", 5);
        }
        if (Transaction.HasBannerImage)
        {
            VehicleMenu.SetBannerType(Transaction.BannerImage);
        }
        else if (Transaction.RemoveBanner)
        {
            VehicleMenu.RemoveBanner();
        }
        UIMenu colorFullMenu = Transaction.MenuPool.AddSubMenu(VehicleMenu, "Colors");
        colorFullMenu.SubtitleText = "COLORS";
        VehicleMenu.MenuItems[VehicleMenu.MenuItems.Count() - 1].Description = "Pick Colors";
        if (Transaction.HasBannerImage) { colorFullMenu.SetBannerType(Transaction.BannerImage); }

        UIMenu primaryColorMenu = Transaction.MenuPool.AddSubMenu(colorFullMenu, "Primary Color");
        primaryColorMenu.SubtitleText = "PRIMARY COLOR GROUPS";
        colorFullMenu.MenuItems[colorFullMenu.MenuItems.Count() - 1].Description = "Pick Primary Colors";
        if (Transaction.HasBannerImage) { primaryColorMenu.SetBannerType(Transaction.BannerImage); }

        UIMenu secondaryColorMenu = Transaction.MenuPool.AddSubMenu(colorFullMenu, "Secondary Color");
        secondaryColorMenu.SubtitleText = "SECONDARY COLOR GROUPS";
        colorFullMenu.MenuItems[colorFullMenu.MenuItems.Count() - 1].Description = "Pick Secondary Colors";
        if (Transaction.HasBannerImage) { secondaryColorMenu.SetBannerType(Transaction.BannerImage); }


        //Add Color Sub Menu Here
        foreach (string colorGroupString in Transaction.VehicleColors.GroupBy(x => x.ColorGroup).Select(x => x.Key).Distinct().OrderBy(x => x))
        {
            UIMenu primarycolorGroupMenu = Transaction.MenuPool.AddSubMenu(primaryColorMenu, colorGroupString);
            primarycolorGroupMenu.SubtitleText = "PRIMARY COLORS";
            primaryColorMenu.MenuItems[primaryColorMenu.MenuItems.Count() - 1].Description = "Choose a color group";
            if (Transaction.HasBannerImage) { primarycolorGroupMenu.SetBannerType(Transaction.BannerImage); }

            UIMenu secondarycolorGroupMenu = Transaction.MenuPool.AddSubMenu(secondaryColorMenu, colorGroupString);
            secondarycolorGroupMenu.SubtitleText = "SECONDARY COLORS";
            secondaryColorMenu.MenuItems[secondaryColorMenu.MenuItems.Count() - 1].Description = "Choose a color group";
            if (Transaction.HasBannerImage) { secondarycolorGroupMenu.SetBannerType(Transaction.BannerImage); }

            foreach (VehicleColorLookup cl in Transaction.VehicleColors.Where(x => x.ColorGroup == colorGroupString))
            {
                UIMenuItem actualColorPrimary = new UIMenuItem(cl.ColorName, cl.FullColorName);
                actualColorPrimary.RightBadge = UIMenuItem.BadgeStyle.Heart;
                actualColorPrimary.RightBadgeInfo.Color = cl.RGBColor;
                actualColorPrimary.Activated += (sender, selectedItem) =>
                {
                    PrimaryColor = cl.ColorID;
                    if (Transaction.SellingVehicle.Exists())
                    {
                        NativeFunction.Natives.SET_VEHICLE_COLOURS(Transaction.SellingVehicle, PrimaryColor, SecondaryColor);
                    }
                };
                primarycolorGroupMenu.AddItem(actualColorPrimary);

                UIMenuItem actualColorSecondary = new UIMenuItem(cl.ColorName, cl.FullColorName);

                actualColorSecondary.RightBadge = UIMenuItem.BadgeStyle.Heart;
                actualColorSecondary.RightBadgeInfo.Color = cl.RGBColor;

                actualColorSecondary.Activated += (sender, selectedItem) =>
                {
                    SecondaryColor = cl.ColorID;
                    if (Transaction.SellingVehicle.Exists())
                    {
                        NativeFunction.Natives.SET_VEHICLE_COLOURS(Transaction.SellingVehicle, PrimaryColor, SecondaryColor);
                    }
                };
                secondarycolorGroupMenu.AddItem(actualColorSecondary);
            }
        }

        if (Transaction.SellingVehicle.Exists())//it will never exist here......
        {
            int Livery1Count = NativeFunction.Natives.GET_VEHICLE_LIVERY_COUNT<int>(Transaction.SellingVehicle);
            int Livery2Count = NativeFunction.Natives.GET_VEHICLE_LIVERY2_COUNT<int>(Transaction.SellingVehicle);

            if (Livery1Count > -1 || Livery2Count > -1)
            {
                UIMenu liveryFullMenu = Transaction.MenuPool.AddSubMenu(VehicleMenu, "Liveries");
                liveryFullMenu.SubtitleText = "LIVERIES";
                VehicleMenu.MenuItems[VehicleMenu.MenuItems.Count() - 1].Description = "Pick Livery";
                if (Transaction.HasBannerImage) { liveryFullMenu.SetBannerType(Transaction.BannerImage); }
                if (Livery1Count > -1)
                {
                    for (int i = -1; i <= Livery1Count - 1; i++)
                    {
                        UIMenuItem liveryOneMenu = new UIMenuItem($"Livery: {i}");
                        liveryOneMenu.Activated += (sender, selectedItem) =>
                        {
                            Livery1 = i;
                            EntryPoint.WriteToConsole($"Livery 1 Activated {Livery1}");
                            if (Transaction.SellingVehicle.Exists())
                            {
                                NativeFunction.Natives.SET_VEHICLE_LIVERY(Transaction.SellingVehicle, Livery1);
                            }
                        };
                        liveryFullMenu.AddItem(liveryOneMenu);
                    }
                }
                if (Livery2Count > -1)
                {

                }

            }
        }


        //Purchase Stuff Here
        UIMenuItem Purchase = new UIMenuItem($"Purchase", "Select to purchase this vehicle") { RightLabel = formattedPurchasePrice };
        Purchase.Activated += (sender, selectedItem) =>
        {
            if (menuItem != null)
            {
                EntryPoint.WriteToConsole($"Vehicle Purchase {menuItem.ModItemName} Player.Money {player.BankAccounts.Money} menuItem.PurchasePrice {menuItem.PurchasePrice}", 5);
                if (player.BankAccounts.Money < menuItem.PurchasePrice)
                {
                    Transaction.DisplayInsufficientFundsMessage();
                    return;
                }
                if (!PurchaseVehicle(Transaction, menuItem, player,settings, world))
                {
                    return;
                }
                player.BankAccounts.GiveMoney(-1 * menuItem.PurchasePrice);
                Transaction.MoneySpent += menuItem.PurchasePrice;
            }
            sender.Visible = false;
            //Dispose();
        };
        VehicleMenu.AddItem(Purchase);
        
    }
    private bool PurchaseVehicle(Transaction transaction, MenuItem CurrentMenuItem, ILocationInteractable player, ISettingsProvideable settings, IEntityProvideable world)
    {
        bool ItemInDeliveryBay = true;
        SpawnPlace ChosenSpawn = null;
        foreach (SpawnPlace sp in transaction.ItemDeliveryLocations.OrderBy(x => RandomItems.GetRandomNumber(0f, 1f)))
        {
            ItemInDeliveryBay = Rage.World.GetEntities(sp.Position, 7f, GetEntitiesFlags.ConsiderAllVehicles).Any();
            if (!ItemInDeliveryBay)
            {
                ChosenSpawn = sp;
                break;
            }
        }
        if (!ItemInDeliveryBay && ChosenSpawn != null)
        {
            Vehicle NewVehicle = new Vehicle(ModelItem.ModelName, ChosenSpawn.Position, ChosenSpawn.Heading);
            if (NewVehicle.Exists())
            {  
                CurrentMenuItem.ItemsSoldToPlayer += 1;
                NativeFunction.Natives.SET_VEHICLE_COLOURS(NewVehicle, PrimaryColor, SecondaryColor);
                if(Livery1 != -1)
                {
                    NativeFunction.Natives.SET_VEHICLE_LIVERY(NewVehicle, Livery1);
                }
                NewVehicle.Wash();
                NewVehicle.LicensePlate = new PlateType(0, "", "San Andreas", 0, "12ABC345").GenerateNewLicensePlateNumber();
                VehicleExt MyNewCar = world.Vehicles.GetVehicleExt(NewVehicle);
                if (MyNewCar == null)
                {
                    MyNewCar = new VehicleExt(NewVehicle, settings);
                    MyNewCar.Setup();
                    EntryPoint.WriteToConsole("New Vehicle Created in PurchaseVehicle");
                }
                world.Vehicles.AddEntity(MyNewCar, ResponseType.None);
                player.VehicleOwnership.TakeOwnershipOfVehicle(MyNewCar, false);
                transaction.OnItemPurchased(this, CurrentMenuItem, 1);
                return true;
            }
            else
            {
                transaction.PlayErrorSound();
                transaction.DisplayMessage("~r~Delivery Failed", "We are sorry, we are unable to complete this transation");
                return false;
            }
        }
        else
        {
            transaction.PlayErrorSound();
            transaction.DisplayMessage("~o~Blocked Delivery", "We are sorry, we are unable to complete this transation, the delivery bay is blocked");
            return false;
        }
    }
    public override void CreatePreview(Transaction Transaction, Camera StoreCam, bool isPurchase)
    {
        if (ModelItem != null && NativeFunction.Natives.IS_MODEL_VALID<bool>(Game.GetHashKey(ModelItem.ModelName)))
        {
            NativeFunction.Natives.CLEAR_AREA(Transaction.ItemPreviewPosition.X, Transaction.ItemPreviewPosition.Y, Transaction.ItemPreviewPosition.Z, 4f, true, false, false, false);
            Transaction.SellingVehicle = new Vehicle(ModelItem.ModelName, Transaction.ItemPreviewPosition, Transaction.ItemPreviewHeading);
        }
        if (Transaction.SellingVehicle.Exists())
        {
            Transaction.SellingVehicle.Wash();
            if (isPurchase)
            {
                NativeFunction.Natives.SET_VEHICLE_COLOURS(Transaction.SellingVehicle, PrimaryColor, SecondaryColor);
            }
            else
            {
                Transaction.SellingVehicle.PrimaryColor = SellPrimaryColor;
                Transaction.SellingVehicle.SecondaryColor = SellSecondaryColor;
                //NativeFunction.Natives.SET_VEHICLE_COLOURS(Transaction.SellingVehicle, SellPrimaryColor, SellSecondaryColor);
            }
            NativeFunction.Natives.SET_VEHICLE_ON_GROUND_PROPERLY<bool>(Transaction.SellingVehicle, 5.0f);
            Transaction.SellingVehicle.LicensePlate = new PlateType(0, "", "San Andreas", 0, "12ABC345").GenerateNewLicensePlateNumber();
        }
    }
}

