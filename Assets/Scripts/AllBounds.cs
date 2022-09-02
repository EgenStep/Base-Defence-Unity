using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BasDefence
{    
    public class AllBounds : MonoBehaviour
    {
        public GameObject[] fencePrefabs;
        public float[] fenceLength;
        public GameObject forceBarrierPrefab;
        public float forceBarrierHeight;
        int fenceIndex;        
        public Vector3 minPoint;
        public Vector3 maxPoint;
        public float gateZ;
        public float gateLength;
        Vector3 leftUp;
        Vector3 rightDown;
        Vector3 gateLeft;
        Vector3 gateRight;
        float centreX;

        // Start is called before the first frame update
        void Start()
        {
            fenceIndex = 0;
            leftUp = new Vector3(minPoint.x, 0, maxPoint.z);
            rightDown = new Vector3(maxPoint.x, 0, minPoint.z);
            FencesLine(minPoint, leftUp);
            FencesLine(leftUp, maxPoint);
            FencesLine(maxPoint, rightDown);
            FencesLine(rightDown, minPoint);
            fenceIndex = 1;
            centreX = (maxPoint.x + minPoint.x) / 2f;
            gateLeft = new Vector3(centreX - gateLength / 2f, 0, gateZ);
            gateRight = new Vector3(centreX + gateLength / 2f, 0, gateZ);
            Vector3 gateLeft0 = new Vector3(minPoint.x, 0, gateZ);
            Vector3 gateRight1 = new Vector3(maxPoint.x, 0, gateZ);
            FencesLine(gateLeft0, gateLeft);
            FencesLine(gateRight, gateRight1);
            fenceIndex = 2;
            float shift = fenceLength[fenceIndex] / 2f;
            MakeGate(shift);            
        }      
        
        void MakeGate(float shift)
        {
            var sandLeft = Instantiate(fencePrefabs[fenceIndex], gateLeft + new Vector3(-shift, 0, shift), Quaternion.identity);
            sandLeft.transform.parent = transform;
            var sandRight = Instantiate(fencePrefabs[fenceIndex], gateRight + new Vector3(shift, 0, shift), Quaternion.identity);
            sandRight.transform.parent = transform;
            var forceBarrier = Instantiate(forceBarrierPrefab, new Vector3(centreX, forceBarrierHeight / 2f, gateZ + shift * 1.6f), Quaternion.identity);
            forceBarrier.transform.parent = transform;
            forceBarrier.transform.localScale = new Vector3(gateLength, forceBarrierHeight, 0.01f);
        }

        void FencesLine(Vector3 from, Vector3 to)
        {
            Vector3 direction = to - from;
            float size = direction.magnitude;
            var count = (int)(size / fenceLength[fenceIndex]);
            Vector3 step = direction / count;

            for (int i = 0; i < count; i++)
            {
                var coord = from + step / 2f + i * step;
                var fence = Instantiate(fencePrefabs[fenceIndex], coord, Quaternion.identity);
                fence.transform.rotation = Quaternion.FromToRotation(transform.right, direction);
                fence.transform.parent = transform;
            }
        }
    }
}
