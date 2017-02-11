using System.Collections.Generic;
using System.ServiceModel;

namespace ServiceLibrary
{
    [ServiceContract(SessionMode = SessionMode.Required)]
    interface IOrderContract
    {
        [OperationContract]
        string AddOrder(BusinessOrderItem[] businessOrder);
        [OperationContract]
        IEnumerable<BusinessOrder> GetOrders();
    }
}
