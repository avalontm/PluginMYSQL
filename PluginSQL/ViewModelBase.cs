using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace PluginSQL
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
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
