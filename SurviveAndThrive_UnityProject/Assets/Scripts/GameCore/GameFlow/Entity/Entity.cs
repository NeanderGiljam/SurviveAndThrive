using UnityEngine;
using System.Collections;

public class Entity : MonoBehaviour {

	public GameItemDatabase gameItemDatabase { get; protected set; }

    public virtual void Initialize() {
		gameItemDatabase = GameAccesPoint.Instance.mainGameState._gameItemDatabase;
    }

    public virtual void Destroy(float time = 0) {
        Destroy(gameObject, time);
    }

    public virtual void UpdateGameTime() {
        
    }
}