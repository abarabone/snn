using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Neuron.ActiveEmit
{
	public class NeuronUnitTrigger : NeuronUnit
	{

		public float	trigger;


		//new void Awake() => base.Awake();
		//new void Start() => base.Start();

		void LateUpdate()
		{
			
			this.Clear();
			this.Emit( trigger );

		}

		void OnMouseDown()
		{

			this.trigger = this.trigger > 0.0f ? 0.0f : 1.0f;

		}

	}
}
