using Core.Entites.SignalR;
using Core.IRepository;
using Core.Specifications;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class DeleteMessageSevice : BackgroundService
    {
        private readonly IServiceProvider serviceProvider;

        public DeleteMessageSevice(IServiceProvider _serviceProvider)
        {
            serviceProvider = _serviceProvider;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var Scope = serviceProvider.CreateScope();
            var UniteOfWork = Scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

            var AfterSemister = DateTime.UtcNow.AddDays(-120);
            var spec = new MessageSpec(AfterSemister);
            var OldMessages = await UniteOfWork.Entity<Message>().GetAllIncludedAsync(spec);

            foreach (var oldMessage in OldMessages)
                UniteOfWork.Entity<Message>().Delete(oldMessage);
            await UniteOfWork.SaveAsync();

            await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
        }
    }
}
