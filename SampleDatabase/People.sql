CREATE TABLE [dbo].[People]
(
    [Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [FullName] NVARCHAR(MAX) NOT NULL, 
    [Email] NVARCHAR(254) NOT NULL, 
    [OptOut] BIT NOT NULL 
)

GO


CREATE INDEX [IX_People_Email] ON [dbo].[People] ([Email])

GO


CREATE INDEX [IX_People_OptOut] ON [dbo].[People] ([OptOut])
