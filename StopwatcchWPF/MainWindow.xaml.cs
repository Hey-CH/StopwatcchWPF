using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace StopwatcchWPF {
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window {
        ViewModel vm;
        DateTime now;
        DateTime startTime;
        BackgroundWorker bw;
        TimeSpan ts;
        List<TimeSpan> tsl = new List<TimeSpan>();
        public MainWindow() {
            InitializeComponent();
            vm = new ViewModel();
            this.DataContext = vm;
            ts = new TimeSpan(0);
            bw = new BackgroundWorker();
            bw.DoWork += Bw_DoWork;
            bw.RunWorkerAsync();
        }
        private void Bw_DoWork(object sender, DoWorkEventArgs e) {
            while (true) {
                now = DateTime.Now;
                if (vm.Mode == ViewModel.StopwatchMode.Stopwatch) {
                    ts = now - startTime;
                    try {
                        vm.Time = String.Format("{0:D2}:{1:D2}:{2:D2}.{3:D3}", ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds);
                    } catch (Exception ex) { }
                } else if (vm.Mode == ViewModel.StopwatchMode.Time) {
                    try {
                        vm.Time = String.Format("{0:D2}:{1:D2}:{2:D2}", now.Hour, now.Minute, now.Second, now.Millisecond);
                    } catch (Exception ex) { }
                } else if (vm.Mode == ViewModel.StopwatchMode.Pause) {
                    //何もしない
                }
            }
        }

        private void button1_Click(object sender, RoutedEventArgs e) {
            if ((string)button1.Content == "Start") {
                if (vm.Mode != ViewModel.StopwatchMode.Pause) {
                    startTime = DateTime.Now;
                } else if (vm.Mode == ViewModel.StopwatchMode.Pause) {
                    startTime = DateTime.Now - ts;
                }
                vm.Mode = ViewModel.StopwatchMode.Stopwatch;
                button1.Content = "Pause";
            } else {
                vm.Mode = ViewModel.StopwatchMode.Pause;
                button1.Content = "Start";
            }
        }

        private void button2_Click(object sender, RoutedEventArgs e) {
            vm.Mode = ViewModel.StopwatchMode.Time;
            button1.Content = "Start";
        }

        private void button3_Click(object sender, RoutedEventArgs e) {
            if (vm.Mode == ViewModel.StopwatchMode.Time) {
                vm.LAPs.Clear();
            } else if (vm.Mode == ViewModel.StopwatchMode.Stopwatch) {
                tsl.Add(ts);
                TimeSpan tmp = new TimeSpan(0);
                if (tsl.Count <= 1) {
                    tmp = tsl[tsl.Count - 1];
                } else {
                    tmp = tsl[tsl.Count - 1] - tsl[tsl.Count - 2];
                }
                var lap = new LAP();
                lap.Time = String.Format("{0:D2}:{1:D2}:{2:D2}.{3:D3}", now.Hour, now.Minute, now.Second, now.Millisecond);
                lap.LAPTime = String.Format("{0:D2}:{1:D2}:{2:D2}.{3:D3}", tmp.Hours, tmp.Minutes, tmp.Seconds, tmp.Milliseconds);
                vm.LAPs.Add(lap);
            }
        }
    }
    public class ViewModel : INotifyPropertyChanged {
        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged(string propertyName) {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        public enum StopwatchMode {
            Time,
            Stopwatch,
            Pause
        }
        StopwatchMode _Mode = StopwatchMode.Time;
        public StopwatchMode Mode {
            get { return _Mode; }
            set {
                _Mode = value;
                RaisePropertyChanged("Mode");
            }
        }
        string _Time = "00:00:00.000";
        public string Time {
            get { return _Time; }
            set {
                _Time = value;
                RaisePropertyChanged("Time");
                RaisePropertyChanged("LAPText");
            }
        }
        ObservableCollection<LAP> _LAPs = new ObservableCollection<LAP>();
        public ObservableCollection<LAP> LAPs {
            get { return _LAPs; }
            set {
                _LAPs = value;
                RaisePropertyChanged("LAPs");
            }
        }
        public string LAPText {
            get {
                if (Mode == StopwatchMode.Time && LAPs.Count > 0) {
                    return "C LAP";
                } else
                    return "LAP";
            }
        }
    }
    public class LAP {
        public string Time { get; set; }
        public string LAPTime { get; set; }
    }
}
