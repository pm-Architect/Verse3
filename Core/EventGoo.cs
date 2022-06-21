using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{

    public interface IEventGoo<E>
    {
        GooEventHandler Event { get; set; }

        public delegate void GooEventHandler(object sender, E e, string x);
        public void RaiseEvent(object sender, E e, string g = "")
        {
            if (Event != null)
                Event(sender, e, "");
            Event += RaiseEvent;
            Event?.Invoke(sender, e, "");
            Event.BeginInvoke(sender, e, "", callback, null);
        }
        AsyncCallback callback { get; set; }

    }
}
