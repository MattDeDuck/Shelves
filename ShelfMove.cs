using UnityEngine;

namespace Shelves
{
    public class ShelfMove : MonoBehaviour, IDraggablePrimary
    {
        public bool dragging = false;
        public float distance;

        public void OnGrabPrimary()
        {
            distance = Vector3.Distance(transform.position, Camera.main.transform.position);
            dragging = true;
            Debug.Log(this.gameObject.name + " is being moved");
        }

        public void OnReleasePrimary()
        {
            dragging = false;
            Debug.Log(this.gameObject.name + " has been placed");
        }

        public void Update()
        {
            if (dragging)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                Vector3 rayPoint = ray.GetPoint(distance);
                transform.position = rayPoint;
            }
        }
    }
}
