using Microsoft.EntityFrameworkCore.Storage;
using Repo.Core.Infrastructure.Database;
using Repo.Core.Infrastructure.UnityOfWork;
using Repo.Server.AuthModule.Interfaces;
using Repo.Server.UserManagmentModule.Interfaces;
namespace Repo.Server.UnityOfWork;

public class UnityOfWork : IUnityOfWork<MyDbContext>
{
    public MyDbContext Context { get; }
    public IUserRepository UserRepository { get; private set; }
    public IRefreshTokenRepository RefreshTokenRepository { get; private set; }
    private IDbContextTransaction _transaction;

    public UnityOfWork(MyDbContext context, IUserRepository userRepository, IRefreshTokenRepository refreshTokenRepository)
    {
        Context = context;
        UserRepository = userRepository;
        RefreshTokenRepository = refreshTokenRepository;
    }

    public void CreateTransaction()
    {
        if (_transaction is not null)
            throw new InvalidOperationException("A transaction has already been started.");
        _transaction = Context.Database.BeginTransaction();
    }

    public void Commit()
    {
        ValidateTransactionExists("A transaction has not been started.");
        try
        {
            _transaction.Commit();
        }
        catch (Exception)
        {
            _transaction.Rollback();
            throw;
        }
        finally
        {
            DisposeTransaction();
        }
    }

    public void Rollback()
    {
        ValidateTransactionExists("A transaction has not been started.");
        try
        {
            _transaction.Rollback();
        }
        finally
        {
            DisposeTransaction();
        }
    }

    public void Save()
    {
        Context.SaveChanges();
    }

    private void ValidateTransactionExists(string errorMessage)
    {
        if (_transaction is null)
            throw new InvalidOperationException(errorMessage);
    }

    private void DisposeTransaction()
    {
        _transaction?.Dispose();
        _transaction = null;
    }
}