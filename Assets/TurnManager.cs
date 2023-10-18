using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class TurnManager : NetworkBehaviour
{
    public NetworkVariable<bool> whiteTaken = new NetworkVariable<bool>();
    public NetworkVariable<bool> blackTaken = new NetworkVariable<bool>();


    public GameObject highlightPrefab;
    public Vector3 prefabPosition;
    public Vector3 prefabPositionBlack;

    public GameObject promotionUI;
    public GameObject promotionUIBlack;
    public GameObject takenByPromotionPiece;
    public GameObject takenByPromotionPieceStandin;


    public Vector2Int takenByPromotionPieceCoord;
    public Vector2Int targetCoord;

    public Vector3 originalPosition;
    
    public bool promotionInProgress = false;
    public bool promotionInProgressCheck = false;

    public int promotedPieceLocalID;


    public GameObject selectedChessPiece;
    public GameObject promotedPieceToBeSpawned;
    public GameObject promotingPiece;

    public GameObject whiteQueenPrefab;
    public GameObject whiteRookPrefab;
    public GameObject whiteBishopPrefab;
    public GameObject whiteKnightPrefab;

    public GameObject blackQueenPrefab;
    public GameObject blackRookPrefab;
    public GameObject blackBishopPrefab;
    public GameObject blackKnightPrefab;



    public ChessGameManager gameManager;
    // Player 1 starts
    public bool spawnedPieces = false;

    public GameObject test;



    public NetworkVariable<Vector2Int> piecePreviousPosition = new NetworkVariable<Vector2Int>();
    public NetworkVariable<Vector2Int> pieceNewPosition = new NetworkVariable<Vector2Int>();

    public NetworkVariable<bool> whiteExecuted = new NetworkVariable<bool>();
    public NetworkVariable<bool> blackExecuted = new NetworkVariable<bool>();

    public NetworkVariable<int> promotedPieceID = new NetworkVariable<int>();

    private GameObject SyncedChessPiece;
    private Vector2Int resetCheck = new Vector2Int(-999, -999);
    public ChessPieceColor currentPlayerColor;



    public bool testBool = false;

    public override void OnNetworkSpawn()
    {
        currentPlayerColor = ChessPieceColor.White;
        whiteTaken.Value = false;
        blackTaken.Value = false;
        whiteExecuted.Value = false;
        blackExecuted.Value = false;


        whiteTaken.OnValueChanged += OnWhiteChanged;
        blackTaken.OnValueChanged += OnBlackChanged;

        piecePreviousPosition.OnValueChanged += UpdatePreviousPosition;
        pieceNewPosition.OnValueChanged += UpdateNewPosition;

        whiteExecuted.OnValueChanged += OnWhiteExecuted;
        blackExecuted.OnValueChanged += OnBlackExecuted;

        promotedPieceID.OnValueChanged += Promote;


    }





    private IEnumerator WaitForSyncedChessPiece(Action action)
    {
        while (SyncedChessPiece == null)
        { yield return null; }

        action.Invoke();
    }



    private void Start()
    {
        Debug.Log("pieceNewPosition's value is " + pieceNewPosition.Value);

    }


    //Server synchronizes and updates the data on both game managers.
    public void UpdatePreviousPosition(Vector2Int previous, Vector2Int current)
    {
        Debug.Log("Previous Position Updated!");
        testBool = true;
        Debug.Log(current.x + " " +current.y + " " + gameManager.chessboard[current.x, current.y]);
        SyncedChessPiece = gameManager.chessboard[current.x, current.y];
        Debug.Log(SyncedChessPiece.name);

        gameManager.chessboard[current.x, current.y] = null;
        Debug.Log("pieceNewPosition's value is " + pieceNewPosition.Value);

    }

    public void UpdateNewPosition(Vector2Int previous, Vector2Int current)
    {
        //if one piece goes to x, then enemy piece captures that piece at x
        if (pieceNewPosition.Value != resetCheck)
        {
            Debug.Log("UpdateNewPosition started!");
            StartCoroutine(WaitForSyncedChessPiece(() =>
            {

                ChessPiece chessPiece = SyncedChessPiece.GetComponent<ChessPiece>();

                Debug.Log("HIII");
                Debug.Log(pieceNewPosition.Value);
                Debug.Log(SyncedChessPiece.name);

                //check for promotion


                if (gameManager.chessboard[pieceNewPosition.Value.x, pieceNewPosition.Value.y] != null)
                {
                    Debug.Log("Attempting to take piece...");

                    //remove from list
                    GameObject takenPiece = gameManager.chessboard[pieceNewPosition.Value.x, pieceNewPosition.Value.y];
                    ChessPiece takenPieceProperties = takenPiece.GetComponent<ChessPiece>();
                    gameManager.allChessPieces.Remove(takenPieceProperties);


                    Destroy(gameManager.chessboard[pieceNewPosition.Value.x, pieceNewPosition.Value.y]);
                    gameManager.chessboard[pieceNewPosition.Value.x, pieceNewPosition.Value.y] = null;
                    Debug.Log("Piece Taken!");
                }

                //en passant logic
                if (chessPiece.type == ChessPieceType.Pawn)
                {
                    Debug.Log("En Passant Check!");

                    if (chessPiece.col - 1 > -1)
                    {
                        if (gameManager.chessboard[chessPiece.col - 1, chessPiece.row] != null)
                        {
                            Debug.Log("Possible piece for en passant!");
                            Debug.Log(gameManager.chessboard[chessPiece.col - 1, chessPiece.row].name);
                            GameObject enPassantedPiece = gameManager.chessboard[chessPiece.col - 1, chessPiece.row];
                            ChessPiece epProperties = enPassantedPiece.GetComponent<ChessPiece>();

                            if (epProperties.pawnDoubleMoved == true && epProperties.movedTurnCount == 1)
                            {
                                gameManager.allChessPieces.Remove(epProperties);

                                Destroy(gameManager.chessboard[chessPiece.col - 1, chessPiece.row]);
                                gameManager.chessboard[chessPiece.col - 1, chessPiece.row] = null;
                                Debug.Log("En Passanted!");
                            }

                        }
                    }

                    if (chessPiece.col + 1 < 8)
                    {
                        if (gameManager.chessboard[chessPiece.col + 1, chessPiece.row] != null)
                        {
                            Debug.Log("Possible piece for en passant!");
                            Debug.Log(gameManager.chessboard[chessPiece.col + 1, chessPiece.row].name);
                            GameObject enPassantedPiece = gameManager.chessboard[chessPiece.col + 1, chessPiece.row];
                            ChessPiece epProperties = enPassantedPiece.GetComponent<ChessPiece>();

                            if (epProperties.pawnDoubleMoved == true && epProperties.movedTurnCount == 1)
                            {
                                gameManager.allChessPieces.Remove(epProperties);

                                Destroy(gameManager.chessboard[chessPiece.col + 1, chessPiece.row]);
                                gameManager.chessboard[chessPiece.col + 1, chessPiece.row] = null;
                                Debug.Log("En Passanted!");
                            }

                        }
                    }
                    
                }

                //castling logic
                if (chessPiece.type == ChessPieceType.King)
                {
                        //moving left
                        if ((piecePreviousPosition.Value.x - pieceNewPosition.Value.x) == 2)
                        {
                            for (int targetCol = piecePreviousPosition.Value.x - 1; targetCol > -1; targetCol--)
                            {
                                if (gameManager.chessboard[targetCol, piecePreviousPosition.Value.y] != null)
                                    {
                                        //check if object is rook
                                        GameObject rook = gameManager.chessboard[targetCol, piecePreviousPosition.Value.y];
                                        ChessPiece rookProperties = rook.GetComponent<ChessPiece>();
                                        //check if rook has moved
                                    if (rookProperties.hasMoved == false)
                                    {
                                            //ADD CODE TO TELEPORT ROOK TO KING's SIDE
                                       rookProperties.col = pieceNewPosition.Value.x + 1;

                                       if (gameManager.myColor == ChessPieceColor.White)
                                            {
                                                rook.transform.position = gameManager.CalculateChessPiecePosition(pieceNewPosition.Value.x + 1, pieceNewPosition.Value.y, gameManager.chessboardBounds);
                                            }

                                            else if (gameManager.myColor == ChessPieceColor.Black)
                                            {
                                                rook.transform.position = gameManager.CalculateChessPiecePosition(7-(pieceNewPosition.Value.x + 1), 7-(pieceNewPosition.Value.y), gameManager.chessboardBounds);

                                            }


                                        gameManager.chessboard[targetCol, piecePreviousPosition.Value.y] = null;
                                        gameManager.chessboard[pieceNewPosition.Value.x + 1, pieceNewPosition.Value.y] = rook;
                                        break;
                                    }

                                    }
                            }
                        }

                        //moving right
                        else if ((piecePreviousPosition.Value.x - pieceNewPosition.Value.x) == -2)
                        {
                            for (int targetCol = piecePreviousPosition.Value.x + 1; targetCol < 8; targetCol++)
                            {
                                if (gameManager.chessboard[targetCol, piecePreviousPosition.Value.y] != null)
                                {
                                    //check if object is rook
                                    GameObject rook = gameManager.chessboard[targetCol, piecePreviousPosition.Value.y];
                                    ChessPiece rookProperties = rook.GetComponent<ChessPiece>();
                                    //check if rook has moved
                                    if (rookProperties.hasMoved == false)
                                    {
                                        //ADD CODE TO TELEPORT ROOK TO KING's SIDE
                                        rookProperties.col = pieceNewPosition.Value.x - 1;

                                        if (gameManager.myColor == ChessPieceColor.White)
                                        {
                                            rook.transform.position = gameManager.CalculateChessPiecePosition(pieceNewPosition.Value.x - 1, pieceNewPosition.Value.y, gameManager.chessboardBounds);
                                        }

                                        else if (gameManager.myColor == ChessPieceColor.Black)
                                        {
                                            rook.transform.position = gameManager.CalculateChessPiecePosition(7 - (pieceNewPosition.Value.x - 1), 7 - pieceNewPosition.Value.y, gameManager.chessboardBounds);
                                        }

                                        
                                        gameManager.chessboard[targetCol, piecePreviousPosition.Value.y] = null;
                                        gameManager.chessboard[pieceNewPosition.Value.x - 1, pieceNewPosition.Value.y] = rook;
                                        break;
                                    }

                                }
                            }
                        }
                    


                    //add black code

                    //is castling
                    //teleports rook

                }


                if (gameManager.myColor == ChessPieceColor.White)
                {
                    Debug.Log("Enemy Chess Piece Moved!");

                    Debug.Log(pieceNewPosition.Value);
                    
                    chessPiece.col = pieceNewPosition.Value.x;
                    chessPiece.row = pieceNewPosition.Value.y;

                    SyncedChessPiece.transform.position = gameManager.CalculateChessPiecePosition(pieceNewPosition.Value.x, pieceNewPosition.Value.y, gameManager.chessboardBounds);


                }

                if (gameManager.myColor == ChessPieceColor.Black)
                {
                    Debug.Log("Enemy Chess Piece Moved!");
                    Debug.Log(pieceNewPosition.Value.x);
                    Debug.Log(pieceNewPosition.Value.y);

                    chessPiece.col = pieceNewPosition.Value.x;
                    chessPiece.row = pieceNewPosition.Value.y;

                    SyncedChessPiece.transform.position = gameManager.CalculateChessPiecePosition(7 - pieceNewPosition.Value.x, 7 - pieceNewPosition.Value.y, gameManager.chessboardBounds);
                }

                //en passant update
                if (chessPiece.type == ChessPieceType.Pawn && Mathf.Abs(piecePreviousPosition.Value.y - pieceNewPosition.Value.y) == 2)
                {
                    chessPiece.pawnDoubleMoved = true;
                }


                chessPiece.hasMoved = true;

                //en passant
                foreach (ChessPiece x in gameManager.allChessPieces)
                {
                    if (x.hasMoved == true)
                    {
                        x.movedTurnCount++;
                    }
                }



                gameManager.chessboard[pieceNewPosition.Value.x, pieceNewPosition.Value.y] = SyncedChessPiece;
                Debug.Log(pieceNewPosition.Value + " is now " + SyncedChessPiece.name);

               

               

                chessPiece.isSelected = false;

                foreach (Transform child in transform)
                {
                    Destroy(child.gameObject);
                }
                


                Debug.Log("Piece deselected!");


                //this part of the code is not running, because promotionInProgressCheck is false for the other side.
                if (promotionInProgressCheck == true)
                {
                    Debug.Log("Promotion in progress...");


                    GameObject promotingPieceToBeDestroyed = gameManager.chessboard[pieceNewPosition.Value.x, pieceNewPosition.Value.y];

                    Debug.Log(promotingPieceToBeDestroyed.transform.position);

                    ChessPiece pptbdChessPiece = promotingPieceToBeDestroyed.GetComponent<ChessPiece>();

                    //remove from list
                    gameManager.allChessPieces.Remove(pptbdChessPiece);




                    switch (promotedPieceID.Value)
                    {
                            case 1:
                                promotedPieceToBeSpawned = whiteQueenPrefab;
                                break;

                            case 2:
                                promotedPieceToBeSpawned = whiteRookPrefab;
                                break;

                            case 3:
                                promotedPieceToBeSpawned = whiteBishopPrefab;
                                break;

                            case 4:
                                promotedPieceToBeSpawned = whiteKnightPrefab;
                                break;

                            case 5:
                                promotedPieceToBeSpawned = blackQueenPrefab;
                                break;

                            case 6:
                                promotedPieceToBeSpawned = blackRookPrefab;
                                break;

                            case 7:
                                promotedPieceToBeSpawned = blackBishopPrefab;
                                break;

                            case 8:
                                promotedPieceToBeSpawned = blackKnightPrefab;
                                break;
                    }


                    Debug.Log(promotingPieceToBeDestroyed.transform.position);

                    GameObject spawnedPiece = Instantiate(promotedPieceToBeSpawned, promotingPieceToBeDestroyed.transform.position, Quaternion.identity);
                    Debug.Log(promotingPieceToBeDestroyed.name);

                    Debug.Log(promotingPieceToBeDestroyed.transform.position);

                    Destroy(promotingPieceToBeDestroyed);

                    gameManager.chessboard[pieceNewPosition.Value.x, pieceNewPosition.Value.y] = spawnedPiece;
                    
                    
                    //add new piece to list
                    ChessPiece spawnedPieceProperties = spawnedPiece.GetComponent<ChessPiece>();

                    spawnedPieceProperties.col = pieceNewPosition.Value.x;
                    spawnedPieceProperties.row = pieceNewPosition.Value.y;


                    

                    




                    //reset all flags
                    takenByPromotionPiece = null;
                    takenByPromotionPieceCoord = new Vector2Int (0,0);
                    promotedPieceToBeSpawned = null;
                    promotionInProgressCheck = false;
                    promotionInProgress = false;
                    

                }


                //problem is around here.

                gameManager.CalculateLegalMovesForAllPieces();

                //castle check logic
                foreach (ChessPiece targetPiece in gameManager.allChessPieces)
                {
                    if (targetPiece.type == ChessPieceType.King)
                    {
                        if (targetPiece.hasMoved == false)
                        {
                            //black logic
                            if (gameManager.myColor == ChessPieceColor.Black && targetPiece.color == ChessPieceColor.Black)
                            {
                                Vector2Int currentSquare = new Vector2Int(targetPiece.col, targetPiece.row);
                                if (!gameManager.whiteLegalMoves.Contains(currentSquare))
                                {
                                    Debug.Log(currentSquare);
                                    //check right
                                    for (int targetCol = targetPiece.col - 1; targetCol > -1; targetCol--)
                                    {
                                        Vector2Int targetSquare = new Vector2Int(targetCol, targetPiece.row);

                                        if (gameManager.whiteLegalMoves.Contains(targetSquare))
                                        {
                                            Debug.Log(targetSquare);
                                            break;
                                        }

                                        else if (!gameManager.whiteLegalMoves.Contains(targetSquare))
                                        {
                                            if (gameManager.chessboard[targetCol, targetPiece.row] != null)
                                            {
                                                //check if object is rook
                                                GameObject piece = gameManager.chessboard[targetCol, targetPiece.row];
                                                ChessPiece pieceProperties = piece.GetComponent<ChessPiece>();
                                                //check if rook has moved

                                                if (pieceProperties.color == ChessPieceColor.Black && pieceProperties.type == ChessPieceType.Rook && pieceProperties.hasMoved == false)
                                                {
                                                    //CASTLE
                                                    targetPiece.legalMoves.Add(new Vector2Int(targetPiece.col - 2, targetPiece.row));

                                                    //ADD CODE TO TELEPORT ROOK TO KING's SIDE
                                                    break;
                                                }

                                                else
                                                {
                                                    break;
                                                }

                                            }



                                        }

                                    }
                                    //check left
                                    for (int targetCol = targetPiece.col + 1; targetCol < 8; targetCol++)
                                    {
                                        Vector2Int targetSquare = new Vector2Int(targetCol, targetPiece.row);

                                        if (gameManager.whiteLegalMoves.Contains(targetSquare))
                                        {
                                            break;
                                        }

                                        else if (!gameManager.whiteLegalMoves.Contains(targetSquare))
                                        {
                                            if (gameManager.chessboard[targetCol, targetPiece.row] != null)
                                            {
                                                //check if object is rook
                                                GameObject piece = gameManager.chessboard[targetCol, targetPiece.row];
                                                ChessPiece pieceProperties = piece.GetComponent<ChessPiece>();
                                                //check if rook has moved
                                                if (pieceProperties.color == ChessPieceColor.Black && pieceProperties.type == ChessPieceType.Rook && pieceProperties.hasMoved == false)
                                                {
                                                    //CASTLE
                                                    targetPiece.legalMoves.Add(new Vector2Int(targetPiece.col + 2, targetPiece.row));
                                                    //ADD CODE TO TELEPORT ROOK TO KING's SIDE
                                                    break;
                                                }

                                                else
                                                {
                                                    break;
                                                }

                                            }
                                        }
                                    }

                                }

                                else if (gameManager.whiteLegalMoves.Contains(currentSquare))
                                {
                                    Debug.Log("You're in check!");
                                }
                            }

                            //white logic
                            if (gameManager.myColor == ChessPieceColor.White && targetPiece.color == ChessPieceColor.White)
                            {
                                Vector2Int currentSquare = new Vector2Int(targetPiece.col, targetPiece.row);
                                if (!gameManager.blackLegalMoves.Contains(currentSquare))
                                {
                                    Debug.Log(currentSquare);
                                    //check right
                                    for (int targetCol = targetPiece.col - 1; targetCol > -1; targetCol--)
                                    {
                                        Vector2Int targetSquare = new Vector2Int(targetCol, targetPiece.row);

                                        if (gameManager.blackLegalMoves.Contains(targetSquare))
                                        {
                                            Debug.Log(targetSquare);
                                            break;
                                        }

                                        else if (!gameManager.blackLegalMoves.Contains(targetSquare))
                                        {
                                            if (gameManager.chessboard[targetCol, targetPiece.row] != null)
                                            {
                                                //check if object is rook
                                                GameObject piece = gameManager.chessboard[targetCol, targetPiece.row];
                                                ChessPiece pieceProperties = piece.GetComponent<ChessPiece>();
                                                //check if rook has moved

                                                if (pieceProperties.color == ChessPieceColor.White && pieceProperties.type == ChessPieceType.Rook && pieceProperties.hasMoved == false)
                                                {
                                                    //CASTLE
                                                    targetPiece.legalMoves.Add(new Vector2Int(targetPiece.col - 2, targetPiece.row));

                                                    //ADD CODE TO TELEPORT ROOK TO KING's SIDE
                                                    break;
                                                }

                                                else
                                                {
                                                    break;
                                                }

                                            }



                                        }

                                    }
                                    //check left
                                    for (int targetCol = targetPiece.col + 1; targetCol < 8; targetCol++)
                                    {
                                        Vector2Int targetSquare = new Vector2Int(targetCol, targetPiece.row);

                                        if (gameManager.blackLegalMoves.Contains(targetSquare))
                                        {
                                            break;
                                        }

                                        else if (!gameManager.blackLegalMoves.Contains(targetSquare))
                                        {
                                            if (gameManager.chessboard[targetCol, targetPiece.row] != null)
                                            {
                                                //check if object is rook
                                                GameObject piece = gameManager.chessboard[targetCol, targetPiece.row];
                                                ChessPiece pieceProperties = piece.GetComponent<ChessPiece>();
                                                //check if rook has moved
                                                if (pieceProperties.color == ChessPieceColor.White && pieceProperties.type == ChessPieceType.Rook && pieceProperties.hasMoved == false)
                                                {
                                                    //CASTLE
                                                    targetPiece.legalMoves.Add(new Vector2Int(targetPiece.col + 2, targetPiece.row));
                                                    //ADD CODE TO TELEPORT ROOK TO KING's SIDE
                                                    break;
                                                }

                                                else
                                                {
                                                    break;
                                                }

                                            }
                                        }
                                    }

                                }

                                else if (gameManager.blackLegalMoves.Contains(currentSquare))
                                {
                                    Debug.Log("You're in check!");
                                }
                            }

                        }
                    }
                }


                //end of castling logic




                selectedChessPiece = null;
                SyncedChessPiece = null;
                Debug.Log("Switching Turns!");
                SwitchTurn();


                if (gameManager.myColor == ChessPieceColor.White)
                {
                    toggleWhiteExecutedServerRpc();
                }

                if (gameManager.myColor == ChessPieceColor.Black)
                {
                    toggleBlackExecutedServerRpc();
                }


            }
            ));

        }


    }



    [ServerRpc(RequireOwnership = false)]
    void UpdateChessPiecePositionPreviousServerRpc(Vector2Int position)
    {
        Debug.Log("Changing Piece Previous Position...");
       
        piecePreviousPosition.Value = new Vector2Int(position.x, position.y);
        Debug.Log(piecePreviousPosition.Value);

    }

    [ServerRpc(RequireOwnership = false)]
    void UpdateChessPiecePositionNewServerRpc(Vector2Int clickedPosition)
    {
        Debug.Log("Changing Piece New Position...");

        pieceNewPosition.Value = new Vector2Int(clickedPosition.x, clickedPosition.y);
        Debug.Log(pieceNewPosition.Value);

    }

    [ServerRpc(RequireOwnership = false)]
    void choosePromotionPieceIDServerRpc(int IDNumber)
    {
        Debug.Log("changing piece ID value");
        Debug.Log(promotedPieceID.Value);
        Debug.Log(IDNumber);
        promotedPieceID.Value = IDNumber;
        Debug.Log(promotedPieceID.Value);
    }






    public void SwitchTurn()
    {
        currentPlayerColor = (currentPlayerColor == ChessPieceColor.White) ? ChessPieceColor.Black : ChessPieceColor.White;
    }




    private void OnWhiteChanged(bool previous, bool current)
    {
        Debug.Log("White Taken Value is now " + whiteTaken.Value);
        if (blackTaken.Value == true)
        { 
            gameManager.SpawnChessPieces();
        }
    }

    private void OnBlackChanged(bool previous, bool current)
    {
        Debug.Log("Black Taken Value is now " + blackTaken.Value);
        if (whiteTaken.Value == true)
        {
            gameManager.SpawnChessPieces();
        }
    }


    [ServerRpc(RequireOwnership = false)]
    void changeWhiteTakenValueServerRpc()
    {
        Debug.Log("WhiteValue Changed");
        whiteTaken.Value = !whiteTaken.Value;
        Debug.Log("whiteTaken's value is " + whiteTaken.Value);
    }

    [ServerRpc(RequireOwnership = false)]
    void changeBlackTakenValueServerRpc()
    {
        blackTaken.Value = !blackTaken.Value;
    }

    [ServerRpc(RequireOwnership = false)]
    void toggleWhiteExecutedServerRpc()
    {
        whiteExecuted.Value = !whiteExecuted.Value;
    }

    [ServerRpc(RequireOwnership = false)]
    void toggleBlackExecutedServerRpc()
    {
        blackExecuted.Value = !blackExecuted.Value;
    }





    private void OnWhiteExecuted(bool previous, bool current)
    {
        if (whiteExecuted.Value == true && blackExecuted.Value == true)
        {
            UpdateChessPiecePositionNewServerRpc(resetCheck);
            choosePromotionPieceIDServerRpc(9);
            toggleWhiteExecutedServerRpc();
            toggleBlackExecutedServerRpc();
        }
    }

    private void OnBlackExecuted(bool previous, bool current)
    {
        if (whiteExecuted.Value == true && blackExecuted.Value == true)
        {
            UpdateChessPiecePositionNewServerRpc(resetCheck);
            choosePromotionPieceIDServerRpc(9);
            toggleWhiteExecutedServerRpc();
            toggleBlackExecutedServerRpc();
        }
    }


    public void AssignWhite()
    {
        if (whiteTaken.Value == false && spawnedPieces == false)
        {
            gameManager.myColor = ChessPieceColor.White;
            Debug.Log(gameManager.myColor);
            spawnedPieces = true;
            Debug.Log("Pieces are Spawned is " + spawnedPieces);
            changeWhiteTakenValueServerRpc();
        }

        if (whiteTaken.Value == true)
        { Debug.Log("White is taken!"); }

        if (spawnedPieces == true)
        {
            Debug.Log("You have already chosen your color!");
        }
    }


    public void AssignBlack()
    {
        if (blackTaken.Value == false && spawnedPieces == false)
        {
            gameManager.myColor = ChessPieceColor.Black;
            Debug.Log(gameManager.myColor);
            spawnedPieces = true;
            Debug.Log("blackTaken is " + blackTaken.Value);
            changeBlackTakenValueServerRpc();
        }

        if (blackTaken.Value == true)
        { Debug.Log("Black is taken!"); }

        if (spawnedPieces == true)
        {
            Debug.Log("You have already chosen your color!");
        }

        Debug.Log("blackTaken is " + blackTaken.Value);

    }

    public void ClickedSquare()
    {

        ChessPiece chessPiece = selectedChessPiece.GetComponent<ChessPiece>();
        Debug.Log(selectedChessPiece);

        

        foreach (Vector2Int lm in chessPiece.legalMoves)
        {
            Debug.Log(lm);
        } 
        

        

        //Deselects all other pieces except for the current selected one
        foreach (ChessPiece otherPieces in gameManager.allChessPieces)
        {
            if (otherPieces != chessPiece && otherPieces.isSelected == true)
            {
                otherPieces.isSelected = false;
                Debug.Log(otherPieces.gameObject.name + " is deselected!");
                foreach (Transform child in transform)
                {
                    Destroy(child.gameObject);
                }
                Debug.Log(this.gameObject.name + "is deselected!");
            }
        }

       

        // Check if this chess piece is selected & if its the correct turn
        if (chessPiece != null && chessPiece.isSelected)
        {
            Debug.Log("Calculating moves...");
            // Handle movement logic here, including validation
            if (Input.GetMouseButtonDown(0))
            {
                // Convert mouse screen position to world position
                Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                // Map world position to chessboard square
                chessPiece.clickedSquare = chessPiece.WorldToChessboardSquare(mousePosition);

                // Now 'clickedSquare' contains the grid coordinates of the clicked square
                Debug.Log("Clicked on square: " + chessPiece.clickedSquare);

                Debug.Log(gameManager.chessboard[chessPiece.clickedSquare.x, chessPiece.clickedSquare.y]);


                foreach (Vector2Int lm in chessPiece.legalMoves)
                    {
                        Debug.Log(lm.ToString() + "is a legal move");
                        prefabPosition = gameManager.CalculateChessPiecePosition(lm.x, lm.y, gameManager.chessboardBounds);
                        prefabPositionBlack = gameManager.CalculateChessPiecePosition(7 - lm.x, 7 - lm.y, gameManager.chessboardBounds);

                        if (gameManager.myColor == ChessPieceColor.White)
                        {
                            Instantiate(highlightPrefab, prefabPosition, Quaternion.identity, this.gameObject.transform);
                        }

                        if (gameManager.myColor == ChessPieceColor.Black)
                        {
                            Instantiate(highlightPrefab, prefabPositionBlack, Quaternion.identity, this.gameObject.transform);
                        }

                    }






                //Updates position of GameObject
                if (chessPiece.legalMoves.Contains(chessPiece.clickedSquare) && chessPiece.color == ChessPieceColor.White)
                {
                    //check for promotion
                    if (chessPiece.type == ChessPieceType.Pawn && chessPiece.clickedSquare.y == 7)
                    {
                        
                        originalPosition = selectedChessPiece.transform.position;
                        targetCoord = chessPiece.clickedSquare;
                        selectedChessPiece.transform.position = gameManager.CalculateChessPiecePosition(chessPiece.clickedSquare.x, chessPiece.clickedSquare.y, gameManager.chessboardBounds);

                        if (gameManager.chessboard[chessPiece.clickedSquare.x, chessPiece.clickedSquare.y] != null)
                        {
                            takenByPromotionPiece = gameManager.chessboard[chessPiece.clickedSquare.x, chessPiece.clickedSquare.y];
                            takenByPromotionPieceCoord = new Vector2Int(chessPiece.clickedSquare.x, chessPiece.clickedSquare.y);
                            takenByPromotionPiece.SetActive(false);
                        }

                        //promotion logic happens
                        promotionUI.SetActive(true);

                        promotingPiece = selectedChessPiece;

                        //UI element position
                        promotionUI.transform.position = new Vector3(selectedChessPiece.transform.position.x, (selectedChessPiece.transform.position.y - (gameManager.chessboardBounds.size.y / 16) * 3), promotionUI.transform.position.z);

                        promotionInProgress = true;



                        return;

                    }

                    Debug.Log("Move is legal!");

                    Debug.Log("Preparing Server Rpc...");

                    //Update Chessboard Array (NEEDS RPC FUNCTION HERE)
                    UpdateChessPiecePositionPreviousServerRpc(new Vector2Int(chessPiece.col, chessPiece.row));
                    UpdateChessPiecePositionNewServerRpc(chessPiece.clickedSquare);

                    //if taking a piece (NEEDS RPC FUNCTION HERE)


                    Debug.Log("Server Rpc executed.");
                }

                //Watch the black moves for bug; this part can get gnarly
                else if (chessPiece.legalMoves.Contains(chessPiece.clickedSquare) && chessPiece.color == ChessPieceColor.Black)
                {
                    //check for promotion
                    if (chessPiece.type == ChessPieceType.Pawn && chessPiece.clickedSquare.y == 0)
                    {
                        originalPosition = selectedChessPiece.transform.position;
                        targetCoord = chessPiece.clickedSquare;
                        selectedChessPiece.transform.position = gameManager.CalculateChessPiecePosition(7 - chessPiece.clickedSquare.x, 7 - chessPiece.clickedSquare.y, gameManager.chessboardBounds);

                        if (gameManager.chessboard[chessPiece.clickedSquare.x, chessPiece.clickedSquare.y] != null)
                        {
                            takenByPromotionPiece = gameManager.chessboard[chessPiece.clickedSquare.x, chessPiece.clickedSquare.y];
                            takenByPromotionPieceCoord = new Vector2Int(chessPiece.clickedSquare.x, chessPiece.clickedSquare.y);
                            takenByPromotionPiece.SetActive(false);
                        }

                        //promotion logic happens
                        promotionUIBlack.SetActive(true);

                        promotingPiece = selectedChessPiece;

                        //UI element position
                        promotionUIBlack.transform.position = new Vector3(selectedChessPiece.transform.position.x, (selectedChessPiece.transform.position.y - (gameManager.chessboardBounds.size.y / 16) * 3), promotionUIBlack.transform.position.z);

                        promotionInProgress = true;
                        return;

                    }

                    Debug.Log("Move is legal!");

                    Debug.Log("Preparing Server Rpc...");

                    //Update Chessboard Array (NEEDS RPC FUNCTION HERE)
                    UpdateChessPiecePositionPreviousServerRpc(new Vector2Int(chessPiece.col, chessPiece.row));
                    UpdateChessPiecePositionNewServerRpc(chessPiece.clickedSquare);

                    //if taking a piece (NEEDS RPC FUNCTION HERE)

                    Debug.Log("Server Rpc executed.");


                }

                else if (!chessPiece.legalMoves.Contains(chessPiece.clickedSquare) && chessPiece.clickedSquare != new Vector2Int(chessPiece.col, chessPiece.row))
                {



                    chessPiece.isSelected = false;
                    Debug.Log("Piece deselected!");
                    selectedChessPiece = null;

                    foreach (Transform child in transform)
                    {
                        Destroy(child.gameObject);
                    }

                }

                else
                {
                    Debug.Log("Selecting Piece");
                }

                if (promotionInProgress == true)
                {
                    if (takenByPromotionPiece != null)
                    {
                        takenByPromotionPieceStandin = takenByPromotionPiece;
                        takenByPromotionPiece.SetActive(true);
                        takenByPromotionPiece = null;
                    }

                    promotingPiece.transform.position = originalPosition;

                    promotionUI.SetActive(false);
                    promotionUIBlack.SetActive(false);

                    promotionInProgress = false;
                }


            }
            // Update the position of the GameObject based on the selected move
            // Deselect the piece when the move is completed
        }
    }


    public void Promote(int previous, int current)
    { 

        if (current != 9)
        {
            Debug.Log("The method Promote has been executed.");
            promotionInProgressCheck = true;

            //information about what the piece is promoted to needs to be relayed to the other side
            if (currentPlayerColor == gameManager.myColor)
            {
                promotionUI.SetActive(false);
                promotionUIBlack.SetActive(false);
               

                ChessPiece chessPiece = promotingPiece.GetComponent<ChessPiece>();

                if (takenByPromotionPiece != null)
                {

                    UpdateChessPiecePositionPreviousServerRpc(new Vector2Int(chessPiece.col, chessPiece.row));
                    Debug.Log(takenByPromotionPieceCoord);
                    UpdateChessPiecePositionNewServerRpc(takenByPromotionPieceCoord);
                }

                else
                {
                    UpdateChessPiecePositionPreviousServerRpc(new Vector2Int(chessPiece.col, chessPiece.row));
                    UpdateChessPiecePositionNewServerRpc(targetCoord);
                }
                
            }
        }

        if (current == 9)
        {
            return;
        }


    }

    public void PromotingPiece(int promotionID)
    {
        Debug.Log("The method PromotingPiece has been executed.");

        promotedPieceLocalID = promotionID;
        Debug.Log(promotedPieceLocalID);

        choosePromotionPieceIDServerRpc(promotedPieceLocalID);
    }



}
