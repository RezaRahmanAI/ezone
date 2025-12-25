using IMSWEB.Data;
using IMSWEB.Model;
using IMSWEB.Model.TO;
using IMSWEB.Model.TO.Bkash;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IMSWEB.Service.Infrastructure;

namespace IMSWEB.Service
{
    public class SisterConcernService : ISisterConcernService
    {
        private readonly IBaseRepository<SisterConcern> _sisterConcernRepository;
        private readonly IUnitOfWork _unitOfWork;

        public SisterConcernService(IBaseRepository<SisterConcern> SisterConcernRepository, IUnitOfWork unitOfWork)
        {
            _sisterConcernRepository = SisterConcernRepository;
            _unitOfWork = unitOfWork;
        }

        public void AddSisterConcern(SisterConcern comapany)
        {
            _sisterConcernRepository.Add(comapany);
        }

        public void UpdateSisterConcern(SisterConcern comapany)
        {
            _sisterConcernRepository.Update(comapany);
        }

        public void SaveSisterConcern()
        {
            _unitOfWork.Commit();
            LookupCache.RemoveByPrefix("Lookup:SisterConcern");
        }

        public IEnumerable<SisterConcern> GetAllSisterConcern()
        {
            return _sisterConcernRepository.All.ToList();
        }
        public IEnumerable<SisterConcern> GetAll()
        {
            return _sisterConcernRepository.GetAll();
        }

        public IEnumerable<SisterConcern> GetToAll()
        {
            return _sisterConcernRepository.GetToAll();
        }
        public async Task<IEnumerable<SisterConcern>> GetAllSisterConcernAsync()
        {
            return await _sisterConcernRepository.GetAllSisterConcernAsync();
        }

        public SisterConcern GetSisterConcernById(int id)
        {
            return _sisterConcernRepository.FindBy(x => x.ConcernID == id).First();
        }

        public void DeleteSisterConcern(int id)
        {
            _sisterConcernRepository.Delete(x => x.ConcernID == id);
            LookupCache.RemoveByPrefix("Lookup:SisterConcern");
        }

        public List<SisterConcern> GetFamilyTree(int ConcernID)
        {
            var cacheKey = LookupCache.BuildTenantKey("Lookup:SisterConcern:FamilyTree", ConcernID);
            return LookupCache.GetOrCreate(cacheKey,
                () => _sisterConcernRepository.GetFamilyTree(ConcernID).ToList(),
                TimeSpan.FromMinutes(90));
        }
        public List<IdNameList> GetFamilyTreeDDL(int ConcernID)
        {
            var cacheKey = LookupCache.BuildTenantKey("Lookup:SisterConcern:FamilyTreeDDL", ConcernID);
            return LookupCache.GetOrCreate(cacheKey,
                () => _sisterConcernRepository.GetFamilyTree(ConcernID).Select(d => new IdNameList
                {
                    Id = d.ConcernID,
                    Name = d.Name
                }).ToList(),
                TimeSpan.FromMinutes(90));
        }
        public List<SisterConcern> GetFamilyTreeForNotLoggedInUser(int ConcernID)
        {
            return _sisterConcernRepository.GetFamilyTreeForNotLoggedInUser(ConcernID).ToList();
        }

        public bool IsChildConcern(int concernId)
        {
            SisterConcern concern = _sisterConcernRepository.FindBy(c => c.ConcernID == concernId).FirstOrDefault();

            return concern.ParentID > 0;
        }
        //public List<SisterConcerPayTO> GetAllConcernByConcernId(int concernId)
        //{
        //    List<SisterConcerPayTO> allConcern = _sisterConcernRepository.GetFamilyTree(concernId).ToList().Select(s => new SisterConcerPayTO
        //    {
        //        Id = s.ConcernID,
        //        Name = s.Name,
        //        ServiceCharge = s.ServiceCharge
        //    }).ToList();

        //    return allConcern;
        //}

        public List<SisterConcerPayTO> GetAllConcernByConcernId(int concernId)
        {
            List<SisterConcerPayTO> allConcern = _sisterConcernRepository.GetFamilyTreeForNotLoggedInUser(concernId).ToList().Select(s => new SisterConcerPayTO
            {
                Id = s.ConcernID,
                Name = s.Name,
                ServiceCharge = s.ServiceCharge
            }).ToList();

            return allConcern;
        }

        public string GetConcernNameById(int concernId)
        {
            SisterConcern concern = _sisterConcernRepository.FindBy(d => d.ConcernID == concernId).FirstOrDefault();
            return concern != null ? concern.Name : string.Empty;
        }
        public int GetParentConcernId(int concernId)
        {
            SisterConcern concern = _sisterConcernRepository.FindBy(d => d.ConcernID == concernId).FirstOrDefault();
            return concern.ParentID > 0 ? concern.ParentID : concern.ConcernID;
        }
    }
}
