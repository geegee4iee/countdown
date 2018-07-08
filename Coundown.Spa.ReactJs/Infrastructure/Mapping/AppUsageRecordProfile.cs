using AutoMapper;
using Coundown.Spa.ReactJs.Dtos;
using Countdown.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Coundown.Spa.ReactJs.Infrastructure.Mapping
{
    public class AppUsageRecordProfile : Profile
    {
        public AppUsageRecordProfile()
        {
            CreateMap<KeyValuePair<string, ProcessInfo>, ProcessInfo>().ConvertUsing(src => src.Value);
            CreateMap<ProcessInfo, ProcessInfoDto>()
                .ForMember(dest => dest.Seconds, opt => opt.MapFrom(src => (int)src.TotalAmountOfTime.TotalSeconds))
                .ForMember(dest => dest.WindowTitle, opt => opt.MapFrom(src => src.MainWindowTitle));
        }

        private void BeforeMap(KeyValuePair<string, ProcessInfoDto> arg1, ProcessInfo arg2)
        {

        }
    }
}
