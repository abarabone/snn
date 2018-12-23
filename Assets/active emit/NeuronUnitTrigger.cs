using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Neuron.ActiveEmit
{
	public class NeuronUnitTrigger : NeuronUnit
	{


		new void Awake() => base.Awake();
		new void Start() => base.Start();

		new void LateUpdate()
		{
			
			this.Emit( 1.0f );

		}

	}
}
