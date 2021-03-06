﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VideoStore.Business.Entities;

namespace VideoStore.Business.Components.Interfaces
{
    public interface IDeliveryNotificationProvider
    {
        void NotifyDeliverySubmitted(String orderNnmber, Guid pDeliveryId, DeliveryStatus status, String errorMsg);

        void NotifyDeliveryCompletion(Guid pDeliveryId, DeliveryStatus status);
    }
}
