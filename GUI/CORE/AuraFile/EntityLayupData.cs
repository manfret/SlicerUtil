using System.Collections.Generic;
using devDept.Geometry;
using Newtonsoft.Json;
using Settings.LayupData;

namespace Aura.CORE.AuraFile
{
    [JsonObject(MemberSerialization.OptIn)]
    public struct EntityLayupData
    {
        [JsonProperty]
        public List<SerializationPoint3D> Points { get; set; }

        [JsonProperty]
        public List<SmoothTriangle> Triangles { get; set; }

        [JsonProperty]
        public List<LayupDataLayerIndex> LayupDatas { get; set; }

        [JsonProperty]
        public string ModelName { get; set; }

        [JsonProperty]
        public bool? Enabled { get; set; }

        public bool TrueEnabled => Enabled.Value;

        [JsonProperty]
        public bool? EnableData { get; set; }

        public bool TrueEnableData => EnableData.Value;

        [JsonConstructor]
        public EntityLayupData(List<SerializationPoint3D> points, List<SmoothTriangle> triangles, List<LayupDataLayerIndex> layupDatas, string modelName, bool? enabled, bool? enabledData)
        {
            Points = points;
            Triangles = triangles;
            LayupDatas = layupDatas;
            ModelName = modelName;
            Enabled = enabled ?? true;
            EnableData = enabledData ?? true;
        }
    }

    //уже не нужен, сотавляем для обратной совместимости
    [JsonObject(MemberSerialization.OptIn)]
    public class LayupDataLayerIndex
    {
        [JsonProperty]
        public IRegionData RegionData { get; set; }

        [JsonProperty]
        public int LayerIndex { get; set; }

        [JsonConstructor]
        public LayupDataLayerIndex(RegionData regionData, int layerIndex)
        {
            RegionData = regionData;
            LayerIndex = layerIndex;
        }

        public LayupDataLayerIndex()
        {
        }

        public LayupDataLayerIndex Clone()
        {
            return new LayupDataLayerIndex(new RegionData(this.RegionData), LayerIndex);
        }
    }

    [JsonObject(MemberSerialization.OptIn)]
    public struct SerializationPoint3D
    {
        [JsonProperty]
        public double X { get; set; }

        [JsonProperty]
        public double Y { get; set; }

        [JsonProperty]
        public double Z { get; set; }

        [JsonConstructor]
        public SerializationPoint3D(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }
    }
}