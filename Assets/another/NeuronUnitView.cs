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

	readonly int	colorPropId = Shader.PropertyToID( "_Color" );


	public void Init( NeuronUnit n )
	{
		this.value	= n;
		this.mpb	= new MaterialPropertyBlock();
		this.nodeRenderer	= GetComponent<LineRenderer>();
    }
	
	void OnOnWillRenderObject()
	{
		this.mpb.SetColor( this.colorPropId, this.value.activation.ToColor() );
		this.nodeRenderer.SetPropertyBlock( this.mpb );
	}
	
}
