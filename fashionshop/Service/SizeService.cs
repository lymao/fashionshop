using Data.Repositories;
using Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public interface ISizeService
    {
        IEnumerable<Size> GetAll();
        Size GetDetail(int id);
    }
    public class SizeService : ISizeService
    {
        ISizeRepository _sizeRepostory;
        public SizeService(ISizeRepository sizeRepository)
        {
            this._sizeRepostory = sizeRepository;
        }
        public IEnumerable<Size> GetAll()
        {
            return _sizeRepostory.GetAll();
        }

        public Size GetDetail(int id)
        {
            return _sizeRepostory.GetSingleById(id);
        }
    }
}
