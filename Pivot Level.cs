using cAlgo.API;
using cAlgo.API.Indicators;
using cAlgo.API.Internals;
using System;
using System.Collections.Generic;
using System.Linq;

namespace cAlgo
{
    [Indicator(IsOverlay = true, TimeZone = TimeZones.UTC, AccessRights = AccessRights.None)]
    public class MultiPivotLevels : Indicator
    {
        #region Parameters
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

        [Parameter("Show Previous Levels", DefaultValue = 1, MinValue = 0, MaxValue = 10)]
        public int PreviousDays { get; set; }

        [Parameter("Show Price Labels", DefaultValue = true)]
        public bool ShowPriceLabels { get; set; }

        [Parameter("Label Offset (bars)", DefaultValue = 5, MinValue = 0, MaxValue = 20)]
        public int LabelOffset { get; set; }

        [Parameter("Line Thickness", DefaultValue = 1, MinValue = 1, MaxValue = 5)]
        public int LineThickness { get; set; }

        [Parameter("Line Style", DefaultValue = LineStyle.Solid)]
        public LineStyle LineStyleType { get; set; }

        [Parameter("Show R3/S3", DefaultValue = true)]
        public bool ShowExtremeLevels { get; set; }

        [Parameter("Show Debug Info", DefaultValue = false)]
        public bool ShowDebug { get; set; }

        [Parameter("Extend Lines Right", DefaultValue = true)]
        public bool ExtendRight { get; set; }
        #endregion

        #region Private Fields
        private Bars _pivotBars;
        private DateTime _lastPivotTime = DateTime.MinValue;
        private bool _isInitialized = false;
        private const string INDICATOR_PREFIX = "MPL_";
        #endregion

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
            try
            {
                TimeFrame selectedTimeframe = UseAutoTimeframe ? GetPivotTimeFrame(Chart.TimeFrame) : PivotTimeframe;
                _pivotBars = MarketData.GetBars(selectedTimeframe);
                DebugPrint($"Initialized with timeframe: {selectedTimeframe}");
            }
            catch (Exception ex)
            {
                Print($"Error initializing MultiPivotLevels: {ex.Message}");
            }
        }

        public override void Calculate(int index)
        {
            try
            {
                if (!IsLastBar)
                    return;

                if (_pivotBars == null || _pivotBars.Count < 2)
                {
                    DebugPrint("Insufficient pivot bars data");
                    return;
                }

                var currentPivotTime = _pivotBars.OpenTimes[_pivotBars.Count - 2];

                if (!_isInitialized || currentPivotTime != _lastPivotTime)
                {
                    DebugPrint($"New pivot detected at {currentPivotTime}");
                    RemoveIndicatorObjects();
                    _lastPivotTime = currentPivotTime;
                    DrawAllLevels();
                    _isInitialized = true;
                }
                else
                {
                    UpdateCurrentLevel();
                }
            }
            catch (Exception ex)
            {
                Print($"Error in Calculate: {ex.Message}");
            }
        }

        private void RemoveIndicatorObjects()
        {
            try
            {
                var objectsToRemove = new List<string>();
                
                foreach (var obj in Chart.Objects)
                {
                    if (obj.Name.StartsWith(INDICATOR_PREFIX))
                    {
                        objectsToRemove.Add(obj.Name);
                    }
                }
                
                foreach (var name in objectsToRemove)
                {
                    Chart.RemoveObject(name);
                }
                
                DebugPrint($"Removed {objectsToRemove.Count} objects");
            }
            catch (Exception ex)
            {
                Print($"Error removing objects: {ex.Message}");
            }
        }

        private void DrawAllLevels()
        {
            for (int dayOffset = 0; dayOffset <= PreviousDays; dayOffset++)
            {
                var pivotBarIndex = _pivotBars.Count - 2 - dayOffset;
                
                if (!IsValidBarIndex(pivotBarIndex))
                    continue;

                var high = _pivotBars.HighPrices[pivotBarIndex];
                var low = _pivotBars.LowPrices[pivotBarIndex];
                var close = _pivotBars.ClosePrices[pivotBarIndex];
                var open = _pivotBars.OpenPrices[pivotBarIndex];

                if (!IsValidPriceData(high, low, close, open))
                {
                    DebugPrint($"Invalid price data at index {pivotBarIndex}");
                    continue;
                }

                DateTime startTime = _pivotBars.OpenTimes[pivotBarIndex];
                DateTime endTime = CalculateEndTime(dayOffset, pivotBarIndex);

                double pivot, r1, r2, r3, s1, s2, s3;
                CalculatePivotLevels(high, low, close, open, out pivot, out r1, out r2, out r3, out s1, out s2, out s3);

                var color = dayOffset == 0 ? LineColor : PreviousLineColor;
                var nameSuffix = dayOffset == 0 ? "" : $"_prev{dayOffset}";

                DrawLevel($"Pivot{nameSuffix}", pivot, $"P{nameSuffix}", color, startTime, endTime, pivot);
                DrawLevel($"R1{nameSuffix}", r1, $"R1{nameSuffix}", color, startTime, endTime, r1);
                DrawLevel($"R2{nameSuffix}", r2, $"R2{nameSuffix}", color, startTime, endTime, r2);
                
                if (ShowExtremeLevels)
                {
                    DrawLevel($"R3{nameSuffix}", r3, $"R3{nameSuffix}", color, startTime, endTime, r3);
                    DrawLevel($"S3{nameSuffix}", s3, $"S3{nameSuffix}", color, startTime, endTime, s3);
                }
                
                DrawLevel($"S1{nameSuffix}", s1, $"S1{nameSuffix}", color, startTime, endTime, s1);
                DrawLevel($"S2{nameSuffix}", s2, $"S2{nameSuffix}", color, startTime, endTime, s2);
            }
        }

        private void UpdateCurrentLevel()
        {
            var pivotBarIndex = _pivotBars.Count - 2;
            
            if (!IsValidBarIndex(pivotBarIndex))
                return;

            var high = _pivotBars.HighPrices[pivotBarIndex];
            var low = _pivotBars.LowPrices[pivotBarIndex];
            var close = _pivotBars.ClosePrices[pivotBarIndex];
            var open = _pivotBars.OpenPrices[pivotBarIndex];

            if (!IsValidPriceData(high, low, close, open))
                return;

            DateTime startTime = _pivotBars.OpenTimes[pivotBarIndex];
            DateTime endTime = ExtendRight ? Bars.OpenTimes.LastValue : CalculateEndTime(0, pivotBarIndex);

            double pivot, r1, r2, r3, s1, s2, s3;
            CalculatePivotLevels(high, low, close, open, out pivot, out r1, out r2, out r3, out s1, out s2, out s3);

            var color = LineColor;

            UpdateOrDrawLevel("Pivot", pivot, "P", color, startTime, endTime, pivot);
            UpdateOrDrawLevel("R1", r1, "R1", color, startTime, endTime, r1);
            UpdateOrDrawLevel("R2", r2, "R2", color, startTime, endTime, r2);
            
            if (ShowExtremeLevels)
            {
                UpdateOrDrawLevel("R3", r3, "R3", color, startTime, endTime, r3);
                UpdateOrDrawLevel("S3", s3, "S3", color, startTime, endTime, s3);
            }
            
            UpdateOrDrawLevel("S1", s1, "S1", color, startTime, endTime, s1);
            UpdateOrDrawLevel("S2", s2, "S2", color, startTime, endTime, s2);
        }

        private void UpdateOrDrawLevel(string name, double price, string label, Color color, DateTime startTime, DateTime endTime, double priceValue)
        {
            string fullName = INDICATOR_PREFIX + name;
            var trendLine = Chart.FindObject(fullName) as ChartTrendLine;
            
            if (trendLine != null)
            {
                trendLine.Time2 = endTime;
                trendLine.Y2 = price;
                trendLine.Y1 = price;
                
                var textObj = Chart.FindObject(fullName + "_label") as ChartText;
                if (textObj != null && ShowPriceLabels)
                {
                    DateTime labelTime = CalculateLabelTime(endTime);
                    textObj.Time = labelTime;
                    textObj.Y = price;
                    textObj.Text = $"{label} ({priceValue.ToString($"F{Symbol.Digits}")})";
                }
            }
            else
            {
                DrawLevel(name, price, label, color, startTime, endTime, priceValue);
            }
        }

        private void CalculatePivotLevels(double high, double low, double close, double open, 
            out double pivot, out double r1, out double r2, out double r3, out double s1, out double s2, out double s3)
        {
            double range;
            
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
                    range = high - low;
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
            if (chartTimeFrame == TimeFrame.Minute || chartTimeFrame == TimeFrame.Minute5)
                return TimeFrame.Hour;
            if (chartTimeFrame == TimeFrame.Minute15 || chartTimeFrame == TimeFrame.Minute30)
                return TimeFrame.Hour4;
            if (chartTimeFrame == TimeFrame.Hour)
                return TimeFrame.Daily;
            if (chartTimeFrame == TimeFrame.Hour4)
                return TimeFrame.Weekly;
            if (chartTimeFrame == TimeFrame.Daily)
                return TimeFrame.Weekly;
            if (chartTimeFrame == TimeFrame.Weekly)
                return TimeFrame.Monthly;
            if (chartTimeFrame == TimeFrame.Monthly)
                return TimeFrame.Monthly;

            return TimeFrame.Daily;
        }

        private DateTime CalculateEndTime(int dayOffset, int pivotBarIndex)
        {
            if (dayOffset == 0 && ExtendRight)
                return Bars.OpenTimes.LastValue;
                
            DateTime startTime = _pivotBars.OpenTimes[pivotBarIndex];
            
            if (_pivotBars.TimeFrame == TimeFrame.Monthly)
            {
                return startTime.AddMonths(1);
            }
            
            return startTime + GetTimeFrameDuration(_pivotBars.TimeFrame);
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

        private DateTime CalculateLabelTime(DateTime endTime)
        {
            if (endTime == Bars.OpenTimes.LastValue && LabelOffset > 0)
            {
                int labelIndex = Math.Max(0, Bars.Count - 1 - LabelOffset);
                return Bars.OpenTimes[labelIndex];
            }
            return endTime;
        }

        private void DrawLevel(string name, double price, string label, Color color, DateTime startTime, DateTime endTime, double priceValue)
        {
            string fullName = INDICATOR_PREFIX + name;
            
            Chart.DrawTrendLine(fullName, startTime, price, endTime, price, color, LineThickness, LineStyleType);

            if (ShowPriceLabels)
            {
                DateTime labelTime = CalculateLabelTime(endTime);
                string displayLabel = $"{label} ({priceValue.ToString($"F{Symbol.Digits}")})";
                Chart.DrawText(fullName + "_label", displayLabel, labelTime, price, color);
            }
        }

        private bool IsValidBarIndex(int index)
        {
            return index >= 0 && index < _pivotBars.Count;
        }

        private bool IsValidPriceData(double high, double low, double close, double open)
        {
            return high > 0 && low > 0 && close > 0 && open > 0 && high >= low;
        }

        private void DebugPrint(string message)
        {
            if (ShowDebug)
                Print($"[{DateTime.Now:HH:mm:ss}] MPL: {message}");
        }
    }
}