using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using devDept.Eyeshot;
using devDept.Eyeshot.Entities;
using devDept.Graphics;
using Block = devDept.Eyeshot.Block;
using DrawingColor = System.Drawing.Color;
using Material = devDept.Graphics.Material;
using MediaColor = System.Windows.Media.Color;
using Point3D = devDept.Geometry.Point3D;
using ProgressBar = devDept.Eyeshot.ProgressBar;
using Vector3D = devDept.Geometry.Vector3D;

namespace Aura
{
    public class AuraViewportLayout : Model
    {
        public class MyProgressBar : ProgressBar
        {
            public double Maximum { get; set; }

            private double _newValue;
            public double NewValue
            {
                get => _newValue;
                set
                {
                    _newValue = value;
                    Value = (int)(NewValue / Maximum * 100);
                }
            }

            public MyProgressBar()
            {
                Visible = false;
            }

            public void Draw(DrawSceneParams myParams)
            {
                base.Draw(myParams);
            }
        }

        public MyProgressBar AuraProgressBar = new MyProgressBar();
        private readonly Mesh _origin;
        internal Dictionary<MaterialTypes, string> AuraMaterials { get; private set; }

        #region ACCENT COLOR

        public static DependencyProperty AccentColorProperty =
            DependencyProperty.Register(
                "AccentColor",
                typeof(MediaColor),
                typeof(AuraViewportLayout),
                new PropertyMetadata(Color.FromArgb(255, 100, 100, 100), PropertyChangedAccentColor));

        private static void PropertyChangedAccentColor(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((AuraViewportLayout) d).OnPropertyChangedAccentColor(d, e);
        }

        private void OnPropertyChangedAccentColor(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var color = (MediaColor) e.NewValue;
            _origin.Color = DrawingColor.FromArgb(color.A, color.R, color.G, color.B);
        }

        public MediaColor AccentColor
        {
            get { return (MediaColor) GetValue(AccentColorProperty); }
            set { SetValue(AccentColorProperty, value); }
        }

        #endregion

        public AuraViewportLayout()
        {
            HideSmall = true;
            SmallSize = 10;
            
            SelectionColor = DrawingColor.DodgerBlue;
            AskForAntiAliasing = true;
            AntiAliasing = true;
            AntiAliasingSamples = antialiasingSamplesNumberType.x4;
            Rendered.ShowEdges = false;
            Rendered.PlanarReflections = true;
            

            #region ORIGIN

            _origin = Mesh.CreateCone(0, 2, new Point3D(0, 0, 0), new Point3D(0, 0, 8), 30);
            _origin.ColorMethod = colorMethodType.byEntity;

            var block = new Block(new Point3D(0, 0));
            block.Entities.Add(_origin);

            Blocks.Add("Origin", block);

            var blockReference = new BlockReference(new Point3D(0, 0), "Origin", 1, 1, 1, 0) {Selectable = false};
            Entities.Add(blockReference);

            #endregion

            AuraMaterials = new Dictionary<MaterialTypes, string>();
            
        }

        #region DRAW OVERLAY

        protected override void DrawOverlay(DrawSceneParams data)
        {
            base.DrawOverlay(data);

            if (AuraProgressBar.Visible)
            {
                data.Viewport = Viewports[ActiveViewport];
                AuraProgressBar.Draw(data);
            }
        }

        #endregion

        #region INITIALIZE

        public void Initialize()
        {
            var cube = GetViewCubeIcon();
            var color = (cube.BackColor as SolidColorBrush).Color;
            var brushColor = Color.FromArgb(100, color.R, color.G, color.B);
            cube.BackColor = new SolidColorBrush(brushColor);
            cube.FrontColor = new SolidColorBrush(brushColor);
            cube.TopColor = new SolidColorBrush(brushColor);
            cube.BottomColor = new SolidColorBrush(brushColor);
            cube.LeftColor = new SolidColorBrush(brushColor);
            cube.RightColor = new SolidColorBrush(brushColor);
            cube.FitAfterViewChange = false;

            var materialComposite = new Material(DrawingColor.FromArgb(255, 15, 15, 15),
                DrawingColor.FromArgb(255, 100, 100, 100),
                DrawingColor.FromArgb(255, 100, 100, 100),
                0f, 0.0f);
            Materials.Add("Composite", materialComposite);

            var materialTransparentComposite = new Material(DrawingColor.FromArgb(254, 15, 15, 15),
                DrawingColor.FromArgb(254, 100, 100, 100),
                DrawingColor.FromArgb(254, 100, 100, 100),
                0f, 0.0f);
            Materials.Add("TransparentComposite", materialTransparentComposite);

            var materialUltraTransparentComposite = new Material(DrawingColor.FromArgb(50, 15, 15, 15),
                DrawingColor.FromArgb(50, 100, 100, 100),
                DrawingColor.FromArgb(50, 100, 100, 100),
                0f, 0.0f);
            Materials.Add("UltraTransparentComposite", materialUltraTransparentComposite);

            var materialError = new Material(DrawingColor.FromArgb(255, 200, 50, 50),
                DrawingColor.FromArgb(255, 100, 100, 100),
                DrawingColor.FromArgb(255, 100, 100, 100),
                0f, 0.0f);
            Materials.Add("Error", materialError);

            AuraMaterials.Add(MaterialTypes.NORMAL, "Composite");
            AuraMaterials.Add(MaterialTypes.ERROR, "Error");
            AuraMaterials.Add(MaterialTypes.TRANSPARENT, "TransparentComposite");
            AuraMaterials.Add(MaterialTypes.ULTRA_TRANSPARENT, "UltraTransparentComposite");

            Light1 = new LightSettings(new Vector3D(0.8, 0.8, -1),
                DrawingColor.FromArgb(220, 220, 220))
            {
                Active = true,
                Type = lightType.Directional
            };
            Light2 = new LightSettings(new Vector3D(-0.8, 0.8, -1), DrawingColor.FromArgb(220, 220, 220))
            {
                Active = true,
                Type = lightType.Directional
            };
            Light3 = new LightSettings(new Vector3D(0.7, -0.7, -1), DrawingColor.FromArgb(110, 100, 100))
            {
                Active = true,
                Type = lightType.Directional
            };
            Light5.Active = false;
            Light6.Active = false;
            Light7.Active = false;
            Light8.Active = false;
        }

        #endregion

        protected override void OnKeyDown(KeyEventArgs e)
        {
            var isDel = e.Key == Key.Delete;
            var isCtrlC = e.Key == Key.C && (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl));
            var isCtrlV = e.Key == Key.V && (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl));
            var isCtrlX = e.Key == Key.X && (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl));
            if (isDel || isCtrlC || isCtrlV || isCtrlX)
                
            {
                return;
            }
            base.OnKeyDown(e);
        }
    }

    #region MATERIAL TYPES

    public enum MaterialTypes
    {
        NORMAL,
        ERROR,
        TRANSPARENT,
        ULTRA_TRANSPARENT
    }

    #endregion
}