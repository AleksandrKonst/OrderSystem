using FluentMigrator;

namespace DatabaseMigrator.Migrations;

[Migration(202505111100)]
public class CreateInitialTables : Migration
{
    public override void Up()
    {
        Create.Table("Orders")
            .WithColumn("Id").AsInt64().PrimaryKey().NotNullable()
            .WithColumn("CustomerId").AsInt64().NotNullable()
            .WithColumn("Status").AsString(50).NotNullable()
            .WithColumn("TotalAmount").AsDecimal(18, 2).NotNullable()
            .WithColumn("Currency").AsString(3).NotNullable()
            .WithColumn("OriginalAmount").AsDecimal(18, 2).Nullable()
            .WithColumn("OriginalCurrency").AsString(3).Nullable()
            .WithColumn("DiscountId").AsString(50).Nullable()
            .WithColumn("DiscountName").AsString(100).Nullable()
            .WithColumn("DiscountDescription").AsString(500).Nullable()
            .WithColumn("DiscountPercentage").AsDecimal(10, 2).Nullable()
            .WithColumn("DiscountValidUntil").AsDateTime().Nullable()
            .WithColumn("CreatedAt").AsDateTime().NotNullable()
            .WithColumn("UpdatedAt").AsDateTime().Nullable();

        Create.Table("OrderItems")
            .WithColumn("Id").AsInt64().PrimaryKey().NotNullable()
            .WithColumn("OrderId").AsInt64().NotNullable()
            .WithColumn("ProductId").AsString(50).NotNullable()
            .WithColumn("ProductName").AsString(200).NotNullable()
            .WithColumn("Quantity").AsInt32().NotNullable()
            .WithColumn("Price").AsDecimal(18, 2).NotNullable()
            .WithColumn("Currency").AsString(3).NotNullable();

        Create.ForeignKey("FK_OrderItems_Orders")
            .FromTable("OrderItems").ForeignColumn("OrderId")
            .ToTable("Orders").PrimaryColumn("Id");

        Create.Index("IX_OrderItems_OrderId")
            .OnTable("OrderItems")
            .OnColumn("OrderId");

        Create.Index("IX_Orders_CustomerId")
            .OnTable("Orders")
            .OnColumn("CustomerId");

        Create.Index("IX_Orders_Status")
            .OnTable("Orders")
            .OnColumn("Status");
    }

    public override void Down()
    {
        Delete.ForeignKey("FK_OrderItems_Orders").OnTable("OrderItems");
        Delete.Table("OrderItems");
        Delete.Table("Orders");
    }
} 