using ECommerce.Domain.Entities;
using ECommerce.Domain.IRepositories;
using ECommerce.Domain.IRepositories.Base;
using ECommerce.InfraStructure.Presistance;
using ECommerce.InfraStructure.Reposatories.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Numerics;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ECommerce.InfraStructure.Reposatories
{
    public class UnitOfWork : IUnitOfWork
    {
        private DBContext context;
        public IRepository<User> User { get; private set; }
     




        public UnitOfWork(DBContext context)
        {
            this.context = context;
            User = new Repository<User>(this.context);
           
        }


        public void Dispose()
        {
            context.Dispose();
        }
        public int Save()
        {
            try
            {
                return context.SaveChanges();
            }
            catch (Exception ex) { return -1; }
        }
        public async Task<int> SaveAsync()
        {
            return await context.SaveChangesAsync();
        }


    }
}
