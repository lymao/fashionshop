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
    }
    public class CommonService : ICommonService
    {
        ISystemConfigRepository _systemConfigRepository;
        IFooterRepository _footerRepository;
        IUnitOfWork _unitOfWork;
        ISlideRepository _slideRepository;
        public CommonService(ISystemConfigRepository systemConfigRepository, IFooterRepository footerRepository, IUnitOfWork unitOfWork,ISlideRepository slideRepository)
        {
            _footerRepository = footerRepository;
            _unitOfWork = unitOfWork;
            _slideRepository = slideRepository;
            _systemConfigRepository = systemConfigRepository;
        }
        public Footer GetFooter()
        {
            return _footerRepository.GetSingleByCondition(x => x.ID == CommonConstants.DefaultFooterId);
        }

        public SystemConfig GetSystemConfig(string code)
        {
            return _systemConfigRepository.GetSingleByCondition(x => x.Code == code);
        }
    }
}
