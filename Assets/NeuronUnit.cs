using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NeuronUnit : MonoBehaviour
{

	[SerializeField]
	float	volume;

	//(NeuronUnit n, float v)[]	forwardLinks;
	[SerializeField]
	LinkUnit[]	forwardLinks;
	[SerializeField]
	List<LinkUnit>	backLinks;

	[HideInInspector]
	public ZoneBase	ParentZone;

	//public float	UnitLinkRadius;
	//public float	UnitLinkArmDistance;

	Vector3	linkerCenter;
	Vector3	position;


	Renderer				rndrr;
	MaterialPropertyBlock	mpb;
	static readonly int		colorNameId = Shader.PropertyToID( "_Color" );


	public void Emit( float v )
	{
		addVolume();

		if( !isEmit() ) return;
		
		forwardPropagation();

		return;


		void addVolume()
		{
			this.volume += v;
		}
		bool isEmit()
		{
			return this.volume >= this.backLinks.Count * 0.3f;
		}
		void forwardPropagation()
		{
			foreach( var link in this.forwardLinks )
			{
				link.end.Emit( link.value );
			}
		}
	}

	public void Effect( float v )
	{

	}

	public void Clear()
	{
		this.volume = 0.0f;
	}


	protected void Awake()
	{
		initValues();

		return;


		void initValues()
		{
			this.ParentZone	= this.GetComponentInParent<ZoneBase>();
			this.rndrr		= this.GetComponent<Renderer>();
			this.mpb		= new MaterialPropertyBlock();
			this.backLinks	= new List<LinkUnit>();
		}
	}


	protected void Start()
    {
		
		initLocation();

		setLinks();
		
		return;


		void initLocation()
		{
			this.position		= this.transform.position;
			this.linkerCenter	= this.ParentZone.CalucNeuronForwardosition( this.position, this.ParentZone.UnitLinkArmDistance );
		}

		void setLinks()
		{
			this.forwardLinks = Physics.OverlapSphere( this.linkerCenter, this.ParentZone.UnitLinkRadius )
				.Select( col => col.GetComponent<NeuronUnit>() )
				.Where( n => n != null )
				.Where( n => n != this )
				.Where( n => this.ParentZone.IsLinkTarget(n.ParentZone) )
				.Select( n => new LinkUnit{ start = this, end = n, value = Random.value } )
				.ToArray()
				;
			foreach( var link in this.forwardLinks )
			{
				link.end.backLinks.Add( link );
			}
		}
    }


	protected void Update()
	{
		showState();
		showLink();
		//this.volume = 0;
	}

	
	void showState()
	{
		this.mpb.SetColor( NeuronUnit.colorNameId, this.volume.ToColor() );
		this.rndrr.SetPropertyBlock( this.mpb );
	}

	void showLink()
	{
		Debug.DrawLine( this.position, this.linkerCenter, this.volume.ToColor() );
		foreach( var link in forwardLinks )
		{
			Debug.DrawLine( this.linkerCenter, link.end.position, link.value.ToColor() );
		}
	}

	
}


public class LinkUnit
{
	public NeuronUnit	start;
	public NeuronUnit	end;

	public float		value;
}


static class ColorExtentions
{
	static public Color ToColor( ref this Vector3 v )
	{
		return new Color( v.x, v.y, v.z );
	}
	static public Color ToColor( this float f )
	{
		var v = Vector3.one * f;
		return v.ToColor();
	}
}
