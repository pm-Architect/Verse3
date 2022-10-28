using Core;
//using EventsLibrary;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows;
using VanillaElements;
using Verse3;
using Verse3.VanillaElements;

namespace CodeLibrary
{
    public class CSharp : BaseComp
    {

        #region Constructors

        public CSharp() : base()
        {
        }

        public CSharp(int x, int y) : base(x, y)
        {
        }

        #endregion

        public override CompInfo GetCompInfo() => new CompInfo(this, "CSharp", "CSharp", "Code");

        public override void Compute()
        {
            try
            {
                _script = this.ideElement.Script;
                this.previewTextBlock.DisplayedText = "Compiling...";
                byte[] ASMbytes = AssemblyCompiler.Compile(_script, "RuntimeCompiled_CSharp_Verse3");
                List<IElement> elements = new List<IElement>(AssemblyLoader.Load(ASMbytes, Main_Verse3.domain_));
                foreach (IElement element in elements)
                {
                    CoreConsole.Log(element.ID.ToString());
                    //AssemblyCompiler.CompileLog.Add("Compiled ID: " + element.ID.ToString());
                    if (element is IRenderable)
                    {
                        try
                        {
                            DataTemplateManager.RegisterDataTemplate(element as IRenderable);
                        }
                        catch (Exception ex)
                        {
                            CoreConsole.Log(ex.Message);
                        }
                        //TODO: Check for other types of constructors
                        //TODO: Get LibraryInfo
                        MethodInfo? mi = element.GetType().GetRuntimeMethod("GetCompInfo", new Type[] { });
                        if (mi != null)
                        {
                            if (mi.ReturnType == typeof(CompInfo))
                            {
                                compiledCompInfo = (CompInfo)mi.Invoke(element, null);
                                if (compiledCompInfo.ConstructorInfo != null)
                                {
                                    AssemblyCompiler.CompileLog.Add("Compiled Name: " + compiledCompInfo.Name + " @ ID: " + element.ID.ToString());
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                CoreConsole.Log(ex.Message);
            }
            
            _log = AssemblyCompiler.CompileLog;
            
            //string? dataIN = "";
            if (_log.Count > 1)
            {
                this.previewTextBlock.DisplayedText = "";
                if (_log.Count <= 5)
                {
                    foreach (string entry in _log)
                    {
                        this.previewTextBlock.DisplayedText += (entry + "\n");
                    }
                }
                else
                {
                    foreach (string entry in (_log.GetRange((_log.Count - 5), 5)))
                    {
                        this.previewTextBlock.DisplayedText += (entry + "\n");
                    }
                }
            }
            //this.ChildElementManager.AdjustBounds(true);
            RenderingCore.Render(this);
        }

        private string _script = "";
        private CompInfo compiledCompInfo;

        private IDEElement ideElement = new IDEElement();
        internal ButtonElement buttonBlock = new ButtonElement();
        internal ButtonElement buttonBlock1 = new ButtonElement();
        internal ButtonElement buttonBlock2 = new ButtonElement();
        private List<string> _log = new List<string>();

        //private ButtonClickedEventNode nodeBlock;
        //private NumberDataNode nodeBlock1;
        //private NumberDataNode nodeBlock2;
        public override void Initialize()
        {
            this.titleTextBlock.TextRotation = 0;

            ideElement = new IDEElement();
            ideElement.ScriptChanged += IdeElement_ScriptChanged;
            //ideElement.Width = 600;
            //ideElement.Height = 350;
            this.ChildElementManager.AddElement(ideElement);

            buttonBlock = new ButtonElement();
            buttonBlock.DisplayedText = "Compile";
            buttonBlock.OnButtonClicked += ButtonBlock_OnButtonClicked;
            //buttonBlock.Width = 200;
            this.ChildElementManager.AddElement(buttonBlock);

            buttonBlock1 = new ButtonElement();
            buttonBlock1.DisplayedText = "Load Instance";
            buttonBlock1.OnButtonClicked += ButtonBlock1_OnButtonClicked;
            //buttonBlock1.Width = 200;
            this.ChildElementManager.AddElement(buttonBlock1);

            buttonBlock2 = new ButtonElement();
            buttonBlock2.DisplayedText = "Add to Arsenal";
            buttonBlock2.OnButtonClicked += ButtonBlock2_OnButtonClicked;
            //buttonBlock2.Width = 200;
            this.ChildElementManager.AddElement(buttonBlock2);
        }

        private void ButtonBlock1_OnButtonClicked(object? sender, RoutedEventArgs e)
        {
            ComputationCore.Compute(this, false);
            try
            {

                if (compiledCompInfo.ConstructorInfo != null)
                {
                    if (compiledCompInfo.ConstructorInfo.GetParameters().Length > 0)
                    {
                        ParameterInfo[] pi = compiledCompInfo.ConstructorInfo.GetParameters();
                        object[] args = new object[pi.Length];
                        for (int i = 0; i < pi.Length; i++)
                        {
                            if (!(pi[i].DefaultValue is DBNull)) args[i] = pi[i].DefaultValue;
                            else
                            {
                                if (pi[i].ParameterType == typeof(int) && pi[i].Name.ToLower() == "x")
                                    args[i] = DataViewModel.WPFControl.GetMouseRelPosition().X;
                                else if (pi[i].ParameterType == typeof(int) && pi[i].Name.ToLower() == "y")
                                    args[i] = DataViewModel.WPFControl.GetMouseRelPosition().Y;
                            }
                        }
                        //IElement? elInst = compInfo.ConstructorInfo.Invoke(args) as IElement;
                        try
                        {
                            //TODO: LOAD/INSTANTIATE ASSEMBLY INTO RIBBON AND ON CANVAS
                            EditorForm.compsPendingInst.Add(compiledCompInfo, args);
                            Main_Verse3.ActiveMain.ActiveEditor.AddToCanvas_OnCall(this, new EventArgs());
                        }
                        catch (Exception ex)
                        {
                            CoreConsole.Log(ex.Message);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Add(ex.Message);
                if (_log.Count > 1)
                {
                    this.previewTextBlock.DisplayedText = "";
                    if (_log.Count <= 5)foreach (string entry in _log) this.previewTextBlock.DisplayedText += (entry + "\n");
                    else foreach (string entry in (_log.GetRange((_log.Count - 5), 5))) this.previewTextBlock.DisplayedText += (entry + "\n");
                }
                CoreConsole.Log(ex);
            }
        }

        private void ButtonBlock2_OnButtonClicked(object? sender, RoutedEventArgs e)
        {
            ComputationCore.Compute(this, false);
            try
            {

                if (compiledCompInfo.ConstructorInfo != null)
                {
                    if (compiledCompInfo.ConstructorInfo.GetParameters().Length > 0)
                    {
                        ParameterInfo[] pi = compiledCompInfo.ConstructorInfo.GetParameters();
                        object[] args = new object[pi.Length];
                        for (int i = 0; i < pi.Length; i++)
                        {
                            if (!(pi[i].DefaultValue is DBNull)) args[i] = pi[i].DefaultValue;
                            else
                            {
                                if (pi[i].ParameterType == typeof(int) && pi[i].Name.ToLower() == "x")
                                    args[i] = DataViewModel.WPFControl.GetMouseRelPosition().X;
                                else if (pi[i].ParameterType == typeof(int) && pi[i].Name.ToLower() == "y")
                                    args[i] = DataViewModel.WPFControl.GetMouseRelPosition().Y;
                            }
                        }
                        //IElement? elInst = compInfo.ConstructorInfo.Invoke(args) as IElement;
                        try
                        {
                            //TODO: LOAD/INSTANTIATE ASSEMBLY INTO RIBBON AND ON CANVAS
                            EditorForm.compsPendingAddToArsenal.Add(compiledCompInfo);
                            Main_Verse3.ActiveMain.ActiveEditor.AddToCanvas_OnCall(this, new EventArgs());
                        }
                        catch (Exception ex)
                        {
                            CoreConsole.Log(ex.Message);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Add(ex.Message);
                if (_log.Count > 1)
                {
                    this.previewTextBlock.DisplayedText = "";
                    if (_log.Count <= 5) foreach (string entry in _log) this.previewTextBlock.DisplayedText += (entry + "\n");
                    else foreach (string entry in (_log.GetRange((_log.Count - 5), 5))) this.previewTextBlock.DisplayedText += (entry + "\n");
                }
                CoreConsole.Log(ex);
            }
        }

        private void IdeElement_ScriptChanged(object? sender, EventArgs e)
        {
            ComputationCore.Compute(this);
        }

        private void ButtonBlock_OnButtonClicked(object? sender, RoutedEventArgs e)
        {
            this.ideElement.TriggerScriptUpdate();
        }
    }
}
