using Revenda.Models;
using System;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;
using static Revenda.Controllers.ConsignmentsController;
using static Revenda.Models.Movement;
using static Revenda.Models.StockLedger;

namespace Revenda.Classes
{
    public class MovementsHelper : IDisposable
    {
        private static RevendaContext db = new RevendaContext();
        private static TypeOfMovement typeMoviment;

        public void Dispose()
        {
            db.Dispose();
        }

        public static Response NewOrder(NewOrderView view, string userName, bool api)
        {

            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    var user = db.Users.Where(u => u.UserName == userName).FirstOrDefault();
                    var order = new Order
                    {
                        CompanyId = user.CompanyId,
                        CustomerId = view.CustomerId,
                        OrderDate = view.OrderDate,
                        Remarks = view.Remarks,
                        ConditionId = view.ConditionId,
                        SellerId = view.SellerId,
                    };
                    db.Orders.Add(order);
                    db.SaveChanges();
                    if (api == false) // A origem é MVC
                    {
                        var details = db.OrderDetailTmps.Where(odt => odt.UserName == userName).ToList();
                        foreach (var detail in details)
                        {
                            var orderDetail = new OrderDetail
                            {
                                Description = detail.Description,
                                OrderId = order.OrderId,
                                Price = detail.Price,
                                ProductId = detail.ProductId,
                                Quantity = detail.Quantity,

                            };

                            db.OrderDetails.Add(orderDetail);
                            db.OrderDetailTmps.Remove(detail);
                        }
                    }
                    else
                    {
                        var details1 = view.Details;
                        foreach (var detail in details1)
                        {
                            var orderDetail = new OrderDetail
                            {
                                Description = detail.Description,
                                OrderId = order.OrderId,
                                Price = detail.Price,
                                ProductId = detail.ProductId,
                                Quantity = detail.Quantity,

                            };

                            db.OrderDetails.Add(orderDetail);
                        }
                    }

                    db.SaveChanges();
                    transaction.Commit();
                    return new Response { Succeeded = true, };
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return new Response
                    {
                        Message = ex.Message,
                        Succeeded = false,
                    };
                }
            }
        }

        public static Response NewEntry(Entry entry, string userName)
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    var user = db.Users.Where(u => u.UserName == userName).FirstOrDefault();

                    var entries = new Entry
                    {
                        AccountId = entry.AccountId,
                        Data = entry.Data,
                        Description = entry.Description,
                        Value = entry.Value,
                    };
                    db.Entries.Add(entries);
                    db.SaveChanges();

                    // Atualiza Fluxo de Caixa Previsto
                    var parameter = db.Parameters.Where(p => p.Identity == "DIVE").FirstOrDefault();
                    var description = entry.Description;
                    var value = entry.Value;
                    var data = entry.Data;
                    var statementtype = TypeOfStatement.Previsto;
                    UpdateCashFlow(parameter.ParameterId, description, value, statementtype, data, entry.AccountId);
                    db.SaveChanges();

                    // Atualiza Fluxo de Caixa Realizado
                    var description1 = entry.Description;
                    var value1 = entry.Value;
                    var data1 = entry.Data;
                    var statementtype1 = TypeOfStatement.Realizado;
                    UpdateCashFlow(parameter.ParameterId, description1, value1, statementtype1, data1, entry.AccountId);
                    db.SaveChanges();
                    transaction.Commit();
                    return new Response { Succeeded = true, };
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return new Response
                    {
                        Message = ex.Message,
                        Succeeded = false,
                    };
                }
            }
        }

        public static void CreatePayable(int purchaseId, string Origem)
        {
            //
            // Gerar contas a Pagar
            //

            decimal soma = 0;
            decimal valor = 0;
            decimal valortotal = 0;
            int PId;
            DateTime payableData = DateTime.Now;
            try
            {
                var purchase = db.Purchases.Find(purchaseId);
                valortotal = purchase.TotalCost.Value;
                if (Origem == "Compra")
                {
                    payableData = purchase.Date;
                }
                var payable = new Payable
                {
                    SupplierId = purchase.SupplierId,
                    Date = payableData,
                    ConditionId = purchase.ConditionId,
                    PurchaseId = purchase.PurchaseId,
                    Status = "Em Aberto",
                    Payment = "A Vencer",
                };
                db.Payables.Add(payable);
                db.SaveChanges();

                PId = payable.PayableId;

                var query = (from c in db.Conditions
                                where c.ConditionId == payable.ConditionId
                                select new { c.Quantity }).Single();
                int quantity = query.Quantity;
                var query1 = (from c in db.Conditions
                                where c.ConditionId == payable.ConditionId
                                select new { c.Interval }).Single();
                int interval = query1.Interval;

                DateTime datavenc;
                DateTime database = purchase.Date;

                for (int i = 0; i < quantity; i++)
                {
                    datavenc = database.AddDays(interval);
                    var payableDetail = new PayableDetail
                    {
                        PayableId = payable.PayableId,
                        DueDate = datavenc,
                        Value = valortotal / quantity,
                        Balance = valortotal / quantity,
                        ValuePaid = 0,
                    };

                    valor = Math.Round(payableDetail.Value, 2);
                    payableDetail.Value = valor;
                    valor = Math.Round(payableDetail.Balance, 2);
                    payableDetail.Balance = valor;
                    db.PayableDetails.Add(payableDetail);
                    database = datavenc;
                    soma += valor;

                    // Atualiza Fluxo de Caixa
                    var parameter = db.Parameters.FirstOrDefault(p => p.Identity == "CPRO");
                    if (parameter == null)
                    {
                        //transaction.Rollback();
                    }
                    var id = parameter.ParameterId;
                    //var accountId = Convert.ToInt32(parameter.Value);
                    var account = db.Accounts.Where(a => a.AccountCode == parameter.Value).FirstOrDefault();
                    var description = "Parcela a Pagar";
                    var value = payableDetail.Value;
                    var data = payableDetail.DueDate.Value;
                    var statementtype = TypeOfStatement.Previsto;
                    UpdateCashFlow(id, description, value, statementtype, data, account.AccountId);
                    db.SaveChanges();
                }
                }
            catch (Exception ex )
            {
                //transaction.Rollback();
                var message = ex;
            }
        }

        public static Response Calculus(int id, int pid)
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                decimal difer = 0;
                decimal soma = 0;
                decimal valor = 0;
                int PId;

                try
                {
                    Settlement settlement = db.Settlements.Where(s => s.SettlementId == id).FirstOrDefault();
                    var qry = (from sa in db.Sales
                               join sd in db.SalesDetails on sa.SaleId equals sd.SaleId
                               where (sa.Date >= settlement.LowerDate && sa.Date <= settlement.UpperDate)
                               select new { sd }).ToList();

                    decimal valortotal = 0;
                    decimal vtotal = 0;

                    foreach (var item in qry)
                    {
                        vtotal = item.sd.Price * item.sd.Quantity;
                        valortotal += vtotal;
                    }

                    var commissions = db.Commissions.OrderBy(co => co.CommissionId).ToList();
                    int pointer = 0;
                    decimal percentual = 0;
                    decimal comissao;
                    int IdCommission = 0;
                    foreach (var faixa in commissions)
                    {
                        if (valortotal >= faixa.LowerLimit && valortotal <= faixa.UpperLimit)
                        {
                            pointer = faixa.CommissionId;
                            percentual = faixa.Percent;
                            IdCommission = faixa.CommissionId;
                            break;
                        }
                    }

                    comissao = Math.Round((valortotal * percentual), 2);

                    var settlementDetail = new SettlementDetail
                    {
                        Bonus = 0,
                        Comission = comissao,
                        NetValue = valortotal - comissao,
                        Percent = percentual,
                        SettlementId = settlement.SettlementId,
                        TotalSales = valortotal,
                    };

                    db.SettlementDetails.Add(settlementDetail);

                    settlement.CommissionId = IdCommission;
                    settlement.NetValue = valortotal - comissao;
                    settlement.Percent = percentual;
                    settlement.TotalSales = valortotal;
                    settlement.CommissionValue = comissao;


                    db.Entry(settlement).State = EntityState.Modified;

                    // Gerar cabeçalho do Contas a Pagar

                    var purchase = db.Purchases.Find(pid);
                    var condition = db.Conditions.Where(c => c.SupplierCondition == true).FirstOrDefault();

                    var payable = new Payable
                    {
                        ConditionId = condition.ConditionId,  // Será atualizado na geração de itens de contas a pagar
                        Date = DateTime.Now,
                        Status = "Em Aberto",
                        Payment = "A Vencer",
                        PurchaseId = pid,
                        SupplierId = purchase.SupplierId,
                        //SettlementId = settlement.SettlementId,
                    };
                    db.Payables.Add(payable);


                    db.SaveChanges();

                    var settlementPayable = new SettlementPayable
                    {
                        PayableId = payable.PayableId,
                        SettlementId = settlement.SettlementId,
                    };

                    db.SettlementPayables.Add(settlementPayable);   // Liga Acerto com Contas a Pagar                  

                    db.Entry(settlement).State = EntityState.Modified;
                    PId = payable.PayableId;

                    var query = (from c in db.Conditions
                                 where c.ConditionId == payable.ConditionId
                                 select new { c.Quantity }).Single();
                    int quantity = query.Quantity;
                    var query1 = (from c in db.Conditions
                                  where c.ConditionId == payable.ConditionId
                                  select new { c.Interval }).Single();
                    int interval = query1.Interval;

                    DateTime datavenc;
                    DateTime database = payable.Date;

                    for (int i = 0; i < quantity; i++)
                    {
                        datavenc = database.AddDays(interval);
                        var payableDetail = new PayableDetail
                        {
                            PayableId = payable.PayableId,
                            DueDate = datavenc,
                            Value = (valortotal - comissao) / quantity,
                            Balance = (valortotal - comissao) / quantity,
                            ValuePaid = 0,
                        };

                        valor = Math.Round(payableDetail.Value, 2);
                        payableDetail.Value = valor;
                        valor = Math.Round(payableDetail.Balance, 2);
                        payableDetail.Balance = valor;
                        db.PayableDetails.Add(payableDetail);
                        database = datavenc;
                        soma += valor;
                    }

                    db.SaveChanges();
                    transaction.Commit();
                    if (soma != (valortotal - comissao))
                    {
                        difer = valortotal - soma;
                        var payableDetail = db.PayableDetails.Where(r => r.PayableId == PId).FirstOrDefault();
                        payableDetail.Value = payableDetail.Value + difer;
                        payableDetail.Balance = payableDetail.Balance + difer;
                        db.Entry(payableDetail).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    return new Response { Succeeded = true, };
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return new Response
                    {
                        Message = ex.Message,
                        Succeeded = false,
                    };
                }
            }

        }

        public static void CreateReceivable(int saleId, string Origem)
        {
            //
            // Gerar contas a Receber
            //
            using (var transaction = db.Database.BeginTransaction())
            {
                decimal soma = 0;
                decimal valor = 0;
                decimal valortotal = 0;
                int RId;
                var receivableData = DateTime.Now;

                try
                {
                    var sale = db.Sales.Find(saleId);
                    valortotal = sale.TotalValue.Value;
                    if (Origem == "Venda")
                    {
                        receivableData = sale.Date;
                    }
                    var receivable = new Receivable
                    {
                        CustomerId = sale.CustomerId,
                        Date = receivableData,
                        ConditionId = sale.ConditionId,
                        SaleId = sale.SaleId,
                        Status = "Em Aberto",
                        Payment = "A Vencer",
                    };
                    db.Receivables.Add(receivable);
                    db.SaveChanges();

                    RId = receivable.ReceivableId;

                    var query = (from c in db.Conditions
                                 where c.ConditionId == receivable.ConditionId
                                 select new { c.Quantity }).Single();
                    int quantity = query.Quantity;
                    var query1 = (from c in db.Conditions
                                  where c.ConditionId == receivable.ConditionId
                                  select new { c.Interval }).Single();
                    int interval = query1.Interval;

                    DateTime datavenc;
                    DateTime database = sale.Date;

                    for (int i = 0; i < quantity; i++)
                    {
                        datavenc = database.AddDays(interval);
                        var receivableDetail = new ReceivableDetail
                        {
                            ReceivableId = receivable.ReceivableId,
                            DueDate = datavenc,
                            Value = valortotal / quantity,
                            Balance = valortotal / quantity,
                            ValuePaid = 0,
                        };

                        valor = Math.Round(receivableDetail.Value, 2);
                        receivableDetail.Value = valor;
                        valor = Math.Round(receivableDetail.Balance, 2);
                        receivableDetail.Balance = valor;
                        db.ReceivableDetails.Add(receivableDetail);
                        database = datavenc;
                        soma += valor;

                        // Atualiza Fluxo de Caixa
                        var parameter = db.Parameters.FirstOrDefault(p => p.Identity == "RVDI");
                        if (parameter == null)
                        {
                            transaction.Rollback();
                        }
                        var id = parameter.ParameterId;
                        var account = db.Accounts.Where(a => a.AccountCode == parameter.Value).FirstOrDefault();
                        var description = "Parcela a Receber";
                        var value = receivableDetail.Value;
                        var data = receivableDetail.DueDate.Value;
                        var statementtype = TypeOfStatement.Previsto;
                        UpdateCashFlow(id, description, value, statementtype, data, account.AccountId);
                        db.SaveChanges();
                    }
                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();

                }
            }
        }

        public static Response ChangeConsignment(ChangeConsignmentView view, string userName)
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    var user = db.Users.Where(u => u.UserName == userName).FirstOrDefault();
                    var consignment = db.Consignments.Find(view.ConsignmentId);
                    consignment.TotalValue = 0;
                    consignment.TotalQuantity = 0;

                    var detalhes = db.ConsignmentsDetails.Where(pd => pd.ConsignmentId == consignment.ConsignmentId).ToList();
                    foreach (var detalhe in detalhes)
                    {
                        var typeDocument = TypeOfDocument.UPDC;
                        // Atualiza Mostruário Origem
                        UpdateInventories(consignment.WarehouseId, "ADD", detalhe.ProductId, detalhe.Quantity, true, typeDocument, consignment.ConsignmentId);
                        // Atualiza Mostruário Destino
                        UpdateInventories(consignment.WarehouseId, "SUBTRACT", detalhe.ProductId, detalhe.Quantity, false, typeDocument, consignment.ConsignmentId);

                        db.ConsignmentsDetails.Remove(detalhe);
                    }

                    var details = db.ConsignmentsDetailTmps.Where(pdt => pdt.UserName == userName && pdt.ConsignmentId == view.ConsignmentId).ToList();

                    foreach (var detail in details)
                    {
                        var consignmentsDetail = new ConsignmentsDetail
                        {
                            Description = detail.Description,
                            ConsignmentId = consignment.ConsignmentId,
                            Price = detail.Price,
                            ProductId = detail.ProductId,
                            Quantity = detail.Quantity,                            
                        };

                        db.ConsignmentsDetails.Add(consignmentsDetail);

                        consignment.TotalValue += detail.Value;
                        consignment.TotalQuantity += detail.Quantity;
                        // Atualiza Mostruário Origem
                        var typeDocument = TypeOfDocument.UPDC;
                        UpdateInventories(consignment.WarehouseId, "SUBTRACT", detail.ProductId, detail.Quantity, true, typeDocument, consignment.ConsignmentId);
                        // Atualiza Mostruário Destino
                        UpdateInventories(consignment.WarehouseId, "ADD", detail.ProductId, detail.Quantity, false, typeDocument, consignment.ConsignmentId);

                        db.ConsignmentsDetailTmps.Remove(detail);
                    }
                    db.Entry(consignment).State = EntityState.Modified;
                    db.SaveChanges();
                    transaction.Commit();
                    return new Response { Succeeded = true, };
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return new Response
                    {
                        Message = ex.Message,
                        Succeeded = false,
                    };
                }
            }
        }

        public static void UpdatePayable(Payment payment, int payableDetailid, int settlementid)
        {
            decimal bonus = 0;
           
            var payableDetails = new PayableDetail();
            var payables = new Payable();
            var settlements = new Settlement();
            var settlementDetails = new SettlementDetail();
            var condition = new Condition();
            var parameters = new Parameter();

            condition = db.Conditions.Where(c => c.ConditionId == payment.ConditionId).FirstOrDefault();
            payableDetails = db.PayableDetails.Where(pd => pd.PayableDetailId == payableDetailid).FirstOrDefault();
            if (condition.Description == "À Vista")
            {
                parameters = db.Parameters.Where(p => p.Identity == "COMV").FirstOrDefault();
                bonus = Math.Round((payment.Value * (Convert.ToDecimal(parameters.Value)) / 100), 2);

  //              payables = db.Payables.Where(pa => pa.SettlementId == settlementid).FirstOrDefault();

                //settlements = db.Settlements.Where(s => s.SettlementId == settlementid).FirstOrDefault();
                //settlementDetails = db.SettlementDetails.Where(sd => sd.SettlementId == settlementid).FirstOrDefault();
                //settlements.Bonus = bonus;
                //settlements.NetValue -= bonus;
                //settlementDetails.Bonus = bonus;
                //settlementDetails.NetValue -= bonus;
                //settlements.Status = "Liquidado";
                payables.Status = "Liquidado";
                payables.Payment = "Liquidado";
            }

            db.Entry(payables).State = EntityState.Modified;
            //db.Entry(settlements).State = EntityState.Modified;

        }

        public static void UpdateInventories(int warehouse, string operacao, int productId, decimal quantity, 
                                            bool warehouseBase, TypeOfDocument typeDocument, int documentNumber)
        {
            var typeDoc = Convert.ToInt32(typeDocument);
            string detailDescription = "";
            string filter = "MOBA";
            int wBase = 0;
            var parameter = db.Parameters.FirstOrDefault(p => p.Identity == filter);
            wBase = Convert.ToInt32(parameter.Value);
            var prod = db.Products.Find(productId);
            var warh = db.Warehouses.Find(warehouse);
            var warhb = db.Warehouses.Find(wBase);
            decimal totalBefore = 0;
            decimal totalAfter = 0;
            decimal qty = 0;
            decimal stock = 0; // servirá para iniciar o totalBefore do Ledger
            qty = quantity;
            int warhId; string warhName;
            if (warehouseBase)
            {
                warhId = warhb.WarehouseId;
                warhName = warhb.Name;
            }
            else
            {
                warhId = warh.WarehouseId;
                warhName = warh.Name;
            }

            if (operacao == "SUBTRACT")
            {
                qty = qty * (-1);
                typeMoviment = TypeOfMovement.Saída;
            }
            else
            {
                typeMoviment = TypeOfMovement.Entrada;
            }

            if (typeDoc == 0)
            {
                detailDescription = "Compra de Produtos - Doc. Número : " + documentNumber.ToString("D5");
                //if (operacao == "SUBTRACT")
                //{
                //    qty = qty * (-1);
                //    typeMoviment = TypeOfMovement.Saída;
                //} else
                //{
                //    typeMoviment = TypeOfMovement.Entrada;
                //}
            }
            else if (typeDoc == 1)
            {
                detailDescription = "Venda de Produtos - Doc. Número : " + documentNumber.ToString("D5");
                //if (operacao == "SUBTRACT")
                //{
                //    qty = qty * (-1);
                //    typeMoviment = TypeOfMovement.Saída;
                //}
                //else
                //{
                //    typeMoviment = TypeOfMovement.Entrada;
                //}
            }
            else if (typeDoc == 2)
            {
                detailDescription = "Devolução/Cancelamento de Compras de Produtos - Doc. Número : " + documentNumber.ToString("D5");
                //if (operacao == "SUBTRACT")
                //{
                //    qty = qty * (-1);
                //    typeMoviment = TypeOfMovement.Saída;
                //}
                //else
                //{
                //    typeMoviment = TypeOfMovement.Entrada;
                //}
            }
            else if (typeDoc == 3)
            {
                detailDescription = "Devolução/Cancelamento de Venda de Produtos - Doc. Número : " + documentNumber.ToString("D5");
                //if (operacao == "SUBTRACT")
                //{
                //    qty = qty * (-1);
                //    typeMoviment = TypeOfMovement.Saída;
                //}
                //else
                //{
                //    typeMoviment = TypeOfMovement.Entrada;
                //}
            }
            else if (typeDoc == 4)
            {
                detailDescription = "Remessa de Consignação de Produtos - Doc. Número : " + documentNumber.ToString("D5");
                //if (operacao == "SUBTRACT")
                //{
                //    qty = qty * (-1);
                //    typeMoviment = TypeOfMovement.Saída;
                //}
                //else
                //{
                //    typeMoviment = TypeOfMovement.Entrada;
                //}
            }
            else if (typeDoc == 5)
            {
                detailDescription = "Retorno de Consignação de Produtos - Doc. Número : " + documentNumber.ToString("D5");
                //if (operacao == "SUBTRACT")
                //{
                //    qty = qty * (-1);
                //    typeMoviment = TypeOfMovement.Saída;
                //}
                //else
                //{
                //    typeMoviment = TypeOfMovement.Entrada;
                //}
            }
            else if (typeDoc == 6)
            {
                detailDescription = "Cancelamento de Consignação de Produtos - Doc. Número : " + documentNumber.ToString("D5");
                //if (operacao == "SUBTRACT")
                //{
                //    qty = qty * (-1);
                //    typeMoviment = TypeOfMovement.Saída;
                //}
                //else
                //{
                //    typeMoviment = TypeOfMovement.Entrada;
                //}
            }
            else if (typeDoc == 7)
            {
                detailDescription = "Atualização de Consignação de Produtos - Doc. Número : " + documentNumber.ToString("D5");
                //if (operacao == "SUBTRACT")
                //{
                //    qty = qty * (-1);
                //    typeMoviment = TypeOfMovement.Saída;
                //}
                //else
                //{
                //    typeMoviment = TypeOfMovement.Entrada;
                //}
            }
            else if (typeDoc == 8)
            {
                detailDescription = "Ajuste Manual de Estoque de Produtos - Doc. Número : " + documentNumber.ToString("D5");

                // No caso de ajuste de estoque o inventário que, por ventura exista, deve ser eliminado já que estaremos criando
                // um novo.
                var inventory9 = db.Inventories.Where(i => i.WarehouseId == warhId
                                        && i.ProductId == productId).FirstOrDefault();
                if (inventory9 != null)
                {
                    db.Inventories.Remove(inventory9);
                }
                db.SaveChanges();
                //if (operacao == "SUBTRACT")
                //{
                //    qty = qty * (-1);
                //    typeMoviment = TypeOfMovement.Saída;
                //}
                //else
                //{
                //    typeMoviment = TypeOfMovement.Entrada;
                //}
            }
            else if (typeDoc == 9)
            {
                detailDescription = "Entrada Consignação no Mostruário Destino - Doc. Número : " + documentNumber.ToString("D5");
                //if (operacao == "SUBTRACT")
                //{
                //    qty = qty * (-1);
                //    typeMoviment = TypeOfMovement.Saída;
                //}
                //else
                //{
                //    typeMoviment = TypeOfMovement.Entrada;
                //}
            }

            if (warehouseBase)
            {
                // Retira Qtde do Produto do Mostruário Origem

                var inventory = db.Inventories.Where(i => i.WarehouseId == warhId
                    && i.ProductId == productId).FirstOrDefault();

                if (operacao == "SUBTRACT")
                {
                    if (inventory == null)
                    {
                        var inventory1 = new Inventory
                        {
                            ProductId = productId,
                            WarehouseId = wBase,
                            Stock = quantity * (-1),
                        };
                        db.Inventories.Add(inventory1);
                        stock = 0;
                    }
                    else
                    {
                        stock = inventory.Stock;
                        inventory.Stock -= quantity;
                        db.Entry(inventory).State = EntityState.Modified;
                    }
                }
                if (operacao == "ADD")
                {
                    if (inventory == null)
                    {
                        var inventory1 = new Inventory
                        {
                            ProductId = productId,
                            WarehouseId = wBase,
                            Stock = quantity,
                        };
                        db.Inventories.Add(inventory1);
                        stock = 0;
                    }
                    else
                    {
                        stock = inventory.Stock;
                        inventory.Stock += quantity;
                        db.Entry(inventory).State = EntityState.Modified;
                    }
                }
            }
            else
            {
                var inventorys3 = db.Inventories.Where(i => i.WarehouseId == warhId
                            && i.ProductId == productId).FirstOrDefault();
                if (operacao == "SUBTRACT")
                {
                    if (inventorys3 == null)
                    {
                        var inventory4 = new Inventory
                        {
                            ProductId = productId,
                            WarehouseId = warehouse,
                            Stock = quantity * (-1),
                        };
                        db.Inventories.Add(inventory4);
                        stock = 0;
                    }
                    else
                    {
                        stock = inventorys3.Stock;
                        inventorys3.Stock -= quantity;
                        db.Entry(inventorys3).State = EntityState.Modified;
                    }
                }
                if (operacao == "ADD")
                {
                    if (inventorys3 == null)
                    {
                        var inventory4 = new Inventory
                        {
                            ProductId = productId,
                            WarehouseId = warehouse,
                            Stock = quantity,
                        };
                        db.Inventories.Add(inventory4);
                        stock = 0;
                    }
                    else
                    {
                        stock = inventorys3.Stock;
                        inventorys3.Stock += quantity;
                        db.Entry(inventorys3).State = EntityState.Modified;
                    }
                }
            }
            db.SaveChanges();
            // Atualiza Ledger
            if (typeDoc == 8)
            {
                // Como se está fazendo um Ajuste de Estoque significa que todo e qualquer ledger deverá
                // ser previamente excluído e por consequencia um novo ledger será iniciado.
                var inventory = db.Inventories.Where(st => st.WarehouseId == warhId && st.ProductId == productId).FirstOrDefault();
                var id = inventory.InventoryId;
                detailDescription = "Ajuste Manual de Estoque de Produtos - Doc. Número : " + id.ToString("D5");
                var ledger1 = db.StockLedgers.Where(st => st.WarehouseId == warhId && st.ProductId == productId).ToList();
                foreach (var item in ledger1)
                {
                    db.StockLedgers.Remove(item);
                };
            }
            db.SaveChanges();
            var ledger = db.StockLedgers.Where(st => st.WarehouseId == warhId && st.ProductId == productId)
                .OrderByDescending(st => st.Id)
                .FirstOrDefault();

            if (ledger == null)
            {
                totalBefore = stock;
                totalAfter = totalBefore + qty;
            }
            else
            {
                totalBefore = ledger.TotalAfter;
                totalAfter = totalBefore + qty;
            }
            var stockLedger = new StockLedger
            {
                Amount = quantity,
                ProductCode = prod.ProductCode,
                ProductDescription = prod.Description,
                WarehouseId = warhId,
                WarehouseDescription = warhName,
                ProductId = prod.ProductId,
                TotalBefore = totalBefore,
                TotalAfter = totalAfter,
                DocumentNumber = documentNumber,
                DocumentType = typeDocument,
                TypeMoviment = typeMoviment,
                DetailDescription = detailDescription,                
            };

            db.StockLedgers.Add(stockLedger);
            db.SaveChanges();
        }

        public static Response NewConsignment(NewConsignmentView view, string userName)
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                decimal valortotal = 0;

                // Recuperar número de dias previsto para acerto da consignação
                var parameter = db.Parameters.FirstOrDefault(p => p.Identity == "DTPA");
                var days = Convert.ToInt32(parameter.Value);
                var predictedDate = view.Data.AddDays(days);
                try
                {
                    var user = db.Users.Where(u => u.UserName == userName).FirstOrDefault();
                    var consignment = new Consignment
                    {
                        CompanyId = user.CompanyId,
                        Data = view.Data,
                        Remarks = view.Remarks,
                        WarehouseId = view.WarehouseId,
                        ConditionId = view.ConditionId,
                        SellerId = view.SellerId,
                        TotalReturnQuantity = 0,
                        TotalReturnValue = 0,
                        TotalSaleQuantity = 0,
                        TotalSaleValue = 0,
                        HitDate = null,
                        Status = "Em Aberto",
                        PredictedDate = predictedDate,
                    };
                    db.Consignments.Add(consignment);
                    db.SaveChanges();

                    var details = db.ConsignmentsDetailTmps.Where(sdt => sdt.UserName == userName).ToList();

                    foreach (var detail in details)
                    {
                        var consignmentsDetail = new ConsignmentsDetail
                        {
                            Description = detail.Description,
                            ConsignmentId = consignment.ConsignmentId,
                            Price = detail.Price,
                            ProductId = detail.ProductId,
                            Quantity = detail.Quantity,
                        };
                        valortotal = valortotal + detail.Value;
                        Math.Round(valortotal, 2);

                        db.ConsignmentsDetails.Add(consignmentsDetail);

                        consignment.TotalValue += detail.Value;
                        consignment.TotalQuantity += detail.Quantity;

                        // Retira Qtde do Produto do Mostruário Origem
                        var typeDocument = TypeOfDocument.CONS;
                        UpdateInventories(consignment.WarehouseId, "SUBTRACT", detail.ProductId, detail.Quantity, true, typeDocument, consignment.ConsignmentId);

                        typeDocument = TypeOfDocument.CONE;
                        // Aloca Qtde do Produto no Mostruário Destino
                        UpdateInventories(consignment.WarehouseId, "ADD", detail.ProductId, detail.Quantity, false, typeDocument, consignment.ConsignmentId);

                        db.ConsignmentsDetailTmps.Remove(detail);
                    }

                    //// Atualiza Fluxo de Caixa
                    //var param = db.Parameters.FirstOrDefault(p => p.Identity == "RVCO");
                    //if (parameter == null)
                    //{
                    //    transaction.Rollback();
                    //}
                    //var id = parameter.ParameterId;
                    //var account = db.Accounts.Where(a => a.AccountCode == parameter.Value).FirstOrDefault();
                    //var description = "Provisão Venda Consignada";
                    //var value = consignment.TotalValue;
                    //var data = consignment.Data;
                    //var statementtype = TypeOfStatement.Previsto;
                    //UpdateCashFlow(id, description, value, statementtype, data, account.AccountId);

                    //// Atualiza Fluxo de Caixa
                    //param = db.Parameters.FirstOrDefault(p => p.Identity == "EVCO");
                    //if (param == null)
                    //{
                    //    transaction.Rollback();
                    //}
                    //id = param.ParameterId;
                    //account = db.Accounts.Where(a => a.AccountCode == param.Value).FirstOrDefault();
                    //description = "Estorno Provisão Venda Consignada";
                    //value = consignment.TotalValue;
                    //data = consignment.Data;
                    //statementtype = TypeOfStatement.Previsto;
                    //UpdateCashFlow(id, description, value, statementtype, data, account.AccountId);


                    db.SaveChanges();
                    transaction.Commit();
                    return new Response { Succeeded = true, };
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return new Response
                    {
                        Message = ex.Message,
                        Succeeded = false,
                    };
                }
            }
        }

        public static Response CopyConsignment(int id, string username)
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    var consignments = db.Consignments.Find(id);
                    var consignmentsDetails = db.ConsignmentsDetails.Where(cd => cd.ConsignmentId == id).ToList();
                    foreach (var detail in consignmentsDetails)
                    {
                        var consignmentsDetailTmp = new ConsignmentsDetailTmp
                        {
                            Description = detail.Description,
                            Price = detail.Price,
                            ProductId = detail.ProductId,
                            Quantity = detail.Quantity,
                            ReturnQuantity = detail.ReturnQuantity,
                            SaleQuantity = detail.SaleQuantity,
                            UserName = username,
                        };
                        db.ConsignmentsDetailTmps.Add(consignmentsDetailTmp);
                    }
                    db.SaveChanges();
                    transaction.Commit();
                    return new Response { Succeeded = true, };
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return new Response
                    {
                        Message = ex.Message,
                        Succeeded = false,
                    };
                }
            }
        }

        public static Response CancelConsignment(int id)
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                var details = db.ConsignmentsDetails.Where(cd => cd.ConsignmentId == id);
                var consignment = db.Consignments.Find(id);
                if (consignment.Status == "CR Gerado" || consignment.Status == "Liquidado")
                {
                    return new Response
                    {
                        Message = "Existe documento de Contas a Receber gerado ou liquidado.",
                        Succeeded = true,
                    };
                }
                else
                {
                    try
                    {

                        foreach (var detail in details)
                        {
                            var typeDocument = TypeOfDocument.CANC;
                            // Retira Qtde do Produto do Mostruário Origem
                            if (detail.ReturnQuantity > 0)
                            {
                                UpdateInventories(consignment.WarehouseId, "ADD", detail.ProductId, detail.ReturnQuantity, false, typeDocument, consignment.ConsignmentId);
                                UpdateInventories(consignment.WarehouseId, "SUBTRACT", detail.ProductId, detail.ReturnQuantity, true, typeDocument, consignment.ConsignmentId);
                            }

                            // Aloca Qtde do Produto no Mostruário Destino
                            if (detail.SaleQuantity > 0)
                            {
                                UpdateInventories(consignment.WarehouseId, "ADD", detail.ProductId, detail.SaleQuantity, false, typeDocument, consignment.ConsignmentId);
                            }

                            detail.ReturnQuantity = 0;
                            detail.SaleQuantity = 0;
                            db.Entry(detail).State = EntityState.Modified;
                        }
                        consignment.TotalReturnQuantity = 0;
                        consignment.TotalReturnValue = 0;
                        consignment.TotalSaleQuantity = 0;
                        consignment.TotalSaleValue = 0;
                        consignment.HitDate = null;
                        consignment.Status = "Em Aberto";
                        db.Entry(consignment).State = EntityState.Modified;

                        db.SaveChanges();
                        transaction.Commit();
                        return new Response { Succeeded = true, };
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        return new Response
                        {
                            Message = ex.Message,
                            Succeeded = false,
                        };
                    }
                }
            }
        }

        public static Response UpdateConsignment(int id)
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    var details = db.ConsignmentsDetails.Where(cd => cd.ConsignmentId == id);
                    var consignment = db.Consignments.Find(id);
                    foreach (var detail in details)
                    {
                        if (detail.ReturnQuantity == 0)
                        {
                            detail.SaleQuantity = detail.Quantity;
                            detail.ReturnQuantity = 0;

                            // Desaloca Qtde do Produto no Mostruário Destino
                            var typeDocument = TypeOfDocument.UPDC;
                            UpdateInventories(consignment.WarehouseId, "SUBTRACT", detail.ProductId, detail.SaleQuantity, false, typeDocument, consignment.ConsignmentId);

                            db.Entry(detail).State = EntityState.Modified;

                            consignment.TotalSaleQuantity += detail.SaleQuantity;
                            consignment.TotalSaleValue += detail.Price * detail.SaleQuantity;
                        }
                    }
                    consignment.Status = "Calculado";
                    consignment.HitDate = DateTime.Now;

                    db.Entry(consignment).State = EntityState.Modified;
                    db.SaveChanges();
                    transaction.Commit();
                    return new Response { Succeeded = true, };
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return new Response
                    {
                        Message = ex.Message,
                        Succeeded = false,
                    };
                }
            }
        }

        public static Response ReturnConsignment(ConsignmentsDetail view)
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    var consignmentsDetail = db.ConsignmentsDetails.Find(view.ConsignmentsDetailId);
                    var consignment = db.Consignments.Find(consignmentsDetail.ConsignmentId);
                    var typeDocument = TypeOfDocument.RETC;
                    if (view.ReturnQuantity > 0)
                    { 
                        consignmentsDetail.ReturnQuantity = view.ReturnQuantity;

                        // Aloca Qtde do Produto no Mostruário Base

                        UpdateInventories(consignment.WarehouseId, "ADD", consignmentsDetail.ProductId, view.ReturnQuantity, true, typeDocument, consignment.ConsignmentId);
                        // Desaloca Qtde do Produto no Mostruário Destino
                        UpdateInventories(consignment.WarehouseId, "SUBTRACT", consignmentsDetail.ProductId, view.ReturnQuantity, false, typeDocument, consignment.ConsignmentId);

                        consignment.TotalReturnQuantity += view.ReturnQuantity;
                        consignment.TotalReturnValue += consignmentsDetail.Price * view.ReturnQuantity;

                        consignmentsDetail.SaleQuantity =  consignmentsDetail.Quantity - view.ReturnQuantity;
                        if (consignmentsDetail.SaleQuantity > 0)
                        {                        
                            // Desaloca Qtde do Produto no Mostruário Destino
                            UpdateInventories(consignment.WarehouseId, "SUBTRACT", consignmentsDetail.ProductId, consignmentsDetail.SaleQuantity, false, typeDocument, consignment.ConsignmentId);
                            consignment.TotalSaleQuantity += consignmentsDetail.SaleQuantity;
                            consignment.TotalSaleValue += consignmentsDetail.Price * consignmentsDetail.SaleQuantity;
                        }
                    }
                    else
                    {
                        consignmentsDetail.SaleQuantity = consignmentsDetail.Quantity;
                        // Desaloca Qtde do Produto no Mostruário Destino
                        UpdateInventories(consignment.WarehouseId, "SUBTRACT", consignmentsDetail.ProductId, consignmentsDetail.SaleQuantity, false, typeDocument, consignment.ConsignmentId);

                        consignment.TotalSaleQuantity += consignmentsDetail.SaleQuantity;
                        consignment.TotalSaleValue += consignmentsDetail.Price * consignmentsDetail.SaleQuantity;
                    }

                    db.Entry(consignmentsDetail).State = EntityState.Modified;
                    
                    db.Entry(consignment).State = EntityState.Modified;

                    db.SaveChanges();
                    transaction.Commit();
                    return new Response { Succeeded = true, };
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return new Response
                    {
                        Message = ex.Message,
                        Succeeded = false,
                    };
                }
            }
        }

        public static Response DeleteConsignment(int id, string userName)
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                var Consignment = db.Consignments.Find(id);
                string status = Consignment.Status;
                var typeDocument = TypeOfDocument.CANC;
                if ( status != "Em Aberto")
                {
                    return new Response
                    {
                        Message = "Impossível excluir. Já existe acerto realizado.",
                        Succeeded = false,
                    };
                }
                else
                {
                    try
                    {
                        var details = db.ConsignmentsDetails.Where(sd => sd.ConsignmentId == Consignment.ConsignmentId).ToList();
                        foreach (var detail in details)
                        {
                            // Aloca Qtde do Produto no Mostruário Base
                            UpdateInventories(Consignment.WarehouseId, "ADD", detail.ProductId, detail.Quantity, true, typeDocument, Consignment.ConsignmentId);

                            // Desaloca Qtde do Produto no Mostruário Destino
                            UpdateInventories(Consignment.WarehouseId, "SUBTRACT", detail.ProductId, detail.Quantity, false, typeDocument, Consignment.ConsignmentId);

                            db.Entry(detail).State = EntityState.Deleted;
                        }

                        db.Entry(Consignment).State = EntityState.Deleted;
                        db.SaveChanges();
                        transaction.Commit();
                        return new Response { Succeeded = true, };
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        return new Response
                        {
                            Message = ex.Message,
                            Succeeded = false,
                        };
                    }
                }
            }
        }

        public static Response ChangePurchase(ChangePurchaseView view, string userName)
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                decimal OriginalValue = 0;
                decimal difer = 0;

                try
                {
                    var user = db.Users.Where(u => u.UserName == userName).FirstOrDefault();
                    var purchase = db.Purchases.Find(view.PurchaseId);
                    OriginalValue = purchase.TotalCost.Value; // Save previous TotalCost
                    purchase.TotalCost = 0;
                    purchase.TotalQuantity = 0;

                    var detalhes = db.PurchaseDetails.Where(pd => pd.PurchaseId == purchase.PurchaseId).ToList();
                    foreach (var detalhe in detalhes)
                    {
                        var inventory1 = db.Inventories.Where(i => i.WarehouseId == purchase.WarehouseId
                            && i.ProductId == detalhe.ProductId).FirstOrDefault();
                        if (inventory1 != null)
                        {
                            inventory1.Stock -= detalhe.Quantity;
                            db.Entry(inventory1).State = EntityState.Modified;
                        }
                        db.PurchaseDetails.Remove(detalhe);
                    }

                    var details = db.PurchaseDetailTmps.Where(pdt => pdt.UserName == userName && pdt.PurchaseId == view.PurchaseId).ToList();

                    foreach (var detail in details)
                    {
                        var purchaseDetail = new PurchaseDetail
                        {
                            Description = detail.Description,
                            PurchaseId = purchase.PurchaseId,
                            Cost = detail.Cost,
                            ProductId = detail.ProductId,
                            Quantity = detail.Quantity,                       
                        };

                        db.PurchaseDetails.Add(purchaseDetail);

                        purchase.TotalCost += detail.Value;
                        purchase.TotalQuantity += detail.Quantity;

                        var estoque = db.Inventories.Where(i => i.WarehouseId == purchase.WarehouseId
                            && i.ProductId == detail.ProductId).FirstOrDefault();
                        if (estoque == null)
                        {
                            var estoque1 = new Inventory
                            {
                                ProductId = detail.ProductId,
                                WarehouseId = view.WarehouseId,
                                Stock = detail.Quantity,
                            };
                            db.Inventories.Add(estoque1);

                        }
                        else
                        {
                            estoque.Stock += detail.Quantity;
                            db.Entry(estoque).State = EntityState.Modified;
                        }

                        db.PurchaseDetailTmps.Remove(detail);
                    }
                    // Verify if new TotalCost and older TotalCost
                    if (purchase.TotalCost > OriginalValue)
                    {
                        difer = purchase.TotalCost.Value - OriginalValue; // Need to generate Payables for this difference
                        CreatePayable(purchase.PurchaseId, "Troca");
                    }
                    db.Entry(purchase).State = EntityState.Modified;
                    db.SaveChanges();
                    transaction.Commit();
                    return new Response { Succeeded = true, };
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return new Response
                    {
                        Message = ex.Message,
                        Succeeded = false,
                    };
                }
            }
        }

        public static Response NewPurchase(NewPurchaseView view, string userName)
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                decimal difer = 0;
                decimal soma = 0;
                decimal valor = 0;
                decimal valortotal = 0;
                int PId;
                var typeDocument = TypeOfDocument.COMP;
                try
                {
                    var user = db.Users.Where(u => u.UserName == userName).FirstOrDefault();

                    var purchase = new Purchase
                    {
                        CompanyId = user.CompanyId,
                        SupplierId = view.SupplierId,
                        Date = view.Date,
                        Remarks = view.Remarks,
                        WarehouseId = view.WarehouseId,
                        ConditionId = view.ConditionId,
                        TotalCost = 0,
                        TotalQuantity = 0,
                    };
                    db.Purchases.Add(purchase);
                    db.SaveChanges();
                    var details = db.PurchaseDetailTmps.Where(pdt => pdt.UserName == userName).ToList();

                    foreach (var detail in details)
                    {
                        var purchaseDetail = new PurchaseDetail
                        {
                            Description = detail.Description,
                            PurchaseId = purchase.PurchaseId,
                            Cost = detail.Cost,
                            ProductId = detail.ProductId,
                            Quantity = detail.Quantity,
                        };
                        valortotal = valortotal + detail.Value;
                        Math.Round(valortotal, 2);

                        db.PurchaseDetails.Add(purchaseDetail);

                        purchase.TotalCost += detail.Value;
                        purchase.TotalQuantity += detail.Quantity;

                        // Aloca Qtde do Produto no Mostruário Destino
                        UpdateInventories(purchase.WarehouseId, "ADD", detail.ProductId, detail.Quantity, false, typeDocument, purchase.PurchaseId);

                        db.PurchaseDetailTmps.Remove(detail);
                    }
                    //
                    // Gerar contas a Pagar
                    //
                    var payable = new Payable
                    {
                        SupplierId = view.SupplierId,
                        Date = view.Date,
                        ConditionId = view.ConditionId,
                        PurchaseId = purchase.PurchaseId,
                        Status = "Em Aberto",
                        Payment = "A Vencer",
                    };
                    db.Payables.Add(payable);
                    db.SaveChanges();

                    PId = payable.PayableId;

                    var query = (from c in db.Conditions
                                 where c.ConditionId == payable.ConditionId
                                 select new { c.Quantity }).Single();
                    int quantity = query.Quantity;
                    var query1 = (from c in db.Conditions
                                  where c.ConditionId == payable.ConditionId
                                  select new { c.Interval }).Single();
                    int interval = query1.Interval;

                    DateTime datavenc;
                    DateTime database = view.Date;

                    for (int i = 0; i < quantity; i++)
                    {
                        datavenc = database.AddDays(interval);
                        var payableDetail = new PayableDetail
                        {
                            PayableId = payable.PayableId,
                            DueDate = datavenc,
                            Value = valortotal / quantity,
                            Balance = valortotal / quantity,
                            ValuePaid = 0,
                        };

                        valor = Math.Round(payableDetail.Value, 2);
                        payableDetail.Value = valor;
                        valor = Math.Round(payableDetail.Balance, 2);
                        payableDetail.Balance = valor;
                        db.PayableDetails.Add(payableDetail);
                        database = datavenc;
                        soma += valor;

                        // Atualiza Fluxo de Caixa
                        var parameter = db.Parameters.FirstOrDefault(p => p.Identity == "CPRO");
                        if (parameter == null)
                        {
                            transaction.Rollback();
                            return new Response
                            {
                                Message = "Parametro CPRO - Provisão de contas a pagar não existe.",
                                Succeeded = false,
                            };
                        }
                        var id = parameter.ParameterId;
                        var accountId = parameter.Value;
                        var account = db.Accounts.Where(a => a.AccountCode == accountId).FirstOrDefault();
                        var description = "Parcela a Pagar";
                        var value = payableDetail.Value;
                        var data = payableDetail.DueDate.Value;
                        var statementtype = TypeOfStatement.Previsto;
                        UpdateCashFlow(id, description, value, statementtype, data, account.AccountId);

                    }

                    db.Entry(purchase).State = EntityState.Modified;

                    db.SaveChanges();

                    transaction.Commit();
                    if (soma != valortotal)
                    {
                        difer = valortotal - soma;
                        var payableDetail = db.PayableDetails.Where(r => r.PayableId == PId).FirstOrDefault();
                        payableDetail.Value = payableDetail.Value + difer;
                        payableDetail.Balance = payableDetail.Balance + difer;
                        db.Entry(payableDetail).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    return new Response { Succeeded = true, };
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return new Response
                    {
                        Message = ex.Message,
                        Succeeded = false,
                    };
                }
            }

        }

        public static Response DeletePurchase(int id, string userName)
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                var purchase = db.Purchases.Find(id);
                var typeDocument = TypeOfDocument.DEVC;
                try
                {
                    var details = db.PurchaseDetails.Where(pd => pd.PurchaseId == purchase.PurchaseId).ToList();
                    foreach (var detail in details)
                    {

                        // Desaloca Qtde do Produto no Mostruário Destino
                        UpdateInventories(purchase.WarehouseId, "SUBTRACT", detail.ProductId, detail.Quantity, false, typeDocument, purchase.PurchaseId);

                        db.Entry(detail).State = EntityState.Deleted;
                    }
                    db.Entry(purchase).State = EntityState.Deleted;
                    db.SaveChanges();
                    transaction.Commit();
                    return new Response { Succeeded = true, };
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return new Response
                    {
                        Message = ex.Message,
                        Succeeded = false,
                    };
                }
            }
        }

        public static Response NewSale(NewSaleView view, string userName, bool api)
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                decimal difer = 0;
                decimal soma = 0;
                decimal valor = 0;
                decimal valortotal = 0;
                int RId;
                var typeDocument = TypeOfDocument.VEND;
                try
                {
                    var user = db.Users.Where(u => u.UserName == userName).FirstOrDefault();
                    var sale = new Sale
                    {
                        CompanyId = user.CompanyId,
                        CustomerId = view.CustomerId,
                        Date = view.Date,
                        Remarks = view.Remarks,
                        WarehouseId = view.WarehouseId,
                        ConditionId = view.ConditionId,
                        SellerId = view.SellerId,
                        OrderId = view.OrderId,
                        Status = "Em Aberto",
                        TotalValue = 0,
                        TotalQuantity = 0,
                    };
                    db.Sales.Add(sale);
                    db.SaveChanges();
                    if (api == false) // A origem é MVC
                    { 
                        var details = db.SalesDetailTmps.Where(sdt => sdt.UserName == userName).ToList();

                        foreach (var detail in details)
                        {
                            var salesDetail = new SalesDetail
                            {
                                Description = detail.Description,
                                SaleId = sale.SaleId,
                                Price = detail.Price,
                                ProductId = detail.ProductId,
                                Quantity = detail.Quantity,                                
                            };
                            valortotal = valortotal + detail.Value;
                            Math.Round(valortotal, 2);

                            db.SalesDetails.Add(salesDetail);

                            sale.TotalValue += detail.Value;
                            sale.TotalQuantity += detail.Quantity;

                            // Desaloca Qtde do Produto do Mostruário
                            UpdateInventories(sale.WarehouseId, "SUBTRACT", detail.ProductId, detail.Quantity, false, typeDocument, sale.SaleId);

                            db.SalesDetailTmps.Remove(detail);
                        }
                    }
                    else // Origem API - Criada em App
                    {
                        var details1 = view.Details;
                        foreach (var detail in details1)
                        {
                            var salesDetail = new SalesDetail
                            {
                                Description = detail.Description,
                                SaleId = sale.SaleId,
                                Price = detail.Price,
                                ProductId = detail.ProductId,
                                Quantity = detail.Quantity,
                            };
                            valortotal = valortotal + detail.Value;
                            Math.Round(valortotal, 2);

                            db.SalesDetails.Add(salesDetail);

                            sale.TotalValue += detail.Value;
                            sale.TotalQuantity += detail.Quantity;

                            // Desaloca Qtde do Produto do Mostruário
                            UpdateInventories(sale.WarehouseId, "SUBTRACT", detail.ProductId, detail.Quantity, false, typeDocument, sale.SaleId);
                        }
                    }
                    db.SaveChanges();
                    //
                    // Gerar contas a Receber
                    //
                    var receivable = new Receivable
                    {
                        CustomerId = view.CustomerId,
                        Date = view.Date,
                        ConditionId = view.ConditionId,
                        SaleId = sale.SaleId,
                        Status = "Em Aberto",
                        Payment = "A Vencer",
                    };
                    db.Receivables.Add(receivable);
                    db.SaveChanges();

                    RId = receivable.ReceivableId;

                    var query = (from c in db.Conditions
                                 where c.ConditionId == receivable.ConditionId
                                 select new { c.Quantity }).Single();
                    int quantity = query.Quantity;
                    var query1 = (from c in db.Conditions
                                  where c.ConditionId == receivable.ConditionId
                                  select new { c.Interval }).Single();
                    int interval = query1.Interval;

                    DateTime datavenc;
                    DateTime database = view.Date;

                    for (int i = 0; i < quantity; i++)
                    {
                        datavenc = database.AddDays(interval);
                        var receivableDetail = new ReceivableDetail
                        {
                            ReceivableId = receivable.ReceivableId,
                            DueDate = datavenc,
                            Value = valortotal / quantity,
                            Balance = valortotal / quantity,
                            ValuePaid = 0,
                        };

                        valor = Math.Round(receivableDetail.Value, 2);
                        receivableDetail.Value = valor;
                        valor = Math.Round(receivableDetail.Balance, 2);
                        receivableDetail.Balance = valor;
                        db.ReceivableDetails.Add(receivableDetail);
                        database = datavenc;
                        soma += valor;

                        // Atualiza Fluxo de Caixa
                        var parameter = db.Parameters.FirstOrDefault(p => p.Identity == "RVDI");
                        var id = parameter.ParameterId;
                        var account = db.Accounts.Where(a => a.AccountCode == parameter.Value).FirstOrDefault();
                        var description = "Parcela a Receber";
                        var value = receivableDetail.Value;
                        var data = receivableDetail.DueDate.Value;
                        var statementtype = TypeOfStatement.Previsto;
                        UpdateCashFlow(id, description, value, statementtype, data, account.AccountId);

                    }

                    db.Entry(sale).State = EntityState.Modified;

                    db.SaveChanges();

                    transaction.Commit();
                    if (soma != valortotal)
                    {
                        difer = valortotal - soma;
                        var receivableDetail = db.ReceivableDetails.Where(r => r.ReceivableId == RId).FirstOrDefault();
                        receivableDetail.Value = receivableDetail.Value + difer;
                        receivableDetail.Balance = receivableDetail.Balance + difer;
                        db.Entry(receivableDetail).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    return new Response { Succeeded = true, };
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return new Response
                    {
                        Message = ex.Message,
                        Succeeded = false,
                    };
                }
            }
        }

        public static void UpdateCashFlow(int id, string description, decimal value, TypeOfStatement statementtype, DateTime data, int accountId)
        {
            var movement = new Movement();

            movement.Data = data;
            movement.Description = description;
            movement.StatementType = statementtype;
            movement.Value = value;
            movement.ParameterId = id;
            movement.AccountId = accountId;

            db.Movements.Add(movement);
            db.SaveChanges();
        }

        public static Response DeleteSale(int id, string userName)
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                var typeDocument = TypeOfDocument.DEVV;
                var sale = db.Sales.Find(id);
                string status;
                int temdados = 0;
                var receivable = db.Receivables.Where(r => r.SaleId == sale.SaleId).FirstOrDefault();
                if (receivable == null)
                {
                    status = "Em Aberto";
                    temdados = 0;
                }
                else
                {
                    var query = (from r in db.Receivables
                                 where r.SaleId == sale.SaleId
                                 select new { r.Status }).Single();
                    status = query.Status;
                    temdados = 1;
                }
                if (status != "Em Aberto")
                {
                    return new Response
                    {
                        Message = "Impossível excluir. Já existe parcela paga.",
                        Succeeded = false,
                    };
                }
                else
                {
                    try
                    {
                        var details = db.SalesDetails.Where(sd => sd.SaleId == sale.SaleId).ToList();
                        foreach (var detail in details)
                        {
                            //var inventory = db.Inventories.Where(i => i.WarehouseId == sale.WarehouseId
                            //    && i.ProductId == detail.ProductId).FirstOrDefault();
                            //inventory.Stock += detail.Quantity;
                            //db.Entry(inventory).State = EntityState.Modified;

                            // Aloca Qtde do Produto no Mostruário
                            UpdateInventories(sale.WarehouseId, "ADD", detail.ProductId, detail.Quantity, false, typeDocument, sale.SaleId);

                            db.Entry(detail).State = EntityState.Deleted;
                        }

                        //
                        //Elimina Contas a Receber
                        //
                        if (temdados == 1)
                        {
                            var receivableDetails = db.ReceivableDetails.Where(rd => rd.ReceivableId == receivable.ReceivableId).ToList();
                            foreach (var detail in receivableDetails)
                            {
                                // Atualiza Fluxo de Caixa
                                var parameter = db.Parameters.FirstOrDefault(p => p.Identity == "CVDI");
                                var id1 = parameter.ParameterId;
                                var accountId = Convert.ToInt32(parameter.Value);
                                var account = db.Accounts.Find(accountId);
                                var description = "Parcela a Receber - excluída";
                                var value = detail.Value;
                                var data = detail.DueDate.Value;
                                var statementtype = TypeOfStatement.Previsto;
                                UpdateCashFlow(id1, description, value, statementtype, data, account.AccountId);

                                db.Entry(detail).State = EntityState.Deleted;
                            }

                            db.Entry(receivable).State = EntityState.Deleted;
                        }
                        db.Entry(sale).State = EntityState.Deleted;
                        db.SaveChanges();
                        transaction.Commit();
                        return new Response { Succeeded = true, };
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        return new Response
                        {
                            Message = ex.Message,
                            Succeeded = false,
                        };
                    }
                }
            }
        }

        public static Response DeleteOrder(int id, string name)
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                var order = db.Orders.Find(id);
                try
                {
                    var details = db.OrderDetails.Where(od => od.OrderId == order.OrderId).ToList();
                    foreach (var detail in details)
                    {
                        db.Entry(detail).State = EntityState.Deleted;
                    }
                    db.Entry(order).State = EntityState.Deleted;
                    db.SaveChanges();
                    transaction.Commit();
                    return new Response { Succeeded = true, };
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return new Response
                    {
                        Message = ex.Message,
                        Succeeded = false,
                    };
                }
            }
        }

        public static Response CreateReceivables(int id)
        { 
            using (var transaction = db.Database.BeginTransaction())
            {
                decimal difer = 0;
                decimal soma = 0;
                decimal valor = 0;
                decimal valortotal = 0;
                string tipocomissao = "CCPR";
                int RId;

                try
                {
                    var consignment = db.Consignments.Find(id);
                    var parameter = db.Parameters.FirstOrDefault(p => p.Identity == tipocomissao);
                    var commission = db.Commissions.Find(Convert.ToInt32(parameter.Value));
                    valortotal = consignment.TotalSaleValue.Value * ( 1 - commission.Percent);
                    //
                    // Gerar contas a Receber
                    //
                    var receivable = new ConsignmentReceivable
                    {
                        SellerId = consignment.SellerId,
                        Date = DateTime.Now,
                        ConditionId = consignment.ConditionId,
                        ConsignmentId = consignment.ConsignmentId,
                        Status = "Em Aberto",
                        Payment = "A Vencer",
                    };
                    db.ConsignmentReceivables.Add(receivable);
                    db.SaveChanges();

                    RId = receivable.ConsignmentReceivableId;

                    var query = (from c in db.Conditions
                                 where c.ConditionId == receivable.ConditionId
                                 select new { c.Quantity }).Single();
                    int quantity = query.Quantity;
                    var query1 = (from c in db.Conditions
                                  where c.ConditionId == receivable.ConditionId
                                  select new { c.Interval }).Single();
                    int interval = query1.Interval;

                    DateTime datavenc;
                    DateTime database = receivable.Date;

                    for (int i = 0; i<quantity; i++)
                    {
                        datavenc = database.AddDays(interval);
                        var receivableDetail = new ConsignmentReceivableDetail
                        {
                            ConsignmentReceivableId = receivable.ConsignmentReceivableId,
                            DueDate = datavenc,
                            Value = valortotal / quantity,
                            Balance = valortotal / quantity,
                            ValuePaid = 0,
                        };

                        valor = Math.Round(receivableDetail.Value, 2);
                        receivableDetail.Value = valor;
                        valor = Math.Round(receivableDetail.Balance, 2);
                        receivableDetail.Balance = valor;
                        db.ConsignmentReceivableDetails.Add(receivableDetail);
                        database = datavenc;
                        soma += valor;

                        // Atualiza Fluxo de Caixa
                        var param = db.Parameters.FirstOrDefault(p => p.Identity == "RVCO");
                        var id1 = param.ParameterId;
                        var account = db.Accounts.Where(a => a.AccountCode == parameter.Value).FirstOrDefault();
                        var description = "Contas a Receber - Consignação";
                        var value = receivableDetail.Value;
                        var data = receivableDetail.DueDate.Value;
                        var statementtype = TypeOfStatement.Previsto;
                        UpdateCashFlow(id1, description, value, statementtype, data, account.AccountId);
                    }

                    consignment.Status = "CR Gerado";
                    db.Entry(consignment).State = EntityState.Modified;

                    db.SaveChanges();

                    transaction.Commit();
                    if (soma != valortotal)
                    {
                        difer = valortotal - soma;
                        var receivableDetail = db.ConsignmentReceivableDetails.Where(r => r.ConsignmentReceivableId == RId).FirstOrDefault();
                        receivableDetail.Value = receivableDetail.Value + difer;
                        receivableDetail.Balance = receivableDetail.Balance + difer;
                        db.Entry(receivableDetail).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    return new Response { Succeeded = true, };
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return new Response
                    {
                        Message = ex.Message,
                        Succeeded = false,
                    };
                }
            }
        }
    }
}