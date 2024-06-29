using UnityEngine;

public class Field : MonoBehaviour
{
    void OnMouseDown()
    {
        CheckerSelector selectedChecker = CheckerSelector.GetSelectedChecker();
        if (selectedChecker != null)
        {
            Debug.Log("Поле нажато: " + gameObject.name);
            CheckerSelector enemyChecker;
            if (IsValidMove(selectedChecker, transform.position, out enemyChecker))
            {
                Debug.Log("Допустимый ход для шашки: " + selectedChecker.gameObject.name);
                selectedChecker.MoveTo(transform.position);

                if (enemyChecker != null)
                {
                    enemyChecker.RemoveChecker();
                    if (CanCaptureAgain(selectedChecker, transform.position))
                    {
                        Debug.Log("Можно сделать еще одно взятие");
                        // Не переключаем ход, чтобы игрок мог сделать еще одно взятие
                    }
                    else
                    {
                        Debug.Log("Переключение хода после взятия.");
                        CheckerSelector.ToggleTurn(); // Переключаем ход после взятия, если нет возможности еще одного взятия
                    }
                }
                else
                {
                    Debug.Log("Переключение хода после обычного хода.");
                    CheckerSelector.ToggleTurn(); // Переключаем ход после обычного хода
                }
            }
            else
            {
                Debug.Log("Недопустимый ход для шашки: " + selectedChecker.gameObject.name);
            }
        }
    }

    bool IsValidMove(CheckerSelector checker, Vector3 targetPosition, out CheckerSelector enemyChecker)
    {
        enemyChecker = null;

        if (!IsFieldOccupied(targetPosition))
        {
            float deltaX = Mathf.Abs(targetPosition.x - checker.transform.position.x);
            float deltaZ = targetPosition.z - checker.transform.position.z;

            Debug.Log($"Проверка допустимости хода: deltaX = {deltaX}, deltaZ = {deltaZ}");

            if (checker.isKing)
            {
                if (deltaX == deltaZ)
                {
                    Vector3 direction = (targetPosition - checker.transform.position).normalized;
                    Vector3 currentPos = checker.transform.position + direction;

                    while (currentPos != targetPosition)
                    {
                        if (IsFieldOccupied(currentPos))
                        {
                            enemyChecker = GetCheckerAtPosition(currentPos);
                            if (enemyChecker != null && enemyChecker.IsWhiteChecker() != checker.IsWhiteChecker())
                            {
                                currentPos += direction;
                                if (currentPos == targetPosition && !IsFieldOccupied(currentPos))
                                {
                                    return true;
                                }
                            }
                            return false;
                        }
                        currentPos += direction;
                    }
                    return true;
                }
            }
            else
            {
                if ((deltaX >= 1.7f && deltaX <= 2.3f) &&
                    ((checker.IsWhiteChecker() && (deltaZ >= 1.7f && deltaZ <= 2.3f)) ||
                    (!checker.IsWhiteChecker() && (deltaZ <= -1.7f && deltaZ >= -2.3f))))
                {
                    return true;
                }

                if ((deltaX >= 3.7f && deltaX <= 4.3f) &&
                    ((checker.IsWhiteChecker() && (deltaZ >= 3.7f && deltaZ <= 4.3f)) ||
                    (!checker.IsWhiteChecker() && (deltaZ <= -3.7f && deltaZ >= -4.3f))))
                {
                    Vector3 middlePosition = new Vector3((checker.transform.position.x + targetPosition.x) / 2, checker.transform.position.y, (checker.transform.position.z + targetPosition.z) / 2);
                    enemyChecker = GetCheckerAtPosition(middlePosition);
                    if (enemyChecker != null && enemyChecker.IsWhiteChecker() != checker.IsWhiteChecker())
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    bool IsFieldOccupied(Vector3 targetPosition)
    {
        CheckerSelector[] allCheckers = FindObjectsOfType<CheckerSelector>();
        foreach (CheckerSelector checker in allCheckers)
        {
            if (Vector3.Distance(checker.transform.position, targetPosition) < 0.1f)
            {
                Debug.Log("Поле занято: " + checker.gameObject.name);
                return true;
            }
        }
        return false;
    }

    CheckerSelector GetCheckerAtPosition(Vector3 position)
    {
        CheckerSelector[] allCheckers = FindObjectsOfType<CheckerSelector>();
        foreach (CheckerSelector checker in allCheckers)
        {
            if (Vector3.Distance(checker.transform.position, position) < 0.5f)
            {
                Debug.Log($"Шашка найдена на позиции: {position}, имя: {checker.gameObject.name}");
                return checker;
            }
        }
        return null;
    }

    bool CanCaptureAgain(CheckerSelector checker, Vector3 currentPosition)
    {
        Vector3[] possibleCapturePositions = {
            new Vector3(currentPosition.x + 4, currentPosition.y, currentPosition.z + 4),
            new Vector3(currentPosition.x - 4, currentPosition.y, currentPosition.z + 4),
            new Vector3(currentPosition.x + 4, currentPosition.y, currentPosition.z - 4),
            new Vector3(currentPosition.x - 4, currentPosition.y, currentPosition.z - 4)
        };

        foreach (var targetPosition in possibleCapturePositions)
        {
            CheckerSelector enemyChecker;
            if (IsValidMove(checker, targetPosition, out enemyChecker) && enemyChecker != null)
            {
                Debug.Log($"Есть возможность еще одного взятия: {targetPosition}");
                return true;
            }
        }
        Debug.Log("Нет возможности еще одного взятия.");
        return false;
    }
}
