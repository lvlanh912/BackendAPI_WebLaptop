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
        Task<PagingResult<Voucher>> GetAllVouchers(string? keywords,int PageSize,int pageindex,bool isDisable);
        //sửa thông tin voucher
        Task<Voucher> EditVoucher(Voucher entity, string voucherId);
        Task<bool> DisableVoucher(string voucherId);
        Task<bool> DeleteVoucher(string voucherId);
    }
}
