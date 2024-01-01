using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCloud : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(transform.position.x > -24)
             transform.Translate(new Vector3(-0.001f, 0, 0));
        else
            transform.Translate(new Vector3(24f, 0, 0));

    }
}
