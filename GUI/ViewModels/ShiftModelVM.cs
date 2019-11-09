using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Aura.Managers;
using devDept.Eyeshot;
using devDept.Geometry;
using Prism.Commands;

namespace Aura.ViewModels
{
    public class ShiftModelVM : INotifyPropertyChanged
    {
        private IAuraViewportManager _auraViewportManager;
        private List<EntityDatasItemVM> _interactiveEntities;

        #region X

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

        public DelegateCommand ApplyCommand { get; private set; }
        public DelegateCommand CancelCommand { get; private set; }

        public event EventHandler Applied;
        public event EventHandler Canceled;

        public ShiftModelVM()
        {
            ApplyCommand = new DelegateCommand(ApplyExecuteMethod, ApplyCanExecuteMethod);
            CancelCommand = new DelegateCommand(CancelExecuteMethod, () => true);
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
            return !XHasError && !YHasError;
        }

        private void ApplyExecuteMethod()
        {
            _isChanged = false;
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
            var xyTranslation = new Translation(X - _manipulator.Position.X, Y - _manipulator.Position.Y, 0.0);
            //устанавливаем новую матрицу трансформации для переещения
            _auraViewportManager.SetObjectManipulatorTransformation(xyTranslation);
        }

        private static ObjectManipulator _manipulator;

        public void Initialize(IAuraViewportManager auraViewportManager, List<EntityDatasItemVM> entities)
        {
            _auraViewportManager = auraViewportManager;
            _interactiveEntities = entities;
        }

        private bool _isChanged;
        public void OnLoad()
        {
            _isChanged = true;
            _auraViewportManager.ConfigureObjectManipulatorForShift();
            _manipulator = _auraViewportManager.GetObjectManipulator();
            X = Math.Round(_manipulator.Position.X, 3);
            Y = Math.Round(_manipulator.Position.Y, 3);
            _manipulator.MouseDrag += ManipulatorOnMouseDrag;
            _auraViewportManager.Focus();
        }

        public void OnUnload()
        {
            if (_isChanged)
            {
                var applyCanExecute = ApplyCommand.CanExecute();
                if (applyCanExecute) ApplyCommand.Execute();
                else CancelCommand.Execute();
            }
            _manipulator.MouseDrag -= ManipulatorOnMouseDrag;
        }

        #region MANIPULATOR ON MOUSE DRAG

        private void ManipulatorOnMouseDrag(object sender, ObjectManipulator.ObjectManipulatorEventArgs e)
        {
            var transformation = _auraViewportManager.GetObjectManipulatorTransformation();
            var deltaX = Math.Round(transformation.Matrix[0, 3], 3);
            var deltaY = Math.Round(transformation.Matrix[1, 3], 3);
            X = Math.Round(_manipulator.Position.X + deltaX, 3);
            Y = Math.Round(_manipulator.Position.Y + deltaY, 3);
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