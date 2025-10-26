namespace Repo.Core.Infrastructure.Files;

public class FileOperation : IFileOperations
{
    public void SaveFile(string path, byte[] file)
    {
        using (var fs = new FileStream(path, FileMode.Create))
        {
            fs.Write(file, 0, file.Length);
        }
    }

    public FileStream GetFile(string path)
    {
        throw new NotImplementedException();
    }
}