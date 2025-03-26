

namespace Microblogging.Core.Interfaces
{
    public interface IImageStorage
    {
        Task<string> StoreImage(Stream imageStream, string fileName);
    }
}
