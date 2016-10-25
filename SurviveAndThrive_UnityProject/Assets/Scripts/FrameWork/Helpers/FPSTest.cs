using UnityEngine;

public class FPSTest : MonoBehaviour {

	private float deltaTime = 0.0f;

	private void Update() {
		deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
	}

	private void OnGUI() {
		int w = Screen.width, h = Screen.height;

		GUIStyle style = new GUIStyle();

		Rect rect = new Rect(0, 0, w, h * 2 / 100);
		style.alignment = TextAnchor.UpperLeft;
		style.fontSize = h * 2 / 100;
		style.normal.textColor = new Color(0.0f, 0.0f, 0.5f, 1.0f);
		float msec = deltaTime * 1000.0f;
		float fps = 1.0f / deltaTime;
		string text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);
		GUI.Label(rect, text, style);
	}

	//public  float updateInterval = 0.5F;

	//private float accum   = 0; // FPS accumulated over the interval
	//private int   frames  = 0; // Frames drawn over the interval
	//private float timeleft; // Left time for current interval

	//   private string fpsText;

	//   private void Awake() {
	//       timeleft = updateInterval;
	//       DontDestroyOnLoad(this);
	//   }

	//   private void OnGUI() {
	//       //GUI.Label(new Rect(Screen.width - 120, 5, 250, 20), "FPS: " + fpsText);
	//       DebugHelper.fps = "FPS: " + fpsText;
	//   }

	//private void Update() {
	//	timeleft -= Time.deltaTime;
	//	accum += Time.timeScale/Time.deltaTime;
	//	++frames;

	//	if( timeleft <= 0.0 ) {
	//		float fps = accum/frames;
	//		string format = string.Format("{0:F4}", fps);
	//		fpsText = format;

	//		timeleft = updateInterval;
	//		accum = 0.0F;
	//		frames = 0;
	//	}
	//}
}
