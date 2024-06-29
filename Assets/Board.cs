
using UnityEngine;

public class Board : MonoBehaviour
{
    public LayerMask boardMask;
    public LayerMask pieceMask;
    private GameManager gameManager;

    void Start()
    {
        gameManager = GameManager.instance;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, boardMask))
            {
                Vector3 position = hit.point;
                position = new Vector3(Mathf.Round(position.x), position.y, Mathf.Round(position.z));

                gameManager.MovePiece(position);
            }
        }
    }
}