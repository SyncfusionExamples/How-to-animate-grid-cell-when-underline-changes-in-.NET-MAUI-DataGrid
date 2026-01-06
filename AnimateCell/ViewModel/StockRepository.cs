using AnimateCell.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace AnimateCell.ViewModel
{
    public class StockRepository
    {
        private readonly Random rnd = Random.Shared;
        public ObservableCollection<Stock> Stocks { get; } = new();

        public StockRepository()
        {
            Seed();
            StartRandomFeed();
        }

        private void Seed()
        {
            var symbols = new[] { "DZDW", "HYSN", "KLXH", "ALQJ", "AODF", "LKWO", "TVOQ", "JVPO", "QVWZ", "LYQF", "IPKG", "QSVG", "RLIK", "HCPT" };
            var accounts = new[] { "AmericanFunds", "ChildrenCollegeSavings", "DayTrading", "RetirementSavings", "MountainRanges", "FidelityFunds", "Mortgages", "HousingLoans" };

            for (int i = 0; i < symbols.Length; i++)
            {
                var open = Next(10, 60);
                var prev = open + Next(-5, 5);
                Stocks.Add(new Stock
                {
                    Symbol = symbols[i],
                    Account = accounts[rnd.Next(accounts.Length)],
                    Open = open,
                    PreviousClose = prev,
                    LastTrade = open + Next(-3, 3),
                    Change = Next(-20, 20)
                });
            }
        }

        private double Next(double min, double max) => min + rnd.NextDouble() * (max - min);

        private void StartRandomFeed()
        {
            Application.Current?.Dispatcher.StartTimer(TimeSpan.FromMilliseconds(680), () =>
            {
                if (Stocks.Count == 0) return true;
                int count = Math.Max(1, Stocks.Count * rnd.Next(8, 16) / 100);
                UpdateRandomCells(count, s =>
                {
                    var delta = Next(-3.5, 3.5);
                    s.LastTrade = Math.Max(0, s.LastTrade + delta);
                });
                return true;
            });

            Application.Current?.Dispatcher.StartTimer(TimeSpan.FromMilliseconds(640), () =>
            {
                if (Stocks.Count == 0) return true;
                int count = Math.Max(1, Stocks.Count * rnd.Next(8, 18) / 100);
                UpdateRandomCells(count, s => s.Change = Next(-30, 30));
                return true;
            });

            Application.Current?.Dispatcher.StartTimer(TimeSpan.FromMilliseconds(760), () =>
            {
                if (Stocks.Count == 0) return true;
                int count = Math.Max(1, Stocks.Count * rnd.Next(6, 12) / 100);
                UpdateRandomCells(count, s => s.Open = Math.Max(0, s.Open + Next(-1.5, 1.5)));
                return true;
            });

            Application.Current?.Dispatcher.StartTimer(TimeSpan.FromMilliseconds(820), () =>
            {
                if (Stocks.Count == 0) return true;
                int count = Math.Max(1, Stocks.Count * rnd.Next(6, 12) / 100);
                UpdateRandomCells(count, s => s.PreviousClose = Math.Max(0, s.PreviousClose + Next(-1.5, 1.5)));
                return true;
            });
        }

        private void UpdateRandomCells(int count, Action<Stock> mutator)
        {
            if (Stocks.Count == 0) return;
            var picked = new HashSet<int>();
            while (picked.Count < count && picked.Count < Stocks.Count)
            {
                picked.Add(rnd.Next(0, Stocks.Count));
            }

            foreach (var idx in picked)
            {
                var s = Stocks[idx];
                mutator(s);
            }
        }
    }
}
