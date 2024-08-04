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
using BepInEx.Logging;


namespace JumpClimb.Patches
{

    //Removes the ginterface207_0.IsGrounded check from method_4. Line 159
    internal class IsGroundedPatch : ModulePatch
    {
        public static ManualLogSource LogSource;

        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(GClass2171), nameof(GClass2171.method_4));
        }

        [PatchPrefix]
        static bool Prefix(GClass2171 __instance, IVaultingSettings ___ivaultingSettings_0, GClass2208 ___gclass2208_0, GInterface207 ___ginterface207_0, ref bool __result)
        {
            __result = ___ivaultingSettings_0.IsActive && !__instance.IsVaulting() && ___ginterface207_0.MovementDirection.y >= 0f && ___ginterface207_0.IsGrounded && !___gclass2208_0.ObstacleCalculatorModel.IsTerrain;
            LogSource.LogInfo(__result);
            return false;
        }
    }

    //Removes the ginterface207_0.PlayerAnimatorIsJumpSetted()) check from GetVaultingStrategy. Line 228
    internal class GetVaultingStrategyPatch : ModulePatch
    {
        public static ManualLogSource LogSource;

        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(GClass2171), nameof(GClass2171.GetVaultingStrategy));
        }

        [PatchPrefix]
        static bool Prefix(GClass2171 __instance, ref EVaultingStrategy __result, GClass2208 ___gclass2208_0, GInterface207 ___ginterface207_0, Func<bool> ___func_1, Func<bool> ___func_2)
        {
            if (!__instance.method_4())
            {
                __result = EVaultingStrategy.None;
            }
            bool flag = ___gclass2208_0.StairsCalculatorModel.IsStairsCondition();
            bool flag2 = ___ginterface207_0.IsSprintEnabled && __instance.BehindObstacleRatio > -0.5f;
            if (___ginterface207_0.IsAnimatorInTransitionState(0) || ___ginterface207_0.PlayerAnimatorIsJumpSetted())
            {
                __result = EVaultingStrategy.None;
            }
            if (___func_1() && ___gclass2208_0.VaultingStatesModel.VaultState.CanMove() && !flag2 && !flag)
            {
                __result = EVaultingStrategy.Vault;
            }
            if (___func_2() && ___gclass2208_0.VaultingStatesModel.ClimbState.CanMove() && !___ginterface207_0.IsSprintEnabled && !flag)
            {
                __result = EVaultingStrategy.Climb;
            }
            __result = EVaultingStrategy.None;
            LogSource.LogInfo(__result);

            return false;
        }
    }
};