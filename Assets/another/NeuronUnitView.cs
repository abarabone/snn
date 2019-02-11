﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using a;

public class NeuronUnitView : MonoBehaviour
{
	
	public NeuronUnit	value;
	
	Renderer				nodeRenderer;
	MaterialPropertyBlock	mpb;

	readonly int	colorPropId = Shader.PropertyToID( "_Color" );


	public void Init( NeuronUnit n )
	{
		this.value	= n;
		this.mpb	= new MaterialPropertyBlock();
		this.nodeRenderer	= GetComponent<Renderer>();
    }
	
	void OnWillRenderObject()
	{
		this.mpb.SetColor( this.colorPropId, this.value.activation.ToColor() );
		this.nodeRenderer.SetPropertyBlock( this.mpb );
	}
	
}