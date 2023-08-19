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
        Task<PagingResult<Voucher>> GetAllVouchers(int PageSize=10,int pageindex=1);
        //sửa thông tin voucher
        Task<Voucher> EditVoucher(Voucher entity, string voucherId);
        /*xoá voucher
         * sẽ không thực hiện xoá trong database chỉ đặt giá trị vào biến
        */
        Task<bool> DeleteVoucher(string voucherId);
    }
}
