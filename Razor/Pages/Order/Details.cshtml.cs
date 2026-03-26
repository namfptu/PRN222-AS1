using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SalesManagement.Data.Entities;
using SalesManagement.Service.Interfaces;

namespace Razor.Pages.Orders
{
    public class DetailsModel : PageModel
    {
        private readonly IOrderService _orderService;
        public DetailsModel(IOrderService orderService) => _orderService = orderService;

        public Order? Order { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Order = await _orderService.GetOrderByIdAsync(id);
            if (Order == null) return NotFound();
            return Page();
        }

        public async Task<IActionResult> OnPostAcceptAsync(int id)
        {
            try
            {
                await _orderService.UpdateOrderStatusAsync(id, OrderStatus.Done);
                TempData["SuccessMessage"] = "Cập nhật đơn hàng sang Hoàn thành thành công!";
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            return RedirectToPage(new { id });
        }

        public async Task<IActionResult> OnPostCancelAsync(int id)
        {
            try
            {
                await _orderService.UpdateOrderStatusAsync(id, OrderStatus.Cancelled);
                TempData["SuccessMessage"] = "Đã hủy đơn hàng thành công!";
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            return RedirectToPage(new { id });
        }
    }
}
