namespace DataLayer.DBLayer
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("OrderItem")]
    public partial class OrderItem
    {
        public int OrderItemId { get; set; }

        public int? OrderId { get; set; }

        public int? GoodId { get; set; }

        [Column(TypeName = "numeric")]
        public decimal GoodCount { get; set; }

        [Column(TypeName = "money")]
        public decimal OrderItemSum { get; set; }

        public virtual Good Good { get; set; }

        public virtual Order Order { get; set; }
    }
}
