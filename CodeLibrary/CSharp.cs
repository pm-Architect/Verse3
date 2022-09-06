using Core;
//using EventsLibrary;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows;
using VanillaElements;
using Verse3;
using Verse3.VanillaElements;

namespace CodeLibrary
{
    public class CSharp : BaseComp
    {

        #region Properties

        public string? ElementText
        {
            get
            {
                string? name = this.GetType().FullName;
                string? viewname = this.ViewType.FullName;
                string? dataIN = _script;
                //string? dataIN = "";
                if (_log.Count > 1)
                {
                    dataIN = "";
                    if (_log.Count <= 5)
                    {
                        foreach (string entry in _log)
                        {
                            dataIN += (entry + "\n");
                        }
                    }
                    else
                    {
                        foreach (string entry in (_log.GetRange((_log.Count - 5), 5)))
                        {
                            dataIN += (entry + "\n");
                        }
                    }
                }
                //if (this.ComputationPipelineInfo.IOManager.DataOutputNodes != null && this.ComputationPipelineInfo.IOManager.DataOutputNodes.Count > 0)
                //    dataIN = (Math.Round((((NumberDataNode)this.ComputationPipelineInfo.IOManager.DataOutputNodes[0]).DataGoo.Data), 2)).ToString();
                //string? zindex = DataViewModel.WPFControl.Content.
                //TODO: Z Index control for IRenderable
                return $"Name: {name}" +
                    $"\nView: {viewname}" +
                    $"\nID: {this.ID}" +
                    $"\nX: {this.X}" +
                    $"\nY: {this.Y}" +
                    $"\nOutput Value: {dataIN}";
            }
        }

        #endregion

        #region Constructors

        public CSharp() : base(0, 0)
        {
        }

        public CSharp(int x, int y, int width = 250, int height = 350) : base(x, y)
        {
        }

        #endregion

        public override CompInfo GetCompInfo()
        {
            Type[] types = { typeof(int), typeof(int), typeof(int), typeof(int) };
            CompInfo ci = new CompInfo
            {
                ConstructorInfo = this.GetType().GetConstructor(types),
                Name = "CSharp",
                Group = "CSharp",
                Tab = "Code",
                Description = "",
                Author = "",
                License = "",
                Repository = "",
                Version = "",
                Website = ""
            };
            return ci;
        }

        public override void Compute()
        {
            //double a = this.ChildElementManager.GetData<double>(0, 0);
            //double b = this.ChildElementManager.GetData<double>(1, 0);
            //this.ChildElementManager.SetData<double>((a + b), 0);
            _script = this.ideElement.Script;
            //TODO: Compile the script and display the output

            try
            {
                Assembly a = AssemblyCompiler.Compile(_script);
                List<IElement> elements = new List<IElement>(AssemblyLoader.Load(a)); foreach (IElement element in elements)
                {
                    CoreConsole.Log(element.ID.ToString());
                    if (element is IRenderable)
                    {
                        try
                        {
                            DataTemplateManager.RegisterDataTemplate(element as IRenderable);
                        }
                        catch (Exception ex)
                        {
                            CoreConsole.Log(ex.Message);
                            //throw;
                        }
                        //TODO: Check for other types of constructors
                        //TODO: Get LibraryInfo
                        MethodInfo? mi = element.GetType().GetRuntimeMethod("GetCompInfo", new Type[] { });
                        if (mi != null)
                        {
                            if (mi.ReturnType == typeof(CompInfo))
                            {
                                CompInfo compInfo = (CompInfo)mi.Invoke(element, null);
                                if (compInfo.ConstructorInfo != null)
                                {
                                    if (compInfo.ConstructorInfo.GetParameters().Length > 0)
                                    {
                                        ParameterInfo[] pi = compInfo.ConstructorInfo.GetParameters();
                                        object[] args = new object[pi.Length];
                                        for (int i = 0; i < pi.Length; i++)
                                        {
                                            if (!(pi[i].DefaultValue is DBNull)) args[i] = pi[i].DefaultValue;
                                            else
                                            {
                                                if (pi[i].ParameterType == typeof(int) && pi[i].Name.ToLower() == "x")
                                                    args[i] = DataViewModel.WPFControl.GetMouseRelPosition().X;
                                                //args[i] = InfiniteCanvasWPFControl.GetMouseRelPosition().X;
                                                else if (pi[i].ParameterType == typeof(int) && pi[i].Name.ToLower() == "y")
                                                    args[i] = DataViewModel.WPFControl.GetMouseRelPosition().Y;
                                                //args[i] = InfiniteCanvasWPFControl.GetMouseRelPosition().Y;
                                            }
                                        }
                                        IElement? elInst = compInfo.ConstructorInfo.Invoke(args) as IElement;
                                        try
                                        {
                                            DataModel.Instance.Elements.Add(elInst);
                                        }
                                        catch (Exception ex)
                                        {
                                            CoreConsole.Log(ex.Message);
                                            //throw;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                CoreConsole.Log(ex.Message);
                //throw;
            }
            
            _log = AssemblyCompiler.CompileLog;

            textBlock.DisplayedText = this.ElementText;
            this.ChildElementManager.AdjustBounds();
            RenderingCore.Render(this);
        }

        private string _script = "";

        private TextElement textBlock = new TextElement();
        private IDEElement ideElement = new IDEElement();
        internal ButtonElement buttonBlock = new ButtonElement();
        private List<string> _log = new List<string>();

        //private ButtonClickedEventNode nodeBlock;
        //private NumberDataNode nodeBlock1;
        //private NumberDataNode nodeBlock2;
        public override void Initialize()
        {
            //nodeBlock = new NumberDataNode(this, NodeType.Input);
            ////nodeBlock.Width = 50;
            //this.ChildElementManager.AddDataInputNode(nodeBlock, "A");

            //nodeBlock1 = new NumberDataNode(this, NodeType.Input);
            ////nodeBlock1.Width = 50;
            //this.ChildElementManager.AddDataInputNode(nodeBlock1, "B");

            //nodeBlock2 = new NumberDataNode(this, NodeType.Output);
            ////nodeBlock2.Width = 50;
            //this.ChildElementManager.AddDataOutputNode(nodeBlock2, "Result");
            
            //nodeBlock = new ButtonClickedEventNode(this, NodeType.Input);
            //nodeBlock.Width = 50;
            //nodeBlock.NodeEvent += NodeBlock_NodeEvent;
            //this.ChildElementManager.AddEventInputNode(nodeBlock as IEventNode, "Compile");

            ideElement = new IDEElement();
            ideElement.ScriptChanged += IdeElement_ScriptChanged;
            //ideElement.BoundingBox.Size.Width = 600;
            //ideElement.BoundingBox.Size.Height = 350;
            this.ChildElementManager.AddElement(ideElement);

            buttonBlock = new ButtonElement();
            buttonBlock.DisplayedText = "Compile";
            buttonBlock.OnButtonClicked += ButtonBlock_OnButtonClicked;
            this.ChildElementManager.AddElement(buttonBlock);

            textBlock = new TextElement();
            textBlock.DisplayedText = this.ElementText;
            textBlock.TextAlignment = TextAlignment.Left;
            textBlock.BoundingBox.Size.Width = 600;
            this.ChildElementManager.AddElement(textBlock);
        }

        private void IdeElement_ScriptChanged(object? sender, EventArgs e)
        {
            ComputationCore.Compute(this);
        }

        //private void NodeBlock_NodeEvent(IEventNode container, EventArgData e)
        //{
        //    ComputationCore.Compute(this);
        //}

        private void ButtonBlock_OnButtonClicked(object? sender, RoutedEventArgs e)
        {
            this.ideElement.TriggerScriptUpdate();
        }
    }
}
