using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

namespace SQLCoderConsole;

public class Program
{
    public static async Task Main(string[] args)
    {

        var builder = Kernel.CreateBuilder();
        var modelId = "mannix/defog-llama3-sqlcoder-8b"; //https://github.com/defog-ai/sqlcoder
        var endpoint = new Uri("http://localhost:11434");

        var httpClient = new HttpClient
        {
            Timeout = TimeSpan.FromMinutes(10),
            BaseAddress = new Uri("http://localhost:11434"),
        };

#pragma warning disable SKEXP0070 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
        builder.Services.AddOllamaChatCompletion(modelId, httpClient);
#pragma warning restore SKEXP0070 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

        var kernel = builder.Build();

        var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();

		Console.WriteLine(DateTime.Now);

        string? input = @"### Task
						Generate a SQL query to answer [QUESTION] List down First Name, Last Name, Job Title of all the people from Sales and order the records by sum of the subtotal of thier SalesOrderHeader [/ QUESTION]

						### Instructions
						- If you cannot answer the question with the available database schema, return 'I do not know'
						- A Person from Sales will have their PersonType as 'SP'

						### Database Schema
						This query will run on a database whose schema is represented in this string:
						CREATE TABLE [Person].[Person](
							[BusinessEntityID] [int] NOT NULL, -- Primary Key of the Table and the ID for the Person
							[PersonType] [nchar](2) NOT NULL, 
							[FirstName] [dbo].[Name] NOT NULL, -- First Name of the Person
							[LastName] [dbo].[Name] NOT NULL, -- Last Name of the Person
						) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
						GO
						ALTER TABLE [Person].[Person] ADD  CONSTRAINT [PK_Person_BusinessEntityID] PRIMARY KEY CLUSTERED 
						(
							[BusinessEntityID] ASC
						)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
						GO

						CREATE TABLE [HumanResources].[Employee](
							[BusinessEntityID] [int] NOT NULL, -- This is the primary key of the table which is also the foreign key for the Person.Person Table
							[JobTitle] [nvarchar](50) NOT NULL, -- Job Title of the employee
						) ON [PRIMARY]
						GO
						ALTER TABLE [HumanResources].[Employee] ADD  CONSTRAINT [PK_Employee_BusinessEntityID] PRIMARY KEY CLUSTERED 
						(
							[BusinessEntityID] ASC
						)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
						GO
						ALTER TABLE [HumanResources].[Employee]  WITH CHECK ADD  CONSTRAINT [FK_Employee_Person_BusinessEntityID] FOREIGN KEY([BusinessEntityID])
						REFERENCES [Person].[Person] ([BusinessEntityID])
						GO

						CREATE TABLE [Sales].[SalesPerson](
							[BusinessEntityID] [int] NOT NULL, -- This is the primary key of the table which is also the foreign key for the [HumanResources].[Employee] Table 
							[TerritoryID] [int] NULL,
							[SalesQuota] [money] NULL,
							[Bonus] [money] NOT NULL,
						) ON [PRIMARY]
						GO
						ALTER TABLE [Sales].[SalesPerson] ADD  CONSTRAINT [PK_SalesPerson_BusinessEntityID] PRIMARY KEY CLUSTERED 
						(
							[BusinessEntityID] ASC
						)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
						GO
						ALTER TABLE [Sales].[SalesPerson]  WITH CHECK ADD  CONSTRAINT [FK_SalesPerson_Employee_BusinessEntityID] FOREIGN KEY([BusinessEntityID])
						REFERENCES [HumanResources].[Employee] ([BusinessEntityID])
						GO
						ALTER TABLE [Sales].[SalesPerson] CHECK CONSTRAINT [FK_SalesPerson_Employee_BusinessEntityID]
						GO

						CREATE TABLE [Sales].[SalesOrderHeader](
							[SalesOrderID] [int] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
							[Status] [tinyint] NOT NULL,
							[SalesPersonID] [int] NULL,  -- Foreign key that refers [Sales].[SalesPerson].[BusinessEntityID]
							[SubTotal] [money] NOT NULL,  -- Subtotal of the Sales Order
						) ON [PRIMARY]
						GO
						ALTER TABLE [Sales].[SalesOrderHeader] ADD  CONSTRAINT [PK_SalesOrderHeader_SalesOrderID] PRIMARY KEY CLUSTERED 
						(
							[SalesOrderID] ASC
						)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
						GO
						ALTER TABLE [Sales].[SalesOrderHeader]  WITH CHECK ADD  CONSTRAINT [FK_SalesOrderHeader_SalesPerson_SalesPersonID] FOREIGN KEY([SalesPersonID])
						REFERENCES [Sales].[SalesPerson] ([BusinessEntityID])
						GO
						ALTER TABLE [Sales].[SalesOrderHeader] CHECK CONSTRAINT [FK_SalesOrderHeader_SalesPerson_SalesPersonID]
						GO

						-- [HumanResources].[Employee].[BusinessEntityID] can be joined with [Person].[Person].[BusinessEntityID]
						-- [Sales].[SalesPerson].[BusinessEntityID] can be joined with [HumanResources].[Employee].[BusinessEntityID]
						-- [Sales].[SalesOrderHeader].[SalesPersonID] can be joined with [Sales].[SalesPerson].[BusinessEntityID]

						### Answer
						[SQL]
						";

        try
        {
            ChatMessageContent chatResult = await chatCompletionService.GetChatMessageContentAsync(input, null, kernel);
            Console.Write($"\n>>> Result: {chatResult.ToString()}\n\n> ");
            Console.WriteLine(DateTime.Now);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}\n\n> ");
        }
    }
}

