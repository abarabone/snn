using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

namespace nn
{
	using ActivationFunctions;

	//[Serializable]
	public class NeuronLinkUnit
	{
		public NeuronUnit	back;
		public NeuronUnit	forward;

		public float	weight;
		public float	delta_weighted;
	}
	
	[Serializable]
	public class LayerUnit
	{
		public NeuronUnit[]	neurons;

		public LayerUnit( int length, IActivationFunction actfunc )
		{
			var q = from i in Enumerable.Range( 0, length )
					select new NeuronUnit
					{
						sign = 1.0f,
						activation	= 0.0f,
						bias		= UnityEngine.Random.value,
						af			= actfunc != null ? (IActivationFunction)Activator.CreateInstance( actfunc.GetType() ) : null
					}
					;
			this.neurons = q.ToArray();
		}
	}
	
}