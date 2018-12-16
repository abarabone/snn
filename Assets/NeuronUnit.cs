using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeuronUnit : MonoBehaviour
{

	public float	limit;

	(NeuronUnit n, float ratio)[]	forwardLinks;


    // Start is called before the first frame update
    void Start()
    {
        var tf	= this.transform;
		tf.position = Random.insideUnitSphere * 100.0f;

		void setPosition()
		{

		}
		void setLinks()
		{

		}
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
