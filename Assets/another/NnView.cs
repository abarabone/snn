using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using a;

public class NnView : MonoBehaviour
{

	public int[]	NeuronLengthPerLayers;

	public LinkUnitView		LinkViewTemplate;
	public NeuronUnitView	NeuronViewTemplate;

	public float	LayerViewDistace;

	public readonly float	layerViewDistance	= 3.0f;
	public readonly float	nodeViewDistance	= 1.0f;

	[SerializeField]
	N	nn;
	
    void Start()
    {
        this.nn	= new N( this.NeuronLengthPerLayers );

		var q_nodes = from l in this.nn.layers.Select( (x,i) => (x,i) )
					  from n in l.x.neurons.Select( (x,i) => (x,i) )
					  select (node:n.x, li:l.i, ni:n.i)
					  ;
		foreach( var n in q_nodes )
		{
			var pos	= new Vector3( 0.0f, n.ni * this.nodeViewDistance, n.li * this.layerViewDistance );
			var node_view	= Instantiate<NeuronUnitView>( this.NeuronViewTemplate, pos, Quaternion.identity );
			node_view.Init( n.node );
		}
			foreach( var link in n.node.forwards )
			{
				var link_view	= Instantiate<LinkUnitView>( this.LinkViewTemplate, pos, Quaternion.identity );
				var endpos		= new Vector3( 0.0f, )
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