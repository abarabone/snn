using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Neuron.ActiveEmit
{
	
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

		LineRenderer	armRenderer;
		LineRenderer[]	linkRenderers;

		Renderer				nodeRenderer;
		MaterialPropertyBlock	mpb;
		static readonly int		colorNameId = Shader.PropertyToID( "_Color" );


		public void Emit( float v )
		{

			if( IsEmit() ) return;

			addVolume();

			if( !IsEmit() ) return;
		
			forwardPropagation();

			return;


			void addVolume()
			{
				this.volume += v;
				this.volume = Mathf.Clamp01( this.volume );
			}
			void forwardPropagation()
			{
				foreach( var link in this.forwardLinks )
				{
					link.end.Emit( link.value );
				}
			}
		}

		public void Teach( bool isSuccess )
		{
			
			var ii = this.IsEmit() ? this.volume : this.volume ;
			var aa = this.IsEmit() ? this.volume : this.volume ;

			var v = isSuccess ? this.volume : -this.volume;//+0.01f : -0.01f;

			backPropagation();

			return;


			void backPropagation()
			{
				foreach( var link in this.backLinks )
				{
					var vv = link.start.IsEmit() ? v : v * -1.0f;

					link.value += v;
					link.start.Teach( isSuccess );
				}
			}
		}

		bool IsEmit()
		{
			return this.volume >= 1.0f;
		}

		public void Clear()
		{
			this.volume = 0.0f;
		}


		protected void Awake()
		{
			initValues();

			initLocation();

			return;


			void initValues()
			{
				this.ParentZone	= this.GetComponentInParent<ZoneBase>();
				this.nodeRenderer		= this.GetComponent<Renderer>();
				this.mpb		= new MaterialPropertyBlock();
				this.backLinks	= new List<LinkUnit>();
			}
			void initLocation()
			{
				this.position		= this.transform.position;
				this.linkerCenter	= this.ParentZone.CalucNeuronForwardosition( this.position, this.ParentZone.UnitLinkArmDistance );
			}

		}


		void OnMouseDown()
		{
			Debug.Log(this.name);

			Teach( !Input.GetKey(KeyCode.LeftShift) );

		}


		protected void Start()
		{
		
			setLinks();
			createLines();
		
			return;


			void setLinks()
			{
				this.forwardLinks = Physics.OverlapSphere( this.linkerCenter, this.ParentZone.UnitLinkRadius )
					.Select( col => col.GetComponent<NeuronUnit>() )
					.Where( n => n != null )
					.Where( n => n != this )
					.Where( n => this.ParentZone.IsLinkTarget(n.ParentZone) )
					.Select( n => new LinkUnit{ start = this, end = n, value = Random.value * 2.0f - 1.0f } )
					.ToArray()
					;
				foreach( var link in this.forwardLinks )
				{
					link.end.backLinks.Add( link );
				}
			}

			void createLines()
			{

				var tf = this.transform;

				this.armRenderer	= createLine( this.position, this.linkerCenter );

				this.linkRenderers	= this.forwardLinks
					.Select( link => createLine(this.linkerCenter, link.end.position) )
					.ToArray()
					;

				LineRenderer createLine( Vector3 start, Vector3 end )
				{
					var go	= new GameObject();
					go.transform.SetParent( tf, worldPositionStays:true );
					var r	= go.AddComponent<LineRenderer>();
					r.material	= nodeRenderer.material;
					r.positionCount = 2;
					r.SetPosition( 0, start );
					r.SetPosition( 1, end );
					r.startWidth	= 0.05f;
					r.endWidth		= 0.05f;
					return r;
				}
			}
		}


		void Update()
		{
			this.volume = 0;
		}
		void OnWillRenderObject()
		{
			showState();
			showLink();
		}

	
		void showState()
		{
			this.mpb.SetColor( NeuronUnit.colorNameId, this.volume.ToColor() );
			this.nodeRenderer.SetPropertyBlock( this.mpb );
		}

		void showLink()
		{
		
			setLineColor( this.armRenderer, this.volume.ToColor() );

			foreach( var x in forwardLinks.Select( (link, i) => (link, i) ) )
			{
				setLineColor( this.linkRenderers[x.i], x.link.value.ToColor() );
			}

			void setLineColor( LineRenderer line, Color color )
			{
				this.mpb.SetColor( NeuronUnit.colorNameId, color );
				line.SetPropertyBlock( this.mpb );
				//line.startColor = color;
				//line.endColor	= color;
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
			var v = new Vector3( Mathf.Clamp01(-f), f, Mathf.Clamp01(f) );

			return v.ToColor();
		}
	}

}