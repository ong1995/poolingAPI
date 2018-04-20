using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UserPoolingApi.Enums;
using UserPoolingApi.Helper;
using UserPoolingApi.Models;
using UserPoolingApi.Models.Enums;
using UserPoolingApi.ViewModels;

namespace UserPoolingApi.ConfigServices
{
    public class AutoMapperConfig : Profile
    {
        public AutoMapperConfig()
        {
            CreateMap<User, UserViewModel>().ReverseMap()
                .ForMember(a => a.Filename, b => b.ResolveUsing(a => a.Filename.FileName))
                .ForMember(a => a.Status, b => b.ResolveUsing(a => StatusEnum.New));
            CreateMap<User, DisplayUserViewModel>()
                .ForMember(a => a.Status, b => b.ResolveUsing(a => a.Status.ToString().ToSentenceCase()));

            CreateMap<Skill, SkillViewModel>().ReverseMap();
            CreateMap<UserSkillViewModel, UserSkills>().ReverseMap();
            CreateMap<DisplayUserSkillViewModel, UserSkills>().ReverseMap()
                .ForMember(a => a.SkillName, b => b.ResolveUsing(a => a.Skill.SkillName));

            CreateMap<User, PageResultViewModel>().ReverseMap();
        }
    }

    public static class MapperConfigService
    {
        public static IServiceCollection RegisterMapper(this IServiceCollection services)
        {
            services.AddAutoMapper();

            return services;
        }
    }
}