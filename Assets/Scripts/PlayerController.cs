using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 15f;
    private GameManager gameManager;
    private RectTransform canvasRectTransform;
    private SoundManager soundManager;
    public ScreenShake screenShake;

    private Vector2 startTouchPosition;
    private Vector2 endTouchPosition;
    private bool swipeDetected;

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        if (gameManager == null)
        {
            Debug.LogError("GameManager не найден! Убедитесь, что объект с GameManager добавлен на сцену.");
        }

        screenShake = FindObjectOfType<Canvas>().GetComponent<ScreenShake>();
        if (screenShake == null)
        {
            Debug.LogError("ScreenShake не найден на канвасе!");
        }

        canvasRectTransform = FindObjectOfType<Canvas>().GetComponent<RectTransform>();
        soundManager = FindObjectOfType<SoundManager>();
        if (soundManager == null)
        {
            Debug.LogError("SoundManager не найден! Убедитесь, что объект с SoundManager добавлен на сцену.");
        }
    }

    void Update()
    {
        float moveInput = Input.GetAxis("Horizontal");

        // Обработка свайпа
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            // Сохраняем начальную позицию касания
            if (touch.phase == TouchPhase.Began)
            {
                startTouchPosition = touch.position;
                swipeDetected = false;
            }
            // Определяем конец свайпа
            else if (touch.phase == TouchPhase.Ended)
            {
                endTouchPosition = touch.position;
                Vector2 swipeDirection = endTouchPosition - startTouchPosition;

                if (swipeDirection.magnitude > 50) // Минимальная длина для распознавания свайпа
                {
                    swipeDetected = true;
                    // Проверяем направление свайпа
                    if (swipeDirection.x < 0) // Свайп влево
                    {
                        moveInput = -1;
                    }
                    else if (swipeDirection.x > 0) // Свайп вправо
                    {
                        moveInput = 1;
                    }
                }
            }
        }

        Vector2 newPosition = (Vector2)transform.position + Vector2.right * moveInput * Time.deltaTime * moveSpeed;

        if (IsWithinCanvasBounds(newPosition))
        {
            transform.position = newPosition;
        }
    }

    private bool IsWithinCanvasBounds(Vector2 position)
    {
        Vector3[] canvasCorners = new Vector3[4];
        canvasRectTransform.GetWorldCorners(canvasCorners);

        float minX = canvasCorners[0].x;
        float maxX = canvasCorners[2].x;
        float minY = canvasCorners[0].y;
        float maxY = canvasCorners[2].y;

        return position.x > minX && position.x < maxX && position.y > minY && position.y < maxY;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Gem"))
        {
            if (gameManager != null)
            {
                gameManager.UpdateScore(10);
            }
            else
            {
                Debug.LogWarning("GameManager не найден. Очки не будут добавлены.");
            }

            if (soundManager != null)
            {
                soundManager.PlayGemSound();
            }

            Destroy(other.gameObject);
        }
        else if (other.CompareTag("Fire"))
        {
            if (gameManager != null)
            {
                gameManager.UpdateHealth(-10);
            }
            else
            {
                Debug.LogWarning("GameManager не найден. Здоровье не будет уменьшено.");
            }

            if (soundManager != null)
            {
                soundManager.PlayFireSound();
            }

            if (screenShake != null)
            {
                StartCoroutine(screenShake.Shake(0.5f, 0.1f));
            }

            Destroy(other.gameObject);
        }
    }
}
