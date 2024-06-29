using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    private CheckerSelector selectedPiece;
    public bool isRedTurn = true;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public bool IsPlayerTurn(bool isRed)
    {
        return isRed == isRedTurn;
    }

    public void SelectPiece(CheckerSelector piece)
    {
        if (selectedPiece != null)
        {
       //     selectedPiece.ResetPosition();
        }

        selectedPiece = piece;
    }

    public void MovePiece(Vector3 position)
    {
        if (selectedPiece != null)
        {
         //   selectedPiece.MoveTo(position);
            EndTurn();
        }
    }

    private void EndTurn()
    {
        isRedTurn = !isRedTurn;
        selectedPiece = null;
    }
}