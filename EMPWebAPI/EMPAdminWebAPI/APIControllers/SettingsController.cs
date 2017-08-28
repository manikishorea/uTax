using EMPAdmin.Transactions.Settings;
using EMPAdmin.Transactions.Settings.DTO;
using EMPAdminWebAPI.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;

namespace EMPAdminWebAPI.APIControllers
{
    [TokenAuthorization]
    public class SettingsController : ApiController
    {
        SettingsService _settingsservice = new SettingsService();

        // GET: api/Settings
        [ResponseType(typeof(SettingsDTO))]
        public IHttpActionResult GetSettings()
        {
            var sets = _settingsservice.GetSettings();
            if (sets == null)
            {
                return NotFound();
            }
            return Ok(sets);
        }

        // POST: api/SaveSettings
        [ResponseType(typeof(int))]
        public IHttpActionResult PostSettings(SettingsDTO _Dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            int result = _settingsservice.Save(_Dto);
            
            return Ok(result);
        }
    }
}
