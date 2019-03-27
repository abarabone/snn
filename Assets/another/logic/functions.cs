using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

namespace nn
{

	namespace ActivationFunctions
	{

		public interface IActivationFunction
		{
			float f( float sum_value );
			float d();
		}
		public class Identity : IActivationFunction
		{
			public float f( float sum_value ) => sum_value;
			public float d() => 1.0f;
		}
		public class Sigmoid : IActivationFunction
		{
			public float	activation_;

			public float f( float sum_value ) => this.activation_ = (float)(1.0d / ( 1.0d + Math.Exp(-sum_value) ));
			public float d() => (float)( this.activation_ * ( 1.0d - this.activation_ ) );
		}
		public class ReLU : IActivationFunction
		{
			float	sum_value_;
			
			public float f( float sum_value ) => (this.sum_value_ = sum_value) > 0.0f ? sum_value : 0.0f;
			public float d() => this.sum_value_ > 0.0f ? 1.0f : 0.0f;
		}
		public class Tanh : IActivationFunction
		{
			float	sum_value_;
			
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
		public class aaa : IActivationFunction
		{
			float	sum_value_;
			
			public float f( float sum_value )
			{
				this.sum_value_ = sum_value;
				if( sum_value >=  0.5f ) return sum_value;
				if( sum_value <= -0.5f ) return sum_value;
				return 0.0f;
			}
			public float d()
			{
				if( this.sum_value_ >=  0.5f ) return 1.0f;
				if( this.sum_value_ <= -0.5f ) return 1.0f;
				return 0.0f;
			}
		}

	}
	
	//namespace LossFunctions
	//{

	//	public interface ILossFunction
	//	{
	//		float a( IEnumerable<float> activations, float bias, IActivationFunction activation_function );
	//		float f( IEnumerable<float> input_values, IEnumerable<float> correct_values );
	//		float d( );
	//	}
	//	public class MSE : ILossFunction
	//	{
	//		public float d()
	//		{
	//			throw new NotImplementedException();
	//		}
	//		public float f( IEnumerable<float> input_values, IEnumerable<float> correct_values )
	//		{
	//			throw new NotImplementedException();
	//		}
	//		public float a( IEnumerable<float> activations, float bias, IActivationFunction activation_function )
	//		{
	//			return activation_function.f( activations.Sum() + bias );
	//		}
	//	}
	//	public class CrossEntropy : ILossFunction
	//	{
	//		public float d()
	//		{
	//			throw new NotImplementedException();
	//		}
	//		public float f( IEnumerable<float> input_values, IEnumerable<float> correct_values )
	//		{
	//			throw new NotImplementedException();
	//		}
	//		public float a( IEnumerable<float> activations, float bias, IActivationFunction activation_function )
	//		{
	//			switch( activation_function )
	//			{
	//				case ActivationFunctions.Sigmoid	af:break;
	//				case SoftMax	af:break;
	//				case var		af:				return activation_function.f( activations.Sum() + bias );
	//			}
	//			return activations.Sum() + bias;
	//		}
	//	}
		
	//}

	namespace OutputProcessFunctions
	{

		public interface IOutputFunction
		{
			void forward_propergate( IEnumerable<NeuronUnit> dst_nodes );
			void back_propergate( IEnumerable<NeuronUnit> nodes );
		}

		public class SoftMax : IOutputFunction
		{
			public void forward_propergate( IEnumerable<NeuronUnit> nodes )
			{
				var q_acts	= nodes.SelectMany( node => node.backs ).Select( link => link.back.activation );
				var max		= q_acts.Max();
				var exps	= q_acts.Select( a => Math.Exp(a - max) );
				var sum		= exps.Sum();
				foreach( var (a, node) in Enumerable.Zip(exps, nodes, (x, node) => (x / sum, node)) )
				{
					node.activation = (float)a;
				}
			}
			public void back_propergate( IEnumerable<NeuronUnit> nodes )
			{
				var q_backs		= nodes.SelectMany( node => node.backs );
				var q_forwards	= nodes.SelectMany( node => node.forwards );
				foreach( var (back_link, forward_link) in Enumerable.Zip(q_backs, q_forwards, (x,y)=>(x,y)) )
				{
					back_link.delta_weighted = forward_link.delta_weighted;
				}
			}
		}
	}

}
