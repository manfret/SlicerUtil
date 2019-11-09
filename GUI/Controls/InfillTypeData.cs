using Settings.Entities.Infill;
using Settings.Memento;
using Util.GeometryBasics;

namespace Aura.Controls
{
    public class InfillTypeData
    {
        public INFILL_TYPE InfillType { get; set; }
        public string InfillTypeCaption { get; set; }
    }

    public class FiberInfillTypeData
    {
        public FiberInfillType InfillType { get; set; }
        public string InfillTypeCaption { get; set; }
    }

    public class AnisogridRibPlacementData
    {
        public AnisogridRibPlacement AnisogridRibPlacement { get; set; }
        public string AnisogridRibPlacementCaption { get; set; }
    }
}
