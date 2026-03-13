using ECommerce.Domain.Entities;
using ECommerce.Domain.IRepositories.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Numerics;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ECommerce.Domain.IRepositories
{
    public interface IUnitOfWork : IDisposable
    {

        IRepository<User> User { get; }
        IRepository<Product> Product { get; }
        IRepository<Cart> Cart { get; }
        IRepository<CartItem> CartItem { get; }
        IRepository<Category> Category { get; }
        IRepository<ProductCategory> ProductCategory { get; }
        IRepository<Order> Order { get; }
        IRepository<OrderItem> OrderItem { get; }



        int Save();
        Task<int> SaveAsync();
    }
}
