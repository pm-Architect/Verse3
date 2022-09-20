using Core;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse3.VanillaElements;

namespace RhinoCommonLibrary
{
    public class RhinoGeometryDataNode : DataNodeElement<GeometryBase>
    {
        public RhinoGeometryDataNode(IRenderable parent, NodeType type = NodeType.Unset) : base(parent, type)
        {
        }
    }
}
