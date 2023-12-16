using HarmonyLib;
using PotionCraft.ManagersSystem.Game;
using PotionCraft.SceneLoader;

namespace Shelves
{
    class GamePatchManager
    {
        [HarmonyPostfix, HarmonyPatch(typeof(GameManager), "Start")]
        public static void Start_Postfix()
        {
            ObjectsLoader.AddLast("SaveLoadManager.SaveNewGameState", () => Functions.PlaceShelves());
        }
    }
}