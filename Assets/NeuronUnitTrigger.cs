using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeuronUnitTrigger : NeuronUnit
{


	new void Awake() => base.Awake();
	new void Start() => base.Start();

	new void Update()
	{

		this.Clear();

		this.Emit( 1.0f );


		base.Update();
	}

}
