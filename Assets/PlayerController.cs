using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 5.0f;  // Скорость перемещения персонажа
    public float rotationSpeed = 100.0f;  // Скорость поворота

    private Rigidbody rb;  // Ссылка на компонент Rigidbody
    private Animator animator;  // Ссылка на компонент Animator

    void Start()
    {
        // Получаем компонент Rigidbody прикреплённый к этому объекту
        rb = GetComponent<Rigidbody>();

        // Получаем компонент Animator прикреплённый к этому объекту
        animator = GetComponent<Animator>();
    }

    void FixedUpdate()
    {
        // Ввод от пользователя
        float moveHorizontal = Input.GetAxis("Horizontal");  // Получаем горизонтальный ввод (A/D, Left/Right)
        float moveVertical = Input.GetAxis("Vertical");  // Получаем вертикальный ввод (W/S, Up/Down)

        // Вычисляем вектор движения
        Vector3 movement = transform.forward * moveVertical + transform.right * moveHorizontal;

        // Применяем движение к Rigidbody
        rb.MovePosition(rb.position + movement * speed * Time.fixedDeltaTime);

        // Вычисляем поворот
        float rotation = moveHorizontal * rotationSpeed * Time.fixedDeltaTime;

        // Применяем поворот к Rigidbody
        Quaternion deltaRotation = Quaternion.Euler(0f, rotation, 0f);
        rb.MoveRotation(rb.rotation * deltaRotation);

        // Рассчитываем текущую скорость персонажа
        float currentSpeed = movement.magnitude * speed;

        // Обновляем параметр "Speed" в Animator
        animator.SetFloat("Speed", currentSpeed);
    }
}