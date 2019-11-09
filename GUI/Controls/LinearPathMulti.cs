using System.Drawing;
using devDept.Eyeshot;
using devDept.Eyeshot.Entities;
using devDept.Geometry;
using devDept.Graphics;
using Point = System.Drawing.Point;

namespace Aura.Controls
{
    public class LinearPathMulti : LinearPath
    {
        public Point3D[][] Paths { get; set; }

        private Layer _layer;

        public LinearPathMulti(Point3D[][] paths, Point3D boxMin, Point3D boxMax, Layer layer = null) : base(boxMin, boxMax)
        {
            Paths = paths;
            _layer = layer;
        }

        protected override void DrawWireEntity(RenderContextBase context, object myParams)
        {
            foreach (var path in Paths)
            {
                context.DrawLineStrip(path);
            }
        }

        protected override void Draw(DrawParams data)
        {
            var color = _layer?.Color ?? Color;
            data.RenderContext.SetColorWireframe(color);
            data.RenderContext.SetShader(shaderType.NoLights, null, false);
            data.RenderContext.SetLineSize(LineWeight);
            base.Draw(data);
        }
    }
}