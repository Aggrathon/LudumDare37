using UnityEngine;
using UnityEngine.Events;

public class TriggerEvent : MonoBehaviour {
	public bool playerOnly;
	public UnityEvent onTrigger;

	void OnTriggerEnter(Collider col)
	{
		if (!playerOnly || col.transform.CompareTag("Player"))
		{
			onTrigger.Invoke();
		}
	}
}
