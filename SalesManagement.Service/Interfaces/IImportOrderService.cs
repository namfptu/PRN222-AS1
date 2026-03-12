using SalesManagement.Data.Entities;

namespace SalesManagement.Service.Interfaces
{
    public interface IImportOrderService
    {
        Task<IEnumerable<ImportOrder>> GetAllImportOrdersAsync();

        //Xử lý lưu Phiếu nhập + Chi tiết + Cộng tồn kho
        Task<bool> CreateImportOrderAsync(ImportOrder order, List<ImportOrderDetail> details);
        Task<ImportOrder?> GetImportOrderByIdAsync(int id);
    }
}