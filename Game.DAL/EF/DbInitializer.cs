using Microsoft.EntityFrameworkCore;
using System;

namespace Game.DAL.EF
{
    public static class DbInitializer
    {
        public static void Initialize(GameSpdbContext context)
        {
            var sql = @"
                IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Notifications' AND xtype='U')
                BEGIN
                    CREATE TABLE Notifications (
                        Id INT IDENTITY(1,1) PRIMARY KEY,
                        Message NVARCHAR(MAX) NOT NULL,
                        CreatedAt DATETIME NOT NULL,
                        TargetRole NVARCHAR(50) NULL,
                        TargetUserId INT NULL
                    );
                END

                IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='NotificationReadStates' AND xtype='U')
                BEGIN
                    CREATE TABLE NotificationReadStates (
                        Id INT IDENTITY(1,1) PRIMARY KEY,
                        UserId INT NOT NULL,
                        NotificationId INT NOT NULL,
                        IsRead BIT NOT NULL DEFAULT 0,
                        CONSTRAINT FK_NotificationReadStates_Users FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE,
                        CONSTRAINT FK_NotificationReadStates_Notifications FOREIGN KEY (NotificationId) REFERENCES Notifications(Id) ON DELETE CASCADE
                    );
                END";

            context.Database.ExecuteSqlRaw(sql);
        }
    }
}
