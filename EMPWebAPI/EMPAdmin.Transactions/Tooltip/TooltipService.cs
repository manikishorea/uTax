using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EMPEntityFramework.Edmx;
using EMPAdmin.Transactions.Tooltip.DTO;
using EMP.Core.Utilities;

namespace EMPAdmin.Transactions.Tooltip
{
    public class TooltipService : ITooltipService
    {
        public DatabaseEntities db = new DatabaseEntities();

        public IQueryable<TooltipDTO> GetAll()
        {
            db = new DatabaseEntities();
            var data = db.TooltipMasters.Select(o => new TooltipDTO
            {
                Id = o.Id,
                Field = o.Field,
                ToolTipText = o.ToolTipText,
                Description = o.Description,
                StatusCode = o.StatusCode,
                SitemapTitle = o.SitemapMaster.Name,
                IsUIVisible=o.IsUIVisible
            }).DefaultIfEmpty();
            return data;
        }

        public async Task<TooltipDTO> GetById(Guid Id)
        {
            var data = await db.TooltipMasters.Where(o => o.Id == Id).Select(o => new TooltipDTO
            {
                Id = o.Id,
                Field = o.Field,
                ToolTipText = o.ToolTipText,
                Description = o.Description,
                StatusCode = o.StatusCode,
                SitemapId = o.SitemapId??Guid.Empty,
                SitemapTitle = o.SitemapMaster.Name,
                IsUIVisible = o.IsUIVisible
            }).FirstOrDefaultAsync();

            return data;
        }

        public async Task<Guid> Save(TooltipDTO _Dto, Guid Id, int EntityState)
        {
            TooltipMaster _TooltipMaster = new TooltipMaster();

            if (_Dto != null)
            {
                _TooltipMaster.Id = Id;
                _TooltipMaster.Field = _Dto.Field;
                _TooltipMaster.ToolTipText = _Dto.ToolTipText;
                _TooltipMaster.Description = _Dto.Description;
                _TooltipMaster.StatusCode = EMPConstants.Active;
                _TooltipMaster.SitemapId = _Dto.SitemapId;
                _TooltipMaster.IsUIVisible = _Dto.IsUIVisible;
            }

            if (EntityState == (int)System.Data.Entity.EntityState.Modified)
            {
                _TooltipMaster.CreatedBy = _Dto.UserId;
                _TooltipMaster.CreatedDate = DateTime.Now;
                _TooltipMaster.LastUpdatedBy = _Dto.UserId;
                _TooltipMaster.LastUpdatedDate = DateTime.Now;
                db.Entry(_TooltipMaster).State = System.Data.Entity.EntityState.Modified;
            }
            else
            {
                _TooltipMaster.LastUpdatedBy = _Dto.UserId;
                _TooltipMaster.LastUpdatedDate = DateTime.Now;
                db.TooltipMasters.Add(_TooltipMaster);
            }

            try
            {
                await db.SaveChangesAsync();
                return _TooltipMaster.Id;
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!IsExists(_TooltipMaster.Id))
                {
                    return Guid.Empty;
                }
                else
                {
                    throw;
                }
            }
        }

        public async Task<Guid> SaveStatus(TooltipDTO _Dto, Guid Id, int EntityState)
        {
            TooltipMaster Tooltip = new TooltipMaster();
            Tooltip = await db.TooltipMasters.Where(o => o.Id == Id).FirstOrDefaultAsync();
            if (Tooltip.StatusCode == EMPConstants.InActive)
            {
                Tooltip.StatusCode = EMPConstants.Active;
            }
            else if (Tooltip.StatusCode == EMPConstants.Active)
            {
                Tooltip.StatusCode = EMPConstants.InActive;
            }

            if (EntityState == (int)System.Data.Entity.EntityState.Modified)
            {
                Tooltip.LastUpdatedDate = DateTime.Now;
                Tooltip.LastUpdatedBy = _Dto.UserId;
                db.Entry(Tooltip).State = System.Data.Entity.EntityState.Modified;
            }

            try
            {
                await db.SaveChangesAsync();
                db.Dispose();
                return Tooltip.Id;
            }

            catch (DbUpdateConcurrencyException)
            {
                if (!IsExists(Tooltip.Id))
                {
                    return Guid.Empty;
                }
                else
                {
                    throw;
                }
            }
        }

        private bool IsExists(Guid id)
        {
            return db.TooltipMasters.Count(e => e.Id == id) > 0;
        }

        public async Task<bool> Delete(Guid Id)
        {
            try
            {
                var deletedata = db.TooltipMasters.Where(o => o.Id == Id).FirstOrDefault();
                if (deletedata != null)
                {
                    db.TooltipMasters.Remove(deletedata);
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
