using TMPro; // Подключение библиотеки TextMesh Pro для работы с текстом
using UnityEngine; // Подключение основной библиотеки Unity
using UnityEngine.UI; // Подключение библиотеки Unity для работы с UI элементами

public class CheckerSelector : MonoBehaviour
{
    private Material originalMaterial; // Исходный материал шашки
    public Material selectedMaterial; // Материал для выделенной шашки
    public Material kingMaterial; // Материал для шашки-короля
    private static CheckerSelector selectedChecker; // Статическая переменная для хранения текущей выделенной шашки

    public bool isWhiteChecker; // true для белой шашки, false для черной
    public bool isKing = false; // true, если шашка повышена до короля
    private static bool isWhiteTurn = true; // true, если ход белых, false, если ход черных

    void Start()
    {
        originalMaterial = GetComponent<Renderer>().material; // Сохранение исходного материала шашки при старте
    }

    void OnMouseDown()
    {
        if (IsCurrentPlayerTurn()) // Проверка, является ли текущий ход игрока
        {
            if (selectedChecker != null && selectedChecker != this) // Если уже есть выделенная шашка и это не текущая шашка
            {
                selectedChecker.DeselectChecker(); // Снять выделение с предыдущей шашки
            }

            SelectChecker(); // Выделить текущую шашку
        }
        else
        {
            Debug.Log("Not your turn!"); // Вывод сообщения, если это не ход текущего игрока
        }
    }

    void SelectChecker()
    {
        GetComponent<Renderer>().material = selectedMaterial; // Установка материала выделенной шашки
        selectedChecker = this; // Установка текущей шашки как выделенной
        Debug.Log($"Checker selected: {transform.position} " + gameObject.name); // Вывод сообщения о выделении шашки
    }

    void DeselectChecker()
    {
        GetComponent<Renderer>().material = originalMaterial; // Восстановление исходного материала шашки
        Debug.Log("Checker deselected: " + gameObject.name); // Вывод сообщения о снятии выделения
        selectedChecker = null; // Сброс выделенной шашки
    }

    public Field GetCurrentField()
    {
        Field[] allFields = FindObjectsOfType<Field>(); // Получение всех полей на доске
        foreach (var item in allFields)
        {
            if (Vector3.Distance(transform.position, item.gameObject.transform.position) < 0.7f)
            {
                return item; // Возврат поля, на котором находится шашка
            }
        }
        return null; // Если поле не найдено, возвращаем null
    }

    public static CheckerSelector GetSelectedChecker()
    {
        return selectedChecker; // Возврат текущей выделенной шашки
    }

    public bool IsWhiteChecker()
    {
        return isWhiteChecker; // Возврат цвета шашки
    }

    public void MoveTo(Vector3 newPosition)
    {
        newPosition.y = transform.position.y; // Сохранение исходного значения y
        transform.position = newPosition; // Перемещение шашки на новую позицию
        Debug.Log("Checker moved to: " + newPosition); // Вывод сообщения о перемещении шашки

        CheckPromotion(newPosition); // Проверка на повышение до короля
        DeselectChecker(); // Снятие выделения после перемещения
    }

    void CheckPromotion(Vector3 newPosition)
    {
        Debug.Log($"Checking promotion to king: {gameObject.name}, position: {newPosition}");

        // Проверка на повышение для черных шашек
        if (!isWhiteChecker && newPosition.z < 3.5f)
        {
            PromoteToKing();
        }
        // Проверка на повышение для белых шашек
        else if (isWhiteChecker && newPosition.z > 15.9f)
        {
            PromoteToKing();
        }
    }

    public void PromoteToKing()
    {
        isKing = true; // Установка флага короля
        GetComponent<Renderer>().material = kingMaterial; // Смена материала на королевский
        Debug.Log("Checker promoted to king: " + gameObject.name); // Вывод сообщения о повышении до короля
        DisplayPromotionMessage(); // Отображение сообщения о повышении
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
        text.text = "Checker promoted to king: " + gameObject.name;
        text.fontSize = 48;
        text.color = Color.red;
        text.alignment = TextAlignmentOptions.Center;

        // Удаление текста через 2 секунды
        Destroy(promotionTextGO, 2.0f);
    }

    public static void ToggleTurn(Field _field)
    {
        // Проверка конца игры
        int blackPossible = 0;
        int whitePossible = 0;

        // Использование Field _field для получения ссылки на функции без изменения структуры
        CheckerSelector[] allCheckers = FindObjectsOfType<CheckerSelector>();
        foreach (CheckerSelector checker in allCheckers)
        {
            if (_field.CanAnyMove(checker))
            {
                if (checker.isWhiteChecker) whitePossible++;
                else blackPossible++;
            }
        }
        if (isWhiteTurn && blackPossible == 0)
        {
            DisplayEndGameMessage("Game over, white wins");
        }

        if (!isWhiteTurn && whitePossible == 0)
        {
            DisplayEndGameMessage("Game over, black wins");
        }

        isWhiteTurn = !isWhiteTurn; // Переключение хода
        Debug.Log("Turn toggled. It's white's turn: " + isWhiteTurn);
    }

    private bool IsCurrentPlayerTurn()
    {
        return isWhiteTurn == isWhiteChecker; // Проверка, является ли текущий ход игрока
    }

    public void RemoveChecker()
    {
        Debug.Log("Checker removed: " + gameObject.name); // Вывод сообщения об удалении шашки
        DestroyImmediate(gameObject); // Немедленное уничтожение объекта шашки
    }

    public static void DisplayEndGameMessage(string text)
    {
        // Получение PauseManager и отображение сообщения о конце игры
        PauseManager pauseManager = FindObjectOfType<PauseManager>();
        if (pauseManager != null)
        {
            pauseManager.DisplayEndGame(text);
        }
        else
        {
            Debug.LogError("PauseManager not found!");
        }
    }
}
