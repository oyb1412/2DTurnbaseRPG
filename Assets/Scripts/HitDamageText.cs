using UnityEngine;

/// <summary>
/// 타격시 데미지 텍스트
/// </summary>
public class HitDamageText : MonoBehaviour
{
    private float destroyTimer;

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
