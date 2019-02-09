using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using a;

public class NeuronUnitView : MonoBehaviour
{

	[SerializeField]
	NeuronUnit	value;
	
	Renderer				nodeRenderer;
	MaterialPropertyBlock	mpb;
	static readonly int		colorNameId = Shader.PropertyToID( "_Color" );

    // Start is called before the first frame update
    void Start()
    {
				this.mpb		= new MaterialPropertyBlock();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
