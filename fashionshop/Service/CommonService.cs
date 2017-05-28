using Common;
using Data.Infrastructure;
using Data.Repositories;
using Model.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public interface ICommonService
    {
        Footer GetFooter();
        SystemConfig GetSystemConfig(string code);
        Tag GetTagById(string tagId);
    }
    public class CommonService : ICommonService
    {
        ISystemConfigRepository _systemConfigRepository;
        IFooterRepository _footerRepository;
        IUnitOfWork _unitOfWork;
        ISlideRepository _slideRepository;
        ITagRepository _tagRepository;
        public CommonService(ITagRepository tagRepository, ISystemConfigRepository systemConfigRepository, IFooterRepository footerRepository, IUnitOfWork unitOfWork,ISlideRepository slideRepository)
        {
            _footerRepository = footerRepository;
            _unitOfWork = unitOfWork;
            _slideRepository = slideRepository;
            _systemConfigRepository = systemConfigRepository;
            _tagRepository = tagRepository;
        }
        public Footer GetFooter()
        {
            return _footerRepository.GetSingleByCondition(x => x.ID == CommonConstants.DefaultFooterId);
        }

        public SystemConfig GetSystemConfig(string code)
        {
            return _systemConfigRepository.GetSingleByCondition(x => x.Code == code);
        }

        public Tag GetTagById(string tagId)
        {
            return _tagRepository.GetSingleByCondition(x=>x.ID==tagId);
        }
    }
}
