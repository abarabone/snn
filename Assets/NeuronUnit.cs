using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NeuronUnit : MonoBehaviour
{

	public float	limit;

	(NeuronUnit n, float v)[]	forwardLinks;


	public Zone	selfZone;
	public Zone	outZone;
	


	public void Spark( float v )
	{
		if( v >)

		foreach( var node in forwardLinks )
		{
			if( node.v )
		}
	}
	public void 

	
    void Start()
    {
        var tf	= this.transform;

		setPosition();
		setLinks();
		
		return;

		void setPosition()
		{
			tf.position = selfZone.Tf.position + Random.insideUnitSphere * selfZone.Radius;
		}

		void setLinks()
		{
			this.forwardLinks = Physics
				.OverlapSphere( tf.position, selfZone.linkRadius )
				.Select( x => (x.GetComponent<NeuronUnit>(), 0.0f) )
				.ToArray()
				;
		}
    }
	
}
