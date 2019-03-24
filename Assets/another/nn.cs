using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

namespace nn
{
	
	//[Serializable]
	public class NeuronUnit
	{
		public NeuronLinkUnit[]	forwards	= new NeuronLinkUnit [] { };
		public NeuronLinkUnit[]	backs		= new NeuronLinkUnit [] { };

		public float	bias;
		public float	activation;
		public float	sum_value;
		public float	propergated_value;

		public Func<float, float>			f;
		public Func<float, float, float>	d;
		public IActivationFunction	af;
		public ILossFunction		lf;

		public void activate()
		{
			this.sum_value = this.backs
				.Sum( link => link.weight * link.back.activation )
				+ this.bias
				;
			this.activation = this.f( this.sum_value );
		}

		public void learn( float learning_rate )
		{

			var delta_value = retrieve_delta_from_forwards_();
			if( float.IsNaN(delta_value) ) delta_value = 0.0f;//

			modify_to_backs_( delta_value );

			//Debug.Log( $"a:{this.activation} b:{this.bias} {this.GetHashCode()}" );
			return;


			/// 出力側から重みのかけられたδを取得し、合計する。
			float retrieve_delta_from_forwards_()
			{
				var sum_delta_forwards = this.forwards
					.Sum( link => link.delta_weighted )
					;
				return sum_delta_forwards * this.d( this.sum_value, this.activation );
			}

			/// 入力側へδを伝える。
			void modify_to_backs_( float delta_value_ )
			{
				foreach( var link in this.backs )
				{
					link.delta_weighted	= link.weight * delta_value_;// 更新前の重みを使用する。
					link.weight			-= delta_value_ * link.back.activation * learning_rate;
					//Debug.Log( $"w:{link.weight} b:{this.bias} {link.GetHashCode()}" );
				}
				this.bias	-= delta_value_ * this.backs.Length * learning_rate;
			}
		}
		public void learn2( float learning_rate )
		{
			var sum_forward_propergated_value = this.forwards
				.Select( link => link.weight * link.forward.propergated_value )
				.Sum();
			
			foreach( var link in this.forwards )
			{
				var modify = link.weight * link.forward.activation;
				link.weight -= modify * learning_rate;
			}

			if( this.forwards.Length != 0 )
			{
				this.propergated_value = sum_forward_propergated_value * d( this.sum_value, this.activation );
			}

			this.bias -= this.propergated_value * learning_rate;
		}

		public void caluclate_delta_value( float correct_value )
		{
			this.forwards[0].delta_weighted	= this.activation - correct_value;
		}
		

		public interface IActivationFunction
		{
			float sum( IEnumerable<(float activation, float weight)> values, float bias );
			float f( float sum_value );
			float d();
		}
		public class Identity : IActivationFunction
		{
			public float sum( IEnumerable<(float activation, float weight)> values, float bias )
			{
				return values.Select( x => x.activation * x.weight ).Sum() + bias;
			}
			public float f( float sum_value ) => sum_value;
			public float d() => 1.0f;
		}
		public class Sigmoid : IActivationFunction
		{
			float	activation_;

			public float sum( IEnumerable<(float activation, float weight)> values, float bias )
			{
				return values.Select( x => x.activation * x.weight ).Sum() + bias;
			}
			public float f( float sum_value ) => this.activation_ = (float)(1.0d / ( 1.0d + Math.Exp(-sum_value) ));
			public float d() => this.activation_ * ( 1.0f - this.activation_ );
			//public float d( float sum_value, float activation_value ) => f(sum_value) * ( 1.0f - f(sum_value) );
		}
		public class ReLU : IActivationFunction
		{
			float	sum_value_;
			
			public float sum( IEnumerable<(float activation, float weight)> values, float bias )
			{
				this.sum_value_ = values.Select( x => x.activation * x.weight ).Sum() + bias;
				return this.sum_value_;
			}
			public float f( float sum_value ) => (this.sum_value_ = sum_value) > 0.0f ? sum_value : 0.0f;
			public float d() => this.sum_value_ > 0.0f ? 1.0f : 0.0f;
		}
		public class Tanh : IActivationFunction
		{
			float	sum_value_;

			public float sum( IEnumerable<(float activation, float weight)> values, float bias )
			{
				this.sum_value_ = values.Select( x => x.activation * x.weight ).Sum() + bias;
				return this.sum_value_;
			}
			public float f( float sum_value )
			{
				this.sum_value_ = sum_value;
				var ex = Math.Exp( -2.0d * sum_value );
				return (float)( ( 1.0d - ex ) / ( 1.0d + ex ) );
			}
			public float d()
			{
				var ee  = Math.Exp(this.sum_value_) + Math.Exp(-this.sum_value_);
				return (float)( 4.0d / ( ee * ee ) );
			}
		}
		public class SoftMax : IActivationFunction
		{
			public int	class_index;
			float		activation_;

			public float sum( IEnumerable<(float activation, float weight)> values, float bias )
			{
				return values.Select( x => (float)Math.Exp(x.activation) ).Sum();
			}
			public float f( float sum_value ) => throw new NotImplementedException();
			public float d() => this.activation_ * ( 1.0f - this.activation_ );
		}
		public class aaa : IActivationFunction
		{
			float	sum_value_;

			public float sum( IEnumerable<(float activation, float weight)> values, float bias )
			{
				this.sum_value_ = values.Select( x => x.activation * x.weight ).Sum() + bias;
				return this.sum_value_;
			}
			public float f( float sum_value )
			{
				if( this.sum_value_ >=  0.5f ) return sum_value;
				if( this.sum_value_ <= -0.5f ) return sum_value;
				return 0.0f;
			}
			public float d()
			{
				if( this.sum_value_ >=  0.5f ) return 1.0f;
				if( this.sum_value_ <= -0.5f ) return 1.0f;
				return 0.0f;
			}
		}

		public interface ILossFunction
		{
			float a( IEnumerable<float> activations, float bias, IActivationFunction activation_function );
			float f( IEnumerable<float> input_values, IEnumerable<float> correct_values );
			float d( );
		}
		public class MSE : ILossFunction
		{
			public float d()
			{
				throw new NotImplementedException();
			}
			public float f( IEnumerable<float> input_values, IEnumerable<float> correct_values )
			{
				throw new NotImplementedException();
			}
			public float a( IEnumerable<float> activations, float bias, IActivationFunction activation_function )
			{
				return activation_function.f( activations.Sum() + bias );
			}
		}
		public class CrossEntropy : ILossFunction
		{
			public float d()
			{
				throw new NotImplementedException();
			}
			public float f( IEnumerable<float> input_values, IEnumerable<float> correct_values )
			{
				throw new NotImplementedException();
			}
			public float a( IEnumerable<float> activations, float bias, IActivationFunction activation_function )
			{
				switch( activation_function )
				{
					case Sigmoid	af:break;
					case SoftMax	af:break;
					case var		af:				return activation_function.f( activations.Sum() + bias );
				}
				return activations.Sum() + bias;
			}
		}
	}

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
		public LayerUnit( int length, NeuronUnit.IActivationFunction actfunc )
		{
			var q = from i in Enumerable.Range( 0, length )
					select new NeuronUnit
					{
						activation	= 0.0f,
						bias		= UnityEngine.Random.value,
						f = actfunc.f,
						d = actfunc.d
					}
					;
			this.neurons = q.ToArray();
		}
	}
	
	//[Serializable]
	public class N
	{
		public LayerUnit[]	layers;
		public float		learning_rate;


		public N( IEnumerable<int> neuron_length_per_layers, IEnumerable<NeuronUnit.IActivationFunction> actfuncs )
		{
			create_layers( neuron_length_per_layers, actfuncs );
			init_links();
		}

		public void create_layers( IEnumerable<int> neuron_length_per_layers, IEnumerable<NeuronUnit.IActivationFunction> actfuncs )
		{
			this.layers = Enumerable.Zip( neuron_length_per_layers, actfuncs, (x,y)=>(num:x, func:y) )
				.Select( x => new LayerUnit(x.num, x.func) )
				.ToArray()
				;
		}

		public void propergate_forward()
		{
			foreach( var n in layers.Skip( 1 ).SelectMany( layer => layer.neurons ) )
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
				n.learn( this.learning_rate );
			}
		}

		public void set_input_values( float[] input_values )
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
				node.caluclate_delta_value( correct_value );
			}
		}
		public void set_correct_values_cross_entropy( IEnumerable<float> correct_values )
		{
			var output_nodes = this.layers.Last().neurons;
			var q = Enumerable.Zip( correct_values, output_nodes, (c, n) => (c, n) );
			foreach( var (correct_value, node) in q )
			{
				node.forwards[ 0 ].delta_weighted = -( correct_value / node.activation ) + ( 1.0f - correct_value ) / ( 1.0f - node.activation );
				if( float.IsNaN(node.forwards[ 0 ].delta_weighted) ) node.forwards[ 0 ].delta_weighted = 0.0f;
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

