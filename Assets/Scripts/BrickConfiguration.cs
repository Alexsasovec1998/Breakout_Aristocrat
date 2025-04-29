[System.Serializable]
public class BrickConfiguration
{
    public int columns; 
    public System.Collections.Generic.List<BrickData> bricks; 
}

[System.Serializable]
public class BrickData
{
    public int row;
    public int col;
    public string color;
    public int points;
    public bool isExtraBall;
}
