using cAlgo.API;
using cAlgo.API.Indicators;
using cAlgo.API.Internals;
using System;
using System.Collections.Generic;

namespace cAlgo
{
    [Indicator(IsOverlay = true, TimeZone = TimeZones.UTC, AccessRights = AccessRights.None)]
    public class MultiPivotLevels : Indicator
    {
        [Parameter("Use Auto Timeframe", DefaultValue = true)]
        public bool UseAutoTimeframe { get; set; }

        [Parameter("Pivot Timeframe", DefaultValue = "Weekly")]
        public TimeFrame PivotTimeframe { get; set; }

        [Parameter("Pivot Method", DefaultValue = PivotMethod.Classic)]
        public PivotMethod Method { get; set; }

        [Parameter("Levels Color", DefaultValue = "Gray")]
        public Color LineColor { get; set; }

        [Parameter("Previous Levels Color", DefaultValue = "DarkGray")]
        public Color PreviousLineColor { get; set; }

        [Parameter("Show Previous Levels", DefaultValue = 1, MinValue = 0)]
        public int PreviousDays { get; set; }

        [Parameter("Show Price Labels", DefaultValue = true)]
        public bool ShowPriceLabels { get; set; }

        private Bars _pivotBars;
        private DateTime _lastPivotTime = DateTime.MinValue; // Melacak waktu pivot terakhir
        private bool _isInitialized = false; // Melacak apakah level sudah digambar pertama kali

        public enum PivotMethod
        {
            Classic,
            Fibonacci,
            Camarilla,
            Woodie,
            DeMark
        }

        protected override void Initialize()
        {
            TimeFrame selectedTimeframe = UseAutoTimeframe ? GetPivotTimeFrame(Chart.TimeFrame) : PivotTimeframe;
            _pivotBars = MarketData.GetBars(selectedTimeframe);
        }

        public override void Calculate(int index)
        {
            if (!IsLastBar)
                return;

            var currentPivotTime = _pivotBars.OpenTimes[_pivotBars.Count - 2];

            // Jika belum diinisialisasi atau ada pivot baru, gambar semua level
            if (!_isInitialized || currentPivotTime != _lastPivotTime)
            {
                Chart.RemoveAllObjects(); // Hapus semua objek hanya saat inisialisasi atau pivot baru
                _lastPivotTime = currentPivotTime;
                DrawAllLevels();
                _isInitialized = true;
            }
            else
            {
                // Hanya perbarui level terbaru tanpa menghapus yang lain
                UpdateCurrentLevel();
            }
        }

        private void DrawAllLevels()
        {
            for (int dayOffset = 0; dayOffset <= PreviousDays; dayOffset++)
            {
                var pivotBarIndex = _pivotBars.Count - 2 - dayOffset;
                if (pivotBarIndex < 0)
                    continue;

                var high = _pivotBars.HighPrices[pivotBarIndex];
                var low = _pivotBars.LowPrices[pivotBarIndex];
                var close = _pivotBars.ClosePrices[pivotBarIndex];
                var open = _pivotBars.OpenPrices[pivotBarIndex];

                DateTime startTime = _pivotBars.OpenTimes[pivotBarIndex];
                DateTime endTime = dayOffset == 0 
                    ? Bars.OpenTimes.LastValue 
                    : startTime + GetTimeFrameDuration(_pivotBars.TimeFrame);

                double pivot, r1, r2, r3, s1, s2, s3;
                CalculatePivotLevels(high, low, close, open, out pivot, out r1, out r2, out r3, out s1, out s2, out s3);

                var color = dayOffset == 0 ? LineColor : PreviousLineColor;
                var nameSuffix = dayOffset == 0 ? "" : $"_prev{dayOffset}";

                DrawLevel($"Pivot{nameSuffix}", pivot, $"P{nameSuffix}", color, startTime, endTime, pivot);
                DrawLevel($"R1{nameSuffix}", r1, $"R1{nameSuffix}", color, startTime, endTime, r1);
                DrawLevel($"R2{nameSuffix}", r2, $"R2{nameSuffix}", color, startTime, endTime, r2);
                DrawLevel($"R3{nameSuffix}", r3, $"R3{nameSuffix}", color, startTime, endTime, r3);
                DrawLevel($"S1{nameSuffix}", s1, $"S1{nameSuffix}", color, startTime, endTime, s1);
                DrawLevel($"S2{nameSuffix}", s2, $"S2{nameSuffix}", color, startTime, endTime, s2);
                DrawLevel($"S3{nameSuffix}", s3, $"S3{nameSuffix}", color, startTime, endTime, s3);
            }
        }

        private void UpdateCurrentLevel()
        {
            var pivotBarIndex = _pivotBars.Count - 2;
            if (pivotBarIndex < 0)
                return;

            var high = _pivotBars.HighPrices[pivotBarIndex];
            var low = _pivotBars.LowPrices[pivotBarIndex];
            var close = _pivotBars.ClosePrices[pivotBarIndex];
            var open = _pivotBars.OpenPrices[pivotBarIndex];

            DateTime startTime = _pivotBars.OpenTimes[pivotBarIndex];
            DateTime endTime = Bars.OpenTimes.LastValue; // Selalu perpanjang ke kanan untuk level terbaru

            double pivot, r1, r2, r3, s1, s2, s3;
            CalculatePivotLevels(high, low, close, open, out pivot, out r1, out r2, out r3, out s1, out s2, out s3);

            var color = LineColor;
            var nameSuffix = "";

            // Perbarui level terbaru tanpa menghapus level sebelumnya
            DrawLevel($"Pivot{nameSuffix}", pivot, $"P{nameSuffix}", color, startTime, endTime, pivot);
            DrawLevel($"R1{nameSuffix}", r1, $"R1{nameSuffix}", color, startTime, endTime, r1);
            DrawLevel($"R2{nameSuffix}", r2, $"R2{nameSuffix}", color, startTime, endTime, r2);
            DrawLevel($"R3{nameSuffix}", r3, $"R3{nameSuffix}", color, startTime, endTime, r3);
            DrawLevel($"S1{nameSuffix}", s1, $"S1{nameSuffix}", color, startTime, endTime, s1);
            DrawLevel($"S2{nameSuffix}", s2, $"S2{nameSuffix}", color, startTime, endTime, s2);
            DrawLevel($"S3{nameSuffix}", s3, $"S3{nameSuffix}", color, startTime, endTime, s3);
        }

        private void CalculatePivotLevels(double high, double low, double close, double open, 
            out double pivot, out double r1, out double r2, out double r3, out double s1, out double s2, out double s3)
        {
            switch (Method)
            {
                case PivotMethod.Classic:
                    pivot = (high + low + close) / 3;
                    r1 = (2 * pivot) - low;
                    s1 = (2 * pivot) - high;
                    r2 = pivot + (high - low);
                    s2 = pivot - (high - low);
                    r3 = high + 2 * (pivot - low);
                    s3 = low - 2 * (high - pivot);
                    break;

                case PivotMethod.Fibonacci:
                    pivot = (high + low + close) / 3;
                    var range = high - low;
                    r1 = pivot + (0.382 * range);
                    r2 = pivot + (0.618 * range);
                    r3 = pivot + (1.000 * range);
                    s1 = pivot - (0.382 * range);
                    s2 = pivot - (0.618 * range);
                    s3 = pivot - (1.000 * range);
                    break;

                case PivotMethod.Camarilla:
                    range = high - low;
                    pivot = (high + low + close) / 3;
                    r1 = close + range * 1.1 / 12;
                    r2 = close + range * 1.1 / 6;
                    r3 = close + range * 1.1 / 4;
                    s1 = close - range * 1.1 / 12;
                    s2 = close - range * 1.1 / 6;
                    s3 = close - range * 1.1 / 4;
                    break;

                case PivotMethod.Woodie:
                    pivot = (high + low + (2 * close)) / 4;
                    r1 = (2 * pivot) - low;
                    s1 = (2 * pivot) - high;
                    r2 = pivot + (high - low);
                    s2 = pivot - (high - low);
                    r3 = high + 2 * (pivot - low);
                    s3 = low - 2 * (high - pivot);
                    break;

                case PivotMethod.DeMark:
                    if (close < open)
                    {
                        pivot = high + (2 * low) + close;
                    }
                    else if (close > open)
                    {
                        pivot = (2 * high) + low + close;
                    }
                    else
                    {
                        pivot = high + low + (2 * close);
                    }
                    pivot = pivot / 4;
                    r1 = (2 * pivot) - low;
                    s1 = (2 * pivot) - high;
                    r2 = pivot + (high - low);
                    s2 = pivot - (high - low);
                    r3 = high + 2 * (pivot - low);
                    s3 = low - 2 * (high - pivot);
                    break;

                default:
                    pivot = r1 = r2 = r3 = s1 = s2 = s3 = 0;
                    return;
            }
        }

        private TimeFrame GetPivotTimeFrame(TimeFrame chartTimeFrame)
        {
            if (chartTimeFrame == TimeFrame.Hour4)
                return TimeFrame.Weekly;
            if (chartTimeFrame == TimeFrame.Minute)
                return TimeFrame.Hour;
            if (chartTimeFrame == TimeFrame.Minute5)
                return TimeFrame.Hour;
            if (chartTimeFrame == TimeFrame.Minute15)
                return TimeFrame.Hour4;
            if (chartTimeFrame == TimeFrame.Minute30)
                return TimeFrame.Hour4;
            if (chartTimeFrame == TimeFrame.Hour)
                return TimeFrame.Daily;
            if (chartTimeFrame == TimeFrame.Daily)
                return TimeFrame.Weekly;
            if (chartTimeFrame == TimeFrame.Weekly)
                return TimeFrame.Monthly;
            if (chartTimeFrame == TimeFrame.Monthly)
                return TimeFrame.Monthly;

            return TimeFrame.Daily;
        }

        private TimeSpan GetTimeFrameDuration(TimeFrame timeFrame)
        {
            if (timeFrame == TimeFrame.Minute)
                return TimeSpan.FromMinutes(1);
            if (timeFrame == TimeFrame.Minute5)
                return TimeSpan.FromMinutes(5);
            if (timeFrame == TimeFrame.Minute15)
                return TimeSpan.FromMinutes(15);
            if (timeFrame == TimeFrame.Minute30)
                return TimeSpan.FromMinutes(30);
            if (timeFrame == TimeFrame.Hour)
                return TimeSpan.FromHours(1);
            if (timeFrame == TimeFrame.Hour4)
                return TimeSpan.FromHours(4);
            if (timeFrame == TimeFrame.Daily)
                return TimeSpan.FromDays(1);
            if (timeFrame == TimeFrame.Weekly)
                return TimeSpan.FromDays(7);
            if (timeFrame == TimeFrame.Monthly)
                return TimeSpan.FromDays(30);

            return TimeSpan.FromDays(1);
        }

        private void DrawLevel(string name, double price, string label, Color color, DateTime startTime, DateTime endTime, double priceValue)
        {
            Chart.DrawTrendLine(name, startTime, price, endTime, price, color, 1, LineStyle.Solid);

            DateTime labelTime = endTime;
            if (endTime == Bars.OpenTimes.LastValue)
            {
                int offsetBars = 5;
                int labelIndex = Bars.Count - 1 - offsetBars;
                if (labelIndex >= 0)
                    labelTime = Bars.OpenTimes[labelIndex];
            }

            string displayLabel = ShowPriceLabels ? $"{label} ({priceValue:F2})" : label;
            Chart.DrawText(name + "_label", displayLabel, labelTime, price, color);
        }
    }
}