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
    public interface IContactDetailService
    {
        IEnumerable<ContactDetail> GetAll();
        IEnumerable<ContactDetail> GetAll(string keyword);
        ContactDetail Add(ContactDetail contactDetail);
        ContactDetail GetContactDetailById(int id);
        IEnumerable<ContactDetail> GetContactDetail();
        void Update(ContactDetail contactDetail);
        ContactDetail Delete(int id);
        void Save();
        ContactDetail GetDefaultContact();
    }
    public class ContactDetailService : IContactDetailService
    {
        IContactDetailRepository _contactDetailRepository;
        IUnitOfWork _unitOfWork;
        public ContactDetailService(IContactDetailRepository contactDetailRepository, IUnitOfWork unitOfWork)
        {
            this._contactDetailRepository = contactDetailRepository;
            this._unitOfWork = unitOfWork;
        }

        public ContactDetail Add(ContactDetail contactDetail)
        {
            return _contactDetailRepository.Add(contactDetail);
        }

        public ContactDetail Delete(int id)
        {
            return _contactDetailRepository.Delete(id);
        }

        public IEnumerable<ContactDetail> GetAll()
        {
            return _contactDetailRepository.GetAll();
        }

        public IEnumerable<ContactDetail> GetAll(string keyword)
        {
            if (!string.IsNullOrEmpty(keyword))
            {
                return _contactDetailRepository.GetMulti(x => x.Website.Contains(keyword) || x.Name.Contains(keyword));
            }
            else
            {
                return _contactDetailRepository.GetAll();
            }
        }

        public IEnumerable<ContactDetail> GetContactDetail()
        {
            return _contactDetailRepository.GetMulti(m => m.Status);
        }

        public ContactDetail GetContactDetailById(int id)
        {
            return _contactDetailRepository.GetSingleById(id);
        }

        public ContactDetail GetDefaultContact()
        {
            return _contactDetailRepository.GetSingleByCondition(x => x.Status);
        }

        public void Save()
        {
            _unitOfWork.Commit();
        }

        public void Update(ContactDetail contactDetail)
        {
            _contactDetailRepository.Update(contactDetail);
        }
    }
}
