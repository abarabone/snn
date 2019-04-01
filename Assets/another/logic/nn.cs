using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

namespace nn
{
	using ActivationFunctions;
	using OutputProcessFunctions;
	using LossFunctions;

	//[Serializable]
	public class N
	{
		public LayerUnit[]	layers;
		public float		learning_rate;

		public IOutputFunction	output_layer_func;
		public ILossFunction	loss_func;

		public N(
			IEnumerable<int> neuron_length_per_layers,
			IEnumerable<IActivationFunction> actfuncs,
			IOutputFunction	output_layer_function,
			ILossFunction	loss_function
		)
		{
			create_layers( neuron_length_per_layers, actfuncs );
			init_links();
			//foreach( var n in this.layers.Take( this.layers.Length - 1 ).SelectMany( l => l.neurons ) )
			//{
			//	n.sign = UnityEngine.Random.value > 0.3f ? 1.0f : -1.0f;
			//}

			this.loss_func = loss_function;
			this.output_layer_func = output_layer_function;
			
			if( output_layer_function is SoftMax ) add_softmax_layer();
		}

		public void create_layers( IEnumerable<int> neuron_length_per_layers, IEnumerable<IActivationFunction> actfuncs )
		{
			this.layers = Enumerable.Zip( neuron_length_per_layers, actfuncs, (x,y)=>(num:x, func:y) )
				.Select( x => new LayerUnit(x.num, x.func) )
				.ToArray()
				;
		}
		public void add_softmax_layer()
		{
			var softmax_layer = new LayerUnit( this.layers.Last().neurons.Length, actfunc:null );

			var q = Enumerable.Zip( this.layers.Last().neurons, softmax_layer.neurons, (x,y)=>(x,y) );
			foreach( var (prev_node, last_node) in q )
			{
				prev_node.forwards[0].forward = last_node;

				var new_last_link	= new NeuronLinkUnit()
				{
					back	= last_node,
					forward	= null,
					weight	= 1.0f
				};
				last_node.backs		= new NeuronLinkUnit [] { prev_node.forwards[0] };
				last_node.forwards	= new NeuronLinkUnit [] { new_last_link };
			}

			this.layers = this.layers
				.Concat( new LayerUnit [] { softmax_layer } )
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
			
			output_layer_func.forward_propergate( layers.Last().neurons );
		}
		public void propergate_back()
		{
			output_layer_func.back_propergate( layers.Last().neurons );
			foreach( var n in layers.Last().neurons ) Debug.Log( n.backs[ 0 ].delta_weighted );
			foreach( var n in layers.Take(layers.Length-1).Last().neurons ) Debug.Log( n.forwards[ 0 ].delta_weighted );
			foreach( var n in layers.Skip(1).Reverse<LayerUnit>().SelectMany( layer => layer.neurons ) )
			//foreach( var n in from layer in this.layers.Reverse<LayerUnit>() from n in layer.neurons select n )
			{
				n.learn( this.learning_rate );
			}
		}

		public void set_input_values( IEnumerable<float> input_values )
		{
			var input_nodes = this.layers.First().neurons;
			var q = Enumerable.Zip( input_values, input_nodes, (v, n) => (v, n) );
			foreach( var (input_value, node) in q )
			{
				node.activation = input_value;
			}
		}
		public void set_correct_values( IEnumerable<float> correct_values )
		{
			var output_nodes = this.layers.Last().neurons;
			var q = Enumerable.Zip( correct_values, output_nodes, (c, n) => (c, n) );
			foreach( var (correct_value, node) in q )
			{
				node.set_loss_delta( loss_func.d(node.activation, correct_value) );
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

