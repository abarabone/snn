using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using nn;

public class NnView : MonoBehaviour
{

	public int[]	NeuronLengthPerLayers;
	public ActivationFunction[]	funcs;

	public NeuronUnitView	NeuronViewTemplate;
	public LinkUnitView		LinkViewTemplate;
	
	public float	LayerViewDistance;
	public float	NodeViewDistance;

	//[SerializeField]
	[HideInInspector]
	public N	value;

	public int		learnFreq;
	public float	learningRate;

	void Awake()
	{
		var tf	= transform;
		create_nn_view_();
		create_node_views_();
		create_link_views_();

		return;

		
		void create_nn_view_()
		{
			this.value	= new N( this.NeuronLengthPerLayers, funcs.Select( x => x.GetActivationFunction() ) );
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
				)
				;

			foreach( var n in q_nodes )
			{
				var ntf	= n.view.transform;
				n.view.Init( n.value );
				ntf.SetParent( tf, worldPositionStays:true );
				n.view.name = $"{ntf.position.z / this.LayerViewDistance} : {ntf.position.y / this.NodeViewDistance}";
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
				)
				;
			foreach( var link in q_links )
			{
				link.view.Init( link.value, link.nv_st, link.nv_ed );
				link.view.transform.SetParent( tf, worldPositionStays:true );
				link.view.name = $"{node_view_dict[link.value.back].name} - {node_view_dict[link.value.forward].name}";
			}
		}
	}

	void OnEnable()
    {
		Teaching( learnFreq );

		this.value.propergate_forward();
    }

	
	public void Teaching( int freq )
	{
		this.value.learning_rate = this.learningRate;

		foreach( var i in Enumerable.Range(0, freq) )
		{
			var rnds = make_random_values_( this.value.layers.First().neurons.Length ).ToArray();
			this.value.set_input_values( rnds );
			this.value.propergate_forward();
			//Debug.Log(
			//	this.value.layers.First().neurons.Select( x => x.activation.ToString() ).Aggregate( ( x, y ) => x + " " + y )
			//	+ " / " +
			//	Enumerable.Range( 1, rnds.Count() ).Select( x => x == (int)rnds.Sum() ? 1.0f : 0.0f ).Select( x => x.ToString() ).Aggregate( ( x, y ) => x + " " + y )
			//);

			//Debug.Log( $"{rnds.Sum()} {this.value.layers.Last().neurons.First().activation}" );
			//this.value.set_correct_values_cross_entropy( rnds );
			//this.value.set_correct_values( rnds );
			//this.value.set_correct_values( new[] { rnds.Sum() >= rnds.Length * 0.5f ? 1.0f : 0.0f } );
			this.value.set_correct_values_cross_entropy( Enumerable.Range( 1, rnds.Count() ).Select( x => x == (int)rnds.Sum() ? 1.0f : 0.0f ) );
			this.value.propergate_back();
		}
		return;

		IEnumerable<float> make_random_values_( int length )
		{
			return Enumerable.Range(0, length).Select( i => Random.value >= 0.5f ? 1.0f : 0.0f );
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