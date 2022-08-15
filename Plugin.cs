using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using PotionCraft.ManagersSystem.Room;
using PotionCraft.ObjectBased.Shelf;
using System.Linq;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace Shelves
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, "1.0.4.0")]
    public class Plugin : BaseUnityPlugin
    {
        // Logging source
        public static ManualLogSource Log { get; set; }

        // Get the plugin location
        public static string pluginLoc = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        // List of rooms
        public static string[] rooms = new string[] { "Room Bedroom", "Room Meeting", "Room Lab", "Room Basement" };

        // List of locations for the rooms
        public static Vector3[] shelvesBedroom = new Vector3[] { new Vector3(-9.1f, 1.6f, 0f), new Vector3(-9.1f, -0.7f, 0f), new Vector3(3.6f, 0.5f, 0f) };
        public static Vector3[] shelvesMeeting = new Vector3[] { new Vector3(-8f, 5f, 0f), new Vector3(-8f, 2.5f, 0f), new Vector3(-9.9f, -5.73f, 0f), new Vector3(-5f, -5.73f, 0f) };
        public static Vector3[] shelvesLab = new Vector3[] { new Vector3(-9.9f, -5.73f, 0f), new Vector3(-5f, -5.73f, 0f) };
        public static Vector3[] shelvesBasement = new Vector3[] { new Vector3(3.7f, 3.5f, 0f), new Vector3(-0.6f, 5f, 0f) };
        public static Vector3 shelvesDesk = new Vector3(4.5f, -2.2f, 0f);

        private void Awake()
        {
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");

            Log = this.Logger;

            // Make sure Harmony patches
            Harmony.CreateAndPatchAll(typeof(Plugin));
        }

        public static void CreateShelf(string name, int num, string room, Vector3 pos, bool isHidden = false)
        {
            // Grab the shelf object from resources
            Shelf shelf = Resources.FindObjectsOfTypeAll<Shelf>().First();

            // Make a clone of it
            shelf = Instantiate(shelf, GameObject.Find(room).transform);

            // Move it to the position
            shelf.transform.localPosition = pos;

            if(isHidden)
            {
                GameObject shelfChild = shelf.transform.GetChild(0).gameObject;
                var sr = shelfChild.GetComponent<SpriteRenderer>();
                sr.enabled = false;
            }else
            {
                GameObject shelfChild = shelf.transform.GetChild(0).gameObject;
                var sr = shelfChild.GetComponent<SpriteRenderer>();
                sr.enabled = true;
            }

            shelf.name = "(Shelf) " + num + " " + name;

            Log.LogInfo($"Added shelf {num} in {room}");
        }

        [HarmonyPostfix, HarmonyPatch(typeof(RoomManager), "OrderedStart")]
        public static void OrderedStart_Postfix()
        {
            Log.LogInfo("Creating Shelves...");

            int shelveNum = 1;
            foreach(Vector3 pos in shelvesMeeting)
            {
                CreateShelf("Meeting", shelveNum, "Room Meeting", pos);
                shelveNum += 1;
            }

            shelveNum = 1;
            foreach (Vector3 pos in shelvesLab)
            {
                CreateShelf("Lab", shelveNum, "Room Lab", pos);
                shelveNum += 1;
            }

            shelveNum = 1;
            foreach (Vector3 pos in shelvesBasement)
            {
                CreateShelf("Basement", shelveNum, "Room Basement", pos);
                shelveNum += 1;
            }

            shelveNum = 1;
            foreach (Vector3 pos in shelvesBedroom)
            {
                CreateShelf("Bedroom", shelveNum, "Room Bedroom", pos);
                shelveNum += 1;
            }

            CreateShelf("Desk", 1, "Room Bedroom", shelvesDesk, true);
        }
    }   
}