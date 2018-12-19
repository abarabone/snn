using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NeuronUnit : MonoBehaviour
{

	float	volume;

	[SerializeField]
	(NeuronUnit n, float v)[]	forwardLinks;

	[HideInInspector]
	public ZoneBase	ParentZone;

	public float	UnitLinkRadius;
	public float	UnitLinkArmDistance;

	Vector3	linkerCenter;
	Vector3	position;


	MaterialPropertyBlock	mpb;
	static readonly int		colorNameId = Shader.PropertyToID( "Color_" );


	public void Emit( float v )
	{

		if( !isEmit() ) return;
		
		forwardPropagation();

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
	}

	private void Awake()
	{
		initValues();

		return;

		void initValues()
		{
			this.ParentZone	= this.GetComponentInParent<ZoneBase>();
			this.mpb		= new MaterialPropertyBlock();
			this.GetComponent<MeshRenderer>().SetPropertyBlock( this.mpb );
		}
	}


	void Start()
    {
		
		initLocation();

		setLinks();
		
		return;


		void setLinks()
		{
			this.forwardLinks = Physics.OverlapSphere( this.linkerCenter, this.UnitLinkRadius )
				.Select( col => col.GetComponent<NeuronUnit>() )
				.Where( n => n != null )
				.Where( n => n != this )
				.Where( n => this.ParentZone.IsLinkTarget(n.ParentZone) )
				.Select( n => (n, 0.0f) )
				.Select( x => (x.n, Random.value))// v のランダム初期化
				.ToArray()
				;
		}
    }


	private void Update()
	{
		showState();
		showLink();
	}



	void initLocation()
	{
        this.position		= this.transform.position;
		this.linkerCenter	= this.ParentZone.CalucNeuronForwardosition( this.position, this.UnitLinkArmDistance );
	}

	
	void showState()
	{
		var vol = this.volume;
		this.mpb.SetColor( NeuronUnit.colorNameId, new Color(vol,vol,vol) );
	}
	void showLink()
	{
		Debug.DrawLine( this.position, this.linkerCenter );
		foreach( var link in forwardLinks )
		{
			Debug.DrawLine( this.linkerCenter, link.n.position );
		}
	}
}
