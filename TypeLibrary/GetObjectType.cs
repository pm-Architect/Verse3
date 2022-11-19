using Core;
using System;
using Verse3;
using Verse3.VanillaElements;

namespace MathLibrary
{
    public class GetObjectType : BaseComp
    {
        public GetObjectType() : base()
        {
        }
        public GetObjectType(int x, int y) : base(x, y)
        {
        }
        
        private GenericDataNode ObjectInput;
        private TypeDataNode TypeOutput;
        private BooleanDataNode IsArrayOutput;
        public override void Initialize()
        {
            //EVENT NODES

            //DATA NODES
            ObjectInput = new GenericDataNode(this, NodeType.Input);
            this.ChildElementManager.AddDataInputNode(ObjectInput, "Object");

            TypeOutput = new TypeDataNode(this, NodeType.Output);
            this.ChildElementManager.AddDataOutputNode(TypeOutput, "Type", true);

            IsArrayOutput = new BooleanDataNode(this, NodeType.Output);
            this.ChildElementManager.AddDataOutputNode(IsArrayOutput, "Is Array");
        }

        public override CompInfo GetCompInfo() => new CompInfo(this, "Get Object Type", "Type", "Type");

        public override void Compute()
        {
            DataStructure<object> obj = this.ChildElementManager.GetData(ObjectInput);
            if (obj != null) this.ChildElementManager.SetData(obj.Data.GetType(), TypeOutput);
            else return;
            if (obj != null && obj.Data != null) this.ChildElementManager.SetData(obj.Data.GetType().IsArray, IsArrayOutput);
            else this.ChildElementManager.SetData(false, IsArrayOutput);
        }
    }

    public class TypeDataNode : DataNodeElement<Type>
    {
        public TypeDataNode(IRenderable parent, NodeType type = NodeType.Unset) : base(parent, type)
        {
        }
    }
}
