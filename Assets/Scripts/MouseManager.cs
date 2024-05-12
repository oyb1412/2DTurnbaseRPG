using UnityEngine;

/// <summary>
/// 마우스 관리
/// </summary>
public class MouseManager : MonoBehaviour
{
    [Header("--Instance--")]
    public static MouseManager instance;
    private void Awake()
    {
        instance = this;
    }

    /// <summary>
    /// 마우스 Enter오브젝트 반환
    /// </summary>
    /// <param name="tag">검출 tag</param>
    /// <returns></returns>
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
