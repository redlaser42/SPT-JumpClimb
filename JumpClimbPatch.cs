using SPT.Reflection.Patching;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using EFT;
using EFT.Vaulting;
using System.Runtime.CompilerServices;
using SPT.Reflection;


namespace JumpClimb.Patches
{
    //Removes the ginterface207_0.IsGrounded check from method_4. Line 159
    internal class IsGroundedPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(GClass2171), nameof(GClass2171.method_4));
        }

        [PatchPrefix]
        static bool Prefix(GClass2171 __instance, ref IVaultingSettings __IVaultSettings, GClass2208 ___gclass2208_0, GInterface207 ___GInterface207_0, ref bool __result)
        {
            __result = __IVaultSettings.IsActive && !__instance.IsVaulting() && ___GInterface207_0.MovementDirection.y >= 0f && !___gclass2208_0.ObstacleCalculatorModel.IsTerrain;
            return false;
        }
    }

    //Removes the ginterface207_0.PlayerAnimatorIsJumpSetted()) check from GetVaultingStrategy. Line 228
    internal class GetVaultingStrategyPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(GClass2171), nameof(GClass2171.GetVaultingStrategy));
        }

        [PatchPrefix]
        static bool Prefix(GClass2171 __instance, ref EVaultingStrategy __result, GClass2208 ___gclass2208_0, GInterface207 ___GInterface207_0, Func<bool> __func_1, Func<bool> __func_2)
        {
            if (!__instance.method_4())
            {
                __result = EVaultingStrategy.None;
            }
            bool flag = ___gclass2208_0.StairsCalculatorModel.IsStairsCondition();
            bool flag2 = ___GInterface207_0.IsSprintEnabled && __instance.BehindObstacleRatio > -0.5f;
            if (___GInterface207_0.IsAnimatorInTransitionState(0))
            {
                __result = EVaultingStrategy.None;
            }
            if (__func_1() && ___gclass2208_0.VaultingStatesModel.VaultState.CanMove() && !flag2 && !flag)
            {
                __result = EVaultingStrategy.Vault;
            }
            if (__func_2() && ___gclass2208_0.VaultingStatesModel.ClimbState.CanMove() && !___GInterface207_0.IsSprintEnabled && !flag)
            {
                __result = EVaultingStrategy.Climb;
            }
            __result = EVaultingStrategy.None;

            return false;
        }
    }
};