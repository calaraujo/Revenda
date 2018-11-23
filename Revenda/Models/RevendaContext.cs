using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace Revenda.Models
{
    public class RevendaContext : DbContext
    {
        public RevendaContext() : base("DefaultConnection")
        {

        }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
        }

        public DbSet<Estado> Estadoes { get; set; }

        public DbSet<City> Cities { get; set; }

        public DbSet<Company> Companies { get; set; }

        public DbSet<User> Users { get; set; }

        public DbSet<Category> Categories { get; set; }

        public DbSet<Product> Products { get; set; }

        public DbSet<Warehouse> Warehouses { get; set; }

        public DbSet<Supplier> Suppliers { get; set; }

        public DbSet<Customer> Customers { get; set; }

        public DbSet<Seller> Sellers { get; set; }

        public DbSet<Condition> Conditions { get; set; }

        public DbSet<Order> Orders { get; set; }

        public DbSet<OrderDetail> OrderDetails { get; set; }

        public DbSet<Inventory> Inventories { get; set; }

        public DbSet<OrderDetailTmp> OrderDetailTmps { get; set; }

        public DbSet<Purchase> Purchases { get; set; }

        public DbSet<PurchaseDetail> PurchaseDetails { get; set; }

        public DbSet<PurchaseDetailTmp> PurchaseDetailTmps { get; set; }

        public DbSet<Sale> Sales { get; set; }

        public DbSet<SalesDetail> SalesDetails { get; set; }

        public DbSet<SalesDetailTmp> SalesDetailTmps { get; set; }

        public DbSet<Receivable> Receivables { get; set; }

        public DbSet<ReceivableDetail> ReceivableDetails { get; set; }

        public DbSet<Parameter> Parameters { get; set; }

        public DbSet<Payable> Payables { get; set; }

        public DbSet<PayableDetail> PayableDetails { get; set; }

        public DbSet<Commission> Commissions { get; set; }

        public DbSet<Settlement> Settlements { get; set; }

        public DbSet<SettlementDetail> SettlementDetails { get; set; }

        public DbSet<SettlementPayable> SettlementPayables { get; set; }

        public DbSet<ConsignmentsDetailTmp> ConsignmentsDetailTmps { get; set; }

        public DbSet<Consignment> Consignments { get; set; }

        public DbSet<ConsignmentsDetail> ConsignmentsDetails { get; set; }

        public DbSet<ConsignmentReceivable> ConsignmentReceivables { get; set; }

        public DbSet<ConsignmentReceivableDetail> ConsignmentReceivableDetails { get; set; }

        public System.Data.Entity.DbSet<Revenda.Models.AccountClass> AccountClasses { get; set; }

        public System.Data.Entity.DbSet<Revenda.Models.AccountSubClass> AccountSubClasses { get; set; }

        public System.Data.Entity.DbSet<Revenda.Models.Account> Accounts { get; set; }

        public System.Data.Entity.DbSet<Revenda.Models.Movement> Movements { get; set; }

        public System.Data.Entity.DbSet<Revenda.Models.Entry> Entries { get; set; }

        public DbSet<StockLedger> StockLedgers { get; set; }

    }
}