using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EMPEntityFramework.Edmx;
using EMP.Core.ZipCodeService;
using EMP.Core.DTO;
using EMP.Core.Utilities;

namespace EMP.Core.ZipCodeService
{
    public class ZipCodeService : IZipCodeService
    {
        public DatabaseEntities db = new DatabaseEntities();


        public ZipCodeModel GetZipCodeByCode(string value)
        {
            var data = db.ZipCodeMasters.Where(o => o.ZipCode == value).Select(o => new ZipCodeModel()
            {
                value = o.ZipCode,
                Id = o.ZIPID,
                ZipCode = o.ZipCode,
                City = o.City,
                State = o.State,
                Country = o.Country,
                Latitude = o.Latitude,
                Longitude = o.Longitude,
                TimeZone = o.TimeZone,
                dst = o.dst
            }).FirstOrDefault();

            return data;
        }



        public IQueryable<ZipCodeModel> GetCountry()
        {
            db = new DatabaseEntities();
            var data = db.ZipCodeMasters.Select(o => new ZipCodeModel()
            {
                //Id = o.ZIPID,
                value = o.Country,
            }).DefaultIfEmpty();
            return data;
        }

        public IQueryable<ZipCodeModel> GetState()
        {
            db = new DatabaseEntities();
            var data = db.ZipCodeMasters.Select(o => new ZipCodeModel()
            {
                //Id = o.ZIPID,
                value = o.State,
            }).DefaultIfEmpty();
            return data;
        }

        public IQueryable<ZipCodeModel> GetCity()
        {
            db = new DatabaseEntities();
            var data = db.ZipCodeMasters.Select(o => new ZipCodeModel()
            {
                // Id = o.ZIPID,
                value = o.City,
            }).DefaultIfEmpty();
            return data;
        }

        public IQueryable<ZipCodeModel> GetZipCode()
        {
            db = new DatabaseEntities();
            var data = db.ZipCodeMasters.Select(o => new ZipCodeModel()
            {
                //  Id = o.ZIPID,
                value = o.ZipCode,
            }).DefaultIfEmpty();
            return data;
        }




    }
}
