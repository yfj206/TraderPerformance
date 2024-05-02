using System;
using System.Collections.Generic;
using TraderPerformance.Models;

namespace TraderPerformance.ViewModels;

public class DashboardViewModel
{
    public Portfolio Portfolio { get; set; }
    public List<GroupedTradeViewModel> GroupedTrades { get; set; }
    public bool ShowToday { get; internal set; }
    public decimal GainLossToday { get; internal set; }
    public decimal GainLossAllTime { get; internal set; }
    public object GainLossWeekly { get; internal set; }
    public object GainLossMonthly { get; internal set; }
}
public class GroupedTradeViewModel
{
    public Guid SecurityID { get; set; }
    public string TickerSymbol { get; set; }
    public decimal TotalCost { get; set; }
    public decimal TotalSale { get; set; }
    public decimal GainLoss { get; set; }
    public decimal ProgressWidth { get; set; }
    public bool ShowToday { get; set; }
    public DateTime Date { get; internal set; }
}




//namespace TraderPerformance.ViewModels
//{
//    public class DashboardViewModel
//    {
//        public PortfolioViewModel Portfolio { get; set; }
//        public List<GroupedTradeViewModel> GroupedTrades { get; set; }
//        public decimal GainLossToday { get; set; }
//        public decimal GainLossAllTime { get; set; }
//        public decimal GainLossMonthly { get; set; }
//        public decimal GainLossWeekly { get; set; }
//    }

//    public class PortfolioViewModel
//    {
//        public Guid Id { get; set; }
//        public string Name { get; set; }
//    }

//    public class GroupedTradeViewModel
//    {
//        public Guid SecurityID { get; set; }
//        public string TickerSymbol { get; set; }
//        public decimal TotalCost { get; set; }
//        public decimal TotalSale { get; set; }
//        public decimal GainLoss { get; set; }
//        public decimal ProgressWidth { get; set; }
//        public string Type { get; internal set; }
//        public DateTime Date { get; internal set; }
//        public int Quantity { get; internal set; }
//        public int Price { get; internal set; }
//    }
//}
