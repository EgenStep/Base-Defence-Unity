using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BasDefence
{
    public class ForceBarrier : MonoBehaviour
    {
        Material material;
        public float speed;
        // Start is called before the first frame update
        void Start()
        {
            material = GetComponent<Renderer>().material;
        }

        // Update is called once per frame
        void Update()
        {
            material.mainTextureOffset += new Vector2(speed * Time.deltaTime, 0f);
        }
    }
}
