using GameNetcodeStuff;
using HarmonyLib;
using System.Collections.Generic;
using Unity.Netcode;

namespace LethalSprayCans.Patches
{
    [HarmonyPatch(typeof(SprayPaintItem))]
    internal class SprayCanPatch : NetworkBehaviour
    {
		public static Dictionary<SprayPaintItem, int> currentProbability = new Dictionary<SprayPaintItem, int>();
		static bool TrySpraying(SprayPaintItem spray)
        {
			if(spray == null)
            {
				LethalSprayCanBase.mls.LogWarning("Spray item is null uh oh!");
				return false;
            }
			PlayerControllerB player = GameNetworkManager.Instance.localPlayerController;
			HUDManager.Instance.ShakeCamera(ScreenShakeType.Big);
			player.DiscardHeldObject(false, null, default, true);
			player.DamagePlayer(UnityEngine.Random.Range(1,player.health+1), true, true, CauseOfDeath.Unknown, 0, false, default);
			Landmine.SpawnExplosion(player.gameplayCamera.transform.position, true, 0, 0);
			return false;
		}
        [HarmonyPatch(nameof(SprayPaintItem.ItemInteractLeftRight))]
        [HarmonyPostfix]
        static void Interact(SprayPaintItem __instance, ref float ___sprayCanShakeMeter, ref bool ___isSpraying, ref float ___sprayCanTank, bool right)
        {
			if(!currentProbability.ContainsKey(__instance))
            {
				currentProbability.Add(__instance, 0);
			}
			if (right)
			{
				return;
			}
			if (__instance.playerHeldBy == null)
			{
				return;
			}
			if (___isSpraying)
			{
				return;
			}
			if (___sprayCanTank <= 0f || ___sprayCanShakeMeter <= 0f)
			{
				return;
			}
			if (___sprayCanShakeMeter + 0.15f > 1f)
			{
                if (__instance.IsOwner)
                {
					currentProbability[__instance] += UnityEngine.Random.Range(LethalSprayCanBase.ProbabilityMin.Value, LethalSprayCanBase.ProbabilityMax.Value);
					float rnd = UnityEngine.Random.Range(1, 100);
					if (rnd <= currentProbability[__instance])
					{
						currentProbability[__instance] = 0;
						TrySpraying(__instance);
						HUDManager.Instance.DisplayTip("it went boom", "ohnyo", false, false);
						return;
					}
					HUDManager.Instance.DisplayTip("GO BOOM", $"Current probability chance: {currentProbability[__instance]}", false, false);
				}
			}
			return;
		}
    }
}
