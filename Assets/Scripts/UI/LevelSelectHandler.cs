using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelSelectHandler : MonoBehaviour
{
    public string _levelName;
    private UnityEngine.UI.Button _levelButton;

    private void Start()
    {
        _levelButton = GetComponent<UnityEngine.UI.Button>();
        _levelButton.onClick.AddListener(OnLevelSelectButtonClick);
    }

    private void OnLevelSelectButtonClick()
    {
        SceneManager.LoadScene(_levelName);

    }
}
