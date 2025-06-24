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
    
    public async Task UpdateTransactionAsync(Transaction transaction)
    {
        _context.Transactions.Update(transaction);
        await _context.SaveChangesAsync();
    }
    
    public async Task DeleteTransactionAsync(int transaction)
    {
        var transactionToDelete = _context.Transactions.FirstOrDefault(x => x.Id == transaction);
        if (transactionToDelete != null) _context.Transactions.Remove(transactionToDelete);
        await _context.SaveChangesAsync();
    }
    
    public async Task<Transaction?> GetTransactionByIdAsync(int id)
    {
        var transaction = await _context.Transactions.FirstOrDefaultAsync(x => x.Id == id);
        return transaction;
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
    
    public async Task<List<CategorySummary>> GetAllCategorySumsAsync()
    {
        // SQLite-net-pcl tłumaczy to na zapytanie SQL z GROUP BY
        var summaries = await _context.Transactions
            .GroupBy(t => t.CategoryType)
            .Select(g => new CategorySummary
            {
                CategoryType = (CashCategory)g.Key,
                TotalAmount = g.Sum(t => t.Amount)
            })
            .OrderBy(s => s.CategoryType)
            .ToListAsync();

        return summaries;
    }

}