using AutoMapper;
using Coundown.Spa.ReactJs.Dtos;
using Coundown.Spa.ReactJs.Infrastructure;
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
        private readonly IAppUsageRecordRepositoryAsync _repository;

        public AppUsageRecordController(IAppUsageRecordRepositoryAsync repository)
        {
            this._repository = repository;
        }

        [Route("today")]
        public async Task<IActionResult> GetForToday()
        {
            var id = AppUsageRecord.GetGeneratedId(DateTime.Now);
            var appUsage = await _repository.GetAsync(id);
            var processInfoCollection = Mapper.Map<IEnumerable<ProcessInfo>, IEnumerable<ProcessInfoDto>>(appUsage.ActiveApps.Select(c => c.Value));
            return Ok(processInfoCollection);
        }

        [Route("lastweek")]
        public async Task<IActionResult> GetForLastWeek()
        {
            var appUsage = await _repository.FindAsync(new LastWeekRecordSpecification(DateTime.Now));
            var processInfoCollection = Mapper.Map<IEnumerable<AppUsageRecord>, IEnumerable<AppUsageRecordDto>>(appUsage);

            return Ok(processInfoCollection);
        }

        [HttpGet("{date}")]
        public async Task<IActionResult> Get(DateTime date)
        {
            var appUsage = await _repository.GetAsync(AppUsageRecord.GetGeneratedId(date));
            var processInfoCollection = Mapper.Map<IEnumerable<ProcessInfo>, IEnumerable<ProcessInfoDto>>(appUsage.ActiveApps.Select(c => c.Value));

            return Ok(processInfoCollection);
        }
    }
}
