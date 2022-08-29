﻿using LSR.Vehicles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LosSantosRED.lsr.Interface
{
    public interface IButtonPromptable
    {
        PedExt CurrentLookedAtPed { get; }

        bool CanConverseWithLookedAtPed { get; }
        bool IsInteracting { get; }
        bool IsInteractingWithLocation { get; }
        InteractableLocation ClosestInteractableLocation { get; }
        bool CanExitCurrentInterior { get; }
        bool CanPerformActivities { get; }
        bool IsNearScenario { get; }
        Scenario ClosestScenario { get; }
        bool IsInVehicle { get; }
        bool IsNotWanted { get; }
        bool CanLootLookedAtPed { get; }
        bool CanGrabLookedAtPed { get; }
        float ClosestPoliceDistanceToPlayer { get; }
        bool AnyPoliceRecentlySeenPlayer { get; }
        bool CanDragLookedAtPed { get; }
        bool CanPauseCurrentActivity { get; }
        bool CanCancelCurrentActivity { get; }
        bool IsPerformingActivity { get; }
        bool IsCurrentActivityPaused { get; }
        string PauseCurrentActivityPrompt { get; }
        string CancelCurrentActivityPrompt { get; }
        string ContinueCurrentActivityPrompt { get; }
        bool IsDead { get; }
        bool IsBusted { get; }
        VehicleExt CurrentLookedAtVehicle { get; }
        bool IsMovingFast { get; }
        bool IsWanted { get; }
        bool IsStill { get; }
        bool IsAliveAndFree { get; }
        PoliceResponse PoliceResponse { get; }
        bool IsIncapacitated { get; }
        WeaponEquipment WeaponEquipment { get; }
        bool IsCop { get; }
        bool IsGettingIntoAVehicle { get; }
        bool IsBreakingIntoCar { get; }
        SurrenderActivity Surrendering { get; }
        GangMember CurrentLookedAtGangMember { get; }
        bool CanRecruitLookedAtGangMember { get; }
        Rage.Object CurrentLookedAtObject { get; }
        bool CanSitOnCurrentLookedAtObject { get; }
        bool IsSitting { get; }
        bool IsConversing { get; }
        //GameLocation ClosestTeleportEntrance { get; }
    }
}
