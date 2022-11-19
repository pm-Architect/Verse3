using Core;
using System;
using System.Threading;
using System.Windows;
using Verse3;
using Verse3.VanillaElements;

namespace EventsLibrary
{
    public class Timer : BaseComp
    {
        private int _interval = 1000;
        private bool _enabled = false;

        private System.Threading.Timer _timer;

        #region Constructors

        public Timer() : base()
        {
        }
        public Timer(int x, int y) : base(x, y)
        {
            this.previewTextBlock.DisplayedText = "Interval: " + _interval.ToString();
        }

        private void _timer_Tick(object? state)
        {
            if (_enabled)
            {
                tickEvent.EventOccured(new EventArgData());
                ComputationCore.Compute(this);
            }
        }

        #endregion

        public override void Compute()
        {
            if (_enabled) toggleBlock.DisplayedText = "Enabled";
            else toggleBlock.DisplayedText = "Disabled";
            int i = (int)this.ChildElementManager.GetData(interval, 1000);
            if (i != _interval)
            {
                _interval = i;
                _timer.Change(_interval, _interval);
            }
            this.previewTextBlock.DisplayedText = "Interval: " + _interval.ToString();
        }
        public override CompInfo GetCompInfo() => new CompInfo(this, "Timer", "Timing", "Events");

        private NumberDataNode interval;
        private GenericEventNode tickEvent;
        private ToggleElement toggleBlock;
        public override void Initialize()
        {
            interval = new NumberDataNode(this, NodeType.Input);
            this.ChildElementManager.AddDataInputNode(interval, "Interval");

            tickEvent = new GenericEventNode(this, NodeType.Output);
            this.ChildElementManager.AddEventOutputNode(tickEvent, "Tick");

            toggleBlock = new ToggleElement();
            toggleBlock.Value = _enabled;
            if (_enabled) toggleBlock.DisplayedText = "Enabled";
            else toggleBlock.DisplayedText = "Disabled";
            toggleBlock.ToggleChecked += ButtonBlock_ToggleChecked;
            toggleBlock.ToggleUnchecked += ButtonBlock_ToggleUnchecked;
            toggleBlock.Width = 200;
            this.ChildElementManager.AddElement(toggleBlock);

            _timer = new System.Threading.Timer(_timer_Tick, _enabled, _interval, _interval);
        }

        private void ButtonBlock_ToggleChecked(object? sender, RoutedEventArgs e)
        {
            _enabled = true;
            //if (toggleBlock.Value.HasValue) _enabled = toggleBlock.Value.Value;
            ComputationCore.Compute(this, false);
        }

        private void ButtonBlock_ToggleUnchecked(object? sender, RoutedEventArgs e)
        {
            _enabled = false;
            //if (toggleBlock.Value.HasValue) _enabled = toggleBlock.Value.Value;
            ComputationCore.Compute(this, false);
        }
    }
}
