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
		public float	delta_value;

		public Func<float, float>	f;
		public Func<float, float>	d;

		public readonly float	learning_rate	= 0.3f;

		public void activate()
		{ 
			this.sum_value = this.backs
				.Sum( link => link.weight * link.back.activation )
				;
			this.activation = this.f( this.sum_value );
		}
		public void learn()
		{
			var delta = this.forwards
				.Sum( link => link.forward.delta_value * link.weight_old )
				;
			this.delta_value = delta * this.d( this.sum_value );

			var modify_backs = this.backs
				.Select( link => this.delta_value * link.back.activation * this.learning_rate )
				;
			foreach( var (modify, link) in Enumerable.Zip(modify_backs, this.backs) )
			{
				link.weight += modify;
			}
			this.bias += modify_backs.Sum();
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
			foreach( var n in layers.SelectMany( layer => layer.neurons ) )
			{
				n.activate();
			}
		}
		public void propergate_back()
		{
			foreach( var n in layers.Reverse<LayerUnit>().SelectMany( layer => layer.neurons ) )
			{
				n.learn();
			}
		}
	}
}

