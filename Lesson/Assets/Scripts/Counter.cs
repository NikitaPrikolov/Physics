using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Counter : MonoBehaviour
{
    private AppleControl appleControl;
    public InputField inputField; // ���� ����� ��� ������������
    public GameObject correctAnswerPanel;

    private void Start()
    {
        appleControl = FindObjectOfType<AppleControl>(); // ����� ������� � ����������� AppleControl � �����

        if (appleControl != null)
        {
            appleControl.OnHeightChanged += CalculateVelocity; // �������� �� �������
        }
        else
        {
            Debug.LogError("AppleControl �� ������ � �����!");
        }
    }
    private void CalculateVelocity(float height)
    {
        if (height > 0)
        {
            // ����������� ������ �� ����������� � �����
            float heightInMeters = height / 100f;

            // ��������� �������� �������
            float g = 9.81f; // ��������� ���������� �������
            float calculatedVelocity = Mathf.Sqrt(2 * g * heightInMeters);
            calculatedVelocity = Mathf.Round(calculatedVelocity * 100f) / 100f; // ��������� �� ���� ������ ����� �������

            // ������� ���������� ����� � �������
            Debug.Log("���������� ��������: " + calculatedVelocity + " �/�");
            inputField.onValueChanged.RemoveAllListeners(); // ������� ���������� ���������
            inputField.onValueChanged.AddListener(delegate { CheckVelocity(calculatedVelocity); });
        }
    }
    private void CheckVelocity(float calculatedVelocity)
    {
        // ����������� ���� ������������
        string userInput = inputField.text.Replace('.', ','); // ������ ����� �� �������

        // ���������, �������� �� ���� ������
        if (float.TryParse(userInput, out float enteredVelocity))
        {
            // ���������� ����������� �������� � ������ ������������
            if (Mathf.Abs(calculatedVelocity - enteredVelocity) < 0.01f) // ��������� � ������ �����������
            {
                correctAnswerPanel.SetActive(true);
                Debug.Log("�������� ���������!");
            }
            else
            {
                Debug.Log("�������� �� ���������. �������� ��������: " + enteredVelocity);
            }
        }
        else
        {
            Debug.Log("������� �� �����.");
        }
    }
}