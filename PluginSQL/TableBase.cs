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
    public partial class TableBase : ViewModelBase
    {
        public TableBase()
        {

        }

        public void Copy(object src)
        {
            if (src == null)
            {
                return;
            }
            var srcT = src.GetType();
            var dstT = this.GetType();
            foreach (var f in srcT.GetFields())
            {
                var dstF = dstT.GetField(f.Name);
                if (dstF == null)
                    continue;
                dstF.SetValue(this, f.GetValue(src));
            }

            foreach (var f in srcT.GetProperties())
            {
                var dstF = dstT.GetProperty(f.Name);
                if (dstF == null)
                    continue;

                dstF.SetValue(this, f.GetValue(src, null), null);
            }
        }

        public void ToCopy(object src)
        {
            if (src == null)
            {
                return;
            }
            var srcT = this.GetType();
            var dstT = src.GetType();
            foreach (var f in dstT.GetFields())
            {
                var dstF = srcT.GetField(f.Name);
                if (dstF == null)
                    continue;
                dstF.SetValue(dstT, f.GetValue(src));
            }

            foreach (var f in dstT.GetProperties())
            {
                var dstF = srcT.GetProperty(f.Name);
                if (dstF == null)
                    continue;

                dstF.SetValue(dstT, f.GetValue(src, null), null);
            }
        }
    }
}
