using UnityEngine;

public class DuelPlayerInfo : MonoBehaviour
{
	public UILabel level;

	public UILabel[] playerName;

	public UILabel clanName;

	public UITexture clanTexture;

	private Transform pointInWorld;

	private Transform character;

	public Rect touchZone;

	private Quaternion defaultRotation = Quaternion.identity;

	private Vector2 lastPosition = Vector2.zero;

	private float lastTapTime;

	private int tapFingerID = -1;

	private static int fingerInUse = -1;

	public void FillByTable(NetworkStartTable table)
	{
		level.text = table.myRanks.ToString();
		UILabel[] array = playerName;
		foreach (UILabel uILabel in array)
		{
			uILabel.text = table.NamePlayer;
		}
		clanName.text = table.myClanName;
		clanTexture.mainTexture = table.myClanTexture;
	}

	public void SetPointInWorld(Transform pointTransform, Transform character)
	{
		this.character = character;
		pointInWorld = pointTransform;
		if (defaultRotation == Quaternion.identity)
		{
			defaultRotation = character.rotation;
		}
	}

	private void Awake()
	{
		touchZone = new Rect(-0.2f * (float)Screen.width, -0.65f * (float)Screen.height, 0.45f * (float)Screen.width, 0.65f * (float)Screen.height);
	}

	private void Update()
	{
		if (!(pointInWorld == null) && !(NickLabelController.currentCamera == null))
		{
			Vector3 vector = NickLabelController.currentCamera.WorldToScreenPoint(pointInWorld.position + Vector3.up);
			base.transform.localPosition = new Vector3(vector.x / (float)Screen.height * 768f, vector.y / (float)Screen.height * 768f, 0f);
			RotateCharacters(vector);
		}
	}

	private void RotateCharacters(Vector2 relativePoint)
	{
		Vector2 zero = Vector2.zero;
		Vector2 vector = Vector2.zero;
		bool flag = false;
		if (Application.isEditor)
		{
			Vector2 vector2 = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
			if ((tapFingerID == -1 && touchZone.Contains(vector2 - relativePoint)) || tapFingerID == 1)
			{
				if (Input.GetMouseButtonDown(0))
				{
					tapFingerID = 1;
				}
				else if (Input.GetMouseButtonUp(0))
				{
					tapFingerID = -1;
					lastPosition = Vector2.zero;
				}
				else if (Input.GetMouseButton(0))
				{
					flag = true;
					vector = vector2;
				}
			}
		}
		else
		{
			for (int i = 0; i < Input.touchCount; i++)
			{
				Touch touch = Input.GetTouch(i);
				if ((tapFingerID == -1 && !fingerInUse.Equals(touch.fingerId) && touchZone.Contains(touch.position - relativePoint)) || touch.fingerId == tapFingerID)
				{
					if (touch.phase == TouchPhase.Began)
					{
						tapFingerID = touch.fingerId;
					}
					else if (touch.phase == TouchPhase.Ended)
					{
						tapFingerID = -1;
						lastPosition = Vector2.zero;
					}
					else
					{
						flag = true;
						vector = touch.position;
					}
				}
			}
		}
		if (flag)
		{
			fingerInUse = tapFingerID;
			lastTapTime = Time.time;
			if (lastPosition == Vector2.zero)
			{
				lastPosition = vector;
			}
			zero = vector - lastPosition;
			lastPosition = vector;
			character.Rotate(Vector3.up, zero.x * Time.deltaTime * RilisoftRotator.RotationRateForCharacterInMenues);
		}
		else
		{
			fingerInUse = -1;
			if (Time.time - lastTapTime > ShopNGUIController.IdleTimeoutPers)
			{
				character.rotation = Quaternion.RotateTowards(character.rotation, defaultRotation, 300f * Time.deltaTime);
			}
		}
	}

	private void OnDisable()
	{
		tapFingerID = -1;
		fingerInUse = -1;
	}
}
