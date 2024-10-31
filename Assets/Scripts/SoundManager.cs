using UnityEngine;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    [Header("Audio Sources")]
    public AudioSource backgroundMusic; // Фоновая музыка
    public AudioSource soundEffects;     // Звуковые эффекты

    [Header("Audio Toggles")]
    public Toggle musicToggle;           // Toggle для фоновой музыки
    public Toggle soundEffectsToggle;    // Toggle для звуковых эффектов

    [Header("Sound Effects")]
    public AudioClip gemSound;           // Звук при касании гема
    public AudioClip fireSound;          // Звук при касании огня

    private bool isMusicOn = true; // Статус фоновой музыки
    private bool areSoundEffectsOn = true; // Статус звуковых эффектов

    private void Start()
    {
        // Установите начальные состояния Toggle
        musicToggle.isOn = isMusicOn;
        soundEffectsToggle.isOn = areSoundEffectsOn;

        // Добавьте слушателей
        musicToggle.onValueChanged.AddListener(ToggleMusic);
        soundEffectsToggle.onValueChanged.AddListener(ToggleSoundEffects);

        // Обновите состояния звука
        UpdateAudioStates();
    }

    public void ToggleMusic(bool isOn)
    {
        isMusicOn = isOn; // Устанавливаем статус музыки
        backgroundMusic.mute = !isMusicOn; // Включаем/выключаем музыку
    }

    public void ToggleSoundEffects(bool isOn)
    {
        areSoundEffectsOn = isOn; // Устанавливаем статус звуковых эффектов
        soundEffects.mute = !areSoundEffectsOn; // Включаем/выключаем звуковые эффекты
    }

    private void UpdateAudioStates()
    {
        backgroundMusic.mute = !isMusicOn; // Применяем статус музыки
        soundEffects.mute = !areSoundEffectsOn; // Применяем статус звуковых эффектов
    }

    // Метод для воспроизведения звуковых эффектов
    public void PlayGemSound()
    {
        if (areSoundEffectsOn && gemSound != null)
        {
            soundEffects.PlayOneShot(gemSound);
        }
    }

    public void PlayFireSound()
    {
        if (areSoundEffectsOn && fireSound != null)
        {
            soundEffects.PlayOneShot(fireSound);
        }
    }

    // Метод для установки фона музыки
    public void SetBackgroundMusic(AudioClip clip)
    {
        backgroundMusic.clip = clip;
        backgroundMusic.Play();
    }
}
