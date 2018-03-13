using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace marketplace.api.Gallery
{
    public interface ImageService
    {
        int CreateImage(byte[] data);
        void DeleteImage(int id);
        byte[] GetImage(int id);
    }
}
