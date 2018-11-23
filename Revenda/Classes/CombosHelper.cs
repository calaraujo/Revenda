using Revenda.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections;
using System.Data.Entity;

namespace Revenda.Classes
{
    public class CombosHelper
    {
        private static RevendaContext db = new RevendaContext();
        public int Pedido = 0;

        public static List<Estado> GetEstadoes()
        {
            var estados = db.Estadoes.ToList();

            estados.Add(new Estado
            {
                EstadoId = 0,
                Name = "[Selecione um Estado ...]"
            }); 

            return estados = estados.OrderBy(d => d.Name).ToList();

        }

        public static List<Seller> GetSeller1()
        {
            var sellers = db.Sellers.ToList();

            sellers.Add(new Seller
            {
                SellerId = 0,
                FirstName = "[Todos os Vendedores ...]"
            });

            return sellers = sellers.OrderBy(s => s.FirstName).ThenBy(s => s.LastName).ToList();
        }

        public static List<Customer> GetCustomers1()
        {

            var customers = db.Customers.ToList();

            customers.Add(new Customer
            {
                CustomerId = 0,
                FirstName = "[Todos os Clientes ...]"
            });

            return customers = customers.OrderBy(c => c.FirstName).ThenBy(c => c.LastName).ToList();
        }

        public static List<Product> GetProduct()
        {
            var products = db.Products.ToList();

            products.Add(new Product
            {
                ProductId = 0,
                Description = "[Selecione um Produto ...]"
            });

            return products = products.OrderBy(p => p.Description).ToList();
        }

        public static List<Product> GetProduct(int productId)
        {
            var products = db.Products.Where(p => p.ProductId == productId).ToList();

            products.Add(new Product
            {
                ProductId = 0,
                Description = "[Selecione um Produto ...]"
            });

            return products = products.OrderBy(p => p.Description).ToList();
        }

        public static List<AccountSubClass> GetAccountSubClasses()
        {
            var accountSubClass = db.AccountSubClasses.ToList();

            accountSubClass.Add(new AccountSubClass
            {
                AccountSubClassId = 0,
                SubGroupName = "[Selecione um Sub-Grupo ...]"
            });

            return accountSubClass = accountSubClass
                .OrderBy(a => a.AccountClassId)
                .ToList();
        }

        public static List<AccountSubClass> GetAccountSubClasses(int accountSubClassId)
        {
            var accountSubClass = db.AccountSubClasses.Where(a => a.AccountSubClassId == accountSubClassId).ToList();

            accountSubClass.Add(new AccountSubClass
            {
                AccountSubClassId = 0,
                SubGroupName = "[Selecione um Sub-Grupo ...]"
            });

            return accountSubClass = accountSubClass
                .OrderBy(a => a.AccountClassId)
                .ToList();
        }

        public static List<AccountClass> GetAccountClasses()
        {
            var accountClass = db.AccountClasses.ToList();

            accountClass.Add(new AccountClass
            {
                AccountClassId = 0,
                GroupName = "[Selecione um Grupo ...]"
            });

            return accountClass = accountClass
                .OrderBy(a => a.AccountClassId)
                .ToList();
        }

        public static List<AccountClass> GetAccountClasses(int accountClassId)
        {
            var accountClass = db.AccountClasses.Where(a=>a.AccountClassId == accountClassId).ToList();

            accountClass.Add(new AccountClass
            {
                AccountClassId = 0,
                GroupName = "[Selecione um Grupo ...]"
            });

            return accountClass = accountClass
                .OrderBy(a => a.AccountClassId)
                .ToList();
        }

        public static List<Account> GetAccounts()
        {
            var accounts = db.Accounts.ToList();

            accounts.Add(new Account
            {
                AccountId = 0,
                AccountName = "[Selecione uma Conta ...]"
            });

            return accounts = accounts
                .OrderBy(a => a.AccountClassId)
                .ThenBy(a => a.AccountSubClassId)
                .ThenBy(a => a.AccountCode)
                .ToList();
        }

        public static List<Account> GetAccounts(int accountId)
        {
            var accounts = db.Accounts.Where(a => a.AccountId == accountId).ToList();

            accounts.Add(new Account
            {
                AccountId = 0,
                AccountName = "[Selecione uma Conta ...]"
            });

            return accounts = accounts
                .OrderBy(a => a.AccountClassId)
                .ThenBy(a => a.AccountSubClassId)
                .ThenBy(a => a.AccountCode)
                .ToList();
        }

        public static List<Receivable> GetReceivables(int id, bool sw)
        {
            var receivables = db.Receivables.Where(r => r.SaleId == id).ToList();
            return receivables.OrderBy(r => r.Date).ToList();
        }

        public static List<Product> GetProducts(int companyId, bool sw)
        {
            var products = db.Products.Where(p => p.CompanyId == companyId).ToList();
            return products.OrderBy(p => p.ProductCode).ToList();
        }

        public static List<Product> GetProducts(int companyId)
        {
            var products = db.Products.Where(p => p.CompanyId == companyId).ToList();

            products.Add(new Product
            {
                ProductId = 0,
                Description = "[Selecione um Produto ...]"
            });

            return products = products.OrderBy(p => p.Description).ToList();
        }

        public static IEnumerable GetPayables(int purchaseId, bool v)
        {
            var payables = db.Payables.Where(r => r.PurchaseId == purchaseId).ToList();
            return payables.OrderBy(r => r.Date).ToList();
        }

        public static List<City> GetCities()
        {
            var cities = db.Cities.ToList();

            cities.Add(new City
            {
                CityId = 0,
                Name = "[Selecione uma Cidade ...]"
            });

            return cities = cities.OrderBy(c => c.Name).ToList();

        }

        public static List<Supplier> GetSuppliers()
        {
            var suppliers = db.Suppliers.ToList();

            suppliers.Add(new Supplier
            {
                SupplierId = 0,
                FirstName = "[Selecione um Fornecedor ...]"
            });

            return suppliers = suppliers.OrderBy(c => c.FirstName).ThenBy(c => c.LastName).ToList();
        }

        internal static IEnumerable GetConditions()
        {
            var conditions = db.Conditions.ToList();
            return conditions.OrderBy(c => c.Description).ToList();
        }

      
        public static IEnumerable GetCustomers(int customerId)
        {

            var customers = db.Customers.Where(p => p.CustomerId == customerId).ToList(); 
            
            customers.Add(new Customer
            {
                CustomerId = 0,
                FirstName = "[Selecione um Cliente ...]"
            });

            return customers = customers.OrderBy(c => c.FirstName).ThenBy(c => c.LastName).ToList();
        }

        //internal static List<Commission> GetCommission()
        //{
        //    var commissions = db.Commissions.ToList();

        //    commissions.Add(new Commission
        //    {
        //        CommissionId = 0,
        //        Description = "[Selecione uma Faixa ...]"
        //    });

        //    return commissions.OrderBy(d => d.Description).ToList();
        //}
    
        public static IEnumerable GetOrders(int companyId, bool sw)
        {
            var orders = db.Orders.Where(ord => ord.CompanyId == companyId).ToList();
            return orders.OrderBy(ord => ord.CustomerId).ToList();
        }

        public static List<ConsignmentReceivable> GetConsignmentReceivables(int consignmentId, bool v)
        {
                var consignmentReceivables = db.ConsignmentReceivables.Where(r => r.ConsignmentId == consignmentId).ToList();
                return consignmentReceivables.OrderBy(r => r.Date).ToList();
        }

        public static List<Warehouse> GetWarehouses()
        {
            var warehouses = db.Warehouses.ToList();

            warehouses.Add(new Warehouse
            {
                WarehouseId = 0,
                Name = "[Selecione um Mostruário ...]"
            });

            return warehouses.OrderBy(d => d.Name).ToList();
        }

        public static List<Warehouse> GetWarehouses(int warehouseId)
        {
            var warehouses = db.Warehouses.Where(w => w.WarehouseId == warehouseId).ToList();

            warehouses.Add(new Warehouse
            {
                WarehouseId = 0,
                Name = "[Selecione um Mostruário ...]"
            });

            return warehouses.OrderBy(d => d.Name).ToList();
        }

        public static List<Product> GetProductsWithStock(int companyId, bool v)
        {          
            var products = (from p in db.Products
                           from i in db.Inventories
                           from w in db.Warehouses
                           where p.ProductId == i.ProductId
                                && i.Stock > 0M
                                && w.WarehouseId == 1
                                && i.WarehouseId == w.WarehouseId
                           orderby p.ProductCode
                           select new 
                           {
                               p.ProductId,
                               p.ProductCode,
                               p.Description,
                               p.Price,
                               p.Cost
                           });

            var product = new List<Product>();
            foreach (var item in products)
            {
                product.Add(new Product()
                {
                    ProductId = item.ProductId,
                    ProductCode = item.ProductCode,
                    Description = item.Description,
                    Price = item.Price,
                    Cost = item.Cost,
                });
            }
            return product;
        }

        public static List<City> GetCities(int estadoId)
        {
            var cities = db.Cities.Where(c => c.EstadoId == estadoId).ToList();
            cities.Add(new City
            {
                CityId = 0,
                Name = "[Selecione uma Cidade ...]",
            });

            return cities.OrderBy(d => d.Name).ToList();
        }

        public static List<Company> GetCompanies()
        {
            var companies = db.Companies.ToList();

            companies.Add(new Company
            {
                CompanyId = 0,
                Name = "[Selecione uma Empresa ...]"
            });

            return companies = companies.OrderBy(d => d.Name).ToList();

        }

        public static List<Category> GetCategories(int companyId)      
        {
            var categories = db.Categories.Where(c => c.CompanyId == companyId).ToList();
            categories.Add(new Category
            {
                CategoryId = 0,
                Description = "[Selecione uma Categoria ...]"
            });

            return categories = categories.OrderBy(d => d.Description).ToList();
        }

        public static List<Customer> GetCustomers()
        {

            var customers = db.Customers.ToList();

            customers.Add(new Customer
            {
                CustomerId = 0,
                FirstName = "[Selecione um Cliente ...]"
            });

            return customers = customers.OrderBy(c => c.FirstName).ThenBy(c => c.LastName).ToList();

        }

        public static List<Condition> GetCondition(int id)
        {
            var conditions = db.Conditions.Where(c => c.ConditionId == id).ToList();
            conditions.Add(new Condition
            {
                ConditionId = 0,
                Description = "[Selecione uma Cond.Pagamento ...]"
            });

            return conditions = conditions.OrderBy(d => d.Description).ToList();
        }

        public static IEnumerable GetCondition()
        {
            var conditions = db.Conditions.ToList();

            conditions.Add(new Condition
            {
                ConditionId = 0,
                Description = "[Selecione uma Cond.Pagamento ...]"
            });

            return conditions = conditions.OrderBy(c => c.Description).ToList();
        }

        public static IEnumerable GetSeller()
        {
            var sellers = db.Sellers.ToList();

            sellers.Add(new Seller
            {
                SellerId = 0,
                FirstName = "[Selecione um Vendedor ...]"
            });

            return sellers = sellers.OrderBy(c => c.FirstName).ThenBy(c => c.LastName).ToList();
        }

        public void Dispose()
        {
            db.Dispose();
        }

        public static List<Purchase> GetPurchases(int id, bool sw)
        {
            var purchases = db.Purchases.Where(r => r.PurchaseId == id).ToList();
            return purchases.OrderBy(p => p.Date).ToList();
        }

        public static IEnumerable GetCommission()
        {
            var commissions = db.Commissions.ToList();

            commissions.Add(new Commission
            {
                CommissionId = 0,
                Description = "[Selecione uma faixa de comissão ...]"
            });

            return commissions = commissions.OrderBy(c => c.Description).ToList();
        }
    }
}
