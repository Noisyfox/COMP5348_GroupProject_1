using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VideoStore.Business.Components.Interfaces;
using VideoStore.Business.Entities;
using System.Transactions;
using Microsoft.Practices.ServiceLocation;
using DeliveryCo.MessageTypes;
using System.Configuration;

namespace VideoStore.Business.Components
{
    public class OrderProvider : IOrderProvider, ITransferNotificationProvider
    {
        public IEmailProvider EmailProvider
        {
            get { return ServiceLocator.Current.GetInstance<IEmailProvider>(); }
        }

        public IUserProvider UserProvider
        {
            get { return ServiceLocator.Current.GetInstance<IUserProvider>(); }
        }

        public void SubmitOrder(Entities.Order pOrder)
        {
           
            using (TransactionScope lScope = new TransactionScope())
            {
                LoadMediaStocks(pOrder);
                MarkAppropriateUnchangedAssociations(pOrder);
                using (VideoStoreEntityModelContainer lContainer = new VideoStoreEntityModelContainer())
                {
                    try
                    {
                        pOrder.OrderNumber = Guid.NewGuid();
                        TransferFundsFromCustomer(UserProvider.ReadUserById(pOrder.Customer.Id).BankAccountNumber, pOrder.Total ?? 0.0, pOrder.OrderNumber.ToString());

                        //pOrder.UpdateStockLevels();

                        //PlaceDeliveryForOrder(pOrder);
                        lContainer.Orders.ApplyChanges(pOrder);
         
                        lContainer.SaveChanges();
                        lScope.Complete();
                        
                    }
                    catch (Exception lException)
                    {
                        SendOrderErrorMessage(pOrder, lException);
                        throw;
                    }
                }
            }
        }

        private void MarkAppropriateUnchangedAssociations(Order pOrder)
        {
            pOrder.Customer.MarkAsUnchanged();
            pOrder.Customer.LoginCredential.MarkAsUnchanged();
            foreach (OrderItem lOrder in pOrder.OrderItems)
            {
                lOrder.Media.Stocks.MarkAsUnchanged();
                lOrder.Media.MarkAsUnchanged();
            }
        }

        private void LoadMediaStocks(Order pOrder)
        {
            using (VideoStoreEntityModelContainer lContainer = new VideoStoreEntityModelContainer())
            {
                foreach (OrderItem lOrder in pOrder.OrderItems)
                {
                    lOrder.Media.Stocks = lContainer.Stocks.Where((pStock) => pStock.Media.Id == lOrder.Media.Id).FirstOrDefault();    
                }
            }
        }

        

        private void SendOrderErrorMessage(Order pOrder, Exception pException)
        {
            EmailProvider.SendMessage(new EmailMessage()
            {
                ToAddress = pOrder.Customer.Email,
                Message = "There was an error in processsing your order " + pOrder.OrderNumber + ": "+ pException.Message +". Please contact Video Store"
            });
        }

        private void SendOrderPlacedConfirmation(Order pOrder)
        {
            EmailProvider.SendMessage(new EmailMessage()
            {
                ToAddress = pOrder.Customer.Email,
                Message = "Your order " + pOrder.OrderNumber + " has been placed"
            });
        }

        private void PlaceDeliveryForOrder(Order pOrder)
        {
            Delivery lDelivery = new Delivery() { DeliveryStatus = DeliveryStatus.Submitting, SourceAddress = "Video Store Address", DestinationAddress = pOrder.Customer.Address, Order = pOrder };

            //Guid lDeliveryIdentifier = 
            ExternalServiceFactory.Instance.DeliveryService.SubmitDelivery(new DeliveryInfo()
            { 
                OrderNumber = lDelivery.Order.OrderNumber.ToString(),  
                SourceAddress = lDelivery.SourceAddress,
                DestinationAddress = lDelivery.DestinationAddress,
                DeliveryNotificationAddress = ConfigurationManager.AppSettings["emailNotifyAddress"]
            });

            //lDelivery.ExternalDeliveryIdentifier = lDeliveryIdentifier;
            pOrder.Delivery = lDelivery;
            
        }

        private void TransferFundsFromCustomer(int pCustomerAccountNumber, double pTotal, String reference)
        {
            try
            {
                ExternalServiceFactory.Instance.TransferService.Transfer(pTotal, pCustomerAccountNumber, RetrieveVideoStoreAccountNumber(), ConfigurationManager.AppSettings["transferNotifyAddress"], reference);
            }
            catch(Exception e)
            {
                throw new Exception("Error Transferring funds for order.");
            }
        }


        private int RetrieveVideoStoreAccountNumber()
        {
            return 123;
        }

        public void NotifyTransferSuccess(string pOrderNumber)
        {
            using (var lScope = new TransactionScope())
            {
                var orderNumber = Guid.Parse(pOrderNumber);
                using (var lContainer = new VideoStoreEntityModelContainer())
                {
                    var order = lContainer.Orders.Include("Customer").Include("OrderItems.Media.Stocks").FirstOrDefault(pOrder => pOrder.OrderNumber == orderNumber);
                    try
                    {
                        if (order != null)
                        {
                            order.UpdateStockLevels();

                            PlaceDeliveryForOrder(order);
                            lContainer.Orders.ApplyChanges(order);

                            lContainer.SaveChanges();
                            lScope.Complete();
                        }
                    }
                    catch (Exception lException)
                    {
                        SendOrderErrorMessage(order, lException);
                        throw;
                    }
                }
            }
        }

        public void NotifyTransferFailed(string pOrderNumber, string reason)
        {
            using (var lScope = new TransactionScope())
            {
                var orderNumber = Guid.Parse(pOrderNumber);
                using (var lContainer = new VideoStoreEntityModelContainer())
                {
                    var order = lContainer.Orders.Include("Customer").FirstOrDefault(pOrder => pOrder.OrderNumber == orderNumber);
                    try
                    {
                        if (order != null)
                        {
                            EmailProvider.SendMessage(new EmailMessage()
                            {
                                ToAddress = order.Customer.Email,
                                Message = "There was an error in processsing your order " + order.OrderNumber + ": " + reason + ". Please contact Video Store"
                            });

                            lScope.Complete();
                        }
                    }
                    catch (Exception lException)
                    {
                        SendOrderErrorMessage(order, lException);
                        throw;
                    }
                }
            }
        }
    }
}
