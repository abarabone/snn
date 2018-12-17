using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NeuronUnit : MonoBehaviour
{

	float	volume;

	[SerializeField]
	(NeuronUnit n, float v)[]	forwardLinks;

	Zone zone;

	Vector3	forward;// 軸索の方向、あとで入れるかも（それぞれが一番近い出力ゾーンに向かう、でいいかな？遠い場合は平均？）


	MaterialPropertyBlock	mpb;
	static readonly int		colorNameId = Shader.PropertyToID( "Color_" );


	public void Emit( float v )
	{

		if( !isEmit() ) return;
		
		forwardPropagation();
		showState();

		return;


		bool isEmit()
		{
			this.volume += v;

			return this.volume >= 1.0f;
		}
		void forwardPropagation()
		{
			foreach( var node in this.forwardLinks )
			{
				node.n.Emit( node.v );
			}
		}
		void showState()
		{
			var vol = this.volume;
			this.mpb.SetColor( NeuronUnit.colorNameId, new Color(vol,vol,vol) );
		}
	}

	private void Awake()
	{

        var tf	= this.transform;

		this.zone	= this.GetComponentInParent<Zone>();
		this.mpb	= new MaterialPropertyBlock();
		this.GetComponent<MeshRenderer>().SetPropertyBlock( this.mpb );
		
	}

	void Start()
    {

        var tf	= this.transform;

		setLinks();
		
		return;


		void setLinks()
		{
			this.forwardLinks = Physics.OverlapSphere( tf.position, this.zone.linkRadius )
				.Select( x => (n:x.GetComponent<NeuronUnit>(), v:0.0f) )
				.Select( x => (x.n, Random.value))// v のランダム初期化
				.ToArray()
				;
		}
    }
	
}
