using UnityEngine;
using UnityEngine.UI;

public sealed class CameraTouchControlSchemeConfigurator : MonoBehaviour
{
	public Toggle toggleCleanNGUI;

	public Toggle toggleSmoothDump;

	public Toggle toggleLowPassFilter;

	public Toggle toggleUFPS;

	public RectTransform panelCleanNGUI;

	public RectTransform panelSmoothDump;

	public RectTransform panelLowPassFilter;

	public RectTransform panelUFPS;

	public InputField firstDragClampedMax1;

	public InputField startMovingThreshold1;

	public InputField senseModifier;

	public InputField senseModifierByAxisX;

	public InputField senseModifierByAxisY;

	public InputField dampingTime;

	public InputField firstDragClampedMax2;

	public InputField lerpCoeff;

	public InputField startMovingThreshold2;

	public InputField mouseLookSensitivityX;

	public InputField mouseLookSensitivityY;

	public InputField mouseLookSmoothSteps;

	public InputField mouseLookSmoothWeight;

	public Toggle mouseLookAcceleration;

	public InputField mouseLookAccelerationThreshold;

	public static CameraTouchControlSchemeConfigurator Instance { get; private set; }
}
