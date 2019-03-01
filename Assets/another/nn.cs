using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

namespace a
{
	
	[Serializable]
	public class NeuronUnit
	{
		public NeuronLinkUnit[]	forwards	= new NeuronLinkUnit [] { };
		public NeuronLinkUnit[]	backs		= new NeuronLinkUnit [] { };

		public float	bias;
		public float	activation;
		//public float	sum_value;

		public Func<float, float>	f;
		public Func<float, float>	d;
		
		public readonly float	learning_rate	= 0.3f;

		public void activate()
		{
			//Debug.Log("a0:"+this.activation+" "+this.backs.Length);
			var sum_value = this.backs
			//	.Select( x => {Debug.Log(x.back.activation+" "+x.forward.activation);return x;} )
				.Sum( link => link.weight * link.back.activation )
				;
			this.activation = this.f( sum_value + this.bias );
			Debug.Log( $"a:{this.activation} b:{this.bias} {this.GetHashCode()}" );
		}

		public void learn()
		{

			var delta_value = retrieve_delta_from_forwards_();

			modify_to_backs_( delta_value );

			//Debug.Log( $"a:{this.activation} b:{this.bias} {this.GetHashCode()}" );
			return;


			/// 出力側から重みのかけられたδを取得し、合計する。
			float retrieve_delta_from_forwards_()
			{
				var sum_delta_forwards = this.forwards
					.Sum( link => link.delta_weighted )
					;
				return sum_delta_forwards * this.d( this.activation );
			}

			/// 入力側へδを伝える。
			void modify_to_backs_( float delta_value_ )
			{
				var modify_for_backs = this.backs
					.Select( link => delta_value_ * link.back.activation * this.learning_rate )
					;
				foreach( var (modify, link) in Enumerable.Zip(modify_for_backs, this.backs, (x,y) => (x,y)) )
				{
					link.delta_weighted	= link.weight * delta_value;// 更新前の重みを使用する。
					link.weight			-= modify;
					this.bias			-= modify;
					//Debug.Log( $"w:{link.weight} b:{this.bias} {link.GetHashCode()}" );
				}
			}
		}
		float __delta_value;
		void learn2()
		{
			var sum_delta = this.forwards
				.Sum( link => link.weight * link.forward.__delta_value )
				;
		}

		public void caluclate_delta_value( float correct_value )
		{
			this.forwards[0].delta_weighted	= this.activation - correct_value;
		}

		public NeuronUnit()
		{
			this.f = sum_value => 1.0f / ( 1.0f + (float)Math.Exp((float)-sum_value) );
			this.d = activation => activation * ( 1.0f - activation );
		}
	}

	[Serializable]
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
		public LayerUnit( int length )
		{
			var q = from i in Enumerable.Range( 0, length )
					select new NeuronUnit
					{
						activation	= 0.0f,
						bias		= UnityEngine.Random.value
					}
					;
			this.neurons = q.ToArray();
		}
	}
	
	[Serializable]
	public class N
	{
		public LayerUnit[]	layers;

		public N( int[] neuron_length_per_layers )
		{
			create_layers( neuron_length_per_layers );
			init_links();
		}

		public void create_layers( int[] neuron_length_per_layers )
		{
			this.layers = neuron_length_per_layers
				.Select( num => new LayerUnit(num) )
				.ToArray()
				;
		}

		public void propergate_forward()
		{
			foreach( var n in layers.Skip(1).SelectMany( layer => layer.neurons ) )
			//foreach( var n in from layer in this.layers from n in layer.neurons select n )
			{
				n.activate();
			}
		}
		public void propergate_back()
		{
			foreach( var n in layers.Skip(1).Reverse<LayerUnit>().SelectMany( layer => layer.neurons ) )
			//foreach( var n in from layer in this.layers.Reverse<LayerUnit>() from n in layer.neurons select n )
			{
				n.learn();
			}
		}

		public void set_input_values( float[] input_values )
		{
			var input_nodes = this.layers.First().neurons;
			var q = Enumerable.Zip( input_values, input_nodes, (v, n) => (input_value:v, node:n) );
			foreach( var x in q )
			{
				x.node.activation = x.input_value;
			}
		}
		public void set_correct_values( float[] correct_values )
		{
			var output_nodes = this.layers.Last().neurons;
			var q = Enumerable.Zip( correct_values, output_nodes, (c, n) => (correct_value:c, node:n) );
			foreach( var x in q )
			{
				x.node.caluclate_delta_value( x.correct_value );
			}
		}

		private void init_links()
		{
			this.layers.Aggregate( (prev_layer, next_layer) => set_links_to_both_side_nodes_(prev_layer, next_layer) );

			set_links_output_terminate_();

			return;


			/// 層の間にリンクを張る。
			LayerUnit set_links_to_both_side_nodes_( LayerUnit prev_layer, LayerUnit next_layer )
			{
				var q = from pn in prev_layer.neurons
						from nn in next_layer.neurons
						select new NeuronLinkUnit
						{
							back	= pn,
							forward	= nn,
							weight	= UnityEngine.Random.value,
						}
						;
				var neuron_pairs = q.ToArray();
				foreach( var prev_group in from n in neuron_pairs group n by n.back )
				{
					prev_group.Key.forwards = prev_group.ToArray();
				}
				foreach( var next_group in from n in neuron_pairs group n by n.forward )
				{
					next_group.Key.backs = next_group.ToArray();
				}
				return next_layer;
			}

			/// 出力層にも null ノードへのリンクを張っておく。
			/// ・一般化されるため、逆伝搬で出力層用のコードが必要なくなる。
			void set_links_output_terminate_()
			{
				var q = from node in layers.Last().neurons
						select new NeuronLinkUnit
						{
							back	= node,
							forward	= null,
							weight	= 1.0f
						}
						;
				foreach( var link in q )
				{
					link.back.forwards = new NeuronLinkUnit[]{ link };
				}
			}
		}
	}
}

