using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseManager : MonoBehaviour
{
    public static MouseManager instance;
    // Start is called before the first frame update
    private void Awake()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
   
    }

    public Collider2D MouseRayCast(string tag)
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero, 0f);

        if (hit.collider != null && hit.collider.CompareTag(tag))
        {
            return hit.collider;
        }
        else
            return null;
    }
}
