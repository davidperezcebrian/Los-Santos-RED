﻿using ExtensionsMethods;
using LosSantosRED.lsr;
using LosSantosRED.lsr.Data;
using LosSantosRED.lsr.Helper;
using LosSantosRED.lsr.Interface;
using LosSantosRED.lsr.Locations;
using LosSantosRED.lsr.Player;
using LSR.Vehicles;
using Rage;
using Rage.Native;
using RAGENativeUI;
using RAGENativeUI.Elements;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Mod
{
    public class Player : IDispatchable, IActivityPerformable, IIntoxicatable, ITargetable, IPoliceRespondable, IInputable, IPedSwappable, IMuggable, IRespawnable, IViolateable, IWeaponDroppable, IDisplayable,
                          ICarStealable, IPlateChangeable, IActionable, IInteractionable, IInventoryable, IRespawning, ISaveable, IPerceptable, ILocateable, IDriveable, ISprintable, IWeatherReportable,
                          IBusRideable, IGangRelateable, IWeaponSwayable, IWeaponRecoilable, IWeaponSelectable, ICellPhoneable, ITaskAssignable, IContactInteractable, IGunDealerRelateable, ILicenseable, IPropertyOwnable, ILocationInteractable, IButtonPromptable, IHumanStateable, IStanceable,
                          IItemEquipable, IDestinateable, IVehicleOwnable, IBankAccountHoldable, IActivityManageable, IHealthManageable, IGroupManageable, IMeleeManageable, ISeatAssignable, ICameraControllable, IPlayerVoiceable
    {
        public int UpdateState = 0;
        private float CurrentVehicleRoll;
        private uint GameTimeGotInVehicle;
        private uint GameTimeGotOutOfVehicle;
        private uint GameTimeLastBusted;
        private uint GameTimeLastCrashedVehicle;
        private uint GameTimeLastDied;
        private uint GameTimeLastFedUpCop;
        private uint GameTimeLastMoved;
        private uint GameTimeLastMovedFast;
        private uint GameTimeLastSetWanted;
        private uint GameTimeLastShot;
        private uint GameTimeLastUpdatedLookedAtPed;
        private uint GameTimeStartedHotwiring;
        private uint GameTimeStartedMoving;
        private uint GameTimeStartedMovingFast;
        private uint GameTimeStartedPlaying;
        private uint GameTimeWantedLevelStarted;
        private bool HasThrownGotOffFreeway;
        private bool HasThrownGotOnFreeway;
        private bool isActive = true;
        private bool isAiming;
        private bool isAimingInVehicle;
        private bool isCheckingExcessSpeed;
        private bool isExcessiveSpeed;
        private bool isGettingIntoVehicle;
        private bool isHotwiring;
        private bool isInVehicle;
        private bool isJacking = false;
        private bool isShooting;
        private Vector3 position;
        private uint prevCurrentLookedAtObjectHandle;
        private int PreviousWantedLevel;
        private int storedViewMode = -1;
        private uint targettingHandle;
        private int wantedLevel = 0;

        private IIntoxicants Intoxicants;
        private IModItems ModItems;
        private INameProvideable Names;
        private IPlacesOfInterest PlacesOfInterest;
        private IRadioStations RadioStations;
        private IScenarios Scenarios;
        private ISettingsProvideable Settings;
        private ISpeeches Speeches;
        private IEntityProvideable World;
        private IZones Zones;
        private IDances Dances;
        private IWeapons Weapons;
        private ITimeControllable TimeControllable;
        private ICrimes Crimes;
        private IGangTerritories GangTerritories;
        private IGameSaves GameSaves;
        private ISeats Seats;
        private IAgencies Agencies;
        private uint GameTimeLastRaiseHandsEmote;

        public Player(string modelName, bool isMale, string suspectsName, IEntityProvideable provider, ITimeControllable timeControllable, IStreets streets, IZones zones, ISettingsProvideable settings, IWeapons weapons, IRadioStations radioStations, IScenarios scenarios, ICrimes crimes
            , IAudioPlayable audio, IAudioPlayable secondaryAudio, IPlacesOfInterest placesOfInterest, IInteriors interiors, IModItems modItems, IIntoxicants intoxicants, IGangs gangs, IJurisdictions jurisdictions, IGangTerritories gangTerritories, IGameSaves gameSaves, INameProvideable names, IShopMenus shopMenus
            , IPedGroups pedGroups, IDances dances, ISpeeches speeches, ISeats seats, IAgencies agencies)
        {
            ModelName = modelName;
            IsMale = isMale;
            PlayerName = suspectsName;
            Crimes = crimes;
            World = provider;
            TimeControllable = timeControllable;
            Settings = settings;
            Weapons = weapons;
            RadioStations = radioStations;
            Scenarios = scenarios;
            GameTimeStartedPlaying = Game.GameTime;
            PlacesOfInterest = placesOfInterest;
            ModItems = modItems;
            Intoxicants = intoxicants;
            GangTerritories = gangTerritories;
            Zones = zones;
            GameSaves = gameSaves;
            Names = names;
            Seats = seats;
            Agencies = agencies;
            Scanner = new Scanner(provider, this, audio, secondaryAudio, Settings, TimeControllable, PlacesOfInterest);
            HealthState = new HealthState(new PedExt(Game.LocalPlayer.Character, Settings, Crimes, Weapons, PlayerName, "Person", World), Settings, true);
            if (CharacterModelIsFreeMode)
            {
                HealthState.MyPed.VoiceName = FreeModeVoice;
            }
            CurrentLocation = new LocationData(Game.LocalPlayer.Character, streets, zones, interiors);
            Surrendering = new SurrenderActivity(this, World, Settings);
            Violations = new Violations(this, TimeControllable, Crimes, Settings, Zones, GangTerritories, World);
            Investigation = new Investigation(this, Settings, provider);
            CriminalHistory = new CriminalHistory(this, Settings, TimeControllable);
            PoliceResponse = new PoliceResponse(this, Settings, TimeControllable, World);
            SearchMode = new SearchMode(this, Settings);
            Inventory = new Inventory(this, Settings, ModItems);
            Sprinting = new Sprinting(this, Settings);
            Intoxication = new Intoxication(this);
            Respawning = new Respawning(TimeControllable, World, this, Weapons, PlacesOfInterest, Settings, this, this);
            RelationshipManager = new RelationshipManager(gangs, Settings, PlacesOfInterest, TimeControllable, this, this);
            CellPhone = new CellPhone(this, this, jurisdictions, Settings, TimeControllable, gangs, PlacesOfInterest, Zones, streets, GangTerritories, Crimes, World, ModItems);
            PlayerTasks = new PlayerTasks(this, TimeControllable, gangs, PlacesOfInterest, Settings, World, Crimes, names, Weapons, shopMenus, ModItems, pedGroups);
            Licenses = new Licenses(this);
            Properties = new Properties(this, PlacesOfInterest, TimeControllable);
            ButtonPrompts = new ButtonPrompts(this, Settings);
            Injuries = new Injuries(this, Settings);
            Dances = dances;
            HumanState = new HumanState(this, TimeControllable, Settings);
            Speeches = speeches;
            Stance = new Stance(this, Settings);
            WeaponEquipment = new WeaponEquipment(this, this, Weapons, Settings, this, this, this);
            GPSManager = new GPSManager(this, World);
            VehicleOwnership = new VehicleOwnership(this,World);
            BankAccounts = new BankAccounts(this, Settings);
            ActivityManager = new ActivityManager(this,settings,this,this,this, this, this,TimeControllable,RadioStations,Crimes,ModItems,Dances,World,Intoxicants,this,Speeches,Seats,Weapons, PlacesOfInterest, Zones, shopMenus, gangs, gangTerritories);
            HealthManager = new HealthManager(this, Settings);
            GroupManager = new GroupManager(this, this, Settings, World, gangs, Weapons);
            MeleeManager = new MeleeManager(this, Settings);
            PlayerVoice = new PlayerVoice(this, Settings, Speeches);
        }
        public RelationshipManager RelationshipManager { get; private set; }
        public GPSManager GPSManager { get; private set; }
        public CriminalHistory CriminalHistory { get; private set; }
        public PlayerTasks PlayerTasks { get; private set; }
        public PoliceResponse PoliceResponse { get; private set; }
        public ButtonPrompts ButtonPrompts { get; private set; }
        public CellPhone CellPhone { get; private set; }
        public LocationData CurrentLocation { get; set; }
        public WeaponEquipment WeaponEquipment { get; private set; }
        public HumanState HumanState { get; set; }
        public HealthState HealthState { get; private set; }
        public Injuries Injuries { get; private set; }
        public Intoxication Intoxication { get; private set; }
        public Inventory Inventory { get; set; }
        public Investigation Investigation { get; private set; }
        public Licenses Licenses { get; private set; }
        public Properties Properties { get; private set; }
        public Sprinting Sprinting { get; private set; }
        public Stance Stance { get; private set; }
        public Violations Violations { get; private set; }
        public Respawning Respawning { get; private set; }
        public Scanner Scanner { get; private set; }
        public SearchMode SearchMode { get; private set; }
        public SurrenderActivity Surrendering { get; private set; }
        public VehicleOwnership VehicleOwnership { get; private set; }
        public BankAccounts BankAccounts { get; private set; }
        public ActivityManager ActivityManager { get; private set; }
        public HealthManager HealthManager { get; private set; }
        public GroupManager GroupManager { get; private set; }
        public MeleeManager MeleeManager { get; private set; }
        public PlayerVoice PlayerVoice { get; private set; }
        public float ActiveDistance => Investigation.IsActive ? Investigation.Distance : 500f + (WantedLevel * 200f);
        public bool AnyGangMemberCanHearPlayer { get; set; }
        public bool AnyGangMemberCanSeePlayer { get; set; }
        public bool AnyGangMemberRecentlySeenPlayer { get; set; }
        public bool AnyHumansNear => World.Pedestrians.LivingPeople.Any(x => x.DistanceToPlayer <= 10f);
        public bool AnyPoliceCanHearPlayer { get; set; }
        public bool AnyPoliceCanRecognizePlayer { get; set; }
        public bool AnyPoliceCanSeePlayer { get; set; }
        public bool AnyPoliceKnowInteriorLocation { get; set; }
        public bool AnyPoliceRecentlySeenPlayer { get; set; }
        public int AssignedSeat => -1;
        public VehicleExt AssignedVehicle => null;
        public List<Rage.Object> AttachedProp { get; set; } = new List<Rage.Object>();
        public bool BeingArrested { get; private set; }
        public List<uint> BlackListedVehicles => new List<uint>();
        public bool CanSitOnCurrentLookedAtObject { get; private set; }
        public int CellX { get; private set; }
        public int CellY { get; private set; }
        public Ped Character => Game.LocalPlayer.Character;
        public bool CharacterModelIsFreeMode => ModelName.ToLower() == "mp_f_freemode_01" || ModelName.ToLower() == "mp_m_freemode_01";
        public bool CharacterModelIsPrimaryCharacter => ModelName.ToLower() == "player_zero" || ModelName.ToLower() == "player_one" || ModelName.ToLower() == "player_two";
        public Cop ClosestCopToPlayer { get; set; }
        public Agency AssignedAgency { get; set; }
        public InteractableLocation ClosestInteractableLocation { get; private set; }
        public float ClosestPoliceDistanceToPlayer { get; set; }
        public Scenario ClosestScenario { get; private set; }
        public GangMember CurrentLookedAtGangMember { get; private set; }
        public Rage.Object CurrentLookedAtObject { get; private set; }
        public PedExt CurrentLookedAtPed { get; private set; }
        public VehicleExt CurrentLookedAtVehicle { get; private set; }
        public PedVariation CurrentModelVariation { get; set; }
        public VehicleExt CurrentSeenVehicle => CurrentVehicle ?? VehicleGettingInto;
        public PedExt CurrentTargetedPed { get; private set; }
        public VehicleExt CurrentVehicle { get; set; }
        public bool CurrentVehicleIsInAir { get; set; }
        public bool CurrentVehicleIsRolledOver { get; set; }
        public string DebugString { get; set; }
        public bool DiedInVehicle { get; private set; }
        public float FootSpeed { get; set; }
        public string FreeModeVoice { get; set; }//IsMale ? Settings.SettingsManager.PlayerOtherSettings.MaleFreeModeVoice : Settings.SettingsManager.PlayerOtherSettings.FemaleFreeModeVoice;
        public string Gender => IsMale ? "M" : "F";
        public int GroupID { get; set; }
        public uint Handle => Game.LocalPlayer.Character.Handle;
        public bool HasBeenMoving => GameTimeStartedMoving != 0 && Game.GameTime - GameTimeStartedMoving >= 5000;
        public bool HasBeenMovingFast => GameTimeStartedMovingFast != 0 && Game.GameTime - GameTimeStartedMovingFast >= 2000;
        public bool HasOnBodyArmor { get; private set; }
        public bool IsAiming
        {
            get => isAiming;
            private set
            {
                if (isAiming != value)
                {
                    isAiming = value;
                    OnAimingChanged();
                }
            }
        }
        public bool IsAimingInVehicle
        {
            get => isAimingInVehicle;
            private set
            {
                if (isAimingInVehicle != value)
                {
                    isAimingInVehicle = value;
                    OnAimingInVehicleChanged();
                }
            }
        }
        public bool IsAlive => !IsDead;
        public bool IsAliveAndFree => !IsBusted && !IsDead;
        public bool IsArrested { get; set; }
        public bool IsAttemptingToSurrender => Surrendering.HandsAreUp && !PoliceResponse.IsWeaponsFree;
        public bool IsBeingANuisance { get; set; }
        public bool IsBeingBooked { get; set; }
        public bool IsBreakingIntoCar => IsCarJacking || IsLockPicking || IsHotWiring || isJacking;
        public bool IsBustable => !Settings.SettingsManager.ViolationSettings.IsUnBustable && IsAliveAndFree && PoliceResponse.HasBeenWantedFor >= 3000 && !ActivityManager.IsCommitingSuicide && !ActivityManager.IsHoldingHostage && !RecentlyBusted && !RecentlyResistedArrest && !PoliceResponse.IsWeaponsFree && (IsIncapacitated || (!IsMoving && !IsMovingDynamically)) && (!IsInVehicle || WantedLevel == 1 || IsIncapacitated);
        public bool IsBusted { get; private set; }
        public bool IsCarJacking { get; set; }
        public bool IsChangingLicensePlates { get; set; }
        public bool IsCop { get; set; } = false;
        public bool IsCrouched { get; set; }
        public bool IsCustomizingPed { get; set; }
        public bool IsDangerouslyArmed => WeaponEquipment.IsDangerouslyArmed;
        public bool IsDead { get; private set; }
        public bool IsDealingDrugs { get; set; } = false;
        public bool IsDealingIllegalGuns { get; set; } = false;
        public bool IsDisplayingCustomMenus => IsTransacting || IsCustomizingPed || ActivityManager.IsConversing;
        public bool IsDoingSuspiciousActivity { get; set; } = false;
        public bool IsDriver { get; private set; }
        public bool IsDuckingInVehicle { get; set; } = false;
        public bool IsGangMember => RelationshipManager.GangRelationships.CurrentGang != null;
        public bool IsGettingIntoAVehicle
        {
            get => isGettingIntoVehicle;
            private set
            {
                if (isGettingIntoVehicle != value)
                {
                    isGettingIntoVehicle = value;
                    OnGettingIntoAVehicleChanged();
                }
            }
        }
        public bool IsHotWiring { get; private set; }
        public bool IsInAirVehicle { get; private set; }
        public bool IsInAutomobile { get; private set; }
        public bool IsOnBicycle { get; private set; }
        public bool IsIncapacitated => IsStunned || IsRagdoll;
        public bool IsInCover { get; private set; }
        public bool IsInFirstPerson { get; private set; }
        public bool IsInSearchMode { get; set; }
        public bool IsInVehicle
        {
            get => isInVehicle;
            private set
            {
                if (isInVehicle != value)
                {
                    isInVehicle = value;
                    OnIsInVehicleChanged();
                }
            }
        }
        public bool IsLockPicking { get; set; }
        public bool IsMakingInsultingGesture { get; set; }
        public bool IsMale { get; set; }
        public bool IsMobileRadioEnabled { get; private set; }
        public bool IsMoveControlPressed { get; set; }
        public bool IsMoving => GameTimeLastMoved != 0 && Game.GameTime - GameTimeLastMoved <= 2000;
        public bool IsMovingDynamically { get; private set; }
        public bool IsMovingFast => GameTimeLastMovedFast != 0 && Game.GameTime - GameTimeLastMovedFast <= 2000;
        public bool IsNearScenario { get; private set; }
        public bool IsNotHoldingEnter { get; set; }
        public bool IsNotWanted => wantedLevel == 0;
        public bool IsOnFoot => !IsInVehicle;
        public bool IsOnMotorcycle { get; private set; }
        public bool IsOnMuscleRelaxants { get; set; }
        public bool IsPressingFireWeapon { get; set; }
        public bool IsRagdoll { get; private set; }
        public bool IsResting { get; set; } = false;
        public bool IsRidingBus { get; set; }
        public bool IsShooting
        {
            get => isShooting;
            private set
            {
                if (isShooting != value)
                {
                    isShooting = value;
                    OnIsShootingChanged();
                }
            }
        }
        public bool IsSleeping { get; set; } = false;
        public bool IsSleepingOutside { get; set; } = false;
        public bool IsStill { get; private set; }
        public bool IsStunned { get; private set; }
        public bool IsTransacting { get; set; }
        public bool IsVisiblyArmed { get; set; }
        public bool IsWanted => wantedLevel > 0;
        public Vehicle LastFriendlyVehicle { get; set; }
        public int LastSeatIndex => -1;
        public string ModelName { get; set; }
        public Ped Pedestrian => Game.LocalPlayer.Character;
        public Vector3 PlacePoliceLastSeenPlayer { get; set; }
        public Vector3 PlacePoliceShouldSearchForPlayer { get; set; }
        public string PlayerName { get; set; }
        public Vector3 Position => position;
        public VehicleExt PreviousVehicle { get; private set; }
        public bool RecentlyBribedPolice => Respawning.RecentlyBribedPolice;
        public bool RecentlyBusted => GameTimeLastBusted != 0 && Game.GameTime - GameTimeLastBusted <= 5000;
        public bool RecentlyCrashedVehicle => GameTimeLastCrashedVehicle != 0 && Game.GameTime - GameTimeLastCrashedVehicle <= 5000;
        public bool RecentlyFedUpCop => GameTimeLastFedUpCop != 0 && Game.GameTime - GameTimeLastFedUpCop <= 5000;
        public bool RecentlyGotOutOfVehicle => GameTimeGotOutOfVehicle != 0 && Game.GameTime - GameTimeGotOutOfVehicle <= 1000;
        public bool RecentlyPaidFine => Respawning.RecentlyPaidFine;
        public bool RecentlyResistedArrest => Respawning.RecentlyResistedArrest;
        public bool RecentlyRespawned => Respawning.RecentlyRespawned;
        public bool RecentlySetWanted => GameTimeLastSetWanted != 0 && Game.GameTime - GameTimeLastSetWanted <= 5000;
        public bool RecentlyShot => GameTimeLastShot != 0 && !RecentlyStartedPlaying && Game.GameTime - GameTimeLastShot <= 3000;
        public bool RecentlyStartedPlaying => GameTimeStartedPlaying != 0 && Game.GameTime - GameTimeStartedPlaying <= 3000;
        public bool ReleasedFireWeapon { get; set; }
        public List<VehicleExt> ReportedStolenVehicles => TrackedVehicles.Where(x => x.NeedsToBeReportedStolen && !x.HasBeenDescribedByDispatch && !x.AddedToReportedStolenQueue).ToList();
        public float SearchModePercentage => SearchMode.SearchModePercentage;
        public bool ShouldCheckViolations => !Settings.SettingsManager.ViolationSettings.TreatAsCop && !IsCop && !RecentlyStartedPlaying;
        public int SpeechSkill { get; set; }
        public uint TargettingHandle
        {
            get => targettingHandle;
            private set
            {
                if (targettingHandle != value)
                {
                    targettingHandle = value;
                    OnTargettingHandleChanged();
                }
            }
        }
        public uint TimeInCurrentVehicle => GameTimeGotInVehicle == 0 || !IsInVehicle ? 0 : Game.GameTime - GameTimeGotInVehicle;
        public uint TimeInSearchMode => SearchMode.TimeInSearchMode;
        public uint TimeToRecognize
        {
            get
            {
                uint Time = Settings.SettingsManager.PlayerOtherSettings.Recognize_BaseTime;
                if (TimeControllable.IsNight)
                {
                    Time += Settings.SettingsManager.PlayerOtherSettings.Recognize_NightPenalty;
                }
                else if (IsInVehicle)
                {
                    Time += Settings.SettingsManager.PlayerOtherSettings.Recognize_VehiclePenalty;
                    if (NativeFunction.Natives.GET_PED_CONFIG_FLAG<bool>(Character, 359, true))//isduckinginvehicle?
                    {
                        Time += 5000;
                    }
                }
                return Time;
            }
        }
        public List<VehicleExt> TrackedVehicles { get; private set; } = new List<VehicleExt>();
        public VehicleExt VehicleGettingInto { get; private set; }
        public float VehicleSpeed { get; private set; }
        public float VehicleSpeedKMH => VehicleSpeed * 3.6f;
        public float VehicleSpeedMPH => VehicleSpeed * 2.23694f;
        public bool VeryRecentlyShot => GameTimeLastShot != 0 && Game.GameTime - GameTimeLastShot <= 500;
        public int WantedLevel => wantedLevel;
        public bool WasDangerouslyArmedWhenBusted { get; private set; }
        public bool IsUsingController { get; set; }
        public bool IsShowingActionWheel { get; set; }
        public bool IsInPoliceVehicle { get; private set; }

        //Required
        public void Setup()
        {
            Violations.Setup();
            Respawning.Setup();
            Scanner.Setup();
            RelationshipManager.Setup();
            CellPhone.Setup();
            PlayerTasks.Setup();
            Properties.Setup();
            ButtonPrompts.Setup();
            HumanState.Setup();
            GPSManager.Setup();
            SetWantedLevel(0, "Initial", true);
            NativeFunction.CallByName<bool>("SET_MAX_WANTED_LEVEL", 0);
            WeaponEquipment.SetUnarmed();
            VehicleOwnership.Setup();
            BankAccounts.Setup();
            HealthManager.Setup();
            GroupManager.Setup();
            MeleeManager.Setup();
            PlayerVoice.Setup();
            ActivityManager.Setup();
            ModelName = Game.LocalPlayer.Character.Model.Name;
            CurrentModelVariation = NativeHelper.GetPedVariation(Game.LocalPlayer.Character);
            FreeModeVoice = Game.LocalPlayer.Character.IsMale ? Settings.SettingsManager.PlayerOtherSettings.MaleFreeModeVoice : Settings.SettingsManager.PlayerOtherSettings.FemaleFreeModeVoice;
            if (Game.LocalPlayer.Character.IsInAnyVehicle(false) && Game.LocalPlayer.Character.CurrentVehicle.Exists())
            {
                UpdateCurrentVehicle();
                VehicleOwnership.TakeOwnershipOfVehicle(CurrentVehicle, false);
            }
            if (Settings.SettingsManager.VehicleSettings.DisableAutoEngineStart)
            {
                NativeFunction.Natives.SET_PED_CONFIG_FLAG<bool>(Game.LocalPlayer.Character, (int)PedConfigFlags._PED_FLAG_DISABLE_STARTING_VEH_ENGINE, true);
            }
            if (Settings.SettingsManager.VehicleSettings.DisableAutoHelmet)
            {
                NativeFunction.Natives.SET_PED_CONFIG_FLAG<bool>(Game.LocalPlayer.Character, (int)PedConfigFlags._PED_FLAG_PUT_ON_MOTORCYCLE_HELMET, false);
            }
            if (Settings.SettingsManager.PlayerOtherSettings.DisableVanillaGangHassling)
            {
                NativeFunction.Natives.SET_PLAYER_CAN_BE_HASSLED_BY_GANGS(Game.LocalPlayer, false);
            }
            if (Settings.SettingsManager.PlayerOtherSettings.AllowAttackingFriendlyPeds)
            {
                NativeFunction.Natives.SET_CAN_ATTACK_FRIENDLY(Character, true, false);
            }
            WeaponEquipment.Setup();
            GameFiber.StartNew(delegate
            {
                try
                {
                    while (isActive)
                    {
                        CellPhone.Update();
                        GameFiber.Yield();
                    }
                }
                catch (Exception ex)
                {
                    EntryPoint.WriteToConsole(ex.Message + " " + ex.StackTrace, 0);
                    EntryPoint.ModController.CrashUnload();
                }
            }, "CellPhone");
            if (Settings.SettingsManager.CellphoneSettings.TerminateVanillaCellphone)
            {
                NativeFunction.Natives.TERMINATE_ALL_SCRIPTS_WITH_THIS_NAME("cellphone_flashhand");
                NativeFunction.Natives.TERMINATE_ALL_SCRIPTS_WITH_THIS_NAME("cellphone_controller");
            }
            SpeechSkill = RandomItems.GetRandomNumberInt(Settings.SettingsManager.PlayerOtherSettings.PlayerSpeechSkill_Min, Settings.SettingsManager.PlayerOtherSettings.PlayerSpeechSkill_Max);
        }
        public void Update()
        {
            UpdateVehicleData();
            GameFiber.Yield();
            UpdateWeaponData();
            GameFiber.Yield();
            UpdateStateData();
            GameFiber.Yield();
            bool IntoxicationIsPrimary = false;
            if (Intoxication.CurrentIntensity > Injuries.CurrentIntensity)
            {
                IntoxicationIsPrimary = true;
            }
            Intoxication.Update(IntoxicationIsPrimary);
            GameFiber.Yield();//TR Yield RemovedTest 1
            Injuries.Update(!IntoxicationIsPrimary);
            GameFiber.Yield();//TR Yield RemovedTest 1
            HumanState.Update();
            BankAccounts.Update();
            HealthManager.Update();
            GroupManager.Update();
            GameFiber.Yield();//TR Yield RemovedTest 1
            ButtonPrompts.Update();
            MeleeManager.Update();
            GameFiber.Yield();//TR Yield RemovedTest 1
            PlayerVoice.Update();

            ActivityManager.Update();


        }
        public void SetNotBusted()
        {
            BeingArrested = false;
            IsBusted = false;
        }
        public void Reset(bool resetWanted, bool resetTimesDied, bool resetWeapons, bool resetCriminalHistory, bool resetInventory, bool resetIntoxication, bool resetRelationships, bool resetOwnedVehicles, bool resetCellphone, bool resetActiveTasks, bool resetProperties, 
            bool resetHealth, bool resetNeeds, bool resetGroup, bool resetLicenses, bool resetActivites, bool resetGracePeriod)
        {
            IsDead = false;
            IsBusted = false;
            IsArrested = false;
            IsBeingBooked = false;
            Game.LocalPlayer.HasControl = true;
            BeingArrested = false;
            HealthState.Reset();
            if (resetActivites)
            {
                ActivityManager.Reset();
            }
            CurrentVehicle = null;
            GPSManager.Reset();
            NativeFunction.Natives.SET_MOBILE_RADIO_ENABLED_DURING_GAMEPLAY(false);
            IsMobileRadioEnabled = false;


            if(resetGracePeriod)
            {
                PoliceResponse.ResetGracePeriod();
            }
            else
            {
                PoliceResponse.AddToGracePeriod();
            }

            if (resetWanted)
            {
                GameTimeStartedPlaying = Game.GameTime;
                PoliceResponse.Reset();
                Investigation.Reset();
                Violations.Reset();            
                Scanner.Reset();
                Update();
            }
            if (resetTimesDied)
            {
                Respawning.Reset();
            }
            if (resetWeapons)
            {
                WeaponEquipment.Reset();           
            }
            if (resetCriminalHistory)
            {
                CriminalHistory.Reset();
            }
            if (resetInventory)
            {
                Inventory.Reset();
            }
            if (resetIntoxication)
            {
                Intoxication.Reset();
            }
            if (resetRelationships)
            {
                RelationshipManager.Reset(false);
            }
            if (resetOwnedVehicles)
            {
                VehicleOwnership.Reset();
            }
            if (resetCellphone)
            {
                CellPhone.Reset();
            }
            if (resetActiveTasks)
            {
                PlayerTasks.Reset();
            }
            if (resetProperties)
            {
                Properties.Reset();
            }
            if (resetHealth)
            {
                Injuries.Reset();
            }
            if (resetNeeds)
            {
                HumanState.Reset();
            }
            if(resetGroup)
            {
                GroupManager.Reset();
            }
            if(resetLicenses)
            {
                Licenses.Reset();
            }
            if (Settings.SettingsManager.VehicleSettings.DisableAutoEngineStart)
            {
                NativeFunction.Natives.SET_PED_CONFIG_FLAG<bool>(Game.LocalPlayer.Character, (int)PedConfigFlags._PED_FLAG_DISABLE_STARTING_VEH_ENGINE, true);
            }
            if (Settings.SettingsManager.VehicleSettings.DisableAutoHelmet)
            {
                NativeFunction.Natives.SET_PED_CONFIG_FLAG<bool>(Game.LocalPlayer.Character, (int)PedConfigFlags._PED_FLAG_PUT_ON_MOTORCYCLE_HELMET, false);
            }
            if (Settings.SettingsManager.PlayerOtherSettings.AllowAttackingFriendlyPeds)
            {
                NativeFunction.Natives.SET_CAN_ATTACK_FRIENDLY(Game.LocalPlayer.Character, true, false);
            }
        }
        public void Dispose()
        {
            isActive = false;
            Scanner.Dispose();
            Investigation.Dispose(); //remove blip
            CriminalHistory.Dispose(); //remove blip
            PoliceResponse.Dispose(); //same ^
            //Interaction?.Dispose();
            SearchMode.Dispose();
            RelationshipManager.Dispose();
            CellPhone.Dispose();
            PlayerTasks.Dispose();
            Properties.Dispose();
            ButtonPrompts.Dispose();
            Intoxication.Dispose();
            Injuries.Dispose();
            HumanState.Dispose();
            WeaponEquipment.Dispose();
            GPSManager.Dispose();
            VehicleOwnership.Dispose();
            BankAccounts.Dispose();
            HealthManager.Dispose();
            GroupManager.Dispose();
            MeleeManager.Dispose();
            Violations.Dispose();
            PlayerVoice.Dispose();
            ActivityManager.Dispose();

            NativeFunction.Natives.SET_PED_RESET_FLAG(Game.LocalPlayer.Character, 186, true);

            NativeFunction.Natives.SET_PED_CONFIG_FLAG<bool>(Game.LocalPlayer.Character, (int)PedConfigFlags._PED_FLAG_PUT_ON_MOTORCYCLE_HELMET, true);
            NativeFunction.Natives.SET_PED_CONFIG_FLAG<bool>(Game.LocalPlayer.Character, (int)PedConfigFlags._PED_FLAG_DISABLE_STARTING_VEH_ENGINE, false);
            NativeFunction.Natives.SET_PED_IS_DRUNK<bool>(Game.LocalPlayer.Character, false);
            NativeFunction.Natives.RESET_PED_MOVEMENT_CLIPSET<bool>(Game.LocalPlayer.Character);
            NativeFunction.Natives.SET_PED_CONFIG_FLAG<bool>(Game.LocalPlayer.Character, (int)PedConfigFlags.PED_FLAG_DRUNK, false);
            if (Settings.SettingsManager.UIGeneralSettings.AllowScreenEffectReset)//this should be moved methinks
            {
                NativeFunction.Natives.CLEAR_TIMECYCLE_MODIFIER<int>();
                NativeFunction.Natives.x80C8B1846639BB19(0);
                NativeFunction.Natives.STOP_GAMEPLAY_CAM_SHAKING<int>(true);
            }
            Game.LocalPlayer.WantedLevel = 0;
            NativeFunction.Natives.SET_FAKE_WANTED_LEVEL(0);
            NativeFunction.Natives.SET_MAX_WANTED_LEVEL(6);
            NativeFunction.Natives.SET_PED_AS_COP(Game.LocalPlayer.Character, false);      
            if (Settings.SettingsManager.PlayerOtherSettings.SetSlowMoOnDeath)
            {
                Game.TimeScale = 1f;
            }
            NativeFunction.Natives.ENABLE_ALL_CONTROL_ACTIONS(0);//enable all controls in case we left some disabled
            NativeFunction.Natives.SET_CAN_ATTACK_FRIENDLY(Character, false, false);
            NativeFunction.Natives.SET_PLAYER_CAN_BE_HASSLED_BY_GANGS(Game.LocalPlayer, true);

            NativeFunction.Natives.DESTROY_ALL_CAMS(0);
            NativeFunction.Natives.CLEAR_FOCUS();
            Game.LocalPlayer.HasControl = true;
            if(Game.IsScreenFadedOut || Game.IsScreenFadingOut)
            {
                Game.FadeScreenIn(0, false);
            }
        }
        //Needed
        public void ChangeName(string newName)
        {
            PlayerName = newName;
            EntryPoint.WriteToConsole($"PLAYER EVENT: ChangeName {newName}", 3);
        }
        public void DisplayPlayerNotification()
        {
            string NotifcationText = "Warrants: ~g~None~s~";
            if (PoliceResponse.HasObservedCrimes)
            {
                NotifcationText = "Wanted For:" + PoliceResponse.PrintCrimes(true);
            }
            else if (CriminalHistory.HasHistory)
            {
                NotifcationText = "Wanted For:" + CriminalHistory.PrintCriminalHistory();
            }
            Game.DisplayNotification("CHAR_BLANK_ENTRY", "CHAR_BLANK_ENTRY", "~b~Personal Info", $"~y~{PlayerName}", NotifcationText);
            // DisplayPlayerVehicleNotification();
        }
        public void SetDemographics(string modelName, bool isMale, string playerName, int money, int speechSkill, string voiceName)
        {
            ModelName = modelName;
            PlayerName = playerName;
            IsMale = isMale;
            BankAccounts.SetMoney(money);
            SpeechSkill = speechSkill;// 
            if (voiceName == "")
            {
                FreeModeVoice = IsMale ? Settings.SettingsManager.PlayerOtherSettings.MaleFreeModeVoice : Settings.SettingsManager.PlayerOtherSettings.FemaleFreeModeVoice;
            }
            else
            {
                FreeModeVoice = voiceName;
            }
            EntryPoint.WriteToConsole($"PLAYER EVENT: SetDemographics MoneyToSet {money} Current: {BankAccounts.Money} {NativeHelper.CashHash(Settings.SettingsManager.PedSwapSettings.MainCharacterToAlias)}", 3);
        }
        public void LocationUpdate()
        {
            CurrentLocation.Update(Character);
            if (CurrentLocation.HasBeenOnHighway && !HasThrownGotOnFreeway)
            {
                OnGotOnFreeway();
                HasThrownGotOnFreeway = true;
                HasThrownGotOffFreeway = false;
            }
            else if (CurrentLocation.HasBeenOffHighway && !HasThrownGotOffFreeway && HasThrownGotOnFreeway)
            {
                OnGotOffFreeway();
                HasThrownGotOnFreeway = false;
                HasThrownGotOffFreeway = true;
            }
        }
        public void PlaySpeech(string speechName, bool useMegaphone)
        {
            if (CharacterModelIsFreeMode && FreeModeVoice != "")
            {
                if (useMegaphone)
                {
                    Character.PlayAmbientSpeech(FreeModeVoice, speechName, 0, SpeechModifier.ForceMegaphone);

                }
                else
                {
                    Character.PlayAmbientSpeech(FreeModeVoice, speechName, 0, SpeechModifier.Force);
                }
                EntryPoint.WriteToConsole($"FREEMODE COP SPEAK {Character.Handle} freeModeVoice {FreeModeVoice} speechName {speechName}");
            }
            else
            {
                Character.PlayAmbientSpeech(speechName, useMegaphone);
            }
        }
        //Maybe?
        public int FineAmount()
        {
            int InitialAmount = Settings.SettingsManager.PoliceSettings.GeneralFineAmount;
            if (PoliceResponse.PlayerSeenInVehicleDuringWanted)
            {
                if (!Licenses.HasValidDriversLicense(TimeControllable))
                {
                    InitialAmount += Settings.SettingsManager.PoliceSettings.DrivingWithoutLicenseFineAmount;
                }
            }
            if(Respawning.TimesTalked > 0)
            {
                InitialAmount += Settings.SettingsManager.PoliceSettings.TalkFailFineAmount;
            }
            return InitialAmount;
        }
        public void SetDenStatus(Gang gang, bool isEnabled)
        {
           // EntryPoint.WriteToConsole($"SET DEN {gang.ShortName} {isEnabled}");
            World.Places.StaticPlaces.SetGangLocationActive(gang.ID, isEnabled);
        }
        public void SetAngeredCop()
        {
            GameTimeLastFedUpCop = Game.GameTime;
        }
        public void SetBodyArmor(int Type)
        {
            if (CharacterModelIsFreeMode)
            {
                int NumberOfTextureVariations = NativeFunction.Natives.GET_NUMBER_OF_PED_TEXTURE_VARIATIONS<int>(Character, 9, Type) - 1;
                int TextureID = 0;

                if (NumberOfTextureVariations > 0)
                {
                    RandomItems.GetRandomNumberInt(0, NumberOfTextureVariations);
                }
                NativeFunction.Natives.SET_PED_COMPONENT_VARIATION<bool>(Character, 9, Type, TextureID, 0);
                if (!HasOnBodyArmor)
                {
                    Character.Armor = 200;
                }
                HasOnBodyArmor = true;
            }
        }
        public void ShootAt(Vector3 TargetCoordinate)
        {
            NativeFunction.CallByName<bool>("SET_PED_SHOOTS_AT_COORD", Game.LocalPlayer.Character, TargetCoordinate.X, TargetCoordinate.Y, TargetCoordinate.Z, true);
            GameTimeLastShot = Game.GameTime;
        }
        public void SetShot()
        {
            GameTimeLastShot = Game.GameTime;
        }
        public void ToggleBodyArmor(int Type)
        {
            if (CharacterModelIsFreeMode)
            {
                if (HasOnBodyArmor)
                {
                    NativeFunction.Natives.SET_PED_COMPONENT_VARIATION<bool>(Character, 9, 0, 0, 0);
                    HasOnBodyArmor = false;
                    Character.Armor = 0;
                }
                else
                {
                    int NumberOfTextureVariations = NativeFunction.Natives.GET_NUMBER_OF_PED_TEXTURE_VARIATIONS<int>(Character, 9, Type) - 1;
                    int TextureID = 0;

                    if (NumberOfTextureVariations > 0)
                    {
                        RandomItems.GetRandomNumberInt(0, NumberOfTextureVariations);
                    }
                    NativeFunction.Natives.SET_PED_COMPONENT_VARIATION<bool>(Character, 9, Type, TextureID, 0);
                    HasOnBodyArmor = true;
                    Character.Armor = 200;
                }
            }
        }
        public void SetCopStatus(bool isCop, Agency toassign)
        {
            if(isCop)
            {
                if(toassign == null)
                {
                    toassign = Agencies.GetAgency("LSPD");
                }
                Cop meAsCop = new Cop(Character, Settings, Character.MaxHealth, toassign, true, Crimes, Weapons, PlayerName,ModelName,World);
                meAsCop.CanBeTasked = false;
                meAsCop.CanBeAmbientTasked = false;
                World.Pedestrians.AddEntity(meAsCop);
                AssignedAgency = toassign;
            }
            else
            {
                AssignedAgency = null;
                World.Pedestrians.PoliceList.RemoveAll(x => x.Handle == Handle);
            }
            IsCop = isCop;
        }
        public void ToggleCopTaskable()
        {
            Cop meCop = World.Pedestrians.Police.FirstOrDefault(x => x.Handle == Handle);
            if(meCop != null)
            {
                meCop.CanBeAmbientTasked = !meCop.CanBeAmbientTasked;
                meCop.CanBeTasked = !meCop.CanBeTasked;
                Game.DisplaySubtitle($"Player Cop Taskable: {meCop.CanBeTasked}");
               EntryPoint.WriteToConsole($"Player Cop Taskable: {meCop.CanBeTasked}");
            }
        }
        //Events
        public void OnAppliedWantedStats(int wantedLevel) => Scanner.OnAppliedWantedStats(wantedLevel);
        public void OnGotOffFreeway()
        {
            GameFiber.Yield();
            if (IsWanted && AnyPoliceCanSeePlayer && TimeInCurrentVehicle >= 10000 && IsAliveAndFree)
            {
                Scanner.OnGotOffFreeway();
            }
            EntryPoint.WriteToConsole($"PLAYER EVENT: OnGotOffFreeway (5 Second Delay)", 3);
        }
        public void OnGotOnFreeway()
        {
            GameFiber.Yield();
            if (IsWanted && AnyPoliceCanSeePlayer && TimeInCurrentVehicle >= 10000 && IsAliveAndFree)
            {
                Scanner.OnGotOnFreeway();
            }
            EntryPoint.WriteToConsole($"PLAYER EVENT: OnGotOnFreeway (5 Second Delay)", 3);
        }
        public void OnInvestigationExpire()
        {
            GameFiber.Yield();
            PoliceResponse.Reset();
            Scanner.OnInvestigationExpire();
            EntryPoint.WriteToConsole($"PLAYER EVENT: OnInvestigationExpire", 3);
        }
        public void OnKilledCop()
        {
            PlayerVoice.OnKilledCop();
        }
        public void OnKilledCivilian()
        {
            PlayerVoice.OnKilledCivilian();
        }
        public void OnLawEnforcementSpawn(Agency agency, DispatchableVehicle vehicleType, DispatchablePerson officerType)
        {
            GameFiber.Yield();
            if (IsWanted)
            {
                if (agency?.ID == "ARMY")
                {
                    Scanner.OnArmyDeployed();
                }
                else if (agency?.ID == "NOOSE")
                {
                    Scanner.OnNooseDeployed();
                }
                else if (agency?.ID == "FIB" && WantedLevel >= 4)
                {
                    Scanner.OnFIBHRTDeployed();
                }
                else if (vehicleType?.IsHelicopter == true)
                {
                    Scanner.OnHelicoptersDeployed();
                }
            }
        }
        public void OnLethalForceAuthorized() => Scanner.OnLethalForceAuthorized();
        public void OnPoliceNoticeVehicleChange() => Scanner.OnPoliceNoticeVehicleChange();
        public void OnRequestedBackUp() => Scanner.OnRequestedBackUp();
        public void OnSuspectEluded()//runs before OnWantedLevelChanged
        {
            GameFiber.Yield();
            if (WantedLevel > 1)
            {
                CriminalHistory.OnSuspectEluded(PoliceResponse.CrimesObserved.Select(x => x.AssociatedCrime).ToList(), PlacePoliceLastSeenPlayer);
                Scanner.OnSuspectEluded();
            }
            PlayerVoice.OnSuspectEluded();
        }
        public void OnVehicleCrashed()
        {
            GameFiber.Yield();
            if (IsWanted && AnyPoliceRecentlySeenPlayer && IsInVehicle && TimeInCurrentVehicle >= 5000 && IsAliveAndFree)
            {
                GameTimeLastCrashedVehicle = Game.GameTime;
                Scanner.OnVehicleCrashed();
            }

            PlayerVoice.OnCrashedCar();

            EntryPoint.WriteToConsole($"PLAYER EVENT: OnVehicleCrashed", 5);
        }
        public void OnVehicleEngineHealthDecreased(float amount, bool isCollision)
        {
            GameFiber.Yield();
            if (isCollision && IsWanted && AnyPoliceRecentlySeenPlayer && IsInVehicle && amount >= 50f && TimeInCurrentVehicle >= 5000)
            {
                GameTimeLastCrashedVehicle = Game.GameTime;
                Scanner.OnVehicleCrashed();
            }
            EntryPoint.WriteToConsole($"PLAYER EVENT: OnVehicleEngineHealthDecreased {amount} {isCollision}", 5);
        }
        public void OnVehicleHealthDecreased(int amount, bool isCollision)
        {
            GameFiber.Yield();
            if (isCollision && IsWanted && AnyPoliceRecentlySeenPlayer && IsInVehicle && amount >= 50 && TimeInCurrentVehicle >= 5000)
            {
                GameTimeLastCrashedVehicle = Game.GameTime;
                Scanner.OnVehicleCrashed();
            }
            EntryPoint.WriteToConsole($"PLAYER EVENT: OnVehicleHealthDecreased {amount} {isCollision}", 5);
        }
        public void OnVehicleStartedFire()
        {
            GameFiber.Yield();
            if (IsWanted && AnyPoliceRecentlySeenPlayer && IsInVehicle && TimeInCurrentVehicle >= 5000 && IsAliveAndFree)
            {
                Scanner.OnVehicleStartedFire();
            }
            //EntryPoint.WriteToConsole($"PLAYER EVENT: OnVehicleStartedFire", 5);
        }
        public void OnWantedActiveMode()
        {
            Scanner.OnWantedActiveMode();
            PlayerVoice.OnWantedActiveMode();
        }
        public void OnWantedSearchMode()
        {
            Scanner.OnWantedSearchMode();
            PlayerVoice.OnWantedSearchMode();
        }
        public void OnWeaponsFree() => Scanner.OnWeaponsFree();
        private void OnAimingChanged()
        {
            if (IsAiming)
            {
            }
            else
            {
            }
            //EntryPoint.WriteToConsole($"PLAYER EVENT: IsAiming Changed to: {IsAiming}", 5);
        }
        private void OnAimingInVehicleChanged()
        {
            if (IsAimingInVehicle)
            {
                if (WeaponEquipment.CurrentWeapon == null)
                {
                    IsMakingInsultingGesture = true;
                }
                else
                {
                    EntryPoint.WriteToConsole($"CurrentWeapon {WeaponEquipment.CurrentWeapon.ModelName}", 5);
                }
            }
            else
            {
                IsMakingInsultingGesture = false;
            }
            //EntryPoint.WriteToConsole($"PLAYER EVENT: IsAimingInVehicle Changed to: {IsAimingInVehicle}", 5);
        }
        private void OnExcessiveSpeed()
        {
            GameFiber.Yield();
            if (IsWanted && VehicleSpeedMPH >= 75f && AnyPoliceCanSeePlayer && TimeInCurrentVehicle >= 10000 && !isCheckingExcessSpeed && IsAliveAndFree)
            {
                GameFiber SpeedWatcher = GameFiber.StartNew(delegate
                {
                    try
                    {
                        isCheckingExcessSpeed = true;
                        GameFiber.Sleep(5000);
                        if (isExcessiveSpeed)
                        {
                            Scanner.OnExcessiveSpeed();
                        }
                        isCheckingExcessSpeed = false;
                    }
                    catch (Exception ex)
                    {
                        EntryPoint.WriteToConsole(ex.Message + " " + ex.StackTrace, 0);
                        EntryPoint.ModController.CrashUnload();
                    }
                }, "FastForwardWatcher");
            }
            EntryPoint.WriteToConsole($"PLAYER EVENT: OnExcessiveSpeed", 3);
        }
        private void OnGettingIntoAVehicleChanged()
        {
            //GameFiber.Yield();//TR Yield RemovedTest 2
            if (IsGettingIntoAVehicle)
            {
                Vehicle VehicleTryingToEnter = Game.LocalPlayer.Character.VehicleTryingToEnter;
                int SeatTryingToEnter = Game.LocalPlayer.Character.SeatIndexTryingToEnter;
                if (VehicleTryingToEnter == null)
                {
                    return;
                }
                UpdateCurrentVehicle();
                //GameFiber.Yield();//TR Yield RemovedTest 2
                if (CurrentVehicle != null)
                {
                    Blip attachedBlip = CurrentVehicle.Vehicle.GetAttachedBlip();
                    VehicleGettingInto = CurrentVehicle;
                    if (VehicleOwnership.OwnedVehicles.Any(x => x.Handle == CurrentVehicle.Handle) && CurrentVehicle.Vehicle.Exists())//if (OwnedVehicle != null && CurrentVehicle.Handle == OwnedVehicle.Handle && CurrentVehicle.Vehicle.Exists())
                    {
                        CurrentVehicle.Vehicle.LockStatus = (VehicleLockStatus)1;
                        CurrentVehicle.Vehicle.MustBeHotwired = false;
                    }
                    else if (CurrentVehicle.Vehicle.Exists() && CurrentVehicle.Vehicle.IsPersistent && CurrentVehicle.Vehicle.GetAttachedBlip()?.Sprite == BlipSprite.GangVehicle)//vanilla owned vehicles?
                    {
                        CurrentVehicle.Vehicle.LockStatus = (VehicleLockStatus)1;
                        CurrentVehicle.Vehicle.MustBeHotwired = false;
                    }
                    else if (CurrentVehicle.Vehicle.Exists() && LastFriendlyVehicle.Exists() && CurrentVehicle.Vehicle.Handle == LastFriendlyVehicle.Handle)//vanilla owned vehicles?
                    {
                        CurrentVehicle.Vehicle.LockStatus = (VehicleLockStatus)1;
                        CurrentVehicle.Vehicle.MustBeHotwired = false;
                    }
                    else
                    {
                        if (CurrentVehicle.Vehicle.Exists() && CurrentVehicle.Vehicle.IsPersistent && !Settings.SettingsManager.VehicleSettings.AllowLockMissionVehicles)//vanilla owned vehicles?
                        {
                            CurrentVehicle.Vehicle.LockStatus = (VehicleLockStatus)1;
                            CurrentVehicle.Vehicle.MustBeHotwired = false;
                        }
                        if (!CurrentVehicle.HasBeenEnteredByPlayer && !IsCop)
                        {
                            CurrentVehicle.AttemptToLock();
                            //GameFiber.Yield();//TR Yield RemovedTest 2
                        }
                        bool hasScrewDriver = Inventory.Has(typeof(ScrewdriverItem)); //Inventory.HasTool(ToolTypes.Screwdriver);
                        if (Settings.SettingsManager.VehicleSettings.RequireScrewdriverForHotwire)
                        {
                            if (CurrentVehicle.Vehicle.MustBeHotwired)
                            {
                                CurrentVehicle.IsHotWireLocked = true;
                                CurrentVehicle.Vehicle.MustBeHotwired = false;
                            }
                            if (CurrentVehicle.IsHotWireLocked && hasScrewDriver)
                            {
                                CurrentVehicle.IsHotWireLocked = false;
                                CurrentVehicle.Vehicle.MustBeHotwired = true;
                            }
                        }

                        if (Settings.SettingsManager.VehicleSettings.RequireScrewdriverForLockPickEntry && !hasScrewDriver && IsNotHoldingEnter && VehicleTryingToEnter.Driver == null && VehicleTryingToEnter.LockStatus == (VehicleLockStatus)7 && !VehicleTryingToEnter.IsEngineOn)
                        {
                            Game.DisplayHelp("Screwdriver required to lockpick");
                        }


                        if (IsNotHoldingEnter && VehicleTryingToEnter.Driver == null && VehicleTryingToEnter.LockStatus == (VehicleLockStatus)7 && !VehicleTryingToEnter.IsEngineOn && (!Settings.SettingsManager.VehicleSettings.RequireScrewdriverForLockPickEntry || hasScrewDriver))//no driver && Unlocked
                        {
                            EntryPoint.WriteToConsole($"PLAYER EVENT: LockPick Start", 3);
                            CarLockPick MyLockPick = new CarLockPick(this, VehicleTryingToEnter, SeatTryingToEnter);
                            MyLockPick.PickLock();
                        }
                        else if (IsNotHoldingEnter && SeatTryingToEnter == -1 && VehicleTryingToEnter.Driver != null && VehicleTryingToEnter.Driver.IsAlive) //Driver
                        {
                            EntryPoint.WriteToConsole($"PLAYER EVENT: CarJack Start", 3);
                            PedExt jackedPed = World.Pedestrians.GetPedExt(VehicleTryingToEnter.Driver.Handle);
                            Violations.TheftViolations.AddCarJacked(jackedPed);
                            CarJack MyJack = new CarJack(this, CurrentVehicle, jackedPed, SeatTryingToEnter, WeaponEquipment.CurrentWeapon);
                            MyJack.Start();
                        }
                        else if (VehicleTryingToEnter.LockStatus == (VehicleLockStatus)7 && CurrentVehicle.IsCar)
                        {
                            EntryPoint.WriteToConsole($"PLAYER EVENT: Car Break-In Start LockStatus {VehicleTryingToEnter.LockStatus}", 3);
                            CarBreakIn MyBreakIn = new CarBreakIn(this, VehicleTryingToEnter, Settings, SeatTryingToEnter);
                            MyBreakIn.BreakIn();
                        }
                        //else if (SeatTryingToEnter != -1)
                        //{
                        //    if (VehicleTryingToEnter.Exists() && VehicleTryingToEnter.Model.Name.ToLower().Contains("bus"))
                        //    {
                        //        EntryPoint.WriteToConsole($"PLAYER EVENT: BusRide Start LockStatus {VehicleTryingToEnter.LockStatus}", 3);
                        //        BusRide MyBusRide = new BusRide(this, VehicleTryingToEnter, World, PlacesOfInterest);
                        //        MyBusRide.Start();
                        //    }
                        //    else
                        //    {
                        //        EntryPoint.WriteToConsole($"PLAYER EVENT: Car Enter as Passenger {VehicleTryingToEnter.LockStatus}", 3);
                        //    }
                        //}
                    }
                }
                else
                {
                    EntryPoint.WriteToConsole($"PLAYER EVENT: IsGettingIntoVehicle ERROR VEHICLE NOT FOUND (ARE YOU SCANNING ENOUGH?)", 3);
                }
            }
            else
            {
            }
            isGettingIntoVehicle = IsGettingIntoAVehicle;
            EntryPoint.WriteToConsole($"PLAYER EVENT: IsGettingIntoVehicleChanged to {IsGettingIntoAVehicle}, HoldingEnter {IsNotHoldingEnter}", 3);
        }
        private void OnIsInVehicleChanged()
        {
            GameFiber.Yield();
            if (IsInVehicle)
            {
                GameTimeGotInVehicle = Game.GameTime;
                GameTimeGotOutOfVehicle = 0;
                if (IsWanted && AnyPoliceCanSeePlayer && IsAliveAndFree)
                {
                    Scanner.OnGotInVehicle();
                }
                //RemoveOwnedVehicleBlip();
                if (CurrentVehicle != null)
                {
                    CurrentVehicle.HasAutoSetRadio = false;
                }







            }
            else
            {
                GameTimeGotOutOfVehicle = Game.GameTime;
                GameTimeGotInVehicle = 0;
                if (IsWanted && AnyPoliceCanSeePlayer && !IsRagdoll && IsAliveAndFree)
                {
                    Scanner.OnGotOutOfVehicle();
                }
                //CreateOwnedVehicleBlip();

                if (!Settings.SettingsManager.PlayerOtherSettings.AllowMobileRadioOnFoot && IsMobileRadioEnabled && !ActivityManager.IsDancing)
                {
                    IsMobileRadioEnabled = false;
                    NativeFunction.CallByName<bool>("SET_MOBILE_RADIO_ENABLED_DURING_GAMEPLAY", false);
                }



            }
            //UpdateOwnedBlips();
            EntryPoint.WriteToConsole($"PLAYER EVENT: IsInVehicle to {IsInVehicle}", 3);
        }
        private void OnPlayerBusted()
        {
            GameFiber.Yield();
            DiedInVehicle = IsInVehicle;
            IsBusted = true;
            BeingArrested = true;
            GameTimeLastBusted = Game.GameTime;
            WasDangerouslyArmedWhenBusted = IsDangerouslyArmed;
            Surrendering.OnPlayerBusted();
            Respawning.OnPlayerBusted();
            ActivityManager.OnPlayerBusted();
            if (Settings.SettingsManager.PlayerOtherSettings.SetSlowMoOnBusted)
            {
                Game.TimeScale = Settings.SettingsManager.PlayerOtherSettings.SlowMoOnBustedSpeed;// 0.4f;
            }
            NativeHelper.DisablePlayerControl();
            //Game.LocalPlayer.HasControl = false;
            Scanner.OnPlayerBusted();
            EntryPoint.WriteToConsole($"PLAYER EVENT: IsBusted Changed to: {IsBusted}", 3);
        }
        private void OnPlayerDied()
        {
            GameFiber.Yield();
            TimeControllable.PauseTime();
            DiedInVehicle = IsInVehicle;
            IsDead = true;
            GameTimeLastDied = Game.GameTime;
            Game.LocalPlayer.Character.Kill();
            Game.LocalPlayer.Character.Health = 0;
            Game.LocalPlayer.Character.IsInvincible = true;
            if (Settings.SettingsManager.PlayerOtherSettings.SetSlowMoOnDeath)
            {
                Game.TimeScale = Settings.SettingsManager.PlayerOtherSettings.SlowMoOnDeathSpeed;// 0.4f;
            }
            Scanner.OnSuspectWasted();
            ActivityManager.OnPlayerDied();
            EntryPoint.WriteToConsole($"PLAYER EVENT: IsDead Changed to: {IsDead}", 3);
        }
        private void OnIsShootingChanged()
        {
            if (IsShooting)
            {
                if (IsWanted && WantedLevel <= 4 && AnyPoliceRecentlySeenPlayer)
                {
                    Scanner.OnSuspectShooting();
                }
                PlayerVoice.OnShotGun();
                EntryPoint.WriteToConsole("PLAYER EVENT: Starting Shooting");
            }
            else
            {
                EntryPoint.WriteToConsole("PLAYER EVENT: Stopped Shooting");
            }
        }
        private void OnStartedDuckingInVehicle()
        {
            if (Settings.SettingsManager.VehicleSettings.ForceFirstPersonOnVehicleDuck)
            {
                int viewMode = NativeFunction.Natives.GET_FOLLOW_VEHICLE_CAM_VIEW_MODE<int>();
                if (viewMode != 4)
                {
                    storedViewMode = viewMode;
                    NativeFunction.Natives.SET_FOLLOW_VEHICLE_CAM_VIEW_MODE(4);
                }
                EntryPoint.WriteToConsole($"OnStartedDuckingInVehicle viewMode {viewMode} storedViewMode {storedViewMode}", 5);
            }
        }
        private void OnStoppedDuckingInVehicle()
        {
            if (Settings.SettingsManager.VehicleSettings.ForceFirstPersonOnVehicleDuck)
            {
                int viewMode = NativeFunction.Natives.GET_FOLLOW_VEHICLE_CAM_VIEW_MODE<int>();
                if (viewMode != storedViewMode)
                {
                    NativeFunction.Natives.SET_FOLLOW_VEHICLE_CAM_VIEW_MODE(storedViewMode);
                    storedViewMode = -1;
                }
                EntryPoint.WriteToConsole($"OnStoppedDuckingInVehicle storedViewMode {storedViewMode}", 5);
            }
        }
        private void OnTargettingHandleChanged()
        {
            if (TargettingHandle != 0)
            {
                CurrentTargetedPed = World.Pedestrians.GetPedExt(TargettingHandle);
                GameFiber.Yield();
                ActivityManager.OnTargetHandleChanged();
            }
            else
            {
                CurrentTargetedPed = null;
            }
            //EntryPoint.WriteToConsole($"PLAYER EVENT: CurrentTargetedPed to {CurrentTargetedPed?.Pedestrian?.Handle} CanHoldUpTargettedPed {CanHoldUpTargettedPed} CurrentTargetedPed?.CanBeMugged {CurrentTargetedPed?.CanBeMugged}", 5);
        }
        private void OnWantedLevelChanged()//runs after OnSuspectEluded (If Applicable)
        {
            GameFiber.Yield();
            if (IsNotWanted && PreviousWantedLevel != 0)//Lost Wanted
            {
                if (!RecentlySetWanted)//only allow my process to set the wanted level
                {
                    if (Settings.SettingsManager.PoliceSettings.TakeExclusiveControlOverWantedLevel)
                    {
                        EntryPoint.WriteToConsole($"PLAYER EVENT: GAME AUTO SET WANTED TO {WantedLevel}, RESETTING TO {PreviousWantedLevel}", 3);
                        SetWantedLevel(PreviousWantedLevel, "GAME AUTO SET WANTED", true);
                    }
                }
                else
                {
                    CriminalHistory.OnLostWanted();
                    GameFiber.Yield();
                    PoliceResponse.OnLostWanted();
                    GameFiber.Yield();
                    PlayerVoice.OnLostWanted();
                    GameFiber.Yield();//TR 05
                    RelationshipManager.GangRelationships.OnLostWanted();
                    GameFiber.Yield();//TR 05
                    World.Pedestrians.CivilianList.ForEach(x => x.PlayerCrimesWitnessed.Clear());
                    EntryPoint.WriteToConsole($"PLAYER EVENT: LOST WANTED", 3);
                }
            }
            else if (IsWanted && PreviousWantedLevel == 0)//Added Wanted Level
            {
                if (!RecentlySetWanted)//only allow my process to set the wanted level
                {
                    if (Settings.SettingsManager.PoliceSettings.TakeExclusiveControlOverWantedLevel)
                    {
                        EntryPoint.WriteToConsole($"PLAYER EVENT: GAME AUTO SET WANTED TO {WantedLevel}, RESETTING", 3);
                        SetWantedLevel(0, "GAME AUTO SET WANTED", true);
                    }
                }
                else
                {
                    Investigation.Reset();
                    GameFiber.Yield();
                    PoliceResponse.OnBecameWanted();
                    GameFiber.Yield();
                    PlayerVoice.OnBecameWanted();
                    GameFiber.Yield();//TR 05
                    RelationshipManager.GangRelationships.OnBecameWanted();
                    EntryPoint.WriteToConsole($"PLAYER EVENT: BECAME WANTED", 3);
                }
            }
            else if (IsWanted && PreviousWantedLevel < WantedLevel)//Increased Wanted Level (can't decrease only remove for now.......)
            {
                PoliceResponse.OnWantedLevelIncreased();
                EntryPoint.WriteToConsole($"PLAYER EVENT: WANTED LEVEL INCREASED", 3);
                //BigMessage.ShowColoredShard("WANTED", $"{wantedLevel} stars", HudColor.Gold, HudColor.InGameBackground);
            }
            else if (IsWanted && PreviousWantedLevel > WantedLevel)
            {
                //PoliceResponse.OnWantedLevelDecreased();
                EntryPoint.WriteToConsole($"PLAYER EVENT: WANTED LEVEL DECREASED", 3);
            }
            EntryPoint.WriteToConsole($"Wanted Changed: {WantedLevel} Previous: {PreviousWantedLevel}", 3);
            PreviousWantedLevel = wantedLevel;// NativeFunction.Natives.GET_FAKE_WANTED_LEVEL<int>();//PreviousWantedLevel = Game.LocalPlayer.WantedLevel;
        }
        //Crimes
        public void AddCrime(Crime crimeObserved, bool isObservedByPolice, Vector3 Location, VehicleExt VehicleObserved, WeaponInformation WeaponObserved, bool HaveDescription, bool AnnounceCrime, bool isForPlayer)
        {
            if (RecentlyBribedPolice && crimeObserved.ResultingWantedLevel <= 2)
            {
                return;
            }
            else if (RecentlyPaidFine && crimeObserved.ResultingWantedLevel <= 1)
            {
                return;
            }
            else if (RecentlyStartedPlaying)
            {
                return;
            }
            GameFiber.Yield();//TR 6 this is new, seems helpful so far with no downsides
            CrimeSceneDescription description = new CrimeSceneDescription(!IsInVehicle, isObservedByPolice, Location, HaveDescription) { VehicleSeen = VehicleObserved, WeaponSeen = WeaponObserved, Speed = Game.LocalPlayer.Character.Speed };
            PoliceResponse.AddCrime(crimeObserved, description, isForPlayer);
            if (!isObservedByPolice && IsNotWanted)
            {
                Investigation.Start(Location, PoliceResponse.PoliceHaveDescription, true, false, false);
            }
            if (AnnounceCrime)
            {
                Scanner.AnnounceCrime(crimeObserved, description);
            }
        }
        public void AddMedicalEvent(Vector3 position)
        {
            if (Settings.SettingsManager.EMSSettings.ManageDispatching && Settings.SettingsManager.EMSSettings.ManageTasking && World.TotalWantedLevel <= 1 && World.Pedestrians.PedExts.Any(x => (x.IsUnconscious || x.IsInWrithe) && !x.IsDead && !x.HasStartedEMTTreatment))
            {
                //Scanner.Reset();
                Investigation.Start(position, false, false, true, false);
                Scanner.OnMedicalServicesRequested();
            }
        }
        public void Arrest()
        {
            BeingArrested = true;
            if (!IsBusted)
            {
                OnPlayerBusted();
            }
        }
        public void SetWantedLevel(int desiredWantedLevel, string Reason, bool UpdateRecent)
        {
            if (desiredWantedLevel <= Settings.SettingsManager.PoliceSettings.MaxWantedLevel)
            {
                if (UpdateRecent)
                {
                    GameTimeLastSetWanted = Game.GameTime;
                }
                if (WantedLevel < desiredWantedLevel || (desiredWantedLevel == 0 && WantedLevel != 0))
                {
                    if (Settings.SettingsManager.PoliceSettings.UseFakeWantedLevelSystem)
                    {
                        NativeFunction.CallByName<bool>("SET_MAX_WANTED_LEVEL", 0);
                        if (Settings.SettingsManager.UIGeneralSettings.ShowFakeWantedLevelStars && desiredWantedLevel <= 6)
                        {
                            NativeFunction.Natives.SET_FAKE_WANTED_LEVEL(desiredWantedLevel);
                        }
                    }
                    else
                    {
                        NativeFunction.CallByName<bool>("SET_MAX_WANTED_LEVEL", desiredWantedLevel);
                        Game.LocalPlayer.WantedLevel = desiredWantedLevel;
                    }

                    wantedLevel = desiredWantedLevel;

                    if (desiredWantedLevel > 0)
                    {
                        GameTimeWantedLevelStarted = Game.GameTime;
                    }
                    OnWantedLevelChanged();
                    EntryPoint.WriteToConsole($"Set Wanted: From {WantedLevel} to {desiredWantedLevel} Reason: {Reason}", 3);
                }
            }
        }

        //Updates
        public void UpdateStateData()
        {
            if (Game.LocalPlayer.Character.IsDead && !IsDead)
            {
                OnPlayerDied();
            }
            if (NativeFunction.CallByName<bool>("IS_PLAYER_BEING_ARRESTED", 0))
            {
                BeingArrested = true;
            }
            if (NativeFunction.CallByName<bool>("IS_PLAYER_BEING_ARRESTED", 1))
            {
                BeingArrested = true;
                //Game.LocalPlayer.Character.Tasks.Clear();
                NativeFunction.Natives.CLEAR_PED_TASKS(Game.LocalPlayer.Character);
            }
            if (BeingArrested && !IsBusted)
            {
                OnPlayerBusted();
            }
            //this was below that, see if this helps with the flashing.....
            int realWantedLevel = Game.LocalPlayer.WantedLevel;
            if (realWantedLevel != 0)//NativeFunction.Natives.GET_FAKE_WANTED_LEVEL<int>()) //if (PreviousWantedLevel != Game.LocalPlayer.WantedLevel)
            {
                if (!Settings.SettingsManager.PoliceSettings.TakeExclusiveControlOverWantedLevel)
                {
                    //this setting is new, allow the game and mods to set 2+ stars
                    if (Settings.SettingsManager.PoliceSettings.TakeExclusiveControlOverWantedLevelOneStarAndBelow)
                    {
                        if (realWantedLevel > 1)
                        {
                            SetWantedLevel(realWantedLevel, "Something Else Set, Allowed by settings (1)", true);
                            PlacePoliceLastSeenPlayer = Position;

                        }
                    }
                    else//or is they want my mod to just accept any wanted level generated
                    {
                        SetWantedLevel(realWantedLevel, "Something Else Set, Allowed by settings (2)", true);
                        PlacePoliceLastSeenPlayer = Position;
                    }
                }

                Game.LocalPlayer.WantedLevel = 0;
                NativeFunction.CallByName<bool>("SET_MAX_WANTED_LEVEL", 0);
            }
            if (Settings.SettingsManager.UIGeneralSettings.ShowFakeWantedLevelStars && NativeFunction.Natives.GET_FAKE_WANTED_LEVEL<int>() != wantedLevel)
            {
                NativeFunction.Natives.SET_FAKE_WANTED_LEVEL(wantedLevel);
            }
            //if (Game.LocalPlayer.WantedLevel != 0)//NativeFunction.Natives.GET_FAKE_WANTED_LEVEL<int>()) //if (PreviousWantedLevel != Game.LocalPlayer.WantedLevel)
            //{
            //    Game.LocalPlayer.WantedLevel = 0;
            //    NativeFunction.CallByName<bool>("SET_MAX_WANTED_LEVEL", 0);
            //}

            if (PreviousWantedLevel != wantedLevel)//NativeFunction.Natives.GET_FAKE_WANTED_LEVEL<int>()) //if (PreviousWantedLevel != Game.LocalPlayer.WantedLevel)
            {
                GameFiber.Yield();
                OnWantedLevelChanged();
            }
            if (CurrentLocation.EntityToLocate.Exists() && CurrentLocation.EntityToLocate.Handle != Game.LocalPlayer.Character.Handle)
            {
                CurrentLocation.EntityToLocate = Game.LocalPlayer.Character;
            }
            if (HealthState.MyPed.Pedestrian.Exists() && HealthState.MyPed.Pedestrian.Handle != Game.LocalPlayer.Character.Handle)
            {
                HealthState.MyPed = new PedExt(Game.LocalPlayer.Character, Settings, Crimes, Weapons, PlayerName, "Person", World);
                if (CharacterModelIsFreeMode)
                {
                    HealthState.MyPed.VoiceName = FreeModeVoice;
                }

            }
            HealthState.UpdatePlayer(this);
            IsStunned = Game.LocalPlayer.Character.IsStunned;
            IsRagdoll = Game.LocalPlayer.Character.IsRagdoll;
            IsInCover = Game.LocalPlayer.Character.IsInCover;
            IsMovingDynamically = IsInCover || Game.LocalPlayer.Character.IsInCombat || Game.LocalPlayer.Character.IsJumping || Game.LocalPlayer.Character.IsRunning;
            position = Game.LocalPlayer.Character.Position;
            // RootPosition = NativeFunction.Natives.GET_WORLD_POSITION_OF_ENTITY_BONE<Vector3>(Game.LocalPlayer.Character, NativeFunction.CallByName<int>("GET_PED_BONE_INDEX", Game.LocalPlayer.Character, 57005));// if you are in a car, your position is the mioddle of the car, hopefully this fixes that
            //See which cell it is in now
            CellX = (int)(position.X / EntryPoint.CellSize);
            CellY = (int)(position.Y / EntryPoint.CellSize);
            EntryPoint.FocusCellX = CellX;
            EntryPoint.FocusCellY = CellY;
            EntryPoint.FocusZone = CurrentLocation?.CurrentZone;


            if (IsSleeping && IsNotWanted && !Investigation.IsActive && !Investigation.IsSuspicious)
            {
                if (!TimeControllable.IsFastForwarding)
                {
                    TimeControllable.FastForward(999);
                }
            }
            else
            {
                if (TimeControllable.IsFastForwarding)
                {
                    TimeControllable.StopFastForwarding();
                }
            }








            //GameFiber.Yield();//TR Yield RemovedTest 1
            ClosestInteractableLocation = null;
            if (!IsMovingFast && IsAliveAndFree && !ActivityManager.IsConversing)
            {
                float ClosestDistance = 999f;
                ClosestInteractableLocation = null;
                ClosestDistance = 999f;
                foreach (InteractableLocation gl in World.Places.ActiveInteractableLocations)// PlacesOfInterest.GetAllStores())
                {
                    if (gl.DistanceToPlayer <= 3.0f && gl.CanInteract && !ActivityManager.IsInteractingWithLocation && gl.CanCurrentlyInteract(this))
                    //if (gl.IsOpen(TimeControllable.CurrentHour) && gl.DistanceToPlayer <= 3.0f && gl.CanInteract && !ActivityManager.IsInteractingWithLocation && gl.CanCurrentlyInteract(this))
                    {
                        if (gl.DistanceToPlayer < ClosestDistance)
                        {
                            ClosestInteractableLocation = gl;
                            ClosestDistance = gl.DistanceToPlayer;
                        }
                    }
                }
            }
            //GameFiber.Yield();//TR Yield RemovedTest 1
            GameFiber.Yield();






            Stance.Update();

            Sprinting.Update();
            if (Settings.SettingsManager.ActivitySettings.AllowStartingScenarios && IsNotWanted && !IsInVehicle)//works fine, just turned off by default, needs some work
            {
                IsNearScenario = NativeFunction.Natives.DOES_SCENARIO_EXIST_IN_AREA<bool>(Position.X, Position.Y, Position.Z, 2f, true) && !NativeFunction.Natives.IS_SCENARIO_OCCUPIED<bool>(Position.X, Position.Y, Position.Z, 2f, true);
                ClosestScenario = new Scenario("", "Unknown");
                if (IsNearScenario)
                {
                    foreach (Scenario scenario in Scenarios.ScenarioList)
                    {
                        if (NativeFunction.Natives.DOES_SCENARIO_OF_TYPE_EXIST_IN_AREA<bool>(Position.X, Position.Y, Position.Z, scenario.InternalName, 2f, true))
                        {
                            ClosestScenario = scenario;
                            break;
                        }
                    }
                }
            }
            else
            {
                IsNearScenario = false;
            }








            if (IsMakingInsultingGesture && CurrentLookedAtPed != null)// && !CurrentLookedAtPed.IsFedUpWithPlayer)
            {
                CurrentLookedAtPed.InsultedByPlayer();
            }


            GPSManager.Update();





            //if (Surrendering.IsWavingHands)
            //{
            //    if (Game.GameTime - GameTimeLastRaiseHandsEmote >= 5000)
            //    {
            //        if (!Investigation.IsActive && World.Pedestrians.Police.Any(x => x.DistanceToPlayer <= 100f) && World.Pedestrians.Civilians.Any(x => x.WantedLevel == 0 && x.CurrentlyViolatingWantedLevel > 0 && ((x.DistanceToPlayer <= 70f && x.CanSeePlayer) || x.DistanceToPlayer <= 30f)))
            //        {
            //            Investigation.Start(Position, false, true, false, false);
            //        }
            //        PlaySpeech("GENERIC_FRIGHTENED_HIGH", false);
            //        GameTimeLastRaiseHandsEmote = Game.GameTime;
            //    }
            //}
            //else 
            
            if (Surrendering.HandsAreUp)
            {

                if (Game.GameTime - GameTimeLastRaiseHandsEmote >= 10000)
                {
                    if (RandomItems.RandomPercent(50))
                    {
                        PlaySpeech("GENERIC_FRIGHTENED_MED", false);
                    }
                    else
                    {
                        PlaySpeech("GUN_BEG", false);
                    }
                    GameTimeLastRaiseHandsEmote = Game.GameTime;
                }

            }






            if (IsInVehicle)
            {
                int VehicleViewMode = NativeFunction.Natives.GET_FOLLOW_VEHICLE_CAM_VIEW_MODE<int>();
                if (VehicleViewMode == 4)
                {
                    IsInFirstPerson = true;
                }
                else
                {
                    IsInFirstPerson = false;
                }
            }
            else
            {
                int ViewMode = NativeFunction.Natives.GET_FOLLOW_PED_CAM_VIEW_MODE<int>();
                if (ViewMode == 4)
                {
                    IsInFirstPerson = true;
                }
                else
                {
                    IsInFirstPerson = false;
                }
            }
            PlayerTasks.Update();




            if (CurrentLookedAtObject != null && CurrentLookedAtObject.Exists())
            {
                if(CurrentLookedAtObject.Handle != prevCurrentLookedAtObjectHandle)
                {
                    if(Seats.CanSit(CurrentLookedAtObject))
                    {
                        CanSitOnCurrentLookedAtObject = true;
                    }
                    else
                    {
                        CanSitOnCurrentLookedAtObject = false;
                    }
                    prevCurrentLookedAtObjectHandle = CurrentLookedAtObject.Handle;
                }
            }
            else
            {
                CanSitOnCurrentLookedAtObject = false;
                prevCurrentLookedAtObjectHandle = 0;
            }

            UpdateCop();
            //GameFiber.Yield();//TR Yield RemovedTest 1
        }
        public void UpdateVehicleData()
        {
            IsInVehicle = Character.IsInAnyVehicle(false);
            IsGettingIntoAVehicle = Character.IsGettingIntoVehicle;
            if (IsInVehicle)
            {
                if (Character.CurrentVehicle.Exists() && VehicleOwnership.OwnedVehicles.Any(x => x.Vehicle.Exists() && x.Vehicle.Handle == Character.CurrentVehicle.Handle))//OwnedVehicle != null && OwnedVehicle.Vehicle.Exists() && Character.CurrentVehicle.Handle == OwnedVehicle.Vehicle.Handle)
                {
                    isJacking = false;
                }
                else if (Character.CurrentVehicle.Exists() && LastFriendlyVehicle.Exists() && LastFriendlyVehicle.Handle == Character.CurrentVehicle.Handle)
                {
                    isJacking = false;
                }
                else
                {
                    isJacking = Character.IsJacking;
                }
                IsDriver = Character.SeatIndex == -1;
                IsInAirVehicle = Character.IsInAirVehicle;

                bool isModelBike = false;
                bool isModelBicycle = false;

                if (Character.CurrentVehicle.Exists())
                {
                    isModelBike = NativeFunction.Natives.IS_THIS_MODEL_A_BIKE<bool>((uint)Character.CurrentVehicle.Model.Hash);
                    isModelBicycle = NativeFunction.Natives.IS_THIS_MODEL_A_BICYCLE<bool>((uint)Character.CurrentVehicle.Model.Hash);
                }
                IsOnBicycle = isModelBicycle && isModelBike;
                IsOnMotorcycle = !isModelBicycle && isModelBike;
                IsInAutomobile = !(IsInAirVehicle || Game.LocalPlayer.Character.IsInSeaVehicle || IsOnBicycle || IsOnMotorcycle || Game.LocalPlayer.Character.IsInHelicopter);
                UpdateCurrentVehicle();
                GameFiber.Yield();
                if (CurrentVehicle != null && CurrentVehicle.Vehicle.Exists())
                {
                    VehicleSpeed = CurrentVehicle.Vehicle.Speed;
                    IsHotWiring = CurrentVehicle != null && CurrentVehicle.Vehicle.Exists() && CurrentVehicle.IsStolen && CurrentVehicle.Vehicle.MustBeHotwired;
                    CurrentVehicleRoll = NativeFunction.Natives.GET_ENTITY_ROLL<float>(CurrentVehicle.Vehicle); ;
                    if (CurrentVehicleRoll >= 80f || CurrentVehicleRoll <= -80f)
                    {
                        CurrentVehicleIsRolledOver = true;
                    }
                    else
                    {
                        CurrentVehicleIsRolledOver = false;
                    }
                    CurrentVehicleIsInAir = NativeFunction.Natives.IS_ENTITY_IN_AIR<bool>(CurrentVehicle.Vehicle);
                }
                else
                {
                    CurrentVehicleIsRolledOver = false;
                    VehicleSpeed = 0f;
                }
                if (VehicleSpeedMPH >= 80f)
                {
                    if (!isExcessiveSpeed)
                    {
                        OnExcessiveSpeed();
                        isExcessiveSpeed = true;
                    }
                }
                else
                {
                    if (isExcessiveSpeed)
                    {
                        isExcessiveSpeed = false;
                    }
                }
                if (isHotwiring != IsHotWiring)
                {
                    if (IsHotWiring)
                    {
                        GameTimeStartedHotwiring = Game.GameTime;
                    }
                    else
                    {
                        GameTimeStartedHotwiring = 0;
                    }
                    isHotwiring = IsHotWiring;
                }


                if (CurrentVehicle != null && CurrentVehicle.Vehicle.Exists() && CurrentVehicle.Vehicle.IsPoliceVehicle)
                {
                    IsInPoliceVehicle = true;
                }
                else
                {
                    IsInPoliceVehicle = false;
                }

                if (Settings.SettingsManager.VehicleSettings.AllowRadioInPoliceVehicles && CurrentVehicle != null && CurrentVehicle.Vehicle.Exists() && CurrentVehicle.Vehicle.IsEngineOn && IsInPoliceVehicle)
                {
                    if (!IsMobileRadioEnabled)
                    {
                        IsMobileRadioEnabled = true;
                        NativeFunction.CallByName<bool>("SET_MOBILE_RADIO_ENABLED_DURING_GAMEPLAY", true);
                    }
                }
                else
                {
                    if (IsMobileRadioEnabled)
                    {
                        IsMobileRadioEnabled = false;
                        NativeFunction.CallByName<bool>("SET_MOBILE_RADIO_ENABLED_DURING_GAMEPLAY", false);
                    }
                }

                if (Settings.SettingsManager.PoliceTaskSettings.AllowSettingSirenState && CurrentVehicle != null && CurrentVehicle.Vehicle.Exists() && CurrentVehicle.Vehicle.HasSiren && CurrentVehicle.Vehicle.IsSirenSilent)
                {
                    CurrentVehicle.Vehicle.IsSirenSilent = false;
                }


                if (VehicleSpeed >= 0.1f)
                {
                    GameTimeLastMoved = Game.GameTime;
                }
                else
                {
                    GameTimeLastMoved = 0;
                }
                if (VehicleSpeed >= 2.0f)
                {
                    GameTimeLastMovedFast = Game.GameTime;
                }
                else
                {
                    GameTimeLastMovedFast = 0;
                }
                IsStill = VehicleSpeed <= 0.1f;

                if (VehicleSpeedMPH >= 25f)
                {
                    if (GameTimeStartedMovingFast == 0)
                    {
                        GameTimeStartedMovingFast = Game.GameTime;
                    }
                }
                else
                {
                    GameTimeStartedMovingFast = 0;
                }

                if (VehicleSpeedMPH >= 5f)
                {
                    if (GameTimeStartedMoving == 0)
                    {
                        GameTimeStartedMoving = Game.GameTime;
                    }
                }
                else
                {
                    GameTimeStartedMoving = 0;
                }


                if (CurrentVehicle != null && CurrentVehicle.Vehicle.Exists() && Character.CurrentVehicle.Exists() && Character.SeatIndex != -1 && !IsRidingBus && CurrentVehicle.Vehicle.Model.Name.ToLower().Contains("bus"))
                {
                    IsRidingBus = true;
                    BusRide MyBusRide = new BusRide(this, CurrentVehicle.Vehicle, World, PlacesOfInterest, Settings);
                    MyBusRide.Start();
                }

            }
            else
            {
                CurrentVehicleIsRolledOver = false;
                IsDriver = false;
                IsOnMotorcycle = false;
                IsInAutomobile = false;
                IsInPoliceVehicle = false;
                IsHotWiring = false;
                PreviousVehicle = CurrentVehicle;
                CurrentVehicle = null;
                VehicleSpeed = 0f;

                float PlayerSpeed = Character.Speed;
                FootSpeed = PlayerSpeed;
                if (PlayerSpeed >= 0.1f)
                {
                    GameTimeLastMoved = Game.GameTime;
                }
                else
                {
                    GameTimeLastMoved = 0;
                }
                if (PlayerSpeed >= 7.0f)
                {
                    GameTimeLastMovedFast = Game.GameTime;
                }
                else
                {
                    GameTimeLastMovedFast = 0;
                }
                IsStill = Character.IsStill;

                if (PlayerSpeed >= 3.0f)
                {
                    if (GameTimeStartedMovingFast == 0)
                    {
                        GameTimeStartedMovingFast = Game.GameTime;
                    }
                }
                else
                {
                    GameTimeStartedMovingFast = 0;
                }
                if (PlayerSpeed >= 0.5f)
                {
                    if (GameTimeStartedMoving == 0)
                    {
                        GameTimeStartedMoving = Game.GameTime;
                    }
                }
                else
                {
                    GameTimeStartedMoving = 0;
                }

                if (!Settings.SettingsManager.PlayerOtherSettings.AllowMobileRadioOnFoot && !ActivityManager.IsDancing)
                {
                    NativeFunction.CallByName<bool>("SET_MOBILE_RADIO_ENABLED_DURING_GAMEPLAY", false);
                }
                isJacking = Character.IsJacking;
            }
            GameFiber.Yield();
            TrackedVehicles.RemoveAll(x => !x.Vehicle.Exists());
            bool isDuckingInVehicle = NativeFunction.Natives.GET_PED_CONFIG_FLAG<bool>(Character, 359, 1);
            if (IsDuckingInVehicle != isDuckingInVehicle)
            {
                if (isDuckingInVehicle)
                {
                    OnStartedDuckingInVehicle();
                }
                else
                {
                    OnStoppedDuckingInVehicle();
                }
                IsDuckingInVehicle = isDuckingInVehicle;
            }
            VehicleOwnership.Update();
        }
        public void UpdateWeaponData()
        {           
            if (Game.LocalPlayer.Character.IsShooting)
            {
                GameTimeLastShot = Game.GameTime;
            }
            IsAiming = Game.LocalPlayer.IsFreeAiming;
            IsAimingInVehicle = IsInVehicle && IsAiming;

            WeaponEquipment.Update();
            UpdateTargetedPed();
            GameFiber.Yield();
            UpdateLookedAtPed();
            GameFiber.Yield();
            IsShooting = RecentlyShot;
        }
        private void UpdateCurrentVehicle() //should this be public?
        {
            bool IsGettingIntoVehicle = Game.LocalPlayer.Character.IsGettingIntoVehicle;
            bool IsInVehicle = Game.LocalPlayer.Character.IsInAnyVehicle(false);
            if (!IsInVehicle && !IsGettingIntoVehicle)
            {
                PreviousVehicle = CurrentVehicle;
                CurrentVehicle = null;
                return;
            }
            Vehicle vehicle;
            if (IsGettingIntoVehicle)
            {
                vehicle = Game.LocalPlayer.Character.VehicleTryingToEnter;
            }
            else
            {
                vehicle = Game.LocalPlayer.Character.CurrentVehicle;
            }
            if (!vehicle.Exists())
            {
                PreviousVehicle = CurrentVehicle;
                CurrentVehicle = null;
                return;
            }
            uint newVehicleHandle = vehicle.Handle;
            if (CurrentVehicle == null)
            {
                if (PreviousVehicle != null && PreviousVehicle.Handle == newVehicleHandle)
                {
                    CurrentVehicle = PreviousVehicle;
                }
                else
                {
                    VehicleExt existingVehicleExt = World.Vehicles.GetVehicleExt(vehicle);
                    GameFiber.Yield();
                    if (vehicle.Exists())
                    {
                        if (existingVehicleExt == null)
                        {
                            VehicleExt createdVehicleExt = new VehicleExt(vehicle, Settings);
                            createdVehicleExt.Setup();
                            World.Vehicles.AddEntity(createdVehicleExt, ResponseType.None);
                            TrackedVehicles.Add(createdVehicleExt);
                            existingVehicleExt = createdVehicleExt;
                            EntryPoint.WriteToConsole("New Vehicle Created in UpdateCurrentVehicle");
                        }
                        if (!TrackedVehicles.Any(x => x.Vehicle.Handle == vehicle.Handle))
                        {
                            TrackedVehicles.Add(existingVehicleExt);
                        }
                        if (IsInVehicle && !existingVehicleExt.HasBeenEnteredByPlayer)
                        {
                            existingVehicleExt.SetAsEntered();
                        }
                        if(existingVehicleExt.IsAircraft)
                        {
                            if(Licenses.HasPilotsLicense && Licenses.PilotsLicense.CanFlyType(existingVehicleExt.VehicleClass))
                            {
                                existingVehicleExt.IsDisabled = false;
                            }
                            else
                            {
                                existingVehicleExt.IsDisabled = true;
                            }
                        }
                        existingVehicleExt.Engine.Synchronize();
                        existingVehicleExt.Update(this);
                        GameFiber.Yield();//TR removed 4
                        if (vehicle.Exists())
                        {
                            if (!existingVehicleExt.IsStolen)
                            {
                                if (IsDriver && !VehicleOwnership.OwnedVehicles.Any(x => x.Handle == existingVehicleExt.Handle))// == null || existingVehicleExt.Handle != OwnedVehicle.Handle))
                                {
                                    existingVehicleExt.IsStolen = true;
                                }
                            }
                            CurrentVehicle = existingVehicleExt;

                            EntryPoint.WriteToConsole("PLAYER VEHICLE UPDATE Needed to re look up vehicle", 5);
                        }
                    }
                }
            }
            else
            {
                CurrentVehicle.Update(this);
            }
        }
        private void UpdateLookedAtPed()
        {
            if (Game.GameTime - GameTimeLastUpdatedLookedAtPed >= 500)//750)//750)//750
            {
                GameFiber.Yield();
                Vector3 RayStart = Game.LocalPlayer.Character.GetBonePosition(PedBoneId.Head);
                Vector3 RayEnd = RayStart + NativeHelper.GetGameplayCameraDirection() * 6.0f;
                HitResult result = Rage.World.TraceCapsule(RayStart, RayEnd, 0.25f, TraceFlags.IntersectVehicles | TraceFlags.IntersectPeds, Game.LocalPlayer.Character);
                if (result.Hit && result.HitEntity is Ped)
                {
                    CurrentLookedAtObject = null;
                    CurrentLookedAtPed = World.Pedestrians.GetPedExt(result.HitEntity.Handle);
                    if (CurrentLookedAtPed?.IsGangMember == true)
                    {
                        CurrentLookedAtGangMember = World.Pedestrians.GetGangMember(result.HitEntity.Handle);
                    }
                    else
                    {
                        CurrentLookedAtGangMember = null;
                    }
                    CurrentLookedAtVehicle = null;
                    //EntryPoint.WriteToConsole("HIT PED");
                }
                else if (result.Hit && result.HitEntity is Vehicle)
                {
                    CurrentLookedAtObject = null;
                    Vehicle myCar = (Vehicle)result.HitEntity;
                    if (myCar.Exists())
                    {
                        CurrentLookedAtVehicle = World.Vehicles.GetVehicleExt(myCar);
                    }
                    else
                    {
                        CurrentLookedAtVehicle = null;
                    }


                    if (myCar.Exists() && Character.CurrentVehicle.Exists() && myCar.Handle == Character.CurrentVehicle.Handle)
                    {
                        CurrentLookedAtObject = null;
                        CurrentLookedAtVehicle = null;
                        CurrentLookedAtPed = null;
                        CurrentLookedAtGangMember = null;
                    }
                    else
                    {

                        if (myCar.Exists() && myCar.Driver.Exists())
                        {
                            Ped closestPed = null;
                            float ClosestDistance = 999f;
                            foreach (Ped occupant in myCar.Occupants)
                            {
                                if (occupant.Exists())
                                {
                                    float distanceTo = occupant.GetBonePosition(0).DistanceTo2D(Character);
                                    if (distanceTo <= ClosestDistance)
                                    {
                                        closestPed = occupant;
                                        ClosestDistance = distanceTo;
                                    }
                                }
                            }
                            if (closestPed.Exists())
                            {
                                CurrentLookedAtPed = World.Pedestrians.GetPedExt(closestPed.Handle);
                                if (CurrentLookedAtPed?.IsGangMember == true)
                                {
                                    CurrentLookedAtGangMember = World.Pedestrians.GetGangMember(closestPed.Handle);
                                }
                                else
                                {
                                    CurrentLookedAtGangMember = null;
                                }
                            }
                        }
                        else
                        {
                            CurrentLookedAtVehicle = null;
                            CurrentLookedAtPed = null;
                            CurrentLookedAtGangMember = null;
                            CurrentLookedAtObject = null;
                        }
                    }
                }
                else
                {
                    GameFiber.Yield();
                    result = Rage.World.TraceCapsule(RayStart, RayEnd, 0.25f, TraceFlags.IntersectObjects, Game.LocalPlayer.Character);
                    if (result.Hit && result.HitEntity is Rage.Object)
                    {
                        Rage.Object objectHit = (Rage.Object)result.HitEntity;
                        CurrentLookedAtObject = objectHit;
                        CurrentLookedAtVehicle = null;
                        CurrentLookedAtPed = null;
                        CurrentLookedAtGangMember = null;
                        //EntryPoint.WriteToConsole("HIT OBJECT");
                    }
                    else
                    {
                        CurrentLookedAtVehicle = null;
                        CurrentLookedAtPed = null;
                        CurrentLookedAtGangMember = null;
                        CurrentLookedAtObject = null;
                    }
                }




                //else if (result.Hit && result.HitEntity is Rage.Object)
                //{
                //    Rage.Object objectHit = (Rage.Object)result.HitEntity;
                //    CurrentLookedAtObject = objectHit;
                //    CurrentLookedAtVehicle = null;
                //    CurrentLookedAtPed = null;
                //    CurrentLookedAtGangMember = null;
                //    //EntryPoint.WriteToConsole("HIT OBJECT");
                //}
                //else
                //{
                //    CurrentLookedAtVehicle = null;
                //    CurrentLookedAtPed = null;
                //    CurrentLookedAtGangMember = null;
                //    CurrentLookedAtObject = null;
                //}
                GameTimeLastUpdatedLookedAtPed = Game.GameTime;
                GameFiber.Yield();
            }
        }
        private void UpdateTargetedPed()
        {
            if (IsAiming)
            {
                Entity AimingAt = Game.LocalPlayer.GetFreeAimingTarget();
                if (AimingAt.Exists())
                {
                    TargettingHandle = AimingAt.Handle;
                }
                else
                {
                    TargettingHandle = 0;
                }
            }
            else
            {
                TargettingHandle = NativeHelper.GetTargettingHandle();
            }
        }
        private void UpdateCop()
        {
            if(IsCop)
            {

            }
        }
    }
}