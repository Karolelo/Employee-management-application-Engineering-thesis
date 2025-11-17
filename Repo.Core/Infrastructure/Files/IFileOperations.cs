using System.Runtime.InteropServices;
using Microsoft.EntityFrameworkCore;

namespace Repo.Core.Infrastructure.Files;

public interface IFileOperations
{
    void SaveFile(string path, byte[] file);

    FileStream GetFile(string path);
}