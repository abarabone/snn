using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class ZoneBase : MonoBehaviour
{
    
	public ZoneBase[]	ForwardZones;

	public NeuronUnit	NeuronTemplate;

	public float	UnitLinkRadius;
	public float	UnitLinkArmDistance;
	//public float	UnitQuantize;
	
	[HideInInspector]
	public Collider Shape;



	abstract public bool IsLinkTarget( ZoneBase other );
	

	public Vector3 CalucNeuronForwardosition( Vector3 neuronPosition, float distance )
	{
		if( this.ForwardZones.Length == 0 ) return neuronPosition;

		var nearestZonePoint = this.ForwardZones
			.Where( zone => zone != null )
			.Select( zone => zone.Shape.ClosestPoint(neuronPosition) )
			.Select( clpoint => (clpoint, dist:Vector3.Distance(neuronPosition, clpoint)) )
			.Aggregate( (pre, cur)=> pre.dist > cur.dist ? cur : pre )
			.clpoint
			;

		return neuronPosition + ( nearestZonePoint - neuronPosition ).normalized * distance;
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

}
