using FluentMigrator;

namespace DatabaseMigrator.Migrations;

[Migration(202505111700)]
public class RevertToTimestampWithTZ : Migration
{
    public override void Up()
    {
        // Изменение типов колонок с датами в таблице Orders
        Execute.Sql(@"
            -- Изменение типа datetime полей с timestamp without time zone на timestamp with time zone
            ALTER TABLE ""Orders"" 
                ALTER COLUMN ""CreatedAt"" TYPE timestamp with time zone,
                ALTER COLUMN ""UpdatedAt"" TYPE timestamp with time zone,
                ALTER COLUMN ""DiscountValidUntil"" TYPE timestamp with time zone;
        ");
    }

    public override void Down()
    {
        // Возврат к прежнему типу
        Execute.Sql(@"
            ALTER TABLE ""Orders"" 
                ALTER COLUMN ""CreatedAt"" TYPE timestamp without time zone,
                ALTER COLUMN ""UpdatedAt"" TYPE timestamp without time zone,
                ALTER COLUMN ""DiscountValidUntil"" TYPE timestamp without time zone;
        ");
    }
} 