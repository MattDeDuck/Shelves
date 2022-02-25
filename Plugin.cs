using BepInEx;
using BepInEx.Logging;
using UnityEngine;
using HarmonyLib;
using ObjectBased.Shelf;
using System.Linq;
using System.Collections.Generic;
using System.Reflection.Emit;
using System;
using System.IO;
using System.Reflection;

namespace Shelves
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, "1.0.4.0")]
    public class Plugin : BaseUnityPlugin
    {
        // Logging source
        public static ManualLogSource Log { get; set; }

        // Get the plugin location
        public static string pluginLoc = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        // List of rooms
        public static string[] rooms = new string[] { "Room Bedroom", "Room Meeting", "Room Lab", "Room Basement" };

        // List of locations for the rooms
        public static Vector3[] shelvesBedroom = new Vector3[] { new Vector3(-9.1f, 1.6f, 0f), new Vector3(4.5f, 1f, 0f), new Vector3(-9.1f, -1f, 0f), new Vector3(4.5f, 1.6f, 0f) };
        public static Vector3[] shelvesMeeting = new Vector3[] { new Vector3(-8f, 5f, 0f), new Vector3(-8f, 2.5f, 0f), new Vector3(-9.9f, -5.73f, 0f), new Vector3(-5f, -5.73f, 0f) };
        public static Vector3[] shelvesLab = new Vector3[] { new Vector3(-9.9f, -5.73f, 0f), new Vector3(-5f, -5.73f, 0f) };
        public static Vector3[] shelvesBasement = new Vector3[] { new Vector3(3.7f, 3.5f, 0f), new Vector3(-0.6f, 5f, 0f) };

        // Help with naming the shelves
        public static int rmNumber = 1;

        // Create shelf texture and sprite
        public static Texture2D customShelfTexture;
        public static Sprite customShelfSprite;

        // Dictionary to convert roomName to object name
        public static Dictionary<string, string> actualRoom = new Dictionary<string, string>();

        private void Awake()
        {
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");

            Log = this.Logger;

            // Grab texture from plugin folder
            customShelfTexture = LoadTextureFromFile(pluginLoc + "/customshelf.png");

            // Create a sprite from the texture
            customShelfSprite = Sprite.Create(customShelfTexture, new Rect(0, 0, customShelfTexture.width, customShelfTexture.height), new Vector2(0.5f, 0.5f));

            // Add values to the dictionary
            actualRoom.Add("MeetingRoom", "Room Meeting");
            actualRoom.Add("GardenRoom", "Room Garden");
            actualRoom.Add("BasementRoom", "Room Basement");
            actualRoom.Add("LabRoom", "Room Lab");
            actualRoom.Add("BedroomRoom", "Room Bedroom");

            // Make sure Harmony patches
            Harmony.CreateAndPatchAll(typeof(Plugin));
        }

        // Remove collision of left trading panel
        [HarmonyPatch(typeof(Markers.TraderInventory), "Show")]
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            Log.LogInfo("Collision being patched");
            return new CodeMatcher(instructions)
                .MatchForward(false,
                    new CodeMatch(OpCodes.Call, AccessTools.PropertyGetter(typeof(Managers), "Trade"))
                )
                .SetOpcodeAndAdvance(OpCodes.Nop)
                .SetOpcodeAndAdvance(OpCodes.Nop)
                .SetOpcodeAndAdvance(OpCodes.Nop)
                .SetOpcodeAndAdvance(OpCodes.Nop)
                .SetOpcodeAndAdvance(OpCodes.Nop)
                .SetOpcodeAndAdvance(OpCodes.Nop)
                .SetOpcodeAndAdvance(OpCodes.Nop)
                .SetOpcodeAndAdvance(OpCodes.Nop)
                .InstructionEnumeration();
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

            // Change the sprite to a smaller shelf
            GameObject shelfChild = shelf.transform.GetChild(0).gameObject;
            var sr = shelfChild.GetComponent<SpriteRenderer>();
            sr.sprite = customShelfSprite;

            // Need to alter collider for smaller sprite
            GameObject shelfPC = shelf.transform.GetChild(1).gameObject;
            var bc = shelfPC.GetComponent<BoxCollider2D>();
            bc.size = new Vector2(1.6f, 0.05f);
            bc.offset = new Vector2(-0.015f, 0.1667f);

            // Add the behaviour so we can move it
            shelf.gameObject.AddComponent<ShelfMove>();

            // Move it to the position
            shelf.transform.localPosition = pos;

            // Give it a name
            shelf.name = "(Shelf) " + rmNumber.ToString();
        }

        // Move shelf
        private static void EnableShelfMove(GameObject rm, int shelfNo)
        {
            GameObject shelveObject = rm.transform.Find("(Shelf) " + shelfNo.ToString()).gameObject;
            if (shelveObject != null)
            {
                var dt = shelveObject.GetComponent<ShelfMove>();
                dt.dragging = true;
                Debug.Log(rm.name + " " + shelveObject.name + " being moved");
            }
        }

        // Place shelf
        private static void DisableShelfMove(GameObject rm, int shelfNo)
        {
            GameObject shelveObject = rm.transform.Find("(Shelf) " + shelfNo.ToString()).gameObject;
            if (shelveObject != null)
            {
                var dt = shelveObject.GetComponent<ShelfMove>();
                dt.dragging = false;
                Debug.Log(rm.name + " " + shelveObject.name + " has been placed");
            }
        }

        [HarmonyPostfix, HarmonyPatch(typeof(RoomManager), "OrderedStart")]
        public static void OrderedStart_Postfix()
        {
            Log.LogInfo("Creating Shelves...");
            
            // Create the shelves in each room
            foreach (string rm in rooms)
            {
                CustomShelves(rm);
            }
        }

        [HarmonyPostfix, HarmonyPatch(typeof(InputManager), "Update")]
        public static void Init_Postfix()
        {
            if (Input.GetMouseButton(2)) // Mouse wheel click
            {
                // Get the room name
                string roomName = Managers.Room.settings.rooms[(int)Managers.Room.currentRoom].name;

                // Convert it to room object name
                string curRoom = actualRoom[roomName];

                // Find the room object
                GameObject roomObject = GameObject.Find(curRoom).gameObject;

                // Move/Place shelf 1
                if(Input.GetKeyDown(KeyCode.Alpha1))
                {
                    EnableShelfMove(roomObject, 1);
                }
                if (Input.GetKeyUp(KeyCode.Alpha1))
                {
                    DisableShelfMove(roomObject, 1);
                }

                // Move/Place shelf 2
                if (Input.GetKeyDown(KeyCode.Alpha2))
                {
                    EnableShelfMove(roomObject, 2);
                }
                if (Input.GetKeyUp(KeyCode.Alpha2))
                {
                    DisableShelfMove(roomObject, 2);
                }

                // Move/Place shelf 3
                if (Input.GetKeyDown(KeyCode.Alpha3))
                {
                    EnableShelfMove(roomObject, 3);
                }
                if (Input.GetKeyUp(KeyCode.Alpha3))
                {
                    DisableShelfMove(roomObject, 3);
                }

                // Move/Place shelf 4
                if (Input.GetKeyDown(KeyCode.Alpha4))
                {
                    EnableShelfMove(roomObject, 4);
                }
                if (Input.GetKeyUp(KeyCode.Alpha4))
                {
                    DisableShelfMove(roomObject, 4);
                }
            }
        }

        // Texture loader
        public static Texture2D LoadTextureFromFile(string filePath)
        {
            var data = File.ReadAllBytes(filePath);

            // Do not create mip levels for this texture, use it as-is.
            var tex = new Texture2D(0, 0, TextureFormat.ARGB32, false, false)
            {
                filterMode = FilterMode.Bilinear,
            };

            if (!tex.LoadImage(data))
            {
                throw new Exception($"Failed to load image from file at \"{filePath}\".");
            }

            return tex;
        }
    }   
}