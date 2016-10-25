using UnityEngine;

public class PixelateEffect : MonoBehaviour {

	public float pixelDensity = 3;
	public float colorOffset = 5;

	private Material mat;

	void Start() {
		mat = new Material(Shader.Find("Hidden/Pixel_Shader"));
		mat.SetFloat("_height", Screen.height);
		mat.SetFloat("_width", Screen.width);
		mat.SetFloat("_pixelDensity", pixelDensity);
		mat.SetFloat("_colorOffset", colorOffset);
	}

	void OnRenderImage(RenderTexture source, RenderTexture destination) {
		Graphics.Blit(source, destination, mat);
	}
}