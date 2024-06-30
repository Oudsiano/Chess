using System.Collections.Generic;
using UnityEngine;

public class Field : MonoBehaviour
{
    void OnMouseDown()
    {
        Debug.Log($"Поле нажато: {transform.position} " + gameObject.name);
        CheckerSelector selectedChecker = CheckerSelector.GetSelectedChecker();


        if (selectedChecker != null)
        {
            Debug.Log(Vector3.Distance(selectedChecker.transform.position, transform.position));
            //Debug.Log($"Поле нажато: {transform.position}" + gameObject.name);
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
                        CheckerSelector.ToggleTurn(this); // Переключаем ход после взятия, если нет возможности еще одного взятия
                    }
                }
                else
                {
                    Debug.Log("Переключение хода после обычного хода.");
                    CheckerSelector.ToggleTurn(this); // Переключаем ход после обычного хода
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

        //out board
        if (targetPosition.x < -5 || targetPosition.x > 10 || targetPosition.z > 17 || targetPosition.z < 2)
            return false;

        if (!IsFieldOccupied(targetPosition))
        {
            float deltaX = Mathf.Abs(targetPosition.x - checker.transform.position.x);
            float deltaZ = targetPosition.z - checker.transform.position.z;

            //Debug.Log($"Проверка допустимости хода: deltaX = {deltaX}, deltaZ = {deltaZ}");

            if (checker.isKing)
            {
                bool _alreadyFindEnemy = false;

                if (Mathf.Abs(deltaX) - Mathf.Abs(deltaZ) < 0.5f) //"fix" for float mistakes
                {
                    //Надо собрать состояния всех ячеек по пути.
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
            if (Vector3.Distance(checker.transform.position, targetPosition) < 0.7f)
            {
                //Debug.Log("Поле занято: " + checker.gameObject.name);
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
            if (Vector3.Distance(checker.transform.position, position) < 0.7f)
            {
                //Debug.Log($"Шашка найдена на позиции: {position}, имя: {checker.gameObject.name}");
                return checker;
            }
        }
        return null;
    }

    public bool CanCaptureAgain(CheckerSelector checker, Vector3 currentPosition)
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
                Debug.Log($"Есть возможность еще одного взятия: {targetPosition} {enemyChecker.name}");
                return true;
            }
        }
        //Debug.Log("Нет возможности еще одного взятия.");
        return false;
    }
    public bool CanAnyMove(CheckerSelector checker)
    {
        Vector3 currentPosition = checker.transform.position;

        //Первая проверка. Уже ее достаточно.
        if (CanCaptureAgain(checker, currentPosition)) 
            return true;
        //Но если не прошло, то код пойдет дальше

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
