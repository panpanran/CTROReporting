using CTRPReporting.Models;
using CTRPReporting.ViewModels;
using AutoMapper;

namespace CTRPReporting.App_Start
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