using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BasDefence
{
    public class BaseDetector : MonoBehaviour
    {
        [SerializeField]
        GameObject player;
        int index = 0;

        // Start is called before the first frame update
        void Start()
        {


        }

        private void OnTriggerStay(Collider other)
        {
            if (other.gameObject.tag == "BaseTerritory")
            {
                player.GetComponent<PlayerAI>().onBaseTerritory = true;
                //Debug.Log("On base " + index);
                index++;
            }
        }
    }
}
