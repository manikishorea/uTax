using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EMPEntityFramework.Edmx;
using EMPAdmin.Transactions.APIIntegrationMaster.DTO;
using EMP.Core.Utilities;

namespace EMPAdmin.Transactions.APIIntegrationMaster
{
    public class APIIntegrationService : IAPIIntegrationService
    {
        public DatabaseEntities db = new DatabaseEntities();

        public IQueryable<APIIntegrationDTO> GetAll()
        {
            db = new DatabaseEntities();
            var data = db.APIIntegrations.Select(o => new APIIntegrationDTO
            {
                Id = o.Id,
                Name = o.Name,
                URL = o.URL,
                UserName = o.UserName,
                Password = o.Password, //o.Password,
                StatusCode = o.StatusCode
            }).DefaultIfEmpty();

            List<APIIntegrationDTO> APIIntegrationDTOs = new List<APIIntegrationDTO>();

            foreach (APIIntegrationDTO API in data)
            {
                APIIntegrationDTO APIInteg = new APIIntegrationDTO();
                APIInteg.Id = API.Id;
                APIInteg.Id = API.Id;
                APIInteg.Name = API.Name;
                APIInteg.URL = API.URL;
                APIInteg.UserName = API.UserName;
                APIInteg.Password = PasswordManager.DecryptText(API.Password); //o.Password,
                APIInteg.StatusCode = API.StatusCode;
                APIIntegrationDTOs.Add(APIInteg);
            }

            return APIIntegrationDTOs.AsQueryable();
        }

        public async Task<APIIntegrationDTO> GetById(Guid Id)
        {
            var data = await db.APIIntegrations.Where(o => o.Id == Id).Select(o => new APIIntegrationDTO
            {
                Id = o.Id,
                Name = o.Name,
                URL = o.URL,
                UserName = o.UserName,
                Password = PasswordManager.DecryptText(o.Password), //o.Password,
                StatusCode = o.StatusCode
            }).FirstOrDefaultAsync();

            return data;
        }

        public async Task<Guid> Save(APIIntegrationDTO _Dto, Guid Id, int EntityState)
        {
            APIIntegration _APIIntegration = new APIIntegration();

            if (_Dto != null)
            {
                _APIIntegration.Id = Id;
                _APIIntegration.Name = _Dto.Name;
                _APIIntegration.URL = _Dto.URL;
                _APIIntegration.UserName = _Dto.UserName;
                _APIIntegration.Password = PasswordManager.DecryptText(_Dto.Password);
                _APIIntegration.StatusCode = EMPConstants.Active;
            }

            if (_Dto.Id != null)
            {
                _APIIntegration.CreatedBy = _Dto.UserId;
                _APIIntegration.CreatedDate = DateTime.Now;
                _APIIntegration.LastUpdatedBy = _Dto.UserId;
                _APIIntegration.LastUpdatedDate = DateTime.Now;
            }
            else
            {
                _APIIntegration.LastUpdatedBy = _Dto.UserId;
                _APIIntegration.LastUpdatedDate = DateTime.Now;
            }

            if (EntityState == (int)System.Data.Entity.EntityState.Modified)
            {
                db.Entry(_APIIntegration).State = System.Data.Entity.EntityState.Modified;
            }
            else
            {
                db.APIIntegrations.Add(_APIIntegration);
            }

            try
            {
                await db.SaveChangesAsync();
                return _APIIntegration.Id;
            }

            catch (DbUpdateConcurrencyException)
            {
                if (!IsExists(_APIIntegration.Id))
                {
                    return _APIIntegration.Id;
                }
                else
                {
                    throw;
                }
            }
        }

        private bool IsExists(Guid id)
        {
            return db.APIIntegrations.Count(e => e.Id == id) > 0;
        }

        public async Task<bool> Delete(Guid Id)
        {
            try
            {
                var deletedata = db.APIIntegrations.Where(o => o.Id == Id).FirstOrDefault();
                if (deletedata != null)
                {
                    db.APIIntegrations.Remove(deletedata);
                    await db.SaveChangesAsync();
                    db.Dispose();
                }
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                return false;
            }
        }

    }
}
