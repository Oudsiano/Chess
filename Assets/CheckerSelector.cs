using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CheckerSelector : MonoBehaviour
{
    private Material originalMaterial;
    public Material selectedMaterial;
    public Material kingMaterial;
    private static CheckerSelector selectedChecker;

    public bool isWhiteChecker; // true для белых, false для черных
    public bool isKing = false; // true, если шашка превращена в дамку
    private static bool isWhiteTurn = true; // true, если ход белых, false, если ход черных

    private Field _currentField;

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
        Debug.Log($"Шашка выбрана: {transform.position} " + gameObject.name);
    }

    void DeselectChecker()
    {
        GetComponent<Renderer>().material = originalMaterial;
        Debug.Log("Шашка снята с выбора: " + gameObject.name);
        selectedChecker = null;
    }

    public Field GetCurrentField()
    {
        Field[] allFields = FindObjectsOfType<Field>();
        foreach (var item in allFields)
        {
            if (Vector3.Distance(transform.position, item.gameObject.transform.position) < 0.7f)
            {
                return item;
            }
        }
        return null;
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
        DisplayPromotionMessage();
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
        if (!isWhiteChecker && newPosition.z < 3.5f)
        {
            PromoteToKing();
        }
        // Для черных шашек проверяем достижение конечной позиции
        else if (isWhiteChecker && newPosition.z > 15.9f)
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
        GameObject canvasGO = GameObject.Find("Canvas");
        if (canvasGO == null)
        {
            canvasGO = new GameObject("Canvas");
            Canvas canvas = canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasGO.AddComponent<CanvasScaler>();
            canvasGO.AddComponent<GraphicRaycaster>();
        }
        GameObject promotionTextGO = new GameObject("PromotionTextUI");
        promotionTextGO.transform.SetParent(canvasGO.transform);

        RectTransform rectTransform = promotionTextGO.AddComponent<RectTransform>();
        rectTransform.anchoredPosition = Vector2.zero; 
        rectTransform.sizeDelta = new Vector2(600, 200);

        TMPro.TextMeshProUGUI text = promotionTextGO.AddComponent<TMPro.TextMeshProUGUI>();
        text.text = "Шашка превращена в дамку: " + gameObject.name;
        text.fontSize = 48;
        text.color = Color.red;
        text.alignment = TextAlignmentOptions.Center;

        // Удаление текста через 2 секунды
        Destroy(promotionTextGO, 2.0f);
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
