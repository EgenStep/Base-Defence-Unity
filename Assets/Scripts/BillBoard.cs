using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BasDefence
{
    public class BillBoard : MonoBehaviour
    {
        [SerializeField]
        Transform cam;

        // Update is called once per frame
        void LateUpdate()
        {
            var direction = cam.transform.position - transform.position;
            transform.LookAt(transform.position + cam.forward);
            transform.position = transform.parent.position + direction.normalized * 5f;
        }
    }
}
