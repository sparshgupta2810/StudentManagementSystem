using StudentManagementSystemApp.Enums;
using StudentManagementSystemApp.Interfaces;
using StudentManagementSystemApp.Models;

namespace StudentManagementSystemApp.Services;

public class BookService
{
    private readonly IBookRepository _repository;
    private readonly IAuditService _auditService;

    public BookService(
        IBookRepository repository,
        IAuditService auditService)
    {
        _repository = repository;
        _auditService = auditService;
    }

    #region Get

    public async Task<IEnumerable<Book>> GetAllBooksAsync()
    {
        return await _repository.GetActiveAsync();
    }

    public async Task<IEnumerable<Book>> GetBookHistoryAsync()
    {
        return await _repository.GetInactiveAsync();
    }

    #endregion

    #region Pagination

    public async Task<PagedResult<Book>> GetPagedBooksAsync(
        int page,
        int pageSize,
        string? search = null)
    {
        return await _repository.GetPagedAsync(
            page,
            pageSize,
            search);
    }

    #endregion

    #region History Pagination

    public async Task<PagedResult<Book>> GetPagedBookHistoryAsync(
        int page,
        int pageSize,
        string? search = null)
    {
        return await _repository.GetHistoryPagedAsync(
            page,
            pageSize,
            search);
    }

    #endregion

    #region Get By Id

    public async Task<Book?> GetBookByIdAsync(int id)
    {
        return await _repository.GetByIdAsync(id);
    }

    #endregion

    #region Add

    public async Task<int> AddBookAsync(Book book)
    {
        if (await _repository.BookCodeExistsAsync(book.BookCode))
            throw new Exception("Book Code already exists.");

        if (await _repository.BookNameExistsAsync(book.BookName))
            throw new Exception("Book Name already exists.");

        // Automatically initialize available copies
        book.AvailableCopies = book.TotalCopies;

        int id = await _repository.AddAsync(book);

        if (id > 0)
        {
            book.Id = id;

            await _auditService.LogAsync(
                module: AuditModule.Book,
                action: AuditAction.Add,
                entityId: id,
                entityName: book.BookName,
                details: $"Book '{book.BookName}' created.",
                oldValue: null,
                newValue: book);
        }

        return id;
    }

    #endregion

    #region Update

    public async Task<bool> UpdateBookAsync(Book book)
    {
        if (book.TotalCopies < book.AvailableCopies)
            throw new Exception("Total Copies cannot be less than Available Copies.");

        if (await _repository.BookCodeExistsAsync(book.BookCode, book.Id))
            throw new Exception("Book Code already exists.");

        if (await _repository.BookNameExistsAsync(book.BookName, book.Id))
            throw new Exception("Book Name already exists.");

        var oldBook =
            await _repository.GetByIdAsync(book.Id);

        bool result =
            await _repository.UpdateAsync(book);

        if (result)
        {
            await _auditService.LogAsync(
                module: AuditModule.Book,
                action: AuditAction.Update,
                entityId: book.Id,
                entityName: book.BookName,
                details: $"Book '{book.BookName}' updated.",
                oldValue: oldBook,
                newValue: book);
        }

        return result;
    }

    #endregion

    #region Inactivate

    public async Task<bool> InactivateBookAsync(int id)
    {
        var book =
            await _repository.GetByIdAsync(id);

        bool result =
            await _repository.InactivateAsync(id);

        if (result && book != null)
        {
            await _auditService.LogAsync(
                module: AuditModule.Book,
                action: AuditAction.Delete,
                entityId: id,
                entityName: book.BookName,
                details: $"Book '{book.BookName}' removed.",
                oldValue: book,
                newValue: null);
        }

        return result;
    }

    #endregion

    #region Restore

    public async Task<bool> RestoreBookAsync(int id)
    {
        var book =
            await _repository.GetByIdAsync(id);

        bool result =
            await _repository.RestoreAsync(id);

        if (result && book != null)
        {
            await _auditService.LogAsync(
                module: AuditModule.Book,
                action: AuditAction.Restore,
                entityId: id,
                entityName: book.BookName,
                details: $"Book '{book.BookName}' restored.",
                oldValue: null,
                newValue: book);
        }

        return result;
    }

    #endregion
}