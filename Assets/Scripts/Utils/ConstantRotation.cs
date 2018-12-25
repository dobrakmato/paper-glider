using UnityEngine;

public class ConstantRotation : MonoBehaviour
{
	public Vector3 Axis;
	public float Speed = 1f;
	
	void Update ()
	{
		transform.localRotation *= Quaternion.Euler(Axis * Speed);
	}
}
