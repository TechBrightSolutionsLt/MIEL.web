using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using MIEL.web.Models.ViewModel;

public class VATReportController : Controller
{
    private readonly IConfiguration _config;

    public VATReportController(IConfiguration config)
    {
        _config = config;
    }

    public IActionResult Index()
    {
        VATReportVM vm = new VATReportVM();
        vm.FromDate = DateTime.Today;
        vm.ToDate = DateTime.Today;
        vm.Type = "PURCHASE";
        return View(vm);
    }

    [HttpPost]
    public IActionResult Index(VATReportVM vm)
    {
        vm.Results = GetVATReport(vm);
        return View(vm);
    }
   
    [HttpPost]
    public IActionResult ExportToExcel(VATReportVM vm)
    {
        var data = GetVATReport(vm);

        var builder = new System.Text.StringBuilder();
        builder.AppendLine("GST REPORT");
        builder.AppendLine($"From {vm.FromDate:dd-MM-yyyy} To {vm.ToDate:dd-MM-yyyy}");
        builder.AppendLine("");
        builder.AppendLine("Invoice No,Date,GST Amount,Taxable Amount,Net Amount");

        decimal totalGst = 0;
        decimal totalTaxable = 0;
        decimal totalNet = 0;

        foreach (var item in data)
        {
            builder.AppendLine($"{item.InvoiceNo},{item.InvoiceDate:dd-MM-yyyy},{item.GstAmount},{item.TaxableAmount},{item.NetAmount}");
            totalGst += item.GstAmount;
            totalTaxable += item.TaxableAmount;
            totalNet += item.NetAmount;
        }

        builder.AppendLine($",Total,{totalGst},{totalTaxable},{totalNet}");

        return File(
            System.Text.Encoding.UTF8.GetBytes(builder.ToString()),
            "text/csv",
            "GSTReport.csv"
        );
    }
    private List<VATReportResult> GetVATReport(VATReportVM vm)
    {
        List<VATReportResult> list = new List<VATReportResult>();

        string conStr = _config.GetConnectionString("MielConnectionString");

        using (SqlConnection con = new SqlConnection(conStr))
        {
            SqlCommand cmd = new SqlCommand("SP_VAT_REPORT", con);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@Type", vm.Type);
            cmd.Parameters.AddWithValue("@FromDate", vm.FromDate);
            cmd.Parameters.AddWithValue("@ToDate", vm.ToDate);

            con.Open();
            SqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                list.Add(new VATReportResult
                {
                    InvoiceNo = dr["InvoiceNo"].ToString(),
                    InvoiceDate = Convert.ToDateTime(dr["InvoiceDate"]),
                    PartyName = dr["PartyName"].ToString(),
                    GstAmount = Convert.ToDecimal(dr["GstAmount"]),
                    TaxableAmount = Convert.ToDecimal(dr["TaxableAmount"]),
                    NetAmount = Convert.ToDecimal(dr["NetAmount"])
                });
            }
        }

        return list;
    }
}