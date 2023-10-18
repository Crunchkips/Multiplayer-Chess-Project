using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public enum ChessPieceType
{
    Empty,
    Pawn,
    Knight,
    Bishop,
    Rook,
    Queen,
    King
}

public enum ChessPieceColor
{
    Black,
    White,
    Null
}


public class ChessGameManager : MonoBehaviour
{

    public GameObject chessBoard;

    public List<ChessPiece> allChessPieces = new List<ChessPiece>();
    public GameObject[,] chessboard = new GameObject[8, 8];
    public Bounds chessboardBounds;

    //generic gameObject, will substitute with specific prefabs later
    public GameObject whitePawnPrefab;
    public GameObject whiteRookPrefab;
    public GameObject whiteKnightPrefab;
    public GameObject whiteBishopPrefab;
    public GameObject whiteQueenPrefab;
    public GameObject whiteKingPrefab;

    public GameObject blackPawnPrefab;
    public GameObject blackRookPrefab;
    public GameObject blackKnightPrefab;
    public GameObject blackBishopPrefab;
    public GameObject blackQueenPrefab;
    public GameObject blackKingPrefab;


    public TurnManager turnManager;
    public PlayerManager playerManager;
    public ChessPieceColor myColor = ChessPieceColor.Null;

    public List<Vector2Int> whiteLegalMoves = new List<Vector2Int>();
    public List<Vector2Int> blackLegalMoves = new List<Vector2Int>();


    public void FindAllChessPieces()
    {
        ChessPiece[] pieces = FindObjectsOfType<ChessPiece>();
        allChessPieces.AddRange(pieces);

    }

    public void SpawnChessPieces()
    {
        chessboardBounds = CalculateChessboardBounds();

        for (int row = 0; row < 8; row++)
        {
            for (int col = 0; col < 8; col++)
            {
                if (myColor == ChessPieceColor.White)
                {
                    if (row == 1)
                    {

                        GameObject chessPiece = Instantiate(whitePawnPrefab);


                        ChessPiece chessPieceProperties = chessPiece.GetComponent<ChessPiece>();
                        chessPieceProperties.row = row;
                        chessPieceProperties.col = col;
                        chessboard[col, row] = chessPiece;

                        Vector3 chessPiecePosition = CalculateChessPiecePosition(col, row, chessboardBounds);
                        chessPiece.transform.position = chessPiecePosition;
                    }

                    else if (row == 0 && (col == 0 || col == 7))
                    {
                        GameObject chessPiece = Instantiate(whiteRookPrefab);


                        ChessPiece chessPieceProperties = chessPiece.GetComponent<ChessPiece>();
                        chessPieceProperties.row = row;
                        chessPieceProperties.col = col;
                        chessboard[col, row] = chessPiece;

                        Vector3 chessPiecePosition = CalculateChessPiecePosition(col, row, chessboardBounds);
                        chessPiece.transform.position = chessPiecePosition;

                    }

                    else if (row == 0 && (col == 1 || col == 6))
                    {
                        GameObject chessPiece = Instantiate(whiteKnightPrefab);


                        ChessPiece chessPieceProperties = chessPiece.GetComponent<ChessPiece>();
                        chessPieceProperties.row = row;
                        chessPieceProperties.col = col;
                        chessboard[col, row] = chessPiece;

                        Vector3 chessPiecePosition = CalculateChessPiecePosition(col, row, chessboardBounds);
                        chessPiece.transform.position = chessPiecePosition;

                    }

                    else if (row == 0 && (col == 2 || col == 5))
                    {
                        GameObject chessPiece = Instantiate(whiteBishopPrefab);

                        ChessPiece chessPieceProperties = chessPiece.GetComponent<ChessPiece>();
                        chessPieceProperties.row = row;
                        chessPieceProperties.col = col;
                        chessboard[col, row] = chessPiece;

                        Vector3 chessPiecePosition = CalculateChessPiecePosition(col, row, chessboardBounds);
                        chessPiece.transform.position = chessPiecePosition;

                    }

                    else if (row == 0 && (col == 3))
                    {
                        GameObject chessPiece = Instantiate(whiteQueenPrefab);


                        ChessPiece chessPieceProperties = chessPiece.GetComponent<ChessPiece>();
                        chessPieceProperties.row = row;
                        chessPieceProperties.col = col;
                        chessboard[col, row] = chessPiece;

                        Vector3 chessPiecePosition = CalculateChessPiecePosition(col, row, chessboardBounds);
                        chessPiece.transform.position = chessPiecePosition;

                    }

                    else if (row == 0 && (col == 4))
                    {
                        GameObject chessPiece = Instantiate(whiteKingPrefab);

                        ChessPiece chessPieceProperties = chessPiece.GetComponent<ChessPiece>();
                        chessPieceProperties.row = row;
                        chessPieceProperties.col = col;
                        chessboard[col, row] = chessPiece;

                        Vector3 chessPiecePosition = CalculateChessPiecePosition(col, row, chessboardBounds);
                        chessPiece.transform.position = chessPiecePosition;

                    }

                    if (row == 6)
                    {
                        GameObject chessPiece = Instantiate(blackPawnPrefab);

                        ChessPiece chessPieceProperties = chessPiece.GetComponent<ChessPiece>();
                        chessPieceProperties.row = row;
                        chessPieceProperties.col = col;
                        chessboard[col, row] = chessPiece;

                        Vector3 chessPiecePosition = CalculateChessPiecePosition(col, row, chessboardBounds);
                        chessPiece.transform.position = chessPiecePosition;
     
                    }

                    else if (row == 7 && (col == 0 || col == 7))
                    {
                        GameObject chessPiece = Instantiate(blackRookPrefab);

                        ChessPiece chessPieceProperties = chessPiece.GetComponent<ChessPiece>();
                        chessPieceProperties.row = row;
                        chessPieceProperties.col = col;
                        chessboard[col, row] = chessPiece;

                        Vector3 chessPiecePosition = CalculateChessPiecePosition(col, row, chessboardBounds);
                        chessPiece.transform.position = chessPiecePosition;

                    }

                    else if (row == 7 && (col == 1 || col == 6))
                    {
                        GameObject chessPiece = Instantiate(blackKnightPrefab);


                        ChessPiece chessPieceProperties = chessPiece.GetComponent<ChessPiece>();
                        chessPieceProperties.row = row;
                        chessPieceProperties.col = col;
                        chessboard[col, row] = chessPiece;

                        Vector3 chessPiecePosition = CalculateChessPiecePosition(col, row, chessboardBounds);
                        chessPiece.transform.position = chessPiecePosition;

                    }

                    else if (row == 7 && (col == 2 || col == 5))
                    {
                        GameObject chessPiece = Instantiate(blackBishopPrefab);


                        ChessPiece chessPieceProperties = chessPiece.GetComponent<ChessPiece>();
                        chessPieceProperties.row = row;
                        chessPieceProperties.col = col;
                        chessboard[col, row] = chessPiece;

                        Vector3 chessPiecePosition = CalculateChessPiecePosition(col, row, chessboardBounds);
                        chessPiece.transform.position = chessPiecePosition;

                    }

                    else if (row == 7 && (col == 3))
                    {
                        GameObject chessPiece = Instantiate(blackQueenPrefab);


                        ChessPiece chessPieceProperties = chessPiece.GetComponent<ChessPiece>();
                        chessPieceProperties.row = row;
                        chessPieceProperties.col = col;
                        chessboard[col, row] = chessPiece;

                        Vector3 chessPiecePosition = CalculateChessPiecePosition(col, row, chessboardBounds);
                        chessPiece.transform.position = chessPiecePosition;

                    }

                    else if (row == 7 && (col == 4))
                    {
                        GameObject chessPiece = Instantiate(blackKingPrefab);


                        ChessPiece chessPieceProperties = chessPiece.GetComponent<ChessPiece>();
                        chessPieceProperties.row = row;
                        chessPieceProperties.col = col;
                        chessboard[col, row] = chessPiece;

                        Vector3 chessPiecePosition = CalculateChessPiecePosition(col, row, chessboardBounds);
                        chessPiece.transform.position = chessPiecePosition;

                    }


                }

                else if (myColor == ChessPieceColor.Black)
                {
                    if (row == 1)
                    {

                        GameObject chessPiece = Instantiate(whitePawnPrefab);

                        ChessPiece chessPieceProperties = chessPiece.GetComponent<ChessPiece>();
                        chessPieceProperties.row = row;
                        chessPieceProperties.col = col;
                        chessboard[col, row] = chessPiece;

                        Vector3 chessPiecePosition = CalculateChessPiecePosition(7 - col, 7 - row, chessboardBounds);
                        chessPiece.transform.position = chessPiecePosition;

                    }

                    else if (row == 0 && (col == 0 || col == 7))
                    {
                        GameObject chessPiece = Instantiate(whiteRookPrefab);


                        ChessPiece chessPieceProperties = chessPiece.GetComponent<ChessPiece>();
                        chessPieceProperties.row = row;
                        chessPieceProperties.col = col;
                        chessboard[col, row] = chessPiece;

                        Vector3 chessPiecePosition = CalculateChessPiecePosition(7 - col, 7 - row, chessboardBounds);
                        chessPiece.transform.position = chessPiecePosition;

                    }

                    else if (row == 0 && (col == 1 || col == 6))
                    {
                        GameObject chessPiece = Instantiate(whiteKnightPrefab);

                        ChessPiece chessPieceProperties = chessPiece.GetComponent<ChessPiece>();
                        chessPieceProperties.row = row;
                        chessPieceProperties.col = col;
                        chessboard[col, row] = chessPiece;

                        Vector3 chessPiecePosition = CalculateChessPiecePosition(7 - col, 7 - row, chessboardBounds);
                        chessPiece.transform.position = chessPiecePosition;

                    }

                    else if (row == 0 && (col == 2 || col == 5))
                    {
                        GameObject chessPiece = Instantiate(whiteBishopPrefab);


                        ChessPiece chessPieceProperties = chessPiece.GetComponent<ChessPiece>();
                        chessPieceProperties.row = row;
                        chessPieceProperties.col = col;
                        chessboard[col, row] = chessPiece;

                        Vector3 chessPiecePosition = CalculateChessPiecePosition(7 - col, 7 - row, chessboardBounds);
                        chessPiece.transform.position = chessPiecePosition;

                    }

                    else if (row == 0 && (col == 3))
                    {
                        GameObject chessPiece = Instantiate(whiteQueenPrefab);

                        ChessPiece chessPieceProperties = chessPiece.GetComponent<ChessPiece>();
                        chessPieceProperties.row = row;
                        chessPieceProperties.col = col;
                        chessboard[col, row] = chessPiece;

                        Vector3 chessPiecePosition = CalculateChessPiecePosition(7 - col, 7 - row, chessboardBounds);
                        chessPiece.transform.position = chessPiecePosition;

                    }

                    else if (row == 0 && (col == 4))
                    {
                        GameObject chessPiece = Instantiate(whiteKingPrefab);


                        ChessPiece chessPieceProperties = chessPiece.GetComponent<ChessPiece>();
                        chessPieceProperties.row = row;
                        chessPieceProperties.col = col;
                        chessboard[col, row] = chessPiece;

                        Vector3 chessPiecePosition = CalculateChessPiecePosition(7 - col, 7 - row, chessboardBounds);
                        chessPiece.transform.position = chessPiecePosition;

                    }


                    else if (row == 6)
                    {
                        GameObject chessPiece = Instantiate(blackPawnPrefab);

                        ChessPiece chessPieceProperties = chessPiece.GetComponent<ChessPiece>();
                        chessPieceProperties.row = row;
                        chessPieceProperties.col = col;
                        chessboard[col, row] = chessPiece;

                        Vector3 chessPiecePosition = CalculateChessPiecePosition(7 - col, 7 - row, chessboardBounds);
                        chessPiece.transform.position = chessPiecePosition;

                    }

                    else if (row == 7 && (col == 0 || col == 7))
                    {
                        GameObject chessPiece = Instantiate(blackRookPrefab);


                        ChessPiece chessPieceProperties = chessPiece.GetComponent<ChessPiece>();
                        chessPieceProperties.row = row;
                        chessPieceProperties.col = col;
                        chessboard[col, row] = chessPiece;

                        Vector3 chessPiecePosition = CalculateChessPiecePosition(7 - col, 7 - row, chessboardBounds);
                        chessPiece.transform.position = chessPiecePosition;

                    }

                    else if (row == 7 && (col == 1 || col == 6))
                    {
                        GameObject chessPiece = Instantiate(blackKnightPrefab);

                        ChessPiece chessPieceProperties = chessPiece.GetComponent<ChessPiece>();
                        chessPieceProperties.row = row;
                        chessPieceProperties.col = col;
                        chessboard[col, row] = chessPiece;

                        Vector3 chessPiecePosition = CalculateChessPiecePosition(7 - col, 7 - row, chessboardBounds);
                        chessPiece.transform.position = chessPiecePosition;

                    }

                    else if (row == 7 && (col == 2 || col == 5))
                    {
                        GameObject chessPiece = Instantiate(blackBishopPrefab);

                        ChessPiece chessPieceProperties = chessPiece.GetComponent<ChessPiece>();
                        chessPieceProperties.row = row;
                        chessPieceProperties.col = col;
                        chessboard[col, row] = chessPiece;

                        Vector3 chessPiecePosition = CalculateChessPiecePosition(7 - col, 7 - row, chessboardBounds);
                        chessPiece.transform.position = chessPiecePosition;

                    }

                    else if (row == 7 && (col == 3))
                    {
                        GameObject chessPiece = Instantiate(blackQueenPrefab);

                        ChessPiece chessPieceProperties = chessPiece.GetComponent<ChessPiece>();
                        chessPieceProperties.row = row;
                        chessPieceProperties.col = col;
                        chessboard[col, row] = chessPiece;

                        Vector3 chessPiecePosition = CalculateChessPiecePosition(7 - col, 7 - row, chessboardBounds);
                        chessPiece.transform.position = chessPiecePosition;

                    }

                    else if (row == 7 && (col == 4))
                    {
                        GameObject chessPiece = Instantiate(blackKingPrefab);


                        ChessPiece chessPieceProperties = chessPiece.GetComponent<ChessPiece>();
                        chessPieceProperties.row = row;
                        chessPieceProperties.col = col;
                        chessboard[col, row] = chessPiece;

                        Vector3 chessPiecePosition = CalculateChessPiecePosition(7 - col, 7 - row, chessboardBounds);
                        chessPiece.transform.position = chessPiecePosition;

                    }



                }

            }
        }


        
        Debug.Log(allChessPieces.Count);
        CalculateLegalMovesForAllPieces();



    }




    // Method to calculate legal moves for all pieces in the game
    public void CalculateLegalMovesForAllPieces()
    {
        whiteLegalMoves.Clear();
        blackLegalMoves.Clear();
        foreach (ChessPiece piece in allChessPieces)
        {
            piece.CalculateLegalMoves();
        }



        //castle check
    }

    public Bounds CalculateChessboardBounds()
    {
        // Assuming that the chessboard asset has a collider attached to it (e.g., a BoxCollider or MeshCollider)
        Collider2D chessboardCollider = chessBoard.GetComponent<Collider2D>();

        if (chessboardCollider != null)
        {
            // Get the bounds of the collider, which represent the size and position of the chessboard
            Bounds chessboardBounds = chessboardCollider.bounds;
            return chessboardBounds;
        }
        else
        {
            // If there's no collider attached, you can manually specify the bounds
            // Be sure to adjust these values according to your chessboard asset
            float width = 1.0f;  // Width of the chessboard in Unity units
            float height = 1.0f; // Height of the chessboard in Unity units
            float depth = 0.01f; // Depth or thickness of the chessboard in Unity units

            // Calculate the center position of the chessboard
            Vector3 center = transform.position;

            // Create bounds based on the specified size, center, and depth
            Bounds chessboardBounds = new Bounds(center, new Vector3(width, height, depth));
            Debug.Log("I fked up");
            return chessboardBounds;
        }
    }

    public Vector3 CalculateChessPiecePosition(int row, int col, Bounds chessboardBounds)
    {
        // Calculate the position of the chess piece based on row, col, and chessboardBounds

        // Calculate the size of one square on the chessboard based on the bounds
        float squareSizeX = chessboardBounds.size.x / 8;
        float squareSizeY = chessboardBounds.size.y / 8;

        // Calculate the position based on the row and column
        float x = chessboardBounds.min.x + squareSizeX * (row + 0.5f);
        float y = chessboardBounds.min.y + squareSizeY * (col + 0.5f);
        float z = chessboardBounds.center.z - 0.01f; // Adjust this value if pieces should be slightly above the board

        // Create the position vector
        Vector3 chessPiecePosition = new Vector3(x, y, z);

        return chessPiecePosition;
    }



    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && turnManager.currentPlayerColor == myColor)
        {
            if (turnManager.selectedChessPiece != null)
            {
                turnManager.ClickedSquare();
            }
        }


    }




}