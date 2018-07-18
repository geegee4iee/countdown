using AutoMapper;
using Coundown.Spa.ReactJs.Dtos;
using Countdown.Core.Infrastructure;
using Countdown.Core.MachineLearning;
using Countdown.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Coundown.Spa.ReactJs.Infrastructure.Mapping
{
    public class ProcessInfoPredictionProfile : Profile
    {
        public ProcessInfoPredictionProfile()
        {
            CreateMap<ProcessInfoLabeledItemDto, ProcessInfoLabeledItem>().AfterMap(AfterMap);
            CreateMap<ProcessInfo, ProcessInfoLabeledItemDto>()
                .ForMember(dest => dest.Process, opt => opt.MapFrom(src => src.ProcessName))
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.MainWindowTitle))
                .ForMember(dest => dest.Seconds, opt => opt.MapFrom(src => src.TotalAmountOfTime.TotalSeconds));
        }

        private void AfterMap(ProcessInfoLabeledItemDto arg1, ProcessInfoLabeledItem arg2)
        {
            arg2.GenerateId(new Murmur3Hash());
        }
    }
}
