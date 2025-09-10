using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LapTrinhWeb.Models.Vnpay
{
    public class PaymentInformationModel
    {
        public string Name { get; set; }
        public decimal Amount { get; set; }
        public string OrderDescription { get; set; }
        public string OrderType { get; set; }
    }

}