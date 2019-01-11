using UnityEngine;

public class ConstantRotation : MonoBehaviour
{
	public Vector3 Axis;
	public float Speed = 1f;
	public bool RandomOffset = true;

	private void Start()
	{
		if (RandomOffset)
		{
			transform.localRotation *= Quaternion.Euler(Axis * 360 * Random.Range(0f, 1f));
		}
	}

	void Update ()
	{
		transform.localRotation *= Quaternion.Euler(Axis * Speed);
	}
}
