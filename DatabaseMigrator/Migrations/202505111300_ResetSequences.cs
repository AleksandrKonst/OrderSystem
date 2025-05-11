using FluentMigrator;

namespace DatabaseMigrator.Migrations;

[Migration(202505111300)]
public class ResetSequences : Migration
{
    public override void Up()
    {
        // Reset sequences to start from a high number to avoid conflicts
        Execute.Sql(@"
            SELECT setval('orders_id_seq', 1000);
            SELECT setval('order_items_id_seq', 1000);
        ");
    }

    public override void Down()
    {
        // Nothing to revert - can't restore old sequence values
        Execute.Sql("SELECT 1");
    }
} 