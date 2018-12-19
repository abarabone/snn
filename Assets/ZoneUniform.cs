using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ZoneUniform : ZoneBase
{
	
	public int		xlen, ylen, zlen;

	public bool		IsLinkableSelfZone;

	
	

	public override bool IsLinkTarget( ZoneBase other )
	{
		return
			this.IsLinkableSelfZone && (ZoneBase)this == other
			||
			this.ForwardZones.Any( zone => zone == other )
			;
	}


	private void Start()
	{

		createNeurons();

		return;


		void createNeurons()
		{
			
			var tf	= this.transform;

			var box	= this.Shape as BoxCollider;
			if( box == null ) return;

			var xspan	= box.size.x / (this.xlen + 1);
			var yspan	= box.size.y / (this.ylen + 1);
			var zspan	= box.size.z / (this.zlen + 1);

			var offset	= ( box.center - new Vector3( box.size.x * 0.5f, box.size.y * 0.5f, box.size.z * 0.5f ) );


			foreach( var i in Enumerable.Range(0, this.xlen * this.ylen * this.zlen) )
			{
				GameObject.Instantiate<NeuronUnit>
					( this.NeuronTemplate, getNextPosition(i), Quaternion.identity, tf );
			}

			return;


			Vector3 getNextPosition( int i )
			{
				if( this.UnitLinkRadius == 0.0f ) return this.Shape.bounds.center;

				var x = i / (this.ylen * this.zlen) + 0.5f;
				var y = i % this.zlen * this.xlen + 0.5f;
				var z = i % (this.xlen * this.ylen) + 0.5f;

				return offset + new Vector3( x * xspan, y * yspan, z * zspan );
			}
			
		}
		
	}

}
