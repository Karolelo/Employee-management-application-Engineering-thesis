using System.Data.Common;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Repo.Core.Extensions;

public static class DbExceptionExtensions
{
    public static bool IsUniqueViolation(this Exception ex)
        => ex is DbUpdateException dbe && 
           dbe.InnerException is SqlException sql &&
           (sql.Number == 2601 || sql.Number == 2627);
    
    public static bool IsForeignKeyViolation(this Exception ex)
        => ex is DbUpdateException dbe &&
           dbe.InnerException is SqlException sql &&
           sql.Number == 547;
}