using UnityEngine;

public class CheckerSelector : MonoBehaviour
{
    private Material originalMaterial;
    public Material selectedMaterial;
    public Material kingMaterial;
    private static CheckerSelector selectedChecker;

    public bool isWhiteChecker; // true для белых, false для черных
    public bool isKing = false; // true, если шашка превращена в дамку
    private static bool isWhiteTurn = true; // true, если ход белых, false, если ход черных

    void Start()
    {
        originalMaterial = GetComponent<Renderer>().material;
    }

    void OnMouseDown()
    {
        if (IsCurrentPlayerTurn())
        {
            if (selectedChecker != null && selectedChecker != this)
            {
                selectedChecker.DeselectChecker();
            }

            SelectChecker();
        }
        else
        {
            Debug.Log("Это не ваш ход!");
        }
    }

    void SelectChecker()
    {
        GetComponent<Renderer>().material = selectedMaterial;
        selectedChecker = this;
        Debug.Log("Шашка выбрана: " + gameObject.name);
    }

    void DeselectChecker()
    {
        GetComponent<Renderer>().material = originalMaterial;
        Debug.Log("Шашка снята с выбора: " + gameObject.name);
        selectedChecker = null;
    }

    public static CheckerSelector GetSelectedChecker()
    {
        return selectedChecker;
    }

    public bool IsWhiteChecker()
    {
        return isWhiteChecker;
    }

    public void MoveTo(Vector3 newPosition)
    {
        newPosition.y = transform.position.y; // Сохраняем исходное значение y
        transform.position = newPosition;
        Debug.Log("Шашка перемещена на: " + newPosition);

        CheckPromotion(newPosition);
        DeselectChecker();
    }

    void CheckPromotion(Vector3 newPosition)
    {
        Debug.Log($"Проверка превращения в дамку: {gameObject.name}, позиция: {newPosition}");

        // Для белых шашек проверяем достижение конечной позиции
        if (isWhiteChecker && newPosition.z < 3.5f)
        {
            PromoteToKing();
        }
        // Для черных шашек проверяем достижение конечной позиции
        else if (!isWhiteChecker && newPosition.z > 15.9f)
        {
            PromoteToKing();
        }
    }

    public void PromoteToKing()
    {
        isKing = true;
        GetComponent<Renderer>().material = kingMaterial;
        Debug.Log("Шашка превращена в дамку: " + gameObject.name);
        DisplayPromotionMessage();
    }

    private void DisplayPromotionMessage()
    {
        // Создание текста на экране
        GameObject promotionText = new GameObject("PromotionText");
        promotionText.transform.position = new Vector3(0, 0, 0);

        TextMesh textMesh = promotionText.AddComponent<TextMesh>();
        textMesh.text = "Шашка превращена в дамку: " + gameObject.name;
        textMesh.fontSize = 48;
        textMesh.color = Color.red;
        textMesh.characterSize = 0.1f;
        textMesh.anchor = TextAnchor.MiddleCenter;

        // Удаление текста через 2 секунды
        Destroy(promotionText, 2.0f);
    }

    public static void ToggleTurn()
    {
        isWhiteTurn = !isWhiteTurn;
        Debug.Log("Ход переключен. Сейчас ход белых: " + isWhiteTurn);
    }

    private bool IsCurrentPlayerTurn()
    {
        return isWhiteTurn == isWhiteChecker;
    }

    public void RemoveChecker()
    {
        Debug.Log("Шашка удалена: " + gameObject.name);
        Destroy(gameObject);
    }
}
