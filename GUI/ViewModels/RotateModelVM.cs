using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Aura.Managers;
using devDept.Eyeshot;
using devDept.Geometry;
using Prism.Commands;
using Util.GeometryBasics;

namespace Aura.ViewModels
{
    public class RotateModelVM : INotifyPropertyChanged
    {
        private IAuraViewportManager _auraViewportManager;
        private ObservableCollection<EntityDatasItemVM> _interactiveEntities;

        #region X

        private double _oldX;
        private double _x;
        public double X
        {
            get => _x;
            set
            {
                _x = value;
                OnPropertyChanged();
                UpdateManipulator();
            }
        }

        #endregion

        #region X HAS ERROR

        private bool _xHasError;
        public bool XHasError
        {
            get => _xHasError;
            set
            {
                _xHasError = value;
                ApplyCommand?.RaiseCanExecuteChanged();
                OnPropertyChanged();
            }
        }

        #endregion

        #region Y

        private double _oldY;
        private double _y;
        public double Y
        {
            get => _y;
            set
            {
                _y = value;
                OnPropertyChanged();
                UpdateManipulator();
            }
        }

        #endregion

        #region Y HAS ERROR

        private bool _yHasError;
        public bool YHasError
        {
            get => _yHasError;
            set
            {
                _yHasError = value;
                ApplyCommand?.RaiseCanExecuteChanged();
                OnPropertyChanged();
            }
        }

        #endregion

        #region Z

        private double _oldZ;
        private double _z;
        public double Z
        {
            get => _z;
            set
            {
                _z = value;
                OnPropertyChanged();
                UpdateManipulator();
            }
        }

        #endregion

        #region Z HAS ERROR

        private bool _zHasError;
        public bool ZHasError
        {
            get => _zHasError;
            set
            {
                _zHasError = value;
                ApplyCommand?.RaiseCanExecuteChanged();
                OnPropertyChanged();
            }
        }

        #endregion

        public DelegateCommand ApplyCommand { get; private set; }
        public DelegateCommand CanceCommand { get; private set; }

        public event EventHandler Applied;
        public event EventHandler Canceled;

        public RotateModelVM()
        {
            ApplyCommand = new DelegateCommand(ApplyExecuteMethod, ApplyCanExecuteMethod);
            CanceCommand = new DelegateCommand(CancelExecuteMethod, () => true);
        }

        public void Cancel()
        {
            //сбрасываем все перемещения
            _auraViewportManager.SetObjectManipulatorTransformation(new Identity());
            _auraViewportManager.ObjectManipulatorCancel();
        }

        private void CancelExecuteMethod()
        {
            _isChanged = false;
            Cancel();
            Canceled?.Invoke(this, null);
        }

        private bool ApplyCanExecuteMethod()
        {
            return !XHasError && !YHasError && !ZHasError;
        }

        private void ApplyExecuteMethod()
        {
            _isChanged = false;
            foreach (var interactiveEntity in _interactiveEntities)
            {
                X = InfillHelper.RecalculateAngle(X);
                Y = InfillHelper.RecalculateAngle(Y);
                Z = InfillHelper.RecalculateAngle(Z);
                interactiveEntity.UpdateRotationData(X, Y, Z);
            }
            //применяем изменения
            _auraViewportManager.ObjectManipulatorApply();
            foreach (var interactiveEntity in _interactiveEntities)
            {
                _auraViewportManager.PutEntityOnTable(interactiveEntity.Entity);
            }
            _auraViewportManager.CheckIntersections();
            Applied?.Invoke(this, null);

        }

        private void UpdateManipulator()
        {
            var xRad = (X - _oldX) * Math.PI / 180;
            var yRad = (Y - _oldY) * Math.PI / 180;
            var zRad = (Z - _oldZ) * Math.PI / 180;

            var omPosition = _auraViewportManager.GetObjectManipulatorPosition();

            var xTr = new Rotation(xRad, Vector3D.AxisX, omPosition);
            var yTr = new Rotation(yRad, Vector3D.AxisY, omPosition);
            var zTr = new Rotation(zRad, Vector3D.AxisZ, omPosition);

            var tr = xTr * yTr * zTr;
            //устанавливаем новую матрицу трансформации для переещения
            _auraViewportManager.SetObjectManipulatorTransformation(tr);
        }

        private static ObjectManipulator _manipulator;
        public void Initialize(IAuraViewportManager auraViewportManager, ObservableCollection<EntityDatasItemVM> entities)
        {
            _auraViewportManager = auraViewportManager;
            _interactiveEntities = entities;
            _auraViewportManager.Focus();
        }

        private bool _isChanged;

        public void OnLoad()
        {
            _isChanged = true;
            _auraViewportManager.ConfigureObjectManipulatorForRotate();
            _manipulator = _auraViewportManager.GetObjectManipulator();
            _manipulator.MouseDrag += ManipulatorOnMouseDrag;

            if (_interactiveEntities.Count == 1)
            {
                X = _interactiveEntities[0].RotationData.X;
                Y = _interactiveEntities[0].RotationData.Y;
                Z = _interactiveEntities[0].RotationData.Z;
            }
            else
            {
                X = 0;
                Y = 0;
                Z = 0;
            }

            _oldX = X;
            _oldY = Y;
            _oldZ = Z;
        }

        public void OnUnload()
        {
            if (_isChanged)
            {
                var applyCanExecute = ApplyCommand.CanExecute();
                if (applyCanExecute) ApplyCommand.Execute();
                else CanceCommand.Execute();
            }
            _manipulator.MouseDrag -= ManipulatorOnMouseDrag;
        }

        #region MANIPULATOR ON MOUSE DRAG

        private void ManipulatorOnMouseDrag(object sender, ObjectManipulator.ObjectManipulatorEventArgs e)
        {
            var transformation = _auraViewportManager.GetObjectManipulatorTransformation();
            double ax, ay, az;
            if (Math.Abs(transformation.Matrix[2, 0] - 1) < 0.0001 || Math.Abs(transformation.Matrix[2, 0] - (-1)) < 0.0001)
            {
                az = 0;
                var delta = Math.Atan2(transformation.Matrix[1, 0], transformation.Matrix[2, 0]);
                if (Math.Abs(transformation.Matrix[2, 0] - (-1)) < 0.0001)
                {
                    ay = Math.PI / 2;
                    ax = az + delta;
                }
                else
                {
                    ay = -Math.PI / 2;
                    ax = -az + delta;
                }
            }
            else
            {
                ay = -Math.Asin(transformation.Matrix[2, 0]);
                ax = Math.Atan2(transformation.Matrix[2, 1] / Math.Cos(ay),
                    transformation.Matrix[2, 2] / Math.Cos(ay));
                az = Math.Atan2(transformation.Matrix[1, 0] / Math.Cos(ay),
                    transformation.Matrix[0, 0] / Math.Cos(ay));
            }

            if (Math.Abs(ax - Math.PI) < 0.0001) ax = 0;
            if (Math.Abs(ay - Math.PI) < 0.0001) ay = 0;
            if (Math.Abs(az - Math.PI) < 0.0001) az = 0;

            X = Math.Round(ax * 180 / Math.PI, 3);
            Y = Math.Round(ay * 180 / Math.PI, 3);
            Z = Math.Round(az * 180 / Math.PI, 3);
        }

        #endregion

        #region ON PROPERTY CHANGED

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string caller = null)
        {
            var handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(caller));
        }

        #endregion
    }
}