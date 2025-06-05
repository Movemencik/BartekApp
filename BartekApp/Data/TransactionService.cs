using System.Collections.ObjectModel;
using BartekApp.Enums;
using BartekApp.Models;
using Microsoft.EntityFrameworkCore;

namespace BartekApp.Data;

public class TransactionService
{
    private readonly AppDbContext _context;
    
    public TransactionService(AppDbContext context)
    {
        _context = context;
        InitializeDatabase();
    }
    
    private void InitializeDatabase()
    {
        _context.Database.EnsureCreated();
    }
    
    public async Task AddTransactionAsync(Transaction transaction)
    {
        _context.Transactions.Add(transaction);
        await _context.SaveChangesAsync();
    }
    
    public async Task<ObservableCollection<Transaction>> GetTransactionsAsync(bool? isIncome = null)
    {
        var query = _context.Transactions.AsQueryable();
        
        if (isIncome != null)
            query = query.Where(t => t.IsIncome == isIncome);
            
        var transactions = await query.OrderByDescending(t => t.Date).ToListAsync();
        return new ObservableCollection<Transaction>(transactions);
    }
    
    public async Task<ObservableCollection<Transaction>> GetTransactionsVAsync(CashCategory? category)
    {
        var query = _context.Transactions.AsQueryable();
        if (category != null)
            query = query.Where(t => t.CategoryType == category);
        
        var transactions = await query.OrderByDescending(t => t.Date).ToListAsync();
        return new ObservableCollection<Transaction>(transactions);
    }
    
    public async Task<decimal> GetBalanceAsync()
    {
        var incomes = await _context.Transactions
            .Where(t => t.IsIncome && t.ToBalance == true)
            .SumAsync(t => t.Amount);
            
        var expenses = await _context.Transactions
            .Where(t => !t.IsIncome)
            .SumAsync(t => t.Amount);
            
        return incomes - expenses;
    }

    public async Task<decimal> GetCostsAsync()
    {
        var expenses = await _context.Transactions
            .Where(t => t.CategoryType == CashCategory.Expenses)
            .SumAsync(t => t.Amount);
        return expenses;
    }

    public async Task<decimal> GetCashRegisterAsync()
    {
        var cashRegisterTransactions = await _context.Transactions
            .Where(t => t.CategoryType == CashCategory.CashRegister)
            .SumAsync(t => t.Amount);
        return cashRegisterTransactions;
    }

    public async Task<decimal> GetDeliveryAsync()
    {
        var deliveryTransactions = await _context.Transactions
            .Where(t => t.CategoryType == CashCategory.Delivery)
            .SumAsync(t => t.Amount);
        return deliveryTransactions;
    }

    public async Task<decimal> GetIncomeAsync()
    {
        var incomeTransactions = await _context.Transactions
            .Where(t => t.CategoryType == CashCategory.Selling)
            .SumAsync(t => t.Amount);
        return incomeTransactions;
    }

    public async Task<decimal> GetEmployeeAsync()
    {
        var employeeTransactions = await _context.Transactions
            .Where(t => t.CategoryType == CashCategory.EmployeeSalary)
            .SumAsync(t => t.Amount);
        return employeeTransactions;
    }
}