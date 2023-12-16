using BepInEx.Logging;
using PotionCraft.ObjectBased.Shelf;
using System.Linq;
using UnityEngine;

namespace Shelves
{
    public class Functions
    {
        static ManualLogSource Log => Plugin.Log;

        public static void CloneShelf(string name, int num, string room, Vector3 pos, bool isHidden = false)
        {
            // Grab the shelf object from resources
            Shelf shelf = Resources.FindObjectsOfTypeAll<Shelf>().First();

            // Make a clone of it
            shelf = Plugin.Instantiate(shelf, GameObject.Find(room).transform);

            // Move it to the position
            shelf.transform.localPosition = pos;

            if (isHidden)
            {
                GameObject shelfChild = shelf.transform.GetChild(0).gameObject;
                var sr = shelfChild.GetComponent<SpriteRenderer>();
                sr.enabled = false;
            }
            else
            {
                GameObject shelfChild = shelf.transform.GetChild(0).gameObject;
                var sr = shelfChild.GetComponent<SpriteRenderer>();
                sr.enabled = true;
            }

            shelf.name = "(Shelf) " + num + " " + name;

            Log.LogInfo($"Added shelf {num} in {room}");
        }

        public static void PlaceShelves()
        {
            Log.LogInfo("Building Shelves...");

            int shelveNum = 1;
            foreach (Vector3 pos in Storage.shelvesMeeting)
            {
                CloneShelf("Meeting", shelveNum, "Room Meeting", pos);
                shelveNum += 1;
            }

            shelveNum = 1;
            foreach (Vector3 pos in Storage.shelvesBasement)
            {
                CloneShelf("Basement", shelveNum, "Room Basement", pos);
                shelveNum += 1;
            }

            shelveNum = 1;
            foreach (Vector3 pos in Storage.shelvesBedroom)
            {
                CloneShelf("Bedroom", shelveNum, "Room Bedroom", pos);
                shelveNum += 1;
            }

            CloneShelf("Desk", 1, "Room Bedroom", Storage.shelvesDesk, true);
        }
    }
}