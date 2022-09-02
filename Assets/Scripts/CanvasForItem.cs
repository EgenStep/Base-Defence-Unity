using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasForItem : MonoBehaviour
{
    [SerializeField]
    GameObject CanvasJoystick;
    float scaleAndroid;
    CanvasScaler cs;

    // Start is called before the first frame update
    void Start()
    {
#if UNITY_ANDROID
        scaleAndroid = CanvasJoystick.GetComponent<CanvasJoystick>().androidScale;
        cs = GetComponent<CanvasScaler>();
        cs.scaleFactor = scaleAndroid;
#endif
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
