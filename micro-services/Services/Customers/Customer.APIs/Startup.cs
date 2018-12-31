using Common.Messaging.Queues;
using Customers.Domain.Services;
using Customers.Infrastructure;
using Customers.Infrastructure.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Customer.APIs
{
    public class Startup
    {   
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<CustomerContext>(options => options.UseSqlServer(Configuration.GetConnectionString("CustomersConnection")), ServiceLifetime.Transient);
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddTransient<ICustomerService, CustomerService>();
            services.AddTransient<ICustomerRepository, CustomerRepository>();
            services.AddTransient<ICustomerVehicleRepository, CustomerVehiclesRepository>();
            services.AddTransient<IMessagingQueueHandler, MessagingQueueHandler>();
            services.AddTransient<IMessageQueueHelper, MessageQueueHelper>();
            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseCors(builder => builder.WithOrigins("http://localhost:4200"));
            app.UseMvc();

        }
    
}
}
