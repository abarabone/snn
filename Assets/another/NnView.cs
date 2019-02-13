using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using a;

public class NnView : MonoBehaviour
{

	public int[]	NeuronLengthPerLayers;

	public NeuronUnitView	NeuronViewTemplate;
	public LinkUnitView		LinkViewTemplate;
	
	public float	LayerViewDistance;
	public float	NodeViewDistance;

	[SerializeField]
	public N	value;


	void Awake()
	{
		var tf	= transform;
		create_nn_view_();
		create_node_views_();
		create_link_views_();

		return;

		
		void create_nn_view_()
		{
			this.value	= new N( this.NeuronLengthPerLayers );
		}

		void create_node_views_()
		{
			var q_nodes =
				from l in this.value.layers.Select( (x,i) => (x,i) )
				from n in l.x.neurons.Select( (x,i) => (x,i) )
				let pos = new Vector3( 0.0f, n.i * this.NodeViewDistance, l.i * this.LayerViewDistance )
				select (
					value:	n.x,
					view:	Instantiate( this.NeuronViewTemplate, pos, Quaternion.identity )
				);
			foreach( var n in q_nodes )
			{
				n.view.Init( n.value );
				n.view.transform.SetParent( tf, worldPositionStays:true );
				n.view.name = n.view.transform.position.y.ToString() + n.view.transform.position.z.ToString();
			}
		}
        
		void create_link_views_()
		{
			var node_view_dict = GetComponentsInChildren<NeuronUnitView>()
				//.Select( nv => {Debug.Log(nv.name + nv.value.forwards[0].weight); return nv;})
				.ToDictionary( keySelector: nv => nv.value, elementSelector: nv => nv )
				;

			var q_links =
				from nv in node_view_dict.Values
				from x in nv.value.forwards
				where x.back != null && x.forward != null
				let nv_st	= node_view_dict[ x.back ]
				let nv_ed	= node_view_dict[ x.forward ]
				select (
					value:	x,
					view:	Instantiate( this.LinkViewTemplate, nv_st.transform.position, Quaternion.identity ),
					nv_st,
					nv_ed
				);
			foreach( var link in q_links )
			{
				link.view.Init( link.value, link.nv_st, link.nv_ed );
				link.view.transform.SetParent( tf, worldPositionStays:true );
			}
		}
	}

	void Start()
    {
		Teaching( 1000 );

		this.value.propergate_forward();
    }
	
	void Teaching( int freq )
	{
		foreach( var i in Enumerable.Range(0, freq) )
		{
			var rnds = make_random_values_( this.value.layers.First().neurons.Length );
			this.value.set_input_values( rnds );
			this.value.propergate_forward();

			this.value.set_correct_values( new[] { rnds.Sum() >= 3.0f ? 1.0f : 0.0f } );
			this.value.propergate_back();
		}
		return;

		float[] make_random_values_( int length )
		{
			Random.InitState( length );
			return Enumerable.Range(0, length).Select( i => Random.value > 0.5f ? 1.0f : 0.0f ).ToArray();
		}
	}
}


static class ColorExtentions
{
	static public Color ToColor( ref this Vector3 v )
	{
		return new Color( v.x, v.y, v.z );
	}
	static public Color ToColor( this float f )
	{
		var v = new Vector3( Mathf.Clamp01(-f), Mathf.Clamp01(f), Mathf.Clamp01(f) );

		return v.ToColor();
	}
}