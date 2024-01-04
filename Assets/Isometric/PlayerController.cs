using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Tilemaps;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    public float speed = 5f;
    public float tileSize = 0.5f;
    public Tilemap tileMap;
    private Vector3 clickPos = Vector3.zero;
    private Vector3Int tilemapCell = Vector3Int.zero;
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 pos = transform.position;
            Vector3 dir = TilemapInfo() - pos;
            dir.x = Mathf.Abs(dir.x);
            dir.y = Mathf.Abs(dir.y);
            if(dir.x < 1 && dir.y < 0.5)
            transform.DOMove(new Vector3(TilemapInfo().x, TilemapInfo().y, -1f), 1f);
            //GameObject tileobj = tileMap.GetInstantiatedObject(tilemapCell);
            //Debug.Log(tileobj.transform.position);
        }
    }

    public Vector3 TilemapInfo()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -1));

        tilemapCell = tileMap.WorldToCell(mousePos);
        clickPos = tileMap.GetCellCenterLocal(tilemapCell);
        return clickPos;
    }
}
