using UnityEngine;
using System.Linq;

public class OffsetRotationPin : MonoBehaviour
{

	public GameObject LookObject;
	public GameObject FixationPoint;

	public GameObject OffsetCam;

	public GameObject HeadLookSphere;

	public Quaternion _offsetWorldRotation;

	public GameObject scriptController;

	private void Start()
	{
		_offsetWorldRotation = transform.rotation;
	}

	void Update()
	{
        NaturalRenderingManager manager = scriptController.GetComponent<NaturalRenderingManager>();
     
        bool Freeze = manager.PinFixationPoint;

		if (Freeze == true)
        {
			transform.LookAt (manager.NR_fP);
		}

	}

	public void CalculatePivotAngle()
	{

		if (OffsetCam != null)
			_offsetWorldRotation = OffsetCam.transform.rotation;

		transform.LookAt(FixationPoint.transform.position);

		if (OffsetCam != null)
			OffsetCam.transform.rotation = _offsetWorldRotation;
	}
}


