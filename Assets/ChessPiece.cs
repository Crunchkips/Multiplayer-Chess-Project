using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;


public class ChessPiece : MonoBehaviour
{
    public ChessPieceType type;
    public ChessPieceColor color;
    public int row = 0;
    public int col = 0;
    public bool hasMoved = false;
    public bool isSelected = false;
    public List<Vector2Int> legalMoves = new List<Vector2Int>();


    public bool pawnDoubleMoved = false;
    public int movedTurnCount = 0;
    

    public Vector2Int clickedSquare;


    public TurnManager turnManager;
    public ChessGameManager manager;
    // Other properties like position, color, etc.




    private void Awake()
    {
        manager = FindObjectOfType<ChessGameManager>();
        turnManager = FindObjectOfType<TurnManager>();
        

        manager.allChessPieces.Add(this);
    }

    //Tells server that piece has been moved away from previous square



    private void OnMouseDown()
    {
        if (manager.myColor == color)
        {
            ToggleSelection();
        }
    }

    public void ToggleSelection()
    {
        if (turnManager.promotionInProgress == false)
        {
            isSelected = !isSelected;
            if (isSelected)
            {
                // The piece is selected
                Debug.Log(this.gameObject.name + "is selected!");
                turnManager.selectedChessPiece = this.gameObject;
                Debug.Log(turnManager.selectedChessPiece.name);

            }
            else if (!isSelected)
            {
                // The piece is deselected
                turnManager.selectedChessPiece = null;
                foreach (Transform child in turnManager.transform)
                {
                    Destroy(child.gameObject);
                }
                Debug.Log(this.gameObject.name + "is deselected!");

            }
        }
        
    }






    //WIP
    public Vector2Int WorldToChessboardSquare(Vector2 worldPosition)
    {
        if (color == ChessPieceColor.White)
        {
            Vector2 relativePosition = new Vector2(
    worldPosition.x - manager.chessBoard.transform.position.x,
    worldPosition.y - manager.chessBoard.transform.position.y);


            float squareSize = manager.chessboardBounds.size.x / 8;

            // Calculate grid coordinates by dividing relative position by square size
            int x = Mathf.FloorToInt((relativePosition.x + 4.18f) / squareSize);
            int y = Mathf.FloorToInt((relativePosition.y + 4.18f) / squareSize);

            // Ensure the coordinates are within the chessboard bounds
            x = Mathf.Clamp(x, 0, 7);
            y = Mathf.Clamp(y, 0, 7);

            return new Vector2Int(x, y);
        }

        else if (color == ChessPieceColor.Black)
        {
            Vector2 relativePosition = new Vector2(
    worldPosition.x - manager.chessBoard.transform.position.x,
    worldPosition.y - manager.chessBoard.transform.position.y);


            float squareSize = manager.chessboardBounds.size.x / 8;

            // Calculate grid coordinates by dividing relative position by square size
            int x = Mathf.FloorToInt((relativePosition.x + 4.18f) / squareSize);
            int y = Mathf.FloorToInt((relativePosition.y + 4.18f) / squareSize);

            // Ensure the coordinates are within the chessboard bounds
            x = Mathf.Clamp(x, 0, 7);
            y = Mathf.Clamp(y, 0, 7);

            return new Vector2Int(7-x, 7-y);
        }

        else { return new Vector2Int(0,0); }
    }




    public void CalculateLegalMoves()
    {
        legalMoves.Clear();
        //ADD A CHECK/GUARD STATEMENT FOR IF MOVING PLACES THE KING IN CHECK!!!
        if (color == ChessPieceColor.White)
        {
            switch (type)
            {
                case ChessPieceType.Pawn:

                    int oneForward = row + 1;
                    int twoForward = row + 2;
                    int oneLeft = col - 1;
                    int oneRight = col + 1;

                    int ptargetRow;
                    int ptargetCol;

                    if (oneForward < 8) {
                        //one square move
                        if (manager.chessboard[col, oneForward] == null)
                        {
                            legalMoves.Add(new Vector2Int(col, oneForward));
                            manager.whiteLegalMoves.Add(new Vector2Int(col, oneForward));
                        }

                        //first move, two square move
                        if (hasMoved == false && manager.chessboard[col, twoForward] == null)
                        {
                            legalMoves.Add(new Vector2Int(col, twoForward));
                            manager.whiteLegalMoves.Add(new Vector2Int(col, twoForward));
                        }

                        //diagonal capture
                        if (oneRight < 8)
                        {
                            ptargetRow = oneForward;
                            ptargetCol = oneRight;

                            if (manager.chessboard[ptargetCol, ptargetRow] != null)
                            {
                                GameObject pawnPiece = manager.chessboard[ptargetCol, ptargetRow];
                                ChessPiece pawnPieceProperties = pawnPiece.GetComponent<ChessPiece>();

                                if (pawnPieceProperties.color == ChessPieceColor.Black)
                                {
                                    legalMoves.Add(new Vector2Int(oneRight, oneForward));
                                    manager.whiteLegalMoves.Add(new Vector2Int(oneRight, oneForward));
                                }
                            }

                            //en Passant
                            if (manager.chessboard[ptargetCol, row] != null)
                            {
                                GameObject enPassantPiece = manager.chessboard[ptargetCol, row];
                                ChessPiece enPassantPieceProperties = enPassantPiece.GetComponent<ChessPiece>();

                                if (enPassantPieceProperties.color == ChessPieceColor.Black && enPassantPieceProperties.pawnDoubleMoved == true && enPassantPieceProperties.movedTurnCount == 1)
                                {
                                    Debug.Log("enPassant is available on " + enPassantPiece.name);
                                    Debug.Log("enPassant's move turn count is " + enPassantPieceProperties.movedTurnCount);
                                    legalMoves.Add(new Vector2Int(oneRight, oneForward));
                                    manager.whiteLegalMoves.Add(new Vector2Int(oneRight, oneForward));
                                }


                            }
                        }

                        if (oneLeft > -1)
                        {
                            ptargetRow = oneForward;
                            ptargetCol = oneLeft;

                            if (manager.chessboard[ptargetCol, ptargetRow] != null)
                            {
                                GameObject pawnPiece = manager.chessboard[ptargetCol, ptargetRow];
                                ChessPiece pawnPieceProperties = pawnPiece.GetComponent<ChessPiece>();
                                if (pawnPieceProperties.color == ChessPieceColor.Black)
                                {
                                    legalMoves.Add(new Vector2Int(oneLeft, oneForward));
                                    manager.whiteLegalMoves.Add(new Vector2Int(oneLeft, oneForward));
                                }
                            }

                            //en passant
                            if (manager.chessboard[ptargetCol, row] != null)
                            {
                                GameObject enPassantPiece = manager.chessboard[ptargetCol, row];
                                ChessPiece enPassantPieceProperties = enPassantPiece.GetComponent<ChessPiece>();

                                if (enPassantPieceProperties.color == ChessPieceColor.Black && enPassantPieceProperties.pawnDoubleMoved == true && enPassantPieceProperties.movedTurnCount == 1)
                                {
                                    Debug.Log("enPassant is available on " + enPassantPiece.name);
                                    Debug.Log("enPassant's move turn count is " + enPassantPieceProperties.movedTurnCount);
                                    legalMoves.Add(new Vector2Int(oneLeft, oneForward));
                                    manager.whiteLegalMoves.Add(new Vector2Int(oneLeft, oneForward));
                                }


                            }
                        }
                    }
                    //add en passant



                    break;



                case ChessPieceType.Rook:

                    //check forward
                    for (int targetRow = row + 1; targetRow < 8; targetRow++)
                    {
                        
                        if (manager.chessboard[col, targetRow] == null)
                        {
                            legalMoves.Add(new Vector2Int(col, targetRow));
                            manager.whiteLegalMoves.Add(new Vector2Int(col, targetRow));
                        }

                        else if (manager.chessboard[col, targetRow] != null)
                        {
                            GameObject piece = manager.chessboard[col, targetRow];
                            ChessPiece pieceProperties = piece.GetComponent<ChessPiece>();

                            //check for allied piece
                            if (pieceProperties.color == ChessPieceColor.White)
                            {
                                break;
                            }

                            else if (pieceProperties.color == ChessPieceColor.Black)
                            {
                                legalMoves.Add(new Vector2Int(col, targetRow));
                                manager.whiteLegalMoves.Add(new Vector2Int(col, targetRow));
                                break;
                            }
                        }

                    }
                    //check backward
                    for (int targetRow = row - 1; targetRow > -1; targetRow--)
                    {

                        if (manager.chessboard[col, targetRow] == null)
                        {
                            legalMoves.Add(new Vector2Int(col, targetRow));
                            manager.whiteLegalMoves.Add(new Vector2Int(col, targetRow));
                        }

                        else if (manager.chessboard[col, targetRow] != null)
                        {
                            GameObject piece = manager.chessboard[col, targetRow];
                            ChessPiece pieceProperties = piece.GetComponent<ChessPiece>();
                            //check for allied piece
                            if (pieceProperties.color == ChessPieceColor.White)
                            {
                                break;
                            }

                            else if (pieceProperties.color == ChessPieceColor.Black)
                            {
                                legalMoves.Add(new Vector2Int(col, targetRow));
                                manager.whiteLegalMoves.Add(new Vector2Int(col, targetRow));
                                break;
                            }
                        }
                    }
                    //check left
                    for (int targetCol = col - 1; targetCol > -1; targetCol--)
                    {
                        if (manager.chessboard[targetCol, row] == null)
                        {
                            legalMoves.Add(new Vector2Int(targetCol, row));
                            manager.whiteLegalMoves.Add(new Vector2Int(targetCol, row));
                        }

                        else if (manager.chessboard[targetCol, row] != null)
                        {
                            GameObject piece = manager.chessboard[targetCol, row];
                            ChessPiece pieceProperties = piece.GetComponent<ChessPiece>();

                            //check for allied piece
                            if (pieceProperties.color == ChessPieceColor.White)
                            {
                                break;
                            }

                            else if (pieceProperties.color == ChessPieceColor.Black)
                            {
                                legalMoves.Add(new Vector2Int(targetCol, row));
                                manager.whiteLegalMoves.Add(new Vector2Int(targetCol, row));
                                break;
                            }
                        }
                    }
                    //check right
                    for (int targetCol = col + 1; targetCol < 8; targetCol++)
                    {
                        if (manager.chessboard[targetCol, row] == null)
                        {
                            legalMoves.Add(new Vector2Int(targetCol, row));
                            manager.whiteLegalMoves.Add(new Vector2Int(targetCol, row));
                        }

                        else if (manager.chessboard[targetCol, row] != null)
                        {
                            GameObject piece = manager.chessboard[targetCol, row];
                            ChessPiece pieceProperties = piece.GetComponent<ChessPiece>();

                            //check for allied piece
                            if (pieceProperties.color == ChessPieceColor.White)
                            {
                                break;
                            }

                            else if (pieceProperties.color == ChessPieceColor.Black)
                            {
                                legalMoves.Add(new Vector2Int(targetCol, row));
                                manager.whiteLegalMoves.Add(new Vector2Int(targetCol, row));
                                break;
                            }
                        }
                    }
                    break;

                case ChessPieceType.Knight:
                    int oneF = row + 1;
                    int twoF = row + 2;
                    int oneL = col - 1;
                    int oneR = col + 1;

                    int twoL = col - 2;
                    int twoR = col + 2;
                    int oneB = row - 1;
                    int twoB = row - 2;




                    //2F1L
                    if (twoF < 8 && oneL > -1)
                    {
                        if (manager.chessboard[oneL, twoF] == null)
                        {
                            legalMoves.Add(new Vector2Int(oneL, twoF));
                            manager.whiteLegalMoves.Add(new Vector2Int(oneL, twoF));
                        }

                        else if (manager.chessboard[oneL, twoF] != null)
                        {
                            GameObject knightPiece = manager.chessboard[oneL, twoF];
                            ChessPiece knightPieceProperties = knightPiece.GetComponent<ChessPiece>();
                            if (knightPieceProperties.color == ChessPieceColor.Black)
                            {
                                legalMoves.Add(new Vector2Int(oneL, twoF));
                                manager.whiteLegalMoves.Add(new Vector2Int(oneL, twoF));
                            }
                        }
                    }

                    //1F2L
                    if (oneF < 8 && twoL > -1)
                    {


                        if (manager.chessboard[twoL, oneF] == null)
                        {
                            legalMoves.Add(new Vector2Int(twoL, oneF));
                            manager.whiteLegalMoves.Add(new Vector2Int(twoL, oneF));
                        }

                        else if (manager.chessboard[twoL, oneF] != null)
                        {
                            GameObject knightPieceTwo = manager.chessboard[twoL, oneF];
                            ChessPiece knightPieceTwoProperties = knightPieceTwo.GetComponent<ChessPiece>();
                            if (knightPieceTwoProperties.color == ChessPieceColor.Black)
                            {
                                legalMoves.Add(new Vector2Int(twoL, oneF));
                                manager.whiteLegalMoves.Add(new Vector2Int(twoL, oneF));
                            }
                        }
                    }

                    //2F1R
                    if (twoF < 8 && oneR < 8)
                    {

                        if (manager.chessboard[oneR, twoF] == null)
                        {
                            legalMoves.Add(new Vector2Int(oneR, twoF));
                            manager.whiteLegalMoves.Add(new Vector2Int(oneR, twoF));
                        }

                        else if (manager.chessboard[oneR, twoF] != null)
                        {
                            GameObject knightPieceThree = manager.chessboard[oneR, twoF];
                            ChessPiece knightPieceThreeProperties = knightPieceThree.GetComponent<ChessPiece>();
                            if (knightPieceThreeProperties.color == ChessPieceColor.Black)
                            {
                                legalMoves.Add(new Vector2Int(oneR, twoF));
                                manager.whiteLegalMoves.Add(new Vector2Int(oneR, twoF));
                            }
                        }

                    }

                    //1F2R
                    if (oneF < 8 && twoR < 8)
                    {

                        if (manager.chessboard[twoR, oneF] == null)
                        {
                            legalMoves.Add(new Vector2Int(twoR, oneF));
                            manager.whiteLegalMoves.Add(new Vector2Int(twoR, oneF));
                        }

                        else if (manager.chessboard[twoR, oneF] != null)
                        {
                            GameObject knightPieceFour = manager.chessboard[twoR, oneF];
                            ChessPiece knightPieceFourProperties = knightPieceFour.GetComponent<ChessPiece>();
                            if (knightPieceFourProperties.color == ChessPieceColor.Black)
                            {
                                legalMoves.Add(new Vector2Int(twoR, oneF));
                                manager.whiteLegalMoves.Add(new Vector2Int(twoR, oneF));
                            }
                        }

                    }

                    //2B1L
                    if (twoB > -1 && oneL > -1)
                    {
                        if (manager.chessboard[oneL, twoB] == null)
                        {
                            legalMoves.Add(new Vector2Int(oneL, twoB));
                            manager.whiteLegalMoves.Add(new Vector2Int(oneL, twoB));
                        }

                        else if (manager.chessboard[oneL, twoB] != null)
                        {
                            GameObject knightPieceFive = manager.chessboard[oneL, twoB];
                            ChessPiece knightPieceFiveProperties = knightPieceFive.GetComponent<ChessPiece>();
                            if (knightPieceFiveProperties.color == ChessPieceColor.Black)
                            {
                                legalMoves.Add(new Vector2Int(oneL, twoB));
                                manager.whiteLegalMoves.Add(new Vector2Int(oneL, twoB));
                            }
                        }

                    }


                    //1B2L
                    if (oneB > -1 && twoL > -1)
                    {
                        if (manager.chessboard[twoL, oneB] == null)
                        {
                            legalMoves.Add(new Vector2Int(twoL, oneB));
                            manager.whiteLegalMoves.Add(new Vector2Int(twoL, oneB));
                        }

                        else if (manager.chessboard[twoL, oneB] != null)
                        {
                            GameObject knightPieceSix = manager.chessboard[twoL, oneB];
                            ChessPiece knightPieceSixProperties = knightPieceSix.GetComponent<ChessPiece>();
                            if (knightPieceSixProperties.color == ChessPieceColor.Black)
                            {
                                legalMoves.Add(new Vector2Int(twoL, oneB));
                                manager.whiteLegalMoves.Add(new Vector2Int(twoL, oneB));
                            }
                        }
                    }

                    //2B1R
                    if (twoB > -1 && oneR < 8)
                    {
                        if (manager.chessboard[oneR, twoB] == null)
                        {
                            legalMoves.Add(new Vector2Int(oneR, twoB));
                            manager.whiteLegalMoves.Add(new Vector2Int(oneR, twoB));
                        }


                        else if (manager.chessboard[oneR, twoB] != null)
                        {
                            GameObject knightPieceSeven = manager.chessboard[oneR, twoB];
                            ChessPiece knightPieceSevenProperties = knightPieceSeven.GetComponent<ChessPiece>();

                            if (knightPieceSevenProperties.color == ChessPieceColor.Black)
                            {
                                legalMoves.Add(new Vector2Int(oneR, twoB));
                                manager.whiteLegalMoves.Add(new Vector2Int(oneR, twoB));
                            }
                        }
                    }

                    //1B2R
                    if (oneB > -1 && twoR < 8)
                    {
                        if (manager.chessboard[twoR, oneB] == null)
                        {
                            legalMoves.Add(new Vector2Int(twoR, oneB));
                            manager.whiteLegalMoves.Add(new Vector2Int(twoR, oneB));
                        }
                        else if (manager.chessboard[twoR, oneB] != null)
                        {
                            GameObject knightPieceEight = manager.chessboard[twoR, oneB];
                            ChessPiece knightPieceEightProperties = knightPieceEight.GetComponent<ChessPiece>();
                            if (knightPieceEightProperties.color == ChessPieceColor.Black)
                            {
                                legalMoves.Add(new Vector2Int(twoR, oneB));
                                manager.whiteLegalMoves.Add(new Vector2Int(twoR, oneB));
                            }
                        }

                    }
                    break;


                case ChessPieceType.Bishop:
                    //BISHOPS TELEPORT FKING FIX THAT
                    int bTargetCol = col;
                    int bTargetRow = row;

                    //check left-up diagonal

                    bTargetCol = col - 1;
                    bTargetRow = row + 1;
                    while (bTargetCol > -1 && bTargetRow < 8)
                    {

                        if (manager.chessboard[bTargetCol, bTargetRow] == null)
                        {
                            legalMoves.Add(new Vector2Int(bTargetCol, bTargetRow));
                            manager.whiteLegalMoves.Add(new Vector2Int(bTargetCol, bTargetRow));
                            bTargetCol--;
                            bTargetRow++;
                        }

                        else if (manager.chessboard[bTargetCol, bTargetRow] != null)
                        {
                            GameObject piece = manager.chessboard[bTargetCol, bTargetRow];
                            ChessPiece pieceProperties = piece.GetComponent<ChessPiece>();

                            if (pieceProperties.color == ChessPieceColor.White)
                            {
                                break;
                            }
                            else if (pieceProperties.color == ChessPieceColor.Black)
                            {
                                legalMoves.Add(new Vector2Int(bTargetCol, bTargetRow));
                                manager.whiteLegalMoves.Add(new Vector2Int(bTargetCol, bTargetRow));
                                break;
                            }

                        }

                    }



                    //check right-up diagonal
                    bTargetCol = col + 1;
                    bTargetRow = row + 1;
                    while (bTargetCol < 8 && bTargetRow < 8)
                    {

                        if (manager.chessboard[bTargetCol, bTargetRow] == null)
                        {
                            legalMoves.Add(new Vector2Int(bTargetCol, bTargetRow));
                            manager.whiteLegalMoves.Add(new Vector2Int(bTargetCol, bTargetRow));
                            bTargetCol++;
                            bTargetRow++;
                        }

                        else if (manager.chessboard[bTargetCol, bTargetRow] != null)
                        {
                            GameObject piece = manager.chessboard[bTargetCol, bTargetRow];
                            ChessPiece pieceProperties = piece.GetComponent<ChessPiece>();

                            if (pieceProperties.color == ChessPieceColor.White)
                            {
                                break;
                            }
                            else if (pieceProperties.color == ChessPieceColor.Black)
                            {
                                legalMoves.Add(new Vector2Int(bTargetCol, bTargetRow));
                                manager.whiteLegalMoves.Add(new Vector2Int(bTargetCol, bTargetRow));
                                break;
                            }

                        }

                    }

                    //check left-down diagonal
                    bTargetCol = col - 1;
                    bTargetRow = row - 1;

                    while (bTargetCol > -1 && bTargetRow > -1)
                    {

                        if (manager.chessboard[bTargetCol, bTargetRow] == null)
                        {
                            legalMoves.Add(new Vector2Int(bTargetCol, bTargetRow));
                            manager.whiteLegalMoves.Add(new Vector2Int(bTargetCol, bTargetRow));
                            bTargetCol--;
                            bTargetRow--;
                        }

                        else if (manager.chessboard[bTargetCol, bTargetRow] != null)
                        {
                            GameObject piece = manager.chessboard[bTargetCol, bTargetRow];
                            ChessPiece pieceProperties = piece.GetComponent<ChessPiece>();

                            if (pieceProperties.color == ChessPieceColor.White)
                            {
                                break;
                            }
                            else if (pieceProperties.color == ChessPieceColor.Black)
                            {
                                legalMoves.Add(new Vector2Int(bTargetCol, bTargetRow));
                                manager.whiteLegalMoves.Add(new Vector2Int(bTargetCol, bTargetRow));
                                break;
                            }

                        }

                    }

                    //check right-down diagonal
                    bTargetCol = col + 1;
                    bTargetRow = row - 1;
                    while (bTargetCol < 8 && bTargetRow > -1)
                    {

                        if (manager.chessboard[bTargetCol, bTargetRow] == null)
                        {
                            legalMoves.Add(new Vector2Int(bTargetCol, bTargetRow));
                            manager.whiteLegalMoves.Add(new Vector2Int(bTargetCol, bTargetRow));
                            bTargetCol++;
                            bTargetRow--;
                        }

                        else if (manager.chessboard[bTargetCol, bTargetRow] != null)
                        {
                            GameObject piece = manager.chessboard[bTargetCol, bTargetRow];
                            ChessPiece pieceProperties = piece.GetComponent<ChessPiece>();

                            if (pieceProperties.color == ChessPieceColor.White)
                            {
                                break;
                            }
                            else if (pieceProperties.color == ChessPieceColor.Black)
                            {
                                legalMoves.Add(new Vector2Int(bTargetCol, bTargetRow));
                                manager.whiteLegalMoves.Add(new Vector2Int(bTargetCol, bTargetRow));
                                break;
                            }

                        }

                    }
                    break;

                case ChessPieceType.Queen:
                    //Rook Movement
                    //check forward
                    for (int targetRow = row + 1; targetRow < 8; targetRow++)
                    {

                        if (manager.chessboard[col, targetRow] == null)
                        {
                            legalMoves.Add(new Vector2Int(col, targetRow));
                            manager.whiteLegalMoves.Add(new Vector2Int(col, targetRow));
                        }

                        else if (manager.chessboard[col, targetRow] != null)
                        {
                            GameObject piece = manager.chessboard[col, targetRow];
                            ChessPiece pieceProperties = piece.GetComponent<ChessPiece>();

                            //check for allied piece
                            if (pieceProperties.color == ChessPieceColor.White)
                            {
                                break;
                            }

                            else if (pieceProperties.color == ChessPieceColor.Black)
                            {
                                legalMoves.Add(new Vector2Int(col, targetRow));
                                manager.whiteLegalMoves.Add(new Vector2Int(col, targetRow));
                                break;
                            }
                        }

                    }
                    //check backward
                    for (int targetRow = row - 1; targetRow > -1; targetRow--)
                    {
                        Debug.Log("PromotedQueenTest");

                        if (manager.chessboard[col, targetRow] == null)
                        {
                            legalMoves.Add(new Vector2Int(col, targetRow));
                            manager.whiteLegalMoves.Add(new Vector2Int(col, targetRow));
                        }

                        else if (manager.chessboard[col, targetRow] != null)
                        {
                            GameObject piece = manager.chessboard[col, targetRow];
                            ChessPiece pieceProperties = piece.GetComponent<ChessPiece>();
                            //check for allied piece
                            if (pieceProperties.color == ChessPieceColor.White)
                            {
                                break;
                            }

                            else if (pieceProperties.color == ChessPieceColor.Black)
                            {
                                legalMoves.Add(new Vector2Int(col, targetRow));
                                manager.whiteLegalMoves.Add(new Vector2Int(col, targetRow));
                                break;
                            }
                        }
                    }
                    //check left
                    for (int targetCol = col - 1; targetCol > -1; targetCol--)
                    {
                        if (manager.chessboard[targetCol, row] == null)
                        {
                            legalMoves.Add(new Vector2Int(targetCol, row));
                            manager.whiteLegalMoves.Add(new Vector2Int(targetCol, row));
                        }

                        else if (manager.chessboard[targetCol, row] != null)
                        {
                            GameObject piece = manager.chessboard[targetCol, row];
                            ChessPiece pieceProperties = piece.GetComponent<ChessPiece>();

                            //check for allied piece
                            if (pieceProperties.color == ChessPieceColor.White)
                            {
                                break;
                            }

                            else if (pieceProperties.color == ChessPieceColor.Black)
                            {
                                legalMoves.Add(new Vector2Int(targetCol, row));
                                manager.whiteLegalMoves.Add(new Vector2Int(targetCol, row));
                                break;
                            }
                        }
                    }
                    //check right
                    for (int targetCol = col + 1; targetCol < 8; targetCol++)
                    {
                        if (manager.chessboard[targetCol, row] == null)
                        {
                            legalMoves.Add(new Vector2Int(targetCol, row));
                            manager.whiteLegalMoves.Add(new Vector2Int(targetCol, row));
                        }

                        else if (manager.chessboard[targetCol, row] != null)
                        {
                            GameObject piece = manager.chessboard[targetCol, row];
                            ChessPiece pieceProperties = piece.GetComponent<ChessPiece>();

                            //check for allied piece
                            if (pieceProperties.color == ChessPieceColor.White)
                            {
                                break;
                            }

                            else if (pieceProperties.color == ChessPieceColor.Black)
                            {
                                legalMoves.Add(new Vector2Int(targetCol, row));
                                manager.whiteLegalMoves.Add(new Vector2Int(targetCol, row));
                                break;
                            }
                        }
                    }

                    //Bishop Movement
                    int qTargetCol = col;
                    int qTargetRow = row;

                    //check left-up diagonal

                    qTargetCol = col - 1;
                    qTargetRow = row + 1;
                    while (qTargetCol > -1 && qTargetRow < 8)
                    {

                        if (manager.chessboard[qTargetCol, qTargetRow] == null)
                        {
                            legalMoves.Add(new Vector2Int(qTargetCol, qTargetRow));
                            manager.whiteLegalMoves.Add(new Vector2Int(qTargetCol, qTargetRow));
                            qTargetCol--;
                            qTargetRow++;
                        }

                        else if (manager.chessboard[qTargetCol, qTargetRow] != null)
                        {
                            GameObject piece = manager.chessboard[qTargetCol, qTargetRow];
                            ChessPiece pieceProperties = piece.GetComponent<ChessPiece>();

                            if (pieceProperties.color == ChessPieceColor.White)
                            {
                                break;
                            }
                            else if (pieceProperties.color == ChessPieceColor.Black)
                            {
                                legalMoves.Add(new Vector2Int(qTargetCol, qTargetRow));
                                manager.whiteLegalMoves.Add(new Vector2Int(qTargetCol, qTargetRow));
                                break;
                            }

                        }

                    }



                    //check right-up diagonal
                    qTargetCol = col + 1;
                    qTargetRow = row + 1;
                    while (qTargetCol < 8 && qTargetRow < 8)
                    {

                        if (manager.chessboard[qTargetCol, qTargetRow] == null)
                        {
                            legalMoves.Add(new Vector2Int(qTargetCol, qTargetRow));
                            manager.whiteLegalMoves.Add(new Vector2Int(qTargetCol, qTargetRow));
                            qTargetCol++;
                            qTargetRow++;
                        }

                        else if (manager.chessboard[qTargetCol, qTargetRow] != null)
                        {
                            GameObject piece = manager.chessboard[qTargetCol, qTargetRow];
                            ChessPiece pieceProperties = piece.GetComponent<ChessPiece>();

                            if (pieceProperties.color == ChessPieceColor.White)
                            {
                                break;
                            }
                            else if (pieceProperties.color == ChessPieceColor.Black)
                            {
                                legalMoves.Add(new Vector2Int(qTargetCol, qTargetRow));
                                manager.whiteLegalMoves.Add(new Vector2Int(qTargetCol, qTargetRow));
                                break;
                            }

                        }

                    }

                    //check left-down diagonal
                    qTargetCol = col - 1;
                    qTargetRow = row - 1;

                    while (qTargetCol > -1 && qTargetRow > -1)
                    {

                        if (manager.chessboard[qTargetCol, qTargetRow] == null)
                        {
                            legalMoves.Add(new Vector2Int(qTargetCol, qTargetRow));
                            manager.whiteLegalMoves.Add(new Vector2Int(qTargetCol, qTargetRow));
                            qTargetCol--;
                            qTargetRow--;
                        }

                        else if (manager.chessboard[qTargetCol, qTargetRow] != null)
                        {
                            GameObject piece = manager.chessboard[qTargetCol, qTargetRow];
                            ChessPiece pieceProperties = piece.GetComponent<ChessPiece>();

                            if (pieceProperties.color == ChessPieceColor.White)
                            {
                                break;
                            }
                            else if (pieceProperties.color == ChessPieceColor.Black)
                            {
                                legalMoves.Add(new Vector2Int(qTargetCol, qTargetRow));
                                manager.whiteLegalMoves.Add(new Vector2Int(qTargetCol, qTargetRow));
                                break;
                            }

                        }

                    }

                    //check right-down diagonal
                    qTargetCol = col + 1;
                    qTargetRow = row - 1;
                    while (qTargetCol < 8 && qTargetRow > -1)
                    {

                        if (manager.chessboard[qTargetCol, qTargetRow] == null)
                        {
                            legalMoves.Add(new Vector2Int(qTargetCol, qTargetRow));
                            manager.whiteLegalMoves.Add(new Vector2Int(qTargetCol, qTargetRow));
                            qTargetCol++;
                            qTargetRow--;
                        }

                        else if (manager.chessboard[qTargetCol, qTargetRow] != null)
                        {
                            GameObject piece = manager.chessboard[qTargetCol, qTargetRow];
                            ChessPiece pieceProperties = piece.GetComponent<ChessPiece>();

                            if (pieceProperties.color == ChessPieceColor.White)
                            {
                                break;
                            }
                            else if (pieceProperties.color == ChessPieceColor.Black)
                            {
                                legalMoves.Add(new Vector2Int(qTargetCol, qTargetRow));
                                manager.whiteLegalMoves.Add(new Vector2Int(qTargetCol, qTargetRow));
                                break;
                            }

                        }

                    }
                    break;


                case ChessPieceType.King:
                    //add clause for threatened spaces and protected pieces;check list of all legal moves of all enemy pieces
                    //add castle rules
                    int kingForward = row + 1;
                    int kingBackward = row - 1;
                    int kingLeft = col - 1;
                    int kingRight = col + 1;

                    int kTargetRow;
                    int kTargetCol;

                    //up
                    if (kingForward < 8)
                    {
                        kTargetRow = kingForward;
                        kTargetCol = col;

                        if (manager.chessboard[kTargetCol, kTargetRow] == null)
                        {
                            legalMoves.Add(new Vector2Int(col, kingForward));
                            manager.whiteLegalMoves.Add(new Vector2Int(col, kingForward));
                        }

                        if (manager.chessboard[kTargetCol, kTargetRow] != null)
                        {
                            GameObject targetPiece = manager.chessboard[kTargetCol, kTargetRow];
                            ChessPiece targetPieceProperties = targetPiece.GetComponent<ChessPiece>();

                            if (targetPieceProperties.color == ChessPieceColor.Black)
                            {
                                legalMoves.Add(new Vector2Int(col, kingForward));
                                manager.whiteLegalMoves.Add(new Vector2Int(col, kingForward));
                            }
                        }
                    }

                    //down
                    if (kingBackward > -1)
                    {
                        kTargetRow = kingBackward;
                        kTargetCol = col;

                        if (manager.chessboard[kTargetCol, kTargetRow] == null)
                        {
                            legalMoves.Add(new Vector2Int(col, kingBackward));
                            manager.whiteLegalMoves.Add(new Vector2Int(col, kingBackward));
                        }

                        if (manager.chessboard[kTargetCol, kTargetRow] != null)
                        {
                            GameObject targetPiece = manager.chessboard[kTargetCol, kTargetRow];
                            ChessPiece targetPieceProperties = targetPiece.GetComponent<ChessPiece>();

                            if (targetPieceProperties.color == ChessPieceColor.Black)
                            {
                                legalMoves.Add(new Vector2Int(col, kingBackward));
                                manager.whiteLegalMoves.Add(new Vector2Int(col, kingBackward));
                            }
                        }
                    }

                    //left
                    if (kingLeft > -1)
                    {
                        kTargetRow = row;
                        kTargetCol = kingLeft;

                        if (manager.chessboard[kTargetCol, kTargetRow] == null)
                        {
                            legalMoves.Add(new Vector2Int(kingLeft, row));
                            manager.whiteLegalMoves.Add(new Vector2Int(kingLeft, row));
                        }

                        if (manager.chessboard[kTargetCol, kTargetRow] != null)
                        {
                            GameObject targetPiece = manager.chessboard[kTargetCol, kTargetRow];
                            ChessPiece targetPieceProperties = targetPiece.GetComponent<ChessPiece>();

                            if (targetPieceProperties.color == ChessPieceColor.Black)
                            {
                                legalMoves.Add(new Vector2Int(kingLeft, row));
                                manager.whiteLegalMoves.Add(new Vector2Int(kingLeft, row));
                            }
                        }
                    }

                    //right
                    if (kingRight < 8)
                    {
                        kTargetRow = row;
                        kTargetCol = kingRight;

                        if (manager.chessboard[kTargetCol, kTargetRow] == null)
                        {
                            legalMoves.Add(new Vector2Int(kingRight, row));
                            manager.whiteLegalMoves.Add(new Vector2Int(kingRight, row));
                        }

                        if (manager.chessboard[kTargetCol, kTargetRow] != null)
                        {
                            GameObject targetPiece = manager.chessboard[kTargetCol, kTargetRow];
                            ChessPiece targetPieceProperties = targetPiece.GetComponent<ChessPiece>();

                            if (targetPieceProperties.color == ChessPieceColor.Black)
                            {
                                legalMoves.Add(new Vector2Int(kingRight, row));
                                manager.whiteLegalMoves.Add(new Vector2Int(kingRight, row));
                            }
                        }
                    }



                    //diagonal right-up
                    if (kingRight < 8 && kingForward < 8)
                    {
                        kTargetRow = kingForward;
                        kTargetCol = kingRight;

                        if (manager.chessboard[kTargetCol, kTargetRow] == null)
                        {
                            legalMoves.Add(new Vector2Int(kingRight, kingForward));
                            manager.whiteLegalMoves.Add(new Vector2Int(kingRight, kingForward));
                        }

                        if (manager.chessboard[kTargetCol, kTargetRow] != null)
                        {
                            GameObject targetPiece = manager.chessboard[kTargetCol, kTargetRow];
                            ChessPiece targetPieceProperties = targetPiece.GetComponent<ChessPiece>();

                            if (targetPieceProperties.color == ChessPieceColor.Black)
                            {
                                legalMoves.Add(new Vector2Int(kingRight, kingForward));
                                manager.whiteLegalMoves.Add(new Vector2Int(kingRight, kingForward));
                            }
                        }
                    }

                    //diagonal left-up
                    if (kingLeft > -1 && kingForward < 8)
                    {
                        kTargetRow = kingForward;
                        kTargetCol = kingLeft;

                        if (manager.chessboard[kTargetCol, kTargetRow] == null)
                        {
                            legalMoves.Add(new Vector2Int(kingLeft, kingForward));
                            manager.whiteLegalMoves.Add(new Vector2Int(kingLeft, kingForward));
                        }

                        else if (manager.chessboard[kTargetCol, kTargetRow] != null)
                        {
                            GameObject targetPiece = manager.chessboard[kTargetCol, kTargetRow];
                            ChessPiece targetPieceProperties = targetPiece.GetComponent<ChessPiece>();
                            if (targetPieceProperties.color == ChessPieceColor.Black)
                            {
                                legalMoves.Add(new Vector2Int(kingLeft, kingForward));
                                manager.whiteLegalMoves.Add(new Vector2Int(kingLeft, kingForward));
                            }
                        }
                    }

                    //diagonal left-down
                    if (kingLeft > -1 && kingBackward > -1)
                    {
                        kTargetRow = kingBackward;
                        kTargetCol = kingLeft;

                        if (manager.chessboard[kTargetCol, kTargetRow] == null)
                        {
                            legalMoves.Add(new Vector2Int(kingLeft, kingBackward));
                            manager.whiteLegalMoves.Add(new Vector2Int(kingLeft, kingBackward));
                        }

                        else if (manager.chessboard[kTargetCol, kTargetRow] != null)
                        {
                            GameObject targetPiece = manager.chessboard[kTargetCol, kTargetRow];
                            ChessPiece targetPieceProperties = targetPiece.GetComponent<ChessPiece>();
                            if (targetPieceProperties.color == ChessPieceColor.Black)
                            {
                                legalMoves.Add(new Vector2Int(kingLeft, kingBackward));
                                manager.whiteLegalMoves.Add(new Vector2Int(kingLeft, kingBackward));
                            }
                        }
                    }

                    //diagonal right-down
                    if (kingRight < 8 && kingBackward > -1)
                    {
                        kTargetRow = kingBackward;
                        kTargetCol = kingRight;

                        if (manager.chessboard[kTargetCol, kTargetRow] == null)
                        {
                            legalMoves.Add(new Vector2Int(kingRight, kingBackward));
                            manager.whiteLegalMoves.Add(new Vector2Int(kingRight, kingBackward));
                        }

                        else if (manager.chessboard[kTargetCol, kTargetRow] != null)
                        {
                            GameObject targetPiece = manager.chessboard[kTargetCol, kTargetRow];
                            ChessPiece targetPieceProperties = targetPiece.GetComponent<ChessPiece>();
                            if (targetPieceProperties.color == ChessPieceColor.Black)
                            {
                                legalMoves.Add(new Vector2Int(kingRight, kingBackward));
                                manager.whiteLegalMoves.Add(new Vector2Int(kingRight, kingBackward));
                            }
                        }
                    }

                    //castle
                    
                    break;
            }
        }

        //black pieces
        else if (color == ChessPieceColor.Black)
        {
            switch (type)
            {
                case ChessPieceType.Pawn:

                    int oneForward = row - 1;
                    int twoForward = row - 2;
                    int oneLeft = col - 1;
                    int oneRight = col + 1;

                    int ptargetRow;
                    int ptargetCol;

                    if (oneForward > -1)
                    {
                        //one square move
                        if (manager.chessboard[col, oneForward] == null)
                        {
                            legalMoves.Add(new Vector2Int(col, oneForward));
                            manager.blackLegalMoves.Add(new Vector2Int(col, oneForward));
                        }

                        //first move, two square move
                        if (hasMoved == false && manager.chessboard[col, twoForward] == null)
                        {
                            legalMoves.Add(new Vector2Int(col, twoForward));
                            manager.blackLegalMoves.Add(new Vector2Int(col, twoForward));
                        }

                        //diagonal capture
                        if (oneRight < 8)
                        {
                            ptargetRow = oneForward;
                            ptargetCol = oneRight;

                            if (manager.chessboard[ptargetCol, ptargetRow] != null)
                            {
                                GameObject pawnPiece = manager.chessboard[ptargetCol, ptargetRow];
                                ChessPiece pawnPieceProperties = pawnPiece.GetComponent<ChessPiece>();

                                if (pawnPieceProperties.color == ChessPieceColor.White)
                                {
                                    legalMoves.Add(new Vector2Int(oneRight, oneForward));
                                    manager.blackLegalMoves.Add(new Vector2Int(oneRight, oneForward));
                                }
                            }

                            if (manager.chessboard[ptargetCol, row] != null)
                            {
                                GameObject enPassantPiece = manager.chessboard[ptargetCol, row];
                                ChessPiece enPassantPieceProperties = enPassantPiece.GetComponent<ChessPiece>();

                                if (enPassantPieceProperties.color == ChessPieceColor.White && enPassantPieceProperties.pawnDoubleMoved == true && enPassantPieceProperties.movedTurnCount == 1)
                                {
                                    Debug.Log("enPassant is available on " + enPassantPiece.name);
                                    Debug.Log("enPassant's move turn count is " + enPassantPieceProperties.movedTurnCount);
                                    legalMoves.Add(new Vector2Int(oneRight, oneForward));
                                    manager.blackLegalMoves.Add(new Vector2Int(oneRight, oneForward));
                                }
                            }

                        }
                        if (oneLeft > -1)
                        {
                            ptargetRow = oneForward;
                            ptargetCol = oneLeft;

                            if (manager.chessboard[ptargetCol, ptargetRow] != null)
                            {
                                GameObject pawnPiece = manager.chessboard[ptargetCol, ptargetRow];
                                ChessPiece pawnPieceProperties = pawnPiece.GetComponent<ChessPiece>();
                                if (pawnPieceProperties.color == ChessPieceColor.White)
                                {
                                    legalMoves.Add(new Vector2Int(oneLeft, oneForward));
                                    manager.blackLegalMoves.Add(new Vector2Int(oneLeft, oneForward));
                                }
                            }

                            if (manager.chessboard[ptargetCol, row] != null)
                            {
                                GameObject enPassantPiece = manager.chessboard[ptargetCol, row];
                                ChessPiece enPassantPieceProperties = enPassantPiece.GetComponent<ChessPiece>();

                                if (enPassantPieceProperties.color == ChessPieceColor.White && enPassantPieceProperties.pawnDoubleMoved == true && enPassantPieceProperties.movedTurnCount == 1)
                                {
                                    Debug.Log("enPassant is available on " + enPassantPiece.name);
                                    Debug.Log("enPassant's move turn count is " + enPassantPieceProperties.movedTurnCount);
                                    legalMoves.Add(new Vector2Int(oneLeft, oneForward));
                                    manager.blackLegalMoves.Add(new Vector2Int(oneLeft, oneForward));
                                }
                            }
                        }
                    }
                    //add en passant

                    break;



                case ChessPieceType.Rook:

                    //check forward
                    for (int targetRow = row + 1; targetRow < 8; targetRow++)
                    {

                        if (manager.chessboard[col, targetRow] == null)
                        {
                            legalMoves.Add(new Vector2Int(col, targetRow));
                            manager.blackLegalMoves.Add(new Vector2Int(col, targetRow));
                        }

                        else if (manager.chessboard[col, targetRow] != null)
                        {
                            GameObject piece = manager.chessboard[col, targetRow];
                            ChessPiece pieceProperties = piece.GetComponent<ChessPiece>();

                            //check for allied piece
                            if (pieceProperties.color == ChessPieceColor.Black)
                            {
                                break;
                            }

                            else if (pieceProperties.color == ChessPieceColor.White)
                            {
                                legalMoves.Add(new Vector2Int(col, targetRow));
                                manager.blackLegalMoves.Add(new Vector2Int(col, targetRow));
                                break;
                            }
                        }

                    }
                    //check backward
                    for (int targetRow = row - 1; targetRow > -1; targetRow--)
                    {

                        if (manager.chessboard[col, targetRow] == null)
                        {
                            legalMoves.Add(new Vector2Int(col, targetRow));
                            manager.blackLegalMoves.Add(new Vector2Int(col, targetRow));
                        }

                        else if (manager.chessboard[col, targetRow] != null)
                        {
                            GameObject piece = manager.chessboard[col, targetRow];
                            ChessPiece pieceProperties = piece.GetComponent<ChessPiece>();
                            //check for allied piece
                            if (pieceProperties.color == ChessPieceColor.Black)
                            {
                                break;
                            }

                            else if (pieceProperties.color == ChessPieceColor.White)
                            {
                                legalMoves.Add(new Vector2Int(col, targetRow));
                                manager.blackLegalMoves.Add(new Vector2Int(col, targetRow));
                                break;
                            }
                        }
                    }
                    //check left
                    for (int targetCol = col - 1; targetCol > -1; targetCol--)
                    {
                        if (manager.chessboard[targetCol, row] == null)
                        {
                            legalMoves.Add(new Vector2Int(targetCol, row));
                            manager.blackLegalMoves.Add(new Vector2Int(targetCol, row));
                        }

                        else if (manager.chessboard[targetCol, row] != null)
                        {
                            GameObject piece = manager.chessboard[targetCol, row];
                            ChessPiece pieceProperties = piece.GetComponent<ChessPiece>();

                            //check for allied piece
                            if (pieceProperties.color == ChessPieceColor.Black)
                            {
                                break;
                            }

                            else if (pieceProperties.color == ChessPieceColor.White)
                            {
                                legalMoves.Add(new Vector2Int(targetCol, row));
                                manager.blackLegalMoves.Add(new Vector2Int(targetCol, row));
                                break;
                            }
                        }
                    }
                    //check right
                    for (int targetCol = col + 1; targetCol < 8; targetCol++)
                    {
                        if (manager.chessboard[targetCol, row] == null)
                        {
                            legalMoves.Add(new Vector2Int(targetCol, row));
                            manager.blackLegalMoves.Add(new Vector2Int(targetCol, row));
                        }

                        else if (manager.chessboard[targetCol, row] != null)
                        {
                            GameObject piece = manager.chessboard[targetCol, row];
                            ChessPiece pieceProperties = piece.GetComponent<ChessPiece>();

                            //check for allied piece
                            if (pieceProperties.color == ChessPieceColor.Black)
                            {
                                break;
                            }

                            else if (pieceProperties.color == ChessPieceColor.White)
                            {
                                legalMoves.Add(new Vector2Int(targetCol, row));
                                manager.blackLegalMoves.Add(new Vector2Int(targetCol, row));
                                break;
                            }
                        }
                    }
                    break;

                case ChessPieceType.Knight:
                    int oneF = row + 1;
                    int twoF = row + 2;
                    int oneL = col - 1;
                    int oneR = col + 1;

                    int twoL = col - 2;
                    int twoR = col + 2;
                    int oneB = row - 1;
                    int twoB = row - 2;




                    //2F1L
                    if (twoF < 8 && oneL > -1)
                    {
                        if (manager.chessboard[oneL, twoF] == null)
                        {
                            legalMoves.Add(new Vector2Int(oneL, twoF));
                            manager.blackLegalMoves.Add(new Vector2Int(oneL, twoF));
                        }

                        else if (manager.chessboard[oneL, twoF] != null)
                        {
                            GameObject knightPiece = manager.chessboard[oneL, twoF];
                            ChessPiece knightPieceProperties = knightPiece.GetComponent<ChessPiece>();
                            if (knightPieceProperties.color == ChessPieceColor.White)
                            {
                                legalMoves.Add(new Vector2Int(oneL, twoF));
                                manager.blackLegalMoves.Add(new Vector2Int(oneL, twoF));
                            }
                        }
                    }

                    //1F2L
                    if (oneF < 8 && twoL > -1)
                    {


                        if (manager.chessboard[twoL, oneF] == null)
                        {
                            legalMoves.Add(new Vector2Int(twoL, oneF));
                            manager.blackLegalMoves.Add(new Vector2Int(twoL, oneF));
                        }

                        else if (manager.chessboard[twoL, oneF] != null)
                        {
                            GameObject knightPieceTwo = manager.chessboard[twoL, oneF];
                            ChessPiece knightPieceTwoProperties = knightPieceTwo.GetComponent<ChessPiece>();
                            if (knightPieceTwoProperties.color == ChessPieceColor.White)
                            {
                                legalMoves.Add(new Vector2Int(twoL, oneF));
                                manager.blackLegalMoves.Add(new Vector2Int(twoL, oneF));
                            }
                        }
                    }

                    //2F1R
                    if (twoF < 8 && oneR < 8)
                    {

                        if (manager.chessboard[oneR, twoF] == null)
                        {
                            legalMoves.Add(new Vector2Int(oneR, twoF));
                            manager.blackLegalMoves.Add(new Vector2Int(oneR, twoF));
                        }

                        else if (manager.chessboard[oneR, twoF] != null)
                        {
                            GameObject knightPieceThree = manager.chessboard[oneR, twoF];
                            ChessPiece knightPieceThreeProperties = knightPieceThree.GetComponent<ChessPiece>();
                            if (knightPieceThreeProperties.color == ChessPieceColor.White)
                            {
                                legalMoves.Add(new Vector2Int(oneR, twoF));
                                manager.blackLegalMoves.Add(new Vector2Int(oneR, twoF));
                            }
                        }

                    }

                    //1F2R
                    if (oneF < 8 && twoR < 8)
                    {

                        if (manager.chessboard[twoR, oneF] == null)
                        {
                            legalMoves.Add(new Vector2Int(twoR, oneF));
                            manager.blackLegalMoves.Add(new Vector2Int(twoR, oneF));
                        }

                        else if (manager.chessboard[twoR, oneF] != null)
                        {
                            GameObject knightPieceFour = manager.chessboard[twoR, oneF];
                            ChessPiece knightPieceFourProperties = knightPieceFour.GetComponent<ChessPiece>();
                            if (knightPieceFourProperties.color == ChessPieceColor.White)
                            {
                                legalMoves.Add(new Vector2Int(twoR, oneF));
                                manager.blackLegalMoves.Add(new Vector2Int(twoR, oneF));
                            }
                        }

                    }

                    //2B1L
                    if (twoB > -1 && oneL > -1)
                    {
                        if (manager.chessboard[oneL, twoB] == null)
                        {
                            legalMoves.Add(new Vector2Int(oneL, twoB));
                            manager.blackLegalMoves.Add(new Vector2Int(oneL, twoB));
                        }

                        else if (manager.chessboard[oneL, twoB] != null)
                        {
                            GameObject knightPieceFive = manager.chessboard[oneL, twoB];
                            ChessPiece knightPieceFiveProperties = knightPieceFive.GetComponent<ChessPiece>();
                            if (knightPieceFiveProperties.color == ChessPieceColor.White)
                            {
                                legalMoves.Add(new Vector2Int(oneL, twoB));
                                manager.blackLegalMoves.Add(new Vector2Int(oneL, twoB));
                            }
                        }

                    }


                    //1B2L
                    if (oneB > -1 && twoL > -1)
                    {
                        if (manager.chessboard[twoL, oneB] == null)
                        {
                            legalMoves.Add(new Vector2Int(twoL, oneB));
                            manager.blackLegalMoves.Add(new Vector2Int(twoL, oneB));
                        }

                        else if (manager.chessboard[twoL, oneB] != null)
                        {
                            GameObject knightPieceSix = manager.chessboard[twoL, oneB];
                            ChessPiece knightPieceSixProperties = knightPieceSix.GetComponent<ChessPiece>();
                            if (knightPieceSixProperties.color == ChessPieceColor.White)
                            {
                                legalMoves.Add(new Vector2Int(twoL, oneB));
                                manager.blackLegalMoves.Add(new Vector2Int(twoL, oneB));
                            }
                        }
                    }

                    //2B1R
                    if (twoB > -1 && oneR < 8)
                    {
                        if (manager.chessboard[oneR, twoB] == null)
                        {
                            legalMoves.Add(new Vector2Int(oneR, twoB));
                            manager.blackLegalMoves.Add(new Vector2Int(oneR, twoB));
                        }


                        else if (manager.chessboard[oneR, twoB] != null)
                        {
                            GameObject knightPieceSeven = manager.chessboard[oneR, twoB];
                            ChessPiece knightPieceSevenProperties = knightPieceSeven.GetComponent<ChessPiece>();

                            if (knightPieceSevenProperties.color == ChessPieceColor.White)
                            {
                                legalMoves.Add(new Vector2Int(oneR, twoB));
                                manager.blackLegalMoves.Add(new Vector2Int(oneR, twoB));
                            }
                        }
                    }

                    //1B2R
                    if (oneB > -1 && twoR < 8)
                    {
                        if (manager.chessboard[twoR, oneB] == null)
                        {
                            legalMoves.Add(new Vector2Int(twoR, oneB));
                            manager.blackLegalMoves.Add(new Vector2Int(twoR, oneB));
                        }
                        else if (manager.chessboard[twoR, oneB] != null)
                        {
                            GameObject knightPieceEight = manager.chessboard[twoR, oneB];
                            ChessPiece knightPieceEightProperties = knightPieceEight.GetComponent<ChessPiece>();
                            if (knightPieceEightProperties.color == ChessPieceColor.White)
                            {
                                legalMoves.Add(new Vector2Int(twoR, oneB));
                                manager.blackLegalMoves.Add(new Vector2Int(twoR, oneB));
                            }
                        }

                    }
                    break;


                case ChessPieceType.Bishop:
                    //BISHOPS TELEPORT FKING FIX THAT
                    int bTargetCol = col;
                    int bTargetRow = row;

                    //check left-up diagonal

                    bTargetCol = col - 1;
                    bTargetRow = row + 1;
                    while (bTargetCol > -1 && bTargetRow < 8)
                    {

                        if (manager.chessboard[bTargetCol, bTargetRow] == null)
                        {
                            legalMoves.Add(new Vector2Int(bTargetCol, bTargetRow));
                            manager.blackLegalMoves.Add(new Vector2Int(bTargetCol, bTargetRow));
                            bTargetCol--;
                            bTargetRow++;
                        }

                        else if (manager.chessboard[bTargetCol, bTargetRow] != null)
                        {
                            GameObject piece = manager.chessboard[bTargetCol, bTargetRow];
                            ChessPiece pieceProperties = piece.GetComponent<ChessPiece>();

                            if (pieceProperties.color == ChessPieceColor.Black)
                            {
                                break;
                            }
                            else if (pieceProperties.color == ChessPieceColor.White)
                            {
                                legalMoves.Add(new Vector2Int(bTargetCol, bTargetRow));
                                manager.blackLegalMoves.Add(new Vector2Int(bTargetCol, bTargetRow));
                                break;
                            }

                        }

                    }



                    //check right-up diagonal
                    bTargetCol = col + 1;
                    bTargetRow = row + 1;
                    while (bTargetCol < 8 && bTargetRow < 8)
                    {

                        if (manager.chessboard[bTargetCol, bTargetRow] == null)
                        {
                            legalMoves.Add(new Vector2Int(bTargetCol, bTargetRow));
                            manager.blackLegalMoves.Add(new Vector2Int(bTargetCol, bTargetRow));
                            bTargetCol++;
                            bTargetRow++;
                        }

                        else if (manager.chessboard[bTargetCol, bTargetRow] != null)
                        {
                            GameObject piece = manager.chessboard[bTargetCol, bTargetRow];
                            ChessPiece pieceProperties = piece.GetComponent<ChessPiece>();

                            if (pieceProperties.color == ChessPieceColor.Black)
                            {
                                break;
                            }
                            else if (pieceProperties.color == ChessPieceColor.White)
                            {
                                legalMoves.Add(new Vector2Int(bTargetCol, bTargetRow));
                                manager.blackLegalMoves.Add(new Vector2Int(bTargetCol, bTargetRow));
                                break;
                            }

                        }

                    }

                    //check left-down diagonal
                    bTargetCol = col - 1;
                    bTargetRow = row - 1;

                    while (bTargetCol > -1 && bTargetRow > -1)
                    {

                        if (manager.chessboard[bTargetCol, bTargetRow] == null)
                        {
                            legalMoves.Add(new Vector2Int(bTargetCol, bTargetRow));
                            manager.blackLegalMoves.Add(new Vector2Int(bTargetCol, bTargetRow));
                            bTargetCol--;
                            bTargetRow--;
                        }

                        else if (manager.chessboard[bTargetCol, bTargetRow] != null)
                        {
                            GameObject piece = manager.chessboard[bTargetCol, bTargetRow];
                            ChessPiece pieceProperties = piece.GetComponent<ChessPiece>();

                            if (pieceProperties.color == ChessPieceColor.Black)
                            {
                                break;
                            }
                            else if (pieceProperties.color == ChessPieceColor.White)
                            {
                                legalMoves.Add(new Vector2Int(bTargetCol, bTargetRow));
                                manager.blackLegalMoves.Add(new Vector2Int(bTargetCol, bTargetRow));
                                break;
                            }

                        }

                    }

                    //check right-down diagonal
                    bTargetCol = col + 1;
                    bTargetRow = row - 1;
                    while (bTargetCol < 8 && bTargetRow > -1)
                    {

                        if (manager.chessboard[bTargetCol, bTargetRow] == null)
                        {
                            legalMoves.Add(new Vector2Int(bTargetCol, bTargetRow));
                            manager.blackLegalMoves.Add(new Vector2Int(bTargetCol, bTargetRow));
                            bTargetCol++;
                            bTargetRow--;
                        }

                        else if (manager.chessboard[bTargetCol, bTargetRow] != null)
                        {
                            GameObject piece = manager.chessboard[bTargetCol, bTargetRow];
                            ChessPiece pieceProperties = piece.GetComponent<ChessPiece>();

                            if (pieceProperties.color == ChessPieceColor.Black)
                            {
                                break;
                            }
                            else if (pieceProperties.color == ChessPieceColor.White)
                            {
                                legalMoves.Add(new Vector2Int(bTargetCol, bTargetRow));
                                manager.blackLegalMoves.Add(new Vector2Int(bTargetCol, bTargetRow));
                                break;
                            }

                        }

                    }
                    break;

                case ChessPieceType.Queen:
                    //Rook Movement
                    //check forward
                    for (int targetRow = row + 1; targetRow < 8; targetRow++)
                    {

                        if (manager.chessboard[col, targetRow] == null)
                        {
                            legalMoves.Add(new Vector2Int(col, targetRow));
                            manager.blackLegalMoves.Add(new Vector2Int(col, targetRow));
                        }

                        else if (manager.chessboard[col, targetRow] != null)
                        {
                            GameObject piece = manager.chessboard[col, targetRow];
                            ChessPiece pieceProperties = piece.GetComponent<ChessPiece>();

                            //check for allied piece
                            if (pieceProperties.color == ChessPieceColor.Black)
                            {
                                break;
                            }

                            else if (pieceProperties.color == ChessPieceColor.White)
                            {
                                legalMoves.Add(new Vector2Int(col, targetRow));
                                manager.blackLegalMoves.Add(new Vector2Int(col, targetRow));
                                break;
                            }
                        }

                    }
                    //check backward
                    for (int targetRow = row - 1; targetRow > -1; targetRow--)
                    {

                        if (manager.chessboard[col, targetRow] == null)
                        {
                            legalMoves.Add(new Vector2Int(col, targetRow));
                            manager.blackLegalMoves.Add(new Vector2Int(col, targetRow));
                        }

                        else if (manager.chessboard[col, targetRow] != null)
                        {
                            GameObject piece = manager.chessboard[col, targetRow];
                            ChessPiece pieceProperties = piece.GetComponent<ChessPiece>();
                            //check for allied piece
                            if (pieceProperties.color == ChessPieceColor.Black)
                            {
                                break;
                            }

                            else if (pieceProperties.color == ChessPieceColor.White)
                            {
                                legalMoves.Add(new Vector2Int(col, targetRow));
                                manager.blackLegalMoves.Add(new Vector2Int(col, targetRow));
                                break;
                            }
                        }
                    }
                    //check left
                    for (int targetCol = col - 1; targetCol > -1; targetCol--)
                    {
                        if (manager.chessboard[targetCol, row] == null)
                        {
                            legalMoves.Add(new Vector2Int(targetCol, row));
                            manager.blackLegalMoves.Add(new Vector2Int(targetCol, row));
                        }

                        else if (manager.chessboard[targetCol, row] != null)
                        {
                            GameObject piece = manager.chessboard[targetCol, row];
                            ChessPiece pieceProperties = piece.GetComponent<ChessPiece>();

                            //check for allied piece
                            if (pieceProperties.color == ChessPieceColor.Black)
                            {
                                break;
                            }

                            else if (pieceProperties.color == ChessPieceColor.White)
                            {
                                legalMoves.Add(new Vector2Int(targetCol, row));
                                manager.blackLegalMoves.Add(new Vector2Int(targetCol, row));
                                break;
                            }
                        }
                    }
                    //check right
                    for (int targetCol = col + 1; targetCol < 8; targetCol++)
                    {
                        if (manager.chessboard[targetCol, row] == null)
                        {
                            legalMoves.Add(new Vector2Int(targetCol, row));
                            manager.blackLegalMoves.Add(new Vector2Int(targetCol, row));
                        }

                        else if (manager.chessboard[targetCol, row] != null)
                        {
                            GameObject piece = manager.chessboard[targetCol, row];
                            ChessPiece pieceProperties = piece.GetComponent<ChessPiece>();

                            //check for allied piece
                            if (pieceProperties.color == ChessPieceColor.Black)
                            {
                                break;
                            }

                            else if (pieceProperties.color == ChessPieceColor.White)
                            {
                                legalMoves.Add(new Vector2Int(targetCol, row));
                                manager.blackLegalMoves.Add(new Vector2Int(targetCol, row));
                                break;
                            }
                        }
                    }

                    //Bishop Movement
                    int qTargetCol = col;
                    int qTargetRow = row;

                    //check left-up diagonal

                    qTargetCol = col - 1;
                    qTargetRow = row + 1;
                    while (qTargetCol > -1 && qTargetRow < 8)
                    {

                        if (manager.chessboard[qTargetCol, qTargetRow] == null)
                        {
                            legalMoves.Add(new Vector2Int(qTargetCol, qTargetRow));
                            manager.blackLegalMoves.Add(new Vector2Int(qTargetCol, qTargetRow));
                            qTargetCol--;
                            qTargetRow++;
                        }

                        else if (manager.chessboard[qTargetCol, qTargetRow] != null)
                        {
                            GameObject piece = manager.chessboard[qTargetCol, qTargetRow];
                            ChessPiece pieceProperties = piece.GetComponent<ChessPiece>();

                            if (pieceProperties.color == ChessPieceColor.Black)
                            {
                                break;
                            }
                            else if (pieceProperties.color == ChessPieceColor.White)
                            {
                                legalMoves.Add(new Vector2Int(qTargetCol, qTargetRow));
                                manager.blackLegalMoves.Add(new Vector2Int(qTargetCol, qTargetRow));
                                break;
                            }

                        }

                    }



                    //check right-up diagonal
                    qTargetCol = col + 1;
                    qTargetRow = row + 1;
                    while (qTargetCol < 8 && qTargetRow < 8)
                    {

                        if (manager.chessboard[qTargetCol, qTargetRow] == null)
                        {
                            legalMoves.Add(new Vector2Int(qTargetCol, qTargetRow));
                            manager.blackLegalMoves.Add(new Vector2Int(qTargetCol, qTargetRow));
                            qTargetCol++;
                            qTargetRow++;
                        }

                        else if (manager.chessboard[qTargetCol, qTargetRow] != null)
                        {
                            GameObject piece = manager.chessboard[qTargetCol, qTargetRow];
                            ChessPiece pieceProperties = piece.GetComponent<ChessPiece>();

                            if (pieceProperties.color == ChessPieceColor.Black)
                            {
                                break;
                            }
                            else if (pieceProperties.color == ChessPieceColor.White)
                            {
                                legalMoves.Add(new Vector2Int(qTargetCol, qTargetRow));
                                manager.blackLegalMoves.Add(new Vector2Int(qTargetCol, qTargetRow));
                                break;
                            }

                        }

                    }

                    //check left-down diagonal
                    qTargetCol = col - 1;
                    qTargetRow = row - 1;

                    while (qTargetCol > -1 && qTargetRow > -1)
                    {

                        if (manager.chessboard[qTargetCol, qTargetRow] == null)
                        {
                            legalMoves.Add(new Vector2Int(qTargetCol, qTargetRow));
                            manager.blackLegalMoves.Add(new Vector2Int(qTargetCol, qTargetRow));
                            qTargetCol--;
                            qTargetRow--;
                        }

                        else if (manager.chessboard[qTargetCol, qTargetRow] != null)
                        {
                            GameObject piece = manager.chessboard[qTargetCol, qTargetRow];
                            ChessPiece pieceProperties = piece.GetComponent<ChessPiece>();

                            if (pieceProperties.color == ChessPieceColor.Black)
                            {
                                break;
                            }
                            else if (pieceProperties.color == ChessPieceColor.White)
                            {
                                legalMoves.Add(new Vector2Int(qTargetCol, qTargetRow));
                                manager.blackLegalMoves.Add(new Vector2Int(qTargetCol, qTargetRow));
                                break;
                            }

                        }

                    }

                    //check right-down diagonal
                    qTargetCol = col + 1;
                    qTargetRow = row - 1;
                    while (qTargetCol < 8 && qTargetRow > -1)
                    {

                        if (manager.chessboard[qTargetCol, qTargetRow] == null)
                        {
                            legalMoves.Add(new Vector2Int(qTargetCol, qTargetRow));
                            manager.blackLegalMoves.Add(new Vector2Int(qTargetCol, qTargetRow));
                            qTargetCol++;
                            qTargetRow--;
                        }

                        else if (manager.chessboard[qTargetCol, qTargetRow] != null)
                        {
                            GameObject piece = manager.chessboard[qTargetCol, qTargetRow];
                            ChessPiece pieceProperties = piece.GetComponent<ChessPiece>();

                            if (pieceProperties.color == ChessPieceColor.Black)
                            {
                                break;
                            }
                            else if (pieceProperties.color == ChessPieceColor.White)
                            {
                                legalMoves.Add(new Vector2Int(qTargetCol, qTargetRow));
                                manager.blackLegalMoves.Add(new Vector2Int(qTargetCol, qTargetRow));
                                break;
                            }

                        }

                    }
                    break;


                case ChessPieceType.King:
                    //add clause for threatened spaces and protected pieces;check list of all legal moves of all enemy pieces
                    //add castle rules
                    int kingForward = row + 1;
                    int kingBackward = row - 1;
                    int kingLeft = col - 1;
                    int kingRight = col + 1;

                    int kTargetRow;
                    int kTargetCol;

                    //up
                    if (kingForward < 8)
                    {
                        kTargetRow = kingForward;
                        kTargetCol = col;

                        if (manager.chessboard[kTargetCol, kTargetRow] == null)
                        {
                            legalMoves.Add(new Vector2Int(col, kingForward));
                            manager.blackLegalMoves.Add(new Vector2Int(col, kingForward));
                        }

                        if (manager.chessboard[kTargetCol, kTargetRow] != null)
                        {
                            GameObject targetPiece = manager.chessboard[kTargetCol, kTargetRow];
                            ChessPiece targetPieceProperties = targetPiece.GetComponent<ChessPiece>();

                            if (targetPieceProperties.color == ChessPieceColor.White)
                            {
                                legalMoves.Add(new Vector2Int(col, kingForward));
                                manager.blackLegalMoves.Add(new Vector2Int(col, kingForward));
                            }
                        }
                    }

                    //down
                    if (kingBackward > -1)
                    {
                        kTargetRow = kingBackward;
                        kTargetCol = col;

                        if (manager.chessboard[kTargetCol, kTargetRow] == null)
                        {
                            legalMoves.Add(new Vector2Int(col, kingBackward));
                            manager.blackLegalMoves.Add(new Vector2Int(col, kingBackward));
                        }

                        if (manager.chessboard[kTargetCol, kTargetRow] != null)
                        {
                            GameObject targetPiece = manager.chessboard[kTargetCol, kTargetRow];
                            ChessPiece targetPieceProperties = targetPiece.GetComponent<ChessPiece>();

                            if (targetPieceProperties.color == ChessPieceColor.White)
                            {
                                legalMoves.Add(new Vector2Int(col, kingBackward));
                                manager.blackLegalMoves.Add(new Vector2Int(col, kingBackward));
                            }
                        }
                    }

                    //left
                    if (kingLeft > -1)
                    {
                        kTargetRow = row;
                        kTargetCol = kingLeft;

                        if (manager.chessboard[kTargetCol, kTargetRow] == null)
                        {
                            legalMoves.Add(new Vector2Int(kingLeft, row));
                            manager.blackLegalMoves.Add(new Vector2Int(kingLeft, row));
                        }

                        if (manager.chessboard[kTargetCol, kTargetRow] != null)
                        {
                            GameObject targetPiece = manager.chessboard[kTargetCol, kTargetRow];
                            ChessPiece targetPieceProperties = targetPiece.GetComponent<ChessPiece>();

                            if (targetPieceProperties.color == ChessPieceColor.White)
                            {
                                legalMoves.Add(new Vector2Int(kingLeft, row));
                                manager.blackLegalMoves.Add(new Vector2Int(kingLeft, row));
                            }
                        }
                    }

                    //right
                    if (kingRight < 8)
                    {
                        kTargetRow = row;
                        kTargetCol = kingRight;

                        if (manager.chessboard[kTargetCol, kTargetRow] == null)
                        {
                            legalMoves.Add(new Vector2Int(kingRight, row));
                            manager.blackLegalMoves.Add(new Vector2Int(kingRight, row));
                        }

                        if (manager.chessboard[kTargetCol, kTargetRow] != null)
                        {
                            GameObject targetPiece = manager.chessboard[kTargetCol, kTargetRow];
                            ChessPiece targetPieceProperties = targetPiece.GetComponent<ChessPiece>();

                            if (targetPieceProperties.color == ChessPieceColor.White)
                            {
                                legalMoves.Add(new Vector2Int(kingRight, row));
                                manager.blackLegalMoves.Add(new Vector2Int(kingRight, row));
                            }
                        }
                    }



                    //diagonal right-up
                    if (kingRight < 8 && kingForward < 8)
                    {
                        kTargetRow = kingForward;
                        kTargetCol = kingRight;

                        if (manager.chessboard[kTargetCol, kTargetRow] == null)
                        {
                            legalMoves.Add(new Vector2Int(kingRight, kingForward));
                            manager.blackLegalMoves.Add(new Vector2Int(kingRight, kingForward));
                        }

                        if (manager.chessboard[kTargetCol, kTargetRow] != null)
                        {
                            GameObject targetPiece = manager.chessboard[kTargetCol, kTargetRow];
                            ChessPiece targetPieceProperties = targetPiece.GetComponent<ChessPiece>();

                            if (targetPieceProperties.color == ChessPieceColor.White)
                            {
                                legalMoves.Add(new Vector2Int(kingRight, kingForward));
                                manager.blackLegalMoves.Add(new Vector2Int(kingRight, kingForward));
                            }
                        }
                    }

                    //diagonal left-up
                    if (kingLeft > -1 && kingForward < 8)
                    {
                        kTargetRow = kingForward;
                        kTargetCol = kingLeft;

                        if (manager.chessboard[kTargetCol, kTargetRow] == null)
                        {
                            legalMoves.Add(new Vector2Int(kingLeft, kingForward));
                            manager.blackLegalMoves.Add(new Vector2Int(kingLeft, kingForward));
                        }

                        else if (manager.chessboard[kTargetCol, kTargetRow] != null)
                        {
                            GameObject targetPiece = manager.chessboard[kTargetCol, kTargetRow];
                            ChessPiece targetPieceProperties = targetPiece.GetComponent<ChessPiece>();
                            if (targetPieceProperties.color == ChessPieceColor.White)
                            {
                                legalMoves.Add(new Vector2Int(kingLeft, kingForward));
                                manager.blackLegalMoves.Add(new Vector2Int(kingLeft, kingForward));
                            }
                        }
                    }

                    //diagonal left-down
                    if (kingLeft > -1 && kingBackward > -1)
                    {
                        kTargetRow = kingBackward;
                        kTargetCol = kingLeft;

                        if (manager.chessboard[kTargetCol, kTargetRow] == null)
                        {
                            legalMoves.Add(new Vector2Int(kingLeft, kingBackward));
                            manager.blackLegalMoves.Add(new Vector2Int(kingLeft, kingBackward));
                        }

                        else if (manager.chessboard[kTargetCol, kTargetRow] != null)
                        {
                            GameObject targetPiece = manager.chessboard[kTargetCol, kTargetRow];
                            ChessPiece targetPieceProperties = targetPiece.GetComponent<ChessPiece>();
                            if (targetPieceProperties.color == ChessPieceColor.White)
                            {
                                legalMoves.Add(new Vector2Int(kingLeft, kingBackward));
                                manager.blackLegalMoves.Add(new Vector2Int(kingLeft, kingBackward));
                            }
                        }
                    }

                    //diagonal right-down
                    if (kingRight < 8 && kingBackward > -1)
                    {
                        kTargetRow = kingBackward;
                        kTargetCol = kingRight;

                        if (manager.chessboard[kTargetCol, kTargetRow] == null)
                        {
                            legalMoves.Add(new Vector2Int(kingRight, kingBackward));
                            manager.blackLegalMoves.Add(new Vector2Int(kingRight, kingBackward));
                        }

                        else if (manager.chessboard[kTargetCol, kTargetRow] != null)
                        {
                            GameObject targetPiece = manager.chessboard[kTargetCol, kTargetRow];
                            ChessPiece targetPieceProperties = targetPiece.GetComponent<ChessPiece>();
                            if (targetPieceProperties.color == ChessPieceColor.White)
                            {
                                legalMoves.Add(new Vector2Int(kingRight, kingBackward));
                                manager.blackLegalMoves.Add(new Vector2Int(kingRight, kingBackward));
                            }
                        }
                    }

                    //castle
                    break;
            }
        }

    }

}
