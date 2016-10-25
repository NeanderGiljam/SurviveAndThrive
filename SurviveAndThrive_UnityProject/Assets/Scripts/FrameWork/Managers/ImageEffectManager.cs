using UnityEngine;
using UnityEngine.UI;

public delegate void OnScreenFadedHandler();

public class ImageEffectManager : BaseManager {
	public event OnScreenFadedHandler OnScreenFaded;

	// --------------- Singleton Pattern ---------------
	private static ImageEffectManager instance = null;

	public static ImageEffectManager Instance {
		get {
			if (instance == null) {
				instance = Object.FindObjectOfType(typeof(ImageEffectManager)) as ImageEffectManager;

				if (instance == null) {
					GameObject go = new GameObject("Managers");
					DontDestroyOnLoad(go);
					instance = go.AddComponent<ImageEffectManager>();
				}
				DontDestroyOnLoad(instance.gameObject);
			}
			return instance;
		}
	}
	// --------------- Singleton Pattern ---------------

	private float alphaFadeValue = 0;
	private float fadeSpeed = 1;
	private bool fade = false;

	private Image fadeScreen;
	private Texture2D blackTexture;
	private Direction currentDir;

	public override void Initialize() {
		GameAccesPoint.Instance.managerSystem.imageEffectManager = this;
    }

	public void FadeScreen(Direction dir, float speed = 1f) {
		if (fadeScreen == null) {
			GameObject fadeScreenObj = GameObject.Find("r_FadeScreen");
			if (fadeScreenObj != null) {
				fadeScreen = fadeScreenObj.GetComponent<Image>();
			}
		}

		alphaFadeValue = dir == Direction.In ? 1 : 0;

		currentDir = dir;
		fadeSpeed = speed;
		fade = true;
	}

	private void Update() {
		if (!fade) {
			return;
		}

		if (currentDir == Direction.Out) {
			alphaFadeValue += Mathf.Clamp01(fadeSpeed * Time.deltaTime);
		} else {
			alphaFadeValue -= Mathf.Clamp01(fadeSpeed * Time.deltaTime);
		}

		if (fadeScreen != null) {
			fadeScreen.color = new Color(fadeScreen.color.r, fadeScreen.color.g, fadeScreen.color.b, alphaFadeValue);
		}

		if (alphaFadeValue <= 0 && currentDir == Direction.In || alphaFadeValue >= 1 && currentDir == Direction.Out) {
			if (OnScreenFaded != null) {
				OnScreenFaded();
			}
			fade = false;
		}
	}
}