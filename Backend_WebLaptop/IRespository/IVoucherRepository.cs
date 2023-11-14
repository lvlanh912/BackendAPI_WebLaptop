using Backend_WebLaptop.Model;

namespace Backend_WebLaptop.IRespository
{
    public interface IVoucherRepository
    {
        //tạo Voucher mới
        Task<Voucher> CreateVoucher(Voucher entity);
        //lấy thông tin Voucher
        Task<Voucher> GetVoucherbyId(string voucherId);
        Task<Voucher> GetVoucherbyCode(string voucherCode);
        Task<PagingResult<Voucher>> GetAllVouchers(string? keywords , DateTime? createTimeStart, DateTime? createTimeEnd, bool? Active, int pageSize, int pageindex,string sort);
        //sửa thông tin voucher
        Task<Voucher> EditVoucher(Voucher entity, string voucherId);
        Task<bool> DisableVoucher(string voucherId);
        Task<bool> DeleteVoucher(string voucherId);
        Task<bool> IsValidCode(string code);
    }
}
