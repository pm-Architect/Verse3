extern alias R3dmIo;
extern alias RCommon;
using Core;
using CoreInterop;
using RCommon::Rhino;
using RCommon::Rhino.Commands;
using RCommon::Rhino.Input;
using RCommon::Rhino.Input.Custom;
using RCommon::Rhino.Runtime;
using RCommon::Rhino.Geometry;
using System;
using System.Collections.Generic;
using static CoreInterop.InteropClient;

using R3 = R3dmIo::Rhino.Geometry;
using RC = RCommon::Rhino.Geometry;
using RCommon::Rhino.DocObjects;

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
        public override string LocalName => "Verse3Interop";

        string lastMessage = "";
        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            if (client != null)
            {
                //TODO: Reconnect if disconnected
                //client = new InteropClient("Verse3", "token");
                //client.ServerMessage += Client_ServerMessage;
            }
            else
            {
                //RhinoApp.
                client = new InteropClient("Verse3", "token");
                client.ServerMessage += Client_ServerMessage;
                client.Disconnected += Client_Disconnected;
                client.Error += Client_Error;
            }
            if (lastMessage != "" || lastMessage != null) RhinoApp.WriteLine("Last Message: " + lastMessage);
            using (GetObject gs = new GetObject())
            {
                gs.SetCommandPrompt("Send to Verse3");
                gs.AcceptNothing(false);
                gs.AcceptNumber(true, true);
                gs.AcceptString(true);
                GetResult result = gs.Get();
                switch (result)
                {
                    case GetResult.NoResult:
                        return Result.Failure;
                    case GetResult.Cancel:
                        return Result.Cancel;
                    case GetResult.Nothing:
                        return Result.Nothing;
                    case GetResult.Option:
                        break;
                    case GetResult.Number:
                        {
                            double outMessage = gs.Number();
                            RhinoApp.WriteLine("Sent Number: " + outMessage.ToString());
                            DataStructure<double> goo = new DataStructure<double>(outMessage);
                            client.Send(goo);
                            return Result.Success;
                        }
                    case GetResult.Color:
                        break;
                    case GetResult.Undo:
                        break;
                    case GetResult.Miss:
                        break;
                    case GetResult.Point:
                        break;
                    case GetResult.Point2d:
                        break;
                    case GetResult.Line2d:
                        break;
                    case GetResult.Rectangle2d:
                        break;
                    case GetResult.Object:
                        {
                            ObjRef obj = gs.Object(0);
                            RC.GeometryBase geoObj = obj.Geometry();
                            if (geoObj != null && geoObj.IsValid)
                            {
                                R3.GeometryBase geoOut = RhCommon3dmConverterSingleton.ConvertGeometryBaseToR3dm(geoObj);
                                RhinoApp.WriteLine("Sent Geometry: " + geoOut.ToString());
                                DataStructure<R3.GeometryBase> goo = new DataStructure<R3.GeometryBase>(geoOut);
                                client.Send(goo);
                                return Result.Success;
                            }
                            return Result.Failure;
                        }
                    case GetResult.String:
                        {
                            string outMessage = gs.StringResult();
                            RhinoApp.WriteLine("Sent String: " + outMessage);
                            DataStructure<string> goo = new DataStructure<string>(outMessage);
                            client.Send(goo);
                            return Result.Success;
                        }
                    case GetResult.CustomMessage:
                        break;
                    case GetResult.Timeout:
                        break;
                    case GetResult.Circle:
                        break;
                    case GetResult.Plane:
                        break;
                    case GetResult.Cylinder:
                        break;
                    case GetResult.Sphere:
                        break;
                    case GetResult.Angle:
                        break;
                    case GetResult.Distance:
                        break;
                    case GetResult.Direction:
                        break;
                    case GetResult.Frame:
                        break;
                    case GetResult.User1:
                        break;
                    case GetResult.User2:
                        break;
                    case GetResult.User3:
                        break;
                    case GetResult.User4:
                        break;
                    case GetResult.User5:
                        break;
                    case GetResult.ExitRhino:
                        break;
                    default:
                        break;
                }
                if (gs.CommandResult() != Result.Success)
                    return gs.CommandResult();
                else return Result.Failure;
                //if (string.IsNullOrEmpty(gs.StringResult()))
                //    return Result.Success;
                //string outMessage = gs.StringResult();
                //DataStructure<string> goo = new DataStructure<string>(outMessage);
                //client.Send(goo);
            }
        }

        private void Client_Error(object sender, Exception e)
        {
            RhinoApp.WriteLine("Error: " + e.Message);
        }

        private void Client_Disconnected(object sender, EventArgs e)
        {
            RhinoApp.WriteLine("Disconnected: " + e.ToString());
        }

        //public delegate bool RunScriptDelegate(string script, bool echo);
        //public delegate void ClearObjectsDelegate();
        //public delegate void ProcessMessageDelegate(double num, ref List<Guid> refGeo);
        public delegate DataStructure ProcessMessageDelegate(ref List<Guid> referencedObjectGuids, params object[] args);
        //public delegate Guid AddSphereDelegate(Sphere sphere);
        //public delegate bool RemoveObjectDelegate(Guid guid, bool quiet);
        private List<Guid> referencedGeo = new List<Guid>();
        private void Client_ServerMessage(object sender, DataStructure e)
        {
            if (!e.IsValid)
            {
                RhinoApp.WriteLine("Warning: Invalid Message Data");
            }
            else
            {
                lastMessage = e.ToString();
                RhinoApp.WriteLine("InteropMessage: " + lastMessage);
                Type dt = e.DataType;
                object data = e.Data;
                if (dt is null)
                {
                    RhinoApp.WriteLine("Data Type Not Serialized/Found");
                    try
                    {
                        dt = e.Data.GetType();
                        if (dt is null)
                        {
                            throw new Exception("Data type is null");
                        }
                    }
                    catch (Exception ex)
                    {
                        RhinoApp.WriteLine("Data Type Error: " + ex.Message);
                        return;
                    }
                }
                if (dt.IsArray && e.Data is object[] array)
                {
                    if (e.DataStructurePattern == DataStructurePattern.Item)
                    {
                        if (array.Length == 1 && array[0] != null)
                        {
                            dt = array[0].GetType();
                            data = array[0];
                        }
                    }
                }
                InteropDelegate dgt = null;
                switch (dt)
                {
                    case Type t when t == typeof(string):
                        {
                            RhinoApp.WriteLine("String: " + e.ToString());
                            if (!RhinoApp.RunScript(e.ToString(), true))
                            {
                                RhinoApp.WriteLine("Run script failed: " + e.ToString());
                                try
                                {
                                    if (RhinoApp.ExecuteCommand(RhinoDoc.ActiveDoc, e.ToString()) == Result.Success)
                                    {
                                        RhinoApp.WriteLine("Command executed: " + e.ToString());
                                    }
                                    else
                                    {
                                        RhinoApp.WriteLine("Command failed: " + e.ToString());
                                    }
                                }
                                catch (Exception ex)
                                {
                                    RhinoApp.WriteLine("Command failed with exception: " + ex.Message);
                                }
                            }
                            break;
                        }
                    case Type t when t == typeof(double):
                        {
                            RhinoApp.WriteLine("Double: " + data.ToString());
                            dgt = new InteropDelegate(MakeCubeOfSize);
                            break;
                        }
                    case Type t when typeof(GeometryBase).IsAssignableFrom(t):
                        {
                            if (data is GeometryBase gb)
                            {
                                RhinoApp.WriteLine("GeometryBase: " + gb.ObjectType.ToString() + "; Value: " + data.ToString());
                                dgt = new InteropDelegate(MakeGeometry);
                            }
                            break;
                        }
                    case Type t when typeof(CommonObject).IsAssignableFrom(t):
                        {
                            RhinoApp.WriteLine("CommonObject: " + dt.ToString() + "; Value: " + data.ToString());
                            break;
                        }
                    case Type t when typeof(DataStructure).IsAssignableFrom(t):
                        {
                            RhinoApp.WriteLine("DataStructure: " + dt.ToString() + "; Value: " + data.ToString());
                            break;
                        }
                    case Type t when typeof(R3.GeometryBase).IsAssignableFrom(t):
                        {
                            RhinoApp.WriteLine("Rhino3dm Geometry: " + dt.ToString() + "; Value: " + data.ToString());
                            if (oldGuids.Count > 0)
                            {
                                foreach (Guid guid in oldGuids)
                                {
                                    GeometryBase g = RhinoDoc.ActiveDoc.Objects.FindGeometry(guid);
                                    if (g != null)
                                    {
                                        RhinoDoc.ActiveDoc.Objects.Delete(guid, true);
                                    }
                                }
                                oldGuids.Clear();
                            }
                            if (data is R3.GeometryBase geor3)
                            {
                                try
                                {
                                    RC.GeometryBase geoconv = RhCommon3dmConverterSingleton.ConvertGeometryBaseToRhinoCommon(geor3);
                                    Guid id = RhinoDoc.ActiveDoc.Objects.Add(geoconv);
                                    oldGuids.Add(id);
                                    //return id;
                                }
                                catch (Exception ex)
                                {
                                    throw ex;
                                }
                            }
                            RhinoDoc.ActiveDoc.Views.Redraw();
                            break;
                        }
                    case Type t when typeof(RC.GeometryBase).IsAssignableFrom(t):
                        {
                            RhinoApp.WriteLine("RhinoCommon Geometry: " + dt.ToString() + "; Value: " + data.ToString());
                            dgt = new InteropDelegate(MakeGeometry);
                            break;
                        }
                    //case Type t when typeof(object[]).IsAssignableFrom(t):
                    //    {
                    //        RhinoApp.WriteLine("Objects: " + e.DataStructurePattern.ToString() + "; Value: " + data.ToString());
                    //        if (e.DataStructurePattern == DataStructurePattern.Item)
                    //        {
                    //            RhinoApp.WriteLine(((object[])e.Data)[0].GetType().ToString());
                    //        }
                    //        dgt = new InteropDelegate(MakeGeometry);
                    //        break;
                    //    }
                    default:
                        {
                            RhinoApp.WriteLine("Unhandled Type: " + dt.ToString() + "; Value: " + data.ToString());
                            break;
                        }
                }
                if (dgt != null)
                {
                    try
                    {
                        object[] args = { data };
                        //if (referencedGeo.Count > 0)
                        //{
                        //    lock (referencedGeo)
                        //    {
                        //        args = new object[] { e, referencedGeo };
                        //        RhinoApp.InvokeOnUiThread(dgt, args);
                        //    }
                        //    return;
                        //}
                        RhinoApp.InvokeOnUiThread(dgt, args);
                        return;
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
            }
        }

        //DEV NOTE: Dynamic used here!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        private List<Guid> oldGuids = new List<Guid>();
        private dynamic MakeGeometry(params object[] args)
        {
            RhinoApp.WriteLine("MakeGeo: " + args.ToString());
            //if (args[1] is List<Guid> oldGuids)
            //{
            if (oldGuids.Count > 0)
            {
                foreach (Guid guid in oldGuids)
                {
                    GeometryBase g = RhinoDoc.ActiveDoc.Objects.FindGeometry(guid);
                    if (g != null)
                    {
                        RhinoDoc.ActiveDoc.Objects.Delete(guid, true);
                    }
                }
                oldGuids.Clear();
            }
            //}
            if (args[0] is RC.GeometryBase geo)
            {
                Guid id = RhinoDoc.ActiveDoc.Objects.Add(geo);
                oldGuids.Add(id);
                return id;
            }
            else if (args[0] is R3.GeometryBase geor3)
            {
                try
                {
                    RC.GeometryBase geoconv = RhCommon3dmConverterSingleton.ConvertGeometryBaseToRhinoCommon(geor3);
                    Guid id = RhinoDoc.ActiveDoc.Objects.Add(geoconv);
                    oldGuids.Add(id);
                    return id;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return null;
        }

        //DEV NOTE: Dynamic used here!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        private dynamic MakeCubeOfSize(params object[] args)
        {
            RhinoApp.WriteLine("MakeCubeOfSize: " + args.ToString());
            if (args[1] is List<Guid> oldGuids)
            {
                if (oldGuids.Count > 0)
                {
                    foreach (Guid guid in oldGuids)
                    {
                        GeometryBase g = RhinoDoc.ActiveDoc.Objects.FindGeometry(guid);
                        if (g != null)
                        {
                            RhinoDoc.ActiveDoc.Objects.Delete(guid, true);
                        }
                    }
                    oldGuids.Clear();
                }
            }
            if ((args[0] as DataStructure).Data is double num)
            {
                RhinoApp.WriteLine("Size: " + num.ToString());
                Box b = new Box(Plane.WorldXY, new Interval((-num), num), new Interval((-num), num), new Interval((-num), num));
                return RhinoDoc.ActiveDoc.Objects.AddBox(b);
            }
            return null;
        }
    }
}
