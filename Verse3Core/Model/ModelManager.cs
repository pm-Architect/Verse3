using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse3.Core.Utilities;

namespace Verse3.Core.Model
{
    public sealed class ModelManager : Observable
    {
        private static ModelManager instance = new ModelManager();
        public static ModelManager Instance
        {
            get { return instance; }
            private set { instance = value; }
        }

        private ModelManager() { }

        private ObservableCollection<DataModel> models;
        public ObservableCollection<DataModel> Models
        {
            get { return models; }            
        }
    }
}
