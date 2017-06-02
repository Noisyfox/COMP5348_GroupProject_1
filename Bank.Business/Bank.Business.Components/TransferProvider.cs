using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bank.Business.Components.Interfaces;
using Bank.Business.Entities;
using System.Transactions;
using Bank.Services.Interfaces;

namespace Bank.Business.Components
{
    public class TransferProvider : ITransferProvider
    {


        public void Transfer(double pAmount, int pFromAcctNumber, int pToAcctNumber,
            string pTransferNotificationAddress, string pTransferReference)
        {
            try
            {
                using (TransactionScope lScope = new TransactionScope())
                using (BankEntityModelContainer lContainer = new BankEntityModelContainer())
                {

                    try
                    {
                        Account lFromAcct = GetAccountFromNumber(pFromAcctNumber);
                        Account lToAcct = GetAccountFromNumber(pToAcctNumber);
                        lFromAcct.Withdraw(pAmount);
                        lToAcct.Deposit(pAmount);
                        lContainer.Attach(lFromAcct);
                        lContainer.Attach(lToAcct);
                        lContainer.ObjectStateManager.ChangeObjectState(lFromAcct, System.Data.EntityState.Modified);
                        lContainer.ObjectStateManager.ChangeObjectState(lToAcct, System.Data.EntityState.Modified);
                        lContainer.SaveChanges();

                        TransferNotificationServiceFactory.GetTransferNotificationService(pTransferNotificationAddress)
                            .NotifyTransferSuccess(pTransferReference);

                        lScope.Complete();
                    }
                    catch (Exception lException)
                    {
                        Console.WriteLine("Error occured while transferring money:  " + lException.Message);
                        throw;
                    }
                }
            }
            catch (Exception lException)
            {
                using (TransactionScope lScope = new TransactionScope(TransactionScopeOption.Suppress))
                {
                    TransferNotificationServiceFactory.GetTransferNotificationService(pTransferNotificationAddress)
                        .NotifyTransferFailed(pTransferReference, lException.Message);

                    lScope.Complete();
                }
            }
        }

        private Account GetAccountFromNumber(int pToAcctNumber)
        {
            using (BankEntityModelContainer lContainer = new BankEntityModelContainer())
            {
                return lContainer.Accounts.Where((pAcct) => (pAcct.AccountNumber == pToAcctNumber)).FirstOrDefault();
            }
        }
    }
}
