using DataLayer.DBLayer;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.ServiceModel;

namespace ServiceLibrary
{
    [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
    class GoodsContract : IGoodsContract
    {
        public IEnumerable<BusinessGood> GetGoods()
        {
            ShopContext context = new ShopContext();
            context.Configuration.ProxyCreationEnabled = false;
            try
            {
                List<BusinessGood> goods = new List<BusinessGood>();
                foreach (var good in context.Goods)
                {
                    BusinessGood newBusinessGood = new BusinessGood();
                    newBusinessGood.GoodId = good.GoodId;
                    newBusinessGood.GoodName = good.GoodName;
                    Manufacturer manufacturer = context.Manufacturers.Where(m => m.ManufacturerId == good.ManufacturerId).FirstOrDefault();
                    newBusinessGood.ManufacturerName = (manufacturer == null) ? "unknown" : manufacturer.ManufacturerName;
                    Category category = context.Categories.Where(c => c.CategoryId == good.CategoryId).FirstOrDefault();
                    newBusinessGood.CategoryName = (category == null) ? "unknown" : category.CategoryName;
                    newBusinessGood.Price = good.Price;
                    newBusinessGood.GoodCount = good.GoodCount;
                    newBusinessGood.Photo = good.Photo;
                    goods.Add(newBusinessGood);
                }
                return goods;
            }
            catch (Exception exc)
            {
                throw new FaultException("Error Server FaultException: " + exc.Message);
            }
        }
        
        public BusinessGood Get(int GoodId)
        {
            ShopContext context = new ShopContext();
            context.Configuration.ProxyCreationEnabled = false;
            try
            {
                Good good = context.Goods.Find(GoodId);
                if (good == null)
                    throw new FaultException("NULL");
                else
                {
                    BusinessGood newBusinessGood = new BusinessGood();
                    newBusinessGood.GoodId = good.GoodId;
                    newBusinessGood.GoodName = good.GoodName;
                    newBusinessGood.ManufacturerName = good.Manufacturer.ManufacturerName;
                    newBusinessGood.CategoryName = good.Category.CategoryName;
                    newBusinessGood.Price = good.Price;
                    newBusinessGood.GoodCount = good.GoodCount;
                    newBusinessGood.Photo = good.Photo;
                    return newBusinessGood;
                }
            }
            catch (Exception exc)
            {
                throw new FaultException("Error Server FaultException Get(int GoodId): " + exc.Message);
            }
        }
    }
}
