using System;
using System.Collections.Generic;

namespace MIEL.web.Models.ViewModel
{
   
        public class VATReportVM
        {
            public string Type { get; set; }
            public DateTime FromDate { get; set; }
            public DateTime ToDate { get; set; }

            public List<VATReportResult> Results { get; set; }
        }

        public class VATReportResult
        {
            public string InvoiceNo { get; set; }
            public DateTime InvoiceDate { get; set; }
            public decimal GstAmount { get; set; }
            public decimal TaxableAmount { get; set; }
            public decimal NetAmount { get; set; }
        }
    
}
