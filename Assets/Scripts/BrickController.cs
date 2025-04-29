using UnityEngine;

public class BrickController : MonoBehaviour
{
    public int points = 1;
    public bool isExtraBall = false;
    /*private void OnDestroy()
    {
        if (isExtraBall)
        {
            GameController.Instance.BrickDestroyed(gameObject);
        }
    }*/
    public void DestroySelf()
    {
        if (GameController.Instance != null)
        {
            GameController.Instance.BrickDestroyed(gameObject);
        }

        Destroy(gameObject);
    }
}
