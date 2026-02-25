using SalesManagement.Data.Entities;
using SalesManagement.Repo.Interfaces;
using SalesManagement.Service.Interfaces;

namespace SalesManagement.Service.Implementations
{
    public class SupplierService : ISupplierService
    {
        private readonly IGenericRepository<Supplier> _supplierRepository;

        public SupplierService(IGenericRepository<Supplier> supplierRepository)
        {
            _supplierRepository = supplierRepository;
        }

        public async Task<IEnumerable<Supplier>> GetAllSuppliersAsync()
        {
            return await _supplierRepository.GetAllAsync();
        }

        public async Task<Supplier> GetSupplierByIdAsync(int id)
        {
            var supplier = await _supplierRepository.GetByIdAsync(id);
            return supplier;
        }

        public async Task<bool> CreateSupplierAsync(Supplier supplier)
        {
            // Sửa lại thành true (đại diện cho trạng thái Active)
            supplier.Status = true;

            await _supplierRepository.AddAsync(supplier);
            await _supplierRepository.SaveChangesAsync();

            return true;
        }

        public async Task<bool> UpdateSupplierAsync(Supplier supplier)
        {
            _supplierRepository.Update(supplier);
            await _supplierRepository.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteSupplierAsync(int id)
        {
            var supplier = await _supplierRepository.GetByIdAsync(id);
            if (supplier != null)
            {
                // Thực hiện xóa mềm: Sửa lại thành false (đại diện cho Inactive)
                supplier.Status = false;

                _supplierRepository.Update(supplier);
                await _supplierRepository.SaveChangesAsync();

                return true;
            }
            return false;
        }
    }
}