using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 씬 체인지 관리
/// </summary>
public class ChangeScene : MonoBehaviour
{
    /// <summary>
    /// 씬 체인지(콜백으로 호출)
    /// </summary>
    /// <param name="index">변경할 씬</param>
    public void ChangeSceneEnter(int index)
    {
        SceneManager.LoadScene(index);
    }
}
