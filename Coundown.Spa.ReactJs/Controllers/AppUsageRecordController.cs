using AutoMapper;
using Coundown.Spa.ReactJs.Dtos;
using Countdown.Core.Infrastructure;
using Countdown.Core.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Coundown.Spa.ReactJs.Controllers
{
    [Route("api/[controller]")]
    public class AppUsageRecordController : Controller
    {
        private readonly IRepository<AppUsageRecord> _repository;

        public AppUsageRecordController(IRepository<AppUsageRecord> repository)
        {
            this._repository = repository;
        }

        [Route("today")]
        public async Task<IActionResult> GetForToday()
        {
            try
            {
                var id = AppUsageRecord.GetGeneratedId(DateTime.Now);
                var appUsage = await _repository.GetAsync(id);
                var processInfoCollection = Mapper.Map<IEnumerable<ProcessInfo>, IEnumerable<ProcessInfoDto>>(appUsage.ActiveApps.Select(c=> c.Value));
                return Ok(processInfoCollection);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
            }
        }
    }
}
