using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasJoystick : MonoBehaviour
{
    CanvasScaler cs;
    [SerializeField]
    internal float androidScale;
    
    // Start is called before the first frame update
    void Start()
    {
        cs = GetComponent<CanvasScaler>();
        cs.scaleFactor = 1f;
#if UNITY_ANDROID
        Debug.Log("Android canvas joystick");
        cs.scaleFactor = androidScale;
#endif

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
