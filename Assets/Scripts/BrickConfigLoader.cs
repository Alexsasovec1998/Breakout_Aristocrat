using UnityEngine;

public class BrickConfigLoader : MonoBehaviour
{
    public string fileName = "brickConfiguration"; // Name of the file

    public BrickConfigLoader LoadLayout()
    {
        TextAsset jsonText = Resources.Load<TextAsset>(fileName);

        if (jsonText == null)
        {
            Debug.LogError("Brick layout file not found!");
            return null;
        }

        BrickConfigLoader layoutConfig = JsonUtility.FromJson<BrickConfigLoader>(jsonText.ToString());

        return layoutConfig;
    }
}
