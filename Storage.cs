using UnityEngine;

namespace Shelves
{
    class Storage
    {
        // List of rooms
        public static string[] rooms = new string[] { "Room Bedroom", "Room Meeting", "Room Lab", "Room Basement" };

        // List of locations for the rooms
        public static Vector3[] shelvesBedroom = new Vector3[] { new Vector3(-9.1f, 1.6f, 0f), new Vector3(-9.1f, -0.7f, 0f), new Vector3(3.6f, 0.5f, 0f) };
        public static Vector3[] shelvesMeeting = new Vector3[] { new Vector3(-8f, 5f, 0f), new Vector3(-8f, 1.7f, 0f), new Vector3(-9.9f, -5.73f, 0f), new Vector3(-5f, -5.73f, 0f) };
        public static Vector3[] shelvesBasement = new Vector3[] { new Vector3(3.7f, 3.5f, 0f), new Vector3(-10.1f, 4.3f, 0f) };
        public static Vector3 shelvesDesk = new Vector3(4.5f, -2.2f, 0f);
    }
}
