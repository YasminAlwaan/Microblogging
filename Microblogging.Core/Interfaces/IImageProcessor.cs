using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microblogging.Core.Interfaces
{
    public interface IImageProcessor
    {
        Task ProcessImage(string imageUrl, string contentType);
    }
}
