using Rilisoft;
using UnityEngine;

[RequireComponent(typeof(UISprite))]
[RequireComponent(typeof(UICenterOnPanelComponent))]
public class ProfileCup : MonoBehaviour
{
	private UISprite _cup;

	[SerializeField]
	public RatingSystem.RatingLeague League;

	public GameObject Outline;

	private LeaguesGUIController _controller;

	private UICenterOnPanelComponent _centerMonitor;

	public UISprite Cup
	{
		get
		{
			return _cup ?? (_cup = GetComponent<UISprite>());
		}
	}

	private void Start()
	{
		_controller = base.gameObject.GetComponentInParents<LeaguesGUIController>();
		_centerMonitor = GetComponent<UICenterOnPanelComponent>();
		_centerMonitor.OnCentered.RemoveListener(OnCentered);
		_centerMonitor.OnCentered.AddListener(OnCentered);
	}

	private void OnCentered()
	{
		_controller.CupCentered(this);
	}

	private void OnEnable()
	{
		Outline.SetActive(League == RatingSystem.instance.currentLeague);
		Cup.spriteName = string.Format("{0} {1}", League, 3 - RatingSystem.instance.DivisionInLeague(League));
	}
}
