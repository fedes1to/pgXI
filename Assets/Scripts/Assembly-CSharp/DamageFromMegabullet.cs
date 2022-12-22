using UnityEngine;

public class DamageFromMegabullet : MonoBehaviour
{
	private Rocket myRocketScript;

	private void Start()
	{
		myRocketScript = base.transform.root.GetComponent<Rocket>();
	}

	private void Update()
	{
		if (myRocketScript == null)
		{
			myRocketScript = base.transform.root.GetComponent<Rocket>();
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (!(myRocketScript == null))
		{
			myRocketScript.OnMegabulletTriggerEnter(other);
		}
	}
}
