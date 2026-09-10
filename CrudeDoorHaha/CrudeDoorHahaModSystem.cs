using HarmonyLib;
using Vintagestory.API.Common;
using Vintagestory.GameContent;

namespace CrudeDoorHaha;

public class CrudeDoorHahaModSystem : ModSystem
{
    private Harmony? harmony;

    public override void StartPre(ICoreAPI api)
    {
        if (Harmony.HasAnyPatches(Mod.Info.ModID)) return;

        harmony = new Harmony(Mod.Info.ModID);
        harmony.PatchAllUncategorized();
    }

    public override void Dispose()
    {
        harmony?.UnpatchAll(harmony.Id);
        base.Dispose();
    }
}

[HarmonyPatch(typeof(BEBehaviorDoor), nameof(BEBehaviorDoor.ToggleDoorState))]
internal static class CrudeDoorPatch
{
    [HarmonyPostfix]
    private static void PlayLaugh(BEBehaviorDoor __instance)
    {
        var world = __instance.Api.World;
        var pos = __instance.Pos;
        var air = 0;

        // ln 15: toggleDoorState does also run for opening doors
        // so this exists to not make the laugh play for doors that did not break
        // .. and if for some reason there will ever exist more door-crude's (modded, whatever)
        if (__instance.Api.Side != EnumAppSide.Server || __instance.Block.Code.Path != "door-crude" || world.BlockAccessor.GetBlock(pos).Id != air) return;

        var laughAssLoc = new AssetLocation("crudedoorhaha", $"sounds/laugh{world.Rand.Next(2, 11)}");
        world.PlaySoundAt(laughAssLoc, pos, 0, null, false, range: 32, volume: 1);
    }
}
