﻿using ExtensionsMethods;
using LosSantosRED.lsr.Helper;
using LosSantosRED.lsr.Interface;
using LSR.Vehicles;
using Rage;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using static DispatchScannerFiles;

//Needs some refactoring
namespace LosSantosRED.lsr
{
    public class Scanner
    {
        private bool AbortedAudio;
        private Dispatch AnnounceStolenVehicle;
        private Dispatch AssaultingOfficer;
        private Dispatch AttemptingSuicide;
        private Dispatch AttemptToReacquireSuspect;
        private List<AudioSet> AttentionAllUnits;
        private IAudioPlayable AudioPlayer;
        private Dispatch CarryingWeapon;
        private Dispatch ChangedVehicles;
        private Dispatch CivilianDown;
        private Dispatch CivilianInjury;
        private Dispatch CivilianShot;
        private List<AudioSet> CiviliansReport;
        private Dispatch CriminalActivity;
        private DispatchEvent CurrentlyPlaying;
        private CrimeSceneDescription CurrentlyPlayingCallIn;
        private IPoliceRespondable CurrentPlayer;
        private List<Dispatch> DispatchList = new List<Dispatch>();
        private List<CrimeDispatch> DispatchLookup;
        private List<Dispatch> DispatchQueue = new List<Dispatch>();
        private Dispatch DrivingAtStolenVehicle;
        private Dispatch DrunkDriving;
        private Dispatch AssaultingCivilians;
        private Dispatch AimingWeaponAtPolice;
        private Dispatch DealingDrugs;
        private Dispatch AssaultingCiviliansWithDeadlyWeapon;
        private bool ExecutingQueue;
        private Dispatch FelonySpeeding;
        private uint GameTimeLastAnnouncedDispatch;
        private uint GameTimeLastDisplayedSubtitle;
        private uint GameTimeLastMentionedStreet;
        private uint GameTimeLastMentionedZone;
        private Dispatch GrandTheftAuto;
        private int HighestCivilianReportedPriority = 99;
        private int HighestOfficerReportedPriority = 99;
        private Dispatch Kidnapping;
        private List<AudioSet> LethalForce;
        private Dispatch LethalForceAuthorized;
        private List<AudioSet> LicensePlateSet;
        private Dispatch Mugging;
        private Dispatch NoFurtherUnitsNeeded;
        private List<uint> NotificationHandles = new List<uint>();
        private Dispatch OfficerDown;
        private Dispatch OfficerNeedsAssistance;
        private Dispatch Harassment;
        private Dispatch OfficersNeeded;
        private List<AudioSet> OfficersReport;
        private Dispatch OnFoot;
        private Dispatch GotOnFreeway;
        private Dispatch GotOffFreeway;
        private Dispatch PedHitAndRun;
        private Dispatch PublicIntoxication;
        private List<string> RadioEnd;
        private List<string> RadioStart;
        private Dispatch RecklessDriving;
        private Dispatch RemainInArea;
        private bool ReportedLethalForceAuthorized;
        private bool ReportedRequestAirSupport;
        private bool ReportedWeaponsFree;
        private Dispatch RequestAirSupport;
        private Dispatch RequestBackup;
        private Dispatch RequestMilitaryUnits;
        private Dispatch RequestNOOSEUnits;
        private Dispatch ResistingArrest;
        private Dispatch ResumePatrol;
        private Dispatch RunningARedLight;
        private ISettingsProvideable Settings;
        private Dispatch ShotsFired;
        private Dispatch ShotsFiredAtAnOfficer;
        private Dispatch StealingAirVehicle;
        private StreetScannerAudio StreetScannerAudio;
        private Dispatch SuspectArrested;
        private Dispatch SuspectEvaded;
        private Dispatch SuspectSpotted;
        private Dispatch SuspectWasted;
        private Dispatch SuspiciousActivity;
        private Dispatch SuspiciousVehicle;
        private Dispatch TamperingWithVehicle;
        private Dispatch TerroristActivity;
        private Dispatch ThreateningOfficerWithFirearm;
        private Dispatch TrespassingOnGovernmentProperty;
        private Dispatch VehicleHitAndRun;
        private VehicleScannerAudio VehicleScannerAudio;
        private Dispatch WantedSuspectSpotted;
        private Dispatch WeaponsFree;
        private IEntityProvideable World;
        private ZoneScannerAudio ZoneScannerAudio;
        private ITimeReportable Time;
        private Dispatch VehicleCrashed;

        public Scanner(IEntityProvideable world, IPoliceRespondable currentPlayer, IAudioPlayable audioPlayer, ISettingsProvideable settings, ITimeReportable time)
        {
            AudioPlayer = audioPlayer;
            CurrentPlayer = currentPlayer;
            World = world;
            Settings = settings;
            Time = time;
            VehicleScannerAudio = new VehicleScannerAudio();
            VehicleScannerAudio.ReadConfig();
            StreetScannerAudio = new StreetScannerAudio();
            StreetScannerAudio.ReadConfig();
            ZoneScannerAudio = new ZoneScannerAudio();
            ZoneScannerAudio.ReadConfig();
            DefaultConfig();
        }
        private enum LocationSpecificity
        {
            Nothing = 0,
            Zone = 1,
            HeadingAndStreet = 3,
            StreetAndZone = 5,
            Street = 6,
        }
        public bool RecentlyAnnouncedDispatch => GameTimeLastAnnouncedDispatch != 0 && Game.GameTime - GameTimeLastAnnouncedDispatch <= 25000;
        public bool RecentlyMentionedStreet => GameTimeLastMentionedStreet != 0 && Game.GameTime - GameTimeLastMentionedStreet <= 10000;
        public bool RecentlyMentionedZone => GameTimeLastMentionedZone != 0 && Game.GameTime - GameTimeLastMentionedZone <= 10000;
        public bool VeryRecentlyAnnouncedDispatch => GameTimeLastAnnouncedDispatch != 0 && Game.GameTime - GameTimeLastAnnouncedDispatch <= 10000;
        public void Abort()
        {
            AudioPlayer.Abort();
            RemoveAllNotifications();
        }
        public void AnnounceCrime(Crime crimeAssociated, CrimeSceneDescription reportInformation)
        {
            Dispatch ToAnnounce = DetermineDispatchFromCrime(crimeAssociated);
            if (ToAnnounce != null)
            {
                if (!ToAnnounce.HasVeryRecentlyBeenPlayed && ((ToAnnounce.CanBeReportedMultipleTimes && ToAnnounce.TimesPlayed <= 2) || ToAnnounce.TimesPlayed == 0))
                {
                    if (reportInformation.SeenByOfficers)
                    {
                        if (ToAnnounce.Priority <= HighestOfficerReportedPriority)
                        {
                            AddToQueue(ToAnnounce, reportInformation);
                        }
                    }
                    else
                    {
                        if (CurrentPlayer.IsNotWanted)
                        {
                            if (ToAnnounce.Priority < HighestCivilianReportedPriority || (ToAnnounce.Priority == HighestCivilianReportedPriority && !ToAnnounce.HasRecentlyBeenPlayed))
                            {
                                AddToQueue(ToAnnounce, reportInformation);
                            }
                        }
                    }
                }
            }
        }
        public void OnAppliedWantedStats()
        {
            if (!WantedSuspectSpotted.HasRecentlyBeenPlayed)
            {
                AddToQueue(WantedSuspectSpotted, new CrimeSceneDescription(!CurrentPlayer.IsInVehicle, true, CurrentPlayer.PlacePoliceLastSeenPlayer) { VehicleSeen = CurrentPlayer.CurrentVehicle });
            }
            EntryPoint.WriteToConsole($"SCANNER EVENT: OnAppliedWantedStats", 3);
        }
        public void OnArmyDeployed()
        {
            if (CurrentPlayer.IsWanted && !RequestMilitaryUnits.HasBeenPlayedThisWanted && World.AnyArmyUnitsSpawned)
            {
                AddToQueue(RequestMilitaryUnits);
            }
        }
        public void OnBribedPolice()
        {
            if (!ResumePatrol.HasRecentlyBeenPlayed)
            {
                AddToQueue(ResumePatrol);
            }
            EntryPoint.WriteToConsole($"SCANNER EVENT: OnBribedPolice", 3);
        }
        public void OnGotInVehicle()
        {
            if (!ChangedVehicles.HasRecentlyBeenPlayed && CurrentPlayer.CurrentVehicle != null && CurrentPlayer.CurrentVehicle.HasBeenDescribedByDispatch)
            {
                AddToQueue(ChangedVehicles);
            }
            EntryPoint.WriteToConsole($"SCANNER EVENT: InVehicle", 3);
        }
        public void OnGotOutOfVehicle()
        {
            if (!OnFoot.HasRecentlyBeenPlayed)
            {
                AddToQueue(OnFoot);
            }
            EntryPoint.WriteToConsole($"SCANNER EVENT: OnFoot", 3);
        }
        public void OnVehicleCrashed()
        {
            if (!VehicleCrashed.HasRecentlyBeenPlayed && CurrentPlayer.AnyPoliceCanSeePlayer)
            {
                AddToQueue(VehicleCrashed);
            }
            EntryPoint.WriteToConsole($"SCANNER EVENT: OnVehicleCrashed", 3);
        }
        public void OnHelicoptersDeployed()
        {
            if (CurrentPlayer.IsWanted && !ReportedRequestAirSupport && !RequestAirSupport.HasBeenPlayedThisWanted && World.AnyHelicopterUnitsSpawned)
            {
                AddToQueue(RequestAirSupport);
            }
        }
        public void OnInvestigationExpire()
        {
            if (!NoFurtherUnitsNeeded.HasRecentlyBeenPlayed)
            {
                Reset();
                AddToQueue(NoFurtherUnitsNeeded);
            }
            else
            {
                Reset();
            }
            
            EntryPoint.WriteToConsole($"SCANNER EVENT: OnInvestigationExpire", 3);
        }
        public void OnLethalForceAuthorized()
        {
            if (!ReportedLethalForceAuthorized && !LethalForceAuthorized.HasBeenPlayedThisWanted && CurrentPlayer.PoliceResponse.IsDeadlyChase)
            {
                AddToQueue(LethalForceAuthorized);
            }
            EntryPoint.WriteToConsole($"SCANNER EVENT: OnLethalForceAuthorized", 3);
        }
        public void OnNooseDeployed()
        {
            if (CurrentPlayer.IsWanted && !RequestNOOSEUnits.HasBeenPlayedThisWanted && World.AnyNooseUnitsSpawned)
            {
                AddToQueue(RequestNOOSEUnits);
            }
        }
        public void OnPaidFine()
        {
            if (!ResumePatrol.HasRecentlyBeenPlayed)
            {
                AddToQueue(ResumePatrol);
            }
            EntryPoint.WriteToConsole($"SCANNER EVENT: OnBribedPolice", 3);
        }
        public void OnPlayerBusted()
        {
            if (!SuspectArrested.HasRecentlyBeenPlayed && CurrentPlayer.AnyPoliceCanSeePlayer)
            {
                AddToQueue(SuspectArrested);
            }
            EntryPoint.WriteToConsole($"SCANNER EVENT: OnSuspectBusted", 3);
        }
        public void OnPoliceNoticeVehicleChange()
        {
            if (!ChangedVehicles.HasVeryRecentlyBeenPlayed && CurrentPlayer.CurrentVehicle != null)// && !CurrentPlayer.CurrentVehicle.HasBeenDescribedByDispatch)
            {
                AddToQueue(ChangedVehicles, new CrimeSceneDescription(!CurrentPlayer.IsInVehicle, true, CurrentPlayer.PlacePoliceLastSeenPlayer) { VehicleSeen = CurrentPlayer.CurrentVehicle });
            }
            EntryPoint.WriteToConsole($"SCANNER EVENT: OnPoliceNoticeVehicleChange", 3);
        }
        public void OnRequestedBackUp()
        {
            if (!RequestBackup.HasRecentlyBeenPlayed)
            {
                AddToQueue(RequestBackup, new CrimeSceneDescription(!CurrentPlayer.IsInVehicle, true, CurrentPlayer.PlacePoliceLastSeenPlayer));
            }
            EntryPoint.WriteToConsole($"SCANNER EVENT: OnRequestedBackUp", 3);
        }
        public void OnSuspectEluded()
        {
            //this is becuase when the search mode times out, it just sets the wanted to zero, which clears all the scanner dispatch queue stuff, so this doesnt get played, temp waiting 5 seconds so it will go in after this
            //long teerm need to change how the wanted level is set maybe with the chase result flag
            GameFiber TempWait = GameFiber.StartNew(delegate
            {
                GameFiber.Sleep(1000);
                if (!RemainInArea.HasRecentlyBeenPlayed)
                {
                    AddToQueue(RemainInArea, new CrimeSceneDescription(!CurrentPlayer.IsInVehicle, true, CurrentPlayer.PlacePoliceLastSeenPlayer));
                }
                EntryPoint.WriteToConsole($"SCANNER EVENT: OnSuspectEluded", 3);
            }, "PlayDispatchQueue");
        }
        public void OnSuspectWasted()
        {
            if (!SuspectWasted.HasRecentlyBeenPlayed && CurrentPlayer.AnyPoliceRecentlySeenPlayer && CurrentPlayer.MaxWantedLastLife > 0)
            {
                AddToQueue(SuspectWasted);
            }
            EntryPoint.WriteToConsole($"SCANNER EVENT: OnSuspectWasted", 3);
        }
        public void OnWantedActiveMode()
        {
            if (!SuspectSpotted.HasVeryRecentlyBeenPlayed && !DispatchQueue.Any() && CurrentPlayer.PoliceResponse.HasBeenWantedFor > 25000)
            {
                AddToQueue(SuspectSpotted, new CrimeSceneDescription(!CurrentPlayer.IsInVehicle, true, Game.LocalPlayer.Character.Position));
            }
            EntryPoint.WriteToConsole($"SCANNER EVENT: OnStarsActive", 3);
        }
        public void OnWantedSearchMode()
        {
            if (!SuspectEvaded.HasRecentlyBeenPlayed && !DispatchQueue.Any() && !World.AnyCopsNearPosition(CurrentPlayer.Position,100f))
            {
                AddToQueue(SuspectEvaded, new CrimeSceneDescription(!CurrentPlayer.IsInVehicle, true, CurrentPlayer.PlacePoliceLastSeenPlayer));
            }
            EntryPoint.WriteToConsole($"SCANNER EVENT: OnStarsGreyedOut", 3);
        }
        public void OnWeaponsFree()
        {
            if (!ReportedWeaponsFree & !WeaponsFree.HasBeenPlayedThisWanted)
            {
                AddToQueue(WeaponsFree);
            }
            EntryPoint.WriteToConsole($"SCANNER EVENT: OnWeaponsFree", 3);
        }
        public void OnGotOnFreeway()
        {
            if (!GotOnFreeway.HasRecentlyBeenPlayed && CurrentPlayer.IsInVehicle)
            {
                AddToQueue(GotOnFreeway);
            }
            EntryPoint.WriteToConsole($"SCANNER EVENT: OnGotOnFreeway", 5);
        }
        public void OnGotOffFreeway()
        {
            if (!GotOffFreeway.HasRecentlyBeenPlayed && CurrentPlayer.IsInVehicle)
            {
                AddToQueue(GotOffFreeway);
            }
            EntryPoint.WriteToConsole($"SCANNER EVENT: OnGotOffFreeway", 5);
        }
        public void Reset()
        {
            DispatchQueue.Clear();
            ReportedLethalForceAuthorized = false;
            ReportedWeaponsFree = false;
            ReportedRequestAirSupport = false;
            HighestCivilianReportedPriority = 99;
            HighestOfficerReportedPriority = 99;
            foreach (Dispatch ToReset in DispatchList)
            {
                ToReset.HasBeenPlayedThisWanted = false;
                ToReset.LatestInformation = new CrimeSceneDescription();
                ToReset.TimesPlayed = 0;
            }
            //newish
            GameTimeLastAnnouncedDispatch = 0;
            GameTimeLastDisplayedSubtitle = 0;
            GameTimeLastMentionedStreet = 0;
            GameTimeLastMentionedZone = 0;
            //end newish
            DispatchQueue.Clear();
        }
        public void Tick()
        {
            if (Settings.SettingsManager.PlayerSettings.Scanner_IsEnabled)
            {
                CheckDispatch();
                if (DispatchQueue.Count > 0 && !ExecutingQueue)
                {
                    ExecutingQueue = true;
                    GameFiber PlayDispatchQueue = GameFiber.StartNew(delegate
                    {
                        GameFiber.Sleep(RandomItems.MyRand.Next(Settings.SettingsManager.PlayerSettings.Scanner_DelayMinTime, Settings.SettingsManager.PlayerSettings.Scanner_DelayMaxTime));//GameFiber.Sleep(RandomItems.MyRand.Next(2500, 4500));//Next(1500, 2500)
                        if (DispatchQueue.Any(x => x.LatestInformation.SeenByOfficers))
                        {
                            DispatchQueue.RemoveAll(x => !x.LatestInformation.SeenByOfficers);
                        }
                        if (DispatchQueue.Count() > 1)
                        {
                            Dispatch HighestItem = DispatchQueue.OrderBy(x => x.Priority).FirstOrDefault();
                            DispatchQueue.Clear();
                            if (HighestItem != null)
                            {
                                DispatchQueue.Add(HighestItem);
                            }
                        }
                        while (DispatchQueue.Count > 0)
                        {
                            Dispatch Item = DispatchQueue.OrderBy(x => x.Priority).ToList()[0];
                            bool AddToPlayed = true;
                            if(CurrentPlayer.IsNotWanted && Item.LatestInformation.SeenByOfficers)
                            {
                                AddToPlayed = false;
                            }
                            BuildDispatch(Item, AddToPlayed);
                            if (DispatchQueue.Contains(Item))
                                DispatchQueue.Remove(Item);
                        }
                        ExecutingQueue = false;
                    }, "PlayDispatchQueue");
                }
            }
        }
        private void AddAudioSet(DispatchEvent dispatchEvent, AudioSet audioSet)
        {
            if (audioSet != null)
            {
                dispatchEvent.SoundsToPlay.AddRange(audioSet.Sounds);
                dispatchEvent.Subtitles += " " + audioSet.Subtitles;
            }
        }
        private void AddHaveDescription(DispatchEvent dispatchEvent)
        {
            dispatchEvent.NotificationText += "~n~~r~Have Description~s~";
        }
        private void AddHeading(DispatchEvent dispatchEvent)
        {
            dispatchEvent.SoundsToPlay.Add(new List<string>() { suspect_heading.TargetLastSeenHeading.FileName, suspect_heading.TargetReportedHeading.FileName, suspect_heading.TargetSeenHeading.FileName, suspect_heading.TargetSpottedHeading.FileName }.PickRandom());
            dispatchEvent.Subtitles += " ~s~suspect heading~s~";
            string heading = NativeHelper.GetSimpleCompassHeading(Game.LocalPlayer.Character.Heading);
            if (heading == "N")
            {
                dispatchEvent.SoundsToPlay.Add(direction_heading.North.FileName);
                dispatchEvent.Subtitles += " ~g~North~s~";
            }
            else if (heading == "S")
            {
                dispatchEvent.SoundsToPlay.Add(direction_heading.South.FileName);
                dispatchEvent.Subtitles += " ~g~South~s~";
            }
            else if (heading == "E")
            {
                dispatchEvent.SoundsToPlay.Add(direction_heading.East.FileName);
                dispatchEvent.Subtitles += " ~g~East~s~";
            }
            else if (heading == "W")
            {
                dispatchEvent.SoundsToPlay.Add(direction_heading.West.FileName);
                dispatchEvent.Subtitles += " ~g~West~s~";
            }
        }
        private void AddLethalForce(DispatchEvent dispatchEvent)
        {
            if (!ReportedLethalForceAuthorized)
            {
                AddAudioSet(dispatchEvent, LethalForce.PickRandom());
                ReportedLethalForceAuthorized = true;
            }
        }
        private void AddLocationDescription(DispatchEvent dispatchEvent, LocationSpecificity locationSpecificity)
        {
            if (locationSpecificity == LocationSpecificity.HeadingAndStreet)
            {
                AddHeading(dispatchEvent);
            }

            if (locationSpecificity == LocationSpecificity.Street || locationSpecificity == LocationSpecificity.HeadingAndStreet || locationSpecificity == LocationSpecificity.StreetAndZone)
            {
                AddStreet(dispatchEvent);
            }
            if (locationSpecificity == LocationSpecificity.Zone || locationSpecificity == LocationSpecificity.StreetAndZone)
            {
                AddZone(dispatchEvent);
            }

        }
        private void AddRapSheet(DispatchEvent dispatchEvent)
        {
            dispatchEvent.NotificationText = "Wanted For:" + CurrentPlayer.PoliceResponse.PrintCrimes();
        }
        private void AddRequestAirSupport(DispatchEvent dispatchEvent)
        {
            if (!ReportedRequestAirSupport)
            {
                AddAudioSet(dispatchEvent, new List<AudioSet>()
                {
                    new AudioSet(new List<string>() { officer_requests_air_support.Officersrequestinghelicoptersupport.FileName },"officers requesting helicopter support"),
                    new AudioSet(new List<string>() { officer_requests_air_support.Code99unitsrequestimmediateairsupport.FileName },"code-99 units request immediate air support"),
                    new AudioSet(new List<string>() { officer_requests_air_support.Officersrequireaerialsupport.FileName },"officers require aerial support"),
                    new AudioSet(new List<string>() { officer_requests_air_support.Officersrequireaerialsupport1.FileName },"officers require aerial support"),
                    new AudioSet(new List<string>() { officer_requests_air_support.Officersrequireairsupport.FileName },"officers require air support"),
                    new AudioSet(new List<string>() { officer_requests_air_support.Unitsrequestaerialsupport.FileName },"units request aerial support"),
                    new AudioSet(new List<string>() { officer_requests_air_support.Unitsrequestingairsupport.FileName },"units requesting air support"),
                    new AudioSet(new List<string>() { officer_requests_air_support.Unitsrequestinghelicoptersupport.FileName },"units requesting helicopter support"),
                }.PickRandom());
                ReportedRequestAirSupport = true;
                dispatchEvent.NotificationText += "~n~~r~Air Support Requested~s~";
            }
        }
        private void AddSpeed(DispatchEvent dispatchEvent, float Speed)
        {
            Speed = Speed * 2.23694f;//convert to mph
            if (Speed >= 40f)
            {
                dispatchEvent.SoundsToPlay.Add(suspect_last_seen.TargetLastReported.FileName);
                dispatchEvent.Subtitles += " ~s~target last reported~s~";
                if (Speed >= 40f && Speed < 50f)
                {
                    dispatchEvent.SoundsToPlay.Add(doing_speed.Doing40mph.FileName);
                    dispatchEvent.Subtitles += " ~s~doing ~o~40 mph~s~";
                    dispatchEvent.NotificationText += "~n~Speed Exceeding: ~o~40 mph~s~";
                }
                else if (Speed >= 50f && Speed < 60f)
                {
                    dispatchEvent.SoundsToPlay.Add(doing_speed.Doing50mph.FileName);
                    dispatchEvent.Subtitles += " ~s~doing ~o~50 mph~s~";
                    dispatchEvent.NotificationText += "~n~Speed Exceeding: ~o~50 mph~s~";
                }
                else if (Speed >= 60f && Speed < 70f)
                {
                    dispatchEvent.SoundsToPlay.Add(doing_speed.Doing60mph.FileName);
                    dispatchEvent.Subtitles += " ~s~doing ~o~60 mph~s~";
                    dispatchEvent.NotificationText += "~n~Speed Exceeding: ~o~60 mph~s~";
                }
                else if (Speed >= 70f && Speed < 80f)
                {
                    dispatchEvent.SoundsToPlay.Add(doing_speed.Doing70mph.FileName);
                    dispatchEvent.Subtitles += " ~s~doing ~o~70 mph~s~";
                    dispatchEvent.NotificationText += "~n~Speed Exceeding: ~o~70 mph~s~";
                }
                else if (Speed >= 80f && Speed < 90f)
                {
                    dispatchEvent.SoundsToPlay.Add(doing_speed.Doing80mph.FileName);
                    dispatchEvent.Subtitles += " ~s~doing ~o~80 mph~s~";
                    dispatchEvent.NotificationText += "~n~Speed Exceeding: ~o~80 mph~s~";
                }
                else if (Speed >= 90f && Speed < 100f)
                {
                    dispatchEvent.SoundsToPlay.Add(doing_speed.Doing90mph.FileName);
                    dispatchEvent.Subtitles += " ~s~doing ~o~90 mph~s~";
                    dispatchEvent.NotificationText += "~n~Speed Exceeding: ~o~90 mph~s~";
                }
                else if (Speed >= 100f && Speed < 104f)
                {
                    dispatchEvent.SoundsToPlay.Add(doing_speed.Doing100mph.FileName);
                    dispatchEvent.Subtitles += " ~s~doing ~o~100 mph~s~";
                    dispatchEvent.NotificationText += "~n~Speed Exceeding: ~o~100 mph~s~";
                }
                else if (Speed >= 105f)
                {
                    dispatchEvent.SoundsToPlay.Add(doing_speed.Doingover100mph.FileName);
                    dispatchEvent.Subtitles += " ~s~doing ~o~over 100 mph~s~";
                    dispatchEvent.NotificationText += "~n~Speed Exceeding: ~o~105 mph~s~";
                }
            }
        }
        private void AddStreet(DispatchEvent dispatchEvent)
        {
            if (RecentlyMentionedStreet)
            {
                return;
            }
            Street MyStreet = CurrentPlayer.CurrentLocation.CurrentStreet;
            if (MyStreet != null)
            {
                string StreetAudio = StreetScannerAudio.GetAudio(MyStreet.Name);
                if (StreetAudio != "")
                {
                    dispatchEvent.SoundsToPlay.Add(new List<string>() { conjunctives.On.FileName, conjunctives.On1.FileName, conjunctives.On2.FileName, conjunctives.On3.FileName, conjunctives.On4.FileName }.PickRandom());
                    dispatchEvent.SoundsToPlay.Add(StreetAudio);
                    dispatchEvent.Subtitles += " ~s~on ~HUD_COLOUR_YELLOWLIGHT~" + MyStreet.Name + "~s~";
                    dispatchEvent.NotificationText += "~n~~HUD_COLOUR_YELLOWLIGHT~" + MyStreet.Name + "~s~";
                    dispatchEvent.HasStreetAudio = true;

                    if (CurrentPlayer.CurrentLocation.CurrentCrossStreet != null)
                    {
                        Street MyCrossStreet = CurrentPlayer.CurrentLocation.CurrentCrossStreet;
                        if (MyCrossStreet != null)
                        {
                            string CrossStreetAudio = StreetScannerAudio.GetAudio(MyCrossStreet.Name);
                            if (CrossStreetAudio != "")
                            {
                                dispatchEvent.SoundsToPlay.Add(new List<string>() { conjunctives.AT01.FileName, conjunctives.AT02.FileName }.PickRandom());
                                dispatchEvent.SoundsToPlay.Add(CrossStreetAudio);
                                dispatchEvent.NotificationText += " ~s~at ~HUD_COLOUR_YELLOWLIGHT~" + MyCrossStreet.Name + "~s~";
                                dispatchEvent.Subtitles += " ~s~at ~HUD_COLOUR_YELLOWLIGHT~" + MyCrossStreet.Name + "~s~";
                            }
                        }
                    }
                }
            }
        }
        private void AddToQueue(Dispatch ToAdd, CrimeSceneDescription ToCallIn)
        {
            Dispatch Existing = DispatchQueue.FirstOrDefault(x => x.Name == ToAdd.Name);
            if (Existing != null)
            {
                Existing.LatestInformation = ToCallIn;
            }
            else
            {
                ToAdd.LatestInformation = ToCallIn;
                //EntryPoint.WriteToConsole("ScannerScript " + ToAdd.Name);
                DispatchQueue.Add(ToAdd);
            }
        }
        private void AddToQueue(Dispatch ToAdd)
        {
            Dispatch Existing = DispatchQueue.FirstOrDefault(x => x.Name == ToAdd.Name);
            if (Existing == null && Settings.SettingsManager.PlayerSettings.Scanner_IsEnabled)
            {
                DispatchQueue.Add(ToAdd);
                //EntryPoint.WriteToConsole("ScannerScript " + ToAdd.Name);
            }
        }
        private void AddVehicleDescription(DispatchEvent dispatchEvent, VehicleExt VehicleToDescribe, bool IncludeLicensePlate, Dispatch DispatchToPlay)
        {
            if (VehicleToDescribe == null)
                return;
            if (VehicleToDescribe.HasBeenDescribedByDispatch)
                return;
            //else
            //    VehicleToDescribe.HasBeenDescribedByDispatch = true;

            if (VehicleToDescribe != null && VehicleToDescribe.Vehicle.Exists())
            {
                dispatchEvent.NotificationText += "~n~Vehicle:~s~";
                //dispatchEvent.SoundsToPlay.Add(suspect_is.SuspectIs.FileName);
                //dispatchEvent.SoundsToPlay.Add(conjunctives.Drivinga.FileName);
                //dispatchEvent.Subtitles += " suspect is driving a ~s~";

                Color CarColor = VehicleToDescribe.VehicleColor(); //Vehicles.VehicleManager.VehicleColor(VehicleToDescribe);
                string MakeName = VehicleToDescribe.MakeName();// Vehicles.VehicleManager.MakeName(VehicleToDescribe);
                int ClassInt = VehicleToDescribe.ClassInt();// Vehicles.VehicleManager.ClassInt(VehicleToDescribe);
                string ClassName = VehicleScannerAudio.ClassName(ClassInt);
                string ModelName = VehicleToDescribe.ModelName();// Vehicles.VehicleManager.ModelName(VehicleToDescribe);

                string ColorAudio = VehicleScannerAudio.GetColorAudio(CarColor);
                string MakeAudio = VehicleScannerAudio.GetMakeAudio(MakeName);
                string ClassAudio = VehicleScannerAudio.GetClassAudio(ClassInt);
                string ModelAudio = VehicleScannerAudio.GetModelAudio(VehicleToDescribe.Vehicle.Model.Hash);

                //if(VehicleToDescribe.IsStolen)
                //{
                //    dispatchEvent.SoundsToPlay.Add(suspect_is.SuspectIs.FileName);
                //    dispatchEvent.SoundsToPlay.Add(conjunctives.In.FileName);
                //    dispatchEvent.Subtitles += " suspect is in a stolen vehicle, a ~s~";
                //    dispatchEvent.SoundsToPlay.Add(crime_10_851.Astolenvehicle.FileName);
                //    dispatchEvent.SoundsToPlay.Add(conjunctives.A01.FileName);
                //}
                //else
                //{
                    dispatchEvent.SoundsToPlay.Add(suspect_is.SuspectIs.FileName);
                    dispatchEvent.SoundsToPlay.Add(conjunctives.Drivinga.FileName);
                    dispatchEvent.Subtitles += " suspect is driving a ~s~";
                    
                //}

                if (ColorAudio != "")
                {
                    dispatchEvent.SoundsToPlay.Add(ColorAudio);
                    dispatchEvent.Subtitles += " ~s~" + CarColor.Name + "~s~";
                    dispatchEvent.NotificationText += " ~s~" + CarColor.Name + "~s~";
                }
                if (MakeAudio != "")
                {
                    dispatchEvent.SoundsToPlay.Add(MakeAudio);
                    dispatchEvent.Subtitles += " ~s~" + MakeName + "~s~";
                    dispatchEvent.NotificationText += " ~s~" + MakeName + "~s~";
                }

                if (ModelAudio != "")
                {
                    dispatchEvent.SoundsToPlay.Add(ModelAudio);
                    dispatchEvent.Subtitles += " ~s~" + ModelName + "~s~";
                    dispatchEvent.NotificationText += " ~s~" + ModelName + "~s~";
                }
                else if (ClassAudio != "")
                {
                    dispatchEvent.SoundsToPlay.Add(ClassAudio);
                    dispatchEvent.Subtitles += " ~s~" + ClassName + "~s~";
                    dispatchEvent.NotificationText += " ~s~" + ClassName + "~s~";
                }

                if (IncludeLicensePlate)
                {
                    AddAudioSet(dispatchEvent, LicensePlateSet.PickRandom());
                    string LicensePlateText = VehicleToDescribe.OriginalLicensePlate.PlateNumber;
                    dispatchEvent.SoundsToPlay.AddRange(VehicleScannerAudio.GetPlateAudio(LicensePlateText));
                    dispatchEvent.Subtitles += " ~s~" + LicensePlateText + "~s~";
                    dispatchEvent.NotificationText += " ~s~Plate: " + LicensePlateText + "~s~";
                }
                if(DispatchToPlay.Name == "Suspicious Vehicle")
                {
                    dispatchEvent.NotificationText += "~n~~s~For: " + VehicleToDescribe.IsSuspicious(Time.IsNight) + "~s~";
                }
                //EntryPoint.WriteToConsole(string.Format("ScannerScript Color {0}, Make {1}, Class {2}, Model {3}, RawModel {4}", CarColor.Name, MakeName, ClassName, ModelName, VehicleToDescribe.Vehicle.Model.Name));
            }
        }
        private void AddWeaponDescription(DispatchEvent dispatchEvent, WeaponInformation WeaponToDescribe)
        {
            dispatchEvent.NotificationText += "~n~Weapon:~s~";
            dispatchEvent.SoundsToPlay.Add(suspect_is.SuspectIs.FileName);
            if (WeaponToDescribe == null)
            {
                dispatchEvent.SoundsToPlay.Add(carrying_weapon.Carryingaweapon.FileName);
                dispatchEvent.Subtitles += " suspect is carrying a ~r~weapon~s~";
                dispatchEvent.NotificationText += " Unknown";
            }
            else if (WeaponToDescribe.ModelName == "weapon_rpg")
            {
                dispatchEvent.SoundsToPlay.Add(carrying_weapon.ArmedwithanRPG.FileName);
                dispatchEvent.Subtitles += " suspect is armed with an ~r~RPG~s~";
                dispatchEvent.NotificationText += " RPG";
            }
            else if (WeaponToDescribe.ModelName == "weapon_bat")
            {
                dispatchEvent.SoundsToPlay.Add(carrying_weapon.Armedwithabat.FileName);
                dispatchEvent.Subtitles += " suspect is armed with a ~r~bat~s~";
                dispatchEvent.NotificationText += " Bat";
            }
            else if (WeaponToDescribe.ModelName == "weapon_grenadelauncher" || WeaponToDescribe.ModelName == "weapon_grenadelauncher_smoke" || WeaponToDescribe.ModelName == "weapon_compactlauncher")
            {
                dispatchEvent.SoundsToPlay.Add(carrying_weapon.Armedwithagrenadelauncher.FileName);
                dispatchEvent.Subtitles += " suspect is armed with a ~r~grenade launcher~s~";
                dispatchEvent.NotificationText += " Grenade Launcher";
            }
            else if (WeaponToDescribe.Category == WeaponCategory.Throwable || WeaponToDescribe.ModelName == "weapon_grenadelauncher_smoke" || WeaponToDescribe.ModelName == "weapon_compactlauncher")
            {
                dispatchEvent.SoundsToPlay.Add(carrying_weapon.Armedwithexplosives.FileName);
                dispatchEvent.Subtitles += " suspect is armed with ~r~explosives~s~";
                dispatchEvent.NotificationText += " Explosives";
            }
            else if (WeaponToDescribe.ModelName == "weapon_dagger" || WeaponToDescribe.ModelName == "weapon_knife" || WeaponToDescribe.ModelName == "weapon_switchblade")
            {
                dispatchEvent.SoundsToPlay.Add(carrying_weapon.Armedwithaknife.FileName);
                dispatchEvent.Subtitles += " suspect is armed with a ~r~knife~s~";
                dispatchEvent.NotificationText += " Knife";
            }
            else if (WeaponToDescribe.ModelName == "weapon_minigun")
            {
                dispatchEvent.SoundsToPlay.Add(carrying_weapon.Armedwithaminigun.FileName);
                dispatchEvent.Subtitles += " suspect is armed with a ~r~minigun~s~";
                dispatchEvent.NotificationText += " Minigun";
            }
            else if (WeaponToDescribe.ModelName == "weapon_sawnoffshotgun")
            {
                dispatchEvent.SoundsToPlay.Add(carrying_weapon.Armedwithasawedoffshotgun.FileName);
                dispatchEvent.Subtitles += " suspect is armed with a ~r~sawed off shotgun~s~";
                dispatchEvent.NotificationText += " Sawed Off Shotgun";
            }
            else if (WeaponToDescribe.Category == WeaponCategory.LMG)
            {
                dispatchEvent.SoundsToPlay.Add(carrying_weapon.Armedwithamachinegun.FileName);
                dispatchEvent.Subtitles += " suspect is armed with a ~r~machine gun~s~";
                dispatchEvent.NotificationText += " Machine Gun";
            }
            else if (WeaponToDescribe.Category == WeaponCategory.Pistol)
            {
                dispatchEvent.SoundsToPlay.Add(carrying_weapon.Armedwithafirearm.FileName);
                dispatchEvent.Subtitles += " suspect is armed with a ~r~pistol~s~";
                dispatchEvent.NotificationText += " Pistol";
            }
            else if (WeaponToDescribe.Category == WeaponCategory.Shotgun)
            {
                dispatchEvent.SoundsToPlay.Add(carrying_weapon.Armedwithashotgun.FileName);
                dispatchEvent.Subtitles += " suspect is armed with a ~r~shotgun~s~";
                dispatchEvent.NotificationText += " Shotgun";
            }
            else if (WeaponToDescribe.Category == WeaponCategory.SMG)
            {
                dispatchEvent.SoundsToPlay.Add(carrying_weapon.Armedwithasubmachinegun.FileName);
                dispatchEvent.Subtitles += " suspect is armed with a ~r~submachine gun~s~";
                dispatchEvent.NotificationText += " Submachine Gun";
            }
            else if (WeaponToDescribe.Category == WeaponCategory.AR)
            {
                dispatchEvent.SoundsToPlay.Add(carrying_weapon.Carryinganassaultrifle.FileName);
                dispatchEvent.Subtitles += " suspect is carrying an ~r~assault rifle~s~";
                dispatchEvent.NotificationText += " Assault Rifle";
            }
            else if (WeaponToDescribe.Category == WeaponCategory.Sniper)
            {
                dispatchEvent.SoundsToPlay.Add(carrying_weapon.Armedwithasniperrifle.FileName);
                dispatchEvent.Subtitles += " suspect is armed with a ~r~sniper rifle~s~";
                dispatchEvent.NotificationText += " Sniper Rifle";
            }
            else if (WeaponToDescribe.Category == WeaponCategory.Heavy)
            {
                dispatchEvent.SoundsToPlay.Add(status_message.HeavilyArmed.FileName);
                dispatchEvent.Subtitles += " suspect is ~r~heaviy armed~s~";
                dispatchEvent.NotificationText += " Heavy Weapon";
            }
            else if (WeaponToDescribe.Category == WeaponCategory.Melee)
            {
                dispatchEvent.SoundsToPlay.Add(carrying_weapon.Carryingaweapon.FileName);
                dispatchEvent.Subtitles += " suspect is carrying a ~r~weapon~s~";
                dispatchEvent.NotificationText += " Close Combat Weapon";
            }
            else
            {
                int Num = RandomItems.MyRand.Next(1, 5);
                if (Num == 1)
                {
                    dispatchEvent.SoundsToPlay.Add(carrying_weapon.Armedwithafirearm.FileName);
                    dispatchEvent.Subtitles += " suspect is armed with a ~r~firearm~s~";
                }
                else if (Num == 2)
                {
                    dispatchEvent.SoundsToPlay.Add(carrying_weapon.Armedwithagat.FileName);
                    dispatchEvent.Subtitles += " suspect is armed with a ~r~gat~s~";
                }
                else if (Num == 3)
                {
                    dispatchEvent.SoundsToPlay.Add(carrying_weapon.Carryingafirearm.FileName);
                    dispatchEvent.Subtitles += " suspect is carrying a ~r~firearm~s~";
                }
                else
                {
                    dispatchEvent.SoundsToPlay.Add(carrying_weapon.Carryingagat.FileName);
                    dispatchEvent.Subtitles += " suspect is carrying a ~r~gat~s~";
                }
                dispatchEvent.NotificationText += " Gat";
            }
        }
        private void AddWeaponsFree(DispatchEvent dispatchEvent)
        {
            if (!ReportedWeaponsFree)
            {
                AddAudioSet(dispatchEvent, new AudioSet(new List<string>() { custom_wanted_level_line.Suspectisarmedanddangerousweaponsfree.FileName }, "suspect is armed and dangerous, weapons free"));
                dispatchEvent.NotificationText += "~n~~r~Weapons Free~s~";
                ReportedWeaponsFree = true;
            }
        }
        private void AddZone(DispatchEvent dispatchEvent)
        {
            if (RecentlyMentionedZone)
            {
                return;
            }
            Zone MyZone = CurrentPlayer.CurrentLocation.CurrentZone;
            if (MyZone != null)
            {
                string ScannerAudio = ZoneScannerAudio.GetAudio(MyZone.InternalGameName);
                if (ScannerAudio != "")
                {
                    dispatchEvent.HasZoneAudio = true;
                    if (MyZone.IsSpecificLocation || Settings.SettingsManager.PlayerSettings.Scanner_UseNearForLocations)
                    {
                        dispatchEvent.SoundsToPlay.Add(new List<string> { conjunctives.Nearumm.FileName, conjunctives.Closetoum.FileName, conjunctives.Closetouhh.FileName }.PickRandom());
                        dispatchEvent.Subtitles += " ~s~near ~p~" + MyZone.DisplayName + "~s~";
                    }
                    else
                    {
                        dispatchEvent.SoundsToPlay.Add(new List<string> { conjunctives.In.FileName, conjunctives.Inuhh.FileName, conjunctives.Inuhh2.FileName, conjunctives.Inuhh3.FileName }.PickRandom());
                        dispatchEvent.Subtitles += " ~s~in ~p~" + MyZone.DisplayName + "~s~";
                    }
                    dispatchEvent.SoundsToPlay.Add(ScannerAudio);
                    dispatchEvent.NotificationText += "~n~~p~" + MyZone.DisplayName + "~s~";
                }
            }
        }
        private void AddAttentionUnits(DispatchEvent dispatchEvent)
        {
            Zone MyZone = CurrentPlayer.CurrentLocation.CurrentZone;
            if (MyZone != null)
            {
                ZoneLookup zoneAudio = ZoneScannerAudio.GetLookup(MyZone.InternalGameName);
                if (zoneAudio != null)
                {
                    string ScannerAudio = zoneAudio.ScannerUnitValues.PickRandom();
                    if (ScannerAudio != "")
                    {
                        dispatchEvent.SoundsToPlay.Add(ScannerAudio);
                    }
                }
            }
        }
        private void BuildDispatch(Dispatch DispatchToPlay, bool addtoPlayed)
        {
            EntryPoint.WriteToConsole($"SCANNER EVENT: Building {DispatchToPlay.Name}, MarkVehicleAsStolen: {DispatchToPlay.MarkVehicleAsStolen} Vehicle: {DispatchToPlay.LatestInformation?.VehicleSeen?.Vehicle.Handle} Instances: {DispatchToPlay.LatestInformation?.InstancesObserved}", 3);
            DispatchEvent EventToPlay = new DispatchEvent();
            if (DispatchToPlay.HasPreamble)
            {
                EventToPlay.SoundsToPlay.Add(RadioStart.PickRandom());
                AddAudioSet(EventToPlay, DispatchToPlay.PreambleAudioSet.PickRandom());
                EventToPlay.SoundsToPlay.Add(RadioEnd.PickRandom());
            }
            EventToPlay.SoundsToPlay.Add(RadioStart.PickRandom());
            EventToPlay.NotificationTitle = DispatchToPlay.NotificationTitle;
            if (DispatchToPlay.IsStatus)
            {
                EventToPlay.NotificationSubtitle = "~g~Status";
            }
            else if (DispatchToPlay.LatestInformation.SeenByOfficers)
            {
                EventToPlay.NotificationSubtitle = "~r~Crime Observed";
            }
            else
            {
                EventToPlay.NotificationSubtitle = "~o~Crime Reported";
            }
            EventToPlay.NotificationText = DispatchToPlay.NotificationText;
            if (DispatchToPlay.IncludeAttentionAllUnits)
            {
                AddAudioSet(EventToPlay, AttentionAllUnits.PickRandom());
            }
            else if(!DispatchToPlay.LatestInformation.SeenByOfficers && !DispatchToPlay.IsStatus)
            {
                AddAttentionUnits(EventToPlay);
            }



            if (DispatchToPlay.IncludeReportedBy)
            {
                if (DispatchToPlay.LatestInformation.SeenByOfficers)
                {
                    AddAudioSet(EventToPlay, OfficersReport.PickRandom());
                }
                else
                {
                    AddAudioSet(EventToPlay, CiviliansReport.PickRandom());
                }
            }
            if (DispatchToPlay.LatestInformation.InstancesObserved > 1 && DispatchToPlay.MainMultiAudioSet.Any())
            {
                AddAudioSet(EventToPlay, DispatchToPlay.MainMultiAudioSet.PickRandom());
            }
            else
            {
                AddAudioSet(EventToPlay, DispatchToPlay.MainAudioSet.PickRandom());
            }
            if (DispatchToPlay.SecondaryAudioSet.Any())
            {
                AddAudioSet(EventToPlay, DispatchToPlay.SecondaryAudioSet.PickRandom());
            }
            if (DispatchToPlay.IncludeDrivingVehicle)
            {
                AddVehicleDescription(EventToPlay, DispatchToPlay.LatestInformation.VehicleSeen, !DispatchToPlay.LatestInformation.SeenByOfficers && DispatchToPlay.IncludeLicensePlate, DispatchToPlay);
                GameFiber.Yield();
            }
            if (DispatchToPlay.IncludeRapSheet)
            {
                AddRapSheet(EventToPlay);
            }
            if (DispatchToPlay.MarkVehicleAsStolen && DispatchToPlay.LatestInformation != null && DispatchToPlay.LatestInformation.VehicleSeen != null && CurrentPlayer.CurrentVehicle != null)//temp current vehicle BS
            {
                //THIS NEED TO NOT BE CURRENT VEHICLE, BUT OTHERWISE THE LINK GETS MESSED UP?
                CurrentPlayer.CurrentVehicle.WasReportedStolen = true;
                CurrentPlayer.CurrentVehicle.OriginalLicensePlate.IsWanted = true;
                if (CurrentPlayer.CurrentVehicle.OriginalLicensePlate.PlateNumber == CurrentPlayer.CurrentVehicle.CarPlate.PlateNumber)
                {
                    CurrentPlayer.CurrentVehicle.CarPlate.IsWanted = true;
                }
            }
            if (DispatchToPlay.IncludeCarryingWeapon && (DispatchToPlay.LatestInformation.WeaponSeen != null || DispatchToPlay.Name == "Carrying Weapon"))
            {
                AddWeaponDescription(EventToPlay, DispatchToPlay.LatestInformation.WeaponSeen);
                GameFiber.Yield();
            }
            if (DispatchToPlay.ResultsInLethalForce && !LethalForceAuthorized.HasBeenPlayedThisWanted && DispatchToPlay.Name != LethalForceAuthorized.Name)
            {
                AddLethalForce(EventToPlay);
            }
            if (CurrentPlayer.IsWanted && !CurrentPlayer.IsDead && CurrentPlayer.PoliceResponse.IsWeaponsFree && !WeaponsFree.HasBeenPlayedThisWanted && DispatchToPlay.Name != WeaponsFree.Name)
            {
                AddWeaponsFree(EventToPlay);
            }
            if (CurrentPlayer.IsWanted && !CurrentPlayer.IsDead && World.AnyHelicopterUnitsSpawned && !RequestAirSupport.HasBeenPlayedThisWanted && DispatchToPlay.Name != RequestAirSupport.Name)
            {
                AddRequestAirSupport(EventToPlay);
            }
            if (DispatchToPlay.IncludeDrivingSpeed && CurrentPlayer.CurrentVehicle != null && CurrentPlayer.CurrentVehicle.Vehicle.Exists())
            {
                AddSpeed(EventToPlay, CurrentPlayer.CurrentVehicle.Vehicle.Speed);
                GameFiber.Yield();
            }
            if (DispatchToPlay.LocationDescription != LocationSpecificity.Nothing)
            {
                AddLocationDescription(EventToPlay, DispatchToPlay.LocationDescription);
                GameFiber.Yield();
            }
            if (CurrentPlayer.PoliceResponse.PoliceHaveDescription && !DispatchToPlay.LatestInformation.SeenByOfficers && !DispatchToPlay.IsStatus)
            {
                AddHaveDescription(EventToPlay);
            }
            EventToPlay.SoundsToPlay.Add(RadioEnd.PickRandom());
            EventToPlay.Subtitles = NativeHelper.FirstCharToUpper(EventToPlay.Subtitles);
            EventToPlay.Priority = DispatchToPlay.Priority;

            if (addtoPlayed)
            {
                DispatchToPlay.SetPlayed();
                if (DispatchToPlay.LatestInformation.SeenByOfficers && DispatchToPlay.Priority < HighestOfficerReportedPriority)
                {
                    HighestOfficerReportedPriority = DispatchToPlay.Priority;
                }
                else if (!DispatchToPlay.LatestInformation.SeenByOfficers && !DispatchToPlay.IsStatus && DispatchToPlay.Priority < HighestCivilianReportedPriority)
                {
                    HighestCivilianReportedPriority = DispatchToPlay.Priority;
                }
            }

            PlayDispatch(EventToPlay, DispatchToPlay.LatestInformation);
        }
        private void CheckDispatch()
        {
            if (CurrentPlayer.RecentlyStartedPlaying)
            {
                return;//don't care right when you become a new person
            }
            // CheckCrimesToAnnounce();
            CheckStatusToAnnounce();
        }
        private void CheckStatusToAnnounce()
        {
            if (CurrentPlayer.IsWanted && CurrentPlayer.IsAliveAndFree && Settings.SettingsManager.PlayerSettings.Scanner_AllowStatusAnnouncements)
            {
                if (CurrentPlayer.PoliceResponse.HasBeenWantedFor > 25000)
                {
                    if (!SuspectSpotted.HasRecentlyBeenPlayed && !VeryRecentlyAnnouncedDispatch && CurrentPlayer.AnyPoliceCanSeePlayer)
                    {
                        EntryPoint.WriteToConsole($"SCANNER EVENT: ADDED SuspectSpotted", 3);
                        AddToQueue(SuspectSpotted, new CrimeSceneDescription(!CurrentPlayer.IsInVehicle, true, Game.LocalPlayer.Character.Position));
                    }
                    else if (!CurrentPlayer.AnyPoliceRecentlySeenPlayer && !AttemptToReacquireSuspect.HasRecentlyBeenPlayed && !SuspectEvaded.HasRecentlyBeenPlayed)
                    {
                        EntryPoint.WriteToConsole($"SCANNER EVENT: ADDED AttemptToReacquireSuspect", 3);
                        AddToQueue(AttemptToReacquireSuspect, new CrimeSceneDescription(false, true, CurrentPlayer.PlacePoliceLastSeenPlayer));
                    }
                }
            }
            else
            {
                foreach (VehicleExt StolenCar in CurrentPlayer.ReportedStolenVehicles)
                {
                    StolenCar.AddedToReportedStolenQueue = true;
                    AddToQueue(AnnounceStolenVehicle, new CrimeSceneDescription(!CurrentPlayer.IsInVehicle, false, StolenCar.PlaceOriginallyEntered) { VehicleSeen = StolenCar });
                }
            }
        }
        private void DefaultConfig()
        {
            SetupDispatches();
            DispatchLookup = new List<CrimeDispatch>
        {
            new CrimeDispatch("AttemptingSuicide",AttemptingSuicide),
            new CrimeDispatch("BrandishingWeapon",CarryingWeapon),
            new CrimeDispatch("ChangingPlates",TamperingWithVehicle),
            new CrimeDispatch("DrivingAgainstTraffic",RecklessDriving),
            new CrimeDispatch("DrivingOnPavement",RecklessDriving),
            new CrimeDispatch("FelonySpeeding",FelonySpeeding),
            new CrimeDispatch("FiringWeapon",ShotsFired),
            new CrimeDispatch("FiringWeaponNearPolice",ShotsFiredAtAnOfficer),
            new CrimeDispatch("GotInAirVehicleDuringChase",StealingAirVehicle),
            new CrimeDispatch("GrandTheftAuto",GrandTheftAuto),
            new CrimeDispatch("HitCarWithCar",VehicleHitAndRun),
            new CrimeDispatch("HitPedWithCar",PedHitAndRun),
            new CrimeDispatch("RunningARedLight",RunningARedLight),
            new CrimeDispatch("HurtingCivilians",CivilianInjury),
            new CrimeDispatch("HurtingPolice",AssaultingOfficer),
            new CrimeDispatch("KillingCivilians",CivilianDown),
            new CrimeDispatch("KillingPolice",OfficerDown),
            new CrimeDispatch("Mugging",Mugging),
            new CrimeDispatch("NonRoadworthyVehicle",SuspiciousVehicle),
            new CrimeDispatch("ResistingArrest",ResistingArrest),
            new CrimeDispatch("TrespessingOnGovtProperty",TrespassingOnGovernmentProperty),
            new CrimeDispatch("DrivingStolenVehicle",DrivingAtStolenVehicle),
            new CrimeDispatch("TerroristActivity",TerroristActivity),
            new CrimeDispatch("BrandishingCloseCombatWeapon",CarryingWeapon),
            new CrimeDispatch("SuspiciousActivity",SuspiciousActivity),
            new CrimeDispatch("DrunkDriving",DrunkDriving),
            new CrimeDispatch("Kidnapping",Kidnapping),
            new CrimeDispatch("PublicIntoxication",PublicIntoxication),
            new CrimeDispatch("InsultingOfficer",OfficerNeedsAssistance),//these are bad
            new CrimeDispatch("OfficersNeeded",OfficersNeeded),
            new CrimeDispatch("Harassment",Harassment),
            new CrimeDispatch("AssaultingCivilians",AssaultingCivilians),
            new CrimeDispatch("AssaultingWithDeadlyWeapon",AssaultingCiviliansWithDeadlyWeapon),
            new CrimeDispatch("DealingDrugs",DealingDrugs),
            new CrimeDispatch("AimingWeaponAtPolice",AimingWeaponAtPolice),

        };
            DispatchList = new List<Dispatch>
        {
            OfficerDown
            ,ShotsFiredAtAnOfficer
            ,AssaultingOfficer
            ,ThreateningOfficerWithFirearm
            ,TrespassingOnGovernmentProperty
            ,StealingAirVehicle
            ,ShotsFired
            ,CarryingWeapon
            ,CivilianDown
            ,CivilianShot
            ,CivilianInjury
            ,GrandTheftAuto
            ,SuspiciousActivity
            ,CriminalActivity
            ,Mugging
            ,TerroristActivity
            ,SuspiciousVehicle
            ,DrivingAtStolenVehicle
            ,ResistingArrest
            ,AttemptingSuicide
            ,FelonySpeeding
            ,PedHitAndRun
            ,VehicleHitAndRun
            ,RecklessDriving
            ,AnnounceStolenVehicle
            ,RequestAirSupport
            ,RequestMilitaryUnits
            ,RequestNOOSEUnits
            ,SuspectSpotted
            ,SuspectEvaded
            ,RemainInArea
            ,ResumePatrol
            ,AttemptToReacquireSuspect
            ,NoFurtherUnitsNeeded
            ,SuspectArrested
            ,SuspectWasted
            ,ChangedVehicles
            ,RequestBackup
            ,WeaponsFree
            ,LethalForceAuthorized
            ,RunningARedLight
            ,DrunkDriving
            ,Kidnapping
            ,PublicIntoxication
            ,Harassment
            ,OfficerNeedsAssistance
            ,OfficersNeeded
            ,AssaultingCivilians
            ,AssaultingCiviliansWithDeadlyWeapon
            ,DealingDrugs
        };
        }
        private Dispatch DetermineDispatchFromCrime(Crime crimeAssociated)
        {
            CrimeDispatch ToLookup = DispatchLookup.FirstOrDefault(x => x.CrimeID == crimeAssociated.ID);
            if (ToLookup != null && ToLookup.Dispatch != null)
            {
                ToLookup.Dispatch.Priority = crimeAssociated.Priority;
                return ToLookup.Dispatch;
            }
            return null;
        }
        private void PlayDispatch(DispatchEvent MyAudioEvent, CrimeSceneDescription MyDispatch)
        {
            //EntryPoint.WriteToConsole($"Scanner Start. Playing: {string.Join(",", MyAudioEvent.SoundsToPlay)}",5);
            if (MyAudioEvent.CanInterrupt && CurrentlyPlaying != null && CurrentlyPlaying.CanBeInterrupted && MyAudioEvent.Priority < CurrentlyPlaying.Priority)
            {
                EntryPoint.WriteToConsole(string.Format("ScannerScript ABORT! Incoming: {0}, Playing: {1}", MyAudioEvent.NotificationText, CurrentlyPlaying.NotificationText), 4);
                AbortedAudio = true;
                Abort();
            }
            if (CurrentlyPlaying != null && CurrentlyPlayingCallIn != null && !CurrentlyPlayingCallIn.SeenByOfficers && MyDispatch.SeenByOfficers)
            {
                EntryPoint.WriteToConsole(string.Format("ScannerScript ABORT! OFFICER REPORTED STOPPING CIV REPORTING Incoming: {0}, Playing: {1}", MyAudioEvent.NotificationText, CurrentlyPlaying.NotificationText), 4);
                AbortedAudio = true;
                Abort();
            }
            GameFiber PlayAudioList = GameFiber.StartNew(delegate
            {
                if (AbortedAudio)
                {
                    EntryPoint.WriteToConsole($"Scanner Aborted. Incoming: {string.Join(",", MyAudioEvent.SoundsToPlay)}",5);
                    AudioPlayer.Play(RadioEnd.PickRandom(), Settings.SettingsManager.PlayerSettings.Scanner_AudioVolume);
                    AbortedAudio = false;
                    GameFiber.Sleep(1000);
                }
                while (AudioPlayer.IsAudioPlaying)
                {
                    GameFiber.Yield();
                }
                if (MyAudioEvent.NotificationTitle != "" && Settings.SettingsManager.PlayerSettings.Scanner_EnableNotifications)
                {
                    RemoveAllNotifications();
                    NotificationHandles.Add(Game.DisplayNotification("CHAR_CALL911", "CHAR_CALL911", MyAudioEvent.NotificationTitle, MyAudioEvent.NotificationSubtitle, MyAudioEvent.NotificationText));
                }
                CurrentlyPlaying = MyAudioEvent;
                CurrentlyPlayingCallIn = MyDispatch;

                if (Settings.SettingsManager.PlayerSettings.Scanner_EnableAudio)
                {
                    foreach (string audioname in MyAudioEvent.SoundsToPlay)
                    {
                        //EntryPoint.WriteToConsole($"Scanner Playing. ToAudioPlayer: {audioname}", 5);
                        AudioPlayer.Play(audioname, Settings.SettingsManager.PlayerSettings.Scanner_AudioVolume);
                        while (AudioPlayer.IsAudioPlaying)
                        {
                            if (MyAudioEvent.Subtitles != "" && Settings.SettingsManager.PlayerSettings.Scanner_EnableSubtitles && Game.GameTime - GameTimeLastDisplayedSubtitle >= 1500)
                            {
                                Game.DisplaySubtitle(MyAudioEvent.Subtitles, 2000);
                                GameTimeLastDisplayedSubtitle = Game.GameTime;
                            }
                            GameTimeLastAnnouncedDispatch = Game.GameTime;
                            if (MyAudioEvent.HasStreetAudio)
                            {
                                GameTimeLastMentionedStreet = Game.GameTime;
                            }
                            if (MyAudioEvent.HasZoneAudio)
                            {
                                GameTimeLastMentionedZone = Game.GameTime;
                            }
                            GameFiber.Yield();
                            if (AbortedAudio)
                            {
                                EntryPoint.WriteToConsole($"AbortedAudio1", 5);
                                break;
                            }
                        }
                        if (AbortedAudio)
                        {
                            EntryPoint.WriteToConsole($"AbortedAudio2", 5);
                            break;
                        }
                    }
                }
                if (AbortedAudio)
                {
                    AbortedAudio = false;
                }
                CurrentlyPlaying = null;
                if (MyDispatch.VehicleSeen != null)
                {
                    MyDispatch.VehicleSeen.HasBeenDescribedByDispatch = true;
                }
            }, "PlayAudioList");
        }
        private void RemoveAllNotifications()
        {
            foreach (uint handles in NotificationHandles)
            {
                Game.RemoveNotification(handles);
            }
            NotificationHandles.Clear();
        }
        private void SetupDispatches()
        {
            RadioStart = new List<string>() { AudioBeeps.Radio_Start_1.FileName };
            RadioEnd = new List<string>() { AudioBeeps.Radio_End_1.FileName };
            AttentionAllUnits = new List<AudioSet>()
        {
            new AudioSet(new List<string>() { attention_all_units_gen.Attentionallunits.FileName},"attention all units"),
            new AudioSet(new List<string>() { attention_all_units_gen.Attentionallunits1.FileName },"attention all units"),
            new AudioSet(new List<string>() { attention_all_units_gen.Attentionallunits3.FileName },"attention all units"),
        };
            OfficersReport = new List<AudioSet>()
        {
            new AudioSet(new List<string>() { we_have.OfficersReport_1.FileName},"officers report"),
            new AudioSet(new List<string>() { we_have.OfficersReport_2.FileName },"officers report"),
            new AudioSet(new List<string>() { we_have.UnitsReport_1.FileName },"units report"),
        };
            CiviliansReport = new List<AudioSet>()
        {
            new AudioSet(new List<string>() { we_have.CitizensReport_1.FileName },"citizens report"),
            new AudioSet(new List<string>() { we_have.CitizensReport_2.FileName },"citizens report"),
            new AudioSet(new List<string>() { we_have.CitizensReport_3.FileName },"citizens report"),
            new AudioSet(new List<string>() { we_have.CitizensReport_4.FileName },"citizens report"),
        };
            LethalForce = new List<AudioSet>()
        {
            new AudioSet(new List<string>() { lethal_force.Useofdeadlyforceauthorized.FileName},"use of deadly force authorized"),
            new AudioSet(new List<string>() { lethal_force.Useofdeadlyforceisauthorized.FileName },"use of deadly force is authorized"),
            new AudioSet(new List<string>() { lethal_force.Useofdeadlyforceisauthorized1.FileName },"use of deadly force is authorized"),
            new AudioSet(new List<string>() { lethal_force.Useoflethalforceisauthorized.FileName },"use of lethal force is authorized"),
            new AudioSet(new List<string>() { lethal_force.Useofdeadlyforcepermitted1.FileName },"use of deadly force permitted"),
        };
            LicensePlateSet = new List<AudioSet>()
        {
            new AudioSet(new List<string>() { suspect_license_plate.SuspectLicensePlate.FileName},"suspect license plate"),
            new AudioSet(new List<string>() { suspect_license_plate.SuspectsLicensePlate01.FileName },"suspects license plate"),
            new AudioSet(new List<string>() { suspect_license_plate.SuspectsLicensePlate02.FileName },"suspects license plate"),
            new AudioSet(new List<string>() { suspect_license_plate.TargetLicensePlate.FileName },"target license plate"),
            new AudioSet(new List<string>() { suspect_license_plate.TargetsLicensePlate.FileName },"targets license plate"),
            new AudioSet(new List<string>() { suspect_license_plate.TargetVehicleLicensePlate.FileName },"target vehicle license plate"),
        };
            OfficerDown = new Dispatch()
            {
                Name = "Officer Down",
                IncludeAttentionAllUnits = true,
                ResultsInLethalForce = true,

                LocationDescription = LocationSpecificity.StreetAndZone,
                MainAudioSet = new List<AudioSet>()
            {
                new AudioSet(new List<string>() { we_have.We_Have_1.FileName, crime_officer_down.AcriticalsituationOfficerdown.FileName },"we have a critical situation, officer down"),
                new AudioSet(new List<string>() { we_have.We_Have_1.FileName, crime_officer_down.AnofferdownpossiblyKIA.FileName },"we have an officer down, possibly KIA"),
                new AudioSet(new List<string>() { we_have.We_Have_1.FileName, crime_officer_down.Anofficerdown.FileName },"we have an officer down"),
                new AudioSet(new List<string>() { we_have.We_Have_2.FileName, crime_officer_down.Anofficerdownconditionunknown.FileName },"we have an officer down, condition unknown"),
            },
                SecondaryAudioSet = new List<AudioSet>()
            {
                new AudioSet(new List<string>() { dispatch_respond_code.AllunitsrespondCode99.FileName },"all units repond code-99"),
                new AudioSet(new List<string>() { dispatch_respond_code.AllunitsrespondCode99emergency.FileName },"all units repond code-99 emergency"),
                new AudioSet(new List<string>() { dispatch_respond_code.Code99allunitsrespond.FileName },"code-99 all units repond"),
                new AudioSet(new List<string>() { custom_wanted_level_line.Code99allavailableunitsconvergeonsuspect.FileName },"code-99 all available units converge on suspect"),
                new AudioSet(new List<string>() { custom_wanted_level_line.Wehavea1099allavailableunitsrespond.FileName },"we have a 10-99  all available units repond"),
                new AudioSet(new List<string>() { dispatch_respond_code.Code99allunitsrespond.FileName },"code-99 all units respond"),
                new AudioSet(new List<string>() { dispatch_respond_code.EmergencyallunitsrespondCode99.FileName },"emergency all units respond code-99"),
                new AudioSet(new List<string>() { escort_boss.Immediateassistancerequired.FileName },"immediate assistance required"),
            },
                MainMultiAudioSet = new List<AudioSet>()
            {
                new AudioSet(new List<string>() { we_have.We_Have_1.FileName, crime_officers_down.Multipleofficersdown.FileName },"we have multiple officers down"),
                new AudioSet(new List<string>() { we_have.We_Have_2.FileName, crime_officers_down.Severalofficersdown.FileName },"we have several officers down"),
            },
            };
            ShotsFiredAtAnOfficer = new Dispatch()
            {
                Name = "Shots Fired at an Officer",
                IncludeAttentionAllUnits = true,
                ResultsInLethalForce = true,
                LocationDescription = LocationSpecificity.StreetAndZone,
                MainAudioSet = new List<AudioSet>()
            {
                new AudioSet(new List<string>() { crime_shots_fired_at_an_officer.Shotsfiredatanofficer.FileName },"shots fired at an officer"),
                new AudioSet(new List<string>() { crime_shots_fired_at_officer.Afirearmattackonanofficer.FileName },"a firearm attack on an officer"),
              //  new AudioSet(new List<string>() { crime_shots_fired_at_officer.Anofficershot.FileName },"an officer shot"),
                new AudioSet(new List<string>() { crime_shots_fired_at_officer.Anofficerunderfire.FileName },"a officer under fire"),
                new AudioSet(new List<string>() { crime_shots_fired_at_officer.Shotsfiredatanofficer.FileName },"a shots fired at an officer"),
            },
                SecondaryAudioSet = new List<AudioSet>()
            {
                new AudioSet(new List<string>() { dispatch_respond_code.AllunitsrespondCode99.FileName },"all units repond code-99"),
                new AudioSet(new List<string>() { dispatch_respond_code.AllunitsrespondCode99emergency.FileName },"all units repond code-99 emergency"),
                new AudioSet(new List<string>() { dispatch_respond_code.Code99allunitsrespond.FileName },"code-99 all units repond"),
                new AudioSet(new List<string>() { custom_wanted_level_line.Code99allavailableunitsconvergeonsuspect.FileName },"code-99 all available units converge on suspect"),
                new AudioSet(new List<string>() { custom_wanted_level_line.Wehavea1099allavailableunitsrespond.FileName },"we have a 10-99  all available units repond"),
                new AudioSet(new List<string>() { dispatch_respond_code.Code99allunitsrespond.FileName },"code-99 all units respond"),
                new AudioSet(new List<string>() { dispatch_respond_code.EmergencyallunitsrespondCode99.FileName },"emergency all units respond code-99"),
                new AudioSet(new List<string>() { escort_boss.Immediateassistancerequired.FileName },"immediate assistance required"),
            }
            };
            AssaultingOfficer = new Dispatch()
            {
                Name = "Assault on an Officer",
                LocationDescription = LocationSpecificity.Street,
                MainAudioSet = new List<AudioSet>()
            {
                new AudioSet(new List<string>() { crime_assault_on_an_officer.Anassaultonanofficer.FileName },"an assault on an officer"),
                new AudioSet(new List<string>() { crime_assault_on_an_officer.Anofficerassault.FileName },"an officer assault"),
            },
            };
            ThreateningOfficerWithFirearm = new Dispatch()
            {
                Name = "Threatening an Officer with a Firearm",
                LocationDescription = LocationSpecificity.StreetAndZone,
                MainAudioSet = new List<AudioSet>()
            {
                new AudioSet(new List<string>() { crime_suspect_threatening_an_officer_with_a_firearm.Asuspectthreateninganofficerwithafirearm.FileName },"a suspect threatening an officer with a firearm"),
            },
            };
            TrespassingOnGovernmentProperty = new Dispatch()
            {
                Name = "Trespassing on Government Property",
                ResultsInLethalForce = true,
                LocationDescription = LocationSpecificity.Zone,
                MainAudioSet = new List<AudioSet>()
            {
                new AudioSet(new List<string>() { crime_trespassing_on_government_property.Trespassingongovernmentproperty.FileName },"trespassing on government property"),
            },
            };
            StealingAirVehicle = new Dispatch()
            {
                Name = "Stolen Air Vehicle",
                ResultsInLethalForce = true,
                IncludeDrivingVehicle = true,
                MarkVehicleAsStolen = true,
                LocationDescription = LocationSpecificity.Zone,
                MainAudioSet = new List<AudioSet>()
            {
                new AudioSet(new List<string>() { crime_stolen_aircraft.Astolenaircraft.FileName},"a stolen aircraft"),
                new AudioSet(new List<string>() { crime_hijacked_aircraft.Ahijackedaircraft.FileName },"a hijacked aircraft"),
                new AudioSet(new List<string>() { crime_theft_of_an_aircraft.Theftofanaircraft.FileName },"theft of an aircraft"),
            },
            };
            ShotsFired = new Dispatch()
            {
                Name = "Shots Fired",
                LocationDescription = LocationSpecificity.StreetAndZone,
                MainAudioSet = new List<AudioSet>()
            {
                new AudioSet(new List<string>() { crime_shooting.Afirearmssituationseveralshotsfired.FileName },"a firearms situation, several shots fired"),
                new AudioSet(new List<string>() { crime_shooting.Aweaponsincidentshotsfired.FileName },"a weapons incdient, shots fired"),
                new AudioSet(new List<string>() { crime_shoot_out.Ashootout.FileName },"a shoot-out"),
                new AudioSet(new List<string>() { crime_firearms_incident.AfirearmsincidentShotsfired.FileName },"a firearms incident, shots fired"),
                new AudioSet(new List<string>() { crime_firearms_incident.Anincidentinvolvingshotsfired.FileName },"an incident involving shots fired"),
                new AudioSet(new List<string>() { crime_firearms_incident.AweaponsincidentShotsfired.FileName },"a weapons incident, shots fired"),
            },
            };
            CarryingWeapon = new Dispatch()
            {
                Name = "Carrying Weapon",
                LocationDescription = LocationSpecificity.StreetAndZone,
                IncludeCarryingWeapon = true,
                CanBeReportedMultipleTimes = false,
            };
            TerroristActivity = new Dispatch()
            {
                Name = "Terrorist Activity",
                LocationDescription = LocationSpecificity.StreetAndZone,
                MainAudioSet = new List<AudioSet>()
            {
                new AudioSet(new List<string>() {  crime_terrorist_activity.Possibleterroristactivity.FileName },"possible terrorist activity"),
                new AudioSet(new List<string>() {  crime_terrorist_activity.Possibleterroristactivity1.FileName},"possible terrorist activity"),
                new AudioSet(new List<string>() {  crime_terrorist_activity.Possibleterroristactivity2.FileName },"possible terrorist activity"),
                new AudioSet(new List<string>() {  crime_terrorist_activity.Terroristactivity.FileName },"terrorist activity"),
            },
            };
            CivilianDown = new Dispatch()
            {
                Name = "Civilian Down",
                LocationDescription = LocationSpecificity.StreetAndZone,
                CanBeReportedMultipleTimes = true,
                MainAudioSet = new List<AudioSet>()
            {
                new AudioSet(new List<string>() { crime_civilian_fatality.Acivilianfatality.FileName },"civilian fatality"),
                new AudioSet(new List<string>() { crime_civilian_down.Aciviliandown.FileName },"civilian down"),

                new AudioSet(new List<string>() { crime_1_87.A187.FileName },"a 1-87"),
                new AudioSet(new List<string>() { crime_1_87.Ahomicide.FileName },"a homicide"),
            },
            };
            CivilianShot = new Dispatch()
            {
                Name = "Civilian Shot",
                LocationDescription = LocationSpecificity.Street,
                MainAudioSet = new List<AudioSet>()
            {
                new AudioSet(new List<string>() { crime_civillian_gsw.AcivilianGSW.FileName },"a civilian GSW"),
                new AudioSet(new List<string>() { crime_civillian_gsw.Acivilianshot.FileName },"a civilian shot"),
                new AudioSet(new List<string>() { crime_civillian_gsw.Agunshotwound.FileName },"a gunshot wound"),
            },
            };
            CivilianInjury = new Dispatch()
            {
                Name = "Civilian Injury",
                LocationDescription = LocationSpecificity.StreetAndZone,
                MainAudioSet = new List<AudioSet>()
            {
                new AudioSet(new List<string>() { crime_injured_civilian.Aninjuredcivilian.FileName },"an injured civilian"),
                new AudioSet(new List<string>() { crime_civilian_needing_assistance.Acivilianinneedofassistance.FileName },"a civilian in need of assistance"),
                new AudioSet(new List<string>() { crime_civilian_needing_assistance.Acivilianrequiringassistance.FileName },"a civilian requiring assistance"),
                new AudioSet(new List<string>() { crime_assault_on_a_civilian.Anassaultonacivilian.FileName },"an assault on a civilian"),
            },
            };
            GrandTheftAuto = new Dispatch()
            {
                Name = "Grand Theft Auto",
                IncludeDrivingVehicle = true,
                MarkVehicleAsStolen = true,
                IncludeLicensePlate = true,
                IncludeCarryingWeapon = true,
                LocationDescription = LocationSpecificity.HeadingAndStreet,
                MainAudioSet = new List<AudioSet>()
            {
                new AudioSet(new List<string>() { crime_grand_theft_auto.Agrandtheftauto.FileName },"a grand theft auto"),
                new AudioSet(new List<string>() { crime_grand_theft_auto.Agrandtheftautoinprogress.FileName },"a grand theft auto in progress"),
                new AudioSet(new List<string>() { crime_grand_theft_auto.AGTAinprogress.FileName },"a GTA in progress"),
                new AudioSet(new List<string>() { crime_grand_theft_auto.AGTAinprogress1.FileName },"a GTA in progress"),
            },
            };
            SuspiciousActivity = new Dispatch()
            {
                Name = "Suspicious Activity",
                LocationDescription = LocationSpecificity.StreetAndZone,
                IncludeCarryingWeapon = true,
                MainAudioSet = new List<AudioSet>()
            {
                new AudioSet(new List<string>() { crime_suspicious_activity.Suspiciousactivity.FileName },"suspicious activity"),
                new AudioSet(new List<string>() { crime_9_25.Asuspiciousperson.FileName },"a suspicious person"),
            },
            };
            TamperingWithVehicle = new Dispatch()
            {
                Name = "Tampering With Vehicle",
                LocationDescription = LocationSpecificity.StreetAndZone,
                IncludeCarryingWeapon = true,
                MainAudioSet = new List<AudioSet>()
            {
                new AudioSet(new List<string>() { crime_5_04.Tamperingwithavehicle.FileName },"tampering with a vehicle"),
            },
            };
            CriminalActivity = new Dispatch()
            {
                Name = "Criminal Activity",
                LocationDescription = LocationSpecificity.Street,
                IncludeCarryingWeapon = true,
                MainAudioSet = new List<AudioSet>()
            {
                new AudioSet(new List<string>() { crime_criminal_activity.Criminalactivity.FileName },"criminal activity"),
                new AudioSet(new List<string>() { crime_criminal_activity.Illegalactivity.FileName },"illegal activity"),
                new AudioSet(new List<string>() { crime_criminal_activity.Prohibitedactivity.FileName },"prohibited activity"),
            },
            };
            Mugging = new Dispatch()
            {
                Name = "Mugging",
                LocationDescription = LocationSpecificity.Street,
                IncludeCarryingWeapon = true,
                MainAudioSet = new List<AudioSet>()
            {
                new AudioSet(new List<string>() { crime_mugging.Apossiblemugging.FileName },"a possible mugging"),
            },
            };
            TerroristActivity = new Dispatch()
            {
                Name = "Terrorist Activity",
                LocationDescription = LocationSpecificity.Street,
                MainAudioSet = new List<AudioSet>()
            {
                new AudioSet(new List<string>() { crime_terrorist_activity.Possibleterroristactivity.FileName },"possible terrorist activity in progress"),
                new AudioSet(new List<string>() { crime_terrorist_activity.Possibleterroristactivity1.FileName },"possible terrorist activity in progress"),
                new AudioSet(new List<string>() { crime_terrorist_activity.Possibleterroristactivity2.FileName },"possible terrorist activity in progress"),
                new AudioSet(new List<string>() { crime_terrorist_activity.Terroristactivity.FileName },"terrorist activity"),
            },
            };
            SuspiciousVehicle = new Dispatch()
            {
                Name = "Suspicious Vehicle",
                IncludeDrivingVehicle = true,
                LocationDescription = LocationSpecificity.StreetAndZone,
                MainAudioSet = new List<AudioSet>()
            {
                new AudioSet(new List<string>() { crime_suspicious_vehicle.Asuspiciousvehicle.FileName },"a suspicious vehicle"),
            },
            };
            DrivingAtStolenVehicle = new Dispatch()
            {
                Name = "Driving a Stolen Vehicle",
                IncludeDrivingVehicle = true,
                LocationDescription = LocationSpecificity.HeadingAndStreet,
                IncludeDrivingSpeed = true,
                MainAudioSet = new List<AudioSet>()
            {
                new AudioSet(new List<string>() { crime_person_in_a_stolen_car.Apersoninastolencar.FileName},"a person in a stolen car"),
                new AudioSet(new List<string>() { crime_person_in_a_stolen_vehicle.Apersoninastolenvehicle.FileName },"a person in a stolen vehicle"),
            },
            };
            ResistingArrest = new Dispatch()
            {
                Name = "Resisting Arrest",
                LocationDescription = LocationSpecificity.Zone,
                IncludeCarryingWeapon = true,
                CanBeReportedMultipleTimes = false,
                CanAlwaysBeInterrupted = true,
                MainAudioSet = new List<AudioSet>()
            {
                new AudioSet(new List<string>() { crime_person_resisting_arrest.Apersonresistingarrest.FileName },"a person resisting arrest"),
                new AudioSet(new List<string>() { crime_suspect_resisting_arrest.Asuspectresistingarrest.FileName },"a suspect resisiting arrest"),

                new AudioSet(new List<string>() { crime_1_48_resist_arrest.Acriminalresistingarrest.FileName },"a criminal resisiting arrest"),
                new AudioSet(new List<string>() { crime_1_48_resist_arrest.Acriminalresistingarrest1.FileName },"a criminal resisiting arrest"),
                new AudioSet(new List<string>() { crime_1_48_resist_arrest.Asuspectfleeingacrimescene.FileName },"a suspect fleeing a crime scene"),
                new AudioSet(new List<string>() { crime_1_48_resist_arrest.Asuspectontherun.FileName },"a suspect on the run"),
            }
            };
            AttemptingSuicide = new Dispatch()
            {
                Name = "Suicide Attempt",
                LocationDescription = LocationSpecificity.Street,
                MainAudioSet = new List<AudioSet>()
            {
                new AudioSet(new List<string>() { crime_9_14a_attempted_suicide.Apossibleattemptedsuicide.FileName },"a possible attempted suicide"),
                new AudioSet(new List<string>() { crime_9_14a_attempted_suicide.Anattemptedsuicide.FileName },"an attempted suicide")
            }
            };
            FelonySpeeding = new Dispatch()
            {
                Name = "Felony Speeding",
                IncludeDrivingVehicle = true,
                VehicleIncludesIn = true,
                IncludeDrivingSpeed = true,
                LocationDescription = LocationSpecificity.Street,
                CanAlwaysBeInterrupted = true,
                MainAudioSet = new List<AudioSet>()
            {
                new AudioSet(new List<string>() { crime_speeding_felony.Aspeedingfelony.FileName },"a speeding felony"),
                new AudioSet(new List<string>() { crime_5_10.A510.FileName,crime_5_10.Speedingvehicle.FileName },"a 5-10, speeding vehicle"),
            },
            };
            PedHitAndRun = new Dispatch()
            {
                Name = "Pedestrian Hit-and-Run",
                LocationDescription = LocationSpecificity.Street,
                CanAlwaysBeInterrupted = true,
                CanBeReportedMultipleTimes = true,
                MainAudioSet = new List<AudioSet>()
            {
                new AudioSet(new List<string>() { crime_ped_struck_by_veh.Apedestrianstruck.FileName},"a pedestrian struck"),
                new AudioSet(new List<string>() { crime_ped_struck_by_veh.Apedestrianstruck1.FileName },"a pedestrian struck"),
                new AudioSet(new List<string>() { crime_ped_struck_by_veh.Apedestrianstruckbyavehicle.FileName },"a pedestrian struck by a vehicle"),
                new AudioSet(new List<string>() { crime_ped_struck_by_veh.Apedestrianstruckbyavehicle1.FileName },"a pedestrian struck by a vehicle"),
            },
            };
            VehicleHitAndRun = new Dispatch()
            {
                Name = "Motor Vehicle Accident",
                LocationDescription = LocationSpecificity.Street,
                CanAlwaysBeInterrupted = true,
                CanBeReportedMultipleTimes = true,
                MainAudioSet = new List<AudioSet>()
            {
                new AudioSet(new List<string>() { crime_motor_vehicle_accident.Amotorvehicleaccident.FileName},"a motor vehicle accident"),
                new AudioSet(new List<string>() { crime_motor_vehicle_accident.AnAEincident.FileName },"an A&E incident"),
                new AudioSet(new List<string>() { crime_motor_vehicle_accident.AseriousMVA.FileName },"a serious MVA"),
            },
            };
            RunningARedLight = new Dispatch()
            {
                Name = "Running a Red Light",
                LocationDescription = LocationSpecificity.Street,
                CanAlwaysBeInterrupted = true,
                MainAudioSet = new List<AudioSet>()
            {
                new AudioSet(new List<string>() { crime_person_running_a_red_light.Apersonrunningaredlight.FileName},"a person running a red light"),
            },
            };
            RecklessDriving = new Dispatch()
            {
                Name = "Reckless Driving",
                LocationDescription = LocationSpecificity.Street,
                CanAlwaysBeInterrupted = true,
                MainAudioSet = new List<AudioSet>()
            {
                new AudioSet(new List<string>() { crime_reckless_driver.Arecklessdriver.FileName},"a reckless driver"),
                new AudioSet(new List<string>() { crime_5_05.A505.FileName,crime_5_05.Adriveroutofcontrol.FileName },"a 505, a driver out of control"),
            },
            };
            DrunkDriving = new Dispatch()
            {
                Name = "Drunk Driving",
                LocationDescription = LocationSpecificity.Street,
                CanAlwaysBeInterrupted = true,
                MainAudioSet = new List<AudioSet>()
            {
                new AudioSet(new List<string>() { crime_5_02.Adriverundertheinfluence.FileName},"a driver under the influence"),
                new AudioSet(new List<string>() { crime_5_02.Adriverundertheinfluence1.FileName},"a driver under the influence"),
                new AudioSet(new List<string>() { crime_5_02.ADUI.FileName},"a dui"),
                new AudioSet(new List<string>() { crime_5_02.A502DUI.FileName},"a 502 dui"),
            },
            };
            AssaultingCivilians = new Dispatch()
            {
                Name = "Assault on a Civilian",
                LocationDescription = LocationSpecificity.Street,
                CanAlwaysBeInterrupted = true,
                MainAudioSet = new List<AudioSet>()
            {
                new AudioSet(new List<string>() { crime_assault.Apossibleassault.FileName},"a possible assault"),
                new AudioSet(new List<string>() { crime_assault.Apossibleassault1.FileName},"a possible assault"),
                new AudioSet(new List<string>() { crime_assault_on_a_civilian.Anassaultonacivilian.FileName},"an assault on a civilian"),
                new AudioSet(new List<string>() { crime_assault_and_battery.AnAE.FileName},"an A&B"),
                new AudioSet(new List<string>() { crime_assault_and_battery.Anassaultandbattery.FileName},"an assault and battery"),
            },
            };
            AimingWeaponAtPolice = new Dispatch()
            {
                Name = "Threatening Officer With Firearm",
                LocationDescription = LocationSpecificity.Street,
                CanAlwaysBeInterrupted = true,
                MainAudioSet = new List<AudioSet>()
            {
                new AudioSet(new List<string>() { crime_suspect_threatening_an_officer_with_a_firearm.Asuspectthreateninganofficerwithafirearm.FileName},"a suspect threatening and officer with a firearm"),
            },
            };
            DealingDrugs = new Dispatch()
            {
                Name = "Dealing Drugs",
                LocationDescription = LocationSpecificity.Street,
                CanAlwaysBeInterrupted = true,
                MainAudioSet = new List<AudioSet>()
            {
                new AudioSet(new List<string>() { crime_drug_deal.Adrugdeal.FileName},"a drug deal"),
                new AudioSet(new List<string>() { crime_drug_deal.Adrugdealinprogress.FileName},"a drug deal in progress"),
                new AudioSet(new List<string>() { crime_drug_deal.Apossibledrugdeal.FileName},"a possible drug deal"),
                new AudioSet(new List<string>() { crime_drug_deal.Narcoticstrafficking.FileName},"narcotics trafficing"),
            },
            };
            AssaultingCiviliansWithDeadlyWeapon = new Dispatch()
            {
                Name = "Assault With a Deadly Weapon",
                LocationDescription = LocationSpecificity.Street,
                CanAlwaysBeInterrupted = true,
                MainAudioSet = new List<AudioSet>()
            {
                new AudioSet(new List<string>() { crime_assault_with_a_deadly_weapon.Assaultwithadeadlyweapon.FileName},"an assault with a deadly weapon"),
                new AudioSet(new List<string>() { crime_assault_with_a_deadly_weapon.AnADW.FileName},"an ADW"),
            },
            };
            Kidnapping = new Dispatch()
            {
                Name = "Kidnapping",
                LocationDescription = LocationSpecificity.Street,
                CanAlwaysBeInterrupted = true,
                MainAudioSet = new List<AudioSet>()
            {
                new AudioSet(new List<string>() { crime_2_07.Akidnapping.FileName},"a kidnapping"),
                new AudioSet(new List<string>() { crime_2_07.Akidnapping1.FileName},"a kidnapping"),
            },
            };
            PublicIntoxication = new Dispatch()
            {
                Name = "Public Intoxication",
                LocationDescription = LocationSpecificity.Street,
                CanAlwaysBeInterrupted = true,

                MainAudioSet = new List<AudioSet>()
            {
                new AudioSet(new List<string>() { crime_3_90.Publicintoxication.FileName},"public intoxication"),
            },
            };
            OfficerNeedsAssistance = new Dispatch()
            {
                Name = "Officer Needs Assistance",
                LocationDescription = LocationSpecificity.Street,
                CanAlwaysBeInterrupted = true,

                MainAudioSet = new List<AudioSet>()
            {
                new AudioSet(new List<string>() { crime_officer_in_need_of_assistance.Anofficerinneedofassistance.FileName},"an officer in need of assistance"),
                 new AudioSet(new List<string>() { crime_officer_in_need_of_assistance.Anofficerrequiringassistance.FileName},"an officer requiring assistance"),
            },
            };
            Harassment = new Dispatch()
            {
                Name = "Harassment",
                LocationDescription = LocationSpecificity.Street,
                CanAlwaysBeInterrupted = true,

                MainAudioSet = new List<AudioSet>()
            {
                new AudioSet(new List<string>() { crime_5_07.Apublicnuisance.FileName},"a public nuisance"),
                new AudioSet(new List<string>() { crime_disturbance.Apossibledisturbance.FileName},"a possible disturbance"),
                new AudioSet(new List<string>() { crime_disturbance.Adisturbance.FileName},"a disturbance"),
                new AudioSet(new List<string>() { crime_disturbance.Adisturbance1.FileName},"a disturbance"),

            },
            };
            OfficersNeeded = new Dispatch()
            {
                Name = "Officers Needed",
                LocationDescription = LocationSpecificity.Zone,
                CanAlwaysBeInterrupted = true,

                MainAudioSet = new List<AudioSet>()
            {
                new AudioSet(new List<string>() { assistance_required.Officersneeded.FileName},"officers needed"),
                 new AudioSet(new List<string>() { assistance_required.Officersrequired.FileName},"officers required"),
            },
            };
            AnnounceStolenVehicle = new Dispatch()
            {
                Name = "Stolen Vehicle Reported",
                
                IncludeDrivingVehicle = true,
                CanAlwaysBeInterrupted = true,
                MarkVehicleAsStolen = true,
                IncludeLicensePlate = true,
                MainAudioSet = new List<AudioSet>()
            {
                new AudioSet(new List<string>() {crime_stolen_vehicle.Apossiblestolenvehicle.FileName},"a possible stolen vehicle"),
            },
            };
            RequestAirSupport = new Dispatch()
            {
                Name = "Air Support Requested",
                IsStatus = true,
                IncludeReportedBy = false,
                LocationDescription = LocationSpecificity.Zone,
                MainAudioSet = new List<AudioSet>()
            {
                new AudioSet(new List<string>() { officer_requests_air_support.Officersrequestinghelicoptersupport.FileName },"officers requesting helicopter support"),
                new AudioSet(new List<string>() { officer_requests_air_support.Code99unitsrequestimmediateairsupport.FileName },"code-99 units request immediate air support"),
                new AudioSet(new List<string>() { officer_requests_air_support.Officersrequireaerialsupport.FileName },"officers require aerial support"),
                new AudioSet(new List<string>() { officer_requests_air_support.Officersrequireaerialsupport1.FileName },"officers require aerial support"),
                new AudioSet(new List<string>() { officer_requests_air_support.Officersrequireairsupport.FileName },"officers require air support"),
                new AudioSet(new List<string>() { officer_requests_air_support.Unitsrequestaerialsupport.FileName },"units request aerial support"),
                new AudioSet(new List<string>() { officer_requests_air_support.Unitsrequestingairsupport.FileName },"units requesting air support"),
                new AudioSet(new List<string>() { officer_requests_air_support.Unitsrequestinghelicoptersupport.FileName },"units requesting helicopter support"),
            },
            };
            RequestMilitaryUnits = new Dispatch()
            {
                IncludeAttentionAllUnits = true,
                Name = "Military Units Requested",
                IsStatus = true,
                IncludeReportedBy = false,
                LocationDescription = LocationSpecificity.Zone,
                MainAudioSet = new List<AudioSet>()
            {
                new AudioSet(new List<string>() { custom_wanted_level_line.Code13militaryunitsrequested.FileName },"code-13 military units requested"),
            },
            };
            RequestNOOSEUnits = new Dispatch()
            {
                IncludeAttentionAllUnits = true,
                Name = "NOOSE Units Requested",
                IsStatus = true,
                IncludeReportedBy = false,
                LocationDescription = LocationSpecificity.Zone,
                MainAudioSet = new List<AudioSet>()
            {
                new AudioSet(new List<string>() { dispatch_units_full.DispatchingSWATunitsfrompoliceheadquarters.FileName },"dispatching swat units from police headquarters"),
                new AudioSet(new List<string>() { dispatch_units_full.DispatchingSWATunitsfrompoliceheadquarters1.FileName },"dispatching swat units from police headquarters"),
            },
            };
            SuspectSpotted = new Dispatch()
            {
                Name = "Suspect Spotted",
                IsStatus = true,
                IncludeReportedBy = false,
                LocationDescription = LocationSpecificity.HeadingAndStreet,
                IncludeDrivingVehicle = true,
                CanAlwaysInterrupt = true,
                CanAlwaysBeInterrupted = true,
            };
            WantedSuspectSpotted = new Dispatch()
            {
                Name = "Wanted Suspect Spotted",
                IsStatus = true,
                IncludeReportedBy = true,
                IncludeRapSheet = true,
                Priority = 10,
                LocationDescription = LocationSpecificity.HeadingAndStreet,
                IncludeDrivingVehicle = true,
                CanAlwaysInterrupt = true,

                MainAudioSet = new List<AudioSet>()
            {
                new AudioSet(new List<string>() { crime_wanted_felon_on_the_loose.Awantedfelonontheloose.FileName },"a wanted felon on the loose"),
            },
            };
            OnFoot = new Dispatch()
            {
                Name = "On Foot",
                IsStatus = true,
                IncludeReportedBy = false,
                LocationDescription = LocationSpecificity.Nothing,
                IncludeDrivingVehicle = false,
                CanAlwaysBeInterrupted = true,
                MainAudioSet = new List<AudioSet>()
            {
                new AudioSet(new List<string>() { suspect_is.SuspectIs.FileName, on_foot.Onfoot.FileName },"suspect is on foot"),
                new AudioSet(new List<string>() { suspect_is.SuspectIs.FileName, on_foot.Onfoot1.FileName },"suspect is on on foot"),


                new AudioSet(new List<string>() {s_f_y_cop_black_full_01.SuspectOnFoot2.FileName },"suspect is on on foot"),
                new AudioSet(new List<string>() {s_f_y_cop_black_full_02.SuspectOnFoot2.FileName },"suspect is on on foot"),
                new AudioSet(new List<string>() {s_f_y_cop_white_full_01.SuspectOnFoot2.FileName },"suspect is on on foot"),
                new AudioSet(new List<string>() {s_f_y_cop_white_full_02.SuspectOnFoot2.FileName },"suspect is on on foot"),
                new AudioSet(new List<string>() {s_m_y_cop_black_full_01.SuspectOnFoot2.FileName },"suspect is on on foot"),
                new AudioSet(new List<string>() {s_m_y_cop_black_full_02.SuspectOnFoot2.FileName },"suspect is on on foot"),
                new AudioSet(new List<string>() {s_m_y_cop_black_mini_01.SuspectOnFoot2.FileName },"suspect is on on foot"),
                new AudioSet(new List<string>() {s_m_y_cop_black_mini_02.SuspectOnFoot2.FileName },"suspect is on on foot"),
                new AudioSet(new List<string>() {s_m_y_cop_black_mini_03.SuspectOnFoot2.FileName },"suspect is on on foot"),
                new AudioSet(new List<string>() {s_m_y_cop_black_mini_04.SuspectOnFoot2.FileName },"suspect is on on foot"),
                new AudioSet(new List<string>() {s_m_y_cop_white_full_01.SuspectOnFoot2.FileName },"suspect is on on foot"),
                new AudioSet(new List<string>() {s_m_y_cop_white_full_02.SuspectOnFoot2.FileName },"suspect is on on foot"),
                new AudioSet(new List<string>() {s_m_y_cop_white_mini_01.SuspectOnFoot2.FileName },"suspect is on on foot"),
                new AudioSet(new List<string>() {s_m_y_cop_white_mini_02.SuspectOnFoot2.FileName },"suspect is on on foot"),
                new AudioSet(new List<string>() {s_m_y_cop_white_mini_03.SuspectOnFoot2.FileName },"suspect is on on foot"),
                new AudioSet(new List<string>() {s_m_y_cop_white_mini_04.SuspectOnFoot2.FileName },"suspect is on on foot"),
                new AudioSet(new List<string>() {s_m_y_hwaycop_black_full_01.SuspectOnFoot2.FileName },"suspect is on on foot"),
                new AudioSet(new List<string>() {s_m_y_hwaycop_black_full_02.SuspectOnFoot2.FileName },"suspect is on on foot"),
                new AudioSet(new List<string>() {s_m_y_hwaycop_white_full_01.SuspectOnFoot2.FileName },"suspect is on on foot"),
                new AudioSet(new List<string>() {s_m_y_hwaycop_white_full_02.SuspectOnFoot2.FileName },"suspect is on on foot"),
                new AudioSet(new List<string>() {s_m_y_sheriff_white_full_01.SuspectOnFoot2.FileName },"suspect is on on foot"),
                new AudioSet(new List<string>() {s_m_y_sheriff_white_full_02.SuspectOnFoot2.FileName },"suspect is on on foot"),

                new AudioSet(new List<string>() {s_m_y_cop_black_full_02.SuspectOnFoot3.FileName },"suspect is on on foot"),
                new AudioSet(new List<string>() {s_m_y_cop_black_mini_01.SuspectOnFoot3.FileName },"suspect is on on foot"),
                new AudioSet(new List<string>() {s_m_y_cop_black_mini_02.SuspectOnFoot3.FileName },"suspect is on on foot"),
                new AudioSet(new List<string>() {s_m_y_cop_black_mini_03.SuspectOnFoot3.FileName },"suspect is on on foot"),
                new AudioSet(new List<string>() {s_m_y_cop_black_mini_04.SuspectOnFoot3.FileName },"suspect is on on foot"),
                new AudioSet(new List<string>() {s_m_y_cop_white_full_01.SuspectOnFoot3.FileName },"suspect is on on foot"),
                new AudioSet(new List<string>() {s_m_y_cop_white_full_02.SuspectOnFoot3.FileName },"suspect is on on foot"),
                new AudioSet(new List<string>() {s_m_y_cop_white_mini_02.SuspectOnFoot3.FileName },"suspect is on on foot"),
                new AudioSet(new List<string>() {s_m_y_cop_white_mini_03.SuspectOnFoot3.FileName },"suspect is on on foot"),
                new AudioSet(new List<string>() {s_m_y_cop_white_mini_04.SuspectOnFoot3.FileName },"suspect is on on foot"),
                new AudioSet(new List<string>() {s_m_y_hwaycop_black_full_01.SuspectOnFoot3.FileName },"suspect is on on foot"),
                new AudioSet(new List<string>() {s_m_y_hwaycop_black_full_02.SuspectOnFoot3.FileName },"suspect is on on foot"),
                new AudioSet(new List<string>() {s_m_y_hwaycop_white_full_01.SuspectOnFoot3.FileName },"suspect is on on foot"),
                new AudioSet(new List<string>() {s_m_y_hwaycop_white_full_02.SuspectOnFoot3.FileName },"suspect is on on foot"),


                new AudioSet(new List<string>() {s_f_y_cop_black_full_01.SuspectOnFoot4.FileName },"suspect is on on foot"),
                new AudioSet(new List<string>() {s_f_y_cop_black_full_02.SuspectOnFoot4.FileName },"suspect is on on foot"),
                new AudioSet(new List<string>() {s_f_y_cop_white_full_01.SuspectOnFoot4.FileName },"suspect is on on foot"),
                new AudioSet(new List<string>() {s_f_y_cop_white_full_02.SuspectOnFoot4.FileName },"suspect is on on foot"),
                new AudioSet(new List<string>() {s_m_y_cop_black_full_01.SuspectOnFoot4.FileName },"suspect is on on foot"),
                new AudioSet(new List<string>() {s_m_y_cop_white_full_01.SuspectOnFoot4.FileName },"suspect is on on foot"),
                new AudioSet(new List<string>() {s_m_y_cop_white_full_02.SuspectOnFoot4.FileName },"suspect is on on foot"),
                new AudioSet(new List<string>() {s_m_y_hwaycop_black_full_01.SuspectOnFoot4.FileName },"suspect is on on foot"),
                new AudioSet(new List<string>() {s_m_y_hwaycop_black_full_02.SuspectOnFoot4.FileName },"suspect is on on foot"),
                new AudioSet(new List<string>() {s_m_y_hwaycop_white_full_01.SuspectOnFoot4.FileName },"suspect is on on foot"),
                new AudioSet(new List<string>() {s_m_y_hwaycop_white_full_02.SuspectOnFoot4.FileName },"suspect is on on foot"),
                new AudioSet(new List<string>() {s_m_y_sheriff_white_full_01.SuspectOnFoot4.FileName },"suspect is on on foot"),
                new AudioSet(new List<string>() {s_m_y_sheriff_white_full_02.SuspectOnFoot4.FileName },"suspect is on on foot"),

            },
        };

            GotOnFreeway = new Dispatch()
            {
                Name = "Entered Freeway",
                IsStatus = true,
                IncludeReportedBy = false,
                LocationDescription = LocationSpecificity.Nothing,
                IncludeDrivingVehicle = false,
                CanAlwaysBeInterrupted = true,
                MainAudioSet = new List<AudioSet>()
            {
                new AudioSet(new List<string>() { s_f_y_cop_black_full_01.SuspectEnteringTheFreeway.FileName },"suspect has entered the freeway"),
                new AudioSet(new List<string>() { s_f_y_cop_black_full_02.SuspectEnteringTheFreeway.FileName },"suspect has entered the freeway"),
                new AudioSet(new List<string>() { s_f_y_cop_white_full_01.SuspectEnteringTheFreeway.FileName },"suspect has entered the freeway"),
                new AudioSet(new List<string>() { s_f_y_cop_white_full_02.SuspectEnteringTheFreeway.FileName },"suspect has entered the freeway"),
                new AudioSet(new List<string>() { s_m_y_cop_black_full_01.SuspectEnteringTheFreeway.FileName },"suspect has entered the freeway"),
                new AudioSet(new List<string>() { s_m_y_cop_black_full_02.SuspectEnteringTheFreeway.FileName },"suspect has entered the freeway"),
                new AudioSet(new List<string>() { s_m_y_cop_black_mini_02.SuspectEnteringTheFreeway.FileName },"suspect has entered the freeway"),
                new AudioSet(new List<string>() { s_m_y_cop_black_mini_03.SuspectEnteringTheFreeway.FileName },"suspect has entered the freeway"),
                new AudioSet(new List<string>() { s_m_y_cop_black_mini_04.SuspectEnteringTheFreeway.FileName },"suspect has entered the freeway"),
                new AudioSet(new List<string>() { s_m_y_cop_white_full_01.SuspectEnteringTheFreeway.FileName },"suspect has entered the freeway"),
                new AudioSet(new List<string>() { s_m_y_cop_white_full_02.SuspectEnteringTheFreeway.FileName },"suspect has entered the freeway"),
                new AudioSet(new List<string>() { s_m_y_cop_white_mini_01.SuspectEnteringTheFreeway.FileName },"suspect has entered the freeway"),
                new AudioSet(new List<string>() { s_m_y_cop_white_mini_02.SuspectEnteringTheFreeway.FileName },"suspect has entered the freeway"),
                new AudioSet(new List<string>() { s_m_y_cop_white_mini_03.SuspectEnteringTheFreeway.FileName },"suspect has entered the freeway"),
                new AudioSet(new List<string>() { s_m_y_cop_white_mini_04.SuspectEnteringTheFreeway.FileName },"suspect has entered the freeway"),
                new AudioSet(new List<string>() { s_m_y_hwaycop_black_full_01.SuspectEnteringTheFreeway.FileName },"suspect has entered the freeway"),
                new AudioSet(new List<string>() { s_m_y_hwaycop_black_full_02.SuspectEnteringTheFreeway.FileName },"suspect has entered the freeway"),
                new AudioSet(new List<string>() { s_m_y_hwaycop_white_full_01.SuspectEnteringTheFreeway.FileName },"suspect has entered the freeway"),
                new AudioSet(new List<string>() { s_m_y_hwaycop_white_full_02.SuspectEnteringTheFreeway.FileName },"suspect has entered the freeway"),
                new AudioSet(new List<string>() { s_m_y_sheriff_white_full_01.SuspectEnteringTheFreeway.FileName },"suspect has entered the freeway"),
                new AudioSet(new List<string>() { s_m_y_sheriff_white_full_02.SuspectEnteringTheFreeway.FileName },"suspect has entered the freeway"),
                new AudioSet(new List<string>() { s_f_y_cop_black_full_01.SuspectEnteringTheFreeway2.FileName },"suspect has entered the freeway"),
                new AudioSet(new List<string>() { s_f_y_cop_black_full_02.SuspectEnteringTheFreeway2.FileName },"suspect has entered the freeway"),
                new AudioSet(new List<string>() { s_f_y_cop_white_full_01.SuspectEnteringTheFreeway2.FileName },"suspect has entered the freeway"),
                new AudioSet(new List<string>() { s_f_y_cop_white_full_02.SuspectEnteringTheFreeway2.FileName },"suspect has entered the freeway"),
                new AudioSet(new List<string>() { s_m_y_cop_black_full_01.SuspectEnteringTheFreeway2.FileName },"suspect has entered the freeway"),
                new AudioSet(new List<string>() { s_m_y_cop_black_full_02.SuspectEnteringTheFreeway2.FileName },"suspect has entered the freeway"),
                new AudioSet(new List<string>() { s_m_y_cop_white_full_01.SuspectEnteringTheFreeway2.FileName },"suspect has entered the freeway"),
                new AudioSet(new List<string>() { s_m_y_cop_white_full_02.SuspectEnteringTheFreeway2.FileName },"suspect has entered the freeway"),
                new AudioSet(new List<string>() { s_m_y_hwaycop_black_full_01.SuspectEnteringTheFreeway2.FileName },"suspect has entered the freeway"),
                new AudioSet(new List<string>() { s_m_y_hwaycop_black_full_02.SuspectEnteringTheFreeway2.FileName },"suspect has entered the freeway"),
                new AudioSet(new List<string>() { s_m_y_hwaycop_white_full_01.SuspectEnteringTheFreeway2.FileName },"suspect has entered the freeway"),
                new AudioSet(new List<string>() { s_m_y_hwaycop_white_full_02.SuspectEnteringTheFreeway2.FileName },"suspect has entered the freeway"),
                new AudioSet(new List<string>() { s_m_y_sheriff_white_full_01.SuspectEnteringTheFreeway2.FileName },"suspect has entered the freeway"),
                new AudioSet(new List<string>() { s_m_y_sheriff_white_full_02.SuspectEnteringTheFreeway2.FileName },"suspect has entered the freeway"),
            },
            };
            GotOffFreeway = new Dispatch()
            {
                Name = "Exited Freeway",
                IsStatus = true,
                IncludeReportedBy = false,
                LocationDescription = LocationSpecificity.Nothing,
                IncludeDrivingVehicle = false,
                CanAlwaysBeInterrupted = true,
                MainAudioSet = new List<AudioSet>()
            {
                new AudioSet(new List<string>() { s_f_y_cop_black_full_01.SuspectLeftFreeway.FileName},"suspect left the freeway"),
                new AudioSet(new List<string>() { s_f_y_cop_black_full_02.SuspectLeftFreeway.FileName},"suspect left the freeway"),
                new AudioSet(new List<string>() { s_f_y_cop_white_full_01.SuspectLeftFreeway.FileName},"suspect left the freeway"),
                new AudioSet(new List<string>() { s_f_y_cop_white_full_02.SuspectLeftFreeway.FileName},"suspect left the freeway"),
                new AudioSet(new List<string>() { s_m_y_cop_black_full_01.SuspectLeftFreeway.FileName},"suspect left the freeway"),
                new AudioSet(new List<string>() { s_m_y_cop_black_full_02.SuspectLeftFreeway.FileName},"suspect left the freeway"),
                new AudioSet(new List<string>() { s_m_y_cop_white_full_01.SuspectLeftFreeway.FileName},"suspect left the freeway"),
                new AudioSet(new List<string>() { s_m_y_cop_white_full_02.SuspectLeftFreeway.FileName},"suspect left the freeway"),
                new AudioSet(new List<string>() { s_m_y_hwaycop_black_full_01.SuspectLeftFreeway.FileName},"suspect left the freeway"),
                new AudioSet(new List<string>() { s_m_y_hwaycop_black_full_02.SuspectLeftFreeway.FileName},"suspect left the freeway"),
                new AudioSet(new List<string>() { s_m_y_hwaycop_white_full_01.SuspectLeftFreeway.FileName},"suspect left the freeway"),
                new AudioSet(new List<string>() { s_m_y_hwaycop_white_full_02.SuspectLeftFreeway.FileName},"suspect left the freeway"),
                new AudioSet(new List<string>() { s_m_y_sheriff_white_full_01.SuspectLeftFreeway.FileName},"suspect left the freeway"),
                new AudioSet(new List<string>() { s_m_y_sheriff_white_full_02.SuspectLeftFreeway.FileName},"suspect left the freeway"),
                new AudioSet(new List<string>() { s_f_y_cop_black_full_01.SuspectLeftFreeway2.FileName},"suspect left the freeway"),
                new AudioSet(new List<string>() { s_f_y_cop_black_full_02.SuspectLeftFreeway2.FileName},"suspect left the freeway"),
                new AudioSet(new List<string>() { s_f_y_cop_white_full_01.SuspectLeftFreeway2.FileName},"suspect left the freeway"),
                new AudioSet(new List<string>() { s_f_y_cop_white_full_02.SuspectLeftFreeway2.FileName},"suspect left the freeway"),
                new AudioSet(new List<string>() { s_m_y_cop_black_full_01.SuspectLeftFreeway2.FileName},"suspect left the freeway"),
                new AudioSet(new List<string>() { s_m_y_cop_black_full_02.SuspectLeftFreeway2.FileName},"suspect left the freeway"),
                new AudioSet(new List<string>() { s_m_y_cop_white_full_01.SuspectLeftFreeway2.FileName},"suspect left the freeway"),
                new AudioSet(new List<string>() { s_m_y_cop_white_full_02.SuspectLeftFreeway2.FileName},"suspect left the freeway"),
                new AudioSet(new List<string>() { s_m_y_hwaycop_black_full_01.SuspectLeftFreeway2.FileName},"suspect left the freeway"),
                new AudioSet(new List<string>() { s_m_y_hwaycop_black_full_02.SuspectLeftFreeway2.FileName},"suspect left the freeway"),
                new AudioSet(new List<string>() { s_m_y_hwaycop_white_full_01.SuspectLeftFreeway2.FileName},"suspect left the freeway"),
                new AudioSet(new List<string>() { s_m_y_hwaycop_white_full_02.SuspectLeftFreeway2.FileName},"suspect left the freeway"),
                new AudioSet(new List<string>() { s_m_y_sheriff_white_full_01.SuspectLeftFreeway2.FileName},"suspect left the freeway"),
                new AudioSet(new List<string>() { s_m_y_sheriff_white_full_02.SuspectLeftFreeway2.FileName},"suspect left the freeway"),
            },
            };
            VehicleCrashed = new Dispatch()
            {
                Name = "Vehicle Crashed",
                IsStatus = true,
                IncludeReportedBy = false,
                LocationDescription = LocationSpecificity.Nothing,
                IncludeDrivingVehicle = false,
                CanAlwaysBeInterrupted = true,
                MainAudioSet = new List<AudioSet>()
            {
                new AudioSet(new List<string>() { s_f_y_cop_black_full_01.SuspectCrashed.FileName},"suspect crashed"),
                new AudioSet(new List<string>() { s_f_y_cop_black_full_02.SuspectCrashed.FileName},"suspect crashed"),
                new AudioSet(new List<string>() { s_f_y_cop_white_full_01.SuspectCrashed.FileName},"suspect crashed"),
                new AudioSet(new List<string>() { s_f_y_cop_white_full_02.SuspectCrashed.FileName},"suspect crashed"),
                new AudioSet(new List<string>() { s_m_y_cop_black_full_01.SuspectCrashed.FileName},"suspect crashed"),
                new AudioSet(new List<string>() { s_m_y_cop_black_full_02.SuspectCrashed.FileName},"suspect crashed"),
                new AudioSet(new List<string>() { s_m_y_cop_white_full_01.SuspectCrashed.FileName},"suspect crashed"),
                new AudioSet(new List<string>() { s_m_y_cop_white_full_02.SuspectCrashed.FileName},"suspect crashed"),
                new AudioSet(new List<string>() { s_m_y_hwaycop_black_full_01.SuspectCrashed.FileName},"suspect crashed"),
                new AudioSet(new List<string>() { s_m_y_hwaycop_black_full_02.SuspectCrashed.FileName},"suspect crashed"),
                new AudioSet(new List<string>() { s_m_y_hwaycop_white_full_01.SuspectCrashed.FileName},"suspect crashed"),
                new AudioSet(new List<string>() { s_m_y_hwaycop_white_full_02.SuspectCrashed.FileName},"suspect crashed"),
                new AudioSet(new List<string>() { s_m_y_sheriff_white_full_01.SuspectCrashed.FileName},"suspect crashed"),
                new AudioSet(new List<string>() { s_m_y_sheriff_white_full_02.SuspectCrashed.FileName},"suspect crashed"),
            },
            };
            NoFurtherUnitsNeeded = new Dispatch()
            {
                Name = "Officers On-Site, Code 4-ADAM",
                IsStatus = true,
                IncludeReportedBy = false,
                CanAlwaysBeInterrupted = true,
                MainAudioSet = new List<AudioSet>()
            {
                new AudioSet(new List<string>() { officers_on_scene.Officersareatthescene.FileName },"officers are at the scene"),
                new AudioSet(new List<string>() { officers_on_scene.Officersarrivedonscene.FileName },"offices have arrived on scene"),
                new AudioSet(new List<string>() { officers_on_scene.Officershavearrived.FileName },"officers have arrived"),
                new AudioSet(new List<string>() { officers_on_scene.Officersonscene.FileName },"officers on scene"),
                new AudioSet(new List<string>() { officers_on_scene.Officersonsite.FileName },"officers on site"),
            },
                SecondaryAudioSet = new List<AudioSet>()
            {
                new AudioSet(new List<string>() { no_further_units.Noadditionalofficersneeded.FileName },"no additional officers needed"),
                new AudioSet(new List<string>() { no_further_units.Noadditionalofficersneeded1.FileName },"no additional officers needed"),
                new AudioSet(new List<string>() { no_further_units.Nofurtherunitsrequired.FileName },"no further units required"),
                new AudioSet(new List<string>() { no_further_units.WereCode4Adam.FileName },"we're code-4 adam"),
                new AudioSet(new List<string>() { no_further_units.Code4Adamnoadditionalsupportneeded.FileName },"code-4 adam no additional support needed"),
            },
            };
            SuspectArrested = new Dispatch()
            {
                Name = "Suspect Apprehended",
                IsStatus = true,
                IncludeReportedBy = false,
                CanAlwaysInterrupt = true,
                MainAudioSet = new List<AudioSet>()
            {
                new AudioSet(new List<string>() { crook_arrested.Officershaveapprehendedsuspect.FileName },"officers have apprehended suspect"),
                new AudioSet(new List<string>() { crook_arrested.Officershaveapprehendedsuspect1.FileName },"officers have apprehended suspect"),
            },
            };
            SuspectWasted = new Dispatch()
            {
                Name = "Suspect Neutralized",
                IsStatus = true,
                IncludeReportedBy = false,
                CanAlwaysInterrupt = true,
                MainAudioSet = new List<AudioSet>()
            {
                new AudioSet(new List<string>() { crook_killed.Criminaldown.FileName },"criminal down"),
                new AudioSet(new List<string>() { crook_killed.Suspectdown.FileName },"suspect down"),
                new AudioSet(new List<string>() { crook_killed.Suspectneutralized.FileName },"suspect neutralized"),
                new AudioSet(new List<string>() { crook_killed.Suspectdownmedicalexaminerenroute.FileName },"suspect down, medical examiner in route"),
                new AudioSet(new List<string>() { crook_killed.Suspectdowncoronerenroute.FileName },"suspect down, coroner in route"),
                new AudioSet(new List<string>() { crook_killed.Officershavepacifiedsuspect.FileName },"officers have pacified suspect"),
             },
            };
            ChangedVehicles = new Dispatch()
            {
                Name = "Suspect Changed Vehicle",
                IsStatus = true,
                IncludeDrivingVehicle = true,
                CanAlwaysInterrupt = true,
                LocationDescription = LocationSpecificity.StreetAndZone,
                MainAudioSet = new List<AudioSet>()
            {
                new AudioSet(new List<string>() { "" },""),
             },
            };
            RequestBackup = new Dispatch()
            {
                IncludeAttentionAllUnits = true,
                Name = "Backup Required",
                IsStatus = true,
                IncludeReportedBy = false,
                CanAlwaysInterrupt = true,

                MainAudioSet = new List<AudioSet>()
            {
                new AudioSet(new List<string>() { assistance_required.Assistanceneeded.FileName },"assistance needed"),
                new AudioSet(new List<string>() { assistance_required.Assistancerequired.FileName },"Assistance required"),
                new AudioSet(new List<string>() { assistance_required.Backupneeded.FileName },"backup needed"),
                new AudioSet(new List<string>() { assistance_required.Backuprequired.FileName },"backup required"),
                new AudioSet(new List<string>() { assistance_required.Officersneeded.FileName },"officers needed"),
                new AudioSet(new List<string>() { assistance_required.Officersrequired.FileName },"officers required"),
             },
                SecondaryAudioSet = new List<AudioSet>()
            {
                new AudioSet(new List<string>() { dispatch_respond_code.UnitsrespondCode3.FileName },"units respond code-3"),
             },
                PreambleAudioSet = new List<AudioSet>()
            {
                new AudioSet(new List<string>() {s_f_y_cop_black_full_01.NeedBackup1.FileName },"requesting backup"),
                new AudioSet(new List<string>() {s_f_y_cop_black_full_02.NeedBackup1.FileName },"requesting backup"),
                new AudioSet(new List<string>() {s_f_y_cop_white_full_01.NeedBackup1.FileName },"requesting backup"),
                new AudioSet(new List<string>() {s_f_y_cop_white_full_02.NeedBackup1.FileName },"requesting backup"),
                new AudioSet(new List<string>() {s_m_y_cop_black_full_01.NeedBackup1.FileName },"requesting backup"),
                new AudioSet(new List<string>() {s_m_y_cop_black_full_02.NeedBackup1.FileName },"requesting backup"),
                new AudioSet(new List<string>() {s_m_y_cop_white_full_01.NeedBackup1.FileName },"requesting backup"),
                new AudioSet(new List<string>() {s_m_y_cop_white_full_02.NeedBackup1.FileName },"requesting backup"),
                new AudioSet(new List<string>() {s_m_y_hwaycop_black_full_01.NeedBackup1.FileName },"requesting backup"),
                new AudioSet(new List<string>() {s_m_y_hwaycop_black_full_02.NeedBackup1.FileName },"requesting backup"),
                new AudioSet(new List<string>() {s_m_y_hwaycop_white_full_01.NeedBackup1.FileName },"requesting backup"),
                new AudioSet(new List<string>() {s_m_y_hwaycop_white_full_02.NeedBackup1.FileName },"requesting backup"),
                new AudioSet(new List<string>() {s_m_y_sheriff_white_full_01.NeedBackup1.FileName },"requesting backup"),
                new AudioSet(new List<string>() {s_m_y_sheriff_white_full_02.NeedBackup1.FileName },"requesting backup"),

                new AudioSet(new List<string>() {s_f_y_cop_black_full_01.NeedBackup2.FileName },"requesting backup"),
                new AudioSet(new List<string>() {s_f_y_cop_black_full_02.NeedBackup2.FileName },"requesting backup"),
                new AudioSet(new List<string>() {s_f_y_cop_white_full_01.NeedBackup2.FileName },"requesting backup"),
                new AudioSet(new List<string>() {s_f_y_cop_white_full_02.NeedBackup2.FileName },"requesting backup"),
                new AudioSet(new List<string>() {s_m_y_cop_black_full_01.NeedBackup2.FileName },"requesting backup"),
                new AudioSet(new List<string>() {s_m_y_cop_black_full_02.NeedBackup2.FileName },"requesting backup"),
                new AudioSet(new List<string>() {s_m_y_cop_white_full_01.NeedBackup2.FileName },"requesting backup"),
                new AudioSet(new List<string>() {s_m_y_cop_white_full_02.NeedBackup2.FileName },"requesting backup"),
                new AudioSet(new List<string>() {s_m_y_hwaycop_black_full_01.NeedBackup2.FileName },"requesting backup"),
                new AudioSet(new List<string>() {s_m_y_hwaycop_black_full_02.NeedBackup2.FileName },"requesting backup"),
                new AudioSet(new List<string>() {s_m_y_hwaycop_white_full_01.NeedBackup2.FileName },"requesting backup"),
                new AudioSet(new List<string>() {s_m_y_hwaycop_white_full_02.NeedBackup2.FileName },"requesting backup"),
                new AudioSet(new List<string>() {s_m_y_sheriff_white_full_01.NeedBackup2.FileName },"requesting backup"),
                new AudioSet(new List<string>() {s_m_y_sheriff_white_full_02.NeedBackup2.FileName },"requesting backup"),

                new AudioSet(new List<string>() {s_f_y_cop_black_full_01.NeedBackup3.FileName },"requesting backup"),
                new AudioSet(new List<string>() {s_f_y_cop_black_full_02.NeedBackup3.FileName },"requesting backup"),
                new AudioSet(new List<string>() {s_f_y_cop_white_full_01.NeedBackup3.FileName },"requesting backup"),
                new AudioSet(new List<string>() {s_f_y_cop_white_full_02.NeedBackup3.FileName },"requesting backup"),
                new AudioSet(new List<string>() {s_m_y_cop_black_full_01.NeedBackup3.FileName },"requesting backup"),
                new AudioSet(new List<string>() {s_m_y_cop_black_full_02.NeedBackup3.FileName },"requesting backup"),
                new AudioSet(new List<string>() {s_m_y_cop_black_mini_01.NeedBackup3.FileName },"requesting backup"),
                new AudioSet(new List<string>() {s_m_y_cop_black_mini_02.NeedBackup3.FileName },"requesting backup"),
                new AudioSet(new List<string>() {s_m_y_cop_black_mini_03.NeedBackup3.FileName },"requesting backup"),
                new AudioSet(new List<string>() {s_m_y_cop_black_mini_04.NeedBackup3.FileName },"requesting backup"),
                new AudioSet(new List<string>() {s_m_y_cop_white_full_01.NeedBackup3.FileName },"requesting backup"),
                new AudioSet(new List<string>() {s_m_y_cop_white_full_02.NeedBackup3.FileName },"requesting backup"),
                new AudioSet(new List<string>() {s_m_y_cop_white_mini_01.NeedBackup3.FileName },"requesting backup"),
                new AudioSet(new List<string>() {s_m_y_cop_white_mini_02.NeedBackup3.FileName },"requesting backup"),
                new AudioSet(new List<string>() {s_m_y_cop_white_mini_03.NeedBackup3.FileName },"requesting backup"),
                new AudioSet(new List<string>() {s_m_y_cop_white_mini_04.NeedBackup3.FileName },"requesting backup"),
                new AudioSet(new List<string>() {s_m_y_hwaycop_black_full_01.NeedBackup3.FileName },"requesting backup"),
                new AudioSet(new List<string>() {s_m_y_hwaycop_black_full_02.NeedBackup3.FileName },"requesting backup"),
                new AudioSet(new List<string>() {s_m_y_hwaycop_white_full_01.NeedBackup3.FileName },"requesting backup"),
                new AudioSet(new List<string>() {s_m_y_hwaycop_white_full_02.NeedBackup3.FileName },"requesting backup"),
                new AudioSet(new List<string>() {s_m_y_sheriff_white_full_01.NeedBackup3.FileName },"requesting backup"),
                new AudioSet(new List<string>() {s_m_y_sheriff_white_full_02.NeedBackup3.FileName },"requesting backup"),
             },
            };
            WeaponsFree = new Dispatch()
            {
                IncludeAttentionAllUnits = true,
                Name = "Weapons Free",
                IsStatus = true,
                IncludeReportedBy = false,
                CanAlwaysInterrupt = true,
                MainAudioSet = new List<AudioSet>()
            {
                new AudioSet(new List<string>() { custom_wanted_level_line.Suspectisarmedanddangerousweaponsfree.FileName },"suspect is armed and dangerous, weapons free"),
             },
            };
            LethalForceAuthorized = new Dispatch()
            {
                IncludeAttentionAllUnits = true,
                Name = "Lethal Force Authorized",
                IsStatus = true,
                IncludeReportedBy = false,
                ResultsInLethalForce = true,
                CanAlwaysInterrupt = true,
            };
            //Status
            SuspectEvaded = new Dispatch()
            {
                Name = "Suspect Evaded",
                IsStatus = true,
                IncludeReportedBy = false,
                LocationDescription = LocationSpecificity.Zone,
                CanAlwaysInterrupt = true,
                CanAlwaysBeInterrupted = true,
                MainAudioSet = new List<AudioSet>()
            {
                new AudioSet(new List<string>() { suspect_eluded_pt_1.SuspectEvadedPursuingOfficiers.FileName },"suspect evaded pursuing officers"),
                new AudioSet(new List<string>() { suspect_eluded_pt_1.OfficiersHaveLostVisualOnSuspect.FileName },"officers have lost visual on suspect"),
            },
                PreambleAudioSet = new List<AudioSet>()
            {

                new AudioSet(new List<string>() {s_m_y_cop_black_full_02.CantSeeSuspect1.FileName },"need location on suspect"),
                new AudioSet(new List<string>() {s_m_y_cop_black_mini_01.CantSeeSuspect1.FileName },"need location on suspect"),
                new AudioSet(new List<string>() {s_m_y_cop_black_mini_02.CantSeeSuspect1.FileName },"need location on suspect"),
                new AudioSet(new List<string>() {s_m_y_cop_black_mini_03.CantSeeSuspect1.FileName },"need location on suspect"),
                new AudioSet(new List<string>() {s_m_y_cop_black_mini_04.CantSeeSuspect1.FileName },"need location on suspect"),
                new AudioSet(new List<string>() {s_m_y_cop_white_full_01.CantSeeSuspect1.FileName },"need location on suspect"),
                new AudioSet(new List<string>() {s_m_y_cop_white_full_02.CantSeeSuspect1.FileName },"need location on suspect"),
                new AudioSet(new List<string>() {s_m_y_cop_white_mini_02.CantSeeSuspect1.FileName },"need location on suspect"),
                new AudioSet(new List<string>() {s_m_y_cop_white_mini_03.CantSeeSuspect1.FileName },"need location on suspect"),
                new AudioSet(new List<string>() {s_m_y_cop_white_mini_04.CantSeeSuspect1.FileName },"need location on suspect"),
                new AudioSet(new List<string>() {s_m_y_hwaycop_black_full_01.CantSeeSuspect1.FileName },"need location on suspect"),
                new AudioSet(new List<string>() {s_m_y_hwaycop_black_full_02.CantSeeSuspect1.FileName },"need location on suspect"),
                new AudioSet(new List<string>() {s_m_y_hwaycop_white_full_01.CantSeeSuspect1.FileName },"need location on suspect"),
                new AudioSet(new List<string>() {s_m_y_hwaycop_white_full_02.CantSeeSuspect1.FileName },"need location on suspect"),
             },
            };
            RemainInArea = new Dispatch()//runs when you lose wanted organicalls
            {
                Name = "Remain in Area",
                IsStatus = true,
                IncludeReportedBy = false,
                CanAlwaysInterrupt = true,
                CanAlwaysBeInterrupted = true,
                MainAudioSet = new List<AudioSet>()
            {
                new AudioSet(new List<string>() { suspect_eluded_pt_2.AllUnitsStayInTheArea.FileName },"all units stay in the area"),
                new AudioSet(new List<string>() { suspect_eluded_pt_2.AllUnitsRemainOnAlert.FileName },"all units remain on alert"),

              //  new AudioSet(new List<string>() { suspect_eluded_pt_2.AllUnitsStandby.FileName },"all units standby"),
                new AudioSet(new List<string>() { suspect_eluded_pt_2.AllUnitsStayInTheArea.FileName },"all units stay in the area"),
                new AudioSet(new List<string>() { suspect_eluded_pt_2.AllUnitsRemainOnAlert.FileName },"all units remain on alert"),
            },
            };
            AttemptToReacquireSuspect = new Dispatch()//is the status one
            {
                Name = "Attempt To Reacquire",
                IsStatus = true,
                IncludeReportedBy = false,
                LocationDescription = LocationSpecificity.Zone,
                CanAlwaysInterrupt = true,
                CanAlwaysBeInterrupted = true,
                MainAudioSet = new List<AudioSet>()
            {
                new AudioSet(new List<string>() { attempt_to_find.AllunitsATonsuspects20.FileName },"all units ATL on suspects 20"),
                new AudioSet(new List<string>() { attempt_to_find.Allunitsattempttoreacquire.FileName },"all units attempt to reacquire"),
                new AudioSet(new List<string>() { attempt_to_find.Allunitsattempttoreacquirevisual.FileName },"all units attempt to reacquire visual"),
                new AudioSet(new List<string>() { attempt_to_find.RemainintheareaATL20onsuspect.FileName },"remain in the area, ATL-20 on suspect"),
                new AudioSet(new List<string>() { attempt_to_find.RemainintheareaATL20onsuspect1.FileName },"remain in the area, ATL-20 on suspect"),
            },
                PreambleAudioSet = new List<AudioSet>()
            {

                new AudioSet(new List<string>() {s_m_y_cop_black_full_02.CantSeeSuspect1.FileName },"need location on suspect"),
                new AudioSet(new List<string>() {s_m_y_cop_black_mini_01.CantSeeSuspect1.FileName },"need location on suspect"),
                new AudioSet(new List<string>() {s_m_y_cop_black_mini_02.CantSeeSuspect1.FileName },"need location on suspect"),
                new AudioSet(new List<string>() {s_m_y_cop_black_mini_03.CantSeeSuspect1.FileName },"need location on suspect"),
                new AudioSet(new List<string>() {s_m_y_cop_black_mini_04.CantSeeSuspect1.FileName },"need location on suspect"),
                new AudioSet(new List<string>() {s_m_y_cop_white_full_01.CantSeeSuspect1.FileName },"need location on suspect"),
                new AudioSet(new List<string>() {s_m_y_cop_white_full_02.CantSeeSuspect1.FileName },"need location on suspect"),
                new AudioSet(new List<string>() {s_m_y_cop_white_mini_02.CantSeeSuspect1.FileName },"need location on suspect"),
                new AudioSet(new List<string>() {s_m_y_cop_white_mini_03.CantSeeSuspect1.FileName },"need location on suspect"),
                new AudioSet(new List<string>() {s_m_y_cop_white_mini_04.CantSeeSuspect1.FileName },"need location on suspect"),
                new AudioSet(new List<string>() {s_m_y_hwaycop_black_full_01.CantSeeSuspect1.FileName },"need location on suspect"),
                new AudioSet(new List<string>() {s_m_y_hwaycop_black_full_02.CantSeeSuspect1.FileName },"need location on suspect"),
                new AudioSet(new List<string>() {s_m_y_hwaycop_white_full_01.CantSeeSuspect1.FileName },"need location on suspect"),
                new AudioSet(new List<string>() {s_m_y_hwaycop_white_full_02.CantSeeSuspect1.FileName },"need location on suspect"),
             },



            };
            ResumePatrol = new Dispatch()
            {
                Name = "Resume Patrol",
                IsStatus = true,
                IncludeReportedBy = false,
                CanAlwaysInterrupt = true,
                CanAlwaysBeInterrupted = true,
                MainAudioSet = new List<AudioSet>()
            {
                new AudioSet(new List<string>() { officer_begin_patrol.Beginpatrol.FileName },"begin patrol"),
                new AudioSet(new List<string>() { officer_begin_patrol.Beginbeat.FileName },"begin beat"),

                new AudioSet(new List<string>() { officer_begin_patrol.Assigntopatrol.FileName },"assign to patrol"),
                new AudioSet(new List<string>() { officer_begin_patrol.Proceedtopatrolarea.FileName },"proceed to patrol area"),
                new AudioSet(new List<string>() { officer_begin_patrol.Proceedwithpatrol.FileName },"proceed with patrol"),
            },
            };
        }



        private class CrimeDispatch
        {
            public CrimeDispatch(string crimeID, Dispatch dispatchToPlay)
            {
                CrimeID = crimeID;
                Dispatch = dispatchToPlay;
            }
            public string CrimeID { get; set; }
            public Dispatch Dispatch { get; set; }
        }
        private class Dispatch
        {
            private uint GameTimeLastPlayed;
            public Dispatch()
            {
            }
            public bool CanAlwaysBeInterrupted { get; set; }
            public bool CanAlwaysInterrupt { get; set; }
            public bool CanBeReportedMultipleTimes { get; set; } = true;
            public bool HasBeenPlayedThisWanted { get; set; }
            public bool HasPreamble
            {
                get
                {
                    if (PreambleAudioSet.Any())
                        return true;
                    else
                        return false;
                }
            }
            public bool HasRecentlyBeenPlayed
            {
                get
                {
                    uint TimeBetween = 25000;
                    if (TimesPlayed > 0)
                    {
                        TimeBetween = 60000;
                    }
                    if (Game.GameTime - GameTimeLastPlayed <= TimeBetween)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            public bool HasVeryRecentlyBeenPlayed
            {
                get
                {
                    if (Game.GameTime - GameTimeLastPlayed <= 15000)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            public bool IncludeAttentionAllUnits { get; set; }
            public bool IncludeCarryingWeapon { get; set; }
            public bool IncludeDrivingSpeed { get; set; }
            public bool IncludeDrivingVehicle { get; set; }
            public bool IncludeLicensePlate { get; set; }
            public bool IncludeRapSheet { get; set; }
            public bool IncludeReportedBy { get; set; } = true;
            public bool IsStatus { get; set; }
            public CrimeSceneDescription LatestInformation { get; set; } = new CrimeSceneDescription();
            public LocationSpecificity LocationDescription { get; set; } = LocationSpecificity.Nothing;
            public List<AudioSet> MainAudioSet { get; set; } = new List<AudioSet>();
            public List<AudioSet> MainMultiAudioSet { get; set; } = new List<AudioSet>();
            public bool MarkVehicleAsStolen { get; set; }
            public string Name { get; set; } = "Unknown";
            public string NotificationText
            {
                get
                {
                    return Name;
                }
            }
            public string NotificationTitle { get; set; } = "Police Scanner";
            public List<AudioSet> PreambleAudioSet { get; set; } = new List<AudioSet>();
            public int Priority { get; set; } = 99;
            public bool ResultsInLethalForce { get; set; }
            public List<AudioSet> SecondaryAudioSet { get; set; } = new List<AudioSet>();
            public int TimesPlayed { get; set; }
            public bool VehicleIncludesIn { get; set; }
            public void SetPlayed()
            {
                GameTimeLastPlayed = Game.GameTime;
                HasBeenPlayedThisWanted = true;
                TimesPlayed++;
            }
        }
        private class DispatchEvent
        {
            public DispatchEvent()
            {
            }
            public bool CanBeInterrupted { get; set; } = true;
            public bool CanInterrupt { get; set; } = true;
            public bool HasStreetAudio { get; set; }
            public bool HasZoneAudio { get; set; }
            public string NotificationSubtitle { get; set; } = "Status";
            public string NotificationText { get; set; } = "~b~Scanner Audio";
            public string NotificationTitle { get; set; } = "Police Scanner";
            public Vector3 PositionToReport { get; set; }
            public int Priority { get; set; } = 99;
            public List<string> SoundsToPlay { get; set; } = new List<string>();
            public string Subtitles { get; set; }
        }
    }
}