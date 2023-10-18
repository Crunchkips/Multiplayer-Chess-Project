using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Promotion : MonoBehaviour
{
    public GameObject promotedUnitPrefab;
    public TurnManager turnManager;
    public ChessGameManager gameManager;

    public int x;
    



    private void OnMouseDown()
    {


        Debug.Log(x);

        Debug.Log("promotingPiece = turnManager.selectedChessPiece");
        

        turnManager.PromotingPiece(x);

        CloseUI();

        //call serverRPC
        //switch turns
    }


    private void CloseUI()
    {
        if (turnManager.promotionInProgress == true)
        {

            if (turnManager.takenByPromotionPiece != null)
            {
                turnManager.takenByPromotionPieceStandin = turnManager.takenByPromotionPiece;
                turnManager.takenByPromotionPiece.SetActive(true);
                turnManager.takenByPromotionPiece = null;
            }

            turnManager.promotingPiece.transform.position = turnManager.originalPosition;

            

            turnManager.promotionInProgress = false;




            foreach (ChessPiece chessPiece in gameManager.allChessPieces)
            {

                chessPiece.isSelected = false;
                
            }

            Debug.Log("Piece deselected!");
            turnManager.selectedChessPiece = null;

            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }


            turnManager.promotionUI.SetActive(false);
            turnManager.promotionUIBlack.SetActive(false);

            return;

            




        }
    }
}
