using UnityEngine;
using System.Collections;

public abstract class BaseManager : MonoBehaviour, IPausable {

    protected bool isPaused = false;

	public abstract void Initialize();

    public virtual void Pause() {
        isPaused = true;
    }

    public virtual void Resume() {
        isPaused = false;
    }

    public bool IsPaused {
        get { return isPaused; }
    }

    protected virtual void OnLevelWasLoaded() {

    }
}
