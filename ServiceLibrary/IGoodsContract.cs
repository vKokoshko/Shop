using System.Collections.Generic;
using System.ServiceModel;

namespace ServiceLibrary
{
    [ServiceContract]
    interface IGoodsContract
    {
        [OperationContract]
        IEnumerable<BusinessGood> GetGoods();
        [OperationContract]
        BusinessGood Get(int GoodId);
    }
}
