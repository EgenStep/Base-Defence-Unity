using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyBoard : MonoBehaviour
{
    Dictionary<KeyCode, Vector2> keys = new Dictionary<KeyCode, Vector2>();
    [HideInInspector]
    internal Vector3 direction;


    // Start is called before the first frame update
    void Start()
    {
        keys.Add(KeyCode.W, Vector2.up);
        keys.Add(KeyCode.S, Vector2.down);
        keys.Add(KeyCode.A, Vector2.left);
        keys.Add(KeyCode.D, Vector2.right);
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 dirXY = Vector2.zero;
        foreach(var key in keys)
        {
            var code = key.Key;
            var dir = key.Value;
            if (Input.GetKey(code)) dirXY += dir;
        }
        dirXY = dirXY.normalized;
        direction = new Vector3(dirXY.x, dirXY.y, 0f) * 10f;
    }
}
