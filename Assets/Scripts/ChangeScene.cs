using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// �� ü���� ����
/// </summary>
public class ChangeScene : MonoBehaviour
{
    /// <summary>
    /// �� ü����(�ݹ����� ȣ��)
    /// </summary>
    /// <param name="index">������ ��</param>
    public void ChangeSceneEnter(int index)
    {
        SceneManager.LoadScene(index);
    }
}
