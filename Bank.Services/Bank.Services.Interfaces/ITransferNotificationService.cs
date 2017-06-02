using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace Bank.Services.Interfaces
{

    [ServiceContract]
    public interface ITransferNotificationService
    {
        [OperationContract(IsOneWay = true)]
        void NotifyTransferSuccess(String reference);

        [OperationContract(IsOneWay = true)]
        void NotifyTransferFailed(String reference, String reason);
    }
}
