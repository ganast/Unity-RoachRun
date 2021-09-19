using UnityEngine;

public class RotatorY : MonoBehaviour {

    [SerializeField]
    private float angularVelocity = 360;

	protected void Update () {
        transform.Rotate(Vector3.up, angularVelocity * Time.deltaTime, Space.World);
	}
}
