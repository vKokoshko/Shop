using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ServiceLibrary
{
    [DataContract]
    public class BusinessOrder
    {
        [DataMember]
        public int OrderId { get; set; }
        [DataMember]
        public string OrderDate { get; set; }
        [DataMember]
        public decimal OrderSum { get; set; }
        [DataMember]
        public List<BusinessOrderItem> OrderItems { get; set; }
    }

    [DataContract]
    public class BusinessOrderItem
    {
        [DataMember]
        public int GoodId { get; set; }
        [DataMember]
        public string GoodName { get; set; }
        [DataMember]
        public decimal GoodCount { get; set; }
        [DataMember]
        public decimal OrderItemSum { get; set; }
    }

    [DataContract]
    public class BusinessGood
    {
        [DataMember]
        public int GoodId { get; set; }
        [DataMember]
        public string GoodName { get; set; }
        [DataMember]
        public string ManufacturerName { get; set; }
        [DataMember]
        public string CategoryName { get; set; }
        [DataMember]
        public decimal Price { get; set; }
        [DataMember]
        public decimal GoodCount { get; set; }
        [DataMember]
        public string Photo { get; set; }
    }
}
