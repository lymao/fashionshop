using Data.Infrastructure;
using Data.Repositories;
using Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public interface ISlideService
    {
        IEnumerable<Slide> GetAll();
        IEnumerable<Slide> GetAll(string keyword);
        Slide Add(Slide slide);
        Slide GetSlideById(int id);
        IEnumerable<Slide> GetSlides();
        void Update(Slide slide);
        Slide Delete(int id);
        void Save();
    }
    
    public class SlideService:ISlideService
    {
        ISlideRepository _slideRepository;
        IUnitOfWork _unitOfWork;
        public SlideService(ISlideRepository slideRepository,IUnitOfWork unitOfWork)
        {
            this._slideRepository = slideRepository;
            this._unitOfWork = unitOfWork;
        }

        public Slide Add(Slide slide)
        {
           return _slideRepository.Add(slide);
        }

        public Slide Delete(int id)
        {
            return _slideRepository.Delete(id);
        }

        public IEnumerable<Slide> GetAll()
        {
            return _slideRepository.GetAll();
        }

        public IEnumerable<Slide> GetAll(string keyword)
        {
            if (!string.IsNullOrEmpty(keyword))
            {
               return _slideRepository.GetMulti(x => x.Description.Contains(keyword) || x.Name.Contains(keyword));
            }
            else
            {
               return _slideRepository.GetAll();
            }
        }

        public Slide GetSlideById(int id)
        {
           return _slideRepository.GetSingleById(id);
        }

        public IEnumerable<Slide> GetSlides()
        {
            return _slideRepository.GetMulti(m => m.Status);
        }

        public void Save()
        {
            _unitOfWork.Commit();
        }

        public void Update(Slide slide)
        {
            _slideRepository.Update(slide);
        }
    }
}
