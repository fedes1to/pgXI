using UnityEngine;

public class NickLabelStack : MonoBehaviour
{
	public static NickLabelStack sharedStack;

	public int lengthStack = 30;

	public NickLabelController[] lables;

	private int currentIndexLabel;

	private void Awake()
	{
		sharedStack = this;
	}

	private void Start()
	{
		Object.DontDestroyOnLoad(base.gameObject);
		base.transform.localPosition = Vector3.zero;
		Transform transform = base.transform.GetChild(0).transform;
		base.transform.position = Vector3.zero;
		lables = new NickLabelController[lengthStack];
		lables[0] = transform.GetChild(0).GetComponent<NickLabelController>();
		while (transform.childCount < lengthStack)
		{
			GameObject gameObject = Object.Instantiate(transform.GetChild(0).gameObject);
			Transform transform2 = gameObject.transform;
			transform2.parent = transform;
			transform2.localPosition = Vector3.zero;
			transform2.localScale = new Vector3(1f, 1f, 1f);
			transform2.rotation = Quaternion.identity;
			lables[transform.childCount - 1] = gameObject.GetComponent<NickLabelController>();
		}
	}

	public NickLabelController GetNextCurrentLabel()
	{
		base.transform.localPosition = Vector3.zero;
		bool flag = true;
		do
		{
			currentIndexLabel++;
			if (currentIndexLabel >= lables.Length)
			{
				if (!flag)
				{
					return null;
				}
				currentIndexLabel = 0;
				flag = false;
			}
		}
		while (lables[currentIndexLabel].target != null);
		lables[currentIndexLabel].currentType = NickLabelController.TypeNickLabel.None;
		return lables[currentIndexLabel];
	}

	public NickLabelController GetCurrentLabel()
	{
		return lables[currentIndexLabel];
	}

	private void OnDestroy()
	{
		sharedStack = null;
	}
}
