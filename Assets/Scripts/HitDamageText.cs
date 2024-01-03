using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitDamageText : MonoBehaviour
{
    float destroyTimer;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        destroyTimer += Time.deltaTime;
        transform.Translate(new Vector2(0f, 1f));
        if (destroyTimer > 0.5f)
            gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        Destroy(gameObject);
    }
}
