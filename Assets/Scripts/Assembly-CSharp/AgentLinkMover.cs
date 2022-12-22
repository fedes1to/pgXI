using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class AgentLinkMover : MonoBehaviour
{
	[SerializeField]
	private OffMeshLinkMoveMethod _moveMethod = OffMeshLinkMoveMethod.Parabola;

	[SerializeField]
	private float _duration = 0.5f;

	[Header("special parameters")]
	[SerializeField]
	private float _parabolaHeight = 2f;

	[SerializeField]
	private AnimationCurve _curve = new AnimationCurve();

	private void OnEnable()
	{
		StartCoroutine(Manage());
	}

	private IEnumerator Manage()
	{
		NavMeshAgent agent = GetComponent<NavMeshAgent>();
		agent.autoTraverseOffMeshLink = false;
		while (true)
		{
			if (agent.isOnOffMeshLink)
			{
				if (_moveMethod == OffMeshLinkMoveMethod.NormalSpeed)
				{
					yield return StartCoroutine(NormalSpeed(agent));
				}
				else if (_moveMethod == OffMeshLinkMoveMethod.Parabola)
				{
					yield return StartCoroutine(Parabola(agent, _parabolaHeight, _duration));
				}
				else if (_moveMethod == OffMeshLinkMoveMethod.Curve)
				{
					yield return StartCoroutine(Curve(agent, _duration, _curve));
				}
				agent.CompleteOffMeshLink();
			}
			yield return null;
		}
	}

	private IEnumerator NormalSpeed(NavMeshAgent agent)
	{
		Vector3 endPos = agent.currentOffMeshLinkData.endPos + Vector3.up * agent.baseOffset;
		while (agent.transform.position != endPos)
		{
			agent.transform.position = Vector3.MoveTowards(agent.transform.position, endPos, agent.speed * Time.deltaTime);
			yield return null;
		}
	}

	private IEnumerator Parabola(NavMeshAgent agent, float height, float duration)
	{
		OffMeshLinkData data = agent.currentOffMeshLinkData;
		Vector3 startPos = agent.transform.position;
		Vector3 endPos = data.endPos + Vector3.up * agent.baseOffset;
		float normalizedTime = 0f;
		while (normalizedTime < 1f)
		{
			float yOffset = height * 4f * (normalizedTime - normalizedTime * normalizedTime);
			agent.transform.position = Vector3.Lerp(startPos, endPos, normalizedTime) + yOffset * Vector3.up;
			normalizedTime += Time.deltaTime / duration;
			yield return null;
		}
	}

	private IEnumerator Curve(NavMeshAgent agent, float duration, AnimationCurve curve)
	{
		OffMeshLinkData data = agent.currentOffMeshLinkData;
		Vector3 startPos = agent.transform.position;
		Vector3 endPos = data.endPos + Vector3.up * agent.baseOffset;
		float normalizedTime = 0f;
		while (normalizedTime < 1f)
		{
			float yOffset = curve.Evaluate(normalizedTime);
			agent.transform.position = Vector3.Lerp(startPos, endPos, normalizedTime) + yOffset * Vector3.up;
			normalizedTime += Time.deltaTime / duration;
			yield return null;
		}
	}
}
