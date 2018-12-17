using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Zone : MonoBehaviour
{

	//public Transform	Tf { get; private set; }
	public float		Radius;

	public float	linkRadius;
	public int		MaxNeurons;
	
	public Zone[]	forwardZones;


	public NeuronUnit	NeuronTemplate;


	Collider	Shape;
	
	public Vector3 GetRandomPosition( int i )
	{
		while( false )
		{
			Random.InitState( i );
			var bbox	= this.Shape.bounds;
			var px		= Random.Range( bbox.min.x,bbox.max.x );
			var py		= Random.Range( bbox.min.y,bbox.max.y );
			var pz		= Random.Range( bbox.min.z,bbox.max.z );
			var rndInBbox = new Vector3( px, py, pz );

			if( this.Shape.ClosestPoint(rndInBbox) != rndInBbox ) return rndInBbox;
		}return new Vector3();
	}

	private void Awake()
	{
		this.Shape	= this.GetComponent<Collider>();

		var tf	= this.transform;

		createNeurons();

		return;


		void createNeurons()
		{
			foreach( var i in Enumerable.Range(0, this.MaxNeurons) )
			{
				GameObject.Instantiate<NeuronUnit>
					( this.NeuronTemplate, this.GetRandomPosition(i), Quaternion.identity, tf );
			}
		}
	}

}
