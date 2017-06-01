﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace VideoStore.Business.Components.BankServiceReference {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="BankServiceReference.ITransferService")]
    public interface ITransferService {
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/ITransferService/Transfer")]
        void Transfer(double pAmount, int pFromAcctNumber, int pToAcctNumber);
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/ITransferService/Transfer")]
        System.Threading.Tasks.Task TransferAsync(double pAmount, int pFromAcctNumber, int pToAcctNumber);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface ITransferServiceChannel : VideoStore.Business.Components.BankServiceReference.ITransferService, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class TransferServiceClient : System.ServiceModel.ClientBase<VideoStore.Business.Components.BankServiceReference.ITransferService>, VideoStore.Business.Components.BankServiceReference.ITransferService {
        
        public TransferServiceClient() {
        }
        
        public TransferServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public TransferServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public TransferServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public TransferServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public void Transfer(double pAmount, int pFromAcctNumber, int pToAcctNumber) {
            base.Channel.Transfer(pAmount, pFromAcctNumber, pToAcctNumber);
        }
        
        public System.Threading.Tasks.Task TransferAsync(double pAmount, int pFromAcctNumber, int pToAcctNumber) {
            return base.Channel.TransferAsync(pAmount, pFromAcctNumber, pToAcctNumber);
        }
    }
}
