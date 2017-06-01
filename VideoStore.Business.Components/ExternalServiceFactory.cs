using DeliveryCo.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Messaging;
using System.ServiceModel;
using System.Text;
using VideoStore.Business.Components.BankServiceReference;
using VideoStore.Business.Components.EmailServiceEx;

namespace VideoStore.Business.Components
{
    public class ExternalServiceFactory
    {
        private static ExternalServiceFactory sFactory = new ExternalServiceFactory();

        public static ExternalServiceFactory Instance
        {
            get
            {
                return sFactory;
            }
        }



        public IEmailService EmailService
        {
            get { return new EmailServiceClient(); }
        }

        public ITransferService TransferService
        {
            get
            {
                return new TransferServiceClient();
            }
        }

        public IDeliveryService DeliveryService
        {
            get
            {
                return GetTcpService<IDeliveryService>("net.tcp://localhost:9030/DeliveryService");
            }
        }



        private T GetTcpService<T>(String pAddress)
        {
            NetTcpBinding tcpBinding = new NetTcpBinding() { TransactionFlow = true };
            EndpointAddress address = new EndpointAddress(pAddress);
            return new ChannelFactory<T>(tcpBinding, pAddress).CreateChannel();
        }
    }
}
