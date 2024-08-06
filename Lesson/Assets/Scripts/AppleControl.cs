using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AppleControl : MonoBehaviour
{
    public GameObject theoryPanel;
    public event Action<float> OnHeightChanged;
    private Counter counter;
    private Vector3 mouseOffset;
    private bool isDragging = false;
    private Rigidbody rb;
    public Animator theoryAnimator;
    private Animator animator;
    public Animator canvasAnimator;

    // ������ �� ��������� ����� ��� ����������� ������ � �������
    public Text heightText; // ��� ����������� ������� ������
    public Text releasedHeightText; // ��� ����������� ������ ��� ���������� ������ ����

    // ������� �����������
    public float minX = -5f;  // ����������� �������� �� X
    public float maxX = 5f;   // ������������ �������� �� X
    public float minY = 2.5529f; // ����������� �������� �� Y (���� 0 ��)
    public float maxY = 6f;       // ������������ �������� �� Y (���� 50 ��)
    public float minZ = -5f;  // ����������� �������� �� Z
    public float maxZ = 5f;   // ������������ �������� �� Z

    // ��� ����������� ������
    public float minCustomHeight = 0f; // ��� 0 ��
    public float maxCustomHeight = 50f; // ��� 50 ��
    public float releasedHeight = 0f; // ������ ��� ���������� ������

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        rb.isKinematic = true;
        animator.enabled = false;
        canvasAnimator.enabled = false;

        // ���������� ��������� ������
        UpdateHeightText(CustomHeight(2.5529f)); // ��������� �������� �� ��� Y
        releasedHeightText.text = "������: "; // ������������� ������
    }

    void OnMouseDown()
    {
        if (theoryPanel.activeSelf) return; // ���� ������ �������, �� ��������� ��������������

        isDragging = true;
        mouseOffset = Input.mousePosition - Camera.main.WorldToScreenPoint(transform.position);
    }


    void OnMouseDrag()
    {
        if (isDragging)
        {
            canvasAnimator.enabled = true;
            animator.enabled = true;
            Vector3 newPosition = Input.mousePosition - mouseOffset;
            newPosition.z = Camera.main.WorldToScreenPoint(transform.position).z; // ��������� ���������� �� ������
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(newPosition);

            // ����������� ����������� � �������� ��������
            worldPosition.x = Mathf.Clamp(worldPosition.x, minX, maxX);
            worldPosition.y = Mathf.Clamp(worldPosition.y, minY, maxY);
            worldPosition.z = Mathf.Clamp(worldPosition.z, minZ, maxZ);
            transform.position = worldPosition;
            UpdateHeightText(CustomHeight(worldPosition.y)); // �������������� ������
        }
    }

    void OnMouseUp()
    {
        isDragging = false;
        // ��������� ������� �������, �� �� ������ �
        Vector3 currentPosition = transform.position;
        // ��������� ������, ���� ��� ������ 0.5 ��
        float heightAtRelease = CustomHeight(currentPosition.y);
        if (heightAtRelease > 0.5f)
        {
            releasedHeight = heightAtRelease;
            releasedHeightText.text = "������: " + releasedHeight.ToString("F2") + " ��"; // ��������� �����
        }
        else
        {
            releasedHeightText.text = "������: ������ 0.5 ��"; // � ������, ���� ������ ������ 0.5 ��
        }
        // ��������� �������� ��� ��������� isKinematic
        StartCoroutine(ReleaseAppleAfterDelay(1)); // 1 ���� ��������
        OnHeightChanged?.Invoke(releasedHeight);
    }

    IEnumerator ReleaseAppleAfterDelay(int frames)
    {
        // ���� ��������� ���������� ������
        for (int i = 0; i < frames; i++)
        {
            yield return null;
        }

        // ��������� ������ �������������� �� ������
        rb.isKinematic = false;

        // ������������� ��������� ��������� �������� ����
        rb.velocity = new Vector3(0, -4.1f, 0);
    }
    private void Update()
    {
        if (isDragging)
        {
            canvasAnimator.Play("Hider");
            animator.Play("In");
        }
        else
        {
            canvasAnimator.Play("Exposer");
            animator.Play("Out");
        }
    }
    // ����� ��� �������������� ������ �� Unity � ��������� ��� ������� ������� ���������, � ������ - ����������
    private float CustomHeight(float unityHeight)
    {
        // �������� ������������ ��� �������������� ������
        return Mathf.Lerp(minCustomHeight, maxCustomHeight, (unityHeight - minY) / (maxY - minY));
    }

    // ����� ��� ���������� ������ ������
    private void UpdateHeightText(float height)
    {
        heightText.text = "������: " + height.ToString("F2") + " ��"; // ����������� � ����� ������� ����� �������
    }
    public void ToggleActiveState()
    {
        if (theoryPanel != null && theoryAnimator != null)
        {
            // ��������� ������� ��������� ���������� ������
            bool isActive = theoryPanel.activeSelf;

            // �������� ��� ��������� �������� � ����������� �� ���������
            if (isActive)
            {
                // ���� ������ �������, ��������� �������� ������������
                theoryAnimator.Play("TheoryOut");
            }
            else
            {
                // ���� ������ �� �������, �������� �� � ��������� �������� ����������
                theoryPanel.SetActive(true);
                theoryAnimator.Play("TheoryIn");
            }
        }
    }
}