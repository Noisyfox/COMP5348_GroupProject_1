using DeliveryCo.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Messaging;
using System.ServiceModel;
using System.Text;
using Bank.Services.Interfaces;
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
                return GetMsmqService<ITransferService>("net.msmq://localhost/private/BankServiceMessageQueueTransacted");
            }
        }

        public IDeliveryService DeliveryService
        {
            get
            {
                return GetMsmqService<IDeliveryService>("net.msmq://localhost/private/DeliveryServiceMessageQueueTransacted");
            }
        }



        private T GetTcpService<T>(String pAddress)
        {
            NetTcpBinding tcpBinding = new NetTcpBinding() { TransactionFlow = true };
            EndpointAddress address = new EndpointAddress(pAddress);
            return new ChannelFactory<T>(tcpBinding, pAddress).CreateChannel();
        }

        private T GetMsmqService<T>(String pAddress)
        {
            NetMsmqBinding msmqBinding = new NetMsmqBinding();
            msmqBinding.Security.Mode = NetMsmqSecurityMode.None;
            EndpointAddress address = new EndpointAddress(pAddress);
            return new ChannelFactory<T>(msmqBinding, pAddress).CreateChannel();
        }
    }
}
