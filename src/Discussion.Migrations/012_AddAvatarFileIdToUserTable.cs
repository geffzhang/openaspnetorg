using FluentMigrator;

namespace Discussion.Migrations
{
    [Migration(12)]
    public class AddAvatarFileIdToUserTable: Migration
    {
        public override void Up()
        {
            Alter.Table(CreateUserTable.TABLE_NAME)
                .AddColumn("AvatarFileId").AsInt32()
                .Nullable();
        }

        public override void Down()
        {
            Delete.Column("AvatarFileId").FromTable(CreateUserTable.TABLE_NAME);
        }
    }
}