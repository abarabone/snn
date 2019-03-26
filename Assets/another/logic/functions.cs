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
			//float sum( IEnumerable<(float activation, float weight)> values, float bias );
			float f( float sum_value );
			float d();
		}
		public class Identity : IActivationFunction
		{
			//public float sum( IEnumerable<(float activation, float weight)> values, float bias )
			//{
			//	return values.Select( x => x.activation * x.weight ).Sum() + bias;
			//}
			public float f( float sum_value ) => sum_value;
			public float d() => 1.0f;
		}
		public class Sigmoid : IActivationFunction
		{
			public float	activation_;

			//public float sum( IEnumerable<(float activation, float weight)> values, float bias )
			//{
			//	return values.Select( x => x.activation * x.weight ).Sum() + bias;
			//}
			public float f( float sum_value ) => this.activation_ = (float)(1.0d / ( 1.0d + Math.Exp(-sum_value) ));
			public float d() => this.activation_ * ( 1.0f - this.activation_ );
			//public float d( float sum_value, float activation_value ) => f(sum_value) * ( 1.0f - f(sum_value) );
		}
		public class ReLU : IActivationFunction
		{
			float	sum_value_;
			
			//public float sum( IEnumerable<(float activation, float weight)> values, float bias )
			//{
			//	this.sum_value_ = values.Select( x => x.activation * x.weight ).Sum() + bias;
			//	return this.sum_value_;
			//}
			public float f( float sum_value ) => (this.sum_value_ = sum_value) > 0.0f ? sum_value : 0.0f;
			public float d() => this.sum_value_ > 0.0f ? 1.0f : 0.0f;
		}
		public class Tanh : IActivationFunction
		{
			float	sum_value_;

			//public float sum( IEnumerable<(float activation, float weight)> values, float bias )
			//{
			//	this.sum_value_ = values.Select( x => x.activation * x.weight ).Sum() + bias;
			//	return this.sum_value_;
			//}
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
		//public class SoftMax : IActivationFunction
		//{
		//	public int	class_index;
		//	float		activation_;
		//	static int	seed_ = 0;
		//	public SoftMax() => class_index = seed_++ - 1;

		//	public float sum( IEnumerable<(float activation, float weight)> values, float bias )
		//	{
		//		var max = values.Select( x => x.activation * x.weight ).Max();
		//		return values.Select( x => (float)Math.Exp(x.activation * x.weight - max) ).ElementAt(this.class_index)
		//			/ values.Select( x => (float)Math.Exp(x.activation * x.weight - max) ).Sum();
		//	}
		//	public float f( float sum_value ) => this.activation_ = sum_value;
		//	public float d() => this.activation_ * ( 1.0f - this.activation_ );
		//}
		public class aaa : IActivationFunction
		{
			float	sum_value_;

			//public float sum( IEnumerable<(float activation, float weight)> values, float bias )
			//{
			//	this.sum_value_ = values.Select( x => x.activation * x.weight ).Sum() + bias;
			//	return this.sum_value_;
			//}
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
			void forward_propergate( IEnumerable<float> activations );
			void back_propergate(  );
		}

	}

}
