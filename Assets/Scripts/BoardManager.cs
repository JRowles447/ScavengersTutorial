using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;


public class BoardManager : MonoBehaviour
{
    [Serializable]
    public class Count
    {
        public int minimum;
        public int maximum; 

        public Count (int min, int max)
        {
            minimum = min;
            maximum = max;
        }
    }

    // game board size 
    public int columns = 8;
    public int rows = 8;

    public Count wallCount = new Count(5, 9);
    public Count foodCount = new Count(1, 5);

    // variables to hold prefabs
    public GameObject exit;
    // arrays for multiple objects and choice of spawn for variations
    public GameObject[] floorTiles;
    public GameObject[] wallTiles;
    public GameObject[] foodTiles;
    public GameObject[] enemyTiles;
    public GameObject[] outerWallTiles;
    // fill in prefab in the inpsector

    // Keep them objects collapsed in hierarchy - keep it clean
    private Transform boardHolder;
    // list of possible positions on the board
    private List<Vector3> gridPositions = new List<Vector3>();


    // Initialize list of grid positions
    void InitialiseList()
    {
        gridPositions.Clear();

        // start at 1 to keep the border clear so there are not impossible levels
        for (int x = 1; x < columns - 1; x++)
        {
            for (int y = 1; y < rows - 1; y++)
            {
                gridPositions.Add(new Vector3(x, y, 0f));
            }
        }
    }


    // Use this to set up outerwalls and floor 
    void BoardSetup()
    {
        boardHolder = new GameObject("Board").transform;

        for (int x = -1; x < columns + 1; x++)
        {
            for (int y = -1; y < rows + 1; y++)
            {
                GameObject toInstantiate = floorTiles[Random.Range(0, floorTiles.Length)];
                // outer wall components
                if(x == -1 || x == columns || y == -1 || y == rows)
                {
                    toInstantiate = wallTiles[Random.Range(0, wallTiles.Length)];
                }

                GameObject instance = Instantiate(toInstantiate, new Vector3(x, y, 0f), Quaternion.identity) as GameObject;

                //set parent to track the object under the boardHolder object
                instance.transform.SetParent(boardHolder);
            }
        }
    }


    // Generate random position on the game board for items, wallTiles, and enemies
    Vector3 RandomPosition()
    {
        int randomIndex = Random.Range(0, gridPositions.Count);
        Vector3 randomPosition = gridPositions[randomIndex];
        // dont want duplicates
        gridPositions.RemoveAt(randomIndex);
        return randomPosition;
    }

    // spawn object at random position 
    void LayoutObjectAtRandom(GameObject[] tileArray, int minimum, int maximum)
    {
        int objectCount = Random.Range(minimum, maximum + 1);

        for (int i = 0; i < objectCount; i++)
        {
            Vector3 randomPosition = RandomPosition();
            GameObject tileChoice = tileArray[Random.Range(0, tileArray.Length)];

            Instantiate(tileChoice, randomPosition, Quaternion.identity);
        }
    }


    // this is the only public function in the class. This function will be called by the GameManager to 
    // lay out the game board
    public void SetupScene(int level)
    {
        // Create the outer walls and the floor
        BoardSetup();
        // Create the list of positions on the board
        InitialiseList();

        // Instantiate random wall barriers based on min and max values at random grid positions
        LayoutObjectAtRandom(wallTiles, wallCount.minimum, wallCount.maximum);
        // Instantiate food tiles based on the min and max values at random grid positions
        LayoutObjectAtRandom(foodTiles, foodCount.minimum, foodCount.maximum);

        // logarithmic increase in enemy count as player progresses in levels
        int enemyCount = (int)Mathf.Log(level, 2f);
        // Instantiate enemyCount enemies at random board positions
        LayoutObjectAtRandom(enemyTiles, enemyCount, enemyCount);

        // exit is always at the top right of board
        Instantiate(exit, new Vector3(columns - 1, rows - 1, 0f), Quaternion.identity);
    }

}
