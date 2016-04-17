using Microsoft.Band.Admin;
using Microsoft.Band.Admin.Streaming;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Band.Sensors;

namespace unBand.BandHelpers.Sensors
{
    public class BandPedometer : INotifyPropertyChanged
    {

        private CargoClient _client;
        private long _totalSteps;
        private long _totalMovements;

        public long TotalSteps
        {
            get { return _totalSteps; }
            set
            {
                if (_totalSteps != value)
                {
                    _totalSteps = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public long TotalMovements
        {
            get { return _totalMovements; }
            set
            {
                if (_totalMovements != value)
                {
                    _totalMovements = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public BandPedometer(CargoClient client)
        {
            _client = client;
        }

        public async Task InitAsync()
        {
            await OneTimePedometerReading();
        }

        /// <summary>
        /// To start us off we get an initial, one-off, Pedometer reading.
        /// To get consistent updates use TODO: StartPedometer();
        /// </summary>
        private async Task OneTimePedometerReading()
        {
            _client.Pedometer.ReadingChanged += _client_OneTimePedometerUpdated;
            await _client.Pedometer.StopReadingsAsync();
        }

        void _client_OneTimePedometerUpdated<T>(object sender, BandSensorReadingEventArgs<T> e) where T : IBandSensorReading
        {
            var asPedo = e.SensorReading as BandPedometerReading;
            if (asPedo != null)
            {
                _client.Pedometer.StopReadingsAsync();

                TotalSteps = asPedo.StepsToday;
                TotalMovements = asPedo.TotalSteps;

                _client.Pedometer.ReadingChanged -= _client_OneTimePedometerUpdated;
            }
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
                }));
            }
        }

        #endregion
    }
}
