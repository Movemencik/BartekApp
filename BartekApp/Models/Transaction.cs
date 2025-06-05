using System.ComponentModel.DataAnnotations;
using BartekApp.Enums;

namespace BartekApp.Models;

public class Transaction
{
    [Key]
    public int Id { get; set; }
    [Required(ErrorMessage = "Kwota jest wymagana")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Kwota musi być większa niż 0")]
    public decimal Amount { get; set; }
    [Required]
    public bool IsIncome { get; set; }
    [Required]
    public bool ToBalance { get; set; } = true;
    [Required]
    public DateTime Date { get; set; } = DateTime.Now;
    
    // Kategoria przychodu lub wydatku
    public CashCategory? CategoryType { get; set; }
    /*public IncomeCategory? IncomeType { get; set; }
    public ExpenseCategory? ExpenseType { get; set; }*/
}