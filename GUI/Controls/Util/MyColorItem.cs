using System.Collections.ObjectModel;
using System.Windows.Media;
using Xceed.Wpf.Toolkit;

namespace Aura.Controls.Util
{

    public class MyColorItem : ColorItem
    {
        public MyColorItem(Color? color, string name) : base(color, name)
        {
        }

        public MyColorItem() : this(null, string.Empty) { }

    }

    public sealed class ObservableCollectionWrapper : ObservableCollection<ColorItem>
    {
    }
}