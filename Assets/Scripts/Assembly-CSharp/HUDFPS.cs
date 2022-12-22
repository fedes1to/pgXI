using UnityEngine;

[AddComponentMenu("Utilities/HUDFPS")]
public class HUDFPS : MonoBehaviour
{
	private Rect startRect = new Rect((float)Screen.width * 0.2f, (float)Screen.height - 55f * Defs.Coef, 150f * Defs.Coef, 55f * Defs.Coef);

	public bool updateColor = true;

	public bool allowDrag = true;

	public float frequency = 0.5f;

	public int nbDecimal = 1;

	private float accum;

	private int frames;

	private Color color = Color.white;

	private string sFPS = string.Empty;

	private GUIStyle style;

	private string maxFPS = "0.0";

	private string minFPS = "300.0";

	private string middleFPS = "0.0";

	private float updateTime = 5f;

	private float sumFps;

	private int countFps;
}
