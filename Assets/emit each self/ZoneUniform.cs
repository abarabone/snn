using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Neuron
{
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

				var span = new Vector3
				{
					x	= box.size.x / this.xlen,
					y	= box.size.y / this.ylen,
					z	= box.size.z / this.zlen
				};

				var offset	= box.center - box.size * 0.5f + span * 0.5f;

			
				foreach( var i in Enumerable.Range( 0, this.xlen * this.ylen * this.zlen ) )
				{
					GameObject.Instantiate<NeuronUnit>
						( this.NeuronTemplate, getNextPosition(i), Quaternion.identity, tf );
				}

				return;

			
				Vector3 getNextPosition( int i )
				{
					if( this.UnitLinkRadius == 0.0f ) return this.Shape.bounds.center;

					var iv	= new Vector3
					{
						x = i % this.xlen,
						y = i / this.xlen % this.ylen,
						z = i / (this.xlen * this.ylen)// % this.zlen
					};

					return tf.TransformPoint( offset + Vector3.Scale( span, iv ) );
				}
			
			}
		
		}

	}

}