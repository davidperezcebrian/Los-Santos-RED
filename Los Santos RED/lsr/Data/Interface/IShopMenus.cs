﻿using Rage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LosSantosRED.lsr.Interface
{
    public interface IShopMenus
    {
        List<ShopMenu> ShopMenuList { get; }

        ShopMenu GetMenu(string v);
        ShopMenu GetRandomDrugDealerMenu();
        ShopMenu GetVendingMenu(string v);
        ShopMenu GetRandomDrugCustomerMenu();
        ShopMenu GetRandomMenu(string dealerMenuID);
        Tuple<int, int> GetPrices(string name);
    }
}
