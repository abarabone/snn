using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Neuron.ActiveEmit
{
	
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



		int	seed;
		public IEnumerator AutoTeaching( int freq )
		{
			var children	= this.GetComponentsInChildren<NeuronUnit>();
			var inputs		= this.transform
				.parent
				.GetComponentsInChildren<ZoneBase>()
				.Where( zone => zone.NeuronTemplate is NeuronUnitTrigger )
				.SelectMany( zone => zone.GetComponentsInChildren<NeuronUnitTrigger>() )
				;

			Random.InitState( this.seed++ );

			foreach( var i in Enumerable.Range(0,freq) )
			{
				var emittedInputCount = setRandomInputs();
				teachAll( emittedInputCount >= 5 && children[0].IsEmit() || emittedInputCount < 5 && !children[0].IsEmit() );

				yield return null;
			}
			
			yield return null;


			int setRandomInputs()
			{
				foreach( var input in inputs )
				{
					input.Clear();
					input.Emit( Random.value > 0.5f ? 1.0f : 0.0f );//Debug.Log($"{input.name} {input.volume}");
				}
				return inputs.Where( input => input.IsEmit() ).Count();
			}

			void teachAll( bool isSuccess )
			{
				foreach( var child in children )
				{
					child.Teach( isSuccess );//Debug.Log($"{child.name} {child.volume}");
					child.Clear();
				}
			}
		}
	}

}