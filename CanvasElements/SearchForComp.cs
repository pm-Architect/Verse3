﻿using Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using Verse3;
using Verse3.VanillaElements;

namespace MathLibrary
{
    public class SearchForComp : BaseComp
    {

        public static SearchForComp Instance = null;

        #region Properties

        public string? ElementText
        {
            get
            {
                string? name = this.GetType().FullName;
                string? viewname = this.ViewType.FullName;
                string? dataIN = "";
                if (this.ComputationPipelineInfo.IOManager.DataOutputNodes != null && this.ComputationPipelineInfo.IOManager.DataOutputNodes.Count > 0)
                    dataIN = (Math.Round((((NumberDataNode)this.ComputationPipelineInfo.IOManager.DataOutputNodes[0]).DataGoo.Data), 2)).ToString();
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

        public SearchForComp() : base(0, 0)
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                DataViewModel.Instance.Elements.Remove(Instance);
                Instance = this;
            }
        }

        public SearchForComp(int x, int y) : base(x, y)
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                DataViewModel.Instance.Elements.Remove(Instance);
                Instance = this;
            }
        }

        #endregion

        public override CompInfo GetCompInfo()
        {
            Type[] types = { typeof(int), typeof(int) };
            CompInfo ci = new CompInfo
            {
                ConstructorInfo = this.GetType().GetConstructor(types),
                Name = "Search",
                Group = "_CanvasElements",
                Tab = "_CanvasElements",
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
        }
        
        private SearchBoxElement searchBlock = new SearchBoxElement();
        public override void Initialize()
        {
            base.titleTextBlock.TextRotation = 0;
            
            searchBlock = new SearchBoxElement();
            searchBlock.Width = 200;
            searchBlock.InputText = "";
            searchBlock.IsSelected = true;
            searchBlock.SearchStarted += SearchBlock_SearchStarted;
            this.ChildElementManager.AddElement(searchBlock);
        }

        private Dictionary<ButtonElement, CompInfo> buttonDictionary = new Dictionary<ButtonElement, CompInfo>();

        private void SearchBlock_SearchStarted(object? sender, HandyControl.Data.FunctionEventArgs<string> e)
        {
            try
            {
                if (buttonDictionary.Count > 0)
                {
                    foreach (ButtonElement renderable in buttonDictionary.Keys)
                    {
                        this.ChildElementManager.RemoveElement(renderable);
                    }
                }
                buttonDictionary.Clear();

                string query = e.Info;
                //Get a list of types that inherit BaseComp in loaded assemblies
                Type[] types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(s => s.GetTypes()).Where(p => typeof(BaseComp).IsAssignableFrom(p)).ToArray();
                if (types != null && types.Length > 0)
                {
                    //get an array of all the names of the types
                    string[] names = types.Select(t => t.Name).ToArray();
                    //search for the query in the names array
                    string[] results = names.Where(n => n.Contains(query)).ToArray();
                    //remove duplicate values from results
                    List<string> temp1 = new List<string>();
                    for (int i = 0; i < results.Length; i++)
                    {
                        if (!temp1.Contains(results[i])) temp1.Add(results[i]);
                    }
                    results = temp1.ToArray();
                    //reduce results to first 5
                    if (results.Length > 5)
                    {
                        string[] temp = new string[5];
                        for (int i = 0; i < 5; i++)
                        {
                            temp[i] = results[i];
                        }
                        results = temp;
                    }

                    if (results.Length > 0)
                    {
                        foreach (string result in results)
                        {
                            ButtonElement btn = new ButtonElement();
                            btn.DisplayedText = result;
                            btn.Width = 200;
                            btn.OnButtonClicked += Btn_OnButtonClicked;

                            //get the type from the types array
                            Type? type = types.Where(t => t.Name == result).FirstOrDefault();
                            if (type != null)
                            {
                                //create a new instance of the type
                                if (type == typeof(BaseComp)) continue;
                                if (type == typeof(SearchForComp)) continue;
                                object? comp = Activator.CreateInstance(type);
                                //get the CompInfo from the type
                                CompInfo ci = (CompInfo)type.GetMethod("GetCompInfo").Invoke(comp, null);
                                //add the button and the CompInfo to the dictionary
                                buttonDictionary.Add(btn, ci);
                            }
                        }
                    }

                    if (buttonDictionary.Count > 0)
                    {
                        foreach (ButtonElement button in buttonDictionary.Keys)
                        {
                            if (!this.ChildElementManager.BottomUIItems.Contains(button)) this.ChildElementManager.AddElement(button);
                        }
                    }
                }
                RenderingCore.Render(this);
                DataViewModel.WPFControl.ExpandContent();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }

        private void Btn_OnButtonClicked(object? sender, RoutedEventArgs e)
        {
            if (sender is Button btn)
            {
                if (btn.Content is string name)
                {
                    //find the buttonelement in the dictionary with compinfo.name
                    ButtonElement? btnElement = buttonDictionary.Where((kv) => kv.Value.Name == name).FirstOrDefault().Key;
                    if (btnElement is null) return;
                    if (buttonDictionary[btnElement].ConstructorInfo != null)
                    {
                        ////TODO: Invoke constructor based on <PluginName>.cfg json file
                        ////TODO: Allow user to place the comp with MousePosition
                        if (buttonDictionary[btnElement].ConstructorInfo.GetParameters().Length > 0)
                        {
                            ParameterInfo[] pi = buttonDictionary[btnElement].ConstructorInfo.GetParameters();
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
                            IElement? elInst = buttonDictionary[btnElement].ConstructorInfo.Invoke(args) as IElement;
                            DataViewModel.Instance.Elements.Add(elInst);
                            //DataViewModel.WPFControl.ExpandContent();
                        }
                    }
                }
            }
        }
    }
}