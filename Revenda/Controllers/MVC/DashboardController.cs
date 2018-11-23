using Newtonsoft.Json;
using Revenda.Classes;
using Revenda.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using System.Web.Helpers;
using System.Collections;

namespace Revenda.Controllers.MVC
{
    [Authorize(Roles = "User, Admin")]
    public class DashboardController : Controller
    {

        private RevendaContext db = new RevendaContext();
        private RevendaEntities context = new RevendaEntities();

        // GET: Dashboard
        public ActionResult Index()
        {

            ViewBag.CountCustomers = db.Customers.Count();
            ViewBag.CountProducts = db.Products.Count();
            ViewBag.CountPurchases = db.Purchases.Count();
            ViewBag.CountOrders = db.Orders.Count();
            ViewBag.CountSales = db.Sales.Count();
            ViewBag.CountConsignations = db.Consignments.Count();
            ViewBag.CountEntries = db.Entries.Count();
            ViewBag.CountStocks = db.Inventories.Where(i => i.Stock > 0).Count();
            ViewBag.CountAccounts = db.Accounts.Count();

            return View();
        }

        public ActionResult ChartColumn()
        {
            SqlDataReader reader = null;
            List<GraphModel1> graphModel1 = new List<GraphModel1>();
            string CS = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            using (SqlConnection con = new SqlConnection(CS))
            {
                var textString =
                    "select TOP 5 sum(Stock) AS Stock, ProductCode from ProductsStocksByWarehouse " +
                    "group by ProductCode " +
                    "ORDER BY SUM(Stock) desc";

                SqlCommand cmd = new SqlCommand(textString, con);
                // Executando o commando e obtendo o resultado
                con.Open();
                reader = cmd.ExecuteReader();

                // Exibindo os registros


                while (reader.Read())
                {
                    GraphModel1 obj1 = new GraphModel1();
                    if (reader["Stock"] == DBNull.Value)
                    {
                        obj1.Stock = 0;
                    }
                    else
                    {
                        obj1.Stock = Convert.ToDecimal(reader["Stock"]);
                    }
                    if (reader["ProductCode"] == DBNull.Value)
                    {
                        obj1.ProductCode = "";
                    }
                    else
                    {
                        obj1.ProductCode = Convert.ToString(reader["ProductCode"]);
                    }
                    graphModel1.Add(obj1);
                }
            }
            ArrayList xValue = new ArrayList();
            ArrayList yValue = new ArrayList();
            graphModel1.ToList().ForEach(gm => xValue.Add(gm.ProductCode));
            graphModel1.ToList().ForEach(gm => yValue.Add(gm.Stock));

            new Chart(width: 500, height: 250, theme: ChartTheme.Blue)
                .AddTitle("5 Maiores Estoques")
                .SetXAxis("Produtos")
                .SetYAxis("Quantidades")
                .AddSeries("Default", chartType: "Column", xValue: xValue, yValues: yValue)
                .Write("bmp");
            return null;
        }

        public ActionResult ChartColumn1()
        {
            List<GraphModel> graphModel1 = new List<GraphModel>();

            SqlDataReader reader = null;

            string CS = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            using (SqlConnection con = new SqlConnection(CS))
            {
                var textString =
                        "SELECT MesAno,  [0] AS Previsto, [1] AS Realizado " +
                        "FROM(SELECT CONCAT(datepart(yy, Data), FORMAT(Data, 'MM')) AS MesAno, StatementType, Value FROM Movements) p " +
                        "PIVOT(SUM(Value) FOR StatementType IN([0], [1]) " +
                        ") AS pvt Order By MesAno";

                SqlCommand cmd = new SqlCommand(textString, con);
                // Executando o commando e obtendo o resultado
                con.Open();
                reader = cmd.ExecuteReader();

                // Exibindo os registros

                while (reader.Read())
                {
                    int mes = 0;
                    string dia = "01";
                    int ano = 0;
                    DateTime DataPar = DateTime.Now;
                    string mesAnoExtenso, mesExt, mesExt1, anoExt1, dataExt;
                    mesExt = Convert.ToString(reader["MesAno"]);
                    mesAnoExtenso = mesExt.Substring(4, 2) + "/" + mesExt.Substring(0, 4);
                    mesExt1 = mesExt.Substring(4, 2);
                    anoExt1 = mesExt.Substring(0, 4);
                    dataExt = dia + "/" + mesExt1 + "/" + anoExt1;
                    mes = Convert.ToDateTime(dataExt).Month;
                    ano = Convert.ToDateTime(dataExt).Year;
                    DataPar = Convert.ToDateTime(dataExt);
                    mesAnoExtenso = Convert.ToDateTime(DataPar).ToString("MMMM", new CultureInfo("pt-BR"));
                    mesAnoExtenso = new CultureInfo("pt-BR").DateTimeFormat.GetAbbreviatedMonthName(mes);
                    mesAnoExtenso = char.ToUpper(mesAnoExtenso[0]) + mesAnoExtenso.Substring(1) + "/" + Convert.ToString(ano);

                    GraphModel obj1 = new GraphModel();
                    if (reader["Realizado"] == DBNull.Value)
                    {
                        obj1.Actual = 0;
                    }
                    else
                    {
                        obj1.Actual = Convert.ToDecimal(reader["Realizado"]);
                    }
                    obj1.Month = mesAnoExtenso;
                    if (reader["Previsto"] == DBNull.Value)
                    {
                        obj1.Predictable = 0;
                    }
                    else
                    {
                        obj1.Predictable = Convert.ToDecimal(reader["Previsto"]);
                    }
                    graphModel1.Add(obj1);
                }
            }
            ArrayList xValue = new ArrayList();
            ArrayList yValue = new ArrayList();
            ArrayList zValue = new ArrayList();
            graphModel1.ToList().ForEach(gm => xValue.Add(gm.Month));
            graphModel1.ToList().ForEach(gm => yValue.Add(gm.Predictable));
            graphModel1.ToList().ForEach(gm => zValue.Add(gm.Actual));

            new Chart(width: 500, height: 250, theme: ChartTheme.Green)
                .AddTitle("Fluxo de Caixa")
                .SetXAxis("Meses")
                .SetYAxis("Valores")                
                .AddSeries("Previsto", chartType: "Column", xValue: xValue, yValues: yValue)
                .AddSeries("Realizado", chartType: "Column", xValue: xValue, yValues: zValue)
                .Write("bmp");
            return null;
        }

        [ValidateAntiForgeryToken]
        public ActionResult GetStock()
        {
            List<GraphModel1> graphModel1 = new List<GraphModel1>();

            SqlDataReader reader = null;

            string CS = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            using (SqlConnection con = new SqlConnection(CS))
            {
                var textString =
                    "select TOP 5 sum(Stock) AS Stock, ProductCode from ProductsStocksByWarehouse " +
                    "group by ProductCode " + 
                    "ORDER BY SUM(Stock) desc";

                SqlCommand cmd = new SqlCommand(textString, con);
                // Executando o commando e obtendo o resultado
                con.Open();
                reader = cmd.ExecuteReader();

                // Exibindo os registros

                while (reader.Read())
                {
                    GraphModel1 obj1 = new GraphModel1();
                    if (reader["Stock"] == DBNull.Value)
                    {
                        obj1.Stock = 0;
                    }
                    else
                    {
                        obj1.Stock = Convert.ToDecimal(reader["Stock"]);
                    }
                    if (reader["ProductCode"] == DBNull.Value)
                    {
                        obj1.ProductCode = "";
                    }
                    else
                    {
                        obj1.ProductCode = Convert.ToString(reader["ProductCode"]);
                    }
                    graphModel1.Add(obj1);
                }



                //if (con != null)
                //{
                //    con.Close();
                //}
                //return Content(JsonConvert.SerializeObject(graphModel1), "application/json");
                return Json(graphModel1, JsonRequestBehavior.AllowGet);
            }
        }

        [ValidateAntiForgeryToken]
        public ActionResult GetData()
        {
            List<GraphModel> graphModel1 = new List<GraphModel>();

            SqlDataReader reader = null;

            string CS = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            using (SqlConnection con = new SqlConnection(CS))
            {
                var textString =
                        "SELECT MesAno,  [0] AS Previsto, [1] AS Realizado " +
                        "FROM(SELECT CONCAT(datepart(yy, Data), FORMAT(Data, 'MM')) AS MesAno, StatementType, Value FROM Movements) p " +
                        "PIVOT(SUM(Value) FOR StatementType IN([0], [1]) " +
                        ") AS pvt Order By MesAno";

                SqlCommand cmd = new SqlCommand(textString, con);
                // Executando o commando e obtendo o resultado
                con.Open();
                reader = cmd.ExecuteReader();

                // Exibindo os registros

                while (reader.Read())
                {
                    int mes = 0;
                    string dia = "01";
                    int ano = 0;
                    DateTime DataPar = DateTime.Now;
                    string mesAnoExtenso, mesExt, mesExt1, anoExt1, dataExt;
                    mesExt = Convert.ToString(reader["MesAno"]);
                    mesAnoExtenso = mesExt.Substring(4, 2) + "/" + mesExt.Substring(0, 4);
                    mesExt1 = mesExt.Substring(4, 2);
                    anoExt1 = mesExt.Substring(0, 4);
                    dataExt = dia + "/" + mesExt1 + "/" + anoExt1;
                    mes = Convert.ToDateTime(dataExt).Month;
                    ano = Convert.ToDateTime(dataExt).Year;
                    DataPar = Convert.ToDateTime(dataExt);
                    mesAnoExtenso = Convert.ToDateTime(DataPar).ToString("MMMM", new CultureInfo("pt-BR"));
                    mesAnoExtenso = new CultureInfo("pt-BR").DateTimeFormat.GetAbbreviatedMonthName(mes);
                    mesAnoExtenso = char.ToUpper(mesAnoExtenso[0]) + mesAnoExtenso.Substring(1) + "/" + Convert.ToString(ano);
                    
                    GraphModel obj1 = new GraphModel();
                    if (reader["Realizado"] == DBNull.Value)
                    {
                        obj1.Actual = 0;
                    }
                    else
                    {
                        obj1.Actual = Convert.ToDecimal(reader["Realizado"]);
                    }
                    obj1.Month = mesAnoExtenso;
                    if (reader["Previsto"] == DBNull.Value)
                    {
                        obj1.Predictable = 0;
                    }
                    else
                    {
                        obj1.Predictable = Convert.ToDecimal(reader["Previsto"]);
                    }
                    graphModel1.Add(obj1);
                }
                //if (con != null)
                //{
                //    con.Close();
                //}
                return Json(graphModel1, JsonRequestBehavior.AllowGet);

                //return Content(JsonConvert.SerializeObject(graphModel1), "application/json");
            }
            //ArrayList xValue = new ArrayList();
            //ArrayList yValue = new ArrayList();
            //graphModel1.ToList().ForEach(gm => xValue.Add(gm.Month));
            //graphModel1.ToList().ForEach(gm => yValue.Add(gm.Predictable));
            //graphModel1.ToList().ForEach(gm => yValue.Add(gm.Actual));

            //new Chart(width: 600, height: 200, theme: ChartTheme.Green)
            //    .AddTitle("5 Maiores Estoques")
            //    .AddSeries("Default", chartType: "Column", xValue: xValue, yValues: yValue)
            //    .Write("bmp");
            //return null;
        }

        [HttpGet]
        public ActionResult GetDetails(string type)
        {
            List<ProductOrCustomerViewModel> view = new List<ProductOrCustomerViewModel>();
            if (type == "customers")
            {
                var result1 = db.Customers.ToList();
                for (int i = 0; i < result1.Count(); i++)
                {
                    ProductOrCustomerViewModel obj1 = new ProductOrCustomerViewModel();
                    obj1.Name = result1[i].FullName;
                    obj1.TypeOrCountryOrAccount = result1[i].City.Name + "/" + result1[i].Estado.Name;
                    obj1.Type = result1[i].Phone;
                    view.Add(obj1);
                }
            }
            else if (type == "products")
            {
                var result1 = db.Products.ToList();
                for (int i = 0; i < result1.Count(); i++)
                {
                    ProductOrCustomerViewModel obj1 = new ProductOrCustomerViewModel();
                    obj1.Name = result1[i].Description;
                    obj1.TypeOrCountryOrAccount = result1[i].ProductCode;
                    obj1.Type = result1[i].Category.Description;
                    view.Add(obj1);
                }
            } else if (type == "accounts")
            {
                var result1 = db.Accounts.ToList();
                for (int i = 0; i < result1.Count(); i++)
                {
                    ProductOrCustomerViewModel obj1 = new ProductOrCustomerViewModel();
                    obj1.Name = result1[i].AccountName;
                    obj1.TypeOrCountryOrAccount = result1[i].AccountCode;
                    obj1.Type = result1[i].AccountSubClass.SubGroupName;
                    view.Add(obj1);
                }
            }
            return PartialView("GetDetails", view);
        }

        [HttpGet]
        public ActionResult GetProccess(string type)
        {
            List<ProccessViewModel> view = new List<ProccessViewModel>();
            if (type == "purchases")
            {
                var result1 = db.Purchases.ToList();
                for (int i = 0; i < result1.Count(); i++)
                {
                    ProccessViewModel obj1 = new ProccessViewModel();
                    obj1.Code = result1[i].PurchaseId;
                    obj1.Partner = result1[i].Supplier.FirstName + " " + result1[i].Supplier.LastName;
                    obj1.Date = result1[i].Date;
                    obj1.Value = result1[i].TotalCost.Value;
                    obj1.Quantity = result1[i].TotalQuantity.Value;
                    view.Add(obj1);
                }
            }
            else if (type == "sales")
            {
                var result1 = db.Sales.ToList();
                for (int i = 0; i < result1.Count(); i++)
                {
                    ProccessViewModel obj1 = new ProccessViewModel();
                    obj1.Code = result1[i].SaleId;
                    obj1.Partner = result1[i].Customer.FullName;
                    obj1.Date = result1[i].Date;
                    obj1.Value = result1[i].TotalValue.Value;
                    obj1.Quantity = result1[i].TotalQuantity.Value;
                    view.Add(obj1);
                }
            }
            else if (type == "orders")
            {
                var result1 = db.Orders.ToList();
                for (int i = 0; i < result1.Count(); i++)
                {
                    ProccessViewModel obj1 = new ProccessViewModel();
                    obj1.Code = result1[i].OrderId;
                    obj1.Partner = result1[i].Customer.FullName;
                    obj1.Date = result1[i].OrderDate;
                    obj1.Value = 0;
                    obj1.Quantity = 0;
                    view.Add(obj1);
                }
            }
            else if (type == "consignments")
            {
                var result1 = db.Consignments.ToList();
                for (int i = 0; i < result1.Count(); i++)
                {
                    ProccessViewModel obj1 = new ProccessViewModel();
                    obj1.Code = result1[i].ConsignmentId;
                    obj1.Partner = result1[i].Seller.FullName;
                    obj1.Date = result1[i].Data;
                    obj1.Value = result1[i].TotalValue;
                    obj1.Quantity = result1[i].TotalQuantity;
                    view.Add(obj1);
                }
            }
            else if (type == "entries")
            {
                var result1 = db.Entries.ToList();
                for (int i = 0; i < result1.Count(); i++)
                {
                    ProccessViewModel obj1 = new ProccessViewModel();
                    obj1.Code = result1[i].EntryId;
                    obj1.Partner = result1[i].Account.AccountName;
                    obj1.Date = result1[i].Data;
                    obj1.Value = result1[i].Value;
                    obj1.Quantity = 0;
                    view.Add(obj1);
                }
            }
            return PartialView("GetProccess", view);
        }

    }
}