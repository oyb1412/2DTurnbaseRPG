using UnityEngine;

/// <summary>
/// �ʵ� �� ���� ����
/// </summary>
public class MoveCloud : MonoBehaviour
{
    void Update()
    {
        if(transform.position.x > -24)
             transform.Translate(new Vector3(-0.001f, 0, 0));
        else
            transform.Translate(new Vector3(24f, 0, 0));
    }
}
