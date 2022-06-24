using Microsoft.EntityFrameworkCore;
using MusicPlayerData.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicPlayerClient.ViewModels
{
    public class HomeViewModel : ViewModelBase
    {
        public string? ConnectionString { get; }
        public HomeViewModel(IDbContextFactory<DataContext> dbContextFactory)
        {
            using (var dbContext = dbContextFactory.CreateDbContext())
            {
                ConnectionString = dbContext.Database.GetConnectionString();
            }
        }
    }
}
