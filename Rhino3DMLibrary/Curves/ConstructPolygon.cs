﻿using Core;
using System;
using System.Windows;
using Verse3;
using Verse3.VanillaElements;
using Rhino;
using Rhino.Geometry;

namespace Rhino3DMLibrary
{
    public class ConstructPolygon : BaseComp
    {
        public ConstructPolygon() : base()
        {
        }
        public ConstructPolygon(int x, int y) : base(x, y)
        {
        }

        public override void Compute()
        {
            ((Rhino.Geometry.PlaneSurface)this.ChildElementManager.GetData<GeometryBase>(0)).TryGetPlane(out Rhino.Geometry.Plane plane);
            double radius = this.ChildElementManager.GetData<double>(1, 50);
            double sideCount = this.ChildElementManager.GetData<double>(2, 10);

            if (plane.IsValid)
            {
                Circle circle = new Circle(plane, radius);
                Polyline polyline = Polyline.CreateInscribedPolygon(circle, Math.Abs((int)sideCount));
                GeometryBase geo = polyline.ToPolylineCurve();
                this.ChildElementManager.SetData<GeometryBase>(geo, 0);
            }

        }

        public override CompInfo GetCompInfo() => new CompInfo(this, "Construct Polygon", "Line", "Curve");

        private RhinoGeometryDataNode nodeBlockX;
        private NumberDataNode nodeBlockY;
        private NumberDataNode nodeBlockZ;
        private RhinoGeometryDataNode nodeBlockResult;
        public override void Initialize()
        {
            nodeBlockX = new RhinoGeometryDataNode(this, NodeType.Input);
            this.ChildElementManager.AddDataInputNode(nodeBlockX, "Plane");

            nodeBlockY = new NumberDataNode(this, NodeType.Input);
            this.ChildElementManager.AddDataInputNode(nodeBlockY, "Radius");

            nodeBlockY = new NumberDataNode(this, NodeType.Input);
            this.ChildElementManager.AddDataInputNode(nodeBlockY, "Side Count");

            nodeBlockResult = new RhinoGeometryDataNode(this, NodeType.Output);
            this.ChildElementManager.AddDataOutputNode(nodeBlockResult, "Polygon", true);
        }
    }
}
