using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

namespace a
{

	public class NeuronUnit
	{
		public LinkUnit[]	forwards;
		public LinkUnit[]	backs;

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
			foreach( var (modify, link) in Enumerable.Zip(modify_backs, this.backs, (x,y) => (x,y) ) )
			{
				link.weight += modify;
				this.bias	+= modify;
			}
		}
		public NeuronUnit()
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
		public NeuronUnit[]	neurons;
		public LayerUnit( int length ) => this.neurons = new NeuronUnit[ length ];
	}

	public class N
	{
		public LayerUnit[]	layers =
		{
			new LayerUnit( 2 ),
			new LayerUnit( 1 )
		};

		public N()
		{
			init_links();
		}

		public void propergate_forward()
		{
			//foreach( var n in layers.SelectMany( layer => layer.neurons ) )
			foreach( var n in from layer in this.layers from n in layer.neurons select n )
			{
				n.activate();
			}
		}
		public void propergate_back()
		{
			//foreach( var n in layers.Reverse<LayerUnit>().SelectMany( layer => layer.neurons ) )
			foreach( var n in from layer in this.layers.Reverse<LayerUnit>() from n in layer.neurons select n )
			{
				n.learn();
			}
		}

		private void init_links()
		{
			this.layers.Aggregate( (prev_layer, next_layer) => set_links_to_both_side_nodes_( prev_layer, next_layer ) );

			return;

			LayerUnit set_links_to_both_side_nodes_( LayerUnit prev_layer, LayerUnit next_layer )
			{
				var q = from pn in prev_layer.neurons
						from nn in next_layer.neurons
						select new LinkUnit
						{
							back	= pn,
							forward	= nn,
							weight	= 0,
						}
						;
				foreach( var prev_group in from n in q group q by n.back )
				{
					prev_group.Key.forwards = prev_group.SelectMany( links => links ).ToArray();
				}
				foreach( var next_group in from n in q group q by n.forward )
				{
					next_group.Key.forwards = next_group.SelectMany( links => links ).ToArray();
				}
				return next_layer;
			}
		}
	}
}

