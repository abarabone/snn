using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NnView : MonoBehaviour
{

	public int[]	neuron_length_per_layers;

	[SerializeField]
	a.N	nn;

    void Start()
    {
        this.nn	= new a.N( this.neuron_length_per_layers );
    }
	
}
