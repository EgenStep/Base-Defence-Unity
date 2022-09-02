using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BasDefence
{
    public class Joystick : MonoBehaviour
    {
        [SerializeField]
        GameObject marker;
        Vector3 pos;
        [HideInInspector]
        public Vector3 direction;
        [SerializeField]
        float radiusInit;
        float radius;

        // Start is called before the first frame update
        void Start()
        {
            marker.transform.position = transform.position;
            direction = Vector3.zero;
            radius = radiusInit;
#if UNITY_ANDROID
            Debug.Log("Android on joystick");
            radius = radiusInit * gameObject.GetComponentInParent<CanvasJoystick>().androidScale;
           
#endif
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetMouseButton(0))
            {
                pos = Input.mousePosition;
                direction = pos - transform.position;
                if (direction.magnitude > radius)
                {
                    direction = Vector3.zero;
                    return;
                }
                marker.transform.position = pos;
            }
            else
            {
                marker.transform.position = transform.position;
                direction = Vector3.zero;
            }
        }
    }
}
