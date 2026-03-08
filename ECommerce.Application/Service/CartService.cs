using AutoMapper;
using ECommerce.Application.Common.SharedResources;
using ECommerce.Application.Communications;
using ECommerce.Application.DTOS.CartDTO;
using ECommerce.Application.IService;
using ECommerce.Domain.Entities;
using ECommerce.Domain.IRepositories;
using Microsoft.Extensions.Localization;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Service
{
    public class CartService : ICartService
    {
        private readonly IMapper _Imapper;
        private readonly IStringLocalizer<GeneralMessages> _localization;
        private readonly IUnitOfWork _unit;
        public CartService(IMapper imapper, IStringLocalizer<GeneralMessages> localization, IUnitOfWork unitOfWork)
        {
            _Imapper = imapper;
            _localization = localization;
            _unit = unitOfWork;
        }

        public async Task<GeneralResponse<List<CartDto>>> GetAll()
        {

            var carts = _unit.Cart.All()
        .Include(x => x.User)
        .Include(x => x.Items)
            .ThenInclude(i => i.Product)
        .ToList();

            var result = carts.Select(c => new CartDto
            {
                Id = c.Id,
                UserId = c.UserId,
                UserName = c.User.UserName,
                CartTotalPrice = c.Items
                    .Where(i => !i.IsDeleted) // filter out deleted items
                    .Sum(i => i.Product.Price * i.Quantity),

                Items = c.Items
                    .Where(i => !i.IsDeleted) // filter out deleted items
                    .Select(i => new CartItemDto
                    {
                        Id = i.Id,
                        ProductId = i.ProductId,
                        ProductName = i.Product.Name,
                        Quantity = i.Quantity,
                        Price = i.Product.Price,
                        TotalPrice = i.Quantity * i.Product.Price
                    })
                    .ToList()
            }).ToList();



            return new GeneralResponse<List<CartDto>>(result, _localization["Succes"].Value, result.Count());
        }

        public async Task<GeneralResponse<CartDto>> GetByIdAsync(Guid Id)
        {
            var cart = await _unit.Cart.GetByIdAsync(Id);
            if (cart == null)
                return new GeneralResponse<CartDto>(_localization["NotFound"].Value, System.Net.HttpStatusCode.NotFound);

            var result = _unit.Cart.All().Include(x => x.User).Include(x => x.Items).ThenInclude(i => i.Product).Where(d => d.Id == Id).ToList().Select(x => new CartDto
            {
                Id = x.Id,
                UserId = x.UserId,
                UserName = x.User.UserName,
                CartTotalPrice = x.Items
                    .Where(i => !i.IsDeleted) // filter out deleted items
                    .Sum(i => i.Product.Price * i.Quantity),

                Items = x.Items
                    .Where(i => !i.IsDeleted) // filter out deleted items
                    .Select(i => new CartItemDto
                    {
                        Id = i.Id,
                        ProductId = i.ProductId,
                        ProductName = i.Product.Name,
                        Quantity = i.Quantity,
                        Price = i.Product.Price,
                        TotalPrice = i.Quantity * i.Product.Price
                    }).ToList()

            }).FirstOrDefault();
            return new GeneralResponse<CartDto>(result, _localization["Succes"].Value);
        }

        public async Task<GeneralResponse<Guid>> Add(CartInput Input, Guid UserId)
        {
            try
            {
                // 1. Check if user already has a cart
                var existingCart = _unit.Cart.All()
                    .FirstOrDefault(c => c.UserId == UserId);

                if (existingCart != null)
                {
                    // 2. Update existing cart
                    foreach (var item in Input.Items)
                    {
                        var existingItem = existingCart.Items
                            .FirstOrDefault(i => i.ProductId == item.ProductId);

                        if (existingItem != null)
                        {
                            existingItem.Quantity += item.Quantity; // Update quantity
                        }
                        else
                        {
                            existingCart.Items.Add(new CartItem
                            {
                                Id = Guid.NewGuid(),
                                ProductId = item.ProductId,
                                Quantity = item.Quantity
                            });
                        }
                    }

                    existingCart.UpdatedDate = DateTime.Now;

                    await _unit.Cart.UpdateAsync(existingCart);
                    var result = _unit.Save();

                    return result > 0
                        ? new GeneralResponse<Guid>(existingCart.Id, _localization["Succesfully"].Value)
                        : new GeneralResponse<Guid>(_localization["ErrorInSave"].Value, System.Net.HttpStatusCode.BadRequest);
                }
                else
                {
                    // 3. Create new cart
                    var cart = _Imapper.Map<CartInput, Cart>(Input);
                    cart.UserId = UserId;
                    cart.CreationDate = DateTime.Now;

                    await _unit.Cart.AddAsync(cart);
                    var result = _unit.Save();

                    return result > 0
                        ? new GeneralResponse<Guid>(cart.Id, _localization["Succesfully"].Value)
                        : new GeneralResponse<Guid>(_localization["ErrorInSave"].Value, System.Net.HttpStatusCode.BadRequest);
                }
            }
            catch (Exception ex)
            {

                return new GeneralResponse<Guid>(ex.Message + "-" + ex.InnerException?.Message, System.Net.HttpStatusCode.BadRequest);

            }
        }

        public async Task<GeneralResponse<Guid>> Update(CartUpdateInput Input, Guid UserId)
        {
            try
            {
                // 1. جلب الكارت مع العناصر والمنتجات
                var cart = _unit.Cart.All()
                    .Include(c => c.User)
                    .Include(c => c.Items)
                        .ThenInclude(i => i.Product)
                    .FirstOrDefault(c => c.UserId == UserId);

                if (cart == null)
                {
                    return new GeneralResponse<Guid>(_localization["CartNotFound"].Value, System.Net.HttpStatusCode.NotFound);
                }

                bool saved = false;
                while (!saved)
                {
                    try
                    {
                        // 2. تحديث أو إضافة العناصر
                        foreach (var item in Input.Items)
                        {
                            var existingItem = cart.Items
                                .FirstOrDefault(i => i.ProductId == item.ProductId && !i.IsDeleted);

                            if (existingItem != null)
                            {
                                if (item.Quantity <= 0)
                                {
                                    // إزالة العنصر إذا الكمية <= 0
                                    cart.Items.Remove(existingItem);
                                }
                                else
                                {
                                    // تحديث الكمية
                                    existingItem.Quantity = item.Quantity;
                                }
                            }
                            else if (item.Quantity > 0)
                            {
                                // إضافة عنصر جديد إذا المنتج موجود
                                var productExists = _unit.Product.All().Any(p => p.Id == item.ProductId);
                                if (productExists)
                                {
                                    cart.Items.Add(new CartItem
                                    {
                                        Id = Guid.NewGuid(),
                                        ProductId = item.ProductId,
                                        Quantity = item.Quantity
                                    });
                                }
                            }
                        }

                        cart.UpdatedDate = DateTime.Now;

                        // 3. تحديث الكارت في الـ DbContext
                        _unit.Cart.UpdateAsync(cart);
                        var result = _unit.Save();

                        if (result > 0)
                        {
                            saved = true;
                            return new GeneralResponse<Guid>(cart.Id, _localization["Succesfully"].Value);
                        }
                        else
                        {
                            return new GeneralResponse<Guid>(_localization["ErrorInSave"].Value, System.Net.HttpStatusCode.BadRequest);
                        }
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        // 4. التعامل مع الـ concurrency exception
                        cart = _unit.Cart.All()
                            .Include(c => c.Items)
                                .ThenInclude(i => i.Product)
                            .FirstOrDefault(c => c.UserId == UserId);

                        if (cart == null)
                        {
                            return new GeneralResponse<Guid>(_localization["CartNotFound"].Value, System.Net.HttpStatusCode.NotFound);
                        }

                        // ممكن تضيف هنا logic لإعادة دمج البيانات القديمة والجديدة قبل إعادة المحاولة
                    }
                }

                return new GeneralResponse<Guid>(_localization["ErrorInSave"].Value, System.Net.HttpStatusCode.BadRequest);
            }
            catch (Exception ex)
            {
                return new GeneralResponse<Guid>(ex.Message + "-" + ex.InnerException?.Message, System.Net.HttpStatusCode.BadRequest);
            }
        }
        public async Task<GeneralResponse<Guid>> SoftDelete(Guid Id)
        {
            await _unit.Cart.SoftDelete(Id);
            var results = await _unit.SaveAsync();

            return results >= 1 ? new GeneralResponse<Guid>(Id, _localization["DeletedSuccesfully"].Value) :
                new GeneralResponse<Guid>(_localization["ErrorInDelete"].Value, System.Net.HttpStatusCode.BadRequest);
        }

        public async Task<GeneralResponse<List<Guid>>> SoftRangeDelete(List<Guid> Id)
        {
            await _unit.Cart.SoftDeleteRangeAsync(Id);

            var results = _unit.Save();

            return results >= 1 ? new GeneralResponse<List<Guid>>(Id, _localization["DeletedSuccesfully"].Value) :
                 new GeneralResponse<List<Guid>>(_localization["ErrorInDelete"].Value, System.Net.HttpStatusCode.BadRequest);

        }


        // Remove a single item from cart
        public async Task<GeneralResponse<Guid>> RemoveItemFromCart(Guid ItemId, Guid userId)
        {
            try
            {
                var cart = _unit.Cart.All().Include(x=>x.Items)
                    .FirstOrDefault(c => c.UserId == userId);

                if (cart == null)
                    return new GeneralResponse<Guid>(_localization["Cart not found"].Value, System.Net.HttpStatusCode.BadRequest);

                var item = cart.Items.FirstOrDefault(i => i.Id == ItemId);
                if (item == null)
                    return new GeneralResponse<Guid>(_localization["Item not found in cart"].Value, System.Net.HttpStatusCode.BadRequest);

                await _unit.CartItem.SoftDelete(item.Id);
                var result = await _unit.SaveAsync();

                //cart.Items.Remove(item);
                //cart.UpdatedDate = DateTime.Now;

                //await _unit.Cart.UpdateAsync(cart);
                //var result = _unit.Save();

                return result > 0
                    ? new GeneralResponse<Guid>(cart.Id, _localization["Item removed successfully"])
                    : new GeneralResponse<Guid>(_localization["Error saving changes"], System.Net.HttpStatusCode.BadRequest);
            }
            catch (Exception ex)
            {
                return new GeneralResponse<Guid>(ex.Message + "-" + ex.InnerException?.Message, System.Net.HttpStatusCode.BadRequest);
            }
        }

        // Clear all items in the cart
        public async Task<GeneralResponse<Guid>> ClearCart(Guid userId)
        {
            try
            {
                var cart = _unit.Cart.All()
                    .FirstOrDefault(c => c.UserId == userId);

                if (cart == null)
                    return new GeneralResponse<Guid>(_localization["Cart not found"], System.Net.HttpStatusCode.NotFound);

                foreach (var item in cart.Items)
                {
                    var existingItem = cart.Items
                        .FirstOrDefault(i => i.ProductId == item.ProductId);
                    await _unit.CartItem.SoftDelete(item.Id);
                }
                var result = await _unit.SaveAsync();

                //cart.Items.Clear();
                //cart.UpdatedDate = DateTime.Now;

                //await _unit.Cart.UpdateAsync(cart);
                //var result = _unit.Save();

                return result > 0
                    ? new GeneralResponse<Guid>(cart.Id, _localization["Cart cleared successfully"])
                    : new GeneralResponse<Guid>(_localization["Error saving changes"], System.Net.HttpStatusCode.BadRequest);
            }
            catch (Exception ex)
            {
                return new GeneralResponse<Guid>(ex.Message + "-" + ex.InnerException?.Message, System.Net.HttpStatusCode.BadRequest);
            }
        }

        // Get cart by user
        public async Task<GeneralResponse<CartDto>> GetCartByUserId(Guid Id)
        {
            var user = await _unit.User.GetByIdAsync(Id);
            if (user == null)
                return new GeneralResponse<CartDto>(_localization["NotFound"].Value, System.Net.HttpStatusCode.NotFound);

            var result = _unit.Cart.All().Include(x => x.User).Include(x => x.Items).ThenInclude(i => i.Product).Where(d => d.UserId==user.Id).ToList().Select(x => new CartDto
            {
                Id = x.Id,
                UserId = x.UserId,
                UserName = x.User.UserName,
                CartTotalPrice = x.Items.Sum(i => i.Product.Price * i.Quantity), // calculate total price

                Items = x.Items.Select(i => new CartItemDto
                {
                    Id = i.Id,
                    ProductId = i.ProductId,
                    ProductName = i.Product.Name,
                    Quantity = i.Quantity,
                    Price = i.Product.Price,
                    TotalPrice = (i.Quantity * i.Product.Price)
                }).ToList()

            }).FirstOrDefault();
            return new GeneralResponse<CartDto>(result, _localization["Succes"].Value);
        }

    }
}
