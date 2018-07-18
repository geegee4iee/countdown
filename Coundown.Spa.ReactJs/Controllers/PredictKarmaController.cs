using AutoMapper;
using Coundown.Spa.ReactJs.Dtos;
using Coundown.Spa.ReactJs.Infrastructure;
using Coundown.Spa.ReactJs.Services;
using Coundown.Spa.ReactJs.Utils;
using Countdown.Core.MachineLearning;
using Countdown.Core.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Coundown.Spa.ReactJs.Controllers
{
    [Route("api/[controller]/[action]")]
    public class PredictKarmaController : Controller
    {
        private readonly IProcessInfoLabeledItemRepositoryAsync _labeledItemRepository;
        private readonly IPredictKarmaService _predictKarmaService;
        private readonly IAppUsageRecordRepositoryAsync _appUsageRecordRepository;

        public PredictKarmaController(IProcessInfoLabeledItemRepositoryAsync repository, IPredictKarmaService predictKarmaService, IAppUsageRecordRepositoryAsync appUsageRecordRepository)
        {
            _labeledItemRepository = repository;
            _predictKarmaService = predictKarmaService;
            _appUsageRecordRepository = appUsageRecordRepository;
        }

        [HttpPost]
        public async Task<IActionResult> AddLabeledItem([FromBody]ProcessInfoLabeledItemDto trainingItem)
        {
            var entity = Mapper.Map<ProcessInfoLabeledItem>(trainingItem);

            await _labeledItemRepository.Add(entity);

            return Ok();
        }

        [HttpPost]
        public IActionResult TrainModel()
        {
            _predictKarmaService.TrainModel();

            return Ok();
        }

        /// <summary>
        /// This task is intensive, might take a large amount of time to complete!!!!!!.
        /// </summary>
        /// <param name="processInfoDto"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Predict(ProcessInfoDto processInfoDto)
        {
            var entity = Mapper.Map<ProcessInfo>(processInfoDto);
            var result = _predictKarmaService.Predict(entity);

            return Ok(result.ToString());
        }

        [HttpGet("{date}")]
        public async Task<IActionResult> GetPredictedForDate(DateTime date)
        {
            var appUsage = await _appUsageRecordRepository.GetAsync(AppUsageRecord.GetGeneratedId(date));
            var processInfoCollection = appUsage.ActiveApps.Select(pair => pair.Value).ToArray();
            var dtos = Mapper.Map<ProcessInfoLabeledItemDto[]>(processInfoCollection);

            var results =_predictKarmaService.Predict(processInfoCollection);

            dtos.Merge(results, (dto, category) =>
            {
                dto.Category = category;
            });

            return Ok(dtos);
        }

    }
}
