﻿using Attitude_Loose.Models;
using Attitude_Loose.ViewModels;
using AutoMapper;

namespace Attitude_Loose.App_Start
{
    public class AutoMapperConfig
    {
        public static void Configure()
        {
            Mapper.Initialize(
                cfg =>
                {
                    cfg.CreateMap<UserProfile, UserProfileFormModel>(); 
                    cfg.CreateMap<UserProfileFormModel, UserProfile>();
                    cfg.CreateMap<Topic, TopicFormModel>();
                    cfg.CreateMap<TopicFormModel, Topic>();
                    cfg.CreateMap<Topic, TopicListViewModel>().ForMember(x => x.UserName, opt => opt.MapFrom(source => source.User.UserName));
                });
        }
    }
}