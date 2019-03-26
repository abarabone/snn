using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Neuron
{
	
	public class NeuronUnit : MonoBehaviour
	{
	/*
		[HideInInspector]
		public ZoneBase	ParentZone;
		
		[SerializeField]
		float	volume;
		
		List<(NeuronUnit n, float v)> backLinks;


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
		
			var v = isSuccess ? +10.1f : -10.1f;

			backPropagation();

			return;


			void backPropagation()
			{
				foreach( var link in this.backLinks )
				{
					if( !link.start.IsEmit() ) continue;

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
			inifloats();

			return;


			void inifloats()
			{
				this.ParentZone		= this.GetComponentInParent<ZoneBase>();
				this.nodeRenderer	= this.GetComponent<Renderer>();
				this.mpb			= new MaterialPropertyBlock();
				this.backLinks		= new List<LinkUnit>();
			}
		}


		void OnMouseUp()
		{
			Debug.Log(this.name);

			Teach( Input.GetMouseButton(0) );

		}


		protected void Start()
		{
		
			initLocation();

			setLinks();
			createLines();
		
			return;


			void initLocation()
			{
				this.position		= this.transform.position;
				this.linkerCenter	= this.ParentZone.CalucNeuronForwardosition( this.position, this.ParentZone.UnitLinkArmDistance );
			}

			void setLinks()
			{
				this.forwardLinks = Physics.OverlapSphere( this.linkerCenter, this.ParentZone.UnitLinkRadius )
					.Select( col => col.GetComponent<NeuronUnit_Act>() )
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


		protected void LateUpdate()
		{
			showState();
			showLink();
			this.volume = 0;
		}

	
		void showState()
		{
			this.mpb.SetColor( NeuronUnit_Act.colorNameId, this.volume.ToColor() );
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
				this.mpb.SetColor( NeuronUnit_Act.colorNameId, color );
				line.SetPropertyBlock( this.mpb );
				//line.startColor = color;
				//line.endColor	= color;
			}
		}

	*/
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