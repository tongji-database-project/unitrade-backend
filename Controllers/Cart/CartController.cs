using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using UniTrade.Tools;
using UniTrade.Models;
using SqlSugar;
using UniTrade.ViewModels;

namespace UniTrade.Controllers.Cart

{
    /// <summary>
    /// Handles all cart-related actions including adding items to the cart,
    /// removing items from the cart, and retrieving cart items.
    /// </summary>
    [Route("[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private List<CartItemViewModel> _cartItems = new List<CartItemViewModel>();

        // 获取购物车列表
        [HttpGet]
        public ActionResult<IEnumerable<CartItemViewModel>> GetCartItems(string customerId)
        {
            var items = _cartItems.Where(c => c.UserId == customerId).ToList();
            return Ok(items);
        }

        // 添加商品到购物车
        [HttpPost]
        public ActionResult AddToCart([FromBody] CartItemViewModel cartItem)
        {
            _cartItems.Add(cartItem);
            return Ok("Added to cart successfully.");
        }

        // 从购物车删除商品
        [HttpDelete("{customerId}/{merchandiseId}")]
        public ActionResult RemoveFromCart(string customerId, string merchandiseId)
        {
            var item = _cartItems.FirstOrDefault(c => c.UserId == customerId && c.MerchandiseId == merchandiseId);
            if (item != null)
            {
                _cartItems.Remove(item);
                return Ok("Removed from cart successfully.");
            }
            return BadRequest("Item not found in cart.");
        }

        // 修改商品状态
        [HttpPost("update")]
        public ActionResult UpdateCartItem([FromBody] CartItemViewModel cartItemUpdate)
        {
            var item = _cartItems.FirstOrDefault(c => c.UserId == cartItemUpdate.UserId && c.MerchandiseId == cartItemUpdate.MerchandiseId);
            if (item != null)
            {
                item.Quantity = cartItemUpdate.Quantity;
                item.Selected = cartItemUpdate.Selected;
                return Ok("Cart item updated successfully.");
            }
            return NotFound("Cart item not found.");
        }
    }
}
