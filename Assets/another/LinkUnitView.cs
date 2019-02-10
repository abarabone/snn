using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using a;

public class LinkUnitView : MonoBehaviour
{
	
	public NeuronLinkUnit	value;

	LineRenderer	linkRenderer;
	MaterialPropertyBlock	mpb;

	readonly int	colorPropId = Shader.PropertyToID( "_Color" );


	public void Init( NeuronLinkUnit link, NeuronUnitView startNode, NeuronUnitView endNode )
	{
		var tf	= this.transform;
		var r	= this.GetComponent<LineRenderer>();

		r.positionCount = 2;
		r.SetPosition( 0, startNode.transform.position );
		r.SetPosition( 1, endNode.transform.position );
		r.startWidth	= 0.05f;
		r.endWidth		= 0.05f;

		this.value	= link;
		this.mpb	= new MaterialPropertyBlock();
		this.linkRenderer	= r;
    }

	void OnWillRenderObject()
	{
		this.mpb.SetColor( this.colorPropId, this.value.weight.ToColor() );
		this.linkRenderer.SetPropertyBlock( this.mpb );
	}
	
}
