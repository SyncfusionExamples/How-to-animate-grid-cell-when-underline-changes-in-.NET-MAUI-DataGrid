using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace AnimateCell.Model
{
    public class Stock : INotifyPropertyChanged
    {
        private string symbol = string.Empty;
        private string account = string.Empty;
        private double lastTrade;
        private double change;
        private double open;
        private double previousClose;

        public string Symbol { get => symbol; set { if (symbol == value) return; symbol = value; OnPropertyChanged(nameof(Symbol)); } }
        public string Account { get => account; set { if (account == value) return; account = value; OnPropertyChanged(nameof(Account)); } }
        public double LastTrade { get => lastTrade; set { if (Math.Abs(lastTrade - value) < 0.0001) return; lastTrade = value; OnPropertyChanged(nameof(LastTrade)); } }
        public double Change { get => change; set { if (Math.Abs(change - value) < 0.0001) return; change = value; OnPropertyChanged(nameof(Change)); } }
        public double Open { get => open; set { if (Math.Abs(open - value) < 0.0001) return; open = value; OnPropertyChanged(nameof(Open)); } }
        public double PreviousClose { get => previousClose; set { if (Math.Abs(previousClose - value) < 0.0001) return; previousClose = value; OnPropertyChanged(nameof(PreviousClose)); } }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
