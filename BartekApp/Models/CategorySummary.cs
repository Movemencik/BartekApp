using BartekApp.Enums;

namespace BartekApp.Models;

public class CategorySummary
{
    public CashCategory CategoryType { get; set; }
    public decimal TotalAmount { get; set; }
}