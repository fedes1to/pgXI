using UnityEngine;

public class MovingDummy : MonoBehaviour
{
	private BaseDummy myDummy;

	private Vector3 startPoint;

	public Vector3[] pathPoints;

	public float speed = 2f;

	public float smooth = 1f;

	private int currentPoint;

	private Vector3 moveVector;

	private void Awake()
	{
		myDummy = GetComponent<BaseDummy>();
	}

	private void Start()
	{
		startPoint = base.transform.localPosition;
	}

	public void ResetPath()
	{
		base.transform.localPosition = startPoint;
		moveVector = Vector3.zero;
		currentPoint = 0;
	}

	private void Update()
	{
		if (pathPoints != null && pathPoints.Length != 0 && !myDummy.isDead)
		{
			if (currentPoint >= pathPoints.Length)
			{
				currentPoint = 0;
			}
			if ((base.transform.localPosition - (startPoint + pathPoints[currentPoint])).sqrMagnitude > 1f)
			{
				Vector3 normalized = (startPoint + pathPoints[currentPoint] - base.transform.localPosition).normalized;
				moveVector = Vector3.MoveTowards(moveVector, normalized, smooth * Time.deltaTime);
				base.transform.localPosition += moveVector * speed * Time.deltaTime;
			}
			else
			{
				currentPoint++;
			}
		}
	}
}
