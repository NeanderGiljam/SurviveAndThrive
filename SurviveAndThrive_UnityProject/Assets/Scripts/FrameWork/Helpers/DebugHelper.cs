using UnityEngine;

public class DebugHelper {
    public static bool debugMode = false;

    public static string playerCoords = "";
    public static string chunkCoords = "";
    public static string biomeType = "";
	public static string inputType = "";
    public static string fps = "";

    private static string[] debugData = new string[5];

	private static bool enabled = false;

    public static void SetDebugWindowData() {
        debugData[0] = playerCoords;
        debugData[1] = chunkCoords;
        debugData[2] = biomeType;
		debugData[3] = inputType;
        debugData[4] = fps;

		if (debugMode && enabled == true) {
			Color color = new Color(0.5f, 0.5f, 0.5f, 0.8f);
			Rect position = new Rect(0, 0, 300, 20 * debugData.Length + 15);

			Texture2D texture = new Texture2D(1, 1);
			texture.SetPixel(0, 0, color);
			texture.Apply();
			GUI.skin.box.normal.background = texture;
			GUI.Box(position, GUIContent.none);

			for (int i = 0; i < debugData.Length; i++) {
				GUI.Label(new Rect(10, 5 + (20 * i), 300, 20), debugData[i]);
			}
		}
    }

	public static void ToggleDebugMode() {
		debugMode = !debugMode;
	}
}