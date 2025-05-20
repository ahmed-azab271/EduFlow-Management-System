using APIs.Helpers;
using Core.Entites.Identity;
using Core.IRepository;
using Core.IServices;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Repository;
using Repository.Data;
using Repository.Identity;
using Services;

namespace APIs.Exctensions
{
    public static class AddServices
    {
        public static IServiceCollection AddServicesExctension(this IServiceCollection Services)
        {
            Services.AddScoped(typeof(IUnitOfWork) , typeof(UnitOfWork));
            Services.AddScoped(typeof(IGetTeacher) , typeof(GetTeacher));
            Services.AddAutoMapper(typeof(MappingProfiles));
            
            Services.AddSignalR();
            Services.AddAuthorization();
            Services.AddSingleton<IUserIdProvider, NameUserIdProvider>();

            Services.AddHostedService<DeleteMessageSevice>();
            return Services;
        }
    }
}
