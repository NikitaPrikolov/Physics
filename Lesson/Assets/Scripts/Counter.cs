using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Counter : MonoBehaviour
{
    private AppleControl appleControl;
    public InputField inputField; // Поле ввода для пользователя
    public GameObject correctAnswerPanel;

    private void Start()
    {
        appleControl = FindObjectOfType<AppleControl>(); // Поиск объекта с компонентом AppleControl в сцене

        if (appleControl != null)
        {
            appleControl.OnHeightChanged += CalculateVelocity; // Подписка на событие
        }
        else
        {
            Debug.LogError("AppleControl не найден в сцене!");
        }
    }
    private void CalculateVelocity(float height)
    {
        if (height > 0)
        {
            // Преобразуем высоту из сантиметров в метры
            float heightInMeters = height / 100f;

            // Вычисляем скорость падения
            float g = 9.81f; // Ускорение свободного падения
            float calculatedVelocity = Mathf.Sqrt(2 * g * heightInMeters);
            calculatedVelocity = Mathf.Round(calculatedVelocity * 100f) / 100f; // Округляем до двух знаков после запятой

            // Выводим правильный ответ в консоль
            Debug.Log("Правильная скорость: " + calculatedVelocity + " м/с");
            inputField.onValueChanged.RemoveAllListeners(); // Удаляем предыдущие слушатели
            inputField.onValueChanged.AddListener(delegate { CheckVelocity(calculatedVelocity); });
        }
    }
    private void CheckVelocity(float calculatedVelocity)
    {
        // Преобразуем ввод пользователя
        string userInput = inputField.text.Replace('.', ','); // Замена точки на запятую

        // Проверяем, является ли ввод числом
        if (float.TryParse(userInput, out float enteredVelocity))
        {
            // Сравниваем высчитанную скорость с вводом пользователя
            if (Mathf.Abs(calculatedVelocity - enteredVelocity) < 0.01f) // Сравнение с учетом погрешности
            {
                correctAnswerPanel.SetActive(true);
                Debug.Log("Скорость совпадает!");
            }
            else
            {
                Debug.Log("Скорость не совпадает. Введённая скорость: " + enteredVelocity);
            }
        }
        else
        {
            Debug.Log("Введено не число.");
        }
    }
}