using UnityEngine;
using System.Collections;

public class BaseController : MonoBehaviour, IPausable, IGameTimeListener {

    protected bool isPaused = false;
    protected bool isInit = false;

    public bool IsPaused { get { return isPaused; } }

    public virtual void Initialize() {
        GameAccesPoint.Instance.mainGameState._gameSpeedController.AddGameTimeListener(this as IGameTimeListener);
    }

    public virtual void Pause() {
        isPaused = true;
        OnPause();
    }

    public virtual void Resume() {
        isPaused = false;
        OnResume();
    }

    protected virtual void OnPause() {

    }

    protected virtual void OnResume() {

    }

    public virtual void UpdateGameTime(float globalGameTime, float deltaGameTime) {
        
    }
}
