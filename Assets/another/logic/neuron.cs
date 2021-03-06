﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace nn
{
	
	using ActivationFunctions;
	//using LossFunctions;
	using OutputProcessFunctions;

	
	//[Serializable]
	public class NeuronUnit
	{
		public NeuronLinkUnit[]	forwards	= new NeuronLinkUnit [] { };
		public NeuronLinkUnit[]	backs		= new NeuronLinkUnit [] { };

		public float	bias;
		public float	activation;
		//public float	sum_value;
		//public float	propergated_value;
		
		public IActivationFunction	af;

		public float	sign;

		public void activate()
		{
			if( af == null ) return;

			var sum_value = this.backs
				.Sum( link => (double)link.weight * link.back.activation )
				+ this.bias
				;
			this.activation = this.af.f( (float)sum_value ) * this.sign;
		}

		public void learn( float learning_rate )
		{
			if( af == null ) return;

			var delta_value = retrieve_delta_from_forwards_();
			if( double.IsNaN( delta_value ) ) delta_value = 0.0d;

			modify_to_backs_( delta_value );

			//Debug.Log( $"a:{this.activation} b:{this.bias} {this.GetHashCode()}" );
			return;


			/// 出力側から重みのかけられたδを取得し、合計する。
			double retrieve_delta_from_forwards_()
			{
				var sum_delta_forwards = this.forwards
					.Sum( link => (double)link.delta_weighted )
					;
				return sum_delta_forwards * this.af.d();
			}

			/// 入力側へδを伝える。
			void modify_to_backs_( double delta_value_ )
			{
				foreach( var link in this.backs )
				{
					link.delta_weighted	=  (float)( link.weight * delta_value_ );// 更新前の重みを使用する。
					link.weight			-= (float)( delta_value_ * link.back.activation * learning_rate ) * this.sign;
					//Debug.Log( $"w:{link.weight} b:{this.bias} {link.GetHashCode()}" );
				}
				this.bias	-= (float)( delta_value_ * learning_rate ) * this.sign;
			}
		}
		public void learn2( float learning_rate )
		{
			//var sum_forward_propergated_value = this.forwards
			//	.Select( link => link.weight * link.forward.propergated_value )
			//	.Sum();

			//foreach( var link in this.forwards )
			//{
			//	var modify = link.weight * link.forward.activation;
			//	link.weight -= modify * learning_rate;
			//}

			//if( this.forwards.Length != 0 )
			//{
			//	this.propergated_value = sum_forward_propergated_value * d( this.sum_value, this.activation );
			//}

			//this.bias -= this.propergated_value * learning_rate;
		}

		public void set_loss_delta( float loss_delta )
		{
			this.forwards[0].delta_weighted	= loss_delta;
		}
		
	}
}