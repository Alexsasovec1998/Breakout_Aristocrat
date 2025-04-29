using UnityEngine;
using UnityEngine.SceneManagement; 
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    public Button standardModeButton;
    public Button customModeButton;

    void Start()
    {
        standardModeButton.onClick.AddListener(OnStandardModeClick);
        customModeButton.onClick.AddListener(OnCustomModeClick);
    }

    void OnStandardModeClick()
    {
        SceneManager.LoadScene("SampleScene"); 
    }

    void OnCustomModeClick()
    {       
        SceneManager.LoadScene("CustomBrickLayoutMode");
    }
}
