using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using a;

public class LinkUnitView : MonoBehaviour
{

	[SerializeField]
	NeuronLinkUnit	value;


    // Start is called before the first frame update
    void Start()
    {
	}
	public void Init(  )
		var tf	= this.transform;
		var r	= this.GetComponent<LineRenderer>();
		r.material		= tf.parent.GetComponent<Renderer>().material;
		r.positionCount = 2;
		r.SetPosition( 0, this.value.forward. );
		r.SetPosition( 1, end );
		r.startWidth	= 0.05f;
		r.endWidth		= 0.05f;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
