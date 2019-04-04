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
		public class StepWide : IActivationFunction
		{
			float	sum_value_;
			
			public float f( float sum_value )
			{
				this.sum_value_ = sum_value;
				return Math.Sign(sum_value);
			}
			public float d()
			{
				return Math.Sign(this.sum_value_);
			}
		}

	}

	namespace LossFunctions
	{

		public interface ILossFunction
		{
			float f( IEnumerable<float> input_values, IEnumerable<float> correct_values );
			float d( float activation_value, float correct_value );
		}
		public class MSE : ILossFunction
		{
			public float f( IEnumerable<float> input_values, IEnumerable<float> correct_values )
			{
				throw new NotImplementedException();
			}
			public float d( float activation_value, float correct_value )
			{
				return activation_value - correct_value;
			}
		}
		public class CrossEntropy : ILossFunction
		{
			public float f( IEnumerable<float> input_values, IEnumerable<float> correct_values )
			{
				throw new NotImplementedException();
			}
			public float d( float activation_value, float correct_value )
			{
				var loss_delta =
					-( (double)correct_value / activation_value )
					+ ( 1.0d - correct_value ) / ( 1.0d - activation_value );
				
				if( double.IsNaN(loss_delta) ) loss_delta = 0.0f;
				
				return (float)loss_delta;
			}
		}

	}

	namespace OutputProcessFunctions
	{

		public interface IOutputFunction
		{
			void forward_propergate( IEnumerable<NeuronUnit> dst_nodes );
			void back_propergate( IEnumerable<NeuronUnit> nodes );
		}

		public class StabOutput : IOutputFunction
		{
			public void back_propergate( IEnumerable<NeuronUnit> nodes )
			{}
			public void forward_propergate( IEnumerable<NeuronUnit> dst_nodes )
			{}
		}

		public class SoftMax : IOutputFunction
		{
			public void forward_propergate( IEnumerable<NeuronUnit> nodes )
			{
				var q_acts	= nodes.Select( node => node.backs[0].back.activation );
				var max		= q_acts.Max();
				var exps	= q_acts.Select( a => Math.Exp(a - max) );
				var sum		= exps.Sum();
				var q		= Enumerable.Zip( exps, nodes, (x, node) => (x / sum, node) );
				foreach( var (a, node) in q )
				{
					node.activation = (float)a;
				}
			}
			public void back_propergate( IEnumerable<NeuronUnit> nodes )
			{
				foreach( var (back_link, forward_link) in nodes.Select( node => (node.backs[0], node.forwards[0]) ) )
				{
					//Debug.Log( $"{forward_link.delta_weighted} {back_link.back.activation} {back_link.forward.activation}" );
					back_link.delta_weighted = forward_link.delta_weighted * back_link.back.activation;
				}
			}
		}
	}

}
