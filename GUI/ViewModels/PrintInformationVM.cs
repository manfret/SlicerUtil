using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Settings;

namespace Aura.ViewModels
{
    public class PrintInformationVM : INotifyPropertyChanged
    {
        #region PLASTIC CONSUMPTION

        private ObservableCollection<MaterialData> _plasticConsumption;
        public ObservableCollection<MaterialData> PlasticConsumptions
        {
            get => _plasticConsumption;
            private set
            {
                _plasticConsumption = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region FIBER CONSUMPTIONS

        private ObservableCollection<MaterialPFData> _compositeConsumptions;
        public ObservableCollection<MaterialPFData> CompositeConsumptions
        {
            get => _compositeConsumptions;
            private set
            {
                _compositeConsumptions = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region MASS

        private double _mass;
        public double Mass
        {
            get => _mass;
            private set
            {
                _mass = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region COST

        private double _cost;
        public double Cost
        {
            get => _cost;
            private set
            {
                _cost = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region HOURS

        private int _hours;
        public int Hours
        {
            get => _hours;
            private set
            {
                _hours = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region MINUTES

        public int _minutes;
        public int Minutes
        {
            get => _minutes;
            set
            {
                _minutes = value;
                OnPropertyChanged();
            }
        }

        #endregion

        public PrintInformationVM()
        {
            PlasticConsumptions = new ObservableCollection<MaterialData>();
            CompositeConsumptions = new ObservableCollection<MaterialPFData>();
        }

        public void Initialize(List<MaterialData> plasticConsumption, 
            List<MaterialPFData> fiberConsumptions,
            int hours,
            int minutes)
        {
            PlasticConsumptions.Clear();
            CompositeConsumptions.Clear();
            Mass = 0;
            Cost = 0;
            double mass = 0;
            double cost = 0;
            if (plasticConsumption != null && plasticConsumption.Any())
            {
                foreach (var materialData in plasticConsumption)
                {
                    PlasticConsumptions.Add(materialData);
                    mass += materialData.Mass;
                    cost += materialData.Cost;
                }
            }
            if (fiberConsumptions != null && fiberConsumptions.Any())
            {
                foreach (var materialPfData in fiberConsumptions)
                {
                    CompositeConsumptions.Add(materialPfData);
                    mass += materialPfData.MassP;
                    cost += materialPfData.MassP;
                    mass += materialPfData.MassF;
                    cost += materialPfData.MassF;
                }
            }
            Mass = mass;
            Cost = cost;
            Hours = hours;
            Minutes = minutes;
        }

        public void OnUnload()
        {

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