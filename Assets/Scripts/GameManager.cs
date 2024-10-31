using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    [Header("Player Settings")]
    public int health = 100;
    public int score = 0;
    public int maxScore = 100;

    [Header("Game Objects")]
    public GameObject gemPrefab;
    public GameObject firePrefab;
    public GameObject player;
    public Canvas canvas;
    public GameObject losePanel;
    public GameObject winPanel;
    public GameObject gameObjectToActivate;
    public GameObject optionsMenu; // Меню опций
    public GameObject mainMenu;

    [Header("UI Elements")]
    public TMP_Text levelText;
    public TMP_Text healthText;
    public TMP_Text scoreText;
    public Slider timerSlider;

    [Header("Lose/Win Panels UI Elements")]
    public TMP_Text loseHealthText;
    public TMP_Text loseScoreText;
    public TMP_Text loseLevelText;
    public TMP_Text winHealthText;
    public TMP_Text winScoreText;
    public TMP_Text winLevelText;
    public Slider winTimerSlider;
    public Slider loseTimerSlider;
    [Header("Win Panel Buttons")]

    public Button nextLevelButton; // Ссылка на кнопку "Next"

    [Header("Level Buttons")]
    public Button[] levelButtons;

    [Header("Bonuses")]
    public bool isDoubleTimeActive = false; // Бонус Х2 время
    public bool isDoubleHPActive = false;   // Бонус Х2 здоровье
    public bool isDoublePointsActive = false; // Бонус Х2 очки


    [Header("Game Settings")]
    public TMP_Text optionsMenuTitle; // Ссылка на текст заголовка в меню опций
    public float spawnInterval = 1f;
    public float spawnRangeX = 500f;
    public float gameTime = 10f;
    private float timeRemaining = 10f; // Пример: 60 секунд на уровень

    private int currentLevel = 0;
    private const int totalLevels = 15;

    private bool isPaused = false; // Переменная для отслеживания состояния паузы

    void Start()
    {
        InitializeLevel();
        InitializeLevelButtons();
    }

    void Update()
{
    if (!isPaused)
    {
        if (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
            timerSlider.value = timeRemaining;

            // Проверка на истечение времени
            if (timeRemaining <= 0)
            {
                EndGame("Time's up!"); // Обработка поражения из-за времени
            }
        }
    }
}

    void InitializeLevel()
    {
        if (gameObjectToActivate.activeSelf)
        {
            levelText.text = $"Level {currentLevel + 1}";

            health = isDoubleHPActive ? 200 : 100;
            timeRemaining = isDoubleTimeActive ? gameTime + 60 : gameTime;

            timerSlider.maxValue = timeRemaining;
            timerSlider.value = timeRemaining;

            UpdateHealthText();
            UpdateScoreText();

            InvokeRepeating("SpawnObject", spawnInterval, spawnInterval);
        }
    }
// Метод для переключения состояния X2 времени
    public void ToggleDoubleTime()
    {
        isDoubleTimeActive = !isDoubleTimeActive;
        Debug.Log("Double Time Bonus: " + (isDoubleTimeActive ? "Enabled" : "Disabled"));
    }

    // Метод для переключения состояния X2 здоровья
    public void ToggleDoubleHP()
    {
        isDoubleHPActive = !isDoubleHPActive;
        Debug.Log("Double HP Bonus: " + (isDoubleHPActive ? "Enabled" : "Disabled"));
    }

    // Метод для переключения состояния X2 очков
    public void ToggleDoublePoints()
    {
        isDoublePointsActive = !isDoublePointsActive;
        Debug.Log("Double Points Bonus: " + (isDoublePointsActive ? "Enabled" : "Disabled"));
    }

    void InitializeLevelButtons()
    {
        for (int i = 0; i < levelButtons.Length; i++)
        {
            int levelIndex = i;
            levelButtons[i].interactable = (i == 0); // Доступна только кнопка первого уровня
            levelButtons[i].onClick.AddListener(() => OnLevelButtonClicked(levelIndex));
        }
    }

    void OnLevelButtonClicked(int levelIndex)
    {
        if (levelIndex <= currentLevel) // Можно выбирать уровни только до текущего включительно
        {
            currentLevel = levelIndex; // Обновляем текущий уровень
            gameObjectToActivate.SetActive(true);
            InitializeLevel();
        }
    }

void SpawnObject()
{
    RectTransform canvasRect = canvas.GetComponent<RectTransform>();
    float padding = 10f;
    float randomX = Random.Range(-canvasRect.rect.width / 2 + padding, canvasRect.rect.width / 2 - padding);
    float spawnY = canvasRect.rect.height / 2 + 10;
    Vector2 spawnPosition = new Vector2(randomX, spawnY);
    GameObject objectToSpawn = Random.value < 0.7f ? gemPrefab : firePrefab;
    GameObject spawnedObject = Instantiate(objectToSpawn, canvas.transform);
    
    // Убедитесь, что тег установлен правильно
    spawnedObject.tag = objectToSpawn.CompareTag("Fire") ? "Fire" : "Gem"; 

    RectTransform rectTransform = spawnedObject.GetComponent<RectTransform>();
    rectTransform.anchoredPosition = spawnPosition;

    Debug.Log("Spawned object: " + spawnedObject.name); // Лог для проверки
}


    public void OnPlayerTouchFire()
    {
        UpdateHealth(-15);
    }

    public void UpdateHealth(int amount)
    {
        health += amount;
        UpdateHealthText();

        if (health <= 0)
        {
            ShowLosePanel();
        }
    }

    public void UpdateScore(int amount)
{
    score += isDoublePointsActive ? amount * 2 : amount;
    UpdateScoreText();

    if (score >= maxScore)
    {
        ShowWinPanel();
    }
}


    void UpdateHealthText()
    {
        healthText.text = $"{health} HP";
    }

    void UpdateScoreText()
    {
        scoreText.text = $"{score:D3}/{maxScore}";
    }

    void ShowLosePanel()
    {
        losePanel.SetActive(true);
        UpdateLosePanelText();
        DestroySpawnedObjects();
        CancelInvoke("SpawnObject");
        UpdateLevelButtons(); // Обновляем состояние кнопок
    }

void ShowWinPanel()
{
    winPanel.SetActive(true);
    UpdateWinPanelText();
    DestroySpawnedObjects();
    CancelInvoke("SpawnObject");
    UpdateLevelButtons(); // Обновляем состояние кнопок

    // Скрываем кнопку "Next", если это последний уровень
    if (currentLevel >= totalLevels - 1)
    {
        nextLevelButton.gameObject.SetActive(false); // Скрываем кнопку "Next"
    }
    else
    {
        nextLevelButton.gameObject.SetActive(true); // Показываем кнопку "Next"
    }
}


    void UpdateWinPanelText()
    {
        winLevelText.text = $"Level {currentLevel + 1} win!";
        winHealthText.text = $"{health} HP";
        winScoreText.text = $"{score}/{maxScore}";

        if (winTimerSlider != null)
            winTimerSlider.value = timeRemaining;
    }

    void UpdateLosePanelText()
    {
        loseLevelText.text = $"Level {currentLevel + 1} lost!";
        loseHealthText.text = $"{health} HP";
        loseScoreText.text = $"{score}/{maxScore}";

        if (loseTimerSlider != null)
            loseTimerSlider.value = timeRemaining;
    }

    public void OnRetryButtonClicked()
    {
        ResetLevel();
        losePanel.SetActive(false);
        InitializeLevel();
    }

    public void OnNextLevelButtonClicked()
    {
        if (currentLevel < totalLevels - 1)
        {
            currentLevel++;
            ResetLevel();
            winPanel.SetActive(false);
            InitializeLevel();

            if (currentLevel < levelButtons.Length)
            {
                levelButtons[currentLevel].interactable = true;
            }
        }
    }

void ResetLevel()
{
    // Сбрасываем здоровье и счет
    health = 100;
    score = 0;

    // Сбрасываем время
    timeRemaining = gameTime;

    // Обновляем текст на экране
    UpdateHealthText();
    UpdateScoreText();
    timerSlider.value = timeRemaining;

    // Останавливаем спавн объектов
    CancelInvoke("SpawnObject");
    
    // Уничтожаем все спавненные объекты
    DestroySpawnedObjects();
}


    void EndGame(string message)
{
    Debug.Log("Game Over: " + message);
    ShowLosePanel(); // Отображаем панель проигрыша
    CancelInvoke("SpawnObject"); // Останавливаем спавн объектов (если у вас есть такой метод)
}


void DestroySpawnedObjects()
{
    // Уничтожаем объекты с тегом "Fire"
    GameObject[] fireObjects = GameObject.FindGameObjectsWithTag("Fire");
    foreach (GameObject obj in fireObjects)
    {
        Destroy(obj);
        Debug.Log("Destroyed Fire object: " + obj.name);
    }

    // Уничтожаем объекты с тегом "Gem"
    GameObject[] gemObjects = GameObject.FindGameObjectsWithTag("Gem");
    foreach (GameObject obj in gemObjects)
    {
        Destroy(obj);
        Debug.Log("Destroyed Gem object: " + obj.name);
    }
}

    public void ToggleOptionsMenu(bool isActive)
    {
        optionsMenu.SetActive(isActive);
        Time.timeScale = isActive ? 0 : 1; // Остановить или возобновить время
        isPaused = isActive; // Установить состояние паузы

        // Изменяем текст заголовка в зависимости от контекста
        if (isActive)
        {
            if (gameObjectToActivate.activeSelf) // Если в игре
            {
                optionsMenuTitle.text = "Pause"; // Меняем на "Pause"
            }
            else // Если в главном меню
            {
                optionsMenuTitle.text = "Options"; // Меняем на "Options"
            }
        }

        if (isActive) // Если меню опций открыто
        {
            CancelInvoke("SpawnObject"); // Остановить спавн объектов
        }
        else // Если меню опций закрыто
        {
            if (!isPaused && gameObjectToActivate.activeSelf) // Проверяем, не находимся ли мы в игре и объект активен
            {
                InvokeRepeating("SpawnObject", spawnInterval, spawnInterval); // Возобновляем спавн объектов
            }
        }
    }




 public void GoToMenu()
    {
        // Сначала останавливаем спавн
        CancelInvoke("SpawnObject");

        // Уничтожаем все объекты перед активацией главного меню
        DestroySpawnedObjects();

        // Переключаем на главное меню
        mainMenu.SetActive(true);
        gameObjectToActivate.SetActive(false);
        ResetLevel();
        
        // Устанавливаем заголовок меню опций в главное меню
        optionsMenuTitle.text = "Options"; // Убедитесь, что заголовок в главном меню правильный
    }


// Метод для выхода из игры
    public void Quit()
    {
        #if UNITY_EDITOR
            // Если в редакторе, останавливаем игру
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            // Если в сборке, выходим из приложения
            Application.Quit();
        #endif
    }

    // Метод для возврата из меню опций
    public void OnBackButtonClicked()
    {
        ToggleOptionsMenu(false); // Закрыть меню опций
    }

    void UpdateLevelButtons()
    {
        for (int i = 0; i < levelButtons.Length; i++)
        {
            levelButtons[i].interactable = (i <= currentLevel); // Доступны все уровни до текущего включительно
        }
    }
} 
