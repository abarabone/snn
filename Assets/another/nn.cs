using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

namespace a
{

	public class NeuronUnit
	{
		public List<LinkUnit>	forwards;
		public List<LinkUnit>	backs;

		public float	bias;
		public float	activation;
		public float	sum_value;

		public Func<float, float>	f;
		public Func<float, float>	d;

		public void activate()
		{
			this.activation = this.backs
				.Sum( link => link.weight * link.back.activation )
				;
		}
		public void learn()
		{
			this.forwards
				.Select( link => link.delta * link.weight_old * this.d(this.sum_value) )
				.
		}
		public void init()
		{
			this.f = sum_value => 1.0f / ( 1.0f + (float)Math.Exp((float)-sum_value) );
			this.d = activation => activation * ( 1.0f - activation );
		}
	}

	public class LinkUnit
	{
		public float	weight;
		public float	weight_old;
		public float	delta;

		public NeuronUnit	back;
		public NeuronUnit	forward;
	}

	public class LayerUnit
	{
		public List<NeuronUnit>	neurons;
	}

	public class N
	{
		public List<LayerUnit>	layers;

		public void propergate_forward()
		{
			layers
				.Select(  )
		}
	}
}

