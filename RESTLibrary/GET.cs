using Core;
using Verse3;
using Verse3.VanillaElements;
using RestSharp;

namespace MathLibrary
{
    public class GET : BaseComp
    {
        public GET() : base()
        {
        }
        public GET(int x, int y) : base(x, y)
        {
        }

        //private NumberDataNode A;
        //private NumberDataNode B;
        //private NumberDataNode Result;
        public override void Initialize()
        {
            //EVENT NODES

            //DATA NODES
            //A = new NumberDataNode(this, NodeType.Input);
            //this.ChildElementManager.AddDataInputNode(A, "A");

            //B = new NumberDataNode(this, NodeType.Input);
            //this.ChildElementManager.AddDataInputNode(B, "B");

            //Result = new NumberDataNode(this, NodeType.Output);
            //this.ChildElementManager.AddDataOutputNode(Result, "Result", true);

            
        }

        public override CompInfo GetCompInfo() => new CompInfo(this, "GET", "Requests", "REST");

        public override void Compute()
        {
            RestClient restClient = new RestClient("https://api.github.com");
            RestRequest restRequest = new RestRequest("/repos/RestSharp/RestSharp/releases/latest", Method.Get);
            RestResponse restResponse = restClient.Execute(restRequest);
            string? content = restResponse.Content;
            //Result.DataGoo.Data = content;
        }
    }
}
