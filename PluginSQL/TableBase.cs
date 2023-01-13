using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;

/***********************************/
/* COMPLEMENTO CREADO POR AVALONTM */
/***********************************/

namespace PluginSQL
{
    public partial class TableBase : ViewModelBase, IDisposable
    {
        public TableBase()
        {

        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            Debug.WriteLine($"[Dispose] {this.GetType().Name}");
        }
    }
}
