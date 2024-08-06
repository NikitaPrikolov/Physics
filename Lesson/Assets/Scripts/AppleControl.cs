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

    // Ссылки на текстовые блоки для отображения высоты и времени
    public Text heightText; // Для отображения текущей высоты
    public Text releasedHeightText; // Для сохраненной высоты при отпускании кнопки мыши

    // Границы перемещения
    public float minX = -5f;  // Минимальное значение по X
    public float maxX = 5f;   // Максимальное значение по X
    public float minY = 2.5529f; // Минимальное значение по Y (ваше 0 см)
    public float maxY = 6f;       // Максимальное значение по Y (ваши 50 см)
    public float minZ = -5f;  // Минимальное значение по Z
    public float maxZ = 5f;   // Максимальное значение по Z

    // Для отображения высоты
    public float minCustomHeight = 0f; // Это 0 см
    public float maxCustomHeight = 50f; // Это 50 см
    public float releasedHeight = 0f; // Высота при отпускании кнопки

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        rb.isKinematic = true;
        animator.enabled = false;
        canvasAnimator.enabled = false;

        // Отображаем начальную высоту
        UpdateHeightText(CustomHeight(2.5529f)); // Начальное значение по оси Y
        releasedHeightText.text = "Высота: "; // Инициализация текста
    }

    void OnMouseDown()
    {
        if (theoryPanel.activeSelf) return; // Если панель активна, не позволять перетаскивание

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
            newPosition.z = Camera.main.WorldToScreenPoint(transform.position).z; // Удержание расстояния от камеры
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(newPosition);

            // Ограничение перемещения в заданных границах
            worldPosition.x = Mathf.Clamp(worldPosition.x, minX, maxX);
            worldPosition.y = Mathf.Clamp(worldPosition.y, minY, maxY);
            worldPosition.z = Mathf.Clamp(worldPosition.z, minZ, maxZ);
            transform.position = worldPosition;
            UpdateHeightText(CustomHeight(worldPosition.y)); // Преобразование высоты
        }
    }

    void OnMouseUp()
    {
        isDragging = false;
        // Сохраняем текущую позицию, но не меняем её
        Vector3 currentPosition = transform.position;
        // Сохраняем высоту, если она больше 0.5 см
        float heightAtRelease = CustomHeight(currentPosition.y);
        if (heightAtRelease > 0.5f)
        {
            releasedHeight = heightAtRelease;
            releasedHeightText.text = "Высота: " + releasedHeight.ToString("F2") + " см"; // Обновляем текст
        }
        else
        {
            releasedHeightText.text = "Высота: меньше 0.5 см"; // В случае, если высота меньше 0.5 см
        }
        // Запускаем корутину для изменения isKinematic
        StartCoroutine(ReleaseAppleAfterDelay(1)); // 1 кадр задержки
        OnHeightChanged?.Invoke(releasedHeight);
    }

    IEnumerator ReleaseAppleAfterDelay(int frames)
    {
        // Ждем указанное количество кадров
        for (int i = 0; i < frames; i++)
        {
            yield return null;
        }

        // Позволяем физике воздействовать на объект
        rb.isKinematic = false;

        // Устанавливаем небольшую начальную скорость вниз
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
    // Метод для преобразования высоты из Unity в локальные для проекта единицы измерения, а именно - сантиметры
    private float CustomHeight(float unityHeight)
    {
        // Линейная интерполяция для преобразования высоты
        return Mathf.Lerp(minCustomHeight, maxCustomHeight, (unityHeight - minY) / (maxY - minY));
    }

    // Метод для обновления текста высоты
    private void UpdateHeightText(float height)
    {
        heightText.text = "Высота: " + height.ToString("F2") + " см"; // Форматируем с двумя знаками после запятой
    }
    public void ToggleActiveState()
    {
        if (theoryPanel != null && theoryAnimator != null)
        {
            // Проверяем текущее состояние активности панели
            bool isActive = theoryPanel.activeSelf;

            // Включаем или выключаем анимацию в зависимости от состояния
            if (isActive)
            {
                // Если панель активна, запускаем анимацию исчезновения
                theoryAnimator.Play("TheoryOut");
            }
            else
            {
                // Если панель не активна, включаем ее и запускаем анимацию проявления
                theoryPanel.SetActive(true);
                theoryAnimator.Play("TheoryIn");
            }
        }
    }
}