using Core;
using CoreInterop;
using Rhino;
using Rhino.Commands;
using Rhino.DocObjects;
using Rhino.Geometry;
using Rhino.Input;
using Rhino.Input.Custom;
using System;
using System.Collections.Generic;

namespace Verse3InteropRhinoPlugin
{
    public class Verse3InteropCommand : Command
    {
        private InteropClient client;
        public Verse3InteropCommand()
        {
            // Rhino only creates one instance of each command class defined in a
            // plug-in, so it is safe to store a refence in a static property.
            Instance = this;
        }

        ///<summary>The only instance of this command.</summary>
        public static Verse3InteropCommand Instance { get; private set; }

        ///<returns>The command name as it appears on the Rhino command line.</returns>
        public override string EnglishName => "Verse3InteropCommand";

        string lastMessage = "";
        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            if (client != null)
            {
                RhinoApp.WriteLine("Last Message: " + lastMessage);
            }
            else
            {
                //RhinoApp.
                client = new InteropClient("Verse3", "token");
                client.ServerMessage += Client_ServerMessage;
            }
            using (GetString gs = new GetString())
            {
                gs.SetCommandPrompt("Enter a message to send to Verse3");
                gs.AcceptNothing(true);
                gs.Get();
                if (gs.CommandResult() != Result.Success)
                    return gs.CommandResult();
                if (string.IsNullOrEmpty(gs.StringResult()))
                    return Result.Success;
                string outMessage = gs.StringResult();
                DataStructure<string> goo = new DataStructure<string>(outMessage);
                client.Send(goo);
                return Result.Success;
            }

            // TODO: start here modifying the behaviour of your command.
            // ---
            //RhinoApp.WriteLine("The {0} command will add a line right now.", EnglishName);

            //Point3d pt0;
            //using (GetPoint getPointAction = new GetPoint())
            //{
            //    getPointAction.SetCommandPrompt("Please select the start point");
            //    if (getPointAction.Get() != GetResult.Point)
            //    {
            //        RhinoApp.WriteLine("No start point was selected.");
            //        return getPointAction.CommandResult();
            //    }
            //    pt0 = getPointAction.Point();
            //}

            //Point3d pt1;
            //using (GetPoint getPointAction = new GetPoint())
            //{
            //    getPointAction.SetCommandPrompt("Please select the end point");
            //    getPointAction.SetBasePoint(pt0, true);
            //    getPointAction.DynamicDraw +=
            //      (sender, e) => e.Display.DrawLine(pt0, e.CurrentPoint, System.Drawing.Color.DarkRed);
            //    if (getPointAction.Get() != GetResult.Point)
            //    {
            //        RhinoApp.WriteLine("No end point was selected.");
            //        return getPointAction.CommandResult();
            //    }
            //    pt1 = getPointAction.Point();
            //}

            //doc.Objects.AddLine(pt0, pt1);
            //doc.Views.Redraw();
            //RhinoApp.WriteLine("The {0} command added one line to the document.", EnglishName);

            //// ---
            //return Result.Success;
        }
        //public delegate bool RunScriptDelegate(string script, bool echo);
        //public delegate void ClearObjectsDelegate();
        public delegate void ProcessMessageDelegate(double num, ref List<Guid> refGeo);
        //public delegate Guid AddSphereDelegate(Sphere sphere);
        //public delegate bool RemoveObjectDelegate(Guid guid, bool quiet);
        private List<Guid> referencedGeo = new List<Guid>();
        private void Client_ServerMessage(object sender, DataStructure e)
        {
            //label1.Text = e.ToString();
            lastMessage = e.ToString();
            RhinoApp.WriteLine("InteropMessage: " + lastMessage);

            //RunScriptDelegate dgt = new RunScriptDelegate(RhinoApp.RunScript);
            //RhinoApp.InvokeOnUiThread(dgt, ("-_SelAll _Delete _Sphere 0,0,0 " + lastMessage), true);

            //if (referencedGeo.Count > 0)
            //{
            //ClearObjectsDelegate clrdgt = new ClearObjectsDelegate(RhinoDoc.ActiveDoc.Objects.Clear);
            //RhinoApp.InvokeOnUiThread(clrdgt);
            //foreach (RhinoObject o in )
            //{
            //    RemoveObjectDelegate dgt = new RemoveObjectDelegate(RhinoDoc.ActiveDoc.Objects.Delete);
            //    RhinoApp.InvokeOnUiThread(dgt, o.Id, true);
            //    //RhinoDoc.ActiveDoc.Objects.Delete(guid, true);
            //    //referencedGeo.Remove(o.Id);
            //}
            //}
            if (double.TryParse(lastMessage, out double num))
            {
                //AddSphereDelegate dgt = new AddSphereDelegate(RhinoDoc.ActiveDoc.Objects.AddSphere);
                //RhinoApp.InvokeOnUiThread(dgt, new Sphere(new Point3d(0, 0, 0), num));
                ProcessMessageDelegate dgt = new ProcessMessageDelegate(ProcessMessage);
                RhinoApp.InvokeOnUiThread(dgt, num, referencedGeo);
                //referencedGeo.Add(RhinoDoc.ActiveDoc.Objects.AddSphere(new Sphere(new Point3d(0, 0, 0), num)));
            }
        }

        public void ProcessMessage(double num, ref List<Guid> refGeo)
        {
            //RhinoDoc.ActiveDoc.Objects.Clear();
            if (refGeo.Count > 0)
            {
                try
                {
                    foreach (Guid guid in refGeo)
                    {

                        if (refGeo.Count > 0)
                        {
                            RhinoDoc.ActiveDoc.Objects.Delete(guid, true);
                            refGeo.Remove(guid);
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    //RhinoApp.WriteLine(ex.Message);
                }
            }
            refGeo.Add(RhinoDoc.ActiveDoc.Objects.AddBox(new Box(Plane.WorldXY, new Interval((-num), num), new Interval((-num), num), new Interval((-num), num))));
            if (refGeo.Count > 0 && RhinoDoc.ActiveDoc.Objects.Count > 0)
            {
                RhinoDoc.ActiveDoc.Views.Redraw();
                return;
            }
        }
    }
}
