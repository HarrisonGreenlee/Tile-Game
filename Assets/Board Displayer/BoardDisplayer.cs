/*

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

using TileGame;

// A class to display the board in Unity
// TODO - non-hardcoded board sizes
public class BoardDisplayer : MonoBehaviour
{
    // Tell the BoardDisplayer which prefab to use for tiles when displaying the game board
    // Must be assigned in Unity
    [SerializeField]GameObject TilePrefab;

    // Tell the BoardDisplayer which solver to keep in sync with
    // Must be assigned in Unity
    [SerializeField]Board board;

    // Start is called before the first frame update
    void Start()
    {
        // Find and assign=
        //WARNING - may cause problems if there are multiple boards in the scene
        //board = FindObjectOfType<Board>();

        // Create every tile in the board
        for (int i = 0; i < 9; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                // Set the board tile to a copy of the prefab
                GameObject boardTile = Instantiate(new Sprite());
                // Set parent transform
                boardTile.transform.parent = transform;
                // Change local position based on the parent transform
                // X and Y are swapped and i is subtracted by eight because of linear algebra magic - changing this code will cause the tiles to be displayed sideways or upside down
                boardTile.transform.localPosition = new Vector2(j, 8-i);
                // Place a reference to the board tile's input field into the board array
                board[i, j] = boardTile.GetComponentInChildren<TMP_InputField>();
                // Tell the tile where it exists within the displayed tile array
                boardTile.GetComponentInChildren<TileInput>().SetCoords(i,j);
                // Give the tile a reference to the solver so it can keep the solver updated
                boardTile.GetComponentInChildren<TileInput>().SetSodokuSolver(solver);
            }
        }
    }

    // Used by the solver by to display tiles when the board is solved
    public void SetGridValue(int i, int j, object value)
    {
        board[i, j].SetTextWithoutNotify(value.ToString());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

*/