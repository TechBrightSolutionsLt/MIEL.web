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
                    GstAmount = Convert.ToDecimal(dr["GstAmount"]),
                    TaxableAmount = Convert.ToDecimal(dr["TaxableAmount"]),
                    NetAmount = Convert.ToDecimal(dr["NetAmount"])
                });
            }
        }

        return list;
    }
}