using Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Verse3;
using Verse3.VanillaElements;

namespace TextLibrary
{
    public class CoreConsoleDisplay : BaseComp
    {
        public CoreConsoleDisplay() : base()
        {
        }
        public CoreConsoleDisplay(int x, int y) : base(x, y)
        {
        }

        public override void Compute()
        {
            //DataStructure data = this.ChildElementManager.GetData(writeToConsole);
            CoreConsole.GetEntries(5, out string[] entries);
            DataStructure<string> data = new DataStructure<string>(entries);
            this.previewTextBlock.DisplayedText = "";
            if (data is null || data.Count == 0)
            {
                return;
            }
            else
            {
                if (data.Count > 0)
                {
                    foreach (IDataGoo goo in data)
                    {
                        this.previewTextBlock.DisplayedText += (goo.Data.ToString() + "\n");
                    }
                }
            }
            //this.previewTextBlock.Width = 450;
            //this.ChildElementManager.AdjustBounds(false);
            this.ChildElementManager.AdjustBounds(false);
        }

        public override CompInfo GetCompInfo() => new CompInfo(this, "Console Display", "Debug", "Code");

        internal GenericEventNode writeToConsole;
        private GenericEventNode consoleOutput;
        public override void Initialize()
        {
            writeToConsole = new GenericEventNode(this, NodeType.Input);
            writeToConsole.NodeEvent += WriteToConsole_NodeEvent;
            this.ChildElementManager.AddEventInputNode(writeToConsole, "Write to Console");

            consoleOutput = new GenericEventNode(this, NodeType.Output);
            this.ChildElementManager.AddEventOutputNode(consoleOutput, "Console Output");

            CoreConsole.OnLog += CoreConsole_OnLogEvent;
        }

        private void WriteToConsole_NodeEvent(IEventNode container, EventArgData e)
        {
            CoreConsole.Log(e.Data.ToString());
        }
        private void CoreConsole_OnLogEvent(object sender, EventArgData e)
        {
            this.ChildElementManager.EventOccured(consoleOutput, e);
            ComputationCore.Compute(this, false);
        }
    }
}
