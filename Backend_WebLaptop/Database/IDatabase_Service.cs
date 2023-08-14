﻿using Backend_WebLaptop.Model;
using MongoDB.Driver;

namespace Backend_WebLaptop.Database
{
    public interface IDatabase_Service
    {
        IMongoCollection<Account> Get_Accounts_Collection();
        IMongoCollection<Province> Get_Provinces_Collection();
        IMongoCollection<District> Get_District_Collection();
        IMongoCollection<Ward> Get_Ward_Collection();
        //IMongoCollection<Account> Get_Accounts_Collection();
    }
}