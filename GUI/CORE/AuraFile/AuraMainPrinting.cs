using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Aura.Managers;
using Aura.ViewModels;
using devDept.Geometry;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using Settings.LayupData;
using Settings.Memento;

namespace Aura.CORE.AuraFile
{
    public class AuraMainPrinting
    {
        public void SaveFile(List<EntityDatasItemVM> entityLayupDatas, 
            ISessionMemento sessionMemento, 
            string path)
        {
            var modelsAndLayups = new List<EntityLayupData>();
            foreach (var entityLayupData in entityLayupDatas)
            {
                var mesh = AuraViewportManager.GetMesh(entityLayupData.Entity);
                var layupData = new EntityLayupData
                {
                    Points = mesh.Vertices.Select(a => new SerializationPoint3D(a.X, a.Y, a.Z)).ToList(),
                    LayupDatas = entityLayupData.Datas.Take(entityLayupData.Datas.Count-1).ToList(),
                    ModelName = entityLayupData.EntityName,
                    Enabled = entityLayupData.IsEnabled,
                    EnableData = entityLayupData.IsDataEnabled
                };
                var triangles = new List<SmoothTriangle>();
                foreach (var meshTriangle in mesh.Triangles)
                {
                    var sT = meshTriangle as SmoothTriangle;
                    triangles.Add(sT);
                }
                layupData.Triangles = triangles;
                modelsAndLayups.Add(layupData);
            }

            var ms = new MemoryStream();
            var projectName = Path.GetFileNameWithoutExtension(path);
            var memento = new AuraMainFile()
            {
                SessionMemento = sessionMemento,
                ModelsAndLayups = modelsAndLayups,
                ProjectName = projectName
            };
            using (var writer = new BsonWriter(ms))
            {
                var serializer = new JsonSerializer();
                serializer.Serialize(writer, memento);
                var data = Convert.ToBase64String(ms.ToArray());
                File.WriteAllText(path, data);
            }
        }

        #region LOAD FILE

        public static AuraMainFile ReadFile(string path)
        {
            using (var file = File.OpenText(path))
            {
                var text = file.ReadToEnd();
                var data = Convert.FromBase64String(text);
                var ms = new MemoryStream(data);
                using (var reader = new BsonReader(ms))
                {
                    var serializer = new JsonSerializer();
                    var auraFile = serializer.Deserialize<AuraMainFile>(reader);
                    auraFile.ProjectName = Path.GetFileNameWithoutExtension(path);
                    return auraFile;
                }
            }
        }

        public static (Dictionary<IMaterialPMemento, List<int>> plastic, Dictionary<IMaterialFMemento, List<int>> fibers) GetMaterialInExtruder(AuraMainFile auraMainFile)
        {
            var plastics = new Dictionary<IMaterialPMemento, List<int>>();
            foreach (var extruderPMaterial in auraMainFile.SessionMemento.ExtrudersPMaterials)
            {
                var itemInPlastic = plastics.SingleOrDefault(a => a.Key.Equals(extruderPMaterial.Material));
                if (itemInPlastic.Key == null) plastics.Add(extruderPMaterial.Material, new List<int> { extruderPMaterial.Index });
                else if (!itemInPlastic.Value.Contains(extruderPMaterial.Index)) itemInPlastic.Value.Add(extruderPMaterial.Index);
            }

            var fibers = new Dictionary<IMaterialFMemento, List<int>>();
            foreach (var extruderPFMaterial in auraMainFile.SessionMemento.ExtrudersPFMaterials)
            {
                var itemInPlastic = plastics.SingleOrDefault(a => a.Key.Equals(extruderPFMaterial.MaterialP));
                if (itemInPlastic.Key == null) plastics.Add(extruderPFMaterial.MaterialP, new List<int> { extruderPFMaterial.Index });
                else if (!itemInPlastic.Value.Contains(extruderPFMaterial.Index)) itemInPlastic.Value.Add(extruderPFMaterial.Index);

                var itemInFiber = fibers.SingleOrDefault(a => a.Key.Equals(extruderPFMaterial.MaterialF));
                if (itemInFiber.Key == null) fibers.Add(extruderPFMaterial.MaterialF, new List<int> { extruderPFMaterial.Index });
                else if (!itemInFiber.Value.Contains(extruderPFMaterial.Index)) itemInFiber.Value.Add(extruderPFMaterial.Index);
            }
            return (plastics, fibers);
        }

        public static void UpdatePlastics(AuraMainFile auraMainFile,
            Dictionary<IMaterialPMemento, List<int>> plasticInExtruders)
        {
            foreach (var plastic in plasticInExtruders)
            {
                var extrudersPlastic = auraMainFile.SessionMemento.ExtrudersPMaterials.Where(a => plastic.Value.Contains(a.Index));
                foreach (var extruderPMaterial in extrudersPlastic)
                {
                    extruderPMaterial.Material = plastic.Key as MaterialPMemento;
                }
                var extrudersFiber = auraMainFile.SessionMemento.ExtrudersPFMaterials.Where(a => plastic.Value.Contains(a.Index));
                foreach (var extruderPFMaterial in extrudersFiber)
                {
                    extruderPFMaterial.MaterialP = plastic.Key as MaterialPMemento;
                }
            }
        }

        public static void UpdateFibers(AuraMainFile auraMainFile,
            Dictionary<IMaterialFMemento, List<int>> fiberInExtruders)
        {
            foreach (var fiber in fiberInExtruders)
            {
                var extruders = auraMainFile.SessionMemento.ExtrudersPFMaterials.Where(a => fiber.Value.Contains(a.Index));
                foreach (var extruderPFMaterial in extruders)
                {
                    extruderPFMaterial.MaterialF = fiber.Key as MaterialFMemento;
                }
            }
        }

            #endregion

        [JsonObject(MemberSerialization.OptIn)]
        public class AuraMainFile
        {
            [JsonProperty]
            public List<EntityLayupData> ModelsAndLayups { get; set; }

            [JsonProperty]
            public ISessionMemento SessionMemento { get; set; }

            [JsonProperty]
            public string ProjectName { get; set; }

            [JsonConstructor]
            public AuraMainFile(List<EntityLayupData> modelsAndLayups, SessionMemento sessionMemento, string projectName) : this()
            {
                ModelsAndLayups = modelsAndLayups;
                if (ModelsAndLayups != null)
                {
                    //конвертируем предыдущие проекты, в которых еще не было приоритетов
                    //просто расставляем их по порядку, так чтобы они не повторялисб
                    BackwardCompabilityConverting(ModelsAndLayups, sessionMemento);
                }

                SessionMemento = sessionMemento;
                ProjectName = projectName;
            }

            public AuraMainFile()
            {
            }

            private void BackwardCompabilityConverting(List<EntityLayupData> modelsAndLayups, SessionMemento sessionMemento)
            {
                foreach (var modelsAndLayup in modelsAndLayups)
                {
                    if (modelsAndLayup.LayupDatas != null)
                    {
                        for (var i = 0; i < modelsAndLayup.LayupDatas.Count; i++)
                        {
                            var layupDataLayerIndex = modelsAndLayup.LayupDatas[i];
                            {
                                if (layupDataLayerIndex.RegionData.Priority == null || layupDataLayerIndex.RegionData.Priority == 0)
                                {
                                    layupDataLayerIndex.RegionData.Priority = i;
                                }
                            }
                        }

                        var rd = new RegionData
                        {
                            Priority = modelsAndLayup.LayupDatas.Count + 1,
                            StartHeightMM = modelsAndLayup.Points.Min(a => a.Z),
                            EndHeightMM = modelsAndLayup.Points.Max(a => a.Z),
                            LayupRule = new LayupRule(sessionMemento.Profile.LayupRule)
                        };
                        var ldLi = new LayupDataLayerIndex
                        {
                            RegionData = rd,
                            LayerIndex = modelsAndLayup.LayupDatas.Count + 1
                        };
                        modelsAndLayup.LayupDatas.Add(ldLi);
                    }
                }
            }
        }
    }
}
