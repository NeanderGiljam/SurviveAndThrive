using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public interface IGameTimeListener {
    void UpdateGameTime(float globalGameTime, float deltaGameTime);
}

public delegate void OnGameStart();
public delegate void OnGamePause();
public delegate void OnGameResume();
public delegate void OnGameStop();

public class GameSpeedController : BaseController {

    public float deltaGameTime { get; set; }
    public float globalGameTime { get; set; }

    public float globalGameSpeed { get; set; }
    public bool isRunning { get; set; }

	public OnGameStart onGameStart;
	public OnGamePause onGamePause;
	public OnGameResume onGameResume;
	public OnGameStop onGameStop;

    private bool isFocused = true;
    private List<IGameTimeListener> listeners = new List<IGameTimeListener>();
    private List<bool> pauzed = new List<bool>();
    private List<Type> pauzedTypes = new List<Type>();

    public void Play(bool reInitListeners = true) {
        isRunning = true;
        globalGameSpeed = 1.0f;

        if (reInitListeners) {
            CollectAllGameTimeListeners();
        }

		if (onGameStart != null) {
			onGameStart();
		}
	}

    public override void Pause() {
        base.Pause();

		if (onGamePause != null) {
			onGamePause();
		}
    }

    public override void Resume() {
        base.Resume();

		if (onGameResume != null) {
			onGameResume();
		}
	}

    public void Stop() {
        isRunning = false;

		if (onGameStop != null) {
			onGameStop();
		}

		SaveManager.Instance.SaveGameState();
	}

    public void OnApplicationFocus(bool focusStatus) {
        isFocused = focusStatus;

        if (!focusStatus) {
            GameAccesPoint.Instance.managerSystem.Pause();
        } else {
            GameAccesPoint.Instance.managerSystem.Resume();
        }
    }

    public void SetGameSpeed(float value) {
        globalGameSpeed = value;
    }

    private void CollectAllGameTimeListeners() {
        MonoBehaviour[] allMonoBehaviours = GameObject.FindObjectsOfType<MonoBehaviour>();
        foreach (MonoBehaviour behaviour in allMonoBehaviours) {
            if (behaviour is IGameTimeListener) {
                AddGameTimeListener((IGameTimeListener)behaviour);
            }
        }
    }

    public void AddGameTimeListener(IGameTimeListener listener) {
        if (!listeners.Contains(listener)) {
            listeners.Add(listener);
            pauzed.Add(pauzedTypes.Contains(listener.GetType()));
        }
    }

    public void RemoveGameTimeListener(IGameTimeListener listener) {
        int index = listeners.IndexOf(listener);
        if (index != -1) {
            listeners.RemoveAt(index);
            pauzed.RemoveAt(index);
        }
    }

    private void Update() {
        if (!isRunning || !isFocused || isPaused) {
            return;
        }

        float delta = Time.smoothDeltaTime;
        deltaGameTime = delta * globalGameSpeed;
        globalGameTime += deltaGameTime;

        UpdateGameTimeListeners(globalGameTime, deltaGameTime);
    }

    private void UpdateGameTimeListeners(float globalGameTime, float deltaGameTime) {
        if (listeners == null || listeners.Count == 0) {
            return;
        }

        int i = listeners.Count;
        while (--i > -1) {
            IGameTimeListener listener = listeners[i];
            if (listener == null) {
                listeners.RemoveAt(i);
                pauzed.RemoveAt(i);
            } else {
                if (!pauzed[i]) {
                    listener.UpdateGameTime(globalGameTime, deltaGameTime);
                }
            }
        }
    }

    public bool PauzeGameTimeListenerByType<T>() where T : IGameTimeListener {
        return SetStateByType<T>(true);
    }

    private bool ResumeGameTimeListenerByType<T>() where T : IGameTimeListener {
        return SetStateByType<T>(false);
    }

    private bool SetStateByType<T>(bool state) {
        bool succes = false;
        int i = listeners.Count;
        Type iGameTimeType = typeof(T);

        while (--i > -1) {
            IGameTimeListener listener = listeners[i];
            if (listener != null) {
                Type type = listener.GetType();
                if (type.IsAssignableFrom(iGameTimeType)) {
                    pauzed[i] = state;
                    succes = true;

                    if (state && !pauzedTypes.Contains(type)) {
                        pauzedTypes.Add(type);
                    } else if (!state && pauzedTypes.Contains(type)) {
                        pauzedTypes.Remove(type);
                    }
                }
            }
        }
        return succes;
    }
}