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
	public IZone ParentZone;

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
			this.ParentZone	= this.GetComponentInParent<IZone>();
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
			this.forwardLinks = Physics.OverlapSphere( this.linkerCenter, this.ParentZone.UnitLinkRadius )
				.Select( col => col.GetComponent<NeuronUnit>() )
				.Where( n => n != null )
				.Where( n => n != this )
				.Where( n =>
					this.ParentZone.IsLinkableSelfZone && n.ParentZone == this.ParentZone ||
					this.ParentZone.ForwardZones.Any( x => x == n.ParentZone )
				)
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
		this.linkerCenter	= this.position;

		if( this.ParentZone.ForwardZones.Length == 0 ) return;

		var nearestZonePoint = this.ParentZone.ForwardZones
			.Where( zone => zone != null )
			.Select( zone => zone.Shape.ClosestPoint(this.linkerCenter) )
			.Select( clpoint => (clpoint, dist:Vector3.Distance(this.linkerCenter, clpoint)) )
			.Aggregate( (pre, cur)=> pre.dist > cur.dist ? cur : pre )
			.clpoint
			;
		this.linkerCenter	= this.position + ( nearestZonePoint - this.position ).normalized * this.ParentZone.UnitLinkArmDistance;
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
