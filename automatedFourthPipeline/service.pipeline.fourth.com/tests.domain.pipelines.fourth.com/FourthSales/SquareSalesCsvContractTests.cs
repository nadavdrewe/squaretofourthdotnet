using System;
using System.Collections.Generic;
using System.Linq;
using com.fourth.pipeline.pos;
using domain.pipeline.fourth.com.Services.Square;
using NUnit.Framework;
using Shouldly;
using Square;

namespace tests.domain.pipelines.fourth.com.FourthSales
{
    [TestFixture]
    public class SquareSalesCsvContractTests
    {
        [Test]
        public void CreateSalesRows_GeneratesExpectedContract_ForCompletedDailyOrder()
        {
            var closedAt = "2026-04-18T09:15:00Z";

            var generator = new SquareToFourthCSVGenerator("unused")
            {
                _squareBrandSalesDataset = new SquareBrandSalesDataset
                {
                    allEmployees = new[]
                    {
                        new TeamMember
                        {
                            Id = "tm-1",
                            GivenName = "Alex",
                            FamilyName = "Cook"
                        }
                    },
                    entireCatalog = Array.Empty<CatalogObject>(),
                    allCategories = new[]
                    {
                        new CatalogObjectCategory
                        {
                            Id = "cat-1",
                            CategoryData = new CatalogCategory
                            {
                                Name = "Food"
                            }
                        }
                    },
                    allItems = new[]
                    {
                        new CatalogObjectItem
                        {
                            Id = "item-1",
                            ItemData = new CatalogItem
                            {
                                CategoryId = "cat-1"
                            }
                        },
                        new CatalogObjectItem
                        {
                            Id = "item-2",
                            ItemData = new CatalogItem
                            {
                                CategoryId = "cat-1"
                            }
                        }
                    },
                    allProductVariations = new[]
                    {
                        new CatalogObjectItemVariation
                        {
                            Id = "var-1",
                            ItemVariationData = new CatalogItemVariation
                            {
                                ItemId = "item-1",
                                Sku = "BURGER"
                            }
                        },
                        new CatalogObjectItemVariation
                        {
                            Id = "var-2",
                            ItemVariationData = new CatalogItemVariation
                            {
                                ItemId = "item-2",
                                Sku = "FRIES"
                            }
                        }
                    },
                    allModifiers = Array.Empty<CatalogModifierList>()
                }
            };

            var order = new Order
            {
                Id = "order-1",
                LocationId = "loc-1",
                State = OrderState.Completed,
                CreatedAt = "2026-04-17T23:50:00Z",
                ClosedAt = closedAt,
                LineItems = new List<OrderLineItem>
                {
                    new OrderLineItem
                    {
                        Uid = "line-1",
                        CatalogObjectId = "var-1",
                        Name = "Burger",
                        VariationName = "Regular",
                        Quantity = "1",
                        BasePriceMoney = Money(1000),
                        TotalTaxMoney = Money(110),
                        TotalMoney = Money(1010),
                        TotalDiscountMoney = Money(200),
                        Modifiers = new List<OrderLineItemModifier>
                        {
                            new OrderLineItemModifier
                            {
                                Name = "Cheese",
                                BasePriceMoney = Money(100),
                                TotalPriceMoney = Money(100)
                            }
                        },
                        AppliedDiscounts = new List<OrderLineItemAppliedDiscount>
                        {
                            new OrderLineItemAppliedDiscount
                            {
                                DiscountUid = "disc-1"
                            }
                        }
                    },
                    new OrderLineItem
                    {
                        Uid = "line-2",
                        CatalogObjectId = "var-2",
                        Name = "Fries",
                        VariationName = "Large",
                        Quantity = "1",
                        BasePriceMoney = Money(500),
                        TotalTaxMoney = Money(50),
                        TotalMoney = Money(550),
                        TotalDiscountMoney = Money(0)
                    }
                },
                Discounts = new List<OrderLineItemDiscount>
                {
                    new OrderLineItemDiscount
                    {
                        Uid = "disc-1",
                        Name = "Lunch Promo",
                        CatalogObjectId = "discount-1",
                        AppliedMoney = Money(200)
                    }
                },
                ServiceCharges = new List<OrderServiceCharge>
                {
                    new OrderServiceCharge
                    {
                        Uid = "svc-1",
                        Name = "Hospitality Charge",
                        CatalogObjectId = "svc-cat-1",
                        AppliedMoney = Money(50),
                        TotalMoney = Money(50),
                        TotalTaxMoney = Money(0)
                    }
                },
                Tenders = new List<Tender>
                {
                    new Tender
                    {
                        Id = "tender-1",
                        PaymentId = "payment-1",
                        AmountMoney = Money(1660),
                        TipMoney = Money(100),
                        Type = TenderType.Card
                    }
                }
            };

            generator._squareLocationDatasets.Add(new SquareLocationSalesDataset
            {
                Location = new Location
                {
                    Id = "loc-1",
                    Name = "Main Site"
                },
                orders = new[] { order },
                paymentsForOrders = new[]
                {
                    new Payment
                    {
                        Id = "payment-1",
                        OrderId = "order-1",
                        SourceType = "CARD",
                        TeamMemberId = "tm-1",
                        DeviceDetails = new DeviceDetails
                        {
                            DeviceId = "device-1",
                            DeviceName = "Front Till"
                        }
                    }
                },
                refundsForOrders = new[]
                {
                    new PaymentRefund
                    {
                        Id = "refund-1",
                        PaymentId = "payment-1",
                        OrderId = "order-1",
                        AmountMoney = Money(250),
                        Reason = "Contract test refund",
                        Status = "COMPLETED"
                    }
                }
            });

            var rows = generator.CreateSalesRows("UNIT-1").ToList();

            rows.Count(x => x.TransactionTypeCode == TransactionTypeCodes.TAB_OPEN).ShouldBe(1);
            rows.Count(x => x.TransactionTypeCode == TransactionTypeCodes.TAB_CLOSE).ShouldBe(1);
            rows.Count(x => x.TransactionTypeCode == TransactionTypeCodes.SALES_ITEM).ShouldBe(2);
            rows.Count(x => x.TransactionTypeCode == TransactionTypeCodes.MODIFIER_ITEM).ShouldBe(1);
            rows.Count(x => x.TransactionTypeCode == TransactionTypeCodes.DISC_ITEM).ShouldBe(1);
            rows.Count(x => x.TransactionTypeCode == TransactionTypeCodes.SERVICE_CHARGE).ShouldBe(2);
            rows.Count(x => x.TransactionTypeCode == TransactionTypeCodes.TENDER).ShouldBe(2);

            rows.Select(x => x.TradingDate).Distinct().ShouldBe(new[] { "2026-04-18" });

            var tenderRow = rows.Single(x => x.TransactionTypeCode == TransactionTypeCodes.TENDER && x.TenderAmount > 0);
            tenderRow.TenderAmount.ShouldBe(16.60M);
            tenderRow.TerminalCode.ShouldBe("device-1");
            tenderRow.TerminalDesc.ShouldBe("Front Till");
            var refundTenderRow = rows.Single(x => x.TransactionTypeCode == TransactionTypeCodes.TENDER && x.TenderAmount < 0);
            refundTenderRow.TenderAmount.ShouldBe(-2.50M);
            refundTenderRow.TenderTypeCode.ShouldBe("CARD_REFUND");
            refundTenderRow.TenderTypeDesc.ShouldBe("CARD REFUND");

            var serviceChargeRows = rows
                .Where(x => x.TransactionTypeCode == TransactionTypeCodes.SERVICE_CHARGE)
                .OrderBy(x => x.SalesItemDesc)
                .ToList();
            serviceChargeRows.Select(x => x.SalesItemDesc).ShouldBe(new[] { "Hospitality Charge", "Tip" });
            serviceChargeRows.Sum(x => x.PricePaid).ShouldBe(1.50M);

            rows.Single(x => x.TransactionTypeCode == TransactionTypeCodes.TAB_OPEN)
                .TransactionStartEnd.ShouldBe(TransactionStartEndCodes.Start);
            rows.Single(x => x.TransactionTypeCode == TransactionTypeCodes.TAB_CLOSE)
                .TransactionStartEnd.ShouldBe(TransactionStartEndCodes.End);
        }

        private static Money Money(long amount)
        {
            return new Money
            {
                Amount = amount,
                Currency = Currency.Gbp
            };
        }
    }
}
