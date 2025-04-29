using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;
using UnityEngine.SceneManagement;
using System;

public class GameController : MonoBehaviour
{
    public static GameController Instance;

    public GameObject wallPrefab;
    public float wallThickness = 0.5f;
    public float zPosition = 0f;

    public GameObject paddlePrefab;
    public GameObject ballPrefab;
    public GameObject brickPrefab;
    public GameObject extraBallPrefab;

    private GameObject paddleInstance;
    private int bricksRemaining;
    public Text winText;
    public Text levelText;
    private int currentLevel = 1;

    public float ballBaseSpeed = 5f;
    public float ballSpeedIncreaseFactor = 1.1f;
    private float currentBallSpeed;

    public bool IsFirstBallSpawned = false;

    private string levelFilePath = "Assets/Resources/BrickConfiguration.json";

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        currentBallSpeed = ballBaseSpeed;
        levelText.text = "Level " + currentLevel;
        SpawnWalls();
        SpawnPaddle();

        //Custom game mode will let play 1 level from file
        if (SceneManager.GetActiveScene().name == "CustomBrickLayoutMode")
        {
            LoadBricksFromJson();
        }
        else
        {
            SpawnBricks();
        }

        IsFirstBallSpawned = false;
        if (!IsFirstBallSpawned)
        {
            SpawnBall();
        }
    }

    Vector2 SpawnPaddle()
    {
        Vector2 paddleStartPos = new Vector2(0f, -4f);
        paddleInstance = Instantiate(paddlePrefab, paddleStartPos, Quaternion.identity);
        return paddleStartPos;
    }

    // I used FormatNumber to get slightly randomized paddle sizes each round
    void AdjustPaddleSize()
    {
        int paddleSize = FormatNumber(currentLevel);

        float newWidth = Mathf.Lerp(3f, 7f, paddleSize % 10 / 10f);

        Vector3 paddleScale = paddleInstance.transform.localScale;
        paddleScale.x = newWidth;
        paddleInstance.transform.localScale = paddleScale;

        Debug.Log($"Paddle size adjusted to: {newWidth}");
    }

    //This code has issue with converting int to binary. It was converting it to decimal.
    //+ base-1 shift logic was messed and conversion back to int was from decimal, not binary
    public static int FormatNumber(int number)
    {
        string binary = Convert.ToString(number, 2);

        char[] reversed = binary.ToCharArray();
        Array.Reverse(reversed);
        string reversedBinary = new string(reversed);

        int result = Convert.ToInt32(reversedBinary, 2);

        return result;
    }


    public void SpawnBall()
    {
        if (IsFirstBallSpawned) return;
        if (paddleInstance == null) return;

        Vector2 ballStartPos = paddleInstance.transform.position + new Vector3(0f, 0.5f, 0f);
        GameObject ballInstance = Instantiate(ballPrefab, ballStartPos, Quaternion.identity);

        BallController ballController = ballInstance.GetComponent<BallController>();
        if (ballController != null)
        {
            ballController.paddle = paddleInstance.transform;
            ballController.isFirstBall = true;
            IsFirstBallSpawned = true;
        }
    }

    public void SpawnExtraBall()
    {
        if (IsFirstBallSpawned)
        {
            Vector2 paddlePos = paddleInstance.transform.position;
            Vector2 extraBallStartPos = paddlePos + new Vector2(0f, 0.3f);
            GameObject extraBallInstance = Instantiate(ballPrefab, extraBallStartPos, Quaternion.identity);

            BallController ballController = extraBallInstance.GetComponent<BallController>();
            if (ballController != null)
            {
                ballController.paddle = paddleInstance.transform;
                ballController.isFirstBall = false;
            }
        }
    }

    public void RespawnBall()
    {
        BallController existingBall = FindObjectOfType<BallController>();
        if (existingBall != null)
        {
            Destroy(existingBall.gameObject);
        }

        IsFirstBallSpawned = false;
        Invoke(nameof(SpawnBall), 1f);
    }

    void SpawnWalls()
    {
        Camera cam = Camera.main;
        float screenHeight = 2f * cam.orthographicSize;
        float screenWidth = screenHeight * cam.aspect;
        Vector2 center = cam.transform.position;

        CreateWall(new Vector2(center.x - screenWidth / 2f - wallThickness / 2f, center.y), new Vector2(wallThickness, screenHeight));
        CreateWall(new Vector2(center.x + screenWidth / 2f + wallThickness / 2f, center.y), new Vector2(wallThickness, screenHeight));
        CreateWall(new Vector2(center.x, center.y + screenHeight / 2f + wallThickness / 2f), new Vector2(screenWidth, wallThickness));
    }

    void CreateWall(Vector2 position, Vector2 size)
    {
        GameObject wall = Instantiate(wallPrefab, new Vector3(position.x, position.y, zPosition), Quaternion.identity);
        wall.transform.localScale = new Vector3(size.x, size.y, 1f);
    }

    void LoadBricksFromJson()
    {
        string jsonContent = File.ReadAllText(levelFilePath);
        BrickConfiguration brickLayout = JsonUtility.FromJson<BrickConfiguration>(jsonContent);

        bricksRemaining = 0;

        foreach (var brickData in brickLayout.bricks)
        {
            Vector2 spawnPos = new Vector2(brickData.col, brickData.row);
            GameObject brick = Instantiate(brickPrefab, spawnPos, Quaternion.identity);
            BrickController brickControllerScript = brick.GetComponent<BrickController>();

            Color brickColor = Color.white;
            switch (brickData.color.ToLower())
            {
                case "red":
                    brickColor = Color.red;
                    break;
                case "green":
                    brickColor = Color.green;
                    break;
                case "yellow":
                    brickColor = Color.yellow;
                    break;
                case "blue":
                    brickColor = Color.blue;
                    break;
            }

            brick.GetComponent<SpriteRenderer>().color = brickColor;
            brickControllerScript.points = brickData.points;
            brickControllerScript.isExtraBall = brickData.isExtraBall;

            bricksRemaining++;
        }
    }

    void SpawnBricks()
    {
        winText.text = null;
        int rows = 8;
        int columns = 10;
        float spacing = 0f;

        Camera cam = Camera.main;
        float screenWidth = 2f * cam.orthographicSize * cam.aspect;

        float totalSpacing = (columns - 1) * spacing;
        float availableWidth = screenWidth - totalSpacing;
        float brickWidth = availableWidth / columns;
        float brickHeight = 0.5f;

        float startX = -screenWidth / 2f + brickWidth / 2f;
        float startY = 4f;

        bricksRemaining = 0;

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                Vector2 spawnPos = new Vector2(startX + col * brickWidth, startY - row * brickHeight);
                GameObject brick = Instantiate(brickPrefab, spawnPos, Quaternion.identity);
                brick.transform.localScale = new Vector3(brickWidth, brickHeight, 1f);

                Color brickColor = Color.white;
                BrickController brickControllerScript = brick.GetComponent<BrickController>();

                if (row < 2) // red
                {
                    brickColor = Color.red;
                    brickControllerScript.points = 7;
                }
                else if (row < 4) // orange
                {
                    brickColor = new Color(1f, 0.5f, 0f);
                    brickControllerScript.points = 5;
                }
                else if (row < 6) // green
                {
                    brickColor = Color.green;
                    brickControllerScript.points = 3;
                }
                else if (row == 7 && col == 5)
                {
                    brickColor = Color.blue;
                    brickControllerScript.points = 0;
                    brickControllerScript.isExtraBall = true;
                }
                else if (row == 7 && col == 8)
                {
                    brickColor = Color.blue;
                    brickControllerScript.points = 0;
                    brickControllerScript.isExtraBall = true;
                }
                else if (row == 7 && col == 2)
                {
                    brickColor = Color.blue;
                    brickControllerScript.points = 0;
                    brickControllerScript.isExtraBall = true;
                }
                else if (row == 7 && col == 4)
                {
                    brickColor = Color.blue;
                    brickControllerScript.points = 0;
                    brickControllerScript.isExtraBall = true;
                }
                else // YELLOW
                {
                    brickColor = Color.yellow;
                    brickControllerScript.points = 1;
                }

                brick.GetComponent<SpriteRenderer>().color = brickColor;
                bricksRemaining++;
            }
        }
    }

    public void BrickDestroyed(GameObject brick)
    {
        BrickController brickController = brick.GetComponent<BrickController>();
        if (brickController != null && brickController.isExtraBall)
        {
            SpawnExtraBall();
        }

        bricksRemaining--;
        Debug.Log(bricksRemaining);

        if (bricksRemaining <= 0)
        {
            WinRound();
        }
    }

    void WinRound()
    {
        Debug.Log("YOU WIN!");
        if (winText != null)
            winText.text = "YOU WIN!";

        if (SceneManager.GetActiveScene().name == "CustomBrickLayoutMode")
        {
            SceneManager.LoadScene("MainMenu");
        }
        ResetBall();
        currentBallSpeed *= ballSpeedIncreaseFactor;
        currentLevel++;
        levelText.text = "Level " + currentLevel;

        AdjustPaddleSize();

        Invoke(nameof(SpawnBricks), 2f);
    }

    void ResetBall()
    {
        IsFirstBallSpawned = false;
        BallController existingBall = FindObjectOfType<BallController>();
        if (existingBall != null)
        {
            Destroy(existingBall.gameObject);
        }

        Invoke(nameof(SpawnBall), 0.5f);
    }
}
