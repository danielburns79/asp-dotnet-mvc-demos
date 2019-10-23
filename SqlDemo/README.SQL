USE [master]  
GO

USE [Sandbox]  
GO  

SET ANSI_NULLS ON  
GO  
SET QUOTED_IDENTIFIER ON  
GO  

#DROP TABLE [dbo].[Users]
DROP TABLE [dbo].[Classes]
DROP TABLE [dbo].[Enrolement]
DROP TABLE [dbo].[Questions]
DROP TABLE [dbo].[Problems]
DROP TABLE [dbo].[Assignments]
GO

#CREATE TABLE [dbo].[Users](
#    [UserID] [uniqueidentifier] NOT NULL,
#    [Email] [nvarchar](max) NOT NULL,
#    [FirstName] [nvarchar](max) NOT NULL,
#    [LastName] [nvarchar](max) NOT NULL,
#    CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED (  
#        [UserID] ASC  
#    ) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
#) ON [PRIMARY]

#dotnet ef migrations add Initial --context SqlDemo.Models.IdentityDbContext
#dotnet ef database update --context SqlDemo.Models.IdentityDbContext

DECLARE @danielBurnsId uniqueidentifier; SET @danielBurnsId = NEWID()
INSERT [dbo].[IdentityUser] ([Id], [UserName], ,[NormalilzedUserName], [Email], [NormalizedEmail], [EmailConfirmed], ) VALUES (@dnaielBurnsId, 'danielburns79@gmail.com', 'DANIELBURNS79@GMAIL.COM', 'danielburns79@gmail.com', 'DANIELBURNS79@GMAIL.COM', 1)
#INSERT [dbo].[Users] ([UserId], [Email], [FirstName], [LastName]) VALUES (@danielBurnsId, 'danielburns79@gmail.com', 'Dan', 'Burns')
DECLARE @bowieBurnsId uniqueidentifier; SET @bowieBurnsId = NEWID()
#INSERT [dbo].[Users] ([UserId], [Email], [FirstName], [LastName]) VALUES (@bowieBurnsId, 'bowie.burns@dog.com', 'Bowie', 'Burns')
DECLARE @melissaStrandId uniqueidentifier; SET @melissaStrandId = NEWID()
#INSERT [dbo].[Users] ([UserId], [Email], [FirstName], [LastName]) VALUES (@melissaStrandId, 'melissa.strand@gmail.com', 'Melissa', 'Strand')
DECLARE @doogerBurnsId uniqueidentifier; SET @doogerBurnsId = NEWID()
#INSERT [dbo].[Users] ([UserId], [Email], [FirstName], [LastName]) VALUES (@doogerBurnsId, 'dooger.burns@dog.com', 'Dooger', 'Burns')


CREATE TABLE [dbo].[Classes](
    [ClassId] [uniqueidentifier] NOT NULL,
    [ClassName] [nvarchar](max) NOT NULL,
    CONSTRAINT [PK_ClassNames] PRIMARY KEY CLUSTERED (
        [ClassId] ASC
    ) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]


CREATE TABLE [dbo].[Enrolement](
    [ClassId] [uniqueidentifier] NOT NULL,
    [UserId] [uniqueidentifier] NOT NULL,
    [Role] int NOT NULL,
    CONSTRAINT [PK_Classes] UNIQUE (
        [ClassId],
        [UserId]
    )
)


DECLARE @puppyTrainingId uniqueidentifier; SET @puppyTrainingId = NEWID()
INSERT [dbo].[Classes] ([ClassId], [ClassName]) VALUES (@puppyTrainingId, 'Puppy Training 101')
INSERT [dbo].[Enrolement] ([ClassId], [UserId], [Role]) VALUES (@puppyTrainingId, @melissaStrandId, 2)
INSERT [dbo].[Enrolement] ([ClassId], [UserId], [Role]) VALUES (@puppyTrainingId, @bowieBurnsId, 1)
INSERT [dbo].[Enrolement] ([ClassId], [UserId], [Role]) VALUES (@puppyTrainingId, @doogerBurnsId, 1)


CREATE TABLE [dbo].[Questions](
    [QuestionId] [uniqueidentifier] NOT NULL,
    [Question] [nvarchar](max) NOT NULL,
    [DesiredResponse] [nvarchar](max),
    CONSTRAINT [PK_Questions] PRIMARY KEY CLUSTERED (
        [QuestionId] ASC
    ) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]    
) ON [PRIMARY]


DECLARE @question1Id uniqueidentifier; SET @question1Id = NEWID()
INSERT [dbo].[Questions] ([QuestionId], [Question], [DesiredResponse]) VALUES (@question1Id, 'Who is a good boy?', 'Me!')
DECLARE @question2Id uniqueidentifier; SET @question2Id = NEWID()
INSERT [dbo].[Questions] ([QuestionId], [Question], [DesiredResponse]) VALUES (@question2Id, 'Do you want a treat?', 'Yes!')
DECLARE @question3Id uniqueidentifier; SET @question3Id = NEWID()
INSERT [dbo].[Questions] ([QuestionId], [Question], [DesiredResponse]) VALUES (@question3Id, 'Can you speak?', 'Woof.')


CREATE TABLE [dbo].[Problems](
    [ProblemId] [uniqueidentifier] NOT NULL,
    [ClassId] [uniqueidentifier],
    [StudentId] [uniqueidentifier],
    [QuestionId] [uniqueidentifier] NOT NULL,
    [StudentResponse] [nvarchar](max),
    [State] [int] NOT NULL,
    [TeacherResponse] [nvarchar](max),
    CONSTRAINT [PK_Problems] PRIMARY KEY CLUSTERED (
        [ProblemId] ASC
    ) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]    
) ON [PRIMARY]


INSERT [dbo].[Problems] ([ProblemId], [ClassId], [StudentId], [QuestionId], [StudentResponse], [State], [TeacherResponse])
 VALUES ('c692f7b2-182b-45b8-b5d9-1e2e3443830e', @puppyTrainingId, @bowieBurnsId, @question1Id, 'I am.', 1, 'B')
INSERT [dbo].[Problems] ([ProblemId], [ClassId], [StudentId], [QuestionId], [StudentResponse], [State], [TeacherResponse])
 VALUES ('a9bccb17-c127-4b3c-a678-63ea7af994b7', @puppyTrainingId, @bowieBurnsId, @question2Id, 'Yes!', 1, 'A++')
INSERT [dbo].[Problems] ([ProblemId], [ClassId], [StudentId], [QuestionId], [StudentResponse], [State], [TeacherResponse])
 VALUES ('2e1386e9-60e9-4c86-955d-068219d0c903', @puppyTrainingId, @bowieBurnsId, @question3Id, NULL, 1, 'F')
INSERT [dbo].[Problems] ([ProblemId], [ClassId], [StudentId], [QuestionId], [State])
 VALUES ('e78bef26-fa92-4daa-bf2d-cde1efd1d053', @puppyTrainingId, @doogerBurnsId, @question1Id, 0)
INSERT [dbo].[Problems] ([ProblemId], [ClassId], [StudentId], [QuestionId], [State])
 VALUES ('17c0dc81-93ec-4517-a122-c432cca5b1ea', @puppyTrainingId, @doogerBurnsId, @question2Id, 0)
INSERT [dbo].[Problems] ([ProblemId], [ClassId], [StudentId], [QuestionId], [State])
 VALUES ('2189af66-4584-4654-abe5-db2613f8e504', @puppyTrainingId, @doogerBurnsId, @question3Id, 0)


CREATE TABLE [dbo].[Assignments](
    [AssignmentId] [uniqueidentifier] NOT NULL,
    [ProblemId] [uniqueidentifier] NOT NULL,
    CONSTRAINT [PK_Assignments] UNIQUE (
        [AssignmentId],
        [ProblemId]
    )
)


INSERT [dbo].[Assignments] ([AssignmentId], [ProblemId]) VALUES ('904e094a-4135-429b-9814-f9a97b65b931', 'c692f7b2-182b-45b8-b5d9-1e2e3443830e')
INSERT [dbo].[Assignments] ([AssignmentId], [ProblemId]) VALUES ('904e094a-4135-429b-9814-f9a97b65b931', 'a9bccb17-c127-4b3c-a678-63ea7af994b7')
INSERT [dbo].[Assignments] ([AssignmentId], [ProblemId]) VALUES ('904e094a-4135-429b-9814-f9a97b65b931', '2e1386e9-60e9-4c86-955d-068219d0c903')
INSERT [dbo].[Assignments] ([AssignmentId], [ProblemId]) VALUES ('972462fc-e3bf-42c1-8c98-6cf4b51868aa', 'e78bef26-fa92-4daa-bf2d-cde1efd1d053')
INSERT [dbo].[Assignments] ([AssignmentId], [ProblemId]) VALUES ('972462fc-e3bf-42c1-8c98-6cf4b51868aa', '17c0dc81-93ec-4517-a122-c432cca5b1ea')
INSERT [dbo].[Assignments] ([AssignmentId], [ProblemId]) VALUES ('972462fc-e3bf-42c1-8c98-6cf4b51868aa', '2189af66-4584-4654-abe5-db2613f8e504')

###########
useful sql queries
###########

INSERT [dbo].[Users] ([UserId], [Email], [FirstName], [LastName], [Role]) VALUES (NEWID(), 'wagner.burns@dog.com', 'Wagner', 'Burns', 3)

UPDATE [dbo].[Assignments] SET StudentId = 'e9ed8171-cafb-4103-b197-9e0d57a6cb8a' WHERE AssignmentId = '904e094a-4135-429b-9814-f9a97b65b931'
UPDATE [dbo].[Assignments] SET StudentId = 'd9e2f99a-9c37-467c-8a43-d8ad50ed4c8a' WHERE AssignmentId = '972462fc-e3bf-42c1-8c98-6cf4b51868aa'

SELECT C.ClassName, U.FirstName, U.LastName, E.Role, U.UserID
	FROM [dbo].[Classes] C
	JOIN [dbo].[Enrolement] E ON C.ClassId = E.ClassId
	JOIN [dbo].[Users] U ON E.UserId = U.UserId

SELECT Question, DesiredResponse, StudentResponse, TeacherResponse
	FROM [dbo].[Problems] P
	JOIN [dbo].[Questions] Q ON Q.QuestionId = P.QuestionId
	WHERE P.StudentId = '@bowieBurnsId'
    
SELECT FirstName, LastName, Question, DesiredResponse, StudentResponse, TeacherResponse
	FROM [dbo].[Assignments] A
	JOIN [dbo].[Problems] P ON A.ProblemId = P.ProblemId
	JOIN [dbo].[Questions] Q ON P.QuestionId = Q.QuestionId
	JOIN [dbo].[Users] U ON P.StudentId = U.UserId
	WHERE A.AssignmentId = '904e094a-4135-429b-9814-f9a97b65b931'
