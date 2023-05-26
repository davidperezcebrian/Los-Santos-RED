﻿using LosSantosRED.lsr.Helper;
using LosSantosRED.lsr.Interface;
using Mod;
using Rage;
using Rage.Native;
using System;
using System.Drawing;
using System.Linq;

public class Cop : PedExt, IWeaponIssuable, IPlayerChaseable, IAIChaseable
{
    private bool IsSetStayInVehicle;
    private uint GameTimeSpawned;
    private bool WasAlreadySetPersistent = false;
    private bool IsShootingCheckerActive;
    private uint GameTimeFirstSawPlayerViolating;



    public Cop(Ped pedestrian, ISettingsProvideable settings, int health, Agency agency, bool wasModSpawned, ICrimes crimes, IWeapons weapons, string name, string modelName, IEntityProvideable world) : base(pedestrian, settings, crimes, weapons, name, "Cop", world)
    {
        IsCop = true;
        Health = health;
        AssignedAgency = agency;
        WasModSpawned = wasModSpawned;
        ModelName = modelName;
        if (WasModSpawned)
        {
            GameTimeSpawned = Game.GameTime;
        }
        if (Pedestrian.Exists() && Pedestrian.IsPersistent)
        {
            WasAlreadySetPersistent = true;
        }
        if (modelName.ToLower() == "mp_m_freemode_01")
        {
            VoiceName = "S_M_Y_COP_01_WHITE_FULL_01";// "S_M_Y_COP_01";
        }
        else if (modelName.ToLower() == "mp_f_freemode_01")
        {
            VoiceName = "S_F_Y_COP_01_WHITE_FULL_01";// "S_F_Y_COP_01";
        }
        WeaponInventory = new WeaponInventory(this, Settings);
        Voice = new CopVoice(this, ModelName, Settings);
        AssistManager = new CopAssistManager(this);
    }
    public IssuableWeapon GetRandomMeleeWeapon(IWeapons weapons) => AssignedAgency.GetRandomMeleeWeapon(weapons);
    public IssuableWeapon GetRandomWeapon(bool v, IWeapons weapons) => AssignedAgency.GetRandomWeapon(v, weapons);
    public Agency AssignedAgency { get; set; } = new Agency();
    public override Color BlipColor => AssignedAgency != null ? AssignedAgency.Color : base.BlipColor;
    public string CopDebugString => WeaponInventory.DebugWeaponState;
    public uint HasBeenSpawnedFor => Game.GameTime - GameTimeSpawned;
    public virtual bool ShouldBustPlayer => !IsInVehicle && DistanceToPlayer > 0.1f && HeightToPlayer <= 2.5f && !IsUnconscious && !IsInWrithe && DistanceToPlayer <= Settings.SettingsManager.PoliceSettings.BustDistance && Pedestrian.Exists() && !Pedestrian.IsRagdoll;
    public bool IsIdleTaskable => WasModSpawned || !WasAlreadySetPersistent;
    public bool ShouldUpdateTarget => Game.GameTime - GameTimeLastUpdatedTarget >= Settings.SettingsManager.PoliceTaskSettings.TargetUpdateTime;
    public string ModelName { get; set; }

    public bool CanRadioInWanted => SawPlayerViolating &&!IsUnconscious && !IsDead && !IsInWrithe && !IsBeingHeldAsHostage && GameTimeFirstSawPlayerViolating > 0 && Game.GameTime - GameTimeFirstSawPlayerViolating >= Settings.SettingsManager.PoliceSettings.RadioInTime && Pedestrian.Exists() && Pedestrian.Exists() && !Pedestrian.IsRagdoll;

    public bool SawPlayerViolating { get; private set; }
    public override int ShootRate { get; set; } = 500;
    public override int Accuracy { get; set; } = 40;
    public override int CombatAbility { get; set; } = 1;
    public override int TaserAccuracy { get; set; } = 30;
    public override int TaserShootRate { get; set; } = 100;
    public override int VehicleAccuracy { get; set; } = 10;
    public override int VehicleShootRate { get; set; } = 20;
    public override int TurretAccuracy { get; set; } = 30;
    public override int TurretShootRate { get; set; } = 1000;
    public CopAssistManager AssistManager { get; private set;}
    public CopVoice Voice { get; private set; }
    public WeaponInventory WeaponInventory { get; private set; }
    public bool IsRespondingToInvestigation { get; set; }
    public bool IsRespondingToWanted { get; set; }
    public bool IsRespondingToCitizenWanted { get; set; }
    public bool HasTaser { get; set; } = false;
    public int Division { get; set; } = -1;
    public virtual string UnitType { get; set; } = "Lincoln";
    public int BeatNumber { get; set; } = 1;
    public uint GameTimeLastUpdatedTarget { get; set; }
    public override bool KnowsDrugAreas => false;
    public override bool KnowsGangAreas => true;
    public bool IsUsingMountedWeapon { get; set; } = false;
    public PedExt CurrentTarget { get; set; }
    public override bool NeedsFullUpdate
    {
        get
        {
            if (GameTimeLastUpdated == 0)
            {
                return true;
            }
            else if (Game.GameTime > GameTimeLastUpdated + FullUpdateInterval)// + UpdateJitter)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
    private int FullUpdateInterval//dont forget distance and LOS in here
    {
        get
        {
            if (PlayerPerception?.DistanceToTarget >= 300)
            {
                return Settings.SettingsManager.PerformanceSettings.CopUpdateIntervalVeryFar;
            }
            else if (PlayerPerception?.DistanceToTarget >= 200)
            {
                return Settings.SettingsManager.PerformanceSettings.CopUpdateIntervalFar;
            }
            else if (PlayerPerception?.DistanceToTarget >= 50f)
            {
                return Settings.SettingsManager.PerformanceSettings.CopUpdateIntervalMedium;
            }
            else
            {
                return Settings.SettingsManager.PerformanceSettings.CopUpdateIntervalClose;
            }
        }
    }

    public bool IsShooting { get; private set; }

    public override void Update(IPerceptable perceptable, IPoliceRespondable policeRespondable, Vector3 placeLastSeen, IEntityProvideable world)
    {
        PlayerToCheck = policeRespondable;
        if (!Pedestrian.Exists())
        {
            return;
        }
        if (Pedestrian.IsAlive)
        {
            if (NeedsFullUpdate)
            {
                IsInWrithe = Pedestrian.IsInWrithe;
                UpdatePositionData();
                PlayerPerception.Update(perceptable, placeLastSeen);
                if (Settings.SettingsManager.PerformanceSettings.CopUpdatePerformanceMode1 && !PlayerPerception.RanSightThisUpdate)
                {
                    GameFiber.Yield();//TR TEST 30
                }
                if (Settings.SettingsManager.PerformanceSettings.IsCopYield1Active)
                {
                    GameFiber.Yield();//TR TEST 30
                }
                UpdateVehicleState();
                if (Settings.SettingsManager.PerformanceSettings.IsCopYield2Active)
                {
                    GameFiber.Yield();//TR TEST 30
                }
                if (Settings.SettingsManager.PerformanceSettings.CopUpdatePerformanceMode2 && !PlayerPerception.RanSightThisUpdate)
                {
                    GameFiber.Yield();//TR TEST 30
                }

                if (Settings.SettingsManager.PoliceSettings.AllowPoliceToCallEMTsOnBodies)
                {
                    PedAlerts.LookForUnconsciousPeds(world);
                }
                if (Settings.SettingsManager.PoliceSettings.AllowReactionsToBodies)//only care in a bubble around the player, nothing to do with the player tho
                {
                    PedAlerts.LookForBodiesAlert(world);
                }
                if (Settings.SettingsManager.PerformanceSettings.IsCopYield3Active)
                {
                    GameFiber.Yield();//TR TEST 30
                }
                if (PedAlerts.HasSeenUnconsciousPed)
                {
                    perceptable.AddMedicalEvent(PedAlerts.PositionLastSeenUnconsciousPed);
                    PedAlerts.HasSeenUnconsciousPed = false;
                }
                UpdateCombatFlags();
                PlayerViolationChecker(policeRespondable, world);
                GameTimeLastUpdated = Game.GameTime;
            }
        }
        CurrentHealthState.Update(policeRespondable);//has a yield if they get damaged, seems ok 
    }
    public void UpdateSpeech(IPoliceRespondable currentPlayer)
    {
        Voice.Speak(currentPlayer);
        //if (Settings.SettingsManager.PoliceSettings.AllowRadioInAnimations)
        //{
        //    Voice.RadioIn(currentPlayer);
        //}
    }
    public void ForceSpeech(IPoliceRespondable currentPlayer)
    {
        Voice.ResetSpeech();
        Voice.Speak(currentPlayer);
    }
    public void SetStats(DispatchablePerson dispatchablePerson, IWeapons Weapons, bool addBlip, string forceGroupName)
    {
        if (!Pedestrian.Exists())
        {
            return;
        }
        dispatchablePerson.SetPedExtPermanentStats(this, Settings.SettingsManager.PoliceSettings.OverrideHealth, Settings.SettingsManager.PoliceSettings.OverrideArmor, Settings.SettingsManager.PoliceSettings.OverrideAccuracy);
        if (!Pedestrian.Exists())
        {
            return;
        }
        if (!IsAnimal)
        {
            WeaponInventory.IssueWeapons(Weapons, true, true, true, dispatchablePerson);
            GameFiber.Yield();
        }
        if (!Pedestrian.Exists())
        {
            return;
        }
        if (AssignedAgency.Division != -1)
        {
            Division = AssignedAgency.Division;
            UnitType = forceGroupName;
            BeatNumber = AssignedAgency.GetNextBeatNumber();
            GroupName = $"{AssignedAgency.ID} {Division}-{UnitType}-{BeatNumber}";
        }
        else if (AssignedAgency.MemberName != "")
        {
            GroupName = AssignedAgency.MemberName;
        }
        else
        {
            GroupName = "Cop";
        }
        GameFiber.Yield();
        if (!Pedestrian.Exists())
        {
            return;
        }
        if (addBlip)
        {
            AddBlip();
        }
        if (IsAnimal)
        {
            return;
        }
        //return;
        if (Settings.SettingsManager.PoliceSettings.ForceDefaultWeaponAnimations)
        {
            NativeFunction.Natives.SET_WEAPON_ANIMATION_OVERRIDE(Pedestrian, Game.GetHashKey("Default"));
        }
        if(Settings.SettingsManager.PoliceTaskSettings.EnableCombatAttributeCanInvestigate)
        {
            NativeFunction.Natives.SET_PED_COMBAT_ATTRIBUTES(Pedestrian, (int)eCombatAttributes.CA_CAN_INVESTIGATE, true);
        }
        if (Settings.SettingsManager.PoliceTaskSettings.EnableCombatAttributeCanChaseOnFoot)
        {
            NativeFunction.Natives.SET_PED_COMBAT_ATTRIBUTES(Pedestrian, (int)eCombatAttributes.CA_CAN_CHASE_TARGET_ON_FOOT, true);
        }
        if (Settings.SettingsManager.PoliceTaskSettings.EnableCombatAttributeCanFlank)
        {
            NativeFunction.Natives.SET_PED_COMBAT_ATTRIBUTES(Pedestrian, (int)eCombatAttributes.CA_CAN_FLANK, true);
        }
        if (Settings.SettingsManager.PoliceTaskSettings.EnableCombatAttributeDisableEntryReactions)
        {
            NativeFunction.Natives.SET_PED_COMBAT_ATTRIBUTES(Pedestrian, (int)eCombatAttributes.CA_DISABLE_ENTRY_REACTIONS, true);
        }
        if(Settings.SettingsManager.PoliceTaskSettings.OverrrideTargetLossResponse)
        {
            NativeFunction.Natives.SET_PED_TARGET_LOSS_RESPONSE(Pedestrian, Settings.SettingsManager.PoliceTaskSettings.OverrrideTargetLossResponseValue);
        }
        if(Settings.SettingsManager.PoliceTaskSettings.EnableConfigFlagAlwaysSeeAproachingVehicles)
        {
            NativeFunction.Natives.SET_PED_CONFIG_FLAG(Pedestrian, (int)171, true);
        }
        if (Settings.SettingsManager.PoliceTaskSettings.EnableConfigFlagDiveFromApproachingVehicles)
        {
            NativeFunction.Natives.SET_PED_CONFIG_FLAG(Pedestrian, (int)172, true);
        }
        if(Settings.SettingsManager.PoliceTaskSettings.AllowMinorReactions)
        {
            NativeFunction.Natives.SET_PED_ALLOW_MINOR_REACTIONS_AS_MISSION_PED(Pedestrian, true);
        }
        if(Settings.SettingsManager.PoliceTaskSettings.StopWeaponFiringWhenDropped)
        {
            NativeFunction.Natives.STOP_PED_WEAPON_FIRING_WHEN_DROPPED(Pedestrian);
        }
    }
    private void UpdateCombatFlags()
    {
        if (StayInVehicle && IsUsingMountedWeapon)
        {
            if (!IsSetStayInVehicle)
            {
                NativeFunction.Natives.SET_PED_COMBAT_ATTRIBUTES(Pedestrian, (int)eCombat_Attribute.CA_LEAVE_VEHICLES, false);
                IsSetStayInVehicle = true;
                //EntryPoint.WriteToConsoleTestLong($"COP {Handle} SET CA_LEAVE_VEHICLES FALSE");
            }
        }
        else
        {
            if(IsSetStayInVehicle)
            {
                NativeFunction.Natives.SET_PED_COMBAT_ATTRIBUTES(Pedestrian, (int)eCombat_Attribute.CA_LEAVE_VEHICLES, true);
                IsSetStayInVehicle = false;
                //EntryPoint.WriteToConsoleTestLong($"COP {Handle} SET CA_LEAVE_VEHICLES TRUE");
            }
        }
    }
    public override void OnInsultedByPlayer(IInteractionable player)
    {
        base.OnInsultedByPlayer(player);
        if (IsFedUpWithPlayer)
        {
            player.SetAngeredCop();
        }
    }

    private void PlayerViolationChecker(IPoliceRespondable policeRespondable, IEntityProvideable world)
    {
        if(policeRespondable.IsNotWanted && SawPlayerViolating)
        {
            SawPlayerViolating = false;
            GameTimeFirstSawPlayerViolating = 0;
            return;
        }
        if(policeRespondable.IsNotWanted)
        {
            return;
        }
        if (CanSeePlayer && !SawPlayerViolating)
        {
            OnSawPlayerViolating();
        }
        if(policeRespondable.PoliceResponse.WantedLevelHasBeenRadioedIn)
        {
            return;
        }
        if (CanRadioInWanted)
        {
            Voice.RadioInWanted(policeRespondable);
            policeRespondable.PoliceResponse.RadioInWanted();
            EntryPoint.WriteToConsole($"I AM {Handle} AND I RADIOED IN THE WANTED LEVEL");
        }
        if(Settings.SettingsManager.PoliceSettings.AllowShootingInvestigations)
        ShootingChecker(policeRespondable);
        if (!SawPlayerViolating)
        {
            Cop cop = world.Pedestrians.AllPoliceList.FirstOrDefault(x => NativeHelper.IsNearby(CellX, CellY, x.CellX, x.CellY, 3) && x.IsShooting && x.Pedestrian.Exists());
            if(cop != null && cop.Pedestrian.Exists())
            {
                PedAlerts.AddHeardGunfire(cop.Pedestrian.Position);
            }
        }      
    }
    private void OnSawPlayerViolating()
    {
        GameTimeFirstSawPlayerViolating = Game.GameTime;
        SawPlayerViolating = true;
        EntryPoint.WriteToConsole($"I AM {Handle} AND I SAW PLAYER VIOLATING");
    }
    private void ShootingChecker(IPoliceRespondable policeRespondable)
    {
        if (!IsShootingCheckerActive)
        {
            GameFiber.Yield();//TR Yield add 1
            GameFiber.StartNew(delegate
            {
                try
                {
                    IsShootingCheckerActive = true;
                    //EntryPoint.WriteToConsole($"        Ped {PedExt.Pedestrian.Handle} IsShootingCheckerActive {IsShootingCheckerActive}", 5);
                    uint GameTimeLastShot = 0;
                    while (Pedestrian.Exists() && IsShootingCheckerActive && EntryPoint.ModController?.IsRunning == true && !policeRespondable.PoliceResponse.WantedLevelHasBeenRadioedIn)// && CarryingWeapon && IsShootingCheckerActive && ObservedWantedLevel < 3)
                    {
                        if (Pedestrian.IsShooting)
                        {
                            IsShooting = true;
                            GameTimeLastShot = Game.GameTime;
                        }
                        else if (Game.GameTime - GameTimeLastShot >= 5000)
                        {
                            IsShooting = false;
                        }
                        GameFiber.Yield();
                    }
                    IsShootingCheckerActive = false;
                }
                catch (Exception ex)
                {
                    EntryPoint.WriteToConsole(ex.Message + " " + ex.StackTrace, 0);
                    EntryPoint.ModController.CrashUnload();
                }
            }, "Ped Shooting Checker");
        }
    }
    //public void AddDivision(string forceGroupName)
    //{
    //    if (AssignedAgency.Division != -1)
    //    {
    //        Division = AssignedAgency.Division;
    //        UnitType = forceGroupName;
    //        BeatNumber = AssignedAgency.GetNextBeatNumber();
    //        GroupName = $"{AssignedAgency.ID} {Division}-{UnitType}-{BeatNumber}";
    //    }
    //    else if (AssignedAgency.MemberName != "")
    //    {
    //        GroupName = AssignedAgency.MemberName;
    //    }
    //    else
    //    {
    //        GroupName = "Cop";
    //    }
    //}

    //public void SetPedExtPermanentStats(DispatchablePerson dispatchablePerson, bool overrideHealth, bool overrideArmor, bool overrideAccuracy)
    //{
    //    if(dispatchablePerson == null)
    //    {
    //        return;
    //    }
    //    Accuracy = RandomItems.GetRandomNumberInt(dispatchablePerson.AccuracyMin, dispatchablePerson.AccuracyMax);
    //    ShootRate = RandomItems.GetRandomNumberInt(dispatchablePerson.ShootRateMin, dispatchablePerson.ShootRateMax);
    //    CombatAbility = RandomItems.GetRandomNumberInt(dispatchablePerson.CombatAbilityMin, dispatchablePerson.CombatAbilityMax);
    //    CombatMovement = CombatMovement;
    //    CombatRange = CombatRange;
    //    TaserAccuracy = RandomItems.GetRandomNumberInt(dispatchablePerson.TaserAccuracyMin, dispatchablePerson.TaserAccuracyMax);
    //    TaserShootRate = RandomItems.GetRandomNumberInt(dispatchablePerson.TaserShootRateMin, dispatchablePerson.TaserShootRateMax);
    //    VehicleAccuracy = RandomItems.GetRandomNumberInt(dispatchablePerson.VehicleAccuracyMin, dispatchablePerson.VehicleAccuracyMax);
    //    VehicleShootRate = RandomItems.GetRandomNumberInt(dispatchablePerson.VehicleShootRateMin, dispatchablePerson.VehicleShootRateMax);
    //    TurretAccuracy = RandomItems.GetRandomNumberInt(dispatchablePerson.TurretAccuracyMin, dispatchablePerson.TurretAccuracyMax);
    //    TurretShootRate = RandomItems.GetRandomNumberInt(dispatchablePerson.TurretShootRateMin, dispatchablePerson.TurretShootRateMax);
    //    if (AlwaysHasLongGun)
    //    {
    //        EntryPoint.WriteToConsole($"COP {Handle} AlwaysHasLongGun");
    //        AlwaysHasLongGun = true;
    //    }
    //    if (dispatchablePerson.OverrideVoice != null && dispatchablePerson.OverrideVoice.Any())
    //    {
    //        EntryPoint.WriteToConsole($"COP {Handle} VoiceName");
    //        VoiceName = dispatchablePerson.OverrideVoice.PickRandom();
    //    }
    //    if (!Pedestrian.Exists())
    //    {
    //        return;
    //    }



    //    if (dispatchablePerson.DisableBulletRagdoll)
    //    {
    //        EntryPoint.WriteToConsole($"COP {Handle} DisableBulletRagdoll");
    //        NativeFunction.Natives.SET_PED_CONFIG_FLAG(Pedestrian, (int)107, true);//PCF_DontActivateRagdollFromBulletImpact		= 107,  // Blocks ragdoll activation when hit by a bullet
    //    }


    //    if (dispatchablePerson.DisableCriticalHits)
    //    {
    //        EntryPoint.WriteToConsole($"COP {Handle} DisableCriticalHits");
    //        NativeFunction.Natives.SET_PED_SUFFERS_CRITICAL_HITS(Pedestrian, false);
    //    }
    //    HasFullBodyArmor = HasFullBodyArmor;
    //    if (dispatchablePerson.FiringPatternHash != 0)
    //    {
    //        EntryPoint.WriteToConsole($"COP {Handle} FiringPatternHash");
    //        NativeFunction.Natives.SET_PED_FIRING_PATTERN(Pedestrian, dispatchablePerson.FiringPatternHash);
    //    }




    //    if (overrideHealth)
    //    {
    //        EntryPoint.WriteToConsole($"COP {Handle} health");
    //        int health = RandomItems.GetRandomNumberInt(dispatchablePerson.HealthMin, dispatchablePerson.HealthMax) + 100 + (IsAnimal ? 100 : 0);
    //        Pedestrian.MaxHealth = health;
    //        Pedestrian.Health = health;
    //    }
    //    if (overrideArmor)
    //    {
    //        EntryPoint.WriteToConsole($"COP {Handle} armor");
    //        int armor = RandomItems.GetRandomNumberInt(dispatchablePerson.ArmorMin, dispatchablePerson.ArmorMax);
    //        Pedestrian.Armor = armor;
    //    }



    //    if (overrideAccuracy)
    //    {
    //        EntryPoint.WriteToConsole($"COP {Handle} overrideAccuracy");
    //        Pedestrian.Accuracy = Accuracy;
    //        NativeFunction.Natives.SET_PED_SHOOT_RATE(Pedestrian, ShootRate);
    //        NativeFunction.Natives.SET_PED_COMBAT_ABILITY(Pedestrian, CombatAbility);
    //        if (CombatMovement != -1)
    //        {
    //            NativeFunction.Natives.SET_PED_COMBAT_MOVEMENT(Pedestrian, CombatMovement);
    //            EntryPoint.WriteToConsole($"SET COMBAT MOVEMENT {Handle} {CombatMovement}");
    //        }
    //        if (CombatRange != -1)
    //        {
    //            NativeFunction.Natives.SET_PED_COMBAT_RANGE(Pedestrian, CombatRange);
    //            EntryPoint.WriteToConsole($"SET COMBAT RANGE {Handle} {CombatRange}");
    //        }
    //    }

    //  //  return;




    //    //GameFiber.Yield();
    //    //if (!Pedestrian.Exists())
    //    //{
    //    //    return;
    //    //}


    //    //if (dispatchablePerson.PedConfigFlagsToSet != null && dispatchablePerson.PedConfigFlagsToSet.Any())
    //    //{
    //    //    dispatchablePerson.PedConfigFlagsToSet.ForEach(x => x.ApplyToPed(Pedestrian));
    //    //}
    //    //if (dispatchablePerson.CombatAttributesToSet != null && dispatchablePerson.CombatAttributesToSet.Any())
    //    //{
    //    //    dispatchablePerson.CombatAttributesToSet.ForEach(x => x.ApplyToPed(Pedestrian));
    //    //}
    //    //if (dispatchablePerson.CombatFloatsToSet != null && dispatchablePerson.CombatFloatsToSet.Any())
    //    //{
    //    //    dispatchablePerson.CombatFloatsToSet.ForEach(x => x.ApplyToPed(Pedestrian));
    //    //}
    //}
}