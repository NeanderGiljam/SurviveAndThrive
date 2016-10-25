using UnityEngine;
using System.Collections.Generic;

public class AudioController : BaseController {

	//private Vector2 chunkSize;
	//private GameObject ambientAudioContainer;

	//private Dictionary<Vector2, GameObject> ambientAudio = new Dictionary<Vector2, GameObject>();

    public override void Initialize() {
		base.Initialize();

        GameAccesPoint.Instance.audioController = this;

		//chunkSize = GameAccesPoint.Instance.mainGameState._worldController._chunkSize;
		//ambientAudioContainer = (GameObject)Resources.Load("Prefabs/Other/Ambient_Container");

		isInit = true;
    }

	public void PlayGenericButtonAudio() {
        AudioManager.Instance.PlayByName("button_pressed");
    }

	//public void HandleAmbientAudioState(Vector2 chunkPos, bool state = true) {
	//	if (!ambientAudio.ContainsKey(chunkPos)) {
	//		GameObject newAudioContainer = (GameObject)Instantiate(ambientAudioContainer, new Vector3(chunkPos.x * chunkSize.x, 0, chunkPos.y * chunkSize.y), Quaternion.identity);
	//		newAudioContainer.transform.SetParent(transform);
	//		ambientAudio.Add(chunkPos, newAudioContainer);
	//		return;
	//	}

	//	foreach (KeyValuePair<Vector2, GameObject> pair in ambientAudio) {
	//		if (pair.Key == chunkPos) {
	//			pair.Value.SetActive(state);
	//		}
	//	}
	//}
}
