using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ZoneRandom : MonoBehaviour
{
	
	public int		MaxNeurons;

	public float	UnitLinkRadius;
	public float	UnitLinkArmDistance;
	//public float	UnitQuantize;

	public IZone[]	ForwardZones;

	public bool		IsLinkableSelfZone;
	public int		xlen, ylen, zlne;

	public NeuronUnit	NeuronTemplate;


	[HideInInspector]
	public Collider	Shape;
	

	public bool Is(  )
	{
		this.IsLinkableSelfZone && n.ParentZone == this ||
		this.ForwardZones.Any( x => x == n.ParentZone )
	}


	private void Awake()
	{
		initValues();
		
		return;

		void initValues()
		{
			this.Shape	= this.GetComponent<Collider>();
		}
	}

	private void Start()
	{
		var tf	= this.transform;

		createNeurons();

		return;

		void createNeurons()
		{

			foreach( var i in Enumerable.Range(0, this.MaxNeurons) )
			{
				GameObject.Instantiate<NeuronUnit>
					( this.NeuronTemplate, getRandomPosition(i), Quaternion.identity, tf );
			}
			
		}
		
		Vector3 getRandomPosition( int i )
		{
			if( this.UnitLinkRadius == 0.0f ) return this.Shape.bounds.center;

			while( true )
			{
				Random.InitState( i );
				var bbox	= this.Shape.bounds;
				var px		= Random.Range( bbox.min.x, bbox.max.x );
				var py		= Random.Range( bbox.min.y, bbox.max.y );
				var pz		= Random.Range( bbox.min.z, bbox.max.z );
				//px -= px % this.UnitQuantize;
				//py -= py % this.UnitQuantize;
				//pz -= pz % this.UnitQuantize;
				var rndInBbox = new Vector3( px, py, pz );
			
				if( this.Shape.ClosestPoint(rndInBbox) == rndInBbox ) return rndInBbox;
			}
		}
	}

}
