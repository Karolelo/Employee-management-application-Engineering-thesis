using Microsoft.EntityFrameworkCore.Storage;
using Repo.Core.Infrastructure.Database;
using Repo.Core.Infrastructure.UnityOfWork;
using Repo.Server.AuthModule.Interfaces;
using Repo.Server.UserManagmentModule.Interfaces;

namespace Repo.Server.UnityOfWork;
//We create this class just
//for specific use, because of our bad decision we didn't implement a completed pattern,
//Maybe in the long term we could implement this 
public class UnityOfWork : IUnityOfWork<MyDbContext>
{
    public MyDbContext Context { get; }
    public IUserRepository UserRepository { get; private set; }
    public IRefreshTokenRepository RefreshTokenRepository { get; set; }
    private IDbContextTransaction _transaction;

    public UnityOfWork(MyDbContext context, IUserRepository userRepository,IRefreshTokenRepository refreshTokenRepository)
    {
        Context = context;
        UserRepository = userRepository;
        RefreshTokenRepository = refreshTokenRepository;
    }

    public void CreateTransaction()
    {
        _transaction = Context.Database.BeginTransaction();
    }

    public void Commit()
    {
        _transaction.Commit();
    }

    public void Rollback()
    {
        _transaction.Rollback();
        _transaction.Dispose();
    }

    public void Save()
    {
        try
        {
            Context.SaveChanges();
        }
        catch (Exception e)
        {
            throw;
        }
    }
}