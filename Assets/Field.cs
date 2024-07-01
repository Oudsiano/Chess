using System.Collections.Generic;
using UnityEngine;

public class Field : MonoBehaviour
{
    void OnMouseDown()
    {
        Debug.Log($"Поле нажато: {transform.position} " + gameObject.name); // Вывод сообщения о нажатии на поле
        CheckerSelector selectedChecker = CheckerSelector.GetSelectedChecker(); // Получение текущей выделенной шашки

        if (selectedChecker != null)
        {
            Debug.Log(Vector3.Distance(selectedChecker.transform.position, transform.position)); // Вывод расстояния между шашкой и целью
            CheckerSelector enemyChecker;
            if (IsValidMove(selectedChecker, transform.position, out enemyChecker)) // Проверка допустимости хода
            {
                Debug.Log("Допустимый ход для шашки: " + selectedChecker.gameObject.name);
                selectedChecker.MoveTo(transform.position); // Перемещение шашки на новое поле

                if (enemyChecker != null) // Если на пути есть вражеская шашка
                {
                    enemyChecker.RemoveChecker(); // Удаление вражеской шашки
                    if (CanCaptureAgain(selectedChecker, transform.position)) // Проверка возможности еще одного взятия
                    {
                        Debug.Log("Можно сделать еще одно взятие");
                        // Не переключаем ход, чтобы игрок мог сделать еще одно взятие
                    }
                    else
                    {
                        Debug.Log("Переключение хода после взятия.");
                        CheckerSelector.ToggleTurn(this); // Переключение хода, если нет возможности еще одного взятия
                    }
                }
                else
                {
                    Debug.Log("Переключение хода после обычного хода.");
                    CheckerSelector.ToggleTurn(this); // Переключение хода после обычного хода
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

        // Проверка выхода за границы доски
        if (targetPosition.x < -5 || targetPosition.x > 10 || targetPosition.z > 17 || targetPosition.z < 2)
            return false;

        if (!IsFieldOccupied(targetPosition)) // Проверка занятости поля
        {
            float deltaX = Mathf.Abs(targetPosition.x - checker.transform.position.x);
            float deltaZ = targetPosition.z - checker.transform.position.z;

            // Проверка допустимости хода для короля
            if (checker.isKing)
            {
                bool _alreadyFindEnemy = false;

                if (Mathf.Abs(deltaX) - Mathf.Abs(deltaZ) < 0.5f) // "Исправление" ошибок с плавающей точкой
                {
                    // Сбор состояния всех ячеек по пути
                    Vector3 direction = (targetPosition - checker.GetCurrentField().transform.position).normalized * (2 / 0.71f);
                    Vector3 currentPos = checker.transform.position + direction;

                    while (Vector3.Distance(currentPos, targetPosition) > 0.7f)
                    {
                        if (IsFieldOccupied(currentPos))
                        {
                            if (_alreadyFindEnemy)
                                return false;

                            enemyChecker = GetCheckerAtPosition(currentPos);
                            if (enemyChecker.IsWhiteChecker() == checker.IsWhiteChecker())
                                enemyChecker = null;

                            if (enemyChecker != null)
                                _alreadyFindEnemy = true;
                        }
                        currentPos += direction;
                    }
                    return true;
                }
            }
            else
            if (Mathf.Abs(deltaX) - Mathf.Abs(deltaZ) < 0.5f)
            {
                // Проверка допустимости обычного хода
                if ((deltaX >= 1.7f && deltaX <= 2.3f) &&
                    ((checker.IsWhiteChecker() && (deltaZ >= 1.7f && deltaZ <= 2.3f)) ||
                    (!checker.IsWhiteChecker() && (deltaZ <= -1.7f && deltaZ >= -2.3f))))
                {
                    return true;
                }

                // Проверка допустимости хода с взятием
                if ((deltaX >= 3.7f && deltaX <= 4.3f))
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
        return false; // Если ход не допустим, возвращаем false
    }

    bool IsFieldOccupied(Vector3 targetPosition)
    {
        // Проверка, занято ли поле шашкой
        CheckerSelector[] allCheckers = FindObjectsOfType<CheckerSelector>();
        foreach (CheckerSelector checker in allCheckers)
        {
            if (Vector3.Distance(checker.transform.position, targetPosition) < 0.7f)
            {
                return true;
            }
        }
        return false;
    }

    CheckerSelector GetCheckerAtPosition(Vector3 position)
    {
        // Получение шашки на указанной позиции
        CheckerSelector[] allCheckers = FindObjectsOfType<CheckerSelector>();
        foreach (CheckerSelector checker in allCheckers)
        {
            if (Vector3.Distance(checker.transform.position, position) < 0.7f)
            {
                return checker;
            }
        }
        return null;
    }

    public bool CanCaptureAgain(CheckerSelector checker, Vector3 currentPosition)
    {
        // Проверка возможности еще одного взятия
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
                Debug.Log($"Есть возможность еще одного взятия: {targetPosition} {enemyChecker.name}");
                return true;
            }
        }
        return false;
    }

    public bool CanAnyMove(CheckerSelector checker)
    {
        // Проверка возможности любого хода для данной шашки
        Vector3 currentPosition = checker.transform.position;

        if (CanCaptureAgain(checker, currentPosition))
            return true;

        Vector3[] possibleCapturePositions = {
            new Vector3(currentPosition.x + 2, currentPosition.y, currentPosition.z + 2),
            new Vector3(currentPosition.x - 2, currentPosition.y, currentPosition.z + 2),
            new Vector3(currentPosition.x + 2, currentPosition.y, currentPosition.z - 2),
            new Vector3(currentPosition.x - 2, currentPosition.y, currentPosition.z - 2)
        };

        foreach (var targetPosition in possibleCapturePositions)
        {
            CheckerSelector enemyChecker;
            if (IsValidMove(checker, targetPosition, out enemyChecker))
            {
                return true;
            }
        }
        return false;
    }
}
