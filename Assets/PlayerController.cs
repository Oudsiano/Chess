using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 5.0f;  // �������� ����������� ���������
    public float rotationSpeed = 100.0f;  // �������� ��������

    private Rigidbody rb;  // ������ �� ��������� Rigidbody
    private Animator animator;  // ������ �� ��������� Animator

    void Start()
    {
        // �������� ��������� Rigidbody ������������ � ����� �������
        rb = GetComponent<Rigidbody>();

        // �������� ��������� Animator ������������ � ����� �������
        animator = GetComponent<Animator>();
    }

    void FixedUpdate()
    {
        // ���� �� ������������
        float moveHorizontal = Input.GetAxis("Horizontal");  // �������� �������������� ���� (A/D, Left/Right)
        float moveVertical = Input.GetAxis("Vertical");  // �������� ������������ ���� (W/S, Up/Down)

        // ��������� ������ ��������
        Vector3 movement = transform.forward * moveVertical + transform.right * moveHorizontal;

        // ��������� �������� � Rigidbody
        rb.MovePosition(rb.position + movement * speed * Time.fixedDeltaTime);

        // ��������� �������
        float rotation = moveHorizontal * rotationSpeed * Time.fixedDeltaTime;

        // ��������� ������� � Rigidbody
        Quaternion deltaRotation = Quaternion.Euler(0f, rotation, 0f);
        rb.MoveRotation(rb.rotation * deltaRotation);

        // ������������ ������� �������� ���������
        float currentSpeed = movement.magnitude * speed;

        // ��������� �������� "Speed" � Animator
        animator.SetFloat("Speed", currentSpeed);
    }
}