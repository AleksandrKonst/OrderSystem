using FluentMigrator;

namespace DatabaseMigrator.Migrations;

[Migration(202505111200)]
public class AddAutoIncrementSequences : Migration
{
    public override void Up()
    {
        // Create sequences for auto-incrementing IDs
        Execute.Sql(@"
            CREATE SEQUENCE IF NOT EXISTS orders_id_seq;
            CREATE SEQUENCE IF NOT EXISTS order_items_id_seq;
        ");

        // Modify Orders table to use the sequence
        Execute.Sql(@"
            ALTER TABLE ""Orders"" ALTER COLUMN ""Id"" SET DEFAULT nextval('orders_id_seq');
            SELECT setval('orders_id_seq', COALESCE((SELECT MAX(""Id"") FROM ""Orders""), 0) + 1);
        ");

        // Modify OrderItems table to use the sequence
        Execute.Sql(@"
            ALTER TABLE ""OrderItems"" ALTER COLUMN ""Id"" SET DEFAULT nextval('order_items_id_seq');
            SELECT setval('order_items_id_seq', COALESCE((SELECT MAX(""Id"") FROM ""OrderItems""), 0) + 1);
        ");
    }

    public override void Down()
    {
        // Remove default values from columns
        Execute.Sql(@"
            ALTER TABLE ""Orders"" ALTER COLUMN ""Id"" DROP DEFAULT;
            ALTER TABLE ""OrderItems"" ALTER COLUMN ""Id"" DROP DEFAULT;
        ");

        // Drop sequences
        Execute.Sql(@"
            DROP SEQUENCE IF EXISTS orders_id_seq;
            DROP SEQUENCE IF EXISTS order_items_id_seq;
        ");
    }
} 