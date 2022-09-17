using Core;
using CoreInterop;
using Rhino;
using Rhino.Commands;
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
        private void Client_ServerMessage(object sender, DataStructure e)
        {
            //label1.Text = e.ToString();
            lastMessage = e.ToString();
            RhinoApp.WriteLine("InteropMessage: " + lastMessage);
        }
    }
}
