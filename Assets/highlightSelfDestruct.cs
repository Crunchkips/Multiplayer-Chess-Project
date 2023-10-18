using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class highlightSelfDestruct : MonoBehaviour
{
    private ChessPiece parentPiece;

    // Start is called before the first frame update
    void Start()
    {
        parentPiece = GetComponentInParent<ChessPiece>();

    }

    // Update is called once per frame
    void Update()
    {
        if (parentPiece != null && parentPiece.isSelected == false)
        {
            Destroy(this.gameObject);
        }
    }
}
