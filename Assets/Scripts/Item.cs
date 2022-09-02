using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BasDefence
{
    public class Item : MonoBehaviour
    {
        public enum Type { coin = 0, gem = 1 }
        public Type type;
        [SerializeField]
        float angleVelosity;
        [SerializeField]
        float y;
        [SerializeField]
        float dropR;
        Vector3 scaleInit;
        Vector3 scaleStart;
        Vector3 scaleEnd;
        Vector3 posStart;
        Vector3 posEnd;
        Vector3 eulerStart;
        Vector3 eulerEnd;

        // Start is called before the first frame update
        void Start()
        {
            transform.position += Vector3.up * y;
            scaleInit = transform.localScale;
        }

        // Update is called once per frame
        void Update()
        {
            transform.eulerAngles += Vector3.up * angleVelosity * Time.deltaTime;
        }

        internal void PlayerDeath(float scaleItemInBag)
        {
            transform.parent = null;
            scaleEnd = scaleInit;
            scaleStart=transform.localScale;
            float alpha = Random.Range(0f, 180f);
            eulerStart = transform.localEulerAngles;
            eulerEnd = new Vector3(transform.eulerAngles.x - 90f, alpha, 0f);
            //transform.eulerAngles = new Vector3(transform.eulerAngles.x - 90f, alpha, 0f);
            float x = Random.Range(-dropR, dropR);
            float z = Random.Range(-dropR, dropR);
            posStart = transform.position;
            var pos = new Vector3(x, 0f, z);
            pos += transform.position;
            pos.y = y;
            posEnd = pos;
            //transform.position = pos;
            //transform.localScale = scaleEnd;
            StartCoroutine(DropItemAnimate());
        }

        IEnumerator DropItemAnimate()
        {
            float time = 0f;
            float dT = 0.05f;
            float duration = 1.0f;
            while (time < duration) 
            {
                float x = time / duration;
                float y = (1f - 4f * (x - 0.5f) * (x - 0.5f)) * 2f;
                transform.position = Vector3.Lerp(posStart, posEnd, x) + Vector3.up * y;
                transform.localScale = Vector3.Lerp(scaleStart, scaleEnd, x);
                transform.eulerAngles = Vector3.Lerp(eulerStart, eulerEnd, x);
                time += dT;
                yield return new WaitForSeconds(dT);
            }
            yield return null;
        }


    }
}
