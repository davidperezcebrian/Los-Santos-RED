﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class PossibleLocations
{


    public PossibleLocations()
    {

    }
    public List<GameLocation> LocationsList { get; private set; } = new List<GameLocation>();
    public List<DeadDrop> DeadDrops { get; private set; } = new List<DeadDrop>();
    public List<ScrapYard> ScrapYards { get; private set; } = new List<ScrapYard>();
    public List<GangDen> GangDens { get; private set; } = new List<GangDen>();
    public List<GunStore> GunStores { get; private set; } = new List<GunStore>();
    public List<Hotel> Hotels { get; private set; } = new List<Hotel>();
    public List<Residence> Residences { get; private set; } = new List<Residence>();
    public List<CityHall> CityHalls { get; private set; } = new List<CityHall>();
    public List<VendingMachine> VendingMachines { get; private set; } = new List<VendingMachine>();
    public List<PoliceStation> PoliceStations { get; private set; } = new List<PoliceStation>();
    public List<Hospital> Hospitals { get; private set; } = new List<Hospital>();
    public List<FireStation> FireStations { get; private set; } = new List<FireStation>();
    public List<Restaurant> Restaurants { get; private set; } = new List<Restaurant>();
    public List<Pharmacy> Pharmacies { get; private set; } = new List<Pharmacy>();
    public List<Dispensary> Dispensaries { get; private set; } = new List<Dispensary>();
    public List<HeadShop> HeadShops { get; private set; } = new List<HeadShop>();
    public List<HardwareStore> HardwareStores { get; private set; } = new List<HardwareStore>();
    public List<PawnShop> PawnShops { get; private set; } = new List<PawnShop>();
    public List<Stadium> Stadiums { get; private set; } = new List<Stadium>();
    public List<BeautyShop> BeautyShops { get; private set; } = new List<BeautyShop>();
    public List<Bank> Banks { get; private set; } = new List<Bank>();

    public List<ConvenienceStore> ConvenienceStores { get; private set; } = new List<ConvenienceStore>();

}

