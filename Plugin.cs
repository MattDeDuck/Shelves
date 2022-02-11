using BepInEx;
using BepInEx.Logging;
using UnityEngine;
using HarmonyLib;
using ObjectBased.Shelf;
using System.Linq;

namespace Shelves
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, "1.0.1.0")]
    public class Plugin : BaseUnityPlugin
    {
        public static ManualLogSource Log { get; set; }

        // List of rooms
        public static string[] rooms = new string[] { "Room Bedroom", "Room Meeting", "Room Lab", "Room Basement" };

        // List of locations for the rooms
        public static Vector3[] shelvesBedroom = new Vector3[] { new Vector3(-9.1f, 1.6f, 0f), new Vector3(4.5f, 1f, 0f) };
        public static Vector3[] shelvesMeeting = new Vector3[] { new Vector3(-5f, 1f, 0f), new Vector3(-5f, -5.73f, 0f) };        
        public static Vector3[] shelvesLab = new Vector3[] { new Vector3(-9.9f, -5.73f, 0f), new Vector3(-5f, -5.73f, 0f) };
        public static Vector3[] shelvesBasement = new Vector3[] { new Vector3(3.7f, 3.5f, 0f), new Vector3(-0.6f, 5f, 0f) };

        // Help with naming the shelves
        public static int rmNumber = 1;

        private void Awake()
        {
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");

            Log = this.Logger;

            Harmony.CreateAndPatchAll(typeof(Plugin));
        }

        private static void CustomShelves(string roomName)
        {
            if(roomName == rooms[0]) // Basement
            {
                rmNumber = 1;
                foreach(Vector3 loc in shelvesBedroom)
                {
                    CreateShelf(loc, roomName);
                    rmNumber++;
                }
                Log.LogInfo($"{shelvesBedroom.Length} Shelves created in the Bedroom");
            }
            if (roomName == rooms[1]) // Shop
            {
                rmNumber = 1;
                foreach (Vector3 loc in shelvesMeeting)
                {
                    CreateShelf(loc, roomName);
                    rmNumber++;
                }
                Log.LogInfo($"{shelvesMeeting.Length} Shelves created in the Shop");
            }
            if (roomName == rooms[2]) // Lab
            {
                rmNumber = 1;
                foreach (Vector3 loc in shelvesLab)
                {
                    CreateShelf(loc, roomName);
                    rmNumber++;
                }
                Log.LogInfo($"{shelvesLab.Length} Shelves created in the Lab");
            }
            if (roomName == rooms[3]) // Basement
            {
                rmNumber = 1;
                foreach (Vector3 loc in shelvesBasement)
                {
                    CreateShelf(loc, roomName);
                    rmNumber++;
                }
                Log.LogInfo($"{shelvesBasement.Length} Shelves created in the Basement");
            }
        }

        private static void CreateShelf(Vector3 pos, string rmName)
        {
            // Grab the shelf object from resources
            Shelf shelf = Resources.FindObjectsOfTypeAll<Shelf>().First();

            // Make a clone of it
            shelf = Instantiate(shelf, GameObject.Find(rmName).transform);

            // Move it to the position
            shelf.transform.localPosition = pos;

            // Give it a name
            shelf.name = "(Shelf) " + rmNumber.ToString();
        }

        private static void MoveLabEquipment()
        {
            Log.LogInfo("Re-arranging the lab...");

            // Mortar and pestle
            GameObject.Find("Mortar").transform.localPosition = new Vector3(0f, -5.149f, 0f);
            GameObject.Find("Pestle").transform.localEulerAngles = new Vector3(0f, 0f, 28.2356f);
            GameObject.Find("Pestle").transform.localPosition = new Vector3(-0.5585f, -4.267f, 0f);
            GameObject.Find("Pestle").transform.localRotation = new Quaternion(0f, 0f, 28.2356f, 0f);

            // Cauldron and spoon
            GameObject.Find("Cauldron").transform.localPosition = new Vector3(4.5f, -4.276f, 0f);
            GameObject.Find("Spoon").transform.localEulerAngles = new Vector3(0f, 0f, 330.535f);
            GameObject.Find("Spoon").transform.localPosition = new Vector3(5f, -2.4783f, 0f);
            GameObject.Find("Spoon").transform.localRotation = new Quaternion(0f, 0f, 330.535f, 0f);

            // Bellows
            GameObject.Find("Bellows").transform.localPosition = new Vector3(-0.6f, -5.801f, 0f);

            // Water ladle
            GameObject.Find("Ladle").transform.localPosition = new Vector3(1.5f, -4.149f, 0f);
        }

        [HarmonyPostfix, HarmonyPatch(typeof(RoomManager), "OrderedStart")]
        public static void OrderedStart_Postfix()
        {
            Log.LogInfo("Creating Shelves...");
            foreach(string rm in rooms)
            {
                CustomShelves(rm);
            }

            MoveLabEquipment();
        }
    }
}
