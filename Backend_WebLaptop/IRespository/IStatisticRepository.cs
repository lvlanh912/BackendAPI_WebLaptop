﻿

using Backend_WebLaptop.Model;

namespace Backend_WebLaptop.IRespository
{
    public interface IStatisticRepository
    {
        /// <summary>
        /// Return product have stock and product out of stock
        /// </summary>
        /// <returns></returns>
        Task<object> GetStockStatistic();
        /// <summary>
        /// Return list product added each mouth
        /// </summary>
        /// <returns></returns>
        Task<List<long>> GetProductAddStatistic(int year);
        /// <summary>
        /// Return Top ProductSold , type=1 is top Sold, type=2 is Top Views , type =3 is Top stock
        /// </summary>
        /// <returns></returns>
        Task<List<Product>> GetTop(int total, DateTime starTime, DateTime? endTime,int type);
        /// <summary>
        /// Return Total Order per state
        /// </summary>
        /// <param name="startime"></param>
        /// <param name="endtime"></param>
        /// <returns></returns>
        Task<object> GetOrderStatistic();
        /// <summary>
        /// Get total order in month (max 12 month)
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        Task<List<long>> GetOrderPerMonth(DateTime startTime, DateTime endTime);
        /// <summary>
        /// Return total value order end paied order in time
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        Task<object> GetRevenue(DateTime startTime, DateTime endTime);

    }
}
