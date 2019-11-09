using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Aura.Managers;
using devDept.Geometry;
using Prism.Commands;

namespace Aura.ViewModels
{
    public class ResizeModelVM : INotifyPropertyChanged
    {
        private IAuraViewportManager _auraViewportManager;
        private ObservableCollection<EntityDatasItemVM> _interactiveEntities;

        #region IS UNIFORM

        private bool _isUniform;
        public bool IsUniform
        {
            get => _isUniform;
            set
            {
                _isUniform = value;
                OnPropertyChanged();
                UpdateManipulator();
            }
        }

        #endregion

        #region UNIFORM SIZE

        private double _oldUniformSize;
        private double _uniformSize;

        public double UniformSize
        {
            get => _uniformSize;
            set
            {
                _uniformSize = value;
                OnPropertyChanged();
                UpdateManipulator();
            }
        }

        #endregion

        #region UNIFORM SIZE HAS ERROR

        private bool _uniformSizeHasError;
        public bool UniformSizeHasError
        {
            get => _uniformSizeHasError;
            set
            {
                _uniformSizeHasError = value;
                ApplyCommand?.RaiseCanExecuteChanged();
                OnPropertyChanged();
            }
        }

        #endregion

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

        public ResizeModelVM()
        {
            ApplyCommand = new DelegateCommand(ApplyExecuteMethod, ApplyCanExecuteMethod);
            CanceCommand = new DelegateCommand(CancelExecuteMethod, () => true);
        }

        public void Cancel()
        {
            _isChanged = false;
            //сбрасываем все перемещения
            _auraViewportManager.SetObjectManipulatorTransformation(new Identity());
            _auraViewportManager.ObjectManipulatorCancel();
        }

        private void CancelExecuteMethod()
        {
            Cancel();
            Canceled?.Invoke(this, null);
        }

        private bool ApplyCanExecuteMethod()
        {
            return IsUniform? !XHasError && !YHasError && !ZHasError : !UniformSizeHasError;
        }

        private void ApplyExecuteMethod()
        {
            _isChanged = false;
            foreach (var interactiveEntity in _interactiveEntities)
            {
                if (IsUniform) interactiveEntity.UpdateScaleData(UniformSize, UniformSize, UniformSize);
                else interactiveEntity.UpdateScaleData(X, Y, Z);
            }
            //устанавливаем новую матрицу трансформации для переещения
            var xyzResize = new Identity();
            var omPosition = _auraViewportManager.GetObjectManipulatorPosition();
            if (IsUniform)
            {
                var newUniformScale = UniformSize / _oldUniformSize;
                xyzResize.Scaling(omPosition, newUniformScale, newUniformScale, newUniformScale);
            }
            else xyzResize.Scaling(omPosition, X / _oldX, Y / _oldY, Z / _oldZ);
            _auraViewportManager.SetObjectManipulatorTransformation(xyzResize);

            //применяем изменения
            _auraViewportManager.ObjectManipulatorApply();
            foreach (var interactiveEntity in _interactiveEntities)
            {
                _auraViewportManager.PutEntityOnTable(interactiveEntity.Entity);
            }
            _auraViewportManager.CheckIntersections();
            Applied?.Invoke(this, null);

        }

        public void OnLoad()
        {
            _isChanged = true;
            _auraViewportManager.ConfigureObjectManipulatorForResize();
            IsUniform = true;

            if (_interactiveEntities.Count == 1)
            {
                X = _interactiveEntities[0].ScaleData.X;
                Y = _interactiveEntities[0].ScaleData.Y;
                Z = _interactiveEntities[0].ScaleData.Z;
            }
            else
            {
                X = 100;
                Y = 100;
                Z = 100;
            }

            if (Math.Abs(X - Y) < 0.001 && Math.Abs(Y - Z) < 0.001) UniformSize = X;
            else UniformSize = 100;

            _oldX = X;
            _oldY = Y;
            _oldZ = Z;
            _oldUniformSize = UniformSize;
        }

        private bool _isChanged;
        public void OnUnload()
        {
            if (_isChanged)
            {
                var applyCanExecute = ApplyCommand.CanExecute();
                if (applyCanExecute) ApplyCommand.Execute();
                else CanceCommand.Execute();
            }
        }

        private void UpdateManipulator()
        {
            var xyzResize = new Identity();
            var omPosition = _auraViewportManager.GetObjectManipulatorPosition();
            if (IsUniform)
            {
                var newUniformScale = UniformSize / _oldUniformSize;
                xyzResize.Scaling(omPosition, newUniformScale, newUniformScale, newUniformScale);
            }
            else xyzResize.Scaling(omPosition, X / _oldX, Y / _oldY, Z / _oldZ);
            _auraViewportManager.SetObjectManipulatorTransformation(xyzResize);
        }

        public void Initialize(IAuraViewportManager auraViewportManager, ObservableCollection<EntityDatasItemVM> entities)
        {
            _auraViewportManager = auraViewportManager;
            _interactiveEntities = entities;
        }

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