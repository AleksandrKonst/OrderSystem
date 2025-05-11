using FluentMigrator;

namespace DatabaseMigrator.Migrations;

[Migration(202505111500)]
public class ResetDatabaseWithIdentityColumns : Migration
{
    public override void Up()
    {
        // Drop existing tables if they exist
        Execute.Sql(@"
            DROP TABLE IF EXISTS ""OrderItems"";
            DROP TABLE IF EXISTS ""Orders"";
            DROP SEQUENCE IF EXISTS orders_id_seq;
            DROP SEQUENCE IF EXISTS order_items_id_seq;
        ");

        // Create Orders table with IDENTITY column
        Create.Table("Orders")
            .WithColumn("Id").AsInt64().PrimaryKey().Identity()
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

        // Create OrderItems table with IDENTITY column
        Create.Table("OrderItems")
            .WithColumn("Id").AsInt64().PrimaryKey().Identity()
            .WithColumn("OrderId").AsInt64().NotNullable()
            .WithColumn("ProductId").AsString(50).NotNullable()
            .WithColumn("ProductName").AsString(200).NotNullable()
            .WithColumn("Quantity").AsInt32().NotNullable()
            .WithColumn("Price").AsDecimal(18, 2).NotNullable()
            .WithColumn("Currency").AsString(3).NotNullable();

        // Create foreign key
        Create.ForeignKey("FK_OrderItems_Orders")
            .FromTable("OrderItems").ForeignColumn("OrderId")
            .ToTable("Orders").PrimaryColumn("Id");

        // Create indexes
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