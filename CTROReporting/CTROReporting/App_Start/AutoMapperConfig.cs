﻿using CTROLibrary.Model;
using CTROReporting.ViewModels;
using AutoMapper;

namespace CTROLibrary.DBbase
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
                    cfg.CreateMap<Record, RecordListViewModel>().ForMember(x => x.UserName, opt => opt.MapFrom(source => source.User.UserName))
                    .ForMember(x => x.ReportName, opt => opt.MapFrom(source => source.Report.ReportName));
                    cfg.CreateMap<Schedule, ScheduleListViewModel>().ForMember(x => x.UserName, opt => opt.MapFrom(source => source.User.UserName))
                    .ForMember(x => x.ReportName, opt => opt.MapFrom(source => source.Report.ReportName));
                    cfg.CreateMap<ApplicationUser, UserManagementViewModel>().ForMember(x => x.DepartmentName, opt => opt.MapFrom(source => source.Department.DepartmentName));
                });
        }
    }
}