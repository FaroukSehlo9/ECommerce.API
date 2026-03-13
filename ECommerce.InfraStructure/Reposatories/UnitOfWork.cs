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
        public IRepository<Product> Product { get; private set; }
        public IRepository<Cart> Cart { get; private set; }
        public IRepository<CartItem> CartItem { get; private set; }
        public IRepository<Category> Category { get; private set; }
        public IRepository<ProductCategory> ProductCategory { get; private set; }
        public IRepository<Order> Order { get; private set; }
        public IRepository<OrderItem> OrderItem { get; private set; }
     




        public UnitOfWork(DBContext context)
        {
            this.context = context;
            User = new Repository<User>(this.context);
            Product = new Repository<Product>(this.context);
            Cart = new Repository<Cart>(this.context);
            CartItem = new Repository<CartItem>(this.context);
            Category = new Repository<Category>(this.context);
            ProductCategory = new Repository<ProductCategory>(this.context);
            Order = new Repository<Order>(this.context);
            OrderItem = new Repository<OrderItem>(this.context);
           
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
