using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zone : MonoBehaviour
{

	public Transform	Tf { get; private set; }
	public float		Radius;

	public float	linkRadius;


	private void Awake()
	{
		this.Tf	= this.transform;
	}

}
